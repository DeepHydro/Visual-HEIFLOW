using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Hydrodynamics.Susbed.SWMM
{
    public class SubCachement
    {
        public SubCachement(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string RainGage { get; set; }
        public double Area { get; set; }
        public double ImpervPercentage { get; set; }
        public double Width { get; set; }
        public double Slope { get; set; }
        public double N_Imperv { get; set; }
        public double N_Perv { get; set; }
        public double S_Imperv { get; set; }
        public double S_Perv { get; set; }
        public double PctZero { get; set; }
        public double Suction { get; set; }
        public double Ksat { get; set; }
        public double IMD { get; set; }
        public int SectionID { get; set; }
        public string FullFileName { get; set; }
        public DoubleTimeSeries TimeSeries { get; set; }

        public SWMMPackage Parent { get; set; }

        public void Save()
        {
            if (TimeSeries == null)
                return;
            var filename = Path.Combine(ModelService.WorkDirectory, Name + ".txt");
            StreamWriter sw = new StreamWriter(filename);
            string line = Name;
            sw.WriteLine(line);
            sw.WriteLine("");
            sw.WriteLine("累计	进口	出口	坝前	悬移质S1	悬移质S2	悬移质S3	悬移质S4	悬移质S5	悬移质S6	悬移质S7	悬移质S8	推移质S9");
            sw.WriteLine("天数	流量	流量	水位	d=0.007	d=0.01	d=0.025	d=0.05	d=0.1	d=0.25	d=0.5	d=1	d=2");
            sw.WriteLine("(天)	(m3/s)	(m3/s)	（m）	(kg/m3)	(kg/m3)(kg/m3)	(kg/m3)	(kg/m3)	(kg/m3)	(kg/m3)	(kg/m3)	(kg/m3)	(kg/s)");
            sw.WriteLine("0	0	0	0	0	0	0	0	0	0	0	0	0");
            sw.WriteLine("0.00001	0	0	0	0	0	0	0	0	0	0	0	0");
            for (int i = 0; i < TimeSeries.DateTimes.Length; i++)
            {
                var day = (i + 1) * Parent.REPORT_STEP / 86400.0;
                line = string.Format("{0} {1} {2} 0	0	0	0	0	0	0	0	0	0", Math.Round(day, 4), Math.Round(TimeSeries.Values[i], 4), Math.Round(TimeSeries.Values[i], 4));
                sw.WriteLine(line);
                Debug.WriteLine(i);
            }
            sw.Close();
        }
    }
}
