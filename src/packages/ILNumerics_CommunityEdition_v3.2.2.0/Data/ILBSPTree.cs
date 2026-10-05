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
using ILNumerics.Drawing; 

namespace ILNumerics.Data {
    /// <summary>
    /// Primitive struct for BSP tree nodes; may refer to labels, points, lines or triangles
    /// </summary>
    public struct BSPPrimitive<T> {

#if DEBUG
        //public int ID; 
#endif
        public T Data;
        public Vector4[] Colors;
        public Vector3[] Positions;
        public Vector3[] CameraPos;
        public Vector3[] Normals;
        public float LineOffset;

        public BSPPrimitive(Triangle tri, T data) {
            Data = data;
            // screen positions
            Positions = new Vector3[3];
            Positions[0] = tri.P1;
            Positions[1] = tri.P2;
            Positions[2] = tri.P3;
            CameraPos = new Vector3[3];
            CameraPos[0] = tri.PCam1.Xyz;
            CameraPos[1] = tri.PCam2.Xyz;
            CameraPos[2] = tri.PCam3.Xyz;
            Normals = new Vector3[3];
            Normals[0] = tri.N1;
            Normals[1] = tri.N2;
            Normals[2] = tri.N3;
            Colors = new Vector4[3];
            Colors[0] = tri.C1;
            Colors[1] = tri.C2;
            Colors[2] = tri.C3;
            LineOffset = 0; 
        }

        public BSPPrimitive(ILLabel label, T data, Vector3 position) {
            Positions = new Vector3[1] { position };
            Colors = new Vector4[0];
            Normals = new Vector3[0];
            CameraPos = new Vector3[0];
            Data = data;
            LineOffset = 0; 
        }

        public BSPPrimitive(Point point, T points) {
            Positions = new Vector3[1] { point.P1 };
            Colors = new Vector4[1]  { point.C1 };
            Normals = new Vector3[1] { point.N1 };
            CameraPos = new Vector3[0];
            Data = points;
            LineOffset = 0;
        }

        public BSPPrimitive(Line line, T lines) {
            Data = lines;
            // screen positions
            Positions = new Vector3[2];
            Positions[0] = line.P1;
            Positions[1] = line.P2;
            CameraPos = new Vector3[2];
            CameraPos[0] = line.PCam1.Xyz;
            CameraPos[1] = line.PCam2.Xyz;
            Normals = new Vector3[2];
            Normals[0] = line.N1;
            Normals[1] = line.N2;
            Colors = new Vector4[2];
            Colors[0] = line.C1;
            Colors[1] = line.C2;
            LineOffset = 0;
        }
        public int VertexCount {
            get {
                return (Positions != null) ? Positions.Length : 0;
            }
        }
        /// <summary>
        /// Derive a new vertex on an edge of this primitive by interpolating from vertices at both ends
        /// </summary>
        /// <param name="a">first vertex ID</param>
        /// <param name="b">second vertex ID</param>
        /// <param name="t">fraction between a and b; range [0..1]</param>
        /// <param name="storage">target vertex struct to store the results into</param>
        public void Interpolate(int a, int b, float t, ref Vertex storage) {
            if (Colors != null) {
                Vector4 c1 = Colors[a];
                Vector4 c2 = Colors[b];
                storage.Colors = c1 * (1 - t) + c2 * t;
            }
            if (Normals != null) {
                Vector3 n1 = Normals[a];
                Vector3 n2 = Normals[b];
                storage.Normals = n1 * (1 - t) + n2 * t;
            }
            if (CameraPos != null) {
                Vector3 cp1 = CameraPos[a];
                Vector3 cp2 = CameraPos[b];
                storage.CameraPos = cp1 * (1 - t) + cp2 * t;
            }
            float xOff = (Positions[b].X - Positions[a].X);
            float yOff = (Positions[b].Y - Positions[a].Y);
            xOff = (float)Math.Sqrt(xOff * xOff + yOff * yOff);
            storage.LineOffset = storage.LineOffset + xOff * t; 
        }
        /// <summary>
        /// Retrieve individual vertex from the primitive by index
        /// </summary>
        /// <param name="idx">index; range 0 .. VertexCount</param>
        /// <returns>Vertex with position, color and normal information. If colors or normals are not defined in the original shape, default values are returned.</returns>
        public Vertex GetVertexAt(int idx) {
            Vertex ret = new Vertex();
            ret.XYZ = Positions[idx];
            if (Colors != null) {
                ret.Colors = Colors[idx];
            }
            if (Normals != null) {
                ret.Normals = Normals[idx];
            }
            if (CameraPos != null) {
                ret.CameraPos = CameraPos[idx];
            }
            return ret;
        }

        /// <summary>
        /// Create plane from this primitive Positions
        /// </summary>
        /// <returns>Plane</returns>
        public Vector4 GetPlane() {
            switch (VertexCount) {
                case 1:
                    return new Vector4(0, 0, 1, -Positions[0].Z); 
                case 2:
                    Vector3 p = Positions[1] - Positions[0], w;
                    if (p.Length < ILMath.epsf) {
                        return new Vector4(0, 0, 1, -Positions[0].Z);
                    } else {
                        if (Math.Abs(p.X) < ILMath.epsf) w = new Vector3(1, 0, 0);
                        else if (Math.Abs(p.Y) < ILMath.epsf) w = new Vector3(0, 1, 0);
                        else w = new Vector3(0, 0, -1);
                    }
                    Vector3 plane = Vector3.CrossN(p, w);
                    return new Vector4(plane, -Vector3.Dot(plane, Positions[0]));
                default: // triangle
                    Vector3 p1 = Positions[0], p2 = Positions[1] - p1, p3 = Positions[2] - p1;
                    if (p2.LengthFast < ILMath.epsf || p3.LengthFast < ILMath.epsf) {
                        return new Vector4(0, 0, 1, -Positions[0].Z);
                    }
                    Vector3 a = Vector3.CrossN(p2, p3);
                    return new Vector4(a, -Vector3.Dot(a, p1));
            }
        }
        /// <summary>
        /// String representation for this primitive
        /// </summary>
        /// <returns>Primitive description</returns>
        public override string ToString() {
//#if DEBUG
//            return "(" + ID + ") " +  (Data != null ? Data.ToString() : "(empty)");
//#endif 
            return Data != null ? Data.ToString() : "(empty)";
        }

        internal Triangle GetTriangle() {
            System.Diagnostics.Debug.Assert(VertexCount == 3); 
            Triangle ret = new Triangle(); 
            ret.P1 = Positions[0]; ret.P2 = Positions[1]; ret.P3 = Positions[2];
            if (Colors != null) {
                ret.C1 = Colors[0]; ret.C2 = Colors[1]; ret.C3 = Colors[2];
            }
            if (Normals != null) {
                ret.N1 = Normals[0]; ret.N2 = Normals[1]; ret.N3 = Normals[2];
            }
            if (CameraPos != null) {
                ret.PCam1 = CameraPos[0]; ret.PCam2 = CameraPos[1]; ret.PCam3 = CameraPos[2];
            }
            return ret; 
        }
    }
    //internal struct BSPPrimitive {
    //    public ILDrawable Shape;
    //    public int PrimitiveID; 
    //}
    public struct Vertex {
        public Vector4 Colors; 
        public Vector3 XYZ;
        public Vector3 CameraPos; 
        public Vector3 Normals;
        public float LineOffset; 
    }
    
    /// <summary>
    /// A node within the BSP tree
    /// </summary>
    public class ILBSPNode<T> {
        /// <summary>
        /// Collection of front primitives
        /// </summary>
        public ILBSPNode<T> Front { get; set; }
        /// <summary>
        /// Collection of back primitives
        /// </summary>
        public ILBSPNode<T> Back { get; set; }
        /// <summary>
        /// Dividing plane
        /// </summary>
        public Vector4 Plane { get; set; }
        /// <summary>
        /// Collection of on-plane primitives
        /// </summary>
        internal List<BSPPrimitive<T>> Primitives { get; set; }
        /// <summary>
        /// Create string representation of this node
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
//#if DEBUG 
//            return String.Format("Plane: {0} On: {1} Front: {2} Back: {3}",
//                        Plane,
//                        Primitives != null ? String.Join(",", Primitives.Select(p => "(" + p.ID + ")")) : "--",
//                        Front != null ? Front.Count().ToString() : "--",
//                        Back != null ? Back.Count().ToString() : "--");
//#endif 
            return String.Format("Plane: {0} On: {1} Front: {2} Back: {3}", 
                        Plane, 
                        Primitives != null ? Primitives.Count.ToString() : "--",
                        Front != null ? Front.Count().ToString() : "--",
                        Back != null ? Back.Count().ToString() : "--");
        }
        /// <summary>
        /// Compute the number of primitives within the node and subtree 
        /// </summary>
        /// <returns>Primitives count</returns>
        public int Count() {
            int ret = 0; 
            if (Primitives != null) ret += Primitives.Count;
            if (Front != null) ret += Front.Count();
            if (Back != null) ret += Back.Count(); 
            return ret; 
        }

    }
    /// <summary>
    /// Possible values, influencing the BSP tree creation
    /// </summary>
    public enum BSPTreeHint {
        FastCreation,
        FastRunning
    }

    public class ILBSPTreeSettings {
        /// <summary>
        /// Split planes are considered 'thick' in order to handle floating point inaccuracies. Default: single precision eps 
        /// </summary>
        public float PlaneThickness { get; set; }
        /// <summary>
        /// Determines if the tree favors balancing and runtime speed over creation speed. Default: fast creation
        /// </summary>
        public BSPTreeHint Hint { get; set; }
        /// <summary>
        /// For Hint.FastRunning mode: the ratio between a balanced tree and minimal necessary primitive splits. Default: 0.86
        /// </summary>
        public float BalanceSplitRatio { get; set; }
        /// <summary>
        /// Creates a new settings instance with default paramters.
        /// </summary>
        public ILBSPTreeSettings() {
            PlaneThickness = ILMath.epsf;
            Hint = BSPTreeHint.FastCreation;
            BalanceSplitRatio = 0.86f;
        }

    }

    /// <summary>
    /// Binary Space Partioning (BSP) tree class
    /// </summary>
    /// <typeparam name="T">Data type for BSP tree nodes</typeparam>
    /// <remarks>The class is used on several places withing ILNumerics. One example is the preparation of scenes for rendering in SVG driver.</remarks>
    public class ILBSPTree<T> {

        #region attributes
        internal const int FRONT_PRIMITIVE = 1;
        internal const int BACK_PRIMITIVE = -1;
        internal const int COPLA_PRIMITIVE = 0;
        internal const int SPLIT_PRIMITIVE = 99; 
        internal const int FRONT_POINT = -1;
        internal const int BACK_POINT = 1;
        internal const int ON_POINT = 0;
        private ILBSPTreeSettings m_settings; 
        [ThreadStatic]
        private Random m_random;
        /// <summary>
        /// Thread static random generator
        /// </summary>
        protected Random Random {
            get {
                if (m_random == null) {
                    m_random = new Random(); 
                }
                return m_random; 
            }
        }


        /// <summary>
        /// Root node of the tree
        /// </summary>
        public ILBSPNode<T> Root { get; set; }
        #endregion

        #region ctors
        private ILBSPTree() {

        }
        /// <summary>
        /// Create new BSP tree, given the list of primitives 
        /// </summary>
        /// <param name="primitives">individual, prebuild primitives </param>
        /// <param name="settings">[optional] settings for tuning the tree creation. Default: default instance</param>
        public ILBSPTree(IEnumerable<BSPPrimitive<T>> primitives, ILBSPTreeSettings settings = null) {
            m_settings = settings ?? new ILBSPTreeSettings();
            if (primitives.Count() == 0) return;
            
            // create the BSP tree
            Root = new ILBSPNode<T>();

            if (primitives is List<BSPPrimitive<T>>)
                Create(Root, primitives as List<BSPPrimitive<T>>);
            else {
                Create(Root, primitives.ToList());
            }
        }
        #region obsolete 
        ///// <summary>
        ///// Create new BSP tree from given drawable nodes
        ///// </summary>
        ///// <param name="shapes">All drawables participating in the partitioning</param>
        ///// <param name="settings">[optional] BSPTree settings parameter instance. Default: default settings</param>
        ///// <param name="root">A common root node for all shapes, provides the coordinate system for primitives in the BSP nodes</param>
        //public static ILBSPTree<ILDrawable> CreateFromShapes(IEnumerable<ILDrawable> shapes, ILGroup root, ILBSPTreeSettings settings = null) {
        //    ILBSPTree<ILDrawable> ret = new ILBSPTree<ILDrawable>();
        //    ret.m_settings = settings ?? new ILBSPTreeSettings();

        //    if (shapes.Count() == 0) return new ILBSPTree<ILDrawable>();

        //    List<BSPPrimitive<ILDrawable>> primitives = new List<BSPPrimitive<ILDrawable>>();

        //    Stack<Matrix4> transformStack = new Stack<Matrix4>();
        //    foreach (var shape in shapes) {
        //        //shape.Detach(); 
        //        // acquire and transform to root coords
        //        ILGroup parent = shape.Parent;
        //        transformStack.Clear();
        //        transformStack.Push(Matrix4.Identity);

        //        while (parent != null) {
        //            transformStack.Push(parent.Transform);
        //            if (parent.ID == root.ID) break;
        //            parent = parent.Parent;
        //        }
        //        Matrix4 transform = transformStack.Pop();
        //        while (transformStack.Count > 0) {
        //            transform = transform * transformStack.Pop();
        //        }
        //        // collect all primitives 
        //        shape.ToBSPBuildPrimitives(primitives, transform, shape);
        //    }
        //    // create the BSP tree
        //    ret.Root = new ILBSPNode<ILDrawable>();
        //    ret.Create(ret.Root, primitives);
        //    return ret;
        //}
        //public static ILBSPTree<ILPreprocessedShape> CreateFromRenderStates(IEnumerable<ILPreprocessedShape> states, ILBSPTreeSettings settings = null) {
        //    if (states.Count() == 0) return new ILBSPTree<ILPreprocessedShape>();
        //    ILBSPTree<ILPreprocessedShape> ret = new ILBSPTree<ILPreprocessedShape>();
        //    ret.m_settings = settings ?? new ILBSPTreeSettings();

        //    List<BSPPrimitive<ILPreprocessedShape>> primitives = new List<BSPPrimitive<ILPreprocessedShape>>();

        //    Stack<Matrix4> transformStack = new Stack<Matrix4>();
        //    foreach (var state in states) {
        //        //shape.Detach(); 
        //        // acquire and transform to camera coords
        //        Matrix4 transform = state.Model2CameraTransform;
        //        // collect all primitives 
        //        state.Node.ToBSPBuildPrimitives(primitives, transform, state);
        //    }
        //    // create the BSP tree
        //    ret.Root = new ILBSPNode<ILPreprocessedShape>();
        //    ret.Create(ret.Root, primitives);
        //    return ret;
        //}
        #endregion
        #endregion

        #region public interface
        /// <summary>
        /// Retrieve all primitives from the BSP tree, spatially sorted order; for use in foreach loops
        /// </summary>
        /// <param name="cameraPosition">Current camera position</param>
        /// <param name="back2Front">[optional] Flag determining the sort order: back to front (default) or front to back</param>
        /// <returns>individual sorted primitives</returns>
        public IEnumerable<BSPPrimitive<T>> GetSorted(Vector3 cameraPosition, bool back2Front = true) {
            if (Root == null) yield break; 
            Stack<ILBSPNode<T>> stack = new Stack<ILBSPNode<T>>(Root.Count() / 2 );
            ILBSPNode<T> root = Root;
            bool done = false; 
            while (!done) {
                if (root != null) {
                    int pos = LocationPointToPlane(cameraPosition, root.Plane);
                    stack.Push(root);
                    // determine, which node is the "far" direction: compare eye vector to 
                    // plane normal: 
//visit the 'left' node (far for back -> front traversal)
                    bool commonOrder = back2Front;
                    if (pos == BACK_POINT) commonOrder = !commonOrder; 
                    //if (root.Plane.Z < 0) commonOrder = !commonOrder; 
                    if (commonOrder) root = root.Back; 
                    else root = root.Front; 
                } else {
                    if (stack.Count > 0) {
                        root = stack.Pop();
                        foreach (var p in root.Primitives.Where(p => !(p.Data is ILLines))) {
                            yield return p;
                        }
                        foreach (var p in root.Primitives.Where(p => (p.Data is ILLines))) {
                            yield return p;
                        }
                        int pos = LocationPointToPlane(cameraPosition, root.Plane);
                        bool commonOrder = back2Front;
                        if (pos == BACK_POINT) commonOrder = !commonOrder;
                        //if (root.Plane.Z < 0) commonOrder = !commonOrder;
                        if (commonOrder) root = root.Front;
                        else root = root.Back;
                    } else {
                        done = true; 
                    }
                }
            }
        }
        #endregion

        #region private helpers
        private void Create(ILBSPNode<T> root, List<BSPPrimitive<T>> primitives) {
            if (primitives.Count == 0 || root == null) return;
            var stack = new Stack<Tuple<ILBSPNode<T>, List<BSPPrimitive<T>>>>();
            while (true) { //root != null || ) {
                if (primitives.Count == 0 || root == null) {
                    if (stack.Count > 0) {
                        var next = stack.Pop(); 
                        root = next.Item1;
                        primitives = next.Item2; 
                    } else {
                        break; 
                    }
                }
                var plane = PickPlane(primitives);
                var fronts = new List<BSPPrimitive<T>>(primitives.Count / 2);
                var backs = new List<BSPPrimitive<T>>(primitives.Count / 2);
                var copla = new List<BSPPrimitive<T>>();
                foreach (var p in primitives) {
                    switch (ClassifyPolygonToPlane(p, plane)) {
                        case COPLA_PRIMITIVE:
                            copla.Add(p);
                            break;
                        case FRONT_PRIMITIVE:
                            fronts.Add(p);
                            break;
                        case BACK_PRIMITIVE:
                            backs.Add(p);
                            break;
                        case SPLIT_PRIMITIVE:
                            SplitPrimitive(p, plane, fronts, backs);
                            break;
                    }
                }
                if (copla.Count == 0) {
                    // floating point inaccuracy may lead to all vertices falling out of the plane. 
                    // -> increase plane thickness 
                    m_settings.PlaneThickness *= 2f; 
                    continue; 
                }
                root.Front = null;
                root.Back = null;
                root.Primitives = copla;
                root.Plane = plane;
                if (fronts.Count != 0) {
                    root.Front = new ILBSPNode<T>(); 
                }
                if (backs.Count != 0) {
                    root.Back = new ILBSPNode<T>(); 
                    stack.Push(Tuple.Create(root.Back, backs));
                }
                root = root.Front;
                primitives = fronts; 
            } 
        }
        private Vertex[] dummyFronts = new Vertex[4]; // <- these dummies obviously make it not thread safe ... !
        private Vertex[] dummyBacks = new Vertex[4];
        private void SplitPrimitive(BSPPrimitive<T> prim, Vector4 plane, List<BSPPrimitive<T>> fronts, List<BSPPrimitive<T>> backs) {
            /* Note: the whole algorithm is well prepared to handle arbitrary polygons. However, we only need lines and triangles. */
            int numFront = 0, numBack = 0;
            Vector3 a = prim.Positions[prim.VertexCount - 1];
            int aSide = LocationPointToPlane(a, plane);
            if (prim.VertexCount == 2) {
                float t;
                Vertex newVert = new Vertex();
                Vector3 v2 = prim.Positions[0];
                if (aSide == FRONT_POINT) {
                    newVert.XYZ = ILHelper.ComputeNewVertex(a, v2, plane, out t);
                    prim.Interpolate(1, 0, t, ref newVert);
                    dummyFronts[numFront++] = prim.GetVertexAt(1);
                    dummyBacks[numBack++] = prim.GetVertexAt(0);
                } else {
                    newVert.XYZ = ILHelper.ComputeNewVertex(v2, a, plane, out t);
                    prim.Interpolate(0, 1, t, ref newVert);
                    dummyFronts[numFront++] = prim.GetVertexAt(0);
                    dummyBacks[numBack++] = prim.GetVertexAt(1);
                }
                
                //System.Diagnostics.Debug.Assert(LocationPointToPlane(newVert.XYZ, plane) == ON_POINT);
                dummyFronts[numFront++] = dummyBacks[numBack++] = newVert;
            } else {
                // Loop over all edges given by vertex pair (n - 1, n)
                for (int n = 0; n < prim.VertexCount; n++) {
                    int ai = n - 1;
                    if (ai < 0) ai = prim.VertexCount - 1;
                    Vector3 b = prim.Positions[n];
                    int bSide = LocationPointToPlane(b, plane);
                    if (bSide == FRONT_POINT) {
                        if (aSide == BACK_POINT) {
                            // split the edge - create new Vertex3 data for crossing point
                            float t;
                            Vertex newVert = new Vertex();
                            newVert.XYZ = ILHelper.ComputeNewVertex(b, a, plane, out t);
                            //System.Diagnostics.Debug.Assert(LocationPointToPlane(newVert.XYZ, plane) == ON_POINT);
                            prim.Interpolate(n, ai, t, ref newVert);
                            dummyFronts[numFront++] = dummyBacks[numBack++] = newVert;
                        }
                        // In all three cases, output b to the front side
                        dummyFronts[numFront++] = prim.GetVertexAt(n);
                    } else if (bSide == BACK_POINT) {
                        if (aSide == FRONT_POINT) {
                            float t;
                            Vertex newVert = new Vertex();
                            newVert.XYZ = ILHelper.ComputeNewVertex(a, b, plane, out t);
                            //System.Diagnostics.Debug.Assert(LocationPointToPlane(newVert.XYZ, plane) == ON_POINT);
                            prim.Interpolate(ai, n, t, ref newVert);
                            dummyFronts[numFront++] = dummyBacks[numBack++] = newVert;
                        } else if (aSide == ON_POINT) {
                            // Output a when edge (a, b) goes from ‘on’ to ‘behind’ plane
                            dummyBacks[numBack++] = prim.GetVertexAt(ai);
                        }
                        // In all three cases, output b to the back side
                        dummyBacks[numBack++] = prim.GetVertexAt(n);
                    } else {
                        // b is on the plane. In all three cases output b to the front side
                        dummyFronts[numFront++] = prim.GetVertexAt(n);
                        // In one case, also output b to back side
                        if (aSide == BACK_POINT)
                            dummyBacks[numBack++] = prim.GetVertexAt(n);
                    }
                    // Keep b as the starting point of the next edge
                    a = b;
                    aSide = bSide;
                }
            }
            // Analyse how many primitives where created (triangles may create 3 others when splitted
            ComposePrimitives(numFront, dummyFronts, fronts, prim);
            ComposePrimitives(numBack, dummyBacks, backs, prim);
        }
        private void ComposePrimitives(int num, Vertex[] tempStorage, List<BSPPrimitive<T>> target, BSPPrimitive<T> source) {
            System.Diagnostics.Debug.Assert(num > 1 && num < 5);  // right now we only handle plain triangles! 
            switch (num) {
                case 2: // a new line was born
                    BSPPrimitive<T> p = new BSPPrimitive<T>(); 
                    p.Positions = new Vector3[2] { tempStorage[0].XYZ, tempStorage[1].XYZ };
                    if (source.Colors != null) {
                        p.Colors = new Vector4[2] { tempStorage[0].Colors, tempStorage[1].Colors }; 
                    }
                    if (source.Normals != null) {
                        p.Normals = new Vector3[2] { tempStorage[0].Normals, tempStorage[1].Normals }; 
                    }
                    if (source.CameraPos != null) {
                        p.CameraPos = new Vector3[2] { tempStorage[0].CameraPos, tempStorage[1].CameraPos }; 
                    }
                    p.Data = source.Data; 
                    // line offset 
                    float dx = p.Positions[0].X - p.Positions[1].X;
                    float dy = p.Positions[0].Y - p.Positions[1].Y;
                    p.LineOffset = source.LineOffset - (float)Math.Sqrt(dx * dx + dy * dy); 
                    target.Add(p); 
                    break; 
                case 3: // cutted triangle peak
                    p = new BSPPrimitive<T>();
                    p.Positions = new Vector3[3] { tempStorage[0].XYZ, tempStorage[1].XYZ, tempStorage[2].XYZ };
                    if (source.Colors != null) {
                        p.Colors = new Vector4[3] { tempStorage[0].Colors, tempStorage[1].Colors, tempStorage[2].Colors }; 
                    }
                    if (source.Normals != null) {
                        p.Normals = new Vector3[3] { tempStorage[0].Normals, tempStorage[1].Normals, tempStorage[2].Normals }; 
                    }
                    if (source.CameraPos != null) {
                        p.CameraPos = new Vector3[3] { tempStorage[0].CameraPos, tempStorage[1].CameraPos, tempStorage[2].CameraPos }; 
                    }
                    p.Data = source.Data; 
//#if DEBUG
//                    p.ID = source.ID; 
//#endif 
                    target.Add(p); 
                    break; 
                case 4:
                    p = new BSPPrimitive<T>();
                    p.Positions = new Vector3[3] { tempStorage[0].XYZ, tempStorage[1].XYZ, tempStorage[2].XYZ };
                    if (source.Colors != null) {
                        p.Colors = new Vector4[3] { tempStorage[0].Colors, tempStorage[1].Colors, tempStorage[2].Colors };
                    }
                    if (source.Normals != null) {
                        p.Normals = new Vector3[3] { tempStorage[0].Normals, tempStorage[1].Normals, tempStorage[2].Normals };
                    }
                    if (source.CameraPos != null) {
                        p.CameraPos = new Vector3[3] { tempStorage[0].CameraPos, tempStorage[1].CameraPos, tempStorage[2].CameraPos }; 
                    }
                    p.Data = source.Data; 
//#if DEBUG
//                    p.ID = source.ID; 
//#endif 
                    target.Add(p);
                    p = new BSPPrimitive<T>();
                    p.Positions = new Vector3[3] { tempStorage[2].XYZ, tempStorage[3].XYZ, tempStorage[0].XYZ };
                    if (source.Colors != null) {
                        p.Colors = new Vector4[3] { tempStorage[2].Colors, tempStorage[3].Colors, tempStorage[0].Colors };
                    }
                    if (source.Normals != null) {
                        p.Normals = new Vector3[3] { tempStorage[2].Normals, tempStorage[3].Normals, tempStorage[0].Normals };
                    }
                    if (source.CameraPos != null) {
                        p.CameraPos = new Vector3[3] { tempStorage[2].CameraPos, tempStorage[3].CameraPos, tempStorage[0].CameraPos }; 
                    }
                    p.Data = source.Data; 
//#if DEBUG
//                    p.ID = source.ID; 
//#endif 
                    target.Add(p);
                    break;
            }
        }
        private int ClassifyPolygonToPlane(BSPPrimitive<T> p, Vector4 plane) {
            int numfront = 0, numBack = 0;
            for (int i = p.VertexCount; i --> 0;) {
                switch (LocationPointToPlane(p.Positions[i], plane)) {
                    case BACK_POINT: 
                        numBack++; 
                        break; 
                    case FRONT_POINT: 
                        numfront++; 
                        break; 
                }
            }
            if (numfront > 0 && numBack > 0) {
                return SPLIT_PRIMITIVE; 
            }
            if (numfront > 0) return FRONT_PRIMITIVE; 
            if (numBack > 0) return BACK_PRIMITIVE; 
            return COPLA_PRIMITIVE; 
        }
        private int LocationPointToPlane(Vector3 point, Vector4 plane) {
            float k = // Vector4.Dot(plane, point);
                    plane.X * point.X + plane.Y * point.Y + plane.Z * point.Z + plane.W;
            if (k > m_settings.PlaneThickness) {
                return 1;
            } else if (k < -m_settings.PlaneThickness) {
                return -1;
            } else {
                return 0; 
            }
        }
        private Vector4 PickPlane(List<BSPPrimitive<T>> primitives) {
            if (primitives.Count == 1) return primitives[0].GetPlane(); 
            bool fast = false;
            int primCount = primitives.Count; 
            if (primitives.Count > 50 && m_settings.Hint == BSPTreeHint.FastCreation) {
                // pick 5 random planes, take best 
                fast = true;
                primCount = 5; 
            }
            
            Vector4 bestPlane = new Vector4(0,0,1,0);
            float bestScore = float.MaxValue;

            for (int i = 0; i < primCount; i++) {
                int numFront = 0, numBack = 0, numSplit = 0;
                int primId = i; 
                if (fast) {
                    primId = Random.Next(50);
                }
                Vector4 plane = primitives[primId].GetPlane();
                for (int j = primitives.Count; j-- > 0; ) {
                    
                    if (i == j) continue;
                    // we do not only count split primitives, but penalize _edge splits_
                    BSPPrimitive<T> prim = primitives[j]; 
                    switch (ClassifyPolygonToPlane(prim , plane)) {
                        case FRONT_PRIMITIVE:
                            numFront++;
                            break;
                        case BACK_PRIMITIVE:
                            numBack++;
                            break;
                        case SPLIT_PRIMITIVE:
                            numSplit += prim.VertexCount;
                            break;
                    }
                }
                // Compute score as a weighted combination (based on K, with K in range
                // 0..1) between balance and splits (lower score is better)
                float score = m_settings.BalanceSplitRatio * numSplit + (1.0f - m_settings.BalanceSplitRatio) * Math.Abs(numFront - numBack);
                if (score < bestScore) {
                    bestScore = score;
                    bestPlane = plane;
                }
            }
            return bestPlane;
        }
        #endregion

    }
}
