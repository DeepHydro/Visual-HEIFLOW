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
using System.Collections; 
using System.Collections.Generic;
using System.Text;
using ILNumerics.Exceptions; 
using ILNumerics.Data; 

namespace ILNumerics.Misc {
    /// <summary>
    /// Memory pool serving as temporary storage for System.Array objects
    /// </summary>
    /// <remarks>The pool reduces the pressure on the systems memory caused by larger objects.
    /// <para>Arrays created in ILNumerics will first try to reclaim their memory from this pool. If that 
    /// fails, the memory is allocated from the managed heap only.</para>
    /// <para>Disposed array objects register their underlying System.Array in the pool for 
    /// later reusing. The process is triggered by the ILNumerics memory management automatically.</para></remarks>
    internal partial class ILMemoryPoolInternal<T> : IILMemoryPool {

        internal enum MemoryPoolShrinkStrategies {
            Popularity, 
            Smallest
        }

        #region attributes
        private Dictionary<long,Stack<T[]>> m_pool; 
        private Dictionary<long,int> m_requests;
        private ILAVLTree m_lengths; 
        private Dictionary<long, string> m_profiler; 
        private long m_maxBytes; 
        private long m_curBytes; 
        private long m_minArrLength;
        private long m_reclaimedBytesCount;
        private long m_reclaimedObjectsCount;
        private int m_catchedOOMs; 
        private bool[] m_rateHistVals = new bool[100]; 
        private int m_rateHistPos = 0;
        private int m_shrinksCount = 0;  
        private int m_curObjectsCount = 0;
        private double m_autoIncreaseNewRequests = 1.1;
        private double m_maxRequestedLengthIncrease = 1.2;
        private double m_maxBytesIncreaseRate = 1.02;
        private double m_OOMdecreaseMaxBytesRate = 0.8;
        private Comparer<KeyValuePair<long, int>> m_comparer = new ILRequestsValueComparer(); 
        private static readonly int s_singleElementSize;
        private static readonly double s_shrinkPercent = .5;
        private static readonly double s_shrinkRequestLenghtIncreaseRate; 
        private static readonly string s_elementTypeName; 
        internal static ILMemoryPoolInternal<T> Pool; 
        internal static ILPerformanceCounter<T> s_performanceCounters = new ILPerformanceCounter<T>(); 
#if VERBOSE
        internal static string s_logfilename = String.Format("ILMemoryPool_VERBOSE_Log_{0}.log",typeof(T).Name); 
        internal static StringBuilder s_logBuilder = new StringBuilder("ILMemoryPoolInternal allocation history from " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + Environment.NewLine); 
#endif 
        #endregion

        #region properties
        /// <summary>
        /// factor used, to allow returned arrays to exceed the requested array length
        /// </summary>
        public double MaxRequestedLengthIncrease {
            get { return m_maxRequestedLengthIncrease; }
            set {
                m_maxRequestedLengthIncrease = value;
                if (Settings.s_measurePerformanceAtRuntime)
                    s_performanceCounters.PCAcceptLengthIncreaseSet((int)Math.Log10(m_maxRequestedLengthIncrease)); 
            }
        }
        /// <summary>
        /// minimum length of array objects for recognition in the pool (default: 80k)
        /// </summary>
        public long MinArrayLength {
            get { return m_minArrLength; }
            set { m_minArrLength = value; }
        }
        /// <summary>
        /// maximum size of the pool configured in bytes
        /// </summary>
        public long MaxBytes {
            get { return m_maxBytes; }
            set { 
                m_maxBytes = value;
                if (Settings.s_measurePerformanceAtRuntime) 
                    s_performanceCounters.PCmaximumPoolSizeSet(m_maxBytes); 
            }
        }
        /// <summary>
        /// percentage of allocation requests which could be successfully be completed
        /// </summary>
        public double SuccessRate {
            get  {
                int succ = 0; 
                foreach (bool b in m_rateHistVals) {
                    if (b) succ++; 
                }
                return succ / (double)m_rateHistVals.Length * 100d; 
            }
        }
        /// <summary>
        /// Number of reclaimed bytes since the pool exists
        /// </summary>
        /// <remarks>The counter will be reset by calls to <see cref="ILNumerics.Misc.ILMemoryPool.Reset(int, int)"/></remarks>
        public long ReclaimedBytesCount {
            get { return m_reclaimedBytesCount; }
        }
        /// <summary>
        /// Number of reclaimed objects since the pool exists
        /// </summary>
        /// <remarks>The counter will be reset by calls to <see cref="ILNumerics.Misc.ILMemoryPool.Reset(int, int)"/></remarks>
        public long ReclaimedObjectsCount {
            get { return m_reclaimedObjectsCount; }
        }
        #endregion

        #region constructors
        private ILMemoryPoolInternal() 
            : this (getDefaultMinArrayLength(),200) {} // (int)(Environment.WorkingSet / 1024.0 / 1024 / 10)) { }

        private ILMemoryPoolInternal(int MinArrayLength, int PoolSizeMB) {
            m_maxBytes = PoolSizeMB * 1024 * 1024; 
            m_minArrLength = MinArrayLength; 
            m_requests = new Dictionary<long,int>(); 
            m_pool = new Dictionary<long,Stack<T[]>>(); 
            m_lengths = new ILAVLTree(); 
            m_profiler = new Dictionary<long,string>(); 
            MaxRequestedLengthIncrease = 1.2; 
            ILMemoryPool.Pools.Add(typeof(T),this); 
        }

        static ILMemoryPoolInternal() {
            if (typeof(T).IsValueType) {
                T tmp = default(T);
                s_singleElementSize = Math.Max(System.Runtime.InteropServices.Marshal.SizeOf(tmp), 1);
            } else {
                s_singleElementSize = 4; 
            }
            Pool = new ILMemoryPoolInternal<T>();
            s_elementTypeName = typeof(T).Name; 

#if DEBUG   
            //System.Diagnostics.StackTrace.METHODS_TO_SKIP
#endif

#if VERBOSE
            // manage log file 
            System.IO.File.Delete(s_logfilename); 
#endif
        }

        #endregion

        #region public API
        /// <summary>
        /// Reset &amp; reconfigure the pool 
        /// </summary>
        /// <param name="MinArrayLength">Minimum length for array object to be stored inside the pool</param>
        /// <param name="PoolSizeMB">Overall size the memory pool consumes at most</param>
        /// <remarks>Reset will dispose all objects currently hold in the pool!</remarks>
        public void Reset ( long MinArrayLength, int PoolSizeMB ) {
            lock (this) {
                if (PoolSizeMB < 10) 
                    PoolSizeMB = 10; 
                m_maxBytes = PoolSizeMB * 1024 * 1024; 
                m_minArrLength = MinArrayLength; 
                DisposeContent(); 
                m_reclaimedBytesCount = 0; 
                m_reclaimedObjectsCount = 0; 
            }
        }
        /// <summary>
        /// Dispose all objects currently hold in the pool 
        /// </summary>
        /// <remarks>The pool get cleared and continues working with the same parameters after the call has finished. </remarks>
        public void DisposeContent() {
            lock (this) {
                //m_requests.Clear(); // lets keep the statistics
                m_pool.Clear(); 
                m_lengths.Clear();
                m_requests.Clear(); 
                m_curBytes = 0;
                m_curObjectsCount = 0;

                if (Settings.s_measurePerformanceAtRuntime) {
                    s_performanceCounters.PCoverallSizeSet(0);
                }
            }
        }
        /// <summary>
        /// Return or register an array object of value type in the pool that is not used anymore, i.e. free it
        /// </summary>
        /// <typeparam name="T">arbitrary element type</typeparam>
        /// <param name="arr">array</param>
        /// <remarks><para>In order to be stored in the pool, the array must meet the minimum array length and must fit into the global pool size.
        /// Null objects or empty arrays or array not suitable for the pool will be silently ignored.</para>
        /// <para>If the new array is too large to fit into the remaining pool space, the oldest objects in the pool will be released until the object can get registered.</para></remarks>
        public void Free(T[] arr) {
#if VERBOSE
            //System.Diagnostics.Debug.WriteLine("ILMemoryPoolInternal registering array: {0}[] {1}",typeof(T).Name, arr.Length);  
            Log(arr.Length,"REG"); 
#endif 
            if (arr == null || arr.Length == 0 || arr.Length < m_minArrLength)
                return; 
#if DEBUG 
                //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(); 
                //DebuggerTraceHelper.Output.Append(String.Format("\r\n{0}[{1}]-{2}\r\n{3}\r\n======================================"
                //    ,arr.ToString(), arr.Length, arr.GetHashCode() ,st.ToString()));
                //DebuggerTraceHelper.Output.Append(String.Format("=============================")); 
#endif
            if (!String.IsNullOrEmpty(Settings.s_memoryPoolProfileFileName)) {
                lock (m_profiler) {
                    int key = arr.GetHashCode();
                    if (m_profiler.ContainsKey(key))
                        m_profiler.Remove(key);
                }
            }
            // determine size of new array
            long newSize = arr.Length * s_singleElementSize + 8; // up to .NET 4.0
            if (newSize >= m_maxBytes) {
                m_maxBytes = newSize+1; 
                return; 
            }
            lock (this) {
                if (m_curBytes + newSize > m_maxBytes) 
                    Shrink(newSize); 
                // add the array to the pool 
                Stack<T[]> inPool; 
                if (m_pool.TryGetValue(arr.Length, out inPool)) {
                    // append to pool
                    System.Diagnostics.Debug.Assert(!inPool.Contains(arr)); 
                    inPool.Push(arr);
                } else {
                    // add new length to pool 
                    inPool = new Stack<T[]>(); 
                    inPool.Push(arr); 
                    m_pool.Add(arr.Length,inPool);     
                }
                m_lengths.Add(arr.Length);
                m_curObjectsCount++; 
                m_curBytes += newSize;
                if (Settings.s_measurePerformanceAtRuntime) {
                    s_performanceCounters.PCoverallSizeSet(m_curBytes);
                    s_performanceCounters.PCcurObjectsCountSet(m_curObjectsCount);
                }
            }
        }
        /// <summary>
        /// Request a System.Array instance (may not be cleared!)
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="length"><b>minimum</b> length of array</param>
        /// <returns>System.Array - either from the pool or a newly created array</returns>
        /// <remarks><para>If a suitable System.Array was found in the pool, this object is returned. 
        /// Otherwise a new array is created.</para>
        /// <para>There is no way of determining, if the array was reclaimed from pool or newly created! 
        /// If you must be sure, the element values are set to default(T), call the overloaded version 
        /// <see cref="ILNumerics.Misc.ILMemoryPool.New&lt;T&gt;(long, bool, out bool)"/> instead!</para>
        /// <para>If a new array could not get created due to an OutOfMemoryException, a garbage collection
        /// is triggered and the array is again requested from the pool. If this again fails, another attempt 
        /// to create the array is done. Exceptions may thrown from this last attempt are not catched and 
        /// therefore must be handled by the calling function.</para></remarks>
        public T[] New(long length) {
#if VERBOSE
            //System.Diagnostics.Debug.WriteLine(String.Format("Pool<{0}>: requesting length: {1}", typeof(T).Name, length));
            Log((int)length,"NEW"); 
#endif 
            if (length < m_minArrLength) {
                return new T[length];
            } else {
                T[] ret = null;
                lock (this) {
                    // try to find a suitable object in the pool first 
                    if ((ret = GetFromPool(length)) != null) {
                        int oldRequestCount = 0;
                        length = ret.Length;
                        m_requests.TryGetValue(length, out oldRequestCount);
                        m_requests[length] = ++oldRequestCount;
                        return ret;
                    }
                }
                // must create a new one
                try {
                    length = (long)(m_autoIncreaseNewRequests * length);
                    ret = new T[length];
                } catch (OutOfMemoryException) {
                    m_catchedOOMs++;
                    m_maxBytes = (long)(m_maxBytes * m_OOMdecreaseMaxBytesRate);
                    if (Settings.s_measurePerformanceAtRuntime) {
                        s_performanceCounters.PCOOMcatchedIncrement();
                    }
                    DisposeContent();
                }
                if (ret == null) {
                    ret = new T[length];
                }
                lock (this) {
                    int oldRequestCount = 0;
                    m_requests.TryGetValue(length, out oldRequestCount);
                    m_requests[length] = ++oldRequestCount;
                }
                return ret;
            }
        }
        /// <summary>
        /// Request a System.Array instance and optionally clear the array
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="length">length of array</param>
        /// <param name="clear">if true, the elements of the array returned are set to default(T).</param>
        /// <param name="iscleared">out paramater determining if the array returned has been cleared</param>
        /// <returns>System.Array - either from the pool or a newly created array</returns>
        /// <remarks><para>If a suitable System.Array was found in the pool, this object is returned. Otherwise a new array is created.</para>
        /// <para>If the <paramref name="clear">clear </paramref> parameter was set to false, the
        /// <paramref name="iscleared">iscleared</paramref> parameter can be used to determine, if the object
        /// was returnd from the pool and may need extra clearing.</para>
        /// <para>If a new array could not get created due to an OutOfMemoryException, a garbage 
        /// collection is triggered and the array is again requested from the pool. If this again failes, 
        /// another attempt to create the array is done. Exceptions eventually thrown from this last 
        /// attempt are not catched and given back to the callee.</para></remarks>
        public T[] New(long length, bool clear, out bool iscleared) {
#if VERBOSE
            //System.Diagnostics.Debug.Write(String.Format("Pool<{0}>: requesting length: {1}", typeof(T).Name, length));
            Log(length,"NEW"); 
#endif 
            T[] ret = null;
            lock (this) {
                // try to find a suitable object in the pool first 
                if (length >= m_minArrLength && (ret = GetFromPool(length)) != null) {
                    int oldRequestCount = 0;
                    length = ret.Length; 
                    m_requests.TryGetValue(length, out oldRequestCount);
                    m_requests[length] = ++oldRequestCount;
                    if (clear) {
                        for (int i = 0; i < length; i++) {
                            ret[i] = default(T);
                        }
                        iscleared = true;
                    } else {
                        iscleared = false;
                    }
                    return ret;
                }
            }
            // must create a new one
            try {
                length = (int)(m_autoIncreaseNewRequests * length);
                ret = new T[length];
            } catch (OutOfMemoryException) {
                DisposeContent();
                m_maxBytes = (long)(m_maxBytes * m_OOMdecreaseMaxBytesRate);
                m_catchedOOMs++;
                if (Settings.s_measurePerformanceAtRuntime) {
                    s_performanceCounters.PCOOMcatchedIncrement();
                }
            }
            if (ret == null) {
                ret = new T[length];
            }
            iscleared = true;
            lock (this) {
                int oldRequestCount = 0;
                length = ret.Length;
                m_requests.TryGetValue(length, out oldRequestCount);
                m_requests[length] = ++oldRequestCount;
            }
            return ret;
        }
        /// <summary>
        /// Give infos about pool state, optionally reset counters
        /// </summary>
        /// <param name="shortVersion">true: abbreviate infos to: current MB in Pool, reclaimed MB for livetime, reclaimed objects for livetime. False: give full info (see below)</param>
        /// <param name="reset">true: reset internal counter for reclaimed objects/ - bytes</param>
        /// <returns>infos about current pool state</returns>
        public string Info(bool shortVersion, bool reset) {
            if (shortVersion) {
                string s = String.Format("CurMB: {0}| CurObj: {1}| ReclMB: {2}| ReclObj: {3}| Rate: {4:G6}",m_curBytes/1024/1024,countObj(),m_reclaimedBytesCount/1024/1024,m_reclaimedObjectsCount,SuccessRate); 
                if (reset) {
                    m_reclaimedBytesCount = 0; 
                    m_reclaimedObjectsCount = 0; 
                }
                return s; 
            } else {
                StringBuilder sb = new StringBuilder(); 
                sb.Append("ILMemoryPool - "); 
                sb.Append(DateTime.Now.ToLongTimeString() + Environment.NewLine);
                string tmp; 
                lock (this) {
                    sb.Append(String.Format("CurMB: {0}| CurObj: {1}| ReclMB: {2}| ReclObj: {3}| Rate: {4:G6}",m_curBytes/1024/1024,countObj(),m_reclaimedBytesCount/1024/1024,m_reclaimedObjectsCount,SuccessRate) + Environment.NewLine); 
                    sb.Append(String.Format("OOM: {0}| Shrink: {1:}| LFact: {2:G2}| MaxMB: {3}\r\n",m_catchedOOMs, m_shrinksCount, m_maxRequestedLengthIncrease, m_maxBytes/1024/1024)); 
                    foreach (KeyValuePair<long,Stack<T[]>> item in m_pool) {
                        sb.Append(item.Key + ": "); 
                        foreach(Array a in item.Value) {
                            tmp = a.GetType().GetElementType().Name; 
                            if (char.IsNumber(tmp[tmp.Length-1])) {
                                tmp = new String(new char[]{tmp[0], tmp[tmp.Length-1]}); 
                            } else {
                                tmp = tmp.Substring(0,2);
                            }
                            sb.Append(tmp); 
                            sb.Append(" "); 
                        }
                        sb.Append(Environment.NewLine); 
                    }
                    if (reset) {
                        m_reclaimedBytesCount = 0; 
                        m_reclaimedObjectsCount = 0; 
                    }
                }
                return sb.ToString(); 
            }
        }
        /// <summary>
        /// Give extended infos about pool state
        /// </summary>
        /// <returns>Full info about current and reclaimed pool objects</returns>
        /// <remarks>For short version infos use the overloaded version <see cref="ILNumerics.Misc.ILMemoryPool.Info(bool)"/></remarks>
        public string Info() {
            return Info(false, false); 
        }
        /// <summary>
        /// Give infos about pool state, optionally reset counters
        /// </summary>
        /// <param name="shortVersion">true: abbreviate infos to: current MB in Pool, reclaimed MB for livetime, reclaimed objects for livetime. False: give full info (see below)</param>
        /// <returns>infos about current pool state</returns>
        public string Info(bool shortVersion) {
            return Info(shortVersion, false); 
        }
        #endregion

        #region private helper
#if VERBOSE
        private void Log( int length, string prefix ) {
            lock (this) {
                s_logBuilder.Append(
                    String.Format("{0},\t{1},\t{2},\t{3}" + Environment.NewLine
                                    , prefix
                                    , System.DateTime.Now.ToShortDateString()
                                    , System.DateTime.Now.ToLongTimeString() + "." + System.DateTime.Now.Millisecond.ToString()
                                    , length));
                if (s_logBuilder.Length > 10000) {
                    System.IO.File.AppendAllText(s_logfilename, s_logBuilder.ToString()); 
                    s_logBuilder.Clear();
                }
            }
        }
#endif
        private T[] GetFromPool(long length) {
            Stack<T[]> inPool; 
            T[] ret = null; 
            lock (this) {
                if (m_pool.TryGetValue(length, out inPool)) {
                    ret = inPool.Pop();
                    long newSize = (ret.Length * s_singleElementSize + 8);
                    m_curBytes -= newSize;
                    m_curObjectsCount--;
                    if (Settings.s_measurePerformanceAtRuntime) {
                        s_performanceCounters.PCcurObjectsCountSet(m_curObjectsCount);
                        s_performanceCounters.PCoverallSizeSet(m_curBytes);
                        s_performanceCounters.PCsuccessRateIncrement();
                        s_performanceCounters.PCsuccessRateBaseIncrement();
                        s_performanceCounters.PCreclaimedObjectsCountIncrement();
                    }
                    if (inPool.Count == 0) {
                        m_pool.Remove(length);
                        m_lengths.Remove(length);
                        m_requests.Remove(length);
                    }
                    m_reclaimedObjectsCount += 1;
                    m_reclaimedBytesCount += newSize;
                    m_rateHistVals[m_rateHistPos++] = true;
                    if (m_rateHistPos >= m_rateHistVals.Length)
                        m_rateHistPos = 0;
                } else {
                    // try to get next larger array 
                    long largerLength = m_lengths.Next(length);
                    if (largerLength >= 0 && largerLength <= length * m_maxRequestedLengthIncrease) {
                        return GetFromPool(largerLength); 
                    }
                }
            }
            if (ret == null) {
                m_rateHistVals[m_rateHistPos++] = false;
                if (Settings.s_measurePerformanceAtRuntime) {
                    s_performanceCounters.PCsuccessRateBaseIncrement();
                }
                if (m_rateHistPos >= m_rateHistVals.Length) 
                    m_rateHistPos = 0; 
            } else if (!String.IsNullOrEmpty(Settings.s_memoryPoolProfileFileName)) {
                lock (m_profiler) {
                    int key = ret.GetHashCode();
                    m_profiler[key] = s_elementTypeName + "[" + length + "] - " + new System.Diagnostics.StackTrace(4).ToString();
                    if (m_profiler.Count > 500) {
                        //throw new Exception("The profiler status indicates the existence of a memory leak! Check the stack traces for common functions requesting an array which is never returnd to the pool!");
                        System.IO.File.WriteAllText(Settings.s_memoryPoolProfileFileName, 
                                String.Join(Environment.NewLine, m_profiler.Values)); 
                        m_profiler.Clear(); 
                    } 
                }
            }
            return ret; 
        }
        private int countObj() {
            int ret = 0; 
            lock (this)
            foreach (KeyValuePair<long,Stack<T[]>> item in m_pool) {
                foreach(Array a in item.Value) {
                    ret ++; 
                }
            }
            return ret; 
        }
        private static int getDefaultMinArrayLength() {
            T[] tmp = new T[0];
            if (tmp is double[]) {
                return 500; 
            } else {
                return 70 * 1024 / s_singleElementSize; 
            }
        }
        /// <summary>
        /// shrink the pool's content to SHRINK_PERCENT of the maximum pool size or at the size, suitable to store requestLen
        /// </summary>
        /// <param name="requestLen">minimum length to make available (bytes)</param>
        private void Shrink(long requestLen) {
            try {
                long targetSize = Math.Min(m_maxBytes - requestLen, (long)(s_shrinkPercent * m_maxBytes));
                if (m_curBytes <= targetSize)
                    return;
                if (Settings.s_measurePerformanceAtRuntime) {
                    s_performanceCounters.PCshrinksCountIncrement();
                } m_shrinksCount++;
                // recalculate new parameters
                m_maxBytes = (long)(m_maxBytesIncreaseRate * m_maxBytes);
                if (m_pool.Count <= 1) {
                    MaxRequestedLengthIncrease /= 1.02; 
                } else {
                    MaxRequestedLengthIncrease *= 1.02; 
                }

                // first clear all arrays in pool, which were not requested
                List<int> keys4Remove = new List<int>();
                foreach (int key in m_pool.Keys) {
                    if (!m_requests.ContainsKey(key)) {
                        keys4Remove.Add(key);
                    }
                }
                foreach (int key in keys4Remove) {
                    int size = key * s_singleElementSize + 8;
                    Stack<T[]> arrays = m_pool[key];
                    while (arrays.Count > 0) {
                        arrays.Pop();
                        m_curBytes -= size;
                        m_curObjectsCount--;
                        //if (m_curBytes < targetSize) return; 
                    }
                    if (arrays.Count == 0) {
                        m_requests.Remove(key);
                        m_pool.Remove(key);
                        m_lengths.Remove(key);
                    }
                }
                if (m_curBytes < targetSize) {
                    if (Settings.s_measurePerformanceAtRuntime) {
                        s_performanceCounters.PCoverallSizeSet(m_curBytes);
                        s_performanceCounters.PCcurObjectsCountSet(m_curObjectsCount);
                    }
                    return;
                }
                // sort requests statistics by its values 

                List<KeyValuePair<long, int>> sorted = new List<KeyValuePair<long, int>>(m_requests);
                sorted.Sort(m_comparer);
                foreach (KeyValuePair<long, int> item in sorted) {
                    long curKey = item.Key;
                    Stack<T[]> inPool;
                    if (!m_pool.TryGetValue(curKey, out inPool)) {
                        continue;
                    }
                    while (inPool.Count > 0 && m_curBytes > targetSize) {
                        inPool.Pop();
                        m_curBytes -= ((long)curKey * s_singleElementSize + 8);
                        m_curObjectsCount--;
                    }
                    if (inPool.Count == 0) {
                        m_lengths.Remove(curKey);
                        m_pool.Remove(curKey);
                        m_requests.Remove(curKey);
                    }
                    if (m_curBytes <= targetSize) {
                        if (Settings.s_measurePerformanceAtRuntime) {
                            s_performanceCounters.PCoverallSizeSet(m_curBytes);
                            s_performanceCounters.PCcurObjectsCountSet(m_curObjectsCount);
                        }
                        return;
                    }
                }
                if (Settings.s_measurePerformanceAtRuntime) {
                    s_performanceCounters.PCoverallSizeSet(m_curBytes);
                    s_performanceCounters.PCcurObjectsCountSet(m_curObjectsCount);
                }
            } finally {
                m_requests.Clear(); 
            }
        }
        #endregion

    }
}

