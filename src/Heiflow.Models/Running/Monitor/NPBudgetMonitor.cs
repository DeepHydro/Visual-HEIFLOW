//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Running
{
    /// <summary>
    /// N / P 质量平衡分析监视器。
    /// 输入文件为工作目录下的 sz_N_budget.csv 与 sz_P_budget.csv。
    /// </summary>
    /// <remarks>
    /// 列标题约定：
    /// 1. 带 (in) 的行为输入项；
    /// 2. 带 (out) 的行为输出项；
    /// 3. wb 为模型给出的误差（闭合差）项；
    /// 4. sediment 与 harvest 分别为泥沙输移损失项与收割移除项，属于移除输出，不参与质量平衡；
    /// 5. 带 River 的行为河流项，属于河流子系统，不参与包气带质量平衡；
    /// 6. storage(begin) 与 storage(end) 为当前时间步的起始与截止储量，
    ///    储量变化 = storage(begin) - storage(end)，需额外计算；
    /// 7. Date 为日期项。
    /// 注：N 与 P 的输出列并不相同。P 缺少 fix(in)（生物固氮）、volat(out)（挥发）、
    /// denitri(out)（反硝化）与 River_denitri（河流反硝化）四项，
    /// 因此两类元素分别使用各自的列定义（sz_N_budget.csv 29 列，sz_P_budget.csv 25 列）。
    /// </remarks>
    public class NPBudgetMonitor : FileMonitor
    {
        #region 分组名称

        /// <summary>
        /// 泥沙与收割（移除）项
        /// </summary>
        public const string _Removal_Group = "Sediment and Harvest Terms";

        /// <summary>
        /// 误差项
        /// </summary>
        public const string _WB_Group = "Budget Error";

        /// <summary>
        /// 河流项
        /// </summary>
        public const string _River_Group = "Riverine Terms";

        #endregion

        #region 收支项名称

        public const string STORAGE_BEGIN = "Storage (Begin)";
        public const string STORAGE_END = "Storage (End)";
        public const string STORAGE_CHANGE = "Storage Change";
        public const string WB_ERROR = "Budget Error (wb)";
        public const string SEDIMENT = "Sediment Loss";
        public const string HARVEST = "Harvest Removal";
        public const string TOTAL_IN = "Total In";
        public const string TOTAL_OUT = "Total Out";
        public const string RIVER_TOTAL = "Total Riverine Flux";

        public const string RAIN = "Rainfall Deposition";
        public const string DRY_DEPOSITION = "Dry Deposition";
        public const string GW_2_SZ = "Groundwater Discharge to Soil Zone";
        public const string IMPERVIOUS = "Impervious Area Contribution";
        public const string IRRIGATION = "Irrigation Input";
        public const string RESIDUE = "Crop Residue Input";
        public const string FERTILIZER = "Fertilizer Input";
        public const string FIXATION = "Biological Fixation";

        public const string SZ_2_GW = "Leaching to Groundwater";
        public const string SEGMENT = "Loss to Stream Segment";
        public const string FARFIELD = "Far-field Lateral Loss";
        public const string PLANT_UPTAKE = "Plant Uptake";
        public const string VOLATILIZATION = "Volatilization";
        public const string DENITRIFICATION = "Denitrification";

        public const string RIVER_RAINFALL = "River Rainfall";
        public const string RIVER_GW = "River Groundwater Inflow";
        public const string RIVER_NPS = "River Non-point Source";
        public const string RIVER_PS = "River Point Source";
        public const string RIVER_SEDIMENT = "River Sediment";
        public const string RIVER_PHYT = "River Phytoplankton";
        public const string RIVER_DENITRI = "River Denitrification";
        public const string RIVER_OUTLET = "River Outlet Export";
        public const string RIVER_STORAGE = "River Storage";
        public const string RIVER_DIV = "River Diversion";

        public const string INFLOWS_MINUS_OUTFLOWS = "Inflows - Outflows";
        public const string OVERALL_BUDGET_ERROR = "Overall Budget Error";
        public const string PERCENT_DISCREPANCY = "Percent Discrepancy";
        public const string MODEL_REPORTED_ERROR = "Model Reported Error (wb)";

        #endregion

        #region 列定义

        private const string FileNameFormat = "sz_{0}_budget.csv";

        private const string CatIn = "In";
        private const string CatOut = "Out";
        private const string CatRemoval = "Removal";
        private const string CatStorage = "Storage";
        private const string CatError = "Error";
        private const string CatRiver = "River";

        /// <summary>
        /// 养分元素标识：氮，对应 sz_N_budget.csv
        /// </summary>
        public const string Element_N = "N";

        /// <summary>
        /// 养分元素标识：磷，对应 sz_P_budget.csv
        /// </summary>
        public const string Element_P = "P";

        /// <summary>
        /// 与 sz_N_budget.csv 列标题一一对应的列定义（29 列），
        /// 数组下标即是默认的 VariableIndex。
        /// </summary>
        private static readonly BudgetColumn[] _NColumns = new BudgetColumn[]
        {
            new BudgetColumn("storage(begin)",   STORAGE_BEGIN,    _Storage_Group,  CatStorage),
            new BudgetColumn("rain(in)",         RAIN,             _In_Group,       CatIn),
            new BudgetColumn("drydeposition(in)",DRY_DEPOSITION,   _In_Group,       CatIn),
            new BudgetColumn("gw2sz(in)",        GW_2_SZ,          _In_Group,       CatIn),
            new BudgetColumn("imper(in)",        IMPERVIOUS,       _In_Group,       CatIn),
            new BudgetColumn("irrigation(in)",   IRRIGATION,       _In_Group,       CatIn),
            new BudgetColumn("residue(in)",      RESIDUE,          _In_Group,       CatIn),
            new BudgetColumn("fertilizer(in)",   FERTILIZER,       _In_Group,       CatIn),
            new BudgetColumn("fix(in)",          FIXATION,         _In_Group,       CatIn),
            new BudgetColumn("sz2gw(out)",       SZ_2_GW,          _Out_Group,      CatOut),
            new BudgetColumn("segment(out)",     SEGMENT,          _Out_Group,      CatOut),
            new BudgetColumn("farfield(out)",    FARFIELD,         _Out_Group,      CatOut),
            new BudgetColumn("plantuptake(out)", PLANT_UPTAKE,     _Out_Group,      CatOut),
            new BudgetColumn("volat(out)",       VOLATILIZATION,   _Out_Group,      CatOut),
            new BudgetColumn("denitri(out)",     DENITRIFICATION,  _Out_Group,      CatOut),
            new BudgetColumn("storage(end)",     STORAGE_END,      _Storage_Group,  CatStorage),
            new BudgetColumn("wb",               WB_ERROR,         _WB_Group,       CatError),
            new BudgetColumn("sediment",         SEDIMENT,         _Removal_Group,  CatRemoval),
            new BudgetColumn("harvest",          HARVEST,          _Removal_Group,  CatRemoval),
            new BudgetColumn("River_rainfall",   RIVER_RAINFALL,   _River_Group,    CatRiver),
            new BudgetColumn("River_gw",         RIVER_GW,         _River_Group,    CatRiver),
            new BudgetColumn("River_nps",        RIVER_NPS,        _River_Group,    CatRiver),
            new BudgetColumn("River_ps",         RIVER_PS,         _River_Group,    CatRiver),
            new BudgetColumn("River_sediment",   RIVER_SEDIMENT,   _River_Group,    CatRiver),
            new BudgetColumn("River_PHYT",       RIVER_PHYT,       _River_Group,    CatRiver),
            new BudgetColumn("River_denitri",    RIVER_DENITRI,    _River_Group,    CatRiver),
            new BudgetColumn("River_outlet",     RIVER_OUTLET,     _River_Group,    CatRiver),
            new BudgetColumn("River_storage",    RIVER_STORAGE,    _River_Group,    CatRiver),
            new BudgetColumn("River_div",        RIVER_DIV,        _River_Group,    CatRiver),
        };

        /// <summary>
        /// 与 sz_P_budget.csv 列标题一一对应的列定义（25 列）。
        /// 相较于 N，P 不含生物固氮 fix(in)、挥发 volat(out)、反硝化 denitri(out)
        /// 以及河流反硝化 River_denitri 四项。
        /// </summary>
        private static readonly BudgetColumn[] _PColumns = new BudgetColumn[]
        {
            new BudgetColumn("Storage(begin)",   STORAGE_BEGIN,    _Storage_Group,  CatStorage),
            new BudgetColumn("rain(in)",         RAIN,             _In_Group,       CatIn),
            new BudgetColumn("drydeposition(in)",DRY_DEPOSITION,   _In_Group,       CatIn),
            new BudgetColumn("gw2sz(in)",        GW_2_SZ,          _In_Group,       CatIn),
            new BudgetColumn("imper(in)",        IMPERVIOUS,       _In_Group,       CatIn),
            new BudgetColumn("irrigation(in)",   IRRIGATION,       _In_Group,       CatIn),
            new BudgetColumn("residue(in)",      RESIDUE,          _In_Group,       CatIn),
            new BudgetColumn("fertilizer(in)",   FERTILIZER,       _In_Group,       CatIn),
            new BudgetColumn("sz2gw(out)",       SZ_2_GW,          _Out_Group,      CatOut),
            new BudgetColumn("segment(out)",     SEGMENT,          _Out_Group,      CatOut),
            new BudgetColumn("farfield(out)",    FARFIELD,         _Out_Group,      CatOut),
            new BudgetColumn("plantuptake(out)", PLANT_UPTAKE,     _Out_Group,      CatOut),
            new BudgetColumn("Storage(end)",     STORAGE_END,      _Storage_Group,  CatStorage),
            new BudgetColumn("wb",               WB_ERROR,         _WB_Group,       CatError),
            new BudgetColumn("sediment",         SEDIMENT,         _Removal_Group,  CatRemoval),
            new BudgetColumn("harvest",          HARVEST,          _Removal_Group,  CatRemoval),
            new BudgetColumn("River_rainfall",   RIVER_RAINFALL,   _River_Group,    CatRiver),
            new BudgetColumn("River_gw",         RIVER_GW,         _River_Group,    CatRiver),
            new BudgetColumn("River_nps",        RIVER_NPS,        _River_Group,    CatRiver),
            new BudgetColumn("River_ps",         RIVER_PS,         _River_Group,    CatRiver),
            new BudgetColumn("River_sediment",   RIVER_SEDIMENT,   _River_Group,    CatRiver),
            new BudgetColumn("River_PHYT",       RIVER_PHYT,       _River_Group,    CatRiver),
            new BudgetColumn("River_outlet",     RIVER_OUTLET,     _River_Group,    CatRiver),
            new BudgetColumn("River_storage",    RIVER_STORAGE,    _River_Group,    CatRiver),
            new BudgetColumn("River_div",        RIVER_DIV,        _River_Group,    CatRiver),
        };

        /// <summary>
        /// 当前元素所使用的列定义，由构造函数依据 Element 选定
        /// </summary>
        private readonly BudgetColumn[] _Columns;

        /// <summary>
        /// 原始列数量，派生列的 VariableIndex 从该值开始累加，以免与原始列冲突
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return _Columns.Length;
            }
        }

        #endregion

        private string _MassUnit = "kg";
        private Dictionary<string, double> _BudgetItems = new Dictionary<string, double>();
        private Dictionary<string, string[]> _DerivedHeaders = new Dictionary<string, string[]>();
        private List<MonitorItem> _DerivedItems = new List<MonitorItem>();

        public NPBudgetMonitor(string element)
        {
            Element = element;
            _Columns = IsPhosphorus(element) ? _PColumns : _NColumns;
            MonitorName = "NPBudgetMonitor_" + element;
            Intevals = 1;
            _Watcher = new NPBudgetWatcher();

            var root = new MonitorItemCollection(string.Format("{0} Mass Budget", element));
            _Roots.Add(root);

            foreach (var col in _Columns)
            {
                root.Children.Add(new MonitorItem(col.Name)
                {
                    VariableIndex = col.Index,
                    Group = col.Group
                });
            }

            // 派生项：储量变化 = storage(begin) - storage(end)
            var ds = new DerivedBudgetItem(STORAGE_CHANGE, StorageChangeSeries)
            {
                VariableIndex = ColumnCount,
                Group = _Ds_Group
            };
            root.Children.Add(ds);

            root.Children.Add(CreateDerivedItem(TOTAL_IN, _Total_Group, ColumnCount + 1, HeadersOf(CatIn)));
            // Total Out 与 Balance 口径一致：不含泥沙/收割项与河流项
            root.Children.Add(CreateDerivedItem(TOTAL_OUT, _Total_Group, ColumnCount + 2, HeadersOf(CatOut)));
            root.Children.Add(CreateDerivedItem(RIVER_TOTAL, _Total_Group, ColumnCount + 3, HeadersOf(CatRiver)));

            //FileName = Path.Combine(ModelService.WorkDirectory,  ".\\output\\"+ string.Format(FileNameFormat, element));

            // 储量项与误差项不做差分；其余分项若被识别为累积量则转换为步长值
            var indexMask = new bool[ColumnCount];
            for (int i = 0; i < ColumnCount; i++)
                indexMask[i] = true;
            foreach (var col in _Columns)
            {
                if (col.Category == CatStorage || col.Category == CatError)
                    indexMask[col.Index] = false;
            }
            VarIndexIsConvert = indexMask;

            foreach (var item in root.Children)
            {
                item.Monitor = this;
                item.SequenceType = SequenceType.StepbyStep;
            }
        }

        #region 属性

        /// <summary>
        /// 养分元素：N 或 P
        /// </summary>
        [Category("Design")]
        public string Element
        {
            get;
            protected set;
        }

        /// <summary>
        /// 质量单位，仅用于报表标题显示
        /// </summary>
        [Category("Analysis")]
        public string MassUnit
        {
            get
            {
                return _MassUnit;
            }
            set
            {
                _MassUnit = value;
            }
        }

        /// <summary>
        /// 为 true 时将选中步长区间内的累加量归一化到 Intevals 个时间步（年均负荷等）；
        /// 为 false（默认）时直接输出选中步长区间内的累加总量。
        /// </summary>
        [Category("Analysis")]
        [Browsable(true)]
        public bool NormalizeToInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 若 CSV 中的分项为累积量，勾选该项后会在读取时转换为步长值（相邻时间步差分）。
        /// 储量项与误差项不参与差分。
        /// </summary>
        [Category("Analysis")]
        [Browsable(true)]
        public bool ConvertCumulativeToStepRate
        {
            get
            {
                return ConvertToStrepRate;
            }
            set
            {
                ConvertToStrepRate = value;
            }
        }

        /// <summary>
        /// 最近一次 Balance 计算得到的分项汇总
        /// </summary>
        [Browsable(false)]
        public Dictionary<string, double> BudgetItems
        {
            get
            {
                return _BudgetItems;
            }
        }

        #endregion

        #region 列映射

        /// <summary>
        /// 读取完成后依据实际 CSV 列标题重新校正 VariableIndex，
        /// 使列的物理顺序发生变化时仍能正确取值。
        /// </summary>
        public void MapColumns()
        {
            var watcher = _Watcher as NPBudgetWatcher;
            if (watcher == null || watcher.Headers == null)
                return;

            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 1; i < watcher.Headers.Length; i++)
            {
                var key = watcher.Headers[i];
                if (TypeConverterEx.IsNull(key))
                    continue;
                if (!map.ContainsKey(key))
                    map.Add(key, i - 1);
            }

            foreach (var root in _Roots)
            {
                foreach (var item in root.Children)
                {
                    if (_DerivedItems.Contains(item))
                    {
                        RemapDerivedIndex(item, map);
                        continue;
                    }
                    var header = HeaderOf(item.Name);
                    if (header != null && map.ContainsKey(header))
                        item.VariableIndex = map[header];
                }
            }
        }

        private void RemapDerivedIndex(MonitorItem item, Dictionary<string, int> map)
        {
            string[] headers = null;
            if (!_DerivedHeaders.TryGetValue(item.Name, out headers))
                return;

            var buf = new List<int>();
            foreach (var header in headers)
            {
                if (map.ContainsKey(header))
                    buf.Add(map[header]);
            }
            if (buf.Count > 0)
                item.DerivedIndex = buf.ToArray();
        }

        private MonitorItem CreateDerivedItem(string name, string group, int index, string[] headers)
        {
            _DerivedHeaders[name] = headers;
            var item = new MonitorItem(name)
            {
                VariableIndex = index,
                Group = group,
                Derivable = true,
                DerivedIndex = IndexOf(headers)
            };
            _DerivedItems.Add(item);
            return item;
        }

        private string[] HeadersOf(string category)
        {
            return (from col in _Columns where col.Category == category select col.Header).ToArray();
        }

        private int[] IndexOf(string[] headers)
        {
            var buf = new List<int>();
            foreach (var header in headers)
            {
                for (int i = 0; i < _Columns.Length; i++)
                {
                    if (string.Equals(_Columns[i].Header, header, StringComparison.OrdinalIgnoreCase))
                    {
                        buf.Add(_Columns[i].Index);
                        break;
                    }
                }
            }
            return buf.ToArray();
        }

        private string HeaderOf(string itemName)
        {
            foreach (var col in _Columns)
            {
                if (col.Name == itemName)
                    return col.Header;
            }
            return null;
        }

        /// <summary>
        /// 判断元素是否为磷，用于选择 sz_P_budget.csv 的列定义
        /// </summary>
        private static bool IsPhosphorus(string element)
        {
            return string.Equals(element, Element_P, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 储量变化 = storage(begin) - storage(end)
        /// </summary>
        private double[] StorageChangeSeries(ListTimeSeries<double> source)
        {
            if (source == null)
                return null;

            var begin = Select(STORAGE_BEGIN);
            var end = Select(STORAGE_END);
            if (begin == null || end == null)
                return null;

            var values = new double[source.Dates.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = source.Values[begin.VariableIndex][i] - source.Values[end.VariableIndex][i];
            }
            return values;
        }

        #endregion

        #region 质量平衡

        /// <summary>
        /// 生成质量平衡数据表与文本报表
        /// </summary>
        /// <param name="itemname">报表标题</param>
        /// <param name="report">文本报表</param>
        /// <returns>带父子结构的数据表，可直接绑定到 DataTreeListView</returns>
        public override DataTable Balance(string itemname, ref string report)
        {
            if (DataSource == null || DataSource.Dates.Count == 0)
                return null;

            _BudgetItems.Clear();

            ClampSteps(DataSource.Dates.Count);

            int start = StartStep;
            if (start < 1)
                start = 1;
            int end = EndStep;
            if (end < start)
                end = start;
            if (end > DataSource.Dates.Count)
                end = DataSource.Dates.Count;

            int nsteps = end - start + 1;
            double factor = NormalizeToInterval ? (double)Intevals / nsteps : 1.0;

            var dt = CreateMassBudgetTable();
            var lines = new List<BudgetLine>();

            var root = _Roots[0];

            // 第一遍：计算各组合计。
            // 泥沙/收割（Removal）项与河流（Riverine）项不参与质量平衡计算。
            double total_in = SumOf(root, _In_Group, start, end, factor);
            double total_out = SumOf(root, _Out_Group, start, end, factor);
            double total_ds = SumOf(root, _Ds_Group, start, end, factor);
            double wb_total = SumOf(root, _WB_Group, start, end, factor);

            double total_diff = total_in - total_out;
            double total_error = total_in - total_out + total_ds;
            double percent_discrepancy = PercentDiscrepancyEx(total_in, total_out, total_ds);

            // 第二遍：按分组生成数据表行与报表行，百分比以总输入为参照
            lines.Add(BudgetLine.Section("IN TERMS"));
            AddRows(root, dt, lines, _In_Group, 100, start, end, factor, total_in);

            lines.Add(BudgetLine.Section("OUT TERMS"));
            AddRows(root, dt, lines, _Out_Group, 200, start, end, factor, total_in);

            lines.Add(BudgetLine.Section("STORAGE TERMS"));
            AddRows(root, dt, lines, _Storage_Group, 9999, start, end, factor, 0);
            AddRows(root, dt, lines, _Ds_Group, 300, start, end, factor, total_in);

            lines.Add(BudgetLine.Section("MODEL REPORTED ERROR"));
            AddRows(root, dt, lines, _WB_Group, 400, start, end, factor, total_in);

            AddSummaryMassRows(dt, lines, total_in, total_out, total_ds,
                total_diff, total_error, wb_total, percent_discrepancy);

            _BudgetItems[TOTAL_IN] = total_in;
            _BudgetItems[TOTAL_OUT] = total_out;
            _BudgetItems[FileMonitor.Total_Storage_Change] = total_ds;
            _BudgetItems[INFLOWS_MINUS_OUTFLOWS] = total_diff;
            _BudgetItems[OVERALL_BUDGET_ERROR] = total_error;
            _BudgetItems[WB_ERROR] = wb_total;
            _BudgetItems[PERCENT_DISCREPANCY] = percent_discrepancy;

            report = BuildMassReport(itemname, lines, start, end, factor);
            return dt;
        }

        public override Dictionary<string, double> ZonalBudgets()
        {
            return new Dictionary<string, double>(_BudgetItems);
        }

        private double AddRows(IMonitorItem root, DataTable dt, List<BudgetLine> lines,
            string group, int parentId, int start, int end, double factor, double reference)
        {
            foreach (var item in root.Children)
            {
                if (item.Group != group)
                    continue;
                double value = Aggregate(item, start, end, factor);
                AddMassRow(dt, item.VariableIndex, parentId, item.Name, value, reference);
                lines.Add(BudgetLine.Item(item.Name, DExp(value), Fix(PercentOf(value, reference))));
                _BudgetItems[item.Name] = value;
            }
            return SumOf(root, group, start, end, factor);
        }

        /// <summary>
        /// 指定分组在给定步长区间内的合计
        /// </summary>
        private double SumOf(IMonitorItem root, string group, int start, int end, double factor)
        {
            double total = 0;
            foreach (var item in root.Children)
            {
                if (item.Group != group)
                    continue;
                total += Aggregate(item, start, end, factor);
            }
            return Math.Round(total, DecimalDigit);
        }

        /// <summary>
        /// 指定步长区间内的累加值
        /// </summary>
        private double Aggregate(MonitorItem item, int start, int end, double factor)
        {
            double[] vector = null;
            if (item.Derivable)
                vector = item.Derive(DataSource);
            else if (item.VariableIndex >= 0 && item.VariableIndex < DataSource.Values.Length)
                vector = DataSource.Values[item.VariableIndex].ToArray();

            if (vector == null)
                return 0;

            double total = 0;
            for (int i = start - 1; i < end && i < vector.Length; i++)
            {
                total += vector[i];
            }
            return Math.Round(total * factor, DecimalDigit);
        }

        private double PercentOf(double value, double reference)
        {
            if (reference == 0)
                return 0;
            return Math.Round(Math.Abs(value) / Math.Abs(reference) * 100, DecimalDigit);
        }

        /// <summary>
        /// 百分比差异。储量变化按 storage(begin) - storage(end) 约定，
        /// 因此闭合残差为 In - Out + StorageChange。
        /// </summary>
        private double PercentDiscrepancyEx(double total_in, double total_out, double total_ds)
        {
            var denom = total_in + total_out + Math.Abs(total_ds);
            if (denom == 0)
                return 0;
            return Math.Round((total_in - total_out + total_ds) / denom * 2 * 100, DecimalDigit);
        }

        #endregion

        #region 报表与数据表

        private DataTable CreateMassBudgetTable()
        {
            var dt = new DataTable();
            dt.Columns.Add(new DataColumn("ID", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("ParentID", Type.GetType("System.Int32")));
            dt.Columns.Add(new DataColumn("Item", Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("Mass", Type.GetType("System.Double")));
            dt.Columns.Add(new DataColumn("Percentage", Type.GetType("System.Double")));
            return dt;
        }

        private void AddMassRow(DataTable dt, int id, int parentId, string name, double mass, double reference)
        {
            var dr = dt.NewRow();
            dr[0] = id;
            dr[1] = parentId;
            dr[2] = name;
            dr[3] = mass;
            dr[4] = PercentOf(mass, reference);
            dt.Rows.Add(dr);
        }

        private void AddSummaryMassRows(DataTable dt, List<BudgetLine> lines, double total_in, double total_out,
            double total_ds, double total_diff, double total_error, double wb_total, double percent_discrepancy)
        {
            AddMassRow(dt, 100, 9999, TOTAL_IN, total_in, total_in);
            AddMassRow(dt, 200, 9999, TOTAL_OUT, total_out, total_in);
            AddMassRow(dt, 300, 9999, FileMonitor.Total_Storage_Change, total_ds, total_in);

            AddMassRow(dt, 400, 9999, "Budget Error", percent_discrepancy, 0);
            AddMassRow(dt, 401, 400, INFLOWS_MINUS_OUTFLOWS, total_diff, total_in);
            AddMassRow(dt, 402, 400, OVERALL_BUDGET_ERROR, total_error, total_in);
            AddMassRow(dt, 404, 400, PERCENT_DISCREPANCY, percent_discrepancy, 0);

            lines.Add(BudgetLine.Section("MASS BUDGET SUMMARY"));
            lines.Add(BudgetLine.Item(TOTAL_IN, DExp(total_in), Fix(100.0)));
            lines.Add(BudgetLine.Item(TOTAL_OUT, DExp(total_out), Fix(PercentOf(total_out, total_in))));
            lines.Add(BudgetLine.Item(FileMonitor.Total_Storage_Change, DExp(total_ds), Fix(PercentOf(total_ds, total_in))));

            lines.Add(BudgetLine.Section("BUDGET ERROR"));
            lines.Add(BudgetLine.Item(INFLOWS_MINUS_OUTFLOWS, DExp(total_diff), Fix(PercentOf(total_diff, total_in))));
            lines.Add(BudgetLine.Item(OVERALL_BUDGET_ERROR, DExp(total_error), Fix(PercentOf(total_error, total_in))));
            lines.Add(BudgetLine.Item(MODEL_REPORTED_ERROR, DExp(wb_total), Fix(PercentOf(wb_total, total_in))));
            lines.Add(BudgetLine.Item(PERCENT_DISCREPANCY + " (%)", Fix(percent_discrepancy), ""));
        }

        private string BuildMassReport(string title, List<BudgetLine> lines, int start, int end, double factor)
        {
            const int MinNameWidth = 34;
            const int MinValueWidth = 18;
            const int ColumnGap = 4;
            const string MassHeader = "MASS";
            const string PercentHeader = "PERCENT OF TOTAL IN";

            int nameWidth = MinNameWidth;
            int valueWidth = MinValueWidth;
            foreach (var line in lines)
            {
                if (line.IsSection)
                    continue;
                nameWidth = Math.Max(nameWidth, line.Name.Length + 2);
                valueWidth = Math.Max(valueWidth, line.Flow.Length);
                valueWidth = Math.Max(valueWidth, line.Depth.Length);
            }
            valueWidth = Math.Max(valueWidth, MassHeader.Length + 6);
            valueWidth = Math.Max(valueWidth, PercentHeader.Length);

            int totalWidth = nameWidth + 3 + valueWidth + ColumnGap + valueWidth;
            string thickBar = new string('=', totalWidth);
            string thinBar = new string('-', totalWidth);
            string gap = new string(' ', ColumnGap);

            var sb = new StringBuilder();
            sb.AppendLine(thickBar);
            sb.AppendLine(Center("SUMMARY MASS BUDGET", totalWidth));
            if (NormalizeToInterval)
                sb.AppendLine(Center(string.Format("Unit: {0}    Normalized to {1} steps (step {2} - {3})",
                    _MassUnit, Intevals, start, end), totalWidth));
            else
                sb.AppendLine(Center(string.Format("Unit: {0}    Total of step {1} - {2}", _MassUnit, start, end), totalWidth));
            if (!string.IsNullOrWhiteSpace(title))
                sb.AppendLine(Center(title, totalWidth));
            sb.AppendLine(thickBar);
            sb.AppendLine();

            sb.Append(new string(' ', nameWidth));
            sb.Append("   ");
            sb.Append((MassHeader + " (" + _MassUnit + ")").PadLeft(valueWidth));
            sb.Append(gap);
            sb.Append(PercentHeader.PadLeft(valueWidth));
            sb.AppendLine();
            sb.AppendLine(thinBar);

            bool firstSection = true;
            foreach (var line in lines)
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

        #endregion

        #region 内部类

        /// <summary>
        /// 列定义：CSV 列标题、显示名称、所属分组与收支类别
        /// </summary>
        private class BudgetColumn
        {
            public BudgetColumn(string header, string name, string group, string category)
            {
                Header = header;
                Name = name;
                Group = group;
                Category = category;
            }

            public string Header { get; private set; }

            public string Name { get; private set; }

            public string Group { get; private set; }

            public string Category { get; private set; }

            public int Index { get; set; }
        }

        static NPBudgetMonitor()
        {
            InitIndex(_NColumns);
            InitIndex(_PColumns);
        }

        /// <summary>
        /// 列定义数组下标即默认 VariableIndex
        /// </summary>
        private static void InitIndex(BudgetColumn[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i].Index = i;
            }
        }

        #endregion
    }

    /// <summary>
    /// 由自定义函数派生的收支项，用于表示相减等非纯求和运算
    /// </summary>
    public class DerivedBudgetItem : MonitorItem
    {
        private Func<ListTimeSeries<double>, double[]> _DeriveFunc;

        public DerivedBudgetItem(string name, Func<ListTimeSeries<double>, double[]> func)
            : base(name)
        {
            _DeriveFunc = func;
            Derivable = true;
        }

        public override double[] Derive(ListTimeSeries<double> source)
        {
            if (_DeriveFunc == null || source == null)
                return null;
            return _DeriveFunc(source);
        }
    }
}
