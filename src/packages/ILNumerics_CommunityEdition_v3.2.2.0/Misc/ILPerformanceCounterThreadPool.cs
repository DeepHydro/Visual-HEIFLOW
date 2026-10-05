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

    internal partial class ILThreadPool  {
        internal class ILPerformanceCounter {
            // some constants (may get localized)
            private static readonly string PERFORMANCECOUNTER_CATEGORY_NAME = "ILNumerics Thread Pool";
            private static readonly string PERFORMANCECOUNTER_CATEGORY_NAME_HELP = "Monitors treading related data.";
            private static readonly string PERFORMANCECOUNTER_CURRENT_NUMBER_THREADS = "Number of worker threads";
            private static readonly string PERFORMANCECOUNTER_CURRENT_NUMBER_THREADS_HELP = "Number of threads currently active for parallel operations.";
            private static readonly string PERFORMANCECOUNTER_TIME_WAITING_FOR_THREADS = "Time waited for other threads";
            private static readonly string PERFORMANCECOUNTER_TIME_WAITING_FOR_THREADS_HELP = "Timespan in milliseconds, which the main thread was waiting for worker threads to finish the last operation.";
            
            private bool m_countingActive = false;
            public PerformanceCounter m_PCcurThreadsCount; 
            public PerformanceCounter m_PCwait4Threads; 

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
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_CURRENT_NUMBER_THREADS, PERFORMANCECOUNTER_CURRENT_NUMBER_THREADS_HELP, PerformanceCounterType.NumberOfItems32));
                        ccdc.Add(new CounterCreationData(PERFORMANCECOUNTER_TIME_WAITING_FOR_THREADS, PERFORMANCECOUNTER_TIME_WAITING_FOR_THREADS_HELP, PerformanceCounterType.NumberOfItems64));
                        
                        PerformanceCounterCategory.Create(PERFORMANCECOUNTER_CATEGORY_NAME, PERFORMANCECOUNTER_CATEGORY_NAME_HELP
                                                        ,PerformanceCounterCategoryType.SingleInstance,ccdc); 
                    } catch (System.Security.SecurityException) { }
                }
                m_countingActive = true; 
                m_PCcurThreadsCount = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME, PERFORMANCECOUNTER_CURRENT_NUMBER_THREADS, false);
                m_PCwait4Threads = new PerformanceCounter(PERFORMANCECOUNTER_CATEGORY_NAME, PERFORMANCECOUNTER_TIME_WAITING_FOR_THREADS, false);

                m_PCcurThreadsCount.RawValue = 0;
                m_PCwait4Threads.RawValue = 0; 

            }
            public void PCcurNumberThreadsSet(int val) {
                if (m_PCcurThreadsCount != null)
                    m_PCcurThreadsCount.RawValue = val;
            }
            public void PCwaited4ThreadsSet(long value) {
                if (m_PCwait4Threads != null)
                    m_PCwait4Threads.RawValue = value;
            }
        }
    }
}

