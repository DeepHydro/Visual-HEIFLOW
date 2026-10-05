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
    public static class Shapes {

        #region circles
        private static ILCircle s_circle10;
        private static ILCircle s_circle50;
        private static ILCircle s_circle100;
        private static ILCircle s_circle200;
        /// <summary>
        /// Gets a disc as triangle fan shape, 50 points resolution, single colored: gray
        /// </summary>
        public static ILTrianglesFan Disc50 {
            get {
                if (s_circle50 == null) {
                    s_circle50 = new ILCircle(50);
                }
                return (ILTrianglesFan)s_circle50.Fill.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a disc as triangle fan shape, 10 points resolution, single colored: gray
        /// </summary>
        public static ILTrianglesFan Disc10 {
            get {
                if (s_circle10 == null) {
                    s_circle10 = new ILCircle(10);
                }
                return (ILTrianglesFan)s_circle10.Fill.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a disc as triangle fan shape, 200 points resolution, single colored: gray
        /// </summary>
        public static ILTrianglesFan Disc200 {
            get {
                if (s_circle200 == null) {
                    s_circle200 = new ILCircle(200);
                }
                return (ILTrianglesFan)s_circle200.Fill.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a disc as triangle fan shape, 100 points resolution, single colored: gray
        /// </summary>
        public static ILTrianglesFan Disc100 {
            get {
                if (s_circle100 == null) {
                    s_circle100 = new ILCircle(100);
                }
                return (ILTrianglesFan)s_circle100.Fill.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a circle as line strip shape, 10 points resolution, single colored: black
        /// </summary>
        public static ILLineStrip Circle10 {
            get {
                if (s_circle10 == null) {
                    s_circle10 = new ILCircle(10);
                }
                return (ILLineStrip)s_circle10.Border.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a circle as line strip shape, 50 points resolution, single colored: black
        /// </summary>
        public static ILLineStrip Circle50 {
            get {
                if (s_circle50 == null) {
                    s_circle50 = new ILCircle(50);
                }
                return (ILLineStrip)s_circle50.Border.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a circle as line strip shape, 100 points resolution, single colored: black
        /// </summary>
        public static ILLineStrip Circle100 {
            get {
                if (s_circle100 == null) {
                    s_circle100 = new ILCircle(100);
                }
                return (ILLineStrip)s_circle100.Border.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a circle as line strip shape, 100 points resolution, single colored: black
        /// </summary>
        public static ILLineStrip Circle200 {
            get {
                if (s_circle200 == null) {
                    s_circle200 = new ILCircle(200);
                }
                return (ILLineStrip)s_circle200.Border.Copy().Detach();
            }
        }
        #endregion

        #region Spheres
        private static ILSphere s_sphere5;
        private static ILSphere s_sphere4;
        private static ILSphere s_sphere3;
        private static ILSphere s_sphere2;
        private static ILSphere s_hemisphere;

        /// <summary>
        /// Get a hemisphere wireframe lines shape, single colored: green
        /// </summary>
        public static ILLines HemisphereWireframe {
            get {
                if (s_hemisphere == null) {
                    s_hemisphere = new ILSphere(resolution: 4);
                    using (ILScope.Enter()) {
                        ILArray<float> pos = s_hemisphere.Fill.Positions.Storage; 
                        pos[1,pos[1,":"] > 0] = 0; 
                        s_hemisphere.Fill.Positions.Update(pos); 
                    }
                }
                return (ILLines)s_hemisphere.Wireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Get a hemisphere triangles shape, single colored: green, lit
        /// </summary>
        public static ILTriangles Hemisphere {
            get {
                if (s_hemisphere == null) {
                    s_hemisphere = new ILSphere(resolution: 4);
                    using (ILScope.Enter()) {
                        ILArray<float> pos = s_hemisphere.Fill.Positions.Storage;
                        pos[1, pos[1, ":"] > 0] = 0;
                        s_hemisphere.Fill.Positions.Update(pos);
                        s_hemisphere.Configure(); 
                    }
                }
                return (ILTriangles)s_hemisphere.Fill.Copy().Detach();
            }
        }

        /// <summary>
        /// Get a detailed sphere wireframe lines shape, single colored: green
        /// </summary>
        public static ILLines Sphere5Wireframe {
            get {
                if (s_sphere5 == null) {
                    s_sphere5 = new ILSphere(resolution: 5);
                }
                return (ILLines)s_sphere5.Wireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Get a regular sphere wireframe lines shape, single colored: green
        /// </summary>
        public static ILLines SphereWireframe {
            get {
                if (s_sphere4 == null) {
                    s_sphere4 = new ILSphere(resolution: 4);
                }
                return (ILLines)s_sphere4.Wireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Get a rougher sphere wireframe lines shape, single colored: green
        /// </summary>
        public static ILLines Sphere3Wireframe {
            get {
                if (s_sphere3 == null) {
                    s_sphere3 = new ILSphere(resolution: 3);
                }
                return (ILLines)s_sphere3.Wireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Get a rough sphere wireframe lines shape, single colored: green
        /// </summary>
        public static ILLines Sphere2Wireframe {
            get {
                if (s_sphere2 == null) {
                    s_sphere2 = new ILSphere(resolution: 2);
                }
                return (ILLines)s_sphere2.Wireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Get a detailed sphere triangles shape, single colored: green, lit
        /// </summary>
        public static ILTriangles Sphere5 {
            get {
                if (s_sphere5 == null) {
                    s_sphere5 = new ILSphere(resolution: 5);
                    s_sphere5.Configure(); 
                }
                return (ILTriangles)s_sphere5.Fill.Copy().Detach();
            }
        }
        /// <summary>
        /// Get a regular sphere triangles shape, single colored: green, lit
        /// </summary>
        public static ILTriangles Sphere {
            get {
                if (s_sphere4 == null) {
                    s_sphere4 = new ILSphere(resolution: 4);
                    s_sphere4.Configure(); 
                }
                return (ILTriangles)s_sphere4.Fill.Copy().Detach();
            }
        }
        /// <summary>
        /// Get a rougher sphere triangles shape, single colored: green, lit
        /// </summary>
        public static ILTriangles Sphere3 {
            get {
                if (s_sphere3 == null) {
                    s_sphere3 = new ILSphere(resolution: 3);
                    s_sphere3.Configure(); 
                }
                return (ILTriangles)s_sphere3.Fill.Copy().Detach();
            }
        }
        /// <summary>
        /// Get a rough sphere triangles shape, single colored: green, lit
        /// </summary>
        public static ILTriangles Sphere2 {
            get {
                if (s_sphere2 == null) {
                    s_sphere2 = new ILSphere(resolution: 2);
                    s_sphere2.Configure();
                }
                return (ILTriangles)s_sphere2.Fill.Copy().Detach();
            }
        }
        #endregion

        #region Points
        private static ILPoints s_point;
        /// <summary>
        /// Get single point shape at (0,0,0), single colored: red
        /// </summary>
        public static ILPoints Point {
            get {
                if (s_point == null) {
                    s_point = new ILPoints();
                    s_point.Color = Color.Red; 
                    s_point.AutoNormals = false;
                    s_point.Positions.Update(0, 1, ILMath.zeros<float>(3, 1)); 
                }
                return (ILPoints)s_point.Copy().Detach(); 
            } 
        }
        private static ILLines s_line;
        /// <summary>
        /// Get single line shape at (0,0,0) -> (1,1,1), single colored: black
        /// </summary>
        public static ILLines Line {
            get {
                if (s_line == null) {
                    s_line = new ILLines();
                    s_line.Color = Color.Black;
                    s_line.AutoNormals = false;
                    s_line.Positions.Update(0, 2, new float[,] {
                        {0,0,0},
                        {1,1,1},
                    }); 
                }
                return (ILLines)s_line.Copy().Detach(); 
            } 
        }
        #endregion

        #region Triangles
        private static ILTriangles s_triangleIcosceles; 
        private static ILTriangles s_triangleEquilateral;  
        private static ILTriangles s_triangleRight;  
        private static ILTriangles s_triangleInterp; 
        private static ILTriangles s_triangleEquilateralInterp;  
        private static ILTriangles s_triangleRightInterp;  
        private static ILTriangles s_triangleIcoscelesLit; 
        private static ILTriangles s_triangleEquilateralLit;  
        private static ILTriangles s_triangleRightLit;  
        private static ILTriangles s_triangleInterpLit; 
        private static ILTriangles s_triangleEquilateralInterpLit;  
        private static ILTriangles s_triangleRightInterpLit;
        private static ILLines s_triangleWireframe;
        private static ILLines s_triangleEquilateralWireframe;

        /// <summary>
        /// Gets isosceles triangle wireframe as line strip, single colored: black
        /// </summary>
        public static ILLineStrip TriangleWireframe {
            get {
                if (s_triangleWireframe == null) {
                    s_triangleWireframe = new ILLineStrip("TriangleWireframe");
                    s_triangleWireframe.Positions = Triangle.Positions;
                    s_triangleWireframe.Indices.Update(new int[] { 0,1,2,0} ); 
                    s_triangleWireframe.Color = Color.Black;
                }
                return (ILLineStrip)s_triangleWireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets equilateral triangle wireframe as line strip, single colored: black
        /// </summary>
        public static ILLineStrip TriangleEquilateralWireframe {
            get {
                if (s_triangleEquilateralWireframe == null) {
                    s_triangleEquilateralWireframe = new ILLineStrip("TriangleEquilateralWireframe");
                    s_triangleEquilateralWireframe.Positions = TriangleEquilateral.Positions;
                    s_triangleEquilateralWireframe.Indices.Update(new int[] { 0, 1, 2, 0 });
                    s_triangleEquilateralWireframe.Color = Color.Black;
                }
                return (ILLineStrip)s_triangleEquilateralWireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets isosceles triangle, single colored: blue, unlit
        /// </summary>
        public static ILTriangles Triangle {
            get {
                if (s_triangleIcosceles == null) {
                    s_triangleIcosceles = new ILTriangles("Triangle");
                    s_triangleIcosceles.Colors.Update(null);
                    s_triangleIcosceles.Color = Color.Blue;
                    s_triangleIcosceles.Positions.Update(new float[,] {
                        {-1,-1, 0},
                        { 0, 1, 0},
                        { 1,-1, 0}
                    });
                    //Colors.Update(new float[,] {
                    //    {1,0,0,1},
                    //    {0,1,0,1},
                    //    {0,0,1,1}
                    //}); 
                    s_triangleIcosceles.AutoNormals = false;
                }
                return (ILTriangles)s_triangleIcosceles.Copy().Detach();
            }
        }
        /// <summary>
        /// Get equilateral (even sided) triangle, single colored: blue, unlit
        /// </summary>
        public static ILTriangles TriangleEquilateral  {
            get {
                if (s_triangleEquilateral == null) {
                    s_triangleEquilateral = new ILTriangles("TriangleEquilateral"); 
                    s_triangleEquilateral.Colors.Update(null); 
                    s_triangleEquilateral.Color = Color.Blue;
                    float cos30 = (float)Math.Cos(Math.PI / 6);
                    float sin60 = (float)Math.Sin(Math.PI / 6); 
                    s_triangleEquilateral.Positions.Update(new float[,] {
                        {-cos30,-sin60, 0},
                        { 0, 1, 0},
                        { cos30,-sin60, 0}
                    }); 
                    //Colors.Update(new float[,] {
                    //    {1,0,0,1},
                    //    {0,1,0,1},
                    //    {0,0,1,1}
                    //}); 
                    s_triangleEquilateral.AutoNormals = false; 
                }
                return (ILTriangles)s_triangleEquilateral.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Get isosceles triangle, right angle at origin, single colored: blue, unlit
        /// </summary>
        public static ILTriangles TriangleRight  {
            get {
                if (s_triangleRight == null) {
                    s_triangleRight = new ILTriangles("TriangleRight"); 
                    s_triangleRight.Colors.Update(null); 
                    s_triangleRight.Color = Color.Blue; 
                    s_triangleRight.Positions.Update(new float[,] {
                        { 0, 0, 0},
                        { 0, 1, 0},
                        { 1, 0, 0}
                    }); 
                    //Colors.Update(new float[,] {
                    //    {1,0,0,1},
                    //    {0,1,0,1},
                    //    {0,0,1,1}
                    //}); 
                    s_triangleRight.AutoNormals = false; 
                }
                return (ILTriangles)s_triangleRight.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Gets isosceles triangle, individual colors, unlit
        /// </summary>
        public static ILTriangles TriangleInterp {
            get {
                if (s_triangleInterp == null) {
                    s_triangleInterp = new ILTriangles("TriangleInterp"); 
                    s_triangleInterp.Colors.Update(null);
                    s_triangleInterp.Color = null; 
                    s_triangleInterp.Positions.Update(new float[,] {
                        {-1,-1, 0},
                        { 0, 1, 0},
                        { 1,-1, 0}
                    }); 
                    s_triangleInterp.Colors.Update(new float[,] {
                        {1,0,0,1},
                        {0,1,0,1},
                        {0,0,1,1}
                    }); 
                    s_triangleInterp.AutoNormals = false; 
                }
                return (ILTriangles)s_triangleInterp.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Get equilateral (even sided) triangle, individual colors, unlit
        /// </summary>
        public static ILTriangles TriangleEquilateralInterp  {
            get {
                if (s_triangleEquilateralInterp == null) {
                    s_triangleEquilateralInterp = new ILTriangles("TriangleEquilateralInterp"); 
                    s_triangleEquilateralInterp.Color = null; 
                    s_triangleEquilateralInterp.Positions = TriangleEquilateral.Positions;  
                    s_triangleEquilateralInterp.Colors.Update(new float[,] {
                        {1,0,0,1},
                        {0,1,0,1},
                        {0,0,1,1}
                    }); 
                    s_triangleEquilateralInterp.AutoNormals = false; 
                    s_triangleEquilateralInterp.Normals.Update(null);
                }
                return (ILTriangles)s_triangleEquilateralInterp.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Get isosceles triangle, right angle at origin, individual colors, unlit
        /// </summary>
        public static ILTriangles TriangleRightInterp  {
            get {
                if (s_triangleRightInterp == null) {
                    s_triangleRightInterp = new ILTriangles("TriangleRightInterp"); 
                    s_triangleRightInterp.Colors.Update(null); 
                    s_triangleRightInterp.Color = null; 
                    s_triangleRightInterp.Positions.Update(new float[,] {
                        { 0, 0, 0},
                        { 0, 1, 0},
                        { 1, 0, 0}
                    }); 
                    s_triangleRightInterp.Colors.Update(new float[,] {
                        {1,0,0,1},
                        {0,1,0,1},
                        {0,0,1,1}
                    });
                    s_triangleRightInterp.AutoNormals = false; 
                }
                return (ILTriangles)s_triangleRightInterp.Copy().Detach(); 
            }
        }

        /// <summary>
        /// Gets isosceles triangle, single colored: blue, lit
        /// </summary>
        public static ILTriangles TriangleLit {
            get {
                if (s_triangleIcoscelesLit == null) {
                    s_triangleIcoscelesLit = new ILTriangles("TriangleLit"); 
                    s_triangleIcoscelesLit.Colors.Update(null); 
                    s_triangleIcoscelesLit.Color = Color.Blue; 
                    s_triangleIcoscelesLit.Positions.Update(new float[,] {
                        {-1,-1, 0},
                        { 0, 1, 0},
                        { 1,-1, 0}
                    }); 
                    s_triangleIcoscelesLit.Configure(); 
                }
                return (ILTriangles)s_triangleIcoscelesLit.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Get equilateral (even sided) triangle, single colored: blue, lit
        /// </summary>
        public static ILTriangles TriangleEquilateralLit  {
            get {
                if (s_triangleEquilateralLit == null) {
                    s_triangleEquilateralLit = new ILTriangles("TriangleEquilateralLit"); 
                    s_triangleEquilateralLit.Colors.Update(null); 
                    s_triangleEquilateralLit.Color = Color.Blue; 
                    float cos45 = (float)Math.Cos(Math.PI / 4); 
                    s_triangleEquilateralLit.Positions = TriangleEquilateral.Positions;
                    s_triangleEquilateralLit.Configure(); 
                }
                return (ILTriangles)s_triangleEquilateralLit.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Get isosceles triangle, right angle at origin, single colored: blue, lit
        /// </summary>
        public static ILTriangles TriangleRightLit  {
            get {
                if (s_triangleRightLit == null) {
                    s_triangleRightLit = new ILTriangles("TriangleRightLit"); 
                    s_triangleRightLit.Colors.Update(null); 
                    s_triangleRightLit.Color = Color.Blue; 
                    s_triangleRightLit.Positions.Update(new float[,] {
                        { 0, 0, 0},
                        { 0, 1, 0},
                        { 1, 0, 0}
                    }); 
                    s_triangleRightLit.Configure(); 
                }
                return (ILTriangles)s_triangleRightLit.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Gets isosceles triangle, individual colors, lit
        /// </summary>
        public static ILTriangles TriangleInterpLit {
            get {
                if (s_triangleInterpLit == null) {
                    s_triangleInterpLit = new ILTriangles("TriangleInterpLit"); 
                    s_triangleInterpLit.Colors.Update(null); 
                    s_triangleInterpLit.Color = null; 
                    s_triangleInterpLit.Positions.Update(new float[,] {
                        {-1,-1, 0},
                        { 0, 1, 0},
                        { 1,-1, 0}
                    }); 
                    s_triangleInterpLit.Colors.Update(new float[,] {
                        {1,0,0,1},
                        {0,1,0,1},
                        {0,0,1,1}
                    });
                    s_triangleInterpLit.Configure();
                }
                return (ILTriangles)s_triangleInterpLit.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Get equilateral (even sided) triangle, individual colors, lit
        /// </summary>
        public static ILTriangles TriangleEquilateralInterpLit  {
            get {
                if (s_triangleEquilateralInterpLit == null) {
                    s_triangleEquilateralInterpLit = new ILTriangles("TriangleEquilateralInterpLit"); 
                    s_triangleEquilateralInterpLit.Colors.Update(null); 
                    s_triangleEquilateralInterpLit.Color = null; 
                    float cos45 = (float)Math.Cos(Math.PI / 4); 
                    s_triangleEquilateralInterpLit.Positions = TriangleEquilateral.Positions; 
                    s_triangleEquilateralInterpLit.Colors.Update(new float[,] {
                        {1,0,0,1},
                        {0,1,0,1},
                        {0,0,1,1}
                    });
                    s_triangleEquilateralInterpLit.Configure();
                }
                return (ILTriangles)s_triangleEquilateralInterpLit.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Get isosceles triangle, right angle at origin, individual colors, lit
        /// </summary>
        public static ILTriangles TriangleRightInterpLit  {
            get {
                if (s_triangleRightInterpLit == null) {
                    s_triangleRightInterpLit = new ILTriangles("TriangleRightInterpLit"); 
                    s_triangleRightInterpLit.Colors.Update(null); 
                    s_triangleRightInterpLit.Color = null; 
                    s_triangleRightInterpLit.Positions.Update(new float[,] {
                        { 0, 0, 0},
                        { 0, 1, 0},
                        { 1, 0, 0}
                    }); 
                    s_triangleRightInterpLit.Colors.Update(new float[,] {
                        {1,0,0,1},
                        {0,1,0,1},
                        {0,0,1,1}
                    });
                    s_triangleRightInterpLit.Configure();
                }
                return (ILTriangles)s_triangleRightInterpLit.Copy().Detach(); 
            }
        }

        #endregion

        #region UnitCubes
        private static ILLines s_unitCubeWireframe; 
        private static ILTriangles s_unitCubeFilled; 
        private static ILTriangles s_unitCubeFilledLit; 

        /// <summary>
        /// Create unit cube wireframe: (0,0,0) -> (1,1,1)
        /// </summary>
        public static ILLines UnitCubeWireframe {
            get {
                if (s_unitCubeWireframe == null) {
                    s_unitCubeWireframe = new ILLines("UnitCubeWireframe");
                    s_unitCubeWireframe.Positions = ILPositionsBuffer.UnitCube;
                    s_unitCubeWireframe.Indices = ILIndicesBuffer.UnitCube;
                }
                return (ILLines)s_unitCubeWireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Create filled unit cube: (0,0,0) -> (1,1,1) (shared vertices)
        /// </summary>
        public static ILTriangles UnitCubeFilled {
            get {
                if (s_unitCubeFilled == null) { 
                    s_unitCubeFilled = new ILTriangles("UnitCubeFilled");
                    s_unitCubeFilled.Positions = ILPositionsBuffer.UnitCube;
                    s_unitCubeFilled.Indices.Update(new int[] {
                     0,3,1,1,3,2,
                     3,7,2,2,7,6,   
                     7,4,6,6,4,5,
                     4,0,5,5,0,1,
                     1,2,5,5,2,6,
                     4,7,0,0,7,3
                    });
                    s_unitCubeFilled.Color = System.Drawing.Color.Green;
                    s_unitCubeFilled.Colors.Update(null);
                }
                return (ILTriangles)s_unitCubeFilled.Copy().Detach();
            }
        }
        /// <summary>
        /// Create filled unit cube: (0,0,0) -> (1,1,1) (optimized for lighting)
        /// </summary>
        public static ILTriangles UnitCubeFilledLit {
            get {
                if (s_unitCubeFilledLit == null) { 
                    s_unitCubeFilledLit = new ILTriangles("UnitCubeFilledLit");
                    s_unitCubeFilledLit.Positions = ILPositionsBuffer.UnitCubeLighting; 
                    s_unitCubeFilledLit.Indices = ILIndicesBuffer.UnitCubeLighting; 
                    s_unitCubeFilledLit.Color = System.Drawing.Color.Green;
                    s_unitCubeFilledLit.Colors.Update(null);
                    s_unitCubeFilledLit.Configure();
                }
                return (ILTriangles)s_unitCubeFilledLit.Copy().Detach();
            }
        }
        #endregion

        #region Rectangles
        private static ILLines s_rectangleWireframe;
        private static ILTriangles s_rectangleFilled; 
        
        /// <summary>
        /// Create rectangle in XY plane: (0,0),(0,1),(1,1),(1,0)
        /// </summary>
        public static ILLines RectangleWireframe {
            get {
                if (s_rectangleWireframe == null) {
                    s_rectangleWireframe = new ILLineStrip("RectangleWireframe");
                    s_rectangleWireframe.Positions = ILPositionsBuffer.UnitCube;
                    s_rectangleWireframe.Indices.Update(new int[] { 0, 1, 2, 3, 0 });
                }
                return (ILLines)s_rectangleWireframe.Copy().Detach();
            }
        }
        /// <summary>
        /// Create filled rectangle in XY plane: (0,0) -> (1,1)
        /// </summary>
        public static ILTriangles RectangleFilled {
            get {
                if (s_rectangleFilled == null) {
                    s_rectangleFilled = new ILTriangles("RectangleFilled");
                    s_rectangleFilled.Positions = ILPositionsBuffer.UnitCube;
                    s_rectangleFilled.Indices.Update(new int[] { 0, 1, 2, 0, 2, 3 });
                    s_rectangleFilled.AutoNormals = false; 
                }
                return (ILTriangles)s_rectangleFilled.Copy().Detach();
            }
        }
        #endregion

        #region Gears
        private static ILTriangles s_gear2; 
        private static ILTriangles s_gear5;
        private static ILTriangles s_gear10;
        private static ILTriangles s_gear25;
        private static ILTriangles s_gear15;
        private static ILLines s_gear2Wireframe; 
        private static ILLines s_gear5Wireframe;
        private static ILLines s_gear10Wireframe;
        private static ILLines s_gear15Wireframe;
        private static ILLines s_gear25Wireframe;

        /// <summary>
        /// Gets a gear triangle shape with 2 tooths
        /// </summary>
        public static ILTriangles Gear2 {
            get {
                if (s_gear2 == null) {
                    s_gear2 = CreateGear(toothCount: 2);
                    s_gear2.Configure(); 
                }
                return (ILTriangles)s_gear2.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Gets a gear triangle shape with 5 tooths
        /// </summary>
        public static ILTriangles Gear5 {
            get {
                if (s_gear5 == null) {
                    s_gear5 = CreateGear(toothCount: 5);
                    s_gear5.Configure();
                }
                return (ILTriangles)s_gear5.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a gear triangle shape with 10 tooths
        /// </summary>
        public static ILTriangles Gear10 {
            get {
                if (s_gear10 == null) {
                    s_gear10 = CreateGear(toothCount: 10);
                    s_gear10.Configure();
                } 
                return (ILTriangles)s_gear10.Copy().Detach();
            }                             
        }
        /// <summary>
        /// Gets a gear triangle shape with 15 tooths
        /// </summary>
        public static ILTriangles Gear15 {
            get {
                if (s_gear15 == null) {
                    s_gear15 = CreateGear(toothCount: 15);
                    s_gear15.Configure();
                }
                return (ILTriangles)s_gear15.Copy().Detach();
            }
        }
        /// <summary>
        /// Gets a gear triangle shape with 25 tooths
        /// </summary>
        public static ILTriangles Gear25 {
            get {
                if (s_gear25 == null) {
                    s_gear25 = CreateGear(toothCount: 25);
                    s_gear25.Configure();
                }
                return (ILTriangles)s_gear25.Copy().Detach();
            }
        }
        
        /// <summary>
        /// Gets a gear wireframe line shape with 2 tooths
        /// </summary>
        public static ILLines Gear2Wireframe {
            get {
                if (s_gear2Wireframe == null) {
                    s_gear2Wireframe = CreateGearWireframe(toothCount: 2);
                }
                return (ILLines)s_gear2Wireframe.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Gets a gear wireframe line shape with 5 tooths
        /// </summary>
        public static ILLines Gear5Wireframe {
            get {
                if (s_gear5Wireframe == null) {
                    s_gear5Wireframe = CreateGearWireframe(toothCount: 5);
                }
                return (ILLines)s_gear5Wireframe.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Gets a gear wireframe line shape with 10 tooths
        /// </summary>
        public static ILLines Gear10Wireframe {
            get {
                if (s_gear10Wireframe == null) {
                    s_gear10Wireframe = CreateGearWireframe(toothCount: 10);
                }
                return (ILLines)s_gear10Wireframe.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Gets a gear wireframe line shape with 15 tooths
        /// </summary>
        public static ILLines Gear15Wireframe {
            get {
                if (s_gear15Wireframe == null) {
                    s_gear15Wireframe = CreateGearWireframe(toothCount: 15);
                }
                return (ILLines)s_gear15Wireframe.Copy().Detach(); 
            }
        }
        /// <summary>
        /// Gets a gear wireframe line shape with 25 tooths
        /// </summary>
        public static ILLines Gear25Wireframe {
            get {
                if (s_gear25Wireframe == null) {
                    s_gear25Wireframe = CreateGearWireframe(toothCount: 25);
                }
                return (ILLines)s_gear25Wireframe.Copy().Detach(); 
            }
        }

        /// <summary>
        /// Create a new gear triangle shape with arbitrary parameters
        /// </summary>
        /// <param name="inR">inner radius</param>
        /// <param name="outR">outer radius between tooths</param>
        /// <param name="toothR">tooth radius</param>
        /// <param name="toothCount">number of tooths</param>
        /// <param name="thickness">thickness of the gear</param>
        /// <returns>New gear triangles shape</returns>
        public static ILTriangles CreateGear(float inR = 0.2f, float outR = 0.8f, float toothR = 1f, int toothCount = 10, float thickness = 1f) {
            using (ILScope.Enter()) {
                ILTriangles ret = new ILTriangles("Gear"+ toothCount.ToString());
                ILArray<int> indices = 1;
                ILArray<float> normals = 1;
                ret.Positions.Update(Computation.CreateGearTriangles(inR ,outR, toothR, toothCount, thickness, indices: indices, normals: normals));
                ret.Indices.Update(indices);
                ret.Normals.Update(normals);
                ret.AutoNormals = false;
                ret.Color = System.Drawing.Color.Blue;
                //ret.SpecularColor = System.Drawing.Color.LightGray; 
                ret.Colors.Update(null);
                return ret;
            }
        }
        /// <summary>
        /// Create a new gear wireframe lines shape with arbitrary parameters
        /// </summary>
        /// <param name="inR">inner radius</param>
        /// <param name="outR">outer radius between tooths</param>
        /// <param name="toothR">tooth radius</param>
        /// <param name="toothCount">number of tooths</param>
        /// <param name="thickness">thickness of the gear</param>
        /// <returns>New gear wireframe as lines shape</returns>
        public static ILLines CreateGearWireframe(float inR = 0.2f, float outR = 0.8f, float toothR = 1f, int toothCount = 10, float thickness = 1f) {
            using (ILScope.Enter()) {
                ILLines ret = new ILLines("Gear" + toothCount.ToString());
                ILArray<int> indices = 1;
                ILArray<float> normals = 1;
                ret.Positions.Update(Computation.CreateGearLines(inR ,outR, toothR, toothCount, thickness, indices: indices));
                ret.Indices.Update(indices);

                ret.Color = System.Drawing.Color.Black;
                ret.Colors.Update(null);
                return ret;
            }
        }
        #endregion

        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {
            public static ILRetArray<float> CreateGearTriangles(float inR = 0.2f, float outR = 0.8f, float toothR = 1f, int toothCount = 10, float thickness = 1f,
                                                        ILOutArray<int> indices = null, ILOutArray<float> normals = null) {
                using (ILScope.Enter()) {

                    float phase = 0.01f;
                    // tune d here
                    ILArray<float> d = array<float>(outR, outR, outR + phase, toothR - phase, toothR, toothR, toothR - phase, outR + phase).T;
                    int perTooth = d.Length * 2;
                    d["1;:"] = d;
                    d["0;:"] = inR;
                    d.a = repmat(reshape(d, 1, perTooth), 1, toothCount);
                    float toothAng = pif * 2 / toothCount;
                    ILArray<float> ang = linspace<float>(0f, toothAng, perTooth / 2 + 1)[r(0, end - 1)]; //new float[] { 0.00000f, 0.07854f, 0.15708f, 0.23562f, 0.31416f, 0.39270f, 0.47124f, 0.54978f };
                    // tune ang here <--- every entry here corresponds to the polar angle of the _outer_ vertex positions in 'd'
                    ang["1:2:end"] = ang["1:2:end"] + toothAng / 10;  //0.00000f, 0.13854f, 0.15708f, 0.26562f, 0.28416f, 0.42270f, 0.44124f, 0.57978f }; //;

                    ang.a = repmat(ang, 1, toothCount);
                    ang.a = ang + vec<float>(0, toothCount - 1).T * toothAng;
                    ang.a = reshape(ang, 1, perTooth / 2 * toothCount);
                    ang.a = repmat(ang, 2, 1);  // add for inner vertices
                    ang.a = reshape(ang, 1, perTooth * toothCount);
                    /**
                     * d and ang hold polar coordinates for vertex positions now. Storage scheme: 
                     * d: 
                     * inR, outR, inR, outR + phase, ....
                     * 
                     * ang: 
                     * a1, a1, a2, a2, a3, a3, ...  &lt; pif * 2 
                     * 
                     * Length: perTooth * toothCount
                    */


                    ILArray<float> top = zeros<float>(3, perTooth * toothCount);
                    top[0, full] = sin(ang);
                    top[1, full] = cos(ang);
                    top.a = top * d;
                    int firstBottomID = top.S[1];
                    top.a = top[full, cell(full, full)]; // duplicates top -> bottom -> edges (for light)
                    top[2, r(firstBottomID, firstBottomID * 2 - 1)] = -Math.Abs(thickness); // generates bottom
                    top.a = top[full, cell(full, full)]; // duplicates edges (for light)


                    //top.a = repmat(top, 1, 2);
                    if (!isnull(indices)) {
                        int quadsPerTooth = perTooth / 2 - 1;
                        indices.a = array<int>(new int[] { 0, 1, 2, 2, 1, 3 });
                        indices.a = repmat<int>(indices, 1, perTooth / 2 * toothCount);
                        indices.a = indices + 2 * vec<int>(0, perTooth / 2 * toothCount - 1).T;
                        // connect end with start 
                        indices.a = mod(indices, firstBottomID);
                        indices[full, r(perTooth / 2 * toothCount, perTooth * toothCount - 1)] = indices["0,2,1,4,3,5;:"] + firstBottomID;

                        // connect edges 
                        int startID = firstBottomID * 2;
                        ILArray<int> outerEdge = new int[] { 1, firstBottomID + 1, 3, 3, firstBottomID + 1, firstBottomID + 3 };
                        outerEdge.a = repmat<int>(outerEdge, 1, perTooth / 2 * toothCount);
                        outerEdge.a = outerEdge + 2 * vec<int>(0, perTooth / 2 * toothCount - 1).T;
                        outerEdge["2,3;end"] = mod(outerEdge["2,3;end"], firstBottomID);
                        outerEdge["5;end"] = firstBottomID + 1;
                        outerEdge.a = outerEdge + startID;

                        indices[full, r(perTooth * toothCount, 1.5f * perTooth * toothCount - 1)] = outerEdge;

                        outerEdge.a = new int[] { 0, 2, firstBottomID + 2, 0, firstBottomID + 2, firstBottomID };
                        outerEdge.a = repmat<int>(outerEdge, 1, perTooth / 2 * toothCount);
                        outerEdge.a = outerEdge + 2 * vec<int>(0, perTooth / 2 * toothCount - 1).T;
                        outerEdge["1;end"] = 2;
                        outerEdge["2,4;end"] = firstBottomID + 2;
                        outerEdge.a = outerEdge + startID;

                        indices[full, r(1.5f * perTooth * toothCount, perTooth * 2 * toothCount - 1)] = outerEdge;

                    }
                    if (!isnull(normals)) {
                        normals.a = zeros<float>(3, firstBottomID * 4);
                        normals[2, r(0, firstBottomID - 1)] = 1;
                        normals[2, r(firstBottomID, firstBottomID * 2 - 1)] = -1;
                        // edges 
                        ILArray<float> n = top[full, r(3, 2, firstBottomID - 1)] - top[full, r(1, 2, firstBottomID - 2)];
                        n.a = cross(n, repmat(array<float>(0f, 0f, -1f), 1, n.S[1]));
                        normals[full, r(firstBottomID * 2 + 1, 2, firstBottomID * 3)] = n;
                        normals[full, r(firstBottomID * 3 + 1, 2, firstBottomID * 4)] = n;
                        normals[full, firstBottomID * 3 - 1] = n[full, 0];
                        normals[full, firstBottomID * 4 - 1] = n[full, 0]; 
                        // inner hole
                        n = top[full, r(0, 2, firstBottomID - 3)] - top[full, r(2, 2, firstBottomID - 2)];
                        n.a = cross(n, repmat(array<float>(0f, 0f, -1f), 1, n.S[1]));
                        normals[full, r(firstBottomID * 2, 2, firstBottomID * 3 - 2)] = n;
                        normals[full, r(firstBottomID * 3, 2, firstBottomID * 4 - 2)] = n;
                        normals[full, firstBottomID * 3 - 2] = n[full, 0];
                        normals[full, firstBottomID * 4 - 2] = n[full, 0]; 

                    }

                    return top;
                }
            }
            public static ILRetArray<float> CreateGearLines(float inR = 0.2f, float outR = 0.8f, float toothR = 1f, int toothCount = 10, float thickness = 0.4f,
                                                        ILOutArray<int> indices = null) {
                using (ILScope.Enter()) {

                    float phase = 0.01f;
                    // tune d here
                    ILArray<float> d = array<float>(outR, outR, outR + phase, toothR - phase, toothR, toothR, toothR - phase, outR + phase).T;
                    int perTooth = d.Length * 2;
                    d["1;:"] = d;
                    d["0;:"] = inR;
                    d.a = repmat(reshape(d, 1, perTooth), 1, toothCount);
                    float toothAng = pif * 2 / toothCount;
                    ILArray<float> ang = linspace<float>(0f, toothAng, perTooth / 2 + 1)[r(0, end - 1)]; //new float[] { 0.00000f, 0.07854f, 0.15708f, 0.23562f, 0.31416f, 0.39270f, 0.47124f, 0.54978f };
                    // tune ang here <--- every entry here corresponds to the polar angle of the _outer_ vertex positions in 'd'
                    ang["1:2:end"] = ang["1:2:end"] + toothAng / 10;  //0.00000f, 0.13854f, 0.15708f, 0.26562f, 0.28416f, 0.42270f, 0.44124f, 0.57978f }; //;

                    ang.a = repmat(ang, 1, toothCount);
                    ang.a = ang + vec<float>(0, toothCount - 1).T * toothAng;
                    ang.a = reshape(ang, 1, perTooth / 2 * toothCount);
                    ang.a = repmat(ang, 2, 1);  // add for inner vertices
                    ang.a = reshape(ang, 1, perTooth * toothCount);
                    /**
                     * d and ang hold polar coordinates for vertex positions now. Storage scheme: 
                     * d: 
                     * inR, outR, inR, outR + phase, ....
                     * 
                     * ang: 
                     * a1, a1, a2, a2, a3, a3, ...  &lt; pif * 2 
                     * 
                     * Length: perTooth * toothCount
                    */

                    ILArray<float> top = zeros<float>(3, perTooth * toothCount);
                    top[0, full] = sin(ang);
                    top[1, full] = cos(ang);
                    top.a = top * d;
                    int firstBottomID = top.S[1];
                    top.a = top[full, cell(full, full)]; // duplicates top -> bottom -> edges
                    top[2, r(firstBottomID, firstBottomID * 2 - 1)] = -Math.Abs(thickness); // generates bottom

                    if (!isnull(indices)) {
                        int nrl = perTooth / 2 * toothCount; 
                        indices.a = zeros<int>(2,nrl * 6);
                        indices[0, r(0, nrl - 1)] = vec<int>(1, 2, nrl * 2);
                        indices[1, r(0, nrl - 2)] = vec<int>(3, 2, nrl * 2);
                        indices[1, nrl - 1] = 1;

                        // bottom 
                        indices[0, r(nrl, 2 * nrl - 1)] = vec<int>(1, 2, nrl * 2) + firstBottomID;
                        indices[1, r(nrl, 2 * nrl - 2)] = vec<int>(3, 2, nrl * 2) + firstBottomID;
                        indices[1, 2 * nrl - 1] = firstBottomID + 1;

                        // inner hole
                        indices[0, r(2 * nrl, 3 * nrl - 1)] = vec<int>(0, 2, nrl * 2);
                        indices[1, r(2 * nrl, 3 * nrl - 1)] = vec<int>(2, 2, nrl * 2);
                        indices[1, 3 * nrl - 1] = 0;
                        
                        indices[0, r(3 * nrl, 4 * nrl - 1)] = vec<int>(0, 2, nrl * 2) + firstBottomID;
                        indices[1, r(3 * nrl, 4 * nrl - 1)] = vec<int>(2, 2, nrl * 2) + firstBottomID;
                        indices[1, 4 * nrl - 1] = firstBottomID;

                        // outer edges
                        indices[0, r(4 * nrl, 5 * nrl - 1)] = vec<int>(1, 2, nrl * 2);
                        indices[1, r(4 * nrl, 5 * nrl - 1)] = vec<int>(1, 2, nrl * 2) + firstBottomID;
                        
                        indices[0, r(5 * nrl, 6 * nrl - 1)] = vec<int>(0, 2, nrl * 2);
                        indices[1, r(5 * nrl, 6 * nrl - 1)] = vec<int>(0, 2, nrl * 2) + firstBottomID;

                    }

                    return top;
                }
            }
        }

    }
}
