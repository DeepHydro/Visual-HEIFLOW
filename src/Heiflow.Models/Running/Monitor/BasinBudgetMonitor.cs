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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Heiflow.Models.IO;
using System.ComponentModel.Composition;
using Heiflow.Models.Generic;

namespace Heiflow.Models.Running
{
    [Export(typeof(IFileMonitor))]
    public class BasinBudgetMonitor : FileMonitor
    {
        private Dictionary<string, double> _EntireBudgetItems = new Dictionary<string, double>();
          
        public BasinBudgetMonitor()
        {
           
            Correct = false;
            MonitorName = "BasinBudgetMonitor";
            var root = new MonitorItemCollection("Basin Water Budgets");
            _Roots.Add(root);

            MonitorItem ppt = new MonitorItem(PPT)
            {
                VariableIndex = 0,
                Group = _In_Group
            };

            MonitorItem sr_in = new MonitorItem(Streams_Inflow)
            {
                VariableIndex = 1,
                Group = _In_Group
            };

            MonitorItem gw_in = new MonitorItem(Groundwater_Inflow)
            {
                VariableIndex = 2,
                Group = _In_Group
            };

            MonitorItem wells_in = new MonitorItem(Wells_In)
            {
                VariableIndex = 3,
                Group = _In_Group
            };

            MonitorItem lakes_in = new MonitorItem(Lakes_Inflow)
            {
                VariableIndex = 4,
                Group = _In_Group
            };

            root.Children.Add(ppt);
            root.Children.Add(sr_in);
            root.Children.Add(gw_in);
            root.Children.Add(wells_in);
            root.Children.Add(lakes_in);

            MonitorItem et_out = new MonitorItem(Evapotranspiration)
            {
                VariableIndex = 5,
                Group = _Out_Group
            };

            MonitorItem evap_out = new MonitorItem(Evaporation)
            {
                VariableIndex = 6,
                Group = _Out_Group
            };

            MonitorItem sr_out = new MonitorItem(Streams_Outflow)
            {
                VariableIndex = 7,
                Group = _Out_Group
            };

            MonitorItem gw_out = new MonitorItem(Groundwater_Outflow)
            {
                VariableIndex = 8,
                Group = _Out_Group
            };

            MonitorItem wells_out = new MonitorItem(Wells_Out)
            {
                VariableIndex = 9,
                Group = _Out_Group
            };


            root.Children.Add(et_out);
            root.Children.Add(evap_out);
            root.Children.Add(sr_out);
            root.Children.Add(gw_out);
            root.Children.Add(wells_out);


            MonitorItem land_ds = new MonitorItem(Surface_Zone_DS)
            {
                VariableIndex = 10,
                Group = _Ds_Group
            };
            MonitorItem soil_ds = new MonitorItem(Soil_Zone_DS)
            {
                VariableIndex = 11,
                Group = _Ds_Group
            };

            MonitorItem uz_ds = new MonitorItem(Unsaturated_Zone_DS)
            {
                VariableIndex = 12,
                Group = _Ds_Group
            };

            MonitorItem sa_ds = new MonitorItem(Saturated_Zone_DS)
            {
                VariableIndex = 13,
                Group = _Ds_Group
            };

            MonitorItem lake_ds = new MonitorItem(Lakes_Zone_DS)
            {
                VariableIndex = 14,
                Group = _Ds_Group
            };

            MonitorItem canal_ds = new MonitorItem(Canals_Zone_DS)
            {
                VariableIndex = 15,
                Group = _Ds_Group
            };

            root.Children.Add(land_ds);
            root.Children.Add(soil_ds);
            root.Children.Add(uz_ds);
            root.Children.Add(sa_ds);
            root.Children.Add(lake_ds);
            root.Children.Add(canal_ds);

            MonitorItem total_in = new MonitorItem("Total In")
            {
                VariableIndex = 16,
                Group = _Total_Group,
            };

            MonitorItem total_out = new MonitorItem("Total Out")
            {
                VariableIndex = 17,
                Group = _Total_Group,
            };

            MonitorItem total_ds = new MonitorItem(Total_Storage_Change)
            {
                VariableIndex = 18,
                Group = _Total_Group
            };

            MonitorItem total_error = new MonitorItem("Total Budget Error")
            {
                VariableIndex = 19,
                Group = _Total_Group
            };

            MonitorItem total_dis = new MonitorItem("Percent Discrepancy")
            {
                VariableIndex = 20,
                Group = _Total_Group
            };

            root.Children.Add(total_in);
            root.Children.Add(total_out);
            root.Children.Add(total_ds);
            root.Children.Add(total_error);
            root.Children.Add(total_dis);

            foreach (var item in root.Children)
            {
                item.Monitor = this;
                item.SequenceType = SequenceType.Accumulative;
            }
            _Watcher = new CSVWatcher();

            int nvar = root.Children.Count;
            VarIndexIsConvert = new bool[nvar];
            for (int i = 0; i < nvar; i++)
            {
                VarIndexIsConvert[i] = true;
            }
            VarIndexIsConvert[20] = false;
        }

        public bool Correct
        {
            get;
            set;
        }

        /// <summary>
        /// Create balance table
        /// </summary>
        /// <param name="report">balance report</param>
        /// <returns></returns>
        public override DataTable Balance(string itemname, ref string report)
        {
            if (DataSource != null)
            {
                _EntireBudgetItems.Clear();

                ClampSteps(DataSource.Values[0].Count);

                var dt = CreateBudgetTable();
                double nsteps = EndStep - StartStep + 1;
                double factor = Intevals / nsteps;
                double total_in = 0;
                double total_out = 0;
                double total_ds = 0;
                double total_diff = 0;
                double total_error = 0;

                // 累积序列取首末差值
                Func<MonitorItem, IEnumerable<double>> selector = item =>
                {
                    var series = DataSource.Values[item.VariableIndex];
                    return new[] { series[EndStep - 1] - series[StartStep - 1] };
                };

                var lines = new List<BudgetLine>();

                var items = (from item in _Roots[0].Children where item.Group == _In_Group && item.Name != Lakes_Inflow select item).ToArray();
                lines.Add(BudgetLine.Section("IN TERMS"));
                total_in = AddTermRows(items, dt, 100, factor, lines, selector);

                items = (from item in _Roots[0].Children where item.Group == _Out_Group select item).ToArray();
                lines.Add(BudgetLine.Section("OUT TERMS"));
                total_out = AddTermRows(items, dt, 200, factor, lines, selector);

                items = (from item in _Roots[0].Children where item.Group == _Ds_Group select item).ToArray();
                lines.Add(BudgetLine.Section("STORAGE CHANGE TERMS"));
                total_ds = AddTermRows(items, dt, 300, factor, lines, selector);

                foreach (var row in dt.Rows.Cast<DataRow>())
                {
                    var name = row[2].ToString();
                    if (!_EntireBudgetItems.ContainsKey(name))
                        _EntireBudgetItems.Add(name, (double)row[4]);
                }

                total_diff = total_in - total_out;
                total_error = total_diff - total_ds;

                var total_discrepancy = PercentDiscrepancy(total_in, total_out, total_ds);

                AddSummaryRows(dt, lines, total_in, total_out, total_ds, total_diff, total_error, total_discrepancy);

                var wd_ds = ToDepth(total_ds);
                if (!_EntireBudgetItems.ContainsKey(Total_Storage_Change))
                    _EntireBudgetItems.Add(Total_Storage_Change, wd_ds);

                report = BuildReport(itemname, lines);

                return dt;
            }
            else
            {
                return null;
            }
        }
     
        public void Load()
        {
            _DataSource.Clear();

            //_DataSource.Add
            var ppt = Select(Daily_PPT);
            var sfrin = Select(SFR_INFLOW);

            //ppt.Monitor.DataSource[ppt.VariableIndex]
        }

        public override Dictionary<string, double> ZonalBudgets()
        {
            Dictionary<string, double> items = new Dictionary<string, double>();
            var len = DataSource.Values[0].Count;
            if (EndStep <= 0)
                EndStep = len;

            if (EndStep > len)
                EndStep = len;

            if (StartStep > len)
                StartStep = len;

            if (StartStep > EndStep)
                StartStep = EndStep;

            double nsteps = EndStep - StartStep + 1;
            double factor = 1.0;
            var scale = 1.0;
            if (BudgetItemUnit == Running.BudgetItemUnit.BasinAveraged)
            {
                factor = Intevals / nsteps / ModelService.BasinArea * 1000;
                scale = Intevals / ModelService.BasinArea * 1000;
            }
            else
            {
                factor = 1.0;
                scale = 1.0;
            }

            foreach (var root in this.Root)
            {
                foreach (var item in root.Children)
                {
                    var nm = item.Name;
                    items.Add(nm, 0);
                    if (item.Monitor.DataSource != null)
                    {
                        var vector = item.Monitor.DataSource.Values[item.VariableIndex].Skip<double>(StartStep);
                        double dv = 0;
                        if (vector != null && vector.Count() > 0)
                        {
                            if (item.SequenceType == SequenceType.StepbyStep)
                                dv = vector.Average() * scale;
                            else
                                dv = (vector.Last() - vector.ElementAt(StartStep - 1)) * factor;
                            //dv = Math.Round(dv, 1);
                        }
                        items[nm] = dv;
                    }
                }
            }

            foreach (var par in this.Partners)
            {
                foreach (var root in par.Root)
                {
                    foreach (var item in root.Children)
                    {
                        var nm = item.Name;
                        if (items.ContainsKey(nm))
                        {
                            Console.WriteLine(nm);
                            continue;
                        }
                        items.Add(nm, 0);
                        if (item.Derivable)
                        {
                            var vec = item.Derive(item.Monitor.DataSource);
                            var dv = vec.Average() * scale;
                            items[nm] = dv;
                        }
                        else
                        {
                            if (item.Monitor.DataSource != null)
                            {
                                var vector = item.Monitor.DataSource.Values[item.VariableIndex].Skip<double>(StartStep);
                                double dv = 0;
                                if (vector != null && vector.Count() > 0)
                                {
                                    if (item.SequenceType == SequenceType.StepbyStep)
                                        dv = vector.Average() * scale;
                                    else
                                        dv = (vector.Last() - vector.ElementAt(StartStep - 1)) * factor;
                                    //dv = Math.Round(dv, 1);
                                }
                                items[nm] = dv;
                            }
                        }
                    }
                }
            }

            return items;
        }

        protected override void OnConvertToStrepRateChanged()
        {
             if(_ConvertToStrepRate)
             {
                 foreach (var root in this.Root)
                 {
                     foreach (var item in root.Children)
                     {
                         item.SequenceType = SequenceType.StepbyStep;
                     }
                 }
             }
            else
             {
                 foreach (var root in this.Root)
                 {
                     foreach (var item in root.Children)
                     {
                         item.SequenceType = SequenceType.Accumulative;
                     }
                 }
             }
        }
    }
}