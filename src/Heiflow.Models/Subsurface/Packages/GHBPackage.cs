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
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Integration;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using ILNumerics;
using NetTopologySuite.Geometries;
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
    [PackageCategory("Boundary Conditions,Head Dependent Flux", false)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class GHBPackage : MFPackage
    {
        private RegularGridTopology _CellTopo;
        public static string PackageName = "GHB";
        public GHBPackage()
        {
            Name = "GHB";
            _FullName = "General-Head Boundary Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".ghb";
            _PackageInfo.ModuleName = "GHB";
            Description = "The General-Head Boundary package is used to simulate head-dependent flux boundaries.  In the General-Head Boundary package the flux is always proportional to the difference in head.";
            Version = "GHB";
            IsMandatory = false;
            _Layer3DToken = "CHD";
            Category = Resources.HeadDependentCategory; 
        }
        [Category("Basic")]
        [Description("The maximum number of general-head boundary cells in use during any stress period. MXACTB includes cells that are defined using parameters as well as cells that are defined without using parameters.")]
        /// <summary>
        /// the maximum number of general-head boundary cells in use during any stress period
        /// </summary>
        public int MXACTB { get; set; }    
        [Category("Output")]
        [Description("a flag and a unit number.")]
        public int IGHBCB { get; set; }
        //*****DataSet6
        /// <summary>
        /// [NSP,MXACTC,5] (LAYER,ROW,COL,Bhead,Conductance )
        /// </summary>
        /// 
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube<float> ITMP { get; set; }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override void New()
        {
            var mf = (Owner as Modflow);
            this.IGHBCB = mf.NameManager.GetFID(".cbc");
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
                    var mf = Owner as Modflow;
                    var nsp = mf.TimeService.StressPeriods.Count;
                    //Data Set 1: # OPTIONS
                    string newline = ReadComment(sr);
                    var intbuf = TypeConverterEx.Split<int>(newline, 1);
                    MXACTB = intbuf[0];
                    ITMP = new DataCube<float>(nsp, MXACTB, 5, true);
                    ITMP.ColumnNames = new string[] { "LAYER", "ROW", "COL", "Boundary Head", "Conductance" };
                    ITMP.Variables = new string[nsp];

                    for (int i = 0; i < nsp; i++)
                    {
                        ITMP.Variables[i] = "Stress Period " + (i + 1);
                        newline = sr.ReadLine();
                        intbuf = TypeConverterEx.Split<int>(newline, 2);
                        if (intbuf[0] > 0)
                        {
                            ITMP.Flags[i] = TimeVarientFlag.Individual;
                            ITMP.Allocate(i);
                            for (int j = 0; j < intbuf[0]; j++)
                            {
                                newline = sr.ReadLine();
                                var floatbuf = TypeConverterEx.Split<float>(newline, 5);
                                for (int k = 0; k < 5; k++)
                                {
                                    ITMP[i, j, k] = floatbuf[k];
                                }
                            }
                        }
                        else
                        {
                            ITMP.Flags[i] = TimeVarientFlag.Repeat;
                        }
                    }
                    BuildTopology();
                    sr.Close();      
                    result = LoadingState.Normal;
                    Message = string.Format("{0} loaded", this.Name);
                }
                catch (Exception ex)
                {
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progress);
                    result = LoadingState.Warning;
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
                result = LoadingState.Warning;
            }
            OnLoaded(progress, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }
        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            var nsp = GetNumSP();
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, this.Name);
            string line = string.Format("{0}  {1}  # MXACTB IGHBCB", MXACTB, IGHBCB);
            sw.WriteLine(line);
 
            for (int i = 0; i < nsp; i++)
            {
                if (ITMP.Flags[i] == TimeVarientFlag.Individual)
                {
                    line = string.Format("{0} 0 0 # ITMP NP ", MXACTB, IGHBCB);
                    sw.WriteLine(line);
                    for (int j = 0; j < MXACTB; j++)
                    {
                        line = string.Format("{0}\t{1}\t{2}\t{3}\t{4} # LAYER,ROW,COL,BHead, Cond", ITMP[i, j, 0].ToString("F0"),
                            ITMP[i, j, 1].ToString("F0"), ITMP[i, j, 2].ToString("F0"), ITMP[i, j, 3], ITMP[i, j, 4]);
                        sw.WriteLine(line);
                    }
                }
                else
                {
                    line = "-1 0 0";
                    sw.WriteLine(line);
                }
            }

            sw.Close();
            OnSaved(progress);
        }
        public override string CreateFeature(DotSpatial.Projections.ProjectionInfo proj_info, string directory)
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
            fs.DataTable.Columns.Add(new DataColumn("Bhead", typeof(double)));
            fs.DataTable.Columns.Add(new DataColumn("Cond", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(double)));

            for (int i = 0; i < MXACTB; i++)
            {
                int layer = (int)ITMP[0, i, 0];
                int row = (int)ITMP[0, i, 1];
                int col = (int)ITMP[0, i, 2];
                var coor = grid.LocateCentroid(col, row);
                Point geom = new Point(coor);
                IFeature feature = fs.AddFeature(geom);
                feature.DataRow.BeginEdit();
                feature.DataRow["CELL_ID"] = grid.Topology.GetID(row - 1, col - 1);
                feature.DataRow["Layer"] = layer;
                feature.DataRow["Row"] = row;
                feature.DataRow["Column"] = col;
                feature.DataRow["ID"] = i + 1;
                feature.DataRow["Bhead"] = ITMP[0, i, 3];
                feature.DataRow["Cond"] = ITMP[0, i, 4];
                feature.DataRow[RegularGrid.ParaValueField] = 0;
                feature.DataRow.EndEdit();
            }
            fs.SaveAs(filename, true);
            fs.Close();
            return filename;
        }
        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;
            ITMP = null;
            base.Clear();
        }
        public void BuildTopology()
        {
            var grid = Grid as RegularGrid;
            _CellTopo = new RegularGridTopology();
            int sp = 0;
            _CellTopo.ActiveCellLocation = new int[MXACTB][];
            _CellTopo.ActiveCellID = new int[MXACTB];
            _CellTopo.RowCount = grid.RowCount;
            _CellTopo.ColumnCount = grid.ColumnCount;
            _CellTopo.ActiveCellCount = MXACTB;
            for (int i = 0; i < MXACTB; i++)
            {
                int row = (int)ITMP[sp, i, 1];
                int col = (int)ITMP[sp, i, 2];
                _CellTopo.ActiveCellLocation[i] = new int[] { row - 1, col - 1 };
                _CellTopo.ActiveCellID[i] = grid.Topology.GetID(row - 1, col - 1);
            }
            ITMP.Topology = _CellTopo;
        }
    }
}
