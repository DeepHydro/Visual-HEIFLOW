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
using System.Threading; 
using System.Diagnostics; 
using ILNumerics; 
using ILNumerics.Storage; 
using ILNumerics.Misc;
using System.Xml.Serialization; 

namespace ILNumerics.Drawing {
    [DebuggerTypeProxy(typeof(ILBufferDebuggerProxy<>))]
    [DebuggerDisplay("{m_storage.ShortInfo(),nq}")]
    [Serializable]
    public abstract class ILBuffer<T> : ILBufferBase, IDisposable {


        #region attributes
        internal object Lock = new object();
        internal int Version;
        protected int m_referenceCount = 0;
        protected ILArray<T> m_storage;
        protected int m_dataLength; 

        static internal int GlobalBufferIDCount; 
        #endregion

        #region properties
        /// <summary>
        /// returns a copy(!) of the buffers array storage, using a lazy-copy-on-write scheme. Use Update() for modifications! 
        /// </summary>
        public ILRetArray<T> Storage {
            get {
                lock (Lock)
                    return m_storage.C;
            }
        }
        /// <summary>
        /// Number of data vectors (columns)
        /// </summary>
        [XmlAttribute]
        public override int DataCount {
            get {
                lock (Lock)
                    return m_storage.S[1];
            }
        }
        /// <summary>
        /// Dimensionality of a single date in this buffer (number of rows or length of each vector)
        /// </summary>
        [XmlAttribute]
        public override int DataLength {
            get {
                return m_dataLength; 
            }
        }
        /// <summary>
        /// Number of objects currently using this buffer
        /// </summary>
        [XmlIgnore]
        internal override int ReferenceCount {
            get { return m_referenceCount; }
        }
        [XmlIgnore]
        public bool IsDisposed { get; protected set; }
        [XmlIgnore]
        public override bool IsEmpty { get { return m_storage.IsEmpty; } }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new empty buffer.
        /// </summary>
        protected ILBuffer(int dataLength) : base() {
            ID = System.Threading.Interlocked.Increment(ref ILBuffer<float>.GlobalBufferIDCount); 
            IsDisposed = false;
            m_dataLength = dataLength; 
            m_storage = ILMath.localMember<T>();
            m_storage.a = ILMath.empty<T>(dataLength, 0);
        }
        //private int mantid() {
        //    return System.Threading.Thread.CurrentThread.ManagedThreadId; 
        //}
        #endregion

        #region public interface
        //public void Update(int startColumn, int count, ILInArray<double> data) {
        //    using (ILScope.Enter(data)) {
        //        Update(startColumn, count, convert<double,T>(data));
        //    }
        //}
        public virtual void Update(ILInArray<T> data) {
            using (ILScope.Enter(data)) {
                int oldCount = DataCount;
                if (ILMath.isnull(data) || ILMath.isempty(data)) {
                    m_storage.a = ILMath.empty<T>(m_dataLength, 0);
                    Version++;
                    OnChanged(0, oldCount, ChangeQueueActions.Delete);  // todo: make sure, buffered driver react on Delete changes properly
                } else {
                    ILArray<T> data_ = data;
                    if (DataLength == 1 && data.IsColumnVector) {
                        data_.a = data_.T;
                    }
                    if (data_.S[0] != DataLength)
                        throw new Exceptions.ILArgumentException(String.Format("Invalid data size. Data for a buffer of type {0} must provide data columns of length {1}. Found: {2}", BufferType, DataLength, data.S.ToString()));
                    m_storage[ILMath.full, ILMath.r(0, data_.S[1] - 1)] = data_;
                    if (DataCount > data.S[1]) {
                        m_storage[ILMath.full, ILMath.r(data_.S[1], ILMath.end)] = null;
                    }
                    Version++;
                    OnChanged(0, DataCount, ChangeQueueActions.Update);
                }
                // triggers Invalidated & buffered driver change queue
            }
        }
        public void Update(int startColumn, int count, ILInArray<T> data) {
            lock (Lock) {
                using (ILScope.Enter(data)) {
                    ILArray<T> data_ = data;
                    if (DataLength == 1 && data.IsColumnVector) {
                        data_.a = data_.T;
                        count = data_.S[1]; 
                    }
                    if (data_.S.NumberOfElements < count * DataLength)
                        throw new Exceptions.ILArgumentException(String.Format("Invalid data size. Data provided to a buffer of type {0} need at minimum 'count' data columns of length {1}. Found: {2}", BufferType, DataLength, data.S.ToString()));
                    if (count > 1 && data_.IsVector)
                        data_.a = ILMath.reshape<T>(data_, DataLength, count);
                    m_storage[ILMath.full, ILMath.r(startColumn, startColumn + count - 1)] = data_[ILMath.full, ILMath.r(0, count - 1)];
                    // triggers Invalidated & buffered driver change queue
                    Version++; 
                    OnChanged(startColumn, count, ChangeQueueActions.Update);
                }
            }
        }

        internal override void IncreaseReference() {
            System.Threading.Interlocked.Increment(ref m_referenceCount);
        }
        internal override void DecreaseReference() {
            System.Threading.Interlocked.Decrement(ref m_referenceCount); 
            if (ReferenceCount <= 0) {
                Dispose(); 
            }
        }

        public void Dispose() {
            lock (Lock) {
                if (IsDisposed || ReferenceCount > 0) return;
                OnDisposing();
                IsDisposed = true;
                m_storage.Dispose();
            } 
        }
        public abstract ILBuffer<T> Copy();
        #endregion


        #region ILBufferBase Members


        internal override bool Synchronize(ILBufferBase copy_, ILSyncParams syncParams, ref ILBufferBase newBuffer) {
            // IDs? 
            ILBuffer<T> copy = (ILBuffer<T>)copy_;
            if (ID != copy.ID) {
                // buffer has been replaced: create new copy or new reference to existing copy
                if (syncParams.Buffers.ContainsKey(ID)) {
                    newBuffer = (ILBuffer<T>)syncParams.Buffers[ID];
                } else {
                    // create first and only detached reference of this buffer 
                    ILBuffer<T> buffer = Copy();
                    buffer.ID = ID;
                    newBuffer = buffer;
                    syncParams.Buffers.Add(ID, newBuffer);
                    buffer.MonitorsChangesFrom(this);
                }
                return true;  // <- signal to replace existing with new buffer
            }
            // Version matching? 
            if (Version != copy.Version) {
                //System.Diagnostics.Debug.Assert(syncParams.Buffers.ContainsKey(ID) == false);
                copy.m_storage.a = m_storage.C;
                copy.Version = Version;
                return false;
            }
            return false;
        }
        internal void MonitorsChangesFrom(ILBuffer<T> target) {
            target.Changed += new EventHandler<ILBufferChangedEventArgs>(TargetChanged_Handler);
            target.Disposing += new EventHandler<EventArgs>((s, e) => {
                // TODO: check if this is really needed. Possibly, the target buffer and this synced 
                // buffer will be GCed anyway...? 
                target.Changed -= new EventHandler<ILBufferChangedEventArgs>(TargetChanged_Handler);
            });
        }
        protected void TargetChanged_Handler(object sender, ILBufferChangedEventArgs eventargs) {
            lock (Lock) {
                OnChanged(eventargs);
            }
        }
        #endregion

    }
}
 