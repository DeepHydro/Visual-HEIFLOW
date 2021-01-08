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

namespace Heiflow.Tools.Conversion
{
    public class SFRToRasterList : MapLayerRequiredTool
    {
        public enum FilterMode { Maximum, Minimum, None }
        public enum SFRVariablesAbbrv
        {
            FlowIn = 0, FlowLoss = 1, FlowOut = 2, Runoff = 3, RiverRain = 4,
            RiverET = 5, RiverHead = 6, RiverDepth = 7, RiverWidth = 8, RivConduct = 9, FlowToGW = 10, UnsatStor = 11, GWHead = 12
        };
        public SFRToRasterList()
        {
            Name = "SFR To Raster List";
            Category = "Conversion";
            SubCategory = "Raster";
            Description = "Convert SFR results  to raster file with format of TIF";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            DateFormat = "yyyy-MM-dd";
            VariableName = "flow";
            Direcotry = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //TimeInteval = 86400;
            //Start = new DateTime(2000, 1, 1);
            SFRVariable = SFRVariablesAbbrv.FlowOut;
            ScaleFactor = 1 / 86400.0f;
            MaxTimeStep = 10000;
        }
        private IFeatureSet _grid_layer;

        [Category("Input")]
        [Description("Model grid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The name of exported variable")]
        public string VariableName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The name of exported variable")]
        public SFRVariablesAbbrv SFRVariable
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The file folder directory for output")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Direcotry
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Specify the date format used to generate the output files automatically. Its style should be yyyy-MM-dd or yyyy-MM")]
        public string DateFormat
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Scale factor applied to the loaded variable")]
        public float ScaleFactor
        {
            get;
            set;
        }
        //[Category("Optional")]
        //[Description("Specify the start date time.")]
        //public DateTime Start
        //{
        //    get;
        //    set;
        //}

        //[Category("Optional")]
        //[Description("Specify the time inteval in the unit of seconds.")]
        //public int TimeInteval
        //{
        //    get;
        //    set;
        //}
        [Category("Parameter")]
        [Description("Maximum step to load")]
        public int MaxTimeStep
        {
            get;
            set;
        }
        public override void Initialize()
        {
            Initialized = true;
            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            if (_grid_layer == null)
            {
                this.Initialized = false;
            }

            if (TypeConverterEx.IsNull(Direcotry))
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                var grid = mf.Grid as MFGrid;
                var sfr_pck = mf.GetPackage(SFRPackage.PackageName) as SFRPackage;
                var sfrout_pck = mf.GetPackage(SFROutputPackage.PackageName) as SFROutputPackage;
                sfrout_pck.Scan();

                FileStream fs = new FileStream(sfrout_pck.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                try
                {
                    string line = "";
                    int varLen = sfrout_pck.DefaultAttachedVariables.Length;
                    int var_index = (int)SFRVariable;
                    var network = sfr_pck.RiverNetwork;
                    var nstep = sfrout_pck.StepsToLoad;
                    if (MaxTimeStep < nstep)
                        nstep = MaxTimeStep;
                    var vec = new float[network.ReachCount];
                    for (int t = 0; t < nstep; t++)
                    {
                        for (int c = 0; c < 8; c++)
                            sr.ReadLine();
                        var rchindex = 0;
                        for (int i = 0; i < network.RiverCount; i++)
                        {
                            for (int j = 0; j < network.Rivers[i].Reaches.Count; j++)
                            {
                                line = sr.ReadLine();
                                var temp = TypeConverterEx.SkipSplit<float>(line, 5);
                                vec[rchindex] = temp[var_index] * ScaleFactor;
                                rchindex++;
                            }
                        }
                        var Filename = Path.Combine(Direcotry, string.Format("{0}_{1}.tif", VariableName, sfr_pck.TimeService.Timeline[t].ToString(DateFormat)));
                        var raster = Raster.CreateRaster(Filename, string.Empty, grid.Topology.ColumnCount, grid.Topology.RowCount, 1, typeof(float), new[] { string.Empty });
                        raster.NoDataValue = -9999;
                        raster.Bounds = new RasterBounds(grid.Topology.RowCount, grid.Topology.ColumnCount, new Extent(grid.BBox));
                        raster.Projection = _grid_layer.Projection;
                        for (int i = 0; i < network.ReachCount; i++)
                        {
                            var loc = sfr_pck.ReachTopology.ActiveCellLocation[i]; 
                            raster.Value[loc[0], loc[1]] = vec[i];
                        }
                        raster.Save();

                        progress = t * 100 / nstep;
                        if (progress > count)
                        {
                            cancelProgressHandler.Progress("TOOLS", progress, "Processing time step:" + sfr_pck.TimeService.Timeline[t]);
                            count++;
                        }
                    }
                }
                catch (Exception ex)
                {
                   var msg = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                   cancelProgressHandler.Progress("TOOLS", 100, "Error message: " + msg);
                }
                finally
                {
                    sr.Close();
                    fs.Close();
                }

                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }
        }

    }
}