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
using System.Xml.Serialization; 

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public class ILAxisCollection : ILGroup, IEnumerable<ILAxis> {

        #region attributes
        /// <summary>
        /// Length difference threshold for choosing an axis position candidate based on its length rather than its position. Default: 3
        /// </summary>
        public static float AUTOAXIS_PREFER_LENGTH_OVER_POSITION_FACTOR = 3f;
        /// <summary>
        /// Prefered position of axis which do not lay on the hull of the unit cube. Clip coordinates. Default: (1,-1) (lower right outside region)
        /// </summary>
        public static PointF AUTOAXIS_NON_HULL_SCREEN_POSITION = new PointF() { X = 1, Y = -1 };
        /// <summary>
        /// Prefered position of axis which exist on the hull of the unit cube. Clip coordinates. Default: (0.45,1) (lower left corner)
        /// </summary>
        public static PointF AUTOAXIS_HULL_SCREEN_POSITION = new PointF() { X = -1, Y = 1 }; 
        /// <summary>
        /// Tag used to identify the object within the scene graph 
        /// </summary>
        public static readonly string DefaultTag = "AxisCollection"; 

        private static ILArray<float> CubeWorldParameter = ILMath.localMember<float>();
        /// <summary>
        /// Default helper graphics instance; used to measure string extends
        /// </summary>
        internal static Graphics Graphics = Graphics.FromImage(new Bitmap(1,1));
        #endregion

        #region properties
        [XmlIgnore]
        public ILAxis this[int id] {
            get { return (ILAxis)m_children.ElementAt(id); }
            set {
                if (value != null) {
                    m_children[id] = value;
                }
            }
        }
        /// <summary>
        /// Gives the first axis with matching axis name (XAxis/YAxis/ZAxis) or null
        /// </summary>
        [XmlIgnore]
        public ILAxis this[AxisNames name] {
            get { return First<ILAxis>(predicate: (a) => { return a.AxisName == name; }); }
        }
        /// <summary>
        /// Give first XAxis from the axis collection, or null
        /// </summary>
        [XmlIgnore]
        public ILAxis XAxis {
            get { return First<ILAxis>(predicate: (a) => { return a.AxisName == AxisNames.XAxis; }); }
        }
        /// <summary>
        /// Give first YAxis from the axis collection, or null
        /// </summary>
        [XmlIgnore]
        public ILAxis YAxis {
            get { return First<ILAxis>(predicate: (a) => { return a.AxisName == AxisNames.YAxis; }); }
        }
        /// <summary>
        /// Give first ZAxis from the axis collection, or null
        /// </summary>
        [XmlIgnore]
        public ILAxis ZAxis {
            get { return First<ILAxis>(predicate: (a) => { return a.AxisName == AxisNames.ZAxis; }); }
        }

        #endregion

        #region ctors 
        static ILAxisCollection() {
            CubeWorldParameter.a = ILMath.array<float>(new float[] { 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1 }, 3, 8);
        }
        private ILAxisCollection() { }
        /// <summary>
        /// Create new Axis Collection
        /// </summary>
        /// <param name="plotContainer">[optional] IILAxisDataProvider to be used to retrieve axis' data from; default: null (take the first provider found on the path up to the root node)</param>
        /// <param name="tag">[optional] tag used to identify the object within the scene graph</param>
        public ILAxisCollection(IILAxisDataProvider plotContainer = null, object tag = null)
            : base(tag ?? DefaultTag) {
            ILAxis axis = null;
            axis = new ILAxis(plotContainer); axis.AxisName = AxisNames.XAxis; axis.Label.Text = "X Axis"; m_children.Add(axis);
            axis = new ILAxis(plotContainer); axis.AxisName = AxisNames.YAxis; axis.Label.Text = "Y Axis"; m_children.Add(axis);
            axis = new ILAxis(plotContainer); axis.AxisName = AxisNames.ZAxis; axis.Label.Text = "Z Axis"; m_children.Add(axis);
        }
        internal ILAxisCollection(ILAxisCollection source)
            : base(source) {

        }
        #endregion

        #region public interface 
        internal override ILNode Copy() {
            return new ILAxisCollection(this); 
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILAxisCollection();
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILAxisCollection ret = (ILAxisCollection)base.Synchronize(copy, syncParams);
            return ret; 
        }
        /// <summary>
        /// Add new axis to the axes collection
        /// </summary>
        /// <param name="axis">the axis to be added</param>
        /// <returns>axis added to this collection (might differ from <paramref name="axis"/> provided)</returns>
        public ILAxis Add(ILAxis axis) {
            return base.Add(axis); 
        }
        /// <summary>
        /// Remove an axis from the collection
        /// </summary>
        /// <param name="axis"></param>
        public void Remove(ILAxis axis) {
            m_children.Remove(axis); 
        } 
        internal void ConfigureAxes_BeginVisit(ILRenderParameter parameter, ILLimits dataRange) {
            var viewParameter = Computation.getOptimalAxesParameter(parameter);
            System.Diagnostics.Debug.Assert(viewParameter.UnitCube.Count == 3);
            System.Diagnostics.Debug.Assert(viewParameter.Screen.Count == 3); 
            foreach (ILAxis axis in this) {
                axis.ConfigureAxis(parameter, viewParameter);
            }
        }
        internal Size CalculateDefaultSize(ILRenderParameter parameter) {
            int retX = int.MinValue;
            int retY = int.MinValue;
            // get screen (pixel) extend
            foreach (var axis in this) {
                Size size = axis.CalculateDefaultSize(parameter);
                if (size.Width > retX) retX = size.Width;
                if (size.Height > retY) retY = size.Height; 
            }
            if (retX <= 1 || retY <= 1) {
                return Size.Empty; 
            }
            return new Size(retX, retY);
        }

        
        #endregion

        #region IEnumerable<ILAxis> Members

        public IEnumerator<ILAxis> GetEnumerator() {
            foreach (var a in m_children) {
                ILAxis ret = a as ILAxis; 
                if (ret != null) 
                    yield return ret; 
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        [System.Security.SecuritySafeCritical]
        internal class Computation : ILMath {

            private static ILArray<int> s_unitCubeNet = localMember<int>(); 
            internal static ILRetArray<int> UnitCubeNet {
                get {
                    if (s_unitCubeNet.IsEmpty) {
                        s_unitCubeNet.a = new int[,] {
                        {1,3,4},
                        {0,2,5},
                        {1,3,6},
                        {2,0,7},
                        {5,0,7},
                        {4,1,6},
                        {5,2,7},
                        {4,6,3}
                        }; 
                    }
                    return s_unitCubeNet; 
                }
            }

            public static ILViewAxesParameters getOptimalAxesParameter(ILRenderParameter parameter) {
                ILViewAxesParameters ret = new ILViewAxesParameters();
                using (ILScope.Enter()) {
                    // determine position 
                    ILArray<float> unitCube = ILPositionsBuffer.UnitCube.Storage;
                    ILArray<float> unitCubeClip = parameter.ToClipWithDivide(unitCube); //["0,1;:"];
                    ILArray<float> unitCubeScreen = parameter.ViewTransform * unitCubeClip; 
                    ILArray<int> hullIDs = Computation.hull2(unitCubeClip, UnitCubeNet); // * array<float>(1f, -1f));
                    ILArray<float> hullPositionsCube = unitCube[":", hullIDs];

                    ILArray<int> axisTypes = 1;
                    max(abs(diff(hullPositionsCube, dim: 1)), I: axisTypes, dim: 0).Dispose();

                    for (int axisID = 0; axisID < 3; axisID++) {
                        #region axis parameter
                        ILArray<float> direction = zeros<float>(1, 3);
                        direction[axisID] = 1;
                        ILArray<int> axisPos = find(axisTypes == axisID);

                        ILArray<float> screen1 = unitCubeScreen["0,1", hullIDs[axisPos]];
                        ILArray<int> sndPos = axisPos + 1;
                        if (anyall(sndPos >= hullIDs.Length)) {
                            sndPos.a = mod(sndPos, hullIDs.Length);
                        }
                        ILArray<float> screen2 = unitCubeScreen["0,1", hullIDs[sndPos]];

                        if (axisPos.Length == 1) {
                            ILArray<float> bestFit = hullPositionsCube[full, axisPos];
                            ret.UnitCube[axisID] = Tuple.Create(
                                new Vector3(bestFit.GetValue(0), bestFit.GetValue(1), bestFit.GetValue(2)),
                                new Vector3(direction.GetValue(0), direction.GetValue(1), direction.GetValue(2)));

                            ret.Screen[axisID] = Tuple.Create(
                                new PointF(screen1.GetValue(0), screen1.GetValue(1)),
                                new PointF(screen2.GetValue(0), screen2.GetValue(1)));
                        } else if (axisPos.Length >= 2) {
                            // decide which axis is best: either lower left or the longer one
                            ILArray<float> screenLen = screen1 - screen2;
                            ILArray<float> length = sqrt(sum(screenLen * screenLen, 0));
                            ILArray<int> takeID = -1;
                            if (length[0] > length[1] * AUTOAXIS_PREFER_LENGTH_OVER_POSITION_FACTOR) {
                                takeID = 0;
                            } else if (length[1] > length[0] * AUTOAXIS_PREFER_LENGTH_OVER_POSITION_FACTOR) {
                                takeID = 1;
                            } else {
                                ILArray<float> dist = (screen1 + screen2) / 2
                                    - array<float>(AUTOAXIS_HULL_SCREEN_POSITION.X * parameter.Driver.Size.Width, 
                                                    AUTOAXIS_HULL_SCREEN_POSITION.Y * parameter.Driver.Size.Height);
                                dist.a = sum(dist * dist);
                                min(dist, I: takeID, dim: 1).Dispose();
                            }
                            ILArray<float> bestFit = hullPositionsCube[full, axisPos[takeID]];
                            bestFit[axisID] = 0;
                            ret.UnitCube[axisID] = Tuple.Create(
                                new Vector3(bestFit.GetValue(0), bestFit.GetValue(1), bestFit.GetValue(2)),
                                new Vector3(direction.GetValue(0), direction.GetValue(1), direction.GetValue(2)));
                            screen1.a = screen1[":", takeID];
                            screen2.a = screen2[":", takeID]; 
                            ret.Screen[axisID] = Tuple.Create(
                                new PointF(screen1.GetValue(0), screen1.GetValue(1)),
                                new PointF(screen2.GetValue(0), screen2.GetValue(1)));

                        } else if (isempty(axisPos)) {
                            #region some heuristics
                            ILArray<int> thisIds = find(unitCube[axisID, full] == 0);
                            ILArray<float> startClip = unitCubeClip[full, thisIds]; 
                            ILArray<float> endClip = parameter.ToClipWithDivide(unitCube[full, thisIds] + direction.T); //["0,1;:"];
                            ILArray<float> centersClip = (endClip + startClip) / 2f;
                            ILArray<int> minDist = 1;
                            ILArray<float> dist = centersClip - array<float>(AUTOAXIS_NON_HULL_SCREEN_POSITION.X * parameter.Driver.Size.Width,
                                                                             AUTOAXIS_NON_HULL_SCREEN_POSITION.Y * parameter.Driver.Size.Height,
                                                                             0,0);
                            dist.a = sum(dist * dist,0); 
                            min(dist, I: minDist, dim: 1).Dispose();
                            centersClip.a = unitCube[full, thisIds[minDist]];
                            ret.UnitCube[axisID] = Tuple.Create(
                                new Vector3(centersClip.GetValue(0), centersClip.GetValue(1), centersClip.GetValue(2)),
                                new Vector3(direction.GetValue(0), direction.GetValue(1), direction.GetValue(2)));
                            
                            ILArray<float> screen = parameter.ViewTransform * startClip[full,minDist].Concat(endClip[full,minDist],1); 
                            ret.Screen[axisID] = Tuple.Create(
                                new PointF(screen.GetValue(0, 0), screen.GetValue(1, 0)),
                                new PointF(screen.GetValue(0, 1), screen.GetValue(1, 1)));
                            #endregion
                        }
                        #endregion

                        #region grid parameter
                        /* Algo: first all edges are identified (constructed) which are relevant for the current axis. 
                         * All 4 edges correspond to 4 sides of the cube. Each one gets a direction marked by the cross 
                         * product of the * direction along the axis and * a corresponding "along non-axis" edge. 
                         * The resulting (perpendicular) directions should go along the 'area' normal vector.
                         * The sign of its z coordinate says, if the side is watched from inside or outside.
                         */

                        List<Tuple<Vector3,Vector3>> grid = new List<Tuple<Vector3,Vector3>>(); 
                        // find backsides 
                        ILArray<float> pos = zeros<float>(3,5); 
                        ILArray<int> select = new int[] { 0,1,2 }; 
                        ILArray<int> nonDir = select[select != axisID];
                        pos[nonDir,"1,2,3"] = new float[] { 1,0,1,1,0,1 }; 
                        ILArray<float> pos2 = pos.C; 
                        pos2[axisID,full] = pos2[axisID,full] + 1; 
                        pos.a = pos.Concat(pos2,1);
                        
                        float flipY = axisID == 1 ? -1 : 1; 
                        ILArray<float> posClip = parameter.ToClipWithDivide(pos);
                        ILArray<float> c = normalize(cross(normalize(posClip[":;5:8"] - posClip[":;0:3"]), // along axis - direction
                                                 normalize(posClip[":;1:4"] - posClip[":;0:3"]) * flipY)); 
                        //ILArray<float> d = diag(multiply(c.T, normalize(posClip[full, r(0,3)]))); // cos angle with "origins" <- CHECK THIS! 
                        foreach (int id in toint32(find(c[2,full] > 0))) { // argh!! :(
                            ILArray<float> thisDir = pos[full, id + 1] - pos[full, id];
                            grid.Add(Tuple.Create(new Vector3(pos.GetValue(0, id), pos.GetValue(1, id), pos.GetValue(2, id)), 
                                                    new Vector3(thisDir.GetValue(0), thisDir.GetValue(1), thisDir.GetValue(2))
                                                  ));
                        }
                        ret.Grids[axisID] = grid; 
                        //ILArray<float> cubeClip = parameter.ToClip(CubeWorldParameter); 
                        //ILArray<float> dir = normalize(cubeClip[full,axisID] - cubeClip[full, 6]); 
                        //ILArray<float> nonAxisDirs = normalize(cubeClip[full, 6] - cubeClip[full, nonDir]);
                        //ILArray<float> c = normalize(cross(dir[":;0,0"], nonAxisDirs));
                        //c.a = multiply(c.T, normalize(-cubeClip[full, 6]));
                        //foreach (int id in nonDir[c > 0]) {
                        //    ILArray<float> thisDir = CubeWorldParameter[full, id];
                        //    grid.Add(Tuple.Create(new Vector3(0, 0, 0), new Vector3(thisDir.GetValue(0), thisDir.GetValue(1), thisDir.GetValue(2))));
                        //}
                        //nonDir.a = nonDir + 3; 
                        //dir.a = cubeClip[full, axisID + 3] - cubeClip[full, 7]; 
                        //nonAxisDirs.a = cubeClip[full, nonDir] - cubeClip[full, 7]; 
                        //c.a = normalize(cross(dir[":;0,0"], nonAxisDirs));
                        //c.a = multiply(c.T, normalize(-cubeClip[full, 7]));
                        //ILArray<float> startPos = CubeWorldParameter[full, axisID + 3]; 
                        //foreach (int id in nonDir[c >= 0]) {
                        //    ILArray<float> thisDir = CubeWorldParameter[full, id] - CubeWorldParameter[full, 7];
                        //    //grid.Add(Tuple.Create(new Vector3(startPos.GetValue(0), startPos.GetValue(1), startPos.GetValue(2)), 
                        //    //                      new Vector3(thisDir.GetValue(0), thisDir.GetValue(1), thisDir.GetValue(2))));
                        //}
                        #endregion
                    }
                    return ret;
                }
            }
            /// <summary>
            /// Gives indices of vertices in <paramref name="points"/> which form a convex hull
            /// </summary>
            /// <param name="points">vertices, 2D coordinates, 2 rows, n columns</param>
            /// <returns>indices of hull of points</returns>
            public static ILRetArray<int> hull(ILInArray<float> points) {
                using (ILScope.Enter(points)) {
                    // naive graham 
                    ILArray<int> ind = 1; 
                    ILArray<int> ret = zeros<int>(1,points.S[1] + 1); 
                    min(points[1,full],I:ind).Dispose(); // assuming screen coords here 
                    ret[0, 0] = (int)ind; 
                    // sort polar angles with p0 and x axis
                    ILArray<float> angles = points["0,1;:"] - points["0,1",ind]; 
                    angles.a = angles / sqrt(sum(angles * angles, 0)); 
                    angles.a = acos(multiply(array<float>(1f,0f).T, angles["0,1;:"])); 
                    sort(angles, Indices: ind).Dispose(); 
                    ret[0,r(1,end-1)] = toint32(ind[r(0,end-1)]); 
                    // scan loop
                    int m = 1, n = ret.S[1]-1; 
                    for (int i = 2; i < n; i++) {
                        while (true) {
                            ILArray<float> ccw = (points.GetValue(0, ret.GetValue(m)) - points.GetValue(0, ret.GetValue(m - 1))) *
                                                (points.GetValue(1, ret.GetValue(i)) - points.GetValue(1, ret.GetValue(m - 1))) -
                                                (points.GetValue(1, ret.GetValue(m)) - points.GetValue(1, ret.GetValue(m - 1))) *
                                                (points.GetValue(0, ret.GetValue(i)) - points.GetValue(0, ret.GetValue(m - 1)));
                            if (ccw >= 0)
                                break;

                            if (m > 1)
                                m -= 1;
                            else if (i == ret.S[1] - 1)
                                break;
                            else
                                i += 1;
                        }
                        m += 1; 
                        ret[0,m] = ret[0,i]; 
			        }
                    ret[m + 1] = ret[0]; 
                    return ret[r(0,m+1)]; 

                }
            }

            public static ILRetArray<int> hull2(ILInArray<float> clip, ILInArray<int> net) {
                using (ILScope.Enter(clip, net)) {
                    /*!HC: 
                     * 2nd attempt: with orthographic projection the common case is to have multiple points on the same 
                     * screen coords - with different depth. this algorithm is to handle this situation in a more 
                     * reliable way. It does not rely on graham scan but takes the knowledge of the (unit-cube) shape 
                     * into account.
                     */
                    ILArray<int> ret = zeros<int>(1,clip.S[1]+1); 
                    int found = 1; 
                    // find the lowest coordinate (y), handle multiple points on this lowest area (if any) 
                    ILArray<int> ind = 1;
                    ILArray<int> lowest = find(clip[1, full] == min(clip[1, full]));
                    if (lowest.Length > 1) {
                        // take the one most left, than in front
                        lowest.a = lowest[find(clip[0,lowest] == min(clip[0,lowest]))];
                        if (lowest.Length > 1) {
                            lowest.a = lowest[find(clip[2, lowest] == min(clip[2, lowest]))][0]; // handle some degenerated cases
                        }
                    }
                    System.Diagnostics.Debug.Assert(lowest.Length == 1);
                    ILArray<float> startClip = clip[full,lowest];
                    ret[0] = (int)lowest; 
                    // find the initial direction 
                    ILArray<float> cand = clip[full, net[":", lowest]] - startClip;
                    ILArray<float> angles = atan2(cand[1,full].T, cand[0,full].T); 
                    min(angles, I: ind, dim:0).Dispose();
                    // ind is 0,1 or 2
                    int currHullClipID = (int)net[ind, lowest];
                    ILArray<float> next = clip[full, currHullClipID];
                    int prevHullClipID = (int)lowest; 
                    ret[found++] = currHullClipID; 
                    do  {
                        // only 2 posible directions left, since we dont want to go back! 
                        ind = find(net[":", currHullClipID] != prevHullClipID); 
                        cand.a = clip[full, net[ind, currHullClipID]] - next;
                        // one might be 0
                        if (cand[0] == 0 && cand[1] == 0) {
                            prevHullClipID = currHullClipID;
                            currHullClipID = (int)net[ind[1], currHullClipID]; 
                        } else if (cand[0, 1] == 0 && cand[1, 1] == 0) {
                            prevHullClipID = currHullClipID;
                            currHullClipID = (int)net[ind[0], currHullClipID];
                        } else {
                            // take the one, which is furthest "ccw"
                            ILArray<float> q = next + cand[full, 0];
                            ILArray<float> r1 = next + cand[full, 1]; 
                            // angles = atan2(cand[1, full].T, cand[0, full].T);
                            // min(angles, I: ind, dim: 0).Dispose();
                            float c = (q.GetValue(0) - next.GetValue(0)) * (r1.GetValue(1) - next.GetValue(1)) 
                                    - (r1.GetValue(0) - next.GetValue(0)) * (q.GetValue(1) - next.GetValue(1)); ;
                            if (c == 0) {
                                // both lay on a line
                                ILArray<float> len = sum(cand * cand,0);
                                if (len[0] == len[1]) {
                                    // take the closer one 
                                    c = cand[2,0] < cand[2,1] ? 1 : -1; 
                                } else {
                                    // take the one which brings us further 
                                    c = len[0] > len[1] ? 1 : -1;
                                }
                            }
                            if (c > 0) {
                                prevHullClipID = currHullClipID;
                                currHullClipID = (int)net[ind[0], currHullClipID];
                            } else {
                                prevHullClipID = currHullClipID;
                                currHullClipID = (int)net[ind[1], currHullClipID];
                            }
                        }
                        next = clip[full, currHullClipID];
                        ret[found++] = currHullClipID;
                    } while (currHullClipID != (int)lowest && found < ret.Length); 
                    if (found >= ret.Length) 
                        found = ret.Length - 1; 
                    return ret[r(0,found-1)]; 
                }
            }
            public static ILRetArray<float> normalize(ILInArray<float> A) {
                using (ILScope.Enter(A)) {
                    if (A.IsEmpty) {
                        return empty<float>(A.S);
                    }
                    return A / sqrt(sum(A * A, 0)); 
                }
            }

        }
    }
}
