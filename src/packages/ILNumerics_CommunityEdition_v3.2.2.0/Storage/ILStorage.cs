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

namespace ILNumerics.Storage {
    /// <summary>
    /// Background storage object used internally.
    /// </summary>
    [Serializable]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ILStorage { 
        
        /// <summary>
        /// size of this storage
        /// </summary>
        protected ILSize m_size;
        /// <summary>
        /// Size of the storage
        /// </summary>
        public ILSize Size { 
            get { return m_size; }
        }
        internal ILStorage(ILSize dimensions) {
            m_size = dimensions;
        }

        internal abstract StringBuilder ValuesToString(int len);
        internal abstract ILStorage Clone();
        internal abstract bool IsDisposed { get; } 
        internal abstract void SetValue(object value, int[] idx); 
        internal abstract object GetValue(params int[] idx); 
        internal abstract object GetValueSeq(int idx, ref int[] dims);
        internal abstract ILStorage GetValueAsStorage(params int[] innerIndices);
        internal abstract ILBaseArray GetAsBaseArray(); 
        internal abstract bool Equals(object storage); 
        internal abstract void Dispose(bool manual); 
        internal virtual String ShortInfo() {
            StringBuilder ret = new StringBuilder(20); 
            Type type = GetType(); 
            if (IsDisposed)
                return "(disposed)";
            if (type.GetGenericArguments() != null && type.GetGenericArguments().Length > 0) {
                ret.Append("<"+type.GetGenericArguments()[0].Name+"> "); 
            } else {
                ret.Append(GetType().Name + " ");
            }
            if (m_size.NumberOfElements == 1) {
                ILStorage val = GetValueAsStorage(0);
                if (val is ILCellStorage) 
                    ret.Append((val as ILCellStorage).ToString()); 
                else 
                    ret.Append(val.ToString());
            } else 
                ret.Append(Size.ToString()); 
            return ret.ToString(); 
        }

        internal void Dispose() {
            Dispose(true); 
        }
    }
}
