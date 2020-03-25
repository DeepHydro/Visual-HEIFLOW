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
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Tools;
using Heiflow.Models.WRM;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.DataManagement
{
    public class WRAInput: MapLayerRequiredTool
    {
        private string _QuotaFileName;
        private List<ManagementObject> irrg_obj_list = new List<ManagementObject>();
        private List<ManagementObject> indust_obj_list = new List<ManagementObject>();
        private IMapLayerDescriptor _FarmLayer;
        private IRaster _lu_raster;
        private IFeatureSet _farm_fs;
        private string[] _cropland_codes;
        private string _OutputFileName;
        public WRAInput()
        {
            Name = "Create WRA Input File";
            Category = "Irrigation Model";
            Description = "Create Water Allocation Module Input File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            EndCycle = 10;
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
        [Description("Landuse layer")]
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
        //[Category("Farm Parameters")]
        //[Description("Field name of the farm nae")]
        //[EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        //[DropdownListSource("Fields")]
        //public string FarmType
        //{
        //    get;
        //    set;
        //}
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

        [Category("Output Files")]
        [Description("The output filename")]
       [Browsable(false)]
        public string OutputFileName
        {
            get
            {
                if (TypeConverterEx.IsNotNull(_OutputFileName))
                {
                    var fn = Path.Combine(ModelService.WorkDirectory, _OutputFileName);
                    var dic = Path.GetDirectoryName(fn);
                   if(!Directory.Exists(dic))
                        Directory.CreateDirectory(dic);
                    return Path.Combine(ModelService.WorkDirectory, _OutputFileName);
                }
                else
                    return _OutputFileName;
            }
            set
            {
                 _OutputFileName = value;
            }
        }

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

        private List<ManagementObject> GetFarmObj()
        {
            List<ManagementObject> farms = new List<ManagementObject>();
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
            string sw_ctl = "";
            string gw_ctl = "";
            int farmtype = 1;
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

                ManagementObject obj = new ManagementObject()
                {
                    ID = i + 1,
                    Name = farmname,
                    SW_Ratio = swratio,
                    ObjType_Fram = farmtype,
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
                farms.Add(obj);
            }
            return farms;
        }

        private void GetIHRU(List<ManagementObject> farms)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            var mfgrid = model.Grid as RegularGrid;
            Coordinate centroid = null;
            string code = "";
            var cellarea = mfgrid.GetCellArea(); 
            for (int i = 0; i < mfgrid.ActiveCellCount; i++)
            {
                centroid = mfgrid.LocateCentroid(mfgrid.Topology.ActiveCellLocation[i][1] + 1, mfgrid.Topology.ActiveCellLocation[i][0] + 1);
                for (int j = 0; j < _farm_fs.Features.Count; j++)
                {
                    if (SpatialRelationship.PointInPolygon(_farm_fs.Features[j].Geometry.Coordinates, centroid))
                    {
                        var cell = _lu_raster.ProjToCell(centroid);
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

        private void CalcObjPumpConstraint(List<ManagementObject> list, double[,] quota)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var obj = list[i];
                obj.Max_Pump_Rate = new double[obj.HRU_Num];
                obj.Max_Total_Pump = 0;
                var buf = TypeConverterEx.Split<double>(obj.GW_Cntl_Factor, 366);  
                for (int j = 0; j < obj.HRU_Num; j++)
                {
                    double temp = 0;
                    int pump_days = 0;
                    for (int k = 0; k < 366; k++)
                    {
                        if (buf[k] > 0)
                        {
                            temp += obj.HRU_Area[j] * quota[k, i] / 1000 * (1 - obj.SW_Ratio);
                            pump_days++;
                        }
                    }
                    obj.Max_Pump_Rate[j] = System.Math.Round(temp / pump_days, 0);
                    obj.Max_Total_Pump += temp;
                }
                obj.Max_Total_Pump = System.Math.Round(obj.Max_Total_Pump, 0);
            }
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as HeiflowModel;
            var wmm = model.WaterManagementModel;
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
                _OutputFileName = string.Format(".\\Input\\Extension\\{0}.unit", prj.Project.Name);
                wra_pck.StressPeriodFiles.Add(_OutputFileName);

                var out_dic = Path.Combine(model.WorkDirectory, ".\\output");
                if(!Directory.Exists(out_dic))
                    DirectoryHelper.Create(out_dic);

                for (int i = model.TimeService.Start.Year + 1; i <= model.TimeService.End.Year; i++)
                {
                    var cur_row = i - model.TimeService.Start.Year;
                    wra_pck.CyclePeriod[0, cur_row, 0] = i;
                    wra_pck.CyclePeriod[0, cur_row, 1] = 1;
                    wra_pck.CyclePeriod[0, cur_row, 2] = DateTime.IsLeapYear(i) ? 366 : 365;
                    wra_pck.CyclePeriod[0, cur_row, 3] = wra_pck.CyclePeriod[0, cur_row - 1, 4] + 1;
                    wra_pck.CyclePeriod[0, cur_row, 4] = wra_pck.CyclePeriod[0, cur_row, 3] + wra_pck.CyclePeriod[0, cur_row, 2] - 1;
                }

                OutputFileName = wra_pck.StressPeriodFiles[0];
                int num_well_layer = well_layer.Length;
                int num_irrg_obj, num_indust_obj;
                sr_quota = new StreamReader(QuotaFileName);
                sw_out = new StreamWriter(OutputFileName);
                string newline = "";
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
                cancelProgressHandler.Progress("Package_Tool", 10, "Getting farm objects...");
                irrg_obj_list = GetFarmObj();
                cancelProgressHandler.Progress("Package_Tool", 20, "Getting irrigated HRU...");
                GetIHRU(irrg_obj_list);
                cancelProgressHandler.Progress("Package_Tool", 80, "Calculating pumping constraint...");
                CalcObjPumpConstraint(irrg_obj_list, quota);

                cancelProgressHandler.Progress("Package_Tool", 85, "Writing WRA master file...");
                num_irrg_obj = irrg_obj_list.Count;
                num_indust_obj = indust_obj_list.Count;
                newline = "# Water resources allocation package " + DateTime.Now;
                sw_out.WriteLine(newline);
                newline = string.Format("{0}\t{1}\t0\t0\t # num_irrg_obj, num_indu_obj, num_doms_obj, num_ecos_obj ", num_irrg_obj, num_indust_obj);
                sw_out.WriteLine(newline);

                wra_pck.NumObj = num_irrg_obj;
                cancelProgressHandler.Progress("Package_Tool", 90, "Writing WRA units file...");
                if (num_irrg_obj > 0)
                {
                    sw_out.WriteLine("# irrigation objects");
                    for (int i = 0; i < num_irrg_obj; i++)
                    {
                        var obj = irrg_obj_list[i];
                        int oid = i + 1;
                        newline = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t#	oid, hrunum, iseg, ireach, num_well_layer, inlet_type {6}", obj.ID, obj.HRU_Num, obj.SegID, obj.ReachID, num_well_layer, obj.Inlet_Type, obj.Name);
                        sw_out.WriteLine(newline);
                        newline = string.Join("\t", obj.HRU_List);
                        sw_out.WriteLine(newline);
                        var canal_eff = new double[obj.HRU_Num];
                        var canal_ratio = new double[obj.HRU_Num];
                        for (int j = 0; j < obj.HRU_Num; j++)
                        {
                            canal_eff[j] = obj.Canal_Efficiency;
                            canal_ratio[j] = obj.Canal_Ratio;
                        }
                        newline = string.Join("\t", canal_eff);
                        sw_out.WriteLine(newline);
                        newline = string.Join("\t", canal_ratio);
                        sw_out.WriteLine(newline);
                        for (int j = 0; j < num_well_layer; j++)
                        {
                            newline = well_layer[j] + "\t" + layer_ratio[j] + " # well_layer layer_ratio";
                            sw_out.WriteLine(newline);
                        }
                        newline = string.Format("{0}\t#	drawdown constaint of {1}", irrg_obj_list[i].Drawdown, irrg_obj_list[i].Name);
                        sw_out.WriteLine(newline);
                        newline = string.Format("{0}\t{1}\t{2}\t#  inlet	min flow,  max flow and flow ratio  for {3}", irrg_obj_list[i].Inlet_MinFlow, irrg_obj_list[i].Inlet_MaxFlow, irrg_obj_list[i].Inlet_Flow_Ratio,
                            irrg_obj_list[i].Name);
                        sw_out.WriteLine(newline);
                    }
                }
                if (num_indust_obj > 0)
                {
                    sw_out.WriteLine("# industrial objects");
                    for (int i = 0; i < num_indust_obj; i++)
                    {
                        var obj = indust_obj_list[i];
                        newline = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t#	oid, hrunum, iseg, ireach, num_well_layer, inlet_type {6}", obj.ID, obj.HRU_Num, obj.SegID, obj.ReachID, num_well_layer,
                         obj.Inlet_Type, obj.Name);
                        sw_out.WriteLine(newline);
                        newline = string.Format("{0}\t#	hru_id_list", string.Join(" ", obj.HRU_List));
                        sw_out.WriteLine(newline);
                        for (int j = 0; j < num_well_layer; j++)
                        {
                            newline = well_layer[j] + "\t" + layer_ratio[j] + " # well_layer layer_ratio";
                            sw_out.WriteLine(newline);
                        }
                        newline = string.Format("{0}\t#	drawdown constaint of object {1}", obj.Drawdown, obj.Name);
                        sw_out.WriteLine(newline);
                        newline = string.Format("{0}\t{1}\t{2}\t#  minflow,  max_flow and flow ratio for {3}", obj.Inlet_MinFlow, obj.Inlet_MaxFlow, obj.Inlet_Flow_Ratio, obj.Name);
                        sw_out.WriteLine(newline);
                        newline = string.Format("{0}\t#	return_ratio", 0);
                        sw_out.WriteLine(newline);
                    }
                }

                sw_out.WriteLine(StartCycle + " # cycle index");
                sw_out.WriteLine("1	#	quota_flag");
                for (int i = 0; i < nquota; i++)
                {
                    newline = "";
                    for (int j = 0; j < 366; j++)
                    {
                        newline += quota[j, i].ToString("0.0") + "\t";
                    }
                    newline += "quota of object " + (i + 1);
                    sw_out.WriteLine(newline);
                }

                if (num_irrg_obj > 0)
                {
                    newline = "# irrigation objects";
                    sw_out.WriteLine(newline);
                    if (GWCompensate)
                        newline = "1	1	1	1	1	1	1	#	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag,max_pump_rate_flag,max_total_pump_flag";
                    else
                        newline = "1	1	1	1	1	1	1	#	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag";
                    sw_out.WriteLine(newline);
                    //地表水比例
                    for (int i = 0; i < num_irrg_obj; i++)
                    {
                        var ratio = irrg_obj_list[i].SW_Ratio;
                        newline = "";
                        for (int j = 0; j < 366; j++)
                        {
                            newline += ratio.ToString("0.00") + "\t";
                        }
                        newline += "#SW ratio of object " + irrg_obj_list[i].ID;
                        sw_out.WriteLine(newline);
                    }
                    //地表引水控制系数
                    for (int i = 0; i < num_irrg_obj; i++)
                    {
                        newline = string.Format("{0}\t#SW control factor of object {1}", irrg_obj_list[i].SW_Cntl_Factor, irrg_obj_list[i].ID);
                        sw_out.WriteLine(newline);
                    }
                    //地下引水控制系数
                    for (int i = 0; i < num_irrg_obj; i++)
                    {
                        newline = string.Format("{0}\t#GW control factor of object {1}", irrg_obj_list[i].GW_Cntl_Factor, irrg_obj_list[i].ID);
                        sw_out.WriteLine(newline);
                    }
                    //作物类型
                    for (int i = 0; i < num_irrg_obj; i++)
                    {
                        newline = "";
                        for (int j = 0; j < irrg_obj_list[i].HRU_Num; j++)
                        {
                            newline += irrg_obj_list[i].ObjType_Fram + "\t";
                        }
                        newline += "# Plant type of object " + (i + 1);
                        sw_out.WriteLine(newline);
                    }
                    //种植面积
                    for (int i = 0; i < num_irrg_obj; i++)
                    {
                        newline = string.Join("\t", irrg_obj_list[i].HRU_Area);
                        newline += "\t" + "# Plant area of object " + irrg_obj_list[i].ID;
                        sw_out.WriteLine(newline);
                    }
                    if (GWCompensate)
                    {
                        //每个HRU的地下水抽水能力
                        for (int i = 0; i < num_irrg_obj; i++)
                        {
                            newline = string.Join("\t", irrg_obj_list[i].Max_Pump_Rate);
                            newline += "\t" + "# Maximum pumping rate of object " + irrg_obj_list[i].ID;
                            sw_out.WriteLine(newline);
                        }
                        //每个HRU的最大地下水抽水量
                        var objbuf = from ir in irrg_obj_list select (ir.Max_Total_Pump);
                        newline = string.Join("\t", objbuf);
                        newline += "\t" + "# Total maximum pumping amonut";
                        sw_out.WriteLine(newline);
                    }
                }

                if (num_indust_obj > 0)
                {
                    newline = "# industrial objects";
                    sw_out.WriteLine(newline);
                    newline = "1 1	1	1	1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag";
                    sw_out.WriteLine(newline);

                    for (int i = 0; i < num_indust_obj; i++)
                    {
                        newline = "";
                        var control = 1;
                        for (int j = 0; j < 366; j++)
                        {
                            newline += control + "\t";
                        }
                        newline += "# SW control factor of object " + (indust_obj_list[i].ID);
                        sw_out.WriteLine(newline);
                    }

                    //地表引水控制系数
                    for (int i = 0; i < num_indust_obj; i++)
                    {
                        newline = string.Format("{0}\t#SW control factor of object {1}", indust_obj_list[i].SW_Cntl_Factor, indust_obj_list[i].ID);
                        sw_out.WriteLine(newline);
                    }

                    //地下引水控制系数
                    for (int i = 0; i < num_indust_obj; i++)
                    {
                        newline = string.Format("{0}\t#GW control factor of object {1}", indust_obj_list[i].GW_Cntl_Factor, indust_obj_list[i].ID);
                        sw_out.WriteLine(newline);
                    }

                    //用水类型
                    for (int i = 0; i < num_indust_obj; i++)
                    {
                        var obj = indust_obj_list[i];
                        newline = string.Format("{0} # Withdraw type of object {1}", obj.ObjType_Fram, obj.ID);
                        sw_out.WriteLine(newline);
                    }
                }

                for (int i = StartCycle + 1; i <= EndCycle; i++)
                {
                    sw_out.WriteLine(i + " # cycle index");
                    sw_out.WriteLine("-1 # quota_flag");
                    if (num_irrg_obj > 0)
                    {
                        sw_out.WriteLine("# irrigation objects");
                        if (GWCompensate)
                            sw_out.WriteLine("-1 -1	-1 -1 -1 -1 -1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag,max_pump_rate_flag,max_total_pump_flag");
                        else
                            sw_out.WriteLine("-1 -1	-1 -1 -1 -1 -1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag");
                    }
                    if (num_indust_obj > 0)
                    {
                        sw_out.WriteLine("# industrial objects");
                        sw_out.WriteLine("-1 -1	-1	-1	-1  #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag");
                    }
                }
            
                wra_pck.IsDirty = true;
                wra_pck.ChangeState(ModelObjectState.Ready);
                wra_pck.Save(cancelProgressHandler);
                wmm.MasterPackage.IsDirty = true;
                wmm.MasterPackage.Save(cancelProgressHandler);

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