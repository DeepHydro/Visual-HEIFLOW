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
using Heiflow.Models.Properties;
using NetTopologySuite.Geometries;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [Export(typeof(IMFPackage))]
    [PackageCategory("Boundary Conditions,Specified Flux", false)]
    [CoverageItem]
    public class WELPackage : MFPackage
    {
        public static string PackageName = "WEL";
        private RegularGridTopology _WellTopo;
        public WELPackage()
        {
            IWELCB = 0;
            OptionString = "AUXILIARY IFACE NOPRINT";
            Name = PackageName;
            FullName = "Well Package";

            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".wel";
            _PackageInfo.ModuleName = PackageName;
            IsMandatory = false;
            Version = "WEL1";
            _Layer3DToken = "Well";
            Category = Resources.FluxCategory; 
        }
        /// <summary>
        /// The maximum number of wells in use during any stress period
        /// </summary>
        /// 
        public int MXACTW
        {
            get;
            set;
        }
        /// <summary>
        ///  a flag and a unit number. If IWELCB > 0, it is the unit number to which cell-by-cell flow terms will be written when "SAVE BUDGET" or a non-zero value for ICBCFL is specified in Output Control.
        ///  If IWELCB = 0, cell-by-cell flow terms will not be written.
        ///  If IWELCB < 0, well recharge for each well will be written to the listing file when "SAVE BUDGET" or a non-zero value for ICBCFL is specified in Output Control.
        /// </summary>
        public int IWELCB
        {
            get;
            set;
        }
        /// <summary>
        /// Default value is: AUXILIARY IFACE NOPRINT
        /// </summary>
        public string OptionString
        {
            get;
            set;
        }

        /// <summary>
        /// 3d mat [nsp][1][nwell]
        /// </summary>
        [StaticVariableItem("")]
        [Browsable(false)]
        public DataCube<float> FluxRates
        {
            get;
            set;
        }
        //[StaticVariableItem("")]
        //[Browsable(false)]
        //public DataCube<int> Locations
        //{
        //    get;
        //    set;
        //}

        public RegularGridTopology Topology
        {
            get
            {
                return _WellTopo;
            }
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            base.Initialize();
        }

        public override string CreateFeature(ProjectionInfo proj_info, string directory)
        {
            if (FluxRates != null && this.State== ModelObjectState.Ready)
            {
                string filename = Path.Combine(directory, this.Name + ".shp");
                var grid = (Owner as Modflow).Grid as MFGrid;
                int fea_sp = 0;
                FeatureSet fs = new FeatureSet(FeatureType.Point);
                fs.Name = this.Name;
                fs.Projection = proj_info;
                fs.DataTable.Columns.Add(new DataColumn("CELL_ID", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Layer", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Row", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Column", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Elevation", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("Name", typeof(string)));
                fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(double)));
                int np = TimeService.StressPeriods.Count;
                for (int i = 0; i < np; i++)
                {
                    fs.DataTable.Columns.Add(new DataColumn("Flux Rate" + (i + 1), typeof(float)));
                }
                for (int i = 0; i < np; i++)
                {
                    if (FluxRates.Flags[i] == TimeVarientFlag.Individual)
                    {
                        fea_sp = i;
                        break;
                    }
                }
                for (int i = 0; i < MXACTW; i++)
                {
                    int layer = (int)FluxRates[0, fea_sp, i];
                    int row = (int)FluxRates[1, fea_sp, i];
                    int col = (int)FluxRates[2, fea_sp, i];
                    var coor = grid.LocateCentroid(col, row);
                    Point geom = new Point(coor);
                    IFeature feature = fs.AddFeature(geom);
                    feature.DataRow.BeginEdit();
                    feature.DataRow["CELL_ID"] = grid.Topology.GetID(row - 1, col - 1);
                    feature.DataRow["Layer"] = layer;
                    feature.DataRow["Row"] = row;
                    feature.DataRow["Column"] = col;
                    feature.DataRow["ID"] = i + 1;
                    feature.DataRow["Elevation"] = 0;// grid.GetElevationAt(row - 1, col - 1, layer - 1);
                    feature.DataRow["Name"] = "Well" + (i + 1);
                    feature.DataRow[RegularGrid.ParaValueField] = 0;
                    for (int j = 0; j < np; j++)
                    {
                        if (FluxRates.Flags[j] == TimeVarientFlag.Individual)
                        {
                            feature.DataRow["Flux Rate" + (j + 1)] = FluxRates[3, j, i];
                        }
                        else if (FluxRates.Flags[j] == TimeVarientFlag.Constant)
                        {
                            feature.DataRow["Flux Rate" + (j + 1)] = FluxRates.Constants[j];
                        }
                        else if (FluxRates.Flags[j] == TimeVarientFlag.Repeat)
                        {
                            feature.DataRow["Flux Rate" + (j + 1)] = FluxRates[3, j, i];
                        }
                    }
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

        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                var grid = this.Grid as MFGrid;
                int np = TimeService.StressPeriods.Count;
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    var line = ReadComment(sr);
                    var ss = TypeConverterEx.Split<float>(line, 2);
                    MXACTW = (int)ss[0];
                    IWELCB = (int)ss[1];

                    FluxRates = new DataCube<float>(4, np, MXACTW)
                    {
                        DateTimes = new System.DateTime[np],
                        ZeroDimension = DimensionFlag.Time,
                        Variables = new string[4] { "Layer", "Row", "Column", "Q" }
                    };
                    //need to rest flags length
                    FluxRates.InitFlags(np);

                    for (int n = 0; n < np; n++)
                    {
                        ss = TypeConverterEx.Split<float>(sr.ReadLine(), 2);
                        int nwel = (int)ss[0];
                        if (nwel > 0)
                        {
                            FluxRates.Flags[n] = TimeVarientFlag.Individual;
                            FluxRates.Multipliers[n] = 1;
                            FluxRates.IPRN[n] = -1;

                            for (int i = 0; i < nwel; i++)
                            {
                                var vv = TypeConverterEx.Split<float>(sr.ReadLine());
                                FluxRates[0, n, i] = vv[0];
                                FluxRates[1, n, i] = vv[1];
                                FluxRates[2, n, i] = vv[2];
                                FluxRates[3, n, i] = vv[3];
                            }
                        }
                        else if (nwel == 0)
                        {
                            FluxRates.Flags[n] = TimeVarientFlag.Constant;
                            FluxRates.Multipliers[n] = 1;
                            FluxRates.IPRN[n] = -1;
                        }
                        else
                        {
                            FluxRates.Flags[n] = TimeVarientFlag.Repeat;
                            FluxRates.Multipliers[n] = ss[1];
                            FluxRates.IPRN[n] = -1;
                            //var size = FluxRates.GetVariableSize(n - 1);
                            //var buf = new float[size[1]];
                            //FluxRates[n - 1, "0", ":"].CopyTo(buf, 0);
                            //FluxRates[n, "0", ":"] = buf;
                        }
                        FluxRates.DateTimes[n] = TimeService.StressPeriods[n].End;
                    }
                    BuildTopology();
                    result = LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    result = LoadingState.Warning;
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progresshandler);
                }
                finally
                {
                    sr.Close();
                }
            }
            else
            {
                ShowWarning("Failed to load", progresshandler);
                result = LoadingState.Warning;
            }
            OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }

        public override void CompositeOutput(MFOutputPackage mfout)
        {
            if (mfout == null)
                return;
            var cbc = mfout.SelectChild(CBCPackage.PackageName);
            if(cbc != null)
                this.IWELCB = cbc.PackageInfo.FID;
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, "WEL");
            string line = "";
            line = string.Format("{0} {1} {2} # DataSet 2: MXACTW IWELCB Option", MXACTW, IWELCB, OptionString);
            sw.WriteLine(line);
            int np = TimeService.StressPeriods.Count;
            for (int i = 0; i < np; i++)
            {
               if( FluxRates.Flags[i] == TimeVarientFlag.Individual)
                {
                    var nwel= FluxRates.GetSpaceDimLength(i, 0);
                    line = string.Format("{0} {1} # Data Set 5: ITMP NP Stress period {2} ", nwel, 0, i + 1);
                    sw.WriteLine(line);
                    for (int j = 0; j < nwel; j++)
                    {
                        line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", (int)FluxRates[0, i, j], (int)FluxRates[1, i, j], (int)FluxRates[2, i, j], FluxRates[3, i, j], 0);
                        sw.WriteLine(line);
                    }
                }
                else if (FluxRates.Flags[i] == TimeVarientFlag.Constant)
                {
                    line = string.Format("{0} {1} # Data Set 5: ITMP NP Stress period {2} ", 0, 0, i + 1);
                    sw.WriteLine(line);
                }
                else if (FluxRates.Flags[i] == TimeVarientFlag.Repeat)
                {
                    line = string.Format("{0} 0 0 # Data Set 5: ITMP NP Stress period {1} ", -1, i + 1);
                    sw.WriteLine(line);
                }
            }
            sw.Close();
        }

        public override void Clear()
        {
            base.Clear();
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            InitTVArrays();
            base.OnTimeServiceUpdated(time);
        }


        private void InitTVArrays()
        {
            int np = TimeService.StressPeriods.Count;
            this.FluxRates = new DataCube<float>(4, np, 1) { ZeroDimension = DimensionFlag.Time };
            for (int n = 0; n < np; n++)
            {
                FluxRates.Variables[n] = "Flux Rate" + (n + 1);
            }
        }

        public void BuildTopology()
        {
            var grid = Grid as RegularGrid;
            _WellTopo = new RegularGridTopology();
            int sp = 0;
            for (int i = 0; i < FluxRates.Size[1]; i++)
            {
                if (FluxRates.Flags[i] == TimeVarientFlag.Individual)
                {
                    sp = i;
                    break;
                }
            }
            _WellTopo.ActiveCellLocation = new int[MXACTW][];
            _WellTopo.ActiveCellID = new int[MXACTW];
            _WellTopo.RowCount = grid.RowCount;
            _WellTopo.ColumnCount = grid.ColumnCount;
            _WellTopo.ActiveCellCount = MXACTW;
            for (int i = 0; i < MXACTW; i++)
            {
                int row = (int)FluxRates[1, sp, i];
                int col = (int)FluxRates[2, sp, i];
                _WellTopo.ActiveCellLocation[i] = new int[] { row - 1, col - 1 };
                _WellTopo.ActiveCellID[i] = grid.Topology.GetID(row - 1, col - 1);
            }
            FluxRates.Topology = _WellTopo;
        }

    }
}