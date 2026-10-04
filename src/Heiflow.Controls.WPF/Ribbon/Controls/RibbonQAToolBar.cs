using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Odyssey.Controls.Ribbon.Interfaces;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Data;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    /// <summary>
    /// RibbonQuickAccessToolbar
    /// </summary>
    [TemplatePart(Name = partItemsHost)]
    [TemplatePart(Name = partOverflowHost)]
    [TemplatePart(Name = partMenuItemsHost)]
    public class RibbonQAToolBar : ItemsControl
    {
        const string partItemsHost = "PART_ToolBarPanel";
        const string partOverflowHost = "PART_OverflowHost";
        const string partMenuItemsHost = "PART_MenuItemHost";
        const string partMenuItemsOverflowHost = "PART_MenuItemOverflowHost";

        static RibbonQAToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonQAToolBar), new FrameworkPropertyMetadata(typeof(RibbonQAToolBar)));
        }

        public RibbonQAToolBar()
            : base()
        {
            AddHandler(RibbonButton.ClickEvent, new RoutedEventHandler(OnClick));
            AddHandler(RibbonSplitButton.ClickEvent, new RoutedEventHandler(OnClick));
            AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(OnMenuItemClick));
        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            if (e.Source != this)
            {
                IsOverflowOpen = false;
                IsMenuOpen = false;
            }
        }

        void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            IsMenuOpen = false;
            IsOverflowOpen = false;
        }


        private Panel itemsHost;
        private Panel overflowHost;
        private Panel menuItemsHost;
        private Panel menuItemsOverflowHost;


        public override void OnApplyTemplate()
        {
            if (menuItemsOverflowHost != null) menuItemsOverflowHost.Children.Clear();
            if (menuItemsHost != null) menuItemsHost.Children.Clear();
            if (itemsHost != null) itemsHost.Children.Clear();
            if (overflowHost != null) overflowHost.Children.Clear();

            base.OnApplyTemplate();
            itemsHost = GetTemplateChild(partItemsHost) as Panel;
            overflowHost = GetTemplateChild(partOverflowHost) as Panel;

            menuItemsHost = GetTemplateChild(partMenuItemsHost) as Panel;
            menuItemsOverflowHost = GetTemplateChild(partMenuItemsOverflowHost) as Panel;

            if (itemsHost == null) throw new ArgumentException(partItemsHost + " is not defined in ControlTemplate.");
        }

        private IEnumerable<UIElement> GetMenuItems()
        {
            foreach (UIElement e in Items)
            {
                if (!(e is IRibbonButton)) yield return e;
            }
        }

        private IEnumerable<UIElement> GetToolbarItems()
        {
            foreach (UIElement e in Items)
            {
                if ((e is IRibbonButton)) yield return e;
            }
        }

        private void CreateMenuItems()
        {
            if (menuItemsHost != null)
            {
                HasMenuItems = false;
                if (menuItemsOverflowHost != null) menuItemsOverflowHost.Children.Clear();
                menuItemsHost.Children.Clear();
                Panel host = HasOverflowItems && menuItemsOverflowHost != null ? menuItemsOverflowHost : menuItemsHost;
                foreach (UIElement e in GetMenuItems())
                {
                    host.Children.Add(e);
                    //RibbonMenuItem mi = new RibbonMenuItem();
                    //RibbonMenuItem rm = e as RibbonMenuItem;
                    //if (rm != null)
                    //{
                    //    Binding b = new Binding("IsChecked");
                    //    b.Source = e;
                    //    b.Mode = BindingMode.TwoWay;
                    //    mi.SetBinding(RibbonMenuItem.IsCheckedProperty, b);                       
                    //    mi.Image = (e as RibbonMenuItem).Image;
                    //    mi.Header = (e as RibbonMenuItem).Header;
                    //    mi.IsCheckable = rm.IsCheckable;
                    //}
                    //host.Children.Add(mi);
                    HasMenuItems = true;
                }
            }
        }


        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            itemsHost.Children.Clear();
            if (overflowHost != null) overflowHost.Children.Clear();
            CreateMenuItems();
            foreach (UIElement e in GetToolbarItems())
            {
                RibbonBar.SetSize(e, RibbonSize.Small);
                itemsHost.Children.Add(e);
            }
            HasOverflowItems = false;
            Size maxSize = base.MeasureOverride(new Size(double.PositiveInfinity, constraint.Height));
            while (maxSize.Width > constraint.Width)
            {
                int n = itemsHost.Children.Count;
                if (n == 0) break;
                UIElement e = itemsHost.Children[n - 1];
                InvalidateAncestorMeasure(e);
                itemsHost.Children.RemoveAt(n - 1);
                if (overflowHost != null)
                {
                    overflowHost.Children.Insert(0, e);
                    HasOverflowItems = true;
                }
                maxSize = base.MeasureOverride(new Size(double.PositiveInfinity, constraint.Height));
            }
            Size size = base.MeasureOverride(constraint);
            return size;
        }

        private void InvalidateAncestorMeasure(DependencyObject obj)
        {
            UIElement element = obj as UIElement;
            if (element != null)
            {
                element.InvalidateMeasure();
            }
            if ((obj != this) && (obj != null))
            {
                this.InvalidateAncestorMeasure(VisualTreeHelper.GetParent(obj));
            }
        }


        /// <summary>
        /// Specifies where the QuickAccessToolBar is supposed to be placed.
        /// Depending on this information, the layout and design may vary.
        /// This is a dependency property.
        /// </summary>
        public QAPlacement ToolBarPlacement
        {
            get { return (QAPlacement)GetValue(ToolBarPlacementProperty); }
            set { SetValue(ToolBarPlacementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Placement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolBarPlacementProperty =
            DependencyProperty.Register("ToolBarPlacement", typeof(QAPlacement), typeof(RibbonQAToolBar),
            new FrameworkPropertyMetadata(QAPlacement.Top,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));





        /// <summary>
        /// Gets or sets whether the overflow panel button is visible.
        /// This is a dependency property.
        /// </summary>
        public bool HasOverflowItems
        {
            get { return (bool)GetValue(IsOverflowVisibleProperty); }
            set { SetValue(IsOverflowVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDropDownVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOverflowVisibleProperty =
            DependencyProperty.Register("HasOverflowItems", typeof(bool), typeof(RibbonQAToolBar), 
            new UIPropertyMetadata(false, HasOverflowItemsPropertyChanged));

        private static void HasOverflowItemsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonQAToolBar toolbar = (RibbonQAToolBar)o;
            toolbar.CreateMenuItems();
        }

        /// <summary>
        /// Gets or sets whether the overflow panel is open.
        /// This is a dependency property.
        /// </summary>
        public bool IsOverflowOpen
        {
            get { return (bool)GetValue(IsOverflowOpenProperty); }
            set { SetValue(IsOverflowOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDropDownVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOverflowOpenProperty =
            DependencyProperty.Register("IsOverflowOpen", typeof(bool), typeof(RibbonQAToolBar), new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OverflowOpenPropertyChanged));

        public static void OverflowOpenPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonQAToolBar tb = (RibbonQAToolBar)o;
            tb.IsMenuOpen=false;
        }



        /// <summary>
        /// Gets or sets whether the menu is open.
        /// This is a dependency property.
        /// </summary>
        public bool IsMenuOpen
        {
            get { return (bool)GetValue(IsMenuOpenProperty); }
            set { SetValue(IsMenuOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMenuOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMenuOpenProperty =
            DependencyProperty.Register("IsMenuOpen", typeof(bool), typeof(RibbonQAToolBar),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        /// <summary>
        /// Gets or sets whether the menu button is visible.
        /// This is a dependency property.
        /// </summary>
        public bool HasMenuItems
        {
            get { return (bool)GetValue(HasMenuItemsProperty); }
            set { SetValue(HasMenuItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasMenuItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasMenuItemsProperty =
            DependencyProperty.Register("HasMenuItems", typeof(bool), typeof(RibbonQAToolBar), new UIPropertyMetadata(false));





        /// <summary>
        /// Gets or sets the header that appears in the menu.
        /// </summary>
        public object MenuHeader
        {
            get { return (object)GetValue(MenuHeaderProperty); }
            set { SetValue(MenuHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuHeaderProperty =
            DependencyProperty.Register("MenuHeader", typeof(object), typeof(RibbonQAToolBar), new UIPropertyMetadata(null));




        public DataTemplate MenuHeaderTemplate
        {
            get { return (DataTemplate)GetValue(MenuHeaderTemplateProperty); }
            set { SetValue(MenuHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuHeaderTemplateProperty =
            DependencyProperty.Register("MenuHeaderTemplate", typeof(DataTemplate), typeof(RibbonQAToolBar), new UIPropertyMetadata(null));



        /// <summary>
        /// Gets or sets the footer that appears in the menu panel.
        /// </summary>
        public object MenuFooter
        {
            get { return (object)GetValue(MenuFooterProperty); }
            set { SetValue(MenuFooterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupFooter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuFooterProperty =
            DependencyProperty.Register("MenuFooter", typeof(object), typeof(RibbonQAToolBar), new UIPropertyMetadata(null));




        public DataTemplate MenuFooterTemplate
        {
            get { return (DataTemplate)GetValue(MenuFooterTemplateProperty); }
            set { SetValue(MenuFooterTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MenuFooterTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuFooterTemplateProperty =
            DependencyProperty.Register("MenuFooterTemplate", typeof(DataTemplate), typeof(RibbonQAToolBar), new UIPropertyMetadata(null));




        [AttachedPropertyBrowsableForChildren]
        public static QAItemPlacement GetPlacement(DependencyObject obj)
        {
            return (QAItemPlacement)obj.GetValue(PlacementProperty);
        }

        public static void SetPlacement(DependencyObject obj, QAItemPlacement value)
        {
            obj.SetValue(PlacementProperty, value);
        }

        // Using a DependencyProperty as the backing store for Placement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached("Placement", typeof(QAItemPlacement), typeof(RibbonQAToolBar), new UIPropertyMetadata(QAItemPlacement.ToolBar));




        public DataTemplate ItemsTemplate
        {
            get { return (DataTemplate)GetValue(ItemsTemplateProperty); }
            set { SetValue(ItemsTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsTemplateProperty =
            DependencyProperty.Register("ItemsTemplate", typeof(DataTemplate), typeof(RibbonQAToolBar), new UIPropertyMetadata(null));


    }
}
