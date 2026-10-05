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
using System.Diagnostics; 
using System.Threading; 
using System.Runtime.InteropServices; 
using System.Reflection; 
using ILNumerics;

namespace ILNumerics.Misc {
    /// <summary>
    /// This class will support the runtime evaluation of the current processort. This is preliminary work and not used by the current ILNumerics release.
    /// </summary>
    public class ILCPUID {

        #region attributes
        bool? m_supportsCPUID = null; 
        #endregion

        #region properties
        public bool Supported {
            get {
                if (!m_supportsCPUID.HasValue) {
                    checkSupport();
                }
                return m_supportsCPUID.Value; }
        }
        #endregion
        delegate uint cpuid_support_delegate();
        static uint cpuid_support_dummy() {
            return 0;
        }

        //[UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
        delegate void cpuid_delegate(int[] buffer);
        static void cpuid_dummy(int[] buffer) {
        }

        static byte[] cpuid_asmprog = new byte[]{
0x53,                 // push        ebx  
0x56,                 // push        esi  
0x8B, 0xF2,              // mov         esi,edx  
0x83, 0xC6, 0x08,           // add         esi,8  
0x8B, 0x06,              // mov         eax,dword ptr [esi]  
0x0F, 0xA2,              // cpuid  
0x89, 0x06,              // mov         dword ptr [esi],eax  
0x89, 0x5E, 0x04,           // mov         dword ptr [esi+4],ebx  
0x89, 0x4E, 0x08,           // mov         dword ptr [esi+8],ecx  
0x89, 0x56, 0x0C,           // mov         dword ptr [esi+0Ch],edx  
0x5E,                 // pop         esi  
0x5B,                 // pop         ebx  
0xC3

//0x55,                  // push        ebp  
//0x8B, 0xEC,               // mov         ebp,esp  
//0x50,                  // push        eax  
//0x56,                  // push        esi  
//0x8B, 0x75, 0x08,            // mov         esi,dword ptr [ebp+8]  
//0x8B, 0x06,               // mov         eax,dword ptr [esi]  
//0x0F, 0xA2,               // cpuid  
//0x89, 0x06,               // mov         dword ptr [esi],eax  
//0x89, 0x5E, 0x04,            // mov         dword ptr [esi+4],ebx  
//0x89, 0x4E, 0x08,            // mov         dword ptr [esi+8],ecx  
//0x89, 0x56, 0x0C,            // mov         dword ptr [esi+0Ch],edx  
//0x5E,                  // pop         esi  
//0x58,                  // pop         eax  
//0x8B, 0xE5,               // mov         esp,ebp  
//0x5D,                  // pop         ebp 
//0xC3
};

        static byte[] support_cpuid_asmprog = new byte[]{
 0x9C,                // pushfd  
 0x58,                // pop         eax  
 0x8B, 0xC8,             // mov         ecx,eax  
 0x35, 0x00, 0x00, 0x20, 0x00,    // xor         eax,200000h  
 0x50,                // push        eax  
 0x9D,                // popfd  
 0x9C,                // pushfd  
 0x58,                // pop         eax  
 0x33, 0xC1,             // xor         eax,ecx  
 0xC3
    };

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        [DllImport("kernel32")]
        private static extern bool VirtualFree(IntPtr lpAddress, UInt32 dwSize, UInt32 dwFreeType);

        [Flags()]
        public enum AllocationType : uint {
            COMMIT = 0x1000,
            RESERVE = 0x2000,
            RESET = 0x80000,
            LARGE_PAGES = 0x20000000,
            PHYSICAL = 0x400000,
            TOP_DOWN = 0x100000,
            WRITE_WATCH = 0x200000
        }

        [Flags()]
        public enum MemoryProtection : uint {
            EXECUTE = 0x10,
            EXECUTE_READ = 0x20,
            EXECUTE_READWRITE = 0x40,
            EXECUTE_WRITECOPY = 0x80,
            NOACCESS = 0x01,
            READONLY = 0x02,
            READWRITE = 0x04,
            WRITECOPY = 0x08,
            GUARD_Modifierflag = 0x100,
            NOCACHE_Modifierflag = 0x200,
            WRITECOMBINE_Modifierflag = 0x400
        }

        unsafe public ILCPUID() {
        }

        unsafe public void Query(int[] buffer) {
            // the actual cpuid request
            if (buffer == null || buffer.Length < 4) 
                throw new Exception("argument 'buffer' must have length >= 4"); 
            IntPtr instructions = default(IntPtr); 
            try {
                instructions = VirtualAlloc(IntPtr.Zero, new UIntPtr((uint)cpuid_asmprog.Length), AllocationType.COMMIT | AllocationType.RESERVE, MemoryProtection.EXECUTE_READWRITE);
                Marshal.Copy(cpuid_asmprog,0,instructions,cpuid_asmprog.Length); 
                
                cpuid_delegate cpuid = new cpuid_delegate(cpuid_dummy);
                Type funcType = typeof(Delegate);
                FieldInfo funcPointer = funcType.GetField("_methodPtr", BindingFlags.NonPublic | BindingFlags.Instance);
                funcPointer.SetValue(cpuid,instructions); 
                //fixed (int* buffer_fixed = buffer) {
                    cpuid(buffer); 
                //}

            } finally {
                if (instructions != IntPtr.Zero) {
                    VirtualFree(instructions, 0, 0x8000); // MEM_RELEASE
                    instructions = IntPtr.Zero;
                }
            }
        }

        unsafe private IntPtr checkSupport() {
            IntPtr instructions = IntPtr.Zero;
            try {
                //'write' the asm - must be in an instruction section! (data area would not work)

                instructions = VirtualAlloc(IntPtr.Zero, new UIntPtr((ulong)support_cpuid_asmprog.Length), AllocationType.COMMIT | AllocationType.RESERVE, MemoryProtection.EXECUTE_READWRITE);
                Marshal.Copy(support_cpuid_asmprog,0,instructions,support_cpuid_asmprog.Length); 

                Type funcType = typeof(Delegate);
                FieldInfo funcPointer = funcType.GetField("_methodPtr", BindingFlags.NonPublic | BindingFlags.Instance);

                // setup test for cpuid support
                cpuid_support_delegate natcpuid_support = new cpuid_support_delegate(cpuid_support_dummy);
                funcPointer.SetValue(natcpuid_support, instructions);
                if (natcpuid_support() == 0) {
                    Console.WriteLine("No support for cpuid!");
                    m_supportsCPUID = false;
                } else {
                    m_supportsCPUID = true;
                }
            } finally {
                if (instructions != IntPtr.Zero) {
                    VirtualFree(instructions, 0, 0x8000); // MEM_RELEASE
                    instructions = IntPtr.Zero;
                }
            }
            return instructions;
        }
    }
}
