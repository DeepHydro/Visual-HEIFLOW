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
    public class PMPET7 : ModelTool
    {

        public PMPET7()
        {
            Name = "Penman Monteith PET using 7 Variables";
            Category = "Hydrology";
            Description = "Calculate PET based on FAO PM Model using 7 meterological variables";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            InputTemperatureUnit = TemperatureUnit.Kelvin;
            OutputLengthUnit = LengthUnit.inch;
            Start = new DateTime(2000, 1, 1);
        }
        [Category("Input")]
        [Description("The shpfile name")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PointFeatureFileName { get; set; }

        [Category("Input")]
        [Description("The file name of maximum temperature. Temperature unit should be Celsius, Kelvin or Fahrenheit")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string MaxTemperatureFileName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The file name of minimum temperature. Temperature unit should be Celsius, Kelvin or Fahrenheit")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string MinTemperatureFileName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The file name of average temperature. Temperature unit should be Celsius, Kelvin or Fahrenheit")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string AverageTemperatureFileName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The file name of Air Pressure, whose unit shoul be Pa")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string AirPressureFileName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The file name of Relative Humidity, of which the value range must be between 0 and 1")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string RelativeHumidityFileName
        {
            get;
            set;
        }


        [Category("Input")]
        [Description("The file name of Wind Speed,whose unit shoul be m/s")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string WindSpeedFileName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The file name of Net Radiation, whose unit shoul be M/m2")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string NetRadiationFileName
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("The Unit of the  Input Temperature")]
        public TemperatureUnit InputTemperatureUnit { get; set; }

        [Category("Parameter")]
        public LengthUnit OutputLengthUnit { get; set; }

           [Category("Parameter")]
        public DateTime Start
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The name of the output file")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName { get; set; }

        public override void Initialize()
        {
            Initialized = true;
            //string[] files = new string[] {AverageTemperatureFileName, MaxTemperatureFileName, MinTemperatureFileName, RelativeHumidityFileName,
            // AirPressureFileName, WindSpeedFileName, NetRadiationFileName};
            //Initialized = !files.Contains(null);
            //if (!TypeConverterEx.IsNull(PointFeatureFileName) && File.Exists(PointFeatureFileName))
            //{
            //    Initialized = false;
            //}
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
        //    string basedir = @"E:\Heihe\HRB\DataSets\Driving Forces\PXD\";
        //    AverageTemperatureFileName = basedir + "daily_tavk_11243.dcx";
        //    MaxTemperatureFileName = basedir + "daily_tmaxk_11243.dcx";
        //    MinTemperatureFileName = basedir + "daily_tmink_11243.dcx";
        //    RelativeHumidityFileName = @"E:\Heihe\HRB\DataSets\Driving Forces\TY\rh.dcx";
        //    AirPressureFileName = basedir + "daily_ap_11243.dcx";
        //    WindSpeedFileName = basedir + "daily_windspeed_11243.dcx";
        //    NetRadiationFileName = basedir + "daily_sw_11243.dcx";

            IFeatureSet fs = FeatureSet.Open(PointFeatureFileName);
            string[] files = new string[] {AverageTemperatureFileName, MaxTemperatureFileName, MinTemperatureFileName, RelativeHumidityFileName,
             AirPressureFileName, WindSpeedFileName, NetRadiationFileName};
            var npt = fs.NumRows();
            Coordinate[] coors = new Coordinate[npt];
            for (int i = 0; i < npt; i++)
            {
                var geo_pt = fs.GetFeature(i).Geometry;
                coors[i] = geo_pt.Coordinate;
            }

            int nfile = files.Length;
            DataCubeStreamReader[] ass = new DataCubeStreamReader[nfile];
            My3DMat<float>[] mats = new My3DMat<float>[nfile];
            for (int i = 0; i < nfile; i++)
            {
                ass[i] = new DataCubeStreamReader(files[i]);
                ass[i].Open();
            }
            int progress = 0;
            int nstep = ass[0].NumTimeStep;
            int ncell = ass[0].FeatureCount;
            PenmanMonteithET pet = new PenmanMonteithET();

            DataCubeStreamWriter sw = new DataCubeStreamWriter(OutputFileName);
            sw.WriteHeader(new string[] { "pet"}, ncell);
            My3DMat<float> mat_out = new My3DMat<float>(1, 1, ncell);

            int count = 1;
            for (int t = 0; t < nstep; t++)
            {
                for (int i = 0; i < nfile; i++)
                {
                    mats[i] = ass[i].LoadStep();
                }
                for (int n = 0; n < ncell; n++)
                {
                    var tav = mats[0].Value[0][0][n];
                    var tmax = mats[1].Value[0][0][n];
                    var tmin = mats[2].Value[0][0][n];
                    if (InputTemperatureUnit == TemperatureUnit.Fahrenheit)
                    {
                        tmax = (float)UnitConversion.Fahrenheit2Kelvin(tmax);
                        tmin = (float)UnitConversion.Fahrenheit2Kelvin(tmin);
                        tav = (float)UnitConversion.Fahrenheit2Kelvin(tav);
                    }
                    else if (InputTemperatureUnit == TemperatureUnit.Celsius)
                    {
                        tmax = (float)UnitConversion.Celsius2Kelvin(tmax);
                        tmin = (float)UnitConversion.Celsius2Kelvin(tmin);
                        tav = (float)UnitConversion.Celsius2Kelvin(tav);
                    }
                    double shortwave = mats[6].Value[0][0][n] * 0.0864;
                    double ap = mats[4].Value[0][0][n] / 1000;
                    var et0 = pet.ET0(coors[n].Y, coors[n].X, tav, tmax, tmin,
                         mats[3].Value[0][0][n], ap, mats[5].Value[0][0][n], shortwave, Start.AddDays(t));

                    if (OutputLengthUnit == LengthUnit.inch)
                    {
                        mat_out.Value[0][0][n] = (float)System.Math.Round(et0 * UnitConversion.mm2Inch, 3);
                    }
                    else
                    {
                        mat_out.Value[0][0][n] = (float)System.Math.Round(et0, 3);
                    }
                }
                sw.WriteStep(1, ncell, mat_out.Value);
                progress = t * 100 / nstep;
                if (progress > count)
                {
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + (t + 1));
                    count++;
                }
            }

            sw.Close();
            for (int i = 0; i < nfile; i++)
            {
                ass[i].Close();
            }

            return true;
        }

        public bool Execute1(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            //(double tavrg, double tmax, double tmin, double rh, double airP, double wind2, double Rnet, DateTime day)

            string[] files = new string[] {AverageTemperatureFileName, MaxTemperatureFileName, MinTemperatureFileName, RelativeHumidityFileName,
             AirPressureFileName, WindSpeedFileName, NetRadiationFileName};

            int nfile = files.Length;
            DataCubeStreamReader[] ass = new DataCubeStreamReader[nfile];
            My3DMat<float>[] mats = new My3DMat<float>[nfile];
            for (int i = 0; i < nfile; i++)
            {
                ass[i] = new DataCubeStreamReader(files[i]);
                mats[i] = ass[i].Load();
            }
            int progress = 0;
            int nstep = mats[0].DateTimes.Length;
            int ncell = mats[0].Size[2];
            PenmanMonteithET pet = new PenmanMonteithET();

            var mat_out = new My3DMat<float>(1, nstep, ncell);
            mat_out.Name = "mat";
            mat_out.Variables = new string[] { "pet" };

            for (int t = 0; t < nstep;t++ )
            {
                for (int i = 0; i < ncell; i++)
                {
                    var tav = mats[0].Value[0][t][i];
                    var tmax = mats[1].Value[0][t][i];
                    var tmin = mats[2].Value[0][t][i];
                    if (InputTemperatureUnit == TemperatureUnit.Fahrenheit)
                    {
                        tmax = (float)UnitConversion.Fahrenheit2Kelvin(tmax);
                        tmin = (float)UnitConversion.Fahrenheit2Kelvin(tmin);
                        tav = (float)UnitConversion.Fahrenheit2Kelvin(tav);
                    }
                   var et0 =  pet.ET0(0,0,tav,tmax, tmin,
                        mats[3].Value[0][t][i], mats[4].Value[0][t][i], mats[5].Value[0][t][i], mats[6].Value[0][t][i], mats[0].DateTimes[t]);
                  // var et1 = (float)pet.ET0(40, 100, tav, tmax, tmin,
                  //mats[3].Value[0][t][i], mats[4].Value[0][t][i], mats[5].Value[0][t][i], 7, mats[0].DateTimes[t], 0.1);
                   if (et0 <= 0)
                       et0 = 0.1;
                   if (OutputLengthUnit == LengthUnit.inch)
                   {
                       mat_out.Value[0][t][i] = (float) System.Math.Round(et0 * UnitConversion.mm2Inch, 3);
                   }
                    else
                   {
                       mat_out.Value[0][t][i] = (float)System.Math.Round(et0, 3);
                   }
                }

                progress = t * 100 / nstep;
                if (progress % 5 == 0 && progress >0)
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + (t + 1));
            }
            mat_out.DateTimes = mats[0].DateTimes;
            Workspace.Add(mat_out);

                return true;

        }
    }
}
