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
using System.Drawing; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// Interface for all driver implementations, most common implementor is System.Drawing.ILDriver
    /// </summary>
    public interface IILDriver {
        /// <summary>
        /// Must fire befor a frame is rendered
        /// </summary>
        event EventHandler<ILRenderEventArgs> BeginRenderFrame;
        /// <summary>
        /// Must fire after the frame was rendered
        /// </summary>
        event EventHandler<ILRenderEventArgs> EndRenderFrame;
        /// <summary>
        /// Fires, when an unrecoverable error occoured during rendering
        /// </summary>
        event EventHandler<ILRenderErrorEventArgs> RenderingFailed;
        /// <summary>
        /// Configures the scene
        /// </summary>
        void Configure();
        /// <summary>
        /// Trigger a frame rendering at a specific point in time
        /// </summary>
        /// <param name="timeMs"></param>
        void Render(long timeMs = 0); 
        /// <summary>
        /// Return a scene which reflects the rendering result, including all local compositions and modifications
        /// </summary>
        ILScene GetCurrentScene(long timeMs);
        /// <summary>
        /// Reserved for future use
        /// </summary>
        /// <param name="Capability"></param>
        /// <returns>True if the driver is able to support the requested capability</returns>
        bool Supports(Capabilities Capability);
        
        /// <summary>
        /// Performs picking on the current local scene
        /// </summary>
        /// <param name="screenCoords">position in screen coordinates (pixels)</param>
        /// <param name="timeMS">the time at which the scene is picked at</param>
        /// <returns>If an object is visible at the given position, its ID is returned. Otherwise null is returned.</returns>
        int? PickAt(System.Drawing.Point screenCoords, long timeMS);

        /// <summary>
        /// Fires when the current framerate has changed. 
        /// </summary>
        event EventHandler FPSChanged; 

        Size Size { get; set; }
        int FPS { get; }
        ILCamera Camera { get; }
        Color BackColor { get; set; }
        RendererTypes Driver { get; }
        ILScene Scene { get; set; }
        ILScene LocalScene { get; }
        ILGroup SceneSyncRoot { get; }
        ILGroup LocalSceneSyncRoot { get; }
        RectangleF Rectangle { get; set; }
        Matrix4 ViewTransform { get; }
    }
}
