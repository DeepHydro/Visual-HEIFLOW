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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace Tester {
    [System.Security.SecuritySafeCritical]
    internal class ILThreadConfig {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetCurrentThreadId();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GetProcessAffinityMask(IntPtr currentProcess, ref Int64 lpProcessAffinityMask, ref Int64 lpSystemAffinityMask);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetCurrentProcessorNumber();

        static void Main() {
            Int64 lpProcessAffinityMask = 0, lpSystemAffinityMask = 0;
            int i = 0, j = 0;
            Int64 procCount = Environment.ProcessorCount;
            Thread[] t = new Thread[procCount];

            IntPtr currentProcess = Process.GetCurrentProcess().Handle;
            Int64 currProcMask;

            GetProcessAffinityMask(currentProcess, ref lpProcessAffinityMask, ref lpSystemAffinityMask);

            Console.WriteLine("ProcessAffintityMask = {0}, SystemAffinityMask ={1}", lpProcessAffinityMask, lpSystemAffinityMask);

            for (i = 0; i < procCount; i++) {
                currProcMask = lpSystemAffinityMask & (1 << i);
                if (currProcMask >= 1) {
                    t[j] = new Thread(new ParameterizedThreadStart(DoWork));
                    object[] parameters = new object[] { currProcMask };
                    t[j++].Start(parameters);
                }
            }
            for (i = 0; i < j; i++) {
                t[i].Join();
            }
        }

        static void DoWork(object parameters) {

            object[] parameterArray = (object[])parameters;
            Int64 currProcMask = (Int64)parameterArray[0];

            foreach (ProcessThread pt in Process.GetCurrentProcess().Threads) {
                int utid = GetCurrentThreadId();
                if (utid == pt.Id) {
                    pt.ProcessorAffinity = (IntPtr)(currProcMask); // Set affinity for this thread to CPU #1
                    Thread.Sleep(10);
                    Console.WriteLine("Running on processor {0}", GetCurrentProcessorNumber());
                }
            }

        }
    }
}