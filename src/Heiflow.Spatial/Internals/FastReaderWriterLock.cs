//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

#if !MONO && !PocketPC
#define UseFastResourceLock
#endif

namespace Heiflow.Spatial.Internals
{
   using System;
   using System.Threading;
#if !MONO
   using System.Runtime.InteropServices;
#endif

   /// <summary>
   /// custom ReaderWriterLock
   /// in Vista and later uses integrated Slim Reader/Writer (SRW) Lock
   /// http://msdn.microsoft.com/en-us/library/aa904937(VS.85).aspx
   /// http://msdn.microsoft.com/en-us/magazine/cc163405.aspx#S2
   /// </summary>
   public sealed class FastReaderWriterLock
   {
#if !MONO
      private static class NativeMethods
      {
         // Methods
         [DllImport("Kernel32", ExactSpelling = true)]
         internal static extern void AcquireSRWLockExclusive(ref IntPtr srw);
         [DllImport("Kernel32", ExactSpelling = true)]
         internal static extern void AcquireSRWLockShared(ref IntPtr srw);
         [DllImport("Kernel32", ExactSpelling = true)]
         internal static extern void InitializeSRWLock(out IntPtr srw);
         [DllImport("Kernel32", ExactSpelling = true)]
         internal static extern void ReleaseSRWLockExclusive(ref IntPtr srw);
         [DllImport("Kernel32", ExactSpelling = true)]
         internal static extern void ReleaseSRWLockShared(ref IntPtr srw);
      }

      IntPtr LockSRW;

      public FastReaderWriterLock()
      {
         if(UseNativeSRWLock)
         {
            NativeMethods.InitializeSRWLock(out this.LockSRW);
         }
         else
         {
#if UseFastResourceLock
            pLock = new Common.Threading.FastResourceLock();
#endif
         }
      }

#if UseFastResourceLock
      ~FastReaderWriterLock()
      {
         if(pLock != null)
         {
            pLock.Dispose();
            pLock = null;
         }
      }

      Common.Threading.FastResourceLock pLock;
#endif
#endif

      static readonly bool UseNativeSRWLock = Stuff.IsRunningOnVistaOrLater() && IntPtr.Size == 4; // works only in 32-bit mode, any ideas on native 64-bit support? 

#if !UseFastResourceLock
      Int32 busy = 0;
      Int32 readCount = 0;
#endif

      public void AcquireReaderLock()
      {
#if !MONO
         if(UseNativeSRWLock)
         {
            NativeMethods.AcquireSRWLockShared(ref LockSRW);
         }
         else
#endif
         {
#if UseFastResourceLock
            pLock.AcquireShared();
#else
            Thread.BeginCriticalRegion();

            while(Interlocked.CompareExchange(ref busy, 1, 0) != 0)
            {
               Thread.Sleep(1);
            }

            Interlocked.Increment(ref readCount);

            // somehow this fix deadlock on heavy reads
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);

            Interlocked.Exchange(ref busy, 0);
#endif
         }
      }

      public void ReleaseReaderLock()
      {
#if !MONO
         if(UseNativeSRWLock)
         {
            NativeMethods.ReleaseSRWLockShared(ref LockSRW);
         }
         else
#endif
         {
#if UseFastResourceLock
            pLock.ReleaseShared();
#else
            Interlocked.Decrement(ref readCount);
            Thread.EndCriticalRegion();
#endif
         }
      }

      public void AcquireWriterLock()
      {
#if !MONO
         if(UseNativeSRWLock)
         {
            NativeMethods.AcquireSRWLockExclusive(ref LockSRW);
         }
         else
#endif
         {
#if UseFastResourceLock
            pLock.AcquireExclusive();
#else
            Thread.BeginCriticalRegion();

            while(Interlocked.CompareExchange(ref busy, 1, 0) != 0)
            {
               Thread.Sleep(1);
            }

            while(Interlocked.CompareExchange(ref readCount, 0, 0) != 0)
            {
               Thread.Sleep(1);
            }
#endif
         }
      }

      public void ReleaseWriterLock()
      {
#if !MONO
         if(UseNativeSRWLock)
         {
            NativeMethods.ReleaseSRWLockExclusive(ref LockSRW);
         }
         else
#endif
         {
#if UseFastResourceLock
            pLock.ReleaseExclusive();
#else
            Interlocked.Exchange(ref busy, 0);
            Thread.EndCriticalRegion();
#endif
         }
      }
   }
}
