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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using Heiflow.Presentation.Controls;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Presentation.Services;
using Heiflow.Presentation;
using System.ComponentModel.Composition;
using Heiflow.Models.Generic;

namespace Heiflow.Controls.WinForm.MenuItems
{
    public class PEContextMenu : IPEContextMenu, IPackageContextMemu
    {
        public static string MenuSeparator = "Separator";
        public event EventHandler ItemChanged;
        public event EventHandler RemoveItem;
        protected IMyAppManager _AppManager;
        protected IShellService _ShellService;
        protected IProjectService _ProjectService;
        protected IActiveDataService _ActiveDataService;
        protected IPackage _Package;
        protected IExplorerItem _Item;

        public PEContextMenu()
        {
            Checked = false;
            IsSelected = false;
            IsExpanded = true;
            ProjectItemVisible = true;

            Child = new List<IPEContextMenu>();
            ContextMenuItems = new List<ExplorerMenuItem>();
        }

        #region Properties
        public List<ExplorerMenuItem> ContextMenuItems
        {
            get;
            set;
        }

        public bool Checked
        {
            get;
            set;
        }

        public bool IsExpanded
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get;
            set;
        }

        public List<IPEContextMenu> Child
        {
            get;
            private set;
        }

        public bool ProjectItemVisible
        {
            get;
            set;
        }

        public SymbolMode LegendSymbolMode
        {
            get;
            protected set;
        }

        public string LegendText
        {
            get;
            set;
        }

        public Image Icon
        {
            get;
            set;
        }

        public LegendType LegendType
        {
            get;
            protected set;
        }

        public virtual Type ItemType
        {
            get
            {
                return typeof(ExplorerItem);
            }
        }

        public IPackage Package
        {
            get
            {
                return _Package;
            }
            set
            {
                _Package = value;
                this.LegendText = _Package.Name;
                Icon = _Package.Icon;
            }
        }

        public IExplorerItem ExplorerItem
        {
            get
            {
                return _Item;
            }
            set
            {
                _Item = value;
            }
        }

        public object Tag
        {
            get;
            set;
        }

#endregion

        public bool CanReceiveItem(IPEContextMenu item)
        {
            return true;
        }

        public virtual void Initialize()
        {
            _AppManager = MyAppManager.Instance;
            if (_AppManager.AppMode == AppMode.HE)
            {
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            }
            else
            {
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            }
            _ProjectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            _ActiveDataService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IActiveDataService>();

            ContextMenuItems.Add(new ExplorerMenuItem("Property", Resources.property16, ProjectItem_PropertyClicked));
            AddMenuItems();
        }

        public virtual void AddMenuItems()
        {

        }

        protected virtual void ProjectItem_PropertyClicked(object sender, EventArgs e)
        {
            _ShellService.PropertyView.SelectedObject = _Package;
             _ShellService.SelectPanel(DockPanelNames.PropertyPanel);
        }

        /// <summary>
        /// Fires the ItemChanged event, optionally specifying a different
        /// sender
        /// </summary>
        protected void OnItemChanged(object sender)
        {
            if (ItemChanged == null) return;
            ItemChanged(sender, EventArgs.Empty);
        }

        /// <summary>
        /// Instructs the parent legend item to remove this item from the list of legend items.
        /// </summary>
        protected void OnRemoveItem(object sender)
        {
            if (RemoveItem != null)
                RemoveItem(sender, EventArgs.Empty);
            // Maybe we don't need RemoveItem event.  We could just invoke a method on the parent.
            // One less thing to wire.  But we currently need to wire parents.
        }

        protected void ApplyScheme(IMapFeatureLayer gridLayer, string fieldName)
        {
            if (gridLayer != null)
            {
                IFeatureScheme newScheme = gridLayer.Symbology;
                newScheme.EditorSettings.IntervalMethod = IntervalMethod.NaturalBreaks;
                newScheme.EditorSettings.NumBreaks = 5;
                newScheme.EditorSettings.UseGradient = true;
                newScheme.EditorSettings.ClassificationType = ClassificationType.Quantities;
                newScheme.EditorSettings.FieldName = fieldName;
                newScheme.CreateCategories(gridLayer.DataSet.DataTable);
                newScheme.ResumeEvents();
                gridLayer.ApplyScheme(newScheme);
            }
        }

        public void Enable(string itemName, bool enabled)
        {
            var menu = from mn in ContextMenuItems where mn.Name == itemName select mn;
            if (menu != null && menu.Count() == 1)
                menu.First().Enabled = enabled;
        }

        public void EneableAll(bool enabled)
        {
            foreach(var mn in ContextMenuItems)
            {
                mn.Enabled = enabled;
            }
        }


        public virtual void NodeDoubleClick()
        {
            
        }
    }
}
