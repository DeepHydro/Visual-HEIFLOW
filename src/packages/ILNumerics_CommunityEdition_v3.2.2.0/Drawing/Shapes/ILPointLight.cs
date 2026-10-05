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
using System.Drawing; 
using System.Text;
using System.Xml.Serialization;

namespace ILNumerics.Drawing {
    /// <summary>
    /// A light in model coordinates 
    /// </summary>
    [Serializable]
    public class ILPointLight : ILNode {

        #region attributes
        public static string DefaultPointLightTag = "PointLight"; 
        private float m_intensity;
        private Vector3 m_position;
        private Color m_color;
        #endregion

        #region properties
        /// <summary>
        /// Light intensity, range: [0 (invisible) ... float.MaxValue]. Default: 1
        /// </summary>
        [XmlAttribute]
        public float Intensity {
            get {
                return m_intensity;
            }
            set {
                if (m_intensity != value) {
                    m_intensity = value;
                    OnPropertyChanged("Intensity");
                }
            }
        }

        /// <summary>
        /// Model coords position for the light. Default: [0,0,0]
        /// </summary>
        [XmlAttribute]
        public Vector3 Position {
            get {
                return m_position;
            }
            set {
                if (m_position != value) {
                    m_position = value;
                    OnPropertyChanged("Position");
                }
            }
        }
        
        /// <summary>
        /// Ligth color, default: Gray
        /// </summary>
        [ILXmlSerializeAs("{R},{G},{B},{A}")]
        public Color Color {
            get {
                return m_color;
            }
            set {
                if (m_color != value) {
                    m_color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        #endregion

        #region constructors
        /// <summary>
        /// Creates a new point light
        /// </summary>
        /// <param name="tag">[optional] tag identifying the light in the scene</param>
        /// <param name="position">[optional] position for the new light</param>
        public ILPointLight(object tag = null, Vector3 position = new Vector3()) : base(tag ?? DefaultPointLightTag) {
            m_position = position; 
            m_intensity = 1f; 
            m_color = Color.Gray; 
        }
        protected ILPointLight() { }
        internal ILPointLight(ILPointLight source) : base (source) {
            m_position = source.m_position;
            m_intensity = source.m_intensity;
            m_color = source.m_color; 
        }
        #endregion

        public override ILNode Detach() { 
            return this;
        }

        internal override ILNode Copy() {
            return new ILPointLight(this); 
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILPointLight copyLight = (ILPointLight) base.Synchronize(copy, syncParams);
            if (copy == null || copyLight.SynchedVersion != Version) {
                copyLight.Color = Color;
                copyLight.Position = Position;
                copyLight.Intensity = Intensity;
            }
            return copyLight; 
        }
        protected override void VisitInternal(ILRenderParameter parameter) {
            base.BeginVisit(parameter);
            if (parameter.CurrentPassCount == 0) {
                Light light = new Light(); 
                light.Color = new Vector3(Color.R / 255f, Color.G / 255f, Color.B / 255f); 
                light.Intensity = this.Intensity; 
                light.Position = parameter.CurrentModel2CameraTransform * new Vector4(Position, 1f);
                //System.Diagnostics.Debug.WriteLine("Point Light VisitInternal - Current Stack Transform: ");
                //System.Diagnostics.Debug.WriteLine(parameter.CurrentTransform.ToString());
                //System.Diagnostics.Debug.WriteLine("Point Light VisitInternal - Transformed Position: ");
                //System.Diagnostics.Debug.WriteLine(light.Position.ToString());
                parameter.Lights.Add(light);
            }
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILPointLight(); 
        }

    }
}
