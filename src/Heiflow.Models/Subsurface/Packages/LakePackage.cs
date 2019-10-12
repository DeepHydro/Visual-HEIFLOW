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
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Models.Subsurface.Packages;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [PackageCategory("Boundary Conditions,Head Dependent Flux",false)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class LakePackage : MFPackage
    {
        public static string PackageName = "LAK";
        private RegularGridTopology _LakeTopo;
        public LakePackage()
        {
            Name = "LAK";
            _FullName = "Lake Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".lak";
            _PackageInfo.ModuleName = "LAK";
            IsMandatory = false;

            ILKCB = 0;
            THETA = -0.5f;
            NSSITR = 100;
            SSCNCR = 0.01f;
            SURFDEPTH = 0.1f;
            Version = "Lak3";

            LakeSerialIndex = new Dictionary<int, List<int>>();
        }

        #region Properties
        public int[] LakeID { get; set; }

        //*****DataSet1b
        /// <summary>
        /// Number of separate lakes
        /// </summary>
        public int NLAKES { get; set; }
        /// <summary>
        /// Number of separate lakes
        /// </summary>
        public int ILKCB { get; set; }
        //*****DataSet2
        /// <summary>
        /// Explicit (THETA = 0.0), semi-implicit (0.0 .GT. THETA .LT. 1.0), or implicit (THETA = 1.0) solution for lake stages.
        /// SURFDEPTH is read only if THETA is assigned a negative value 
        /// </summary>
        public float THETA { get; set; }
        /// <summary>
        /// Maximum number of iterations for Newton’s method of solution for equilibrium lake stages 
        /// </summary>
        public int NSSITR { get; set; }
        /// <summary>
        /// Convergence criterion 
        /// </summary>
        public float SSCNCR { get; set; }
        /// <summary>
        /// The height of small topological variations 
        /// </summary>
        public float SURFDEPTH { get; set; }

        //*****DataSet3
        /// <summary>
        /// [1,NLAKES,5] (STAGES,SSMN,SSMX,IUNITTAB,CLAKE)
        /// </summary>
        /// 
       [StaticVariableItem]
       [Browsable(false)]
        public DataCube2DLayout<float> STAGES { get; set; }

        //*****DataSet4
        /// <summary>
        /// [1,nperiod,3] (ITMP,ITMP1, LWRT )
        /// </summary>
        /// 
       [StaticVariableItem]
       [Browsable(false)]
       public DataCube<int> ITMP { get; set; }

        //*****DataSet5
        /// <summary>
        ///[nlayer,1,nactcell]   0, the grid cell is not a lake volume cell. > 0, its value is the identification number of the lake occupying the grid cell. 
        /// </summary>
       [StaticVariableItem]
       [Browsable(false)]
       public DataCube<int> LKARR { get; set; }

        //*****DataSet6
        /// <summary>
       /// [nlayer,1,nactcell]. the lakebed leakance that will be assigned to lake/aquifer interfaces that occur in the corresponding grid cell.
        /// </summary>
        /// 
       [StaticVariableItem]
       [Browsable(false)]
       public DataCube<float> BDLKNC { get; set; }

        /// <summary>
       /// [nsp,1,1] The number of sublake systems if coalescing/dividing lakes are to be simulated (only in transient runs). Enter 0 if no sublake systems are to be simulated.
        /// </summary>
        /// 
        [StaticVariableItem]
        [Browsable(false)]
       public DataCube2DLayout<int> NSLMS { get; set; }

        /// <summary>
        /// [np,nlake,6]: PRCPLK EVAPLK RNF WTHDRW [SSMN] [SSMX]
        /// </summary>
        /// 
       [StaticVariableItem]
       [Browsable(false)]
        public DataCube2DLayout<float> WSOUR { get; set; }
        [Browsable(false)]
        /// <summary>
        /// [nlake][num_lake_cell]. 
        /// </summary>
       public Dictionary<int,List<int>> LakeSerialIndex { get; set; }
        #endregion

        public override void Initialize()
        {
            Message = "";
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            base.Initialize();
        }
        public override bool New()
        {

            return base.New();
        }
        public override bool Load(ICancelProgressHandler progresshandler)
        {
            var mf = (Owner as Modflow);
             var grid = (Owner.Grid as MFGrid);
            int nsp = TimeService.StressPeriods.Count;
            int nlayer= grid.ActualLayerCount;
            if (File.Exists(FileName))
            {
                LakeSerialIndex.Clear();
                StreamReader sr = new StreamReader(FileName);
                string line = sr.ReadLine();
                line = sr.ReadLine();
                //7	0	 # DataSet 1b: NLAKES ILKCB
                //-0.5 	100	 0.01 	0.5	 # DataSet 2: THETA NSSITR SSCNCR SURFDEPTH
                int[] intbuf = TypeConverterEx.Split<int>(line, 2);
                NLAKES = intbuf[0];
                ILKCB = intbuf[1];
                line = sr.ReadLine();
                var floatbuf = TypeConverterEx.Split<float>(line, 4);
                THETA = floatbuf[0];
                NSSITR = (int)floatbuf[1];
                SSCNCR = floatbuf[2];
                SURFDEPTH = floatbuf[3];

                STAGES = new DataCube2DLayout<float>(1, NLAKES, 3)
                {
                    ColumnNames = new string[] { "STAGES", "SSMN", "SSMX", "IUNITTAB", "CLAKE" }
                };
                for (int i = 0; i < NLAKES; i++)
                {
                    line = sr.ReadLine();
                    floatbuf = TypeConverterEx.Split<float>(line, 3);
                    STAGES[0][i, ":"] = floatbuf;
                }

                ITMP = new DataCube2DLayout<int>(1, nsp, 3)
                {
                    ColumnNames = new string[] { "ITMP", "ITMP1", "LWRT" }
                };
                LKARR = new DataCube<int>(nlayer, 1, grid.ActiveCellCount)
                {
                    Name = "Lake ID",
                    Variables = new string[nlayer],
                    ZeroDimension = DimensionFlag.Spatial
                };
                for (int l = 0; l < nlayer; l++)
                {
                    LKARR.Variables[l] = "Lake ID of " + " Layer " + (l + 1);
                }

                BDLKNC = new DataCube<float>(nlayer, 1, grid.ActiveCellCount)
                {
                    Name = "Leakance",
                    Variables = new string[nlayer],
                    ZeroDimension = DimensionFlag.Spatial
                };
                for (int l = 0; l < nlayer; l++)
                {
                    BDLKNC.Variables[l] = "Leakance of " + (l + 1);
                }
                NSLMS = new DataCube2DLayout<int>(1, nsp, 1)
                {
                    Name = "Num of Sublakes",
                    Variables = new string[nsp],
                    ColumnNames = new string[] { "Num of Sublakes" }
                };
                for (int l = 0; l < nsp; l++)
                {
                    NSLMS.Variables[l] = "Stress Period " + (l + 1);
                }
                WSOUR = new DataCube2DLayout<float>(nsp, NLAKES, 6)
                {
                    Name = "Recharge Discharge",
                    Variables = new string[nsp],
                    ZeroDimension = DimensionFlag.Time,
                    ColumnNames = new string[] { "PRCPLK", "EVAPLK", "RNF", "WTHDRW", "SSMN", "SSMX" }
                };
                for (int l = 0; l < nsp; l++)
                {
                    WSOUR.Variables[l] = "Stress Period " + (l + 1);
                }
                for (int i = 0; i < nsp; i++)
                {
                    line = sr.ReadLine();
                    intbuf = TypeConverterEx.Split<int>(line, 3);
                    ITMP[0][i, ":"] = intbuf;
                    if (ITMP[0, i, 0] > 0)
                    {
                        for (int j = 0; j < grid.ActualLayerCount; j++)
                        {
                            ReadSerialArray<int>(sr, LKARR, j, 0);
                        }
                        for (int j = 0; j < grid.ActualLayerCount; j++)
                        {
                            ReadSerialArray<float>(sr, BDLKNC, j, 0);
                        }
                        line = sr.ReadLine();
                        intbuf = TypeConverterEx.Split<int>(line, 1);
                        NSLMS[i, 0, 0] = intbuf[0];
                    }
                    if (ITMP[0, i, 1] > 0)
                    {
                        for (int j = 0; j < NLAKES; j++)
                        {
                            line = sr.ReadLine();
                            if (i == 0)
                            {
                                floatbuf = TypeConverterEx.Split<float>(line, 6);
                                WSOUR[i][j, ":"] = floatbuf;
                            }
                            else
                            {
                                floatbuf = TypeConverterEx.Split<float>(line, 4);
                                WSOUR[i][j, "0:3"] = floatbuf;
                            }
                        }
                    }
                }
                var vec = LKARR.GetVector(0, "0", ":");
                foreach(var id in vec.Distinct())
                {
                    LakeSerialIndex.Add(id, new List<int>());
                }
                for (int i = 0; i < vec.Count(); i++ )
                {
                    if (vec[i] != 0)
                    {
                        LakeSerialIndex[vec[i]].Add(i);
                    }
                }
                sr.Close();
                OnLoaded(progresshandler);
                return true;
            }
            else
            {
                Message = string.Format("\r\n Failed to load {0}. The package file does not exist: {1}", Name, FileName);
                progresshandler.Progress(this.Name, 100, Message);
                OnLoadFailed(Message, progresshandler);
                return false;
            }   
        }
        public override bool SaveAs(string filename, ICancelProgressHandler progress)
        {
            var grid = (this.Grid as IRegularGrid);
            var mf = Owner as Modflow;
            int nsp = TimeService.StressPeriods.Count;

            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, "LAK");
            string line = string.Format("{0}\t{1}\t#NLAKES ILKCB", NLAKES, ILKCB);
            sw.WriteLine(line);
            line = string.Format("{0}\t{1}\t{2}\t{3}\t# THETA NSSITR SSCNCR SURFDEPTH", THETA, NSSITR, SSCNCR, SURFDEPTH);
            sw.WriteLine(line);
            for (int i = 0; i < NLAKES; i++)
            {
                line = string.Format("{0}\t{1}\t{2}\t#STAGES,SSMN,SSMX", STAGES[0, i, 0], STAGES[0, i, 1], STAGES[0, i, 2]);
                sw.WriteLine(line);
            }
            for (int i = 0; i < nsp; i++)
            {
                line = string.Format("{0}\t{1}\t{2}\t#ITMP,ITMP1, LWRT", ITMP[0, i, 0], ITMP[0, i, 1], ITMP[0, i, 2]);
                sw.WriteLine(line);
                if (ITMP[0, i, 0] > 0)
                {
                    string cmt = "";
                    for (int j = 0; j < grid.ActualLayerCount; j++)
                    {
                        cmt = " # LKARR  of Layer " + (j + 1);
                        WriteSerialArray<int>(sw, LKARR, j,0, "F0", cmt);
                    }
                    for (int j = 0; j < grid.ActualLayerCount; j++)
                    {
                        cmt = " # BDLKNC  of Layer " + (j + 1);
                        WriteSerialFloatArray(sw, this.BDLKNC, j, 0, "E6", cmt);
                    }
                    line = string.Format("{0}\t#NSLMS ", NSLMS[i, 0, 0]);
                    sw.WriteLine(line);
                }
                if (ITMP[0, i, 1] > 0)
                {
                    for (int j = 0; j < NLAKES; j++)
                    {
                        line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t#PRCPLK EVAPLK RNF WTHDRW [SSMN] [SSMX]",
                            WSOUR[i, j, 0], WSOUR[i, j, 1], WSOUR[i, j, 2], WSOUR[i, j, 3], WSOUR[i, j, 4], WSOUR[i, j, 5]);
                        sw.WriteLine(line);
                    }
                }
            }
            sw.Close();
            OnSaved(progress);
            return true;
        }
        public override string CreateFeature(DotSpatial.Projections.ProjectionInfo proj_info, string directory)
        {
            string filename = Path.Combine(directory, this.Name + ".shp");
            var grid = (Owner as Modflow).Grid as MFGrid;
            FeatureSet fs = new FeatureSet(FeatureType.Polygon);
            fs.Name = this.Name;
            fs.Projection = proj_info;
            fs.DataTable.Columns.Add(new DataColumn("CELL_ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("HRU_ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("ROW", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("COL", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(double)));

            foreach(var id in LakeSerialIndex.Keys)
            {
                foreach(var hru_index in LakeSerialIndex[id])
                {
                    var loc = grid.Topology.ActiveCellLocation[hru_index];
                    var vertices = grid.LocateNodes(loc[1],loc[0]);
                    ILinearRing ring = new LinearRing(vertices);
                    Polygon geom = new Polygon(ring);
                    IFeature feature = fs.AddFeature(geom);
                    feature.DataRow.BeginEdit();
                    feature.DataRow["CELL_ID"] = grid.Topology.GetID(loc[1], loc[0]);
                    feature.DataRow["HRU_ID"] = hru_index+1;
                    feature.DataRow["ROW"] = loc[0]+1;
                    feature.DataRow["COL"] = loc[1] + 1;
                    feature.DataRow[RegularGrid.ParaValueField] = 0;
                    feature.DataRow.EndEdit();
                }
            }
            fs.SaveAs(filename, true);
            fs.Close();
            return filename;

        }
        public override void Clear()
        {
            if (_Initialized)
            {
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
                this.Grid.Updated -= this.OnGridUpdated;
            }
            base.Clear();
        }
        public override void OnGridUpdated(IGrid sender)
        {
            this.FeatureLayer = this.Grid.FeatureLayer;
            this.Feature = this.Grid.FeatureSet;
            base.OnGridUpdated(sender);
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            var nsp = time.StressPeriods.Count;
        }

        public void BuildTopology()
        {
            var grid = Grid as RegularGrid;
            _LakeTopo = new RegularGridTopology();
            _LakeTopo.ActiveCellLocation = new int[NLAKES][];
            _LakeTopo.ActiveCellID = new int[NLAKES];
            _LakeTopo.RowCount = grid.RowCount;
            _LakeTopo.ColumnCount = grid.ColumnCount;
            _LakeTopo.ActiveCellCount = NLAKES;

            int i = 0;
            foreach (var newreach in LakeSerialIndex.Values)
            {
                var loc = grid.Topology.ActiveCellLocation[newreach[0]];
                _LakeTopo.ActiveCellLocation[i] = loc;
                _LakeTopo.ActiveCellID[i] = grid.Topology.GetID(loc[0], loc[1]);
                i++;
            }

        }
    }
}
