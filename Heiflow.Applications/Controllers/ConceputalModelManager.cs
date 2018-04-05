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
