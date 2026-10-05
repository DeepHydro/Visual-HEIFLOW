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
using ILNumerics.Exceptions;

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILTriangles : ILShape {

        #region attributes
        protected Dictionary<int, List<int>> m_shapeIndicesIndex = null; // used for speedy normal calculation

        #endregion

        #region properties
        public override int VerticesPerPrimitive {
            get { return 3; }
        }
        #endregion

        #region constructors
        public ILTriangles(object tag = null)
            : base(tag) {
            Type = Primitives.Triangles;
        }
        internal ILTriangles(ILTriangles source)
            : base(source) {
            Type = Primitives.Triangles;
        }
        #endregion

        #region public interface
        public override void Configure(bool configureDown = true, bool configureUp = true) {
            if (IsDisposed) return; 
            lock (Buffers.ConfigureLock) {
                if (AutoNormals && (Buffers.m_indicesChanged || Buffers.m_positionsChanged)) {
                    AutoComputeNormals();
                }
                base.Configure(configureDown, configureUp);
            }
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILTriangles ret = (ILTriangles)base.Synchronize(copy, syncParams);
            ret.AutoNormals = AutoNormals; 
            return ret; 
        }
        public virtual void AutoComputeNormals() {
            if (Buffers.ShapeIndicesIndex == null) {
                Buffers.ShapeIndicesIndex = Computation.CreateShapeIndicesIndex(GetIndicesForSorting(Indices.Storage));
            }
            Computation.ComputeNormals(Positions, Indices, Buffers.ShapeIndicesIndex, Normals);
        }
        public virtual IEnumerable<Triangle> GetSortedAndClippedScreen(ILRenderParameter parameters, SortingMode sorting = SortingMode.None, bool frustumClipping = false) {
            using (ILScope.Enter()) {
                if (Positions.IsEmpty) yield break;
                ILArray<float> positions_camera = 1;
                ILArray<int> indices = 1;
                ILArray<float> positions_screen = 1;
                ILArray<float> normals_camera = 1;
                bool userClipping = parameters.PeekClipping() != null;
                int minPlaneId = frustumClipping ? 0 : 6;
                int maxPlaneId = userClipping ? 12 : 6;
                StartPipeline(parameters, sorting, frustumClipping, 
                                 positions_camera, indices, positions_screen, normals_camera);

                Stack<Triangle> clippedTriangles = (userClipping || frustumClipping) ? new Stack<Triangle>(16) : null;

                // iterate 
                int curIndex = -1, maxIndex = GetPrimitiveCount(), curClipPlane = 0; 
                for (;;) {
                    Triangle ret;
                    if (clippedTriangles != null && clippedTriangles.Count > 0) {
                        // some clipped triangles were left over
                        ret = clippedTriangles.Pop();
                        curClipPlane = ret.ClipPlaneChecked + 1;
                    } else {
                        if (++curIndex < maxIndex) {
                            #region grab new triangle
                            int i0 = curIndex * 3 + 0; 
                            int i1 = curIndex * 3 + 1; 
                            int i2 = curIndex * 3 + 2;
                            if (!indices.IsEmpty) {
                                i0 = indices.GetValue(i0);
                                i1 = indices.GetValue(i1);
                                i2 = indices.GetValue(i2);
                            }
                            ret = new Triangle();
                            ret.P1 = positions_screen.GetPositionAt(i0);
                            ret.P2 = positions_screen.GetPositionAt(i1);
                            ret.P3 = positions_screen.GetPositionAt(i2);

                            if (float.IsNaN(ret.P1.X) || float.IsNaN(ret.P1.Y) ||
                                float.IsNaN(ret.P2.X) || float.IsNaN(ret.P2.Y) ||
                                float.IsNaN(ret.P3.X) || float.IsNaN(ret.P3.Y))
                                continue; 

                            ret.PCam1 = positions_camera.GetPosition4At(i0);
                            ret.PCam2 = positions_camera.GetPosition4At(i1);
                            ret.PCam3 = positions_camera.GetPosition4At(i2);

                            if (parameters.ColorOverride.Peek().HasValue) {
                                System.Drawing.Color col = parameters.ColorOverride.Peek().GetValueOrDefault();
                                ret.C1 = col.ToVector4();
                                ret.C2 = col.ToVector4();
                                ret.C3 = col.ToVector4();
                            } else if (Color.HasValue) {
                                ret.C1 = Color.Value.ToVector4();
                                ret.C2 = Color.Value.ToVector4();
                                ret.C3 = Color.Value.ToVector4();
                            } else if (Colors.DataCount > i2) {
                                ret.C1 = Colors.GetVector4At(i0);
                                ret.C2 = Colors.GetVector4At(i1);
                                ret.C3 = Colors.GetVector4At(i2);
                            } else {
                                ret.C1 = new Vector4(1, 0, 0, 1);
                                ret.C2 = new Vector4(1, 0, 0, 1);
                                ret.C3 = new Vector4(1, 0, 0, 1);
                            }
                            if (parameters.Alpha.Peek() < 1) {
                                float alpha = parameters.Alpha.Peek(); 
                                ret.C1.W = alpha;
                                ret.C2.W = alpha;
                                ret.C3.W = alpha;
                            }
                            if (Normals != null && Normals.DataCount > i2) {
                                ret.N1 = normals_camera.GetPositionAt(i0);
                                ret.N2 = normals_camera.GetPositionAt(i1);
                                ret.N3 = normals_camera.GetPositionAt(i2);
                            }
                            curClipPlane = minPlaneId;
                            #endregion
                        } else {
                            yield break; 
                        }
                    }
                    #region frustum + user clipping
                    bool skip = false;
                    for (; curClipPlane < maxPlaneId; curClipPlane++) {
                        Vector4 clipCam = parameters.GetClipping(curClipPlane);
                        bool clip1 = false, clip2 = false, clip3 = false;
                        int clipcount = 0;
                        if (Vector4.Dot(clipCam, ret.PCam1) < 0) {
                            clip1 = true;
                            clipcount++;
                        }
                        if (Vector4.Dot(clipCam, ret.PCam2) < 0) {
                            clip2 = true;
                            clipcount++;
                        }
                        if (Vector4.Dot(clipCam, ret.PCam3) < 0) {
                            clip3 = true;
                            clipcount++;
                        }
                        #region clip triangle
                        switch (clipcount) {
                            case 3:
                                skip = true;
                                break;
                            case 2:
                                // no new vertices needed
                                float t;
                                if (!clip1) {
                                    ret.PCam2 = ILHelper.ComputeNewVertex(ret.PCam1, ret.PCam2, clipCam, out t);
                                    ret.C2 = ret.C1 * (1 - t) + ret.C2 * t;
                                    ret.N2 = ret.N1 * (1 - t) + ret.N2 * t;
                                    ret.PCam3 = ILHelper.ComputeNewVertex(ret.PCam1, ret.PCam3, clipCam, out t);
                                    ret.C3 = ret.C1 * (1 - t) + ret.C3 * t;
                                    ret.N3 = ret.N1 * (1 - t) + ret.N3 * t;
                                    ret.P2 = parameters.Cam2Screen(ret.PCam2);
                                    ret.P3 = parameters.Cam2Screen(ret.PCam3);
                                } else if (!clip2) {
                                    ret.PCam1 = ILHelper.ComputeNewVertex(ret.PCam2, ret.PCam1, clipCam, out t);
                                    ret.C1 = ret.C2 * (1 - t) + ret.C1 * t;
                                    ret.N1 = ret.N2 * (1 - t) + ret.N1 * t;
                                    ret.PCam3 = ILHelper.ComputeNewVertex(ret.PCam2, ret.PCam3, clipCam, out t);
                                    ret.C3 = ret.C2 * (1 - t) + ret.C3 * t;
                                    ret.N3 = ret.N2 * (1 - t) + ret.N3 * t;
                                    ret.P1 = parameters.Cam2Screen(ret.PCam1);
                                    ret.P3 = parameters.Cam2Screen(ret.PCam3);
                                } else if (!clip3) {
                                    ret.PCam2 = ILHelper.ComputeNewVertex(ret.PCam3, ret.PCam2, clipCam, out t);
                                    ret.C2 = ret.C3 * (1 - t) + ret.C2 * t;
                                    ret.N2 = ret.N3 * (1 - t) + ret.N2 * t;
                                    ret.PCam1 = ILHelper.ComputeNewVertex(ret.PCam3, ret.PCam1, clipCam, out t);
                                    ret.C1 = ret.C3 * (1 - t) + ret.C1 * t;
                                    ret.N1 = ret.N3 * (1 - t) + ret.N1 * t;
                                    ret.P2 = parameters.Cam2Screen(ret.PCam2);
                                    ret.P1 = parameters.Cam2Screen(ret.PCam1);
                                }
                                // simply go on with next clip plane ... 
                                break;
                            case 1:
                                // need to split into 2 triangles, adds new vertex
                                if (clip1) {
                                    Triangle newTri = ret;
                                    newTri.PCam1 = ILHelper.ComputeNewVertex(ret.PCam2, ret.PCam1, clipCam, out t);
                                    newTri.P1 = parameters.Cam2Screen(newTri.PCam1);
                                    newTri.C1 = ret.C2 * (1 - t) + ret.C1 * t;
                                    newTri.N1 = ret.N2 * (1 - t) + ret.N1 * t;

                                    ret.PCam1 = ILHelper.ComputeNewVertex(ret.PCam3, ret.PCam1, clipCam, out t);
                                    ret.P1 = parameters.Cam2Screen(ret.PCam1);
                                    ret.C1 = ret.C3 * (1 - t) + ret.C1 * t;
                                    ret.N1 = ret.N3 * (1 - t) + ret.N1 * t;
                                    ret.PCam2 = newTri.PCam1;
                                    ret.P2 = newTri.P1;
                                    ret.C2 = newTri.C1;
                                    ret.N2 = newTri.N1;
                                    newTri.ClipPlaneChecked = curClipPlane;
                                    clippedTriangles.Push(newTri);
                                } else if (clip2) {
                                    Triangle newTri = ret;
                                    newTri.PCam2 = ILHelper.ComputeNewVertex(ret.PCam1, ret.PCam2, clipCam, out t);
                                    newTri.P2 = parameters.Cam2Screen(newTri.PCam2);
                                    newTri.C2 = ret.C1 * (1 - t) + ret.C2 * t;
                                    newTri.N2 = ret.N1 * (1 - t) + ret.N2 * t;

                                    ret.PCam2 = ILHelper.ComputeNewVertex(ret.PCam3, ret.PCam2, clipCam, out t);
                                    ret.P2 = parameters.Cam2Screen(ret.PCam2);
                                    ret.C2 = ret.C3 * (1 - t) + ret.C2 * t;
                                    ret.N2 = ret.N3 * (1 - t) + ret.N2 * t;
                                    ret.PCam1 = newTri.PCam2;
                                    ret.P1 = newTri.P2;
                                    ret.C1 = newTri.C2;
                                    ret.N1 = newTri.N2;
                                    newTri.ClipPlaneChecked = curClipPlane;
                                    clippedTriangles.Push(newTri);
                                } else { // is clip3
                                    Triangle newTri = ret;
                                    newTri.PCam3 = ILHelper.ComputeNewVertex(ret.PCam1, ret.PCam3, clipCam, out t);
                                    newTri.P3 = parameters.Cam2Screen(newTri.PCam3);
                                    newTri.C3 = ret.C1 * (1 - t) + ret.C3 * t;
                                    newTri.N3 = ret.N1 * (1 - t) + ret.N3 * t;

                                    ret.PCam3 = ILHelper.ComputeNewVertex(ret.PCam2, ret.PCam3, clipCam, out t);
                                    ret.P3 = parameters.Cam2Screen(ret.PCam3);
                                    ret.C3 = ret.C2 * (1 - t) + ret.C3 * t;
                                    ret.N3 = ret.N2 * (1 - t) + ret.N3 * t;
                                    ret.PCam1 = newTri.PCam3;
                                    ret.P1 = newTri.P3;
                                    ret.C1 = newTri.C3;
                                    ret.N1 = newTri.N3;
                                    newTri.ClipPlaneChecked = curClipPlane;
                                    clippedTriangles.Push(newTri);
                                }
                                break;
                        }
                        #endregion
                        if (skip) break;
                    }
                    if (skip) continue;
                    #endregion

                    yield return ret;
                }
            }
        }

        internal override ILNode Copy() {
            return new ILTriangles(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILTriangles();
        }
        #endregion

        #region private helpers
        private void Normals_Changed(object sender, ILBufferChangedEventArgs args) {
            AutoNormals = false; 
        }

        [System.Security.SecuritySafeCritical]
        private new class Computation : ILMath {

            public static void ComputeNormals(ILPositionsBuffer positions, ILIndicesBuffer indices,
                                              Dictionary<int, List<int>> shapeIndex, ILNormalsBuffer normals) {
                using (ILScope.Enter()) {
                    // calc normals for all individual shapes 
                    ILArray<float> Positions = positions.Storage;
                    ILArray<int> Indices = reshape(indices.Storage, 3, indices.Storage.S[1] / 3);
                    ILArray<float> temp;
                    int lastIdx = -1; 
                    if (!isnullorempty(Indices)) {
                        ILArray<float> a0 = Positions[full, Indices[0, full]];
                        ILArray<float> a1 = Positions[full, Indices[1, full]];
                        ILArray<float> a2 = Positions[full, Indices[2, full]];
                        temp = cross(a1 - a0, a1 - a2, normalize: true);
                    } else {
                        lastIdx = (int)(Math.Floor(Positions.S[1] / 3.0) * 3) - 1; 
                        if (lastIdx < 2) return;
                        ILArray<float> a0 = Positions[full, r(0, 3, lastIdx)];
                        ILArray<float> a1 = Positions[full, r(1, 3, lastIdx)];
                        ILArray<float> a2 = Positions[full, r(2, 3, lastIdx)];
                        temp = cross(a1 - a0, a1 - a2, normalize: true);
                    }
                    // now compute normals for individual vertices 
                    // take all shapes which share a vertex into account
                    if (shapeIndex != null) {
                        ILArray<float> autoNormals = array<float>(Positions.S);
                         
                        for (int i = 0; i < positions.DataCount; i++) {
                            if (!shapeIndex.ContainsKey(i)) continue;
                            List<int> list = shapeIndex[i]; 
                            float s0 = 0, s1 = 0, s2 = 0; 
                            foreach (int li in list) {
                                s0 += temp.GetValue(0, li);
                                s1 += temp.GetValue(1, li);
                                s2 += temp.GetValue(2, li);
                            }
                            float n = (float)Math.Sqrt(
                                        s0 * s0 +
                                        s1 * s1 +
                                        s2 * s2); 
                            s0 /= n; s1 /= n; s2 /= n;
                            autoNormals.SetValue(s0, 0, i);
                            autoNormals.SetValue(s1, 1, i);
                            autoNormals.SetValue(s2, 2, i); 
                        }
                        normals.Update(0, positions.DataCount, autoNormals);
                    } else {
                        // ind scheme: 0,0,0,1,1,1,2,2,2,3........
                        ILArray<int> ind = repmat(counter<int>(0, 1, size(1, temp.S[1])), 3, 1)[full]; 
                        temp.a = temp[full,ind];
                        normals.Update(0, temp.S[1], temp); 
                    }
                }
            }
            /// <summary>
            /// Creates the index of the shape indices.
            /// </summary>
            /// <param name="shapeIndices">The shape indices.</param>
            /// <returns>index of shape indices</returns>
            /// <remarks>The index of shape indices is used for fast facette lookup while (auto) creating
            /// the normal vectors for the vertices. Therefore, the index of every vertex used in the shape
            /// serves as index for a list of those facettes, where that vertex occures.
            /// <para>TODO: may be replaced by a custom data structure in order to decrease memory requirements?</para></remarks>
            public static Dictionary<int, List<int>> CreateShapeIndicesIndex(ILInArray<int> shapeIndices) {
                using (ILScope.Enter(shapeIndices)) {
                    if (isnullorempty(shapeIndices)) 
                        return null; 
                    Dictionary<int, List<int>> ret = new Dictionary<int, List<int>>();
                    int shapeCount = shapeIndices.Size[1];
                    int vertPerShape = shapeIndices.Size[0];
                    int curShapeIdx = 0;
                    int curRowIdx = 0;
                    foreach (int i in shapeIndices) {
                        if (!ret.ContainsKey(i))
                            ret.Add(i, new List<int>());
                        ret[i].Add(curShapeIdx);
                        curRowIdx++;
                        if (curRowIdx >= vertPerShape) {
                            curRowIdx = 0;
                            curShapeIdx++;
                        }
                    }
                    return ret;
                }
            }
        }
        #endregion

    }
}
