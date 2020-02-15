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
using Heiflow.Models.UI;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [PackageCategory("Groundwater Flow ", false)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class UZFPackage : MFPackage
    {
        public static string PackageName = "UZF";
        public UZFPackage()
        {
            Name = PackageName;
            _FullName = "Unsaturated-Zone Flow Package";
            NUZTOP = 1;
            IUZFOPT = 2;
            IRUNFLG = 1;
            IETFLG = 1;
            IUZFCB1 = 9;
            IUZFCB2 = 0;
            NTRAIL2 = 15;
            NSETS2 = 20;
            NUZGAG = 0;
            SURFDEP = 1;
            IFTUNIT = -216;
            SPECIFYTHTI = true;
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".uzf";
            _PackageInfo.ModuleName = "UZF";
            IsMandatory = true;
            Version = "UZF1";
            _Layer3DToken = "RegularGrid";

        }

        #region Properties
        /// <summary>
        /// An integer value used to define which cell in a vertical column that recharge and discharge is simulated.
        /// 1 Recharge to and discharge from only the top model layer. This option assumes land surface is defined as top of layer 1.
        /// 2 Recharge to and discharge from the specified layer in variable IUZFBND. This option assumes land surface is defined as top of layer specified in IUZFBND.
        /// 3 Recharge to and discharge from the highest active cell in each vertical column. Land surface is determined as top of layer specified in IUZFBND. A constant head node intercepts any recharge and prevents deeper percolation.
        /// </summary>
        /// 
        [Description("An integer value used to define which cell in a vertical column that recharge and discharge is simulated." +
"1 Recharge to and discharge from only the top model layer. This option assumes land surface is defined as top of layer 1." +
"2 Recharge to and discharge from the specified layer in variable IUZFBND. This option assumes land surface is defined as top of layer specified in IUZFBND." +
"3 Recharge to and discharge from the highest active cell in each vertical column. Land surface is determined as top of layer specified in IUZFBND. A constant head node intercepts any recharge and prevents deeper percolation.")]
        public int NUZTOP { get; set; }
        /// <summary>
        /// An integer value whose absolute value should equal 1 or 2. An absolute value of 1 indicates that the vertical hydraulic conductivity will be specified within the UZF1 Package input file using array VKS. An absolute value of 2 indicates that the vertical hydraulic conductivity will be specified within either either the BCF or LPF or UPW Package input file. If the BCF package is used, the absolute value of IUZFOPT should be set to 1. IF IUZFOPT is set to -1 or -2, unsaturated flow in the vadose zone is ignored.
        /// </summary>
        /// 
        [Description("An integer value whose absolute value should equal 1 or 2. An absolute value of 1 indicates that the vertical hydraulic conductivity will be specified within the UZF1 Package input file using array VKS. An absolute value of 2 indicates that the vertical hydraulic conductivity will be specified within either either the BCF or LPF or UPW Package input file. If the BCF package is used, the absolute value of IUZFOPT should be set to 1. IF IUZFOPT is set to -1 or -2, unsaturated flow in the vadose zone is ignored.")]
        public int IUZFOPT { get; set; }
        /// <summary>
        /// An integer value that specifies whether ground water that discharges to land surface will be routed to stream segments or lakes as specified in the IRUNBND array (IRUNFLG not equal to zero) or if ground-water discharge is removed from the model simulation and accounted for in the ground-water budget as a loss of water (IRUNFLG=0). The Streamflow-Routing (SFR2) and (or) the Lake (LAK3) Packages must be active if IRUNFLG is not zero.
        /// </summary>
        public int IRUNFLG { get; set; }
        /// <summary>
        /// An integer value that specifies whether or not evapotranspiration (ET) will be simulated. ET will not be simulated if IETFLG is zero, otherwise it will be simulated.
        /// </summary>
        public int IETFLG { get; set; }
        /// <summary>
        /// An integer value used as a flag for writing ground-water recharge, ET, and ground-water discharge to land surface rates to a separate unformatted file using subroutine UBUDSV. If IUZFCB1>0, it is the unit number to which the cell-by-cell rates will be written when “SAVE BUDGET” or a non-zero value for ICBCFL is specified in Output Control. 
        /// If IUZFCB1 is specified as a negative value, then infiltration and unsaturated zone ET are printed to unformatted files using UBUDSV. If IUZFCB1 is specified as a positive value, then only groundwater budget items calculated by UZF are printed, including recharge, groundwater discharge to land surface, and groundwater ET, and excludes infiltration and unsaturated zone ET.
        /// UBUDSV records cell-by-cell flow terms for each cell in the grid. In other packages, UBUDSV is called when the COMPACT BUDGET option is not used.
        /// </summary>
        public int IUZFCB1 { get; set; }

        public int IUZFCB2 { get; set; }

        public int NTRAIL2 { get; set; }
        public int NSETS2 { get; set; }

        public int NUZGAG { get; set; }
        /// <summary>
        /// The average height of undulations in the land surface altitude.
        /// </summary>
        public float SURFDEP { get; set; }

        /// <summary>
        /// An integer value equal to the unit number of the output file.
        /// A positive value is for output of individual cells 
        /// whereas a negative value is for output that is summed over all model cells.
        /// </summary>
        public int IFTUNIT { get; set; }
        /// <summary>
        /// An array of integer values used to define the aerial extent of the active model in which recharge and discharge will be simulated.
        /// </summary>
        /// 
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 1)]
        public DataCube<float> IUZFBND { get; set; }
        /// <summary>
        /// [Serial] An array of integer values used to define the stream segments within the Streamflow-Routing (SFR2) Package or lake numbers in the Lake (LAK3) Package to which overland runoff from excess infiltration and ground-water discharge to land surface will be added. 
        /// A positive integer value identifies the stream segment and a negative integer value identifies the lake number.
        /// </summary>
        /// 
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0)]
        public DataCube<float> IRUNBND { get; set; }
        /// <summary>
        ///  the saturated vertical hydraulic conductivity of the unsaturated zone (LT-1).
        /// </summary>
        /// 
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.001f)]
        public DataCube<float> VKS { get; set; }
        /// <summary>
        ///  define the Brooks-Corey epsilon of the unsaturated zone
        /// </summary>
        /// 
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.3f)]
        public DataCube<float> EPS { get; set; }
        /// <summary>
        ///  define the saturated water content of the unsaturated zone in units of volume of water to total volume (L3L-3).
        /// </summary>
        /// 
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.3f)]
        public DataCube<float> THTS { get; set; }
        /// <summary>
        /// Initial water content
        /// </summary>
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.2f)]
        public DataCube<float> THTI { get; set; }

        public DataCube<int> UZGAG { get; set; }

        /// <summary>
        /// [Serial]  define the infiltration rates (LT-1) at land surface for each vertical column of cells.
        /// </summary>
        /// 
        [StaticVariableItem("Stress Period")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.001f)]
        public DataCube<float> FINF { get; set; }
        /// <summary>
        /// An array of positive real values used to define the ET demand rates (L1T-1) within the ET extinction depth interval for each vertical column of cells.
        /// </summary>
        /// 
        [StaticVariableItem("Stress Period")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.001f)]
        public DataCube<float> PET { get; set; }
        /// <summary>
        /// An array of positive real values used to define the ET extinction depths.
        /// </summary>
        /// 
        [StaticVariableItem("Stress Period")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 2)]
        public DataCube<float> EXTDP { get; set; }
        /// <summary>
        /// An array of positive real values used to define the extinction water content below which ET cannot be removed from the unsaturated zone.
        /// Note 8: EXTWC must have a value between (THTS-Sy) and THTS, where Sy is the specific yield specified in either the LPF or BCF Package
        /// </summary>
        /// 
        [StaticVariableItem("Stress Period")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.1f)]
        public DataCube<float> EXTWC { get; set; }

        public bool SPECIFYTHTI { get; set; }
        #endregion

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            base.Initialize();
        }
        public override void New()
        {
            base.New();
        }
        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {

                var grid = (Owner.Grid as MFGrid);
                int layer = grid.ActualLayerCount;
                int ncell = grid.ActiveCellCount;
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    //# Data Set 1b: NUZTOP IUZFOPT IRUNFLG IETFLG IUZFCB1 IUZFCB2 NTRAIL2 NSETS2 NUZGAG SURFDEP
                    string newline = ReadComment(sr);
                    if (newline.ToUpper() == "SPECIFYTHTI")
                    {
                        SPECIFYTHTI = true;
                        newline = sr.ReadLine();
                    }
                    else
                    {
                        SPECIFYTHTI = false;
                    }

                    float[] fv = TypeConverterEx.Split<float>(newline, 10);
                    this.NUZTOP = (int)fv[0];
                    this.IUZFOPT = (int)fv[1];
                    this.IRUNFLG = (int)fv[2];
                    this.IETFLG = (int)fv[3];
                    this.IUZFCB1 = (int)fv[4];
                    this.IUZFCB2 = (int)fv[5];
                    this.NTRAIL2 = (int)fv[6];
                    this.NSETS2 = (int)fv[7];
                    this.NUZGAG = (int)fv[8];
                    this.SURFDEP = fv[9];

                    this.IUZFBND = new DataCube<float>(1, 1, grid.ActiveCellCount)
                    {
                        Name = "IUZFBND",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    this.IRUNBND = new DataCube<float>(1, 1, grid.ActiveCellCount)
                    {
                        Name = "IRUNBND",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    this.VKS = new DataCube<float>(1, 1, grid.ActiveCellCount)
                    {
                        Name = "VKS",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    this.EPS = new DataCube<float>(1, 1, grid.ActiveCellCount)
                    {
                        Name = "EPS",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    this.THTS = new DataCube<float>(1, 1, grid.ActiveCellCount)
                    {
                        Name = "THTS",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    this.THTI = new DataCube<float>(1, 1, grid.ActiveCellCount)
                    {
                        Name = "THTI",
                        ZeroDimension = DimensionFlag.Spatial
                    };

                    this.IUZFBND.Topology = grid.Topology;
                    this.IRUNBND.Topology = grid.Topology;
                    this.VKS.Topology = grid.Topology;
                    this.EPS.Topology = grid.Topology;
                    this.THTS.Topology = grid.Topology;
                    this.THTI.Topology = grid.Topology;

                    this.IUZFBND.Variables[0] = "IUZFBND Layer 1";
                    this.IRUNBND.Variables[0] = "IRUNBND Layer 1";
                    this.VKS.Variables[0] = "VKS Layer 1";
                    this.EPS.Variables[0] = "EPS Layer 1";
                    this.THTS.Variables[0] = "THTS Layer 1";
                    this.THTI.Variables[0] = "THTI Layer 1";

                    //# Data Set 2: IUZFBND
                    ReadSerialArray(sr, IUZFBND, 0, 0);
                    //Data Set 3
                    if (IRUNFLG > 0)
                        ReadSerialArray(sr, IRUNBND, 0, 0);
                    // Data Set 4
                    if (IUZFOPT == 1 || IUZFOPT == 0)
                        ReadSerialArray(sr, VKS, 0, 0);
                    //Data sets 5-7 are only read if IUZFOPT is greater than or equal to 1.
                    if (IUZFOPT >= 1)
                    {
                        ReadSerialArray(sr, EPS, 0, 0);
                        ReadSerialArray(sr, THTS, 0, 0);
                    }
                    if (SPECIFYTHTI)
                        ReadSerialArray(sr, THTI, 0, 0);
                    //If NUZGAG>0: Item 8 is repeated NUZGAG times
                    if (NUZGAG > 0)
                    {
                        newline = sr.ReadLine();
                        var iv = TypeConverterEx.Split<int>(newline);
                        if (iv.Length == 4)
                        {
                            UZGAG = new DataCube<int>(NUZGAG, 1, 4) { ZeroDimension = DimensionFlag.Spatial };
                            UZGAG[0, 0, 0] = iv[0];
                            UZGAG[0, 0, 1] = iv[1];
                            UZGAG[0, 0, 2] = iv[2];
                            UZGAG[0, 0, 3] = iv[3];
                            for (int i = 1; i < NUZGAG; i++)
                            {
                                newline = sr.ReadLine();
                                iv = TypeConverterEx.Split<int>(newline);
                                UZGAG[0, "0", ":"] = iv;
                                //UZGAG.Value[i][0][1] = iv[1];
                                //UZGAG.Value[i][0][2] = iv[2];
                                //UZGAG.Value[i][0][3] = iv[3];
                            }
                        }
                        else
                        {
                            this.IFTUNIT = iv[0];
                        }
                    }

                    var np = TimeService.StressPeriods.Count;
                    this.FINF = new DataCube<float>(np, 1, grid.ActiveCellCount)
                    {
                        Name = "FINF",
                        ZeroDimension = DimensionFlag.Time
                    };
                    this.PET = new DataCube<float>(np, 1, grid.ActiveCellCount)
                    {
                        Name = "PET",
                        ZeroDimension = DimensionFlag.Time
                    };
                    this.EXTDP = new DataCube<float>(np, 1, grid.ActiveCellCount)
                    {
                        Name = "EXTDP",
                        ZeroDimension = DimensionFlag.Time
                    };
                    this.EXTWC = new DataCube<float>(np, 1, grid.ActiveCellCount)
                    {
                        Name = "EXTWC",
                        ZeroDimension = DimensionFlag.Time
                    };

                    this.FINF.Topology = grid.Topology;
                    this.PET.Topology = grid.Topology;
                    this.EXTDP.Topology = grid.Topology;
                    this.EXTWC.Topology = grid.Topology;

                    for (int p = 0; p < np; p++)
                    {
                        newline = sr.ReadLine();
                        var iv = TypeConverterEx.Split<int>(newline, 1);
                        if (iv[0] >= 0)
                        {
                            //FINF[0, p] = ReadSerialArray<float>(sr).Value;
                            ReadSerialArray<float>(sr, FINF, p, 0);
                        }
                        else
                        {
                            FINF.Flags[p] = TimeVarientFlag.Repeat;
                            var buf = new float[grid.ActiveCellCount];
                            FINF[p - 1, "0", ":"].CopyTo(buf, 0);
                            FINF[p, "0", ":"] = buf;
                        }
                        if (IETFLG > 0)
                        {
                            newline = sr.ReadLine();
                            iv = TypeConverterEx.Split<int>(newline, 1);
                            if (iv[0] >= 0)
                            {
                                ReadSerialArray<float>(sr, PET, p, 0);
                            }
                            else
                            {
                                PET.Flags[p] = TimeVarientFlag.Repeat;
                                var buf = new float[grid.ActiveCellCount];
                                PET[p - 1, "0", ":"].CopyTo(buf, 0);
                                PET[p, "0", ":"] = buf;
                            }

                            newline = sr.ReadLine();
                            iv = TypeConverterEx.Split<int>(newline, 1);
                            if (iv[0] >= 0)
                            {
                                ReadSerialArray<float>(sr, EXTDP, p, 0);
                            }
                            else
                            {
                                EXTDP.Flags[p] = TimeVarientFlag.Repeat;
                                var buf = new float[grid.ActiveCellCount];
                                EXTDP[p - 1, "0", ":"].CopyTo(buf, 0);
                                EXTDP[p, "0", ":"] = buf;
                            }

                            newline = sr.ReadLine();
                            iv = TypeConverterEx.Split<int>(newline, 1);
                            if (iv[0] >= 0)
                            {
                                ReadSerialArray<float>(sr, EXTWC, p, 0);
                            }
                            else
                            {
                                EXTWC.Flags[p] = TimeVarientFlag.Repeat;
                                var buf = new float[grid.ActiveCellCount];
                                EXTWC[p - 1, "0", ":"].CopyTo(buf, 0);
                                EXTWC[p, "0", ":"] = buf;
                            }
                        }

                        this.FINF.Variables[p] = "FIN Stress Period " + (p + 1);
                        this.PET.Variables[p] = "PET Stress Period " + (p + 1);
                        this.EXTDP.Variables[p] = "EXTDP Stress Period " + (p + 1);
                        this.EXTWC.Variables[p] = "EXTWC Stress Period " + (p + 1);
                    }
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
                ShowWarning("Failed to load" + this.Name, progresshandler);
                result = LoadingState.Warning;
            }
            OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }

        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var cbc = mfout.SelectChild(CBCPackage.CBCName);
            this.IUZFCB1 = cbc.PackageInfo.FID;
        }
        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
        public override void SaveAs(string filename, ICancelProgressHandler prg)
        {
            int np = TimeService.StressPeriods.Count;
            var grid = this.Grid as IRegularGrid;
            int layer = grid.ActualLayerCount;

            StreamWriter sw = new StreamWriter(filename);
            //# Data Set 1b: NUZTOP IUZFOPT IRUNFLG IETFLG IUZFCB1 IUZFCB2 NTRAIL2 NSETS2 NUZGAG SURFDEP

            WriteDefaultComment(sw, "UZF");
            if (SPECIFYTHTI)
                sw.WriteLine("SPECIFYTHTI");
            string format = "";
            for (int i = 0; i < 10; i++)
            {
                format += "{" + string.Format("{0}", i) + "}\t";
            }
            string newline = string.Format(format + " # Data Set 1b: NUZTOP IUZFOPT IRUNFLG IETFLG IUZFCB1 IUZFCB2 NTRAIL2 NSETS2 NUZGAG SURFDEP",
                this.NUZTOP, this.IUZFOPT, this.IRUNFLG, this.IETFLG, this.IUZFCB1, this.IUZFCB2,
                 this.NTRAIL2, this.NSETS2, this.NUZGAG, this.SURFDEP);
            sw.WriteLine(newline);

            WriteSerialFloatArray(sw, this.IUZFBND, 0, 0, "F0", " # Data Set 2: IUZFBND");
            if (IRUNFLG > 0)
            {
                WriteSerialFloatArray(sw, this.IRUNBND, 0, 0, "F0", " # Data Set 3: IRUNBND ");
            }
            if (IUZFOPT == 1)
            {
                WriteSerialFloatArray(sw, this.VKS, 0, 0, "E6", " # Data Set 4: VKS");
            }
            WriteSerialFloatArray(sw, this.EPS, 0, 0, "E6", " # Data Set 5: EPS for Stress Period 0");
            WriteSerialFloatArray(sw, this.THTS, 0, 0, "E6", " # Data Set 6: THTS for Stress Period 0");

            if (SPECIFYTHTI)
                WriteSerialFloatArray(sw, this.THTI, 0, 0, "E6", " # Data Set 7: THTI for Stress Period 0");

            //todo
            if (NUZGAG > 0)
            {
                for (int n = 0; n < NUZGAG; n++)
                {
                    newline = string.Format("{0}\t# Data Set 8: IFTUNIT", this.IFTUNIT);
                    sw.WriteLine(newline);
                    IFTUNIT--;
                }
            }
            CheckEXTWC(prg);
            for (int p = 0; p < np; p++)
            {
                int reuse = this.FINF.Flags[p] == TimeVarientFlag.Repeat ? -1 : 0;
                newline = string.Format("{0}\t# Data Set 9: NUZF1", reuse);
                sw.WriteLine(newline);
                if (reuse >= 0)
                    WriteSerialFloatArray(sw, this.FINF, p, 0, "E6", " # Data Set 10 FINF  for Stress Period " + (p + 1));
                if (IETFLG > 0)
                {
                    reuse = this.PET.Flags[p] == TimeVarientFlag.Repeat ? -1 : 0;
                    newline = string.Format("{0}\t# Data Set 11: NUZF2", reuse);
                    sw.WriteLine(newline);
                    if (reuse >= 0)
                        WriteSerialFloatArray(sw, this.PET, p, 0, "E6", " # Data Set 12 PET  for Stress Period " + (p + 1));

                    reuse = this.EXTDP.Flags[p] == TimeVarientFlag.Repeat ? -1 : 0;
                    newline = string.Format("{0}\t# Data Set 13: NUZF3", reuse);
                    sw.WriteLine(newline);
                    if (reuse  >= 0)
                        WriteSerialFloatArray(sw, this.EXTDP, p, 0, "E6", " # Data Set 14 EXTDP  for Stress Period " + (p + 1));

                    reuse = this.EXTWC.Flags[p] == TimeVarientFlag.Repeat ? -1 : 0;
                    newline = string.Format("{0}\t# Data Set 15: NUZF4", reuse);
                    sw.WriteLine(newline);
                    if (reuse >= 0)
                        WriteSerialFloatArray(sw, this.EXTWC, p, 0, "E6", " # Data Set 16 EXTWC  for Stress Period " + (p + 1));
                }
            }
            sw.Close();
            this.OnSaved(prg);
        }

        public override void Clear()
        {
            if (_Initialized)
            {
                this.Grid.Updated -= this.OnGridUpdated;
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            }
            base.Clear();
        }
        /// <summary>
        /// EXTWC must have a value between (THTS-Sy) and THTS, where Sy is the specific yield specified in either the LPF or BCF Package
        /// </summary>
        private void CheckEXTWC(ICancelProgressHandler prg)
        {
            var lpf = Owner.GetPackage(LPFPackage.PackageName) as LPFPackage;
            int count_modfied = 0;
            for (int i = 0; i < MFGridInstance.ActiveCellCount; i++)
            {
                if (THTS[0, 0, i] < lpf.SY[0, 0, i])
                {
                    THTS[0, 0, i] = lpf.SY[0, 0, i];
                }
                var ds = THTS[0, 0, i] - lpf.SY[0, 0, i];
                if (ds == 0)
                {
                    EXTWC[0, 0, i] = 0.05f;
                    count_modfied++;
                }
                else
                {
                    if (EXTWC[0, 0, i] < ds)
                    {
                        EXTWC[0, 0, i] = ds;
                        count_modfied++;
                    }
                }
                if (EXTWC[0, 0, i] > THTS[0, 0, i])
                {
                    EXTWC[0, 0, i] = THTS[0, 0, i];
                    count_modfied++;
                }
            }

           // prg.Progress("uzf", 80, count_modfied + " cells are modified");
        }

        public override void OnGridUpdated(IGrid sender)
        {
            if (this.TimeService.StressPeriods.Count == 0)
                return;
            var mf = Owner as Modflow;
            var grid = sender as RegularGrid;
            this.FeatureLayer = this.Grid.FeatureLayer;
            this.Feature = this.Grid.FeatureSet;

            this.IUZFBND = new DataCube<float>(1, 1, grid.ActiveCellCount)
            {
                Name = "IUZFBND", ZeroDimension= DimensionFlag.Spatial
            };
            this.IRUNBND = new DataCube<float>(1, 1, grid.ActiveCellCount)
            {
                Name = "IRUNBND",                ZeroDimension = DimensionFlag.Spatial
            };
            this.VKS = new DataCube<float>(1, 1, grid.ActiveCellCount)
            {
                Name = "VKS",                ZeroDimension = DimensionFlag.Spatial
            };
            this.EPS = new DataCube<float>(1, 1, grid.ActiveCellCount)
            {
                Name = "EPS",ZeroDimension = DimensionFlag.Spatial
            };
            this.THTS = new DataCube<float>(1, 1, grid.ActiveCellCount)
            {
                Name = "THTS",ZeroDimension = DimensionFlag.Spatial
            };
            this.THTI = new DataCube<float>(1, 1, grid.ActiveCellCount)
            {
                Name = "THTI",ZeroDimension = DimensionFlag.Spatial
            };

            this.IUZFBND.Variables[0] = "IUZFBND Layer 1";
            this.IRUNBND.Variables[0] = "IRUNBND Layer 1";
            this.VKS.Variables[0] = "VKS Layer 1";
            this.EPS.Variables[0] = "EPS Layer 1";
            this.THTS.Variables[0] = "THTS Layer 1";
            this.THTI.Variables[0] = "THTI Layer 1";

            this.IUZFBND.Topology = grid.Topology;
            this.IRUNBND.Topology = grid.Topology;
            this.VKS.Topology = grid.Topology;
            this.EPS.Topology = grid.Topology;
            this.THTS.Topology = grid.Topology;
            this.THTI.Topology = grid.Topology;
            //# Data Set 2: IUZFBND
            for (int i = 0; i < grid.ActiveCellCount;i++ )
            {
                this.IUZFBND[0,0,i] = 1;
                this.VKS[0, 0, i] = 0.1f;
                this.EPS[0, 0, i] = 3.0f;
                this.THTS[0,0,i]  = 0.3f;
                this.THTI[0, 0, i] = 0.05f;
            }
            InitTVArrays();
            base.OnGridUpdated(sender);
        }

        public override void OnTimeServiceUpdated(ITimeService time)
        {
            if (ModflowInstance.Grid == null)
                return;
            InitTVArrays();
            base.OnTimeServiceUpdated(time);
        }

        private void InitTVArrays()
        {
            var mf = Owner as Modflow;
            var grid = mf.Grid as MFGrid;
            int ncell=grid.ActiveCellCount;
            int np = TimeService.StressPeriods.Count;
            this.FINF = new DataCube<float>(np, 1, grid.ActiveCellCount)
            {
                Name = "FINF",ZeroDimension = DimensionFlag.Spatial
            };
            this.PET = new DataCube<float>(np, 1, grid.ActiveCellCount)
            {
                Name = "PET",ZeroDimension = DimensionFlag.Spatial
            };
            this.EXTDP = new DataCube<float>(np, 1, grid.ActiveCellCount)
            {
                Name = "EXTDP",ZeroDimension = DimensionFlag.Spatial
            };
            this.EXTWC = new DataCube<float>(np, 1, grid.ActiveCellCount)
            {
                Name = "EXTWC",ZeroDimension = DimensionFlag.Spatial
            };

            this.FINF.Topology = grid.Topology;
            this.PET.Topology = grid.Topology;
            this.EXTDP.Topology = grid.Topology;
            this.EXTWC.Topology = grid.Topology;

            this.FINF.Variables[0] = "FIN Stress Period 1";
            this.PET.Variables[0] = "PET Stress Period 1";
            this.EXTDP.Variables[0] = "EXTDP Stress Period 1";
            this.EXTWC.Variables[0] = "EXTWC Stress Period 1";

            this.FINF.Flags[0] = TimeVarientFlag.Individual;
            this.PET.Flags[0] = TimeVarientFlag.Individual;
            this.EXTDP.Flags[0] = TimeVarientFlag.Individual;
            this.EXTWC.Flags[0] = TimeVarientFlag.Individual;
            //for (int i = 0; i < ncell; i++)
            //{
                this.FINF.ILArrays[0]["0",":"] = 0.0001f;
                this.PET.ILArrays[0]["0", ":"] = 0.003f;
                this.EXTDP.ILArrays[0]["0", ":"] = 3f;
                this.EXTWC.ILArrays[0]["0", ":"] = 0.15f;
            //}

            for (int p = 1; p < np; p++)
            {
                this.FINF.Flags[p] = TimeVarientFlag.Constant;
                this.PET.Flags[p] = TimeVarientFlag.Constant;
                this.EXTDP.Flags[p] = TimeVarientFlag.Constant;
                this.EXTWC.Flags[p] = TimeVarientFlag.Constant;

                this.FINF.Constants[p] = 0;
                this.PET.Constants[p] = 0;
                this.EXTDP.Constants[p] = 3f;
                this.EXTWC.Constants[p] = 0.15f;

                this.FINF.Variables[p] = "FIN Stress Period " + (p+1);
                this.PET.Variables[p] = "PET Stress Period " + (p + 1);
                this.EXTDP.Variables[p] = "EXTDP Stress Period " + (p + 1);
                this.EXTWC.Variables[p] = "EXTWC Stress Period " + (p + 1);
            }
        }
    }
}
