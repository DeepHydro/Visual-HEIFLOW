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
using Heiflow.Models.GHM;
using Heiflow.Models.IO;
using System.Windows.Forms;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Animation;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Applications;

namespace Heiflow.Controls.WinForm.MenuItems
{
    public class GHMElementItem : PEContextMenu
    {
        public GHMElementItem(IPackage pck, Heiflow.Models.GHM.Item item)
        {
            Package = pck as GHMPackage;
            GHMItem = item; 
            GHMItem.Grid = _ProjectService.Project.Model.Grid as MFGrid;
            if (_AppManager.AppMode == AppMode.VHF)
            {
                ContextMenuItems.Add(new ExplorerMenuItem("Load", Resources.AddContent16, LoadData_Clicked));
                ContextMenuItems.Add(new ExplorerMenuItem("View Values...", Resources.AttributesWindow16, ViewValues_Clicked) { Enabled = false });
                ContextMenuItems.Add(new ExplorerMenuItem("Gridded Values", null, ViewGridData_Clicked) { Enabled = false });
                ContextMenuItems.Add(new ExplorerMenuItem("Show on Map", Resources.LayerRasterOptimized16, ViewInMap_Clicked) { Enabled = false });
            }
            else if (_AppManager.AppMode == AppMode.HE)
            {
                ContextMenuItems.Add(new ExplorerMenuItem("Load", Resources.AddContent16, LoadData_Clicked));
                ContextMenuItems.Add(new ExplorerMenuItem("Symbology", null, Symbol_Clicked));
                ContextMenuItems.Add(new ExplorerMenuItem("View Values...", Resources.AttributesWindow16, ViewGridData_Clicked) { Enabled = false });
                ContextMenuItems.Add(new ExplorerMenuItem("View in 3D", Resources._3dplot16, ViewInGraphy_Clicked) { Enabled = false });
                ContextMenuItems.Add(new ExplorerMenuItem("Animate", Resources.AnimationVideo16, Animation3D_Clicked) { Enabled = false });
                ContextMenuItems.Add(new ExplorerMenuItem("Export...", null, Export_Clicked) { Enabled = false });
            }
        }

        private DataCube<float> _DataSource;



        public Heiflow.Models.GHM.Item GHMItem
        {
            get;
            private set;
        }

        private void LoadData_Clicked(object sender, EventArgs e)
        {
            _DataSource = GHMItem.LoadArray();
            if (_DataSource != null)
            {
                this.Enable(VariablesFolderContextMenu._AT, true);
                this.Enable("View in 3D", true);
                this.Enable("Export...", true);
                if (_DataSource.Size.Length == 3)
                {
                    this.Enable("Animate", true);
                }
            }
        }
 
        protected override void ProjectItem_PropertyClicked(object sender, EventArgs e)
        {
            base.ProjectItem_PropertyClicked(sender, e);
            _ShellService.PropertyView.SelectedObject = Package;
     
        }

        protected void ViewValues_Clicked(object sender, EventArgs e)
        {
            if (_DataSource != null)
            {
                var array = _DataSource.GetVector(0,"0","0");
                _ShellService.DataGridView.Bind<float>(array);
            }
        }

        private void ViewInMap_Clicked(object sender, EventArgs e)
        {
            if (_DataSource != null)
            {
                var dt = _ProjectService.Project.Model.Grid.FeatureSet.DataTable;
                var array = _DataSource.GetVector(0, "0", "0");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][RegularGrid.ParaValueField] = array[i];
                }
                ApplyScheme(_Package.FeatureLayer, RegularGrid.ParaValueField);
            }
        }

        private void ViewGridData_Clicked(object sender, EventArgs e)
        {
            if (_DataSource != null)
            {
                var grid = _ProjectService.Project.Model.Grid as IGrid;
                var array = _DataSource.GetVector(0, "0", "0");
                var matrix = grid.ToMatrix<float>(array, (float)RegularGrid.NoValue);
                _ShellService.DataGridView.Bind<float>(matrix);
                _ShellService.DataGridView.ShowView();
            }

        }

        public  void LoadMatFromASC()
        {
            string dic = @"E:\Heihe\HRB\GeoData\GBHM\IHRB\上游模型集成数据_qy151228\3.merge_soil\";
            string ascfile = dic + "soil_200001.asc";
           // string acfile = @"C:\Users\Administrator\Documents\GBHM\GBHM\result\sm_month_2000-2012_gbhm.ac";
            AscReader asc = new AscReader();
            AcFile ac = new AcFile();
            var vec = asc.LoadSerial(ascfile, _ProjectService.Project.Model.Grid as IRegularGrid);
            int nfeat = vec.Length;
            DataCube<float> mat = new DataCube<float>(1, 156, nfeat,false);

            int i = 0;
            for (int y = 2000; y < 2013; y++)
            {
                for (int m = 1; m < 13; m++)
                {
                    string fn = dic + "soil_" + y + m.ToString("00") + ".asc";
                    vec = asc.LoadSerial(fn, _ProjectService.Project.Model.Grid as IRegularGrid);
                    var buf = vec;
                    for (int c = 0; c < buf.Length; c++ )
                    {
                        if (buf[c] < 0)
                            buf[c] = 0;
                    }
                    mat[0,i.ToString(),":"] = buf;
                    i++;
                }
            }
           // ac.Save(acfile, mat, new string[] { "Monthly Soil Moisture" });
        }

        private void ViewInGraphy_Clicked(object sender, EventArgs e)
        {
            if (_DataSource != null)
            {
                var grid = _ProjectService.Project.Model.Grid as IGrid;
                var vector = _DataSource.GetVector(0, "0", "0");
                if (MyAppManager.Instance.AppMode == AppMode.VHF)
                {
                    if (_ShellService.SurfacePlot != null)
                    {
                        var min = vector.Min();
                        var mat = grid.ToILMatrix<float>(vector, min);
                        _ShellService.SurfacePlot.ShowView(_ShellService.MainForm);
                        _ShellService.SurfacePlot.PlotSurface(mat);
                    }
                }
                else
                {
                    _Package.Layer3D.RenderObject.Render(vector.Cast<float>().ToArray());
                }
            }
        }

        private void Animation3D_Clicked(object sender, EventArgs e)
        {
            if (_DataSource != null )
            {
                if (_AppManager.AppMode == AppMode.VHF)
                {
                    var gridAnimation = new SurfaceAnimation();
                    //gridAnimation.DataSource = _DataSource;
                    //gridAnimation.VariableIndex = 0;
                    //gridAnimation.AnimatedDimension1 = 1;
                    //gridAnimation.AnimatedDimension2 = 0;
                    gridAnimation.Initialize();
                }
                else if (_AppManager.AppMode == AppMode.HE)
                {
                    //if (Package is IAnimatablePackage)
                    //{
                    //    var ani = (Package as IAnimatablePackage).Animator;
                    //    if (ani == null)
                    //    {
                    //        ani = new GridAnimation(GHMItem.ModelRender);
                    //    }

                    //    _ShellService.AnimationPlayer.Animator = ani;
                    //    _ShellService.AnimationPlayer.Animator.DataSource = _DataSource;
                    //    _ShellService.AnimationPlayer.Animator.VariableIndex = 0;
                    //    _ShellService.AnimationPlayer.Animator.Initialize();

                    //}
                }
            }
        }

        private void Symbol_Clicked(object sender, EventArgs e)
        {
            _ShellService.SymbologyControl.SelectedRender = GHMItem.ModelRender;
        }

        private void Export_Clicked(object sender, EventArgs e)
        {
            LoadMatFromASC();
            //TODO use providers to automatically output
            SaveFileDialog sfld = new SaveFileDialog();
            sfld.Filter = "text file | *.txt | binary file | *.ac";
            sfld.FileName = GHMItem.Name;
            if(sfld.ShowDialog() == DialogResult.OK)
            {
                if(sfld.FileName.Contains(".txt"))
                {
                    MatrixTextStream stream = new MatrixTextStream();
                    var vector = _DataSource.GetVector(0, "0", "0");

                    stream.Save<float>(sfld.FileName, vector);
                }
            }
        }
    }
}
