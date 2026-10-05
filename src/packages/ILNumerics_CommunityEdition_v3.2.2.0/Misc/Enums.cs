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


namespace ILNumerics {

    /// <summary>
    /// All toolboxes currently available in ILNumerics 
    /// </summary>
    public enum Toolboxes {
        /// <summary>
        ///  Machine Learning Toolbox
        /// </summary>
        MachineLearning,
        /// <summary>
        /// Statistics Toolbox
        /// </summary>
        Statistics
    }

    /// <summary>
    /// Modes for implicit conversions from logical arrays to System.Boolean 
    /// </summary>
    /// <remarks>
    /// <seealso cref="ILNumerics.Settings.LogicalArrayToBoolConversion"/></remarks>
    public enum LogicalConversionMode {
        /// <summary>
        /// Throw an exception on the attempt to convert a non scaler value to logical/boolean.
        /// </summary>
        NonScalarThrowsException,
        /// <summary>
        /// Convert non scalar values to logical/boolean by applying the allall() function (i.e. it is true when all elements are non zero).
        /// </summary>
        ImplicitAllAll
    }
    /// <summary>
    /// Enumerate all numeric type names used by ILNumerics
    /// </summary>
    public enum NumericType {
        /// <summary>
        /// double element type.
        /// </summary>
        Double,
        /// <summary>
        /// float element type.
        /// </summary>
        Single,
        /// <summary>
        /// complex element type.
        /// </summary>
        Complex ,
        /// <summary>
        /// fcomplex element type.
        /// </summary>
        FComplex,
        /// <summary>
        /// byte element type.
        /// </summary>
        Byte,
        /// <summary>
        /// Int32 element type.
        /// </summary>
        Int32,
        /// <summary>
        /// Int64 element type.
        /// </summary>
        Int64,
    }

    /// <summary>
    /// Defines the way ILArrays are serialized to stream.
    /// </summary>
    /// <seealso cref="ILNumerics.ILDenseArray&lt;T&gt;.ToStream(System.IO.Stream,string,ILArrayStreamSerializationFlags)"/>
    public enum ILArrayStreamSerializationFlags {
        /// <summary>
        /// Print values 'vectorized': one value after each other. The true dimension configuration 
        /// of the array will be lost in the result. 
        /// </summary>
        Serial,
        /// <summary>
        /// Print values 'matrixwise'. The real dimensions configuration for the array are kept 
        /// in the result. The array will be printed by pages, consisting out of the 1st and 2nd 
        /// leading dimnsion. A dimension tag will prefix each page. The format can be used as 
        /// fancier output version for human reading as well as human readable serialization 
        /// format. ILArray's are capable of constructing from streams containing this type of 
        /// output. 
        /// </summary>
        Formatted, 
        /// <summary>
        /// Export whole array instance to matlab 5.0 format
        /// </summary>
        Matlab
    }
    /// <summary>
    /// Possible properties for matrices 
    /// </summary>
    /// <remarks><para>These properties may be returned by function overloads receiving a MatrixProperties 
    /// parameter by reference. </para>
    /// <para><![CDATA[This enum is a bitflag'ed enum! You may query for any combination via the bitwise operators | and &. ]]></para></remarks>
    public enum MatrixProperties : int {
        /// <summary>
        /// Hermitian matrix 
        /// </summary>
        Hermitian = 1,
        /// <summary>
        /// Positive definite
        /// </summary>
        PositivDefinite = 2 ,
        /// <summary>
        /// Upper triangular matrix
        /// </summary>
        UpperTriangular = 4,
        /// <summary>
        /// Lower triangular matrix
        /// </summary>
        LowerTriangular = 8,
        /// <summary>
        /// Square matrix
        /// </summary>
        Square = 16,
        /// <summary>
        /// Diagonal matrix
        /// </summary>
        Diagonal = 32,
        /// <summary>
        /// The matrix is singular 
        /// </summary>
        Singular = 64,
        /// <summary>
        /// Hessenberg matrix
        /// </summary>
        Hessenberg = 128,
        /// <summary>
        /// Householder matrix
        /// </summary>
        Householder = 256,
        /// <summary>
        /// Unitary matrix
        /// </summary>
        Unitary = 512,
        /// <summary>
        /// Orthogonal matrix
        /// </summary>
        Orthogonal = 1024,
        /// <summary>
        /// Orthonormal matrix
        /// </summary>
        Orthonormal = 2048,
        /// <summary>
        /// The matrix has deficient rank
        /// </summary>
        RankDeficient = 4096,
        /// <summary>
        /// The matrix has no special properties
        /// </summary>
        None = 8192,
        /// <summary>
        /// No specific properties known (default)
        /// </summary>
        Unknown = 0
    }
}

