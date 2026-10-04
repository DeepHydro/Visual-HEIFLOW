// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public class CrossSectionPackage : Package
    {
        public static string PackageName = "CrossSection";
        public CrossSectionPackage()
        {
            Name = PackageName;
        }

        public int NumSections { get; set; }
        public CrossSection[] CrossSections { get; private set; }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            if (File.Exists(FileName))
            {
                CrossSections = new CrossSection[NumSections];
                StreamReader sr = new StreamReader(FileName, Encoding.Default);
                var line = sr.ReadLine();
                line = sr.ReadLine();
                line = sr.ReadLine();
                for (int i = 0; i < NumSections; i++)
                {
                    line = sr.ReadLine();
                    var strs = TypeConverterEx.Split<string>(line);
                    CrossSections[i] = new CrossSection()
                    {
                        ID = int.Parse(strs[0]),
                        Name = strs[1],
                        Length = double.Parse(strs[2])
                    };
                }
                line = sr.ReadLine();
                for (int i = 0; i < NumSections; i++)
                {
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    var strs = TypeConverterEx.Split<string>(line);
                    CrossSections[i].NumNodes = int.Parse(strs[1]);
                    for (int j = 0; j < CrossSections[i].NumNodes; j++)
                    {
                        line = sr.ReadLine();
                        strs = TypeConverterEx.Split<string>(line);
                        CrossSections[i].Distance[j] = double.Parse(strs[1]);
                        CrossSections[i].Elevation[j] = double.Parse(strs[2]);
                    }
                    line = sr.ReadLine();
                }

                sr.Close();
                return LoadingState.Normal;
            }
            else
            {
                OnLoadFailed("断面文件不存在：" + FileName);
                return LoadingState.FatalError;
            }
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            // The cross section file is written by the pre-processor. The package only reads it.
        }
    }
}
