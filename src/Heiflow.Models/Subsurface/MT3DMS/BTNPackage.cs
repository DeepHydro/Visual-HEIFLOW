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
using Heiflow.Models.Subsurface.VFT3D;
using Heiflow.Models.UI;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface.MT3DMS
{
    [PackageItem]
    [PackageCategory("Basic", true)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class BTNPackage : MFPackage
    {
        public static string PackageName = "BTN";
        public BTNPackage()
        {
            Name = "BTN";
            _FullName = "Basic Transport Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".btn";
            _PackageInfo.ModuleName = "BTN";
            Description = "The BTN Package consists of nine primary modules";
            _Layer3DToken = "RegularGrid";
            Category = Modflow.MT3DCategory;

            ResetToDefault();
        }
        private int _NCOMP;

        #region
        [Category("Grid")]
        public int NLAY
        {
            get;
            set;
        }
        [Category("Grid")]
        public int NROW
        {
            get;
            set;
        }
        [Category("Grid")]
        public int NCOL
        {
            get;
            set;
        }
        [Category("Time")]
        public int NPER
        {
            get;
            set;
        }

        [Category("Chemical")]
        /// <summary>
        /// NCOMP is the total number of chemical species included in the current simulation. For single-species simulation, set NCOMP = 1;
        /// </summary>
        public int NCOMP
        {
            get
            {
                return _NCOMP;
            }
            set
            {
                _NCOMP = value;
            }
        }
        [Category("Chemical")]
        /// <summary>
        /// MCOMP is the total number of “mobile” species. MCOMP must be equal to or less than NCOMP. For single-species simulation, set MCOMP=1.
        /// </summary>
        public int MCOMP
        {
            get;
            set;
        }

        [Category("Units")]
        public string TUNIT
        {
            get;
            set;
        }
        [Category("Units")]
        public string LUNIT
        {
            get;
            set;
        }
        [Category("Units")]
        public string MUNIT
        {
            get;
            set;
        }
        [Category("Packages")]
        [Description("TRNOP are logical flags for major transport and solution options. TRNOP(1) to (5) correspond to Advection, Dispersion, Sink & Source Mixing, Chemical Reaction, and Generalized Conjugate Gradient Solver packages, respectively")]
        [Browsable(false)]
        /// <summary>
        /// TRNOP are logical flags for major transport and solution options. TRNOP(1) to (5) correspond to Advection, Dispersion, Sink & Source Mixing, Chemical Reaction, and Generalized Conjugate Gradient Solver packages, respectively.
        /// </summary>
        public string TRNOP
        {
            get;
            set;
        }
        [Category("Packages")]
        public bool EnableADV
        {
            get;
            set;
        }
        [Category("Packages")]
        public bool EnableDSP
        {
            get;
            set;
        }
        [Category("Packages")]
        public bool EnableSSM
        {
            get;
            set;
        }
        [Category("Packages")]
        public bool EnableRCT
        {
            get;
            set;
        }
        [Category("Packages")]
        public bool EnableGCG
        {
            get;
            set;
        }

        [Category("Grid")]
        public int[] LAYCON
        {
            get;
            set;
        }
        [Category("Grid")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube2DLayout<float> DELR
        {
            get;
            set;
        }
        [Category("Grid")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> DELC
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> HTOP
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> DZ
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> PRSITY
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<int> ICBUND
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> SCONC
        {
            get;
            set;
        }
        [Browsable(false)]
        public float[] InitialConcentraion
        {
            get;
            set;
        }

        [Category("Inactive Option")]
        [Description("Indicating an inactive concentration cell")]
        public float CINACT
        {
            get;
            set;
        }
        [Category("Grid")]
        [Description("the minimum saturated thickness in a cell, expressed as the decimal fraction of the model layer thickness (DZ) below which the cell is considered inactive. The default value is 0.01")]
        public float THKMIN
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("a flag indicating whether the calculated concentration should be printed to the standard output text file and also serves as a printing-format code if it is printed")]
        public int IFMTCN
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("a flag indicating whether the number of particles in each cell should be printed to the standard output text file and also serves as a printing-format code if it is printed")]
        public int IFMTNP
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("a flag indicating whether the model-calculated retardation factor should be printed to the standard output text file and also serves as a printing-format code if it is printed")]
        public int IFMTRF
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("a flag indicating whether the distance-weighted dispersion coefficient should be printed to the standard output text file and also serves as a printing-format code if it is printed")]
        public int IFMTDP
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("indicating whether the concentration solution should be saved in a default unformatted (binary) file named MT3Dnnn.UCN, where nnn is the species index number, for post-processing purposes or for use as the initial condition in a continuation run.")]
        public bool SAVUCN
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("indicating the frequency of the output and also indicating whether the output frequency is specified in terms of total elapsed simulation time or the transport step number.")]
        public int NPRS
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("The total elapsed time at which the simulation results are printed to the standard output text file or saved in the default unformatted (binary) concentration file")]
        public float[] TIMPRS
        {
            get;
            set;
        }
        [Category("Observation Output")]
        [Description("the number of observation points at which the concentration of each species will be saved")]
        public int NOBS
        {
            get;
            set;
        }
        [Category("Observation Output")]
        [Description("an integer indicating how frequently the concentration at the specified observation points should be saved")]
        public int NPROBS
        {
            get;
            set;
        }
        [Category("Observation Output")]
        [Description("cell indices (layer, row, column) in which the observation point or monitoring well is located")]
        [StaticVariableItem]
        public DataCube2DLayout<int> OBS
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("a logical flag indicating whether a one-line summary of mass balance information should be printed")]
        public bool CHKMAS
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("an integer indicating how frequently the mass budget information should be saved in the mass balance summary file MT3Dnnn.MAS")]
        public int NPRMAS
        {
            get;
            set;
        }
        [Category("Header")]
        public string Head1
        {
            get;
            set;
        }
        [Category("Header")]
        public string Head2
        {
            get;
            set;
        }
        #endregion

        public override void ResetToDefault()
        {
            TUNIT = "d";
            LUNIT = "m";
            MUNIT = "kg";
            Version = "BTN";
            _NCOMP = 7;
            MCOMP = 5;
            IsMandatory = false;
            TRNOP = "T T T F T F F F F F";
            NLAY = 3;
            NPER = 1;
            NROW = 100;
            NCOL = 100;
            EnableADV = true;
            EnableDSP = true;
            EnableGCG = true;
            EnableRCT = false;
            EnableSSM = true;
            CINACT = -999;
            THKMIN = 0.0f;
            IFMTCN = 0;
            IFMTNP = 0;
            IFMTRF = 0;
            IFMTDP = 0;
            SAVUCN = true;
            NOBS = 0;
            NPROBS = 1;
            CHKMAS = true;
            NPRMAS = 1;

            Head1 = "MT3DMS Simulation";
            Head2 = DateTime.Now.ToString();
        }

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
            ResetToDefault();
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
                    int k = 0;
                    var grid = Owner.Grid as MFGrid;
                    var mf = Owner as Modflow;
                    Head1 = sr.ReadLine();
                    Head2 = sr.ReadLine();
                    var line = sr.ReadLine();
                    var intbufs = TypeConverterEx.Split<int>(line);
                    float[] ffbufs = null;
                    NLAY = intbufs[0];
                    NROW = intbufs[1];
                    NCOL = intbufs[2];
                    NPER = intbufs[3];
                    NCOMP = intbufs[4];
                    MCOMP = intbufs[5];
                    line = sr.ReadLine();
                    var strbufs = TypeConverterEx.Split<string>(line);
                    TUNIT = strbufs[0];
                    LUNIT = strbufs[1];
                    MUNIT = strbufs[2];
                    line = sr.ReadLine();
                    TRNOP = line;
                    strbufs = TypeConverterEx.Split<string>(line);
                    EnableADV = strbufs[0].ToUpper() == "T";
                    EnableDSP = strbufs[1].ToUpper() == "T";
                    EnableSSM = strbufs[2].ToUpper() == "T";
                    EnableRCT = strbufs[3].ToUpper() == "T";
                    EnableGCG = strbufs[4].ToUpper() == "T";

                    line = sr.ReadLine();
                    LAYCON = TypeConverterEx.Split<int>(line, NLAY);
                    DELR = new DataCube2DLayout<float>(1, 1, NCOL);
                    DELC = new DataCube2DLayout<float>(1, 1, NROW);
                    InitArrays(grid);

                    ReadSerialArray(sr, DELR, 0, 0, NCOL);
                    ReadSerialArray(sr, DELC, 0, 0, NROW);
                    ReadSerialArray<float>(sr, HTOP, 0, 0);
                    for (int l = 0; l < NLAY; l++)
                    {
                        ReadSerialArray<float>(sr, DZ, l, 0);
                    }
                    for (int l = 0; l < NLAY; l++)
                    {
                        ReadSerialArray<float>(sr, PRSITY, l, 0);
                    }
                    for (int l = 0; l < NLAY; l++)
                    {
                        ReadSerialArray<int>(sr, ICBUND, l, 0);
                    }
                    k = 0;
                    for (int i = 0; i < NCOMP; i++)
                    {
                        for (int j = 0; j < NLAY; j++)
                        {
                            ReadSerialArray<float>(sr, SCONC, k, 0);
                            k++;
                        }
                    }
                    line = sr.ReadLine();
                    strbufs = TypeConverterEx.Split<string>(line);
                    CINACT = float.Parse(strbufs[0]);
                    if (strbufs.Length > 1)
                    {
                        THKMIN = float.Parse(strbufs[1]);
                    }
                    line = sr.ReadLine();
                    strbufs = TypeConverterEx.Split<string>(line);
                    IFMTCN = int.Parse(strbufs[0]);
                    IFMTNP = int.Parse(strbufs[1]);
                    IFMTRF = int.Parse(strbufs[2]);
                    IFMTDP = int.Parse(strbufs[3]);
                    SAVUCN = strbufs[4].ToUpper() == "T";
                    line = sr.ReadLine().Trim();
                    intbufs = TypeConverterEx.Split<int>(line);
                    NPRS = intbufs[0];
                    int nline = (int)Math.Ceiling(NPRS / 8.0);
                    line = sr.ReadLine();
                    line += "\t";
                    for (int c = 1; c < nline; c++)
                    {
                        line += sr.ReadLine() + "\t";
                    }
                    TIMPRS = TypeConverterEx.Split<float>(line);

                    line = sr.ReadLine();
                    strbufs = TypeConverterEx.Split<string>(line);
                    NOBS = int.Parse(strbufs[0]);
                    if (strbufs.Length > 1)
                    {
                        NPROBS = int.Parse(strbufs[1]);
                    }
                    if (NOBS > 0)
                    {
                        OBS = new DataCube2DLayout<int>(1, NOBS, 3);
                        OBS.ColumnNames[0] = "Layer";
                        OBS.ColumnNames[1] = "Row";
                        OBS.ColumnNames[2] = "Column";
                        for (int i = 0; i < NOBS; i++)
                        {
                            line = sr.ReadLine();
                            intbufs = TypeConverterEx.Split<int>(line);
                            OBS.ILArrays[0][i, ":"] = intbufs;
                        }
                    }

                    line = sr.ReadLine();
                    strbufs = TypeConverterEx.Split<string>(line);
                    CHKMAS = strbufs[0].ToUpper() == "T";
                    if (strbufs.Length > 1)
                    {
                        NPRMAS = int.Parse(strbufs[1]);
                    }
                    for (int i = 0; i < NPER; i++)
                    {
                        line = sr.ReadLine();
                        ffbufs = TypeConverterEx.Split<float>(line);
                        if (ffbufs[2] <= 0)
                        {
                            nline = (int)Math.Ceiling(NPER / 8.0);
                            line = sr.ReadLine();
                            line += "\t";
                            for (int c = 1; c < nline; c++)
                            {
                                line += sr.ReadLine() + "\t";
                            }
                            TimeService.StressPeriods[i].TSLNGH = TypeConverterEx.Split<float>(line);
                        }
                        line = sr.ReadLine();
                        strbufs = TypeConverterEx.Split<string>(line);
                        TimeService.StressPeriods[i].DT0 = float.Parse(strbufs[0]);
                        TimeService.StressPeriods[i].MXSTRN = int.Parse(strbufs[1]);
                        if (strbufs.Length >= 3)
                            TimeService.StressPeriods[i].TTSMULT = float.Parse(strbufs[2]);
                        if (strbufs.Length >= 4)
                            TimeService.StressPeriods[i].TTSMAX = float.Parse(strbufs[3]);
                    }

                    //TimeService.StressPeriods
                    result = LoadingState.Normal;
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
        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var mf = Owner as Modflow;
            UCNPackage acn = new UCNPackage()
             {
                 Owner = mf,
                 Parent = this,
                 FileName = this.FileName
             };
            mfout.AddChild(acn);
        }
        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            string line = "";
            sw.WriteLine(Head1);
            sw.WriteLine(Head2);

            line = string.Format("{0}{1}{2}{3}{4}{5}", NLAY.ToString().PadLeft(10, ' '), NROW.ToString().PadLeft(10, ' '), NCOL.ToString().PadLeft(10, ' '), NPER.ToString().PadLeft(10, ' '), NCOMP.ToString().PadLeft(10, ' '), MCOMP.ToString().PadLeft(10, ' '));
            sw.WriteLine(line);

            line = string.Format("{0}{1}{2}", TUNIT.PadLeft(4, ' '), LUNIT.PadLeft(4, ' '), MUNIT.PadLeft(4, ' '));
            sw.WriteLine(line);

            line = string.Format("{0} {1} {2} {3} {4} F F F F F", EnableADV ? "T" : "F", EnableDSP ? "T" : "F", EnableSSM ? "T" : "F", EnableRCT ? "T" : "F", EnableGCG ? "T" : "F");
            sw.WriteLine(line);

            line = " " + string.Join(" ", LAYCON);
            sw.WriteLine(line);

            WriteRegularArrayMT3D(sw, DELR, 0, "F6", 15, "G15.6");
            WriteRegularArrayMT3D(sw, DELC, 0, "F6", 15, "G15.6");
            WriteSerialFloatArrayMT3D(sw, HTOP, 0, 0, "F6", 15, 10, "G15.6");

            for (int i = 0; i < NLAY; i++)
            {
                // WriteSerialFloatArray(sw, DZ, i, 0, "E6", "Thickness of Layer " + (i + 1));
                WriteSerialFloatArrayMT3D(sw, DZ, i, 0, "F6", 15, 10, "G15.6");
            }
            for (int i = 0; i < NLAY; i++)
            {
                //WriteSerialFloatArray(sw, PRSITY, i, 0, "E6", "PRSITY of Layer " + (i + 1));
                WriteSerialFloatArrayMT3D(sw, PRSITY, i, 0, "F6", 15, 10, "G15.6");
            }
            var max_col = 30;
            for (int i = 0; i < NLAY; i++)
            {
                //WriteSerialArray<int>(sw, ICBUND, i, 0, "F0", "Boundary Condition Type of Layer " + (i + 1));
                WriteSerialIntegerArrayMT3D(sw, ICBUND, i, 0, "F0", 3, max_col, "I3");
            }

            var k = 0;
            //    var cmt = "";
            for (int i = 0; i < NCOMP; i++)
            {
                for (int j = 0; j < NLAY; j++)
                {
                    // cmt = string.Format("Starting Concentration of Species {0} in Layer {1}", i + 1, j + 1);
                    //WriteSerialFloatArray(sw, SCONC, k, 0, "E6", cmt);
                    WriteSerialFloatArrayMT3D(sw, SCONC, k, 0, "F6", 15, 10, "G15.6");
                    k++;
                }
            }

            line = string.Format("{0}{1}", CINACT.ToString().PadLeft(10, ' '), THKMIN.ToString().PadLeft(10, ' '));
            sw.WriteLine(line);

            line = string.Format("{0}{1}{2}{3}{4}", IFMTCN.ToString().PadLeft(10, ' '), IFMTNP.ToString().PadLeft(10, ' '), IFMTRF.ToString().PadLeft(10, ' '), IFMTDP.ToString().PadLeft(10, ' '), (SAVUCN ? "T" : "F").PadLeft(10, ' '));
            sw.WriteLine(line);

            line = string.Format("{0}", NPRS.ToString().PadLeft(10, ' '));
            sw.WriteLine(line);

            if (NPRS > 0)
            {
                int nline = (int)Math.Ceiling(NPRS / 8.0);
                k = 8;
                var t = 0;
                while (k <= NPRS)
                {
                    line = "";
                    for (int i = 0; i < 8; i++)
                    {
                        line += TIMPRS[t].ToString("F0").PadLeft(10, ' ');
                        t++;
                    }
                    sw.WriteLine(line);
                    k = k + 8;
                }
                line = "";
                for (; t < NPRS; t++)
                {
                    line += TIMPRS[t].ToString("F0").PadLeft(10, ' ');
                }
                sw.WriteLine(line);
            }

            line = string.Format("{0}{1}", NOBS.ToString().PadLeft(10, ' '), NPROBS.ToString().PadLeft(10, ' '));
            sw.WriteLine(line);

            if (NOBS > 0)
            {
                for (int i = 0; i < NOBS; i++)
                {
                    line = string.Format("{0}{1}{2}", OBS[0, i, 0].ToString().PadLeft(10, ' '), OBS[0, i, 1].ToString().PadLeft(10, ' '), OBS[0, i, 2].ToString().PadLeft(10, ' '));
                    sw.WriteLine(line);
                }
            }

            line = string.Format("{0}{1}", (CHKMAS ? "T" : "F").PadLeft(10, ' '), NPRMAS.ToString().PadLeft(10, ' '));
            sw.WriteLine(line);

            for (int i = 0; i < NPER; i++)
            {
                line = string.Format("{0}{1}{2}", TimeService.StressPeriods[i].NumTimeSteps.ToString().PadLeft(10, ' '), TimeService.StressPeriods[i].NSTP.ToString().PadLeft(10, ' '), TimeService.StressPeriods[i].Multiplier.ToString().PadLeft(10, ' '));
                sw.WriteLine(line);
                line = string.Format("{0}{1}{2}{3}", TimeService.StressPeriods[i].DT0.ToString().PadLeft(10, ' '), TimeService.StressPeriods[i].MXSTRN.ToString().PadLeft(10, ' '), TimeService.StressPeriods[i].TTSMULT.ToString().PadLeft(10, ' '), TimeService.StressPeriods[i].TTSMAX.ToString().PadLeft(10, ' '));
                sw.WriteLine(line);
            }

            sw.Close();
            OnSaved(progress);
        }
      
        public override void OnGridUpdated(IGrid sender)
        {
            if (this.TimeService.StressPeriods.Count == 0)
                return;
            ResetToDefault();

            var mf = Owner as Modflow;
            var grid = sender as RegularGrid;
            var lpf = Owner.GetPackage(LPFPackage.PackageName) as LPFPackage;
            this.FeatureLayer = this.Grid.FeatureLayer;
            this.Feature = this.Grid.FeatureSet;
            var k = 0;

            NLAY = grid.ActualLayerCount;
            NCOL = grid.ColumnCount;
            NROW = grid.RowCount;

            LAYCON = lpf.LAYTYP;
            DELR = new DataCube2DLayout<float>(1, 1, NCOL);
            DELC = new DataCube2DLayout<float>(1, 1, NROW);

            for (int i = 0; i < NCOL; i++)
            {
                DELR[0, 0, i] = grid.DELR[0, 0, i];
            }
            for (int i = 0; i < NROW; i++)
            {
                DELC[0, 0, i] = grid.DELC[0, 0, i];
            }

            InitArrays(grid);

            for (int i = 0; i < grid.ActiveCellCount; i++)
            {
                this.HTOP[0, 0, i] = grid.Elevations[0, 0, i];
            }
            for (int i = 0; i < grid.ActiveCellCount; i++)
            {
                for (int j = 1; j < grid.LayerCount; j++)
                {
                    this.DZ[j - 1, 0, i] = grid.Elevations[j-1, 0, i] - grid.Elevations[j, 0, i];
                }
            }
            for (int j = 0; j < grid.ActualLayerCount; j++)
            {
                PRSITY.ILArrays[j][0, ":"] = 0.15f;
                ICBUND.ILArrays[j][0, ":"] = 1;
            }

            if (InitialConcentraion != null)
            {
                for (int i = 0; i < NCOMP; i++)
                {
                    for (int j = 0; j < NLAY; j++)
                    {
                        SCONC.ILArrays[k][0, ":"] = InitialConcentraion[k];
                        k++;
                    }
                }
            }
            base.OnGridUpdated(sender);
        }

        public override void OnTimeServiceUpdated(ITimeService time)
        {
            NPER = TimeService.StressPeriods.Count;
            NPRS = TimeService.StressPeriods.Count;
            TIMPRS = new float[NPRS];
            var acctime = TimeService.StressPeriods[0].NumTimeSteps;
            TIMPRS[0] = acctime;
            for (int i = 1; i < NPRS; i++)
            {
                acctime += TimeService.StressPeriods[i].NumTimeSteps;
                TIMPRS[i] = acctime;
            }
            for (int i = 0; i < NPER; i++)
            {
                TimeService.StressPeriods[i].DT0 = 0;
                TimeService.StressPeriods[i].MXSTRN = 1000;
                TimeService.StressPeriods[i].TTSMULT = 1;
                TimeService.StressPeriods[i].TTSMAX = 0;
            }

            base.OnTimeServiceUpdated(time);
        }

        private void InitArrays(RegularGrid grid)
        {
            var k = 0;
            HTOP = new DataCube<float>(1, 1, grid.ActiveCellCount)
            {
                Variables = new string[] { "Top Elevation" },
                Topology = grid.Topology,
                ZeroDimension = DimensionFlag.Spatial
            };
            DZ = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Topology = grid.Topology,
                ZeroDimension = DimensionFlag.Spatial
            };
            PRSITY = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Topology = grid.Topology,
                ZeroDimension = DimensionFlag.Spatial
            };
            ICBUND = new DataCube<int>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Topology = grid.Topology,
                ZeroDimension = DimensionFlag.Spatial
            };
            SCONC = new DataCube<float>(grid.ActualLayerCount * NCOMP, 1, grid.ActiveCellCount)
            {
                Topology = grid.Topology,
                ZeroDimension = DimensionFlag.Spatial
            };
            for (int i = 0; i < NLAY; i++)
            {
                DZ.Variables[i] = "Thickness of Layer " + (i + 1);
                PRSITY.Variables[i] = "Porosity of Layer " + (i + 1);
                ICBUND.Variables[i] = "Boundary condition of Layer " + (i + 1);
                PRSITY.ILArrays[i][0, ":"] = 0.1f;
            }
            for (int i = 0; i < NCOMP; i++)
            {
                for (int j = 0; j < NLAY; j++)
                {
                    SCONC.Variables[k] = string.Format("Starting concentration of Specie {0} in Layer {1}", (i + 1), j + 1);
                    SCONC.ILArrays[k][0, ":"] = 0.01f;
                    k++;
                }
            }
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

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

        public void OnNCOMPChanged()
        {
            if (NCOMP < 1)
                NCOMP = 1;
            if (MCOMP > NCOMP)
                MCOMP = NCOMP;

            var phcpck = Owner.GetPackage(PHCPackage.PackageName) as PHCPackage;
            if(phcpck != null)
            {
                phcpck.NumAquComponents = NCOMP;

            }
        }
    }
}