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
using System.Xml.Serialization;

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILIndicesBuffer : ILBuffer<int> {

        #region singleton standard buffers
        private static ILIndicesBuffer s_unitCube;
        [XmlIgnore]
        public static ILIndicesBuffer UnitCube {
            get {
                if (s_unitCube == null) {
                    s_unitCube = new ILIndicesBuffer();
                    s_unitCube.Update(new int[] {
                        0,1,1,2,2,3,3,0,
                        4,5,5,6,6,7,7,4,
                        0,4,1,5,2,6,3,7
                    });
                }
                s_unitCube.m_referenceCount++;
                return s_unitCube;
            }
        }
        private static ILIndicesBuffer s_unitCubeLighting;
        [XmlIgnore]
        public static ILIndicesBuffer UnitCubeLighting {
            get {
                if (s_unitCubeLighting == null) {
                    s_unitCubeLighting = new ILIndicesBuffer();
                    s_unitCubeLighting.Update(new int[] {
                        0,9,6,0,6,3,
                        22,20,7,22,7,10,
                        14,17,18,14,18,21,
                        2,5,15,2,15,13,
                        16,4,8,16,8,19,
                        12,23,11,12,11,1,
                    });
                }
                s_unitCubeLighting.m_referenceCount++;
                return s_unitCubeLighting;
            }
        }
        #endregion

        public ILIndicesBuffer() : base(1) {
            BufferType = Drawing.BufferType.IndexBuffer;
        }

        internal int GetIndexAt(int p) {
            lock (Lock) {
                return m_storage.GetValue(p);
            }
        }
        public override ILBuffer<int> Copy() {
            ILIndicesBuffer ret = new ILIndicesBuffer();
            lock (Lock) {
                ret.m_storage.a = m_storage.C;
            }
            return ret;
        }
        public void Update(int startIDX, params int[] indices) {
            if (indices == null || indices.Length > 0)
                base.Update(startIDX, indices.Length, indices); 
        }
        public override void Update(ILInArray<int> data) {
            using (ILScope.Enter(data)) {
                if (!ILMath.isnullorempty(data) && !data.IsVector) {
                    base.Update(ILMath.reshape(data, data.S.NumberOfElements, 1));
                } else {
                    base.Update(data); 
                }
            }
        }
        public static implicit operator ILIndicesBuffer(ILArray<int> A) {
            using (ILScope.Enter(A)) {
                ILIndicesBuffer ret = new ILIndicesBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILIndicesBuffer(ILInArray<int> A) {
            using (ILScope.Enter(A)) {
                ILIndicesBuffer ret = new ILIndicesBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILIndicesBuffer(ILRetArray<int> A) {
            using (ILScope.Enter(A)) {
                ILIndicesBuffer ret = new ILIndicesBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILIndicesBuffer(int[] A) {
            using (ILScope.Enter()) {
                ILIndicesBuffer ret = new ILIndicesBuffer();
                ret.Update(A);
                return ret;
            }
        }


    }
}
