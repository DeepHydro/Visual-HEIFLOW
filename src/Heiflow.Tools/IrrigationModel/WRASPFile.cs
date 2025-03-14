using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Subsurface;
using Heiflow.Models.WRM;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.DataManagement
{
    public class WRASPFile
    {
        public WRASPFile()
        {

        }

        public void CalcObjPumpConstraint(List<ManagementObject> list, double[,] quota, double pumpScale)
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
                            temp += obj.HRU_Area[j] * quota[k, i] / 1000 * (1 - obj.SW_Ratio) * pumpScale;
                            pump_days++;
                        }
                    }
                    obj.Max_Pump_Rate[j] = System.Math.Round(temp / pump_days, 0);
                    obj.Max_Total_Pump += temp;
                }
                obj.Max_Total_Pump = System.Math.Round(obj.Max_Total_Pump, 0);
            }
        }

        public bool SavePumpWellFiles(IShellService shell, IProject prj, Modflow mf, RegularGrid mfgrid, List<ManagementObject> irrg_obj_list, int[] well_layer, double[] layer_ratio)
        {
            if (mf != null)
            {
                if (!mf.Packages.ContainsKey(WELPackage.PackageName))
                {
                    var wel = mf.Select(WELPackage.PackageName);
                    mf.Add(wel);
                }
                var mfout = mf.GetPackage(MFOutputPackage.PackageName) as MFOutputPackage;
                var pck = mf.GetPackage(WELPackage.PackageName) as WELPackage;

                var np = 2;
                var nhru_well = 0;
                var nwel = 0;
                var nlayer = well_layer.Length;

                var dic = prj.WRAInputDirectory;
                var hru_wellfile = Path.Combine(dic, "hru_well_sp1.txt");
                StreamWriter sw_hruwel = new StreamWriter(hru_wellfile);
                var line = string.Format("{0} {1} # num_pumplayer, num_pumpwell", nlayer, nwel);
                sw_hruwel.WriteLine(line);

                for (int i = 0; i < irrg_obj_list.Count; i++)
                {
                    var obj = irrg_obj_list[i];
                    nhru_well += obj.HRU_List.Length;
                }

                nwel = nhru_well * nlayer;
                var welnum_list = new int[] { nwel, -1 };
                pck.MXACTW = nwel;
                pck.IWELCB = 0;
                pck.FluxRates = new DataCube<float>(4, np, nwel)
                {
                    DateTimes = new System.DateTime[np],
                    Variables = new string[4] { "Layer", "Row", "Column", "Q" }
                };
                pck.FluxRates.Flags[0] = TimeVarientFlag.Individual;
                pck.FluxRates.Multipliers[0] = 1;
                pck.FluxRates.IPRN[0] = -1;

                int k = 0;
                for (int i = 0; i < irrg_obj_list.Count; i++)
                {
                    var obj = irrg_obj_list[i];
                    for (int j = 0; j < obj.HRU_List.Length; j++)
                    {
                        var hruid = obj.HRU_List[j];
                        var loc = mfgrid.Topology.ActiveCellLocation[hruid - 1];
                        for (int l = 0; l < nlayer; l++)
                        {
                            pck.FluxRates[0, 0, k] = well_layer[l];
                            pck.FluxRates[1, 0, k] = loc[0] + 1;
                            pck.FluxRates[2, 0, k] = loc[1] + 1;
                            pck.FluxRates[3, 0, k] = 0;
                            if (k == 0)
                            {
                                line = string.Format("{0} {1} {2} {3} {4} # hru_id layer row column ratio", hruid, well_layer[l], loc[0] + 1, loc[1] + 1, layer_ratio[l]);
                            }
                            else
                            {
                                line = string.Format("{0} {1} {2} {3} {4}", hruid, well_layer[l], loc[0] + 1, loc[1] + 1, layer_ratio[l]);
                            }
                            sw_hruwel.WriteLine(line);
                            k++;
                        }
                    }
                }
                pck.FluxRates.Flags[1] = TimeVarientFlag.Repeat;
                pck.FluxRates.Multipliers[1] = 1;
                pck.FluxRates.IPRN[1] = -1;

                pck.CompositeOutput(mfout);
                pck.CreateFeature(shell.MapAppManager.Map.Projection, prj.GeoSpatialDirectory);
                pck.BuildTopology();
                pck.IsDirty = true;
                pck.Save(null);
                pck.ChangeState(Models.Generic.ModelObjectState.Ready);

                sw_hruwel.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveWRAFile(string filename, List<ManagementObject> irrg_obj_list, List<ManagementObject> indust_obj_list, double[,] quota, int nquota, int[] well_layer, double[] layer_ratio, int startCycle, int endCycle)
        {
            string newline = "";
            StreamWriter sw_out = new StreamWriter(filename);
            int num_irrg_obj = irrg_obj_list.Count;
            int num_indust_obj = indust_obj_list.Count;
            int num_well_layer = well_layer.Length;
            newline = "# Water resources allocation package " + DateTime.Now;
            sw_out.WriteLine(newline);
            newline = string.Format("{0}\t{1}\t0\t0\t # Data Set 1 num_irrg_obj, num_indu_obj, num_doms_obj, num_ecos_obj ", num_irrg_obj, num_indust_obj);
            sw_out.WriteLine(newline);

            if (num_irrg_obj > 0)
            {
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

            sw_out.WriteLine(startCycle + " # cycle index");
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
                newline = "1 1	1	1	1	1	1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, quota_id_flag,plantarea_flag,max_pump_rate_flag,max_total_pump_flag";
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
                        newline += irrg_obj_list[i].ObjType + "\t";
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
                    newline = string.Format("{0} # Withdraw type of object {1}", obj.ObjType, obj.ID);
                    sw_out.WriteLine(newline);
                }
            }

            for (int i = startCycle + 1; i <= endCycle; i++)
            {
                sw_out.WriteLine(i + " # cycle index");
                sw_out.WriteLine("-1 # quota_flag");
                if (num_irrg_obj > 0)
                {
                    sw_out.WriteLine("# irrigation objects");
                    sw_out.WriteLine("-1 -1	-1 -1 -1 -1 -1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag,max_pump_rate_flag,max_total_pump_flag");
                }
                if (num_indust_obj > 0)
                {
                    sw_out.WriteLine("# industrial objects");
                    sw_out.WriteLine("-1 -1	-1	-1	 # 	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag");
                }
            }

        }
    }
}
