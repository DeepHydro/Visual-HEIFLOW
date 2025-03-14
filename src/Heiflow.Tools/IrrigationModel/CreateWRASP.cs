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

using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.WRM;
using Heiflow.Presentation.Services;
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
            Name = "Create WRA Stress Period Input File";
            Category = "Irrigation Model";
            Description = "Create WRA Stress Period Input File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            EndCycle = 4;
            StartCycle = 1;
            PumpScale = 1.5;
            PumpingLayers = "1,2,3";
            PumpingLayerRatios = "0.6,0.2,0.2";
            WelPackageUnitID = 108;
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

        public int WelPackageUnitID
        {
            get;
            set;
        }
        public double PumpScale
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("An integer array that specfies the layers from which pumping are applied. An example is: 1,2,3")]
        public string PumpingLayers
        {
            get;
            set;
        }
        [Category("Farm Parameters")]
        [Description("An double array that specifies the ratio of pumping in each layer. An example is: 0.6,0.2,0.2")]
        public string PumpingLayerRatios
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Initialized = true;
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
                    ObjType = int.Parse(buf[3].Trim()),
                    Drawdown = double.Parse(buf[4].Trim()),
                    SegID = int.Parse(buf[5].Trim()),
                    ReachID = int.Parse(buf[6].Trim()),
                    HRU_Num = int.Parse(buf[7].Trim())
                };
                obj.HRU_List = TypeConverterEx.Split<int>(buf[8].Trim(trims));
                obj.HRU_Area = TypeConverterEx.Split<double>(buf[9].Trim(trims));
                obj.Total_Area = obj.HRU_Area.Sum();
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
 
 
        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
                  var model = prj.Project.Model;
            var mfgrid = model.Grid as RegularGrid;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;

            int[] well_layer = TypeConverterEx.Split<int>(PumpingLayers);
            double[] layer_ratio = TypeConverterEx.Split<double>(PumpingLayerRatios);
            int num_well_layer = well_layer.Length;

            StreamReader sr_quota = new StreamReader(QuotaFileName);
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
            var num_irrg_obj = inttemp[0];
            var num_indust_obj = inttemp[1];
            line = sr_quota.ReadLine();
            irrg_obj_list.Clear();
            indust_obj_list.Clear();
            ReadObj(sr_quota, num_irrg_obj, irrg_obj_list);
            ReadObj(sr_quota, num_indust_obj, indust_obj_list);
            sr_quota.Close();

            var wrafile = new WRASPFile();
            wrafile.CalcObjPumpConstraint(irrg_obj_list, quota, PumpScale);
            wrafile.SavePumpWellFiles(shell, prj.Project, mf, mfgrid, irrg_obj_list, well_layer, layer_ratio);
            cancelProgressHandler.Progress("Package_Tool", 50, "HRU Wel file saved");
            wrafile.SaveWRAFile(OutputFileName, irrg_obj_list, indust_obj_list, quota, nquota, well_layer, layer_ratio, StartCycle, EndCycle);
            cancelProgressHandler.Progress("Package_Tool", 100, "WRA input file saved");
         
            return true;
        }
    }
}