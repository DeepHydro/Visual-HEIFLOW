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
using System.Text;
using System.Drawing; 
using OpenTK;
using System.Runtime.InteropServices; 

namespace ILNumerics.Drawing {
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Triangle {
        [FieldOffset(0)]
        public Vector3 P1;
        [FieldOffset(12)]
        public Vector3 P2;
        [FieldOffset(24)]
        public Vector3 P3;
        
        // camera coords may have 4 components, since we allow non-affine transforms (needed f.e. for PlotCube.DataScreenRect) 
        [FieldOffset(36)]
        public Vector4 PCam1;
        [FieldOffset(52)]
        public Vector4 PCam2;
        [FieldOffset(68)]
        public Vector4 PCam3;

        [FieldOffset(84)]
        public Vector4 C1;
        [FieldOffset(100)]
        public Vector4 C2;
        [FieldOffset(116)]
        public Vector4 C3;

        [FieldOffset(132)]
        public Vector3 N1;
        [FieldOffset(144)]
        public Vector3 N2;
        [FieldOffset(156)]
        public Vector3 N3;

        /// <summary>
        /// internal flag storing, which clip planes have been checked for the triangle already 
        /// </summary>
        [FieldOffset(168)]
        internal int ClipPlaneChecked;
        [FieldOffset(172)]
        private int padding; 
        [FieldOffset(176)]
        public ILDashInfo Pattern;

        //#region IILPrimitive Members

        //public IILPrimitive CopyTo(ref IILPrimitive primitve) {
        //    return this; 
        //}

        //public Vector4 GetPlane() {
        //    Vector3 a = Vector3.CrossN(P2 - P1, P3 - P1);
        //    return new Vector4(a, Vector3.Dot(a, P1));
        //}

        //public void SplitByPlane(ref Vector4 plane, ref IList<IILPrimitive> fronts, ref IList<IILPrimitive> backs, IList<IILPrimitive> copla) {
        //    throw new NotImplementedException();
        //}

        //#endregion

        internal void Subdivide(out Triangle tr1, out Triangle tr2, out Triangle tr3, out Triangle tr4) {
            tr1 = new Triangle() {
                P1 = this.P1, P2 = (this.P1 + this.P2) / 2, P3 = (this.P1 + this.P3) / 2,
                PCam1 = this.PCam1, PCam2 = (this.PCam1 + this.PCam2) / 2, PCam3 = (this.PCam1 + this.PCam3) / 2,
                C1 = this.C1, C2 = (this.C1 + this.C2) / 2, C3 = (this.C1 + this.C3) / 2,
                N1 = this.N1, N2 = (this.N1 + this.N2) / 2, N3 = (this.N1 + this.N3) / 2
            };
            tr2 = new Triangle() {
                P1 = (this.P1 + this.P3) / 2, P2 = (this.P1 + this.P2) / 2, P3 = (this.P2 + this.P3) / 2,
                PCam1 = (this.PCam1 + this.PCam3) / 2, PCam2 = (this.PCam1 + this.PCam2) / 2, PCam3 = (this.PCam2 + this.PCam3) / 2,
                C1 = (this.C1 + this.C3) / 2, C2 = (this.C1 + this.C2) / 2, C3 = (this.C2 + this.C3) / 2,
                N1 = (this.N1 + this.N3) / 2, N2 = (this.N1 + this.N2) / 2, N3 = (this.N2 + this.N3) / 2,
            };
            tr3 = new Triangle() {
                P1 = (this.P1 + this.P2) / 2, P2 = P2, P3 = (this.P2 + this.P3) / 2,
                PCam1 = (this.PCam1 + this.PCam2) / 2, PCam2 = this.PCam2, PCam3 = (this.PCam2 + this.PCam3) / 2,
                C1 = (this.C1 + this.C2) / 2, C2 = this.C2, C3 = (this.C2 + this.C3) / 2,
                N1 = (this.N1 + this.N2) / 2, N2 = this.N2, N3 = (this.N2 + this.N3) / 2,
            }; 
            tr4 = new Triangle() {  
                P1 = (this.P1 + this.P3) / 2, P2 = (this.P2 + this.P3) / 2, P3 = this.P3,
                PCam1 = (this.PCam1 + this.PCam3) / 2, PCam2 = (this.PCam2 + this.PCam3) / 2, PCam3 = this.PCam3,
                C1 = (this.C1 + this.C3) / 2, C2 = (this.C2 + this.C3) / 2, C3 = this.C3,
                N1 = (this.N1 + this.N3) / 2, N2 = (this.N2 + this.N3) / 2, N3 = this.N3,
            }; 
        }
        public override string ToString() {
            return String.Format("P1:{0} P2:{1} P3:{2}", P1, P2, P3);
        }
    }
}
