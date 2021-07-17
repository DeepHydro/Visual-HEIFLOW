//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

namespace DotSpatial
{
    using System.Reflection;

    using DevExpress.Utils;
    using DevExpress.XtraBars;
    using DevExpress.XtraBars.Ribbon;
    using DevExpress.XtraEditors;
    using DevExpress.XtraEditors.Repository;
    using DevExpress.XtraEditors.Controls;
    using DotSpatial.Controls.Header;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Windows.Forms;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Collections;
    using System.Diagnostics;
    using Heiflow.Plugins.Default.Properties;

    /// <summary>
    /// The ribbon header.
    /// </summary>
    [Export(typeof(IHeaderControl))]
    public class RibbonHeader : HeaderControl, IPartImportsSatisfiedNotification, IStatusControl
    {
        private const int INT_MillisecondsToRefreshStatusBar = 200;
        [Import("Shell", typeof(ContainerControl))]
        public ContainerControl Shell { get; set; }

        public Dictionary<string, RepositoryItemComboBox> ComboBoxes { get; private set; } 

        #region Constants and Fields

        private const string STR_DefaultGroupName = "Default Group";
        private bool _nextItemBeginsGroup;

        #endregion
        private DevExpress.XtraBars.Ribbon.RibbonControl _ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.Ribbon.ApplicationMenu backstageMenu;
        private DevExpress.XtraBars.BarStaticItem defaultStatusPanel;
        private DevExpress.XtraBars.BarEditItem defaultProgressPanel;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar defaultProgressBar;
        private static List<string> groups = new List<string>();
        private readonly Stopwatch stopwatch = new Stopwatch();

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the RibbonHeader class.
        /// </summary>
        /// <param name="ribbon">
        /// </param>
        public RibbonHeader()
        {
            // Better look when glass isn't available.
            DevExpress.Skins.SkinManager.EnableFormSkins();

             DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2010 Blue");
       //     DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("DevExpress Dark Style");
         //   DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2010 Black");

            // We start the stop watch to keep our code in the progress event simple.
            stopwatch.Start();

            ComboBoxes = new Dictionary<string, RepositoryItemComboBox>();
        }

        #endregion

        #region Public Methods



        /// <summary>
        /// Adds the specified root item.
        /// </summary>
        /// <param name="item">
        /// The root item.
        /// </param>
        public override void Add(RootItem item)
        {
            Guard.ArgumentNotNull(item, "rootItem");

            if (!ShouldCreateRibbonPage(item.Key))
                return;

            // See if we already added this RootItem.
            RibbonPage page = GetExistingRibbonPage(item.Key);

            if (page == null)
            {
                page = new RibbonPage(item.Caption);
                page.Name = item.Key;
                _ribbon.Pages.Add(page);
            }
            else
            {
                page.Text = item.Caption;
            }

            page.Tag = item.SortOrder;
            page.MergeOrder = item.SortOrder;

            // Get a list of all the pages
            var pages = new List<RibbonPage>(_ribbon.Pages.Count);
            foreach (RibbonPage p in _ribbon.Pages)
            {
                pages.Add(p);
            }

            var sortedPages = (from entry in pages orderby entry.MergeOrder ascending select entry);

            _ribbon.Pages.Clear();
            foreach (var sortedPage in sortedPages)
            {
                this._ribbon.Pages.Add(sortedPage);
            }
            item.PropertyChanged += new PropertyChangedEventHandler(RootItem_PropertyChanged);

        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void Add(MenuContainerItem item)
        {
            Guard.ArgumentNotNull(item, "item");

            RibbonPage page = this.GetRibbonPage(item);
            RibbonPageGroup group = GetOrCreateGroup(
            page, item.GroupCaption ?? this.GetProductName(Assembly.GetCallingAssembly()));

            BarSubItem newItem = new BarSubItem();
            newItem.Caption = item.Caption;
            newItem.Name = item.Key;
            newItem.LargeGlyph = item.LargeImage;

            ProcessSeperator(group.ItemLinks.Add(newItem));
            item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(MenuContainerItem_PropertyChanged);
        }

        /// <summary>
        /// This will add a new item that will appear on the ribbon control.
        /// </summary>
        /// <param name="item">
        /// </param>
        public override void Add(SimpleActionItem item)
        {
            Guard.ArgumentNotNull(item, "item");

            BarButtonItem newItem = CreateBarButtonItem(item);

            // Deal with quickaccess
            if (item.ShowInQuickAccessToolbar)
            {
                AddItemToQuickAccess(newItem);
            }

            if (item.GroupCaption == HeaderControl.ApplicationMenuKey)
            {
                AddItemToBackStage(newItem);
                return;
            }
            if (item.GroupCaption == HeaderControl.HeaderHelpItemKey)
            {
                this._ribbon.PageHeaderItemLinks.Add(newItem);
                return;
            }

            RibbonPage page = this.GetRibbonPage(item);
            RibbonPageGroup group = GetOrCreateGroup(
            page, item.GroupCaption ?? this.GetProductName(Assembly.GetCallingAssembly()));

            if (item.MenuContainerKey == null)
            {
                ProcessSeperator(group.ItemLinks.Add(newItem));
            }
            else
            {
                foreach (BarItemLink itemLink in group.ItemLinks)
                {
                    if (itemLink.Item.Name == item.MenuContainerKey && itemLink.Item is BarSubItem)
                    {
                        var link = ((BarSubItem)itemLink.Item).AddItem(newItem);
                        ProcessSeperator(link);
                        break;
                    }
                }
            }

            // allow the default, leftmost page to be the home page
            this._ribbon.SelectedPage = null;

            item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SimpleActionItem_PropertyChanged);
        }

        private void AddItemToBackStage(BarButtonItem newItem)
        {
            var backstageMenu = this._ribbon.ApplicationButtonDropDownControl as ApplicationMenu;

            if (backstageMenu.ItemLinks.Count == 0)
            {
                backstageMenu.AddItem(newItem);
            }
            else
            {
                backstageMenu.AddItem(newItem);
                var list = new List<BarItem>(backstageMenu.ItemLinks.Count);

                foreach (BarItemLink itemLink in backstageMenu.ItemLinks)
                {
                    list.Add(itemLink.Item);
                }
                list.Sort(new Comparison<BarItem>((x, y) =>
                {

                    if (x == null)
                    {
                        if (y == null)
                        {
                            // If x is null and y is null, they're equal.
                            return 0;
                        }
                        else
                        {
                            // If x is null and y is not null, y is greater.
                            return -1;
                        }
                    }
                    else
                    {
                        // If x is not null...
                        if (y == null)
                        {
                            // ...and y is null, x is greater.
                            return 1;
                        }
                        else
                        {
                            return x.MergeOrder.CompareTo(y.MergeOrder);
                        }
                    }
                }));

                backstageMenu.ItemLinks.Clear();
                backstageMenu.ItemLinks.AddRange(list.ToArray());
            }
        }
        private void AddItemToQuickAccess(BarButtonItem newItem)
        {
            this._ribbon.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Default;
            this._ribbon.ShowToolbarCustomizeItem = false;
            this._ribbon.Toolbar.ItemLinks.Add(newItem);
        }
        private void ProcessSeperator(BarItemLink barItemLink)
        {
            if (_nextItemBeginsGroup)
            {
                barItemLink.BeginGroup = _nextItemBeginsGroup;
                _nextItemBeginsGroup = false;
            }
        }

        void RootItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as RootItem;
            var guiItem = this.GetExistingRibbonPage(item.Key);

            switch (e.PropertyName)
            {
                case "Caption":
                    guiItem.Text = item.Caption;
                    break;

                case "Visible":
                    guiItem.Visible = item.Visible;
                    break;

                case "SortOrder":
                    break;
                default:
                    break;
            }
        }


        void MenuContainerItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as MenuContainerItem;
            var guiItem = this.GetItem(item.Key);

            switch (e.PropertyName)
            {
                case "LargeImage":
                    guiItem.LargeGlyph = item.LargeImage;
                    break;

                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }
        void SimpleActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SimpleActionItem;
            var guiItem = this.GetItem(item.Key);

            switch (e.PropertyName)
            {
                case "SmallImage":
                    guiItem.Glyph = item.SmallImage;
                    break;

                case "LargeImage":
                    guiItem.LargeGlyph = item.LargeImage;
                    break;

                case "MenuContainerKey":
                    Trace.WriteLine("MenuContainerKey must not be changed after item is added to header.");
                    break;

                case "ToggleGroupKey":
                    Trace.WriteLine("ToggleGroupKey must not be changed after item is added to header.");
                    break;

                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }

        private void DropDownActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as DropDownActionItem;
            var guiItem = this.GetItem(item.Key) as BarEditItem;
            var combo = guiItem.Edit as RepositoryItemComboBox;

            switch (e.PropertyName)
            {
                case "AllowEditingText":
                    if (item.AllowEditingText)
                    {
                        combo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                    }
                    else
                    {
                        combo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                    }
                    break;

                case "NullValuePrompt":
                    combo.NullValuePrompt = item.NullValuePrompt;
                    break;

                case "Width":
                    guiItem.Width = item.Width;
                    break;

                case "SelectedItem":
                    guiItem.EditValue = item.SelectedItem;
                    break;

                case "FontColor":
                    combo.Appearance.ForeColor = item.FontColor;
                    break;

                case "DisplayText":
                    guiItem.EditValue = item.DisplayText;
                    break;
                case "MultiSelect":

                    break;



                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }

        private void ActionItem_PropertyChanged(ActionItem item, PropertyChangedEventArgs e)
        {

            var guiItem = this.GetItem(item.Key);
            if (guiItem == null)
                return;
            switch (e.PropertyName)
            {
                case "Caption":
                    guiItem.Caption = item.Caption;
                    break;

                case "Enabled":
                    guiItem.Enabled = item.Enabled;
                    break;

                case "Visible":
                    SetVisibility(item, guiItem);
                    break;

                case "ToolTipText":
                    guiItem.ResetSuperTip();
                    guiItem.SuperTip = new SuperToolTip();
                    guiItem.SuperTip.Items.Add(item.ToolTipText);
                    break;

                case "GroupCaption":
                    Trace.WriteLine("GroupCaption must not be changed after item is added to header.");
                    break;

                case "RootKey":
                    Trace.WriteLine("RootKey must not be changed after item is added to header.");
                    break;

                case "Key":
                default:
                    throw new NotSupportedException(" This Header Control implementation doesn't have an implementation for or has banned modifying that property.");
            }
        }

        private void TextEntryActionItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as TextEntryActionItem;
            var guiItem = this.GetItem(item.Key) as BarEditItem;


            switch (e.PropertyName)
            {
                case "Width":
                    guiItem.Width = item.Width;
                    break;

                case "Text":
                    guiItem.EditValue = item.Text;
                    break;

                case "FontColor":
                    guiItem.Edit.Appearance.ForeColor = item.FontColor;
                    break;

                default:
                    ActionItem_PropertyChanged(item, e);
                    break;
            }
        }

        /// <summary>
        /// Adds a combo box style item
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void Add(DropDownActionItem item)
        {
            Guard.ArgumentNotNull(item, "item");

            RibbonPage page = this.GetRibbonPage(item);
            RibbonPageGroup group = GetOrCreateGroup(
            page, item.GroupCaption ?? this.GetProductName(Assembly.GetCallingAssembly()));
            var newItem = this.CreateBarEditItem(item);

            ProcessSeperator(group.ItemLinks.Add(newItem));

            item.PropertyChanged += DropDownActionItem_PropertyChanged;
        }

        /// <summary>
        /// Adds a visible separator.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(SeparatorItem item)
        {
            Guard.ArgumentNotNull(item, "item");

            _nextItemBeginsGroup = true;
        }

        /// <summary>
        /// Adds the specified textbox item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(TextEntryActionItem item)
        {
            Guard.ArgumentNotNull(item, "item");

            RibbonPage page = this.GetRibbonPage(item);
            RibbonPageGroup group = GetOrCreateGroup(
            page, item.GroupCaption ?? this.GetProductName(Assembly.GetCallingAssembly()));

            var newItem = this.CreateBarEditItem(item);


            ProcessSeperator(group.ItemLinks.Add(newItem));

            item.PropertyChanged += new PropertyChangedEventHandler(TextEntryActionItem_PropertyChanged);
        }


        /// <summary>
        /// Gets the name of the plugin. This must be called from one of the public override methods as it looks for the calling assembly.
        /// </summary>
        /// <value>
        /// The name of the plugin.
        /// </value>
        /// <param name="assembly">
        /// </param>
        /// <returns>
        /// The get product name.
        /// </returns>
        public string GetProductName(Assembly assembly)
        {
            Guard.ArgumentNotNull(assembly, "assembly");
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
        }

        /// <summary>
        /// Remove item from the standard toolbar or ribbon control
        /// </summary>
        /// <param name="key">
        /// The string itemName to remove from the standard toolbar or ribbon control
        /// </param>
        public override void Remove(string key)
        {
            foreach (BarItem item in this._ribbon.Items)
            {
                if (item.Caption == key)
                {
                    RibbonPageGroupItemLinkCollection groupLinkCollection = item.Links[0].Links as RibbonPageGroupItemLinkCollection;
                    if (groupLinkCollection == null)
                    {
                        // TODO: We don't provide complete support in the situation where there are "menus" embedded in the ribbon.
                        // We should remove the associated Page Group and Page when appropriate.
                        item.Dispose();
                        break;
                    }
                    else
                    {
                        RibbonPageGroup pg = groupLinkCollection.PageGroup;
                        RibbonPage p = pg.Page;

                        this._ribbon.Items.Remove(item);

                        // Remove the group when nothing remains in it.
                        if (pg.ItemLinks.Count == 0)
                        {
                            p.Groups.Remove(pg);

                            // If we removed the last group on a tab, remove the tab as well.
                            if (p.Groups.Count == 0)
                            {
                                _ribbon.Pages.Remove(p);
                            }
                        }
                        // remove all sub items?
                        break;
                    }
                }
            }

            base.Remove(key);
        }

        #endregion

        #region Methods

        private static void SetVisibility(ActionItem item, BarItem guiItem)
        {
            // todo: reduce near duplicate code in this method.
            RibbonPageGroupItemLinkCollection groupLinkCollection = null;
            if (guiItem.Links.Count > 0)
                groupLinkCollection = guiItem.Links[0].Links as RibbonPageGroupItemLinkCollection;

            if (item.Visible)
            {
                guiItem.Visibility = BarItemVisibility.Always;
                if (groupLinkCollection != null)
                {
                    RibbonPageGroup pg = groupLinkCollection.PageGroup;
                    RibbonPage p = pg.Page;

                    foreach (BarItemLink link in groupLinkCollection)
                    {
                        if (link.Item != guiItem)
                            if (link.Item.Visibility != BarItemVisibility.Always)
                                return;
                    }

                    pg.Visible = true;
                }
            }
            else
            {
                guiItem.Visibility = BarItemVisibility.OnlyInCustomizing;

                if (groupLinkCollection != null)
                {
                    RibbonPageGroup pg = groupLinkCollection.PageGroup;
                    RibbonPage p = pg.Page;

                    foreach (BarItemLink link in groupLinkCollection)
                    {
                        if (link.Item != guiItem)
                            if (link.Item.Visibility == BarItemVisibility.Always)
                                return;
                    }

                    pg.Visible = false;
                }
            }
        }

        /// <summary>
        /// create bar button item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// </returns>
        private static BarButtonItem CreateBarButtonItem(SimpleActionItem item)
        {
            var newItem = new BarButtonItem();
            newItem.Caption = item.Caption;
            newItem.Name = item.Key;
            newItem.Glyph = item.SmallImage;
            newItem.LargeGlyph = item.LargeImage;
            if (item.ToolTipText != null)
            {
                newItem.SuperTip = new SuperToolTip();
                newItem.SuperTip.Items.Add(item.ToolTipText);
            }
            newItem.MergeOrder = item.SortOrder;
            newItem.Enabled = item.Enabled;
            SetVisibility(item, newItem);
            newItem.ItemClick += (sender, e) => item.OnClick(e);

            // we're grouping all Toggle buttons together into the same group.
            if (item.ToggleGroupKey != null)
            {
                int groupIndex = groups.IndexOf(item.ToggleGroupKey);
                if (groupIndex == -1)
                {
                    groups.Add(item.ToggleGroupKey);
                    groupIndex = groups.IndexOf(item.ToggleGroupKey);
                    // If there is only one button in the group, it can be toggled off and on.
                    newItem.AllowAllUp = true;
                }
                else
                {
                    // If there are multiple buttons in the group, clicking on one twice will not elevate it.
                    newItem.AllowAllUp = false;
                }

                newItem.ButtonStyle = BarButtonStyle.Check;
                // This GroupIndex should be one-based (no 0 value).
                newItem.GroupIndex = groupIndex + 1;

                item.Toggling += (sender, e) =>
                {
                    newItem.Toggle();
                };
            }

            return newItem;
        }

        /// <summary>
        /// Get or create group.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="groupCaption">
        /// The group caption.
        /// </param>
        /// <returns>
        /// </returns>
        private static RibbonPageGroup GetOrCreateGroup(RibbonPage page, string groupCaption)
        {
            // make sure the group caption is present or use a default.
            if (groupCaption == null)
            {
                groupCaption = STR_DefaultGroupName;
            }

            RibbonPageGroup group = page.Groups[groupCaption];
            if (group == null)
            {
                RibbonPageGroup newGroup = new RibbonPageGroup();
                newGroup.Name = groupCaption;
                newGroup.Text = groupCaption;
                newGroup.ShowCaptionButton = false;
                page.Groups.Add(newGroup);
                group = newGroup;
            }

            return group;
        }

        private BarEditItem CreateBarEditItem(DropDownActionItem item)
        {
            var barEditItem = new BarEditItem();
            barEditItem.Name = item.Key;
            barEditItem.Caption = item.Caption;

            RepositoryItemComboBox combo = new RepositoryItemComboBox();
            barEditItem.Edit = combo;
            combo.Items.AddRange(item.Items);
            combo.NullValuePromptShowForEmptyValue = true;
            if (!ComboBoxes.Keys.Contains(item.Key))
                ComboBoxes.Add(item.Key, combo);
            combo.Click += delegate(object sender, EventArgs e)
            {
                if (item.MultiSelect == true)
                {
                    item.SelectedItem = null;
                    item.MultiSelect = false;
                }
            };

            combo.CustomDisplayText += delegate(object sender, CustomDisplayTextEventArgs e)
            {
                if (item.MultiSelect == true)
                {
                    e.DisplayText = "Multiple Selected";
                    barEditItem.Edit.Appearance.ForeColor = System.Drawing.Color.Gray;
                }
                else
                {
                    barEditItem.Edit.Appearance.ForeColor = System.Drawing.Color.Black;
                }
            };

            if (item.AllowEditingText)
            {
                combo.ImmediatePopup = true;
            }

            if (!item.AllowEditingText)
            {
                combo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            }

            if (item.Width != 0)
            {
                barEditItem.Width = item.Width;
            }

            combo.NullValuePrompt = item.NullValuePrompt;

            combo.SelectedValueChanged += delegate(object sender, System.EventArgs e)
            {
                var edit = sender as DevExpress.XtraEditors.ComboBoxEdit;

                item.PropertyChanged -= DropDownActionItem_PropertyChanged;
                item.SelectedItem = edit.SelectedItem;
                item.PropertyChanged += DropDownActionItem_PropertyChanged;
            };

            return barEditItem;
        }


        private BarEditItem CreateBarEditItem(TextEntryActionItem item)
        {
            var guiItem = new BarEditItem();
            guiItem.Name = item.Key;
            guiItem.Caption = item.Caption;

            RepositoryItemTextEdit textBox = new RepositoryItemTextEdit();
            guiItem.Edit = textBox;

            if (item.Width != 0)
                guiItem.Width = item.Width;

            if (!item.Enabled)
                guiItem.Enabled = false;

            textBox.EditValueChanged += delegate(object sender, System.EventArgs e)
            {
                var edit = sender as DevExpress.XtraEditors.TextEdit;
                item.PropertyChanged -= TextEntryActionItem_PropertyChanged;
                item.Text = edit.Text;
                item.PropertyChanged += TextEntryActionItem_PropertyChanged;
            };

            return guiItem;
        }

        /// <summary>
        /// Ensure the extensions tab exists.
        /// </summary>
        private void EnsureExtensionsTabExists()
        {
            bool exists = this.GetExistingRibbonPage(HeaderControl.ExtensionsRootKey) != null;

            if (!exists)
            {
                this.Add(new RootItem(HeaderControl.ExtensionsRootKey, Resources.Extensions_root));
            }
        }

        /// <summary>
        /// Make sure the root key is present or use a default.
        /// </summary>
        /// <param name="item">
        /// </param>
        private void EnsureNonNullRoot(GroupedItem item)
        {
            if (item.RootKey == null)
            {
                this.EnsureExtensionsTabExists();
                item.RootKey = HeaderControl.ExtensionsRootKey;
            }
        }

        /// <summary>
        /// The get existing ribbon page.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        private RibbonPage GetExistingRibbonPage(string key)
        {
            foreach (RibbonPage page in this._ribbon.Pages)
            {
                if (page.Name == key)
                {
                    return page;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the item.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// </returns>
        private BarItem GetItem(string key)
        {
            foreach (BarItem item in this._ribbon.Items)
            {
                if (item.Name == key)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the ribbon page or a default page based on the item RootKey.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// </returns>
        private RibbonPage GetRibbonPage(GroupedItem item)
        {
            this.EnsureNonNullRoot(item);

            RibbonPage existingPage = this.GetExistingRibbonPage(item.RootKey);
            if (existingPage == null)
            {
                // We create a new page and hope someone assigns a caption to it later by calling Add(rootitem)
                if (ShouldCreateRibbonPage(item.RootKey))
                {
                    RibbonPage page = new RibbonPage();
                    page.Name = item.RootKey;
                    _ribbon.Pages.Add(page);
                    Debug.WriteLine(item.RootKey + " ribbon page was created without a caption (perhaps one will be assigned later by creating a RootItem).");
                    return page;
                }
            }

            return existingPage;
        }

        #endregion

        /// <summary>
        /// Selects the root making it topmost.
        /// </summary>
        /// <param name="key">The key.</param>
        public override void SelectRoot(string key)
        {
            this._ribbon.SelectedPage = this.GetExistingRibbonPage(key);
        }

        public void OnImportsSatisfied()
        {
            this._ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.backstageMenu = new DevExpress.XtraBars.Ribbon.ApplicationMenu();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.defaultStatusPanel = new DevExpress.XtraBars.BarStaticItem();
            this.defaultProgressPanel = new DevExpress.XtraBars.BarEditItem();
            this.defaultProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this._ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backstageMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultProgressBar)).BeginInit();
            Shell.SuspendLayout();
            // 
            // ribbon
            // 
            this._ribbon.ApplicationButtonDropDownControl = this.backstageMenu;
            this._ribbon.ApplicationButtonText = Resources.File_root;
            this._ribbon.ExpandCollapseItem.Id = 0;
            this._ribbon.ExpandCollapseItem.Name = "";
            this._ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this._ribbon.ExpandCollapseItem,
            this.defaultStatusPanel,
            this.defaultProgressPanel});
            this._ribbon.Location = new System.Drawing.Point(0, 0);
            this._ribbon.MaxItemId = 9;
            this._ribbon.AutoSizeItems = true;
            this._ribbon.Name = "ribbon";
            this._ribbon.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.defaultProgressBar});
            this._ribbon.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
            this._ribbon.Size = new System.Drawing.Size(790, 50);
            this._ribbon.StatusBar = this.ribbonStatusBar;
            this._ribbon.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
            // The Application Button is not visible in the Office 2007 paint style, unless you place the RibbonControl onto a RibbonForm.
            //http://documentation.devexpress.com/#WindowsForms/DevExpressXtraBarsRibbonRibbonControl_ShowApplicationButtontopic
            this._ribbon.ShowApplicationButton = DefaultBoolean.True;

            this._ribbon.SelectedPageChanged += new EventHandler(Ribbon_SelectedPageChanged);

            // 
            // backstageMenu
            // 
            this.backstageMenu.Name = "backstageMenu";
            this.backstageMenu.Ribbon = this._ribbon;
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.ItemLinks.Add(this.defaultStatusPanel);
            this.ribbonStatusBar.ItemLinks.Add(this.defaultProgressPanel);
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 468);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this._ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(790, 31);

            // 
            // defaultStatusPanel
            // 
            this.defaultStatusPanel.Caption = "Ready.";
            this.defaultStatusPanel.Id = 4;
            this.defaultStatusPanel.Name = "defaultStatusPanel";
            this.defaultStatusPanel.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // defaultProgressPanel
            // 
            this.defaultProgressPanel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.defaultProgressPanel.Edit = this.defaultProgressBar;
            this.defaultProgressPanel.EditValue = "50";
            this.defaultProgressPanel.Id = 5;
            this.defaultProgressPanel.Name = "defaultProgressPanel";
            this.defaultProgressPanel.Width = 100;
            this.defaultProgressPanel.Visibility = BarItemVisibility.Never;
            // 
            // repositoryItemProgressBar1
            // 
            this.defaultProgressBar.Name = "defaultProgressBar";

            Shell.Controls.Add(this.ribbonStatusBar);
            Shell.Controls.Add(this._ribbon);

            // If it is a DevExpress.XtraBars.Ribbon.RibbonForm...  
            var form = Shell as DevExpress.XtraBars.Ribbon.RibbonForm;
            if (form != null)
            {
                form.Ribbon = this._ribbon;
                form.StatusBar = this.ribbonStatusBar;
            }

            ((System.ComponentModel.ISupportInitialize)(this._ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backstageMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.defaultProgressBar)).EndInit();
            Shell.ResumeLayout(false);
        }

        private void Ribbon_SelectedPageChanged(object sender, EventArgs e)
        {
            if (_ribbon == null) return;
            if (_ribbon.SelectedPage == null) return;

            OnRootItemSelected(_ribbon.SelectedPage.Name);
        }



        public void Progress(string key, int percent, string message)
        {
            defaultStatusPanel.Caption = message;
            //defaultProgressPanel.EditValue = percent;

            //if (percent > 0)
            //    defaultProgressPanel.Visibility = BarItemVisibility.Always;
            //else
            //{
            //    defaultProgressPanel.Visibility = BarItemVisibility.Never;
            //}
            ribbonStatusBar.Invoke((MethodInvoker)delegate
            {
                defaultProgressPanel.EditValue = percent;

                if (percent > 0)
                    defaultProgressPanel.Visibility = BarItemVisibility.Always;
                else
                {
                    defaultProgressPanel.Visibility = BarItemVisibility.Never;
                }
            });

            // we update the values regardless, we just repaint if it is time.
            if (stopwatch.ElapsedMilliseconds > INT_MillisecondsToRefreshStatusBar)
            {
                // Allow CPU intensive operations executing on the UI thread to repaint the status bar,
                // even though the WinMsg isn't coming through unless DoEvents is called.
                // A better option would be to require developers to refactor loops to use true .NET threading
                if (this.ribbonStatusBar.InvokeRequired)
                {
                    this.ribbonStatusBar.BeginInvoke(new MethodInvoker(() => defaultStatusPanel.Refresh()));
                }
                else
                {
                    defaultStatusPanel.Refresh();
                }

                stopwatch.Restart();
            }

        }

        public void Add(StatusPanel panel)
        {
            var statusPanel = new DevExpress.XtraBars.BarStaticItem();
            statusPanel.Name = panel.Key;
            statusPanel.Caption = panel.Caption;
            statusPanel.Width = panel.Width;

            this._ribbon.Items.Add(statusPanel);
            // insert the panel towards the left.
            this.ribbonStatusBar.ItemLinks.Insert(0, statusPanel);
            panel.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                // StatusPanel panel = sender as StatusPanel ;
                switch (e.PropertyName)
                {
                    case "Caption":
                        statusPanel.Caption = panel.Caption;
                        break;

                    default:
                        break;
                }
            };

        }

        public void Remove(StatusPanel panel)
        {
            foreach (BarItemLink itemLink in this.ribbonStatusBar.ItemLinks)
            {
                if (itemLink.Item.Name == panel.Key)
                {
                    this._ribbon.Items.Remove(itemLink.Item);
                    this.ribbonStatusBar.ItemLinks.Remove(itemLink);
                    // this will only remove the first link... and skip any others.
                    break;
                }
            }
        }

        private static bool ShouldCreateRibbonPage(string key)
        {
            // We don't create a Ribbon Tab for the "File" Application Menu.
            if (key == HeaderControl.ApplicationMenuKey)
                return false;

            // We don't create a Ribbon Tab for the Header icons.
            if (key == HeaderControl.HeaderHelpItemKey)
                return false;

            return true;
        }
    }
}
