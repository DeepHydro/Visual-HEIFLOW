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
using ILNumerics.Data;
using System.Xml.Serialization;

namespace ILNumerics.Drawing {
    /// <summary>
    /// Base class for drawable scene node objects, like shapes and labels
    /// </summary>
    [Serializable]
    public abstract class ILDrawable : ILNode {

        #region attributes
        private Color? m_color;
        #endregion

        #region ctors  
        /// <summary>
        /// Creates a new drawable object as clone of an existing one
        /// </summary>
        /// <param name="source">Source object</param>
        protected ILDrawable(ILDrawable source) : base(source) {
            Color = source.Color; 
        }
        /// <summary>
        /// Creates a new drawable object, alternatively specify a tag (recommended).
        /// </summary>
        /// <param name="tag">tag, describing the object</param>
        public ILDrawable(object tag = null) : base(tag) {
            // prefer custom handlers over protected overrides!
            MouseEnter += (s, e) => {
                if (Markable && e.Target.ID == this.ID) {
                    Marked = true;
                    e.Refresh = true; 
                }
            };
            MouseLeave += (s, e) => {
                if (Markable && e.Target.ID == this.ID) {
                    Marked = false;
                    e.Refresh = true; 
                }
            };
        }
        #endregion

        #region properties
        /// <summary>
        /// Any color (except null) will override individual object vertex colors. 
        /// </summary>
        [ILXmlSerializeAs("{R},{G},{B},{A}")]
        public virtual Color? Color {
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
        /// <summary>
        /// determine transparency state for this shape
        /// </summary>
        [XmlIgnore]
        internal virtual bool IsTransparent {
            get {
                return Color.HasValue && Color.Value.A < 255;
            }
        }
        #endregion

        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILDrawable ret = (ILDrawable)base.Synchronize(copy, syncParams);
            if (ret.SynchedVersion != Version) {
                ret.Color = Color; 
            }
            return ret; 
        }
        /// <summary>
        /// Get the anchor position for this object 
        /// </summary>
        /// <returns>The current anchor position</returns>
        public abstract Vector3 GetPosition();

        //#region methods needed by BSP tree implementation
        ////internal abstract void ToBSPBuildPrimitives<T>(IList<BSPPrimitive<T>> primitives, Matrix4 model2CameraTransform, Matrix4 camera2ClipTransform, T data);
        ////internal abstract void FromBSPBuildPrimitives(IEnumerable<BSPPrimitive<ILDrawable>> primitive); 
        //#endregion

    }
}
