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
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Core.Hydrology;
using Heiflow.Core.MyMath;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;

namespace Heiflow.Tools.ConceptualModel
{
    public class CorrectReachVKTool  : MapLayerRequiredTool
    {
        public CorrectReachVKTool()
        {
            Name = "Correct Reach VK By Surface Leakage";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Correct SFR VK By Surface Leakage. If the absolute value of Surface Leakage is greater than the Threashhold at a reach, VK of the reach will be set to the specified VK";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            ModifiedVK = 0;
            Threashhold = 100000000;
        }

        [Category("Input")]
        [Description("The surface leakage data cube. The Data Cube style should be mat[0][0][:]")]
        public string SurfLeakageDataCube
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The threashhold value used to determine which SFR reaches would be modified.")]
        public double Threashhold
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The modified VK value")]
        public double ModifiedVK
        {
            get;
            set;
        }
        public override void Initialize()
        {
            Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
            var dis = prj.Project.Model.GetPackage(DISPackage.PackageName) as DISPackage;
            var vec_src = GetVector(SurfLeakageDataCube);
            string msg = "";
            int count = 0;
            if (vec_src != null && sfr != null)
            {
                var rvnet = sfr.RiverNetwork;
                for (int i = 0; i < vec_src.Length;i++ )
                {
                    if (System.Math.Abs(vec_src[i]) > System.Math.Abs(Threashhold))
                    {
                        var loc = grid.Topology.ActiveCellLocation[i];
                        var rches = rvnet.GetReachByLocation(loc[0] + 1, loc[1] + 1);
                        if (rches != null)
                        {
                            foreach (var rch in rches)
                            {
                                rch.STRHC1 = this.ModifiedVK;
                                msg = string.Format("The reach at {0},{1} is modified.", loc[0] + 1, loc[1] + 1);
                                cancelProgressHandler.Progress("tool", i / vec_src.Length * 100, msg);
                                count++;
                            }
                        }
                    }
                }
                sfr.NetworkToMat();
                msg = string.Format("Total number of modified reaches is: {0}", count);
                cancelProgressHandler.Progress("tool", 100, msg);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed. The source or target matrix style is wrong.");
                return false;
            }
        }
    }
}