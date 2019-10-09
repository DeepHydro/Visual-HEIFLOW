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
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.UI;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [Export(typeof(IMFPackage))]
    [PackageCategory("Boundary Conditions,Head Dependent Flux",false)]
    public class SFRPackage : MFPackage
    {
        public static string PackageName = "SFR";
        private RegularGridTopology _SegTopo;
        private RegularGridTopology _ReachTopo;
        public SFRPackage()
        {
            Name = "SFR";
            _FullName = "Streamflow-Routing Package";
            InputFilesInfo = new List<PackageInfo>();
            DefaultAttachedVariables = new string[] { "Flow into stream", "Stream loss", "Flow out of stream", "Overland runoff","Direct pricipitation", "Stream ET"
            ,"Stream head", "Stream depth","Stream width", "Stream conductance", "Flow to water table", "Change of unsat. stor.", "Groundwater head"};
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".sfr";
            _PackageInfo.ModuleName = "SFR";
            IsMandatory = true;
            Version = "SFR2";

            EnableREACHINPUT = false;
            EnableTRANSROUTE = true;
            NSFRPAR = 0;
            NPARSEG = 0;
            CONST = 86400;
            DLEAK = 0.01f;
            ISTCB1 = 9;
            ISTCB2 = 58;
            ISFROPT = 2;
            NSTRAIL = 5;
            ISUZN = 1;
            NSFRSETS = 30;
            IRTFLG = 1;
            NUMTIM = 24;
            WEIGHT = 0.9f;
            FLWTOL = 0.01f;

            _SegTopo = new RegularGridTopology();
            _ReachTopo = new RegularGridTopology();
            _Layer3DToken = "SFR";
        }

        #region Properties
        //# Data Set 1: NSTRM NSS NSFRPAR NPARSEG CONST DLEAK ISTCB1  ISTCB2 ISFROPT NSTRAIL ISUZN NSFRSETS IRTFLG NUMTIM WEIGHT FLWTOL   
        //   10676 389 0 0 8.640000000000E+004  0.01        9         58          2            5             1        30            0            1            24 0          .9        2.5  IFACE 
        public bool EnableREACHINPUT { get; set; }
        public bool EnableTRANSROUTE { get; set; }
        public int NUMTAB { get; set; }
        public int MAXVAL { get; set; }
        public int NSTRM { get; set; }
        public int NSS { get; set; }
        public int NSFRPAR { get; set; }
        public int NPARSEG { get; set; }
        public float CONST { get; set; }
        public float DLEAK { get; set; }
        /// <summary>
        /// cbc 
        /// </summary>
        public int ISTCB1 { get; set; }
        /// <summary>
        /// An integer value used as a flag for writing to a separate formatted file all information on inflows and outflows from each reach; 
        /// on stream depth, width, and streambed conductance; and on head difference and gradient across the streambed
        /// </summary>
        public int ISTCB2 { get; set; }
        /// <summary>
        /// defines the input structureUnsaturated flow is simulated for ISFROPT ≥ 2;
        /// unsaturated flow is not simulated for ISFROPT = 0 or 1. 
        /// </summary>
        public int ISFROPT { get; set; }
        /// <summary>
        /// An integer value that is the number of trailing wave increments used to represent a trailing wave. 
        /// </summary>
        public int NSTRAIL { get; set; }
        /// <summary>
        /// the maximum number of vertical cells used to the define the unsaturated zone beneath a stream reach. If ICALC is 1 for all segments then ISUZN should be set to 1.
        /// </summary>
        public int ISUZN { get; set; }
        /// <summary>
        /// the maximum number of different sets of trailing waves used to allocate arrays.
        /// </summary>
        public int NSFRSETS { get; set; }
        /// <summary>
        /// whether transient streamflow routing is active. 
        /// If IRTFLG > 0 then streamflow will be routed using the kinematic-wave equation (see USGS Techniques and Methods 6-D1, p. 68-69); 
        /// otherwise, IRTFLG should be specified as 0.
        /// </summary>
        public int IRTFLG { get; set; }
        /// <summary>
        ///  the number of sub time steps used to route streamflow. The time step that will be used to route streamflow will be equal to the MODFLOW time step divided by NUMTIM.
        /// </summary>
        public int NUMTIM { get; set; }
        /// <summary>
        /// A real number equal to the time weighting factor used to calculate the change in channel storage. 
        /// WEIGHT has a value between 0.5 and 1. See equation 83 in USGS Techniques and Methods 6-D1 for further details.
        /// </summary>
        public float WEIGHT { get; set; }
        /// <summary>
        /// A real number equal to the streamflow tolerance for convergence of the kinematic wave equation used for transient streamflow routing.
        /// </summary>
        public float FLWTOL { get; set; }

        [Browsable(false)]
        public RiverNetwork RiverNetwork { get; set; }
        [Browsable(false)]
        public PackageInfo OutputDataFile { get; set; }
        [Browsable(false)]
        public string[] DefaultAttachedVariables { get; private set; }
        /// <summary>
        /// [NUMTAB,3]: SEGNUM NUMVAL IUNIT
        /// </summary>
        [Browsable(false)]
        public int[,] TablesInfo { get; set; }

        [Browsable(false)]
        public List<River> skippedRiver;

        [Browsable(false)]    
        public IDataPackage DataPackage
        {
            get;
            set;
        }

        [Browsable(false)]
        [VariablesFolderItem("Stream Inflows")]
        public SFRInputPackage StreamInflows
        {
            get;
            private set;
        }
        [Browsable(false)]
        public List<PackageInfo> InputFilesInfo { get; set; }

        [Browsable(false)]
        [PackageOptionalViewItem("SFR")]
        public override IPackageOptionalView OptionalView
        {
            get;
            set;
        }

        [StaticVariableItem]
        public DataCube<float> Segments
        {
            get;
            private set;
        }

        [StaticVariableItem]
        public DataCube<float> Reaches
        {
            get;
            private set;
        }

        public RegularGridTopology ReachTopology
        {
            get
            {
                return _ReachTopo;
            }
        }

        public RegularGridTopology SegTopology
        {
            get
            {
                return _SegTopo;
            }
        }
        #endregion

        public override bool New()
        {
            var mf = Owner as Modflow;
            this.ISTCB1 = mf.NameManager.GetFID(".cbc");
            this.ISTCB2 = mf.NameManager.NextFID();
            var pckinfo = new PackageInfo();
            pckinfo.ModuleName = "DATA";
            pckinfo.FID = this.ISTCB2;
            pckinfo.Format = FileFormat.Text;
            pckinfo.IOState = IOState.REPLACE;
            pckinfo.FileName = Modflow.OutputDic + mf.Project.Name + ".sft_out";
            pckinfo.FileExtension = ".sft_out";
            pckinfo.Name = Path.GetFileName(pckinfo.FileName);
            pckinfo.WorkDirectory = mf.WorkDirectory;
            mf.NameManager.AddInSilence(pckinfo);
            base.New();
            return true;
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            StreamInflows = new SFRInputPackage(this)
            {
                Owner = this.Owner
            };
            StreamInflows.Initialize();
            Children.Add(StreamInflows);
            base.Initialize();
        }

        public override string CreateFeature(ProjectionInfo proj_info, string directory)
        {
            if (RiverNetwork != null)
            {
                string filename = Path.Combine(directory, this.Name + ".shp");
                var grid = (Owner as Modflow).Grid as MFGrid;
                FeatureSet fs = new FeatureSet(FeatureType.Polygon);
                fs.Name = this.Name;
                fs.Projection = proj_info;
                fs.DataTable.Columns.Add(new DataColumn("CELL_ID", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("ISEG", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("IRCH", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("JRCH", typeof(int)));

                fs.DataTable.Columns.Add(new DataColumn("BedThick", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("TopElev", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("Slope", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("Width", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("Length", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(double)));

                for (int i = 0; i < RiverNetwork.RiverCount; i++)
                {
                    var river=RiverNetwork.Rivers[i];
                    for (int j = 0; j < river.Reaches.Count; j++)
                    {
                        var reach = river.Reaches[j];
                        var vertices = grid.LocateNodes(reach.JRCH - 1, reach.IRCH - 1);
                        ILinearRing ring = new LinearRing(vertices);
                        Polygon geom = new Polygon(ring);
                        IFeature feature = fs.AddFeature(geom);
                        feature.DataRow.BeginEdit();
                        feature.DataRow["CELL_ID"] = grid.Topology.GetID(reach.IRCH - 1, reach.JRCH - 1);
                        feature.DataRow["ISEG"] = reach.ISEG;
                        feature.DataRow["IRCH"] = reach.IRCH;
                        feature.DataRow["JRCH"] = reach.JRCH;
                        feature.DataRow["BedThick"] = reach.BedThick;
                        feature.DataRow["TopElev"] = reach.TopElevation;
                        feature.DataRow["Slope"] = reach.Slope;
                        feature.DataRow["Width"] = reach.Width;
                        feature.DataRow["Length"] = reach.Length;
                        feature.DataRow[RegularGrid.ParaValueField] = 0;
                        feature.DataRow.EndEdit();
                    }
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
        public override bool Load(ICancelProgressHandler progresshandler)
        {
            var mf = (Owner as Modflow);
            if (File.Exists(FileName))
            {
                InputFilesInfo.Clear();
                RiverNetwork = LoadNetwork(FileName);
             
                var info = from pi in mf.NameManager.MasterList where pi.FID == ISTCB2 select pi;

                for (int i = 0; i < NUMTAB; i++)
                {
                    info = from pi in mf.NameManager.MasterList where pi.FID == TablesInfo[i, 2] select pi;
                    if (info.Count() == 1)
                    {
                        InputFilesInfo.Add(info.First());
                    }
                }
                NetworkToMat();
                BuildTopology();
                OnLoaded(progresshandler);
                return true;
            }
            else
            {
                this.ISTCB1 = mf.NameManager.GetFID(".cbc");
                this.ISTCB2 = mf.NameManager.GetFID(".sft_out");
                OnLoadFailed("Failed to load "+ this.Name, progresshandler);
                return false;
            }
        }
        public override bool SaveAs(string filename, ICancelProgressHandler prg)
        {
            var grid = Owner.Grid as MFGrid;
            int np = TimeService.StressPeriods.Count;
            var rvnet = RiverNetwork;
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, this.Name);

            NSS = rvnet.Rivers.Count;
            NSTRM = rvnet.GetReachCount();

            string newline = "";
            if (EnableREACHINPUT)
            {
                newline += "REACHINPUT\t";
            }
            if (EnableTRANSROUTE)
            {
                newline += "TRANSROUTE\t";
            }
            if (newline != "")
                sw.WriteLine(newline);
            if (NUMTAB > 0)
            {
                newline = string.Format("TABFILES\t{0}\t{1}", NUMTAB, MAXVAL);
                sw.WriteLine(newline);
            }

            //# Data Set 1: NSTRM NSS NSFRPAR NPARSEG CONST DLEAK ISTCB1  ISTCB2 ISFROPT NSTRAIL ISUZN NSFRSETS IRTFLG NUMTIM WEIGHT FLWTOL
            string format = "";
            for (int i = 0; i < 16; i++)
            {
                format += "{" + i + "}\t";
            }
            format += "# Data Set 1: NSTRM NSS NSFRPAR NPARSEG CONST DLEAK ISTCB1  ISTCB2 ISFROPT NSTRAIL ISUZN NSFRSETS IRTFLG NUMTIM WEIGHT FLWTOL";
            newline = string.Format(format, NSTRM, NSS, NSFRPAR, NPARSEG,
              CONST, DLEAK, ISTCB1, ISTCB2, ISFROPT, NSTRAIL,
                ISUZN, NSFRSETS, IRTFLG, NUMTIM, WEIGHT, FLWTOL);
            sw.WriteLine(newline);

            //# Data Set 2: KRCH IRCH JRCH ISEG IREACH RCHLEN STRTOP SLOPE STRTHICK STRHC1 THTS THTI EPS IFACE Defined by object: reach_1
            format = "";
            for (int i = 0; i < 14; i++)
            {
                format += "{" + i + "}\t";
            }
            foreach (var river in rvnet.Rivers)
            {
                foreach (var reach in river.Reaches)
                {
                    reach.Slope = reach.Slope < 0.0001 ? 0.0001 : reach.Slope;
                    newline = string.Format(format, reach.KRCH, reach.IRCH, reach.JRCH, reach.ISEG, reach.IREACH, reach.Length.ToString("E5"), reach.TopElevation.ToString("E5"), reach.Slope.ToString("E5"),
                        reach.BedThick, reach.STRHC1.ToString("E5"), reach.THTS, reach.THTI, reach.EPS, reach.IFACE);
                    sw.WriteLine(newline);
                }
            }

            format = "";
            for (int i = 0; i < 9; i++)
            {
                format += "{" + i + "}\t";
            }
            format += "# Data Set 6a: NSEG ICALC OUTSEG IUPSEG FLOW RUNOFF ETSW PPTSW ROUGHCH";

            string format1 = "";
            for (int i = 0; i < 10; i++)
            {
                format1 += "{" + i + "}\t";
            }
            format1 += "# Data Set 6a: NSEG ICALC OUTSEG IUPSEG FLOW RUNOFF ETSW PPTSW ROUGHCH";
            for (int i = 0; i < np; i++)
            {
                var spi = MatrixExtension<int>.GetRow(i, SPInfo);
                //if (spi[0] > 0)
                spi[0] = NSS;
                newline = string.Join("\t", spi) + "\t# Data Set 5, Stress period " + (i + 1) + ": ITMP IRDFLG IPTFLG";
                sw.WriteLine(newline);

                if (i == 0)
                {
                    foreach (var river in rvnet.Rivers)
                    {
                        if (river.UpRiverID != 0)
                            newline = string.Format(format1, river.ID, river.ICALC, river.OutRiverID, river.UpRiverID, river.IPrior, river.Flow, river.Runoff, river.ETSW, river.PPTSW, river.ROUGHCH);
                        else
                            newline = string.Format(format, river.ID, river.ICALC, river.OutRiverID, river.UpRiverID, river.Flow, river.Runoff, river.ETSW, river.PPTSW, river.ROUGHCH);
                        sw.WriteLine(newline);
                        newline = string.Format("{0}\t# Data set 6b: WIDTH1", river.Width1);
                        sw.WriteLine(newline);
                        newline = string.Format("{0}\t# Data set 6c: WIDTH2", river.Width2);
                        sw.WriteLine(newline);
                        if (NUMTAB > 0)
                        {
                            for (int t = 0; t < NUMTAB; t++)
                            {
                                newline = string.Format("{0}\t{1}\t{2}", TablesInfo[t, 0], TablesInfo[t, 1], TablesInfo[t, 2]);
                                sw.WriteLine(newline);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var river in rvnet.Rivers)
                    {
                        if (river.UpRiverID != 0)
                            newline = string.Format(format1, river.ID, river.ICALC, river.OutRiverID, river.UpRiverID, river.IPrior, 0, 0, 0,0, river.ROUGHCH);
                        else
                            newline = string.Format(format, river.ID, river.ICALC, river.OutRiverID, river.UpRiverID, 0, 0, 0, 0, river.ROUGHCH);
                        sw.WriteLine(newline);
                    }
                }
            }
            sw.Close();
            OnSaved(prg);
            return true;
        }
        public override void Clear()
        {
            if (_Initialized)
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            EnableREACHINPUT = false;
            NUMTAB = 0;
            MAXVAL = 0;
            base.Clear();
        }

        public override void Remove()
        {
            Clear();
            var mf = Owner as Modflow; 
            mf.NameManager.RemoveBy(this.ISTCB2);
            base.Remove();
        }

        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var mf = Owner as Modflow;
            if (this.ISTCB2 > 0)
            {
                var sfr_info = (from info in mf.NameManager.MasterList where info.FID == this.ISTCB2 select info).First();
               // sfr_info.ModuleName = SFRPackage.PackageName;
                var sfr_out = new SFROutputPackage(this)
                {
                    Owner = mf,
                    PackageInfo = sfr_info,
                    FileName = sfr_info.FileName,
                    Parent=mfout
                };
                sfr_out.Initialize();
             //   sfr_out.Scan();
                this.DataPackage = sfr_out;
            }
        }

        public void Export2SWMM(string filename)
        {
            RiverNetwork.NetworkToSWMM(filename);
        }

        # region Build network
        /// <summary>
        /// Read parameters and load river network
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private RiverNetwork LoadNetwork(string filename)
        {
            if (File.Exists(filename))
            {
                RiverNetwork net = new RiverNetwork();
                StreamReader sr = new StreamReader(filename);
                var mf = Owner as Modflow;
                var grid = Grid as RegularGrid;
                string line = ReadComment(sr);
                int tables = 0;
                string[] strs = null;

                if (line.Contains("REACHINPUT"))
                {
                    EnableREACHINPUT = true;
                    if (line.Contains("TRANSROUTE"))
                        EnableTRANSROUTE = true;
                    else
                        EnableTRANSROUTE = false;
                }

                line = sr.ReadLine().ToUpper();
                if (line.Contains("TABFILES"))
                {
                    strs = TypeConverterEx.Split<string>(line, 3);
                    tables = int.Parse(strs[1]);
                    NUMTAB = tables;
                    MAXVAL = int.Parse(strs[2]);
                    
                    line = sr.ReadLine();
                }
            
                var vv = TypeConverterEx.Split<float>(line, 16);

                net.ReachCount = (int)vv[0];
                net.RiverCount = (int)vv[1];

                NSTRM = net.ReachCount;
                NSS = net.RiverCount;

                NSFRPAR = (int)vv[2];
                NPARSEG = (int)vv[3];
                CONST = vv[4];
                DLEAK = vv[5];
                ISTCB1 = (int)vv[6];
                ISTCB2 = (int)vv[7];

                if (ISTCB2 > 0)
                {
                    var pck = (from pp in mf.NameManager.MasterList where pp.FID == ISTCB2 select pp).First();
                    OutputDataFile = pck;
                }

                ISFROPT = (int)vv[8];
                NSTRAIL = (int)vv[9];
                ISUZN = (int)vv[10];
                NSFRSETS = (int)vv[11];
                IRTFLG = (int)vv[12];
                NUMTIM = (int)vv[13];

                WEIGHT = vv[14];
                FLWTOL = vv[15];

                int oldseg = 0;
                line = sr.ReadLine();
                strs = TypeConverterEx.Split<string>(line);
                int iseg = int.Parse(strs[3]);
                int ireach = int.Parse(strs[4]);
                oldseg = iseg;
                int nodeIndex = 1;
                int riverIndex = 0;
         
                River newriver = new River(iseg)
                {
                    Name = iseg.ToString(),
                    SubIndex = riverIndex
                };
                net.Rivers.Add(newriver);

                Reach newreach = ParseReach(strs, 1);
                CreateReachJunctions(newreach, nodeIndex, null);
                // KRCH IRCH JRCH ISEG IREACH RCHLEN STRTOP SLOPE STRTHICK STRHC1 THTS THTI EPS IFACE
                newreach.Name = newreach.ID.ToString();
                newreach.SubIndex = 0;
                newreach.SubID = 1;
                newriver.AddReach(newreach);

                int i = 1;
                try
                {
              
                    for (i = 1; i < net.ReachCount; i++)
                    {
                        line = sr.ReadLine();
                        strs = TypeConverterEx.Split<string>(line);
                        iseg = int.Parse(strs[3]);

                        newreach = ParseReach(strs, i + 1);
                        newreach.Name = newreach.ID.ToString();
                        newreach.SubIndex = newreach.IREACH - 1;
                        if (oldseg == iseg)
                        {
                            nodeIndex++;
                            newriver.AddReach(newreach);
                        }
                        else
                        {
                            oldseg = iseg;
                            nodeIndex++;
                            riverIndex++;
                            newriver = new River(iseg);
                            newriver.SubIndex = riverIndex;
                            net.Rivers.Add(newriver);
                            newriver.AddReach(newreach);
                        }

                    }
                    foreach (var river in net.Rivers)
                        net.Reaches.AddRange(river.Reaches);
                    ConnectRivers(sr, net);
                  
                    sr.Close();
                }
                catch
                {
                    Debug.WriteLine(i);
                }
                return net;
            }
            else
            {
                return null;
            }
        }

        public void NetworkToMat()
        {
            Reaches = new DataCube<float>(13, 1, RiverNetwork.ReachCount)
            {
                Name = "Reaches", ZeroDimension= DimensionFlag.Spatial
            };
            Segments = new DataCube<float>(11, 1, RiverNetwork.RiverCount)
            {
                Name = "Segments",                ZeroDimension = DimensionFlag.Spatial
            };
            Reaches.DataCubeValueChanged += Reaches_DataCubeValueChanged;
            Reaches.Topology = this.ReachTopology;
            Reaches.Variables = new string[] { "KRCH", "IRCH", "JRCH", "ISEG", "IREACH", "RCHLEN", "STRTOP", 
                "SLOPE", "STRTHICK", "STRHC1", "THTS","THTI","EPS" };
            Segments.Topology = this.SegTopology;
            Segments.Variables = new string[] { "NSEG", "ICALC", "OUTSEG", "IUPSEG", "FLOW", "RUNOFF", "ETSW", "PPTSW", "ROUGHCH", "WIDTH1", "WIDTH2" };
            Segments.DataCubeValueChanged += Segments_DataCubeValueChanged;
            for (int i = 0; i < RiverNetwork.ReachCount; i++)
            {
                var reach = RiverNetwork.Reaches[i];
                Reaches[0,0,i] = reach.KRCH;
                Reaches[1, 0, i] = reach.IRCH;
                Reaches[2, 0, i] = reach.JRCH;
                Reaches[3, 0, i] = reach.ISEG;
                Reaches[4, 0, i] = reach.IREACH;
                Reaches[5, 0, i] = (float)reach.Length;
                Reaches[6, 0, i] = (float)reach.TopElevation;
                Reaches[7, 0, i] = (float)reach.Slope;
                Reaches[8, 0, i] = (float)reach.BedThick;
                Reaches[9, 0, i] = (float)reach.STRHC1;
                Reaches[10, 0, i] = (float)reach.THTS;
                Reaches[11, 0, i] = (float)reach.THTI;
                Reaches[12, 0, i] = (float)reach.EPS;
            }
            for (int i = 0; i < RiverNetwork.RiverCount; i++)
            {
                var seg = RiverNetwork.Rivers[i];
                Segments[0, 0, i] = seg.ID;
                Segments[1, 0, i] = seg.ICALC;
                Segments[2, 0, i] = seg.OutRiverID;
                Segments[3, 0, i] = seg.UpRiverID;
                Segments[4, 0, i] = (float)seg.Flow;
                Segments[5, 0, i] = (float)seg.Runoff;
                Segments[6, 0, i] = (float)seg.ETSW;
                Segments[7, 0, i] = (float)seg.PPTSW;
                Segments[8, 0, i] = (float)seg.ROUGHCH;
                Segments[9, 0, i] = (float)seg.Width1;
                Segments[10, 0, i] = (float)seg.Width2;
            }
        }

        public void BuildTopology()
        {
            var grid = Grid as RegularGrid;

            _ReachTopo.ActiveCellLocation = new int[NSTRM][];
            _ReachTopo.ActiveCellID = new int[NSTRM];
            _ReachTopo.RowCount = grid.RowCount;
            _ReachTopo.ColumnCount = grid.ColumnCount;
            _ReachTopo.ActiveCellCount = NSTRM;
            _SegTopo.ActiveCellLocation = new int[NSS][];
            _SegTopo.ActiveCellID = new int[NSS];
            _SegTopo.RowCount = grid.RowCount;
            _SegTopo.ColumnCount = grid.ColumnCount;
            _SegTopo.ActiveCellCount = NSS;

            int i = 0;
            foreach(var newreach in RiverNetwork.Reaches)
            {
                _ReachTopo.ActiveCellLocation[i] = new int[] { newreach.IRCH - 1, newreach.JRCH - 1 };
                _ReachTopo.ActiveCellID[i] = grid.Topology.GetID(newreach.IRCH - 1, newreach.JRCH - 1);
                i++;
            }

            i = 0;
            foreach(var seg in RiverNetwork.Rivers)
            {
                if (seg.Reaches.Count > 0)
                {
                    var newreach = seg.FirstReach;
                    _SegTopo.ActiveCellLocation[i] = new int[] { newreach.IRCH - 1, newreach.JRCH - 1 };
                    _SegTopo.ActiveCellID[i] = grid.Topology.GetID(newreach.IRCH - 1, newreach.JRCH - 1);
                    i++;
                }
            }
        }

        private Reach ParseReach(string[] strs, int id)
        {
            //KRCH IRCH JRCH ISEG IREACH RCHLEN STRTOP SLOPE STRTHICK STRHC1 THTS THTI EPS UHC IFACE
            int ireach = int.Parse(strs[4]);
            Reach reach = new Reach(id)
            {
                KRCH = int.Parse(strs[0]),
                IRCH = int.Parse(strs[1]),
                JRCH = int.Parse(strs[2]),
                ISEG = int.Parse(strs[3]),
                IREACH = int.Parse(strs[4]),
                Length = double.Parse(strs[5]),
                TopElevation = double.Parse(strs[6]),
                Slope = double.Parse(strs[7]),
                BedThick = double.Parse(strs[8]),
                STRHC1 = double.Parse(strs[9]),
                THTS = double.Parse(strs[10]),
                THTI = double.Parse(strs[11]),
                EPS = double.Parse(strs[12]),
                SubID = ireach
                //   IFACE = int.Parse(strs[14])
            };
            return reach;
        }

        private void ConnectRivers(StreamReader stream, RiverNetwork net)
        {
            int np = TimeService.StressPeriods.Count;
            SPInfo = new int[np, 3];
            for (int n = 0; n < 1; n++)
            {  //389 0 0 # Data Set 5, Stress period 1: ITMP IRDFLG IPTFLG
                string line = stream.ReadLine();
                var vv = TypeConverterEx.Split<int>(line, 3);
                SPInfo[n, 0] = vv[0];
                SPInfo[n, 1] = vv[1];
                SPInfo[n, 2] = vv[2];

                if (vv[0] >= 0)
                { //1 1 6 0 0  0 0  0  0.03  # Data Set 6a: NSEG ICALC OUTSEG IUPSEG FLOW RUNOFF ETSW PPTSW ROUGHCH
                    // 30  # Data set 6b: WIDTH1
                    // 30  # Data set 6c: WIDTH2               
                    string[] strs = null;
                    for (int r = 0; r < net.RiverCount; r++)
                    {
                        line = stream.ReadLine().Trim();
                        strs = TypeConverterEx.Split<string>(line);
                        int nseg = int.Parse(strs[0]);
                        River river = net.GetRiver(nseg);
                        river.ICALC = int.Parse(strs[1]);
                        river.OutRiverID = int.Parse(strs[2]);
                        river.UpRiverID = int.Parse(strs[3]);
                        if (river.UpRiverID != 0)
                        {
                            river.IPrior = int.Parse(strs[4]);
                            river.Flow = double.Parse(strs[5]);
                            river.Runoff = double.Parse(strs[6]);
                            river.ETSW = double.Parse(strs[7]);
                            river.PPTSW = double.Parse(strs[8]);
                            river.ROUGHCH = double.Parse(strs[9]);
                        }
                        else
                        {
                            river.Flow = double.Parse(strs[4]);
                            river.Runoff = double.Parse(strs[5]);
                            river.ETSW = double.Parse(strs[6]);
                            river.PPTSW = double.Parse(strs[7]);
                            river.ROUGHCH = double.Parse(strs[8]);
                        }
                        if (n == 0)
                        {
                            line = stream.ReadLine().Trim();
                            strs = TypeConverterEx.Split<string>(line);
                            river.Width1 = double.Parse(strs[0]);
                            line = stream.ReadLine().Trim();
                            strs = TypeConverterEx.Split<string>(line);
                            river.Width2 = double.Parse(strs[0]);
                        }
                        var downriver = net.GetRiver(river.OutRiverID);
                        if (downriver != null)
                            downriver.Upstreams.Add(river);
                        river.Downstream = downriver;
                    }
                    int ntab = NUMTAB;
                    if (ntab > 0)
                    {
                        TablesInfo = new int[ntab, 3];
                        for (int t = 0; t < ntab; t++)
                        {
                            line = stream.ReadLine();
                            vv = TypeConverterEx.Split<int>(line, 3);
                            TablesInfo[t, 0] = vv[0];
                            TablesInfo[t, 1] = vv[1];
                            TablesInfo[t, 2] = vv[2];
                        }
                    }

                    int nodeIndex = 1;
                    double dw = 1;
                    foreach (var river in net.Rivers)
                    {
                        dw = (river.Width2 - river.Width1) / river.Reaches.Count;
                        for (int i = 0; i < river.Reaches.Count; i++)
                        {
                            var reach = river.Reaches[i];
                            reach.ROUGHCH = river.ROUGHCH;
                            reach.PPTSW = river.PPTSW;
                            reach.Width = Math.Round(river.Width1 + (i + 1) * dw, 2);
                            reach.Width1 = reach.Width;
                            reach.Width2 = reach.Width;
                            if (i == 0)
                            {
                                CreateReachJunctions(reach, nodeIndex, null);
                                nodeIndex += 2;
                            }
                            else
                            {
                                CreateReachJunctions(reach, nodeIndex, river.Reaches[i - 1]);
                                nodeIndex++;
                            }
                            river.InletNode = river.FirstReach.InletNode;
                            river.OutletNode = river.LastReach.OutletNode;
                        }
                    }

                    foreach (var river in net.Rivers)
                    {
                        if (river.Downstream != null)
                        {
                            river.OutletNode = river.Downstream.InletNode;
                            river.LastReach.OutletNode = river.OutletNode;
                        }
                        else
                        {
                            net.Outfalls.Add(river.OutletNode);
                        }

                        foreach (var upr in river.Upstreams)
                        {
                            upr.OutletNode = river.InletNode;
                            upr.LastReach.OutletNode = upr.OutletNode;
                        }

                        if (!net.RiverJunctions.Contains(river.InletNode))
                            net.RiverJunctions.Add(river.InletNode);
                        if (!net.RiverJunctions.Contains(river.OutletNode))
                            net.RiverJunctions.Add(river.OutletNode);
                    }
                }
            }
        }

        public void ConnectRivers(RiverNetwork net)
        {
            for (int r = 0; r < net.RiverCount; r++)
            {
                River river = net.GetRiver(r + 1);
                var downriver = net.GetRiver(river.OutRiverID);
                if (downriver != null)
                    downriver.Upstreams.Add(river);
                river.Downstream = downriver;
            }
            int nodeIndex = 1;
            double dw = 1;
            foreach (var river in net.Rivers)
            {
                dw = (river.Width2 - river.Width1) / river.Reaches.Count;
                for (int i = 0; i < river.Reaches.Count; i++)
                {
                    var reach = river.Reaches[i];
                    reach.ROUGHCH = river.ROUGHCH;
                    reach.PPTSW = river.PPTSW;
                    reach.Width = Math.Round(river.Width1 + (i + 1) * dw, 2);
                    reach.Width1 = reach.Width;
                    reach.Width2 = reach.Width;
                    if (i == 0)
                    {
                        CreateReachJunctions(reach, nodeIndex, null);
                        nodeIndex += 2;
                    }
                    else
                    {
                        CreateReachJunctions(reach, nodeIndex, river.Reaches[i - 1]);
                        nodeIndex++;
                    }
                    river.InletNode = river.FirstReach.InletNode;
                    river.OutletNode = river.LastReach.OutletNode;
                }
            }

            foreach (var river in net.Rivers)
            {
                if (river.Downstream != null)
                {
                    river.OutletNode = river.Downstream.InletNode;
                    river.LastReach.OutletNode = river.OutletNode;
                }
                else
                {
                    net.Outfalls.Add(river.OutletNode);
                }

                foreach (var upr in river.Upstreams)
                {
                    upr.OutletNode = river.InletNode;
                    upr.LastReach.OutletNode = upr.OutletNode;
                }

                if (!net.RiverJunctions.Contains(river.InletNode))
                    net.RiverJunctions.Add(river.InletNode);
                if (!net.RiverJunctions.Contains(river.OutletNode))
                    net.RiverJunctions.Add(river.OutletNode);
            }
        
        }

        private void CreateReachJunctions(Reach reach, int id, Reach upReach)
        {
            var ModflowGrid = Owner.Grid as MFGrid;
            if (ModflowGrid != null)
            {
                int outid = 1;
                var c = ModflowGrid.LocateNode(reach.JRCH, reach.IRCH);
                c.Z = reach.TopElevation + 0.5;
                if (upReach != null)
                {
                    reach.InletNode = upReach.OutletNode;
                    outid = id;
                    if (c.Z > upReach.OutletNode.Coordinate.Z)
                        c.Z = upReach.OutletNode.Coordinate.Z;
                }
                else
                {
                    reach.InletNode = new HydroPoint(id)
                    {
                        Coordinate = c,
                        ReachObject = reach,
                        RiverObject = reach.Parent,
                        Row = reach.IRCH,
                        Column = reach.JRCH,
                        Elevation = reach.TopElevation
                    };
                    outid = id + 1;
                }
                float rowInt = 0;
                float colInt = 0;

                rowInt = ModflowGrid.DELR[0, 0, reach.JRCH - 1];
                colInt = ModflowGrid.DELC[0, 0, reach.IRCH - 1];
                
                reach.OutletNode = new HydroPoint(outid)
                {
                    Coordinate = new Coordinate(c.X + rowInt, c.Y + colInt,
                        reach.TopElevation),
                    ReachObject = reach,
                    RiverObject = reach.Parent,
                    Elevation = reach.TopElevation
                };
            }
        }

        private void Segments_DataCubeValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < RiverNetwork.RiverCount; i++)
            {
                var seg = RiverNetwork.Rivers[i];
                seg.ID = (int)Segments[0, 0, i];
                seg.ICALC = (int)Segments[1, 0, i];
                seg.OutRiverID = (int)Segments[2, 0, i];
                seg.UpRiverID = (int)Segments[3, 0, i];
                seg.Flow = Segments[4, 0, i];
                seg.Runoff = Segments[5, 0, i];
                seg.ETSW = Segments[6, 0, i];
                seg.PPTSW = Segments[7, 0, i];
                seg.ROUGHCH = Segments[8, 0, i];
                seg.Width1 = Segments[9, 0, i];
                seg.Width2 = Segments[10, 0, i];
            }
        }

        private void Reaches_DataCubeValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < RiverNetwork.ReachCount; i++)
            {
                var reach = RiverNetwork.Reaches[i];
                reach.KRCH = (int)Reaches[0, 0, i];
                reach.IRCH = (int)Reaches[1, 0, i];
                reach.JRCH = (int)Reaches[2, 0, i];
                reach.ISEG = (int)Reaches[3, 0, i];
                reach.IREACH = (int)Reaches[4, 0, i];
                reach.Length = Reaches[5, 0, i];
                reach.TopElevation = Reaches[6, 0, i];
                reach.Slope = Reaches[7, 0, i];
                reach.BedThick = Reaches[8, 0, i];
                reach.STRHC1 = Reaches[9, 0, i];
                reach.THTS = Reaches[10, 0, i];
                reach.THTI = Reaches[11, 0, i];
                reach.EPS = Reaches[12, 0, i];
            }
        }
        #endregion

        #region Smooth
        /// <summary>
        /// Smooth reaches' elevation as well as grid layer's elevation connected to the reaches
        /// </summary>
        /// <param name="juncEleFile">A csv file that stores river junctions' elevation (it must include two variables named "ID" and "Elevation")</param>
        /// <param name="grid">A MF grid object</param>
        ///  <param name="eleModifier">Add this value to top elevation</param>
        public void Smooth(HydroPoint[] junctions, MFGrid grid, float eleModifier)
        {
            if (skippedRiver == null)
                skippedRiver = new List<River>();

            int i = 0;
            foreach (var junc in junctions)
            {
                var node = (from n in RiverNetwork.RiverJunctions where n.ID == junc.ID select n).FirstOrDefault();
                node.Elevation = junc.Elevation + eleModifier;
                i++;
            }

            Message += "\n\n*********************Top elevations of  following cells were modified\n";
            Message += " Seg_ID\tReach_SubID\tK\tRow\tCol\tTop\tBottom\tNew_Bottom\n";

            for (i = 0; i < RiverNetwork.RiverCount; i++)
            {
                var river = RiverNetwork.Rivers[i];
                if (skippedRiver.Contains(river))
                {
                    double endEle = river.LastReach.OutletNode.Elevation;
                    double sEle = river.FirstReach.InletNode.Elevation;
                    double slope = (sEle - endEle) / river.Length;
                    for (int r = 0; r < river.Reaches.Count; r++)
                    {
                        var reach = river.Reaches[r];
                        reach.Slope = slope;
                        var row = reach.IRCH;
                        var col = reach.JRCH;
                        var index = grid.Topology.GetSerialIndex(row, col);
                        var top = grid.Elevations[row - 1, col - 1, 0];
                        var btm = grid.Elevations[row - 1, col - 1, 1];
                        reach.TopElevation = top + eleModifier;
                        if (reach.TopElevation < btm)
                            reach.TopElevation = btm + Math.Abs(eleModifier);
                    }
                }
                else
                {
                    Modify(river, eleModifier, grid);
                }
            }
        }

        public void CorrectElevation(float offset)
        {
            var grid = (Owner as Modflow).Grid as MFGrid;
            for (int i = 0; i < RiverNetwork.RiverCount; i++)
            {
                var river = RiverNetwork.Rivers[i];
                for (int j = 0; j < river.Reaches.Count; j++)
                {
                    var rch = river.Reaches[j];
                    rch.TopElevation = grid.GetElevationAt(rch.IRCH - 1, rch.JRCH - 1, 0) + offset;
                }
            }
        }

        private void Modify(River seg, float eleDif, MFGrid grid)
        {
            int rchNum = seg.Reaches.Count;
            int row = seg.LastReach.IRCH;
            int col = seg.LastReach.JRCH;
            int k = seg.LastReach.KRCH;
            double endEle = seg.LastReach.OutletNode.Elevation;
            double sEle = seg.FirstReach.InletNode.Elevation;
            double slope = (sEle - endEle) / seg.Length;
            double nextEle = endEle;

            seg.Slope = slope;

            for (int i = rchNum - 1; i > -1; i--)
            {
                var reach = seg.Reaches[i];
                endEle = reach.OutletNode.Elevation;
                sEle = endEle + reach.Length * slope;
                if (i != 0)
                {
                    reach.InletNode.Elevation = sEle;
                }
                reach.TopElevation = (sEle + endEle) * 0.5;
                reach.Slope = slope;
                row = reach.IRCH;
                col = reach.JRCH;
                k = reach.KRCH;

                var heights = GetLayerHeight(grid, row, col, eleDif);
                var index = grid.Topology.GetSerialIndex(row, col);
                var top = grid.Elevations[index, 0, 0];
                var btm = grid.Elevations[index, 1, 0];

                if (top <= btm)
                {
                    Message += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n", seg.ID, reach.SubID, k, row, col,
                        top, btm);
                    Modify(grid, row, col, heights);
                }

                if ((reach.TopElevation - reach.BedThick) < btm)
                {
                    var delh = (float)(reach.BedThick + 5);
                    var newbtm = (float)(reach.TopElevation - delh);
                    grid.Elevations[index, 1, 0] = newbtm;

                    for (int l = 2; l < grid.LayerCount; l++)
                    {
                        grid.Elevations[index, l, 0] -= delh;
                    }

                    Message += string.Format("Layer 1 bottom modified:{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", seg.ID, reach.SubID, k, row, col,
                 reach.TopElevation, btm, newbtm);

                    if (reach.TopElevation < newbtm)
                    {
                        Message += string.Format("Fatal error:{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n", seg.ID, reach.SubID, k, row, col, top, btm);
                    }
                }
            }
        }

        private void Modify(MFGrid grid, int row, int col, float[] height)
        {
            var index = grid.Topology.GetSerialIndex(row, col);
            for (int l = 0; l < grid.ActualLayerCount; l++)
            {
                grid.Elevations[index, l + 1, 0] = grid.Elevations[index, l, 0] - height[l];
            }
        }
        private float[] GetLayerHeight(MFGrid grid, int row, int col, float eleDif)
        {
            float[] heights = new float[grid.ActualLayerCount];
            var index = grid.Topology.GetSerialIndex(row, col);
            for (int l = 0; l < grid.ActualLayerCount; l++)
            {
                heights[l] = grid.Elevations[index, l, 0] - grid.Elevations[index, l + 1, 0];
                if (heights[l] < 0)
                    heights[l] = 10;
                if (heights[l] < eleDif)
                {
                    heights[l] = eleDif * 2;
                }
            }
            return heights;
        }

        #endregion

        #region Set parameter
        /// <summary>
        /// remove ignored river and renumber river id
        /// </summary>
        public void ReNumber()
        {
            var skippedID = (from r in skippedRiver select r.ID).Distinct().ToArray();
            int i = 0;
            int count = RiverNetwork.Rivers.Count;
            List<River> removed = new List<River>();
            for (i = 0; i < count; i++)
            {
                var river = RiverNetwork.Rivers[i];
                if (skippedID.Contains(river.ID))
                {
                    removed.Add(river);
                }
            }
            foreach (var rch in removed)
                RiverNetwork.Rivers.Remove(rch);

            for (i = 0; i < RiverNetwork.Rivers.Count; i++)
            {
                var river = RiverNetwork.Rivers[i];
                int oldID = river.ID;
                river.ID = i + 1;
                river.OldID = oldID;
                foreach (var rch in river.Reaches)
                {
                    rch.ISEG = river.ID;
                }
                var outr = from r in RiverNetwork.Rivers where r.OutRiverID == oldID select r;
                foreach (var or in outr)
                {
                    or.OutRiverID = river.ID;
                }
            }

        }

        public void SetVK(double scale, double minimum = 0.05)
        {
            var lpf = Owner.Packages["LPF"] as LPFPackage;

            foreach (River river in RiverNetwork.Rivers)
            {
                for (int i = 0; i < river.Reaches.Count; i++)
                {
                    var rch = river.Reaches[i];
                    //rch.STRHC1
                    var row = rch.IRCH;
                    var col = rch.JRCH;
                    var index = (Owner.Grid as MFGrid).Topology.GetSerialIndex(row, col);
                    var vk = lpf.HK[index, 0, 0] / lpf.VKA[index, 0, 0];
                    river.Reaches[i].STRHC1 = vk * scale;
                    river.Reaches[i].STRHC1 = river.Reaches[i].STRHC1 < minimum ? minimum : river.Reaches[i].STRHC1;
                }
            }
        }

        public void SetVK(int riverID, double vk)
        {
            var riv = RiverNetwork.GetRiver(riverID);
            foreach (var rch in riv.Reaches)
            {
                rch.STRHC1 = vk;
            }
        }

        public void SetVK(List<River> rivers, double scale = 1.0)
        {
            var lpf = Owner.Packages["LPF"] as LPFPackage;
            foreach (River river in rivers)
            {
                for (int i = 0; i < river.Reaches.Count; i++)
                {
                    var rch = river.Reaches[i];
                    //rch.STRHC1
                    var row = rch.IRCH;
                    var col = rch.JRCH;
                    var index = (Owner.Grid as MFGrid).Topology.GetSerialIndex(row, col);
                    var vk = lpf.HK[index, 0, 0] / lpf.VKA[index, 0, 0];
                    river.Reaches[i].STRHC1 = vk * scale;
                }
            }
        }

        public void SetBedThick(double thick = 2)
        {
            foreach (River river in RiverNetwork.Rivers)
            {
                for (int i = 0; i < river.Reaches.Count; i++)
                {
                    var rch = river.Reaches[i];
                    rch.BedThick = thick;
                }
            }
        }

        public void SaveSegmentAsShp(string shp)
        {
            DotSpatial.Data.FeatureSet fs = new DotSpatial.Data.FeatureSet(FeatureType.Line);
            fs.DataTable.Columns.Add(new DataColumn("ID", Type.GetType("System.Int32")));
            fs.DataTable.Columns.Add(new DataColumn("OldID", Type.GetType("System.Int32")));
            foreach (River river in RiverNetwork.Rivers)
            {
                Coordinate[] cord = new Coordinate[river.Reaches.Count + 1];
                for (int i = 0; i < river.Reaches.Count; i++)
                {
                    cord[i] = river.Reaches[i].InletNode.Coordinate;
                }
                cord[river.Reaches.Count] = river.LastReach.OutletNode.Coordinate;
                LineString lr = new LineString(cord);
                DotSpatial.Data.Feature ft = new DotSpatial.Data.Feature(lr);
                ft.DataRow = fs.DataTable.NewRow();
                ft.DataRow["ID"] = river.ID;
                ft.DataRow["OldID"] = river.OldID;
                fs.Features.Add(ft);
            }
            fs.SaveAs(shp, true);
        }

        public void SaveReachAsShp(string shp)
        {
            var mfgrid = (Owner.Grid as MFGrid);
            DotSpatial.Data.FeatureSet fs = new DotSpatial.Data.FeatureSet(FeatureType.Line);
            fs.DataTable.Columns.Add(new DataColumn("ID", Type.GetType("System.Int32")));
            fs.DataTable.Columns.Add(new DataColumn("Column", Type.GetType("System.Int32")));
            fs.DataTable.Columns.Add(new DataColumn("ROW", Type.GetType("System.Int32")));
            fs.DataTable.Columns.Add(new DataColumn("ISEG", Type.GetType("System.Int32")));
            fs.DataTable.Columns.Add(new DataColumn("IReach", Type.GetType("System.Int32")));
            foreach (River river in RiverNetwork.Rivers)
            {
                for (int i = 0; i < river.Reaches.Count; i++)
                {
                    Coordinate[] cord = new Coordinate[2];
                    var rch = river.Reaches[i];
                    cord[0] = rch.InletNode.Coordinate;
                    cord[1] = rch.OutletNode.Coordinate;
                    LineString lr = new LineString(cord);
                    DotSpatial.Data.Feature ft = new DotSpatial.Data.Feature(lr);
                    ft.DataRow = fs.DataTable.NewRow();
                    ft.DataRow["ID"] = mfgrid.DeterminID(rch.IRCH, rch.JRCH);
                    ft.DataRow["Column"] = rch.JRCH;
                    ft.DataRow["Row"] = rch.IRCH;
                    ft.DataRow["ISEG"] = river.ID;
                    ft.DataRow["IReach"] = rch.IREACH;
                    fs.Features.Add(ft);
                }
                fs.SaveAs(shp, true);
            }

        }
        #endregion

        public override void OnTimeServiceUpdated(ITimeService time)
        {
            var nsp = time.StressPeriods.Count;
        }

        #region Check
        public void CheckReachBedElevation()
        {
            var mfgrid = Owner.Grid as MFGrid;
            Message += "\n\n*********************Checking reach bed elevation\n";
            Message += "River\tReach\tReach_Elevation\tLayerBottom_Elevation\n";
            foreach (River river in RiverNetwork.Rivers)
            {
                for (int i = 0; i < river.Reaches.Count - 1; i++)
                {
                    double se = river.Reaches[i].TopElevation - river.Reaches[i].BedThick;
                    var index = mfgrid.Topology.GetSerialIndex(river.Reaches[i].IRCH, river.Reaches[i].JRCH);
                    double bedele = mfgrid.Elevations[index, 1, 0];
                    if (se <= bedele)
                    {
                        Message += string.Format("{0}\t{1}\t {2}\t{3}\t{4}\n", river.ID, river.Reaches[i].ID, river.Reaches[i].SubID, se, bedele);
                    }
                }
            }
        }

        public void CheckRiver(bool fullRch)
        {

            Message += "\n\n*********************Checking river elevation\n";
            if (fullRch)
            {
                Message += "River\tIn_Elevation\tOut_Elevation\tInlet_ID\tOutlet_I\n";
                for (int i = 0; i < RiverNetwork.RiverCount; i++)
                {
                    var river = RiverNetwork.Rivers[i];
                    double[] reachEle = GetRiverElevations(river);
                    if (reachEle.Length > 1 && reachEle[0] <= reachEle[reachEle.Length - 1])
                    {
                        Message += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n",
                             river.ID, reachEle[0], reachEle[reachEle.Length - 1], river.FirstReach.InletNode.ID, river.LastReach.OutletNode.ID);
                    }
                }
            }
            else
            {
                Message += "River\tIn_Elevation\tOut_Elevation\tInlet_ID\tOutlet_I\n";
                for (int i = 0; i < RiverNetwork.RiverCount; i++)
                {
                    var river = RiverNetwork.Rivers[i];
                    double ele0 = river.FirstReach.InletNode.Elevation;
                    double ele1 = river.LastReach.OutletNode.Elevation;
                    if (ele0 < ele1)
                    {
                        Message += string.Format("$River Elevation:\t{0} \t{1}\t{2}\t{3}\t{4}\n",
                            river.ID, ele0, ele1, river.FirstReach.InletNode.ID, river.LastReach.OutletNode.ID);
                        Message += "--------------------------------------------------------------------\n";
                    }
                }
            }
        }

        public void CheckReach()
        {
            Message += "\n\n*********************Checking reach elevation\n";
            Message += "River\tReach\tUp_Elevation\tDown_Elevation\n";
            foreach (River river in RiverNetwork.Rivers)
            {
                for (int i = 0; i < river.Reaches.Count - 1; i++)
                {
                    double se = Math.Round(river.Reaches[i].TopElevation, 2);
                    double ee = Math.Round(river.Reaches[i + 1].TopElevation, 2);
                    if (se <= ee)
                    {
                        Message += string.Format("{0}\t{1}\t {2}\t{3}\t{4}\n",
                            river.ID, river.Reaches[i].ID, river.Reaches[i].SubID, se, ee);
                        Message += string.Format("{0}\t{1}\t {2}\t{3}\t{4}\n",
                          river.ID, river.Reaches[i].ID, river.Reaches[i].SubID, se, ee);
                    }
                }
            }
            Message += "\n\n*********************Checking reach slope\n";
            Message += "River\tReach\tSubID\tSlope\n";
            foreach (River river in RiverNetwork.Rivers)
            {
                for (int i = 0; i < river.Reaches.Count - 1; i++)
                {
                    if (river.Reaches[i].Slope <= 0)
                    {
                        Message += string.Format("{0}\t {1}\t {2}\t{3} \n",
                          river.ID, river.Reaches[i].ID, river.Reaches[i].SubID, river.Reaches[i].Slope);
                    }
                }
            }
        }

        public void CheckNode()
        {

            Message += "\n\n*********************Checking node elevation\n";
            Message += "River\tReach\tSubID\tInlet_Elevation\tOutlet_Elevation\tInlet_ID\tOutlet_ID\n";
            foreach (River river in RiverNetwork.Rivers)
            {
                for (int i = 0; i < river.Reaches.Count; i++)
                {
                    double se = river.Reaches[i].InletNode.Elevation;
                    double ee = river.Reaches[i].OutletNode.Elevation;
                    if (se < ee)
                    {
                        Message += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n",
                            river.ID, river.Reaches[i].ID, river.Reaches[i].SubID, se, ee, river.Reaches[i].InletNode.ID, river.Reaches[i].OutletNode.ID);
                    }
                }
            }
        }

        private double[] GetRiverElevations(River river)
        {
            double[] reachEle = new double[river.Reaches.Count];
            for (int r = 0; r < reachEle.Length; r++)
            {
                reachEle[r] = river.Reaches[r].TopElevation;
            }
            return reachEle;
        }

        #endregion
    }
}
