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
using System.Linq;
using System.Text;
using System.Linq.Expressions; 
using ILNumerics.Storage; 
using System.Security; 
using System.Security.Permissions; 

namespace ILNumerics.Misc {
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public class ILExpression : ILDenseArray<Expression> {

        public static readonly IDictionary<string, Func<int,int>> Cache = new Dictionary<string, Func<int,int>>(); 

        public ILExpression(Expression exp) : base(new ILDenseStorage<Expression>(
            new Expression[1] { exp }, ILSize.Scalar1_1), false) {
        }

        public Expression Expression {
            get { return GetValue(0); }
        }

        #region operator overload
        public static ILExpression operator -(ILExpression a, ILExpression b) {
            return new ILExpression(Expression.Subtract(a.Expression, b.Expression));
        }
        public static ILExpression operator +(ILExpression a, ILExpression b) {
            return new ILExpression(Expression.Add(a.Expression, b.Expression));
        }
        public static ILExpression operator *(ILExpression a, ILExpression b) {
            return new ILExpression(Expression.Multiply(a.Expression, b.Expression));
        }
        public static ILExpression operator /(ILExpression a, ILExpression b) {
            return new ILExpression(Expression.Divide(a.Expression, b.Expression));
        }
        
        public static ILExpression operator +(ILExpression expr, ILBaseArray a) {
            return new ILExpression(Expression.Add(expr.Expression, BA2Expr(a)));
        }
        public static ILExpression operator +(ILBaseArray a, ILExpression expr) {
            return new ILExpression(Expression.Add(BA2Expr(a), expr.Expression));
        }
        public static ILExpression operator -(ILExpression expr, ILBaseArray a) {
            return new ILExpression(Expression.Subtract(expr.Expression, BA2Expr(a)));
        }
        public static ILExpression operator -(ILBaseArray a, ILExpression expr) {
            return new ILExpression(Expression.Subtract(BA2Expr(a), expr.Expression));
        }

        public static ILExpression operator /(ILExpression expr, ILBaseArray a) {
            return new ILExpression(Expression.Divide(expr.Expression, BA2Expr(a)));
        }
        public static ILExpression operator /(ILBaseArray a, ILExpression expr) {
            return new ILExpression(Expression.Divide(BA2Expr(a), expr.Expression));
        }
        public static ILExpression operator *(ILExpression expr, ILBaseArray a) {
            return new ILExpression(Expression.Multiply(expr.Expression, BA2Expr(a)));
        }
        public static ILExpression operator *(ILBaseArray a, ILExpression expr) {
            return new ILExpression(Expression.Multiply(BA2Expr(a), expr.Expression));
        }
        #endregion 

        #region helper
        private static MethodCallExpression BA2Expr(ILBaseArray a) {
            MemberExpression storageExpr = Expression.Property(Expression.Constant(a), typeof(ILBaseArray).GetProperty("Storage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            MethodCallExpression getValExpr = Expression.Call(
                                                storageExpr
                                                , typeof(ILStorage).GetMethod("GetValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                                                , Expression.Constant(new int[] { 0 }));
            MethodCallExpression convertInt32 = Expression.Call(
                                                typeof(Convert).GetMethod("ToInt32", new Type[] { typeof(object) })
                                                , getValExpr);
            return convertInt32;
        }
        #endregion

        #region ILBaseArray Interface

        public override bool IsComplex {
            get { return false; }
        }

        public override bool IsNumeric {
            get { return false; }
        }

        public override void ToStream(System.IO.Stream outStream, string format, ILArrayStreamSerializationFlags method) {
            throw new NotImplementedException();
        }
        internal override bool EnterScope() {
            return false;
        }
        #endregion ILBaseArray Interface 
    
        internal static int Evaluate(Expression iLExpression, int dimLength) {
            string key = iLExpression.ToString();
            if (Cache.ContainsKey(key)) {
                return Cache[key](dimLength);
            } else {
                ParameterExpression endParameter = (ParameterExpression)ILMath.end.Expression;
                new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Assert();
                Func<int, int> ret = Expression.Lambda<Func<int, int>>(iLExpression, endParameter).Compile();
                ReflectionPermission.RevertAssert(); 
                Cache[key] = ret;
                return ret(dimLength);
            }
        }

    }
}
