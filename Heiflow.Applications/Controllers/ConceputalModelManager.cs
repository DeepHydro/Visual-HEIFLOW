// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Symbology;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GeoSpatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Applications.Controllers
{
    public class ConceputalModelManager
    {
        public ConceputalModelManager()
        {

        }

        public void OnProjectOpened(IMap Map, IProject Project)
        {
            var pcks = Project.Model.GetPackages();
          
            foreach (var lp in Project.FeatureCoverages)
            {
                var fea = MapHelper.SelectDataSet(lp.CoverageFilePath, Map, Project.AbsolutePathToProjectFile);
                if (fea != null)
                {
                    lp.Source = fea;
                }
                //MapHelper.Select()
                var pck = from pp in pcks where pp.Name == lp.PackageName select pp;
                if (pck.Count() == 1)
                    lp.Package = pck.First();

                var paras = lp.Package.GetParameters();
                foreach (var ap in lp.ArealProperties)
                {
                    if (ap.IsParameter)
                    {
                        ap.Parameter = (from pp in paras where pp.Name == ap.ParameterName select pp).First();
                    }
                }
            }
            foreach (var lp in Project.RasterLayerCoverages)
            {
                var ras = MapHelper.SelectDataSet(lp.CoverageFilePath, Map, Project.AbsolutePathToProjectFile);
                if (ras != null)
                {
                    lp.Source = ras;
                }

                var pck = from pp in pcks where pp.Name == lp.PackageName select pp;
                if (pck.Count() == 1)
                    lp.Package = pck.First();

                var paras = lp.Package.GetParameters();
                foreach (var ap in lp.ArealProperties)
                {
                    if (ap.IsParameter)
                    {
                        ap.Parameter = (from pp in paras where pp.Name == ap.ParameterName select pp).First();
                    }
                }
            }
        }
    
    }
}
