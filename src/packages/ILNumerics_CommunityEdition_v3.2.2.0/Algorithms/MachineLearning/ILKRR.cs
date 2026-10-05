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

        /// <summary>
        /// Type definitions for possible kernels in kernel ridge regression
        /// </summary>
        public enum KRRTypes {
            /// <summary>
            /// Linear kernel <c>k(x,y) = x'*y</c>
            /// </summary>
            linear,
            /// <summary>
            /// Polynominal kernel <c>k(x,y) = (x'*y + c)^d</c>
            /// </summary>
            polynomial,
            /// <summary>
            /// Gaussian (exponential) kernel <c>k(x,y) = exp(-norm(x-y)/sigma)</c>
            /// </summary>
            gaussian
        }

        internal delegate ILRetArray<double> ILKRRApplyFunc(ILInArray<double> X, ILKRRResult C); 

        /// <summary>
        /// Encapsulates the result of a kernel ridge regression and makes it applicable to new data
        /// </summary>
        public class ILKRRResult : ILResult<double> {

            private ILKRRApplyFunc m_applyFunc;
            internal ILKRRApplyFunc ApplyFunc {
                get { return m_applyFunc; }
                set { m_applyFunc = value; }
            } 
            internal ILArray<double> Shift = localMember<double>();
            internal ILArray<double> X = localMember<double>();
            internal KRRTypes Kernel = KRRTypes.gaussian;
            internal ILArray<double> KernelParameter = localMember<double>();
            internal ILArray<double> Alpha = localMember<double>();

            public override ILRetArray<double> Apply(ILInArray<double> Data, params ILBaseArray[] arguments)
            {
                if (IsDisposed) 
                    throw new InvalidOperationException("this result has already been disposed");
                return m_applyFunc(Data,this); 
            }

            public override void Dispose() {
                if (IsDisposed) 
                    return; 
                base.Dispose();
                if (!object.ReferenceEquals(Shift, null))
                    Shift.Dispose();
                if (!object.ReferenceEquals(X, null))
                    X.Dispose();
                if (!object.ReferenceEquals(KernelParameter, null))
                    KernelParameter.Dispose(); 
                if (!object.ReferenceEquals(Alpha,null))
                    Alpha.Dispose(); 
            }
        }

        /// <summary>
        /// Calculate the kernel ridge regression (KRR) of X to Y
        /// </summary>
        /// <param name="X">Input data X, observations in columns</param>
        /// <param name="Y">Input data Y (regression target)</param>
        /// <returns>Kernel ridge regression model with linear kernel and no regularization</returns>
        public static ILResult<double> krr(ILInArray<double> X, ILInArray<double> Y) {
            return krr(X, Y, KRRTypes.linear, 1, -1);
        }
        /// <summary>
        /// Calculate the kernel ridge regression (KRR) of X to Y
        /// </summary>
        /// <param name="X">Input data X, observations in columns</param>
        /// <param name="Y">Input data Y (regression target)</param>
        /// <param name="kernel">Kernel type to use</param>
        /// <param name="Kernelparam">Kernel parameters, scalar constant, if invalid: defaults to 1</param>
        /// <param name="Regularization">Regularization constant (set to -1 to have no regularization)</param>
        /// <returns>The kernel ridge regression model</returns>
        public static ILResult<double> krr(ILInArray<double> X, ILInArray<double> Y, KRRTypes kernel, 
                                           ILInArray<double> Kernelparam, ILInArray<double> Regularization) {
            using (ILScope.Enter(X, Y, Kernelparam, Regularization)) {
                ILArray<double> x = check(X);
                ILArray<double> y = check(Y);
                ILArray<double> kernelparam = check(Kernelparam, (a) => { return (a.IsScalar) ? a : 1.0; });
                ILArray<double> regularization = check(Regularization, (a) => { return (a.IsScalar) ? a : 0.0; });

                ILKRRResult C = new ILKRRResult();

                C.ApplyFunc = krrapply; 
                // centering 
                C.Shift.a = mean(x, 1);
                //% X = X - repmat(C.Shift,1,n); 
                x = x - repmat(C.Shift, 1, x.S[1]);
                ILArray<double> K = empty<double>();
                switch (kernel) {
                    case KRRTypes.linear:
                        K = multiply(x.T, x);                                                               // linear: K = X'*X; 
                        break;
                    case KRRTypes.polynomial:
                        K = pow((multiply(x.T, x) + 1), kernelparam);                                           // % K = (X'*X + 1).^kernelparameter;
                        break;
                    case KRRTypes.gaussian:
                        K = gkernel(x, x, kernelparam);                                                     // % K = gkernel(X,X,kernelparameter); 
                        break;
                }
                C.X.a = x.C;
                C.Kernel = kernel;
                C.KernelParameter.a = kernelparam;
                
                y = (y.S[0] < y.S[1]) ? y.T : y.C;                                                          // if (size(y,1) < size(y,2)) y = y'; end
                if (regularization == 0) {                                                                    // if (~exist('regularization') || regularization == 0) 
                    ILArray<double> U = empty<double>();                                                      //    [U,V] = eig(K);
                    ILArray<double> V = empty<double>();
                    U.a = eigSymm(K, V);
                    ILArray<double> c = diag(V);                                                              //    c = diag(V);  
                    ILArray<double> minLooErr = double.PositiveInfinity;                                      //    minLooErr = inf;
                    //                                                                                        //    cr = find(c > 1e-12)';                                 
                    foreach (int ci in find(c > 1e-12)) {                                                     //    for ci = cr      
                        using (ILScope.Enter()) {
                            ILArray<double> Sd = c / (c + c[ci]);                                             //        Sd = c./(c + c(ci)); 
                            ILArray<double> S = multiply(U, diag(Sd), U.T);                                   //        S = U * diag(Sd) * U';
                            ILArray<double> looer = sum(y - multiply(S, y) / pow(1 - diag(S), 2.0)) / x.S[1];   //        looerr = sum((y - S*y)./(1-diag(S)).^2) ./ n; 
                            if (minLooErr > looer) {                                                          //        if (minLooErr > looerr) 
                                minLooErr.a = looer;                                                          //            minLooErr = looerr;
                                regularization.a = c[ci];                                                     //            regularization = c(ci);
                            }                                                                                 //        end
                        }                                                                                     //    end
                    }
                    // since we already have U, lets compute alpha that way
                    C.Alpha.a = multiply(U, diag(1 / (c + regularization)), U.T, y).T;                        //    C.alpha = U * diag(1./(c + regularization)) * U' * y; 
                    //                                                                                        //    C.alpha = C.alpha';
                    return C;                                                                                 //    return; 
                } else {                                                                                      //end
                    C.Alpha.a = linsolve((K + diag(zeros(1, x.S[1]) + regularization)), y).T;                 // C.alpha = (K + diag(zeros(1,n) + regularization))\y; 
                    return C;                                                                                 // C.alpha = C.alpha'; 
                }                                                                                             //end 
            }
        }

        private static ILRetArray<double> gkernel(ILInArray<double> X1, ILInArray<double> X2, ILInArray<double> kernelparam) {
            using (ILScope.Enter(X1, X2, kernelparam)) {
                int d = X1.S[0], n1 = X1.S[1], n2 = X2.S[1];                    // [d,n1] = size(X1); n2 = size(X2,2); 
                ILArray<double> XX = multiply(X1.T, X2);                        // XX = X1'*X2; 
                ILArray<double> K = repmat(sum(X2 * X2, 0), n1, 1);             // K = repmat(sum(X2.^2,1),n1,1); 
                K.a = K + repmat(sum(X1 * X1, 0).T, 1, n2);                     // K = K + repmat(sum(X1.^2,1)',1,n2); 
                K.a = K - 2 * XX;                                               // K = K - 2.*XX; 
                K.a = exp(-K / (2 * d * kernelparam * kernelparam));            // K = exp(-K./(2 * d * w.^2)); 
                return K; 
            }
        }

        private static ILRetArray<double> krrapply(ILInArray<double> X, ILKRRResult C) {
            using (ILScope.Enter(X)) {
                ILArray<double> x = check(X);  
                int d = x.S[0], n = x.S[1];                                     // [d,n] = size(X); 
                x.a = x - repmat(C.Shift, 1, n);                                  // X = X - repmat(C.Shift,1,n); 
                ILArray<double> K = empty<double>();
                switch (C.Kernel) {
                    case KRRTypes.linear:
                        K.a = multiply(x.T, C.X);                               // K = X'*C.X; 
                        break;
                    case KRRTypes.polynomial:
                        K.a = pow(multiply(x.T, C.X) + 1, C.KernelParameter);    // K = (X'*C.X + 1).^C.parameter; 
                        break;
                    case KRRTypes.gaussian:
                        K.a = gkernel(x, C.X, C.KernelParameter);                // K = gkernel(X,C.X,C.parameter); 
                        break;
                }
                return sum(C.Alpha * K, 1).T;                              //Y = sum(repmat(C.alpha,n,1).*K,2)';  
            }
        }

    }
}
