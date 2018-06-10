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

using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using DotSpatial.Data;
using Heiflow.Models.UI;
using Heiflow.Core;
using DotSpatial.Projections;
using NetTopologySuite.Geometries;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [PackageCategory("Observation Process", false)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class HOBPackage : MFPackage
    {
        public static string PackageName = "HOB";
        private IFeatureSet _Feature;

        public HOBPackage()
        {
            Name = PackageName;
            _FullName = "Head Observation Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".hob";
            _PackageInfo.ModuleName = "HOB";
            IsMandatory = false;

            HOBDRY = -999.0f;
            TOMULTH = 1;
            NH = 0;
            MOBS = 0;
            MAXM = 2;
            IUHOBSV = 42;
            HOBDRY = -999.0f;
            TOMULTH = 1.0f;
            Option = "NOPRINT";
            Description = "The Head-Observation input file is used to specify observations of head for use in the Observation process.";
            Version = "HOB";
            Observations = new List<IObservationsSite>();

            var field = new PackageFeatureField()
            {
                FieldName = "SiteID",
                FieldType = typeof(Int32)
            };
            Fields.Add(field);
            field = new PackageFeatureField()
            {
                FieldName = "Cell_ID",
                FieldType = typeof(Int32)
            };
            Fields.Add(field);
            field = new PackageFeatureField()
            {
                FieldName = "ROW",
                FieldType = typeof(Int32)
            };
            Fields.Add(field);
            field = new PackageFeatureField()
            {
                FieldName = "COLUMN",
                FieldType = typeof(Int32)
            };
            Fields.Add(field);
            field = new PackageFeatureField()
            {
                FieldName = "SiteName",
                FieldType = typeof(string)
            };
            Fields.Add(field);
            field = new PackageFeatureField()
            {
                FieldName = "Elevation",
                FieldType = typeof(double)
            };
            Fields.Add(field);

            _Layer3DToken = "HOB";
        }
        public int NH { get; set; }
        public int MOBS { get; set; }
        public int MAXM { get; set; }
        /// <summary>
        /// File unit for saving observation data in a file. 
        /// Specify 0 for no observation output file. 
        /// The file for this unit must be included as type “DATA” in the Name File
        /// </summary>
        public int IUHOBSV { get; set; }
        public float HOBDRY { get; set; }
        public string Option { get; set; }
        public float TOMULTH { get; set; }
        public List<IObservationsSite> Observations { get; private set; }
        [StaticVariableItem("Stress Period")]
        [Browsable(false)]
        public DataCube<float> HOBS { get; set; }
        [Browsable(false)]
        [PackageOptionalViewItem("HOB")]
        public override IPackageOptionalView OptionalView
        {
            get;
            set;
        }
        public RegularGridTopology Topology
        {
            get;
            protected  set;
        }

        public override IFeatureSet Feature
        {
            get
            {
                return _Feature;
            }
            set
            {
                _Feature = value;
                foreach (var pck in Children)
                    pck.Feature = _Feature;
            }
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override bool New()
        {
            var mf = Owner as Modflow;
            this.IUHOBSV = mf.NameManager.NextFID();
            var pckinfo = new PackageInfo();
            pckinfo.ModuleName = "DATA";
            pckinfo.FID = this.IUHOBSV;
            pckinfo.Format = FileFormat.Text;
            pckinfo.IOState = IOState.REPLACE;
            pckinfo.FileName = Modflow.OutputDic + mf.Project.Name + ".hob_out";
            pckinfo.FileExtension = ".hob_out";
            pckinfo.Name = Path.GetFileName(pckinfo.FileName);
            pckinfo.WorkDirectory = mf.WorkDirectory;
            mf.NameManager.AddInSilence(pckinfo);
            return base.New();
        }
        public override string CreateFeature(ProjectionInfo proj_info, string directory)
        {
            if (Observations != null)
            {
                string filename = Path.Combine(directory, this.Name + ".shp");
                var grid = (Owner as Modflow).Grid as MFGrid;
                FeatureSet fs = new FeatureSet(FeatureType.Point);
                fs.Name = this.Name;
                fs.Projection = proj_info;
                fs.DataTable.Columns.Add(new DataColumn("SiteID", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("SiteName", typeof(string)));
                fs.DataTable.Columns.Add(new DataColumn("CELL_ID", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Row", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("Column", typeof(int)));
                fs.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));

                fs.DataTable.Columns.Add(new DataColumn("Elevation", typeof(float)));
                fs.DataTable.Columns.Add(new DataColumn("HSIM", typeof(float)));
                fs.DataTable.Columns.Add(new DataColumn("HOBS", typeof(float)));
                fs.DataTable.Columns.Add(new DataColumn("DepOBS", typeof(float)));
                fs.DataTable.Columns.Add(new DataColumn("DepSIM", typeof(float)));
                fs.DataTable.Columns.Add(new DataColumn("DepDIF", typeof(float)));
                fs.DataTable.Columns.Add(new DataColumn("HDIF", typeof(float)));

                fs.DataTable.Columns.Add(new DataColumn("Name", typeof(string)));
                fs.DataTable.Columns.Add(new DataColumn(RegularGrid.ParaValueField, typeof(int)));

                fs.DataTable.Columns.Add(new DataColumn("IsTR", typeof(int)));
                int i = 1;
                foreach (var vv in Observations)
                {
                    var hob = vv as HeadObservation;
                    var coor = grid.LocateCentroid(hob.Column, hob.Row);
                    Point geom = new Point(coor);
                    IFeature feature = fs.AddFeature(geom);
                    feature.DataRow.BeginEdit();
                    feature.DataRow["CELL_ID"] = grid.Topology.GetID(hob.Row, hob.Column);
                    feature.DataRow["Row"] = hob.Row;
                    feature.DataRow["Column"] = hob.Column;
                    feature.DataRow["ID"] = hob.ID;

                    feature.DataRow["Elevation"] = hob.Elevation;
                    feature.DataRow["HSIM"] = 0;
                    feature.DataRow["HOBS"] = 0;

                    feature.DataRow["Name"] = hob.Name;
                    feature.DataRow[RegularGrid.ParaValueField] = 0;

                    feature.DataRow["IsTR"] = 1;
                    feature.DataRow["SiteID"] = i;
                    feature.DataRow["SiteName"] = hob.Name;
                    i++;
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
        public override bool Load(ICancelProgressHandler progresshandler)
        {
            if (File.Exists(FileName))
            {
                var grid = (Owner.Grid as MFGrid);
                Observations.Clear();
                StreamReader sr = new StreamReader(FileName);
                var line = ReadComment(sr);
                var strs = TypeConverterEx.Split<string>(line, 6);
                NH = int.Parse(strs[0]);
                MOBS = int.Parse(strs[1]);
                MAXM = int.Parse(strs[2]);
                IUHOBSV = int.Parse(strs[3]);
                HOBDRY = float.Parse(strs[4]);
                Option = strs[5];
                TOMULTH = TypeConverterEx.Split<float>(sr.ReadLine(), 1)[0];
                Topology = new RegularGridTopology();
                int i = 0;
                while (!sr.EndOfStream)
                {
                    line =sr.ReadLine();
                    var vv = TypeConverterEx.SkipSplit<float>(line, 1, 8);
                    strs = TypeConverterEx.Split<string>(line);
                    HeadObservation obs = new HeadObservation(i)
                    {
                        Layer = (int)vv[0],
                        Row = (int)vv[1],
                        Column = (int)vv[2],
                        IREFSPFlag = (int)vv[3],
                        Name = strs[0]
                    };
                    obs.CellID = grid.Topology.GetID(obs.Row - 1, obs.Column - 1);
                    if (obs.IREFSPFlag < 0)
                    {
                        line = sr.ReadLine();
                        obs.ITT = TypeConverterEx.Split<int>(line, 1)[0];
                        int nsp = Math.Abs((int)vv[3]);
                        obs.IREFSP = new int[nsp];
                        obs.TOFFSET = new float[nsp];
                        obs.HOBS = new float[nsp];
                        obs.ROFF = vv[5];
                        obs.COFF = vv[6];
                        for (int t = 0; t < nsp; t++)
                        {
                            line = sr.ReadLine();
                            vv = TypeConverterEx.SkipSplit<float>(line, 1, 4);
                            obs.IREFSP[t] = (int)vv[0];
                            obs.TOFFSET[t] = vv[1];
                            obs.HOBS[t] = vv[2];
                        }
                    }
                    else
                    {
                        int nsp = 1;
                        obs.IREFSP = new int[nsp];
                        obs.TOFFSET = new float[nsp];
                        obs.HOBS = new float[nsp];
                        obs.IREFSP[0] = (int)vv[3];
                        obs.TOFFSET[0] = vv[4];
                        obs.ROFF = vv[5];
                        obs.COFF = vv[6];
                        obs.HOBS[0] = vv[7];
                    }
                    obs.Elevation = grid.GetElevationAt(obs.Row - 1, obs.Column - 1, obs.Layer - 1);
                    Observations.Add(obs);
                    i++;
                }
                if (Observations.Any())
                {
                    Topology.ActiveCell = new int[NH][];
                    Topology.ActiveCellIDs = new int[NH];
                    Topology.RowCount = grid.RowCount;
                    Topology.ColumnCount = grid.ColumnCount;
                    Topology.ActiveCellCount = NH;
                    var nsp1 = (Observations[0] as HeadObservation).IREFSP.Length;
                    HOBS = new DataCube<float>(1, nsp1, NH);
                    HOBS.Variables = new string[] { "Head Observation" };
                    for (int t = 0; t < nsp1; t++)
                        for (int k = 0; k < NH; k++)
                        {
                            var hob= Observations[k] as HeadObservation;
                            HOBS[0,t,k] = hob.HOBS[t];
                            Topology.ActiveCell[k] = new int[] { hob.Row - 1, hob.Column - 1 };
                            Topology.ActiveCellIDs[k] = grid.Topology.GetID(hob.Row - 1, hob.Column - 1);
                        }
                    HOBS.Topology = this.Topology;
                }
                sr.Close();
                OnLoaded(progresshandler);
                return true;
            }
            else
            {
                OnLoadFailed("Failed to load "+this.Name, progresshandler);
                return false;
            }
        }
        public override bool SaveAs(string filename, ICancelProgressHandler prg)
        {
            //StreamWriter sw = new StreamWriter(filename);
            //WriteDefaultComment(sw, "HOB");
            //var hobs = Parameters["HOBS"].Value as List<HeadObservation>;

            //string line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t# Data Set 1: NH MOBS MAXM IUHOBSV HOBDRY OPTION", Parameters["NH"].IntValue, Parameters["MOBS"].IntValue, 
            //    Parameters["MAXM"].IntValue, Parameters["IUHOBSV"].IntValue, Parameters["HOBDRY"].FloatValue, Parameters["OPTION"].StringValue);
            //sw.WriteLine(line);
            //line = string.Format("{0}\t # Data Set 2: TOMULTH", 1);
            //sw.WriteLine(line);
            //foreach (var obs in hobs)
            //{
            //    line = "";
            //    line += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t# Data Set 3: OBSNAM LAYER ROW COLUMN IREFSP TOFFSET ROFF COFF HOBS\n",
            //        obs.ID, obs.Layer, obs.Row, obs.Column, obs.IREFSP[0], obs.TOFFSET[0], obs.ROFF, obs.COFF, obs.HOBS[0]);
            //    line += string.Format("{0}\t #Data Set 5: ITT\n", obs.ITT);
            //    for (int t = 0; t < obs.ITT; t++)
            //    {
            //        line += string.Format("{0}\t{1}\t{2}\t{3}\t# Data Set 6: OBSNAM IREFSP TOFFSET HOBS\n", obs.ID, Math.Abs(obs.IREFSP[t]), obs.TOFFSET[t], obs.HOBS[t]);
            //    }
            //    sw.Write(line);
            //}
            //sw.Close();
            return true;
        }
        public override void Clear()
        {
            base.Clear();
        }
        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var mf = Owner as Modflow;
            if (this.IUHOBSV > 0)
            {
                var pck_info = (from info in mf.NameManager.MasterList where info.FID == this.IUHOBSV select info).First();
                pck_info.ModuleName = HOBOutputPackage.PackageName;

                var hob_out = new HOBOutputPackage()
                {
                    Owner = mf,
                    Parent = this,
                    PackageInfo = pck_info,
                    FileName =pck_info.FileName
                };
                hob_out.Initialize();
                mfout.Children.Add(hob_out);
            }
        }

        public HeadObservation[] ExtractFrom(DataTable dt)
        {
            if (SatisfyRquiredField(dt))
            {
                var grid = (Owner as Modflow).Grid as MFGrid;
                var hobs = (from dr in dt.AsEnumerable()
                           select new HeadObservation(int.Parse(dr["SiteID"].ToString()))
                           {
                               Column = int.Parse(dr["COLUMN"].ToString()),
                               Row = int.Parse(dr["ROW"].ToString()),
                               CellID = int.Parse(dr["Cell_ID"].ToString()),
                               Name = dr["SiteName"].ToString(),
                               Elevation = double.Parse(dr["Elevation"].ToString())
                           }).ToArray();
            return hobs;
            }
            else
            {
                return null;
            }
        }

        private bool SatisfyRquiredField(DataTable dt)
        {
            var fields = from ff in Fields select ff.FieldName;
            bool found = true;
            foreach(var ss in fields)
            {
                if(!dt.Columns.Contains(ss))
                {
                    found = false;
                    break;
                }
            }
            return found;
        }
    }
}
