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

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILTrianglesStrip : ILTriangles {

        #region constructors
        public ILTrianglesStrip(object tag = null)
            : base(tag) {
            Type = Primitives.TriangleStrip;
        }
        internal ILTrianglesStrip(ILTrianglesStrip source)
            : base(source) {
            Type = Primitives.TriangleStrip;
        }
        #endregion

        #region public interface

        internal override ILRetArray<int> GetIndicesForSorting(ILInArray<int> sourceIndices) {
            return Computation.GetIndicesForSorting(sourceIndices, Positions.DataCount);  
        }
        public override void AutoComputeNormals() {
            Computation.ComputeNormals(Positions, Indices, Normals);
        }
        internal override ILNode Copy() {
            return new ILTrianglesStrip(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILTrianglesStrip();
        }
        public override int GetPrimitiveCount() {
            int num;
            if (Indices != null && !Indices.IsEmpty) {
                num = Indices.Storage.S.NumberOfElements;
            } else {
                num = Buffers.Positions.DataCount;
            }
            return num - 2;
        }

        #endregion

        #region private helper 
        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {

            public static void ComputeNormals(ILPositionsBuffer positions, ILIndicesBuffer indices,
                                              ILNormalsBuffer normals) {
                using (ILScope.Enter()) {
                    // calc normals for all individual shapes 
                    ILArray<float> Positions = positions.Storage;
                    ILArray<int> Indices = indices.Storage;
                    ILArray<float> temp, a0, a1, a2;
                    ILArray<float> autoNormals = array<float>(Positions.S);
                    if (!isnullorempty(Indices) && Indices.Length > 2) {
                        // indices specified
                        a0 = Positions[full, Indices[r(0, end - 2)]];
                        a1 = Positions[full, Indices[r(1, end - 1)]];
                        a2 = Positions[full, Indices[r(2, end)]];
                        temp = cross(a1 - a0, a1 - a2, normalize: false);
                        // invert each 2nd normal, starting with the 2rd (index 1)
                        temp[full,r(1,2,end)] = temp[full,r(1,2,end)] * -1f; 
                        autoNormals[full, Indices[r(2, end - 2)]] = (temp[full, r(0, end - 2)] + temp[full, r(1, end - 1)] + temp[full, r(2, end)]) / 3f;
                        #region checks for cyclic ends 
                        if (Indices[0].Equals(Indices[end - 1]) && Indices[1].Equals(Indices[end])) {
                            autoNormals[full, Indices[0]] = (temp[full, 0] + temp[full,end - 1] + temp[full,end]) / 3;
                            autoNormals[full, Indices[1]] = (temp[full, 0] + temp[full, 1] + temp[full, end]) / 3;
                        } else {
                            autoNormals[full, Indices[0]] = temp[full, 0];
                            autoNormals[full, Indices[end - 1]] = (temp[full, end - 1] + temp[full, end]) / 2;
                            autoNormals[full, Indices[1]] = (temp[full, 0] + temp[full, 1]) / 2;
                            autoNormals[full, Indices[end]] = temp[full, end];
                        }
                        #endregion
                    } else {
                        // no indices specified
                        a0 = Positions[full, r(0, end - 2)];
                        a1 = Positions[full, r(1, end - 1)];
                        a2 = Positions[full, r(2, end - 0)];
                        temp = cross(a1 - a0, a1 - a2, normalize: false);
                        // invert each 2nd normal, starting with the 2rd (index 1)
                        temp[full,r(1,2,end)] = temp[full,r(1,2,end)] * -1f; 
                        autoNormals[full, r(2, end - 2)] = (temp[full, r(0, end - 2)] + temp[full, r(1, end - 1)] + temp[full, r(2, end)]) / 3f;
                        #region test for cyclic definitions
                        if (a0[full,0].Equals(a1[full,end])) {
                            // connected ends 
                            autoNormals[full,0] = (temp[full,0] + temp[full,end-1] + temp[full,end]) / 3; 
                            autoNormals[full,end-1] = autoNormals[full,0];  
                        } else {
                            autoNormals[full,0] = temp[full,0];
                            autoNormals[full,end-1] = (temp[full,end-1] + temp[full,end]) / 2; 
                        }
                        if (a1[full,0].Equals(a2[full,end])) {
                            // connected ends 
                            autoNormals[full,1] = (temp[full,0] + temp[full,1] + temp[full,end]) / 3; 
                            autoNormals[full,end] = autoNormals[full,1];  
                        } else {
                            autoNormals[full,1] = (temp[full,0] + temp[full,1]) / 2;  
                            autoNormals[full,end] = temp[full,end]; 
                        }
                        #endregion 
                    }
                    
                    // normalize and update 
                    //autoNormals.a = autoNormals / sqrt(sum(autoNormals * autoNormals, 0));
                    normals.Update(0, positions.DataCount, autoNormals);
                }
            }
            public static ILRetArray<int> GetIndicesForSorting(ILInArray<int> sourceIndices, int dataCount) {
                using (ILScope.Enter()) {
                    ILArray<int> indices = sourceIndices;
                    if (isnull(indices) || indices.IsEmpty) {
                        indices.a = counter<int>(0, 1, size(1, dataCount));
                    }
                    ILArray<int> ret = zeros<int>(3, indices.S[1] - 2);
                    ret[0, full] = indices[r(0, end - 2)];
                    ret[1, full] = indices[r(1, end - 1)];
                    ret[2, full] = indices[r(2, end - 0)];
                    return ret;
                }
            }

        }

        #endregion 
    }

}
