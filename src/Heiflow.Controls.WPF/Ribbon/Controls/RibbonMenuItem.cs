using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
   [ContentProperty("Items")]
    public class RibbonMenuItem:MenuItem
    {
       const string partPopup = "PART_Popup";

        static RibbonMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonMenuItem), new FrameworkPropertyMetadata(typeof(RibbonMenuItem)));
        }



        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonMenuItem), new UIPropertyMetadata(null));


        private Popup popup;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (popup != null)
            {
                popup.Opened -= OnPopupOpenend;
                popup.Closed -= OnPopupClosed;
            }
            popup = GetTemplateChild(partPopup) as Popup;
            if (popup != null)
            {
                popup.Opened += new EventHandler(OnPopupOpenend);
                popup.Closed += new EventHandler(OnPopupClosed);
            }
        }

        protected virtual void OnPopupClosed(object sender, EventArgs e)
        {
            IsSubmenuOpen = false;
        }

        protected virtual void OnPopupOpenend(object sender, EventArgs e)
        {
            RibbonApplicationMenu menu = ItemsControl.ItemsControlFromItemContainer(this) as RibbonApplicationMenu;
            if (menu != null)
            {
                Rect subMenuRect = menu.GetSubMenuRect(this);
                popup.Placement = PlacementMode.Relative;
                popup.VerticalOffset = subMenuRect.Top;
                popup.HorizontalOffset = subMenuRect.Left;
                popup.Width = subMenuRect.Width;
                popup.Height = subMenuRect.Height;
            }
        }

        protected override void OnSubmenuOpened(RoutedEventArgs e)
        {
            base.OnSubmenuOpened(e);
            if (popup != null) popup.IsOpen = true;
        }

        protected override void OnSubmenuClosed(RoutedEventArgs e)
        {
            base.OnSubmenuClosed(e);
            if (popup != null) popup.IsOpen = false;
        }

    }
}
