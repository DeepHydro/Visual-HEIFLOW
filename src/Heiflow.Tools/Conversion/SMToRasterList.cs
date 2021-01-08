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
using Heiflow.Core.IO;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Models.Generic.Parameters;

namespace Heiflow.Tools.Conversion
{
    public class SMToRasterList : MapLayerRequiredTool
    {
        public enum FilterMode { Maximum, Minimum, None }
        public SMToRasterList()
        {
            Name = "Soil moisture To raster list";
            Category = "Conversion";
            SubCategory = "Raster";
            Description = "Convert soil moisture  to raster file with format of tif";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            DateFormat = "yyyy-MM-dd";
            Direcotry = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TimeInteval = 86400;
            Start = new DateTime(2000, 1, 1);
            Scale = 1;
            Maximum = 1;
        }
        private IFeatureSet _grid_layer;
        private string _InputFileName;
        private string _ValueField;
        private int _SelectedVarIndex;

        [Category("Input")]
        [Description("The input soil moisture filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string SoilMoistureFileName
        {
            get
            {
                return _InputFileName;
            }
            set
            {
                _InputFileName = value;
                DataCubeStreamReader dr = new DataCubeStreamReader(_InputFileName);
                var info = dr.GetFileInfo();
                Variables = info.VariableNames;
                TotalTimeStepNum = info.TotalTimeSteps / 3;
                CellNumber = info.CellNum;
                for (int i = 0; i < info.VariableNum; i++)
                {
                    Variables[i] = "sm_layer_" + (i + 1);
                }
            }
        }

        [Category("Input")]
        [Description("Model grid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }

        [Browsable(false)]
        public string[] Variables
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Variable to be exported")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Variables")]
        public string VariableName
        {
            get
            {
                return _ValueField;
            }
            set
            {
                _ValueField = value;
                if (Variables != null)
                {
                    for (int i = 0; i < Variables.Length; i++)
                    {
                        if (_ValueField == Variables[i])
                        {
                            _SelectedVarIndex = i;
                            break;
                        }
                    }
                }
            }
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

        [Category("Optional")]
        [Description("Specify the start date time.")]
        public DateTime Start
        {
            get;
            set;
        }

        [Category("Optional")]
        [Description("Specify the time inteval in the unit of seconds.")]
        public int TimeInteval
        {
            get;
            set;
        }

        [Category("Optional")]
        [Description("Scale factor applied to the data")]
        public float Scale
        {
            get;
            set;
        }
        [Category("Optional")]
        [Description("Maximum value")]
        public float Maximum
        {
            get;
            set;
        }
        [Category("InputFile Info")]
        [Description("Total time steps")]
        public int TotalTimeStepNum
        {
            get;
            private set;
        }

        [Category("InputFile Info")]
        [Description("Cell number")]
        public int CellNumber
        {
            get;
            private set;
        }
        public override void Initialize()
        {
            this.Initialized = true;

            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            if (_grid_layer == null)
            {
                this.Initialized = false;
            }

            if (TypeConverterEx.IsNull(Direcotry))
                this.Initialized = false;

            if (TypeConverterEx.IsNull(SoilMoistureFileName))
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            var var_index = 0;
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            PRMS prms = null;
            if (model is HeiflowModel && TotalTimeStepNum > 0)
            {
                mf = (model as HeiflowModel).ModflowModel;
                prms = (model as HeiflowModel).PRMSModel;
                var grid = mf.Grid as RegularGrid;
                var NumTimeStep = 0;
                FileStream fs = new FileStream(SoilMoistureFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                var date = Start.AddSeconds(0);
                var varnum = br.ReadInt32();
                var feaNum = 0;
                for (int i = 0; i < varnum; i++)
                {
                    int varname_len = br.ReadInt32();
                    br.ReadChars(varname_len);
                    feaNum = br.ReadInt32();
                }
                var vec = new float[feaNum];
                var wp = (prms.MMSPackage.Parameters["soil_wp"] as DataCubeParameter<float>).ToVector();
                var sat = (prms.MMSPackage.Parameters["soil_sat"] as DataCubeParameter<float>).ToVector();
                var thick = prms.MMSPackage.Parameters["soil_depth"].FloatDataCube.ILArrays[0][":", _SelectedVarIndex].ToList();

                while (!(fs.Position == fs.Length))
                {
                    if (fs.Position > fs.Length)
                    {
                        NumTimeStep--;
                        break;
                    }
                    var Filename = "";
                    Filename = string.Format("{0}_{1}.tif", Variables[_SelectedVarIndex], date.ToString(DateFormat));
                    Filename = Path.Combine(Direcotry, Filename);
                    var raster = Raster.CreateRaster(Filename, string.Empty, grid.Topology.ColumnCount, grid.Topology.RowCount, 1, typeof(float), new[] { string.Empty });
                    raster.NoDataValue = -9999;
                    raster.Bounds = new RasterBounds(grid.Topology.RowCount, grid.Topology.ColumnCount, new Extent(grid.BBox));
                    raster.Projection = _grid_layer.Projection;
                  
                    for (int s = 0; s < feaNum; s++)
                    {
                        br.ReadBytes(4 * var_index);
                        vec[s] = (br.ReadSingle() + wp[s]) * Scale;
                        vec[s] /= thick[s];
                        if (vec[s] > Maximum)
                            vec[s] = Maximum;
                        br.ReadBytes(4 * (varnum - var_index - 1));
                        br.ReadBytes(4 * varnum * 2);
                    }

                    for (int i = 0; i < grid.ActiveCellCount; i++)
                    {
                        var loc = grid.Topology.ActiveCellLocation[i];
                        raster.Value[loc[0], loc[1]] = vec[i];
                    }
                    raster.Save();
                    progress = NumTimeStep * 100 / TotalTimeStepNum;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing step: " + date);
                        count++;
                    }
                   
                    NumTimeStep++;
                    date = Start.AddSeconds(NumTimeStep * TimeInteval);
                }
                br.Close();
                fs.Close();

                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: HEIFLOW must be used by this tool.");
                return false;
            }
        }

    }
}