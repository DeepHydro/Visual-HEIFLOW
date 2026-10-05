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
using System.Runtime.InteropServices; 
using System.Text;
using System.Numerics; 
using ILNumerics.Storage;
using System.Security; 

namespace ILNumerics {
    /// <summary>
    /// Extension methods for ILNumerics types
    /// </summary>
    public static class ILExtensionMethods {
        /// <summary> 
        /// Convert this complex ILNumerics array to a system array of type System.Numerics.Complex[]
        /// </summary>
        /// <param name="A">this ILNumerics array</param>
        /// <param name="Storage">[option] if given, this system array will be used as storage for the conversion. Defaul: null (a new System.Array is returned)</param>
        /// <returns>System.Array with the elements of A</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the storage given does not have sufficient length for all elements of the ILNumerics array</exception>
        /// <remarks><para>The elements of the ILNumerics array will be copied to the System.Array and converted to the System.Numerics.Complex type.</para>
        /// <para>The System.Array returned will contain all elements of the ILNumerics arrays after conversion - in column major order.</para>
        /// <example>
        /// This example demonstrates a common scenario for interchanging System.Numerics.Complex[] with ILNumerics complex arrays. It takes the converion between System.Numerics.Complex and ILNumerics.complex 
        /// into account, as well as the memory management of all storage involved. 
        /// <code>
        /// ILArray&lt;complex> A = sqrtc(B); 
        /// // interfacing an external library as System.Numerics.Complex[]
        /// // We dont provide a System.Array as target for the conversion,
        /// // so it will be fetched from the ILNumerics memory management. 
        /// System.Numerics.Complex[] A_MSComplex = A.ToMSComplex(); 
        /// // .. do some work here with A_MSComplex
        /// // ...
        /// // The results get converted back to ILNumerics array 
        /// ILArray&lt;complex> Result = A_MSComplex.ToILArray(A.S, true); 
        /// </code>
        /// <para>For the conversion of the System.Numerics.Complex[] array back to an ILNumerics array, we use the same size (<c>A.S</c>) as before. Here, any size 
        /// would work, as long as it fits into the number of elements of the source System.Array.</para>
        /// <para>Note that the calls to <c>A.ToMSComplex()</c> and <c>A_MSComplex.ToILArray(A.S, true)</c> will fetch and return all System.Arrays involved into/from the 
        /// ILINumerics memory pool. So if called frequently, no pressure on the GC is produced, what qualifies this scheme for high performance scenarios.</para> 
        /// </example>
        /// </remarks>
        /// <seealso cref="ToILArray(Complex[], ILSize, bool )"/>
        public unsafe static Complex[] ToMSComplex(this ILArray<complex> A, Complex[] Storage = null) {
            return ToMSComplexHelper(A.Storage, Storage);
        }
        /// <summary>
        /// Convert this complex ILNumerics array to a system array of type System.Numerics.Complex[]
        /// </summary>
        /// <param name="A">this ILNumerics array</param>
        /// <param name="Storage">[option] if given, this system array will be used as storage for the conversion. Defaul: null (a new System.Array is returned)</param>
        /// <returns>System.Array with the elements of A</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the storage given does not have sufficient length for all elements of the ILNumerics array</exception>
        /// <remarks><para>The elements of the ILNumerics array will be copied to the System.Array and converted to the System.Numerics.Complex type.</para>
        /// <para>The System.Array returned will contain all elements of the ILNumerics arrays after conversion - in column major order.</para>
        /// <example>
        /// This example demonstrates a common scenario for interchanging System.Numerics.Complex[] with ILNumerics complex arrays. It takes the converion between System.Numerics.Complex and ILNumerics.complex 
        /// into account, as well as the memory management of all storage involved. 
        /// <code>
        /// ILArray&lt;complex> A = sqrtc(B); 
        /// // interfacing an external library as System.Numerics.Complex[]
        /// // We dont provide a System.Array as target for the conversion,
        /// // so it will be fetched from the ILNumerics memory management. 
        /// System.Numerics.Complex[] A_MSComplex = A.ToMSComplex(); 
        /// // .. do some work here with A_MSComplex
        /// // ...
        /// // The results get converted back to ILNumerics array 
        /// ILArray&lt;complex> Result = A_MSComplex.ToILArray(A.S, true); 
        /// </code>
        /// <para>For the conversion of the System.Numerics.Complex[] array back to an ILNumerics array, we use the same size (<c>A.S</c>) as before. Here, any size 
        /// would work, as long as it fits into the number of elements of the source System.Array.</para>
        /// <para>Note that the calls to <c>A.ToMSComplex()</c> and <c>A_MSComplex.ToILArray(A.S, true)</c> will fetch and return all System.Arrays involved into/from the 
        /// ILINumerics memory pool. So if called frequently, no pressure on the GC is produced, what qualifies this scheme for high performance scenarios.</para> 
        /// </example>
        /// </remarks>
        /// <seealso cref="ToILArray(Complex[], ILSize, bool )"/>
        public unsafe static Complex[] ToMSComplex(this ILInArray<complex> A, Complex[] Storage = null) {
            return ToMSComplexHelper(A.Storage, Storage);
        }
        /// <summary>
        /// Convert this complex ILNumerics array to a system array of type System.Numerics.Complex[]
        /// </summary>
        /// <param name="A">this ILNumerics array</param>
        /// <param name="Storage">[option] if given, this system array will be used as storage for the conversion. Defaul: null (a new System.Array is returned)</param>
        /// <returns>System.Array with the elements of A</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the storage given does not have sufficient length for all elements of the ILNumerics array</exception>
        /// <remarks><para>The elements of the ILNumerics array will be copied to the System.Array and converted to the System.Numerics.Complex type.</para>
        /// <para>The System.Array returned will contain all elements of the ILNumerics arrays after conversion - in column major order.</para>
        /// <example>
        /// This example demonstrates a common scenario for interchanging System.Numerics.Complex[] with ILNumerics complex arrays. It takes the converion between System.Numerics.Complex and ILNumerics.complex 
        /// into account, as well as the memory management of all storage involved. 
        /// <code>
        /// ILArray&lt;complex> A = sqrtc(B); 
        /// // interfacing an external library as System.Numerics.Complex[]
        /// // We dont provide a System.Array as target for the conversion,
        /// // so it will be fetched from the ILNumerics memory management. 
        /// System.Numerics.Complex[] A_MSComplex = A.ToMSComplex(); 
        /// // .. do some work here with A_MSComplex
        /// // ...
        /// // The results get converted back to ILNumerics array 
        /// ILArray&lt;complex> Result = A_MSComplex.ToILArray(A.S, true); 
        /// </code>
        /// <para>For the conversion of the System.Numerics.Complex[] array back to an ILNumerics array, we use the same size (<c>A.S</c>) as before. Here, any size 
        /// would work, as long as it fits into the number of elements of the source System.Array.</para>
        /// <para>Note that the calls to <c>A.ToMSComplex()</c> and <c>A_MSComplex.ToILArray(A.S, true)</c> will fetch and return all System.Arrays involved into/from the 
        /// ILINumerics memory pool. So if called frequently, no pressure on the GC is produced, what qualifies this scheme for high performance scenarios.</para> 
        /// </example>
        /// </remarks>
        /// <seealso cref="ToILArray(Complex[], ILSize, bool )"/>
        public unsafe static Complex[] ToMSComplex(this ILOutArray<complex> A, Complex[] Storage = null) {
            return ToMSComplexHelper(A.Storage, Storage);
        }
        /// <summary>
        /// Convert this complex ILNumerics array to a system array of type System.Numerics.Complex[]
        /// </summary>
        /// <param name="A">this ILNumerics array</param>
        /// <param name="Storage">[option] if given, this system array will be used as storage for the conversion. Defaul: null (a new System.Array is returned)</param>
        /// <returns>System.Array with the elements of A</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the storage given does not have sufficient length for all elements of the ILNumerics array</exception>
        /// <remarks><para>The elements of the ILNumerics array will be copied to the System.Array and converted to the System.Numerics.Complex type.</para>
        /// <para>The System.Array returned will contain all elements of the ILNumerics arrays after conversion - in column major order.</para>
        /// <example>
        /// This example demonstrates a common scenario for interchanging System.Numerics.Complex[] with ILNumerics complex arrays. It takes the converion between System.Numerics.Complex and ILNumerics.complex 
        /// into account, as well as the memory management of all storage involved. 
        /// <code>
        /// ILArray&lt;complex> A = sqrtc(B); 
        /// // interfacing an external library as System.Numerics.Complex[]
        /// // We dont provide a System.Array as target for the conversion,
        /// // so it will be fetched from the ILNumerics memory management. 
        /// System.Numerics.Complex[] A_MSComplex = A.ToMSComplex(); 
        /// // .. do some work here with A_MSComplex
        /// // ...
        /// // The results get converted back to ILNumerics array 
        /// ILArray&lt;complex> Result = A_MSComplex.ToILArray(A.S, true); 
        /// </code>
        /// <para>For the conversion of the System.Numerics.Complex[] array back to an ILNumerics array, we use the same size (<c>A.S</c>) as before. Here, any size 
        /// would work, as long as it fits into the number of elements of the source System.Array.</para>
        /// <para>Note that the calls to <c>A.ToMSComplex()</c> and <c>A_MSComplex.ToILArray(A.S, true)</c> will fetch and return all System.Arrays involved into/from the 
        /// ILINumerics memory pool. So if called frequently, no pressure on the GC is produced, what qualifies this scheme for high performance scenarios.</para> 
        /// </example>
        /// </remarks>
        /// <seealso cref="ToILArray(Complex[], ILSize, bool )"/>
        public unsafe static Complex[] ToMSComplex(this ILRetArray<complex> A, Complex[] Storage = null) {
            try {
                return ToMSComplexHelper(A.Storage, Storage);
            } finally {
                A.Dispose(); 
            }
        }

        [SecuritySafeCritical]
        unsafe private static Complex[] ToMSComplexHelper(ILDenseStorage<complex> A, Complex[] Storage) {
            if (object.Equals(A, null) || A.Size.NumberOfElements == 0)
                return new Complex[0];

            if (!object.Equals(Storage, null) && Storage.Length < A.Size.NumberOfElements)
                throw new Exceptions.ILArgumentException("The storage array given is too small for the conversion.");

            Complex[] retArr = (object.Equals(Storage, null)) ? ILMath.New<Complex>(A.Size.NumberOfElements) : Storage;

            GCHandle retHandle = GCHandle.Alloc(retArr, GCHandleType.Pinned);
            GCHandle inHandle = GCHandle.Alloc(A.GetArrayForRead(), GCHandleType.Pinned);
            complex* retP = (complex*)retHandle.AddrOfPinnedObject();
            complex* inP = (complex*)inHandle.AddrOfPinnedObject();
            ILMath.complex2ComplexHelper(inP, retP, A.Size.NumberOfElements);
            retHandle.Free();
            inHandle.Free();
            return retArr;
        }

        /// <summary>
        /// Convert this System.Numerics.Complex[] array to an ILNumerics array with ILNumerics.complex elements
        /// </summary>
        /// <param name="Storage">the source array, System.Numerics.Complex[]</param>
        /// <param name="size">[optional] if specified, the size and shape of the ILNumerics array returned. Otherwise, a vector of length Storage.Length will be returned.</param>
        /// <param name="recycleStorage">[optional] Places the incoming System.Numerics.Complex[] array into the ILNumerics memory pool. Default: false.</param>
        /// <returns>ILNumerics array of element type ILNumerics.complex</returns>
        /// <remarks>
        /// <para>If the <paramref name="size"/> argument was given, it must define an array shape according to the actual length of <paramref name="Storage"/>. Therefore, 
        /// <c>size.NumberOfElements &leq; Storage.Length</c> must be valid.
        /// </para>
        /// <para>In a high performance scenario, it is recommended to have the function recycle the incoming storage after use. If <paramref name="recycleStorage"/> is set to <c>true</c>, 
        /// the user is responsible not to reference the incoming array afterwards. The array will be stored into the ILNumerics memory pool and eventually be used for later 
        /// allocation requests (for instance in <see cref="ToMSComplex(ILArray<complex>, Complex[])"/>).</para>
        /// <example>
        /// This example demonstrates a common scenario for interchanging System.Numerics.Complex[] with ILNumerics complex arrays. It takes the converion between System.Numerics.Complex and ILNumerics.complex 
        /// into account, as well as the memory management of all storage involved. 
        /// <code>
        /// ILArray&lt;complex> A = sqrtc(B); 
        /// // interfacing an external library as System.Numerics.Complex[]
        /// // We dont provide a System.Array as target for the conversion,
        /// // so it will be fetched from the ILNumerics memory management. 
        /// System.Numerics.Complex[] A_MSComplex = A.ToMSComplex(); 
        /// // .. do some work here with A_MSComplex
        /// // ...
        /// // The results get converted back to ILNumerics array 
        /// ILArray&lt;complex> Result = A_MSComplex.ToILArray(A.S, true); 
        /// </code>
        /// <para>For the conversion of the System.Numerics.Complex[] array back to an ILNumerics array, we use the same size (<c>A.S</c>) as before. Here, any size 
        /// would work, as long as it fits into the number of elements of the source System.Array.</para>
        /// <para>Note that the calls to <c>A.ToMSComplex()</c> and <c>A_MSComplex.ToILArray(A.S, true)</c> will fetch and return all System.Arrays involved into/from the 
        /// ILINumerics memory pool. So if called frequently, no pressure on the GC is produced, what qualifies this scheme for high performance scenarios.</para> 
        /// </example>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the length of the <paramref name="Storage"/> is less than <c>size.NumberOfElements</c></exception>
        [SecuritySafeCritical]
        public unsafe static ILRetArray<complex> ToILArray(this Complex[] Storage, ILSize size = null, bool recycleStorage = false) {
            if (Storage == null || Storage.Length == 0) 
                return ILMath.empty<complex>(); 
            
            ILSize outSize = (size == null) ? ILMath.size(Storage.Length, 1) : size;
            if (outSize.NumberOfElements > Storage.Length) 
                throw new Exceptions.ILArgumentException("Invalid size definition! The number of elements defined by 'size' must be less or equal to the length of 'Storage'.");

            complex[] retArr = ILMath.New<complex>(outSize.NumberOfElements); 

            GCHandle retHandle = GCHandle.Alloc(retArr, GCHandleType.Pinned);
            GCHandle inHandle = GCHandle.Alloc(Storage, GCHandleType.Pinned);
            complex* retP = (complex*)retHandle.AddrOfPinnedObject();
            complex* inP = (complex*)inHandle.AddrOfPinnedObject();
            ILMath.complex2ComplexHelper(inP, retP, outSize.NumberOfElements);
            retHandle.Free();
            inHandle.Free();

            if (recycleStorage) {
                ILMath.free(Storage);
            }
            return ILMath.array<complex>(retArr, outSize);
        }
    }
}
