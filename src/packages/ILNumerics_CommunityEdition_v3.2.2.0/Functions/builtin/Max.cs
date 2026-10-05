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
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;



namespace ILNumerics {

    public partial class ILMath {
    
       


        /// <summary>
        /// Maximum values along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="I">[Optional] If not null I will hold on return the indices into dim of  
        /// the values found. If I is null those indices will not be computed and I will be ignored.</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array of same inner type and size as A, except for dimension 
        /// 'dim' which will be reduced to length 1.</returns>
        public static  ILRetArray<double>  max(ILInArray<double> A, ILOutArray<int> I = null, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (A.IsEmpty) {
                    if (!object.Equals(I, null))
                        I.a = empty<int>(ILSize.Empty00);
                    return new  ILRetArray<double>(A.Size); 
                }
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) {
                    // scalar or sum over singleton -> return copy
                    if (!object.Equals(I, null))
                        I.a = zeros<int>(A.S);
                    return new ILRetArray<double>(A.C.Storage);
                }
                int[] newDims = A.Size.ToIntArray();
                int leadDimLen = A.Size[dim];
                int newLength = A.Size.NumberOfElements / leadDimLen;
                newDims[dim] = 1;
                
                double[] retArr = ILMemoryPool.Pool.New< double>(newLength);
                #region HYCALPER GLOBAL_INIT

                
                double result;
                
                double curval;
                int[] indices = null;
                bool createIndices = false;
                if (!Object.Equals(I, null)) {
                    indices = ILMemoryPool.Pool.New<int>(newLength);
                    createIndices = true;
                }
                #endregion HYCALPER GLOBAL_INIT
                #region HYCALPER INIT_COMPLEX

                #endregion HYCALPER INIT_COMPLEX

                // physical -> pointer arithmetic
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( double* pOutArr = retArr)
                        fixed ( double* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                            
                            double* lastElement;
                            
                            double* tmpOut = pOutArr;
                            
                            double* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {

                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                    (
                                        
                                         double.IsNaN(result)
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }


                                    #region HYCALPER TAKERESULT

                                    #endregion HYCALPER TAKERESULT
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn;

                                        if (curval > result) {
                                            result = curval;

                                            *tmpInd = (int)(tmpIn - (lastElement - leadDimLen));
                                        }
                                        tmpIn++;
                                    }
                                    *(tmpOut++) = ( double)result;
                                    tmpInd++;
                                }
                            } else {   // no indices
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        
                                         double.IsNaN(result)
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                        {
                                        result = *tmpIn;
                                    }

                                    #region HYCALPER TAKERESULT

                                    #endregion HYCALPER TAKERESULT
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn++;

                                        if (curval > result) {
                                            result = curval;

                                        }
                                    }

                                    *(tmpOut++) = ( double)result;

                                }
                            }
                        }
                    }
                    #endregion physical along 1st leading dimension
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( double* pOutArr = retArr)
                        fixed ( double* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                            
                            double* lastElementOut = newLength + pOutArr - 1;
                            int inLength = A.Size.NumberOfElements - 1;
                            
                            double* lastElementIn = pInArr + inLength;
                            int inc = A.Size.SequentialIndexDistance(dim);
                            
                            double* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                            
                            double* leadEnd;
                            
                            double* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        
                                         double.IsNaN(result)
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore                 
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }

                                    #region HYCALPER TAKERESULT

                                    #endregion HYCALPER TAKERESULT
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;

                                        if (curval > result) {
                                            result = curval;

                                            *tmpInd = (int)(leadDimLen - (leadEnd - tmpIn) / inc);
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( double)result;

                                    tmpOut += inc;
                                    tmpInd += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                        tmpInd -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            } else {  // no indices
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while
#pragma warning disable
                                    (
                                        
                                         double.IsNaN(result)
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                    }

                                    #region HYCALPER TAKERESULT

                                    #endregion HYCALPER TAKERESULT
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;

                                        if (curval > result) {
                                            result = curval;

                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( double)result;

                                    tmpOut += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            }
                        }
                    }
                    #endregion
                }
                if (createIndices) {
                    I.a = array<int>(indices, newDims);
                }
                return new ILRetArray<double>(retArr, newDims);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Maximum values along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="I">[Optional] If not null I will hold on return the indices into dim of  
        /// the values found. If I is null those indices will not be computed and I will be ignored.</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array of same inner type and size as A, except for dimension 
        /// 'dim' which will be reduced to length 1.</returns>
        public static  ILRetArray<Int64>  max(ILInArray<Int64> A, ILOutArray<int> I = null, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (A.IsEmpty) {
                    if (!object.Equals(I, null))
                        I.a = empty<int>(ILSize.Empty00);
                    return new  ILRetArray<Int64>(A.Size); 
                }
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) {
                    // scalar or sum over singleton -> return copy
                    if (!object.Equals(I, null))
                        I.a = zeros<int>(A.S);
                    return new ILRetArray<Int64>(A.C.Storage);
                }
                int[] newDims = A.Size.ToIntArray();
                int leadDimLen = A.Size[dim];
                int newLength = A.Size.NumberOfElements / leadDimLen;
                newDims[dim] = 1;
               
                Int64[] retArr = ILMemoryPool.Pool.New< Int64>(newLength);
                #region HYCALPER GLOBAL_INIT

               
                Int64 result;
               
                Int64 curval;
                int[] indices = null;
                bool createIndices = false;
                if (!Object.Equals(I, null)) {
                    indices = ILMemoryPool.Pool.New<int>(newLength);
                    createIndices = true;
                }
                #endregion HYCALPER GLOBAL_INIT
                

                // physical -> pointer arithmetic
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( Int64* pOutArr = retArr)
                        fixed ( Int64* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            Int64* lastElement;
                           
                            Int64* tmpOut = pOutArr;
                           
                            Int64* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {

                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                    (
                                        false
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }


                                    
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                            *tmpInd = (int)(tmpIn - (lastElement - leadDimLen));
                                        }
                                        tmpIn++;
                                    }
                                    *(tmpOut++) = ( Int64)result;
                                    tmpInd++;
                                }
                            } else {   // no indices
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        false
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                        {
                                        result = *tmpIn;
                                    }

                                    
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn++;
                                        if (curval > result) {
                                                result = curval;
                                            
                                        }
                                    }

                                    *(tmpOut++) = ( Int64)result;

                                }
                            }
                        }
                    }
                    #endregion physical along 1st leading dimension
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( Int64* pOutArr = retArr)
                        fixed ( Int64* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            Int64* lastElementOut = newLength + pOutArr - 1;
                            int inLength = A.Size.NumberOfElements - 1;
                           
                            Int64* lastElementIn = pInArr + inLength;
                            int inc = A.Size.SequentialIndexDistance(dim);
                           
                            Int64* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            Int64* leadEnd;
                           
                            Int64* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        false
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore                 
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }

                                    
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                            *tmpInd = (int)(leadDimLen - (leadEnd - tmpIn) / inc);
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( Int64)result;

                                    tmpOut += inc;
                                    tmpInd += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                        tmpInd -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            } else {  // no indices
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while
#pragma warning disable
                                    (
                                        false
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                    }

                                    
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( Int64)result;

                                    tmpOut += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            }
                        }
                    }
                    #endregion
                }
                if (createIndices) {
                    I.a = array<int>(indices, newDims);
                }
                return new ILRetArray<Int64>(retArr, newDims);
            }
        }
        /// <summary>
        /// Maximum values along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="I">[Optional] If not null I will hold on return the indices into dim of  
        /// the values found. If I is null those indices will not be computed and I will be ignored.</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array of same inner type and size as A, except for dimension 
        /// 'dim' which will be reduced to length 1.</returns>
        public static  ILRetArray<Int32>  max(ILInArray<Int32> A, ILOutArray<int> I = null, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (A.IsEmpty) {
                    if (!object.Equals(I, null))
                        I.a = empty<int>(ILSize.Empty00);
                    return new  ILRetArray<Int32>(A.Size); 
                }
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) {
                    // scalar or sum over singleton -> return copy
                    if (!object.Equals(I, null))
                        I.a = zeros<int>(A.S);
                    return new ILRetArray<Int32>(A.C.Storage);
                }
                int[] newDims = A.Size.ToIntArray();
                int leadDimLen = A.Size[dim];
                int newLength = A.Size.NumberOfElements / leadDimLen;
                newDims[dim] = 1;
               
                Int32[] retArr = ILMemoryPool.Pool.New< Int32>(newLength);
                #region HYCALPER GLOBAL_INIT

               
                Int32 result;
               
                Int32 curval;
                int[] indices = null;
                bool createIndices = false;
                if (!Object.Equals(I, null)) {
                    indices = ILMemoryPool.Pool.New<int>(newLength);
                    createIndices = true;
                }
                #endregion HYCALPER GLOBAL_INIT
                

                // physical -> pointer arithmetic
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( Int32* pOutArr = retArr)
                        fixed ( Int32* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            Int32* lastElement;
                           
                            Int32* tmpOut = pOutArr;
                           
                            Int32* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {

                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                    (
                                        false
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }


                                    
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                            *tmpInd = (int)(tmpIn - (lastElement - leadDimLen));
                                        }
                                        tmpIn++;
                                    }
                                    *(tmpOut++) = ( Int32)result;
                                    tmpInd++;
                                }
                            } else {   // no indices
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        false
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                        {
                                        result = *tmpIn;
                                    }

                                    
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn++;
                                        if (curval > result) {
                                                result = curval;
                                            
                                        }
                                    }

                                    *(tmpOut++) = ( Int32)result;

                                }
                            }
                        }
                    }
                    #endregion physical along 1st leading dimension
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( Int32* pOutArr = retArr)
                        fixed ( Int32* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            Int32* lastElementOut = newLength + pOutArr - 1;
                            int inLength = A.Size.NumberOfElements - 1;
                           
                            Int32* lastElementIn = pInArr + inLength;
                            int inc = A.Size.SequentialIndexDistance(dim);
                           
                            Int32* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            Int32* leadEnd;
                           
                            Int32* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        false
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore                 
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }

                                    
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                            *tmpInd = (int)(leadDimLen - (leadEnd - tmpIn) / inc);
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( Int32)result;

                                    tmpOut += inc;
                                    tmpInd += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                        tmpInd -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            } else {  // no indices
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while
#pragma warning disable
                                    (
                                        false
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                    }

                                    
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( Int32)result;

                                    tmpOut += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            }
                        }
                    }
                    #endregion
                }
                if (createIndices) {
                    I.a = array<int>(indices, newDims);
                }
                return new ILRetArray<Int32>(retArr, newDims);
            }
        }
        /// <summary>
        /// Maximum values along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="I">[Optional] If not null I will hold on return the indices into dim of  
        /// the values found. If I is null those indices will not be computed and I will be ignored.</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array of same inner type and size as A, except for dimension 
        /// 'dim' which will be reduced to length 1.</returns>
        public static  ILRetLogical  max(ILInArray<byte> A, ILOutArray<int> I = null, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (A.IsEmpty) {
                    if (!object.Equals(I, null))
                        I.a = empty<int>(ILSize.Empty00);
                    return new  ILRetLogical(A.Size); 
                }
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) {
                    // scalar or sum over singleton -> return copy
                    if (!object.Equals(I, null))
                        I.a = zeros<int>(A.S);
                    return new ILRetLogical(A.C.Storage);
                }
                int[] newDims = A.Size.ToIntArray();
                int leadDimLen = A.Size[dim];
                int newLength = A.Size.NumberOfElements / leadDimLen;
                newDims[dim] = 1;
               
                byte[] retArr = ILMemoryPool.Pool.New< byte>(newLength);
                #region HYCALPER GLOBAL_INIT

               
                byte result;
               
                byte curval;
                int[] indices = null;
                bool createIndices = false;
                if (!Object.Equals(I, null)) {
                    indices = ILMemoryPool.Pool.New<int>(newLength);
                    createIndices = true;
                }
                #endregion HYCALPER GLOBAL_INIT
                

                // physical -> pointer arithmetic
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( byte* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            byte* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            byte* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {

                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                    (
                                        false
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }


                                    
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                            *tmpInd = (int)(tmpIn - (lastElement - leadDimLen));
                                        }
                                        tmpIn++;
                                    }
                                    *(tmpOut++) = ( byte)result;
                                    tmpInd++;
                                }
                            } else {   // no indices
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        false
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                        {
                                        result = *tmpIn;
                                    }

                                    
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn++;
                                        if (curval > result) {
                                                result = curval;
                                            
                                        }
                                    }

                                    *(tmpOut++) = ( byte)result;

                                }
                            }
                        }
                    }
                    #endregion physical along 1st leading dimension
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( byte* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            byte* lastElementOut = newLength + pOutArr - 1;
                            int inLength = A.Size.NumberOfElements - 1;
                           
                            byte* lastElementIn = pInArr + inLength;
                            int inc = A.Size.SequentialIndexDistance(dim);
                           
                            byte* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            byte* leadEnd;
                           
                            byte* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        false
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore                 
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }

                                    
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                            *tmpInd = (int)(leadDimLen - (leadEnd - tmpIn) / inc);
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( byte)result;

                                    tmpOut += inc;
                                    tmpInd += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                        tmpInd -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            } else {  // no indices
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while
#pragma warning disable
                                    (
                                        false
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                    }

                                    
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( byte)result;

                                    tmpOut += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            }
                        }
                    }
                    #endregion
                }
                if (createIndices) {
                    I.a = array<int>(indices, newDims);
                }
                return new ILRetLogical(retArr, newDims);
            }
        }
        /// <summary>
        /// Maximum values along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="I">[Optional] If not null I will hold on return the indices into dim of  
        /// the values found. If I is null those indices will not be computed and I will be ignored.</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array of same inner type and size as A, except for dimension 
        /// 'dim' which will be reduced to length 1.</returns>
        public static  ILRetArray<fcomplex>  max(ILInArray<fcomplex> A, ILOutArray<int> I = null, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (A.IsEmpty) {
                    if (!object.Equals(I, null))
                        I.a = empty<int>(ILSize.Empty00);
                    return new  ILRetArray<fcomplex>(A.Size); 
                }
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) {
                    // scalar or sum over singleton -> return copy
                    if (!object.Equals(I, null))
                        I.a = zeros<int>(A.S);
                    return new ILRetArray<fcomplex>(A.C.Storage);
                }
                int[] newDims = A.Size.ToIntArray();
                int leadDimLen = A.Size[dim];
                int newLength = A.Size.NumberOfElements / leadDimLen;
                newDims[dim] = 1;
               
                fcomplex[] retArr = ILMemoryPool.Pool.New< fcomplex>(newLength);
                #region HYCALPER GLOBAL_INIT

               
                fcomplex result;
               
                fcomplex curval;
                int[] indices = null;
                bool createIndices = false;
                if (!Object.Equals(I, null)) {
                    indices = ILMemoryPool.Pool.New<int>(newLength);
                    createIndices = true;
                }
                #endregion HYCALPER GLOBAL_INIT
                float curabsval; float curabsmaxval; 

                // physical -> pointer arithmetic
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( fcomplex* pOutArr = retArr)
                        fixed ( fcomplex* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            fcomplex* lastElement;
                           
                            fcomplex* tmpOut = pOutArr;
                           
                            fcomplex* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {

                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                    (
                                        fcomplex.IsNaN(result)
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }


                                    curabsmaxval = fcomplex.Abs(result);
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn;
                                        curabsval = fcomplex.Abs(curval);
                                            if (curabsval > curabsmaxval) {
                                                curabsmaxval = curabsval;
                                                result = curval;
                                            
                                            *tmpInd = (int)(tmpIn - (lastElement - leadDimLen));
                                        }
                                        tmpIn++;
                                    }
                                    *(tmpOut++) = ( fcomplex)result;
                                    tmpInd++;
                                }
                            } else {   // no indices
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        fcomplex.IsNaN(result)
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                        {
                                        result = *tmpIn;
                                    }

                                    curabsmaxval = fcomplex.Abs(result);
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn++;
                                        curabsval = fcomplex.Abs(curval);
                                            if (curabsval > curabsmaxval) {
                                                curabsmaxval = curabsval;
                                                result = curval;
                                            
                                        }
                                    }

                                    *(tmpOut++) = ( fcomplex)result;

                                }
                            }
                        }
                    }
                    #endregion physical along 1st leading dimension
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( fcomplex* pOutArr = retArr)
                        fixed ( fcomplex* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            fcomplex* lastElementOut = newLength + pOutArr - 1;
                            int inLength = A.Size.NumberOfElements - 1;
                           
                            fcomplex* lastElementIn = pInArr + inLength;
                            int inc = A.Size.SequentialIndexDistance(dim);
                           
                            fcomplex* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            fcomplex* leadEnd;
                           
                            fcomplex* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        fcomplex.IsNaN(result)
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore                 
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }

                                    curabsmaxval = fcomplex.Abs(result);
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        curabsval = fcomplex.Abs(curval);
                                            if (curabsval > curabsmaxval) {
                                                curabsmaxval = curabsval;
                                                result = curval;
                                            
                                            *tmpInd = (int)(leadDimLen - (leadEnd - tmpIn) / inc);
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( fcomplex)result;

                                    tmpOut += inc;
                                    tmpInd += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                        tmpInd -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            } else {  // no indices
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while
#pragma warning disable
                                    (
                                        fcomplex.IsNaN(result)
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                    }

                                    curabsmaxval = fcomplex.Abs(result);
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        curabsval = fcomplex.Abs(curval);
                                            if (curabsval > curabsmaxval) {
                                                curabsmaxval = curabsval;
                                                result = curval;
                                            
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( fcomplex)result;

                                    tmpOut += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            }
                        }
                    }
                    #endregion
                }
                if (createIndices) {
                    I.a = array<int>(indices, newDims);
                }
                return new ILRetArray<fcomplex>(retArr, newDims);
            }
        }
        /// <summary>
        /// Maximum values along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="I">[Optional] If not null I will hold on return the indices into dim of  
        /// the values found. If I is null those indices will not be computed and I will be ignored.</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array of same inner type and size as A, except for dimension 
        /// 'dim' which will be reduced to length 1.</returns>
        public static  ILRetArray<float>  max(ILInArray<float> A, ILOutArray<int> I = null, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (A.IsEmpty) {
                    if (!object.Equals(I, null))
                        I.a = empty<int>(ILSize.Empty00);
                    return new  ILRetArray<float>(A.Size); 
                }
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) {
                    // scalar or sum over singleton -> return copy
                    if (!object.Equals(I, null))
                        I.a = zeros<int>(A.S);
                    return new ILRetArray<float>(A.C.Storage);
                }
                int[] newDims = A.Size.ToIntArray();
                int leadDimLen = A.Size[dim];
                int newLength = A.Size.NumberOfElements / leadDimLen;
                newDims[dim] = 1;
               
                float[] retArr = ILMemoryPool.Pool.New< float>(newLength);
                #region HYCALPER GLOBAL_INIT

               
                float result;
               
                float curval;
                int[] indices = null;
                bool createIndices = false;
                if (!Object.Equals(I, null)) {
                    indices = ILMemoryPool.Pool.New<int>(newLength);
                    createIndices = true;
                }
                #endregion HYCALPER GLOBAL_INIT
                

                // physical -> pointer arithmetic
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( float* pOutArr = retArr)
                        fixed ( float* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            float* lastElement;
                           
                            float* tmpOut = pOutArr;
                           
                            float* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {

                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                    (
                                        float.IsNaN(result)
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }


                                    
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                            *tmpInd = (int)(tmpIn - (lastElement - leadDimLen));
                                        }
                                        tmpIn++;
                                    }
                                    *(tmpOut++) = ( float)result;
                                    tmpInd++;
                                }
                            } else {   // no indices
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        float.IsNaN(result)
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                        {
                                        result = *tmpIn;
                                    }

                                    
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn++;
                                        if (curval > result) {
                                                result = curval;
                                            
                                        }
                                    }

                                    *(tmpOut++) = ( float)result;

                                }
                            }
                        }
                    }
                    #endregion physical along 1st leading dimension
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( float* pOutArr = retArr)
                        fixed ( float* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            float* lastElementOut = newLength + pOutArr - 1;
                            int inLength = A.Size.NumberOfElements - 1;
                           
                            float* lastElementIn = pInArr + inLength;
                            int inc = A.Size.SequentialIndexDistance(dim);
                           
                            float* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            float* leadEnd;
                           
                            float* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        float.IsNaN(result)
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore                 
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }

                                    
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                            *tmpInd = (int)(leadDimLen - (leadEnd - tmpIn) / inc);
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( float)result;

                                    tmpOut += inc;
                                    tmpInd += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                        tmpInd -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            } else {  // no indices
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while
#pragma warning disable
                                    (
                                        float.IsNaN(result)
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                    }

                                    
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        if (curval > result) {
                                                result = curval;
                                            
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( float)result;

                                    tmpOut += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            }
                        }
                    }
                    #endregion
                }
                if (createIndices) {
                    I.a = array<int>(indices, newDims);
                }
                return new ILRetArray<float>(retArr, newDims);
            }
        }
        /// <summary>
        /// Maximum values along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="I">[Optional] If not null I will hold on return the indices into dim of  
        /// the values found. If I is null those indices will not be computed and I will be ignored.</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array of same inner type and size as A, except for dimension 
        /// 'dim' which will be reduced to length 1.</returns>
        public static  ILRetArray<complex>  max(ILInArray<complex> A, ILOutArray<int> I = null, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (A.IsEmpty) {
                    if (!object.Equals(I, null))
                        I.a = empty<int>(ILSize.Empty00);
                    return new  ILRetArray<complex>(A.Size); 
                }
                if (dim >= A.Size.NumberOfDimensions || A.Size[dim] == 1) {
                    // scalar or sum over singleton -> return copy
                    if (!object.Equals(I, null))
                        I.a = zeros<int>(A.S);
                    return new ILRetArray<complex>(A.C.Storage);
                }
                int[] newDims = A.Size.ToIntArray();
                int leadDimLen = A.Size[dim];
                int newLength = A.Size.NumberOfElements / leadDimLen;
                newDims[dim] = 1;
               
                complex[] retArr = ILMemoryPool.Pool.New< complex>(newLength);
                #region HYCALPER GLOBAL_INIT

               
                complex result;
               
                complex curval;
                int[] indices = null;
                bool createIndices = false;
                if (!Object.Equals(I, null)) {
                    indices = ILMemoryPool.Pool.New<int>(newLength);
                    createIndices = true;
                }
                #endregion HYCALPER GLOBAL_INIT
                double curabsval; double curabsmaxval; 

                // physical -> pointer arithmetic
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( complex* pOutArr = retArr)
                        fixed ( complex* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            complex* lastElement;
                           
                            complex* tmpOut = pOutArr;
                           
                            complex* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {

                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                    (
                                        complex.IsNaN(result)
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }


                                    curabsmaxval = complex.Abs(result);
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn;
                                        curabsval = complex.Abs(curval);
                                            if (curabsval > curabsmaxval) {
                                                curabsmaxval = curabsval;
                                                result = curval;
                                            
                                            *tmpInd = (int)(tmpIn - (lastElement - leadDimLen));
                                        }
                                        tmpIn++;
                                    }
                                    *(tmpOut++) = ( complex)result;
                                    tmpInd++;
                                }
                            } else {   // no indices
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    lastElement = tmpIn + leadDimLen;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        complex.IsNaN(result)
                                        && ++tmpIn < lastElement) 
#pragma warning restore
                                        {
                                        result = *tmpIn;
                                    }

                                    curabsmaxval = complex.Abs(result);
                                    while (tmpIn < lastElement) {
                                        curval = *tmpIn++;
                                        curabsval = complex.Abs(curval);
                                            if (curabsval > curabsmaxval) {
                                                curabsmaxval = curabsval;
                                                result = curval;
                                            
                                        }
                                    }

                                    *(tmpOut++) = ( complex)result;

                                }
                            }
                        }
                    }
                    #endregion physical along 1st leading dimension
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( complex* pOutArr = retArr)
                        fixed ( complex* pInArr = A.GetArrayForRead())
                        fixed (int* pIndices = indices) {
                           
                            complex* lastElementOut = newLength + pOutArr - 1;
                            int inLength = A.Size.NumberOfElements - 1;
                           
                            complex* lastElementIn = pInArr + inLength;
                            int inc = A.Size.SequentialIndexDistance(dim);
                           
                            complex* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            complex* leadEnd;
                           
                            complex* tmpIn = pInArr;
                            if (createIndices) {
                                int* tmpInd = pIndices;
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while 
#pragma warning disable
                                        (
                                        complex.IsNaN(result)
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore                 
                                    {
                                        result = *tmpIn;
                                        *tmpInd += 1;
                                    }

                                    curabsmaxval = complex.Abs(result);
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        curabsval = complex.Abs(curval);
                                            if (curabsval > curabsmaxval) {
                                                curabsmaxval = curabsval;
                                                result = curval;
                                            
                                            *tmpInd = (int)(leadDimLen - (leadEnd - tmpIn) / inc);
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( complex)result;

                                    tmpOut += inc;
                                    tmpInd += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                        tmpInd -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            } else {  // no indices
                                for (int h = newLength; h-- > 0; ) {
                                    leadEnd = tmpIn + leadDimLen * inc;

                                    result = *tmpIn;
                                    while
#pragma warning disable
                                    (
                                        complex.IsNaN(result)
                                        && (tmpIn += inc) < leadEnd)
#pragma warning restore
                                    {
                                        result = *tmpIn;
                                    }

                                    curabsmaxval = complex.Abs(result);
                                    while (tmpIn < leadEnd) {
                                        curval = *tmpIn;
                                        curabsval = complex.Abs(curval);
                                            if (curabsval > curabsmaxval) {
                                                curabsmaxval = curabsval;
                                                result = curval;
                                            
                                        }
                                        tmpIn += inc;
                                    }

                                    *(tmpOut) = ( complex)result;

                                    tmpOut += inc;
                                    if (tmpOut > lastElementOut) {
                                        tmpOut -= outLength;
                                    }
                                    if (tmpIn > lastElementIn)
                                        tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                                }
                            }
                        }
                    }
                    #endregion
                }
                if (createIndices) {
                    I.a = array<int>(indices, newDims);
                }
                return new ILRetArray<complex>(retArr, newDims);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE


        
        /// <summary>Maximum of A and B elementwise</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with the maximum elements of A and B</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<double>  max(ILInArray<double> A, ILInArray<double> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                 double[] retArr;
                 double[] arrA = A.GetArrayForRead();
                 double[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {
                        return array<double>(new double[1] { (A.GetValue(0) > B.GetValue(0)) ? A.GetValue(0) : B.GetValue(0) });
                    } else if (B.IsEmpty) {
                        return ILRetArray<double>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< double>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<double>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<double>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  maxEx(A, B);
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIB;
                        else {
                            retArr = ILMemoryPool.Pool.New<double>(outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<double> retStorage = new ILDenseStorage<double>(retArr, outDims);
                int i = 0, workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    
                    double* cp = (double*)range.Item5 + range.Item1;
                    
                    double scalar;
                    int j = range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                            
                            double* bp = ((double*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (cp[0]  > bp[0]) ? cp[0] : bp[0];
                                cp[1] = (cp[1]  > bp[1]) ? cp[1] : bp[1];
                                cp[2] = (cp[2]  > bp[2]) ? cp[2] : bp[2];
                                cp[3] = (cp[3]  > bp[3]) ? cp[3] : bp[3];
                                cp[4] = (cp[4]  > bp[4]) ? cp[4] : bp[4];
                                cp[5] = (cp[5]  > bp[5]) ? cp[5] : bp[5];
                                cp[6] = (cp[6]  > bp[6]) ? cp[6] : bp[6];
                                cp[7] = (cp[7]  > bp[7]) ? cp[7] : bp[7];
                                cp += 8; bp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > *bp) ? *cp : *bp;
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                            
                            double* ap = ((double*)range.Item3 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > cp[0]) ? ap[0] : cp[0];
                                cp[1] = (ap[1]  > cp[1]) ? ap[1] : cp[1];
                                cp[2] = (ap[2]  > cp[2]) ? ap[2] : cp[2];
                                cp[3] = (ap[3]  > cp[3]) ? ap[3] : cp[3];
                                cp[4] = (ap[4]  > cp[4]) ? ap[4] : cp[4];
                                cp[5] = (ap[5]  > cp[5]) ? ap[5] : cp[5];
                                cp[6] = (ap[6]  > cp[6]) ? ap[6] : cp[6];
                                cp[7] = (ap[7]  > cp[7]) ? ap[7] : cp[7];
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *cp) ? *ap : *cp;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((double*)range.Item3 + range.Item1);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > bp[0]) ? ap[0] : bp[0];
                                cp[1] = (ap[1]  > bp[1]) ? ap[1] : bp[1];
                                cp[2] = (ap[2]  > bp[2]) ? ap[2] : bp[2];
                                cp[3] = (ap[3]  > bp[3]) ? ap[3] : bp[3];
                                cp[4] = (ap[4]  > bp[4]) ? ap[4] : bp[4];
                                cp[5] = (ap[5]  > bp[5]) ? ap[5] : bp[5];
                                cp[6] = (ap[6]  > bp[6]) ? ap[6] : bp[6];
                                cp[7] = (ap[7]  > bp[7]) ? ap[7] : bp[7];
                                ap += 8; bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *bp) ? *ap : *bp;
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((double*)range.Item4);
                            while (j > 7) {
                                cp[0] = (cp[0]  > scalar) ? cp[0] : scalar;
                                cp[1] = (cp[1]  > scalar) ? cp[1] : scalar;
                                cp[2] = (cp[2]  > scalar) ? cp[2] : scalar;
                                cp[3] = (cp[3]  > scalar) ? cp[3] : scalar;
                                cp[4] = (cp[4]  > scalar) ? cp[4] : scalar;
                                cp[5] = (cp[5]  > scalar) ? cp[5] : scalar;
                                cp[6] = (cp[6]  > scalar) ? cp[6] : scalar;
                                cp[7] = (cp[7]  > scalar) ? cp[7] : scalar;
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > scalar) ? *cp : scalar;
                                cp++;
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((double*)range.Item3 + range.Item1);
                            scalar = *((double*)range.Item4);
                            while (j > 7) {
                                cp[0] = (ap[0]  > scalar) ? ap[0] : scalar;
                                cp[1] = (ap[1]  > scalar) ? ap[1] : scalar;
                                cp[2] = (ap[2]  > scalar) ? ap[2] : scalar;
                                cp[3] = (ap[3]  > scalar) ? ap[3] : scalar;
                                cp[4] = (ap[4]  > scalar) ? ap[4] : scalar;
                                cp[5] = (ap[5]  > scalar) ? ap[5] : scalar;
                                cp[6] = (ap[6]  > scalar) ? ap[6] : scalar;
                                cp[7] = (ap[7]  > scalar) ? ap[7] : scalar;
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > scalar) ? *ap : scalar;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((double*)range.Item3);
                            while (j > 7) {
                                cp[0] = (scalar  > cp[0]) ? scalar : cp[0];
                                cp[1] = (scalar  > cp[1]) ? scalar : cp[1];
                                cp[2] = (scalar  > cp[2]) ? scalar : cp[2];
                                cp[3] = (scalar  > cp[3]) ? scalar : cp[3];
                                cp[4] = (scalar  > cp[4]) ? scalar : cp[4];
                                cp[5] = (scalar  > cp[5]) ? scalar : cp[5];
                                cp[6] = (scalar  > cp[6]) ? scalar : cp[6];
                                cp[7] = (scalar  > cp[7]) ? scalar : cp[7];
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *cp) ? scalar : *cp;
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((double*)range.Item3);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (scalar  > bp[0]) ? scalar : bp[0];
                                cp[1] = (scalar  > bp[1]) ? scalar : bp[1];
                                cp[2] = (scalar  > bp[2]) ? scalar : bp[2];
                                cp[3] = (scalar  > bp[3]) ? scalar : bp[3];
                                cp[4] = (scalar  > bp[4]) ? scalar : bp[4];
                                cp[5] = (scalar  > bp[5]) ? scalar : bp[5];
                                cp[6] = (scalar  > bp[6]) ? scalar : bp[6];
                                cp[7] = (scalar  > bp[7]) ? scalar : bp[7];
                                bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *bp) ? scalar : *bp;
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
                };

                #region do the work
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }

                fixed (double* arrAP = arrA)
                fixed (double* arrBP = arrB)
                fixed (double* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }

                #endregion
                return new ILRetArray<double>(retStorage);
            }
        }

        private static unsafe ILRetArray<double>  maxEx(ILInArray<double> A, ILInArray<double> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<double>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<double>(B.S);
                } else if (B.IsEmpty) {
                    return empty<double>(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation

                
                double[] retArr;

                
                double[] arrA = A.GetArrayForRead();

                
                double[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<double>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<double>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILDenseStorage<double> retStorage = new ILDenseStorage<double>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;

                    
                    double* ap;

                    
                    double* bp;

                    
                    double* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (double*)range.Item3;
                                bp = (double*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (double*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (double*)range.Item3;
                                cp = (double*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *cp) ? *ap : *cp;
                                    ap++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (double*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (double*)range.Item4;
                                cp = (double*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            for (int s = 0; s < range.Item2; s++) {
                                bp = (double*)range.Item4;
                                cp = (double*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*cp  > *bp) ? *cp : *bp;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed (double* arrAP = arrA)
                fixed (double* arrBP = arrB)
                fixed (double* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<double>(retStorage);
            }
        }



#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>Maximum of A and B elementwise</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with the maximum elements of A and B</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<Int64>  max(ILInArray<Int64> A, ILInArray<Int64> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                Int64[] retArr;
                Int64[] arrA = A.GetArrayForRead();
                Int64[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {
                        return array<Int64>(new Int64[1] { (A.GetValue(0) > B.GetValue(0)) ? A.GetValue(0) : B.GetValue(0) });
                    } else if (B.IsEmpty) {
                        return ILRetArray<Int64>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< Int64>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<Int64>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<Int64>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  maxEx(A, B);
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIB;
                        else {
                            retArr = ILMemoryPool.Pool.New<Int64>(outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<Int64> retStorage = new ILDenseStorage<Int64>(retArr, outDims);
                int i = 0, workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                   
                    Int64* cp = (Int64*)range.Item5 + range.Item1;
                   
                    Int64 scalar;
                    int j = range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            Int64* bp = ((Int64*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (cp[0]  > bp[0]) ? cp[0] : bp[0];
                                cp[1] = (cp[1]  > bp[1]) ? cp[1] : bp[1];
                                cp[2] = (cp[2]  > bp[2]) ? cp[2] : bp[2];
                                cp[3] = (cp[3]  > bp[3]) ? cp[3] : bp[3];
                                cp[4] = (cp[4]  > bp[4]) ? cp[4] : bp[4];
                                cp[5] = (cp[5]  > bp[5]) ? cp[5] : bp[5];
                                cp[6] = (cp[6]  > bp[6]) ? cp[6] : bp[6];
                                cp[7] = (cp[7]  > bp[7]) ? cp[7] : bp[7];
                                cp += 8; bp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > *bp) ? *cp : *bp;
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            Int64* ap = ((Int64*)range.Item3 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > cp[0]) ? ap[0] : cp[0];
                                cp[1] = (ap[1]  > cp[1]) ? ap[1] : cp[1];
                                cp[2] = (ap[2]  > cp[2]) ? ap[2] : cp[2];
                                cp[3] = (ap[3]  > cp[3]) ? ap[3] : cp[3];
                                cp[4] = (ap[4]  > cp[4]) ? ap[4] : cp[4];
                                cp[5] = (ap[5]  > cp[5]) ? ap[5] : cp[5];
                                cp[6] = (ap[6]  > cp[6]) ? ap[6] : cp[6];
                                cp[7] = (ap[7]  > cp[7]) ? ap[7] : cp[7];
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *cp) ? *ap : *cp;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > bp[0]) ? ap[0] : bp[0];
                                cp[1] = (ap[1]  > bp[1]) ? ap[1] : bp[1];
                                cp[2] = (ap[2]  > bp[2]) ? ap[2] : bp[2];
                                cp[3] = (ap[3]  > bp[3]) ? ap[3] : bp[3];
                                cp[4] = (ap[4]  > bp[4]) ? ap[4] : bp[4];
                                cp[5] = (ap[5]  > bp[5]) ? ap[5] : bp[5];
                                cp[6] = (ap[6]  > bp[6]) ? ap[6] : bp[6];
                                cp[7] = (ap[7]  > bp[7]) ? ap[7] : bp[7];
                                ap += 8; bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *bp) ? *ap : *bp;
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((Int64*)range.Item4);
                            while (j > 7) {
                                cp[0] = (cp[0]  > scalar) ? cp[0] : scalar;
                                cp[1] = (cp[1]  > scalar) ? cp[1] : scalar;
                                cp[2] = (cp[2]  > scalar) ? cp[2] : scalar;
                                cp[3] = (cp[3]  > scalar) ? cp[3] : scalar;
                                cp[4] = (cp[4]  > scalar) ? cp[4] : scalar;
                                cp[5] = (cp[5]  > scalar) ? cp[5] : scalar;
                                cp[6] = (cp[6]  > scalar) ? cp[6] : scalar;
                                cp[7] = (cp[7]  > scalar) ? cp[7] : scalar;
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > scalar) ? *cp : scalar;
                                cp++;
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            scalar = *((Int64*)range.Item4);
                            while (j > 7) {
                                cp[0] = (ap[0]  > scalar) ? ap[0] : scalar;
                                cp[1] = (ap[1]  > scalar) ? ap[1] : scalar;
                                cp[2] = (ap[2]  > scalar) ? ap[2] : scalar;
                                cp[3] = (ap[3]  > scalar) ? ap[3] : scalar;
                                cp[4] = (ap[4]  > scalar) ? ap[4] : scalar;
                                cp[5] = (ap[5]  > scalar) ? ap[5] : scalar;
                                cp[6] = (ap[6]  > scalar) ? ap[6] : scalar;
                                cp[7] = (ap[7]  > scalar) ? ap[7] : scalar;
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > scalar) ? *ap : scalar;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((Int64*)range.Item3);
                            while (j > 7) {
                                cp[0] = (scalar  > cp[0]) ? scalar : cp[0];
                                cp[1] = (scalar  > cp[1]) ? scalar : cp[1];
                                cp[2] = (scalar  > cp[2]) ? scalar : cp[2];
                                cp[3] = (scalar  > cp[3]) ? scalar : cp[3];
                                cp[4] = (scalar  > cp[4]) ? scalar : cp[4];
                                cp[5] = (scalar  > cp[5]) ? scalar : cp[5];
                                cp[6] = (scalar  > cp[6]) ? scalar : cp[6];
                                cp[7] = (scalar  > cp[7]) ? scalar : cp[7];
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *cp) ? scalar : *cp;
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int64*)range.Item3);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (scalar  > bp[0]) ? scalar : bp[0];
                                cp[1] = (scalar  > bp[1]) ? scalar : bp[1];
                                cp[2] = (scalar  > bp[2]) ? scalar : bp[2];
                                cp[3] = (scalar  > bp[3]) ? scalar : bp[3];
                                cp[4] = (scalar  > bp[4]) ? scalar : bp[4];
                                cp[5] = (scalar  > bp[5]) ? scalar : bp[5];
                                cp[6] = (scalar  > bp[6]) ? scalar : bp[6];
                                cp[7] = (scalar  > bp[7]) ? scalar : bp[7];
                                bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *bp) ? scalar : *bp;
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
                };

                #region do the work
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }

                fixed (Int64* arrAP = arrA)
                fixed (Int64* arrBP = arrB)
                fixed (Int64* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }

                #endregion
                return new ILRetArray<Int64>(retStorage);
            }
        }

        private static unsafe ILRetArray<Int64>  maxEx(ILInArray<Int64> A, ILInArray<Int64> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<Int64>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<Int64>(B.S);
                } else if (B.IsEmpty) {
                    return empty<Int64>(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation

               
                Int64[] retArr;

               
                Int64[] arrA = A.GetArrayForRead();

               
                Int64[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<Int64>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<Int64>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILDenseStorage<Int64> retStorage = new ILDenseStorage<Int64>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;

                   
                    Int64* ap;

                   
                    Int64* bp;

                   
                    Int64* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int64*)range.Item3;
                                bp = (Int64*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (Int64*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int64*)range.Item3;
                                cp = (Int64*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *cp) ? *ap : *cp;
                                    ap++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int64*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (Int64*)range.Item4;
                                cp = (Int64*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            for (int s = 0; s < range.Item2; s++) {
                                bp = (Int64*)range.Item4;
                                cp = (Int64*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*cp  > *bp) ? *cp : *bp;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed (Int64* arrAP = arrA)
                fixed (Int64* arrBP = arrB)
                fixed (Int64* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<Int64>(retStorage);
            }
        }


       
        /// <summary>Maximum of A and B elementwise</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with the maximum elements of A and B</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<Int32>  max(ILInArray<Int32> A, ILInArray<Int32> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                Int32[] retArr;
                Int32[] arrA = A.GetArrayForRead();
                Int32[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {
                        return array<Int32>(new Int32[1] { (A.GetValue(0) > B.GetValue(0)) ? A.GetValue(0) : B.GetValue(0) });
                    } else if (B.IsEmpty) {
                        return ILRetArray<Int32>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< Int32>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<Int32>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<Int32>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  maxEx(A, B);
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIB;
                        else {
                            retArr = ILMemoryPool.Pool.New<Int32>(outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<Int32> retStorage = new ILDenseStorage<Int32>(retArr, outDims);
                int i = 0, workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                   
                    Int32* cp = (Int32*)range.Item5 + range.Item1;
                   
                    Int32 scalar;
                    int j = range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            Int32* bp = ((Int32*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (cp[0]  > bp[0]) ? cp[0] : bp[0];
                                cp[1] = (cp[1]  > bp[1]) ? cp[1] : bp[1];
                                cp[2] = (cp[2]  > bp[2]) ? cp[2] : bp[2];
                                cp[3] = (cp[3]  > bp[3]) ? cp[3] : bp[3];
                                cp[4] = (cp[4]  > bp[4]) ? cp[4] : bp[4];
                                cp[5] = (cp[5]  > bp[5]) ? cp[5] : bp[5];
                                cp[6] = (cp[6]  > bp[6]) ? cp[6] : bp[6];
                                cp[7] = (cp[7]  > bp[7]) ? cp[7] : bp[7];
                                cp += 8; bp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > *bp) ? *cp : *bp;
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            Int32* ap = ((Int32*)range.Item3 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > cp[0]) ? ap[0] : cp[0];
                                cp[1] = (ap[1]  > cp[1]) ? ap[1] : cp[1];
                                cp[2] = (ap[2]  > cp[2]) ? ap[2] : cp[2];
                                cp[3] = (ap[3]  > cp[3]) ? ap[3] : cp[3];
                                cp[4] = (ap[4]  > cp[4]) ? ap[4] : cp[4];
                                cp[5] = (ap[5]  > cp[5]) ? ap[5] : cp[5];
                                cp[6] = (ap[6]  > cp[6]) ? ap[6] : cp[6];
                                cp[7] = (ap[7]  > cp[7]) ? ap[7] : cp[7];
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *cp) ? *ap : *cp;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > bp[0]) ? ap[0] : bp[0];
                                cp[1] = (ap[1]  > bp[1]) ? ap[1] : bp[1];
                                cp[2] = (ap[2]  > bp[2]) ? ap[2] : bp[2];
                                cp[3] = (ap[3]  > bp[3]) ? ap[3] : bp[3];
                                cp[4] = (ap[4]  > bp[4]) ? ap[4] : bp[4];
                                cp[5] = (ap[5]  > bp[5]) ? ap[5] : bp[5];
                                cp[6] = (ap[6]  > bp[6]) ? ap[6] : bp[6];
                                cp[7] = (ap[7]  > bp[7]) ? ap[7] : bp[7];
                                ap += 8; bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *bp) ? *ap : *bp;
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((Int32*)range.Item4);
                            while (j > 7) {
                                cp[0] = (cp[0]  > scalar) ? cp[0] : scalar;
                                cp[1] = (cp[1]  > scalar) ? cp[1] : scalar;
                                cp[2] = (cp[2]  > scalar) ? cp[2] : scalar;
                                cp[3] = (cp[3]  > scalar) ? cp[3] : scalar;
                                cp[4] = (cp[4]  > scalar) ? cp[4] : scalar;
                                cp[5] = (cp[5]  > scalar) ? cp[5] : scalar;
                                cp[6] = (cp[6]  > scalar) ? cp[6] : scalar;
                                cp[7] = (cp[7]  > scalar) ? cp[7] : scalar;
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > scalar) ? *cp : scalar;
                                cp++;
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            scalar = *((Int32*)range.Item4);
                            while (j > 7) {
                                cp[0] = (ap[0]  > scalar) ? ap[0] : scalar;
                                cp[1] = (ap[1]  > scalar) ? ap[1] : scalar;
                                cp[2] = (ap[2]  > scalar) ? ap[2] : scalar;
                                cp[3] = (ap[3]  > scalar) ? ap[3] : scalar;
                                cp[4] = (ap[4]  > scalar) ? ap[4] : scalar;
                                cp[5] = (ap[5]  > scalar) ? ap[5] : scalar;
                                cp[6] = (ap[6]  > scalar) ? ap[6] : scalar;
                                cp[7] = (ap[7]  > scalar) ? ap[7] : scalar;
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > scalar) ? *ap : scalar;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((Int32*)range.Item3);
                            while (j > 7) {
                                cp[0] = (scalar  > cp[0]) ? scalar : cp[0];
                                cp[1] = (scalar  > cp[1]) ? scalar : cp[1];
                                cp[2] = (scalar  > cp[2]) ? scalar : cp[2];
                                cp[3] = (scalar  > cp[3]) ? scalar : cp[3];
                                cp[4] = (scalar  > cp[4]) ? scalar : cp[4];
                                cp[5] = (scalar  > cp[5]) ? scalar : cp[5];
                                cp[6] = (scalar  > cp[6]) ? scalar : cp[6];
                                cp[7] = (scalar  > cp[7]) ? scalar : cp[7];
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *cp) ? scalar : *cp;
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int32*)range.Item3);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (scalar  > bp[0]) ? scalar : bp[0];
                                cp[1] = (scalar  > bp[1]) ? scalar : bp[1];
                                cp[2] = (scalar  > bp[2]) ? scalar : bp[2];
                                cp[3] = (scalar  > bp[3]) ? scalar : bp[3];
                                cp[4] = (scalar  > bp[4]) ? scalar : bp[4];
                                cp[5] = (scalar  > bp[5]) ? scalar : bp[5];
                                cp[6] = (scalar  > bp[6]) ? scalar : bp[6];
                                cp[7] = (scalar  > bp[7]) ? scalar : bp[7];
                                bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *bp) ? scalar : *bp;
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
                };

                #region do the work
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }

                fixed (Int32* arrAP = arrA)
                fixed (Int32* arrBP = arrB)
                fixed (Int32* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }

                #endregion
                return new ILRetArray<Int32>(retStorage);
            }
        }

        private static unsafe ILRetArray<Int32>  maxEx(ILInArray<Int32> A, ILInArray<Int32> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<Int32>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<Int32>(B.S);
                } else if (B.IsEmpty) {
                    return empty<Int32>(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation

               
                Int32[] retArr;

               
                Int32[] arrA = A.GetArrayForRead();

               
                Int32[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<Int32>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<Int32>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILDenseStorage<Int32> retStorage = new ILDenseStorage<Int32>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;

                   
                    Int32* ap;

                   
                    Int32* bp;

                   
                    Int32* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int32*)range.Item3;
                                bp = (Int32*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (Int32*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int32*)range.Item3;
                                cp = (Int32*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *cp) ? *ap : *cp;
                                    ap++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int32*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (Int32*)range.Item4;
                                cp = (Int32*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            for (int s = 0; s < range.Item2; s++) {
                                bp = (Int32*)range.Item4;
                                cp = (Int32*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*cp  > *bp) ? *cp : *bp;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed (Int32* arrAP = arrA)
                fixed (Int32* arrBP = arrB)
                fixed (Int32* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<Int32>(retStorage);
            }
        }


       
        /// <summary>Maximum of A and B elementwise</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with the maximum elements of A and B</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<float>  max(ILInArray<float> A, ILInArray<float> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                float[] retArr;
                float[] arrA = A.GetArrayForRead();
                float[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {
                        return array<float>(new float[1] { (A.GetValue(0) > B.GetValue(0)) ? A.GetValue(0) : B.GetValue(0) });
                    } else if (B.IsEmpty) {
                        return ILRetArray<float>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< float>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<float>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<float>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  maxEx(A, B);
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIB;
                        else {
                            retArr = ILMemoryPool.Pool.New<float>(outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<float> retStorage = new ILDenseStorage<float>(retArr, outDims);
                int i = 0, workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                   
                    float* cp = (float*)range.Item5 + range.Item1;
                   
                    float scalar;
                    int j = range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            float* bp = ((float*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (cp[0]  > bp[0]) ? cp[0] : bp[0];
                                cp[1] = (cp[1]  > bp[1]) ? cp[1] : bp[1];
                                cp[2] = (cp[2]  > bp[2]) ? cp[2] : bp[2];
                                cp[3] = (cp[3]  > bp[3]) ? cp[3] : bp[3];
                                cp[4] = (cp[4]  > bp[4]) ? cp[4] : bp[4];
                                cp[5] = (cp[5]  > bp[5]) ? cp[5] : bp[5];
                                cp[6] = (cp[6]  > bp[6]) ? cp[6] : bp[6];
                                cp[7] = (cp[7]  > bp[7]) ? cp[7] : bp[7];
                                cp += 8; bp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > *bp) ? *cp : *bp;
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            float* ap = ((float*)range.Item3 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > cp[0]) ? ap[0] : cp[0];
                                cp[1] = (ap[1]  > cp[1]) ? ap[1] : cp[1];
                                cp[2] = (ap[2]  > cp[2]) ? ap[2] : cp[2];
                                cp[3] = (ap[3]  > cp[3]) ? ap[3] : cp[3];
                                cp[4] = (ap[4]  > cp[4]) ? ap[4] : cp[4];
                                cp[5] = (ap[5]  > cp[5]) ? ap[5] : cp[5];
                                cp[6] = (ap[6]  > cp[6]) ? ap[6] : cp[6];
                                cp[7] = (ap[7]  > cp[7]) ? ap[7] : cp[7];
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *cp) ? *ap : *cp;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((float*)range.Item3 + range.Item1);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > bp[0]) ? ap[0] : bp[0];
                                cp[1] = (ap[1]  > bp[1]) ? ap[1] : bp[1];
                                cp[2] = (ap[2]  > bp[2]) ? ap[2] : bp[2];
                                cp[3] = (ap[3]  > bp[3]) ? ap[3] : bp[3];
                                cp[4] = (ap[4]  > bp[4]) ? ap[4] : bp[4];
                                cp[5] = (ap[5]  > bp[5]) ? ap[5] : bp[5];
                                cp[6] = (ap[6]  > bp[6]) ? ap[6] : bp[6];
                                cp[7] = (ap[7]  > bp[7]) ? ap[7] : bp[7];
                                ap += 8; bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *bp) ? *ap : *bp;
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((float*)range.Item4);
                            while (j > 7) {
                                cp[0] = (cp[0]  > scalar) ? cp[0] : scalar;
                                cp[1] = (cp[1]  > scalar) ? cp[1] : scalar;
                                cp[2] = (cp[2]  > scalar) ? cp[2] : scalar;
                                cp[3] = (cp[3]  > scalar) ? cp[3] : scalar;
                                cp[4] = (cp[4]  > scalar) ? cp[4] : scalar;
                                cp[5] = (cp[5]  > scalar) ? cp[5] : scalar;
                                cp[6] = (cp[6]  > scalar) ? cp[6] : scalar;
                                cp[7] = (cp[7]  > scalar) ? cp[7] : scalar;
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > scalar) ? *cp : scalar;
                                cp++;
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((float*)range.Item3 + range.Item1);
                            scalar = *((float*)range.Item4);
                            while (j > 7) {
                                cp[0] = (ap[0]  > scalar) ? ap[0] : scalar;
                                cp[1] = (ap[1]  > scalar) ? ap[1] : scalar;
                                cp[2] = (ap[2]  > scalar) ? ap[2] : scalar;
                                cp[3] = (ap[3]  > scalar) ? ap[3] : scalar;
                                cp[4] = (ap[4]  > scalar) ? ap[4] : scalar;
                                cp[5] = (ap[5]  > scalar) ? ap[5] : scalar;
                                cp[6] = (ap[6]  > scalar) ? ap[6] : scalar;
                                cp[7] = (ap[7]  > scalar) ? ap[7] : scalar;
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > scalar) ? *ap : scalar;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((float*)range.Item3);
                            while (j > 7) {
                                cp[0] = (scalar  > cp[0]) ? scalar : cp[0];
                                cp[1] = (scalar  > cp[1]) ? scalar : cp[1];
                                cp[2] = (scalar  > cp[2]) ? scalar : cp[2];
                                cp[3] = (scalar  > cp[3]) ? scalar : cp[3];
                                cp[4] = (scalar  > cp[4]) ? scalar : cp[4];
                                cp[5] = (scalar  > cp[5]) ? scalar : cp[5];
                                cp[6] = (scalar  > cp[6]) ? scalar : cp[6];
                                cp[7] = (scalar  > cp[7]) ? scalar : cp[7];
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *cp) ? scalar : *cp;
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((float*)range.Item3);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (scalar  > bp[0]) ? scalar : bp[0];
                                cp[1] = (scalar  > bp[1]) ? scalar : bp[1];
                                cp[2] = (scalar  > bp[2]) ? scalar : bp[2];
                                cp[3] = (scalar  > bp[3]) ? scalar : bp[3];
                                cp[4] = (scalar  > bp[4]) ? scalar : bp[4];
                                cp[5] = (scalar  > bp[5]) ? scalar : bp[5];
                                cp[6] = (scalar  > bp[6]) ? scalar : bp[6];
                                cp[7] = (scalar  > bp[7]) ? scalar : bp[7];
                                bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *bp) ? scalar : *bp;
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
                };

                #region do the work
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }

                fixed (float* arrAP = arrA)
                fixed (float* arrBP = arrB)
                fixed (float* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }

                #endregion
                return new ILRetArray<float>(retStorage);
            }
        }

        private static unsafe ILRetArray<float>  maxEx(ILInArray<float> A, ILInArray<float> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<float>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<float>(B.S);
                } else if (B.IsEmpty) {
                    return empty<float>(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation

               
                float[] retArr;

               
                float[] arrA = A.GetArrayForRead();

               
                float[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<float>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<float>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILDenseStorage<float> retStorage = new ILDenseStorage<float>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;

                   
                    float* ap;

                   
                    float* bp;

                   
                    float* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (float*)range.Item3;
                                bp = (float*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (float*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (float*)range.Item3;
                                cp = (float*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *cp) ? *ap : *cp;
                                    ap++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (float*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (float*)range.Item4;
                                cp = (float*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            for (int s = 0; s < range.Item2; s++) {
                                bp = (float*)range.Item4;
                                cp = (float*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*cp  > *bp) ? *cp : *bp;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed (float* arrAP = arrA)
                fixed (float* arrBP = arrB)
                fixed (float* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<float>(retStorage);
            }
        }


       
        /// <summary>Maximum of A and B elementwise</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with the maximum elements of A and B</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<fcomplex>  max(ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                fcomplex[] retArr;
                fcomplex[] arrA = A.GetArrayForRead();
                fcomplex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {
                        return array<fcomplex>(new fcomplex[1] { (A.GetValue(0) > B.GetValue(0)) ? A.GetValue(0) : B.GetValue(0) });
                    } else if (B.IsEmpty) {
                        return ILRetArray<fcomplex>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< fcomplex>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<fcomplex>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<fcomplex>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  maxEx(A, B);
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIB;
                        else {
                            retArr = ILMemoryPool.Pool.New<fcomplex>(outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<fcomplex> retStorage = new ILDenseStorage<fcomplex>(retArr, outDims);
                int i = 0, workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                   
                    fcomplex* cp = (fcomplex*)range.Item5 + range.Item1;
                   
                    fcomplex scalar;
                    int j = range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            fcomplex* bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (cp[0]  > bp[0]) ? cp[0] : bp[0];
                                cp[1] = (cp[1]  > bp[1]) ? cp[1] : bp[1];
                                cp[2] = (cp[2]  > bp[2]) ? cp[2] : bp[2];
                                cp[3] = (cp[3]  > bp[3]) ? cp[3] : bp[3];
                                cp[4] = (cp[4]  > bp[4]) ? cp[4] : bp[4];
                                cp[5] = (cp[5]  > bp[5]) ? cp[5] : bp[5];
                                cp[6] = (cp[6]  > bp[6]) ? cp[6] : bp[6];
                                cp[7] = (cp[7]  > bp[7]) ? cp[7] : bp[7];
                                cp += 8; bp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > *bp) ? *cp : *bp;
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > cp[0]) ? ap[0] : cp[0];
                                cp[1] = (ap[1]  > cp[1]) ? ap[1] : cp[1];
                                cp[2] = (ap[2]  > cp[2]) ? ap[2] : cp[2];
                                cp[3] = (ap[3]  > cp[3]) ? ap[3] : cp[3];
                                cp[4] = (ap[4]  > cp[4]) ? ap[4] : cp[4];
                                cp[5] = (ap[5]  > cp[5]) ? ap[5] : cp[5];
                                cp[6] = (ap[6]  > cp[6]) ? ap[6] : cp[6];
                                cp[7] = (ap[7]  > cp[7]) ? ap[7] : cp[7];
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *cp) ? *ap : *cp;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((fcomplex*)range.Item3 + range.Item1);
                            bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > bp[0]) ? ap[0] : bp[0];
                                cp[1] = (ap[1]  > bp[1]) ? ap[1] : bp[1];
                                cp[2] = (ap[2]  > bp[2]) ? ap[2] : bp[2];
                                cp[3] = (ap[3]  > bp[3]) ? ap[3] : bp[3];
                                cp[4] = (ap[4]  > bp[4]) ? ap[4] : bp[4];
                                cp[5] = (ap[5]  > bp[5]) ? ap[5] : bp[5];
                                cp[6] = (ap[6]  > bp[6]) ? ap[6] : bp[6];
                                cp[7] = (ap[7]  > bp[7]) ? ap[7] : bp[7];
                                ap += 8; bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *bp) ? *ap : *bp;
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((fcomplex*)range.Item4);
                            while (j > 7) {
                                cp[0] = (cp[0]  > scalar) ? cp[0] : scalar;
                                cp[1] = (cp[1]  > scalar) ? cp[1] : scalar;
                                cp[2] = (cp[2]  > scalar) ? cp[2] : scalar;
                                cp[3] = (cp[3]  > scalar) ? cp[3] : scalar;
                                cp[4] = (cp[4]  > scalar) ? cp[4] : scalar;
                                cp[5] = (cp[5]  > scalar) ? cp[5] : scalar;
                                cp[6] = (cp[6]  > scalar) ? cp[6] : scalar;
                                cp[7] = (cp[7]  > scalar) ? cp[7] : scalar;
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > scalar) ? *cp : scalar;
                                cp++;
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((fcomplex*)range.Item3 + range.Item1);
                            scalar = *((fcomplex*)range.Item4);
                            while (j > 7) {
                                cp[0] = (ap[0]  > scalar) ? ap[0] : scalar;
                                cp[1] = (ap[1]  > scalar) ? ap[1] : scalar;
                                cp[2] = (ap[2]  > scalar) ? ap[2] : scalar;
                                cp[3] = (ap[3]  > scalar) ? ap[3] : scalar;
                                cp[4] = (ap[4]  > scalar) ? ap[4] : scalar;
                                cp[5] = (ap[5]  > scalar) ? ap[5] : scalar;
                                cp[6] = (ap[6]  > scalar) ? ap[6] : scalar;
                                cp[7] = (ap[7]  > scalar) ? ap[7] : scalar;
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > scalar) ? *ap : scalar;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((fcomplex*)range.Item3);
                            while (j > 7) {
                                cp[0] = (scalar  > cp[0]) ? scalar : cp[0];
                                cp[1] = (scalar  > cp[1]) ? scalar : cp[1];
                                cp[2] = (scalar  > cp[2]) ? scalar : cp[2];
                                cp[3] = (scalar  > cp[3]) ? scalar : cp[3];
                                cp[4] = (scalar  > cp[4]) ? scalar : cp[4];
                                cp[5] = (scalar  > cp[5]) ? scalar : cp[5];
                                cp[6] = (scalar  > cp[6]) ? scalar : cp[6];
                                cp[7] = (scalar  > cp[7]) ? scalar : cp[7];
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *cp) ? scalar : *cp;
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((fcomplex*)range.Item3);
                            bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (scalar  > bp[0]) ? scalar : bp[0];
                                cp[1] = (scalar  > bp[1]) ? scalar : bp[1];
                                cp[2] = (scalar  > bp[2]) ? scalar : bp[2];
                                cp[3] = (scalar  > bp[3]) ? scalar : bp[3];
                                cp[4] = (scalar  > bp[4]) ? scalar : bp[4];
                                cp[5] = (scalar  > bp[5]) ? scalar : bp[5];
                                cp[6] = (scalar  > bp[6]) ? scalar : bp[6];
                                cp[7] = (scalar  > bp[7]) ? scalar : bp[7];
                                bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *bp) ? scalar : *bp;
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
                };

                #region do the work
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }

                fixed (fcomplex* arrAP = arrA)
                fixed (fcomplex* arrBP = arrB)
                fixed (fcomplex* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }

                #endregion
                return new ILRetArray<fcomplex>(retStorage);
            }
        }

        private static unsafe ILRetArray<fcomplex>  maxEx(ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<fcomplex>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<fcomplex>(B.S);
                } else if (B.IsEmpty) {
                    return empty<fcomplex>(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation

               
                fcomplex[] retArr;

               
                fcomplex[] arrA = A.GetArrayForRead();

               
                fcomplex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<fcomplex>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<fcomplex>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILDenseStorage<fcomplex> retStorage = new ILDenseStorage<fcomplex>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;

                   
                    fcomplex* ap;

                   
                    fcomplex* bp;

                   
                    fcomplex* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (fcomplex*)range.Item3;
                                bp = (fcomplex*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (fcomplex*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (fcomplex*)range.Item3;
                                cp = (fcomplex*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *cp) ? *ap : *cp;
                                    ap++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (fcomplex*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (fcomplex*)range.Item4;
                                cp = (fcomplex*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            for (int s = 0; s < range.Item2; s++) {
                                bp = (fcomplex*)range.Item4;
                                cp = (fcomplex*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*cp  > *bp) ? *cp : *bp;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed (fcomplex* arrAP = arrA)
                fixed (fcomplex* arrBP = arrB)
                fixed (fcomplex* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<fcomplex>(retStorage);
            }
        }


       
        /// <summary>Maximum of A and B elementwise</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with the maximum elements of A and B</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<complex>  max(ILInArray<complex> A, ILInArray<complex> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                complex[] retArr;
                complex[] arrA = A.GetArrayForRead();
                complex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {
                        return array<complex>(new complex[1] { (A.GetValue(0) > B.GetValue(0)) ? A.GetValue(0) : B.GetValue(0) });
                    } else if (B.IsEmpty) {
                        return ILRetArray<complex>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< complex>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<complex>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<complex>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  maxEx(A, B);
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIB;
                        else {
                            retArr = ILMemoryPool.Pool.New<complex>(outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<complex> retStorage = new ILDenseStorage<complex>(retArr, outDims);
                int i = 0, workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                   
                    complex* cp = (complex*)range.Item5 + range.Item1;
                   
                    complex scalar;
                    int j = range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            complex* bp = ((complex*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (cp[0]  > bp[0]) ? cp[0] : bp[0];
                                cp[1] = (cp[1]  > bp[1]) ? cp[1] : bp[1];
                                cp[2] = (cp[2]  > bp[2]) ? cp[2] : bp[2];
                                cp[3] = (cp[3]  > bp[3]) ? cp[3] : bp[3];
                                cp[4] = (cp[4]  > bp[4]) ? cp[4] : bp[4];
                                cp[5] = (cp[5]  > bp[5]) ? cp[5] : bp[5];
                                cp[6] = (cp[6]  > bp[6]) ? cp[6] : bp[6];
                                cp[7] = (cp[7]  > bp[7]) ? cp[7] : bp[7];
                                cp += 8; bp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > *bp) ? *cp : *bp;
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            complex* ap = ((complex*)range.Item3 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > cp[0]) ? ap[0] : cp[0];
                                cp[1] = (ap[1]  > cp[1]) ? ap[1] : cp[1];
                                cp[2] = (ap[2]  > cp[2]) ? ap[2] : cp[2];
                                cp[3] = (ap[3]  > cp[3]) ? ap[3] : cp[3];
                                cp[4] = (ap[4]  > cp[4]) ? ap[4] : cp[4];
                                cp[5] = (ap[5]  > cp[5]) ? ap[5] : cp[5];
                                cp[6] = (ap[6]  > cp[6]) ? ap[6] : cp[6];
                                cp[7] = (ap[7]  > cp[7]) ? ap[7] : cp[7];
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *cp) ? *ap : *cp;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((complex*)range.Item3 + range.Item1);
                            bp = ((complex*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > bp[0]) ? ap[0] : bp[0];
                                cp[1] = (ap[1]  > bp[1]) ? ap[1] : bp[1];
                                cp[2] = (ap[2]  > bp[2]) ? ap[2] : bp[2];
                                cp[3] = (ap[3]  > bp[3]) ? ap[3] : bp[3];
                                cp[4] = (ap[4]  > bp[4]) ? ap[4] : bp[4];
                                cp[5] = (ap[5]  > bp[5]) ? ap[5] : bp[5];
                                cp[6] = (ap[6]  > bp[6]) ? ap[6] : bp[6];
                                cp[7] = (ap[7]  > bp[7]) ? ap[7] : bp[7];
                                ap += 8; bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *bp) ? *ap : *bp;
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((complex*)range.Item4);
                            while (j > 7) {
                                cp[0] = (cp[0]  > scalar) ? cp[0] : scalar;
                                cp[1] = (cp[1]  > scalar) ? cp[1] : scalar;
                                cp[2] = (cp[2]  > scalar) ? cp[2] : scalar;
                                cp[3] = (cp[3]  > scalar) ? cp[3] : scalar;
                                cp[4] = (cp[4]  > scalar) ? cp[4] : scalar;
                                cp[5] = (cp[5]  > scalar) ? cp[5] : scalar;
                                cp[6] = (cp[6]  > scalar) ? cp[6] : scalar;
                                cp[7] = (cp[7]  > scalar) ? cp[7] : scalar;
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > scalar) ? *cp : scalar;
                                cp++;
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((complex*)range.Item3 + range.Item1);
                            scalar = *((complex*)range.Item4);
                            while (j > 7) {
                                cp[0] = (ap[0]  > scalar) ? ap[0] : scalar;
                                cp[1] = (ap[1]  > scalar) ? ap[1] : scalar;
                                cp[2] = (ap[2]  > scalar) ? ap[2] : scalar;
                                cp[3] = (ap[3]  > scalar) ? ap[3] : scalar;
                                cp[4] = (ap[4]  > scalar) ? ap[4] : scalar;
                                cp[5] = (ap[5]  > scalar) ? ap[5] : scalar;
                                cp[6] = (ap[6]  > scalar) ? ap[6] : scalar;
                                cp[7] = (ap[7]  > scalar) ? ap[7] : scalar;
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > scalar) ? *ap : scalar;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((complex*)range.Item3);
                            while (j > 7) {
                                cp[0] = (scalar  > cp[0]) ? scalar : cp[0];
                                cp[1] = (scalar  > cp[1]) ? scalar : cp[1];
                                cp[2] = (scalar  > cp[2]) ? scalar : cp[2];
                                cp[3] = (scalar  > cp[3]) ? scalar : cp[3];
                                cp[4] = (scalar  > cp[4]) ? scalar : cp[4];
                                cp[5] = (scalar  > cp[5]) ? scalar : cp[5];
                                cp[6] = (scalar  > cp[6]) ? scalar : cp[6];
                                cp[7] = (scalar  > cp[7]) ? scalar : cp[7];
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *cp) ? scalar : *cp;
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((complex*)range.Item3);
                            bp = ((complex*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (scalar  > bp[0]) ? scalar : bp[0];
                                cp[1] = (scalar  > bp[1]) ? scalar : bp[1];
                                cp[2] = (scalar  > bp[2]) ? scalar : bp[2];
                                cp[3] = (scalar  > bp[3]) ? scalar : bp[3];
                                cp[4] = (scalar  > bp[4]) ? scalar : bp[4];
                                cp[5] = (scalar  > bp[5]) ? scalar : bp[5];
                                cp[6] = (scalar  > bp[6]) ? scalar : bp[6];
                                cp[7] = (scalar  > bp[7]) ? scalar : bp[7];
                                bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *bp) ? scalar : *bp;
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
                };

                #region do the work
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }

                fixed (complex* arrAP = arrA)
                fixed (complex* arrBP = arrB)
                fixed (complex* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }

                #endregion
                return new ILRetArray<complex>(retStorage);
            }
        }

        private static unsafe ILRetArray<complex>  maxEx(ILInArray<complex> A, ILInArray<complex> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<complex>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<complex>(B.S);
                } else if (B.IsEmpty) {
                    return empty<complex>(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation

               
                complex[] retArr;

               
                complex[] arrA = A.GetArrayForRead();

               
                complex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<complex>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<complex>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILDenseStorage<complex> retStorage = new ILDenseStorage<complex>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;

                   
                    complex* ap;

                   
                    complex* bp;

                   
                    complex* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (complex*)range.Item3;
                                bp = (complex*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (complex*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (complex*)range.Item3;
                                cp = (complex*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *cp) ? *ap : *cp;
                                    ap++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (complex*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (complex*)range.Item4;
                                cp = (complex*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            for (int s = 0; s < range.Item2; s++) {
                                bp = (complex*)range.Item4;
                                cp = (complex*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*cp  > *bp) ? *cp : *bp;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed (complex* arrAP = arrA)
                fixed (complex* arrBP = arrB)
                fixed (complex* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<complex>(retStorage);
            }
        }


       
        /// <summary>Maximum of A and B elementwise</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with the maximum elements of A and B</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<byte>  max(ILInArray<byte> A, ILInArray<byte> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                byte[] retArr;
                byte[] arrA = A.GetArrayForRead();
                byte[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {
                        return array<byte>(new byte[1] { (A.GetValue(0) > B.GetValue(0)) ? A.GetValue(0) : B.GetValue(0) });
                    } else if (B.IsEmpty) {
                        return ILRetArray<byte>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< byte>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<byte>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<byte>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  maxEx(A, B);
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIB;
                        else {
                            retArr = ILMemoryPool.Pool.New<byte>(outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, outDims);
                int i = 0, workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                   
                    byte* cp = (byte*)range.Item5 + range.Item1;
                   
                    byte scalar;
                    int j = range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            byte* bp = ((byte*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (cp[0]  > bp[0]) ? cp[0] : bp[0];
                                cp[1] = (cp[1]  > bp[1]) ? cp[1] : bp[1];
                                cp[2] = (cp[2]  > bp[2]) ? cp[2] : bp[2];
                                cp[3] = (cp[3]  > bp[3]) ? cp[3] : bp[3];
                                cp[4] = (cp[4]  > bp[4]) ? cp[4] : bp[4];
                                cp[5] = (cp[5]  > bp[5]) ? cp[5] : bp[5];
                                cp[6] = (cp[6]  > bp[6]) ? cp[6] : bp[6];
                                cp[7] = (cp[7]  > bp[7]) ? cp[7] : bp[7];
                                cp += 8; bp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > *bp) ? *cp : *bp;
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            byte* ap = ((byte*)range.Item3 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > cp[0]) ? ap[0] : cp[0];
                                cp[1] = (ap[1]  > cp[1]) ? ap[1] : cp[1];
                                cp[2] = (ap[2]  > cp[2]) ? ap[2] : cp[2];
                                cp[3] = (ap[3]  > cp[3]) ? ap[3] : cp[3];
                                cp[4] = (ap[4]  > cp[4]) ? ap[4] : cp[4];
                                cp[5] = (ap[5]  > cp[5]) ? ap[5] : cp[5];
                                cp[6] = (ap[6]  > cp[6]) ? ap[6] : cp[6];
                                cp[7] = (ap[7]  > cp[7]) ? ap[7] : cp[7];
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *cp) ? *ap : *cp;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (ap[0]  > bp[0]) ? ap[0] : bp[0];
                                cp[1] = (ap[1]  > bp[1]) ? ap[1] : bp[1];
                                cp[2] = (ap[2]  > bp[2]) ? ap[2] : bp[2];
                                cp[3] = (ap[3]  > bp[3]) ? ap[3] : bp[3];
                                cp[4] = (ap[4]  > bp[4]) ? ap[4] : bp[4];
                                cp[5] = (ap[5]  > bp[5]) ? ap[5] : bp[5];
                                cp[6] = (ap[6]  > bp[6]) ? ap[6] : bp[6];
                                cp[7] = (ap[7]  > bp[7]) ? ap[7] : bp[7];
                                ap += 8; bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > *bp) ? *ap : *bp;
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((byte*)range.Item4);
                            while (j > 7) {
                                cp[0] = (cp[0]  > scalar) ? cp[0] : scalar;
                                cp[1] = (cp[1]  > scalar) ? cp[1] : scalar;
                                cp[2] = (cp[2]  > scalar) ? cp[2] : scalar;
                                cp[3] = (cp[3]  > scalar) ? cp[3] : scalar;
                                cp[4] = (cp[4]  > scalar) ? cp[4] : scalar;
                                cp[5] = (cp[5]  > scalar) ? cp[5] : scalar;
                                cp[6] = (cp[6]  > scalar) ? cp[6] : scalar;
                                cp[7] = (cp[7]  > scalar) ? cp[7] : scalar;
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*cp  > scalar) ? *cp : scalar;
                                cp++;
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            scalar = *((byte*)range.Item4);
                            while (j > 7) {
                                cp[0] = (ap[0]  > scalar) ? ap[0] : scalar;
                                cp[1] = (ap[1]  > scalar) ? ap[1] : scalar;
                                cp[2] = (ap[2]  > scalar) ? ap[2] : scalar;
                                cp[3] = (ap[3]  > scalar) ? ap[3] : scalar;
                                cp[4] = (ap[4]  > scalar) ? ap[4] : scalar;
                                cp[5] = (ap[5]  > scalar) ? ap[5] : scalar;
                                cp[6] = (ap[6]  > scalar) ? ap[6] : scalar;
                                cp[7] = (ap[7]  > scalar) ? ap[7] : scalar;
                                ap += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (*ap  > scalar) ? *ap : scalar;
                                ap++; cp++;
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((byte*)range.Item3);
                            while (j > 7) {
                                cp[0] = (scalar  > cp[0]) ? scalar : cp[0];
                                cp[1] = (scalar  > cp[1]) ? scalar : cp[1];
                                cp[2] = (scalar  > cp[2]) ? scalar : cp[2];
                                cp[3] = (scalar  > cp[3]) ? scalar : cp[3];
                                cp[4] = (scalar  > cp[4]) ? scalar : cp[4];
                                cp[5] = (scalar  > cp[5]) ? scalar : cp[5];
                                cp[6] = (scalar  > cp[6]) ? scalar : cp[6];
                                cp[7] = (scalar  > cp[7]) ? scalar : cp[7];
                                cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *cp) ? scalar : *cp;
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((byte*)range.Item3);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (j > 7) {
                                cp[0] = (scalar  > bp[0]) ? scalar : bp[0];
                                cp[1] = (scalar  > bp[1]) ? scalar : bp[1];
                                cp[2] = (scalar  > bp[2]) ? scalar : bp[2];
                                cp[3] = (scalar  > bp[3]) ? scalar : bp[3];
                                cp[4] = (scalar  > bp[4]) ? scalar : bp[4];
                                cp[5] = (scalar  > bp[5]) ? scalar : bp[5];
                                cp[6] = (scalar  > bp[6]) ? scalar : bp[6];
                                cp[7] = (scalar  > bp[7]) ? scalar : bp[7];
                                bp += 8; cp += 8; j -= 8;
                            }
                            while (j-- > 0) {
                                *cp = (scalar  > *bp) ? scalar : *bp;
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
                };

                #region do the work
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }

                fixed (byte* arrAP = arrA)
                fixed (byte* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }

                #endregion
                return new ILRetArray<byte>(retStorage);
            }
        }

        private static unsafe ILRetArray<byte>  maxEx(ILInArray<byte> A, ILInArray<byte> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<byte>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<byte>(B.S);
                } else if (B.IsEmpty) {
                    return empty<byte>(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation

               
                byte[] retArr;

               
                byte[] arrA = A.GetArrayForRead();

               
                byte[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;

                   
                    byte* ap;

                   
                    byte* bp;

                   
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (byte*)range.Item3;
                                bp = (byte*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (byte*)range.Item3;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *cp) ? *ap : *cp;
                                    ap++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (byte*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (byte*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*ap  > *bp) ? *ap : *bp;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            for (int s = 0; s < range.Item2; s++) {
                                bp = (byte*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                    *cp = (*cp  > *bp) ? *cp : *bp;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed (byte* arrAP = arrA)
                fixed (byte* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<byte>(retStorage);
            }
        }



#endregion HYCALPER AUTO GENERATED CODE

    }
}