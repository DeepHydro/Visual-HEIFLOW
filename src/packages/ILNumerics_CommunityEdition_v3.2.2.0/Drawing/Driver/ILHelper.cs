///
///    This file is part of ILNumerics Community Edition.
///
///    ILNumerics Community Edition - high performance computing for applications.
///    Copyright (C) 2006 - 2013 Haymo Kutschbach, http://ilnumerics.net
///
///    ILNumerics Community Edition is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License version 3 as published by
///    the Free Software Foundation.
///
///    ILNumerics Community Edition is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with ILNumerics Community Edition. See the file License.txt in the root
///    of your distribution package. If not, see <http://www.gnu.org/licenses/>.
///
///    In addition this software uses the following components and/or licenses: 
///
///    =================================================================================
///    The Open Toolkit Library License
///    
///    Copyright (c) 2006 - 2009 the Open Toolkit library.
///    
///    Permission is hereby granted, free of charge, to any person obtaining a copy
///    of this software and associated documentation files (the "Software"), to deal
///    in the Software without restriction, including without limitation the rights to 
///    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
///    the Software, and to permit persons to whom the Software is furnished to do
///    so, subject to the following conditions:
///
///    The above copyright notice and this permission notice shall be included in all
///    copies or substantial portions of the Software.
///
///    =================================================================================
///    Intel® Math Kernel Library 11.1 for Windows
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Intel® Math Kernel Library 10.3 for Linux
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Products / Software which is implicitly used by ILNumerics due to the inclusion 
///    of 3rd party components: 
///  
///        BLAS/ LAPACK; see: http://netlib.org
///        FFT Functions; see: http://www.spiral.net, http://fftw.org
///        OpenGL; see: http://opengl.org
///
///    =================================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace ILNumerics.Drawing {
    [System.Security.SecuritySafeCritical]
    internal class ILHelper : ILMath {

        static float Max05Float = 8388607f;
        static float Max10Float = 268435440;
        static float Max100KFloat = 2199023124480;

        internal static bool? s_isDesignMode;  
        /// <summary>
        /// ensures a non-zero range between min .. max, according to a and floating point options
        /// </summary>
        /// <param name="a">base number</param>
        /// <param name="min">min range</param>
        /// <param name="max">max range</param>
        public static void EnsurePlotCubeExtend(float a, out float min, out float max) {
            float absOff = Math.Abs(a);
            if (absOff < Max05Float) {
                min = a - 0.5f;
                max = a + 0.5f;
            } else if (absOff < Max10Float) {
                min = a - 10f; 
                max = a + 10f;
            } else if (absOff < Max100KFloat) {
                min = a - 100000f;
                max = a + 100000f;
            } else if (float.IsNaN(a)) {
                min = float.NaN;
                max = float.NaN; 
            } else {
                // too large to be near exact
                if (a < 0) {
                    min = a; max = -Max100KFloat;
                } else {
                    max = a; min = Max100KFloat; 
                }
            }
        }
        /// <summary>
        /// sort the shape elements for transparent rendering
        /// </summary>
        /// <param name="shape">shape</param>
        /// <param name="transform">model to clip transform</param>
        public static ILRetArray<int> SortIndices(ILShape shape, Matrix4 transform) {
            using (ILScope.Enter()) {
                // get individual indices so we can sort the primitives individually.
                // note: optimized shapes (strips/ fans) do not work here and get 
                // 'converted' to regular shapes (ie. triangles/ lines).
                ILArray<int> indices = shape.GetIndicesForSorting(shape.Indices.Storage); 
                // transform the positions into clipspace
                ILArray<float> positions = transform * shape.Positions.Storage; 
                // sort primitives according to z-coordinates
                ILArray<int> outIndices = empty<int>();
                sort(sum(positions[2, full][indices], 0), outIndices, descending: false).Dispose();

                return indices[full, outIndices];
            }
        }
        public static Vector4 ComputeNewVertex(Vector4 noClip, Vector4 clip, Vector4 K, out float t) {
            t = Vector4.Dot(K, noClip) / Vector4.Dot(K, noClip - clip);
            Vector4 ret = (noClip + t * (clip - noClip));
            System.Diagnostics.Debug.Assert(Math.Abs(Vector4.Dot(ret, K)) < 0.001f);
            return ret;
        }
        public static bool IsDesignMode() {
           try {
               if (!Settings.IsHosted) {
                   if (s_isDesignMode.HasValue) {
                       return s_isDesignMode.GetValueOrDefault(); 
                   }
                   // this code was adopted from the idea of Abel Braaksma: 
                   // http://www.undermyhat.org/blog/2009/07/in-depth-a-definitive-guide-to-net-user-controls-usage-mode-designmode-or-usermode/
                   // 
                   System.Diagnostics.Trace.WriteLine("");
                   System.Diagnostics.Trace.WriteLine("Determining Design Mode...");
                   var currentTypeAssembly = Assembly.GetAssembly(typeof(ILDriver)).GetName().FullName;
                   Assembly entryAssembly = Assembly.GetEntryAssembly();
                   List<string> names = new List<string>();
                   System.Diagnostics.Trace.WriteLine(" Entry Assembly: " + ((entryAssembly != null) ? entryAssembly.FullName : "(null)"));
                   System.Diagnostics.Trace.WriteLine(" CurrentTypeAssembly: " + currentTypeAssembly);
                   System.Diagnostics.Trace.WriteLine(" Loaded Assemblies: ");
                   if (entryAssembly != null && entryAssembly.GetName().FullName == currentTypeAssembly) {
                       System.Diagnostics.Trace.WriteLine("TypeAssembly is EntryAssembly");
                       System.Diagnostics.Trace.WriteLine("Design Mode: False");
                       s_isDesignMode = false; 
                       return false;
                   }
                   GetRecursiveReferencedAssemblyNames(entryAssembly, names);
                   System.Diagnostics.Trace.WriteLine(String.Join(Environment.NewLine, names));
                   var result = from asmName in names
                                where asmName == currentTypeAssembly
                                select asmName;

                   s_isDesignMode = result.Count() == 0;
                   System.Diagnostics.Trace.WriteLine("Design Mode: " + (s_isDesignMode.GetValueOrDefault() ? "True" : "False"));

                   return s_isDesignMode.GetValueOrDefault(); 
               }
               // user specified "hosted" per settings var -> never assume design mode at runtime 
               return false; 
            } catch (SecurityException exc) {
                System.Diagnostics.Trace.WriteLine("Could not determine, if ILNumerics is run in Design mode due to insufficient permissions. Assuming 'false' (not in design mode)." + Environment.NewLine 
                    + "Exception thrown: " + exc.ToString());
                return false; 
            }

        }
        public static Vector3 ComputeNewVertex(Vector3 noClip, Vector3 clip, Vector4 K, out float t) {
            t = Vector4.Dot(K, noClip) / Vector4.Dot(K, new Vector4(noClip - clip,0));
            Vector3 ret = (noClip + t * (clip - noClip));
            System.Diagnostics.Debug.Assert(Math.Abs(Vector4.Dot(ret, K)) < 0.001f);
            return ret;
        }
        internal static void GetRecursiveReferencedAssemblyNames(Assembly assembly, List<string> names) {
            if (object.Equals(assembly, null)) return;
            foreach (var assName in assembly.GetReferencedAssemblies()) {
                if (!names.Contains(assName.FullName)) {
                    names.Add(assName.FullName);
                    try {
                        GetRecursiveReferencedAssemblyNames(Assembly.Load(assName.FullName), names);
                    } catch (Exception) { } // for mono ? 
                }
            }
        }

        internal static bool IsVSTO(ILPanel iLPanel) {
            if (iLPanel != null && iLPanel.Parent != null) {
                var parent = iLPanel.Parent;
                return parent.GetType().Name.Contains("VSTO");
            }
            return false; 
        }
    }
}
