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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using DotSpatial.Data;
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.DataManagement
{
    public class WithdrawObjects : ModelTool
    {
        private string _DiversionFileName;
        private string _QuotaFileName;

        public WithdrawObjects()
        {
            Name = "Water Withdraw Objects";
            Category = "Irrigation Model";
            Description = "Set Canal Parameters";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The diversion filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DiversionFileName
        {
            get
            {
                return _DiversionFileName;
            }
            set
            {
                _DiversionFileName = value;
            }
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
            }
        }

        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int num_well_layer = 3;
            int num_cycle = 13;
            int[] well_layer = new int[] { 1, 2, 3 };
            double[] layer_ratio = new double[] { 0.6, 0.1, 0.3 };
            int nrrg_obj = 0;
             int nindust_obj = 1;

            string div_out = DiversionFileName + ".out";
            StreamReader sr_source = new StreamReader(DiversionFileName);
            StreamReader sr_quota = new StreamReader(QuotaFileName);
            StreamWriter sw_out = new StreamWriter(div_out);
            string newline = "";

            var line = sr_source.ReadLine();
            line = sr_source.ReadLine();
            var strs_buf = TypeConverterEx.Split<string>(line);
            nrrg_obj = int.Parse(strs_buf[0]);
            List<double[]> area_list = new List<double[]>();

            int nquota = 1;
            int ntime = 36;
            line = sr_quota.ReadLine();
            strs_buf = TypeConverterEx.Split<string>(line);
            nquota = int.Parse(strs_buf[0]);
            ntime = int.Parse(strs_buf[1]);
            double[,] quota_src = new double[ntime, nquota];
            double[,] quota = new double [366,nquota];
            double[][] hru_areas = new double[nrrg_obj][];
            int[] hrunum = new int[nrrg_obj];
            int[] plant_type = new int[nrrg_obj];
            int day = 0;
            var start = new DateTime(2000, 1, 1);
            for (int i = 0; i < ntime; i++)
            {
                line = sr_quota.ReadLine().Trim();
                var buf = TypeConverterEx.Split<string>(line);
                var ss = DateTime.Parse(buf[0]);
                var ee = DateTime.Parse(buf[1]);
                var cur = ss;
                while(cur <= ee)
                {
                    for (int j = 0; j < nquota; j++)
                        quota[day, j] = double.Parse(buf[2 + j]);
                    day++;
                    cur = cur.AddDays(1);
                }
            }
            newline = "# Water resources allocation package " + DateTime.Now;
            sw_out.WriteLine(newline);
            newline = string.Format("{0}\t{1}\t0\t0\t # num_irrg_obj, num_indu_obj, num_doms_obj, num_ecos_obj ", nrrg_obj, nindust_obj);
            sw_out.WriteLine(newline);

            sw_out.WriteLine("# irrigation objects");
            for (int i = 0; i < nrrg_obj; i++)
            {
                line = sr_source.ReadLine();
                var buf = TypeConverterEx.Split<string>(line);
                newline = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t#	oid, hrunum, iseg, ireach, num_well_layer", i, buf[1], buf[0], buf[2], num_well_layer);
                sw_out.WriteLine(newline);

                int nhru = int.Parse(buf[1]);
                hrunum[i] = nhru;
                if (nhru > 0)
                {
                    //hru id
                    line = sr_source.ReadLine();
                    newline = line;
                    sw_out.WriteLine(newline);
                    // hru area
                    line = sr_source.ReadLine();
                    hru_areas[i] = new double[nhru];
                    var buf_num = TypeConverterEx.Split<double>(line);
                    for (int j = 0; j < nhru; j++)
                    {
                        hru_areas[i][j] = System.Math.Round(nhru * 1000000 * buf_num[j], 0);
                    }

                    //canal_effciency_rate
                    newline = sr_source.ReadLine();
                    sw_out.WriteLine(newline);
                    // canal_area_rate
                    newline = sr_source.ReadLine();
                    sw_out.WriteLine(newline);

                    for (int j = 0; j < num_well_layer; j++)
                    {
                        newline = well_layer[j] + "\t" + layer_ratio[j] + " # well_layer layer_ratio";
                        sw_out.WriteLine(newline);
                    }
                }
            }
            sw_out.WriteLine("# industrial objects");
            for (int i = 0; i < nindust_obj; i++)
            {
                newline = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t#	oid, hrunum, iseg, ireach, num_well_layer", nrrg_obj + i, 1, 20, 1, num_well_layer);
                sw_out.WriteLine(newline);
                newline = string.Format("{0}\t#	%	hru_id_list", 52935);
                sw_out.WriteLine(newline);
                for (int j = 0; j < num_well_layer; j++)
                {
                    newline = well_layer[j] + "\t" + layer_ratio[j] + " # well_layer layer_ratio";
                    sw_out.WriteLine(newline);
                }
            }

            sw_out.WriteLine("1 # cycle index");
            sw_out.WriteLine("1	#	quota_flag");
            for (int i = 0; i < nquota; i++)
            {
                newline = "";
                for (int j = 0; j< 366; j++)
                {
                    newline += quota[j, i].ToString("0.0") + "\t";
                }
                newline += "quota of object " + (i + 1);
                sw_out.WriteLine(newline);
            }
            newline = "1 1	1	1	1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag";
            sw_out.WriteLine(newline);

            newline = "# irrigation objects";
            sw_out.WriteLine(newline);
            //ID	NAME	地表水比例
            line = sr_quota.ReadLine();
            for (int i = 0; i < nrrg_obj; i++)
            {
                  line = sr_quota.ReadLine();
                  var buf = TypeConverterEx.Split<string>(line.Trim());
                  var ratio = double.Parse(buf[2]);
                  plant_type[i] = int.Parse(buf[3]);
                  newline = "";
                 for(int j=0;j<366;j++)
                 {
                     newline += ratio + "\t";
                 }
                 newline += "SW ratio of object " + (i + 1);
                 sw_out.WriteLine(newline);
            }
            //地表引水控制系数
            for (int i = 0; i < nrrg_obj; i++)
            {
                newline = "";
                var control = 1;
                for (int j = 0; j < 366; j++)
                {
                    newline +=  control + "\t";
                }
                newline += "SW control factor of object " + (i + 1);
                sw_out.WriteLine(newline);
            }
            //地下引水控制系数
            for (int i = 0; i < nrrg_obj; i++)
            {
                newline = "";
                var control = 1;
                for (int j = 0; j < 366; j++)
                {
                    if (j < 75 || j > 305)
                        control = 0;
                    else
                        control = 1;
                    newline += control + "\t";
                }
                newline += "GW control factor of object " + (i + 1);
                sw_out.WriteLine(newline);
            }
            //作物类型
            for (int i = 0; i < nrrg_obj; i++)
            {
                newline = "";
                for (int j = 0; j < hrunum[i];j++ )
                {
                    newline += plant_type[i]+"\t";
                }
                newline += "Plant type of object " + (i + 1);
                sw_out.WriteLine(newline);
            }
            //种植面积
            for (int i = 0; i < nrrg_obj; i++)
            {
                newline = string.Join("\t", hru_areas[i]);
                newline += "\t" + "Plant area of object " + (i + 1);
            }

            newline = "# industrial objects";
            sw_out.WriteLine(newline);
            newline = "1 1	1	1	1 #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag";
            sw_out.WriteLine(newline);
            //地表水比例
            for (int i = 0; i < nindust_obj; i++)
            {
                newline = "";
                var control = 1;
                for (int j = 0; j < 366; j++)
                {
                    newline += control + "\t";
                }
                newline += "SW control factor of object " + (nrrg_obj + i + 1);
                sw_out.WriteLine(newline);
            }

            //地表引水控制系数
            for (int i = 0; i < nindust_obj; i++)
            {
                
                newline = "";
                var control = 1;
                for (int j = 0; j < 366; j++)
                {
                    newline += control + "\t";
                }
                newline += "SW control factor of object " + (nrrg_obj + i + 1);
                sw_out.WriteLine(newline);               
            }

            //地下引水控制系数
            for (int i = 0; i < nindust_obj; i++)
            {
                newline = "";
                var control = 1;
                for (int j = 0; j < 366; j++)
                {
                    newline += control + "\t";
                }
                newline += "GW control factor of object " + (nrrg_obj + i + 1);
                sw_out.WriteLine(newline);
            }

            //用水类型
            for (int i = 0; i < nindust_obj; i++)
            {
                newline = "3 Withdraw type of object " + (nrrg_obj+ i + 1);
                sw_out.WriteLine(newline);
            }

            for (int i = 1; i < num_cycle; i++)
            {
                sw_out.WriteLine( (i+1) + " # cycle index");
                sw_out.WriteLine("-1 # quota_flag");
                sw_out.WriteLine("# irrigation objects");
                sw_out.WriteLine("-1 -1	-1	-1	-1  #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag,plantarea_flag");
                sw_out.WriteLine("# industrial objects");
                sw_out.WriteLine("-1 -1	-1	-1	-1  #	sw_ratio_flag, swctrl_factor_flag , gwctrl_factor_flag, Withdraw_type_flag");
            }

            sr_source.Close();
            sr_quota.Close();
            sw_out.Close();
            cancelProgressHandler.Progress("Package_Tool", 100, "Done");
            return true;
        }
    }
}
