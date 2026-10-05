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
using System.Threading;

namespace ILNumerics {
    /// <summary>
    /// An artificial scope class, used by the ILNumerics memory management
    /// </summary>
    public class ILScope : IDisposable {

        internal class ILThreadingContext {
            public Stack<ILBaseArray> Arrays = new Stack<ILBaseArray>(100);
            public Stack<ILScope> Scopes = new Stack<ILScope>(100);

            public void RegisterArray(ILBaseArray A) {
                if (Scopes.Count > 0) /* && A.EnterScope() ) */ { //ho: commented out, because otherwise common local arrays do return false and hence get not registered in implicit conversions here! 
                    Arrays.Push(A);
                    Scopes.Peek().Count++;
                }
            }
        }
        [ThreadStatic]
        private static ILThreadingContext s_threadContext; 
        /// <summary>
        /// Begins an artificial scope block within a local function block
        /// </summary>
        /// <param name="inputArrays">Any <b>input</b> arry, given as parameter for the current function</param>
        /// <returns>A new scope</returns>
        /// <remarks><para>The <c>ILScope</c> class plays an important role for the ILNumerics memory management. When writing functions in ILNumerics, 
        /// <c>ILScope</c> is used, to define blocks of artificial scopes for local function blocks. ILNumerics ensures, no memory is left as garbage, once 
        /// such a scope block was left. Furthermore, it garantees, input arrays are kept alive during the execution of the block. By following these 
        /// <a href="http://ilnumerics.net/$GeneralRules.html" target="ILMain">simple rules</a>, ILNumerics is able to optimize the execution of the algorithm regarding 
        /// execution speed and memory footprint.</para></remarks>
        /// <example><para>The examples demonstrates a custom function in ILNumerics. It demonstrates the use of distinct array types in the function declaration and the use of 
        /// artificial scopes.</para>
        /// <code><![CDATA[ILRetArray<double> FreqPeaks(ILInArray<double> inData, ILOutArray<double> freq = null, double sampFreq = 44.1) { 
        ///
        ///    using (ILScope.Enter(inData)) {    
        ///             
        ///        ILArray<double> Data = check(inData); 
        ///        ILArray<double> retLength = min(ceil(Data.Length / 2.0 + 1), 5.0);   
        ///        ILArray<double> Window = stdWindowFunc(Data.Length);  
        ///        ILArray<double> magnitudes = abs(fft(Data * Window));  
        ///        magnitudes = magnitudes[r(0,end / 2 + 1)];  
        /// 
        ///        ILArray<double> indices = empty();  
        ///        ILArray<double> sorted = sort(magnitudes, indices, descending:true);  
        ///        if (!isnull(freq)) 
        ///            freq.a = (sampFreq / 2.0 / magnitudes.Length * indices)[r(0,retLength-1)];  
        ///        return magnitudes[r(0,retLength-1)];  
        ///    } 
        ///}]]></code>
        /// </example>
        public static IDisposable Enter(params ILBaseArray[] inputArrays) {
            return new ILScope(inputArrays);
        }
        /// <summary>
        /// Begins an artificial scope block within a local function block
        /// </summary>
        /// <param name="inputArrays">Any <b>input</b> arry, given as parameter for the current function</param>
        /// <returns>A new scope</returns>
        /// <remarks><para>The <c>ILScope</c> class plays an important role for the ILNumerics memory management. When writing functions in ILNumerics, 
        /// <c>ILScope</c> is used, to define blocks of artificial scopes for local function blocks. ILNumerics ensures, no memory is left as garbage, once 
        /// such a scope block was left. Furthermore, it garantees, input arrays are kept alive during the execution of the block. By following these 
        /// <a href="http://ilnumerics.net/$GeneralRules.html" target="ILMain">simple rules</a>, ILNumerics is able to optimize the execution of the algorithm regarding 
        /// execution speed and memory footprint.</para></remarks>
        /// <example><para>The examples demonstrates a custom function in ILNumerics. It demonstrates the use of distinct array types in the function declaration and the use of 
        /// artificial scopes.</para>
        /// <code><![CDATA[ILRetArray<double> FreqPeaks(ILInArray<double> inData, ILOutArray<double> freq = null, double sampFreq = 44.1) { 
        ///
        ///    using (ILScope.Enter(inData)) {    
        ///             
        ///        ILArray<double> Data = check(inData); 
        ///        ILArray<double> retLength = min(ceil(Data.Length / 2.0 + 1), 5.0);   
        ///        ILArray<double> Window = stdWindowFunc(Data.Length);  
        ///        ILArray<double> magnitudes = abs(fft(Data * Window));  
        ///        magnitudes = magnitudes[r(0,end / 2 + 1)];  
        /// 
        ///        ILArray<double> indices = empty();  
        ///        ILArray<double> sorted = sort(magnitudes, indices, descending:true);  
        ///        if (!isnull(freq)) 
        ///            freq.a = (sampFreq / 2.0 / magnitudes.Length * indices)[r(0,retLength-1)];  
        ///        return magnitudes[r(0,retLength-1)];  
        ///    } 
        ///}]]></code>
        /// </example>
        public static IDisposable Enter(ILBaseArray inputArrays) {
            return new ILScope(inputArrays);
        }
        /// <summary>
        /// The threading context - individual for each thread
        /// </summary>
        internal static ILThreadingContext Context {
            get {
                ILThreadingContext ret = s_threadContext;
                if (ret == null) {
                    s_threadContext = new ILThreadingContext();
                    ret = s_threadContext; 
                }
                return ret; 
            }
        }

        internal int Count = 0;

        internal ILScope(ILBaseArray array) {
             if (!object.Equals(array,null)) {
                Stack<ILBaseArray> stack = Context.Arrays;
                int oldStackCount = stack.Count;
                if (array.EnterScope()) {
                    stack.Push(array);
                }
                Count = stack.Count - oldStackCount;
            }
            Context.Scopes.Push(this);
        }
        internal ILScope(params ILBaseArray[] inputArrays) {
            if (inputArrays != null) {
                Stack<ILBaseArray> stack = Context.Arrays;
                int oldStackCount = stack.Count;
                foreach (var a in inputArrays) {
                    if (!object.Equals(a, null)) {
                        if (a.EnterScope()) {
                            stack.Push(a);
                        }
                    }
                }
                Count = stack.Count - oldStackCount;
            }
            Context.Scopes.Push(this); 
            
        }
        #region IDisposable Members

        /// <summary>
        /// Dispose all arrays in this scope
        /// </summary>
        public void Dispose() {
            Stack<ILBaseArray> stack = Context.Arrays; 
            for (int i = Count; i-- > 0; ) {
                stack.Pop().LeaveScope();
            }
#if VERBOSE
            System.Diagnostics.Debug.Assert(object.Equals(Context.Scopes.Peek(), this));
            System.Diagnostics.Debug.WriteLine ("Leaving scope {0} - disposing {1} arrays",Context.Scopes.Peek().GetHashCode(),Count); 
#endif 
            Context.Scopes.Pop();
        }

        #endregion

    }
}
