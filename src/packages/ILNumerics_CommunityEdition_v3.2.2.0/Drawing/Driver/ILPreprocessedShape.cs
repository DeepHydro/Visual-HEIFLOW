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
using System.Text;
using System.Diagnostics; 

namespace ILNumerics.Drawing {

    [DebuggerDisplay("Node:{Node} Position:{Position}")]
    public class ILPreprocessedShape  {
        public ILDrawable Node;
        public Matrix4 Model2CameraTransform;
        public Matrix4 Camera2ClipTransform; 
        public Matrix4 Normalized2ViewTransform; 
        public Matrix4 CameraPositionTransform; 
        public Vector3 Position; 
        public float Alpha; 
        public Color? ColorOverride;
        public RenderTarget Target;
        public Vector4 LogState; 
        public ILClipParams Clipping; 

        public ILPreprocessedShape(ILDrawable node, ILRenderParameter parameters, Vector3 position) {
            Node = node;
            Model2CameraTransform = parameters.CurrentModel2CameraTransform;
            Position = position;
            Alpha = parameters.Alpha.Peek();
            Camera2ClipTransform = parameters.ProjectionTransform;
            Normalized2ViewTransform = parameters.ViewTransform;
            ColorOverride = parameters.ColorOverride.Peek();
            Target = parameters.RenderTargets.Peek(); 
            LogState = parameters.LogState.Peek();
            Clipping = parameters.PeekClipping(); 
            CameraPositionTransform = parameters.WorldToCameraTransform; 
        }

        internal void VisitNode(ILRenderParameter renderParams) {
            renderParams.Push(Model2CameraTransform);
            renderParams.PushProjectionTransform(Camera2ClipTransform);
            renderParams.ViewTransforms.Push(Normalized2ViewTransform);
            renderParams.Alpha.Push(Alpha);
            renderParams.ColorOverride.Push(ColorOverride);
            renderParams.RenderTargets.Push(Target); 
            renderParams.LogState.Push(LogState);
            renderParams.PushClipping(Clipping, true); 
            renderParams.Driver.DrawNode(Node, renderParams);
            renderParams.Pop();
            renderParams.PopProjectionTransform();
            renderParams.ViewTransforms.Pop();
            renderParams.Alpha.Pop();
            renderParams.ColorOverride.Pop();
            renderParams.RenderTargets.Pop();
            renderParams.LogState.Pop();
            renderParams.PopClipping(); 
        }
        /// <summary>
        /// temporarly recall the state stored in this instance forr running the specified action 
        /// </summary>
        /// <param name="action">action to run in the stored state</param>
        /// <param name="renderParams">current render parameters</param>
        public void  RecallStateFor(Action<ILDrawable, ILRenderParameter> action, ILRenderParameter renderParams) {
            renderParams.Push(Model2CameraTransform);
            renderParams.PushProjectionTransform(Camera2ClipTransform);
            renderParams.ViewTransforms.Push(Normalized2ViewTransform);
            renderParams.Alpha.Push(Alpha);
            renderParams.ColorOverride.Push(ColorOverride);
            renderParams.RenderTargets.Push(Target);
            renderParams.LogState.Push(LogState);
            renderParams.PushClipping(Clipping, true);
            action(Node, renderParams);
            renderParams.Pop();
            renderParams.PopProjectionTransform();
            renderParams.ViewTransforms.Pop();
            renderParams.Alpha.Pop();
            renderParams.ColorOverride.Pop();
            renderParams.RenderTargets.Pop();
            renderParams.LogState.Pop();
            renderParams.PopClipping();
        }
    }
}
