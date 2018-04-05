// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.IO;
using Heiflow.Models.Atmosphere;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Math
{
    public class PMPETSingle : ModelTool
    {

        public PMPETSingle()
        {
            Name = "Single Penman Monteith PET using 6 Variables";
            Category = "Hydrology";
            Description = "Calculate PET using FAO PM Model using 6 meterological variables at single location.";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            CloudCover = 0.15;
            PET = "PET";
        }
        [Category("Input")]
        [Description("The data file should be a txt or csv file. It cotains 7 columns, including date, tav,tmax,tmin,relative humidity, air pressure and wind speed. The units should be K, K, K, percentage, Kpa and m/s")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string InputDataFile
        {
            get;
            set;
        }

        public double Longitude
        {
            get;
            set;
        }
        public double Latitude
        {
            get;
            set;
        }
        [Category("Parameter")]
        public double CloudCover
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The matrix name of PET")]
        public string PET
        {
            get;
            set;
        }

        public override void Initialize()
        {
            Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            cancelProgressHandler.Progress("Package_Tool", 10, "Begin to calculate");
            PenmanMonteithET pet = new PenmanMonteithET();
            StreamReader sr = new StreamReader(InputDataFile);
            List<float> et0 = new List<float>();
            int nrow = 0;
            sr = new StreamReader(InputDataFile);
            List<DateTime> dates = new List<DateTime>();
            List<float[]> meto = new List<float[]>();
            List<float> tav = new List<float>();
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (!TypeConverterEx.IsNull(line))
                {
                    var strs = TypeConverterEx.Split<string>(line);
                    var date = DateTime.Parse(strs[0]);
                    var vv = TypeConverterEx.SkipSplit<float>(line, 1);
                    dates.Add(date);
                    meto.Add(vv);
                    tav.Add(vv[0]);
                    nrow++;
                }
            }
            sr.Close();

             var ts = new FloatTimeSeries(tav.ToArray(), dates.ToArray());
             var tav_mon = TimeSeriesAnalyzer.GetMonthlyMean(ts, NumericalDataType.Average);
             pet.MonthTemperature = Array.ConvertAll(tav_mon, x => (double)x);
            for (int i = 0; i < nrow; i++)
            {
                var vv = meto[i];
                et0.Add((float)pet.ET0(Latitude, Longitude, vv[0], vv[1], vv[2], vv[3], vv[4], vv[5], dates[i], CloudCover));

            }
            My3DMat<float> mat_out = new My3DMat<float>(1, 1, et0.Count);
            mat_out.Value[0][0] = et0.ToArray();
            mat_out.Name = PET;
            cancelProgressHandler.Progress("Package_Tool", 100, "Calculated");
            Workspace.Add(mat_out);
            return true;
        }

    }
}