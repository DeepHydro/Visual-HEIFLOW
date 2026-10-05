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

namespace ILNumerics.Drawing {

    /// <summary>
    /// tick provider function delegate definition
    /// </summary>
    /// <param name="min">minimum axis limit</param>
    /// <param name="max">maximum axis limit</param>
    /// <param name="maxCount">maximum number of ticks to create</param>
    /// <returns>list of tick position to be drawn</returns>
    /// <remarks>User defined tick providers must fulfill this delegates signature. 
    /// </remarks>
    public delegate List<float> ILLabeledTickProvider (float min, float max, int maxCount);  

    /// <summary>
    /// occours if the clipping data for an subfigure have changed
    /// </summary>
    /// <param name="sender">object which changed the data</param>
    /// <param name="e">arguments containing the new clipping data</param>
    public delegate void ILClippingDataChangedEvent (object sender, ClippingChangedEventArgs e); 
    
    /// <summary>
    /// delegate used to measure text, device dependent
    /// </summary>
    /// <param name="text">text to be measured</param>
    /// <param name="font">Font used for rendering</param>
    /// <returns>Size in screen coords</returns>
    public delegate Size MeasureTextDelegate(string text, Font font); 

    /// <summary>
    /// arguments on ClippinChangedEvents
    /// </summary>
    public class ClippingChangedEventArgs : EventArgs {
        /// <summary>
        /// creates a new ClippingChangedEventArgs object
        /// </summary>
        /// <param name="clippingData"></param>
        public ClippingChangedEventArgs(ILLimits clippingData) {
            ClippingData = clippingData; 
        }
        /// <summary>
        /// the current (new) clipping data
        /// </summary>
        public ILLimits ClippingData;
    }
    /// <summary>
    /// occours if a graphics device has been reset by the underlying graphics framework
    /// </summary>
    /// <param name="sender">objects who hosts the graphics device</param>
    /// <param name="eventArgs"></param>
    public delegate void ILGraphicsDeviceResetEvent(object sender, EventArgs eventArgs); 
    /// <summary>
    /// occours if a graphics device has been (re)created by an output panel
    /// </summary>
    /// <param name="sender">objects who hosts the graphics device</param>
    /// <param name="eventArgs"></param>
    public delegate void ILGraphicsDeviceCreatedEvent(object sender, EventArgs eventArgs); 
    /// <summary>
    /// arguments to communicate changes on graphs 
    /// </summary>
    public class ILGraphChangedEventArgs : EventArgs {
        /// <summary>
        /// string description of the changed parameter
        /// </summary>
        public readonly string Source; 
        public ILGraphChangedEventArgs (string source) {
            this.Source = source; 
        }
    }
   

    /// <summary>
    /// Event handler handling LabeledTickAdding events 
    /// </summary>
    public class ILLabeledTickAddingArgs : EventArgs {
        public bool Cancel; 
        public float Value; 
        public string Expression; 
        public int Index; 

        public ILLabeledTickAddingArgs (float value, string expression, int index) {
            Value = value; 
            Expression = expression; 
            Cancel = false; 
            Index = index; 
        }
    }
    /// <summary>
    /// Delegate definition for function handling LabeledTickAdding events
    /// </summary>
    /// <param name="sender">the sender of the event (e.g. ILTickCollection)</param>
    /// <param name="args">arguments </param>
    public delegate void LabeledTickAddingHandler (object sender, ILLabeledTickAddingArgs args); 

    /// <summary>
    /// Event arguments for axis changed events
    /// </summary>
    public class ILAxisChangedEventArgs : EventArgs {
        /// <summary>
        /// Name of changed axis (X-,Y-,ZAxis)
        /// </summary>
        public AxisNames AxisName;
        /// <summary>
        /// construct a new instance 
        /// </summary>
        /// <param name="name"></param>
        public ILAxisChangedEventArgs (AxisNames name) {
            AxisName = name; 
        }
    }
    /// <summary>
    /// delegate for functions handling AxisChanged events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void AxisChangedEventHandler (object sender, ILAxisChangedEventArgs args); 

}
