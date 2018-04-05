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

//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.

using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Heiflow.Controls.WinForm
{
    internal static class NativeMethods
    {
        #region Delegates
        internal delegate bool EnumWindowsCallBack(IntPtr hWnd, int lParam);
        #endregion

        #region USER32

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        internal static extern IntPtr CreateWindowEx(
           uint dwExStyle,string lpClassName,string lpWindowName, uint dwStyle,int x,int y,        
           int nWidth,int nHeight,IntPtr hWndParent,IntPtr hMenu,IntPtr hInstance,IntPtr lpParam);
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
        [DllImport("user32.dll")]
        internal static extern bool SetDlgItemText(IntPtr hDlg, int nIDDlgItem, string lpString);
		[DllImport("User32.Dll")]
        public static extern int GetDlgCtrlID(IntPtr hWndCtl);
        [DllImport("user32.dll", SetLastError = true)][return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowInfo(HandleRef hwnd, out WINDOWINFO pwi);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowText(HandleRef hWnd, string lpString);
		[DllImport("User32.Dll")]
		public static extern void GetClassName(HandleRef hWnd, StringBuilder param, int length);
        [DllImport("user32.Dll")][return: MarshalAs(UnmanagedType.Bool)]        
        public static extern bool EnumChildWindows(HandleRef hWndParent, EnumWindowsCallBack lpEnumFunc, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(HandleRef hWndChild,HandleRef hWndNewParent);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, SetWindowPosFlags flags);
        [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(HandleRef hwnd, ref RECT rect);
        [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(HandleRef hwnd, ref RECT rect);
        [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("advapi32.dll", EntryPoint = "RegCreateKeyW")]
        public static extern int RegCreateKeyW([In] UIntPtr hKey, [In] [MarshalAs(UnmanagedType.LPWStr)] string lpSubKey, out IntPtr phkResult);

        [DllImport("advapi32.dll", EntryPoint = "RegOverridePredefKey")]
        public static extern int RegOverridePredefKey([In] UIntPtr hKey, [In] IntPtr hNewHKey);

        [DllImport("advapi32.dll", EntryPoint = "RegCloseKey")]
        public static extern int RegCloseKey([In] IntPtr hKey);
/*
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetForegeoundWindow(IntPtr hWnd);
 */ 
        #endregion
    }
}
