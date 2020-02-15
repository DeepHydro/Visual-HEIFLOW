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
    [PackageCategory("Boundary Conditions,Specified Head", true)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class CHDPackage : MFPackage
    {
        private RegularGridTopology _WellTopo;
        public static string PackageName = "CHD";
        public CHDPackage()
        {
            Name = "CHD";
            _FullName = "Time-Variant Specified-Head Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".chd";
            _PackageInfo.ModuleName = "CHD";
            Description = "The CHD is used to simulate specified head boundaries that can change within or between stress periods.";
            Version = "CHD";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
        }
        [Description("The maximum number of constant-head boundary cells in use during any stress period, including those that are defined using parameters.")]
        /// <summary>
        /// The maximum number of constant-head boundary cells in use during any stress period, including those that are defined using parameters.
        /// </summary>
        public int MXACTC { get; set; }
        public RegularGridTopology Topology { get; set; }
        //*****DataSet6
        /// <summary>
        /// [NSP,MXACTC,5] (LAYER,ROW,COL,SHEAD,EHEAD)
        /// </summary>
        /// 
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube<float> SHEAD { get; set; }

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
                    var mf = Owner as Modflow;
                    var nsp = mf.TimeService.StressPeriods.Count;
                    //Data Set 1: # OPTIONS
                    string newline = ReadComment(sr);
                    var intbuf = TypeConverterEx.Split<int>(newline, 1);
                    MXACTC = intbuf[0];
                    SHEAD = new DataCube<float>(nsp, MXACTC, 5, true);
                    SHEAD.ColumnNames = new string[] { "LAYER", "ROW", "COL", "SHEAD", "EHEAD" };
                    SHEAD.Variables = new string[nsp];

                    for (int i = 0; i < nsp; i++)
                    {
                        SHEAD.Variables[i] = "Stress Period " + (i + 1);
                        newline = sr.ReadLine();
                        intbuf = TypeConverterEx.Split<int>(newline, 3);
                        if (intbuf[0] > 0)
                        {
                            SHEAD.Flags[i] = TimeVarientFlag.Individual;
                            SHEAD.Allocate(i);
                            for (int j = 0; j < intbuf[0]; j++)
                            {
                                newline = sr.ReadLine();
                                var floatbuf = TypeConverterEx.Split<float>(newline, 5);
                                for (int k = 0; k < 5; k++)
                                {
                                    SHEAD[i, j, k] = floatbuf[k];
                                }
                            }
                        }
                        else
                        {
                            SHEAD.Flags[i] = TimeVarientFlag.Repeat;
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
            string line = string.Format("{0}  # MXACTC", MXACTC);
            sw.WriteLine(line);
 
            for (int i = 0; i < nsp; i++)
            {
                if (SHEAD.Flags[i] == TimeVarientFlag.Individual)
                {
                    line = string.Format("{0} 0 0 # ITMP NP ", MXACTC);
                    sw.WriteLine(line);
                    for (int j = 0; j < MXACTC; j++)
                    {
                        line = string.Format("{0}\t{1}\t{2}\t{3}\t{4} # LAYER,ROW,COL,SHEAD,EHEAD", SHEAD[i, j, 0].ToString("F0"),
                            SHEAD[i, j, 1].ToString("F0"), SHEAD[i, j, 2].ToString("F0"), SHEAD[i, j, 3], SHEAD[i, j, 4]);
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
            fs.DataTable.Columns.Add(new DataColumn("SHEAD", typeof(double)));
            fs.DataTable.Columns.Add(new DataColumn("EHEAD", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(double)));

            for (int i = 0; i < MXACTC; i++)
            {
                int layer = (int)SHEAD[0, i, 0];
                int row = (int)SHEAD[0, i, 1];
                int col = (int)SHEAD[0, i, 2];
                var coor = grid.LocateCentroid(col, row);
                Point geom = new Point(coor);
                IFeature feature = fs.AddFeature(geom);
                feature.DataRow.BeginEdit();
                feature.DataRow["CELL_ID"] = grid.Topology.GetID(row - 1, col - 1);
                feature.DataRow["Layer"] = layer;
                feature.DataRow["Row"] = row;
                feature.DataRow["Column"] = col;
                feature.DataRow["ID"] = i + 1;
                feature.DataRow["SHEAD"] = SHEAD[0, i, 3];
                feature.DataRow["EHEAD"] = SHEAD[0, i, 4];
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
            SHEAD = null;
            base.Clear();
        }
        public void BuildTopology()
        {
            var grid = Grid as RegularGrid;
            _WellTopo = new RegularGridTopology();
            int sp = 0;
            _WellTopo.ActiveCellLocation = new int[MXACTC][];
            _WellTopo.ActiveCellID = new int[MXACTC];
            _WellTopo.RowCount = grid.RowCount;
            _WellTopo.ColumnCount = grid.ColumnCount;
            _WellTopo.ActiveCellCount = MXACTC;
            for (int i = 0; i < MXACTC; i++)
            {
                int row = (int)SHEAD[sp, i, 1];
                int col = (int)SHEAD[sp, i, 2];
                _WellTopo.ActiveCellLocation[i] = new int[] { row - 1, col - 1 };
                _WellTopo.ActiveCellID[i] = grid.Topology.GetID(row - 1, col - 1);
            }
            SHEAD.Topology = _WellTopo;
        }
    }
}
