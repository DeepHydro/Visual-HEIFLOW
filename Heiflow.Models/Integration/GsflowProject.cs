// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Heiflow.Models.Integration
{
     [Serializable]
     [Export(typeof(IProject))]
    public class GsflowProject:HeiflowProject
    {
        public GsflowProject()
        {
            this.Name = "GSFLOW Project";
            this.NameToShown = "GSFLOW";
            this.Icon = Resources.RasterImageAnalysisDifference16;
            this.LargeIcon = Resources.RasterImageAnalysisDifference32;
            Description = "GSFLOW model version 1.1.6";
            Token = "GSFLOW";
            ModelExeFileName = Path.Combine(Application.StartupPath, "Models\\gsflow.exe");
        }
    }
}
