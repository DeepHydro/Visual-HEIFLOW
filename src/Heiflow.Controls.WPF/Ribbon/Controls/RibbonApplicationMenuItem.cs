using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public class RibbonApplicationMenuItem:RibbonMenuItem
    {
        static RibbonApplicationMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonApplicationMenuItem), new FrameworkPropertyMetadata(typeof(RibbonApplicationMenuItem)));
        }



        /// <summary>
        /// Gets or sets the title for the sub menu popup.
        /// This is a dependency property.
        /// </summary>
        public object SubMenuTitle
        {
            get { return (object)GetValue(SubMenuTitleProperty); }
            set { SetValue(SubMenuTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubMenuTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubMenuTitleProperty =
            DependencyProperty.Register("SubMenuTitle", typeof(object), typeof(RibbonApplicationMenuItem), new UIPropertyMetadata(null));


    }
}
