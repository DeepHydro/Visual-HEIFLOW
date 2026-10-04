using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public class RibbonComboBoxItem:ComboBoxItem
    {
        static RibbonComboBoxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonComboBoxItem), new FrameworkPropertyMetadata(typeof(RibbonComboBoxItem)));
        }



        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonComboBoxItem), new UIPropertyMetadata(null));

    }
}
