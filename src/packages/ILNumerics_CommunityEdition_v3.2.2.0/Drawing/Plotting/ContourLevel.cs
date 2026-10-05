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
using System.Xml.Serialization; 

namespace ILNumerics.Drawing.Plotting {

    [Serializable]
    public struct ContourLevel {
        public float? Value { get; set; }
        /// <summary>
        /// Get the width of the contour lines of this level or sets it
        /// </summary>
        public int? LineWidth { get; set; }
        /// <summary>
        /// (Not used yet)
        /// </summary>
        public int? LabelSpacing { get; set; }
        /// <summary>
        /// Override the automatic text setting for contour line labels
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Determine the line color by mapping the value into the current colormap
        /// </summary>
        public float? LineColor { get; set; }
        /// <summary>
        /// Determine the fill color by mapping the value into the current colormap
        /// </summary>
        public float? FillColor { get; set; }
        /// <summary>
        /// Get the color for the contour line labels or sets it 
        /// </summary>
        public Color? LabelColor { get; set; }
        /// <summary>
        /// Get the dash style for the contour lines of that level or sets it
        /// </summary>
        public DashStyle? LineStyle { get; set; }
        /// <summary>
        /// If set, determines if contour lines of this level should be labeled, otherwise take Default setting
        /// </summary>
        public bool? ShowLabel { get; set; }
        [XmlIgnore]
        internal int ShapeID { get; set; }

        public static ContourLevel GetSettingOrDefault(ContourLevel levelsSetting) {
            ContourLevel ret = Default;
            if (levelsSetting.FillColor.HasValue) ret.FillColor = levelsSetting.FillColor;
            if (levelsSetting.LabelColor.HasValue) ret.LabelColor = levelsSetting.LabelColor;
            if (levelsSetting.LabelSpacing.HasValue) ret.LabelSpacing = levelsSetting.LabelSpacing;
            if (levelsSetting.LineColor.HasValue) ret.LineColor = levelsSetting.LineColor;
            if (levelsSetting.LineStyle.HasValue) ret.LineStyle = levelsSetting.LineStyle;
            if (levelsSetting.LineWidth.HasValue) ret.LineWidth = levelsSetting.LineWidth;
            if (levelsSetting.ShowLabel.HasValue) ret.ShowLabel = levelsSetting.ShowLabel;
            if (levelsSetting.Text != null) ret.Text = levelsSetting.Text;
            if (levelsSetting.Value.HasValue) ret.Value = levelsSetting.Value;
            return ret;
        }
        public static ContourLevel Default = new ContourLevel {
            FillColor = null,
            LabelColor = Color.Black,
            LabelSpacing = 100, 
            LineColor = null,
            LineStyle = ILNumerics.Drawing.DashStyle.Solid,
            LineWidth = 1,
            Text = null,
            Value = null,
            ShowLabel = true
        };
    }
}
