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
using System.Collections;
using System.Linq;
using System.Text;
using ILNumerics.Exceptions;

namespace ILNumerics.Drawing.Animation {
    public abstract class ILInterpolator<T> where T : struct{

        protected SortedList<long, Nullable<T>> m_keyframes;
        protected Func<T> m_valueOnNullProvider; 

        public ILInterpolator( params Keyframe<T>[] keyframes) {
            m_keyframes = new SortedList<long, Nullable<T>>();
            m_valueOnNullProvider = () => default(T); 

            if (keyframes == null || keyframes.Length < 2) {
                throw new ILArgumentException("at least two keyframes are required");
            }
            foreach (var item in keyframes) {
                m_keyframes.Add(item.Time_ms,item.Value); 
            }
        }
        public Func<T> ValueOnNullProvider {
            get { return m_valueOnNullProvider; }
            set {
                if (value != null)
                    m_valueOnNullProvider = value;
                else {
                    m_valueOnNullProvider = () => default(T); 
                }
            }
        }
        public long StartTime_ms {
            get {
                return m_keyframes.First().Key; 
            }
        }
        public long EndTime_ms {
            get {
                return m_keyframes.Last().Key;
            }
        }
        public T StartValue {
            get {
                T? val = m_keyframes.First().Value;
                if (val.HasValue) {
                    return val.Value;
                } else {
                    return m_valueOnNullProvider();
                }
            }
        }
        public T EndValue {
            get {
                T? val = m_keyframes.Last().Value;
                if (val.HasValue) {
                    return val.Value;
                } else {
                    return m_valueOnNullProvider(); 
                }
            }
        }

        public T Interpolate(long ms) {
            // find the keyframe this time lays in
            if (ms < StartTime_ms) {
                return StartValue; 
            } else if (ms < EndTime_ms) {
                int i = -1;
                foreach (var a in m_keyframes) {
                    if (a.Key > ms) {
                        return InterpolateInternal(i, ms);  
                    } 
                    ++i;
                }
            }
            return EndValue; 
        }
        /// <summary>
        /// the actual interpolation between two keyframes, concrete class can rely on ms laying between two existing keyframes
        /// </summary>
        /// <param name="keyframeStartIDX">the index of the keyframe _before_ ms </param>
        /// <param name="ms">current time</param>
        /// <returns>interpolated value for ms</returns>
        protected abstract T InterpolateInternal(int keyframeStartIDX, long ms);
        public virtual void Recreate() { }

        public SortedList<long,T?> Keyframes {
            get {
                return m_keyframes; 
            }
        }
    }
}
