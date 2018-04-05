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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.MenuItems
{
     [Export(typeof(IPEContextMenu))]
    public class DynamicVariableContextMemu: DisplayablePropertyContextMenu
    {
        public DynamicVariableContextMemu()
        {

        }

        public override Type ItemType
        {
            get
            {
                return typeof(DynamicVariableItem);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Enable(_LD, true);
            this.Enable(_RLEASE, true);
            this.Enable(_AN, true);
            this.Enable(_SOM, false);
            this.Enable(_VI3, false);
        }

        protected override void AttributeTable_Clicked(object sender, EventArgs e)
        {
            if (CheckDataSource())
            {
                _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
                var dp = _Package as IDataPackage;
                var dc = dp.Values as IDataCubeObject;
                dc.SelectedVariableIndex = VariableIndex;
                dc.SelectedTimeIndex = 0;
                _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
                _ShellService.DataGridView.Bind(dc);
            }
        }

        protected override void ShowOnMap_Clicked(object sender, EventArgs e)
        {
            if (CheckDataSource())
            {
                var dp = _Package as IDataPackage;
                var item = _Item as DynamicVariableItem;
                var convertor = dp.Values;
                convertor.SelectedVariableIndex = VariableIndex;
                convertor.SelectedTimeIndex = Package.TimeService.CurrentTimeStep;
                var vector = convertor.GetByTime(VariableIndex, Package.TimeService.CurrentTimeStep);
                if (vector != null && _Package.Feature != null)
                {                  
                    var dt = _Package.Feature.DataTable;
                    if (vector.Length == dt.Rows.Count)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dt.Rows[i][RegularGrid.ParaValueField] = vector.GetValue(i);
                        }

                        ApplyScheme(_Package.FeatureLayer, RegularGrid.ParaValueField);
                    }
                }
            }
        }

        protected override void Add2Toolbox_Clicked(object sender, EventArgs e)
        {
            var dp = _Package as IDataPackage;
            var mat = dp.Values;
            if (mat != null)
            {
                if (mat.Name == "Default")
                {
                    var buf = _Package.Name;
                    mat.Name = buf.Replace(' ', '_');
                }
                _ShellService.PackageToolManager.Workspace.Add(mat);
            }
        }

        protected override void Animate_Clicked(object sender, EventArgs e)
        {
            var dp = _Package as IDataPackage;
            var mat = dp.Values;
            if (mat != null)
            {
                var buf = _Package.Name;
                if(mat.Name == "Default")
                    mat.Name = buf.Replace(' ', '_');
                mat.OwnerName = dp.Name;
                mat.DataOwner = _Package;
                if (mat.Size[2] > 1)
                {
                    _ShellService.SelectPanel(DockPanelNames.AnimationPlayerPanel);
                    _ShellService.AnimationPlayer.DataCubeWorkspace.Add(mat);
                  
                }
            }
        }

        protected override void ReleaseData_Clicked(object sender, EventArgs e)
        {
            var dp = _Package as IDataPackage;
            var mat = dp.Values;
            if (mat != null)
            {
                mat.Value[this.VariableIndex] = null;
                foreach (var item in _sub_menus)
                {
                    item.Enabled = false;
                }
                this.Enable(_LD, true);
                _SelectedNode.Image = Resources.LayerRaster_B_16_gray;
            }
        }

        protected bool CheckDataSource()
        {
            var dp = _Package as IDataPackage;
            if (dp != null && dp.Values != null)
            {
                return true;
            }
            else
            {
                _ShellService.MessageService.ShowWarning(null, "You need to load data at first!");
                return false;
            }
        }
    }
}
