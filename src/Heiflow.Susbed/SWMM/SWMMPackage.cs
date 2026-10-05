using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Hydrodynamics.Susbed.SWMM
{
    public class SWMMPackage : Package
    {
        public static string PackageName = "SWMM";
        public SWMMPackage()
        {
            Name = "SWMM";
            SubCachements = new List<SubCachement>();
            RainGages = new List<RainGage>();
            RunoffIndex = 4;
            NumOfSubCatVar = 8;
        }

        public DateTime REPORT_START_DATE
        {
            get;
            protected set;
        }
        public DateTime  START_DATE
        {
            get;
            protected set;
        }
        /// <summary>
        /// in seconds
        /// </summary>
        public int REPORT_STEP
        {
            get;
            protected set;
        }

        public List<SubCachement> SubCachements
        {
            get;
            protected set;
        }
        public List<RainGage> RainGages
        {
            get;
            protected set;
        }
        public int RunoffIndex { get; set; }
        public int NumOfSubCatVar { get; private set; }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            if(File.Exists(FileName))
            {
                SubCachements.Clear();
                RainGages.Clear();
                StreamReader sr = new StreamReader(FileName);
                string line = "";
                while(!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if(TypeConverterEx.IsNotNull(line))
                    {
                        if(line.Contains("START_DATE"))
                        {
                            var strs = TypeConverterEx.Split<string>(line);
                            if (strs[0] == "START_DATE")
                            {
                                var datestr = strs[1] + " ";
                                strs = TypeConverterEx.Split<string>(sr.ReadLine());
                                datestr += strs[1];
                                START_DATE = DateTime.Parse(datestr);
                            }
                            else
                            {
                                var datestr = strs[1] + " ";
                                strs = TypeConverterEx.Split<string>(sr.ReadLine());
                                datestr += strs[1];
                                REPORT_START_DATE = DateTime.Parse(datestr);
                            }
                        }
                        else if (line.Contains("REPORT_STEP"))
                        {
                            var strs = TypeConverterEx.Split<string>(line);
                            var datestr = "01/01/2000" + " " + strs[1];
                            var date = DateTime.Parse(datestr);
                            REPORT_STEP = date.Hour * 3600 + date.Minute * 60 + date.Second;
                            if (REPORT_STEP == 86400 - 1)
                                REPORT_STEP = 86400;
                        }
                        else if (line.Contains("RAINGAGES"))
                        {
                            line = sr.ReadLine();
                            line = sr.ReadLine();

                            while (!sr.EndOfStream)
                            {
                                line = sr.ReadLine();
                                if (TypeConverterEx.IsNull(line))
                                {
                                    break;
                                }
                                else
                                {
                                    var strs = TypeConverterEx.Split<string>(line);
                                    var fn = strs[5].Replace("\"", "");
                                    RainGage rain = new RainGage(strs[0])
                                    {
                                        RelativeFileName = fn,
                                        ID = strs[6],
                                        FullFileName = Path.Combine(ModelService.WorkDirectory, fn)
                                    };
                                    RainGages.Add(rain);
                                }
                            }
                        }
                        else if (line.Contains("SUBCATCHMENTS"))
                        {
                            line = sr.ReadLine();
                            line = sr.ReadLine();

                            while (!sr.EndOfStream)
                            {
                                line = sr.ReadLine();
                                if (TypeConverterEx.IsNull(line))
                                {
                                    break;
                                }
                                else
                                {
                                    var strs = TypeConverterEx.Split<string>(line);
                                    var cat = new SubCachement(strs[0])
                                    {
                                        RainGage = strs[1],
                                        Area = double.Parse(strs[3]),
                                        ImpervPercentage = double.Parse(strs[4]),
                                        Width = double.Parse(strs[5]),
                                        Slope = double.Parse(strs[6]),
                                        Parent = this
                                    };
                                    SubCachements.Add(cat);
                                }
                            }
                        }
                        else if (line.Contains("SUBAREAS"))
                        {
                            line = sr.ReadLine();
                            line = sr.ReadLine();
                            for (int i = 0; i < SubCachements.Count; i++)
                            {
                                line = sr.ReadLine();
                                var strs = TypeConverterEx.Split<string>(line);
                                SubCachements[i].N_Imperv = double.Parse(strs[1]);
                                SubCachements[i].N_Perv = double.Parse(strs[2]);
                                SubCachements[i].S_Imperv = double.Parse(strs[3]);
                                SubCachements[i].S_Perv = double.Parse(strs[4]);
                                SubCachements[i].PctZero = double.Parse(strs[5]);
                            }
                        }
                        else if (line.Contains("INFILTRATION"))
                        {
                            line = sr.ReadLine();
                            line = sr.ReadLine();
                            for (int i = 0; i < SubCachements.Count; i++)
                            {
                                line = sr.ReadLine();
                                var strs = TypeConverterEx.Split<string>(line);
                                SubCachements[i].Suction = double.Parse(strs[1]);
                                SubCachements[i].Ksat = double.Parse(strs[2]);
                                SubCachements[i].IMD = double.Parse(strs[3]);
                            }
                        }
                    }
                }
                sr.Close();
                return LoadingState.Normal;
            }
            else
            {
                OnLoadFailed("SWMM 输入文件不存在：" + FileName);
                return LoadingState.FatalError;
            }
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            // The SWMM input file is created by SWMM itself and is read only here.
        }
        public void LoadRunoff()
        {
            var filename = Path.Combine(ModelService.WorkDirectory, "swmm.wad");
            if (File.Exists(filename))
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                var ncat = br.ReadInt32();
                NumOfSubCatVar = br.ReadInt32();
                var step = 0;
                var temp = new float[NumOfSubCatVar];
                var runoff = new List<double>[ncat];
                var dates = new List<DateTime>();
                for (int i = 0; i < ncat; i++)
                    runoff[i] = new List<double>();
                while (!(fs.Position == fs.Length))
                {
                    for (int i = 0; i < ncat; i++)
                    {
                        for (int j = 0; j < NumOfSubCatVar; j++)
                        {
                            temp[j] = br.ReadSingle();
                        }
                        runoff[i].Add(temp[RunoffIndex]);
                    }
                    dates.Add(REPORT_START_DATE.AddSeconds(REPORT_STEP * step));
                    step++;
                }
                for (int i = 0; i < ncat; i++)
                {
                    SubCachements[i].TimeSeries = new Core.Data.ODM.DoubleTimeSeries()
                    {
                        Values = runoff[i].ToArray(),
                        DateTimes = dates.ToArray()
                    };
                }
                fs.Close();
                br.Close();
            }
        }
        public void SaveRunoff()
        {
            foreach (var cat in SubCachements)
                cat.Save();
        }
    }
}
