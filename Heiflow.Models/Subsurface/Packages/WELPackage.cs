// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using DotSpatial.Projections;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using NetTopologySuite.Geometries;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [Export(typeof(IMFPackage))]
    [PackageCategory("Boundary Conditions,Specified Flux",false)]
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
        }
        public int MXACTW { get; set; }
        public int IWELCB { get; set; }
        /// <summary>
        /// Default value is: AUXILIARY IFACE NOPRINT
        /// </summary>
        public  string OptionString { get; set; }
        [Description("Number of wells")]
        public int NWell { get; set; }
        /// <summary>
        /// 3d mat [nsp][1][nwell]
        /// </summary>
        [StaticVariableItem("")]
        [Browsable(false)]
        public MyVarient3DMat<float> FluxRates
        {
            get;
            set;
        }
        [StaticVariableItem("")]
        [Browsable(false)]
        public MyVarient4DMat<int> Locations
        {
            get;
            set;
        }
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
            if (Locations != null)
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
                fs.DataTable.Columns.Add(new DataColumn("Elevation", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("Name", typeof(string)));
                fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(int)));
                int np = TimeService.StressPeriods.Count;
                for (int i = 0; i < np; i++)
                {
                    fs.DataTable.Columns.Add(new DataColumn("Flux Rate" + (i + 1), typeof(float)));
                }

                for (int i = 0; i < NWell; i++)
                {
                    int layer = Locations.Value[0][0][i][0];
                    int row = Locations.Value[0][0][i][1];
                    int col = Locations.Value[0][0][i][2];
                    var coor = grid.LocateCentroid(col, row);
                    Point geom = new Point(coor);
                    IFeature feature = fs.AddFeature(geom);
                    feature.DataRow.BeginEdit();
                    feature.DataRow["CELL_ID"] = grid.Topology.GetID(row - 1, col - 1);
                    feature.DataRow["Layer"] = layer;
                    feature.DataRow["Row"] = row;
                    feature.DataRow["Column"] = col;
                    feature.DataRow["ID"] = i + 1;
                    feature.DataRow["Elevation"] = grid.GetElevationAt(row - 1, col - 1, layer - 1);
                    feature.DataRow["Name"] = "Well" + (i + 1);
                    feature.DataRow[RegularGrid.ParaValueField] = 0;
                    for (int j = 0; j < np; j++)
                    {
                        if(FluxRates.Flags[j,0] ==  TimeVarientFlag.Individual)
                        {
                            feature.DataRow["Flux Rate" + (j + 1)] = FluxRates.Value[j][0][i];
                        }
                        else if (FluxRates.Flags[j, 0] == TimeVarientFlag.Constant)
                        {
                            feature.DataRow["Flux Rate" + (j + 1)] = FluxRates.Constants[j,0];
                        }
                        else if (FluxRates.Flags[j, 0] == TimeVarientFlag.Repeat)
                        {
                            int vv = FluxRates.RepeatAt(j);
                            feature.DataRow["Flux Rate" + (j + 1)] = FluxRates.Value[vv][0][i];
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


        public override bool Load()
        {
            if (File.Exists(FileName))
            {
                var grid = this.Grid as MFGrid;
                int np = TimeService.StressPeriods.Count;
                StreamReader sr = new StreamReader(FileName);
                var line = ReadComment(sr);
                var ss = TypeConverterEx.Split<int>(line, 2);
                MXACTW = ss[0];
                IWELCB = ss[1];

                for (int n = 0; n < np; n++)
                {
                    line = sr.ReadLine();
                    ss = TypeConverterEx.Split<int>(line, 2);
                    if (ss[0] > 0)
                    {
                        NWell = ss[0];
                        Locations = new MyVarient4DMat<int>(1, 1);
                        Locations.AllocateSpaceDim(0, 0, NWell, 3);
                        Locations.Variables = new string[] { "Well Locations" };
                        Locations.ColumnNames = new string[] { "Layer", "Row", "Column" };
                        Locations.AllowTableEdit = true;
                        Locations.TimeBrowsable = false;
                       
                        for (int i = 0; i < NWell; i++)
                        {
                            var vv = TypeConverterEx.Split<int>(sr.ReadLine(), 3);
                            Locations.Value[0][0][i][0] = vv[0];
                            Locations.Value[0][0][i][1] = vv[1];
                            Locations.Value[0][0][i][2] = vv[2];                                          
                        }
                        break;
                    }
                }
                sr.Close();

                Load2Mat4d();
                State = ModelObjectState.Ready;
                OnLoaded("Sucessfully loaded");
                return true;
            }
            else
            {
                State = ModelObjectState.Error;
                OnLoadFailed("Failed to load");
                return false;
            }
        }

        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var cbc = mfout.SelectChild(CBCPackage.CBCName);
            this.IWELCB = cbc.PackageInfo.FID;
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

        private void Load2Mat4d()
        {
            if (File.Exists(FileName))
            {
                var grid = this.Grid as MFGrid;
                int np = TimeService.StressPeriods.Count;
                StreamReader sr = new StreamReader(FileName);
                var line = ReadComment(sr);
                var ss = TypeConverterEx.Split<float>(line, 2);
                MXACTW = (int) ss[0];
                IWELCB =  (int)ss[1];

                FluxRates = new MyVarient3DMat<float>(np, 1);
                FluxRates.DateTimes = new System.DateTime[np];
                FluxRates.TimeBrowsable = true;
                FluxRates.AllowTableEdit = false;

                for (int n = 0; n < np; n++)
                {
                    FluxRates.Variables[n] = "Flux Rate" + (n + 1);
                    ss = TypeConverterEx.Split<float>(sr.ReadLine(), 2);
                    int nwel = (int)ss[0];
                    if (nwel > 0)
                    {
                        FluxRates.AllocateSpaceDim(n, 0, nwel);
                        FluxRates.Flags[n, 0] = TimeVarientFlag.Individual;
                        FluxRates.Multipliers[n, 0] = ss[1];
                        FluxRates.IPRN[n, 0] = -1;
                        for (int i = 0; i < nwel; i++)
                        {
                            var vv = TypeConverterEx.Split<float>(sr.ReadLine());
                            FluxRates.Value[n][0][i] = vv[3];
                        }
                    }
                    else if (nwel == 0)
                    {
                        FluxRates.Flags[n, 0] = TimeVarientFlag.Constant;
                        FluxRates.Multipliers[n, 0] =1;
                        FluxRates.IPRN[n, 0] = -1;
                    }
                    else
                    {
                        FluxRates.Flags[n, 0] = TimeVarientFlag.Repeat;
                        FluxRates.Multipliers[n, 0] = ss[1];
                        FluxRates.IPRN[n, 0] = -1;
                    }
                }

                sr.Close();
                BuilTopology();
            }
        }

        private void InitTVArrays()
        {
               int np = TimeService.StressPeriods.Count;
               this.FluxRates = new  MyVarient3DMat<float>(np, 1);
               for (int n = 0; n < np; n++)
               {
                   FluxRates.Variables[n] = "Flux Rate" + (n + 1);
               }
        }

        public void BuilTopology()
        {
            var grid = Grid as RegularGrid;
            _WellTopo = new RegularGridTopology();
            int sp = 0;
            for (int i = 0; i < FluxRates.Size[0]; i++)
            {
                if (FluxRates.Flags[i, 0] == TimeVarientFlag.Individual)
                {
                    sp = i;
                    break;
                }
            }
            _WellTopo.ActiveCell = new int[NWell][];
            _WellTopo.ActiveCellIDs = new int[NWell];
            _WellTopo.RowCount = grid.RowCount;
            _WellTopo.ColumnCount = grid.ColumnCount;
            _WellTopo.ActiveCellCount = NWell;
            for (int i = 0; i < NWell; i++)
            {
                int row = (int)Locations.Value[0][0][i][1];
                int col = (int)Locations.Value[0][0][i][2];
                _WellTopo.ActiveCell[i] = new int[] { row - 1, col - 1 };
                _WellTopo.ActiveCellIDs[i] = grid.Topology.GetID(row - 1, col - 1);
            }
            FluxRates.Topology = _WellTopo;
        }

    }
}
