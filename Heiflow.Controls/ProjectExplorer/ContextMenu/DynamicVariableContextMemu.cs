// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
