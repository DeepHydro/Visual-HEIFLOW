﻿//
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
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.WRM;
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
    public class WithdrawInputFile : ModelTool
    {
        //   private string _DiversionFileName;
        private string _QuotaFileName;
        private List<ManagementObject> irrg_obj_list = new List<ManagementObject>();
        private List<ManagementObject> indust_obj_list = new List<ManagementObject>();
        public WithdrawInputFile()
        {
            Name = "Water Withdraw Input File";
            Category = "Irrigation Model";
            Description = "Create Water Withdraw Input File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            EndCycle = 4;
            StartCycle = 1;
            PumpScale = 1.5;
            GWCompensate = true;
        }

        [Category("Input")]
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
                OutputFileName = _QuotaFileName + ".out";
            }
        }

        public string OutputFileName
        {
            get;
            set;
        }

        public int EndCycle
        {
            get;
            set;
        }

        public int StartCycle
        {
            get;
            set;
        }

        public bool GWCompensate
        {
            get;
            set;
        }
        public double PumpScale
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Initialized = true;
        }

        private void SaveObj()
        {
            StreamReader sr_source = new StreamReader(QuotaFileName);
            string obj_out = QuotaFileName + ".obj";
            StreamWriter sw_obj = new StreamWriter(obj_out);

            var newline = "";
            var line = sr_source.ReadLine();
            line = sr_source.ReadLine();

            for (int i = 0; i < 47; i++)
            {
                string obj_line = "";
                line = sr_source.ReadLine();
                var buf = TypeConverterEx.Split<string>(line);
                obj_line = buf[0] + "," + buf[2] + "," + buf[1];
                var nhru = int.Parse(buf[1]);
                //hru id
                newline = sr_source.ReadLine(); ;
                obj_line += "," + newline.Trim();
                // hru area
                line = sr_source.ReadLine();
                var hru_area = new double[nhru];
                var buf_num = TypeConverterEx.Split<double>(line);
                for (int j = 0; j < nhru; j++)
                {
                    hru_area[j] = System.Math.Round(nhru * 1000000 * buf_num[j], 0);
                    if (hru_area[j] > 1000000)
                        hru_area[j] = 1000000;
                }
                obj_line += "," + string.Join("\t", hru_area);

                //canal_effciency_rate
                newline = sr_source.ReadLine();
                var temp = TypeConverterEx.Split<double>(newline);
                obj_line += "," + temp[0];
                // canal_area_rate
                newline = sr_source.ReadLine();
                temp = TypeConverterEx.Split<double>(newline);
                obj_line += "," + temp[0];
                sw_obj.WriteLine(obj_line);
            }
            sw_obj.Close();
        }

        private void ReadObj(StreamReader sr, int numobj, List<ManagementObject> list)
        {
            char[] trims = new char[] { ' ', '"' };
            for (int i = 0; i < numobj; i++)
            {
                var line = sr.ReadLine();
                var buf = TypeConverterEx.Split<string>(line, TypeConverterEx.Comma);
                ManagementObject obj = new ManagementObject()
                {
                    ID = int.Parse(buf[0].Trim()),
                    Name = buf[1].Trim(),
                    SW_Ratio = double.Parse(buf[2].Trim()),
                    ObjType_Fram = int.Parse(buf[3].Trim()),
                    Drawdown = double.Parse(buf[4].Trim()),
                    SegID = int.Parse(buf[5].Trim()),
                    ReachID = int.Parse(buf[6].Trim()),
                    HRU_Num = int.Parse(buf[7].Trim())
                };
                obj.HRU_List = TypeConverterEx.Split<int>(buf[8].Trim(trims));
                obj.HRU_Area = TypeConverterEx.Split<double>(buf[9].Trim(trims));
                obj.Total_Area = obj.HRU_Area.Sum();
                Debug.WriteLine(obj.Total_Area);
                obj.Canal_Efficiency = double.Parse(buf[10].Trim());
                obj.Canal_Ratio = double.Parse(buf[11].Trim());
                obj.Inlet_Type = int.Parse(buf[12].Trim());
                obj.Inlet_MinFlow = double.Parse(buf[13].Trim());
                obj.Inlet_MaxFlow = double.Parse(buf[14].Trim());
                obj.Inlet_Flow_Ratio = double.Parse(buf[15].Trim());
                obj.SW_Cntl_Factor = buf[16].Trim(trims);
                obj.GW_Cntl_Factor = buf[17].Trim(trims);
                list.Add(obj);
            }
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
                            temp += obj.HRU_Area[j] * quota[k, i] / 1000 * (1 - obj.SW_Ratio) * PumpScale;
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
            int num_well_layer = 3;
            int[] well_layer = new int[] { 1, 2, 3 };
            double[] layer_ratio = new double[] { 0.6, 0.1, 0.3 };
            int num_irrg_obj, num_indust_obj;
            StreamReader sr_quota = new StreamReader(QuotaFileName);
            StreamWriter sw_out = new StreamWriter(OutputFileName);
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

            line = sr_quota.ReadLine().Trim();
            var inttemp = TypeConverterEx.Split<int>(line.Trim());
            num_irrg_obj = inttemp[0];
            num_indust_obj = inttemp[1];
            //ID	NAME	地表水比例  用水类型  允许降深
            line = sr_quota.ReadLine();
            irrg_obj_list.Clear();
            indust_obj_list.Clear();
            ReadObj(sr_quota, num_irrg_obj, irrg_obj_list);
            ReadObj(sr_quota, num_indust_obj, indust_obj_list);
            CalcObjPumpConstraint(irrg_obj_list, quota);

            newline = "# Water resources allocation package " + DateTime.Now;
            sw_out.WriteLine(newline);
            newline = string.Format("{0}\t{1}\t0\t0\t # Data Set 1 num_irrg_obj, num_indu_obj, num_doms_obj, num_ecos_obj ", num_irrg_obj, num_indust_obj);
            sw_out.WriteLine(newline);

            sw_out.WriteLine("# Data Set 2 irrigation objects");
            for (int i = 0; i < num_irrg_obj; i++)
            {
                var obj = irrg_obj_list[i];
                int oid = i + 1;
                newline = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t#	 Data Set 2a oid, hrunum, iseg, ireach, num_well_layer, inlet_type {6}", obj.ID, obj.HRU_Num, obj.SegID, obj.ReachID, num_well_layer, obj.Inlet_Type, obj.Name);
                sw_out.WriteLine(newline);
                newline = string.Join("\t", obj.HRU_List);
                newline += "\t# Data Set 2b IHRU ID list for the object " + oid;
                sw_out.WriteLine(newline);
                var canal_eff = new double[obj.HRU_Num];
                var canal_ratio = new double[obj.HRU_Num];
                for (int j = 0; j < obj.HRU_Num; j++)
                {
                    canal_eff[j] = obj.Canal_Efficiency;
                    canal_ratio[j] = obj.Canal_Ratio;
                }
                newline = string.Join("\t", canal_eff) + "\t# Data Set 2c canal efficiency for each IHRU in the object " + oid;
                sw_out.WriteLine(newline);
                newline = string.Join("\t", canal_ratio) + "\t# Data Set 2d canal area ratio for each IHRU in the object " + oid;
                sw_out.WriteLine(newline);
                for (int j = 0; j < num_well_layer; j++)
                {
                    newline = well_layer[j] + "\t" + layer_ratio[j] + "\t# Data Set 2e well_layer layer_ratio  in the object " + oid;
                    sw_out.WriteLine(newline);
                }
                newline = string.Format("{0}\t# Data Set 2f	drawdown constraint of object {1}", irrg_obj_list[i].Drawdown, oid);
                sw_out.WriteLine(newline);
                newline = string.Format("{0}\t{1}\t{2}\t#  inlet	min flow,  max flow and flow ratio  for object {3}", irrg_obj_list[i].Inlet_MinFlow, irrg_obj_list[i].Inlet_MaxFlow, irrg_obj_list[i].Inlet_Flow_Ratio,
                    irrg_obj_list[i].ID);
                sw_out.WriteLine(newline);
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
                    newline = string.Format("{0}\t#	drawdown constaint of object {1}", obj.Drawdown, obj.ID);
                    sw_out.WriteLine(newline);
                    newline = string.Format("{0}\t{1}\t{2}\t#  inlet	min flow,  max flow and flow ratio for object {3}", obj.Inlet_MinFlow, obj.Inlet_MaxFlow, obj.Inlet_Flow_Ratio, obj.ID);
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

            newline = "# irrigation objects";
            sw_out.WriteLine(newline);
            if(GWCompensate)
                newline = "1 1	1	1	1	1	1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, quota_id_flag,plantarea_flag,max_pump_rate_flag,max_total_pump_flag";
            else
                newline = "1 1	1	1	1	1	1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, quota_id_flag,plantarea_flag";
            sw_out.WriteLine(newline);
            //地表水比例
            for (int i = 0; i < num_irrg_obj; i++)
            {
                var ratio = irrg_obj_list[i].SW_Ratio;
                //if (mid_zone_id.Contains(irrg_obj_list[i].ID))
                //{
                //    ratio = ratio * sw_scale[k];
                //}
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
                newline += "# Quota ID of object " + (i + 1);
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
                    newline += "# SW ratio of object " + (indust_obj_list[i].ID);
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
                sw_out.WriteLine("# irrigation objects");
                if(GWCompensate)
                    sw_out.WriteLine("-1 -1	-1 -1 -1 -1 -1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag,max_pump_rate_flag,max_total_pump_flag");
                else
                    sw_out.WriteLine("-1 -1	-1 -1 -1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag");

                if (num_indust_obj > 0)
                {
                    sw_out.WriteLine("# industrial objects");
                    sw_out.WriteLine("-1 -1	-1	-1	 # 	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag");
                }
            }

            sr_quota.Close();
            sw_out.Close();

            return true;
        }
    }

   
}