using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Native
{
    public static  class NativeMethods
    {
        public const int NOREPOSITION = 0x200;

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateRectRgn(int left, int top, int right, int bottom);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bRedraw);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);
  
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("dwmapi.dll")]
        public static extern bool DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [StructLayout(LayoutKind.Sequential)]             
        public struct NCCALCSIZE_PARAMS
        {
            public RECT rect0, rect1, rect2;                 
            public IntPtr lppos;
        }

        [Flags]
        public enum SWP
        {
            ASYNCWINDOWPOS = 0x4000,
            DEFERERASE = 0x2000,
            DRAWFRAME = 0x20,
            FRAMECHANGED = 0x20,
            HIDEWINDOW = 0x80,
            NOACTIVATE = 0x10,
            NOCOPYBITS = 0x100,
            NOMOVE = 2,
            NOOWNERZORDER = 0x200,
            NOREDRAW = 8,
            NOREPOSITION = 0x200,
            NOSENDCHANGING = 0x400,
            NOSIZE = 1,
            NOZORDER = 4,
            SHOWWINDOW = 0x40
        }

        public enum WM
        {
            ACTIVATE = 6,
            APP = 0x8000,
            CLOSE = 0x10,
            CREATE = 1,
            DESTROY = 2,
            DWMCOMPOSITIONCHANGED = 0x31e,
            ENABLE = 10,
            ERASEBKGND = 20,
            GETDLGCODE = 0x87,
            GETTEXT = 13,
            GETTEXTLENGTH = 14,
            KILLFOCUS = 8,
            MOVE = 3,
            NCACTIVATE = 0x86,
            NCCALCSIZE = 0x83,
            NCCREATE = 0x81,
            NCDESTROY = 130,
            NCHITTEST = 0x84,
            NCLBUTTONDBLCLK = 0xa3,
            NCLBUTTONDOWN = 0xa1,
            NCLBUTTONUP = 0xa2,
            NCMBUTTONDBLCLK = 0xa9,
            NCMBUTTONDOWN = 0xa7,
            NCMBUTTONUP = 0xa8,
            NCMOUSEMOVE = 160,
            NCPAINT = 0x85,
            NCRBUTTONDBLCLK = 0xa6,
            NCRBUTTONDOWN = 0xa4,
            NCRBUTTONUP = 0xa5,
            NULL = 0,
            PAINT = 15,
            QUERYENDSESSION = 0x11,
            QUERYOPEN = 0x13,
            QUIT = 0x12,
            SETFOCUS = 7,
            SETICON = 0x80,
            SETREDRAW = 11,
            SETTEXT = 12,
            SIZE = 5,
            SYNCPAINT = 0x88,
            SYSCHAR = 0x106,
            SYSCOLORCHANGE = 0x15,
            SYSCOMMAND = 0x112,
            SYSDEADCHAR = 0x107,
            SYSKEYDOWN = 260,
            SYSKEYUP = 0x105,
            USER = 0x400,
            WINDOWPOSCHANGED = 0x47,
            WINDOWPOSCHANGING = 70,
            MSG794=794
        }

        public enum HT
        {
            BORDER = 0x12,
            BOTTOM = 15,
            BOTTOMLEFT = 0x10,
            BOTTOMRIGHT = 0x11,
            CAPTION = 2,
            CLIENT = 1,
            CLOSE = 20,
            ERROR = -2,
            GROWBOX = 4,
            HELP = 0x15,
            HSCROLL = 6,
            LEFT = 10,
            MAXBUTTON = 9,
            MENU = 5,
            MINBUTTON = 8,
            NOWHERE = 0,
            OBJECT = 0x13,
            RIGHT = 11,
            SYSMENU = 3,
            TOP = 12,
            TOPLEFT = 13,
            TOPRIGHT = 14,
            TRANSPARENT = -1,
            VSCROLL = 7
        }


        public static IntPtr GetWindowLongPtr(IntPtr hwnd, GWL nIndex)
        {
            if (8 == IntPtr.Size)
            {
                return GetWindowLongPtr64(hwnd, nIndex);
            }
            return GetWindowLongPtr32(hwnd, nIndex);
        }

        public enum GWL
        {
            EXSTYLE = -20,
            HINSTANCE = -6,
            HWNDPARENT = -8,
            ID = -12,
            STYLE = -16,
            USERDATA = -21,
            WNDPROC = -4
        }


        [Flags]
        public enum WS : uint
        {
            BORDER = 0x800000,
            CAPTION = 0xc00000,
            CHILD = 0x40000000,
            CHILDWINDOW = 0x40000000,
            CLIPCHILDREN = 0x2000000,
            CLIPSIBLINGS = 0x4000000,
            DISABLED = 0x8000000,
            DLGFRAME = 0x400000,
            GROUP = 0x20000,
            HSCROLL = 0x100000,
            ICONIC = 0x20000000,
            MAXIMIZE = 0x1000000,
            MAXIMIZEBOX = 0x10000,
            MINIMIZE = 0x20000000,
            MINIMIZEBOX = 0x20000,
            OVERLAPPED = 0,
            OVERLAPPEDWINDOW = 0xcf0000,
            POPUP = 0x80000000,
            POPUPWINDOW = 0x80880000,
            SIZEBOX = 0x40000,
            SYSMENU = 0x80000,
            TABSTOP = 0x10000,
            THICKFRAME = 0x40000,
            TILED = 0,
            TILEDWINDOW = 0xcf0000,
            VISIBLE = 0x10000000,
            VSCROLL = 0x200000
        }



        public static IntPtr SetWindowLongPtr(IntPtr hwnd, GWL nIndex, IntPtr dwNewLong)
        {
            if (8 == IntPtr.Size)
            {
                return SetWindowLongPtr64(hwnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr32(hwnd, nIndex, dwNewLong);
        }


        public static int SignedLoWord(int i)
        {
            return (short)(i & 0xffff);
        }


        public static int SignedHiWord(int i)
        {
            return (short)(i >> 0x10);
        }


        [Obsolete]
        internal static bool xModifyHwndStyle(IntPtr hwnd, WS removeStyle, WS addStyle)
        {
            WS ws = (WS)GetWindowLongPtr(hwnd, GWL.STYLE).ToInt32();
            WS ws2 = (ws & ~removeStyle) | addStyle;
            if (ws == ws2)
            {
                return false;
            }
            SetWindowLongPtr(hwnd, GWL.STYLE, new IntPtr((int)ws2));
            return true;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public int Width { get { return Right - Left; } }
            public int Height { get { return Bottom - Top; } }
        }
    }
}
