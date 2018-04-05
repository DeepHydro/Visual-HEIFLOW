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

using DotSpatial.Symbology;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ILNumerics;
using DotSpatial.Controls;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Surface;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Presentation.Controls;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Applications;

namespace Heiflow.Controls.WinForm.MenuItems
{
    public class MMSParaContextMenu : DisplayablePropertyContextMenu, IParameterContextMenu
    {
        private IParameter _Parameter;
        public MMSParaContextMenu()
        {
        }

        public IParameter Parameter
        {
            get
            {
                return _Parameter;
            }
            set
            {
                _Parameter = value;
            }
        }

        public override void AddMenuItems()
        {
            base.AddMenuItems();
            if (_AppManager.AppMode == AppMode.VHF)
            {
                ContextMenuItems.Add(new ExplorerMenuItem(_S2DF, null, SetToDefault_Clicked));
            }
        }
        protected virtual void SetToDefault_Clicked(object sender, EventArgs e)
        {
            if(_Parameter.VariableType == ParameterType.Parameter)
            {
                if (_ShellService.MessageService.ShowYesNoQuestion(_ShellService.MainForm, "Do you really want to set to default values?"))
                    _Parameter.ResetToDefault();
            }
            else
            {
                _ShellService.MessageService.ShowWarning(_ShellService.MainForm, "This parameter can not be changed");
            }
        }
        protected override void ProjectItem_PropertyClicked(object sender, EventArgs e)
        {         
            base.ProjectItem_PropertyClicked(sender, e);
            _ShellService.PropertyView.SelectedObject = _Parameter;
        }

        protected override void AttributeTable_Clicked(object sender, EventArgs e)
        {
            _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
            var dc = _Parameter as IDataCubeObject;
            _ShellService.DataGridView.Bind(dc);
            _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
        }

        protected override void Add2DCEditor_Clicked(object sender, EventArgs e)
        {
            _ShellService.SelectPanel(DockPanelNames.DCEditorPanel);
            var mat = _Parameter as IDataCubeObject;
            if (mat != null)
            {
                mat.Name = _Parameter.Name;
                mat.OwnerName = _Package.Name;
                _ShellService.TV3DMatEditor.Workspace.Add(mat);
            }
        }

        protected override void Add2Toolbox_Clicked(object sender, EventArgs e)
        {
            var mat = _Parameter.DataCubeObject;
            if (mat != null)
            {
                mat.Name = _Parameter.Name;
                mat.OwnerName = _Package.Name;
                _ShellService.PackageToolManager.Workspace.Add(mat);
            }
        }

        protected override void ShowOnMap_Clicked(object sender, EventArgs e)
        {
            var vector = _Parameter.ToFloat().ToArray();

            if (vector != null && _Package.Feature != null)
            {
                var dt = _Package.Feature.DataTable;
                if (vector.Length == dt.Rows.Count)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i][RegularGrid.ParaValueField] = vector[i];
                    }
                } 
                ApplyScheme(_Package.FeatureLayer, RegularGrid.ParaValueField);
            }
        }

        protected override void ShowOn3D_Clicked(object sender, EventArgs e)
        {
            var grid = _ProjectService.Project.Model.Grid as MFGrid;
            var vector = _Parameter.ToFloat().ToArray();
            if (vector != null)
            {
                if (MyAppManager.Instance.AppMode == AppMode.VHF)
                {
                    if (_ShellService.SurfacePlot != null)
                    {
                        _ShellService.ShowChildWindow(ChildWindowNames.Win3DView);
                        var mat = grid.ToILMatrix<float>(vector, 0);
                        mat.Name = string.Format("{0}", _Parameter.Name);
                        _ShellService.SurfacePlot.PlotSurface(mat);
                    }
                }
                else
                {
                    if (_Package.Layer3D.RenderObject != null)
                    {
                        _Package.Layer3D.RenderObject.Render(vector);
                    }
                }
            }
        }
    }
}
