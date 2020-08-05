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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Math
{
    public class PMPETByRadiationBatch : ModelTool
    {
        private string _ValueField = "";
        private int _SelectedVarAlbedoIndex = -1;
        private string _FeatureFileName;
        private IFeatureSet _FeatureSet;

        public PMPETByRadiationBatch()
        {
            Name = "Penman Monteith PET Batch By Radiation";
            Category = "Hydrology";
            Description = "Calculate PET based on FAO P-M Model using radiation.";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            InputTemperatureUnit = TemperatureUnit.Fahrenheit;
            OutputLengthUnit = LengthUnit.inch;
        }

        [Category("Input")]
        [Description("The content of the input file contains filenames of the following variables: AvT, Relative Humidity,Air Pressure, Winde Speed, Net Radiation")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string InputFileList { get; set; }

        [Category("Parameter")]
        [Description("The Unit of the  Input Temperature")]
        public TemperatureUnit InputTemperatureUnit { get; set; }

        [Category("Parameter")]
        public LengthUnit OutputLengthUnit { get; set; }

        [Category("Output")]
        [Description("The name of the PET output file")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PETOutputFileName { get; set; }

        public override void Initialize()
        {
            Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamReader sr_list = new StreamReader(InputFileList);
            string[] files = new string[5];
            for (int i = 0; i < 5; i++)
            {
                files[i] = sr_list.ReadLine();
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

            DataCubeStreamWriter sw_pet = new DataCubeStreamWriter(PETOutputFileName);
            sw_pet.WriteHeader(new string[] { "pet" }, ncell);
            DataCube<float> pet_out = new DataCube<float>(1, 1, ncell);
            int count = 1;
 
            for (int t = 0; t < nstep; t++)
            {
                for (int i = 0; i < nfile; i++)
                {
                    mats[i] = ass[i].LoadStep();
                }
                for (int n = 0; n < ncell; n++)
                {
                    var tav = mats[0][0, 0, n];
                    if (InputTemperatureUnit == TemperatureUnit.Fahrenheit)
                    {
                        tav = (float)UnitConversion.Fahrenheit2Kelvin(tav);
                    }
                    else if (InputTemperatureUnit == TemperatureUnit.Celsius)
                    {
                        tav = (float)UnitConversion.Celsius2Kelvin(tav);
                    }
                    //AvT, Relative Humidity,Air Pressure, Winde Speed, Net Radiation
                    var et0 = pet.ET0(tav, mats[1][0, 0, n], mats[2][0, 0, n], mats[3][0, 0, n], mats[4][0, 0, n]);
                    if (OutputLengthUnit == LengthUnit.inch)
                    {
                        pet_out[0,0,n] = (float)(et0 * UnitConversion.mm2Inch);
                    }
                    else
                    {
                        pet_out[0, 0, n] = (float)et0;
                    }

                }
                sw_pet.WriteStep(1, ncell, pet_out);

                progress = t * 100 / nstep;
                if (progress > count)
                {
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + (t + 1));
                    count++;
                }
            }

            sw_pet.Close();
            for (int i = 0; i < nfile; i++)
            {
                ass[i].Close();
            }

            return true;
        }
    }
}