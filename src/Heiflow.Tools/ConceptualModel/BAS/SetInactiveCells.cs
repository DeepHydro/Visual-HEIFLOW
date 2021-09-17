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
using System.Windows.Forms.Design;

namespace Heiflow.Tools.ConceptualModel
{
    public class SetInactiveCells : MapLayerRequiredTool
    {
        public SetInactiveCells()
        {
            Name = "Set Inactive Cells";
            Category = Cat_CMG;
            SubCategory = "BAS6";
            Description = "Set Inactive Cells";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }


        [Category("Input")]
        [Description("The filename of storing inactive cell information, including two columns: Row ID and Col ID. The ID starts from 1. A header line is necessary.")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string InactiveCellInfoFileName
        {
            get;
            set;
        }

        public override void Initialize()
        {
            Initialized = TypeConverterEx.IsNotNull(InactiveCellInfoFileName);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var bas = prj.Project.Model.GetPackage(BASPackage.PackageName) as BASPackage;
            string msg = "";
            int count = 0;
            if (File.Exists(InactiveCellInfoFileName))
            {
                StreamReader sr = new StreamReader(InactiveCellInfoFileName);
                var line = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                    {
                        var bufs = TypeConverterEx.Split<int>(line);
                        for (int j = 0; j < grid.ActualLayerCount; j++)
                        {
                            grid.MFIBound[j, bufs[0] - 1, bufs[1] - 1] = 0;
                        }
                        msg = string.Format("The IBOUND at Cell[{0},{1}] is modified.", bufs[0], bufs[1]);
                        cancelProgressHandler.Progress("tool", 50, msg);
                        count++;
                    }
                }
                sr.Close();


                msg = string.Format("Total number of modified cells is: {0}", count);
                cancelProgressHandler.Progress("tool", 100, msg);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed. The specified file dose not exist.");
                return false;
            }
        }
    }
}