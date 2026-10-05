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
using System.Text;

namespace ILNumerics.Misc {
    internal class ILNRandom {
        private double m_spareRand = 0; 
        private bool m_hasSpareRand = false; 
        private Random m_rand; 

        public ILNRandom (int seed) {
            m_rand = new Random(seed); 
        }
        public ILNRandom() {
            m_rand = new Random(Environment.TickCount); 
        }

        public double NextDouble() {
            if (m_hasSpareRand) {
                m_hasSpareRand = false; 
                return m_spareRand; 
            } else {
                double v1 = 0.0, v2, rsq = 0.0, fac = 0.0;  
                do {
                    v1 = 2.0 * m_rand.NextDouble() -1.0; 
                    v2 = 2.0 * m_rand.NextDouble() -1.0; 
                    rsq = v1 * v1 + v2 * v2; 
                } while (rsq >= 1.0 || rsq == 0.0); 
                fac = Math.Sqrt(-2.0 * Math.Log(rsq) / rsq); 
                m_spareRand = v1 * fac; 
                m_hasSpareRand = true; 
                return v2 * fac; 
            }
        }
        
    }
}