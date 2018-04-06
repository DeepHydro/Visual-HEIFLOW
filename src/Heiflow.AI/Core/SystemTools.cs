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

namespace  Heiflow.AI
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Set of systems tools.
    /// </summary>
    /// 
    /// <remarks><para>The class is a container of different system tools, which are used
    /// across the framework. Some of these tools are platform specific, so their
    /// implementation is different on different platform, like .NET and Mono.</para>
    /// </remarks>
    /// 
    public static class SystemTools
    {
        /// <summary>
        /// Copy block of unmanaged memory.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="src">Source pointer.</param>
        /// <param name="count">Memory block's length to copy.</param>
        /// 
        /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
        /// 
        /// <remarks><para>This function is required because of the fact that .NET does
        /// not provide any way to copy unmanaged blocks, but provides only methods to
        /// copy from unmanaged memory to managed memory and vise versa.</para></remarks>
        ///
        public static IntPtr CopyUnmanagedMemory( IntPtr dst, IntPtr src, int count )
        {
            unsafe
            {
                CopyUnmanagedMemory( (byte*) dst.ToPointer( ), (byte*) src.ToPointer( ), count );
            }
            return dst;
        }

        /// <summary>
        /// Copy block of unmanaged memory.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="src">Source pointer.</param>
        /// <param name="count">Memory block's length to copy.</param>
        /// 
        /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
        /// 
        /// <remarks><para>This function is required because of the fact that .NET does
        /// not provide any way to copy unmanaged blocks, but provides only methods to
        /// copy from unmanaged memory to managed memory and vise versa.</para></remarks>
        /// 
        public static unsafe byte* CopyUnmanagedMemory( byte* dst, byte* src, int count )
        {
#if !MONO
            return memcpy( dst, src, count );
#else
            int countUint = count >> 2;
            int countByte = count & 3;

            uint* dstUint = (uint*) dst;
            uint* srcUint = (uint*) src;

            while ( countUint-- != 0 )
            {
                *dstUint++ = *srcUint++;
            }

            byte* dstByte = (byte*) dstUint;
            byte* srcByte = (byte*) srcUint;

            while ( countByte-- != 0 )
            {
                *dstByte++ = *srcByte++;
            }
            return dst;
#endif
        }

        /// <summary>
        /// Fill memory region with specified value.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="filler">Filler byte's value.</param>
        /// <param name="count">Memory block's length to fill.</param>
        /// 
        /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
        /// 
        public static IntPtr SetUnmanagedMemory( IntPtr dst, int filler, int count )
        {
            unsafe
            {
                SetUnmanagedMemory( (byte*) dst.ToPointer( ), filler, count );
            }
            return dst;
        }

        /// <summary>
        /// Fill memory region with specified value.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="filler">Filler byte's value.</param>
        /// <param name="count">Memory block's length to fill.</param>
        /// 
        /// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
        /// 
        public static unsafe byte* SetUnmanagedMemory( byte* dst, int filler, int count )
        {
#if !MONO
            return memset( dst, filler, count );
#else
            int countUint = count >> 2;
            int countByte = count & 3;

            byte fillerByte = (byte) filler;
            uint fiilerUint = (uint) filler | ( (uint) filler << 8 ) |
                                              ( (uint) filler << 16 );// |
                                              //( (uint) filler << 24 );

            uint* dstUint = (uint*) dst;

            while ( countUint-- != 0 )
            {
                *dstUint++ = fiilerUint;
            }

            byte* dstByte = (byte*) dstUint;

            while ( countByte-- != 0 )
            {
                *dstByte++ = fillerByte;
            }
            return dst;
#endif
        }


#if !MONO
        // Win32 memory copy function
        [DllImport( "ntdll.dll" )]
        private static unsafe extern byte* memcpy(
            byte* dst,
            byte* src,
            int count );
        // Win32 memory set function
        [DllImport( "ntdll.dll" )]
        private static unsafe extern byte* memset(
            byte* dst,
            int filler,
            int count );
#endif
    }
}
