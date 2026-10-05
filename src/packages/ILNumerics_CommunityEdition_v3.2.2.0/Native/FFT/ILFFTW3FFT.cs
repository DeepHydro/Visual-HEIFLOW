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


#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using System.Security; 
using System.Runtime.InteropServices;
using ILNumerics.Exceptions;  
using ILNumerics.Misc; 

namespace ILNumerics.Native {
    
    /// <summary>
    /// Wrapper for FFT interface using FFTW3 libs
    /// </summary>
    public unsafe class ILFFTW3FFT : IILFFT, IDisposable {

        #region pinvoke definitions 
        
        /// <summary>
        /// This struct is used to define (n-dimensional) transform sizes
        /// </summary>
        /// <remarks>This struct is only user in C-API. It is NOT used for the Fortran interface (i.e. it's not used in here)!!! </remarks>
        private struct fftw_iodim {
            /// <summary>
            /// length of dimension
            /// </summary>
            public int n;
            /// <summary>
            /// input stride
            /// </summary>
            public int iS;
            /// <summary>
            /// output stride
            /// </summary>
            public int oS;
        }

        #region DOUBLE PRECISION INTERFACE ********* COMPLEX 
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_1d_(ref IntPtr plan, ref int n, [In,Out] complex* input, [In,Out] complex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_2d_(ref IntPtr plan, ref int nx, ref int ny, [In,Out] complex* input, [In,Out] complex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_3d_(ref IntPtr plan, ref int nx, ref int ny, ref int nz, [In,Out] complex* input, [In,Out] complex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] complex* input, [In,Out] complex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        
        /****************************************************************** REAL-COMPLEX ***/
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_r2c_1d_(ref IntPtr plan, ref int n, [In,Out] double* input, [In,Out] complex* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_r2c_2d_(ref IntPtr plan, ref int nx, ref int ny, [In,Out] double* input, [In,Out] complex* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_r2c_3d_(ref IntPtr plan, ref int nx, ref int ny, ref int nz, [In,Out] double* input, [In,Out] complex* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_r2c_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] double* input, [In,Out] complex* output, ref FFTW_PLANCREATION flags);

        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_c2r_1d_(ref IntPtr plan, ref int n, [In,Out] complex* input, [In,Out] double* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_c2r_2d_(ref IntPtr plan, ref int nx, ref int ny, [In,Out] complex* input, [In,Out] double* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_c2r_3d_(ref IntPtr plan, ref int nx, ref int ny, ref int nz, [In,Out] complex* input, [In,Out] double* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_dft_c2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] complex* input, [In,Out] double* output, ref FFTW_PLANCREATION flags);

        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_r2r_1d_(ref IntPtr plan, ref int n, [In,Out] double* input, [In,Out] double* output,  ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_r2r_2d_(ref IntPtr plan, ref int nx, ref int ny, [In,Out] double* input, [In,Out] double* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_r2r_3d_(ref IntPtr plan, ref int nx, ref int ny, ref int nz, [In,Out] double* input, [In,Out] double* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_r2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] double* input, [In,Out] double* output, ref FFTW_PLANCREATION flags);

        /************************************ ADVANCED INTERFACE *************************************/
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void dfftw_plan_many_dft_(ref IntPtr plan, ref int rank, [In,Out] int[] n, ref int howmany, [In,Out] complex* input, [In,Out] int[] inembed, ref int istride, ref int idist, [In,Out] complex* output, [In,Out] int[] onembed, ref int ostride, ref int odist, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);		
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void dfftw_plan_many_dft_r2c_(ref IntPtr plan, ref int rank, [In,Out] int [] n, ref int howmany, [In,Out] double* input, [In,Out] int[] inembed, ref int istride, ref int idist, [In,Out] complex* output, [In,Out] int[] onembed, ref int ostride, ref int odist, ref FFTW_PLANCREATION flags);        
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void dfftw_plan_many_dft_c2r_(ref IntPtr plan, ref int rank, [In,Out] int [] n, ref int howmany, [In,Out] complex* input, [In,Out] int[] inembed, ref int istride, ref int idist, [In,Out] double* output, [In,Out] int[] onembed, ref int ostride, ref int odist, ref FFTW_PLANCREATION flags);        
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void dfftw_plan_many_r2r_(    ref IntPtr plan, ref int rank, [In,Out] int [] n, ref int howmany, [In,Out] double [] input, [In,Out] int[] inembed, ref int istride, ref int idist, [In,Out] double* output, [In,Out] int[] onembed, ref int ostride, ref int odist, ref FFTW_KIND kind, ref FFTW_PLANCREATION flags);        
        
        /************************************ GURU INTERFACE *************************************/
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_guru_dft_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] complex* input, [In,Out] complex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void dfftw_plan_guru_split_dft_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] double* ri, [In,Out] double* ii, [In,Out] double* ro, [In,Out] double* io, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_guru_dft_r2c_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] double* input, [In,Out] complex* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_guru_dft_c2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] complex* input, [In,Out] double* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void dfftw_plan_guru_split_dft_r2c_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] double* ri, [In,Out] double* ro, [In,Out] double* io, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void dfftw_plan_guru_split_dft_c2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] double* ri, [In,Out] double* ii, [In,Out] double* ro, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_plan_guru_r2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] double* input, [In,Out] double* output, ref FFTW_KIND kind, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_execute_dft_(ref IntPtr plan, [In,Out] complex* input, [In,Out] complex* output);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_execute_split_dft_(ref IntPtr plan, [In,Out] double* ri, [In,Out] double* ii, [In,Out] double* ro, [In,Out] double* io);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_execute_dft_r2c_(ref IntPtr plan, [In,Out] double* ri, [In,Out] complex* ii);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_execute_split_dft_r2c_(ref IntPtr plan, [In,Out] double* input, [In,Out] double* ro, [In,Out] double* ri);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_execute_dft_c2r_(ref IntPtr plan, [In,Out] complex* input, [In,Out] double* output);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_execute_split_dft_c2r_(ref IntPtr plan, [In,Out] double* ri, [In,Out] double* ii, [In,Out] double* output);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_execute_r2r_(ref IntPtr plan, [In,Out] double* input, [In,Out] double* output);

        /************************************ BASIC PLAN EXECUTION & MISC ******************************/
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_execute_(ref IntPtr plan);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_destroy_plan_(ref IntPtr plan);
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void dfftw_cleanup_();
        [DllImport("libfftw3-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void fftw_set_timelimit(ref double seconds);
        #endregion DOUBLE PRECISION INTERFACE 

        #region SINGLE PRECISION INTERFACE
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_1d_(ref IntPtr plan, ref int n, [In,Out] fcomplex* input, [In,Out] fcomplex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_2d_(ref IntPtr plan, ref int nx, ref int ny, [In,Out] fcomplex* input, [In,Out] fcomplex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_3d_(ref IntPtr plan, ref int nx, ref int ny, ref int nz, [In,Out] fcomplex* input, [In,Out] fcomplex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] fcomplex* input, [In,Out] fcomplex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        
        /****************************************************************** REAL-COMPLEX ***/
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_r2c_1d_(ref IntPtr plan, ref int n, [In,Out] float* input, [In,Out] fcomplex* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_r2c_2d_(ref IntPtr plan, ref int nx, ref int ny, [In,Out] float* input, [In,Out] fcomplex* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_r2c_3d_(ref IntPtr plan, ref int nx, ref int ny, ref int nz, [In,Out] float* input, [In,Out] fcomplex* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_r2c_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] float* input, [In,Out] fcomplex* output, ref FFTW_PLANCREATION flags);

        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_c2r_1d_(ref IntPtr plan, ref int n, [In,Out] fcomplex* input, [In,Out] float* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_c2r_2d_(ref IntPtr plan, ref int nx, ref int ny, [In,Out] fcomplex* input, [In,Out] float* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_c2r_3d_(ref IntPtr plan, ref int nx, ref int ny, ref int nz, [In,Out] fcomplex* input, [In,Out] float* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_dft_c2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] fcomplex* input, [In,Out] float* output, ref FFTW_PLANCREATION flags);

        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_r2r_1d_(ref IntPtr plan, ref int n, [In,Out] float* input, [In,Out] float* output,  ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_r2r_2d_(ref IntPtr plan, ref int nx, ref int ny, [In,Out] float* input, [In,Out] float* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_r2r_3d_(ref IntPtr plan, ref int nx, ref int ny, ref int nz, [In,Out] float* input, [In,Out] float* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_r2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] float* input, [In,Out] float* output, ref FFTW_PLANCREATION flags);

        /************************************ ADVANCED INTERFACE *************************************/
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void sfftw_plan_many_dft_(ref IntPtr plan, ref int rank, [In,Out] int[] n, ref int howmany, [In,Out] fcomplex* input, [In,Out] int[] inembed, ref int istride, ref int idist, [In,Out] fcomplex* output, [In,Out] int[] onembed, ref int ostride, ref int odist, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);		
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void sfftw_plan_many_dft_r2c_(ref IntPtr plan, ref int rank, [In,Out] int [] n, ref int howmany, [In,Out] float* input, [In,Out] int[] inembed, ref int istride, ref int idist, [In,Out] fcomplex* output, [In,Out] int[] onembed, ref int ostride, ref int odist, ref FFTW_PLANCREATION flags);        
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void sfftw_plan_many_dft_c2r_(ref IntPtr plan, ref int rank, [In,Out] int [] n, ref int howmany, [In,Out] fcomplex* input, [In,Out] int[] inembed, ref int istride, ref int idist, [In,Out] float* output, [In,Out] int[] onembed, ref int ostride, ref int odist, ref FFTW_PLANCREATION flags);        
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void sfftw_plan_many_r2r_(    ref IntPtr plan, ref int rank, [In,Out] int [] n, ref int howmany, [In,Out] float [] input, [In,Out] int[] inembed, ref int istride, ref int idist, [In,Out] float* output, [In,Out] int[] onembed, ref int ostride, ref int odist, ref FFTW_KIND kind, ref FFTW_PLANCREATION flags);        
        
        /************************************ GURU INTERFACE *************************************/
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_guru_dft_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] fcomplex* input, [In,Out] fcomplex* output, ref FFTW_DIRECTION sign, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void sfftw_plan_guru_split_dft_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] float* ri, [In,Out] float* ii, [In,Out] float* ro, [In,Out] float* io, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_guru_dft_r2c_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] float* input, [In,Out] fcomplex* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_guru_dft_c2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] fcomplex* input, [In,Out] float* output, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void sfftw_plan_guru_split_dft_r2c_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] float* ri, [In,Out] float* ro, [In,Out] float* io, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void sfftw_plan_guru_split_dft_c2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] float* ri, [In,Out] float* ii, [In,Out] float* ro, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_plan_guru_r2r_(ref IntPtr plan, ref int rank, [In,Out] int[] n, [In,Out] int[] iS,  [In,Out] int[] iO, ref int howmany_rank, [In,Out] int[] hn, [In,Out] int[] hiS,  [In,Out] int[] hiO, [In,Out] float* input, [In,Out] float* output, ref FFTW_KIND kind, ref FFTW_PLANCREATION flags);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_execute_dft_(ref IntPtr plan, [In,Out] fcomplex* input, [In,Out] fcomplex* output);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_execute_split_dft_(ref IntPtr plan, [In,Out] float* ri, [In,Out] float* ii, [In,Out] float* ro, [In,Out] float* io);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_execute_dft_r2c_(ref IntPtr plan, [In,Out] float* ri, [In,Out] fcomplex* ii);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_execute_split_dft_r2c_(ref IntPtr plan, [In,Out] float* input, [In,Out] float* ro, [In,Out] float* ri);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_execute_dft_c2r_(ref IntPtr plan, [In,Out] fcomplex* input, [In,Out] float* output);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_execute_split_dft_c2r_(ref IntPtr plan, [In,Out] float* ri, [In,Out] float* ii, [In,Out] float* output);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_execute_r2r_(ref IntPtr plan, [In,Out] float* input, [In,Out] float* output);

        /************************************ BASIC PLAN EXECUTION & MISC ******************************/
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_execute_(ref IntPtr plan);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_destroy_plan_(ref IntPtr plan);
        [DllImport("libfftw3f-3",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void sfftw_cleanup_();
        #endregion SINGLE PRECISION INTERFACE
        #endregion

        #region FFTW CONSTANTS 

        [FlagsAttribute]
        public enum FFTW_PLANCREATION : uint {
            MEASURE = 0u, 
            DESTROY_INPUT = (1U << 0), 
            UNALIGNED = (1U << 1), 
            CONSERVE_MEMORY = (1U << 2), 
            EXHAUSTIVE = (1U << 3), /* NO_EXHAUSTIVE is default */
            PRESERVE_INPUT = (1U << 4), /* cancels FFTW_DESTROY_INPUT */
            PATIENT = (1U << 5), /* IMPATIENT is default */
            ESTIMATE = (1U << 6)
        }
        
        public enum FFTW_DIRECTION {
            FORWARD = -1, 
            BACKWARDS = 1
        }

        public enum FFTW_KIND {
            R2HC=0,
            HC2R=1,
            DHT=2,
            REDFT00=3,
            REDFT01=4,
            REDFT10=5,
            REDFT11=6,
            RODFT00=7,
            RODFT01=8,
            RODFT10=9,
            RODFT11=10
        }
        #endregion

        #region attributes
        object _lockobject = new object(); 
        int[] m_n,m_is,m_os, m_hn, m_his, m_hos; 
        //List<int> m_tmpDims = new List<int>(10); 
        #endregion

        #region constructor 
        public ILFFTW3FFT () {
            // fftw internally caches the plans, so ....
            m_n = new int[10]; 
            m_is = new int[10]; 
            m_os = new int[10]; 
            m_hn = new int[2]; 
            m_his = new int[2]; 
            m_hos = new int[2]; 
        }
        #endregion

        #region IILFFT Member - 1-D



        public ILRetArray< complex >  FFTForward1D (ILInArray< double > A, int dim) {
            return  FFTForward (A,dim,1); 
        }

#region HYCALPER AUTO GENERATED CODE


        public ILRetArray< fcomplex >  FFTBackward1D (ILInArray< fcomplex > A, int dim) {
            return  FFTBackward (A,dim,1); 
        }

        public ILRetArray< complex >  FFTBackward1D (ILInArray< complex > A, int dim) {
            return  FFTBackward (A,dim,1); 
        }

        public ILRetArray< fcomplex >  FFTForward1D (ILInArray< fcomplex > A, int dim) {
            return  FFTForward (A,dim,1); 
        }

        public ILRetArray< fcomplex >  FFTForward1D (ILInArray< float > A, int dim) {
            return  FFTForward (A,dim,1); 
        }

        public ILRetArray< complex >  FFTForward1D (ILInArray< complex > A, int dim) {
            return  FFTForward (A,dim,1); 
        }

#endregion HYCALPER AUTO GENERATED CODE



        public ILRetArray< double > FFTBackwSym1D(ILInArray< complex > A, int dim) {
            return ILMath.real(FFTBackward1D(A,dim)); 
        }

#region HYCALPER AUTO GENERATED CODE


        public ILRetArray< float > FFTBackwSym1D(ILInArray< fcomplex > A, int dim) {
            return ILMath.real(FFTBackward1D(A,dim)); 
        }

#endregion HYCALPER AUTO GENERATED CODE

        #endregion

        #region IILFFT Member - n-D



        public ILRetArray< complex >  FFTForward (ILInArray< double > A, int nDims) {
            return  FFTForward (A,0,nDims); 
        }
        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTForward(ILInArray< double> A, int firstDim, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims < 0 || firstDim < 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.empty< complex>(A.Size);
                if (A.IsScalar || (A.Size[firstDim] == 1 && nDims == 1))
                    return  ILMath.tocomplex(A);
                if (nDims > A.Size.NumberOfDimensions)
                    nDims = A.Size.NumberOfDimensions;
                // prepare output array 
                ILArray< complex> ret =  ILMath.tocomplex(A);
                IntPtr plan = IntPtr.Zero;
                int hrank = 2;
                FFTW_DIRECTION dir =  FFTW_DIRECTION.FORWARD;
                lock (_lockobject) {
                    FFTW_PLANCREATION flags = FFTW_PLANCREATION.ESTIMATE;
                    if (nDims > m_n.Length) resizeCache(nDims);
                    for (int i = 0; i < nDims; i++) {
                        m_n[i] = A.Size[i + firstDim];
                        m_is[i] = A.Size.SequentialIndexDistance(i + firstDim);
                        m_os[i] = m_is[i];
                    }
                    m_hn[0] = A.Size.SequentialIndexDistance(firstDim);
                    m_his[0] = 1;
                    m_hos[0] = 1;
                    m_his[1] = A.Size.SequentialIndexDistance(nDims + firstDim);
                    m_hn[1] = A.Size.NumberOfElements / m_his[1];
                    m_hos[1] = m_his[1];
                    fixed ( complex* retArr = ret.GetArrayForWrite()) {
                        
                        dfftw_plan_guru_dft_(ref plan, ref nDims, m_n, m_is, m_os,
                                      ref hrank, m_hn, m_his, m_hos,
                                      retArr, retArr,
                                      ref dir, ref flags);
                    }
                }
                if (plan == IntPtr.Zero)
                    throw new ILInvalidOperationException("error creating plan for fftw3 (guru interface)");
                dfftw_execute_(ref plan);
                dfftw_destroy_plan_(ref plan);
                

                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE


        public ILRetArray< fcomplex >  FFTBackward (ILInArray< fcomplex > A, int nDims) {
            return  FFTBackward (A,0,nDims); 
        }
        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTBackward(ILInArray< fcomplex> A, int firstDim, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims < 0 || firstDim < 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.empty< fcomplex>(A.Size);
                if (A.IsScalar || (A.Size[firstDim] == 1 && nDims == 1))
                    return  A.C;
                if (nDims > A.Size.NumberOfDimensions)
                    nDims = A.Size.NumberOfDimensions;
                // prepare output array 
                ILArray< fcomplex> ret =  A.C;
                IntPtr plan = IntPtr.Zero;
                int hrank = 2;
                FFTW_DIRECTION dir =  FFTW_DIRECTION.BACKWARDS;
                lock (_lockobject) {
                    FFTW_PLANCREATION flags = FFTW_PLANCREATION.ESTIMATE;
                    if (nDims > m_n.Length) resizeCache(nDims);
                    for (int i = 0; i < nDims; i++) {
                        m_n[i] = A.Size[i + firstDim];
                        m_is[i] = A.Size.SequentialIndexDistance(i + firstDim);
                        m_os[i] = m_is[i];
                    }
                    m_hn[0] = A.Size.SequentialIndexDistance(firstDim);
                    m_his[0] = 1;
                    m_hos[0] = 1;
                    m_his[1] = A.Size.SequentialIndexDistance(nDims + firstDim);
                    m_hn[1] = A.Size.NumberOfElements / m_his[1];
                    m_hos[1] = m_his[1];
                    fixed ( fcomplex* retArr = ret.GetArrayForWrite()) {
                       
                        sfftw_plan_guru_dft_(ref plan, ref nDims, m_n, m_is, m_os,
                                      ref hrank, m_hn, m_his, m_hos,
                                      retArr, retArr,
                                      ref dir, ref flags);
                    }
                }
                if (plan == IntPtr.Zero)
                    throw new ILInvalidOperationException("error creating plan for fftw3 (guru interface)");
                dfftw_execute_(ref plan);
                dfftw_destroy_plan_(ref plan);
                ret /= new fcomplex(m_his[1]/m_hn[0],0.0f);
                return ret;
            }
        }

        public ILRetArray< complex >  FFTBackward (ILInArray< complex > A, int nDims) {
            return  FFTBackward (A,0,nDims); 
        }
        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTBackward(ILInArray< complex> A, int firstDim, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims < 0 || firstDim < 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.empty< complex>(A.Size);
                if (A.IsScalar || (A.Size[firstDim] == 1 && nDims == 1))
                    return  A.C;
                if (nDims > A.Size.NumberOfDimensions)
                    nDims = A.Size.NumberOfDimensions;
                // prepare output array 
                ILArray< complex> ret =  A.C;
                IntPtr plan = IntPtr.Zero;
                int hrank = 2;
                FFTW_DIRECTION dir =  FFTW_DIRECTION.BACKWARDS;
                lock (_lockobject) {
                    FFTW_PLANCREATION flags = FFTW_PLANCREATION.ESTIMATE;
                    if (nDims > m_n.Length) resizeCache(nDims);
                    for (int i = 0; i < nDims; i++) {
                        m_n[i] = A.Size[i + firstDim];
                        m_is[i] = A.Size.SequentialIndexDistance(i + firstDim);
                        m_os[i] = m_is[i];
                    }
                    m_hn[0] = A.Size.SequentialIndexDistance(firstDim);
                    m_his[0] = 1;
                    m_hos[0] = 1;
                    m_his[1] = A.Size.SequentialIndexDistance(nDims + firstDim);
                    m_hn[1] = A.Size.NumberOfElements / m_his[1];
                    m_hos[1] = m_his[1];
                    fixed ( complex* retArr = ret.GetArrayForWrite()) {
                       
                        dfftw_plan_guru_dft_(ref plan, ref nDims, m_n, m_is, m_os,
                                      ref hrank, m_hn, m_his, m_hos,
                                      retArr, retArr,
                                      ref dir, ref flags);
                    }
                }
                if (plan == IntPtr.Zero)
                    throw new ILInvalidOperationException("error creating plan for fftw3 (guru interface)");
                dfftw_execute_(ref plan);
                dfftw_destroy_plan_(ref plan);
                ret /= new complex(m_his[1]/m_hn[0],0.0f);
                return ret;
            }
        }

        public ILRetArray< fcomplex >  FFTForward (ILInArray< fcomplex > A, int nDims) {
            return  FFTForward (A,0,nDims); 
        }
        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTForward(ILInArray< fcomplex> A, int firstDim, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims < 0 || firstDim < 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.empty< fcomplex>(A.Size);
                if (A.IsScalar || (A.Size[firstDim] == 1 && nDims == 1))
                    return  A.C;
                if (nDims > A.Size.NumberOfDimensions)
                    nDims = A.Size.NumberOfDimensions;
                // prepare output array 
                ILArray< fcomplex> ret =  A.C;
                IntPtr plan = IntPtr.Zero;
                int hrank = 2;
                FFTW_DIRECTION dir =  FFTW_DIRECTION.FORWARD;
                lock (_lockobject) {
                    FFTW_PLANCREATION flags = FFTW_PLANCREATION.ESTIMATE;
                    if (nDims > m_n.Length) resizeCache(nDims);
                    for (int i = 0; i < nDims; i++) {
                        m_n[i] = A.Size[i + firstDim];
                        m_is[i] = A.Size.SequentialIndexDistance(i + firstDim);
                        m_os[i] = m_is[i];
                    }
                    m_hn[0] = A.Size.SequentialIndexDistance(firstDim);
                    m_his[0] = 1;
                    m_hos[0] = 1;
                    m_his[1] = A.Size.SequentialIndexDistance(nDims + firstDim);
                    m_hn[1] = A.Size.NumberOfElements / m_his[1];
                    m_hos[1] = m_his[1];
                    fixed ( fcomplex* retArr = ret.GetArrayForWrite()) {
                       
                        sfftw_plan_guru_dft_(ref plan, ref nDims, m_n, m_is, m_os,
                                      ref hrank, m_hn, m_his, m_hos,
                                      retArr, retArr,
                                      ref dir, ref flags);
                    }
                }
                if (plan == IntPtr.Zero)
                    throw new ILInvalidOperationException("error creating plan for fftw3 (guru interface)");
                dfftw_execute_(ref plan);
                dfftw_destroy_plan_(ref plan);
                
                return ret;
            }
        }

        public ILRetArray< fcomplex >  FFTForward (ILInArray< float > A, int nDims) {
            return  FFTForward (A,0,nDims); 
        }
        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTForward(ILInArray< float> A, int firstDim, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims < 0 || firstDim < 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.empty< fcomplex>(A.Size);
                if (A.IsScalar || (A.Size[firstDim] == 1 && nDims == 1))
                    return  ILMath.tofcomplex(A);
                if (nDims > A.Size.NumberOfDimensions)
                    nDims = A.Size.NumberOfDimensions;
                // prepare output array 
                ILArray< fcomplex> ret =  ILMath.tofcomplex(A);
                IntPtr plan = IntPtr.Zero;
                int hrank = 2;
                FFTW_DIRECTION dir =  FFTW_DIRECTION.FORWARD;
                lock (_lockobject) {
                    FFTW_PLANCREATION flags = FFTW_PLANCREATION.ESTIMATE;
                    if (nDims > m_n.Length) resizeCache(nDims);
                    for (int i = 0; i < nDims; i++) {
                        m_n[i] = A.Size[i + firstDim];
                        m_is[i] = A.Size.SequentialIndexDistance(i + firstDim);
                        m_os[i] = m_is[i];
                    }
                    m_hn[0] = A.Size.SequentialIndexDistance(firstDim);
                    m_his[0] = 1;
                    m_hos[0] = 1;
                    m_his[1] = A.Size.SequentialIndexDistance(nDims + firstDim);
                    m_hn[1] = A.Size.NumberOfElements / m_his[1];
                    m_hos[1] = m_his[1];
                    fixed ( fcomplex* retArr = ret.GetArrayForWrite()) {
                       
                        sfftw_plan_guru_dft_(ref plan, ref nDims, m_n, m_is, m_os,
                                      ref hrank, m_hn, m_his, m_hos,
                                      retArr, retArr,
                                      ref dir, ref flags);
                    }
                }
                if (plan == IntPtr.Zero)
                    throw new ILInvalidOperationException("error creating plan for fftw3 (guru interface)");
                dfftw_execute_(ref plan);
                dfftw_destroy_plan_(ref plan);
                
                return ret;
            }
        }

        public ILRetArray< complex >  FFTForward (ILInArray< complex > A, int nDims) {
            return  FFTForward (A,0,nDims); 
        }
        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTForward(ILInArray< complex> A, int firstDim, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims < 0 || firstDim < 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.empty< complex>(A.Size);
                if (A.IsScalar || (A.Size[firstDim] == 1 && nDims == 1))
                    return  A.C;
                if (nDims > A.Size.NumberOfDimensions)
                    nDims = A.Size.NumberOfDimensions;
                // prepare output array 
                ILArray< complex> ret =  A.C;
                IntPtr plan = IntPtr.Zero;
                int hrank = 2;
                FFTW_DIRECTION dir =  FFTW_DIRECTION.FORWARD;
                lock (_lockobject) {
                    FFTW_PLANCREATION flags = FFTW_PLANCREATION.ESTIMATE;
                    if (nDims > m_n.Length) resizeCache(nDims);
                    for (int i = 0; i < nDims; i++) {
                        m_n[i] = A.Size[i + firstDim];
                        m_is[i] = A.Size.SequentialIndexDistance(i + firstDim);
                        m_os[i] = m_is[i];
                    }
                    m_hn[0] = A.Size.SequentialIndexDistance(firstDim);
                    m_his[0] = 1;
                    m_hos[0] = 1;
                    m_his[1] = A.Size.SequentialIndexDistance(nDims + firstDim);
                    m_hn[1] = A.Size.NumberOfElements / m_his[1];
                    m_hos[1] = m_his[1];
                    fixed ( complex* retArr = ret.GetArrayForWrite()) {
                       
                        dfftw_plan_guru_dft_(ref plan, ref nDims, m_n, m_is, m_os,
                                      ref hrank, m_hn, m_his, m_hos,
                                      retArr, retArr,
                                      ref dir, ref flags);
                    }
                }
                if (plan == IntPtr.Zero)
                    throw new ILInvalidOperationException("error creating plan for fftw3 (guru interface)");
                dfftw_execute_(ref plan);
                dfftw_destroy_plan_(ref plan);
                
                return ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
       

        
        public ILRetArray< double > FFTBackwSym(ILInArray< complex > A, int nDims) {
            return ILMath.real(FFTBackward(A,nDims));  
        }

#region HYCALPER AUTO GENERATED CODE

       
        public ILRetArray< float > FFTBackwSym(ILInArray< fcomplex > A, int nDims) {
            return ILMath.real(FFTBackward(A,nDims));  
        }

#endregion HYCALPER AUTO GENERATED CODE

        #endregion
        
        #region IILFFT Member - Misc
        
        public bool CachePlans {
            get {
                return true; 
            }
        }

        public void FreePlans() {
            FreeAllDescriptors(); 
        }

        public bool SpeedyHermitian {
            get { return false; }
        }

        #endregion

        #region private helper
        [SecuritySafeCritical]
        private void FreeAllDescriptors() {
            try {
                dfftw_cleanup_();
                sfftw_cleanup_();
            } catch (Exception) { }
        }
        private void resizeCache(int size) {
            if (m_n.Length >= size) return;
            m_n = new int[size];
            m_is = new int[size];
            m_os = new int[size];
        }
        #endregion

        #region IDisposable Member

        public void Dispose() {
            Dispose(true); 
            GC.SuppressFinalize(this); 
        }

        public virtual void Dispose (bool manual) {
            if (manual) {
                FreeAllDescriptors();             
            }
        }

        ~ILFFTW3FFT () {
            Dispose(true);     
        }
        #endregion

    }
}
