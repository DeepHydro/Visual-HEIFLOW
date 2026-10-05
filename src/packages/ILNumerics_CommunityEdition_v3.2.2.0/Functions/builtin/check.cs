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
        /// Check if A is a valid parameter
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="A">Input parameter</param>
        /// <param name="ErrorMessage">[optional] Exception message</param>
        /// <param name="evaluation">[optional] Evaluation function, checks input parameter and transforms it into result, gets only called for A other than null</param>
        /// <param name="allowNullInput">[optional] Only if A is null -> for true: returns null, false: throws exception. If <paramref name="Default"/> was defined, this parameter is ignored.</param>
        /// <param name="Default">[optional] If <paramref name="A"/> is null on input, this value is returned. If no default is given (i.e: null), <paramref name="allowNullInput"/> is evaluated.</param>
        /// <returns>Result of evaluation(A) or A</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null on entry and <paramref name="allowNullInput"/> is false</exception>
        public static ILRetArray<T> check<T>(ILInArray<T> A, Func<ILInArray<T>, ILRetArray<T>> evaluation = null, bool allowNullInput = false, string ErrorMessage = "", ILInArray<T> Default = null) {
            using (ILScope.Enter(A,Default)) {
                if (object.Equals(A, null)) {
                    if (isnull(Default)) {
                        if (!allowNullInput)
                            throw new ILArgumentException("an input parameter was found to be null");
                        else
                            return null;
                    } else {
                        return Default.C; 
                    }
                }
                if (evaluation == null)
                    return A;
                ILRetArray<T> ret = evaluation(A);
                if (object.Equals(ret, null))
                    throw new ILArgumentException(String.IsNullOrEmpty(ErrorMessage) ? "invalid input parameter. check the documentation!" : ErrorMessage);
                else
                    return ret;
            }
        }
        /// <summary>
        /// Check if A is a valid parameter
        /// </summary>
        /// <param name="A">Input parameter</param>
        /// <param name="ErrorMessage">[optional] Exception message</param>
        /// <param name="evaluation">[optional] Evaluation function, checks input parameter and transforms it into result, gets only called for A other than null</param>
        /// <param name="allowNullInput">[optional] Only if A is null -> for true: returns null, false: throws exception</param>
        /// <returns>Result of evaluation(A) or A</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null on entry and <paramref name="allowNullInput"/> is false</exception>
        public static ILRetLogical check(ILInLogical A, Func<ILInLogical, ILRetLogical> evaluation = null, bool allowNullInput = false, string ErrorMessage = "") {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null)) {
                    if (!allowNullInput)
                        throw new ILArgumentException("an input parameter was found to be null");
                    else
                        return null;
                }
                if (evaluation == null)
                    return A;
                ILRetLogical ret = evaluation(A);
                if (object.Equals(ret, null))
                    throw new ILArgumentException(String.IsNullOrEmpty(ErrorMessage) ? "invalid input parameter. check the documentation!" : ErrorMessage);
                else
                    return ret;
            }
        }
        /// <summary>
        /// Check if A is a valid parameter
        /// </summary>
        /// <param name="A">Input parameter</param>
        /// <param name="ErrorMessage">[Optional] Exception message</param>
        /// <param name="evaluation">[Optional] Evaluation function, checks input parameter and transforms it into result, gets only called for A other than null</param>
        /// <param name="allowNullInput">[Optional] Only if A is null -> true: returns null, false: throws exception</param>
        /// <returns>Result of evaluation(A) or A</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null on entry and <paramref name="allowNullInput"/> is false</exception>
        public static ILRetCell check(ILInCell A, Func<ILInCell, ILRetCell> evaluation = null, bool allowNullInput = false, string ErrorMessage = "") {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null)) {
                    if (!allowNullInput)
                        throw new ILArgumentException("an input parameter was null");
                    else
                        return null;
                }
                if (evaluation == null)
                    return A;
                ILRetCell ret = evaluation(A);
                if (object.Equals(ret, null))
                    throw new ILArgumentException(String.IsNullOrEmpty(ErrorMessage) ? "invalid input parameter. check the documentation!" : ErrorMessage);
                else
                    return ret;
            }
        }
    }
}
