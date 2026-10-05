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
using System.Diagnostics; 

namespace ILNumerics.Misc {

    internal partial class ILMemoryPoolInternal<T> : IILMemoryPool {
        internal class ILPerformanceCounter<Ti> {
            // some constants (may get localized)
            private static readonly string PERFORMANCECOUNTER_CATEGORY_NAME = "ILNumerics Memory Pool";
            private static readonly string PERFORMANCECOUNTER_CATEGORY_NAME_HELP = "Monitors usage statistics for the ILNumerics Memory Pool";
            private static readonly string PERFORMANCECOUNTER_OVERALLSIZE = "Overall Bytes";
            private static readonly string PERFORMANCECOUNTER_OVERALLSIZE_HELP = "Sum of the sizes of all arrays currently hold in the pool.";
            private static readonly string PERFORMANCECOUNTER_OOMCATCHED = "Number of catched OOMs";
            private static readonly string PERFORMANCECOUNTER_OOMCATCHED_HELP = "The overall number of OutOfMemoryExceptions catched since the application started.";
            private static readonly string PERFORMANCECOUNTER_SUCCESS_RATE = "Success Rate %";
            private static readonly string PERFORMANCECOUNTER_SUCCESS_RATE_HELP = "Gives the percentage of successfully reused arrays for the last 100 requests.";
            private static readonly string PERFORMANCECOUNTER_CUR_OBJECTS_COUNT = "Current Objects Count";
            private static readonly string PERFORMANCECOUNTER_CUR_OBJECTS_COUNT_HELP = "Number of arrays of that type currently hold in the memory pool";
            private static readonly string PERFORMANCECOUNTER_REC_OBJECTS_COUNT = "Reclaimed Objects Count";
            private static readonly string PERFORMANCECOUNTER_REC_OBJECTS_COUNT_HELP = "Overall count of objects which could sucessfully get reclaimed from the pool.";
            private static readonly string PERFORMANCECOUNTER_MAX_POOL_SIZE = "Max Pool Size";
            private static readonly string PERFORMANCECOUNTER_MAX_POOL_SIZE_HELP = "The maximum size the pool currently is allowed to grow before it gets shrinked.";
            private static readonly string PERFORMANCECOUNTER_SHRINK_COUNT = "Shrink Count";
            private static readonly string PERFORMANCECOUNTER_SHRINK_COUNT_HELP = "Overall number of times the pool had to be shrinked because of the size getting to large or OOM exceptions being thrown.";
            private static readonly string PERFORMANCECOUNTER_DISPOSED_HIGH = "Disposed large arrays";
            private static readonly string PERFORMANCECOUNTER_DISPOSED_HIGH_HELP = "number of disposed large arrays which possibly would be stored in the pool";
            private static readonly string PERFORMANCECOUNTER_LENGTH_FACTOR = "Lengthening Factor (Log.)";
            private static readonly string PERFORMANCECOUNTER_LENGTH_FACTOR_HELP = "Current logarithmic setting of the relative length an array will be considered 'matching' on incoming requests";
            private bool m_countingActive = false;
            public PerformanceCounter m_PCcurObjectsCount; 
            public PerformanceCounter m_PCoverallSize; 
            public PerformanceCounter m_PCOOMcatched; 
            public PerformanceCounter m_PCsuccessRate; 
            public PerformanceCounter m_PCsuccessRateBase; 
            public PerformanceCounter m_PCreclaimedObjectsCount; 
            public PerformanceCounter m_PCmaximumPoolSize; 
            public PerformanceCounter m_PCshrinksCount; 
            public PerformanceCounter m_PCDisposedHigh; 
            public PerformanceCounter m_PCacceptLengthIncreaseRate; 

            public ILPerformanceCounter() {
                if (!Settings.s_measurePerformanceAtRuntime) return; 
                // check if the category is installed 
                if (!PerformanceCounterCategory.Exists(PERFORMANCECOUNTER_CATEGORY_NAME)) {
                    // even if we handle to install the category, the counters will be
                    // not available immediately. So we disable them for this session. 
                    m_countingActive = false; 
                    try {
                        // try to install 
                        CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_CUR_OBJECTS_COUNT, PERFORMANCECOUNTER_CUR_OBJECTS_COUNT_HELP, PerformanceCounterType.NumberOfItems32));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_MAX_POOL_SIZE, PERFORMANCECOUNTER_MAX_POOL_SIZE_HELP, PerformanceCounterType.NumberOfItems64));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_OOMCATCHED, PERFORMANCECOUNTER_OOMCATCHED_HELP, PerformanceCounterType.NumberOfItems32));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_OVERALLSIZE, PERFORMANCECOUNTER_OVERALLSIZE_HELP, PerformanceCounterType.NumberOfItems64));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_REC_OBJECTS_COUNT, PERFORMANCECOUNTER_REC_OBJECTS_COUNT_HELP, PerformanceCounterType.NumberOfItems64));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_SHRINK_COUNT, PERFORMANCECOUNTER_SHRINK_COUNT_HELP, PerformanceCounterType.NumberOfItems32));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_SUCCESS_RATE, PERFORMANCECOUNTER_SUCCESS_RATE_HELP, PerformanceCounterType.RawFraction));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_SUCCESS_RATE+ "Base", PERFORMANCECOUNTER_SUCCESS_RATE_HELP, PerformanceCounterType.RawBase));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_DISPOSED_HIGH, PERFORMANCECOUNTER_DISPOSED_HIGH_HELP, PerformanceCounterType.NumberOfItems64));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_LENGTH_FACTOR, PERFORMANCECOUNTER_LENGTH_FACTOR_HELP, PerformanceCounterType.NumberOfItems32));
                        
                        PerformanceCounterCategory.Create(PERFORMANCECOUNTER_CATEGORY_NAME, PERFORMANCECOUNTER_CATEGORY_NAME_HELP
                                                        ,PerformanceCounterCategoryType.MultiInstance,ccdc); 
                    } catch (System.Security.SecurityException) { }
                } else {
                    m_countingActive = true; 
                    // init performance counters for this type 
                    string currentGenericElementType = " <" + typeof(Ti).Name + ">"; 
                    m_PCcurObjectsCount  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_CUR_OBJECTS_COUNT, currentGenericElementType, false); 
                    m_PCmaximumPoolSize  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_MAX_POOL_SIZE, currentGenericElementType, false); 
                    m_PCOOMcatched  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_OOMCATCHED, currentGenericElementType, false); 
                    m_PCoverallSize  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_OVERALLSIZE, currentGenericElementType, false); 
                    m_PCreclaimedObjectsCount  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_REC_OBJECTS_COUNT, currentGenericElementType, false); 
                    m_PCshrinksCount  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_SHRINK_COUNT, currentGenericElementType, false); 
                    m_PCsuccessRate  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_SUCCESS_RATE, currentGenericElementType, false); 
                    m_PCsuccessRateBase  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_SUCCESS_RATE+"Base", currentGenericElementType, false); 
                    m_PCDisposedHigh  = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME,PERFORMANCECOUNTER_DISPOSED_HIGH, currentGenericElementType, false);
                    m_PCacceptLengthIncreaseRate = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME, PERFORMANCECOUNTER_LENGTH_FACTOR, currentGenericElementType, false); 
                
                    m_PCcurObjectsCount.RawValue = 0;   
                    m_PCmaximumPoolSize.RawValue = 0;   
                    m_PCOOMcatched.RawValue = 0;   
                    m_PCoverallSize.RawValue = 0;   
                    m_PCreclaimedObjectsCount.RawValue = 0;   
                    m_PCshrinksCount.RawValue = 0;   
                    m_PCsuccessRate.RawValue = 0;   
                    m_PCsuccessRateBase.RawValue = 0;  
                    m_PCDisposedHigh.RawValue = 0; 
                    m_PCacceptLengthIncreaseRate.RawValue = 0; 

                }
            }
            public void PCcurObjectsCountSet(int val) {
                if (m_PCcurObjectsCount != null)
                    m_PCcurObjectsCount.RawValue = val;
            }
            public void PCDisposedHighIncrement() {
                if (m_PCDisposedHigh != null)
                    m_PCDisposedHigh.Increment();
            }
            public void PCAcceptLengthIncreaseSet(long value) {
                if (m_PCacceptLengthIncreaseRate != null)
                    m_PCacceptLengthIncreaseRate.RawValue = value;
            }
            public void PCmaximumPoolSizeSet(long value) {
                if (m_PCmaximumPoolSize != null)
                    m_PCmaximumPoolSize.RawValue = value;
            }
            public void PCOOMcatchedIncrement() {
                if (m_PCOOMcatched != null)
                    m_PCOOMcatched.Increment();
            }
            public void PCreclaimedObjectsCountIncrement() {
                if (m_PCreclaimedObjectsCount != null)
                    m_PCreclaimedObjectsCount.Increment();
            }
            public void PCshrinksCountIncrement() {
                if (m_PCshrinksCount != null)
                    m_PCshrinksCount.Increment();
            }
            public void PCsuccessRateIncrement() {
                if (m_PCsuccessRate != null)
                    m_PCsuccessRate.Increment();
            }
            public void PCsuccessRateBaseIncrement() {
                if (m_PCsuccessRateBase != null)
                    m_PCsuccessRateBase.Increment();
            }
            internal void PCoverallSizeSet(long curBytes) {
                if (m_PCoverallSize != null) {
                    m_PCoverallSize.RawValue = curBytes; 
                }
            }
        }
    }
}

