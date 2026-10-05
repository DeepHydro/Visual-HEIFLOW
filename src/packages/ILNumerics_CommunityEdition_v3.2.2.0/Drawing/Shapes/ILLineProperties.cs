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
using System.Text;
using System.Drawing;
using System.ComponentModel; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// properties for line graphs
    /// </summary>
    public class ILLineProperties : INotifyPropertyChanged {

        #region event handling 
        /// <summary>
        /// Fires if a properties was changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnChanged(string name) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name)); 
            }
        }
        #endregion

        #region attributes
        private int m_lineWidth;
        private DashStyle m_lineStyle;
        private short m_linePattern;
        private float m_linePatternScale;
        private bool m_setAntialiasing = false;
        private bool m_antialiasing;
        #endregion

        #region Properties 
        /// <summary>
        /// Get/set the width of the lines, default: 1
        /// </summary>
        public int Width {
            get {
                return m_lineWidth; 
            }
            set {
                // for width = 1, antialiasing will not work and 
                // must get disabled temporarily here 
                if (value <= 1) {
                    if (m_antialiasing) {
                        m_antialiasing = false; 
                        m_setAntialiasing = true; 
                    }
                } else {
                    if (m_setAntialiasing) {
                        m_setAntialiasing = false; 
                        m_antialiasing = true; 
                    }
                }
                m_lineWidth = value;
                OnChanged("Width"); 
            }
        }
        /// <summary>
        /// line style (default: solid)
        /// </summary>
        public DashStyle DashStyle {
            get {
                return m_lineStyle; 
            }
            set {
                m_lineStyle = value;
                OnChanged("DashStyle"); 
            }
        }
        /// <summary>
        /// user defined line stipple pattern for line style 'UserPattern'
        /// </summary>
        /// <remarks>the pattern is defined by corresponding bits 
        /// set in the short value. It may be stretched via the 
        /// LinePatternScale parameter. Default: 15</remarks>
        public short Pattern {
            get {
                return m_linePattern; 
            }
            set {
                m_linePattern = value;
                m_lineStyle = DashStyle.UserPattern;
                OnChanged("Pattern"); 
            }
        }
        /// <summary>
        /// scaling for line stipple patterns (default: 2.0f)
        /// </summary>
        public float PatternScale {
            get {
                return m_linePatternScale; 
            }
            set {
                m_linePatternScale = value;
                OnChanged("PatternScale"); 
            }
        }

        /// <summary>
        /// draw lines with smooth antialiasing (if possible and supported)
        /// </summary>
        /// <remarks>Smooth edges will be drawn if the driver supports antialiased lines. 
        /// This sometimes comes with the drawback of the lines apppearing to be 
        /// thicker. Not all objects support antialiasing. Default value is 'false'.</remarks>
        public bool Antialiasing {
            get {
                return m_antialiasing; 
            }
            set {
                if (m_setAntialiasing && !value) {
                    m_setAntialiasing = false; 
                }
                m_antialiasing = value; 
                OnChanged("Antialiasing"); 
            }
        }
        #endregion 

        #region public properties 
        public ILLineProperties Clone() {
            return (ILLineProperties) this.MemberwiseClone(); 
        }
        public void CopyFrom(ILLineProperties props) {
            if (props != null) {
                this.Antialiasing = props.Antialiasing;
                this.Pattern = props.Pattern;
                this.PatternScale = props.PatternScale;
                this.DashStyle = props.DashStyle;
                this.Width = props.Width; 
            }
        }
        #endregion 

        #region constructors
        /// <summary>
        /// create default properties for graphs
        /// </summary>
        public ILLineProperties() {
            m_lineStyle = DashStyle.Solid; 
            m_lineWidth = 1; 
            m_linePattern = 15; 
            m_linePatternScale = 2.0f;
            m_antialiasing = false; 
        }
        /// <summary>
        /// create a new instance of this class based on another instance
        /// </summary>
        /// <param name="props">properties to be copied</param>
        public ILLineProperties(ILLineProperties props) {
            m_lineStyle = props.DashStyle; 
            m_lineWidth = props.Width; 
            m_linePattern = props.Pattern; 
            m_linePatternScale = props.PatternScale; 
            m_antialiasing = props.m_antialiasing;
        }
        #endregion

    }
}
