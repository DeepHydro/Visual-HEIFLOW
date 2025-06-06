﻿//
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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Math
{
    public class PMPET6Batch : ModelTool
    {
        private string _ValueField = "";
        private int _SelectedVarAlbedoIndex = -1;
        private string _FeatureFileName;
        private IFeatureSet _FeatureSet;

        public PMPET6Batch()
        {
            Name = "Penman Monteith PET Batch";
            Category = "Hydrology";
            Description = "Calculate PET based on FAO P-M Model using 6 meterological variables.";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            InputTemperatureUnit = TemperatureUnit.Fahrenheit;
            OutputLengthUnit = LengthUnit.inch;
            CloudCover = 0.15;
            MaxTimeStep = 0;
            DefaultAlbedo = 0.23;
        }
        [Category("Input")]
        [Description("The shpfile name")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PointFeatureFileName
        {
            get
            {
                return _FeatureFileName;
            }
            set
            {
                _FeatureFileName = value;
                if (File.Exists(_FeatureFileName))
                {
                    _FeatureSet = FeatureSet.Open(_FeatureFileName);
                    var buf = from DataColumn dc in _FeatureSet.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }

        [Category("Input")]
        [Description("The content of the input file contains filenames of the following variables: AvT, MaxT, MinT, RelativeHumidity,AirPressure, WindSpeed")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string InputFileList { get; set; }

        [Category("Parameter")]
        [Description("Name of the variable")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string AlbedoField
        {
            get
            {
                return _ValueField;
            }
            set
            {
                _ValueField = value;
                if (Fields != null)
                {
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        if (_ValueField == Fields[i])
                        {
                            _SelectedVarAlbedoIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
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
        [Category("Parameter")]
        public int MaxTimeStep
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("The name of the PET output file")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PETOutputFileName { get; set; }

        [Category("Output")]
        [Description("Output short-wave and long-wave radiaton in seperated files")]
        public bool OutputRadiation { get; set; }

        [Category("Output")]
        [Description("The name of the short-wave radiation output file")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ShortWaveRadOutputFileName { get; set; }

        [Category("Output")]
        [Description("The name of the long-wave radiation output file")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LongWaveRadOutputFileName { get; set; }



        public override void Initialize()
        {
            Initialized = true;
            //string[] files = new string[] {AverageTemperatureFileName, MaxTemperatureFileName, MinTemperatureFileName, RelativeHumidityFileName,
            // AirPressureFileName, WindSpeedFileName};
            //Initialized = !files.Contains(null);
            //if (!TypeConverterEx.IsNull(PointFeatureFileName) && File.Exists(PointFeatureFileName))
            //{
            //    Initialized = false;
            //}
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamReader sr_list = new StreamReader(InputFileList);
            string[] files = new string[6];
            for (int i = 0; i < 6; i++)
            {
                files[i] = sr_list.ReadLine();
            }

            IFeatureSet fs = FeatureSet.Open(PointFeatureFileName);
          
            var npt = fs.NumRows();
            Coordinate[] coors = new Coordinate[npt];
            double[] albedo_list = new double[npt];
            double albedo = 0.23;
            for (int i = 0; i < npt; i++)
            {
                var geo_pt = fs.GetFeature(i).Geometry;
                coors[i] = geo_pt.Coordinate;
                if (_SelectedVarAlbedoIndex >= 0)
                {
                    double.TryParse(fs.DataTable.Rows[i][_SelectedVarAlbedoIndex].ToString(), out albedo);
                    albedo_list[i] = albedo;
                    if(albedo_list[i] <0 || albedo_list[i] > 1)
                        albedo_list[i] = DefaultAlbedo;
                }
                else
                {
                    albedo_list[i] = DefaultAlbedo;
                }
            }

            int nfile = files.Length;
            DataCubeStreamReader[] ass = new DataCubeStreamReader[nfile];
            DataCube<float>[] mats = new DataCube<float>[nfile];
            for (int i = 0; i < nfile; i++)
            {
                ass[i] = new DataCubeStreamReader(files[i]);
                ass[i].Open();
            }
            int progress = 0;
            int nstep = ass[0].NumTimeStep;
            int ncell = ass[0].FeatureCount;
            PenmanMonteithET pet = new PenmanMonteithET();

            DataCubeStreamWriter sw_shortrd = null;
            DataCubeStreamWriter sw_longrd = null;
            DataCube<float> swr_out = null;
            DataCube<float> lwr_out = null;
            DataCubeStreamWriter sw_pet = new DataCubeStreamWriter(PETOutputFileName);
            sw_pet.WriteHeader(new string[] { "pet" }, ncell);
            DataCube<float> pet_out = new DataCube<float>(1, 1, ncell);

            if(OutputRadiation)
            {
                sw_shortrd = new DataCubeStreamWriter(ShortWaveRadOutputFileName);
                sw_shortrd.WriteHeader(new string[] { "swr" }, ncell);
                swr_out = new DataCube<float>(1, 1, ncell);

                sw_longrd = new DataCubeStreamWriter(LongWaveRadOutputFileName);
                sw_longrd.WriteHeader(new string[] { "lwr" }, ncell); 
                lwr_out = new DataCube<float>(1, 1, ncell);
            }

            if (MaxTimeStep <= 0)
                nstep = ass[0].NumTimeStep;
            else
                nstep = System.Math.Min(MaxTimeStep, ass[0].NumTimeStep);
            int count = 1;
            double short_rad = 0;
            double long_rad = 0;
 
            for (int t = 0; t < nstep; t++)
            {
                for (int i = 0; i < nfile; i++)
                {
                    mats[i] = ass[i].LoadStep();
                }
                for (int n = 0; n < ncell; n++)
                {
                    var tav = mats[0][0, 0, n];
                    var tmax = mats[1][0, 0, n];
                    var tmin = mats[2][0, 0, n];
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
                    var et0 = pet.ET0(coors[n].Y, coors[n].X, tav, tmax, tmin,
                         mats[3][0, 0, n], mats[4][0, 0, n], mats[5][0, 0, n], Start.AddDays(t), CloudCover, albedo_list[n], ref short_rad, ref long_rad);

                    if (OutputLengthUnit == LengthUnit.inch)
                    {
                        pet_out[0,0,n] = (float)(et0 * UnitConversion.mm2Inch);
                    }
                    else
                    {
                        pet_out[0, 0, n] = (float)et0;
                    }
                    if (OutputRadiation)
                    {
                        swr_out[0, 0, n] = (float)short_rad;
                        lwr_out[0, 0, n] = (float)long_rad;
                    }
                }
                sw_pet.WriteStep(1, ncell, pet_out);
                if(OutputRadiation)
                {
                    sw_shortrd.WriteStep(1, ncell, swr_out);
                    sw_longrd.WriteStep(1, ncell, lwr_out);
                }
                progress = t * 100 / nstep;
                if (progress > count)
                {
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + (t + 1));
                    count++;
                }
            }

            sw_pet.Close();
            if (OutputRadiation)
            {
                sw_shortrd.Close();
                sw_longrd.Close();
            }
            for (int i = 0; i < nfile; i++)
            {
                ass[i].Close();
            }

            return true;
        }
    }
}