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
using System.Xml.Serialization;
namespace ILNumerics.Drawing {
    [Serializable]
    public abstract class ILBufferBase {

        protected ILBufferBase() {}
        public abstract bool IsEmpty { get; }

        [XmlAttribute]
        public BufferType BufferType { get; protected set; }
        /// <summary>
        /// Fires before the buffer gets disposed
        /// </summary>
        public event EventHandler<EventArgs> Disposing;
        protected void OnDisposing() {
            // NOTE: this is executed WITHIN the write lock from Update! 
            if (Disposing != null) {
                Disposing(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires once the buffer has been changed
        /// </summary>
        public event EventHandler<ILBufferChangedEventArgs> Changed;
        protected void OnChanged(int start, int length, ChangeQueueActions action) {
            // NOTE: this is executed WITHIN the write lock from Update! 
            if (Changed != null) {
                Changed(this, new ILBufferChangedEventArgs(start, length, BufferType, action));
            }
        }
        protected void OnChanged(ILBufferChangedEventArgs args) {
            // NOTE: this is executed WITHIN the write lock from Update! 
            if (Changed != null) {
                Changed(this, args);
            }
        }
        public abstract int DataCount { get; }
        /// <summary>
        /// Dimensionality of a single date in this buffer (number of rows or length of each vector)
        /// </summary>
        public abstract int DataLength { get; }
        [XmlAttribute]
        public int ID { get; protected set; }
        internal abstract bool Synchronize(ILBufferBase copy, ILSyncParams synParams, ref ILBufferBase sync); 
        internal abstract void DecreaseReference();
        internal abstract void IncreaseReference();
        internal abstract int ReferenceCount { get;  }

        public static string Debug(ILBufferBase buffer) {
             return String.Format("Hash:{0} ID:{1} Attached:{2}", buffer.GetHashCode(), buffer.ID, (buffer.Changed != null) ? buffer.Changed.GetInvocationList().Length : 0); 
        }

    }
}
