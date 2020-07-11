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
    public class CalibrateElevTool : MapLayerRequiredTool
    {
        public CalibrateElevTool()
        {
            Name = "Calibrate Elevation Based On Surf Leakage";
            Category = Cat_CMG;
            SubCategory = "DIS";
            Description = "Calibrate Elevation Based On Surf Leakage";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            Threashhold = 100000000;
            Average = false;
            ElevOffset = 10;

        }

        [Category("Input")]
        [Description("The surface leakage data cube. The Data Cube style should be mat[0][0][:]")]
        public string SurfLeakageDataCube
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("The threashhold value used to determine which IBound would be modified.")]
        public double Threashhold
        {
            get;
            set;
        }
        public float ElevOffset
        {
            get;
            set;
        }
        public bool Average
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
            var bas = prj.Project.Model.GetPackage(BASPackage.PackageName) as BASPackage;
            var vec_src = GetVector(SurfLeakageDataCube);
            string msg = "";
            int count = 0;
            if (vec_src != null && bas != null)
            {
                for (int i = 0; i < vec_src.Length; i++)
                {
                    if (System.Math.Abs(vec_src[i]) > System.Math.Abs(Threashhold))
                    {
                        var loc = grid.Topology.ActiveCellLocation[i];
                        var hruid = grid.Topology.GetSerialIndex(loc[0], loc[1]);
                        msg = string.Format("The IBOUND at Cell[{0},{1}] is modified.", loc[0] + 1, loc[1] + 1);
                        if (Average)
                        {
                            var top = grid.GetElevationAt(loc[0], loc[1], 0);
                            var thickness = new float[grid.ActualLayerCount];
                            for (int j = 0; j < grid.ActualLayerCount; j++)
                            {
                                thickness[j] = grid.GetElevationAt(loc[0], loc[1], j) - grid.GetElevationAt(loc[0], loc[1], j + 1);
                            }
                            float elevbuf = 0;
                            int num = 0;
                            for (int r = -1; r <= 1; r++)
                            {
                                for (int c = -1; c <= 1; c++)
                                {
                                    if (r != 0 && c != 0 && grid.IBound[0, loc[0] + r, loc[1] + c] > 0)
                                    {
                                        elevbuf += grid.GetElevationAt(loc[0] + r, loc[1] + c, 0);
                                        num++;
                                    }
                                }
                            }
                            elevbuf /= num;
                            grid.Elevations[0, 0, hruid] = elevbuf;
                            for (int j = 1; j <= grid.ActualLayerCount; j++)
                            {
                                grid.Elevations[j, 0, hruid] = elevbuf - thickness[j - 1];
                            }
                            for (int j = 0; j < grid.ActualLayerCount; j++)
                            {
                                thickness[j] = grid.GetElevationAt(loc[0], loc[1], j) - grid.GetElevationAt(loc[0], loc[1], j + 1);
                            }
                        }
                        else
                        {
                            for (int j = 0; j <= grid.ActualLayerCount; j++)
                            {
                                grid.Elevations[j, 0, hruid] += ElevOffset;
                            }
                        }
                        cancelProgressHandler.Progress("tool", i / vec_src.Length * 100, msg);
                        count++;
                    }
                }
                msg = string.Format("Total number of modified cells is: {0}", count);
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