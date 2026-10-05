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
using System.Xml.Serialization; 

namespace ILNumerics.Drawing.Lighting {
    [Serializable]
    public class ILMaterial : ICloneable {

        #region attributes
        float m_shininess;
        Color m_specular;
        Color m_emission;
        bool m_lightingEnabled;
        #endregion

        #region properties
        /// <summary>
        /// Switch light support for the material/shape on/off. Default: on
        /// </summary>
        [XmlAttribute]
        public bool LightingEnabled {
            get { return m_lightingEnabled; }
            set { m_lightingEnabled = value; }
        }
        /// <summary>
        /// shape/intensity for specular reflection, range: 1...128
        /// </summary>
        [XmlAttribute]
        public float Shininess {
            get { return m_shininess; }
            set { m_shininess = value; }
        }
        /// <summary>
        /// color for specular reflection
        /// </summary>
        [ILXmlSerializeAs("{R},{G},{B},{A}")]
        public Color Specular {
            get { return m_specular; }
            set { m_specular = value; }
        }
        /// <summary>
        /// color for emissive reflection
        /// </summary>
        [ILXmlSerializeAs("{R},{G},{B},{A}")]
        public Color Emission {
            get { return m_emission; }
            set { m_emission = value; }
        }
        #endregion

        #region public interface
        /// <summary>
        /// construct new material object, initialize default values
        /// </summary>
        public ILMaterial() {
            m_shininess = 0.2f;
            m_specular = Color.FromArgb(255, Color.Gray);
            m_emission = Color.FromArgb(255, Color.Black); 
            m_lightingEnabled = true; 
        }

        public object Clone() {
            return MemberwiseClone(); 
        }
        #endregion

        #region private helpers

        #endregion
    }
}
