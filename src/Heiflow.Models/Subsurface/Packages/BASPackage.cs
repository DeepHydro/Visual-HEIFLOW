﻿//
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
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using ILNumerics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [PackageCategory("Basic", true)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class BASPackage : MFPackage
    {
        public static string PackageName = "BAS6";
        public BASPackage()
        {
            Name = "BAS6";
            _FullName = "Basic Package";
            //IPRN = -1;
            HNOFLO = -999.0f;
            STOPERROR = 10;
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".bas";
            _PackageInfo.ModuleName = "BAS6";
            Description = "The Basic package is used to specify certain data used in all models. These include" +
                                        "\r\n1. the locations of active, inactive, and specified head cells," +
                                        "\r\n2. the head stored in inactive cells, and" +
                                        "\r\n3. the initial heads in all cells.";
            Version = "BAS6";
            IsMandatory = true;
            _Layer3DToken = "RegularGrid";
            Category = Resources.BasicCategory;
        }
        /// <summary>
        /// the value of head to be assigned to all inactive
        /// </summary>
        public float HNOFLO
        {
            get;
            set;
        }

        public float STOPERROR
        {
            get;
            set;
        }

        /// <summary>
        /// 3DMat [ ActualLayerCount, 1, ActiveCellCount]
        /// </summary>
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 100)]
        //[JsonIgnore]
        public DataCube<float> STRT
        {
            get;
            set;
        }


        /// <summary>
        /// 3DMat [ ActualLayerCount, 1, ActiveCellCount]
        /// </summary>
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 1)]
        [JsonIgnore]
        public DataCube<float> IBound
        {
            get
            {
                var grid = (Owner.Grid as MFGrid);
                return grid.IBound;
            }
        }
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 1)]
        [JsonIgnore]
        public DataCube<float> MFIBound
        {
            get
            {
                var grid = (Owner.Grid as MFGrid);
                return grid.MFIBound;
            }
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override void New()
        {
            base.New();
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    //Data Set 1: # OPTIONS
                    string newline = ReadComment(sr);
                    Options = newline;
                    //Data Set 2: # IBOUND

                    var grid = (Owner.Grid as MFGrid);
                    grid.IBound = new DataCube<float>(grid.ActualLayerCount, grid.RowCount, grid.ColumnCount) { ZeroDimension = DimensionFlag.Spatial };
                    grid.MFIBound = new DataCube<float>(grid.ActualLayerCount, grid.RowCount, grid.ColumnCount) { ZeroDimension = DimensionFlag.Spatial };
                    grid.ActiveCellCount = 0;
                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        var array = ReadInternalMatrix<int>(sr);
                        for (int r = 0; r < array.Size[1]; r++)
                        {
                            for (int c = 0; c < array.Size[2]; c++)
                            {
                                grid.IBound[l, r, c] = array[0, r, c];
                                grid.MFIBound[l, r, c] = array[0, r, c];
                            }
                        }
                        if (l == 1)
                        {
                            for (int r = 0; r < array.Size[1]; r++)
                            {
                                for (int c = 0; c < array.Size[2]; c++)
                                {
                                    grid.IBound[0, r, c] = array[0, r, c];
                                    //grid.MFIBound[0, r, c] = array[0, r, c];
                                }
                            }
                        }
                    }

                    if (Owner.Packages.ContainsKey(LakePackage.PackageName))
                    {
                        var lak = (Owner.Packages[LakePackage.PackageName] as LakePackage);
                        {
                            lak.ChangeIBound();
                        }
                    }

                    grid.ActiveCellCount = 0;
                    for (int r = 0; r < grid.RowCount; r++)
                    {
                        for (int c = 0; c < grid.ColumnCount; c++)
                        {
                            if (grid.IBound[0, r, c] != 0)
                                grid.ActiveCellCount++;
                        }
                    }

                    newline = sr.ReadLine();
                    //Data Set 3: # HNOFLO the value of head to be assigned to all inactive 
                    this.HNOFLO = TypeConverterEx.Split<float>(newline, 1)[0];
                    //Data Set 4: STRT—is initial (starting) head
                    this.STRT = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
                    {
                        Name = "STRT",
                        ZeroDimension = DimensionFlag.Spatial
                    };

                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        this.STRT.Variables[l] = "Starting Head " + (l + 1);
                        ReadSerialArray(sr, this.STRT, l, 0);
                    }
                    sr.Close();
                 
                    result = LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progress);
                    result = LoadingState.FatalError;
                }
                finally
                {
                    sr.Close();
                }
            }
            else
            {
                Message = string.Format("Failed to load {0}. The package file does not exist: {1}", Name, FileName);
                ShowWarning(Message, progress);
                result = LoadingState.FatalError;
            }
            OnLoaded(progress, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }

        public override void SaveAs(string filename,ICancelProgressHandler progress)
        {
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, this.Name);
            string line = string.Format("FREE STOPERROR  {0}  # OPTIONS", STOPERROR);
            sw.WriteLine(line);
            string cmt = "";
            for (int k = 0; k < grid.ActualLayerCount; k++)
            {
                cmt = " # IBOUND of Layer " + (k + 1);
                WriteRegularArray<float>(sw, grid.MFIBound, k, "F0", cmt);
            }
            sw.WriteLine( HNOFLO + "  #  HNOFLO");
            for (int k = 0; k < grid.ActualLayerCount; k++)
            {
                cmt = " # Start Head of Layer " + (k + 1);
                WriteSerialFloatArray(sw, this.STRT, k, 0, "E6", cmt);
            }
            sw.Close();
            OnSaved(progress);
        }
        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;
            STRT = null;
            base.Clear();
        }

        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

        public override void OnGridUpdated(IGrid sender)
        {
            this.Grid = sender;
            this.FeatureLayer = this.Grid.FeatureLayer;
            this.Feature = this.Grid.FeatureSet;
            var grid = (sender as IRegularGrid);
            this.STRT = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Name = "STRT", ZeroDimension= DimensionFlag.Spatial
            };
          
            for (int l = 0; l < grid.ActualLayerCount; l++)
            {
             //   this.STRT.Value[l][0] = new float[grid.ActiveCellCount];
                for (int i = 0; i < grid.ActiveCellCount; i++)
                {
                    this.STRT[l, 0, i] = grid.Elevations[l + 1, 0, i];
                }
            }
            base.OnGridUpdated(sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="para">require two parameters: the first is a field name used to  sort the feature; the second is  a string array that stores field names 
        /// used to set initial head (from top to bottom) name
        /// </param>
        public override void Assign(DotSpatial.Data.IFeatureSet feature, params object[] para)
        {
            if (para == null)
                return;
            var dt_grid = feature.DataTable;
            dt_grid.DefaultView.Sort = para[0].ToString();
            string[] strt_fields = para[1] as string[];
            var mfgrid = (this.Owner.Grid as MFGrid);

            //if(mfgrid.IBound == null)
            //    mfgrid.IBound = ILMath.zeros<int>(mfgrid.RowCount, mfgrid.ColumnCount, mfgrid.ActualLayerCount);

            if (mfgrid.IBound == null)
                mfgrid.IBound = new DataCube<float>(mfgrid.ActualLayerCount, mfgrid.RowCount, mfgrid.ColumnCount) { ZeroDimension = DimensionFlag.Spatial };

            mfgrid.ActiveCellCount = 0;
            for (int l = 0; l < mfgrid.ActualLayerCount; l++)
            {
                foreach (DataRow dr in dt_grid.Rows)
                {
                    int row = int.Parse(dr["Row"].ToString());
                    int col = int.Parse(dr["Column"].ToString());
                    mfgrid.IBound[l, row - 1, col - 1] = float.Parse(dr["Active"].ToString());
                    if (mfgrid.IBound[l, row - 1, col - 1] != 0)
                        mfgrid.ActiveCellCount++;
                }
            }

            if (this.STRT == null)
                this.STRT = new DataCube<float>(mfgrid.ActualLayerCount, 1, mfgrid.ActiveCellCount) { ZeroDimension = DimensionFlag.Spatial };

            for (int l = 0; l < mfgrid.ActualLayerCount; l++)
            {
                foreach (DataRow dr in dt_grid.Rows)
                {
                    int row = int.Parse(dr["Row"].ToString());
                    int col = int.Parse(dr["Column"].ToString());
                    int index = mfgrid.Topology.GetSerialIndex(row, col);
                    this.STRT[l, index, 0] = float.Parse(dr[strt_fields[l]].ToString());
                }
            }
        }
        public override IPackage Extract(Modflow newmf)
        {
            BASPackage bas = new BASPackage();
            bas.HNOFLO = this.HNOFLO;
            bas.Owner = newmf;
            MFGrid newgrid = newmf.Grid as MFGrid;
            MFGrid rawgrid = Owner.Grid as MFGrid;
            bas.STRT = new DataCube<float>(newgrid.ActualLayerCount, 1, newgrid.ActiveCellCount) { ZeroDimension = DimensionFlag.Spatial };
            for (int l = 0; l < newgrid.ActualLayerCount; l++)
            {
               // bas.STRT[l, MyMath.full] = rawgrid.GetSubSerialArray(this.STRT, newgrid, l).Value;
            }
            return bas;
        }
    }
}
