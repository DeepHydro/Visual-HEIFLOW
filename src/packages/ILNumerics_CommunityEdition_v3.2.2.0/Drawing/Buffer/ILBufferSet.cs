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
using ILNumerics.Exceptions;
using System.Xml.Serialization;

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILBufferSet {

        #region Events
        public event EventHandler TransparencyChanged;
        protected void OnTransparencyChanged() {
            if (TransparencyChanged != null) {
                TransparencyChanged(this, EventArgs.Empty); 
            }
        }
        #endregion

        #region attributes
        private ILColorsBuffer m_colorsBuffer;
        private ILPositionsBuffer m_positionsBuffer;
        private ILNormalsBuffer m_normalsBuffer;
        private ILIndicesBuffer m_indicesBuffer;
        private Color? m_color;
        private ILLimits m_limits; 

        internal bool m_colorsChanged; 
        internal bool m_positionsChanged; 
        internal bool m_normalsChanged; 
        internal bool m_indicesChanged; 
        
        public readonly object UpdateLock = new object();
        public readonly object ConfigureLock = new object();
        public readonly object DrawLock = new object();
        internal int Ready4Publish;
        internal int Version;
        internal int m_referenceCount;
        private bool m_transpencyFlag;
        private Dictionary<int, List<int>> m_shapeIndicesIndex; 
        #endregion

        #region properties
        [XmlIgnore]
        public Dictionary<int, List<int>> ShapeIndicesIndex {
            get {
                return m_shapeIndicesIndex; 
            }
            set {
                m_shapeIndicesIndex =value; 
            }
        }

        [XmlIgnore]
        internal bool ConfigureFlag { get; set; }
        [XmlIgnore]
        public bool IsDisposed { get; private set; }
        [XmlIgnore]
        internal bool TransparencyFlag {
            get { return m_transpencyFlag; }
            set {
                if (m_transpencyFlag != value) {
                    m_transpencyFlag = value; 
                    OnTransparencyChanged(); 
                }
            }
        }
        internal ILLimits Limits {
            get {
                if (Ready4Publish == 0) {
                    Configure(); 
                }
                return m_limits; 
            }
            private set {
                if (!object.Equals(value,null)) {
                    m_limits = value; 
                }
            }
        }
        public Lighting.ILMaterial Material { get; protected set; }
        [XmlAttribute]
        public float Width { get; set; }
        [ILXmlSerializeAs("{R},{G},{B},{A}")]
        public Color? Color {
            get { return m_color; }
            set {
                lock (UpdateLock) {
                    m_color = value;
                    m_colorsChanged = true;
                    Ready4Publish = 0;
                    Version++;
                }
            }
        }
        public ILColorsBuffer Colors {
            get {
                return m_colorsBuffer;
            }
            set {
                lock (UpdateLock) {
                    m_colorsBuffer = (ILColorsBuffer)ReplaceBuffer(m_colorsBuffer, value, (t,e) => { 
                        m_colorsChanged = true; 
                        Ready4Publish = 0;
                        Version++;
                    });
                    m_colorsChanged = true;
                    Ready4Publish = 0;
                    Version++;
                }
            }
        }
        public ILPositionsBuffer Positions {
            get {
                return m_positionsBuffer;
            }
            set {
                lock (UpdateLock) {
                    m_positionsBuffer = (ILPositionsBuffer)ReplaceBuffer(m_positionsBuffer, value, (t, e) => {
                        m_positionsChanged = true;
                        Ready4Publish = 0;
                        Version++;
                    });
                    m_positionsChanged = true;
                    Ready4Publish = 0;
                    Version++;

                }
            }
        }
        public ILNormalsBuffer Normals {
            get {
                return m_normalsBuffer;
            }
            set {
                lock (UpdateLock) {
                    m_normalsBuffer = (ILNormalsBuffer)ReplaceBuffer(m_normalsBuffer, value, (t, e) => {
                        m_normalsChanged = true;
                        Ready4Publish = 0;
                        Version++; 
                    });
                    m_normalsChanged = true;
                    Ready4Publish = 0;
                    Version++;
                }
            }
        }
        public ILIndicesBuffer Indices {
            get {
                return m_indicesBuffer;
            }
            set {
                lock (UpdateLock) {
                    m_indicesBuffer = (ILIndicesBuffer)ReplaceBuffer(m_indicesBuffer, value, (t, e) => { 
                        m_indicesChanged = true;
                        m_shapeIndicesIndex = null; 
                        Ready4Publish = 0;
                        Version++;
                    });
                    m_indicesChanged = true;
                    m_shapeIndicesIndex = null;
                    Ready4Publish = 0;
                    Version++;
                }
            }
        }
        [XmlIgnore]
        public int ReferenceCount { get { return m_referenceCount; } }
        #endregion

        #region constructors
        public ILBufferSet() {
            ConfigureFlag = false; 
            TransparencyFlag = false; 
            Positions = new ILPositionsBuffer(); 
            Colors = new ILColorsBuffer(); 
            Indices = new ILIndicesBuffer(); 
            Normals = new ILNormalsBuffer(); 
            Limits = new ILLimits(); 
            Material = new Lighting.ILMaterial(); 
        }
        #endregion

        #region public interface
        //public void UpdateColors(int start, int numDataPoints, ILInArray<float> data) {
        //    lock (UpdateLock) {
        //        if (Colors == null) {
        //            Colors = new ILColorsBuffer(); 
        //        }
        //        Colors.Update(start, numDataPoints, data);
        //    }
        //}
        //public void UpdatePositions(int start, int numDataPoints, ILInArray<float> data) {
        //    lock (UpdateLock) {
        //        if (Positions == null) {
        //            Positions = new ILPositionsBuffer();
        //        }
        //        Positions.Update(start, numDataPoints, data);
        //    }
        //}
        //public void UpdateNormals(int start, int numDataPoints, ILInArray<float> data) {
        //    lock (UpdateLock) {
        //        if (Normals == null) {
        //            Normals = new ILNormalsBuffer();
        //        }
        //        Normals.Update(start, numDataPoints, data);
        //    }
        //}
        //public void UpdateIndices(int start, int numDataPoints, ILInArray<int> data) {
        //    lock (UpdateLock) {
        //        if (Indices == null) {
        //            Indices = new ILIndicesBuffer();
        //        }
        //        Indices.Update(start, numDataPoints, data);
        //    }
        //}
        internal void IncreaseReference() {
            System.Threading.Interlocked.Increment(ref m_referenceCount);
        }
        internal void DecreaseReference() {
            System.Threading.Interlocked.Decrement(ref m_referenceCount);
            if (m_referenceCount <= 0) {
                Dispose();
            }
        }
        public void Dispose() {
            lock (UpdateLock) {
                if (!IsDisposed) {
                    
                    IsDisposed = true;
                    Positions = null;
                    Colors = null;
                    Normals = null;
                    Indices = null;
                    Limits = null; 
                }
            }
        }
        public void Configure() {
            if (Ready4Publish > 0)
                return;
            lock (UpdateLock) {
                if (Ready4Publish > 0)
                    return;
                if (m_indicesChanged) {
                    Computation.ComputeBoundingBox(Positions, Indices, m_limits);
                    m_indicesChanged = false;
                    m_positionsChanged = false;
                    if (Color.HasValue) {
                        TransparencyFlag = (Color.Value.A < 0xff);
                    } else if (!Colors.IsEmpty) {
                        TransparencyFlag = Computation.ComputeIsTransparent(Colors, Indices);
                    } else {
                        TransparencyFlag = false;
                    }
                    m_positionsChanged = false;
                } else {
                    if (m_positionsChanged) {
                        Computation.ComputeBoundingBox(Positions, Indices, m_limits);
                        m_positionsChanged = false;
                    }
                    if (m_colorsChanged) {
                        if (Color.HasValue) {
                            TransparencyFlag = (Color.Value.A < 0xff);
                        } else if (!Colors.IsEmpty) {
                            TransparencyFlag = Computation.ComputeIsTransparent(Colors, Indices);
                        } else {
                            TransparencyFlag = false;
                        }
                        m_colorsChanged = false; 
                    }
                }
                Ready4Publish = 1;
                Version++;
            }
        }

        /// <summary>
        /// Creates (lazy) copy of this buffer set which allows local changes without affecting referenced buffer sets 
        /// </summary>
        internal ILBufferSet Copy() {
            lock (UpdateLock) {
                ILBufferSet ret = new ILBufferSet();
                ret.Positions = (ILPositionsBuffer)Positions.Copy();  // this is reference counting aware
                ret.Normals = (ILNormalsBuffer)Normals.Copy();
                ret.Colors = (ILColorsBuffer)Colors.Copy();
                ret.Indices = (ILIndicesBuffer)Indices.Copy();
                ret.ConfigureFlag = ConfigureFlag; 
                ret.TransparencyFlag = TransparencyFlag; 
                ret.Limits = Limits.Clone();
                ret.Material = (Lighting.ILMaterial)Material.Clone(); 
                ret.Version = Version; 
                return ret; 
            }

        }
        internal void Synchronize(ILBufferSet copy, ILSyncParams syncParams) {
            if (Ready4Publish < 1 || Version == copy.Version) {
                return; 
            }
            lock (UpdateLock) {
                if (Ready4Publish > 0 && Version != copy.Version) {
                    copy.Limits.Set(Limits.Min, Limits.Max);
                    copy.IsDisposed = IsDisposed; 
                    copy.Material = Material ?? null; 
                    copy.TransparencyFlag = TransparencyFlag; 
                    ILBufferBase newBuffer = null;
                    // we dont want the common event handling in the synchronized tree..
                    if (Positions.Synchronize(copy.Positions, syncParams, ref newBuffer)) {
                        copy.m_positionsBuffer = (ILPositionsBuffer)ReplaceBuffer(copy.m_positionsBuffer, (ILPositionsBuffer)newBuffer, null);
                    }
                    if (Colors.Synchronize(copy.Colors, syncParams, ref newBuffer)) {
                        copy.m_colorsBuffer = (ILColorsBuffer)ReplaceBuffer(copy.m_colorsBuffer, (ILColorsBuffer)newBuffer, null);
                    }
                    if (Indices.Synchronize(copy.Indices, syncParams, ref newBuffer)) {
                        copy.m_indicesBuffer = (ILIndicesBuffer)ReplaceBuffer(copy.m_indicesBuffer, (ILIndicesBuffer)newBuffer, null);
                    }
                    if (Normals.Synchronize(copy.Normals, syncParams, ref newBuffer)) {
                        copy.m_normalsBuffer = (ILNormalsBuffer)ReplaceBuffer(copy.m_normalsBuffer, (ILNormalsBuffer)newBuffer, null);
                    }
                    copy.Version = Version; 
                }
            }
        }
        #endregion

        #region private helper
        private ILBuffer<T> ReplaceBuffer<T>(ILBuffer<T> curBuffer, ILBuffer<T> newBuffer, EventHandler<EventArgs> handler) {
            if (object.ReferenceEquals(newBuffer,curBuffer)) return newBuffer; 
            lock (UpdateLock) {
                if (newBuffer != null) {
                    newBuffer.IncreaseReference();
                    if (handler != null)
                        newBuffer.Changed += new EventHandler<ILBufferChangedEventArgs>(handler);
                    newBuffer.Lock = UpdateLock; 
                }
                if (curBuffer != null) {
                    if (handler != null)
                        curBuffer.Changed -= new EventHandler<ILBufferChangedEventArgs>(handler);
                    curBuffer.DecreaseReference();
                    curBuffer.Lock = new object(); 
                } 
                return newBuffer;
            }
        }

        #endregion

        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {
            // executed in locked context of position buffer and indices buffer
            public static bool ComputeBoundingBox(ILPositionsBuffer buffer, ILIndicesBuffer indices, ILLimits outClip) {
                using (ILScope.Enter()) {
                    ILArray<float> pos;
                    if (!indices.IsEmpty) {
                        pos = buffer.Storage[full, indices.Storage];
                    } else {
                        pos = buffer.Storage;
                    }
                    ILArray<float> maxPos = max(pos, dim: 1);
                    ILArray<float> minPos = min(pos, dim: 1);
                    // early exit ??
                    if (!isempty(maxPos) && !isempty(minPos)) {
                        outClip.Set(new Vector3(minPos.GetValue(0), minPos.GetValue(1), minPos.GetValue(2)),
                                    new Vector3(maxPos.GetValue(0), maxPos.GetValue(1), maxPos.GetValue(2)));
                    }
                    return true;

                }
            }
            public static bool ComputeIsTransparent(ILColorsBuffer buffer, ILIndicesBuffer indices) {
                using (ILScope.Enter()) {
                    ILArray<float> col;
                    if (!indices.IsEmpty) {
                        col = buffer.Storage[full, indices.Storage];
                    } else {
                        col = buffer.Storage;
                    }
                    return any(col[3, full] < 1.0f);
                }
            }

        }

    }
}
