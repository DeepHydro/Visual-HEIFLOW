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

using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Models.WRM;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.DataManagement
{
    public class CreateWRAMaster: MapLayerRequiredTool
    {
        private string _QuotaFileName;
        private List<ManagementObject> irrg_obj_list = new List<ManagementObject>();
        private List<ManagementObject> indust_obj_list = new List<ManagementObject>();
        private IMapLayerDescriptor _FarmLayer;
        private IRaster _lu_raster;
        private IFeatureSet _farm_fs;
        private string[] _cropland_codes;
        //private string _OutputFileName;
        public CreateWRAMaster()
        {
            Name = "Create WRA Master File";
            Category = "Irrigation Model";
            Description = "Create Water Allocation Module Master Input File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            EndCycle = 20;
            StartCycle = 1;
            PumpingLayers = "1,2,3";
            PumpingLayerRatios = "0.6,0.2,0.2";
            CroplandCodes = "1";
            GWCompensate = true;
        }

        [Category("Input Layers")]
        [Description("Grid centroid layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor CentroidLayer
        {
            get;
            set;
        }
        [Category("Input Layers")]
        [Description("Farm layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor FarmLayer
        {
            get
            {
                return _FarmLayer;
            }
            set
            {
                _FarmLayer = value;
                if (_FarmLayer != null)
                {
                    var buf = from DataColumn dc in (_FarmLayer.DataSet as IFeatureSet).DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }
        [Category("Input Layers")]
        [Description("Landuse layer in raster format")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor LanduseLayer
        {
            get;
            set;
        }

        #region Fields binding
        [Category("Farm Parameters")]
        [Description("Field name of the farm name")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string FarmName
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm type: 0 is farm; 1 is industry")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string FarmType
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SWRatio
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string MaxDrawdown
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SegmentID
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ReachID
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string CanalAreaRatio
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string CanalEfficiency
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string InletType
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string InletMaxflow
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string InletMinflow
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("Field name of the farm nae")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string InletRatio
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("An integer array that specfies the layers from which pumping are applied. An example is: 1,2,3")]
        public string PumpingLayers
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("An double array that specifies the ratio of pumping in each layer. An example is: 0.6,0.2,0.2")]
        public string PumpingLayerRatios
        {
            get;
            set;
        }
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }
        #endregion

        [Category("Input Files")]
        [Description("The quota filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string QuotaFileName
        {
            get
            {
                return _QuotaFileName;
            }
            set
            {
                _QuotaFileName = value;
            }
        }
        [Category("General Parameters")]
        [Description("The end index of the allocation period. It must be greater than StartCycle")]
        public int EndCycle
        {
            get;
            set;
        }
        [Category("General Parameters")]
        [Description("The start index of the allocation period. It must be greater than 0")]
        public int StartCycle
        {
            get;
            set;
        }
        [Category("General Parameters")]
        [Description("If true, demand will be compensated by GW")]
        public bool GWCompensate
        {
            get;
            set;
        }
        [Category("General Parameters")]
        [Description("An array that specifies the codes of cropland in the Landuse layer. An example is: 1,2")]
        public string CroplandCodes
        {
            get;
            set;
        }

       // [Category("Output Files")]
       // [Description("The output filename")]
       //[Browsable(false)]
       // public string OutputFileName
       // {
       //     get
       //     {
       //         if (TypeConverterEx.IsNotNull(_OutputFileName))
       //         {
       //             var fn = Path.Combine(ModelService.WorkDirectory, _OutputFileName);
       //             var dic = Path.GetDirectoryName(fn);
       //            if(!Directory.Exists(dic))
       //                 Directory.CreateDirectory(dic);
       //             return Path.Combine(ModelService.WorkDirectory, _OutputFileName);
       //         }
       //         else
       //             return _OutputFileName;
       //     }
       //     set
       //     {
       //          _OutputFileName = value;
       //     }
       // }

        public override void Initialize()
        {
            this.Initialized = true;
            _lu_raster = LanduseLayer.DataSet as IRaster;
            _farm_fs = FarmLayer.DataSet as IFeatureSet;
            if (_lu_raster == null || _farm_fs == null || _farm_fs.FeatureType != FeatureType.Polygon)
                this.Initialized = false;
            try
            {
                _cropland_codes = TypeConverterEx.Split<string>(CroplandCodes);
            }
            catch
            {
                this.Initialized = false;
            }
            if(TypeConverterEx.IsNotNull(QuotaFileName))
            {
                if(!File.Exists(QuotaFileName))
                    this.Initialized = false;
            }
            else
            {
                this.Initialized = false;
            }
        }

        private void GetFarmObj()
        {
            irrg_obj_list.Clear();
            indust_obj_list.Clear();
            //List<ManagementObject> farms = new List<ManagementObject>();
            int nfarm = _farm_fs.Features.Count;
            double swratio = 0.8;
            double drawdown = 2;
            double canal_ratio = 0.3;
            double canal_effcy = 0.7;
            int inlet_type = 1;
            double inlet_maxflow = 86400;
            double inlet_minflow = 0;
            double inlet_ratio = 0;
            int segid = 1;
            int rchid = 1;
            int farmtype = 0;
            string sw_ctl = "";
            string gw_ctl = "";
            string farmname = "farm";
            for (int i = 0; i < 366; i++)
            {
                sw_ctl += "1\t";
                gw_ctl += "1\t";
            }
            for (int i = 0; i < nfarm; i++)
            {
                DataRow dr = _farm_fs.DataTable.Rows[i];
                if (TypeConverterEx.IsNotNull(FarmName))
                    farmname = dr[FarmName].ToString();
                else
                    farmname = "Farm " + (i + 1);
                if (TypeConverterEx.IsNotNull(SWRatio))
                    double.TryParse(dr[SWRatio].ToString(), out swratio);
                if (TypeConverterEx.IsNotNull(MaxDrawdown))
                    double.TryParse(dr[MaxDrawdown].ToString(), out drawdown);
                if (TypeConverterEx.IsNotNull(CanalEfficiency))
                    double.TryParse(dr[CanalEfficiency].ToString(), out canal_effcy);
                if (TypeConverterEx.IsNotNull(CanalAreaRatio))
                    double.TryParse(dr[CanalAreaRatio].ToString(), out canal_ratio);
                if (TypeConverterEx.IsNotNull(SegmentID))
                    int.TryParse(dr[SegmentID].ToString(), out segid);
                if (TypeConverterEx.IsNotNull(ReachID))
                    int.TryParse(dr[ReachID].ToString(), out rchid);
                if(TypeConverterEx.IsNotNull(InletMaxflow))
                    double.TryParse(dr[InletMaxflow].ToString(), out inlet_maxflow);
                if (TypeConverterEx.IsNotNull(InletMinflow))
                    double.TryParse(dr[InletMinflow].ToString(), out inlet_minflow);
                if (TypeConverterEx.IsNotNull(InletRatio))
                    double.TryParse(dr[InletRatio].ToString(), out inlet_ratio);
                if (TypeConverterEx.IsNotNull(InletType))
                    int.TryParse(dr[InletType].ToString(), out inlet_type);
                if (TypeConverterEx.IsNotNull(FarmType))
                    int.TryParse(dr[FarmType].ToString(), out farmtype);

                ManagementObject obj = new ManagementObject()
                {
                    ID = i + 1,
                    Name = farmname,
                    SW_Ratio = swratio,
                    ObjType = i+1,
                    FramType = farmtype,
                    Drawdown = drawdown,
                    SegID = segid,
                    ReachID = rchid,
                    Canal_Efficiency = canal_effcy,
                    Canal_Ratio = canal_ratio,
                    Inlet_Type = inlet_type,
                    Inlet_MaxFlow = inlet_maxflow,
                    Inlet_MinFlow = inlet_minflow,
                    Inlet_Flow_Ratio = inlet_ratio,
                    SW_Cntl_Factor = sw_ctl,
                    GW_Cntl_Factor = gw_ctl
                };

                if (farmtype == 0)
                    irrg_obj_list.Add(obj);
                else
                    indust_obj_list.Add(obj);
            }
 
        }

        private void GetIHRU(List<ManagementObject> farms)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            var mfgrid = model.Grid as RegularGrid;
            //Coordinate centroid = null;
            string code = "";
            var cellarea = mfgrid.GetCellArea(); 
            for (int i = 0; i < mfgrid.ActiveCellCount; i++)
            {
                //centroid = mfgrid.LocateCentroid(mfgrid.Topology.ActiveCellLocation[i][1] + 1, mfgrid.Topology.ActiveCellLocation[i][0] + 1);
                var centroid = mfgrid.LocateCentroidPoint(mfgrid.Topology.ActiveCellLocation[i][1] + 1, mfgrid.Topology.ActiveCellLocation[i][0] + 1);
                for (int j = 0; j < _farm_fs.Features.Count; j++)
                {
                    //if (SpatialRelationship.PointInPolygon(_farm_fs.Features[j].Geometry.Coordinates, centroid))
                    if (centroid.Within(_farm_fs.Features[j].Geometry))
                    {
                        var cell = _lu_raster.ProjToCell(centroid.Coordinate);
                        if (cell.Column > -1 && cell.Row > -1)
                        {
                            code = _lu_raster.Value[cell.Row, cell.Column].ToString();
                            if(_cropland_codes.Contains(code))
                            {
                                farms[j].IHRUList.Add(i + 1);
                            }
                        }
                        break;
                    }
                }
            }
            List<ManagementObject> removedlist = new List<ManagementObject>();
            for (int i = 0; i < _farm_fs.Features.Count; i++)
            {
                var farm = farms[i];
                if (farm.IHRUList.Count > 0)
                {
                    farm.HRU_List = farm.IHRUList.ToArray();
                    farm.HRU_Num = farm.IHRUList.Count;
                    farm.HRU_Area = new double[farm.HRU_Num];
                    for (int j = 0; j < farm.HRU_Num; j++)
                    {
                        farm.HRU_Area[j] = cellarea;
                    }
                }
                else
                {
                    removedlist.Add(farm);
                }
            }
            for (int i = 0; i < removedlist.Count; i++)
                farms.Remove(removedlist[i]);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as HeiflowModel;
            var wmm = model.WaterManagementModel;
            var mfgrid = model.Grid as RegularGrid;
            Modflow mf = model.ModflowModel;

            int[] well_layer = TypeConverterEx.Split<int>(PumpingLayers);
            double[] layer_ratio = TypeConverterEx.Split<double>(PumpingLayerRatios);
            StreamReader sr_quota = null;
            StreamWriter sw_out = null;
            if (well_layer.Length != layer_ratio.Length)
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed to run. Error messae: the format of PumpingLayers or PumpingLayerRatios is wrong.");
                return false;
            }

            try
            {
                WRAPackage wra_pck = null;
                wmm.MasterPackage.WRAModule = "auto_wra";
                wmm.MasterPackage.WRAModuleFile = string.Format(".\\Input\\Extension\\{0}.wra", prj.Project.Name);

                if (wmm.Packages.ContainsKey(WRAPackage.PackageName))
                {
                    wra_pck = wmm.Packages[WRAPackage.PackageName] as WRAPackage;
                    wra_pck.Clear();
                    wra_pck.FileName = wmm.MasterPackage.WRAModuleFile;
                }
                else
                {
                    wra_pck = new WRAPackage
                    {
                        FileName = wmm.MasterPackage.WRAModuleFile
                    };
                    wmm.Add(wra_pck);
                    wra_pck.Initialize();
                }

                wra_pck.NumCycle = model.TimeService.End.Year - model.TimeService.Start.Year + 1;
                wra_pck.NumStressPeriod = 1;
                this.EndCycle = wra_pck.NumCycle;
                wra_pck.CyclePeriod = new DataCube<int>(1, wra_pck.NumCycle, 5);
                wra_pck.CyclePeriod[0, 0, 0] = model.TimeService.Start.Year;
                wra_pck.CyclePeriod[0, 0, 1] = 1;
                wra_pck.CyclePeriod[0, 0, 2] = DateTime.IsLeapYear(model.TimeService.Start.Year) ? 366 : 355;
                wra_pck.CyclePeriod[0, 0, 3] = 1;
                wra_pck.CyclePeriod[0, 0, 4] = wra_pck.CyclePeriod[0, 0, 3] + wra_pck.CyclePeriod[0, 0, 2] - 1;
                wra_pck.BudgetsReportFile = ".\\output\\wra_budgets.csv";
                wra_pck.MangamentUnitReportFile = ".\\output\\man_units.csv";
                wra_pck.SummaryReportFile = ".\\output\\wra_summary.csv";
                wra_pck.PumpReportFile = ".\\output\\wra_pump_report.csv";
                var wraspfile = ".\\Input\\wra\\wra_sp1.txt";
                wra_pck.StressPeriodFiles.Add(wraspfile);

                for (int i = model.TimeService.Start.Year + 1; i <= model.TimeService.End.Year; i++)
                {
                    var cur_row = i - model.TimeService.Start.Year;
                    wra_pck.CyclePeriod[0, cur_row, 0] = i;
                    wra_pck.CyclePeriod[0, cur_row, 1] = 1;
                    wra_pck.CyclePeriod[0, cur_row, 2] = DateTime.IsLeapYear(i) ? 366 : 365;
                    wra_pck.CyclePeriod[0, cur_row, 3] = wra_pck.CyclePeriod[0, cur_row - 1, 4] + 1;
                    wra_pck.CyclePeriod[0, cur_row, 4] = wra_pck.CyclePeriod[0, cur_row, 3] + wra_pck.CyclePeriod[0, cur_row, 2] - 1;
                }

                cancelProgressHandler.Progress("Package_Tool", 10, "Reading quota file");
                sr_quota = new StreamReader(QuotaFileName);
                int nquota = 1;
                int ntime = 36;
                var line = sr_quota.ReadLine();
                var strs_buf = TypeConverterEx.Split<string>(line);
                nquota = int.Parse(strs_buf[0]);
                ntime = int.Parse(strs_buf[1]);
                double[,] quota_src = new double[ntime, nquota];
                double[,] quota = new double[366, nquota];
                int day = 0;
                var start = new DateTime(2000, 1, 1);
                for (int i = 0; i < ntime; i++)
                {
                    line = sr_quota.ReadLine().Trim();
                    var buf = TypeConverterEx.Split<string>(line);
                    var ss = DateTime.Parse(buf[0]);
                    var ee = DateTime.Parse(buf[1]);
                    var cur = ss;
                    var step = (ee - ss).Days + 1;
                    while (cur <= ee)
                    {
                        for (int j = 0; j < nquota; j++)
                            quota[day, j] = System.Math.Round(double.Parse(buf[2 + j]) / step, 2);
                        day++;
                        cur = cur.AddDays(1);
                    }
                }

                cancelProgressHandler.Progress("Package_Tool", 10, "Reading farm objects...");
                GetFarmObj();
                cancelProgressHandler.Progress("Package_Tool", 20, "Reading irrigated HRU...");
                GetIHRU(irrg_obj_list);

                cancelProgressHandler.Progress("Package_Tool", 50, "Writing WRA master file...");
                wra_pck.NumObj = irrg_obj_list.Count + indust_obj_list.Count;
                wra_pck.IsDirty = true;
                wra_pck.ChangeState(ModelObjectState.Ready);
                wra_pck.Save(cancelProgressHandler);
                wmm.MasterPackage.IsDirty = true;
                wmm.MasterPackage.Save(cancelProgressHandler);

                WRASPFile spfile = new WRASPFile();
                spfile.SavePumpWellFiles(shell, prj.Project, mf, mfgrid, irrg_obj_list, well_layer, layer_ratio);
                cancelProgressHandler.Progress("Package_Tool", 80, "Modflow wel file saved.");
                var spfilename = Path.Combine(prj.Project.WRAInputDirectory, "wra_sp1.txt");
                spfile.SaveWRAFile(spfilename, irrg_obj_list, indust_obj_list, quota, nquota, well_layer, layer_ratio, StartCycle, EndCycle);
                cancelProgressHandler.Progress("Package_Tool", 100, "HRU wel file saved.");
            }
            catch(Exception ex)
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed to run. Error message: " + ex.Message);
            }
            finally
            {
                sr_quota.Close();
                sw_out.Close();
                cancelProgressHandler.Progress("Package_Tool", 100, "File saved.");
            }
            return true;
        }
        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as HeiflowModel;

            if (model != null)
            {
                var wra = prj.Project.Model.GetPackage(WRAPackage.PackageName) as WRAPackage;
                wra.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                shell.ProjectExplorer.ClearContent();
                shell.ProjectExplorer.AddProject(prj.Project);
            }
        }
    }

}