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

namespace ILNumerics.Algorithms {
    
    public partial class Graphic {

        /// <summary>
        /// Increase the number of triangles by doubling existing triangles
        /// </summary>
        /// <param name="vertices">Vertices</param>
        /// <param name="indices">Triangle index definitions</param>
        /// <param name="iterations">Number of iterations, each iteration will make 4 triangles out of each triangle</param>
        /// <param name="outVertices">[Output] Vertices.</param>
        /// <param name="outTriangles">[Output] Triangles</param>
        /// <remarks><para>Incoming triangles are expected to not be degenerated. This means:
        /// Every edge is used only twice at most. No triangle shares more than two
        /// corners with some other triangle. </para></remarks>
        public static void Triangularize(ILInArray<float> vertices, ILInArray<int> triangles,
                                        int iterations,
                                        ILOutArray<float> outVertices,
                                        ILOutArray<int> outTriangles) {
            using (ILScope.Enter(vertices, triangles)) {
                int numVertices = vertices.Size[1]; 
                ILArray<int> indices = triangles.IsVector ? triangles.Reshape(3,triangles.S.NumberOfElements / 3) : triangles.C;
                int numTriangles = indices.Size[1];
                outVertices.a = vertices.C;
                outTriangles.a = indices.C;
                // being pessimistic: expect to create a larger number of vertices than probable
                outVertices[0, numVertices * Math.Pow(3,iterations)] = 0;
                outTriangles[0, numTriangles * Math.Pow(4,iterations)] = 0;

                int triIndLast = numTriangles;
                int vertIndLast = numVertices;
                for (int it = 0; it < iterations; it++) {
                    int triIndItEnd = triIndLast;

                    for (int triInd = 0; triInd < triIndItEnd; triInd++) {
                        int vertInd0 = outTriangles.GetValue(0, triInd);
                        int vertInd1 = outTriangles.GetValue(1, triInd);
                        int vertInd2 = outTriangles.GetValue(2, triInd);
                        // create new vertices
                        float v0x = (outVertices.GetValue(0, vertInd0) + outVertices.GetValue(0, vertInd1)) / 2f;
                        float v0y = (outVertices.GetValue(1, vertInd0) + outVertices.GetValue(1, vertInd1)) / 2f;
                        float v0z = (outVertices.GetValue(2, vertInd0) + outVertices.GetValue(2, vertInd1)) / 2f;
                        float v1x = (outVertices.GetValue(0, vertInd1) + outVertices.GetValue(0, vertInd2)) / 2f;
                        float v1y = (outVertices.GetValue(1, vertInd1) + outVertices.GetValue(1, vertInd2)) / 2f;
                        float v1z = (outVertices.GetValue(2, vertInd1) + outVertices.GetValue(2, vertInd2)) / 2f;
                        float v2x = (outVertices.GetValue(0, vertInd2) + outVertices.GetValue(0, vertInd0)) / 2f;
                        float v2y = (outVertices.GetValue(1, vertInd2) + outVertices.GetValue(1, vertInd0)) / 2f;
                        float v2z = (outVertices.GetValue(2, vertInd2) + outVertices.GetValue(2, vertInd0)) / 2f;
                        #region new vertex exists already? TODO: This needs to be replaced with a b-tree implementation!!
                        int newVertID0 = -1;
                        int newVertID1 = -1;
                        int newVertID2 = -1;
                        for (int vi = 0; vi < vertIndLast; vi++) {
                            float tmpX = outVertices.GetValue(0, vi);
                            float tmpY = outVertices.GetValue(1, vi);
                            float tmpZ = outVertices.GetValue(2, vi);
                            if (tmpX == v0x && tmpY == v0y && tmpZ == v0z) {
                                newVertID0 = vi;
                            }
                            if (tmpX == v1x && tmpY == v1y && tmpZ == v1z) {
                                newVertID1 = vi;
                            }
                            if (tmpX == v2x && tmpY == v2y && tmpZ == v2z) {
                                newVertID2 = vi;
                            }
                            if (newVertID0 >= 0 && newVertID1 >= 0 && newVertID2 >= 0)
                                break;
                        }
                        #endregion

                        if (newVertID0 < 0) {
                            newVertID0 = vertIndLast++;
                            outVertices[0, newVertID0] = v0x;
                            outVertices[1, newVertID0] = v0y;
                            outVertices[2, newVertID0] = v0z;
                        }
                        if (newVertID1 < 0) {
                            newVertID1 = vertIndLast++;
                            outVertices[0, newVertID1] = v1x;
                            outVertices[1, newVertID1] = v1y;
                            outVertices[2, newVertID1] = v1z;
                        }
                        if (newVertID2 < 0) {
                            newVertID2 = vertIndLast++;
                            outVertices[0, newVertID2] = v2x;
                            outVertices[1, newVertID2] = v2y;
                            outVertices[2, newVertID2] = v2z;
                        }

                        // create new triangles 
                        outTriangles.SetValue(newVertID0, 1, triInd);
                        outTriangles.SetValue(newVertID2, 2, triInd);

                        outTriangles.SetValue(newVertID2, 0, triIndLast);
                        outTriangles.SetValue(newVertID1, 1, triIndLast);
                        outTriangles.SetValue(vertInd2, 2, triIndLast++);

                        outTriangles.SetValue(newVertID2, 0, triIndLast);
                        outTriangles.SetValue(newVertID0, 1, triIndLast);
                        outTriangles.SetValue(newVertID1, 2, triIndLast++);

                        outTriangles.SetValue(newVertID0, 0, triIndLast);
                        outTriangles.SetValue(vertInd1, 1, triIndLast);
                        outTriangles.SetValue(newVertID1, 2, triIndLast++);
                    }
                    outVertices.a = outVertices / ILMath.sqrt(ILMath.sum(outVertices * outVertices)); 
                }
                outVertices.a = outVertices[":;0:" + (vertIndLast - 1)];
                outTriangles.a = outTriangles[":;0:" + (triIndLast - 1)];
            }
        }
    }
}
