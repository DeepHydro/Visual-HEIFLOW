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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using System.IO;
using Heiflow.Models.Generic;
using System.ComponentModel;
using Heiflow.Controls.WinForm.Editors;
using System.Windows.Forms.Design;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Presentation.Services;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Integration;
using Heiflow.Models.Tools;

namespace Heiflow.Tools.DataManagement
{
    public class Resample : MapLayerRequiredTool
    {
        public Resample()
        {
            Name = "Resample by spatial map";
            Category = "Conversion";
            Description = @"Resample data cube by considering HRU spatial mapping table.";
            Version = "1.0.0.0";
            OutputDataCubeName = "res";
            this.Author = "Yong Tian";
        }


        [Category("Input")]
        [Description("The name of the datacube being exported. The expression of Source should be mat[0][:][:]")]
        public string Source
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("The name of exported variable")]
        public string OutputDataCubeName
        {
            get;
            set;
        }
        [Category("Input Files")]
        [Description("The HRU Map filename. The file contains nhru+1 rows. The first line is a head line, then followed by CellID, HRUID. The CellID is the ID in the DataCuube")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string HRUMapFileName
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var mat = Get3DMat(Source);
            Initialized = mat != null;
            if (TypeConverterEx.IsNull(HRUMapFileName))
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            var var_index = 0;
            var mat = Get3DMat(Source,ref var_index);
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            var dic = LoadMapTable();
            if (mf != null)
            {
                var ntime = mat.Size[1];
                var grid = mf.Grid as RegularGrid;
                var outdc = new DataCube<float>(1, ntime, grid.ActiveCellCount);
                outdc.Name = OutputDataCubeName;

                for (int t = 0; t < ntime; t++)
                {
                    for (int i = 0; i < grid.ActiveCellCount; i++)
                    {
                        outdc[0, t, i] = mat[var_index, t, dic[i + 1] - 1];
                    }
                    progress = t * 100 / ntime;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing step: " + t);
                        count++;
                    }
                }
                Workspace.Add(outdc);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }
        }


        private Dictionary<int, int> LoadMapTable()
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            StreamReader sr = new StreamReader(HRUMapFileName);
            var line = sr.ReadLine();
            line = sr.ReadLine();
            var bufs = TypeConverterEx.Split<int>(line, 2);
            var nhru = bufs[1];
            for (int i = 0; i < nhru; i++)
            {
                line = sr.ReadLine();
                bufs = TypeConverterEx.Split<int>(line, 2);
                dic.Add(bufs[1], bufs[0]);
            }
            sr.Close();
            return dic;
        }
    }
}