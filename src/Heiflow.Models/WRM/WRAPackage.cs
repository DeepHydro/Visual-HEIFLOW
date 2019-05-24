using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<int> StressPeriod
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
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override bool New()
        {
            base.New();
            return true;
        }
        public override bool Load(ICancelProgressHandler progresshandler)
        {
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
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

                StressPeriod = new DataCube<int>(1, NumCycle, 5)
                {
                    Layout = DataCubeLayout.TwoD
                };
                for (int i = 0; i < NumCycle; i++)
                {
                    line = sr.ReadLine();
                    buf = TypeConverterEx.Split<int>(line, 5);
                    for (int j = 0; j < 5; j++)
                    {
                        StressPeriod[0, i, j] = buf[j];
                    }
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
                sr.Close();
                OnLoaded(progresshandler);
                return true;
            }
            else
            {
                OnLoadFailed("Failed to load " + this.Name, progresshandler);
                return false;
            }
        }

        public override bool SaveAs(string filename, ICancelProgressHandler progress)
        {
            var grid = Owner.Grid as MFGrid;
            StreamWriter sw = new StreamWriter(filename);
            string line = string.Format("#{0}: created on {1} by Visual HEIFLOW", PackageName, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            sw.WriteLine(line);
            line = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} # num_wra_sp num_wra_cycle num_cycle_len num_obj_type drawdown_constaint gw_compensate enable_sw_storage water_source_priority",
                NumStressPeriod, NumCycle, _num_cycle_len, NumObj, TypeConverterEx.Bool2Num(EnableDrawdownConstaint), TypeConverterEx.Bool2Num(EnableGWCompensated),
                TypeConverterEx.Bool2Num(EnableSWStorage), SourcePriority);
            sw.WriteLine(line);
            for (int i = 0; i < NumCycle; i++)
            {
                line = string.Format("{0} {1} {2} {3} {4}", StressPeriod[0, i, 0], StressPeriod[0, i, 1], StressPeriod[0, i, 2], StressPeriod[0, i, 3], StressPeriod[0, i, 4]);
                sw.WriteLine(line);
            }
            line = "# WRA stress period file";
            for (int i = 0; i < NumStressPeriod; i++)
            {
                line = StressPeriodFiles[i];
                sw.WriteLine(line);
            }
            sw.WriteLine("# Summary Report File");
            sw.WriteLine("1");
            sw.WriteLine(SummaryReportFile);
            sw.WriteLine("# Mangament Unit Report File");
            sw.WriteLine("1");
            sw.WriteLine(MangamentUnitReportFile);
            sw.WriteLine("# Budgets Report File");
            sw.WriteLine("1");
            sw.WriteLine(BudgetsReportFile);
            sw.WriteLine("# Pump Report File");
            sw.WriteLine("1");
            sw.WriteLine(PumpReportFile);

            sw.WriteLine("# hru_row_id matrix");
            for (int i = 0; i < grid.ActiveCellCount; i++)
            {
                line = (i + 1) + " " + (grid.Topology.ActiveCell[i][0] + 1) + " " + (grid.Topology.ActiveCell[i][1] + 1);
                sw.WriteLine(line);
            }
            sw.Close();
            return true;
        }

        public override void Clear()
        {
            StressPeriodFiles.Clear();
            base.Clear();
        }
    }
}
