using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    /// <summary>
    /// Represents an item container for a RibbonGallery.
    /// </summary>
    public class RibbonThumbnail:ListBoxItem
    {
        static RibbonThumbnail()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonThumbnail), new FrameworkPropertyMetadata(typeof(RibbonThumbnail)));
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RibbonThumbnail), new UIPropertyMetadata(null));


        public RibbonThumbnail Original { get; internal set; }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            RibbonThumbnail original = this.Original != null ? Original : this;
            if (Gallery!=null) Gallery.SelectedItem = original;
            e.Handled = true;
        }

        private RibbonGallery Gallery
        {
            get {
                RibbonGallery gallery = Parent as RibbonGallery;
                if (gallery == null)
                {
                    FrameworkElement e = Parent as FrameworkElement;
                    gallery = e != null ? e.TemplatedParent as RibbonGallery : null;
                }
                return gallery;
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            // also close the poupup when the item is already selected:           
            if (this.IsFocused)
            {
                RibbonGallery g = Gallery;
                if (g != null) g.IsDropDownOpen = false;
            }
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            RibbonThumbnail thumb = Original != null ? Original : this;
            if (Gallery != null) Gallery.HotThumbnail = thumb;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (Gallery != null) Gallery.HotThumbnail = null;
        }



        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(RibbonThumbnail), new UIPropertyMetadata(Stretch.Fill));



    }
}
