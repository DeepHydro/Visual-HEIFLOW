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
using System.Diagnostics; 
using System.Runtime.InteropServices; 

namespace ILNumerics.Misc {
    /// <summary>
    /// simple thread pool implementation - NOT THREAD SAFE !! 
    /// </summary>
    [System.Security.SecuritySafeCritical]
    internal partial class ILThreadPool {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GetProcessAffinityMask(IntPtr currentProcess, ref Int64 lpProcessAffinityMask, ref Int64 lpSystemAffinityMask);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetCurrentThreadId();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetCurrentProcessorNumber();

        private static ILThreadPool s_pool = new ILThreadPool(Settings.MaxNumberThreads - 1);
        public static ILThreadPool Pool {
            get {
                return s_pool; 
            }
        }
        private static ILPerformanceCounter s_performanceCounters; 
        private static object s_lock = new object(); 
        internal static bool QueueUserWorkItem(int id, Action<object> action, object data) {
            lock (s_lock) {
                ILThread thread = s_pool.m_threads[id];
                ILThreadPoolWorkItem workItem;
                workItem.Action = action;
                workItem.Data = data;
                thread.Queue(workItem);
                return true;
            }
        }

        private int m_maxNumberThreads;
        public int MaxNumberThreads {
            get { return m_maxNumberThreads; }
            set {
                lock (s_lock) {
                    while (m_maxNumberThreads > value) {
                        ILThread thread = m_threads[--m_maxNumberThreads];
                        thread.End();
                        m_threads.Remove(thread);
                    }
                    while (m_maxNumberThreads < value) {
                        m_threads.Add(new ILThread(m_maxNumberThreads));
                        m_maxNumberThreads++; 
                    }
                }
            }
        }
        private List<ILThread> m_threads = new List<ILThread>();

        public ILThreadPool(int maxThreads) {

            m_maxNumberThreads = maxThreads; 
            for (int i = 0; i < m_maxNumberThreads; i++) {
                m_threads.Add(new ILThread(i)); 
            }
            if (Settings.s_isHosted) {
                int targetMask = 8;
                foreach (ProcessThread pt in Process.GetCurrentProcess().Threads) {
                    int osThreadId = GetCurrentThreadId();
                    if (osThreadId == pt.Id) {
                        pt.ProcessorAffinity = (IntPtr)(targetMask);
                        break;
                    }
                }
                Thread.Sleep(10);
                Thread.BeginThreadAffinity();
            }
            if (Settings.s_measurePerformanceAtRuntime) {
                s_performanceCounters = new ILPerformanceCounter();
                s_performanceCounters.PCcurNumberThreadsSet(maxThreads);
            }
        }
        public static void Wait4Workers(ref int workerCount) {
            //var spinWait = new SpinWait();
            //while (workerCount > 0) {
            //    //Thread.MemoryBarrier(); 
            //    spinWait.SpinOnce();
            //}

            //System.Threading.SpinWait.SpinUntil(() => {
            //    Thread.MemoryBarrier();
            //    return workerCount <= 0;
            //});
            long msStart = Stopwatch.GetTimestamp();
            while (workerCount > 0) {
                System.Threading.Thread.SpinWait(8);
            }
            if (Settings.s_measurePerformanceAtRuntime) 
                s_performanceCounters.PCwaited4ThreadsSet(Stopwatch.GetTimestamp() - msStart); 
        }

        public struct ILThreadPoolWorkItem {
            public Action<object> Action;
            public object Data; 
        }

        private class ILThread {
            private int m_index; 
            private Thread m_thread;
            private AutoResetEvent m_resetEvent;
            private Queue<ILThreadPoolWorkItem> m_queue;
            private bool m_ending; 

            internal ILThread(int index) {
                m_index = index; 
                m_ending = false; 
                m_resetEvent = new AutoResetEvent(false);
                m_queue = new Queue<ILThreadPoolWorkItem>();
                m_thread = new Thread(WorkerFunc);
                m_thread.Priority = ThreadPriority.AboveNormal;
                m_thread.IsBackground = true; 
                m_thread.Start(); 
            }

            private int Index {
                get {
                    return m_index; 
                }
            }

            public void End() {
                lock (s_lock) {
                    if (m_queue.Count > 0)
                        throw new Exceptions.ILInvalidOperationException("invalid attempt to remove a thread with pending work items");
                    m_ending = true;
                    m_resetEvent.Set();
                }
            }
            public void Queue(ILThreadPoolWorkItem workItem) {
                m_queue.Enqueue(workItem);
                m_resetEvent.Set(); 
            }

            private void WorkerFunc() {
                if (Settings.s_isHosted) {
                    Int64 targetMask = (Index + 1) << (Index + 1);
                    // TODO: IMPROVE!! 
                    targetMask = 1; 
                    foreach (ProcessThread pt in Process.GetCurrentProcess().Threads) {
                        int osThreadId = GetCurrentThreadId();
                        if (osThreadId == pt.Id) {
                            pt.ProcessorAffinity = (IntPtr)(targetMask);
                            break;
                        }
                    }
                    Thread.Sleep(10);
                    //Thread.BeginThreadAffinity(); 
                }
                while (true) {
                    //SpinWait.SpinUntil(() => { return m_queue.Count > 0; }); 
                    m_resetEvent.WaitOne();
                    while (m_queue.Count > 0) {
                        //Thread.BeginThreadAffinity(); 
                        ILThreadPoolWorkItem workItem = m_queue.Dequeue();
                        workItem.Action(workItem.Data);
                        //Thread.EndThreadAffinity(); 
                    }
                    if (m_ending) break; 
                }

            }
        }


    }
}
