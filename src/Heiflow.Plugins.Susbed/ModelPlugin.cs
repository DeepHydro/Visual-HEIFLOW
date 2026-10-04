// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Climate;
using Heiflow.Controls.WinForm.Modflow;
using Heiflow.Controls.WinForm.Processing;
using Heiflow.Models.Generic;
using Heiflow.Models.Hydrodynamics.Susbed.SWMM;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Presentation;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public class ModelPlugin : Extension
    {
        private SimpleActionItem _overlandflow_para;
        private SimpleActionItem _junction_para;     

        private SimpleActionItem _climate;
        private SimpleActionItem _waterlevel;
        private SimpleActionItem _discharge;
        private SimpleActionItem _overlandflow;
        private SimpleActionItem _time;

        private SimpleActionItem _section;
        private SimpleActionItem _bed;
        private SimpleActionItem _branch_sand;

        private SimpleActionItem _RunRunoffModel;
        private SimpleActionItem _RunFloodModel;

        public ModelPlugin()
        {

        }
        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        public override void Activate()
        {
            _climate = new SimpleActionItem("kModel", "气象数据", Climate_Clicked)
            {
                Key = "kClimate",
                ToolTipText = "气象数据",
                GroupCaption = "模型输入",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.modis_storm,
                SortOrder = 2
            };
            App.HeaderControl.Add(_climate);

            _overlandflow_para = new SimpleActionItem("kModel", "产流过程", OverlandflowPara_Clicked)
            {
                Key = "kOverlandflowPara",
                ToolTipText = "产流过程",
                GroupCaption = "模型输入",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.shapes,
                SortOrder = 2
            };
            App.HeaderControl.Add(_overlandflow_para);

            _junction_para = new SimpleActionItem("kModel", "汇流过程", JunctionPara_Clicked)
            {
                Key = "kJunctionPara",
                ToolTipText = "汇流过程",
                GroupCaption = "模型输入",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.mapView32,
                SortOrder = 2
            };
            App.HeaderControl.Add(_junction_para);

            //_routing_para = new SimpleActionItem("kModel", "洪水演进", Routing_Clicked)
            //{
            //    Key = "kRoutingPara",
            //    ToolTipText = "洪水演进",
            //    GroupCaption = "模型输入",
            //    LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.modis_flood,
            //    SortOrder = 2
            //};
            //App.HeaderControl.Add(_routing_para);

            _section = new SimpleActionItem("kModel", "初始断面", Section_Clicked)
            {
                Key = "kSection",
                ToolTipText = "初始断面",
                GroupCaption = "模型输入",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources._698986_icon_139_package_32,
                SortOrder = 2
            };
            App.HeaderControl.Add(_section);

            _bed = new SimpleActionItem("kModel", "河床级配", Bed_Clicked)
            {
                Key = "kBed",
                ToolTipText = "河床级配",
                GroupCaption = "模型输入",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.local_on,
                SortOrder = 2
            };
            App.HeaderControl.Add(_bed);

            _branch_sand = new SimpleActionItem("kModel", "水沙条件", BranchSand_Clicked)
            {
                Key = "kBranchSand",
                ToolTipText = "干流水沙",
                GroupCaption = "模型输入",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.if_river_494761,
                SortOrder = 2
            };
            App.HeaderControl.Add(_branch_sand);

            //_tributary_sand = new SimpleActionItem("kModel", "支流水沙", TributarySand_Clicked)
            //{
            //    Key = "kTributarySand",
            //    ToolTipText = "支流水沙",
            //    GroupCaption = "输入文件",
            //    LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.river,
            //    SortOrder = 2
            //};
            //App.HeaderControl.Add(_tributary_sand);

            _RunRunoffModel = new SimpleActionItem("kModel", "产流计算", RunRunoff_Clicked)
            {
                Key = "kRunRunoffModel",
                ToolTipText = "产流计算",
                GroupCaption = "模型运行",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.LasRGB32,
                SortOrder = 2
            };
            App.HeaderControl.Add(_RunRunoffModel);
      
            _RunFloodModel = new SimpleActionItem("kModel", "洪水演进", RunFlood_Clicked)
            {
                Key = "kRunFloodModel",
                ToolTipText = "洪水演进计算",
                GroupCaption = "模型运行",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.play_on,
                SortOrder = 2
            };
            App.HeaderControl.Add(_RunFloodModel);


            _time = new SimpleActionItem("kModel", "输出时间步长", OutputTime_Clicked)
            {
                Key = "kOutputTime",
                ToolTipText = "输出时间步长",
                GroupCaption = "模型输出",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.QuickTour32,
                SortOrder = 5,
            };
          //  App.HeaderControl.Add(_time);

            _overlandflow = new SimpleActionItem("kModel", "产流过程", Overlandflow_Clicked)
            {
                Key = "kOverlandflow",
                ToolTipText = "产流过程",
                GroupCaption = "模型输出",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.Delineation_icon_32,
                SortOrder = 5
            };
            App.HeaderControl.Add(_overlandflow);

            _waterlevel = new SimpleActionItem("kModel", "水位过程",WaterLevel_Clicked)
            {
                Key = "kWaterLevel",
                ToolTipText = "水位过程",
                GroupCaption = "模型输出",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.curve_chart,
                SortOrder = 5
            };
            App.HeaderControl.Add(_waterlevel);

            _discharge = new SimpleActionItem("kModel", "流量过程", Discharge_Clicked)
            {
                Key = "kDischarge",
                ToolTipText = "流量过程",
                GroupCaption = "模型输出",
                LargeImage = Heiflow.Plugins.Susbed.Properties.Resources.RasterImageAnalysisInteractiveHistogramStretch32,
                SortOrder = 5
            };
            App.HeaderControl.Add(_discharge);
        }
        private void Climate_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var pck = ProjectManager.Project.Model.Packages[SWMMPackage.PackageName] as SWMMPackage;
                ClimateDataForm form = new ClimateDataForm(pck);
                form.ShowDialog();
            }
        }

        private void OverlandflowPara_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var pck = ProjectManager.Project.Model.Packages[SWMMPackage.PackageName] as SWMMPackage;
                OverlandFlowForm form = new OverlandFlowForm(pck);
                form.ShowDialog();
            }
        }

        private void JunctionPara_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var pck = ProjectManager.Project.Model.Packages[ParameterPackage.PackageName] as ParameterPackage;
                JunctionParaForm form = new JunctionParaForm(pck);
                form.ShowDialog();
            }
        }

        private void Routing_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var pck = ProjectManager.Project.Model.Packages[ParameterPackage.PackageName] as ParameterPackage;
                RoutingParaForm form = new RoutingParaForm();
                form.ShowDialog();
            }
        }

        private void RunFlood_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var catfile = Path.Combine(ModelService.WorkDirectory, "swmm.wad");
                if(!File.Exists(catfile))
                {
                    MessageBox.Show("请先进行产流计算!", "模型运行", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    CreateNoWindow = false,
                    FileName = "susbed.exe",
                    UseShellExecute = false,
                    ErrorDialog = true,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    WorkingDirectory = ModelService.WorkDirectory,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                _RunFloodModel.Enabled = false;
                _RunRunoffModel.Enabled = false;

                var flood_worker = new Process();
                flood_worker.EnableRaisingEvents = true;
                flood_worker.Exited += flood_worker_Exited;
                flood_worker.ErrorDataReceived += flood_worker_ErrorDataReceived;
                flood_worker.StartInfo = info;
                flood_worker.Start();
               
            }
        }

        private void flood_worker_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            MessageBox.Show("洪水演进计算失败，请检查输入文件!", "模型运行", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void RunRunoff_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    CreateNoWindow = false,
                    FileName = "swmm.exe",
                    UseShellExecute = false,
                    ErrorDialog = true,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    WorkingDirectory = ModelService.WorkDirectory,
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = "swmm.inp swmm.rpt swmm.out swmm.wad"
                };
                _RunFloodModel.Enabled = false;
                _RunRunoffModel.Enabled = false;
                var runoff_worker = new Process();
                runoff_worker.EnableRaisingEvents = true;
                runoff_worker.Exited += runoff_worker_Exited;
                runoff_worker.ErrorDataReceived += runoff_worker_ErrorDataReceived;
                runoff_worker.StartInfo = info;
                runoff_worker.Start();
            }
        }

        private void runoff_worker_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            MessageBox.Show("产流计算失败，请检查输入文件!", "模型运行", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        private void flood_worker_Exited(object sender, EventArgs e)
        {
            MessageBox.Show("计算成功!", "模型运行", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _RunFloodModel.Enabled = true;
            _RunRunoffModel.Enabled = true;
        }
        private void runoff_worker_Exited(object sender, EventArgs e)
        {
            var model = ProjectManager.Project.Model as Susbed;
            var swmm = model.Packages[SWMMPackage.PackageName] as SWMMPackage;
            swmm.LoadRunoff();
            swmm.SaveRunoff();
            _RunFloodModel.Enabled = true;
            _RunRunoffModel.Enabled = true;
        }

        private void Section_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var pck = ProjectManager.Project.Model.Packages[CrossSectionPackage.PackageName] as CrossSectionPackage;
                CrossSectionForm form = new CrossSectionForm(pck);
                form.ShowDialog();
            }
        }

        private void Bed_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var pck = ProjectManager.Project.Model.Packages[SedGradationPackage.PackageName] as SedGradationPackage;
                SedGradationForm form = new SedGradationForm(pck);
                form.ShowDialog();
            }
        }
        private void BranchSand_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var model = ProjectManager.Project.Model as Susbed;
                FlowSedForm form = new FlowSedForm(model);
                form.ShowDialog();
            }
        }
        private void TributarySand_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var model = ProjectManager.Project.Model as Susbed;
                FlowSedForm form = new FlowSedForm(model);
                form.ShowDialog();
            }
        }
        private void WaterLevel_Clicked(object sender, EventArgs e)
        {
           if(Check())
           {
               var pck = ProjectManager.Project.Model.Packages["WaterLevel"] as FlowOutPackage;
               var cspck = ProjectManager.Project.Model.Packages[CrossSectionPackage.PackageName] as CrossSectionPackage;
               var parapck = ProjectManager.Project.Model.Packages[ParameterPackage.PackageName] as ParameterPackage;
               FlowOutViewForm form = new FlowOutViewForm(pck, cspck, parapck);
               form.Text = "水位过程";
               form.ShowDialog();
           }
        }

        private void Discharge_Clicked(object sender, EventArgs e)
        {
            if (Check())
            {
                var pck = ProjectManager.Project.Model.Packages["Discharge"] as FlowOutPackage;
                var cspck = ProjectManager.Project.Model.Packages[CrossSectionPackage.PackageName] as CrossSectionPackage;
                var parapck = ProjectManager.Project.Model.Packages[ParameterPackage.PackageName] as ParameterPackage;
                FlowOutViewForm form = new FlowOutViewForm(pck, cspck, parapck);
                form.Text = "流量过程";
                form.ShowDialog();
            }
        }
        private void Overlandflow_Clicked(object sender, EventArgs e)
        {
             if (Check())
             {
                 var pck = ProjectManager.Project.Model.Packages[SWMMPackage.PackageName] as SWMMPackage;
                 RunoffDataForm form = new RunoffDataForm(pck);
                 form.ShowDialog();
             }
        }
        private void OutputTime_Clicked(object sender, EventArgs e)
        {

        }

        private bool Check()
        {
            if (ProjectManager.Project == null)
            {
                MessageBox.Show("请先打开工程文件！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
                return true;
        }

   
    }
}
