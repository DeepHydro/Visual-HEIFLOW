using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.WRM
{
    [PackageItem]
    public class WRAPackage : Package
    {
        private int _num_cycle_len = 366;
        public static string PackageName = "WRA";
        private int[] _ncycle_sp;
        public WRAPackage()
        {
            Name = "WRA";
            FullName = "Water Resources Allocation";
            IsDirty = false;
            IsMandatory = true;
            NumStressPeriod = 1;
            NumCycle = 1;
            NumObj = 1;
            EnableDrawdownConstaint = false;
            EnableGWCompensated = true;
            EnableSWStorage = false;
            SourcePriority = 0;

            StressPeriodFiles = new List<string>();
            ManagementSPs = new List<ManagemenSP>();
        }
        //num_wra_sp num_wra_cycle num_cycle_len num_obj_type drawdown_constaint gw_compensate enable_sw_storage water_source_priority
        [Category("Parameter")]
        [Description("Number of stress period of WRA")]
        public int NumStressPeriod
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("Number of total years")]
        public int NumCycle
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("Number of withdraw objects")]
        public int NumObj
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("If enable, groundwater drawdown constraint will be applied")]
        public bool EnableDrawdownConstaint
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("If enable, water demand will be fully compensated by groundwater")]
        public bool EnableGWCompensated
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("If enable, surface water storage will be used for storing surplus diversion")]
        public bool EnableSWStorage
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("0, surface water will be used at first. 1, groundwater will be used at first")]
        public int SourcePriority
        {
            get;
            set;
        }
        /// <summary>
        /// Array size: [1][ncycle][5]. Columns: [year, stress period, days in the year; start day; end day]
        /// </summary>
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<int> CyclePeriod
        {
            get;
            set;
        }
        [Category("Input Files")]
        [Description("WRA input file for each stress period")]
        public List<string> StressPeriodFiles
        {
            get;
            set;
        }
        [Category("Output file")]
        [Description("Summary report report file")]
        public string SummaryReportFile
        {
            get;
            set;
        }
        [Category("Output file")]
        [Description("Management unit report file")]
        public string MangamentUnitReportFile
        {
            get;
            set;
        }
        [Category("Output file")]
        [Description("Budgets report file")]
        public string BudgetsReportFile
        {
            get;
            set;
        }
        [Category("Output file")]
        [Description("Pump report file")]
        public string PumpReportFile
        {
            get;
            set;
        }
        [Browsable(false)]
        public List<ManagemenSP> ManagementSPs
        {
            get;
            protected set;
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override void New()
        {
            base.New();
        }
        public override string CreateFeature(ProjectionInfo proj_info, string directory)
        {
            string filename = Path.Combine(directory, this.Name + ".shp");
            var grid = Owner.Grid as MFGrid;
            FeatureSet fs = new FeatureSet(FeatureType.Polygon);
            fs.Name = this.Name;
            fs.Projection = proj_info;
            fs.DataTable.Columns.Add(new DataColumn("HRU_ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("FARM_ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("Crop_Type", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(double)));

            var farms = ManagementSPs[0].Farms;
            for (int i = 0; i < farms.Length; i++)
            {
                var farm = farms[i];
                for (int j = 0; j < farm.HRU_Num; j++)
                {
                    try
                    {
                        var vertices = grid.LocateNodes(farm.HRU_List[j] - 1);
                        ILinearRing ring = new LinearRing(vertices);
                        Polygon geom = new Polygon(ring);
                        IFeature feature = fs.AddFeature(geom);
                        feature.DataRow.BeginEdit();
                        feature.DataRow["HRU_ID"] = farm.HRU_List[j];
                        feature.DataRow["FARM_ID"] = farm.ID;
                        feature.DataRow["Crop_Type"] = farm.ObjType_HRU[j];
                        feature.DataRow[RegularGrid.ParaValueField] = 0;
                        feature.DataRow.EndEdit();
                    }
                    catch
                    {
                        Debug.WriteLine(j);
                    }
                }
            }
            fs.SaveAs(filename, true);
            fs.Close();
            return filename;
        }

        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                try
                {
                    var line = sr.ReadLine();
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<int>(line, 8);
                    //3 17 366 59 0 1 0 0 # num_wra_sp num_wra_cycle num_cycle_len num_obj_type drawdown_constaint gw_compensate enable_sw_storage water_source_priority
                    NumStressPeriod = buf[0];
                    NumCycle = buf[1];
                    _num_cycle_len = buf[2];
                    NumObj = buf[3];
                    EnableDrawdownConstaint = TypeConverterEx.Num2Bool(buf[4]);
                    EnableGWCompensated = TypeConverterEx.Num2Bool(buf[5]);
                    EnableSWStorage = TypeConverterEx.Num2Bool(buf[6]);
                    SourcePriority = buf[7];

                    _ncycle_sp = new int[NumStressPeriod];
                    CyclePeriod = new DataCube<int>(1, NumCycle, 5)
                    {
                        Layout = DataCubeLayout.TwoD
                    };
                    for (int i = 0; i < NumCycle; i++)
                    {
                        line = sr.ReadLine();
                        buf = TypeConverterEx.Split<int>(line, 5);
                        for (int j = 0; j < 5; j++)
                        {
                            CyclePeriod[0, i, j] = buf[j];
                        }
                        _ncycle_sp[buf[1] - 1]++;
                    }

                    line = sr.ReadLine();
                    StressPeriodFiles.Clear();
                    for (int i = 0; i < NumStressPeriod; i++)
                    {
                        line = sr.ReadLine();
                        StressPeriodFiles.Add(line.Trim());
                    }

                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    SummaryReportFile = line.Trim();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    MangamentUnitReportFile = line.Trim();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    BudgetsReportFile = line.Trim();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    PumpReportFile = line.Trim();

                    ManagementSPs.Clear();
                    for (int i = 0; i < NumStressPeriod; i++)
                    {
                        var fn = Path.Combine(Owner.WorkDirectory, StressPeriodFiles[i]);
                        var sp = LoadManSP(fn, i + 1, _ncycle_sp[i]);

                        ManagementSPs.Add(sp);
                    }
                    
                    result =  LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    result = LoadingState.Warning;
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progresshandler);
                }
                finally
                {
                    fs.Close();
                    sr.Close();
                }
            }
            else
            {
                Message = string.Format("Failed to load {0}. Error message: {1} does not exist.", Name, FileName);
                ShowWarning(Message, progresshandler);
                result =  LoadingState.Warning;
            }
            OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            var grid = Owner.Grid as MFGrid;
            StreamWriter sw = new StreamWriter(filename);
            string line = string.Format("#{0}: created on {1} by Visual HEIFLOW", PackageName, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            sw.WriteLine(line);
            line = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} # num_wra_sp num_wra_cycle num_cycle_len num_alloc_obj drawdown_constraint gw_compensate enable_sw_storage water_source_priority",
                NumStressPeriod, NumCycle, _num_cycle_len, NumObj, TypeConverterEx.Bool2Num(EnableDrawdownConstaint), TypeConverterEx.Bool2Num(EnableGWCompensated),
                TypeConverterEx.Bool2Num(EnableSWStorage), SourcePriority);
            sw.WriteLine(line);
            for (int i = 0; i < NumCycle; i++)
            {
                line = string.Format("{0} {1} {2} {3} {4}", CyclePeriod[0, i, 0], CyclePeriod[0, i, 1], CyclePeriod[0, i, 2], CyclePeriod[0, i, 3], CyclePeriod[0, i, 4]);
                sw.WriteLine(line);
            }
            line = "# WRA stress period file";
            sw.WriteLine(line);
            for (int i = 0; i < NumStressPeriod; i++)
            {
                line = StressPeriodFiles[i];
                sw.WriteLine(line);
            }
            sw.WriteLine("# Summary File");
            sw.WriteLine("1");
            sw.WriteLine(SummaryReportFile);
            sw.WriteLine("# Managment Objects Report File");
            sw.WriteLine("1");
            sw.WriteLine(MangamentUnitReportFile);
            sw.WriteLine("# Budgets Report File");
            sw.WriteLine("1");
            sw.WriteLine(BudgetsReportFile);
            sw.WriteLine("# Pump Report File");
            sw.WriteLine("1");
            sw.WriteLine(PumpReportFile);

            //sw.WriteLine("# hru_row_id matrix");
            //for (int i = 0; i < grid.ActiveCellCount; i++)
            //{
            //    line = (i + 1) + " " + (grid.Topology.ActiveCellLocation[i][0] + 1) + " " + (grid.Topology.ActiveCellLocation[i][1] + 1);
            //    sw.WriteLine(line);
            //}
            sw.Close();
        }

        private ManagemenSP LoadManSP(string filename, int sp, int ncycle)
        {
            ManagemenSP msp = new ManagemenSP(sp);
            StreamReader sr = new StreamReader(filename);
            int nfarm = 0;
            int nindustry = 0;
            double[] bufdouble = null;
            float[] buffloat = null;
            var line = sr.ReadLine();
            line = sr.ReadLine();
            var buf = TypeConverterEx.Split<int>(line, 4);
            nfarm = buf[0];
            nindustry = buf[1];
            var nobj = nfarm + nindustry;
            msp.Quota = new DataCube<float>(ncycle, 366, nobj);
            msp.QuotaFlag = new int[ncycle];
            msp.Farms = new  ManagementObject[nfarm];
            msp.Industries = new  ManagementObject[nindustry];
            msp.FarmCycles = new ManagementCycle[ncycle];
            msp.IndustryCycles = new ManagementCycle[ncycle];

            if (nfarm > 0)
            {
                line = sr.ReadLine();
                for (int i = 0; i < nfarm; i++)
                {
                    line = sr.ReadLine();
                    buf = TypeConverterEx.Split<int>(line, 6);
                    ManagementObject obj = new ManagementObject();
                    //1   739 41  1   3   1	#	oid, hrunum, iseg, ireach, num_well_layer, inlet_type 北大河灌区
                    obj.ID = buf[0];
                    obj.HRU_Num = buf[1];
                    obj.SegID = buf[2];
                    obj.ReachID = buf[3];
                    obj.Num_well_layer = buf[4];
                    obj.Inlet_Type = buf[5];
                    line = sr.ReadLine();
                    buf = TypeConverterEx.Split<int>(line);
                    obj.HRU_List = buf;
                    obj.IHRUList.AddRange(buf);
                    line = sr.ReadLine();
                    bufdouble = TypeConverterEx.Split<double>(line);
                    obj.Canal_Efficiency_list = bufdouble;
                    obj.Canal_Efficiency = bufdouble[0];
                    line = sr.ReadLine();
                    bufdouble = TypeConverterEx.Split<double>(line);
                    obj.Canal_Ratio_list = bufdouble;
                    obj.Canal_Ratio = bufdouble[0];
                    obj.Well_layers = new int[obj.Num_well_layer];
                    obj.Well_ratios = new double[obj.Num_well_layer];
                    for (int j = 0; j < obj.Num_well_layer; j++)
                    {
                        line = sr.ReadLine();
                        bufdouble = TypeConverterEx.Split<double>(line, 2);
                        obj.Well_layers[j] = (int)bufdouble[0];
                        obj.Well_ratios[j] = bufdouble[1];
                    }
                    line = sr.ReadLine();
                    bufdouble = TypeConverterEx.Split<double>(line, 1);
                    obj.Drawdown = bufdouble[0];
                    line = sr.ReadLine();
                    bufdouble = TypeConverterEx.Split<double>(line, 3);
                    obj.Inlet_MinFlow = bufdouble[0];
                    obj.Inlet_MaxFlow = bufdouble[1];
                    obj.Inlet_Flow_Ratio = bufdouble[2];
                    msp.Farms[i] = obj;
                }
            }
            if (nindustry > 0)
            {
                line = sr.ReadLine();
                for (int i = 0; i < nindustry; i++)
                {
                    line = sr.ReadLine();
                    buf = TypeConverterEx.Split<int>(line, 6);
                    ManagementObject obj = new ManagementObject();
                    //1   739 41  1   3   1	#	oid, hrunum, iseg, ireach, num_well_layer, inlet_type 北大河灌区
                    obj.ID = buf[0];
                    obj.HRU_Num = buf[1];
                    obj.SegID = buf[2];
                    obj.ReachID = buf[3];
                    obj.Num_well_layer = buf[4];
                    obj.Inlet_Type = buf[5];
                    line = sr.ReadLine();
                    buf = TypeConverterEx.Split<int>(line);
                    obj.HRU_List = buf;
                    obj.IHRUList.AddRange(buf);
                    obj.Well_layers = new int[obj.Num_well_layer];
                    obj.Well_ratios = new double[obj.Num_well_layer];
                    for (int j = 0; j < obj.Num_well_layer; j++)
                    {
                        line = sr.ReadLine();
                        bufdouble = TypeConverterEx.Split<double>(line, 2);
                        obj.Well_layers[j] = (int)bufdouble[0];
                        obj.Well_ratios[j] = bufdouble[1];
                    }
                    line = sr.ReadLine();
                    bufdouble = TypeConverterEx.Split<double>(line, 1);
                    obj.Drawdown = bufdouble[0];
                    line = sr.ReadLine();
                    bufdouble = TypeConverterEx.Split<double>(line, 3);
                    obj.Inlet_MinFlow = bufdouble[0];
                    obj.Inlet_MaxFlow = bufdouble[1];
                    obj.Inlet_Flow_Ratio = bufdouble[2];
                    line = sr.ReadLine();
                    bufdouble = TypeConverterEx.Split<double>(line, 1);
                    obj.Returnflow_ratio = bufdouble[0];
                    msp.Industries[i] = obj;
                }
            }

            for (int i = 0; i < ncycle; i++)
            {
                line = sr.ReadLine();
                buf = TypeConverterEx.Split<int>(line, 1);
                ManagementCycle farm_cycle = new ManagementCycle(buf[0], msp);
                line = sr.ReadLine();
                buf = TypeConverterEx.Split<int>(line, 1);
                msp.QuotaFlag[i] = buf[0];
                if (msp.QuotaFlag[i] > 0)
                {
                    for (int j = 0; j < nobj; j++)
                    {
                        line = sr.ReadLine();
                        buffloat = TypeConverterEx.Split<float>(line, 366);
                        msp.Quota[i][":", j] = buffloat;
                    }
                }

                //# irrigation objects
                if (nfarm > 0)
                {
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    buf = TypeConverterEx.Split<int>(line, 7);
                    farm_cycle.sw_ratio_flag = buf[0];
                    farm_cycle.swctrl_factor_flag = buf[1];
                    farm_cycle.gwctrl_factor_flag = buf[2];
                    farm_cycle.Withdraw_type_flag = buf[3];
                    farm_cycle.plantarea_flag = buf[4];
                    farm_cycle.max_pump_rate_flag = buf[5];
                    farm_cycle.max_total_pump_flag = buf[6];

                    if (farm_cycle.sw_ratio_flag > 0)
                    {
                        farm_cycle.sw_ratio_day = new DataCube<float>(1, 366, nfarm);
                        for (int j = 0; j < nfarm; j++)
                        {
                            line = sr.ReadLine();
                            buffloat = TypeConverterEx.Split<float>(line, 366);
                            farm_cycle.sw_ratio_day[0][":", j] = buffloat;
                        }
                    }
                    if (farm_cycle.swctrl_factor_flag > 0)
                    {
                        farm_cycle.swctrl_factor_day = new DataCube<float>(1, 366, nfarm);
                        for (int j = 0; j < nfarm; j++)
                        {
                            line = sr.ReadLine();
                            buffloat = TypeConverterEx.Split<float>(line, 366);
                            farm_cycle.swctrl_factor_day[0][":", j] = buffloat;
                        }
                    }
                    if (farm_cycle.gwctrl_factor_flag > 0)
                    {
                        farm_cycle.gwctrl_factor_day = new DataCube<float>(1, 366, nfarm);
                        for (int j = 0; j < nfarm; j++)
                        {
                            line = sr.ReadLine();
                            buffloat = TypeConverterEx.Split<float>(line, 366);
                            farm_cycle.gwctrl_factor_day[0][":", j] = buffloat;
                        }
                    }
                    if (farm_cycle.Withdraw_type_flag > 0)
                    {
                        for (int j = 0; j < nfarm; j++)
                        {
                            line = sr.ReadLine();
                            buf = TypeConverterEx.Split<int>(line, msp.Farms[j].HRU_Num);
                            msp.Farms[j].ObjType_HRU = buf;
                        }
                    }

                    if (farm_cycle.plantarea_flag > 0)
                    {
                        for (int j = 0; j < nfarm; j++)
                        {
                            line = sr.ReadLine();
                            msp.Farms[j].HRU_Area = TypeConverterEx.Split<double>(line, msp.Farms[j].HRU_Num);
                        }
                    }

                    if (farm_cycle.max_pump_rate_flag > 0)
                    {
                        for (int j = 0; j < nfarm; j++)
                        {
                            line = sr.ReadLine();
                            msp.Farms[j].Max_Pump_Rate = TypeConverterEx.Split<double>(line, msp.Farms[j].HRU_Num);
                        }
                    }

                    if (farm_cycle.max_total_pump_flag > 0)
                    {
                        line = sr.ReadLine();
                        bufdouble = TypeConverterEx.Split<double>(line, nfarm);
                        for (int j = 0; j < nfarm; j++)
                        {
                            msp.Farms[j].Max_Total_Pump = bufdouble[j];
                        }
                    }
                }
                msp.FarmCycles[i] = farm_cycle;


                //# industry objects
                ManagementCycle indust_cycle = new ManagementCycle(farm_cycle.Cycle, msp);
                if (nindustry > 0)
                {
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    //-1 -1	-1	-1	 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag
                    buf = TypeConverterEx.Split<int>(line, 4);
                    indust_cycle.sw_ratio_flag = buf[0];
                    indust_cycle.swctrl_factor_flag = buf[1];
                    indust_cycle.gwctrl_factor_flag = buf[2];
                    indust_cycle.Withdraw_type_flag = buf[3];
                    if (indust_cycle.sw_ratio_flag > 0)
                    {
                        indust_cycle.sw_ratio_day = new DataCube<float>(1, 366, nindustry);
                        for (int j = 0; j < nindustry; j++)
                        {
                            line = sr.ReadLine();
                            buffloat = TypeConverterEx.Split<float>(line, 366);
                            indust_cycle.sw_ratio_day[0][":", j] = buffloat;
                        }
                    }
                    if (indust_cycle.swctrl_factor_flag > 0)
                    {
                        indust_cycle.swctrl_factor_day = new DataCube<float>(1, 366, nindustry);
                        for (int j = 0; j < nindustry; j++)
                        {
                            line = sr.ReadLine();
                            buffloat = TypeConverterEx.Split<float>(line, 366);
                            indust_cycle.swctrl_factor_day[0][":", j] = buffloat;
                        }
                    }
                    if (indust_cycle.gwctrl_factor_flag > 0)
                    {
                        indust_cycle.gwctrl_factor_day = new DataCube<float>(1, 366, nindustry);
                        for (int j = 0; j < nindustry; j++)
                        {
                            line = sr.ReadLine();
                            buffloat = TypeConverterEx.Split<float>(line, 366);
                            indust_cycle.gwctrl_factor_day[0][":", j] = buffloat;
                        }
                    }
                    if (indust_cycle.Withdraw_type_flag > 0)
                    {
                        for (int j = 0; j < nindustry; j++)
                        {
                            line = sr.ReadLine();
                            buf = TypeConverterEx.Split<int>(line, msp.Industries[j].HRU_Num);
                            msp.Industries[j].ObjType_HRU = buf;
                        }
                    }
                }
            }
            sr.Close();
            return msp;
        }

        public override void Clear()
        {
            StressPeriodFiles.Clear();
            ManagementSPs.Clear();
            base.Clear();
        }
    }
}
