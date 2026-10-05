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
using System.Drawing;
using System.Windows.Forms;

namespace ILNumerics.Drawing {
    /// <summary>
    /// Extends common MouseEventArgs for event processing for ILNumerics scenes
    /// </summary>
    public class ILMouseEventArgs : MouseEventArgs {
        /// <summary>
        /// The mouse position relative to the current viewport; range [0...1] 
        /// </summary>
        public PointF LocationF { get; internal set; }
        /// <summary>
        /// True: event processing will be canceled after this node and not get populated further down or up.
        /// </summary>
        public bool Cancel = false;
        /// <summary>
        /// Determines if the SHIFT key is currently pressed (ignored on Mac)
        /// </summary>
        public bool ShiftPressed = false;
        /// <summary>
        /// Determines if the CTRL key is currently pressed (ignored on Mac)
        /// </summary>
        public bool ControlPressed = false;
        /// <summary>
        /// Determines if the ALT key is currently pressed (ignored on Mac)
        /// </summary>
        public bool AltPressed = false;
        /// <summary>
        /// The global scene time when the event was raised
        /// </summary>
        public long TimeMS = 0; 
        /// <summary>
        /// The final target node for the event, commonly the node under the mouse cursor
        /// </summary>
        public ILNode Target { get; internal set; }
        /// <summary>
        /// Signals the driver to redraw the scene after processing the event; default: false
        /// </summary>
        public bool Refresh = false; 
        /// <summary>
        /// Gives the direction of event; false: capture (down), true: bubbling (up) 
        /// </summary>
        public bool DirectionUp { get; internal set; }


        private ILMouseEventArgs() : base(MouseButtons.None,0,0,0,0) { }
        public ILMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, PointF locationF)
            : base(button,clicks,x,y,delta) {
            LocationF = locationF;
        }
        public ILMouseEventArgs(PointF locationF, MouseEventArgs args, bool shift, bool alt, bool control) 
            :base(args.Button,args.Clicks,args.X,args.Y,args.Delta) {
            LocationF = locationF; 
            ShiftPressed = shift; 
            ControlPressed = control; 
            AltPressed = alt; 
        }

        public ILMouseEventArgs Clone() {
            return (ILMouseEventArgs)MemberwiseClone(); 
        }
        public override string ToString() {
            return String.Format("Button:{0} - Cancel:{1} - DirectionUp:{2} - Location:{3} - LocationF:{4} - Target:{5}",
                    this.Button, this.Cancel, this.DirectionUp, this.Location, this.LocationF, this.Target); 
        }
        /// <summary>
        /// Creates an empty ILMouseEventArgs instance
        /// </summary>
        public static ILMouseEventArgs Empty {
            get { return new ILMouseEventArgs(); }
       }
    }
}
