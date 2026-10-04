using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    /// <summary>
    /// Renders the chrome for all Ribbon controls.
    /// </summary>
    [ContentProperty("Content")]
    public class RibbonChrome:ContentControl
    {
        static RibbonChrome()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonChrome), new FrameworkPropertyMetadata(typeof(RibbonChrome)));
        }

        public bool RenderPressed
        {
            get { return (bool)GetValue(RenderPressedProperty); }
            set { SetValue(RenderPressedProperty, value); }
        }

        public static readonly DependencyProperty RenderPressedProperty =
            DependencyProperty.Register("RenderPressed", typeof(bool), typeof(RibbonChrome), new UIPropertyMetadata(false));


        public bool RenderMouseOver
        {
            get { return (bool)GetValue(RenderMouseOverProperty); }
            set { SetValue(RenderMouseOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RenderMouseOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderMouseOverProperty =
            DependencyProperty.Register("RenderMouseOver", typeof(bool), typeof(RibbonChrome), new UIPropertyMetadata(false));



        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(RibbonChrome), new UIPropertyMetadata(null));



        public Brush MousePressedBackground
        {
            get { return (Brush)GetValue(MousePressedBackgroundProperty); }
            set { SetValue(MousePressedBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MousePressedBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePressedBackgroundProperty =
            DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(RibbonChrome), new UIPropertyMetadata(null));




        public Brush MouseCheckedBackground
        {
            get { return (Brush)GetValue(MouseCheckedBackgroundProperty); }
            set { SetValue(MouseCheckedBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseCheckedBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseCheckedBackgroundProperty =
            DependencyProperty.Register("MouseCheckedBackground", typeof(Brush), typeof(RibbonChrome), new UIPropertyMetadata(null));


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonChrome), new UIPropertyMetadata(new CornerRadius(0)));


        public bool RenderFlat
        {
            get { return (bool)GetValue(RenderFlatProperty); }
            set { SetValue(RenderFlatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RenderFlat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderFlatProperty =
            DependencyProperty.Register("RenderFlat", typeof(bool), typeof(RibbonChrome), new UIPropertyMetadata(true));


        public bool NoAnimation
        {
            get { return (bool)GetValue(NoAnimationProperty); }
            set { SetValue(NoAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NoAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoAnimationProperty =
            DependencyProperty.Register("NoAnimation", typeof(bool), typeof(RibbonChrome), new UIPropertyMetadata(true));


        public bool ShowInnerMouseOverBorder
        {
            get { return (bool)GetValue(ShowInnerMouseOverBorderProperty); }
            set { SetValue(ShowInnerMouseOverBorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowInnerMouseOverBorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowInnerMouseOverBorderProperty =
            DependencyProperty.Register("ShowInnerMouseOverBorder", typeof(bool), typeof(RibbonChrome), new UIPropertyMetadata(true));


        public Thickness InnerBorderThickness
        {
            get { return (Thickness)GetValue(InnerBorderThicknessProperty); }
            set { SetValue(InnerBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(RibbonChrome), new UIPropertyMetadata(new Thickness(1,1,0,0)));


        public bool RenderEnabled
        {
            get { return (bool)GetValue(RenderEnabledProperty); }
            set { SetValue(RenderEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RenderEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderEnabledProperty =
            DependencyProperty.Register("RenderEnabled", typeof(bool), typeof(RibbonChrome), new UIPropertyMetadata(true));



    }
}
