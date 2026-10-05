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
using System.Text;
using System.Runtime.InteropServices;
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Native;
using ILNumerics.Exceptions; 
using System.IO;
using System.Diagnostics; 

namespace ILNumerics {

    /// <summary>
    /// Main math class, exposes all static math functions. Users should write algorithms in a class derived from <c>ILMath</c>. 
    /// </summary>
    public partial class ILMath {
		/// <summary>
		/// Concrete interface wrapper class providing the native LAPACK functions
		/// </summary>
		/// <remarks>The LAPACK wrapper will be initialized the first time, 
		/// a call to any static method of ILMath is made. The decision, which 
		/// native module to load is done by use of native CPUID assembly statements.
		/// If the current processor does not support those calls or is not recognizable
		/// by ILNumerics, a generic - not optimized - version of native LAPACK code will
		/// be used than.</remarks>
        public static IILLapack Lapack;

        /// <summary>
        /// Platform specific FFT implementation, <b>internally</b> used to compute fft 
        /// </summary>
        public static IILFFT FFTImplementation; 
        
        /// <summary>
        /// Main math class providing static builtin functions
        /// </summary>
        [System.Security.SecuritySafeCritical]
        static ILMath() {

            #region find bitrate 
            try {
                string path = Environment.GetEnvironmentVariable("PATH");
                // credit goes to SO: http://stackoverflow.com/questions/864484/getting-the-path-of-the-current-assembly
                // this is better than using .Location! See the comments in the answer at SO.
                // Basically, it ensures the location of the assembly to the one installed to. This is 
                // important for ASP.NET scenarios, where the assembly may is shadow copied to a temp folder.
                string myPath = (new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
                myPath = System.Uri.UnescapeDataString(myPath); // removes %20 for spaces a.t.l
                myPath = Path.GetDirectoryName(myPath); // turns forward slashes into backslashes

                if (!String.IsNullOrWhiteSpace(Settings.NativeDependenciesAbsolutePath)) {
                    myPath = Path.GetDirectoryName(Settings.NativeDependenciesAbsolutePath);
                    System.Diagnostics.Trace.WriteLine("Dependency directory set by configuration: " + myPath);
                } else if (Environment.Is64BitProcess) {
                    myPath = Path.Combine(myPath, "bin64");
                    System.Diagnostics.Trace.WriteLine("64 bit process detected.");
                } else {
                    myPath = Path.Combine(myPath, "bin32");
                    System.Diagnostics.Trace.WriteLine("32 bit process detected.");
                }
                if (!path.Contains(myPath)) {
                    System.Diagnostics.Trace.WriteLine("Adding dependency directory to PATH environment variable: ");
                    System.Diagnostics.Trace.WriteLine(myPath);
                    Environment.SetEnvironmentVariable("PATH", myPath + ";" + path);
                } else {
                    System.Diagnostics.Trace.WriteLine("Dependency directory '" + myPath + "' already contained in environment variable PATH. Skipping PATH modification.");
                }
            } catch (System.Security.SecurityException exc) {
                System.Diagnostics.Trace.Write("Error determining native dependency location /modifying PATH variable: " + exc.ToString());
            }
            #endregion

            #region initialize proc specific interfaces
            // for now we rely on the existance of the Intel MKL! 
            try {
                using (ILScope.Enter()) {
                    ILMKLFFT mkl = new ILMKLFFT();
                    FFTImplementation = mkl;
                    ILLapackMKL10_0 lapack = new ILLapackMKL10_0();
                    Lapack = lapack;
                    fft(array<float>(1.0f,0f,0f,0f));
                }
            } catch (System.Security.SecurityException exc) {
                System.Diagnostics.Trace.Write("Error initializing Lapack implementation: " + exc.ToString());
                // MKL missing ... :| 
                // well, our matmult works blocked and is not a bad substitute, at least 
                Settings.ManagedMultiplyMaxElementSize = int.MaxValue;
            } catch (DllNotFoundException exc) {
                System.Diagnostics.Trace.Write("Error initializing Lapack implementation: " + exc.ToString()); 
                // MKL missing ... :| 
                // well, our matmult works blocked and is not a bad substitute, at least 
                Settings.ManagedMultiplyMaxElementSize = int.MaxValue; 
            }
            #endregion
 
            #region initialize machine parameter infos 
            macharD(ref m_machparDouble.ibeta, ref m_machparDouble.it,ref m_machparDouble.irnd,ref m_machparDouble.ngrd,ref m_machparDouble.machep,ref m_machparDouble.negep,ref m_machparDouble.iexp,ref m_machparDouble.minexp,ref m_machparDouble.maxexp,ref m_machparDouble.eps,ref m_machparDouble.epsneg,ref m_machparDouble.xmin,ref m_machparDouble.xmax); 
            macharF(ref m_machparSingle.ibeta, ref m_machparSingle.it,ref m_machparSingle.irnd,ref m_machparSingle.ngrd,ref m_machparSingle.machep,ref m_machparSingle.negep,ref m_machparSingle.iexp,ref m_machparSingle.minexp,ref m_machparSingle.maxexp,ref m_machparSingle.eps,ref m_machparSingle.epsneg,ref m_machparSingle.xmin,ref m_machparSingle.xmax); 
            #endregion
        }

        #region private helper 
        private static readonly char TRANS_NONE = 'n';

        private static byte saturateByte(double a) {
            if (double.IsNaN(a)) return 0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > byte.MaxValue) return byte.MaxValue;
            if (a < byte.MinValue) return byte.MinValue;
            return (byte)a;
        }
        private static byte saturateByte(int a) {
            if (a > byte.MaxValue) return byte.MaxValue;
            if (a < byte.MinValue) return byte.MinValue;
            return (byte)a;
        }
        private static sbyte saturateSByte(double a) {
            if (double.IsNaN(a)) return 0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > sbyte.MaxValue) return sbyte.MaxValue;
            if (a < sbyte.MinValue) return sbyte.MinValue;
            return (sbyte)a;
        }
        private static char saturateChar(double a) {
            if (double.IsNaN(a)) return (char)0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > char.MaxValue) return char.MaxValue;
            if (a < char.MinValue) return char.MinValue;
            return (char)a;
        }
        private static short saturateInt16(double a) {
            if (double.IsNaN(a)) return 0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > short.MaxValue) return short.MaxValue;
            if (a < short.MinValue) return short.MinValue;
            return (short)a;
        }
        private static int saturateInt32(double a) {
            if (double.IsNaN(a)) return 0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > int.MaxValue) return int.MaxValue;
            if (a < int.MinValue) return int.MinValue;
            return (int)a;
        }
        private static long saturateInt64(double a) {
            if (double.IsNaN(a)) return 0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > long.MaxValue) return long.MaxValue;
            if (a < long.MinValue) return long.MinValue;
            return (long)a;
        }
        private static ushort saturateUInt16(double a) {
            if (double.IsNaN(a)) return 0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > ushort.MaxValue) return ushort.MaxValue;
            if (a < ushort.MinValue) return ushort.MinValue;
            return (ushort)a;
        }
        private static uint saturateUInt32(double a) {
            if (double.IsNaN(a)) return 0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > uint.MaxValue) return uint.MaxValue;
            if (a < uint.MinValue) return uint.MinValue;
            return (uint)a;
        }
        private static ulong saturateUInt64(double a) {
            if (double.IsNaN(a)) return 0;
            a = Math.Round(a, MidpointRounding.AwayFromZero);
            if (a > ulong.MaxValue) return ulong.MaxValue;
            if (a < ulong.MinValue) return ulong.MinValue;
            return (ulong)a;
        }

        #endregion
    }
}
