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
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Diagnostics;
using System.ComponentModel;
using Odyssey.Controls.Classes;
using Odyssey.Controls.Ribbon.Interfaces;
using System.Collections.Specialized;
using System.Collections;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    [ContentProperty("Tabs")]
    [TemplatePart(Name = partGroupPanel)]
    [TemplatePart(Name = partPopup)]
    [TemplatePart(Name = partTabItemContainer)]
    [TemplatePart(Name = partPopupGroupPanel)]
    public partial class RibbonBar : Control
    {
        const string partGroupPanel = "PART_GroupPanel";
        const string partPopupGroupPanel = "PART_PopupGroupPanel";
        const string partPopup = "PART_Popup";
        const string partTabItemContainer = "PART_TabItemContainer";

        static RibbonBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonBar), new FrameworkPropertyMetadata(typeof(RibbonBar)));
            RegisterCommands();
        }

        public RibbonBar()
            : base()
        {
            AddHandler(LoadedEvent, new RoutedEventHandler(OnLoaded));
            AddHandler(Button.ClickEvent, new RoutedEventHandler(OnChildClick));
            AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(OnMenuItemClick2));
            AddHandler(RibbonComboBox.DropDownClosedEvent, new RoutedEventHandler(OnMenuItemClick));
            AddHandler(RibbonSplitButton.ClickEvent, new RoutedEventHandler(OnMenuItemClick2));
            AddHandler(RibbonGallery.SelectionChangedEvent, new RoutedEventHandler(OnMenuItemClick));
            RegisterHandlers();
        }

        /// <summary>
        /// Closes any popups when some known routed events occured.
        /// </summary>
        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (popup != null && !(e.OriginalSource is RibbonDropDownButton)) popup.IsOpen = false;
            IsMenuOpen = false;
            RibbonGroup.CloseOpenedPopup();
        }

        /// <summary>
        /// Closes any popups when some known routed events occured.
        /// </summary>
        private void OnMenuItemClick2(object sender, RoutedEventArgs e)
        {
            if (popup != null) popup.IsOpen = false;
            IsMenuOpen = false;
            RibbonGroup.CloseOpenedPopup();
        }


        /// <summary>
        /// Closes any popups when some known routed events occured.
        /// </summary>
        private void OnChildClick(object sender, RoutedEventArgs e)
        {
            if (((e.OriginalSource is IRibbonButton) || (e.OriginalSource is MenuItem)))
            {
                RibbonButton btn = e.OriginalSource as RibbonButton;
                if (btn!= null)
                {
                    if (btn.TemplatedParent is RibbonGallery)
                    {
                        return;
                    }
                }
                RibbonGroup.CloseOpenedPopup();

                if (popup != null)
                {
                    // check if the source is either the left or right scrollbar button for the overlapped tab and don't
                    // collapse the ribbon in that case:
                    RibbonButton b = e.OriginalSource as RibbonButton;
                    if (b != null)
                    {
                        ICommand command = b.Command;
                        if (command == RibbonTabScroller.ScrollRightCommand || command == RibbonTabScroller.ScrollLeftCommand) return;
                    }

                    popup.IsOpen = false;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AddHandler(RibbonBar.SelectedTabIndexChangedEvent, new RoutedEventHandler(SelectedIndexChanged));
            if (SelectedTabIndex >= 0) SelectedIndexChanged(this, e);
        }

        private Control groupPanel;
        private Control popupGroupPanel;
        private Popup popup;
        private Panel tabItemContainer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            groupPanel = GetTemplateChild(partGroupPanel) as Control;
            popupGroupPanel = GetTemplateChild(partPopupGroupPanel) as Control;

            if (popup != null)
            {
                popup.Opened -= OnPopupOpened;
                popup.Closed -= OnPopupClosed;
            }


            popup = GetTemplateChild(partPopup) as Popup;

            if (popup != null)
            {
                popup.Opened += new EventHandler(OnPopupOpened);
                popup.Closed += new EventHandler(OnPopupClosed);
            }

            if (tabItemContainer != null) tabItemContainer.Children.Clear();
            tabItemContainer = GetTemplateChild(partTabItemContainer) as Panel;
            if (tabItemContainer != null)
            {
                foreach (RibbonTabItem tab in Tabs)
                {
                    tabItemContainer.Children.Add(tab);
                }
                SetContextualTabs(ContextualTabSet);
            }

        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            if (!this.IsKeyboardFocusWithin)
            {
                DependencyObject focused = Keyboard.FocusedElement as DependencyObject;
                if (focused == null || !this.IsAncestorOf(focused))
                {
                    IsMenuOpen = false;
                }
            }
        }


        protected virtual void OnPopupOpened(object sender, EventArgs e)
        {
            MeasureGroups();
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        protected virtual void OnPopupClosed(object sender, EventArgs e)
        {
            IsMenuOpen = false;
            IsExpanded = false;
            Mouse.Capture(null);
        }


        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            FrameworkElement parent = oldParent as FrameworkElement;
            if (parent != null) parent.SizeChanged -= ParentSizeChanged;
            base.OnVisualParentChanged(oldParent);

            parent = this.Parent as FrameworkElement;
            if (parent != null)
            {
                parent.SizeChanged += new SizeChangedEventHandler(ParentSizeChanged);
            }
        }

        public static Size InfiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        void ParentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged) MeasureGroups();
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);
            RibbonContextualTabSet set = ContextualTabSet;
            if (set != null)
            {
                double w = 0.0;
                foreach (var tab in set.Tabs) w += tab.DesiredSize.Width;
                set.Width = w;
            }
            return size;
        }


        const double MIN_RIBBON_WIDTH = 240;

        private void MeasureGroups()
        {
            Control parent = null;
            if (CanMinimize)
            {
                parent = popupGroupPanel != null ? popupGroupPanel.Parent as Control : null;
            }
            else
            {
                parent = groupPanel != null ? groupPanel.Parent as Control : null;
            }
            double actualWidth = parent != null ? parent.ActualWidth : this.ActualWidth;
            IsMinimized = ActualWidth < MIN_RIBBON_WIDTH;
            if (!IsMinimized)
            {
                double left = CalculateGroupsWidth();

                left = ExpandOrReduceGroups(left, actualWidth);

                AdjustGroupAlignment(left, actualWidth);
            }
        }

        private void AdjustGroupAlignment(double left, double actualWidth)
        {
            if (left > actualWidth)
            {
                if (GroupAlignment == RibbonBarAlignment.Full) GroupAlignment = RibbonBarAlignment.Left;
            }
            else
            {
                GroupAlignment = RibbonBarAlignment.Full;
            }
        }

        private double ExpandOrReduceGroups(double left, double actualWidth)
        {
            int level = GetMaxLevel();
            while (level >= 0 && left < actualWidth)
            {
                left = ExpandGroups(left, actualWidth, level--);
            }

            if (left > actualWidth)
            {
                level = GetMinLevel();
                while (CanReduce() && left > actualWidth)
                {
                    left = ReduceGroups(left, actualWidth, level++);
                }
            }
            return left;
        }

        private double CalculateGroupsWidth()
        {
            double width = 0;

            foreach (RibbonGroup group in GetSelectedGroups())
            {
                if (!group.IsMeasureValid)
                {
                    group.Measure(InfiniteSize);
                }
                width += group.DesiredSize.Width;
            }
            return width;
        }

        private int GetMinLevel()
        {
            int level = int.MaxValue;
            foreach (RibbonGroup g in GetWrapPanelsByReductionOrder())
            {
                level = Math.Min(level, g.ReductionLevel);
                if (level == 0) break;
            }
            return level;
        }

        private int GetMaxLevel()
        {
            int level = 0;
            foreach (RibbonGroup g in GetWrapPanelsByReductionOrder())
            {
                level = Math.Max(level, g.ReductionLevel);
            }
            return level;
        }

        private bool CanExpand()
        {
            foreach (RibbonGroup g in GetWrapPanelsByReductionOrder())
            {
                if (g.ReductionLevel > 0) return true;
            }
            return false;
        }

        private bool CanReduce()
        {
            foreach (RibbonGroup g in GetWrapPanelsByReductionOrder())
            {
                if (!g.IsMinimized) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the selected TabItem
        /// This is a dependency property.
        /// </summary>
        public RibbonTabItem SelectedTabItem
        {
            get { return GetTabItemFromIndex(SelectedTabIndex); }
            set
            {
                Select(value);
            }
        }

        private double ReduceGroups(double left, double actualWidth, int level)
        {
            if (left > actualWidth)
            {
                foreach (RibbonGroup group in GetWrapPanelsByReductionOrder())
                {
                    if (group.ReductionLevel <= level)
                    {
                        double w0 = group.DesiredSize.Width;
                        group.Reduce();
                        double w1 = group.DesiredSize.Width;
                        double dw = w0 - w1;

                        left -= dw;
                    }
                    if (left <= actualWidth) break;
                }
            }
            return left;
        }

        private double ExpandGroups(double left, double actualWidth, int level)
        {
            foreach (RibbonGroup group in GetWrapPanelsByReductionOrder().Reverse())
            {
                if (group.ReductionLevel > level)
                {
                    double w0 = group.DesiredSize.Width;
                    group.Expand();
                    group.UpdateLayout();
                    double w1 = group.DesiredSize.Width;
                    double dw = w0 - w1;

                    left -= dw;
                }
                if (left > actualWidth) break;
            }

            return left;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            CheckPopupTabToClose(e);
        }

        private void CheckPopupTabToClose(MouseEventArgs e)
        {
            if (popup == null) return;
            if (!CanMinimize) return;
            if (Mouse.Captured != this && popup != null)
            {
                UIElement child = this.popup.Child;
                if (e.OriginalSource == this)
                {
                    if ((Mouse.Captured == null) || !child.IsAncestorOf(Mouse.Captured as DependencyObject))
                    {
                        this.IsExpanded = false;
                        e.Handled = true;
                    }
                }
                else if (child.IsAncestorOf(e.OriginalSource as DependencyObject))
                {
                    if (this.IsExpanded && (Mouse.Captured == null))
                    {
                        Mouse.Capture(this, CaptureMode.SubTree);
                        e.Handled = true;
                    }
                }
                else
                {
                    this.IsExpanded = false;
                    e.Handled = true;
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (e.OriginalSource == this)
            {
                Mouse.Capture(null);
            }
            if (IsExpanded && CanMinimize)
            {
                if (this.CanMinimize)
                {
                    if (!(e.Source is MenuItem) && !(e.Source is RibbonThumbnail))
                    {
                   //     RibbonDropDownButton.CloseOpenedPopup(null);
                    }
                }
            }
        }


        private IEnumerable<RibbonGroup> GetWrapPanelsByReductionOrder()
        {
            RibbonTabItem item = SelectedTabItem;
            if (item != null && item.ReductionOrder != null)
            {
                return GetWrapPanelsFromCollection(item.ReductionOrder);
            }
            return GetDefaultWrapPanelOrder();
        }

        private IEnumerable<RibbonGroup> GetDefaultWrapPanelOrder()
        {
            if (SelectedGroups != null)
            {
                for (int i = this.SelectedGroups.Count - 1; i >= 0; i--)
                {
                    RibbonGroup group = SelectedGroups[i];
                    if (group != null) yield return group;
                }
            }
        }


        private IEnumerable<RibbonGroup> GetWrapPanelsFromCollection(StringCollection names)
        {
            if (SelectedGroups != null)
            {
                Dictionary<string, RibbonGroup> d = (from g in SelectedGroups where !string.IsNullOrEmpty(g.Name) select g).ToDictionary(x => x.Name, x => x);
                foreach (string name in names)
                {
                    if (d.ContainsKey(name)) yield return d[name];
                }
            }
        }


        private void SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            IsMenuOpen = false;
            if (SelectedTabIndex >= 0)
            {
                RibbonTabItem item = GetTabItemFromIndex(SelectedTabIndex);
                SetSelectedTabItem(item);
            }
            MeasureGroups();
        }

        private RibbonTabItem GetTabItemFromIndex(int index)
        {
            if (index < Tabs.Count && Tabs.Count > 0) return Tabs[index];

            index -= Tabs.Count;
            if (ContextualTabSet != null)
            {
                int c = ContextualTabSet.Tabs.Count;
                if (c > 0 && c > index) return ContextualTabSet.Tabs[index];
            }
            return null;
        }

        private void SetSelectedTabItem(RibbonTabItem item)
        {
            SelectedGroups = item != null ? item.Groups : null;
            int index = SelectedTabIndex;
            for (int i = 0; i < Tabs.Count; i++)
            {
                item = Tabs[i];
                item.IsSelected = i == index;
            }
            RibbonContextualTabSet currentSet = ContextualTabSet;
            if (currentSet != null)
            {
                index -= Tabs.Count;
                currentSet.SetSelectedTabItem(index);
            }
            if (contextualTabSets != null)
            {
                foreach (var set in contextualTabSets)
                {
                    if (set != currentSet)
                    {
                        set.SetSelectedTabItem(-1);
                    }
                }
            }
        }

        IEnumerable<RibbonGroup> GetSelectedGroups()
        {
            if (SelectedGroups != null) return SelectedGroups;
            return new RibbonGroup[0];
        }


        /// <summary>
        /// Gets the groups of the selected tab item. 
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// This dependency property is required for the ControlTemplate to attach the groups.
        /// </remarks>
        public Collection<RibbonGroup> SelectedGroups
        {
            get { return (Collection<RibbonGroup>)GetValue(SelectedGroupsProperty); }
            private set { SetValue(SelectedGroupsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedGroup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedGroupsProperty =
            DependencyProperty.Register("SelectedGroups", typeof(Collection<RibbonGroup>), typeof(RibbonBar),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    SelectedGroupsPropertyChanged));

        public static void SelectedGroupsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)o;
            bar.OnSelectedGroupsChanged(e);
        }

        protected virtual void OnSelectedGroupsChanged(DependencyPropertyChangedEventArgs e)
        {
        }


        /// <summary>
        /// Selects the specified Tabitem.
        /// </summary>
        private void Select(RibbonTabItem tabItem)
        {
            int index = GetTabIndex(tabItem);
            bool isCurrent = index == this.SelectedTabIndex;
            this.SelectedTabIndex = index;
            if (CanMinimize)
            {
                if (isCurrent && IsExpanded) this.IsExpanded ^= true; else this.IsExpanded = true;
            }
        }

        private int GetTabIndex(RibbonTabItem tabItem)
        {
            int index = Tabs.IndexOf(tabItem);
            if (index >= 0) return index;

            if (ContextualTabSet != null)
            {
                index = ContextualTabSet.Tabs.IndexOf(tabItem);
                return index + Tabs.Count;
            }
            return -1;
        }



        /// <summary>
        /// Gets or sets whether the tab is expanded when <see>CanMinimize</see> is set to true.
        /// This is a dependency property.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(RibbonBar),
            new UIPropertyMetadata(false, ExpandedPropertyChanged));


        static void ExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)d;
            bar.OnExpandedChanged(e);
        }

        /// <summary>
        /// Occurs when the IsExpanded property has changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            bool isExpanded = (bool)e.NewValue;
            if (popup != null)
            {
                if (GroupAlignment == RibbonBarAlignment.Right) GroupAlignment = RibbonBarAlignment.Left;
                popup.IsOpen = isExpanded;
                if (popup.IsOpen)
                {
                }
            }
        }


        internal RibbonBarAlignment GroupAlignment
        {
            get { return (RibbonBarAlignment)GetValue(GroupAlignmentProperty); }
            set { SetValue(GroupAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupAlignment.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty GroupAlignmentProperty =
            DependencyProperty.Register("GroupAlignment", typeof(RibbonBarAlignment), typeof(RibbonBar),
            new UIPropertyMetadata(RibbonBarAlignment.Full, GroupAlignmentPropertyChanged));


        public static void GroupAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)d;
            RibbonBarAlignment alignment = (RibbonBarAlignment)e.NewValue;

            bar.SetGroupAlignment(alignment);
        }

        protected virtual void SetGroupAlignment(RibbonBarAlignment alignment)
        {
        }



        /// <summary>
        /// Gets or sets whether the ribbon bar can minimize the tab.
        /// This is a dependency property.
        /// </summary>
        public bool CanMinimize
        {
            get { return (bool)GetValue(CanMinimizeProperty); }
            set { SetValue(CanMinimizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanMinimize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.Register("CanMinimize", typeof(bool), typeof(RibbonBar),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender,
                MinimizePropertyChanged));


        private static void MinimizePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)o;
            bool canMinimize = (bool)e.NewValue;
        }

        /// <summary>
        /// Gets or sets the index of the selected tab.
        /// The index is equivalent to the order of the visual tab items, including possible contextual tabs.
        /// This is a dependency property.
        /// </summary>
        public int SelectedTabIndex
        {
            get { return (int)GetValue(SelectedTabIndexProperty); }
            set { SetValue(SelectedTabIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTabIndexProperty =
            DependencyProperty.Register("SelectedTabIndex", typeof(int), typeof(RibbonBar), new UIPropertyMetadata(0, SelectedTabIndexPropertyChanged));


        public static readonly RoutedEvent SelectedTabIndexChangedEvent = EventManager.RegisterRoutedEvent("SelectedTabIndexChanged",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RibbonBar));


        static void SelectedTabIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)d;
            RoutedEventArgs args = new RoutedEventArgs(SelectedTabIndexChangedEvent);
            bar.RaiseEvent(args);
            bar.OnSelectedTabIndexChanged(e);
        }

        protected virtual void OnSelectedTabIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            Color = SelectedTabItem != null && SelectedTabItem.tabSet != null ? SelectedTabItem.tabSet.Color : Colors.Transparent;
        }



        private ObservableCollection<RibbonTabItem> tabs;

        /// <summary>
        /// Gets the collection of RibbonTabItems.
        /// </summary>
        public Collection<RibbonTabItem> Tabs
        {
            get
            {
                if (tabs == null)
                {
                    tabs = new ObservableCollection<RibbonTabItem>();
                    tabs.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnTabItemCollectionChanged);
                }
                return tabs;
            }
        }


        /// <summary>
        /// Occurs when the TabItems collection has changed.
        /// </summary>
        protected virtual void OnTabItemCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (RibbonTabItem tab in e.NewItems)
                    {
                        tab.RibbonBar = this;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (RibbonTabItem tab in e.OldItems)
                    {
                        tab.RibbonBar = null;
                    }
                    break;
            }
        }

        private ObservableCollection<RibbonContextualTabSet> contextualTabSets;

        /// <summary>
        /// Gets the collection of ContextualTabSets.
        /// </summary>
        public Collection<RibbonContextualTabSet> ContextualTabSets
        {
            get
            {
                if (contextualTabSets == null)
                {
                    contextualTabSets = new ObservableCollection<RibbonContextualTabSet>();
                    contextualTabSets.CollectionChanged += new NotifyCollectionChangedEventHandler(OnContextualTabSetsChanged);
                }
                return contextualTabSets;
            }
        }

        /// <summary>
        /// Occurs when the ContextualTabSets collection has changed.
        /// </summary>
        protected virtual void OnContextualTabSetsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (RibbonContextualTabSet set in e.NewItems)
                    {
                        set.RibbonBar = this;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (RibbonContextualTabSet set in e.OldItems)
                    {
                        set.RibbonBar = null;
                    }
                    break;
            }
        }



        /// <summary>
        /// Gets or sets the selected ContextualTabSet.
        /// This is a dependency property.
        /// </summary>
        public RibbonContextualTabSet ContextualTabSet
        {
            get { return (RibbonContextualTabSet)GetValue(ContextualTabSetProperty); }
            set { SetValue(ContextualTabSetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextualTabSet.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextualTabSetProperty =
            DependencyProperty.Register("ContextualTabSet", typeof(RibbonContextualTabSet), typeof(RibbonBar),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                ContextualTabSetPropertyChanged));

        public static void ContextualTabSetPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)o;
            bar.OnContextualTabSetChanged(e);
        }

        protected virtual void OnContextualTabSetChanged(DependencyPropertyChangedEventArgs e)
        {
            RibbonContextualTabSet oldSet = e.OldValue as RibbonContextualTabSet;
            RibbonContextualTabSet newSet = e.NewValue as RibbonContextualTabSet;

            RemovePreviousContextualTabs(oldSet);
            SetContextualTabs(newSet);

            AdjustContextualTabSet(e.NewValue as RibbonContextualTabSet);
            int index = GetTabIndex(SelectedTabItem);
            if (index < 0) SelectedTabIndex = 0;

        }

        private void AdjustContextualTabSet(RibbonContextualTabSet set)
        {
        }

        private void SetContextualTabs(RibbonContextualTabSet newSet)
        {
            if (newSet != null) newSet.IsSelected = true;
            if (tabItemContainer != null && newSet != null)
            {
                foreach (RibbonTabItem tab in newSet.Tabs)
                {
                    tabItemContainer.Children.Add(tab);
                    tab.IsContextual = true;
                }
            }
        }

        private void RemovePreviousContextualTabs(RibbonContextualTabSet oldSet)
        {
            if (oldSet != null) oldSet.IsSelected = false;
            if (tabItemContainer != null && oldSet != null)
            {
                foreach (RibbonTabItem tab in oldSet.Tabs)
                {
                    tab.IsContextual = false;
                    tabItemContainer.Children.Remove(tab);
                }
            }
        }



        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An enumerator for logical child elements of this element.
        /// </returns>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                return new RibbonBarEnumerator(this);
            }
        }

        #region LogicalChildrenEnumerator
        class RibbonBarEnumerator : IEnumerator
        {
            public RibbonBarEnumerator(RibbonBar bar)
                : base()
            {
                this.bar = bar;
                Reset();
            }
            private RibbonBar bar;


            IEnumerable LogicalChildren(RibbonBar bar)
            {
                if (bar.ApplicationMenu != null) yield return bar.ApplicationMenu;
                foreach (var tab in bar.Tabs) yield return tab;
                //                foreach (var set in bar.ContextualTabSets) yield return set;
            }

            #region IEnumerator Members

            private object current;
            public object Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                if (enumeration.Count > index)
                {
                    current = enumeration[index++];
                }
                else
                {
                    current = null;
                }
                return current != null;
            }

            List<object> enumeration = new List<object>();
            int index = 0;

            public void Reset()
            {
                enumeration.Clear();
                foreach (object o in LogicalChildren(bar))
                {
                    enumeration.Add(o);
                }
                index = 0;
                current = enumeration.Count > 0 ? enumeration[0] : null;

            }

            #endregion
        }
        #endregion

        private void AlignGroupsLeft()
        {
            GroupAlignment = RibbonBarAlignment.Left;
            InvalidateArrange();
        }

        private void AlignGroupsRight()
        {
            GroupAlignment = RibbonBarAlignment.Right;
            InvalidateArrange();
        }


        /// <summary>
        /// Gets or sets the RibbonApplicationMenu for the RibbonBar.
        /// This is a dependency property.
        /// </summary>
        public RibbonApplicationMenu ApplicationMenu
        {
            get { return (RibbonApplicationMenu)GetValue(ApplicationMenuProperty); }
            set { SetValue(ApplicationMenuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ApplicationMenu.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ApplicationMenuProperty =
            DependencyProperty.Register("ApplicationMenu", typeof(RibbonApplicationMenu), typeof(RibbonBar), new UIPropertyMetadata(null));



        /// <summary>
        /// Gets or sets whether the ApplicationMenu is open.
        /// Thisis a dependency property.
        /// </summary>
        public bool IsMenuOpen
        {
            get { return (bool)GetValue(IsMenuOpenProperty); }
            set { SetValue(IsMenuOpenProperty, value); }
        }

        public static readonly DependencyProperty IsMenuOpenProperty =
            DependencyProperty.Register("IsMenuOpen", typeof(bool), typeof(RibbonBar), new UIPropertyMetadata(false));

        [AttachedPropertyBrowsableForChildren]
        public static RibbonSize GetSize(DependencyObject obj)
        {
            return (RibbonSize)obj.GetValue(SizeProperty);
        }

        public static void SetSize(DependencyObject obj, RibbonSize value)
        {
            obj.SetValue(SizeProperty, value);
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.RegisterAttached("Size", typeof(RibbonSize), typeof(RibbonBar),
            new FrameworkPropertyMetadata(RibbonSize.Large,
                FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Gets the custom Reduction sizes for a IRibbonControl.
        /// This is an attached dependency property.
        /// </summary>
        [AttachedPropertyBrowsableForChildren]
        public static RibbonSizeCollection GetReduction(DependencyObject obj)
        {
            return (RibbonSizeCollection)obj.GetValue(ReductionProperty);
        }

        /// <summary>
        /// Sets the custom Reduction sizes for a IRibbonControl.
        /// This is an attached dependency property.
        /// </summary>
        [TypeConverter(typeof(RibbonReductionCollectionConverter))]
        public static void SetReduction(DependencyObject obj, RibbonSizeCollection value)
        {
            obj.SetValue(ReductionProperty, value);
        }

        public static readonly DependencyProperty ReductionProperty =
            DependencyProperty.RegisterAttached("Reduction", typeof(RibbonSizeCollection), typeof(RibbonBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the minimum size for a IRibbonControl.
        /// This is an attached dependency property.
        /// </summary>        
        public static RibbonSize GetMinSize(DependencyObject obj)
        {
            return (RibbonSize)obj.GetValue(MinSizeProperty);
        }

        /// <summary>
        /// Sets the minimum size for a IRibbonControl.
        /// This is an attached dependency property.
        /// </summary>        
        public static void SetMinSize(DependencyObject obj, RibbonSize value)
        {
            obj.SetValue(MinSizeProperty, value);
        }

        public static readonly DependencyProperty MinSizeProperty =
            DependencyProperty.RegisterAttached("MinSize", typeof(RibbonSize), typeof(RibbonBar), new UIPropertyMetadata(RibbonSize.Minimized));

        /// <summary>
        /// Gets the maximum size for a IRibbonControl.
        /// This is an attached dependency property.
        /// </summary>        
        public static RibbonSize GetMaxSize(DependencyObject obj)
        {
            return (RibbonSize)obj.GetValue(MaxSizeProperty);
        }

        /// <summary>
        /// Sets the maximum size for a IRibbonControl.
        /// This is an attached dependency property.
        /// </summary>        
        public static void SetMaxSize(DependencyObject obj, RibbonSize value)
        {
            obj.SetValue(MaxSizeProperty, value);
        }

        public static readonly DependencyProperty MaxSizeProperty =
            DependencyProperty.RegisterAttached("MaxSize", typeof(RibbonSize), typeof(RibbonBar), new UIPropertyMetadata(RibbonSize.Large));

        /// <summary>
        /// Gets the color for the active tab.
        /// This is a dependency property.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            private set { SetValue(ColorPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ColorPropertyKey =
            DependencyProperty.RegisterReadOnly("Color", typeof(Color), typeof(RibbonBar), new UIPropertyMetadata(Colors.Transparent));


        public static readonly DependencyProperty ColorProperty = ColorPropertyKey.DependencyProperty;


        /// <summary>
        /// Gets or sets the RibbonTitle which is also the windows title if hosted in a RibbonWindow.
        /// This is a dependency property.
        /// </summary>
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(RibbonBar), new UIPropertyMetadata(null));



        /// <summary>
        /// Gets or sets the QuickAccess Toolbar.
        /// This is a dependency property.
        /// </summary>
        public RibbonQAToolBar QAToolBar
        {
            get { return (RibbonQAToolBar)GetValue(QAToolBarProperty); }
            set { SetValue(QAToolBarProperty, value); }
        }

        public static readonly DependencyProperty QAToolBarProperty =
            DependencyProperty.Register("QAToolBar", typeof(RibbonQAToolBar), typeof(RibbonBar), new UIPropertyMetadata(null));



        /// <summary>
        /// Gets or sets the placement for the QuickAccess Toolbar.
        /// This is a dependency property.
        /// </summary>
        public QAPlacement ToolbarPlacement
        {
            get { return (QAPlacement)GetValue(ToolbarPlacementProperty); }
            set { SetValue(ToolbarPlacementProperty, value); }
        }

        public static readonly DependencyProperty ToolbarPlacementProperty =
            DependencyProperty.Register("ToolbarPlacement", typeof(QAPlacement), typeof(RibbonBar),
            new FrameworkPropertyMetadata(QAPlacement.Top,
                FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                ToolbarPlacementPropertyChanged));

        public static void ToolbarPlacementPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)o;
            if (bar.QAToolBar != null) bar.QAToolBar.ToolBarPlacement = (QAPlacement)e.NewValue;
        }




        /// <summary>
        /// Gets whether the ribbon bar and tabs are visible.
        /// If set to false, only the ApplicationMenu and the QuickAccessToolbar is available.
        /// This is a dependency property.
        /// </summary>
        public bool IsRibbonVisible
        {
            get { return (bool)GetValue(IsRibbonVisibleProperty); }
            set { SetValue(IsRibbonVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsRibbonVisibleProperty =
            DependencyProperty.Register("IsRibbonVisible", typeof(bool), typeof(RibbonBar), new FrameworkPropertyMetadata(true));


        /// <summary>
        /// Gets whether the Ribbonbar is minimized due to a thresold underrun of the minimum width.
        /// This is a dependency property.
        /// </summary>
        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            private set { SetValue(IsMinimizedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsMinimizedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsMinimized", typeof(bool), typeof(RibbonBar), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsMinimizedProperty = IsMinimizedPropertyKey.DependencyProperty;


    }
}
