using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Odyssey.Controls.Ribbon.Interfaces;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public partial class RibbonGallery : Selector, IRibbonControl, IRibbonLargeControl, IRibbonGallery
    {
        const string partPopupupBtn = "PART_PopupBtn";
        const string partWrapPanel = "PART_WrapPanel";
        const string partItemsPresenter = "PART_ItemsPresenter";

        static RibbonGallery()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGallery), new FrameworkPropertyMetadata(typeof(RibbonGallery)));
        }

        public RibbonGallery()
            : base()
        {
            RegisterCommands();
        }



        /// <summary>
        /// Gets the item that is under the mouse cursor.
        /// This is a dependency property.
        /// </summary>
        public object HotItem
        {
            get { return (object)GetValue(HotItemProperty); }
            private set { SetValue(HotItemPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for HotItem.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey HotItemPropertyKey =
            DependencyProperty.RegisterReadOnly("HotItem", typeof(object), typeof(RibbonGallery), new UIPropertyMetadata(null));

        public static readonly DependencyProperty HotItemProperty = HotItemPropertyKey.DependencyProperty;



        /// <summary>
        /// Gets the RibbonThumbnail that is under the mouse cursor.
        /// This is a dependency property.
        /// </summary>
        public RibbonThumbnail HotThumbnail
        {
            get { return (RibbonThumbnail)GetValue(HotThumbnailProperty); }
            internal set { SetValue(HotThumbnailPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for HotThumbnail.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey HotThumbnailPropertyKey =
            DependencyProperty.RegisterReadOnly("HotThumbnail", typeof(RibbonThumbnail), typeof(RibbonGallery),
            new UIPropertyMetadata(null, HotThumbnailPropertyChanged));

        public static readonly DependencyProperty HotThumbnailProperty = HotThumbnailPropertyKey.DependencyProperty;

        public static void HotThumbnailPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery g = (RibbonGallery)o;
            g.OnHotThumbnailChanged(e);
        }

        protected virtual void OnHotThumbnailChanged(DependencyPropertyChangedEventArgs e)
        {
            RibbonThumbnail hot = HotThumbnail;
            HotItem = hot == null || IsItemItsOwnContainerOverride(hot) ? hot : ItemContainerGenerator.ItemFromContainer(hot);

            RoutedEventArgs args = new RoutedEventArgs(RibbonGallery.HotThumbnailChangedEvent);
            RaiseEvent(args);
        }

        public static readonly RoutedEvent HotThumbnailChangedEvent = EventManager.RegisterRoutedEvent("HotThumbnailChangedEvent",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RibbonGallery));

        public event RoutedEventHandler HotThumbnailChanged
        {
            add { AddHandler(HotThumbnailChangedEvent, value); }
            remove { RemoveHandler(HotThumbnailChangedEvent, value); }
        }


        private RibbonThumbnail SelectedThumbnail
        {
            get { return SelectedItem as RibbonThumbnail; }
            set { SelectedItem = value; }
        }


        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            IsDropDownOpen = false;
            if (SelectedItem != null)
            {
                MakeSelectedItemVisible();
            }
        }

        private void MakeSelectedItemVisible()
        {
            foreach (RibbonThumbnail thumb in wrapPanel.Children)
            {
                if (thumb.IsSelected)
                {
                    thumb.BringIntoView();
                    break;
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            AttachThumbnailsToPanel();

        }


        private RibbonDropDownButton popupBtn;
        private Panel wrapPanel;
        private FrameworkElement itemsPresenter;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (popupBtn != null)
            {
                popupBtn.PopupOpened -= OnPopupOpenend;
            }
            popupBtn = GetTemplateChild(partPopupupBtn) as RibbonDropDownButton;
            if (popupBtn != null)
            {
                popupBtn.PopupOpened += new RoutedEventHandler(OnPopupOpenend);
            }

            if (wrapPanel != null) wrapPanel.Children.Clear();
            wrapPanel = GetTemplateChild(partWrapPanel) as Panel;
            if (wrapPanel == null) throw new ArgumentNullException(partWrapPanel);

            itemsPresenter = GetTemplateChild(partItemsPresenter) as FrameworkElement;
            if (wrapPanel == null) throw new ArgumentNullException(partItemsPresenter);
            AttachThumbnailsToPanel();
        }

        private void AttachThumbnailsToPanel()
        {
            if (wrapPanel != null)
            {
                wrapPanel.Children.Clear();
                foreach (object item in Items)
                {
                    RibbonThumbnail container = item as RibbonThumbnail;
                    if (container == null) container = ItemContainerGenerator.ContainerFromItem(item) as RibbonThumbnail;
                    if (container != null)
                    {
                        RibbonThumbnail thumbnail = new RibbonThumbnail();
                        thumbnail.ImageSource = container.ImageSource;
                        Binding b = new Binding("IsSelected")
                        {
                            Mode = BindingMode.TwoWay,
                            Source = container
                        };
                        thumbnail.Original = container;
                        thumbnail.SetBinding(RibbonThumbnail.IsSelectedProperty, b);
                        PrepareContainerForItemOverride(thumbnail, item);
                        wrapPanel.Children.Add(thumbnail);
                    }
                }
            }
        }

        public RibbonThumbnail Original { get; internal set; }


        protected virtual void OnPopupOpenend(object sender, RoutedEventArgs e)
        {
            Popup popup = popupBtn.Popup;


            Thickness margin = new Thickness();

            FrameworkElement fe = popup.Child as FrameworkElement;
            while (fe != null)
            {
                margin.Left += fe.Margin.Left;
                margin.Right += fe.Margin.Right;
                margin.Top += fe.Margin.Top;
                margin.Bottom += fe.Margin.Bottom;
                Decorator c = fe as Decorator;
                fe = c != null ? c.Child as FrameworkElement : null;
            }

            double w = ActualWidth + margin.Right;
            double h = ActualHeight + margin.Bottom;
            Rect rect = new Rect(-margin.Left, -margin.Top, w, h);

            rect = this.TransformToVisual(popup).TransformBounds(rect);
            if (!IsCollapsed)
            {
                popup.Placement = PlacementMode.Relative;
                popup.VerticalOffset = rect.Top;
                popup.HorizontalOffset = rect.Left;
            }

            if (Columns > 0) popup.MinWidth = rect.Width;
            popup.MinHeight = rect.Height;
            itemsPresenter.MaxWidth = DropDownMaxSize.Width;
            itemsPresenter.MaxHeight = DropDownMaxSize.Height;
            itemsPresenter.Width = ThumbnailSize.Width * ActualDropDownColumns;
        }


        /// <summary>
        /// Gets or sets the size that a RibbonThumbnail will have.
        /// This is a dependency property.
        /// </summary>
        public Size ThumbnailSize
        {
            get { return (Size)GetValue(ThumbnailSizeProperty); }
            set { SetValue(ThumbnailSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbnailSizeProperty =
            DependencyProperty.Register("ThumbnailSize", typeof(Size), typeof(RibbonGallery), new UIPropertyMetadata(new Size(48.0, 48.0)));



        /// <summary>
        /// Gets or sets how many columns are visible in the collapsed Gallery.
        /// This is a dependency property.
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(RibbonGallery),
            new FrameworkPropertyMetadata(3,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsMeasure
                , ColumnsPropertyChanged));



        /// <summary>
        /// Gets or sets the number of columns that appear in the drop down menu.
        /// Thisis a dependency property.
        /// </summary>
        public int DropDownColumns
        {
            get { return (int)GetValue(DropDownColumnsProperty); }
            set { SetValue(DropDownColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownColumnsProperty =
            DependencyProperty.Register("DropDownColumns", typeof(int), typeof(RibbonGallery), new UIPropertyMetadata(0));



        public static void ColumnsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery g = (RibbonGallery)o;
            g.OnColumnsChanged(e);
        }

        protected virtual void OnColumnsChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateLayout();
        }



        /// <summary>
        /// Gets or sets whether the ListBox with details is collapsed.
        /// This is a dependency property.
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(RibbonGallery), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets how many columns are available for the in-ribbon panel.
        /// </summary>
        public int ActualColumns
        {
            get
            {
                double w = wrapPanel.DesiredSize.Width;
                return (int)Math.Floor(w / ThumbnailSize.Width);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double maxWrapPanelWidth = ThumbnailSize.Width * Columns;
            if (wrapPanel.MaxWidth != maxWrapPanelWidth)
            {
                wrapPanel.MaxWidth = maxWrapPanelWidth;
                wrapPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            Size size = base.MeasureOverride(constraint);

            double w = CalculateInRibbonThumbnailWidth();
            if (wrapPanel.MaxWidth != w)
            {
                wrapPanel.MaxWidth = w;
                wrapPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                size = base.MeasureOverride(constraint);
            }

            return size;
        }

        private double CalculateInRibbonThumbnailWidth()
        {
            double w = wrapPanel.DesiredSize.Width;
            return this.ThumbnailSize.Width * Math.Floor(w / ThumbnailSize.Width);
        }

        private double CalculateInRibbonThumbnailHeight()
        {
            double height = ThumbnailSize.Height;
            double smallHeight = InternalGroupPanel.MaxSmallHeight;
            double rows;
            if (height <= smallHeight) rows = 3.0;
            else if (height <= (2.0 * smallHeight)) rows = 2.0;
            else rows = 1.0;


            return ((3.0 * smallHeight - 3.0) / rows);
        }


        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RibbonGallery),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsMeasure,
                DropDownOpenPropertyChanged));


        public static void DropDownOpenPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery g = (RibbonGallery)o;
            g.DropDownOpenChanged(e);
        }

        protected virtual void DropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            return;
            //Panel dropDownPanel = GetTemplateChild("xPART_ListBox") as Panel;
            //Panel mainPanel = GetTemplateChild("PART_WrapPanel") as Panel;
            //if (dropDownPanel != null)
            //{
            //    bool opened = (bool)e.NewValue;
            //    if (opened)
            //    {
            //        dropDownPanel.Children.Clear();
            //        foreach (object o in Items)
            //        {
            //            RibbonMenuItem item = o as RibbonMenuItem;
            //            if (item == null)
            //            {
            //                Binding b = new Binding();
            //                b.Source = o;
            //                item = new RibbonMenuItem();
            //                item.SetBinding(RibbonMenuItem.HeaderProperty, b);
            //            }
            //            dropDownPanel.Children.Add(item);
            //        }
            //    }
            //    else
            //    {
            //        mainPanel.Children.Clear();
            //        foreach (object o in Items)
            //        {
            //            RibbonMenuItem item = o as RibbonMenuItem;
            //            if (item == null)
            //            {
            //                Binding b = new Binding();
            //                b.Source = o;
            //                item = new RibbonMenuItem();
            //                item.SetBinding(RibbonMenuItem.HeaderProperty, b);
            //            }
            //            mainPanel.Children.Add(item);
            //        }
            //    }
            //}
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RibbonThumbnail;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonThumbnail();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            RibbonThumbnail tn = element as RibbonThumbnail;
            if (tn != null)
            {
                tn.Width = ThumbnailSize.Width;
                tn.Height = CalculateInRibbonThumbnailHeight();
                Stretch stretch = RibbonGallery.GetStretch(this);
                RibbonGallery.SetStretch(tn, stretch);
            }
        }


        public Size DropDownMaxSize
        {
            get { return (Size)GetValue(DropDownMaxSizeProperty); }
            set { SetValue(DropDownMaxSizeProperty, value); }
        }

        public static readonly DependencyProperty DropDownMaxSizeProperty =
            DependencyProperty.Register("DropDownMaxSize", typeof(Size), typeof(RibbonGallery), new UIPropertyMetadata(new Size(double.PositiveInfinity, double.PositiveInfinity)));



        /// <summary>
        /// Gets the collection of RibbonThumbnails.
        /// This is a dependency property
        /// </summary>
        /// <remarks>
        /// This dependency property is used to bind the generated collection of RibbonThumbnails with the listboxes inside the ControlTemplate.
        /// </remarks>
        public object Thumbnails
        {
            get { return (object)GetValue(ThumbnailsProperty); }
            set { SetValue(ThumbnailsProperty, value); }
        }

        public static readonly DependencyProperty ThumbnailsProperty =
            DependencyProperty.Register("Thumbnails", typeof(object), typeof(RibbonGallery), new UIPropertyMetadata(null));


        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(RibbonGallery), new UIPropertyMetadata(null));


        public object Footer
        {
            get { return (object)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(object), typeof(RibbonGallery), new UIPropertyMetadata(null));


        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(RibbonGallery), new UIPropertyMetadata(null));


        public DataTemplate FooterTemplate
        {
            get { return (DataTemplate)GetValue(FooterTemplateProperty); }
            set { SetValue(FooterTemplateProperty, value); }
        }

        public static readonly DependencyProperty FooterTemplateProperty =
            DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(RibbonGallery), new UIPropertyMetadata(null));

        public static RibbonGalleryColumns GetReductionColumns(DependencyObject obj)
        {
            return (RibbonGalleryColumns)obj.GetValue(ReductionColumnsProperty);
        }

        public static void SetReductionColumns(DependencyObject obj, RibbonGalleryColumns value)
        {
            obj.SetValue(ReductionColumnsProperty, value);
        }

        public static readonly DependencyProperty ReductionColumnsProperty =
            DependencyProperty.RegisterAttached("ReductionColumns", typeof(RibbonGalleryColumns), typeof(RibbonGallery),
            new UIPropertyMetadata(null, ReductionColumnsPropertyChanged));

        static void ReductionColumnsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery g = o as RibbonGallery;
            if (g != null)
            {
                RibbonGalleryColumns columns = e.NewValue as RibbonGalleryColumns;
                if (columns != null && columns.Count > 0)
                {
                    g.Columns = columns[0];
                }
            }
        }

        private int ActualDropDownColumns;

        void IRibbonGallery.SetDropDownColumns(int columns)
        {
            ActualDropDownColumns = DropDownColumns > 0 ? DropDownColumns : columns;
        }


        /// <summary>
        /// Gets or sets the image that appears when the Gallery is collapsed.
        /// This is a dependency property.
        /// </summary>
        public ImageSource LargeImage
        {
            get { return (ImageSource)GetValue(LargeImageProperty); }
            set { SetValue(LargeImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LargeImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LargeImageProperty =
            DependencyProperty.Register("LargeImage", typeof(ImageSource), typeof(RibbonGallery), new UIPropertyMetadata(null));




        /// <summary>
        /// Gets or sets the title that appears when the gallery is collapsed.
        /// This is a dependency property.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RibbonGallery), new UIPropertyMetadata(""));



        /// <summary>
        /// Specifies how to stretch each image of a RibbonThumbnail.
        /// Attach this property to the RibbonGallery rather than to each RibbonThumbnail (see example).
        /// This is a dependency property.
        /// </summary>
        /// <example>
        /// <![CDATA[<RibbonGallery odc:RibbonGallery.Stretch="None"/>]]>
        /// </example>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static Stretch GetStretch(DependencyObject obj)
        {
            return (Stretch)obj.GetValue(StretchProperty);
        }

        public static void SetStretch(DependencyObject obj, Stretch value)
        {
            obj.SetValue(StretchProperty, value);
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.RegisterAttached("Stretch", typeof(Stretch), typeof(RibbonGallery), new UIPropertyMetadata(Stretch.None));


        /// <summary>
        /// Gets or sets the with for the drop down menu.
        /// This is a dependency property.
        /// </summary>
        public double DropDownWidth
        {
            get { return (double)GetValue(DropDownWidthProperty); }
            set { SetValue(DropDownWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownWidthProperty =
            DependencyProperty.Register("DropDownWidth", typeof(double), typeof(RibbonGallery), new UIPropertyMetadata(double.NaN));

    }
}