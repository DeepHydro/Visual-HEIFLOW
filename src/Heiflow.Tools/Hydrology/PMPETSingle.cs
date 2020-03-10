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
            DefaultAlbedo = 0.23;
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
        [Category("Parameter")]
        public double DefaultAlbedo
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
            double short_rad = 0;
            double long_rad = 0;
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

             var ts = new DataCube<float>(tav.ToArray(), dates.ToArray());
             var tav_mon = TimeSeriesAnalyzer.GetMonthlyMean(ts, NumericalDataType.Average);
             pet.MonthTemperature = Array.ConvertAll(tav_mon, x => (double)x);
            for (int i = 0; i < nrow; i++)
            {
                var vv = meto[i];
                et0.Add((float)pet.ET0(Latitude, Longitude, vv[0], vv[1], vv[2], vv[3], vv[4], vv[5], dates[i], CloudCover,DefaultAlbedo, ref short_rad, ref long_rad));

            }
            DataCube<float> mat_out = new DataCube<float>(1, 1, et0.Count);
            mat_out[0,"0",":"] = et0.ToArray();
            mat_out.Name = PET;
            cancelProgressHandler.Progress("Package_Tool", 100, "Calculated");
            Workspace.Add(mat_out);
            return true;
        }

    }
}