using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Odyssey.Controls.Ribbon.Interfaces;
using System.Windows;
using System.Windows.Markup;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    [ContentProperty("Tabs")]
    public class RibbonContextualTabSet:Control,IRibbonControl
    {
        static RibbonContextualTabSet()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContextualTabSet), new FrameworkPropertyMetadata(typeof(RibbonContextualTabSet)));
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
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (RibbonTabItem tab in e.NewItems)
                    {
                        tab.tabSet = this;
                        tab.RibbonBar = RibbonBar;
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (RibbonTabItem tab in e.OldItems)
                    {
                        tab.tabSet = null;
                        tab.RibbonBar = null;
                    }
                    break;
            }
        }

        private RibbonBar ribbonBar;
        public RibbonBar RibbonBar
        {
            get {return ribbonBar;}
            internal set
            {
                if (ribbonBar != value)
                {
                    ribbonBar = value;
                    foreach (var tab in Tabs) tab.RibbonBar = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets the color for the ContextualTabSet.
        /// This is a dependency property.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(RibbonContextualTabSet), new UIPropertyMetadata(Color.FromArgb(64, 255, 255, 255)));




        /// <summary>
        /// Gets or sets the title for the ContextualTabSet.
        /// This is a dependency property.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RibbonContextualTabSet), new UIPropertyMetadata(""));


        internal void SetSelectedTabItem(int index)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                RibbonTabItem item = Tabs[i];
                item.IsSelected = i == index;
            }
        }



        /// <summary>
        /// Gets whether the Tabset is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedPropertyKey, value); }
        }



        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey IsSelectedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsSelected", typeof(bool), typeof(RibbonContextualTabSet), new UIPropertyMetadata(false));

        private static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

    }
}
