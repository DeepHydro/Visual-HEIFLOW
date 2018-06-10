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

using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Atmosphere;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Hydrology
{
    public class TemperatureConversion : ModelTool
    {
        public TemperatureConversion()
        {
            Name = "Temperature Conversion";
            Category = "Data Management";
            Description = "Convert Temperature between diffent units";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            InputTemperatureUnit = TemperatureUnit.Kelvin;
            OutputTemperatureUnit = TemperatureUnit.Fahrenheit;
        }


        [Category("Input")]
        [Description("The full file name of the dcx file")]
        public string InputFileName { get; set; }
        [Category("Output")]
        [Description("The full file name of the dcx file")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName { get; set; }

        [Category("Parameter")]
        [Description("The Unit of the  Output Temperature")]
        public TemperatureUnit OutputTemperatureUnit { get; private set; }

        [Category("Parameter")]
        [Description("The Unit of the  Input Temperature")]
        public TemperatureUnit InputTemperatureUnit { get; set; }

        public override void Initialize()
        {
            Initialized = File.Exists(InputFileName) && !TypeConverterEx.IsNull(OutputFileName);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            DataCubeStreamReader ass = new DataCubeStreamReader(InputFileName);
            DataCubeStreamWriter dcw = new DataCubeStreamWriter(OutputFileName);
            ass.Open();
            dcw.WriteHeader(ass.Variables, ass.FeatureCount);

            if (InputTemperatureUnit == TemperatureUnit.Kelvin)
            {
                for (int t = 0; t < ass.NumTimeStep; t++)
                {
                    var mat = ass.LoadStep();
                    for (int i = 0; i < mat.Size[2]; i++)
                    {
                        mat[0, 0, i] = UnitConversion.Kelvin2Fahrenheit(mat[0, 0, i]);
                    }
                    dcw.WriteStep(1, ass.FeatureCount, mat);
                }
            }
            else if (InputTemperatureUnit == TemperatureUnit.Celsius)
            {
                for (int t = 0; t < ass.NumTimeStep; t++)
                {
                    var mat = ass.LoadStep();
                    for (int i = 0; i < mat.Size[2]; i++)
                    {
                        mat[0, 0, i] = UnitConversion.Celsius2Fahrenheit(mat[0, 0, i]);
                    }
                    dcw.WriteStep(1, ass.FeatureCount, mat);
                }
            }
            ass.Close();
            dcw.Close();
            return true;
        }

    }
}
