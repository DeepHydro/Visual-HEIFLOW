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
using System.ComponentModel;

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public class ILScaleModes : INotifyPropertyChanged {
        private AxisScale m_XAxisScale;
        private AxisScale m_YAxisScale;
        private AxisScale m_ZAxisScale;

        public AxisScale XAxisScale {
            get { return m_XAxisScale; }
            set {
                if (m_XAxisScale != value) {
                    m_XAxisScale = value;
                    OnPropertyChanged("XAxisScale");
                }
            }
        }
        public AxisScale YAxisScale {
            get { return m_YAxisScale; }
            set {
                if (m_YAxisScale != value) {
                    m_YAxisScale = value;
                    OnPropertyChanged("YAxisScale");
                }
            }
        }
        public AxisScale ZAxisScale {
            get { return m_ZAxisScale; }
            set {
                if (m_ZAxisScale != value) {
                    m_ZAxisScale = value;
                    OnPropertyChanged("ZAxisScale");
                }
            }
        }
        public AxisScale this[AxisNames name] {
            get {
                switch (name) {
                    case AxisNames.XAxis:
                        return m_XAxisScale;
                    case AxisNames.YAxis:
                        return m_YAxisScale;
                    default:
                        return m_ZAxisScale; 
                }
            }
            set {
                switch (name) {
                    case AxisNames.XAxis:
                        XAxisScale = value;
                        break; 
                    case AxisNames.YAxis:
                        YAxisScale = value;
                        break;
                    default:
                        ZAxisScale = value;
                        break;
                }
            }
        }

        public ILScaleModes() {
            m_XAxisScale = AxisScale.Linear;
            m_YAxisScale = AxisScale.Linear;
            m_ZAxisScale = AxisScale.Linear; 
        }

        #region public interface 
        public ILScaleModes Copy() {
            ILScaleModes ret = new ILScaleModes(); 
            ret.m_XAxisScale = m_XAxisScale; 
            ret.m_YAxisScale = m_YAxisScale; 
            ret.m_ZAxisScale = m_ZAxisScale; 
            return ret;
        }
        internal Vector4 GetLogState() {
            Vector4 ret = new Vector4(
                m_XAxisScale == AxisScale.Linear ? 0 : 1,
                m_YAxisScale == AxisScale.Linear ? 0 : 1,
                m_ZAxisScale == AxisScale.Linear ? 0 : 1,
                (m_XAxisScale == AxisScale.Logarithmic || m_YAxisScale == AxisScale.Logarithmic || m_ZAxisScale == AxisScale.Logarithmic) ? 1 : 0
                ); 
            return ret; 
        }
        internal Vector3? GetBoundsForLimits() {
            if (m_XAxisScale == AxisScale.Linear && 
                m_YAxisScale == AxisScale.Linear &&
                m_ZAxisScale == AxisScale.Linear) 
                return null; 
            Vector3 ret = new Vector3(
                m_XAxisScale == AxisScale.Linear ? float.NaN : 0,
                m_YAxisScale == AxisScale.Linear ? float.NaN : 0,
                m_ZAxisScale == AxisScale.Linear ? float.NaN : 0
                );
            return ret;
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name)); 
            }
        }

        #endregion

    }
}
