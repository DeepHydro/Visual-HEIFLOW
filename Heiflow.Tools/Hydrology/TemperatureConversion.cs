// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
                        mat.Value[0][0][i] = UnitConversion.Kelvin2Fahrenheit(mat.Value[0][0][i]);
                    }
                    dcw.WriteStep(1, ass.FeatureCount, mat.Value);
                }
            }
            else if (InputTemperatureUnit == TemperatureUnit.Celsius)
            {
                for (int t = 0; t < ass.NumTimeStep; t++)
                {
                    var mat = ass.LoadStep();
                    for (int i = 0; i < mat.Size[2]; i++)
                    {
                        mat.Value[0][0][i] = UnitConversion.Celsius2Fahrenheit(mat.Value[0][0][i]);
                    }
                    dcw.WriteStep(1, ass.FeatureCount, mat.Value);
                }
            }
            ass.Close();
            dcw.Close();
            return true;
        }

    }
}
