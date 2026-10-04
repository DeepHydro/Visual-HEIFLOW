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
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Odyssey.Controls.Classes;
using Odyssey.Controls.Ribbon.Interfaces;
using System.Collections.Specialized;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{

    [ContentProperty("Controls")]
    [TemplatePart(Name = partItemsPanelHost)]
    [TemplatePart(Name = partPopupItemsPanelHost)]
    [TemplatePart(Name = partPopup)]
    public partial class RibbonGroup:Control
    {

        const string partItemsPanelHost = "PART_ItemsPanelHost";
        const string partPopupItemsPanelHost = "PART_PopupItemsPanelHost";
        const string partPopup = "PART_Popup";

        static RibbonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroup), new FrameworkPropertyMetadata(typeof(RibbonGroup)));
            RegisterCommands();
        }


        public RibbonGroup()
            : base()
        {
            controls.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnControlCollectionChanged);
            RegisterHandlers();
        }

        /// <summary>
        /// Occurs when the Controls collection has changed.
        /// </summary>
        protected virtual void OnControlCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            reducableControlIndexes = null;
            ChangeControlSizes();
        }

        private Panel itemPanelInstance = null;
        private Panel ItemPanelInstance
        {
            get
            {
                if (itemPanelInstance == null)
                {
                    if (ItemsPanel != null)
                    {
                        itemPanelInstance = ItemsPanel;
                    }
                    else itemPanelInstance = new RibbonWrapPanel();
                }
                return itemPanelInstance;
            }
        }

        private Decorator itemsPanelHost;
        private Decorator popupItemsPanelHost;
        private Popup popup;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ItemPanelInstance != null)
            {
                ItemPanelInstance.Children.Clear();
            }

            if (ItemPanelInstance != null)
            {
                foreach (UIElement e in Controls)
                {
                    ItemPanelInstance.Children.Add(e);
                }
            }

            itemsPanelHost = GetTemplateChild(partItemsPanelHost) as Decorator;
            popupItemsPanelHost = GetTemplateChild(partPopupItemsPanelHost) as Decorator;

            if (popup != null)
            {
                popup.Opened -= OnPopupOpened;
                popup.Closed -= OnPopupClosed;
            }
            popup = GetTemplateChild(partPopup) as Popup;
            if (popup != null)
            {
                popup.StaysOpen = false;
                popup.Opened += new EventHandler(OnPopupOpened);
                popup.Closed += new EventHandler(OnPopupClosed);
            }

            AttachControlsToItemsPanel();
        }


        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            if (!this.IsKeyboardFocusWithin)
            {
                DependencyObject focused = Keyboard.FocusedElement as DependencyObject;
                if (focused==null || !this.IsAncestorOf(focused))
                {
                    IsDropDownOpen = false;
                }
            }
        }



        public static RibbonGroup PoppedUpGroup;

        void OnPopupClosed(object sender, EventArgs e)
        {
            if (PoppedUpGroup.popup == sender) PoppedUpGroup = null;
            Mouse.Capture(null);
        }

        void OnPopupOpened(object sender, EventArgs e)
        {
            if (PoppedUpGroup != null && PoppedUpGroup.popup != null && PoppedUpGroup.popup != sender)
            {
                PoppedUpGroup.popup.IsOpen = false;
            }
            PoppedUpGroup = this;
           Mouse.Capture(this, CaptureMode.SubTree);
        }



        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RibbonGroup), new UIPropertyMetadata(false));



        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            set { SetValue(IsMinimizedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMinimized.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMinimizedProperty =
            DependencyProperty.Register("IsMinimized", typeof(bool), typeof(RibbonGroup),
            new UIPropertyMetadata(false, MinimizedPropertyChanged));

        public static void MinimizedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroup g = (RibbonGroup)o;
            g.AttachControlsToItemsPanel();
        }


        private void AttachControlsToItemsPanel()
        {

            if (ItemPanelInstance != null)
            {
                Decorator parent = ItemPanelInstance.Parent as Decorator;
                if (parent != null) parent.Child = null;

                Decorator host = IsMinimized ? popupItemsPanelHost : itemsPanelHost;
                if (host != null)
                {
                    host.Child = ItemPanelInstance;
                }
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RibbonGroup), new UIPropertyMetadata(""));



        public bool IsDialogLauncherVisible
        {
            get { return (bool)GetValue(IsDialogLauncherVisibleProperty); }
            set { SetValue(IsDialogLauncherVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDialogLauncherVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDialogLauncherVisibleProperty =
            DependencyProperty.Register("IsDialogLauncherVisible", typeof(bool), typeof(RibbonGroup), new UIPropertyMetadata(false));


        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonGroup), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Panel that contains the Controls.
        /// This is a dependency property.
        /// </summary>
        public Panel ItemsPanel
        {
            get { return (Panel)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(Panel), typeof(RibbonGroup), new UIPropertyMetadata(null, ItemsPanelPropertyChanged));

        static void ItemsPanelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroup grp = (RibbonGroup)d;
            grp.OnItemsPanelChanged(e);
        }

        protected virtual void OnItemsPanelChanged(DependencyPropertyChangedEventArgs e)
        {
            itemPanelInstance = null;
            AttachControlsToItemsPanel();
        }

        //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseLeftButtonDown(e);

        //    if (!(e.Source is MenuItem) && !(e.Source is RibbonThumbnail))
        //    {
        //       RibbonDropDownButton.CloseOpenedPopup(null);
        //    }
        //}

        private ObservableCollection<UIElement> controls = new ObservableCollection<UIElement>();

        /// <summary>
        /// Gets the Collection of controls for this gorup.
        /// </summary>
        public Collection<UIElement> Controls { get { return controls; } }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                return controls.GetEnumerator();
            }
        }

        private void ClosePopup()
        {
            if (popup != null) popup.IsOpen = false;
        }

        public static void CloseOpenedPopup()
        {
            if (PoppedUpGroup != null)
            {
                PoppedUpGroup.ClosePopup();
            }
        }

        static RibbonSizeCollection first = new RibbonSizeCollection { RibbonSize.Large, RibbonSize.Large, RibbonSize.Large, RibbonSize.Minimized };

        int GetGalleryColumns(DependencyObject control, int level, int maxLevels)
        {
            RibbonGalleryColumns columns = RibbonGallery.GetReductionColumns(control);
            if (columns != null && columns.Count > 0)
            {
                level = Math.Max(0, Math.Min(level, columns.Count - 1));
                return columns[level];
            }

            if (level == maxLevels) return 0;

            if (maxLevels > 4)
            {
                level = Math.Max(level, 8);
                return 3 + (4 - level / 2);
            }
            else
            {
                return 3 + (2 - level) * 2;
            }

        }

        /// <summary>
        /// Gets the RibbonSize for a control for a specific level.
        /// </summary>
        /// <param name="control">The control for which to retreive a RibbonSize.</param>
        /// <param name="level">The reduction Level (0=large, 2=medium,3=small,4=minimized,...).</param>
        /// <param name="index">The index of the control in the group.</param>
        /// <returns>The RibbonSize for the control.</returns>
        RibbonSize GetControlSize(DependencyObject control, int level, int index)
        {
            RibbonSizeCollection reductions = RibbonBar.GetReduction(control);
            if (reductions != null && reductions.Count > 0)
            {
                level = Math.Max(0, Math.Min(level, reductions.Count - 1));
                return reductions[level];
            }
            RibbonSize size;
            switch (level)
            {
                case 0: size = GetDefaultSizeForLevel0(index); break;
                case 1: size = GetDefaultSizeForLevel1(index); break;
                case 2: size = GetDefaultSizeForLevel2(index); break;
                default: size = RibbonSize.Minimized; break;
            }

            RibbonSize min = RibbonBar.GetMinSize(control);
            RibbonSize max = RibbonBar.GetMaxSize(control);
            if (size < min) size = min;
            if (size > max) size = max;

            return size;
        }

        private int CountRibbonControls()
        {
            int count = 0;
            foreach (UIElement e in Controls)
            {
                if (e is IRibbonControl) count++;
            }
            return count;
        }

        private RibbonSize GetDefaultSizeForLevel2(int index)
        {
            if (!ReducableControlIndexes.ContainsKey(index)) return RibbonSize.Large;
            return ReducableControlIndexes[index];
        }

        private RibbonSize GetDefaultSizeForLevel1(int index)
        {
            if (!ReducableControlIndexes.ContainsKey(index)) return RibbonSize.Large;
            return RibbonSize.Medium;
        }

        private RibbonSize GetDefaultSizeForLevel0(int index)
        {
            return RibbonSize.Large;
        }


        private int reductionLevel;
        /// <summary>
        /// Gets or sets the reduction level.
        /// </summary>
        public int ReductionLevel
        {
            get { return reductionLevel; }
            set
            {
                if (value < 0) value = 0;
                if (reductionLevel != value)
                {
                    reductionLevel = value;
                    ChangeControlSizes();
                }
            }
        }

        /// <summary>
        /// Resets the ReductionLevel to it's default value (0);
        /// </summary>
        public void ResetSize()
        {
            ReductionLevel = 0;
        }

        /// <summary>
        /// Reduce the ReductionLevel one step.
        /// </summary>
        /// <returns></returns>
        public bool Reduce()
        {
            if (ReductionLevel < GetMaxLevel() + 1)
            {
                ReductionLevel++;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Expand the ReductionLevel one step.
        /// </summary>
        /// <returns></returns>
        public bool Expand()
        {
            if (ReductionLevel > 0)
            {
                ReductionLevel--;
                return true;
            }
            else return false;
        }

        //Gets the maximum possible ReductionLevel that would change any of the controls.
        private int GetMaxLevel()
        {
            int max = 1;
            foreach (UIElement e in Controls)
            {
                if (e is IRibbonControl)
                {
                    RibbonSizeCollection reduction = RibbonBar.GetReduction(e);
                    int m = reduction != null ? reduction.Count : 3;
                    max = Math.Max(max, m);
                }
                if (e is IRibbonGallery)
                {
                    RibbonGalleryColumns columns = RibbonGallery.GetReductionColumns(e);
                    int m = columns != null ? columns.Count : 3;
                    max = Math.Max(max, m);
                }
            }
            return max;
        }


        /// <summary>
        /// Change the RibbonSize for every IRibbonControl inside this group.
        /// </summary>
        private void ChangeControlSizes()
        {
            int level = ReductionLevel;
            int maxLevels = GetMaxLevel();
            bool minimized = level >= maxLevels;
            IsMinimized = minimized;
            if (minimized) level = 0;

            for (int i = 0; i < Controls.Count; i++)
            {
                UIElement e = Controls[i];
                IRibbonGallery gallery = e as IRibbonGallery;
                if (gallery != null)
                {
                    int columns = GetGalleryColumns(e, level, maxLevels);
                    gallery.Columns = columns;
                    gallery.IsCollapsed = columns == 0;
                    gallery.SetDropDownColumns(GetGalleryColumns(e, 0, maxLevels));
                }
                else
                {
                    if (e is IRibbonControl)
                    {
                        RibbonSize size = GetControlSize(e, level, i);
                        RibbonBar.SetSize(e, size);
                    }
                }
            }
            InvalidateMeasure();
            UpdateLayout();

        }


        #region AutoReduction

        class GroupBucket : List<int>
        {
        }

        class GroupBucketCollection : List<GroupBucket>
        {
        }

        GroupBucketCollection GetBuckets()
        {
            GroupBucketCollection buckets = new GroupBucketCollection();
            GroupBucket bucket = new GroupBucket();
            buckets.Add(bucket);

            int index = 0;
            foreach (UIElement e in Controls)
            {
                bool canBeSmall = CanControlBeSmall(e);
                if (!canBeSmall && bucket.Count > 0)
                {
                    bucket = new GroupBucket();
                    buckets.Add(bucket);
                }
                else
                {
                    bucket.Add(index);
                }
                index++;
            }
            return buckets;
        }

        private bool CanControlBeSmall(UIElement e)
        {

            if ((e is IRibbonControl) && !(e is IRibbonLargeControl))
            {
                RibbonSize min = RibbonBar.GetMinSize(e);
                return min < RibbonSize.Large;
            }
            return e.DesiredSize.Height <= InternalGroupPanel.MaxSmallHeight;
        }


        private Dictionary<int, RibbonSize> reducableControlIndexes;

        private Dictionary<int, RibbonSize> ReducableControlIndexes
        {
            get
            {
                if (reducableControlIndexes == null) reducableControlIndexes = GetReducableControlIndexes();
                return reducableControlIndexes;
            }
        }


        /// <summary>
        /// Gets a Dictionary of all control indexs (from Controls) which can be reduced and to which size.
        /// </summary>
        /// <remarks>
        /// To automatically determine which controls can be reduced and to which level, the controls are first grouped into buckets that contain continous controls
        /// that can be reduced to a small size so that 3 of them can share a row. For each of this buckets, the available RibbonSize is determined of where the
        /// control is placed inside the bucket.
        /// </remarks>
        /// <returns></returns>
        private Dictionary<int, RibbonSize> GetReducableControlIndexes()
        {
            Dictionary<int, RibbonSize> reducable = new Dictionary<int, RibbonSize>();
            GroupBucketCollection buckets = GetBuckets();

            foreach (GroupBucket b in buckets)
            {
                int smallLevel = b.Count - 3;
                for (int i = b.Count % 3; i < b.Count; i++)
                {
                    RibbonSize minSize = i >= smallLevel ? RibbonSize.Small : RibbonSize.Medium;
                    reducable.Add(b[i], minSize);
                }
            }
            int index = 0;
            foreach (UIElement e in Controls)
            {
                if (e is IRibbonLargeControl) reducable.Add(index, RibbonSize.Small);
                index++;
            }
            return reducable;
        }



        #endregion

    }
}
