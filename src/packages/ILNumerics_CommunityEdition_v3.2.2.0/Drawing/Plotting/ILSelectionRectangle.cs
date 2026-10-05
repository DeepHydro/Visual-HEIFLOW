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

    /// <summary>
    /// Class used for displaying a selection rectangle for zooming in 2D plots 
    /// </summary>
    [Serializable]
    public class ILSelectionRectangle : ILGroup {

        #region attributes
        /// <summary>
        /// Tag used to identify the selection rectangle lines in the scene graph
        /// </summary>
        public static readonly string LineTag = "SelectionRectangleLines"; 
        /// <summary>
        /// Tag used to identify the selection rectangle group node in the scene graph
        /// </summary>
        public static readonly string GroupTag = "SelectionRectangle"; 
        [XmlIgnore]
        private Vector3 Min { get; set; }
        [XmlIgnore]
        private Vector3 Max { get; set; } 
        #endregion

        #region properties 
        [XmlIgnore]
        public ILLineStrip Lines {
            get {
                return First<ILLineStrip>(LineTag);
            }
        }
        #endregion

        public ILSelectionRectangle(object tag = null) : base(tag ?? GroupTag) {
            Add(new ILLineStrip(LineTag));
            Lines.Positions.Update(new float[,] {
                {0,0,0},
                {0,1,0},
                {1,1,0},
                {1,0,0},
                {0,0,0}
            });
            Lines.Color = Color.Blue;
            Lines.Width = 2; 
            Lines.DashStyle = DashStyle.Dashed;
            Lines.Configure(); 
            Visible = false; 
        }
        public ILSelectionRectangle(ILSelectionRectangle source)
            : base(source) {
            Min = source.Min; 
            Max = source.Max; 
        }

        public void SetSize(PointF min, PointF max) {
            Min = new Vector3(Math.Min(min.X, max.X), Math.Min(min.Y, max.Y), 0);
            Max = new Vector3(Math.Max(min.X, max.X), Math.Max(min.Y, max.Y), 0); 
        }

        internal override ILNode Copy() {
            return new ILSelectionRectangle(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILSelectionRectangle();
        }
        protected override bool BeginVisit(ILRenderParameter parameter) {
            if (Visible && parameter.CurrentPassCount == 0) {
                Vector3 min = Min * new Vector3(2,-2, 1) - new Vector3(1,-1, 1);
                Vector3 max = Max * new Vector3(2,-2, 1) - new Vector3(1,-1, 1); 
                Transform = Matrix4.ScaleTransform(max.X - min.X, max.Y - min.Y, 1)
                    .Translate(min.X, min.Y, 1); 
            }
            return base.BeginVisit(parameter);
        }
    }
}
