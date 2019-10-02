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
using DotSpatial.Projections;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.UI;
using NetTopologySuite.Geometries;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [Export(typeof(IMFPackage))]
    [PackageCategory("Boundary Conditions,Specified Flux",false)]
    [CoverageItem]
    public class FHBPackage : MFPackage
    {
        public static string PackageName = "FHB";
        public FHBPackage()
        {
            Name = PackageName;
            _FullName = "Flow and Head Boundary Package";
            NBDTIM = 1;
            IFHBSS = 1;
            IFHBCB = 0;
            NFHBX1 = 0;
            NFHBX2 = 0;
            IFHBUN = 112;
            CNSTM = 1;
            IFHBPT = 0;
            BDTIM = new int[] { 0 };

            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".fhb";
            _PackageInfo.ModuleName = PackageName;
            Description = "The Flow and Head Boundary package is used for specified head cells and specified flow cells whose properties can vary within a stress period.";
            Version = "FHB1";
            _Layer3DToken = "FHB";
            IsMandatory = false;
        }
        public int NBDTIM { get; set; }
        /// <summary>
        ///  the number of cells at which flows will be specified
        /// </summary>
        public int NFLW { get; set; }
        /// <summary>
        /// the number of cells at which head will be specified
        /// </summary>
        public int NHED { get; set; }
        /// <summary>
        ///  the FHB steady-state option flag.  If the simulation includes any transient-state stress periods, 
        ///  the flag is read but not used; in this case, specified-flow, specified-head, and auxiliary-variable values will be interpolated 
        ///  for steady-state stress periods in the same way that values are interpolated for transient stress periods.
        /// </summary>
        public int IFHBSS
        { get; set; }
        /// <summary>
        /// 	If IFHBCB > 0, it is the unit number on which cell-by-cell flow terms will be recorded whenever ICBCFL is set
        /// </summary>
        public int IFHBCB
        { get; set; }
        /// <summary>
        /// NFHBX1 is the number of auxiliary variables whose values will be computed for each time step for each specified-flow cell.
        /// </summary>
        public int NFHBX1
        { get; set; }
        /// <summary>
        /// the number of auxiliary variables whose values will be computed for each time step for each specified-head cell.
        /// </summary>
        public int NFHBX2
        { get; set; }
        /// <summary>
        /// IFHBUN is the unit number on which data lists will be read.
        /// </summary>
        public int IFHBUN
        { get; set; }
        /// <summary>
        /// CNSTM is a constant multiplier for data list BDTIM
        /// </summary>
        public float CNSTM
        { get; set; }
        /// <summary>
        /// a flag for printing values of data lists in items 4b, 5b, 6b, 7b, and 8b. 
        /// If IFHBPT > 0 data list read at the beginning of the simulation will be printed.
        /// If IFHBCB ≤ 0 data list read at the beginning of the simulation will not be printed.
        /// </summary>
        public float IFHBPT
        { get; set; }
        /// <summary>
        /// BDTIM is simulation time at which values of specified flow and (or) values of specified head will be read. NBDTIM values are required.
        /// </summary>
        public int[] BDTIM { get; set; }
        /// <summary>
        /// 3d mat [4 + NBDTIM][1][NFLW]. The variables are Layer Row Column IAUX  FLWRAT(NBDTIM)
        /// </summary>
      [StaticVariableItem]
        public DataCube<float> FlowRate { get; set; }
      public RegularGridTopology Topology { get; set; }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override bool New()
        {
            this.IFHBUN = this.PackageInfo.FID;
            base.New(); 
            return true;
        }
        public override string CreateFeature(ProjectionInfo proj_info, string directory)
        {
            if (FlowRate != null)
            {
                string filename = Path.Combine(directory, this.Name + ".shp");
                var grid = (Owner as Modflow).Grid as MFGrid;

                FeatureSet fs = new FeatureSet(FeatureType.Point);
                fs.Name = this.Name;
                fs.Projection = proj_info;
                fs.DataTable.Columns.Add(new DataColumn("CELL_ID", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Layer", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Row", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Column", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));

                fs.DataTable.Columns.Add(new DataColumn("Elevation", typeof(float)));
                for (int i = 0; i < NBDTIM; i++)
                {
                    fs.DataTable.Columns.Add(new DataColumn("Flux Rate" + (i + 1), typeof(float)));
                }

                fs.DataTable.Columns.Add(new DataColumn("Name", typeof(string)));
                fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(int)));

                var mat = FlowRate;
                for (int i = 0; i < FlowRate.Size[2];i++ )
                {
                     int layer = (int)mat[0,0,i];  
                    int row = (int)mat[1,0,i];
                    int col = (int)mat[2,0,i];
                   
                    var coor = grid.LocateCentroid(col,row);
                    Point geom = new Point(coor);
                    IFeature feature = fs.AddFeature(geom);
                    feature.DataRow.BeginEdit();
                    feature.DataRow["CELL_ID"] = grid.Topology.GetID(row - 1, col - 1);
                    feature.DataRow["Layer"] = layer;
                    feature.DataRow["Row"] = row;             
                    feature.DataRow["Column"] = col;
                    feature.DataRow["ID"] = (i+1);
                    feature.DataRow["Elevation"] = grid.GetElevationAt(row - 1, col - 1, layer - 1);
                    feature.DataRow["Name"] = "Flow " + (i+1);
                    for (int j = 0; j < NBDTIM; j++)
                    {
                        feature.DataRow["Flux Rate" + (j + 1)] = mat[j + 4, 0, i];
                    }
                    feature.DataRow[RegularGrid.ParaValueField] = 0;
                    feature.DataRow.EndEdit();
                }
                fs.SaveAs(filename, true);
                fs.Close();

                return filename;
            }
            else
            {
                return null;
            }
        }
        public override bool Load(ICancelProgressHandler progress)
        {
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);

                //# Data Set 1 NBDTIM  NFLW  NHED IFHBSS  IFHBCB  NFHBX1  NFHBX2
                var line = sr.ReadLine();
                var temp = TypeConverterEx.Split<int>(line, 7);
                NBDTIM = temp[0];
                NFLW = temp[1];
                NHED = temp[2];
                IFHBSS = temp[3];
                IFHBCB = temp[4];
                NFHBX1 = temp[5];
                NFHBX2 = temp[6];

                //112	1	0	# Data Set 4a IFHBUN CNSTM IFHBPT
                line = sr.ReadLine();
                var strs = TypeConverterEx.Split<string>(line, 3);
                IFHBUN = int.Parse(strs[0]);
                CNSTM = float.Parse(strs[1]);
                IFHBPT = int.Parse(strs[2]);

                //0	1460	4749	# Data Set 4b BDTIM 
                line = sr.ReadLine();
                BDTIM = TypeConverterEx.Split<int>(line);

                //112	1	0	# Data Set 5a IFHBUN CNSTM IFHBPT
                line = sr.ReadLine();
                strs = TypeConverterEx.Split<string>(line, 3);
                IFHBUN = int.Parse(strs[0]);
                CNSTM = float.Parse(strs[1]);
                IFHBPT = int.Parse(strs[2]);

                //# Data Set 5b Layer Row Column IAUX  FLWRAT(NBDTIM)
                //   MFWell[] wells = new MFWell[NFLW];
                FlowRate = new DataCube<float>(4 + NBDTIM, 1, NFLW,true)
                {
                    Name = "FHB_FlowRate",
                    TimeBrowsable = false,
                    AllowTableEdit = true
                };
                FlowRate.Variables[0] = "Layer";//Layer Row Column IAUX  FLWRAT(NBDTIM)
                FlowRate.Variables[1] = "Row";
                FlowRate.Variables[2] = "Column";
                FlowRate.Variables[3] = "IAUX";
                for (int i = 0; i < NBDTIM; i++)
                {
                    FlowRate.Variables[4 + i] = "FLWRAT " + (i + 1);
                }
                for (int i = 0; i < NFLW; i++)
                {
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<float>(line);
                    FlowRate[0,0,i] = (int)buf[0];
                    FlowRate[1,0,i] = (int)buf[1];
                    FlowRate[2,0,i] = (int)buf[2];
                    FlowRate[3,0,i] = (int)buf[3];
                    for (int j = 0; j < NBDTIM; j++)
                    {
                        FlowRate[4+j,0,i] = buf[4+j];
                    }
                }
                FlowRate.TimeBrowsable = false;
                BuildTopology();
                sr.Close();
                OnLoaded(progress);
                return true;
            }
            else
            {
                Message = string.Format("\r\n Failed to load {0}. The package file does not exist: {1}", Name, FileName);
                OnLoadFailed(Message, progress);
                return false;
            }
        }
        public override bool SaveAs(string filename,ICancelProgressHandler prg)
        {
            StreamWriter sw = new StreamWriter(filename);
            NBDTIM = BDTIM.Length;
            string line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t# Data Set 1 NBDTIM  NFLW  NHED IFHBSS  IFHBCB  NFHBX1  NFHBX2",
                NBDTIM, NFLW, NHED, IFHBSS, IFHBCB, NFHBX1, NFHBX2);
            sw.WriteLine(line);
            line = string.Format("{0}\t{1}\t{2}\t# Data Set 4a IFHBUN CNSTM IFHBPT", IFHBUN, CNSTM, IFHBPT);
            sw.WriteLine(line);
            line = "";
            foreach (var d in BDTIM)
            {
                line += d + "\t";
            }
            line += "# Data Set 4b BDTIM ";
            sw.WriteLine(line);
            line = string.Format("{0}\t{1}\t{2}\t# Data Set 5a IFHBUN CNSTM IFHBPT", IFHBUN, CNSTM, IFHBPT);
            sw.WriteLine(line);

            for (int i = 0; i < NFLW; i++)
            {
                line = string.Format("{0}\t{1}\t{2}\t{3}\t", FlowRate[0,0,i],FlowRate[1,0,i],FlowRate[2,0,i],FlowRate[3,0,i]);
                for (int n = 0; n < BDTIM.Length; n++)
                {
                    line += FlowRate[4+n,0,i].ToString() + "\t";
                }
                line += "# Data Set 5b Layer Row Column IAUX  FLWRAT(NBDTIM)";
                sw.WriteLine(line);
            }
            sw.Close();
            OnSaved(prg);
            return true;
        }
        public override void Clear()
        {
            base.Clear();
        }
        public void BuildTopology()
        {
            var grid = Grid as RegularGrid;
            Topology = new RegularGridTopology();
            Topology.ActiveCell = new int[NFLW][];
            Topology.ActiveCellIDs = new int[NFLW];
            Topology.RowCount = grid.RowCount;
            Topology.ColumnCount = grid.ColumnCount;
            Topology.ActiveCellCount = NFLW;
            var mat = FlowRate;
            for (int i = 0; i < NFLW; i++)
            {
                int layer = (int)mat[0,0,i];
                int row = (int)mat[1,0,i];
                int col = (int)mat[2,0,i];
                Topology.ActiveCell[i] = new int[] { row - 1, col - 1 };
                Topology.ActiveCellIDs[i] = grid.Topology.GetID(row - 1, col - 1);
            }
            FlowRate.Topology = this.Topology;
        }
    }
}
