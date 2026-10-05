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


#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;


namespace ILNumerics.Drawing {

    /// <summary>
    /// Modes determining how a legend collects its items
    /// </summary>
    public enum LegendMode {
        /// <summary>
        /// Automatically collect and add all items found
        /// </summary>
        Auto, 
        /// <summary>
        /// Complete manual configuration for legends
        /// </summary>
        Manual
    }

    /// <summary>
    /// Unit scales for lengths
    /// </summary>
    public enum Units {
        /// <summary>
        /// The length/ coordinate is measured in screen pixels 
        /// </summary>
        Pixels, 
        /// <summary>
        /// The length/ coordinate is measured in points 
        /// </summary>
        Points,
        /// <summary>
        /// The length/ coordinate is measured relative to the current viewport size, range [0...1]
        /// </summary>
        Viewport
    }

    /// <summary>
    /// modes for projecting the rendering output to the available client area
    /// </summary>
    public enum AspectRatioMode {
        /// <summary>
        /// plots fill available rendering area of the PlotCubeScreenRectF rectangle
        /// </summary>
        StretchToFill, 
        /// <summary>
        /// plots will be shrinked to fit inside PlotCubeScreeRectF but maintain data aspect ratio
        /// </summary>
        MaintainRatios
    }
    /// <summary>
    /// transition from current to new zoom setting
    /// </summary>
    public enum ZoomModes {
        /// <summary>
        /// No ramp, jump to new zoom setting
        /// </summary>
        Jump,
        /// <summary>
        /// morph softly to new zoom setting
        /// </summary>
        RollSoft,
        /// <summary>
        /// morph faster to new setting
        /// </summary>
        RollHard,
        /// <summary>
        /// morph to new setting by 'going over the limit'
        /// </summary>
        RollOverride
    }
    /// <summary>
    /// predefined colormaps
    /// </summary>
    public enum Colormaps {
        Autumn,
        Bone,
        Colorcube,
        Cool,
        Copper,
        Flag,
        Gray,
        Hot,
        Hsv,
        ILNumerics,
        Jet,
        Lines, 
        Pink,
        Prism,
        Spring,
        Summer,
        White,
        Winter
    }

    public enum SortingMode {
        /// <summary>
        /// No sorting
        /// </summary>
        None,
        /// <summary>
        /// BackToFront
        /// </summary>
        BackToFront,
        /// <summary>
        /// FrontToBack
        /// </summary>
        FrontToBack
    }

    /// <summary>
    /// Names for all axes 
    /// </summary>
    public enum AxisNames {
        /// <summary>
        /// The X axis
        /// </summary>
        XAxis = 0,
        /// <summary>
        /// The Y axis
        /// </summary>
        YAxis = 1,
        /// <summary>
        /// The Z axis
        /// </summary>
        ZAxis = 2, 
        /// <summary>
        /// Custom axis or color scale
        /// </summary>
        CAxis = 3
    }

    /// <summary>
    /// Axis ticks spacing type: linear, logarithmic
    /// </summary>
    public enum AxisScale {
        /// <summary>
        /// Linear axis tick spacing
        /// </summary>
        Linear,
        /// <summary>
        /// Logarithmic axis tick spacing
        /// </summary>
        Logarithmic
    }
    /// <summary>
    /// TickModes - automatic or manual axis tick positioning
    /// </summary>
    public enum TickMode {
        /// <summary>
        /// find tick positions automatically 
        /// </summary>
        Auto,
        /// <summary>
        /// manually create ticks 
        /// </summary>
        Manual
    }
    ///// <summary>
    ///// 
    ///// </summary>
    //public enum NodeScope {
    //    /// <summary>
    //    /// The content of the node is distributed to all drivers (Default).
    //    /// </summary>
    //    Global,
    //    /// <summary>
    //    /// Drivers are responsible to fill the node with local content. Content of the node is not distributed. 
    //    /// </summary>
    //    Local
    //}
    /// <summary>
    /// options for the sizing of the projection of the plot cube onto the 2D screen client area of the control
    /// </summary>
    public enum PlotBoxScreenSizeMode {
        /// <summary>
        /// the projection of the plot cube drawing area fills the whole controls space (labels may be hidden) 
        /// </summary>
        Maximum, 
        /// <summary>
        /// the size of plot cube projection rectangle is automatically determined, taking labels size into account
        /// </summary>
        Optimal,
        /// <summary>
        /// more pixel exact positioning, strictly only the place really needed for labels is used (slower)
        /// </summary>
        StrictOptimal,
        /// <summary>
        /// No automatic resizing for the cube projection size, values from PlotCubeScreenRect are taken
        /// </summary>
        Manual
    }

    /// <summary>
    /// projection types
    /// </summary>
    public enum Projection {
        /// <summary>
        /// 3D graphs will be distorted for opotimized 3D impression
        /// </summary>
        Perspective,
        /// <summary>
        /// graphs will not be distorted. objects in the front will get the same size as objects in the back
        /// </summary>
        Orthographic
    }

    /// <summary>
    /// valid marker styles (partially supported)
    /// </summary>
    public enum MarkerStyle {
        /// <summary>
        /// draw markers as dots
        /// </summary>
        Dot,
        /// <summary>
        /// draw markers as circle
        /// </summary>
        Circle,
        /// <summary>
        /// draw markers as diamonds
        /// </summary>
        Diamond,
        /// <summary>
        /// draw markers as square
        /// </summary>
        Square,
        /// <summary>
        /// draw markers as a flat rectangle
        /// </summary>
        Rectangle,
        /// <summary>
        /// draw markers as up pointing triangles
        /// </summary>
        TriangleUp,
        /// <summary>
        /// draw markers as up pointing triangles
        /// </summary>
        TriangleDown,
        /// <summary>
        /// draw markers as up pointing triangles
        /// </summary>
        TriangleLeft,
        /// <summary>
        /// draw markers as up pointing triangles
        /// </summary>
        TriangleRight,
        /// <summary>
        /// draw markers as plus
        /// </summary>
        Plus,
        /// <summary>
        /// draw markers as cross
        /// </summary>
        Cross,
        /// <summary>
        /// user defined marker shape
        /// </summary>
        Custom,
        None,
    }
    /// <summary>
    /// Possible positions of the camera
    /// </summary>
    public enum CameraQuadrant {
        TopLeftFront,
        TopLeftBack,
        TopRightBack,
        TopRightFront,
        BottomLeftFront,
        BottomLeftBack,
        BottomRightBack,
        BottomRightFront
    }

    /// <summary>
    /// line style
    /// </summary>
    public enum DashStyle {
        /// <summary>
        /// solid line
        /// </summary>
        Solid = Int32.MaxValue, 
        /// <summary>
        /// dashed line
        /// </summary>
        Dashed = Int16.MaxValue,
        /// <summary>
        /// point dashed line
        /// </summary>
        PointDash = unchecked((int)4294837760),
        /// <summary>
        /// dotted line
        /// </summary>
        Dotted = unchecked((int)3435973836),
        /// <summary>
        /// use user stipple pattern 
        /// </summary>
        UserPattern = -1,
    }
    /// <summary>
    /// modes of mouse interaction with ILPanel
    /// </summary>
    public enum InteractiveModes {
        ZoomRectangle, 
        Rotating,
        Selecting,
        Translating,
        None
    }
    /// <summary>
    /// Target coordinate system for objects like labels 
    /// </summary>
    public enum RenderTarget {
        /// <summary>
        /// The object is rendered above Screen2DFar and World3D objects; 2D screen coordinate system: [0,0,-1] -> [1,1,1]
        /// </summary>
        Screen2DNear,
        /// <summary>
        /// The object is rendered above Screen2DFar and World3D objects; 2D screen coordinate system: [0,0,-1] -> [1,1,1]
        /// </summary>
        Screen2DFar,
        /// <summary>
        ///  (default) The object is rendered as regular 3D object; coordinate system range: [-1,-1,-1] -> [1,1,1]
        /// </summary>
        World3D
    }
    /// <summary>
    /// possible types of renderable items 
    /// </summary>
    public enum RenderItemType {
        /// <summary>
        /// the item defines a character 
        /// </summary>
        Character,
        /// <summary>
        /// the item defines a bitmap
        /// </summary>
        Bitmap
    }
    /// <summary>
    /// Options for face culling (specific drivers only)
    /// </summary>
    public enum CullFaces {
        /// <summary>
        /// Faces defined in clockwise order are culled 
        /// </summary>
        CW,
        /// <summary>
        /// Faces defined in counter-clockwise order are culled
        /// </summary>
        CCW,
        /// <summary>
        /// No faces are culled
        /// </summary>
        None
    }

    public enum RendererTypes {
        GDI,
        OpenGL,
        DirectX,
        SVG,
        WebGL,
        XML,
    }
    public enum LoopModes {
        Once, 
        ForwardsBackOnce,
        ForwardsBackLoop,
        ForwardsLoop
    }
    /// <summary>
    /// Scene types, used to determine, where a node is hosted.
    /// </summary>
    public enum SceneTypes {
        /// <summary>
        /// The global scene holds the main shapes and plots and is shareable between drivers/controls.
        /// </summary>
        Global,
        /// <summary>
        /// Adresses the local part of a scene, which is 'private' to every driver
        /// </summary>
        Local
    }
}

//namespace ILNumerics.Drawing.Labeling {
//    /// <summary>
//    /// simple single labeled tick
//    /// </summary>
//    public struct LabeledTick {
//        /// <summary>
//        /// tick label
//        /// </summary>
//        public readonly ILRenderQueue Queue; 
//        /// <summary>
//        /// tick position
//        /// </summary>
//        public readonly float Position; 
//        /// <summary>
//        /// create single labeled tick
//        /// </summary>
//        /// <param name="position">position</param>
//        /// <param name="queue">render queue used to render the item</param>
//        public LabeledTick(float position, ILRenderQueue queue) { 
//            Position = position; 
//            Queue    = queue; 
//        }
//    }
//}
    
