using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Odyssey.Controls.Ribbon.Interfaces;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public class RibbonComboBox : ComboBox, IRibbonControl, IRibbonStretch
    {
        static RibbonComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonComboBox), new FrameworkPropertyMetadata(typeof(RibbonComboBox)));
        }

        public RibbonComboBox()
            : base()
        {
        }


        /// <summary>
        /// Gets or sets the Image of the combobox.
        /// </summary>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonComboBox), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets or sets the title of the combobox that appears with Appearance = Medium.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RibbonComboBox), new UIPropertyMetadata(""));


        /// <summary>
        /// Gets or sets the with for the combobox.
        /// This is a dependency property.
        /// </summary>
        public double ContentWidth
        {
            get { return (double)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register("ContentWidth", typeof(double), typeof(RibbonComboBox), new UIPropertyMetadata(double.NaN));


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            //return item is RibbonMenuItem;
            return item is RibbonComboBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonComboBoxItem();
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            RoutedEventArgs args = new RoutedEventArgs(RibbonComboBox.DropDownClosedEvent);
            RaiseEvent(args);
        }

        public static readonly RoutedEvent DropDownClosedEvent = EventManager.RegisterRoutedEvent("DropDownClosedEvent",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RibbonComboBox));

        /// <summary>
        /// Occurs when the DropDown is closed.
        /// The Ribbonbar uses this event to determine wether to collapse a collapsible ribbon after a combobox is closed.
        /// </summary>
        public event RoutedEventHandler RoutedDropDownClosed
        {
            add { AddHandler(DropDownClosedEvent, value); }
            remove { RemoveHandler(DropDownClosedEvent, value); }
        }

        public object DropDownFooter
        {
            get { return (object)GetValue(DropDownFooterProperty); }
            set { SetValue(DropDownFooterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownFooter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownFooterProperty =
            DependencyProperty.Register("DropDownFooter", typeof(object), typeof(RibbonComboBox), new UIPropertyMetadata(null));




        public object DropDownHeader
        {
            get { return (object)GetValue(DropDownHeaderProperty); }
            set { SetValue(DropDownHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownHeaderProperty =
            DependencyProperty.Register("DropDownHeader", typeof(object), typeof(RibbonComboBox), new UIPropertyMetadata(null));



        public DataTemplate DropDownHeaderTemplate
        {
            get { return (DataTemplate)GetValue(DropDownHeaderTemplateProperty); }
            set { SetValue(DropDownHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownHeaderTemplateProperty =
            DependencyProperty.Register("DropDownHeaderTemplate", typeof(DataTemplate), typeof(RibbonComboBox), new UIPropertyMetadata(null));



        public DataTemplate DropDownFooterTemplate
        {
            get { return (DataTemplate)GetValue(DropDownFooterTemplateProperty); }
            set { SetValue(DropDownFooterTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownFooterTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownFooterTemplateProperty =
            DependencyProperty.Register("DropDownFooterTemplate", typeof(DataTemplate), typeof(RibbonComboBox), new UIPropertyMetadata(null));


    }
}
