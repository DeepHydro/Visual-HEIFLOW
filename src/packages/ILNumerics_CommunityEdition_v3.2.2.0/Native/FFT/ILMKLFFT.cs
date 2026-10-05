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
    /// MKL configuration parameters (constant definitions)
    /// </summary>
    internal unsafe sealed class MKLParameter {
        /* Domain for forward transform. No default value */
        public static readonly int FORWARD_DOMAIN = 0;

        /* Dimensionality; or rank. No default value */
        public static readonly int DIMENSION = 1;

        /* Length(s) of transform. No default value */
        public static readonly int LENGTHS = 2;

        /* Floating point precision. No default value */
        public static readonly int PRECISION = 3;

        /* Scale factor for forward transform [1.0] */
        public static readonly int FORWARD_SCALE  = 4;

        /* Scale factor for backward transform [1.0] */
        public static readonly int BACKWARD_SCALE = 5;

        /* Exponent sign for forward transform [NEGATIVE]  */
        /* FORWARD_SIGN = 6; ## NOT IMPLEMENTED */

        /* Number of data sets to be transformed [1] */
        public static readonly int NUMBER_OF_TRANSFORMS = 7;

        /* Storage of finite complex-valued sequences in complex domain
           [COMPLEX_COMPLEX] */
        public static readonly int COMPLEX_STORAGE = 8;

        /* Storage of finite real-valued sequences in real domain
           [REAL_REAL] */
        public static readonly int REAL_STORAGE = 9;

        /* Storage of finite complex-valued sequences in conjugate-even
           domain [COMPLEX_REAL] */
        public static readonly int CONJUGATE_EVEN_STORAGE = 10;

        /* Placement of result [INPLACE] */
        public static readonly int PLACEMENT = 11;

        /* Generalized strides for input data layout [tight; row-major for C] */
        public static readonly int INPUT_STRIDES = 12;

        /* Generalized strides for output data layout [tight; row-major
           for C] */
        public static readonly int OUTPUT_STRIDES = 13;

        /* Distance between first input elements for multiple transforms
           [0] */
        public static readonly int INPUT_DISTANCE = 14;

        /* Distance between first output elements for multiple transforms
           [0] */
        public static readonly int OUTPUT_DISTANCE = 15;

        /* Ordering of the result [ORDERED] */
        public static readonly int ORDERING = 18;

        /* Possible transposition of result [NONE] */
        public static readonly int TRANSPOSE = 19;

        /* User-settable descriptor name [""] */
        public static readonly int DESCRIPTOR_NAME = 20; /* DEPRECATED */

        /* Packing format for COMPLEX_REAL storage of finite
           conjugate-even sequences [CCS_FORMAT] */
        public static readonly int PACKED_FORMAT = 21;

        /* Commit status of the descriptor - R/O parameter */
        public static readonly int COMMIT_STATUS = 22;

        /* Version string for this DFTI implementation - R/O parameter */
        public static readonly int VERSION = 23;

        /* Number of user threads that share the descriptor [1] */
        public static readonly int NUMBER_OF_USER_THREADS = 26;
    }
    /// <summary>
    /// MKL configuration values (constant definitions) 
    /// </summary>
    internal unsafe sealed class MKLValues {
        public static readonly int MKL_ALL = 0;
        public static readonly int MKL_BLAS = 1;
        public static readonly int MKL_FFT = 2;
        public static readonly int MKL_VML = 3;
        /* Values of the descriptor configuration parameters */
        /* COMMIT_STATUS */
        public static readonly int COMMITTED = 30;
        public static readonly int UNCOMMITTED = 31;

        /* FORWARD_DOMAIN */
        public static readonly int COMPLEX = 32;
        public static readonly int REAL = 33;
            /* CONJUGATE_EVEN = 34;   ## NOT IMPLEMENTED */

        /* PRECISION */
        public static readonly int SINGLE = 35;
        public static readonly int DOUBLE = 36;

        /* COMPLEX_STORAGE and CONJUGATE_EVEN_STORAGE */
        public static readonly int COMPLEX_COMPLEX = 39;
        public static readonly int COMPLEX_REAL = 40;

        /* REAL_STORAGE */
        public static readonly int REAL_COMPLEX = 41;
        public static readonly int REAL_REAL = 42;

        /* PLACEMENT */
        public static readonly int INPLACE = 43;          /* Result overwrites input */
        public static readonly int NOT_INPLACE = 44;      /* Have another place for result */

        /* ORDERING */
        public static readonly int ORDERED = 48;
        public static readonly int BACKWARD_SCRAMBLED = 49;

        /* Allow/avoid certain usages */
        public static readonly int ALLOW = 51;            /* Allow transposition or workspace */
        public static readonly int NONE = 53;

        /* PACKED_FORMAT (for storing congugate-even finite sequence
           in real array) */
        public static readonly int CCS_FORMAT = 54;       /* Complex conjugate-symmetric */
        public static readonly int PACK_FORMAT = 55;      /* Pack format for real DFT */
        public static readonly int PERM_FORMAT = 56;      /* Perm format for real DFT */
        public static readonly int CCE_FORMAT = 57;       /* Complex conjugate-even */
        
        /* Error classes */
        public static readonly int  NO_ERROR = 0; 
        public static readonly int  MEMORY_ERROR = 1;
        public static readonly int  INVALID_CONFIGURATION = 2;
        public static readonly int  INCONSISTENT_CONFIGURATION = 3;
        public static readonly int  MULTITHREADED_ERROR = 4;
        public static readonly int  BAD_DESCRIPTOR = 5;
        public static readonly int  UNIMPLEMENTED = 6;
        public static readonly int  MKL_INTERNAL_ERROR = 7;
        public static readonly int  NUMBER_OF_THREADS_ERROR = 8;
        public static readonly int  ONED_LENGTH_EXCEEDS_INT32 = 9;

        public static readonly int  MAX_MESSAGE_LENGTH = 40; /* Maximum length of error string */
        public static readonly int  MAX_NAME_LENGTH = 10;    /* Maximum length of descriptor name */
        public static readonly int  VERSION_LENGTH = 198;    /* Maximum length of MKL version string */
    }
    /// <summary>
    /// import functions (pinvoke)
    /// </summary>
    internal unsafe sealed class MKLImports {

        #region pinvoke definitions 

        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern int mkl_domain_set_num_threads(ref int num, ref int mask);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern int mkl_domain_get_max_threads(ref int mask);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern void mkl_set_num_threads(int num);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern int mkl_get_max_threads();
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void omp_set_num_threads(int num);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern int omp_get_num_threads();
        
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern void mkl_set_dynamic(ref int boolean_value);

        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        public static extern int DftiCreateDescriptor(ref IntPtr pDescriptor, int precision, int domain, int dimCount, int dim1);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        public static extern int DftiCreateDescriptor(ref IntPtr pDescriptor, int precision, int domain, int dimCount, int[] dims);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiFreeDescriptor(ref IntPtr pDescriptor);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiCommitDescriptor(IntPtr pDescriptor);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiErrorClass(int status, int ERROR_CLASS);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiSetValue(IntPtr pDescriptor, int parameter, int value);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiSetValue(IntPtr pDescriptor, int parameter, double value);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiSetValue(IntPtr pDescriptor, int parameter, int[] values);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiGetValue(IntPtr pDescriptor, int parameter, ref int value);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiGetValue(IntPtr pDescriptor, int parameter, ref double value);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiComputeForward(IntPtr pDescriptor, IntPtr pArray);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
        public static extern int DftiComputeForward(IntPtr pDescriptor, IntPtr pInArray, IntPtr pOutArray);

        /** DFTI native DftiComputeForward declaration */
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = false)]
        internal static extern int DftiComputeForward(IntPtr desc, [In] double[] x_in, [Out] double[] x_out);

        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiComputeBackward(IntPtr pDescriptor, IntPtr pArray);
        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern int DftiComputeBackward(IntPtr pDescriptor, IntPtr pInArray, IntPtr pOutArray);

        /** DFTI native DftiComputeBackward declaration */
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = false)]
        internal static extern int DftiComputeBackward(IntPtr desc, [In] double[] x_in, [Out] double[] x_out);

        [DllImport("mkl_custom",CallingConvention =CallingConvention.Cdecl),SuppressUnmanagedCodeSecurity, SecurityCritical]
		public static extern string DftiErrorMessage(int errID);
        #endregion
    }

    internal sealed class MKLDescriptor : IDisposable {
        private int[] Stride;  
        private int Precision; 
        private int RealComplex; 
        private bool InPlace; 
        private ILSize Dimension; 
        private int AlongDimensionIndex; 
        private IntPtr Descriptor = IntPtr.Zero; 
        private int Dimensionality; 
        private int LocalLoopIncrement; 
        private int LocalLoopLength; 

        [SecuritySafeCritical]
        public IntPtr GetDescriptor(int precision, int realComplex, int dimensionality, 
                                    int dim, ILSize dimension, bool inplace,
                                    out int localLoopLength, out int localLoopIncrement) {
            // DEBUG: remove "false" for descriptor caching!
            if (Descriptor != IntPtr.Zero 
                && precision == Precision 
                && realComplex == RealComplex 
                && dimensionality == Dimensionality
                && dimension.IsSameShape(Dimension)
                && dim == AlongDimensionIndex
                && inplace == InPlace) {
                // reuse old descriptor 
                localLoopIncrement = LocalLoopIncrement;
                localLoopLength = LocalLoopLength;
                return Descriptor; 
            } else {

                // must create a new one 
                if (Descriptor != IntPtr.Zero) {
                    MKLImports.DftiFreeDescriptor(ref Descriptor);
                }
                IntPtr tmpDescriptor = IntPtr.Zero; 
                int error; 
                if (dimensionality == 1) {
#region one dimensional transforms
                    error = MKLImports.DftiCreateDescriptor(ref tmpDescriptor, precision ,realComplex, dimensionality, dimension[dim]);
                    if (error != 0) {
                        throw new ILInvalidOperationException ("error creating descriptor: " + MKLImports.DftiErrorMessage(error)); 
                    }

                    // storage,number of subsequent transformations
                    int nrTransGlobal = dimension.NumberOfElements / dimension[dim];
                    int nrTransPerPass; 
                    if (dim == 0) {
                        // MKL does all transf. at once 
                        MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.NUMBER_OF_TRANSFORMS, nrTransGlobal); 
                        int distances = dimension[0]; 
                        MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.INPUT_DISTANCE, distances); 
                        MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.OUTPUT_DISTANCE, distances); 
                        localLoopIncrement = 1; 
                        localLoopLength = 1;
                        nrTransPerPass = 1; 
                    } else {
                        nrTransPerPass = dimension.SequentialIndexDistance(dim); 
                        MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.NUMBER_OF_TRANSFORMS, nrTransPerPass); 
                        int distances = 1; 
                        MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.INPUT_DISTANCE, distances); 
                        MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.OUTPUT_DISTANCE, distances); 
                        localLoopIncrement = dimension.SequentialIndexDistance(dim) * dimension[dim]; 
                        localLoopLength = nrTransGlobal / nrTransPerPass; 
                    }

                    // spacing between elements
                    if (Stride == null || Stride.Length < 2) {
                        Stride = new int[10];
                    }
                    Stride[1] = nrTransPerPass; 
                    MKLImports.DftiSetValue(tmpDescriptor, MKLParameter.INPUT_STRIDES, Stride);
                    error = MKLImports.DftiSetValue(tmpDescriptor, MKLParameter.OUTPUT_STRIDES, Stride);
                    if (error != 0) {
                        throw new ILInvalidOperationException("error: " + MKLImports.DftiErrorMessage(error));
                    }
#endregion
                } else {
#region multidimensional transforms 
                    int[] tmpDims = dimension.ToIntArray(10); 
                    error = MKLImports.DftiCreateDescriptor(ref tmpDescriptor, precision ,realComplex, dimensionality, tmpDims);            
                    // MKL will handle to do all transforms at once 
                    localLoopIncrement = 1; 
                    localLoopLength = 1; 
                    // how many transformations ? 
                    int nrElementsPerPass = dimension.SequentialIndexDistance(dimensionality); 
                    MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.NUMBER_OF_TRANSFORMS, dimension.NumberOfElements / nrElementsPerPass); 
                    MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.INPUT_DISTANCE, nrElementsPerPass); 
                    MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.OUTPUT_DISTANCE, nrElementsPerPass);

                    // spacing between elements
                    if (Stride == null || Stride.Length < dimensionality + 1) {
                        Stride = new int[dimensionality + 1];
                    }
                    int[] seqDistances = dimension.GetSequentialIndexDistances(dimensionality);
                    for (int i = dimensionality; i-- > 0; ) {
                        Stride[i + 1] = seqDistances[i];
                    }
                    //Stride[0] = 0; // should never gotten changed
                    MKLImports.DftiSetValue(tmpDescriptor, MKLParameter.INPUT_STRIDES, Stride);
                    error = MKLImports.DftiSetValue(tmpDescriptor, MKLParameter.OUTPUT_STRIDES, Stride);
                    if (error != 0) {
                        throw new ILInvalidOperationException("error: " + MKLImports.DftiErrorMessage(error));
                    }
#endregion
                }
                //// backward scale 
                //if (!forwardTransform) {
                //    MKLImports.DftiSetValue(tmpDescriptor,MKLParameter.BACKWARD_SCALE, __arglist(1.0/Dimension[dim]));
                //}
                if (!inplace) {
                    error = MKLImports.DftiSetValue(tmpDescriptor, MKLParameter.PLACEMENT, MKLValues.NOT_INPLACE);
                    //error = MKLImports.DftiSetValue(tmpDescriptor, MKLParameter.REAL_STORAGE, __arglist(MKLValues.REAL_REAL));
                    error = MKLImports.DftiSetValue(tmpDescriptor, MKLParameter.CONJUGATE_EVEN_STORAGE, MKLValues.COMPLEX_COMPLEX);
                }
                
                error = MKLImports.DftiCommitDescriptor(tmpDescriptor);
                if (error != 0) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                
                Precision = precision; 
                RealComplex = realComplex; 
                Dimensionality = dimensionality; 
                Dimension = dimension; 
                AlongDimensionIndex = dim; 
                Descriptor = tmpDescriptor; 
                LocalLoopIncrement = localLoopIncrement;
                LocalLoopLength = localLoopLength;
                InPlace = inplace; 
                return Descriptor; 
            }
        }

        public string GetDescriptorInfo() {
            if (Descriptor != IntPtr.Zero) {
                return GetDescriptorInfo(Descriptor);
            } else {
                return "(no descriptor set)";
            }
        }
        [SecuritySafeCritical]
        public string GetDescriptorInfo(IntPtr descriptor) {
            StringBuilder ret = new StringBuilder();
            int val = 0;
            MKLImports.DftiGetValue(descriptor, MKLParameter.PRECISION, ref val); ret.Append("Precision: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.FORWARD_DOMAIN, ref val); ret.Append("ForwDomain: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.DIMENSION, ref val); ret.Append("DIMENSION: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.LENGTHS, ref val); ret.Append("Length: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.PLACEMENT, ref val); ret.Append("PLACEMENT: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.NUMBER_OF_TRANSFORMS, ref val); ret.Append("NUMBER_OF_TRANSFORMS: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.COMPLEX_STORAGE, ref val); ret.Append("COMPLEX_STORAGE: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.CONJUGATE_EVEN_STORAGE, ref val); ret.Append("CONJUGATE_EVEN_STORAGE: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.INPUT_DISTANCE, ref val); ret.Append("INPUT_DISTANCE: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.OUTPUT_DISTANCE, ref val); ret.Append("OUTPUT_DISTANCE: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.INPUT_STRIDES, ref val); ret.Append("INPUT_STRIDES: " + val + " " + Environment.NewLine);
            MKLImports.DftiGetValue(descriptor, MKLParameter.OUTPUT_STRIDES, ref val); ret.Append("OUTPUT_STRIDES: " + val + " " + Environment.NewLine);
            return ret.ToString();
        }

        #region IDisposable Members
        [SecuritySafeCritical]
        public void Dispose() {
            lock (this)  
            if (Descriptor != IntPtr.Zero) {
                Descriptor = IntPtr.Zero; 
                MKLImports.DftiFreeDescriptor(ref Descriptor);
            }
        }

        ~MKLDescriptor () {
            Dispose(); 
        }
        #endregion
    }
    /// <summary>
    /// Wrapper for FFT interface using MKL 10_03
    /// </summary>
    public class ILMKLFFT : IILFFT {

        public static void Init() {
            try {
                // unless the user explicitely configured the number of threads, let omp configure it! 
                if (Settings.MaxNumberThreadsConfigured) {
                    SetNumThreads(Settings.MaxNumberThreads); 
                }
            } catch (System.IO.FileNotFoundException) {}
        }

        [SecuritySafeCritical]
        public static void SetNumThreads(int numThreads) {
            int mklfft = MKLValues.MKL_FFT; 
            MKLImports.mkl_domain_set_num_threads(ref numThreads,ref mklfft); 
        }

        [SecuritySafeCritical]
        public static int GetMaxThreads() {
            int mklfft = MKLValues.MKL_FFT; 
            return MKLImports.mkl_domain_get_max_threads(ref mklfft); 
        }

        [SecuritySafeCritical]
        private static void SetDynamicThreads(bool dynamic) {
            int val = dynamic ? 1 : 0;
            MKLImports.mkl_set_dynamic(ref val);
        }

        #region attributes 
        [ThreadStatic]
        private static int[] s_strides;
        private static int[] Strides {
            get {
                if (s_strides == null) {
                     s_strides = new int[10];
                }
                return s_strides; 
            }
        }
        [ThreadStatic]
        private static MKLDescriptor s_descriptor;
        private static MKLDescriptor DescriptorCache {
            get  {
                if (s_descriptor == null) {
                    s_descriptor = new MKLDescriptor(); 
                }
                return s_descriptor; 
            }
        }
        #endregion

        #region constructor 
        public ILMKLFFT () {
            Init();  
        }
        #endregion

        #region IILFFT Member - 1-D



        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTForward1D(ILInArray< double> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A,null) || dim < 0) 
                    throw new ILArgumentException("invalid parameter"); 
                if (A.IsEmpty) return ILMath.empty< complex >(A.Size); 
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) 
                    return  ILMath.tocomplex(A); 
                // prepare output array 
                ILArray< complex > ret =  ILMath.tocomplex(A) ;
                int localLoopLength, localLoopIncrement; 

                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.DOUBLE,MKLValues.COMPLEX,1,dim,A.Size,true,
                                    out localLoopLength, out localLoopIncrement);
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( complex * retArr = ret.GetArrayForWrite()) {
                        for (int i = 0; i < localLoopLength && error == 0; i ++)
                            error =   MKLImports.DftiComputeForward(descriptor, (IntPtr)(retArr + i * localLoopIncrement));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                

                return ret;
            }
        }


#region HYCALPER AUTO GENERATED CODE


        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTBackward1D(ILInArray< fcomplex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A,null) || dim < 0) 
                    throw new ILArgumentException("invalid parameter"); 
                if (A.IsEmpty) return ILMath.empty< fcomplex >(A.Size); 
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) 
                    return  A.C; 
                // prepare output array 
                ILArray< fcomplex > ret =  A.C ;
                int localLoopLength, localLoopIncrement; 

                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.SINGLE,MKLValues.COMPLEX,1,dim,A.Size,true,
                                    out localLoopLength, out localLoopIncrement);
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( fcomplex * retArr = ret.GetArrayForWrite()) {
                        for (int i = 0; i < localLoopLength && error == 0; i ++)
                            error =  MKLImports.DftiComputeBackward(descriptor, (IntPtr)(retArr + i * localLoopIncrement));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                ret = ret / new fcomplex(A.D[dim],0);
                return ret;
            }
        }


        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTBackward1D(ILInArray< complex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A,null) || dim < 0) 
                    throw new ILArgumentException("invalid parameter"); 
                if (A.IsEmpty) return ILMath.empty< complex >(A.Size); 
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) 
                    return  A.C; 
                // prepare output array 
                ILArray< complex > ret =  A.C ;
                int localLoopLength, localLoopIncrement; 

                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.DOUBLE,MKLValues.COMPLEX,1,dim,A.Size,true,
                                    out localLoopLength, out localLoopIncrement);
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( complex * retArr = ret.GetArrayForWrite()) {
                        for (int i = 0; i < localLoopLength && error == 0; i ++)
                            error =  MKLImports.DftiComputeBackward(descriptor, (IntPtr)(retArr + i * localLoopIncrement));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                ret = ret / new complex(A.D[dim],0);
                return ret;
            }
        }


        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTForward1D(ILInArray< fcomplex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A,null) || dim < 0) 
                    throw new ILArgumentException("invalid parameter"); 
                if (A.IsEmpty) return ILMath.empty< fcomplex >(A.Size); 
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) 
                    return  A.C; 
                // prepare output array 
                ILArray< fcomplex > ret =  A.C ;
                int localLoopLength, localLoopIncrement; 

                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.SINGLE,MKLValues.COMPLEX,1,dim,A.Size,true,
                                    out localLoopLength, out localLoopIncrement);
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( fcomplex * retArr = ret.GetArrayForWrite()) {
                        for (int i = 0; i < localLoopLength && error == 0; i ++)
                            error =  MKLImports.DftiComputeForward(descriptor, (IntPtr)(retArr + i * localLoopIncrement));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                
                return ret;
            }
        }


        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTForward1D(ILInArray< float> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A,null) || dim < 0) 
                    throw new ILArgumentException("invalid parameter"); 
                if (A.IsEmpty) return ILMath.empty< fcomplex >(A.Size); 
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) 
                    return  ILMath.tofcomplex(A); 
                // prepare output array 
                ILArray< fcomplex > ret =  ILMath.tofcomplex(A) ;
                int localLoopLength, localLoopIncrement; 

                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.SINGLE,MKLValues.COMPLEX,1,dim,A.Size,true,
                                    out localLoopLength, out localLoopIncrement);
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( fcomplex * retArr = ret.GetArrayForWrite()) {
                        for (int i = 0; i < localLoopLength && error == 0; i ++)
                            error =  MKLImports.DftiComputeForward(descriptor, (IntPtr)(retArr + i * localLoopIncrement));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                
                return ret;
            }
        }


        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTForward1D(ILInArray< complex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A,null) || dim < 0) 
                    throw new ILArgumentException("invalid parameter"); 
                if (A.IsEmpty) return ILMath.empty< complex >(A.Size); 
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) 
                    return  A.C; 
                // prepare output array 
                ILArray< complex > ret =  A.C ;
                int localLoopLength, localLoopIncrement; 

                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.DOUBLE,MKLValues.COMPLEX,1,dim,A.Size,true,
                                    out localLoopLength, out localLoopIncrement);
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( complex * retArr = ret.GetArrayForWrite()) {
                        for (int i = 0; i < localLoopLength && error == 0; i ++)
                            error =  MKLImports.DftiComputeForward(descriptor, (IntPtr)(retArr + i * localLoopIncrement));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                
                return ret;
            }
        }


#endregion HYCALPER AUTO GENERATED CODE




        [SecuritySafeCritical]
        public ILRetArray< double> FFTBackwSym1D(ILInArray< complex> A, int dim) {
            using (ILScope.Enter(A)) {  
                if (object.Equals(A,null) || dim < 0) 
                    throw new ILArgumentException("invalid parameter"); 
                if (A.IsEmpty) return ILArray< double >.empty(A.Size); 
                if (A.IsScalar || A.Size[dim] == 1) 
                    return ILMath.real(A.C); 
                // prepare output array, if we query the memory directly from
                // memory pool, we prevent from clearing the elements since every 
                // single one will be overwritten by fft anyway
                int error = 0,inDim = A.Size[dim];   
                 double [] retArr = ILMemoryPool.Pool.New< double >(A.Size.NumberOfElements); 
                ILArray< double > ret = ILMath.array< double >(retArr,A.Size); 
                int localLoopLength, localLoopIncrement; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.DOUBLE , MKLValues.REAL, 1, dim, A.S, false, 
                                    out localLoopLength, out localLoopIncrement);
                // do the transform(s)
                unsafe {
                    fixed ( double * retArrP = ret.GetArrayForWrite()) 
                    fixed ( complex * pA = A.GetArrayForRead()) {
                        for (int i = 0; i < (localLoopLength * localLoopIncrement) && error == 0; i += localLoopIncrement)
                            error = MKLImports.DftiComputeBackward(descriptor, (IntPtr)(pA + i), (IntPtr)(retArrP + i));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                return ret / inDim;
            }
        }


#region HYCALPER AUTO GENERATED CODE



        [SecuritySafeCritical]
        public ILRetArray< float> FFTBackwSym1D(ILInArray< fcomplex> A, int dim) {
            using (ILScope.Enter(A)) {  
                if (object.Equals(A,null) || dim < 0) 
                    throw new ILArgumentException("invalid parameter"); 
                if (A.IsEmpty) return ILArray< float >.empty(A.Size); 
                if (A.IsScalar || A.Size[dim] == 1) 
                    return ILMath.real(A.C); 
                // prepare output array, if we query the memory directly from
                // memory pool, we prevent from clearing the elements since every 
                // single one will be overwritten by fft anyway
                int error = 0,inDim = A.Size[dim];   
                float [] retArr = ILMemoryPool.Pool.New< float >(A.Size.NumberOfElements); 
                ILArray< float > ret = ILMath.array< float >(retArr,A.Size); 
                int localLoopLength, localLoopIncrement; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.SINGLE , MKLValues.REAL, 1, dim, A.S, false, 
                                    out localLoopLength, out localLoopIncrement);
                // do the transform(s)
                unsafe {
                    fixed ( float * retArrP = ret.GetArrayForWrite()) 
                    fixed ( fcomplex * pA = A.GetArrayForRead()) {
                        for (int i = 0; i < (localLoopLength * localLoopIncrement) && error == 0; i += localLoopIncrement)
                            error = MKLImports.DftiComputeBackward(descriptor, (IntPtr)(pA + i), (IntPtr)(retArrP + i));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                return ret / inDim;
            }
        }


#endregion HYCALPER AUTO GENERATED CODE

        #endregion

        #region IILFFT Member - n-D



        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTForward(ILInArray< double> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0 ) 
                    throw new ILArgumentException("invalid argument!");
                if (A.IsEmpty) return ILArray< complex > .empty(A.Size); 
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1)) 
                    return  ILMath.tocomplex(A) ; 
                if (nDims > A.Size.NumberOfDimensions) 
                    nDims = A.Size.NumberOfDimensions; 
                
                if (nDims > 3) return FFTForward(ILMath.tocomplex(A),nDims); 
                // prepare output array 
                ILArray< complex > ret =  ILMath.tocomplex(A) ;
                int localLoopLength, localLoopIncrement; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.DOUBLE , MKLValues.COMPLEX, nDims, 0, A.Size, true,
                                    out localLoopLength, out localLoopIncrement); 
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( complex * retArr = ret.GetArrayForWrite()) {
                        error =  MKLImports.DftiComputeForward (descriptor, (IntPtr)retArr);
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                

                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE


        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTBackward(ILInArray< complex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0 ) 
                    throw new ILArgumentException("invalid argument!");
                if (A.IsEmpty) return ILArray< complex > .empty(A.Size); 
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1)) 
                    return  A.C ; 
                if (nDims > A.Size.NumberOfDimensions) 
                    nDims = A.Size.NumberOfDimensions; 
                
                // prepare output array 
                ILArray< complex > ret =  A.C ;
                int localLoopLength, localLoopIncrement; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.DOUBLE , MKLValues.COMPLEX, nDims, 0, A.Size, true,
                                    out localLoopLength, out localLoopIncrement); 
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( complex * retArr = ret.GetArrayForWrite()) {
                        error =  MKLImports.DftiComputeBackward (descriptor, (IntPtr)retArr);
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                ret = ret / new complex(A.D.SequentialIndexDistance(nDims),0);
                return ret;
            }
        }

        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTBackward(ILInArray< fcomplex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0 ) 
                    throw new ILArgumentException("invalid argument!");
                if (A.IsEmpty) return ILArray< fcomplex > .empty(A.Size); 
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1)) 
                    return  A.C ; 
                if (nDims > A.Size.NumberOfDimensions) 
                    nDims = A.Size.NumberOfDimensions; 
                
                // prepare output array 
                ILArray< fcomplex > ret =  A.C ;
                int localLoopLength, localLoopIncrement; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.SINGLE , MKLValues.COMPLEX, nDims, 0, A.Size, true,
                                    out localLoopLength, out localLoopIncrement); 
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( fcomplex * retArr = ret.GetArrayForWrite()) {
                        error =  MKLImports.DftiComputeBackward (descriptor, (IntPtr)retArr);
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                ret = ret / new fcomplex(A.D.SequentialIndexDistance(nDims),0);
                return ret;
            }
        }

        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTForward(ILInArray< fcomplex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0 ) 
                    throw new ILArgumentException("invalid argument!");
                if (A.IsEmpty) return ILArray< fcomplex > .empty(A.Size); 
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1)) 
                    return  A.C ; 
                if (nDims > A.Size.NumberOfDimensions) 
                    nDims = A.Size.NumberOfDimensions; 
                
                // prepare output array 
                ILArray< fcomplex > ret =  A.C ;
                int localLoopLength, localLoopIncrement; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.SINGLE , MKLValues.COMPLEX, nDims, 0, A.Size, true,
                                    out localLoopLength, out localLoopIncrement); 
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( fcomplex * retArr = ret.GetArrayForWrite()) {
                        error =  MKLImports.DftiComputeForward (descriptor, (IntPtr)retArr);
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                
                return ret;
            }
        }

        [SecuritySafeCritical]
        public ILRetArray< fcomplex>  FFTForward(ILInArray< float> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0 ) 
                    throw new ILArgumentException("invalid argument!");
                if (A.IsEmpty) return ILArray< fcomplex > .empty(A.Size); 
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1)) 
                    return  ILMath.tofcomplex(A) ; 
                if (nDims > A.Size.NumberOfDimensions) 
                    nDims = A.Size.NumberOfDimensions; 
                if (nDims > 3) return FFTForward(ILMath.tofcomplex(A),nDims);
                // prepare output array 
                ILArray< fcomplex > ret =  ILMath.tofcomplex(A) ;
                int localLoopLength, localLoopIncrement; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.SINGLE , MKLValues.COMPLEX, nDims, 0, A.Size, true,
                                    out localLoopLength, out localLoopIncrement); 
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( fcomplex * retArr = ret.GetArrayForWrite()) {
                        error =  MKLImports.DftiComputeForward (descriptor, (IntPtr)retArr);
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                
                return ret;
            }
        }

        [SecuritySafeCritical]
        public ILRetArray< complex>  FFTForward(ILInArray< complex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0 ) 
                    throw new ILArgumentException("invalid argument!");
                if (A.IsEmpty) return ILArray< complex > .empty(A.Size); 
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1)) 
                    return  A.C ; 
                if (nDims > A.Size.NumberOfDimensions) 
                    nDims = A.Size.NumberOfDimensions; 
                
                // prepare output array 
                ILArray< complex > ret =  A.C ;
                int localLoopLength, localLoopIncrement; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.DOUBLE , MKLValues.COMPLEX, nDims, 0, A.Size, true,
                                    out localLoopLength, out localLoopIncrement); 
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( complex * retArr = ret.GetArrayForWrite()) {
                        error =  MKLImports.DftiComputeForward (descriptor, (IntPtr)retArr);
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                
                return ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
       

        
        [SecuritySafeCritical]
        public ILRetArray< double> FFTBackwSym(ILInArray< complex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0 ) 
                    throw new ILArgumentException("invalid argument!");
                if (A.IsEmpty) return ILMath.empty< double > (A.Size); 
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1)) 
                    return ILMath.real(A.C); 
                if (nDims > A.Size.NumberOfDimensions) 
                    nDims = A.Size.NumberOfDimensions; 
                // MKL currently does not handle more than 3 dimensions this way
                if (nDims > 3) {
                    return ILMath.real(FFTBackward(A,nDims)); 
                }
                // prepare output array, if we query the memory directly from
                // memory pool, we prevent from clearing the elements since every 
                // single one will be overwritten by fft anyway
                 double [] retArr = ILMemoryPool.Pool.New< double >(A.Size.NumberOfElements);
                ILArray< double> ret = ILMath.array< double>(retArr, A.Size); 
                int localLoopIncrement, localLoopLength; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.DOUBLE , MKLValues.REAL,nDims, 0, A.S, false,
                                    out localLoopIncrement, out localLoopLength); 
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( double * retArrP = retArr) 
                    fixed ( complex * inArr = A.GetArrayForRead()) {
                        error = MKLImports.DftiComputeBackward(descriptor, (IntPtr)(inArr),(IntPtr)(retArrP));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                ret = ret / A.S.SequentialIndexDistance(nDims);     
                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE

       
        [SecuritySafeCritical]
        public ILRetArray< float> FFTBackwSym(ILInArray< fcomplex> A, int nDims) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || nDims <= 0 ) 
                    throw new ILArgumentException("invalid argument!");
                if (A.IsEmpty) return ILMath.empty< float > (A.Size); 
                if (A.IsScalar || (A.Size[0] == 1 && nDims == 1)) 
                    return ILMath.real(A.C); 
                if (nDims > A.Size.NumberOfDimensions) 
                    nDims = A.Size.NumberOfDimensions; 
                // MKL currently does not handle more than 3 dimensions this way
                if (nDims > 3) {
                    return ILMath.real(FFTBackward(A,nDims)); 
                }
                // prepare output array, if we query the memory directly from
                // memory pool, we prevent from clearing the elements since every 
                // single one will be overwritten by fft anyway
                float [] retArr = ILMemoryPool.Pool.New< float >(A.Size.NumberOfElements);
                ILArray< float> ret = ILMath.array< float>(retArr, A.Size); 
                int localLoopIncrement, localLoopLength; 
                IntPtr descriptor = DescriptorCache.GetDescriptor( MKLValues.SINGLE , MKLValues.REAL,nDims, 0, A.S, false,
                                    out localLoopIncrement, out localLoopLength); 
                int error = 0;   
                // do the transform(s)
                unsafe {
                    fixed ( float * retArrP = retArr) 
                    fixed ( fcomplex * inArr = A.GetArrayForRead()) {
                        error = MKLImports.DftiComputeBackward(descriptor, (IntPtr)(inArr),(IntPtr)(retArrP));
                    }
                }
                if (isMKLError(error)) {
                    throw new ILInvalidOperationException ("error: " + MKLImports.DftiErrorMessage(error)); 
                }
                ret = ret / A.S.SequentialIndexDistance(nDims);     
                return ret;
            }
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
            DescriptorCache.Dispose(); 
        }

        public bool SpeedyHermitian {
            get { return true; }
        }

        #endregion

        #region private helper
        private static bool isMKLError(int error) {
            // TODO check! only the first 32 bits seem to be relevant for error!
            //return MKLImports.DftiErrorClass(error, MKLValues.NO_ERROR) != 1;
            return error != MKLValues.NO_ERROR; 
        }
        #endregion

    }
}
