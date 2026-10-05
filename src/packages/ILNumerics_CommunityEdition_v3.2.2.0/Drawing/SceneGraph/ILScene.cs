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
# define HIDE_BRANDING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing; 
using OpenTK;
using System.ComponentModel; 
using System.Runtime.Serialization; 

namespace ILNumerics.Drawing {
    /// <remarks>The root node of all ILNumerics scene graphs.</remarks>
    [Serializable]
    public class ILScene : ILGroup {

        #region attributes
        [ThreadStatic]
        internal ILSyncParams SyncParams;
        internal static long StartTimeMS { get; set; }

        private Color m_ambientLight;
        private float? m_maxLightIntensity;

        public static readonly string CameraTag = "DefaultCamera";
        public static readonly string ScreenTag = "Screen"; 
        public static readonly string LightGroupTag = "DefaultLightGroup";
        public static readonly string LightTag = "DefaultLightTag";
        public static readonly string SelectionsGroupTag = "Selections"; 
        #endregion

        #region constructors
        /// <summary>
        /// Creates a scene from a given scene graph group node 
        /// </summary>
        /// <param name="root">existing scene graph root</param>
        /// <remarks><para>This constructor will create <b>a shallow copy</b> of the existing <paramref name="root"/> node and its 
        /// children. The copy is done efficiently without copying any buffer data.</para></remarks>
        public ILScene(ILGroup root)
            : base(root) {

        }
        /// <summary>
        /// Create a complete shallow copy of the complete scene 
        /// </summary>
        /// <param name="source">source scene</param>
        public ILScene(ILScene source)
            : base(source) {
            this.m_ambientLight = source.m_ambientLight; 
            this.m_maxLightIntensity = source.m_maxLightIntensity; 

        }
        /// <summary>
        /// Construct a new empty scene
        /// </summary>
        /// <param name="localScene">True: do not create global defaults (Camera, Light, etc.)</param>
        public ILScene(bool localScene = false) : base() {
            if (!localScene) {
                Add(new ILCamera(), CameraTag);
            }
            Add(new ILGroup(), ScreenTag);
            // make it a default screen: [0..1]
            Screen.Scale(2, -2, 1).Translate(-1, 1, 0);
            AmbientLight = Color.FromArgb(1, 120, 120, 120);
            MaxLightIntensity = null;
            if (!localScene) {
                // add single point light as default
                Add(new ILGroup(LightGroupTag)).Translate(0, 0, 10).Add(new ILPointLight(LightTag));
#if !HIDE_BRANDING
                var l = Screen.Add(new ILLabel("powered by: \\color{#444444}IL\\color{#444444}Nu\\color{#444444}merics") {
                    Anchor = new PointF(1.1f, 1.06f),
                    Color = Color.FromArgb(180, 180, 180),
                    Font = new Font("Verdana", 8f),
                    Position = new Vector3(1, 1, 0),
                    Fringe = { Width = 1 }
                });
                Action<object, ILMouseEventArgs, System.Windows.Forms.Cursor> handler = (s, a, p) => {
                    var form = System.Windows.Forms.Form.ActiveForm; 
                    if (form != null) {
                        form.Cursor = p;
                    }
                    a.Cancel = true;
                };
                l.MouseEnter += (s, a) => handler(s, a, System.Windows.Forms.Cursors.Hand);
                l.MouseLeave += (s, a) => handler(s, a, System.Windows.Forms.Cursors.Default);
                l.MouseClick += (s, a) => { System.Diagnostics.Process.Start("http://numeric-computing.com"); a.Cancel = true; }; 
#endif
            } else {
                Add(new ILGroup(SelectionsGroupTag)); 
            }
            StartTimeMS = DateTime.Now.ToFileTime() / 10000; 
        }
        /// <summary>
        /// create a clone of the current scene state (driver-/render task)
        /// </summary>
        /// <returns>Clone</returns>
        internal ILGroup Snapshot(long ms, ILGroup target) {
            if (SyncParams == null) 
                SyncParams = new ILSyncParams();
            SyncParams.Buffers.Clear();
            SyncParams.BufferSets.Clear();
            SyncParams.CurrentTime = ms; 
            lock (StructureLock) {
                if (target != null && target.ID != ID) {
                    // scene has been exchanged. Dispose old scene before overwriting
                    target.Dispose(); 
                    target = null; 
                }
                target = (ILGroup)Synchronize(target, SyncParams);
            }
            return target;
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILScene(true);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ambient light color or sets it
        /// </summary>
        public Color AmbientLight {
            get {
                return m_ambientLight;
            }
            set {
                if (m_ambientLight != value) {
                    m_ambientLight = value;
                    OnPropertyChanged("AmbientLight");
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum intensity for ligths, used for scaling global lighting
        /// </summary>
        public float? MaxLightIntensity {
            get {
                return m_maxLightIntensity;
            }
            set {
                if (m_maxLightIntensity != value) {
                    m_maxLightIntensity = value;
                    OnPropertyChanged("MaxLightIntensity");
                }
            }
        }

        /// <summary>
        /// Gets a reference to the default camera in the scene
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILCamera Camera { get { return First<ILCamera>(CameraTag); } }

        /// <summary>
        /// Gets a reference to the default Screen2D node of the scene. It establishes a 2D coordinate system.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILGroup Screen { get { return First<ILGroup>(ScreenTag); } }
        
        #endregion

    }
}
