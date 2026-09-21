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
        private double total_discrepancy = 0;
        public BasinBudgetMonitor()
        {
            ConvertToStrepRate = true;
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


            MonitorItem land_ds = new MonitorItem(HRU_DS)
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

            //MonitorItem total_in = new MonitorItem("Total In")
            //{
            //    VariableIndex = -1,
            //    Group = _Total_Group,
            //    Derivable = true,
            //    DerivedIndex = new int[] { ppt.VariableIndex, sr_in.VariableIndex, gw_in.VariableIndex, wells_in.VariableIndex }
            //};

            //MonitorItem total_out = new MonitorItem("Total Out")
            //{
            //    VariableIndex = -1,
            //    Group = _Total_Group,
            //    Derivable = true,
            //    DerivedIndex = new int[] { et_out.VariableIndex, evap_out.VariableIndex, sr_out.VariableIndex, gw_out.VariableIndex }
            //};

            MonitorItem total_in = new MonitorItem("Total In")
            {
                VariableIndex = 16,
                Group = _Total_Group,
                //Derivable = true,
                //DerivedIndex = new int[] { ppt.VariableIndex, sr_in.VariableIndex, gw_in.VariableIndex, wells_in.VariableIndex }
            };

            MonitorItem total_out = new MonitorItem("Total Out")
            {
                VariableIndex = 17,
                Group = _Total_Group,
                //Derivable = true,
                //DerivedIndex = new int[] { et_out.VariableIndex, evap_out.VariableIndex, sr_out.VariableIndex, gw_out.VariableIndex }
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
        public override DataTable Balance(ref string report)
        {
            if (DataSource != null)
            {
                _EntireBudgetItems.Clear();

                var len = DataSource.Values[0].Count;
                if (EndStep <= 0)
                    EndStep = len;

                if (EndStep > len)
                    EndStep = len;

                if (StartStep > len)
                    StartStep = len;

                if (StartStep >= EndStep)
                    StartStep = 0;

                report = "SUMMARY VOLUMETRIC BUDGET";
                DataTable dt = new DataTable();
                double nsteps = EndStep - StartStep + 1;
                double factor = Intevals / nsteps;
                double total_in = 0;
                double total_out = 0;
                double total_ds = 0;
                double total_diff = 0;
                double total_error = 0;
              
                string equal = " = ";
                int width_term = 30;
                int width_number = 30;
                var scale = Intevals / ModelService.BasinArea * 1000;

                DataColumn dc = new DataColumn("ID", Type.GetType("System.Int32"));
                dt.Columns.Add(dc);
                dc = new DataColumn("ParentID", Type.GetType("System.Int32"));
                dt.Columns.Add(dc);
                dc = new DataColumn("Item", Type.GetType("System.String"));
                dt.Columns.Add(dc);
                dc = new DataColumn("Volumetric_Flow", Type.GetType("System.Double"));
                dt.Columns.Add(dc);
                dc = new DataColumn("Water_Depth", Type.GetType("System.Double"));
                dt.Columns.Add(dc);

                var items = (from item in _Roots[0].Children where item.Group == _In_Group select item).ToArray();
                report += "\r\nIN TERMS";
                report += "\r\n------------";

                foreach (var item in items)
                {
                    var flow = Math.Round((DataSource.Values[item.VariableIndex][EndStep - 1] - DataSource.Values[item.VariableIndex][StartStep - 1]) * factor, DecimalDigit);
                    var dr = dt.NewRow();
                    dr[0] = item.VariableIndex;
                    dr[1] = 100;
                    dr[2] = item.Name;
                    dr[3] = flow;
                    var wd = Math.Round(flow / ModelService.BasinArea * 1000, DecimalDigit);
                    dr[4] = wd;
                    dt.Rows.Add(dr);
                    total_in += flow;
                    _EntireBudgetItems.Add(item.Name, wd);
                }

                report += "\r\n-";
                report += "\r\n-";
                items = (from item in _Roots[0].Children where item.Group == _Out_Group select item).ToArray();
                report += "\r\nOUT TERMS";
                report += "\r\n-----------------";
                foreach (var item in items)
                {
                    var flow = Math.Round((DataSource.Values[item.VariableIndex][EndStep - 1] - DataSource.Values[item.VariableIndex][StartStep - 1]) * factor, DecimalDigit);
                    var dr = dt.NewRow();
                    dr[0] = item.VariableIndex;
                    dr[1] = 200;
                    dr[2] = item.Name;
                    dr[3] = flow;
                    var wd = Math.Round(flow / ModelService.BasinArea * 1000, DecimalDigit);
                    dr[4] = wd;
                    dt.Rows.Add(dr);
                    total_out += flow;
                    _EntireBudgetItems.Add(item.Name, wd);
                }

                report += "\r\n-";
                report += "\r\n-";
                items = (from item in _Roots[0].Children where item.Group == _Ds_Group select item).ToArray();
                report += "\r\nSTORAGE CHANGE TERMS";
                report += "\r\n------------------------------------";
                var uzf_ds = 0.0;
                foreach (var item in items)
                {
                    var flow = Math.Round((DataSource.Values[item.VariableIndex][EndStep - 1] - DataSource.Values[item.VariableIndex][StartStep - 1]) * factor, DecimalDigit);
                    var dr = dt.NewRow();
                    dr[0] = item.VariableIndex;
                    dr[1] = 300;
                    dr[2] = item.Name;
                    dr[3] = flow;
                    var wd = Math.Round(flow / ModelService.BasinArea * 1000, DecimalDigit);
                    dr[4] = wd;
                    dt.Rows.Add(dr);

                    total_ds += flow;
                    if (item.Name == Unsaturated_Zone_DS)
                        uzf_ds = flow;

                    _EntireBudgetItems.Add(item.Name, wd);
                }

                total_diff = total_in - total_out;
                total_error = total_diff - total_ds;

                total_discrepancy = Math.Round((total_in - total_out - total_ds) / (total_in + total_out + Math.Abs(total_ds)) * 2 * 100, DecimalDigit);

                var dr_totalin = dt.NewRow();
                dr_totalin[0] = 100;
                dr_totalin[1] = 9999;
                dr_totalin[2] = "Total In";
                dr_totalin[3] = total_in;
                dr_totalin[4] = Math.Round(total_in / ModelService.BasinArea * 1000, DecimalDigit);
                dt.Rows.Add(dr_totalin);

                var dr_totalout = dt.NewRow();
                dr_totalout[0] = 200;
                dr_totalout[1] = 9999;
                dr_totalout[2] = "Total Out";
                dr_totalout[3] = total_out;
                dr_totalout[4] = Math.Round(total_out / ModelService.BasinArea * 1000, DecimalDigit);
                dt.Rows.Add(dr_totalout);

                var dr_totalds = dt.NewRow();
                dr_totalds[0] = 300;
                dr_totalds[1] = 9999;
                dr_totalds[2] = Total_Storage_Change;
                dr_totalds[3] = total_ds;
                dr_totalds[4] = Math.Round(total_ds / ModelService.BasinArea * 1000, DecimalDigit);
                dt.Rows.Add(dr_totalds);


                var dr_total_error = dt.NewRow();
                dr_total_error[0] = 400;
                dr_total_error[1] = 9999;
                dr_total_error[2] = "Budget Error";
                dr_total_error[3] = total_discrepancy;
                dt.Rows.Add(dr_total_error);

                var dr_in_out_diff = dt.NewRow();
                dr_in_out_diff[0] = 401;
                dr_in_out_diff[1] = 400;
                dr_in_out_diff[2] = "Inflows - Outflows";
                dr_in_out_diff[3] = total_diff;
                dr_in_out_diff[4] = Math.Round(total_diff / ModelService.BasinArea * 1000, DecimalDigit);
                dt.Rows.Add(dr_in_out_diff);

                var dr_error = dt.NewRow();
                dr_error[0] = 402;
                dr_error[1] = 400;
                dr_error[2] = "Overall Budget Error";
                dr_error[3] = total_error;
                dr_error[4] = Math.Round(total_error / ModelService.BasinArea * 1000, DecimalDigit);
                dt.Rows.Add(dr_error);

                var dr_percent = dt.NewRow();
                dr_percent[0] = 403;
                dr_percent[1] = 400;
                dr_percent[2] = "Percent Discrepancy";
                dr_percent[3] = total_discrepancy;
                dt.Rows.Add(dr_percent);

                var wd_ds = Math.Round(total_ds / ModelService.BasinArea * 1000, DecimalDigit);
                _EntireBudgetItems.Add(Total_Storage_Change, wd_ds);
                report += "\r\nBUDGET SUMMERY";
                report += "\r\n--------------";
                report += "\r\nTOTAL IN".PadLeft(width_term, ' ') + equal + total_in.ToString().PadLeft(width_number, ' ');
                report += "\r\nTOTAL OUT".PadLeft(width_term, ' ') + equal + total_out.ToString().PadLeft(width_number, ' ');
                report += "\r\nTOTAL STORAGE CHANGE".PadLeft(width_term, ' ') + equal + total_ds.ToString().PadLeft(width_number, ' ');

                report += "\r\nBUDGET ERROR";
                report += "\r\n--------------";
                report += "\r\nINFLOWS - OUTFLOWS".PadLeft(width_term, ' ') + equal + total_diff.ToString().PadLeft(width_number, ' ');
                report += "\r\nOVERALL BUDGET ERROR".PadLeft(width_term, ' ') + equal + total_error.ToString().PadLeft(width_number, ' ');
                report += "\r\nPERCENT DISCREPANCY".PadLeft(width_term, ' ') + equal + total_discrepancy.ToString().PadLeft(width_number, ' ');


                return dt;
            }
            else
            {
                return null;
            }
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
            double factor = Intevals / nsteps / ModelService.BasinArea * 1000;

            var scale = Intevals / ModelService.BasinArea * 1000;

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
                            dv = Math.Round(dv, 1);
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
                                    dv = Math.Round(dv, 1);
                                }
                                items[nm] = dv;
                            }
                        }
                    }
                }
            }

            return items;
        }

    }
}