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
using ILNumerics.Storage;
using System.Linq.Expressions;
using ILNumerics.Data; 

namespace ILNumerics.Misc {
    internal sealed class ILRegularRange {

        ILBaseArray m_startA; 
        ILBaseArray m_stepA; 
        ILBaseArray m_endA;
        int m_start; 
        int m_step; 
        int m_end;
        bool m_isEvaluated = false; 
        bool m_basedOnEndPlaceholder = false; 

        internal ILRegularRange(ILBaseArray start, ILBaseArray step, ILBaseArray end) { 
            m_startA = start; 
            m_stepA = step; 
            m_endA = end; 
            //// make sure, Length will always be positive! 
            //if ((m_start > m_end && m_step > 0) || 
            //    (m_start < m_end && m_step < 0)) {
            //    throw new Exceptions.ILArgumentException(String.Format("invalid range: check start:step:end! was: [{0}:{1}:{2}]",m_start,m_step,m_end));
            //}
            //if (m_step == 0)  throw new Exceptions.ILArgumentException("invalid range: step must not equal 0");
        }
        internal ILRegularRange(int start, int end) {
            m_startA = start; 
            m_stepA = 1; 
            m_endA = end; 
            // make sure, Length will always be positive! 
            //if (m_start > m_end) throw new Exceptions.ILArgumentException("invalid range: start must be lower as or equal to end"); 
        }
        internal void Evaluate(int dimLength) {
            if (m_isEvaluated) return; 
            m_step = ILMath.toint32(m_stepA).GetValue(0);
            evaluateSingle(ref m_start, m_startA, dimLength); 
            evaluateSingle(ref m_end, m_endA, dimLength); 
            if (m_step == 0 || Math.Sign((m_end-m_start)/m_step) < 0) {
                Length = 0;
            } else if (m_basedOnEndPlaceholder && dimLength < 0) {
                Length = 0; 
            } else {
                Length =  (m_end-m_start) / m_step + 1; // ?? was: (int)Math.Ceiling((m_end - m_start) / (float)m_step) + 1;
            }
            m_isEvaluated = true; 
        }
        private void evaluateSingle(ref int parameter, ILBaseArray input, int dimLength) {
            if (input is ILBaseArray<Expression>) {
                parameter = ILExpression.Evaluate((input as ILBaseArray<Expression>).GetValue(0),dimLength); 
                m_basedOnEndPlaceholder = true; 
            } else {
                parameter = ILMath.toint32(input).GetValue(0); 
            }
        }
        internal void Expand(ILIntList target, ref int outLen, ref int min, ref int max, int dimLen) {
            if (!m_isEvaluated) 
                Evaluate(dimLen); 
            if (Length <= 0 || Math.Sign((m_end-m_start)/m_step) < 0) 
                return; 

            int i = m_start;
            if (m_start == m_end) {
                target.Add(i); 
            } else if (m_start < m_end) {
                do {
                    target.Add(i);     
                    i += m_step;
                } while (i <= m_end); 
            } else {
                do {
                    target.Add(i);     
                    i += m_step;
                } while (i >= m_end); 
            }

            int _min = Math.Min(m_start,m_end); 
            if (_min < min) min = _min; 
            int _max = Math.Max(m_start,m_end); 
            if (_max > max) max = _max; 
            outLen += Length; 
        }
        public int Length { get; private set; }

        public override string ToString() {
            return String.Format("[{0}:{1}:{2}]",m_start,m_step,m_end);
        }

        internal void Extract<T>(T[] source, T[] dest, int dimLen) {
            if (!m_isEvaluated) 
                Evaluate(dimLen); 
            if (Math.Sign((m_end-m_start)/m_step) < 0) 
                return; 

            int i = m_start;
            int pos = 0;
            if (m_start < m_end) {
                do {
                    dest[pos++] = source[i];
                    i += m_step;
                } while (i <= m_end);
            } else if (m_start > m_end) {
                do {
                    dest[pos++] = source[i];
                    i += m_step;
                } while (i >= m_end);
            } else {
                // start == end
                dest[0] = source[m_start];
            }
        }
    }

}
