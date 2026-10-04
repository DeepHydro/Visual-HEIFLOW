using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Odyssey.Controls.Ribbon.Interfaces;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{

    public class RibbonToggleButton : ToggleButton,IRibbonButton
    {
        static RibbonToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonToggleButton), new FrameworkPropertyMetadata(typeof(RibbonToggleButton)));
        }



        public bool IsFlat
        {
            get { return (bool)GetValue(IsFlatProperty); }
            set { SetValue(IsFlatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFlat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFlatProperty =
            DependencyProperty.Register("IsFlat", typeof(bool), typeof(RibbonToggleButton), new UIPropertyMetadata(true));


        public ImageSource LargeImage
        {
            get { return (ImageSource)GetValue(LargeImageProperty); }
            set { SetValue(LargeImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LargeImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LargeImageProperty =
            DependencyProperty.Register("LargeImage", typeof(ImageSource), typeof(RibbonToggleButton), new UIPropertyMetadata(null));



        public ImageSource SmallImage
        {
            get { return (ImageSource)GetValue(SmallImageProperty); }
            set { SetValue(SmallImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallImageProperty =
            DependencyProperty.Register("SmallImage", typeof(ImageSource), typeof(RibbonToggleButton), new UIPropertyMetadata(null));



        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonToggleButton), new UIPropertyMetadata(new CornerRadius(4)));

    }
}
