// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
     [Serializable]
    [Export(typeof(IProject))]
    public class SusbedProject : BaseProject
    {
         public SusbedProject()
        {
            this.Name = "Susbed Project";
            this.NameToShown = "Susbed";
            Description = "一维非恒定流水沙模型";
            Token = "Susbed";
        }

        protected override string ControlFileExtension
        {
            get { return ".sus"; }
        }

        protected override IBasicModel CreateModel(string controlFileName)
        {
            return new Susbed()
            {
                Project = this,
                WorkDirectory = FullModelWorkDirectory,
                ControlFileName = controlFileName
            };
        }

        public override void AttachFeatures()
        {
            // Susbed is one dimensional, it has neither a grid nor a centroid feature to attach.
        }

        public override void CreateGridFeature()
        {
            // Susbed is one dimensional, no grid feature is created.
        }
    }
}
