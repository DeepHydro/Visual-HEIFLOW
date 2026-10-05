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

namespace ILNumerics.Exceptions
{
    /// <summary>
    /// Generic exception, base class for all exceptions thrown by ILNumerics
    /// </summary>
    [Serializable]
    public class ILException : System.Exception
    {
        private System.Runtime.Serialization.SerializationInfo info;
        private System.Runtime.Serialization.StreamingContext context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILException(String message) : base(message) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILException(String message, Exception innerException) 
                : base(message, innerException) { }

        public ILException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
        }
    }
    /// <summary>
    /// Base class for mathematical exceptions. Needed e.g. in interpreter for proper error
    /// messages
    /// </summary>
    public class ILMathException : ILException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILMathException(String message) : base(message) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILMathException(String message, Exception innerException) 
                : base(message, innerException) { }
    }

    /// <summary>
    /// One of the most common exceptions: The matrix sizes do not match
    /// </summary>
    public class ILDimensionMismatchException : ILMathException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ILDimensionMismatchException() : base("Matrix dimensions must match") { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILDimensionMismatchException(String message, Exception innerException) 
                : base(message, innerException) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILDimensionMismatchException(string message) : base(message) { }
    }

    /// <summary>
    /// Something was wrong with the arguments supplied
    /// </summary>
    [Serializable]
    public class ILArgumentException : ILException {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILArgumentException(String message)
            : base(message) {}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILArgumentException(String message, Exception innerException) 
                : base(message, innerException) { }
        public ILArgumentException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) 
            : base (info, context) { }
    }
    /// <summary>
    /// A function was called with the wrong number of arguments
    /// </summary>
    public class ILArgumentNumberException : ILArgumentException {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILArgumentNumberException(String message)
            : base(message) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILArgumentNumberException(String message, Exception innerException)
            : base(message, innerException) { }
    }
    /// <summary>
    /// A function argument has the wrong size
    /// </summary>
    public class ILArgumentSizeException : ILArgumentException {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILArgumentSizeException(String message)
            : base(message) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILArgumentSizeException(String message, Exception innerException)
            : base(message, innerException) { }
    }
    /// <summary>
    /// A function was called with a wrong argument type
    /// </summary>
    /// <remarks>This exception might be thrown if the size or inner 
    /// type of a argument is invalid. (e.g. matrix expected, but 3D array found)
    /// </remarks>
    public class ILArgumentTypeException : ILArgumentException {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILArgumentTypeException(String message)
            : base(message) {}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
         public ILArgumentTypeException(String message, Exception innerException) 
                : base(message, innerException) { }
    }
    
    /// <summary>
    /// A request could not be completed due to not enough memory available
    /// </summary>
    public class ILMemoryException : ILException {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILMemoryException(String message)
            : base(message) {}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILMemoryException(String message, Exception innerException) 
                : base(message, innerException) { }
    }
    /// <summary>
    /// Thrown on illegal casting attempts
    /// </summary>
    public class ILCastException : ILException
    {
        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="message">Addditional message to be included into the exception</param>
        public ILCastException(String message)
            : base(message) { }
        /// <summary>
        /// Costructor
        /// </summary>
        /// <param name="message">Additional message to be included into the exception</param>
        /// <param name="innerException">On cascaded exception handling, the exception catched before</param>
        public ILCastException(String message, Exception innerException)
            : base(message, innerException) { }
    }
    /// <summary>
    /// ILOutputException, thrown if an I/O attempt fails
    /// </summary>
    public class ILOutputException : ILException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILOutputException(String message)
            : base(message) { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILOutputException(String message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown if an operation could not completed
    /// </summary>
    public class ILInvalidOperationException : ILException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILInvalidOperationException(String message)
            : base(message) {}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        /// <param name="innerException">Inner Exception</param>
        public ILInvalidOperationException(String message, Exception innerException) 
                : base(message, innerException) { }
    }
    /// <summary>
    /// No valid license could be found
    /// </summary>
    public class ILInvalidLicenseException : ILException {
        /// <summary>
        /// Create a new ILInvalidLicenseException 
        /// </summary>
        /// <param name="message">Additional message to be included</param>
        public ILInvalidLicenseException(String message, Exception innerExc)
            : base(message, innerExc) {}
    }
}
