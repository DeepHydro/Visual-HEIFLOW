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
using System.Windows.Markup;
using Odyssey.Controls.Ribbon.Interfaces;
using System.Windows.Media.Effects;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    [ContentProperty("Content")]
    public class RibbonButton : Button, IRibbonButton
    {

        static RibbonButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonButton), new FrameworkPropertyMetadata(typeof(RibbonButton)));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonButton), new UIPropertyMetadata(new CornerRadius(3)));


        public ImageSource LargeImage
        {
            get { return (ImageSource)GetValue(LargeImageProperty); }
            set { SetValue(LargeImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LargeImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LargeImageProperty =
            DependencyProperty.Register("LargeImage", typeof(ImageSource), typeof(RibbonButton), new FrameworkPropertyMetadata(null));


        public ImageSource SmallImage
        {
            get { return (ImageSource)GetValue(SmallImageProperty); }
            set { SetValue(SmallImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallImageProperty =
            DependencyProperty.Register("SmallImage", typeof(ImageSource), typeof(RibbonButton), new FrameworkPropertyMetadata(null));



        public bool IsFlat
        {
            get { return (bool)GetValue(IsFlatProperty); }
            set { SetValue(IsFlatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFlat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFlatProperty =
            DependencyProperty.Register("IsFlat", typeof(bool), typeof(RibbonButton), new UIPropertyMetadata(true));




        /// <summary>
        /// Gets or sets how to stretch an image inside an IRibbonButton
        /// This is an attached dependency property.
        /// </summary>
        public static Stretch GetImageStretch(DependencyObject obj)
        {
            return (Stretch)obj.GetValue(ImageStretchProperty);
        }

        public static void SetImageStretch(DependencyObject obj, Stretch value)
        {
            obj.SetValue(ImageStretchProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageStretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.RegisterAttached("ImageStretch", typeof(Stretch), typeof(RibbonButton), new UIPropertyMetadata(Stretch.Uniform));


    }
}
