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

using Heiflow.Applications;
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.MenuItems
{
    [Export(typeof(IPEContextMenu))]
    public class DisplayablePropertyContextMenu : PEContextMenu, IVariableContextMenu
    {
        /// <summary>
        /// Load Data
        /// </summary>
        public const string _LD = "Load Data";
        /// <summary>
        /// Table View
        /// </summary>
        public const string _AT = "Table View...";
        /// <summary>
        /// Map View
        /// </summary>
        public const string _SOM = "Map View...";
        /// <summary>
        /// 3D View
        /// </summary>
        public const string _VI3 = "3D View...";
        /// <summary>
        /// Animate
        /// </summary>
        public const string _AN = "Animate...";
        /// <summary>
        /// Export
        /// </summary>
        public const string _EX = "Export...";
        /// <summary>
        /// Add to toolbox
        /// </summary>
        public const string _A2T = "Add to toolbox";
        /// <summary>
        /// Edit
        /// </summary>
        public const string _A2DC = "Edit...";
        /// <summary>
        /// Release Data
        /// </summary>
        public const string _RLEASE = "Release Data";
        /// <summary>
        /// Set As Active Data Source
        /// </summary>
        public const string _SETAS_ACTSource = "Set As Active Data Source";
        /// <summary>
        /// Set To Default Values
        /// </summary>
        protected const string _S2DF = "Set To Default Values";
        
        protected Node _SelectedNode;
        protected List<ExplorerMenuItem> _sub_menus = new List<ExplorerMenuItem>();

        public DisplayablePropertyContextMenu()
        {
        }


        public override Type ItemType
        {
            get
            {
                return typeof(DisplayablePropertyItem);
            }
        }

        public int VariableIndex
        {
            get;
            set;
        }

        public override void AddMenuItems()
        {
            if (_AppManager == null)
                base.Initialize();
            _sub_menus.Clear();
            if (_AppManager.AppMode == AppMode.VHF)
            {
                var item = new ExplorerMenuItem(_LD, Resources.AddContent16, LoadValues_Clicked);
                item.Enabled = false;
                _sub_menus.Add(item);
                item = new ExplorerMenuItem(_AT, Resources.AttributesWindow16, AttributeTable_Clicked);
                _sub_menus.Add(item);
                item = new ExplorerMenuItem(_A2T, Resources.GeoprocessingTool16, Add2Toolbox_Clicked);
                _sub_menus.Add(item);
                item = new ExplorerMenuItem(_A2DC, null, Add2DCEditor_Clicked);
                _sub_menus.Add(item);
                item.Enabled = false;
                item = new ExplorerMenuItem(_RLEASE, null, ReleaseData_Clicked);
                _sub_menus.Add(item);

                _sub_menus.Add(new ExplorerMenuItem(PEContextMenu.MenuSeparator, null, null));
                item = new ExplorerMenuItem(_SOM, Resources.LayerRasterOptimized16, ShowOnMap_Clicked);
                _sub_menus.Add(item);
                item = new ExplorerMenuItem(_VI3, Resources._3dplot16, ShowOn3D_Clicked);
                _sub_menus.Add(item);

                var animate = new ExplorerMenuItem(_AN, Resources.AnimationVideo16, Animate_Clicked);
                animate.Enabled = false;
                _sub_menus.Add(animate);

                var set_as_act = new ExplorerMenuItem(_SETAS_ACTSource, null, SetAsActiveSource_Clicked);
                set_as_act.Enabled = false;
                _sub_menus.Add(set_as_act);

                _sub_menus.Add(new ExplorerMenuItem(PEContextMenu.MenuSeparator, null, null));
                item = new ExplorerMenuItem(_EX, null, Export_Clicked);
                _sub_menus.Add(item);

                ContextMenuItems.AddRange(_sub_menus);
                this.Enable(_RLEASE, false);
            }
            else if (_AppManager.AppMode == AppMode.HE)
            {
                var item = new ExplorerMenuItem(_LD, Resources.AddContent16, LoadValues_Clicked);
                item.Enabled = false;
                _sub_menus.Add(item);
               item = new ExplorerMenuItem(_AT, Resources.AttributesWindow16, AttributeTable_Clicked);
                   _sub_menus.Add(item);
                item =(new ExplorerMenuItem(_VI3, Resources._3dplot16, ShowOn3D_Clicked));
                   _sub_menus.Add(item);
                item = new ExplorerMenuItem(_AN, Resources.AnimationVideo16, Animate_Clicked);
                _sub_menus.Add(item);
                item = new ExplorerMenuItem(_SETAS_ACTSource,null, SetAsActiveSource_Clicked);
                _sub_menus.Add(item);
                  item = new ExplorerMenuItem(_RLEASE, null, ReleaseData_Clicked);
                _sub_menus.Add(item);
                    ContextMenuItems.AddRange(_sub_menus);
            }
        }

        protected override void ProjectItem_PropertyClicked(object sender, EventArgs e)
        {
            _ShellService.PropertyView.SelectedObject = _Package;
            base.ProjectItem_PropertyClicked(sender, e);
        }

        protected virtual void LoadValues_Clicked(object sender, EventArgs e)
        {
            if (_Package is IDataPackage)
            {
                _SelectedNode = (sender as ToolStripMenuItem).Tag as Node;
                var dp = _Package as IDataPackage;

                _SelectedNode.Image = Resources.LayerRaster_B_16;
                dp.Layer = Package.TimeService.CurrentGridLayer;
                dp.Loading += dp_Loading;
                dp.Loaded += dp_Loaded;
                dp.LoadFailed += dp_LoadFailed;
                _ShellService.ProgressWindow.DoWork += ProgressPanel_DoWork;
                foreach (var item in _sub_menus)
                {
                    item.Enabled = false;
                }
                _ShellService.ProgressWindow.DefaultStatusText = "Loading " + _Package.FileName;
                _ShellService.ProgressWindow.WorkCompleted += ProgressWindow_WorkCompleted;
                _ShellService.ProgressWindow.Run(dp);
            }
        }

        protected virtual void ProgressPanel_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var dp = e.Argument as IDataPackage;
            dp.Load(VariableIndex, _ShellService.ProgressWindow);
        }
        protected virtual void ProgressWindow_WorkCompleted(object sender, EventArgs e)
        {
            _ShellService.ProgressWindow.WorkCompleted -= ProgressWindow_WorkCompleted;
        }
        protected virtual void dp_Loading(object sender, int e)
        {
            string msg = string.Format("Loading {0}%", e);
            _ShellService.ProgressWindow.Progress(_Package.Name, e, msg);
        }

        protected virtual void dp_Loaded(object sender, object e)
        {
            var dp = _Package as IDataPackage;
            foreach (var item in _sub_menus)
            {
                item.Enabled = true;
            }
            Enable(_SOM, false);
            Enable(_EX, false);
            Enable(_VI3, false);
            Enable(_A2DC, false);

            if (dp.DataCube != null)
            {
                _ActiveDataService.Source = dp.DataCube;
                _ActiveDataService.Source.SelectedVariableIndex = this.VariableIndex;
               // _ActiveDataService.SourceStatistics = dp.DataCube.SpatialMean(this.VariableIndex);
            }
            _ShellService.ProgressWindow.DoWork -= ProgressPanel_DoWork;
            dp.Loading -= dp_Loading;
            dp.Loaded -= dp_Loaded;
        }
        protected void dp_LoadFailed(object sender, string e)
        {
            var dp = _Package as IDataPackage;
            foreach (var item in _sub_menus)
            {
                item.Enabled = true;
            }
            Enable(_SOM, false);
            Enable(_EX, false);
            Enable(_VI3, false);
            Enable(_A2DC, false);

            MessageBox.Show("Failed to load. Error message: " + e, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _ShellService.ProgressWindow.DoWork -= ProgressPanel_DoWork;
            dp.Loading -= dp_Loading;
            dp.Loaded -= dp_Loaded;
            dp.LoadFailed -= dp_LoadFailed;
        }

        protected virtual void AttributeTable_Clicked(object sender, EventArgs e)
        {
            _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
            var dc = _Package.GetType().GetProperty(_Item.PropertyInfo.Name).GetValue(_Package) as IDataCubeObject;
            if (dc != null)
            {
                _ShellService.DataGridView.Bind( dc);
                _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
            }
        }

        protected virtual void Add2Toolbox_Clicked(object sender, EventArgs e)
        {

        }

        protected virtual void Add2DCEditor_Clicked(object sender, EventArgs e)
        {

        }
        protected virtual void ReleaseData_Clicked(object sender, EventArgs e)
        {

        }

        public override void NodeDoubleClick()
        {
            AttributeTable_Clicked(null, null);
        }

        protected virtual void ShowOnMap_Clicked(object sender, EventArgs e)
        {
            var convertor = _Package.GetType().GetProperty(_Item.PropertyInfo.Name).GetValue(_Package) as IDataCubeObject;
            convertor.SelectedVariableIndex = VariableIndex;
            convertor.SelectedTimeIndex = 0;
            var vector = convertor.GetVectorAsArray(VariableIndex, "0", ":");
            if (vector != null && _Package.Feature != null)
            {
                var dt = _Package.Feature.DataTable;

                if (vector.Length == dt.Rows.Count)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i][RegularGrid.ParaValueField] = vector.GetValue(i);
                    }
                }
                var gridLayer = _Package.FeatureLayer;
                ApplyScheme(gridLayer, RegularGrid.ParaValueField);
            }
        }

        protected virtual void ShowOn3D_Clicked(object sender, EventArgs e)
        {
            var grid = _ProjectService.Project.Model.Grid as MFGrid;
            var dc = _Package.GetType().GetProperty(_Item.PropertyInfo.Name).GetValue(_Package) as IDataCubeObject;
            dc.SelectedVariableIndex = VariableIndex;
            dc.SelectedTimeIndex = 0;
            var vector = dc.GetVectorAsArray(VariableIndex, "0", ":");
            if (vector != null)
            {
                if (MyAppManager.Instance.AppMode == AppMode.VHF)
                {
                    if (_ShellService.SurfacePlot != null)
                    {
                        _ShellService.ShowChildWindow(ChildWindowNames.Win3DView);
                        var mat = grid.ToILMatrix<float>(vector, 0);
                        mat.Name = string.Format("{0}[{1}]", dc.Name, dc.Variables[VariableIndex]);
                        _ShellService.SurfacePlot.PlotSurface(mat);
                    }
                    else
                    {
                        if (_Package.Layer3D.RenderObject != null)
                        {
                            _Package.Layer3D.RenderObject.Render(vector.Cast<float>().ToArray());
                        }
                    }
                }
            }
        }

        protected virtual void AnimationMap_Clicked(object sender, EventArgs e)
        {

        }

        protected virtual void Animation3D_Clicked(object sender, EventArgs e)
        {

        }

        protected virtual void Animate_Clicked(object sender, EventArgs e)
        {

        }
        protected virtual void SetAsActiveSource_Clicked(object sender, EventArgs e)
        {

        }
        protected virtual void Export_Clicked(object sender, EventArgs e)
        {

        }
    }

}