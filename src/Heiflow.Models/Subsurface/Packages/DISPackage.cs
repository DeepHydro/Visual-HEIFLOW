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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data;
using Heiflow.Models.Generic;
using Heiflow.Core.Data;
using ILNumerics;
using System.Diagnostics;
using System.ComponentModel.Composition;
using Heiflow.Models.Generic.Attributes;
using DotSpatial.Data;
using GeoAPI.Geometries;
using System.ComponentModel;
using Heiflow.Models.UI;

namespace Heiflow.Models.Subsurface
{

    /// <summary>
    /// MF Grid is constructed through this class, which should be called at fist
    /// </summary>
    /// 
    [PackageItem]
    [Export(typeof(IMFPackage))]
    [PackageCategory("Basic", true)]
    public class DISPackage : MFPackage
    {
        public static string PackageName = "DIS";
        public DISPackage()
        {
            Name = "DIS";
            _FullName = "Discretization File";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".dis";
            _PackageInfo.ModuleName = "DIS";
            Description = "The Discretization File  is used to specify certain data used in all models. These include" +
                                        "\r\n1. the number of rows, columns and layers" +
                                        "\r\n2. the cell sizes" +
                                        "\r\n3. the presence of Quasi-3D confining beds" +
                                        "\r\n4. the time discretization";
            Version = "DIS";
            IsMandatory = true;
            _Layer3DToken = "RegularGrid";
            
        }
       /// <summary>
        /// is a flag, with one value for each model layer, that indicates whether or not a layer has a Quasi-3D confining bed below it. 
        /// 0 indicates no confining bed, and not zero indicates a confining bed. LAYCBD for the bottom layer must be 0.
       /// </summary>
        public int[] LAYCBD { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [StaticVariableItem("Layer")]
        public My3DMat<float> Elevation
        {
            get
            {
                return (Owner.Grid as IRegularGrid).Elevations;
            }
        }

        [Browsable(false)]
        [PackageOptionalViewItem("DIS")]
        public override UI.IPackageOptionalView OptionalView
        {
            get;
            set;
        }

        public override void Initialize()
        {
            Message = "";
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override bool New()
        {        
            base.New();
            return true;
        }
        public override bool Load()
        {
            if (File.Exists(FileName))
            {
                var grid = (Owner.Grid as MFGrid);
                var mf = Owner as Modflow;

                grid.BBox = new Envelope();
                StreamReader sr = new StreamReader(FileName);
                string line = "";
                Coordinate upl = null;
                Coordinate lowr = null;

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine().ToLower().Trim();
                    if (line.StartsWith("#"))
                    {
                        if (line.Contains("upper left corner"))
                        {
                            upl = ParseCoord(line);
                        }
                        else if (line.Contains("lower right corner"))
                        {
                            lowr = ParseCoord(line);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // 5 150 172 10 4 2 # NLAY, NROW, NCOL, NPER, ITMUNI, LENUNI
                int[] strs = TypeConverterEx.Split<int>(line, 6);
                grid.ActualLayerCount = strs[0];
                //grid.LayerCount = grid.ActualLayerCount + 1;
                grid.RowCount = strs[1];
                grid.ColumnCount = strs[2];
                int np = strs[3];
                mf.TimeUnit = strs[4];
                mf.LengthUnit = strs[5];
                mf.TimeService.TimeUnit = mf.TimeUnit;

                //# LAYCB
                line = sr.ReadLine();
                LAYCBD = TypeConverterEx.Split<int>(line, grid.ActualLayerCount);

                grid.DELR = new MyVarient3DMat<float>(1, 1);
                grid.DELC = new MyVarient3DMat<float>(1, 1);
                ReadSerialArray(sr, grid.DELR, 0, 0,grid.ColumnCount);
                ReadSerialArray(sr, grid.DELC, 0, 0, grid.RowCount);

                if (upl == null || lowr == null)
                {
                    double height = grid.DELR.Sum();
                    double width = grid.DELC.Sum();
                    if (grid.Origin != null)
                    {
                        upl = new Coordinate(grid.Origin.X,grid.Origin.Y);
                        lowr = new Coordinate(grid.Origin.X + width, grid.Origin.Y - height);
                    }
                    else
                    {
                        upl = new Coordinate(0, height);
                        lowr = new Coordinate(width, 0);
                    }
                }
                grid.Origin = upl;
                grid.BBox = new Envelope(upl.X, lowr.X, lowr.Y, upl.Y);
                grid.Elevations = new MyVarient3DMat<float>(grid.LayerCount, 1)
                {
                    Name = "Elevations",
                    TimeBrowsable = false,
                    AllowTableEdit = true
                };
                grid.Elevations.Variables[0] = "Top Elevation";
                grid.Elevations.Topology = grid.Topology;

                for (int l = 1; l < grid.LayerCount; l++)
                {
                    grid.Elevations.Variables[l] = "Layer " + l + " Bottom Elevation";
                }

                for (int l = 0; l < grid.LayerCount; l++)
                {
                    ReadSerialArray<float>(sr, grid.Elevations, l, 0);
                }
                mf.TimeService.StressPeriods.Clear();
                for (int i = 0; i < np; i++)
                {
                    line = sr.ReadLine();
                    var ss = TypeConverterEx.Split<string>(line);
                    mf.TimeService.StressPeriods.Add(new StressPeriod()
                    {
                        Length = (int)double.Parse(ss[0]),
                        NSTP = (int)double.Parse(ss[1]),
                        Multiplier = (int)double.Parse(ss[2]),
                        State = ConvertFrom(ss[3]),
                        ID = i + 1
                    });
                }
                sr.Close();
                OnLoaded("successfully loaded");
                return true;
            }
            else
            {
                Message = string.Format("\r\n Failed to load {0}. The package file does not exist: {1}", Name, FileName);
                OnLoadFailed(Message);
                return false;
            }
        }
        public override bool SaveAs(string filename, IProgress progress)
        {
            var grid = (this.Grid as IRegularGrid);
            var mf = Owner as Modflow;

            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, "DIS");
            var ul = new Coordinate(grid.BBox.MinX, grid.BBox.MaxY);// grid.BBox.TopLeft();
            var lr = new Coordinate(grid.BBox.MaxX, grid.BBox.MinY);
            string line = string.Format("#Upper left corner:({0},{1})", ul.X, ul.Y);
            sw.WriteLine(line);
            line = string.Format("#Lower right corner:({0},{1})", lr.X, lr.Y);
            sw.WriteLine(line);

            line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t #NLAY  NROW   NCOL  NPER   ITMUNI  LENUNI", grid.LayerCount - 1, grid.RowCount,
              grid.ColumnCount, TimeService.StressPeriods.Count, mf.TimeUnit, mf.LengthUnit);
            sw.WriteLine(line);
            line = string.Join("\t", LAYCBD) + "\t#LAYCBD";
            sw.WriteLine(line);
            WriteSerialFloatArray(sw, grid.DELR, 0, 0, "F2", "DELR");
            WriteSerialFloatArray(sw, grid.DELC, 0, 0, "F2", "DELC");

            string cmt = " #TOP";
            for (int k = 0; k < grid.LayerCount; k++)
            {
                if (k > 0)
                {
                    cmt = " #BOTTOM OF LAYER " + k;
                }
                WriteSerialFloatArray(sw, grid.Elevations, k, 0, "F2", cmt);
                //WriteSerialFloatInternalMatrix(sw, grid.Elevations.Value[k][0], 1.0f, "F2", -1, cmt);
            }

            foreach (var sp in mf.TimeService.StressPeriods)
            {
                sw.WriteLine(sp.ToString());
            }
            sw.Close();
            OnSaved(progress);
            return true;
        }
        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;
            base.Clear();
        }
        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
        public override void OnGridUpdated(IGrid sender)
        {
            LAYCBD = new int[Grid.ActualLayerCount];
            this.FeatureLayer = this.Grid.FeatureLayer;
            this.Feature = this.Grid.FeatureSet;
            base.OnGridUpdated(sender);
        }
        public override IPackage Extract(Modflow newmf)
        {
            DISPackage dis = new DISPackage();
            dis.Owner = newmf;
            dis.LAYCBD = this.LAYCBD;

            return dis;
        }
        /// <summary>
        ///  require three parameters
        /// </summary>
        /// <param name="feature">Grid feature set</param>
        /// <param name="para"> The first is a field name used to  sort mf_grid;  The second  is
        /// a string array that stores field names used to set layer elevation (from top to bottom)
        /// </param>
        public override void Assign(DotSpatial.Data.IFeatureSet feature, params object[] para)
        {
            var grid = this.Owner.Grid as MFGrid;
            var mfprj = this.Owner as Modflow;

            var dt_grid = feature.DataTable;
            dt_grid.DefaultView.Sort = para[0].ToString();
            string[] ele_fields = para[1] as string[];

            if (grid.Elevations == null)
            {
                grid.Elevations = new MyVarient3DMat<float>(grid.LayerCount, 1, grid.ActiveCellCount);
            }

            for (int l = 0; l < grid.LayerCount; l++)
            {
                foreach (DataRow dr in dt_grid.Rows)
                {
                    int row = int.Parse(dr["Row"].ToString()) - 1;
                    int col = int.Parse(dr["Column"].ToString()) - 1;
                    int id = grid.Topology.GetID(row, col);
                    int index = grid.Topology.CellID2CellIndex[id];
                    grid.Elevations[l,index, 0] = float.Parse(dr[ele_fields[l]].ToString());
                }
            }
        }

        public ModelState ConvertFrom(string str)
        {
            if (str.ToUpper() == "SS")
                return ModelState.SS;
            else
                return ModelState.TR;
        }

        public void GetGridInfo(MFGrid grid)
        {
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                string line = "";

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine().ToLower().Trim();
                    if (!line.StartsWith("#"))
                    {
                        break;
                    }
                }

                // 5 150 172 10 4 2 # NLAY, NROW, NCOL, NPER, ITMUNI, LENUNI
                int[] strs = TypeConverterEx.Split<int>(line, 6);
                grid.ActualLayerCount = strs[0];
                grid.RowCount = strs[1];
                grid.ColumnCount = strs[2];

                sr.Close();
            }
        }

        public override bool Check(out string msg)
        {

            return base.Check(out msg);
        }

        public bool CorrectElevation(float[,] range, float[,] ratio)
        {
            bool successful = true;
            var grid = (Owner.Grid as IRegularGrid);
            int nlayer = this.Elevation.Size[0];
            int ncell = this.Elevation.Size[2];
            int[] range_index = new int[ncell];
            int nrange = range.Length / 2;
            Message = "";
            for (int c = 0; c < ncell; c++)
            {
                range_index[c] = -1;
                for (int i = 0; i < nrange; i++)
                {
                    if (Elevation.Value[0][0][c] < range[i, 1] && Elevation.Value[0][0][c] >= range[i, 0])
                    {
                        range_index[c] = i;
                        break;
                    }
                }
                if (range_index[c] == -1)
                {
                    Message += "Failed to find avaiable range for cell : " + c;
                    successful = false;
                }
            }

            if (!successful)
                return successful;

            for (int c = 0; c < ncell; c++)
            {
                var total_height = Elevation.Value[0][0][c] - Elevation.Value[nlayer - 1][0][c];
                for (int l = 0; l < nlayer - 1; l++)
                {
                    Elevation.Value[l + 1][0][c] = Elevation.Value[l][0][c] - total_height * ratio[range_index[c], l];
                    if (Elevation.Value[l + 1][0][c] < -200)
                        Elevation.Value[l + 1][0][c] = -200;
                }
            }
            return successful;
        }

        private Coordinate ParseCoord(string line)
        {
            string[] newArray = line.Split(new char[] { '(', ')' });
            string[] strnum = newArray[1].Split(new char[] { ',' });
            Coordinate cor = new Coordinate()
            {
                X = double.Parse(strnum[0]),
                Y = double.Parse(strnum[1])
            };
            return cor;
        }

    }
}
