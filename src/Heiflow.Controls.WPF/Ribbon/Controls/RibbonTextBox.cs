using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Odyssey.Controls.Ribbon.Interfaces;


#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public class RibbonTextBox : TextBox, IRibbonControl, IRibbonStretch
    {

        static RibbonTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTextBox), new FrameworkPropertyMetadata(typeof(RibbonTextBox)));
        }


        /// <summary>
        /// Gets or sets the image of the textbox.
        /// </summary>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonTextBox), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets or sets the title of the textbox that appears with Appearance = Medium.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RibbonTextBox), new UIPropertyMetadata(""));



        /// <summary>
        /// Gets or sets the with for the textbox.
        /// This is a dependency property.
        /// </summary>
        public double ContentWidth
        {
            get { return (double)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register("ContentWidth", typeof(double), typeof(RibbonTextBox), new UIPropertyMetadata(double.NaN));


    }
}
