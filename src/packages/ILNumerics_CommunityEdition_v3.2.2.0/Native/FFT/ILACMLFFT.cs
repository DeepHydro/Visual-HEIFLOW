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
    /// Wrapper for FFT interface using ACML ver. 3.6
    /// </summary>
    public unsafe class ILACMLFFT : IILFFT, IDisposable {

        #region pinvoke definitions 
        
        [DllImport("libacml_dll"),SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void zfft1mx(int MODE, double SCALE, int INPL, int NSEQ, int N, IntPtr X, int INCX1, int INCX2, IntPtr Y, int INCY1, int INCY2, IntPtr COMM, ref int INFO);
        [DllImport("libacml_dll"), SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void cfft1mx(int MODE, float SCALE, int INPL, int NSEQ, int N,  IntPtr X, int INCX1, int INCX2, IntPtr Y, int INCY1, int INCY2, IntPtr COMM, ref int INFO);
        [DllImport("libacml_dll"), SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void zfft2dx(int MODE, double SCALE, int LTRANS, int INPL, int M, int N, IntPtr X, int INCX1, int INCX2, IntPtr Y, int INCY1, int INCY2, IntPtr COMM, ref int INFO);
        [DllImport("libacml_dll"), SuppressUnmanagedCodeSecurity, SecurityCritical]
		private static extern void cfft2dx(int MODE, float SCALE,  int LTRANS, int INPL, int M, int N, IntPtr X, int INCX1, int INCX2, IntPtr Y, int INCY1, int INCY2, IntPtr COMM, ref int INFO);

        #endregion

        #region value constants
        private class ACMLValues {
            public static readonly int Double = 8; 
            public static readonly int Single = 4; 
            public static readonly int Real = 1; 
            public static readonly int Complex = 2; 
            public static readonly int Backwards = 1; 
            public static readonly int Forward = -1; 
        }
        #endregion

        #region attributes 
        Dictionary<string,IntPtr> m_descriptors; 
        object _lockobject = new object(); 
        #endregion

        #region constructor 
        public ILACMLFFT () {
            m_descriptors = new Dictionary<string,IntPtr>(10); 
        }
        #endregion

        #region IILFFT Member - 1-D
        


        public ILRetArray< complex>  FFTForward1D(ILInArray< double> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || dim < 0)
                    throw new ILArgumentException("FFTForward1D: invalid parameter!");
                if (A.IsEmpty) return ILMath.array< complex>(A.Size);
                if (A.IsScalar || A.Size[dim] == 1)
                    return  ILMath.tocomplex(A);
                // prepare output array 
                ILArray< complex> ret =  ILMath.tocomplex(A);
                fft1dInplace(dim, ret, ACMLValues.Forward);
                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE


        public ILRetArray< fcomplex>  FFTBackward1D(ILInArray< fcomplex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || dim < 0)
                    throw new ILArgumentException("FFTForward1D: invalid parameter!");
                if (A.IsEmpty) return ILMath.array< fcomplex>(A.Size);
                if (A.IsScalar || A.Size[dim] == 1)
                    return  A.C;
                // prepare output array 
                ILArray< fcomplex> ret =  A.C;
                fft1dInplace(dim, ret, ACMLValues.Backwards);
                return ret;
            }
        }

        public ILRetArray< complex>  FFTBackward1D(ILInArray< complex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || dim < 0)
                    throw new ILArgumentException("FFTForward1D: invalid parameter!");
                if (A.IsEmpty) return ILMath.array< complex>(A.Size);
                if (A.IsScalar || A.Size[dim] == 1)
                    return  A.C;
                // prepare output array 
                ILArray< complex> ret =  A.C;
                fft1dInplace(dim, ret, ACMLValues.Backwards);
                return ret;
            }
        }

        public ILRetArray< fcomplex>  FFTForward1D(ILInArray< fcomplex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || dim < 0)
                    throw new ILArgumentException("FFTForward1D: invalid parameter!");
                if (A.IsEmpty) return ILMath.array< fcomplex>(A.Size);
                if (A.IsScalar || A.Size[dim] == 1)
                    return  A.C;
                // prepare output array 
                ILArray< fcomplex> ret =  A.C;
                fft1dInplace(dim, ret, ACMLValues.Forward);
                return ret;
            }
        }

        public ILRetArray< fcomplex>  FFTForward1D(ILInArray< float> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || dim < 0)
                    throw new ILArgumentException("FFTForward1D: invalid parameter!");
                if (A.IsEmpty) return ILMath.array< fcomplex>(A.Size);
                if (A.IsScalar || A.Size[dim] == 1)
                    return  ILMath.tofcomplex(A);
                // prepare output array 
                ILArray< fcomplex> ret =  ILMath.tofcomplex(A);
                fft1dInplace(dim, ret, ACMLValues.Forward);
                return ret;
            }
        }

        public ILRetArray< complex>  FFTForward1D(ILInArray< complex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || dim < 0)
                    throw new ILArgumentException("FFTForward1D: invalid parameter!");
                if (A.IsEmpty) return ILMath.array< complex>(A.Size);
                if (A.IsScalar || A.Size[dim] == 1)
                    return  A.C;
                // prepare output array 
                ILArray< complex> ret =  A.C;
                fft1dInplace(dim, ret, ACMLValues.Forward);
                return ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        public ILRetArray<double> FFTBackwSym1D(ILInArray<complex> A, int dim) {
            return ILMath.real(FFTBackward1D(A,dim));    
        }

        public ILRetArray<float> FFTBackwSym1D(ILInArray<fcomplex> A, int dim) {
            return ILMath.real(FFTBackward1D(A,dim));    
        }

        #endregion

        #region IILFFT Member n-D



        public ILRetArray< complex>  FFTForward(ILInArray< double> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.array< complex>(A.Size);
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1))
                    return  ILMath.tocomplex(A);
                if (nDims > A.Size.NumberOfDimensions)
                    return  FFTForward(A, A.Size.NumberOfDimensions);

                // prepare output array + transform each dimension inplace 
                ILArray< complex> ret =  ILMath.tocomplex(A);
                switch (nDims) {
                    case 2:
                        fft2dInplace(ret, ACMLValues.Forward);
                        break;
                    default:
                        for (int i = 0; i < nDims; i++) {
                            fft1dInplace(i, ret, ACMLValues.Forward);
                        }
                        break;
                }
                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE


        public ILRetArray< fcomplex>  FFTBackward(ILInArray< fcomplex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.array< fcomplex>(A.Size);
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1))
                    return  A.C;
                if (nDims > A.Size.NumberOfDimensions)
                    return  FFTBackward(A, A.Size.NumberOfDimensions);

                // prepare output array + transform each dimension inplace 
                ILArray< fcomplex> ret =  A.C;
                switch (nDims) {
                    case 2:
                        fft2dInplace(ret, ACMLValues.Backwards);
                        break;
                    default:
                        for (int i = 0; i < nDims; i++) {
                            fft1dInplace(i, ret, ACMLValues.Backwards);
                        }
                        break;
                }
                return ret;
            }
        }

        public ILRetArray< complex>  FFTBackward(ILInArray< complex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.array< complex>(A.Size);
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1))
                    return  A.C;
                if (nDims > A.Size.NumberOfDimensions)
                    return  FFTBackward(A, A.Size.NumberOfDimensions);

                // prepare output array + transform each dimension inplace 
                ILArray< complex> ret =  A.C;
                switch (nDims) {
                    case 2:
                        fft2dInplace(ret, ACMLValues.Backwards);
                        break;
                    default:
                        for (int i = 0; i < nDims; i++) {
                            fft1dInplace(i, ret, ACMLValues.Backwards);
                        }
                        break;
                }
                return ret;
            }
        }

        public ILRetArray< fcomplex>  FFTForward(ILInArray< fcomplex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.array< fcomplex>(A.Size);
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1))
                    return  A.C;
                if (nDims > A.Size.NumberOfDimensions)
                    return  FFTForward(A, A.Size.NumberOfDimensions);

                // prepare output array + transform each dimension inplace 
                ILArray< fcomplex> ret =  A.C;
                switch (nDims) {
                    case 2:
                        fft2dInplace(ret, ACMLValues.Forward);
                        break;
                    default:
                        for (int i = 0; i < nDims; i++) {
                            fft1dInplace(i, ret, ACMLValues.Forward);
                        }
                        break;
                }
                return ret;
            }
        }

        public ILRetArray< fcomplex>  FFTForward(ILInArray< float> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.array< fcomplex>(A.Size);
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1))
                    return  ILMath.tofcomplex(A);
                if (nDims > A.Size.NumberOfDimensions)
                    return  FFTForward(A, A.Size.NumberOfDimensions);

                // prepare output array + transform each dimension inplace 
                ILArray< fcomplex> ret =  ILMath.tofcomplex(A);
                switch (nDims) {
                    case 2:
                        fft2dInplace(ret, ACMLValues.Forward);
                        break;
                    default:
                        for (int i = 0; i < nDims; i++) {
                            fft1dInplace(i, ret, ACMLValues.Forward);
                        }
                        break;
                }
                return ret;
            }
        }

        public ILRetArray< complex>  FFTForward(ILInArray< complex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0)
                    throw new ILArgumentException("invalid parameter!");
                if (A.IsEmpty) return ILMath.array< complex>(A.Size);
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1))
                    return  A.C;
                if (nDims > A.Size.NumberOfDimensions)
                    return  FFTForward(A, A.Size.NumberOfDimensions);

                // prepare output array + transform each dimension inplace 
                ILArray< complex> ret =  A.C;
                switch (nDims) {
                    case 2:
                        fft2dInplace(ret, ACMLValues.Forward);
                        break;
                    default:
                        for (int i = 0; i < nDims; i++) {
                            fft1dInplace(i, ret, ACMLValues.Forward);
                        }
                        break;
                }
                return ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        public ILRetArray<float> FFTBackwSym(ILInArray<fcomplex> A, int nDims) {
            return ILMath.real(FFTBackward(A,nDims)); 
        }

        public ILRetArray<double> FFTBackwSym(ILInArray<complex> A, int nDims) {
            return ILMath.real(FFTBackward(A,nDims)); 
        }

        public bool CachePlans {
            get { return true; }
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
        private void fft1dInplace(int dim, ILOutArray<complex> A, int mode) {
            using (ILScope.Enter(A)) {
                int N = A.Size[dim], info = 0;
                // spacing between elements
                int incx1 = A.Size.SequentialIndexDistance(dim);
                // storage of subsequent transformations
                int incx2 = incx1 * N;
                // number of transformations
                int nseq = A.Size.NumberOfElements / incx2;
                double scale = (mode == ACMLValues.Backwards) ? 1.0 / N : 1.0;
                string hash = hashPlan(ACMLValues.Double, ACMLValues.Complex, N, "zfft1mx");
                IntPtr descriptor;
                lock (_lockobject) {
                    if (!m_descriptors.TryGetValue(hash, out descriptor)) {
                        int commLength = (3 * N + 100) * ACMLValues.Double * 2;
                        try {
                            descriptor = Marshal.AllocCoTaskMem(commLength);
                            zfft1mx(0, scale, 1, nseq, N, IntPtr.Zero, incx1, incx2, IntPtr.Zero, 0, 0, descriptor, ref info);
                            if (info != 0)
                                throw new ILInvalidOperationException("error creating fft-plan");
                            m_descriptors[hash] = descriptor;
                        } catch (OutOfMemoryException exc) {
                            if (m_descriptors.Count > 0) {
                                FreeAllDescriptors();
                                descriptor = Marshal.AllocCoTaskMem(commLength);
                                zfft1mx(0, scale, 1, nseq, N, IntPtr.Zero, incx1, incx2, IntPtr.Zero, 0, 0, descriptor, ref info);
                                if (info != 0)
                                    throw new ILInvalidOperationException("error creating fft-plan");
                                m_descriptors[hash] = descriptor;
                            }
                            throw;
                        }
                    }
                    // do the transform(s)
                    fixed (complex* start = A.GetArrayForWrite()) {
                        for (int i = 0; i < incx1 && info == 0; i++)
                            zfft1mx(mode, scale, 1, nseq, N, (IntPtr)(start + i), incx1, incx2, IntPtr.Zero, 0, 0, descriptor, ref info);
                    }
                }
                if (info != 0) {
                    throw new ILInvalidOperationException(String.Format("error: {0}th parameter was invalid", -info));
                }
            }
        }
        [SecuritySafeCritical]
        private void fft2dInplace(ILOutArray<complex> A, int mode) {
            using (ILScope.Enter(A)) {
                System.Diagnostics.Debug.Assert(A.Size.NumberOfDimensions >= 2);
                int N = A.Size[1], M = A.Size[0], info = 0;
                int MN = M * N;
                // number of transformations
                int nseq = A.Size.NumberOfElements / (MN);
                double scale = (mode == ACMLValues.Backwards) ? 1.0 / MN : 1.0;
                string hash = hashPlan(ACMLValues.Double, ACMLValues.Complex, M, N, "zfft2dx");
                IntPtr descriptor;
                lock (_lockobject) {
                    if (!m_descriptors.TryGetValue(hash, out descriptor)) {
                        int commLength = (3 * (N + M) + MN + 200) * ACMLValues.Double * 2;
                        try {
                            descriptor = Marshal.AllocCoTaskMem(commLength);
                            zfft2dx(0, scale, 1, 1, M, N, IntPtr.Zero, 1, M, IntPtr.Zero, 0, 0, descriptor, ref info);
                            if (info != 0)
                                throw new ILInvalidOperationException("error creating fft-plan");
                            m_descriptors[hash] = descriptor;
                        } catch (OutOfMemoryException exc) {
                            if (m_descriptors.Count > 0) {
                                FreeAllDescriptors();
                                descriptor = Marshal.AllocCoTaskMem(commLength);
                                zfft2dx(0, scale, 1, 1, M, N, IntPtr.Zero, 1, M, IntPtr.Zero, 0, 0, descriptor, ref info);
                                if (info != 0)
                                    throw new ILInvalidOperationException("error creating fft-plan");
                                m_descriptors[hash] = descriptor;
                            }
                            throw exc;
                        }
                    }
                    // do the transform(s)
                    fixed (complex* start = A.GetArrayForWrite()) {
                        for (int i = 0; i < nseq && info == 0; i++)
                            zfft2dx(mode, scale, 1, 1, M, N, (IntPtr)(start + i * MN), 1, M, IntPtr.Zero, 0, 0, descriptor, ref info);
                    }
                }
                if (info != 0) {
                    throw new ILInvalidOperationException(String.Format("error: {0}th parameter was invalid", -info));
                }
            }
        }
        [SecuritySafeCritical]
        private void fft1dInplace(int dim, ILOutArray<fcomplex> A, int mode) {
            using (ILScope.Enter(A)) {
                int N = A.Size[dim], info = 0;
                // spacing between elements
                int incx1 = A.Size.SequentialIndexDistance(dim);
                // storage of subsequent transformations
                int incx2 = incx1 * N;
                // number of transformations
                int nseq = A.Size.NumberOfElements / incx2;
                float scale = (mode == ACMLValues.Backwards) ? 1.0f / N : 1.0f;
                string hash = hashPlan(ACMLValues.Single, ACMLValues.Complex, N, "cfft1mx");
                IntPtr descriptor;
                lock (_lockobject) {
                    if (!m_descriptors.TryGetValue(hash, out descriptor)) {
                        int commLength = (3 * N + 100) * ACMLValues.Single * 2;
                        try {
                            descriptor = Marshal.AllocCoTaskMem(commLength);
                            cfft1mx(0, 1.0f, 1, 1, N, IntPtr.Zero, 1, 1, IntPtr.Zero, 0, 0, descriptor, ref info);
                            if (info != 0)
                                throw new ILInvalidOperationException("error creating fft-plan");
                            m_descriptors[hash] = descriptor;
                        } catch (OutOfMemoryException exc) {
                            if (m_descriptors.Count > 0) {
                                FreeAllDescriptors();
                                descriptor = Marshal.AllocCoTaskMem(commLength);
                                cfft1mx(0, 1.0f, 1, 1, N, IntPtr.Zero, 1, 1, IntPtr.Zero, 0, 0, descriptor, ref info);
                                if (info != 0)
                                    throw new ILInvalidOperationException("error creating fft-plan");
                                m_descriptors[hash] = descriptor;
                            }
                            throw exc;
                        }
                    }
                    // do the transform(s)
                    fixed (fcomplex* start = A.GetArrayForWrite()) {
                        for (int i = 0; i < incx1 && info == 0; i++)
                            cfft1mx(mode, scale, 1, nseq, N, (IntPtr)(start + i), incx1, incx2, IntPtr.Zero, 0, 0, descriptor, ref info);
                    }
                }
                if (info != 0) {
                    throw new ILInvalidOperationException(String.Format("error: {0}th parameter was invalid", -info));
                }
            }
        }
        [SecuritySafeCritical]
        private void fft2dInplace(ILOutArray<fcomplex> A, int mode) {
            using (ILScope.Enter(A)) {
                System.Diagnostics.Debug.Assert(A.Size.NumberOfDimensions >= 2);
                int N = A.Size[1], M = A.Size[0], info = 0;
                int MN = M * N;
                // number of transformations
                int nseq = A.Size.NumberOfElements / (MN);
                float scale = (mode == ACMLValues.Backwards) ? 1.0f / MN : 1.0f;
                string hash = hashPlan(ACMLValues.Single, ACMLValues.Complex, M, N, "cfft2dx");
                IntPtr descriptor;
                lock (_lockobject) {
                    if (!m_descriptors.TryGetValue(hash, out descriptor)) {
                        int commLength = (3 * (N + M) + MN + 200) * ACMLValues.Single * 2;
                        try {
                            descriptor = Marshal.AllocCoTaskMem(commLength);
                            cfft2dx(0, scale, 1, 1, M, N, IntPtr.Zero, 1, M, IntPtr.Zero, 0, 0, descriptor, ref info);
                            if (info != 0)
                                throw new ILInvalidOperationException("error creating fft-plan");
                            m_descriptors[hash] = descriptor;
                        } catch (OutOfMemoryException exc) {
                            if (m_descriptors.Count > 0) {
                                FreeAllDescriptors();
                                descriptor = Marshal.AllocCoTaskMem(commLength);
                                cfft2dx(0, scale, 1, 1, M, N, IntPtr.Zero, 1, M, IntPtr.Zero, 0, 0, descriptor, ref info);
                                if (info != 0)
                                    throw new ILInvalidOperationException("error creating fft-plan");
                                m_descriptors[hash] = descriptor;
                            }
                            throw;
                        }
                    }
                    // do the transform(s)
                    fixed (fcomplex* start = A.GetArrayForWrite()) {
                        for (int i = 0; i < nseq && info == 0; i++)
                            cfft2dx(mode, scale, 1, 1, M, N, (IntPtr)(start + i * MN), 1, M, IntPtr.Zero, 0, 0, descriptor, ref info);
                    }
                }
                if (info != 0) {
                    throw new ILInvalidOperationException(String.Format("error: {0}th parameter was invalid", -info));
                }
            }
        }

        private static string hashPlan(int precision, int realcomplex, int N, string funcname) {
            return hashPlan(precision,realcomplex,0,N,funcname); 
        }
        private static string hashPlan(int precision, int realcomplex, int M, int N, string funcname) {
            return String.Format("{0}|{1}|{2}|{3}|{4}",precision, realcomplex,M,N,funcname); 
        }

        [SecuritySafeCritical]
        private void FreeAllDescriptors() {
            lock (_lockobject) {
                if (m_descriptors != null) {
                    foreach (IntPtr p in m_descriptors.Values) {
                        if (p != IntPtr.Zero)
                            Marshal.FreeCoTaskMem(p); 
                    }
                    m_descriptors.Clear(); 
                }
            }
        }   
        
        #endregion

        #region IDisposable Member

        public void Dispose() {
            Dispose(true); 
            /* we cannot supress finalize,since the class will only wipe all 
             * cached cotask memory. Subsequent calls would potentially allocate these again. 
             * Therefore the finalizer must perform that check at the end. 
             */ 
            // GC.SuppressFinalize(this); 
        }

        protected virtual void Dispose (bool manual) {
            if (m_descriptors != null && m_descriptors.Count > 0) {
                if (manual) {
                    FreeAllDescriptors();             
                }
            }
        }

        ~ILACMLFFT () {
            Dispose(true);     
        }
        #endregion

    }
}
