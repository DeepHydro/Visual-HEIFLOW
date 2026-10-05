// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Hydrodynamics.Susbed.SWMM;
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface.Packages;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    [Export(typeof(IBasicModel))]
    [ModelItem]
    public class Susbed : BaseModel
    {

        /// <summary>
        /// .\Input\MODFLOW\
        /// </summary>
        public const string InputDic = ".\\Input\\";
        /// <summary>
        /// .\Output\
        /// </summary>
        public const string OutputDic = ".\\Output\\";

        public Susbed()
        {
            this.TimeService = new TimeService("Susbed Timeline")
            {
                UseStressPeriods = false
            };
       
            _IsDirty = false;
        }
    
        public override void Initialize()
        {
            TimeServiceList.Add(this.TimeService.Name, this.TimeService);
        }
        public override bool New(ICancelProgressHandler progress)
        {
            bool succ = true;
         
            return succ;
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        { 
            var state = LoadingState.Normal;
            if (File.Exists(ControlFileName))
            {
                var cmt = new char[] { '\'' };
                StreamReader sr = new StreamReader(ControlFileName, Encoding.Default);
                var line = sr.ReadLine();
                var strs = TypeConverterEx.Split<string>(line);
                ParameterPackage para_pck = new ParameterPackage();
                para_pck.Owner = this;
                para_pck.FileName = strs[1];
                progress.Progress("Susbed", 1, "读取参数文件...");
                if (para_pck.Load(progress) == LoadingState.FatalError)
                {
                    progress.Progress("Susbed", 100, "参数文件读取失败");
                    state = LoadingState.FatalError;
                }
                else
                    AddInSilence(para_pck);

                line = sr.ReadLine();
                strs = TypeConverterEx.Split<string>(line);
                string datafile = Path.Combine(WorkDirectory, strs[1]);
                StreamReader sr_data = new StreamReader(datafile, Encoding.Default);
                line = sr_data.ReadLine();
                line = sr_data.ReadLine();

                line = sr_data.ReadLine();
                strs = TypeConverterEx.Split<string>(line);
                CrossSectionPackage cspck = new CrossSectionPackage()
                {
                    Name = "CrossSection",
                    Owner = this,
                    FileName = strs[1].Trim(cmt),
                    NumSections= para_pck.NPXT
                };
                AddInSilence(cspck);
                cspck.Load(progress);

                line = sr_data.ReadLine();
                strs = TypeConverterEx.Split<string>(line);
                SedGradationPackage sgpck = new SedGradationPackage()
                {
                    Owner = this,
                    FileName = strs[1].Trim(cmt),
                    NumSections = para_pck.NPXT
                };
                AddInSilence(sgpck);

                line = sr_data.ReadLine();
                strs = TypeConverterEx.Split<string>(line);
                FlowSedPackage main_flowpck = new FlowSedPackage()
                {
                    Name = "MainStreamFlowSed",
                    Owner = this,
                    DataFiles = new string[] { strs[1].Trim(cmt) },
                    TableNames = new string[1]
                };
                main_flowpck.GetTableName(0);
                AddInSilence(main_flowpck);

                line = sr_data.ReadLine();
                FlowSedPackage tributrary_flowpck = new FlowSedPackage()
                {
                    Name = "TributraryFlowSed",
                    Owner = this
                };
                tributrary_flowpck.DataFiles = new string[para_pck.NUMBR];
                tributrary_flowpck.TableNames = new string[para_pck.NUMBR];
                for (int i = 0; i < para_pck.NUMBR; i++)
                {
                    line = sr_data.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    tributrary_flowpck.DataFiles[i] = strs[1].Trim(cmt);
                    tributrary_flowpck.GetTableName(i);
                }
                AddInSilence(tributrary_flowpck);

                line = sr_data.ReadLine();
                FlowSedPackage node_flowpck = new FlowSedPackage()
                {
                    Name = "NodeFlowSed",
                    Owner = this
                };
                node_flowpck.DataFiles = new string[para_pck.INODE];
                node_flowpck.TableNames = new string[para_pck.INODE];
                for (int i = 0; i < para_pck.INODE; i++)
                {
                    line = sr_data.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    node_flowpck.DataFiles[i] = strs[1].Trim(cmt);
                    node_flowpck.GetTableName(i);
                }
                AddInSilence(node_flowpck);

                line = sr_data.ReadLine();
                var dic = new Dictionary<string, string>();
                for (int i = 0; i < 9; i++)
                {
                    line = sr_data.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    var temp = strs[1].Trim(cmt);
                    dic.Add(strs[0], temp);
                }
                FlowOutPackage waterlevel = new FlowOutPackage()
                {
                    Name = "WaterLevel",
                    Owner = this,
                    FileName = dic["SWGC"]
                };
                AddInSilence(waterlevel);

                FlowOutPackage discharge = new FlowOutPackage()
                {
                    Name = "Discharge",
                    Owner = this,
                    FileName = dic["LLGC"]
                };
                AddInSilence(discharge);

                line = sr.ReadLine();
                strs = TypeConverterEx.Split<string>(line);
                SWMMPackage swmm = new SWMMPackage()
                {
                    Owner = this,
                    FileName = strs[1]
                };
                AddInSilence(swmm);

                CreateNetWork();
                swmm.Load(progress);
                sr.Close();
                sr_data.Close();
            }
            else
            {
                state = LoadingState.FatalError;
            }
            return state;
        }
        public override void Clear()
        {
            foreach(var pck in Packages.Values)
            {
                pck.Clear();
            }
            Packages.Clear();
            TimeServiceList.Clear();
        }

        private void CreateNetWork()
        {
            var cspck = GetPackage("CrossSection") as CrossSectionPackage;
            var para_pck = GetPackage(ParameterPackage.PackageName) as ParameterPackage;
            var cs_ids = (from cs in cspck.CrossSections select cs.ID).ToArray();
            foreach (var tr in para_pck.Tributaries)
            {
                for (int i = tr.LowerSectionID; i <= tr.UpperSectionID; i++)
                {
                    cs_ids[i - 1] = -1;
                    tr.SectionIDs.Add(i);
                }
            }
            for (int i = 0; i < cs_ids.Length; i++)
            {
                if (cs_ids[i] != -1)
                {
                    para_pck.MainStream.SectionIDs.Add(cs_ids[i]);
                }
            }
          
        }

        public override bool Validate()
        {
            return true;
        }
        public override void Save(ICancelProgressHandler progress)
        {
            // Susbed reads the text files written by the pre-processor. Nothing is written back yet.
        }
  
      
        public override bool Exsit(string filename)
        {
            return true;
        }

      
        public override void OnTimeServiceUpdated(ITimeService time)
        {
        }
        public override void OnGridUpdated(IGrid sender)
        {

        }

        public override bool LoadGrid(ICancelProgressHandler progress)
        {
            // Susbed is a one dimensional model, it has no grid to load.
            return false;
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            // Susbed has no spatial feature that has to be added to the map.
        }
    }
}
