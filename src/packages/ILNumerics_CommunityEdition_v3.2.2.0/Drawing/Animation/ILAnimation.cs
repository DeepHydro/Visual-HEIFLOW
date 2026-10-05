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
using ILNumerics.Exceptions;

namespace ILNumerics.Drawing.Animation {

    [Serializable]
    public abstract class ILAnimation<NodeT> {
        public abstract void Frame(long ms, NodeT node);
        public object Tag { get; set; }
        internal NodeT Parent { get; set; }
    }

    [Serializable]
    public abstract class ILAnimation<ValueT, NodeT> : ILAnimation<NodeT> where ValueT : struct  {

        protected LoopModes m_loopMode;
        protected bool m_animating = false;
        protected bool m_finished = false; 
        protected ILInterpolator<ValueT> m_interpolator;
        Action<NodeT,ValueT> m_applicatorFunc;
        protected Func<NodeT, ValueT> m_valueOnNullProvider; 

        public event EventHandler Completed;
        protected void OnCompleted() {
            if (Completed != null) {
                Completed(this, EventArgs.Empty);
            }
        }

        public Action<bool> Completion { get; set; }
        public LoopModes LoopMode { get { return m_loopMode; } protected set { m_loopMode = value; } }
        public ILClock LocalClock { get; set; }
        public ILInterpolator<ValueT> Interpolator { 
            get {
                return m_interpolator; 
            }
        }

        public ILAnimation(ILInterpolator<ValueT> interpolator, 
                        Action<NodeT, ValueT> applicatorFunc, 
                        Func<NodeT, ValueT> valueOnNullProvider = null,
                        ILClock localClock = null, 
                        object tag = null, 
                        LoopModes loopMode = LoopModes.Once, 
                        Action<bool> oncompletion = null) {

            m_interpolator = interpolator;
            if (interpolator == null) {
                throw new ILArgumentException("the argument 'interpolator' must not be null");
            }
            m_valueOnNullProvider = valueOnNullProvider; 
            m_interpolator.ValueOnNullProvider = GetValueOnNull; 
            m_applicatorFunc = applicatorFunc; 
            Completion = oncompletion; 
            LoopMode = loopMode; 
            Tag = tag; 
            LocalClock = localClock; 

        }

        public ValueT GetValueOnNull() {
            if (m_valueOnNullProvider != null) {
                return m_valueOnNullProvider(Parent); 
            }
            return default(ValueT); 
        }
        public override void Frame(long time, NodeT node) {
            long ret = time;
            if (LocalClock != null) {
                ret = LocalClock.TimeMilliseconds; 
            }
            long start = m_interpolator.StartTime_ms;
            long end = m_interpolator.EndTime_ms;

            switch (m_loopMode) {
                case LoopModes.Once:
                    if (ret < start) {
                        m_applicatorFunc(node, m_interpolator.StartValue);
                    } else if (ret < end) {
                        m_applicatorFunc(node, m_interpolator.Interpolate(ret));
                    } else {
                        m_applicatorFunc(node, m_interpolator.EndValue);
                    }
                    break;
                case LoopModes.ForwardsBackOnce:
                    ret = end - Math.Abs(end - ret);
                    if (ret < start) {
                        m_applicatorFunc(node, m_interpolator.StartValue);
                    } else {
                        m_applicatorFunc(node, m_interpolator.Interpolate(ret));
                    }
                    break;
                case LoopModes.ForwardsBackLoop:
                    ret = ((ret - start) % ((end - start) * 2)) + start;
                    ret = end - Math.Abs(ret - end);
                    if (ret < start) {
                        m_applicatorFunc(node, m_interpolator.StartValue);
                    } else {
                        m_applicatorFunc(node, m_interpolator.Interpolate(ret));
                    }
                    break;
                case LoopModes.ForwardsLoop:
                    ret = ((ret - start) % (end - start)) + start; 
                    if (ret < start) {
                        m_applicatorFunc(node, m_interpolator.StartValue);
                    } else {
                        m_applicatorFunc(node, m_interpolator.Interpolate(ret));
                    }
                    break;
            }
        }

    }
}
