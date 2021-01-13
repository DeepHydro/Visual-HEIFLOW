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
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Controls;
using ILNumerics;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace Heiflow.Controls.WinForm.MenuItems
{
    [Export(typeof(IPEContextMenu))]
    public class StaticVariableContextMenu : DisplayablePropertyContextMenu
    {
        public StaticVariableContextMenu()
        {
        }

        public override Type ItemType
        {
            get
            {
                return typeof(StaticVariableItem);
            }
        }

        public override void AddMenuItems()
        {
            var atr = ExplorerItem as StaticVariableItem;
            if (atr != null)
            {
                base.AddMenuItems();
                Enable(_LD, false);
                Enable(_AN, false);
                Enable(_SETAS_ACTSource, false);
                Enable(_EX, false);
                Enable(_A2DC, true);
                Enable(_RLEASE, false);
            }
        }

        protected override void AttributeTable_Clicked(object sender, EventArgs e)
        {
            var item = ExplorerItem as StaticVariableItem;
            _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
            IDataCubeObject binding = null;
            if (_Item.PropertyInfo != null)
                binding = _Package.GetType().GetProperty(_Item.PropertyInfo.Name).GetValue(_Package) as IDataCubeObject;
            else if (_Item.Tag is GHMVariable)
                binding = (_Item.Tag as GHMVariable).DataCube;
            binding.SelectedVariableIndex = item.VariableIndex;
            _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
            _ShellService.DataGridView.Bind(binding);
        }

        protected override void Add2Toolbox_Clicked(object sender, EventArgs e)
        {
            var buf = _Package.GetType().GetProperty(_Item.PropertyInfo.Name).GetValue(_Package);
            var mat = buf as DataCube<float>;
            if (mat != null)
            {
                //mat.Name = _Package.Name + "_" + _Item.PropertyInfo.Name;
                _ShellService.PackageToolManager.Workspace.Add(mat);
            }
        }

        protected override void Add2DCEditor_Clicked(object sender, EventArgs e)
        {
            _ShellService.SelectPanel(DockPanelNames.DCEditorPanel);
            var buf = _Package.GetType().GetProperty(_Item.PropertyInfo.Name).GetValue(_Package);
            var mat = buf as IDataCubeObject;
            if (mat != null)
            {
                mat.Name = _Item.PropertyInfo.Name;
                mat.OwnerName = _Package.Name;
                _ShellService.TV3DMatEditor.Workspace.Add(mat);
            }
        }

        protected override void ShowOnMap_Clicked(object sender, EventArgs e)
        {
            var item = ExplorerItem as StaticVariableItem;
            var grid = _ProjectService.Project.Model.Grid as MFGrid;
            var convertor = _Package.GetType().GetProperty(_Item.PropertyInfo.Name).GetValue(_Package) as IDataCubeObject;
            convertor.SelectedVariableIndex = item.VariableIndex;
            var vector = convertor.GetVectorAsArray(convertor.SelectedVariableIndex, "0", ":");
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
                ApplyScheme(_Package.FeatureLayer, RegularGrid.ParaValueField);
            }
        }

        protected override void ShowOn3D_Clicked(object sender, EventArgs e)
        {
            var item = ExplorerItem as StaticVariableItem;
            IDataCubeObject dc = null;
            if (item.PropertyInfo != null)
                dc = _Package.GetType().GetProperty(_Item.PropertyInfo.Name).GetValue(_Package) as IDataCubeObject;
            else if (item.Tag is GHMVariable)
                dc = (item.Tag as GHMVariable).DataCube;

            dc.SelectedVariableIndex = item.VariableIndex;
            if (MyAppManager.Instance.AppMode == AppMode.VHF)
            {
                if (_ShellService.SurfacePlot != null && dc.Topology != null)
                {
                    _ShellService.ShowChildWindow(ChildWindowNames.Win3DView);
                    var mat = dc.ToSpatialILBaseArray(dc.SelectedVariableIndex, 0) as ILArray<float>;
                    mat.Name = string.Format("{0}[{1}]", dc.Name, dc.Variables[VariableIndex]);
                    _ShellService.SurfacePlot.PlotSurface(mat);
                }
            }
            else
            {
                var ghmv = (item.Tag as GHMVariable);
                var vector = dc.GetVectorAsArray(dc.SelectedVariableIndex, "0", ":");
                if (vector != null && ghmv.RenderableModelObject != null)
                {
                    ghmv.RenderableModelObject.RenderObject.Render(vector.Cast<float>().ToArray());
                }
            }
        }
    }
}