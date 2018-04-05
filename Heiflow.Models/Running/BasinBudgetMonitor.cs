// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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

            MonitorItem lake_out = new MonitorItem(Lakes_Outflow)
            {
                VariableIndex = 9,
                Group = _Out_Group
            };

            root.Children.Add(et_out);
            root.Children.Add(evap_out);
            root.Children.Add(sr_out);
            root.Children.Add(gw_out);
            root.Children.Add(lake_out);

            MonitorItem land_ds = new MonitorItem(LAND_SURFACE_Zone_DS)
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
                Group = _Total_Group
            };

            MonitorItem total_out = new MonitorItem("Total Out")
            {
                VariableIndex = 17,
                Group = _Total_Group
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
            string report = "";
            if (_EntireBudgetItems.Count == 0)
                Balance(ref report);
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
            Dictionary<string, double> items = new Dictionary<string, double>();
            var item_names = new string[] { 
                //HRU in
                Daily_PPT, IR_DIV, IR_PUMP, BASINGW2SZ_HRU, BASINSZREJECT ,
                //HRU out
                BASINPERVET_HRU,BASINIMPERVEVAP_HRU,BASININTCPEVAP_HRU,BASINSNOWEVAP_HRU,BASININTERFLOW,BASINSROFF,BASINSZ2GW,BASINLAKEINSZ,BASINHORTONIANLAKES,
                //UZF IN
                UZF_INFIL,
                //UZF OUT
                UZF_RECHARGE,UZF_ET,
                //UZF DS
                UZF_DS,
                //SZ IN UZF_RECHARGE
                GW_INOUT,STREAM_LEAKAGE,
                //SZ OUT BASINGW2SZ_HRU
                SAT_ET,
                //SZ DS
                SAT_CHANGE_STOR
            };

            foreach (var nm in item_names)
            {
                items.Add(nm, 0);
                var buf = Select(nm);
                if (buf.Monitor.DataSource != null)
                {
                    var vector = buf.Monitor.DataSource.Values[buf.VariableIndex].Skip<double>(StartStep);
                    double dv = 0;
                    if (buf.SequenceType == SequenceType.StepbyStep)
                        dv = vector.Average() * scale;
                    else
                        dv = (vector.Last() - vector.ElementAt(StartStep - 1)) * factor;
                    dv = Math.Round(dv, 1);
                    items[nm] = dv;
                }
            }

            //==========HRU
            var hru_in = items[Daily_PPT] + items[IR_DIV] + items[IR_PUMP] + items[BASINGW2SZ_HRU] + items[BASINSZREJECT];
            var hru_out = items[BASINPERVET_HRU] + items[BASINIMPERVEVAP_HRU] + items[BASININTCPEVAP_HRU] + items[BASINSNOWEVAP_HRU]
                + items[BASININTERFLOW] + items[BASINSROFF] + items[BASINSZ2GW] + items[BASINLAKEINSZ] + items[BASINHORTONIANLAKES];
            var hru_ds_item = Select(HRU_DS);

            var vec = hru_ds_item.Derive(hru_ds_item.Monitor.DataSource);
            var hru_ds = vec.Average() * scale;
            hru_ds = Math.Round(hru_ds, 1);
            var hru_error = hru_in - hru_out - hru_ds;
            var hru_dispy = Math.Round((hru_error) / (hru_in + hru_out + Math.Abs(hru_ds)) * 2 * 100, 2);

            items.Add(HRU_IN, hru_in);
            items.Add(HRU_OUT, hru_out);
            items.Add(HRU_DS, hru_ds);
            items.Add(HRU_ERROR, hru_error);
            items.Add(HRU_DISYP, hru_error);

            //==================UZF
            var uzf_out = items[UZF_RECHARGE] + items[UZF_ET];
            var uzf_ds = items[UZF_DS];
            var uzf_error = items[UZF_INFIL] - uzf_out - uzf_ds;
            var uzf_dispy = Math.Round((uzf_error) / (items[UZF_INFIL] + uzf_out + Math.Abs(uzf_ds)) * 2 * 100, 2);

            items.Add(UZF_IN, items[UZF_INFIL]);
            items.Add(UZF_OUT, uzf_out);
            items.Add(UZF_ERROR, uzf_error);
            items.Add(UZF_DISPY, uzf_dispy);

            //===============SAT
            var sat_names = new string[] 
            { 
                //in 
                CONSTANT_HEAD_IN, WELLS_IN, SPECIFIED_FLOWS_IN ,  STORAGE_IN, UZF_RECHARGE_IN,STREAM_LEAKAGE_IN,LAKE_SEEPAGE_IN,
                //out
                CONSTANT_HEAD_OUT,WELLS_OUT,SPECIFIED_FLOWS_OUT,SURFACE_LEAKAGE_OUT,GW_ET_OUT,STREAM_LEAKAGE_OUT,STORAGE_OUT,LAKE_SEEPAGE_OUT
            };

            foreach (var nm in sat_names)
            {
                items.Add(nm, 0);
                var buf = Select(nm);
                if (buf != null)
                {
                    var vector = buf.Monitor.DataSource.Values[buf.VariableIndex];//.Skip<double>(StartStep);
                    double dv = 0;
                    if (buf.SequenceType == SequenceType.StepbyStep)
                    {
                        dv = vector.Average() * scale;
                    }
                    else
                        dv = (vector.Last() - vector.ElementAt(1)) * factor;

                    dv = Math.Round(dv, 1);
                    items[nm] = dv;
                }
            }

            var sat_in = items[CONSTANT_HEAD_IN] + items[WELLS_IN] + items[SPECIFIED_FLOWS_IN] + items[UZF_RECHARGE_IN]
                + items[STREAM_LEAKAGE_IN] + items[LAKE_SEEPAGE_IN] + items[STORAGE_IN];
            var sat_out = items[CONSTANT_HEAD_OUT] + items[WELLS_OUT] + items[SPECIFIED_FLOWS_OUT] + items[GW_ET_OUT] + items[SURFACE_LEAKAGE_OUT]
    + items[STREAM_LEAKAGE_OUT] + items[LAKE_SEEPAGE_OUT] + items[STORAGE_OUT];
            var sat_ds = items[STORAGE_OUT] - items[STORAGE_IN];
            var sat_error = sat_in - sat_out - sat_ds;
            var sat_discrepancy = Math.Round((sat_error) / (sat_in + sat_out + Math.Abs(sat_ds)) * 2 * 100, 2);

            items.Add(SAT_IN, sat_in);
            items.Add(SAT_OUT, sat_out);
            items.Add(SAT_DS, sat_ds);
            items.Add(SAT_ERROR, sat_error);
            items.Add(SAT_DISPY, sat_discrepancy);

            items.Add(TOTAL_DISPY, total_discrepancy);
            // Canal, SFR, LAKE
            var canal_names = new string[]
            {
                Canal_DS,CANAL_ET,Canal_Drainage,
                SFRET,LAKET,IR_Industry
            };

            foreach (var nm in canal_names)
            {
                items.Add(nm, 0);
                var buf = Select(nm);
                if (buf != null)
                {
                    var vector = buf.Monitor.DataSource.Values[buf.VariableIndex];//.Skip<double>(StartStep);
                    double dv = 0;
                    if (buf.SequenceType == SequenceType.StepbyStep)
                    {
                        dv = vector.Average() * scale;
                    }
                    else
                        dv = (vector.Last() - vector.ElementAt(1)) * factor;

                    dv = Math.Round(dv, 1);
                    items[nm] = dv;
                }
            }

            var percolation = items[UZF_INFIL];
            items.Add(Percolation, percolation);

            foreach (var item in _EntireBudgetItems)
            {
                items.Add(item.Key, item.Value);
            }
            return items;
        }
    }
}