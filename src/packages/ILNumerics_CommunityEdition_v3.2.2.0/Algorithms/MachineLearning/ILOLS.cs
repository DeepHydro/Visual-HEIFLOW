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
using System.Text;

namespace ILNumerics {

    public partial class ILMath {

        internal delegate ILRetArray<T> ILRidgeRegressionApplyFunc<T>(ILInArray<T> X, ILRidgeRegressionResult<T> C); 
        /// <summary>
        /// This class stores the result of a ridge regression
        /// </summary>
        /// <typeparam name="T">Element type (precision) for the result</typeparam>
        /// <remarks>This class is returned from <see cref="ILNumerics.ILMath.ridge_regression(ILInArray&lt;double>, ILInArray&lt;double>, ILBaseArray, ILBaseArray)"/>
        /// and all its overloads. The class stores all data needed to apply the regression result to new data points. Therefore a 
        /// function <see cref="ILNumerics.ILMath.ILRidgeRegressionResult{T}.Apply(ILNumerics.ILInArray{T}, ILNumerics.ILBaseArray[])" /> is provided.
        /// <para>The class implements the <c>IDisposable</c> interface and should be used inside a 'using' block or manually be disposed 
        /// after use.</para></remarks>
        public class ILRidgeRegressionResult<T> : ILResult<T> {
            internal ILRidgeRegressionApplyFunc<T> m_applyFunc; 
            internal ILArray<T> m_centerX = ILMath.localMember<T>();
            internal ILArray<T> m_centerY = ILMath.localMember<T>();
            internal ILArray<T> m_weights = ILMath.localMember<T>();
            internal int m_p = 1; 
            internal ILArray<T> m_pow = ILMath.localMember<T>();
            
            /// <summary>
            /// Apply the result on new datapoints
            /// </summary>
            /// <param name="X">New datapoints, same dimension as used for learning</param>
            /// <returns></returns>
            public override ILRetArray<T> Apply(ILInArray<T> X, params ILBaseArray[] arguments) {
                if (IsDisposed) 
                    throw new Exceptions.ILInvalidOperationException("the regression result is disposed already"); 
                return m_applyFunc(X,this); 
            }

            #region IDisposable Members
            /// <summary>
            /// Dispose off the result and free all storages used 
            /// </summary>
            public override void Dispose() {
                base.Dispose(); 
                m_centerX.Dispose(); 
                m_centerY.Dispose(); 
                m_weights.Dispose(); 
                m_pow.Dispose(); 
            }

            #endregion
        }
        /// <summary>
        /// Ordinary least squares regression/ ridge regression
        /// </summary>
        /// <param name="X">Data matrix, data points in columns</param>
        /// <param name="Y">Training targets (or 'labels') corresponding to <paramref name="X"/></param>
        /// <param name="Degree">Highest degree of polynomials for the design matrix</param> 
        /// <param name="Regularization">Regularization constant (usually some "small" summand for stabilizing the matrix inversion).</param>
        /// <returns>Train result used for applying the model to new data.</returns>
        public static ILRidgeRegressionResult<double> ridge_regression(ILInArray<double> X, ILInArray<double> Y, ILBaseArray Degree, ILBaseArray Regularization) {
            using (ILScope.Enter(X, Y, Degree, Regularization)) {

                ILArray<double> x = check(X);
                ILArray<double> y = check(Y); 

                ILRidgeRegressionResult<double> ret = new ILRidgeRegressionResult<double>(); 
                double c = (double)todouble(Regularization); 
                int p = (int)toint32(Degree); 
                if (c < 0) c = 1e-10; 
                if (p < 1) p = 1; 
                ret.m_pow.a  = repmat(vec<double>(1,p).T,x.S[0],1)[full];
                ILArray<double> Xd = pow(repmat(x, p, 1), repmat(ret.m_pow, 1, x.S[1])); 
                ret.m_p = p; 
                ret.m_centerX.a = mean(Xd,1);
                ret.m_centerY.a = mean(y,1); 
                Xd.a = Xd - ret.m_centerX; 
                ILArray<double> Yd = y - ret.m_centerY; 
                ret.m_weights.a = linsolve(multiply(Xd,Xd.T) + c * eye(Xd.S[0],Xd.S[0]) , multiply(Xd,y.T));
                ret.m_applyFunc = RidgeRegressionApply; 
                return ret; 
            }
        }

        internal static ILRetArray<double> RidgeRegressionApply(ILInArray<double> X, ILRidgeRegressionResult<double> C) {
            using (ILScope.Enter(X)) {
                int n = X.S[1];
                ILArray<double> Xd = pow(repmat(X, C.m_p, 1),repmat(reshape(C.m_pow, numel(C.m_pow), 1), 1, n));
                return multiply(C.m_weights.T, (Xd - repmat(C.m_centerX, 1, n))) + repmat(C.m_centerY, 1, n);
            }
        }

    }
}
