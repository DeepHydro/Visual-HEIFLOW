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

using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Foundation;

namespace Heiflow.Models.Running
{
    public enum BudgetItemUnit { BasinAveraged, Volume}
    public abstract class FileMonitor : Model, Heiflow.Models.Running.IFileMonitor
    {
        protected string _FileName;
        protected List<IMonitorItem> _Roots;
        protected IArrayWatcher _Watcher;
        protected ListTimeSeries<double> _DataSource;
        protected int _CurrentStep;
        protected int _StartStep_Budget = 1;
        protected int _EndStep_Budget = 0;
        protected int _Inteval_Budget = 365;
        protected int _DecimalDigit = 2;

        public const string _In_Group = "In Terms";
        public const string _Out_Group = "Out Terms";
        public const string _Cu_In_Group = "Cumulative In Terms";
        public const string _Cu_Out_Group = "Cumulative Out Terms";
        public const string _Ds_Group = "Storage Change";
        public const string _Total_Group = "Total";
        public const string _Storage_Group = "Storage";

        //Total Budgets Items
        public static string PPT = "Precipitation";
        public static string Streams_Inflow = "Streams Inflow";
        public static string Streams_Outflow = "Streams Outflow";
        public static string Groundwater_Inflow = "Groundwater Inflow";
        public static string Groundwater_Outflow = "Groundwater Outflow";
        public static string Wells_In = "Wells In";
        public static string Wells_Out = "Wells Out";
        public static string Lakes_Inflow = "Lakes Inflow";
        public static string Lakes_Outflow = "Lakes Outflow";
        public static string Evapotranspiration = "Evapotranspiration";
        public static string Evaporation = "Evaporation";
        public static string Surface_Zone_DS = "Land Surface Zone";
        public static string Soil_Zone_DS = "Soil Zone";
        public static string Unsaturated_Zone_DS = "Unsaturated Zone";
        public static string Saturated_Zone_DS = "Saturated Zone";
        public static string Lakes_Zone_DS = "Lakes";
        public static string Canals_Zone_DS = "Canals";
        public static string SFR_DS = "Streams";
        public static string Total_Storage_Change = "Total Storage Change";


        public static string CONSTANT_HEAD_IN = "CONSTANT HEAD IN";
        public static string WELLS_IN = "WELLS IN";
        public static string SPECIFIED_FLOWS_IN = "SPECIFIED FLOWS IN";
        public static string UZF_RECHARGE_IN = "UZF RECHARGE IN";
        public static string GW_ET_IN = "GW ET IN";
        public static string SURFACE_LEAKAGE_IN = "SURFACE LEAKAGE IN";
        public static string STREAM_LEAKAGE_IN = "STREAM LEAKAGE IN";
        public static string LAKE_SEEPAGE_IN = "LAKE SEEPAGE IN";
        public static string STORAGE_IN = "STORAGE IN";
        public static string HEAD_DEP_BOUNDS_IN = "HEAD DEP BOUNDS IN";

        public static string CONSTANT_HEAD_OUT = "CONSTANT HEAD OUT";
        public static string WELLS_OUT = "WELLS OUT";
        public static string SPECIFIED_FLOWS_OUT = "SPECIFIED FLOWS OUT";
        public static string UZF_RECHARGE_OUT = "UZF RECHARGE OUT";
        public static string GW_ET_OUT = "GW ET OUT";
        public static string SURFACE_LEAKAGE_OUT = "SURFACE LEAKAGE OUT";
        public static string STREAM_LEAKAGE_OUT = "STREAM LEAKAGE OUT";
        public static string LAKE_SEEPAGE_OUT = "LAKE SEEPAGE OUT";
        public static string STORAGE_OUT = "STORAGE OUT";
        public static string HEAD_DEP_BOUNDS_OUT = "HEAD DEP BOUNDS OUT";

        public static string SAT_IN = "Total SAT IN";
        public static string SAT_OUT = "Total SAT OUT";
        public static string SAT_DS_CUM = "Cumulative Storage Change";
        public static string SAT_ERROR = "Total SAT ERROR";
        public static string SAT_PERD_Step = "Step Percent Discrepancy";
        public static string SAT_PERD_CUM = "Cumulative Percent Discrepancy";

        public static string LAKET = "Lakes ET";
        public static string LAK_PPT = "Lakes Precipitation";
        public static string LAK_INFLOW = "Lakes Inflow";
        public static string LAK_Outflow = "Lakes Outflow";
        public static string LAK_Runoff = "Lakes Runoff";
        public static string LAK_Storage = "Lakes Storage";
        public static string LAK_Storage_Change = "Total Lakes Storage Change";
        public static string LAK_In = "Total Lakes In";
        public static string LAK_Out = "Total Lakes Out";
        public static string LAK_Error = "Total Lakes Budget Error";
        public static string LAK_Gaining = "Lakes Gaining";
        public static string LAK_Losing = "Lakes Losing";
        public static string LAK_Uzf_Infil = "Lakes UZF Infiltration";
        public static string LAK_Water_Use = "Lakes Water Use";

        public static string SFR_PPT = "Stream Precipitation";
        public static string SFR_INFLOW = "Stream Inflow";
        public static string SFRET = "Stream ET";
        public static string SFR_Storage = "Stream Storage";
        public static string SFR_Storage_Change = "Total Stream Storage Change";
        public static string SFR_In = "Total Stream In";
        public static string SFR_Out = "Total Stream Out";
        public static string SFR_Outflow = "Stream Outflow";
        public static string SFR_Error = "Total Stream Budget Error";
        public static string SFR_Gaining = "Stream Gaining";
        public static string SFR_Losing = "Stream Losing";

        public static string CANAL_ET = "Canals ET";
        public static string Canal_Drainage = "Canals Drainage";
        public static string Canal_Storage = "Canals Storage";
        public static string Canal_DS = "Total Canals Storage Change";

        public static string IR_PUMP = "Pumping";
        public static string IR_DIV = "Diversion";
        public static string IR_Industry = "Industry";
        public static string IR_CANAL_ET = "Irrigation Canals ET";

        public static string BasinLakePrecip = "Lake Precipitation";
        public static string BASINGW2SZ_HRU = "Groundwater Discharge from SAT to Soil Zone";
        public static string BASINSZREJECT = "Rejected  Gravity Drainage by UZ/SAT";

        public static string BASINPERVET_HRU = "Pervious Areas ET";
        public static string BASINIMPERVEVAP_HRU = "Impervious Areas ET";
        public static string BASININTCPEVAP_HRU = "Intercepted Precipitation ET";
        public static string BASINSNOWEVAP_HRU = "Snowpack Sublimation";

        public static string BASININTERFLOW = "Slow interflow and prefer flow to streams";
        /// <summary>
        ///  Hortonian and Dunnian surface runoff to streams
        /// </summary>
        public static string BASINSROFF = "Hortonian and Dunnian surface runoff to streams";
        /// <summary>
        /// Dunnian runoff and interflow to lakes
        /// </summary>
        public static string BASINLAKEINSZ = "Dunnian runoff and interflow to lakes";
        public static string BASINHORTONIANLAKES = "Hortonian runoff to lakes";
        public static string Daily_PPT = "Daily Precipitation";
        public static string Percolation = "Percolation";
        public static string BASINSZ2GW = "Potential gravity drainage from the soil zone to UZ";
        public static string HRU_IN = "Total HRU In";
        public static string HRU_OUT = "Total HRU Out";
        public static string HRU_STORAGE = "Total HRU Storage";
        public static string HRU_DS = "Total HRU Storage Change";
        public static string HRU_ERROR = "Total HRU Budget Error";
        public static string HRU_DISYP = "Total HRU Budget Percent Discrepancy";

        public static string Soil_In = "Total Soil In";
        public static string Soil_Out = "Total Soil Out";
        public static string Soil_Storage_Change = "Soil Zone Storage Change";
        public static string Soil_Out_Eorror = "Total Soil Budget Error";
        public static string Soil_ET = "Soil Zone ET";
        public static string Dunnian_runoff_to_streams = "Dunnian runoff to streams";
        public static string Hortonian_runoff_to_streams = "Hortonian runoff to streams";
        public static string Total_Soil_Zone_Storage = "Total Soil Zone Storage";
        public static string Soil_infiltration = "Soil infiltration";

        public static string UZF_INFIL = "Infiltration to UZ and SZ zones";
        public static string UZF_ET = "ET from UZ zone";
        public static string UZF_RECHARGE = "Recharge from UZ to SZ";
        public static string UZF_IN = "Total UZ In";
        public static string UZF_OUT = "Total UZ Out";
        public static string UZF_DS = "Total UZ Storage Change";
        public static string UZF_ERROR = "Total UZ Budget Error";
        public static string UZF_DISPY = "Total UZ Budget Percent Discrepancy";

        public static string SAT_ET = "Saturated ET";
        protected bool _ConvertToStrepRate = false;

        /// <summary>
        /// Total SAT Discrepancy in 100%
        /// </summary>
        public static string SAT_DISPY = "Total SAT Percent Discrepancy";
        public static string SAT_DS = "Total SAT DS";
        public static string TOTAL_DISPY = "Total Percent Discrepancy";

        public FileMonitor()
        {
            MonitorName = "FileMonitor";
            _Roots = new List<IMonitorItem>();
            Partners = new List<IFileMonitor>();
            _CurrentStep = 0;
            StartStep = 1;
            BudgetItemUnit = Running.BudgetItemUnit.BasinAveraged;
        }
        [Category("Design")]
        public string MonitorName { get; protected set; }
        [Category("Data")]
        public virtual string FileName
        {
            get
            {
                if (TypeConverterEx.IsNull(_FileName))
                    return _FileName;
                else
                    return Path.Combine(ModelService.WorkDirectory, _FileName);
            }
            set
            {
                SetProperty(ref _FileName, value);
                if (_Watcher != null)
                    _Watcher.FileName = _FileName;
            }
        }
        [Browsable(false)]
        public List<IMonitorItem> Root
        {
            get
            {
                return _Roots;
            }
            set
            {
                _Roots = value;
            }
        }
        [Browsable(false)]
        public IArrayWatcher Watcher
        {
            get
            {
                return _Watcher;
            }
            set
            {
                _Watcher = value;
            }
        }
        [Browsable(false)]
        public ListTimeSeries<double> DataSource
        {
            get
            {
                return Watcher.DataSource;
            }
        }
        [Browsable(false)]
        public int CurrentStep
        {
            get;
            set;
        }
        /// <summary>
        /// starting from 1
        /// </summary>
        [Category("Analysis")]
        public int StartStep
        {
            get
            {
                return _StartStep_Budget;
            }
            set
            {
                if (value < 1)
                    value = 1;
                _StartStep_Budget = value;
            }
        }
        /// <summary>
        /// starting from 1
        /// </summary>
        [Category("Analysis")]
        public int EndStep
        {
            get
            {
                return _EndStep_Budget;
            }
            set
            {
                _EndStep_Budget = value;
            }
        }
        [Category("Analysis")]
        public int Intevals
        {
            get
            {
                return _Inteval_Budget;
            }
            set
            {
                if (value < 1)
                    value = 1;
                _Inteval_Budget = value;
            }
        }
        [Browsable(false)]
        public bool IsStarted
        {
            get;
            protected set;
        }
        [Category("Analysis")]
        public int DecimalDigit
        {
            get
            {
                return _DecimalDigit;
            }
            set
            {
                if (value <= 0)
                    value = 1;
                _DecimalDigit = value;
            }
        }

        [Browsable(false)]
        public List<IFileMonitor> Partners
        {
            get;
            protected set;
        }
        [Browsable(false)]
        [Category("Analysis")]
        public bool ConvertToStrepRate
        {
            get
            {
                return _ConvertToStrepRate;
            }
            set
            {
                _ConvertToStrepRate = value;
                OnConvertToStrepRateChanged();
            }
        }
        [Category("Analysis")]
        [Browsable(false)]
        public bool[] VarIndexIsConvert
        {
            get;
            set;
        }
        [Category("Analysis")]
        [Browsable(true)]
        public BudgetItemUnit BudgetItemUnit
        {
            get;
            set;
        }

        public virtual void Clear()
        {
            _CurrentStep = 0;
            _Watcher.Clear();
        }


        public virtual void Start()
        {
            _Watcher.Start();
            IsStarted = true;
        }

        public virtual void Stop()
        {
            _Watcher.Stop();
            _CurrentStep = 0;
            IsStarted = false;
        }

        /// <summary>
        /// Create balance table
        /// </summary>
        /// <param name="report">balance report</param>
        /// <returns></returns>
        public virtual System.Data.DataTable Balance(string itemname, ref string budget)
        {
            return null;
        }

        #region 预算报告通用方法

        /// <summary>
        /// 将起止步校正到数据源长度范围内
        /// </summary>
        protected void ClampSteps(int length)
        {
            if (EndStep <= 0)
                EndStep = length;

            if (EndStep > length)
                EndStep = length;

            if (StartStep > length)
                StartStep = length;

            if (StartStep >= EndStep)
                StartStep = 0;
        }

        /// <summary>
        /// 创建预算数据表：ID、ParentID、Item、Volumetric_Flow、Water_Depth
        /// </summary>
        protected System.Data.DataTable CreateBudgetTable()
        {
            var dt = new System.Data.DataTable();
            dt.Columns.Add(new System.Data.DataColumn("ID", Type.GetType("System.Int32")));
            dt.Columns.Add(new System.Data.DataColumn("ParentID", Type.GetType("System.Int32")));
            dt.Columns.Add(new System.Data.DataColumn("Item", Type.GetType("System.String")));
            dt.Columns.Add(new System.Data.DataColumn("Volumetric_Flow", Type.GetType("System.Double")));
            dt.Columns.Add(new System.Data.DataColumn("Water_Depth", Type.GetType("System.Double")));
            return dt;
        }

        /// <summary>
        /// 向预算表追加一行，depth 为空时该列写入 DBNull
        /// </summary>
        protected System.Data.DataRow AddBudgetRow(System.Data.DataTable dt, int id, int parentId, string name, double flow, double? depth = null)
        {
            var dr = dt.NewRow();
            dr[0] = id;
            dr[1] = parentId;
            dr[2] = name;
            dr[3] = flow;
            dr[4] = depth.HasValue ? (object)depth.Value : DBNull.Value;
            dt.Rows.Add(dr);
            return dr;
        }

        /// <summary>
        /// 汇总分项：按 selector 取序列求和后写入数据表、累计通量并收集报告行
        /// </summary>
        protected double AddTermRows(IEnumerable<MonitorItem> items, System.Data.DataTable dt, int parentId, double factor,
            List<BudgetLine> lines, Func<MonitorItem, IEnumerable<double>> selector = null)
        {
            if (selector == null)
                selector = item => item.Monitor.DataSource.Values[item.VariableIndex].Skip<double>(StartStep);

            double total = 0;
            foreach (var item in items)
            {
                var flow = Math.Round(selector(item).Sum() * factor, DecimalDigit);
                var depth = ToDepth(flow);

                AddBudgetRow(dt, item.VariableIndex, parentId, item.Name, flow, depth);

                total += flow;
                lines.Add(BudgetLine.Item(item.Name, DExp(flow), Fix(depth)));
            }
            return total;
        }

        /// <summary>
        /// 体积通量换算为水深
        /// </summary>
        protected double ToDepth(double flow)
        {
            return Math.Round(flow / ModelService.BasinArea * 1000, DecimalDigit);
        }

        /// <summary>
        /// 百分比差异
        /// </summary>
        protected double PercentDiscrepancy(double total_in, double total_out, double total_ds)
        {
            var denom = total_in + total_out + Math.Abs(total_ds);
            if (denom == 0)
                return 0;
            return Math.Round((total_in - total_out - total_ds) / denom * 2 * 100, DecimalDigit);
        }

        /// <summary>
        /// 百分比差异
        /// </summary>
        protected double PercentDiscrepancyModflow(double total_in, double total_out)
        {
            var denom = (total_in + total_out)/2 ;
            if (denom == 0)
                return 0;
            return Math.Round((total_in - total_out ) / denom * 100, DecimalDigit);
        }

        /// <summary>
        /// 追加汇总与误差行，包括数据表行与报告行
        /// </summary>
        protected void AddSummaryRows(System.Data.DataTable dt, List<BudgetLine> lines, double total_in, double total_out,
            double total_ds, double total_diff, double total_error, double total_discrepancy)
        {
            AddBudgetRow(dt, 100, 9999, "Total In", total_in, ToDepth(total_in));
            AddBudgetRow(dt, 200, 9999, "Total Out", total_out, ToDepth(total_out));
            AddBudgetRow(dt, 300, 9999, Total_Storage_Change, total_ds, ToDepth(total_ds));

            AddBudgetRow(dt, 400, 9999, "Budget Error", total_discrepancy);
            AddBudgetRow(dt, 401, 400, "Inflows - Outflows", total_diff, ToDepth(total_diff));
            AddBudgetRow(dt, 402, 400, "Overall Budget Error", total_error, ToDepth(total_error));
            AddBudgetRow(dt, 403, 400, "Percent Discrepancy", total_discrepancy);

            lines.Add(BudgetLine.Section("BUDGET SUMMARY"));
            lines.Add(BudgetLine.Item("Total In", DExp(total_in), Fix(ToDepth(total_in))));
            lines.Add(BudgetLine.Item("Total Out", DExp(total_out), Fix(ToDepth(total_out))));
            lines.Add(BudgetLine.Item(Total_Storage_Change, DExp(total_ds), Fix(ToDepth(total_ds))));

            lines.Add(BudgetLine.Section("BUDGET ERROR"));
            lines.Add(BudgetLine.Item("Inflows - Outflows", DExp(total_diff), Fix(ToDepth(total_diff))));
            lines.Add(BudgetLine.Item("Overall Budget Error", DExp(total_error), Fix(ToDepth(total_error))));
            lines.Add(BudgetLine.Item("Percent Discrepancy (%)", Fix(total_discrepancy), ""));
        }

        /// <summary>
        /// 以 Fortran 风格的 D 指数格式输出数值，例如 1.2558D+10；零值输出为 0
        /// </summary>
        protected string DExp(double value)
        {
            if (value == 0)
                return "0";
            if (double.IsNaN(value) || double.IsInfinity(value))
                return value.ToString(CultureInfo.InvariantCulture);

            var text = value.ToString("E4", CultureInfo.InvariantCulture);
            int epos = text.IndexOf('E');
            if (epos < 0)
                return text;

            var mantissa = text.Substring(0, epos);
            var sign = text[epos + 1];
            var exponent = text.Substring(epos + 2).TrimStart('0');
            if (exponent.Length == 0)
                exponent = "0";
            else if (exponent.Length < 2)
                exponent = exponent.PadLeft(2, '0');

            return mantissa + "D" + sign + exponent;
        }

        /// <summary>
        /// 以固定小数位输出数值，用于水深与百分比等无量纲量
        /// </summary>
        protected string Fix(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return value.ToString(CultureInfo.InvariantCulture);
            return value.ToString("N" + DecimalDigit.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 生成清单式预算报告：标签右对齐 + " = " + 体积通量右对齐 + 水深右对齐，段落以分隔线区分
        /// </summary>
        protected string BuildReport(string title, IEnumerable<BudgetLine> lines)
        {
            const int MinNameWidth = 30;
            const int MinValueWidth = 18;
            const int ColumnGap = 4;
            const string FlowHeader = "VOLUMETRIC FLOW";
            const string DepthHeader = "WATER DEPTH";

            var rows = new List<BudgetLine>(lines);

            int nameWidth = MinNameWidth;
            int valueWidth = MinValueWidth;
            foreach (var line in rows)
            {
                if (line.IsSection)
                    continue;
                nameWidth = Math.Max(nameWidth, line.Name.Length + 2);
                valueWidth = Math.Max(valueWidth, line.Flow.Length);
                valueWidth = Math.Max(valueWidth, line.Depth.Length);
            }
            valueWidth = Math.Max(valueWidth, FlowHeader.Length);

            int totalWidth = nameWidth + 3 + valueWidth + ColumnGap + valueWidth;
            string thickBar = new string('=', totalWidth);
            string thinBar = new string('-', totalWidth);
            string gap = new string(' ', ColumnGap);

            var sb = new StringBuilder();
            sb.AppendLine(thickBar);
            sb.AppendLine(Center("SUMMARY VOLUMETRIC BUDGET", totalWidth));
            if (!string.IsNullOrWhiteSpace(title))
                sb.AppendLine(Center(title, totalWidth));
            sb.AppendLine(thickBar);
            sb.AppendLine();

            sb.Append(new string(' ', nameWidth));
            sb.Append("   ");
            sb.Append(FlowHeader.PadLeft(valueWidth));
            sb.Append(gap);
            sb.Append(DepthHeader.PadLeft(valueWidth));
            sb.AppendLine();
            sb.AppendLine(thinBar);

            bool firstSection = true;
            foreach (var line in rows)
            {
                if (line.IsSection)
                {
                    if (firstSection)
                        firstSection = false;
                    else
                        sb.AppendLine();
                    sb.AppendLine(line.Name);
                    sb.AppendLine(thinBar);
                }
                else
                {
                    sb.Append(line.Name.PadLeft(nameWidth));
                    sb.Append(" = ");
                    sb.Append(line.Flow.PadLeft(valueWidth));
                    sb.Append(gap);
                    sb.Append(line.Depth.PadLeft(valueWidth));
                    sb.AppendLine();
                }
            }

            sb.AppendLine(thinBar);
            return sb.ToString();
        }

        protected static string Center(string text, int width)
        {
            if (string.IsNullOrEmpty(text) || text.Length >= width)
                return text;
            int left = (width - text.Length) / 2;
            return text.PadLeft(text.Length + left, ' ').PadRight(width, ' ');
        }

        /// <summary>
        /// 报告中的一行：段落标题或数据项
        /// </summary>
        protected class BudgetLine
        {
            public bool IsSection { get; private set; }
            public string Name { get; private set; }
            public string Flow { get; private set; }
            public string Depth { get; private set; }

            private BudgetLine(bool isSection, string name, string flow, string depth)
            {
                IsSection = isSection;
                Name = name;
                Flow = flow;
                Depth = depth;
            }

            public static BudgetLine Section(string name)
            {
                return new BudgetLine(true, name.ToUpperInvariant(), "", "");
            }

            public static BudgetLine Item(string name, string flow, string depth)
            {
                return new BudgetLine(false, name.ToUpperInvariant(), flow, depth);
            }
        }

        #endregion

        public MonitorItem Select(string name)
        {
            MonitorItem selected = null;
            foreach (var root in this.Root)
            {
                foreach (var item in root.Children)
                {
                    if (item.Name == name)
                    {
                        selected = item;
                        break;
                    }
                }
            }
            if (selected == null)
            {
                foreach (var par in this.Partners)
                {
                    foreach (var root in par.Root)
                    {
                        foreach (var item in root.Children)
                        {
                            if (item.Name == name)
                            {
                                selected = item;
                                break;
                            }
                        }
                    }
                }
            }
            return selected;
        }


        public virtual Dictionary<string, double> ZonalBudgets()
        {
            return new Dictionary<string, double>();
        }

        protected virtual void OnConvertToStrepRateChanged()
        {

        }
    }
}