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
    public class ILTrianglesFan : ILTriangles {

        #region constructors
        public ILTrianglesFan(object tag = null)
            : base(tag) {
            Type = Primitives.TriangleFan;
        }
        internal ILTrianglesFan(ILTrianglesFan source)
            : base(source) {
            Type = Primitives.TriangleFan;
        }
        #endregion

        #region public interface
        //public override IEnumerator<Triangle> GetEnumerator() {
        //    if (Indices.IsEmpty) {
        //        if (Positions.DataCount < 3) yield break;
        //        Vector3 p1 = Positions.GetPositionAt(0);
        //        Vector3 p2 = Positions.GetPositionAt(1);
        //        Color c1 = Color.Empty, c2 = Color.Empty;
        //        if (Colors.DataCount > 1) {
        //            c1 = Colors.GetColorAt(0);
        //            c2 = Colors.GetColorAt(1);
        //        }
        //        Vector3 n1 = Vector3.Empty, n2 = Vector3.Empty;
        //        if (Normals.DataCount > 1) {
        //            n1 = Normals.GetNormalAt(0);
        //            n2 = Normals.GetNormalAt(1);
        //        }

        //        for (int i = 2; i < Positions.DataCount; i++) {
        //            Triangle ret = new Triangle();
        //            ret.P1 = p1; ret.P2 = p2;
        //            ret.P3 = Positions.GetPositionAt(i);

        //            if (Colors.DataCount > i) {
        //                ret.C1 = c1;
        //                ret.C2 = c2;
        //                ret.C3 = Colors.GetColorAt(i);
        //            } else {
        //                ret.C1 = Color;
        //                ret.C2 = Color;
        //                ret.C3 = Color;
        //            }
        //            if (Normals.DataCount > i) {
        //                ret.N1 = n1;
        //                ret.N2 = n2;
        //                ret.N3 = Normals.GetNormalAt(i);
        //            }
        //            yield return ret;
        //            p2 = ret.P3;
        //            c2 = ret.C3;
        //            n2 = ret.N3;
        //        }
        //    } else {
        //        if (Indices.DataCount < 3) yield break;
        //        int i1, i2, i3;
        //        i1 = Indices.GetIndexAt(0);
        //        i2 = Indices.GetIndexAt(1);
        //        Vector3 p1 = Positions.GetPositionAt(i1);
        //        Vector3 p2 = Positions.GetPositionAt(i2);
        //        Color c1 = Color.Empty, c2 = Color.Empty;
        //        if (Colors.DataCount > i2) {
        //            c1 = Colors.GetColorAt(i1);
        //            c2 = Colors.GetColorAt(i2);
        //        }
        //        Vector3 n1 = Vector3.Empty, n2 = Vector3.Empty;
        //        if (Normals.DataCount > i2) {
        //            n1 = Normals.GetNormalAt(i1);
        //            n2 = Normals.GetNormalAt(i2);
        //        }

        //        for (int i = 2; i < Indices.DataCount; i++) {
        //            Triangle ret = new Triangle();
        //            i3 = Indices.GetIndexAt(i);
        //            ret.P1 = p1; ret.P2 = p2;
        //            ret.P3 = Positions.GetPositionAt(i3);

        //            if (Colors.DataCount > i3) {
        //                ret.C1 = c1;
        //                ret.C2 = c2;
        //                ret.C3 = Colors.GetColorAt(i3);
        //            } else {
        //                ret.C1 = Color;
        //                ret.C2 = Color;
        //                ret.C3 = Color;
        //            }
        //            if (Normals.DataCount > i3) {
        //                ret.N1 = n1;
        //                ret.N2 = n2;
        //                ret.N3 = Normals.GetNormalAt(i3);
        //            }
        //            yield return ret;
        //            p2 = ret.P3;
        //            c2 = ret.C3;
        //            n2 = ret.N3;
        //        }
        //    }
        //}

        internal override ILRetArray<int> GetIndicesForSorting(ILInArray<int> sourceIndices) {
            return Computation.GetIndicesForSorting(sourceIndices, Positions.DataCount); 
        }
        internal override ILNode Copy() {
            return new ILTrianglesFan(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILTrianglesFan();
        }
        public override void AutoComputeNormals() {
            Computation.ComputeNormals(Positions, Indices, Normals);
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
                    if (!isnullorempty(Indices)) {
                        // indices specified
                        a0 = Positions[full, Indices[0]];
                        a1 = Positions[full, Indices[r(1, end-1)]];
                        a2 = Positions[full, Indices[r(2, end)]];
                        temp = cross(a2 - a0, a1 - a0, normalize: false);
                        autoNormals[full, Indices[0]] = mean(temp, 1);
                        autoNormals[full, Indices[r(2, end - 1)]] = (temp[full, r(1, end)] + temp[full, r(0, end - 1)]) / 2f;
                        // check for connected ends/ close loop - nonindexed only! 
                        if (Indices[end].Equals(Indices[1])) {
                            autoNormals[full, Indices[1]] = (temp[full, 0] + temp[full, end]) / 2f;
                        } else {
                            autoNormals[full, Indices[1]] = temp[full, 0];
                            autoNormals[full, Indices[end]] = temp[full, end];
                        }
                    } else {
                        // no indices specified
                        a0 = Positions[full, 0];
                        a1 = Positions[full, r(1,end-1)];
                        a2 = Positions[full, r(2,end-0)];
                        temp = cross(a2 - a0, a1 - a0, normalize: false);
                        autoNormals[full, 0] = mean(temp, 1);
                        autoNormals[full, r(2, end - 1)] = (temp[full, r(1, end)] + temp[full, r(0, end - 1)]) / 2f;
                        autoNormals[full, 1] = temp[full, 0];
                        autoNormals[full, end] = temp[full, end];
                        // check for connected ends/ close loop - nonindexed only! 
                        if (Positions[full, 1].Equals(Positions[full, end])) {
                            autoNormals[full, 1] = (autoNormals[full, 1] + autoNormals[full, end]) / 2f;
                            autoNormals[full, end] = autoNormals[full, 1];
                        }
                    }
                    // normalize and update 
                    autoNormals.a = autoNormals / sqrt(sum(autoNormals * autoNormals, 0)); 
                    normals.Update(0, positions.DataCount, autoNormals);
                }
            }
            public static ILRetArray<int> GetIndicesForSorting(ILInArray<int> sourceIndices, int dataCount ) {
                using (ILScope.Enter(sourceIndices)) {
                    if (dataCount < 3) {
                        return empty<int>(); 
                    }
                    ILArray<int> indices = sourceIndices;
                    if (isnull(indices) || indices.IsEmpty) {
                        // create standard indices for triangle fan
                        indices.a = counter<int>(0, 1, size(1, dataCount));
                    }
                    ILArray<int> ret = zeros<int>(3, indices.S[1] - 2);
                    ret[0, full] = indices[0];
                    ret[1, full] = indices[r(1, end - 1)];
                    ret[2, full] = indices[r(2, end - 0)];
                    return ret;
                }
            }

        }
        #endregion
    }
}
