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
using System.Drawing;
using System.Linq;
using System.Text;

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILSphere : ILGroup {

        #region attributes
        public static readonly string DefaultFillTag = "Fill";
        public static readonly string DefaultWireframeTag = "Wireframe"; 
        public static readonly string DefaultSphereTag = "Sphere"; 

        #endregion

        #region properties
        public ILTriangles Fill { get { return First<ILTriangles>(DefaultFillTag); } }
        public ILLines Wireframe { get { return First<ILLines>(DefaultWireframeTag); } }
        #endregion

        internal ILSphere(ILSphere source) : base (source) { }
        private ILSphere() : base() { }

        public ILSphere(object tag = null, int resolution = 4) : base(tag ?? DefaultSphereTag) {
            using (ILScope.Enter()) {
                ILArray<int> indices = 1; 
                ILArray<float> vertices = Computation.GetVertices(resolution,indices);

                Add(new ILTriangles(DefaultFillTag));
                Add(new ILLines(DefaultWireframeTag));
                Fill.Positions = Wireframe.Positions;
                Fill.Positions.Update(vertices);
                Fill.Indices.Update(indices);
                ILArray<int> wireInd = indices.Reshape(3,indices.Length / 3); 
                wireInd.a = wireInd["0,1,1,2,2,0;:"]; 

                Wireframe.Indices.Update(wireInd);
                Fill.Color = Color.Green;
                Wireframe.Color = Color.DarkGreen;
                Fill.Colors = new ILColorsBuffer(); 
                Wireframe.Colors = new ILColorsBuffer(); 
            }
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILSphere(); 
        }
        internal override ILNode Copy() {
            return new ILSphere(this);
        }

        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {
            public static ILRetArray<float> GetVertices(int resolution, ILOutArray<int> indices) {
                using (ILScope.Enter()) {
                    // start with octahedron
                    ILArray<float> ret = new float[,]{
                        {-1, 0, 0},
                        { 0, 1, 0},
                        { 1, 0, 0},
                        { 0,-1, 0},
                        { 0, 0, 1},
                        { 0, 0, -1}
                    };
                    if (!isnull(indices)) {
                        indices.a = new int[] {
                            0,1,4,
                            1,2,4,
                            2,3,4,
                            3,0,4,
                            1,0,5,
                            2,1,5,
                            3,2,5,
                            0,3,5
                        };
                    }
                    ILArray<float> vertices = 1; 
                    ILArray<int> indicesRet = 1;
                    Algorithms.Graphic.Triangularize(ret, indices, resolution, vertices, indicesRet);
                    indices.a = indicesRet[full];
                    vertices.a = vertices / sqrt(sum(vertices * vertices)); 

                    return vertices; 
                }
            }
        }
    }
}
