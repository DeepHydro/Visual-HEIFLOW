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
    public enum ConductivitySource { HK, VK };
    public class SetSFRVKTool : MapLayerRequiredTool
    {
        public SetSFRVKTool()
        {
            Name = "Set SFR VK by LPF Value";
            Category = "Conceptual Model";
            Description = "Set SFR VK by LPF Value";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            ScaleFactor = 1;
            ConductivitySource = ConceptualModel.ConductivitySource.HK;
        }
        [Category("Parameter")]
        [Description("The scale factor applied to the original value.")]
        public float ScaleFactor
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("The scale factor applied to the original value.")]
        public ConductivitySource ConductivitySource
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
            var lpf = prj.Project.Model.GetPackage(LPFPackage.PackageName) as LPFPackage;
            if (sfr != null)
            {
                var rvnet = sfr.RiverNetwork;
                foreach (var river in rvnet.Rivers)
                {
                    foreach (Reach reach in river.Reaches)
                    {
                        var index = grid.Topology.GetSerialIndex(reach.IRCH - 1, reach.JRCH - 1);
                        if (ConductivitySource == ConceptualModel.ConductivitySource.HK)
                            reach.STRHC1 = lpf.HK[0, 0, index] * ScaleFactor;
                        else
                            reach.STRHC1 = lpf.HK[0, 0, index] / lpf.VKA[0, 0, index] * ScaleFactor;
                    }
                }
                sfr.NetworkToMat();
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
                return false;
            }

        }
    }
}
