using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Odyssey.Native;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Controls;
using Odyssey.Controls.Classes;
using System.Runtime.InteropServices;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public partial class RibbonWindow : Window
    {
        const string partOuterBorder = "PART_OuterBorder";

        static RibbonWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWindow), new FrameworkPropertyMetadata(typeof(RibbonWindow)));
        }

        public RibbonWindow()
            : base()
        {
            SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
            HookWndProc();
            RegisterCommands();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            SetGlassOn();
        }

        /// <summary>
        /// Gets whether glass is available.
        /// </summary>
        public bool IsGlassAvailable
        {
            get
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    if (NativeMethods.DwmIsCompositionEnabled()) return true;
                }
                return false;
            }
        }

        private void SetGlassOn()
        {
            if (IsGlassAvailable)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                HwndSource src = HwndSource.FromHwnd(hwnd);

                // settings the following value is necassary to have a transparent glass background instead of black:
                src.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                const int GlassBorderSize = 8;
                const int GlassTitleBorderHeight = 31;

                NativeMethods.MARGINS margins = new NativeMethods.MARGINS();
                margins.cxLeftWidth = GlassBorderSize;
                margins.cxRightWidth = GlassBorderSize;
                margins.cyTopHeight = GlassTitleBorderHeight;
                margins.cyBottomHeight = GlassBorderSize;
                NativeMethods.DwmExtendFrameIntoClientArea(hwnd, ref margins);
            }
        }


        private void HookWndProc()
        {
            EventHandler handler = delegate(object sender, EventArgs e)
            {
                ((HwndSource)PresentationSource.FromVisual(this)).AddHook(new HwndSourceHook(this.WndProc));
            };
            base.SourceInitialized += handler;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            UIElement child = this.VisualChildrenCount > 0 ? GetVisualChild(0) as UIElement : null;
            if (child != null)
            {
                child.Arrange(new Rect(arrangeBounds));
            }
            return arrangeBounds;
        }


        /// <summary>
        /// Gets or sets whether Glass is enabled for Vista Aero. This does not necassarily mean that glass is applied but only when the conditions for glass match.
        /// This is a dependency property.
        /// </summary>
        public bool IsGlassEnabled
        {
            get { return (bool)GetValue(IsGlassEnabledProperty); }
            set { SetValue(IsGlassEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsGlassEnabledProperty =
            DependencyProperty.Register("IsGlassEnabled", typeof(bool), typeof(RibbonWindow),
            new FrameworkPropertyMetadata(false, GlassEnabledPropertyChanged));




        /// <summary>
        /// Gets whether Glass is on.
        /// This is a dependency property.
        /// </summary>
        public bool IsGlassOn
        {
            get { return (bool)GetValue(IsGlassOnProperty); }
            private set { SetValue(IsGlassOnPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for IsGlassOn.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey IsGlassOnPropertyKey =
            DependencyProperty.RegisterReadOnly("IsGlassOn", typeof(bool), typeof(RibbonWindow), new UIPropertyMetadata(false, GlassOnPropertyChanged));

        public static DependencyProperty IsGlassOnProperty = IsGlassOnPropertyKey.DependencyProperty;

        static void GlassOnPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow w = (RibbonWindow)o;
            w.SetWindowTitleBrush();
        }

        static void GlassEnabledPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow w = (RibbonWindow)o;
            w.SetIsGlassOnState();
        }

        private void SetIsGlassOnState()
        {
            IsGlassOn = IsGlassEnabled && (Environment.OSVersion.Version.Major >= 6) && NativeMethods.DwmIsCompositionEnabled();
            AttachRegion();
        }



        protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AttachRegion();
            SetWindowTitleBrush();
        }



        private void AttachRegion()
        {
            if (!IsGlassOn)
            {
                NativeMethods.RECT rect;
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                NativeMethods.GetWindowRect(hwnd, out rect);
                int w = rect.Width + 1;
                int h = rect.Height + 1;
                if (WindowState != WindowState.Maximized && RoundedCornerMode != RibbonWindowCornerMode.None)
                {
                    // note: the last two parameters are the diameter, not the radius:
                    IntPtr rgn = NativeMethods.CreateRoundRectRgn(0, 0, w, h, 12, 12);
                    if (RoundedCornerMode == RibbonWindowCornerMode.Top)
                    {
                        IntPtr rgn2 = NativeMethods.CreateRectRgn(0, 6, w, h);
                        NativeMethods.CombineRgn(rgn, rgn2, rgn, 2);
                    }
                    NativeMethods.SetWindowRgn(hwnd, rgn, NativeMethods.IsWindowVisible(hwnd));
                }
                else
                {
                    IntPtr rgn = NativeMethods.CreateRectRgn(0, 0, w, h);
                    NativeMethods.SetWindowRgn(hwnd, rgn, NativeMethods.IsWindowVisible(hwnd));
                }
            }
            else
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                NativeMethods.SetWindowRgn(hwnd, IntPtr.Zero, NativeMethods.IsWindowVisible(hwnd));
            }
        }

        /// <summary>
        /// Handles the WndProc events to enable drawing inside the title bar.
        /// </summary>
        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            IntPtr result = IntPtr.Zero;
            NativeMethods.WM wm = (NativeMethods.WM)msg;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                handled = NativeMethods.DwmDefWindowProc(hwnd, msg, wParam, lParam, out result);
            }
            if (!handled)
                switch (wm)
                {

                    case NativeMethods.WM.SIZE:
                        handled = false;
                        break;

                    //TODO: This causes a uncontrolled moving of the window in windows 7 while dragging from  WindowState.Maximized.
                    // don't paint the border and title:
                    case NativeMethods.WM.NCCALCSIZE:
                        handled = true;
                        break;

                    case NativeMethods.WM.SETICON:
                        handled = true;
                        return IntPtr.Zero;

                    case NativeMethods.WM.SETTEXT:
                        handled = true;
                        InvalidateArrange();
                        break;


                    case NativeMethods.WM.NCACTIVATE:
                        IsWindowActive = wParam.ToInt32() == 1;
                        handled = true;
                        result = NativeMethods.DefWindowProc(hwnd, NativeMethods.WM.NCACTIVATE, wParam, new IntPtr(-1));
                        break;

                    // determine if the titlebar, or any of the borders is under the cursor position coded in m.lParam:
                    case NativeMethods.WM.NCHITTEST:
                        if (result == IntPtr.Zero)
                        {
                            WndProcHitTest(hwnd, lParam, ref handled, ref result);
                        }
                        break;


                    //TODO: changing the DWMCOMPOSITION currently causes a "This freezable cannot be frozen" exception.
                    case NativeMethods.WM.DWMCOMPOSITIONCHANGED:
                        SetIsGlassOnState();
                        AttachRegion();
                        InvalidateVisual();
                        UpdateLayout();
                        handled = true;
                        result = IntPtr.Zero;
                        break;

                }

            return result;
        }


        /// <summary>
        /// Determine if any of the window borders or the titlebar is hit:
        /// </summary>
        private void WndProcHitTest(IntPtr hwnd, IntPtr lParam, ref bool handled, ref IntPtr result)
        {
            int xy = lParam.ToInt32();
            Point p = new Point(NativeMethods.SignedLoWord(xy), NativeMethods.SignedHiWord(xy));
            NativeMethods.RECT rect = new NativeMethods.RECT();
            NativeMethods.GetWindowRect(hwnd, out rect);
            Rect windowRect = new Rect(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            NativeMethods.HT ht = NCHitTest(p, windowRect);
            result = new IntPtr((int)ht);
            handled = ht != NativeMethods.HT.NOWHERE;
        }


        private NativeMethods.HT NCHitTest(Point p, Rect rect)
        {
            const double borderSize = 6.0;
            const double titleHeight = 34.0;

            if (p.Y < rect.Top + borderSize)
            {
                if (p.X < rect.Left + borderSize) return NativeMethods.HT.TOPLEFT;
                if (p.X > rect.Right - borderSize) return NativeMethods.HT.TOPRIGHT;
                return NativeMethods.HT.TOP;
            }
            if (p.Y > rect.Bottom - borderSize)
            {
                if (p.X < rect.Left + borderSize) return NativeMethods.HT.BOTTOMLEFT;
                if (p.X > rect.Right - borderSize) return NativeMethods.HT.BOTTOMRIGHT;
                return NativeMethods.HT.BOTTOM;
            }
            if (p.X < rect.Left + borderSize) return NativeMethods.HT.LEFT;
            if (p.X > rect.Right - borderSize) return NativeMethods.HT.RIGHT;
            if (p.Y < rect.Top + titleHeight)
            {
                Point localPoint = ScreenToLocal(p);
                IInputElement e = InputHitTest(localPoint);
                if (e == null)
                {
                    return NativeMethods.HT.CAPTION;
                }
                else
                {
                    if (e == outerBorder) return NativeMethods.HT.CAPTION;
                    UIElement ue = e as UIElement;
                    if (ue == null || !ue.IsHitTestVisible) return NativeMethods.HT.CAPTION;
                }
            }

            return NativeMethods.HT.NOWHERE;
        }

        private Point ScreenToLocal(Point point)
        {
            NativeMethods.RECT rect;
            NativeMethods.GetWindowRect(new WindowInteropHelper(this).Handle, out rect);
            Matrix matrix = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            point.Offset((double)(-1 * rect.Left), (double)(-1 * rect.Top));
            point.X *= matrix.M11;
            point.Y *= matrix.M22;
            return point;
        }

        private DependencyObject outerBorder;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            outerBorder = GetTemplateChild(partOuterBorder);
        }


        /// <summary>
        /// Gets whether the window is active.
        /// This is a dependency property.
        /// </summary>
        public bool IsWindowActive
        {
            get { return (bool)GetValue(IsWindowActiveProperty); }
            private set { SetValue(IsWindowActivePropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for IsWindowActive.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey IsWindowActivePropertyKey =
            DependencyProperty.RegisterReadOnly("IsWindowActive", typeof(bool), typeof(RibbonWindow),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender,
                WindowActivePropertyChanged));

        public static readonly DependencyProperty IsWindowActiveProperty = IsWindowActivePropertyKey.DependencyProperty;

        static void WindowActivePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow w = (RibbonWindow)o;
            w.SetWindowTitleBrush();
        }

        void SetWindowTitleBrush()
        {
            if (WindowState == WindowState.Maximized && IsGlassOn)
            {
                WindowTitleBrush = Brushes.White;
            }
            else
            {
                WindowTitleBrush = IsWindowActive ? SystemColors.ActiveCaptionTextBrush : SystemColors.InactiveCaptionTextBrush;
            }
        }


        public RibbonWindowCornerMode RoundedCornerMode
        {
            get { return (RibbonWindowCornerMode)GetValue(RoundedCornerModeProperty); }
            set { SetValue(RoundedCornerModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RoundedCornerMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoundedCornerModeProperty =
            DependencyProperty.Register("RoundedCornerMode", typeof(RibbonWindowCornerMode), typeof(RibbonWindow),
            new FrameworkPropertyMetadata(RibbonWindowCornerMode.Top,
                FrameworkPropertyMetadataOptions.AffectsRender,
                RoundedCornerModePropertyChanged));


        public static void RoundedCornerModePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonWindow window = (RibbonWindow)o;
            window.AttachRegion();
        }


        /// <summary>
        /// Gets the Brush for the window title.
        /// This is a dependency property.
        /// </summary>
        public Brush WindowTitleBrush
        {
            get { return (Brush)GetValue(WindowTitleBrushProperty); }
            private set { SetValue(WindowTitleBrushPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for WindowTitleBrush.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey WindowTitleBrushPropertyKey =
            DependencyProperty.RegisterReadOnly("WindowTitleBrush", typeof(Brush), typeof(RibbonWindow), new UIPropertyMetadata(null));

        public static DependencyProperty WindowTitleBrushProperty = WindowTitleBrushPropertyKey.DependencyProperty;



    }
}
