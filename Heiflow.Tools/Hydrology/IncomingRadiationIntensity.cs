using Heiflow.Controls.WinForm.Editors;
using Heiflow.Models.Atmosphere;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.IO;
using Heiflow.Core.Data;
using Heiflow.Core.IO;

namespace Heiflow.Tools.Hydrology
{
    public class IncomingRadiationIntensity : ModelTool
    {
        private ShortWaveRadiation _ShortWaveRadiation = new ShortWaveRadiation(0, 0);
        public IncomingRadiationIntensity()
        {
            Name = "Incoming Radiation Intensity";
            Category = "Hydrology";
            Description = "Calculate Incoming Radiation Intensity.";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The content of the CSV file contains two columns: [longitude,latitude], each line represents a location")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LocationFileName { get; set; }

        [Category("Input")]
        [Description("The content of the CSV file contains one column: [datetime], each line represents a day")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DateTimeFileName { get; set; }

        [Category("Input")]
        [Description("The content of the DCX file contains a data cube: [1][nday][nlocation]")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CloudCoverFileName { get; set; }
        [Category("OutputName")]
        [Description("The name of the output data cube")]
        public string OutputName { get; set; }

        public override void Initialize()
        {
            if (TypeConverterEx.IsNull(DateTimeFileName) || TypeConverterEx.IsNull(CloudCoverFileName) || TypeConverterEx.IsNull(LocationFileName))
            {
                Initialized = false;
                return;
            }
            Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            DataCubeStreamReader ds = new DataCubeStreamReader(CloudCoverFileName);
            var cloudcover = ds.Load();
            CSVFileStream locationcsv = new CSVFileStream(LocationFileName);
            var locations = locationcsv.Load<double>();
            StreamReader sr_dates = new StreamReader(DateTimeFileName);
            int ndays = cloudcover.Size[1];
            int nlocation = cloudcover.Size[2];
            var dates = new DateTime[ndays];
            int progress = 0;
            int np=1;
            var swmat = new My3DMat<float>(cloudcover.Size[0], cloudcover.Size[1], cloudcover.Size[2]);
            swmat.Name = OutputName;
            for (int i = 0; i < ndays; i++)
            {
                var line = sr_dates.ReadLine();
                dates[i] = DateTime.Parse(line);
            }
            sr_dates.Close();
            for (int i = 0; i < ndays; i++)
            {
                for (int j = 0; j < nlocation; j++)
                {
                    swmat.Value[0][i][j] = (float)_ShortWaveRadiation.DailyIncomingRadiationIntensity(locations.GetValue(j, 1), locations.GetValue(j, 0), dates[i], cloudcover.Value[0][i][j]);
                }
                progress = i * 100 / ndays;
                if (progress == 5 * np)
                {
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step: " + progress + "%");
                    np++;
                }
            }
            Workspace.Add(swmat);
            return true;
        }
    }
}
