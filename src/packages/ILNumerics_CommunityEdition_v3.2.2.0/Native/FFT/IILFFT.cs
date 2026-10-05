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
using ILNumerics; 

namespace ILNumerics.Native {

    /// <summary>
    /// Interface for all FFT methods supported
    /// </summary>
    public interface IILFFT {
        /// <summary>
        /// performs backward n-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="nDims">number of dimensions of fft</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<complex> FFTBackward(ILInArray<complex> A, int nDims);
        /// <summary>
        /// performs backward n-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="nDims">number of dimensions of fft</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<fcomplex> FFTBackward(ILInArray<fcomplex> A, int nDims);
        /// <summary>
        /// performs backward 1-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimension to perform fft along</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<fcomplex> FFTBackward1D(ILInArray<fcomplex> A, int dim);
        /// <summary>
        /// performs backward 1-dimensional fft 
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimension to perform fft along</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<complex> FFTBackward1D(ILInArray<complex> A, int dim);
        /// <summary>
        /// performs backward n-dimensional fft on hermitian sequence
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="nDims">number of dimensions of fft</param>
        /// <returns>result, same size as A</returns>
        /// <remarks>This function brings increased performance if the implementation supports it. 
        /// If not, the method will be implemented by repeated calls of (inplace) 1D fft.</remarks>
        ILRetArray<float> FFTBackwSym(ILInArray<fcomplex> A, int nDims);
        /// <summary>
        /// performs backward n-dimensional fft on hermitian sequence
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="nDims">number of dimensions of fft</param>
        /// <returns>result, same size as A</returns>
        /// <remarks>This function brings increased performance if the implementation supports it. 
        /// If not, the method will be implemented by repeated calls of (inplace) 1D fft.</remarks>
        ILRetArray<double> FFTBackwSym(ILInArray<complex> A, int nDims);
        /// <summary>
        /// performs backward 1-dimensional fft on hermitian sequence
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimension to perform fft along</param>
        /// <returns>result, same size as A</returns>
        /// <remarks>This function brings increased performance if the implementation supports it. 
        /// If not, the method will be implemented by repeated calls of (inplace) 1D fft.</remarks>
        ILRetArray<float> FFTBackwSym1D(ILInArray<fcomplex> A, int dim);
        /// <summary>
        /// performs backward 1-dimensional fft on hermitian sequence
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimension to perform fft along</param>
        /// <returns>result, same size as A</returns>
        /// <remarks>This function brings increased performance if the implementation supports it. 
        /// If not, the method will be implemented by repeated calls of (inplace) 1D fft.</remarks>
        ILRetArray<double> FFTBackwSym1D(ILInArray<complex> A, int dim);
        /// <summary>
        /// performs n-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="nDims">number of dimension of fft</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<fcomplex> FFTForward(ILInArray<fcomplex> A, int nDims);
        /// <summary>
        /// performs n-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="nDims">number of dimension of fft</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<complex> FFTForward(ILInArray<double> A, int nDims);
        /// <summary>
        /// performs n-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="nDims">number of dimension of fft</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<complex> FFTForward(ILInArray<complex> A, int nDims);
        /// <summary>
        /// performs n-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="nDims">number of dimension of fft</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<fcomplex> FFTForward(ILInArray<float> A, int nDims);
        /// <summary>
        /// performs 1-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimension to perform fft along</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<complex> FFTForward1D(ILInArray<complex> A, int dim);
        /// <summary>
        /// performs 1-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimension to perform fft along</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<complex> FFTForward1D(ILInArray<double> A, int dim);
        /// <summary>
        /// performs 1-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimension to perform fft along</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<fcomplex> FFTForward1D(ILInArray<fcomplex> A, int dim);
        /// <summary>
        /// performs 1-dimensional fft
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimension to perform fft along</param>
        /// <returns>result, same size as A</returns>
        ILRetArray<fcomplex> FFTForward1D(ILInArray<float> A, int dim);
        /// <summary>
        /// true, if the implementation caches plans between subsequent calls
        /// </summary>
        bool CachePlans { get; }
        /// <summary>
        /// Clear all currently cached plans
        /// </summary>
        void FreePlans();
        /// <summary>
        /// true, if the implementation efficiently transforms from/to hermitian sequences (hermitian symmetry). 
        /// </summary>
        /// <remarks>If this property returns 'true', the implementation brings increased performance. 
        /// If not, the symmetry methods will bring no performance advantage over the 1D transforms. </remarks>
        bool SpeedyHermitian { get; }

    }
}
