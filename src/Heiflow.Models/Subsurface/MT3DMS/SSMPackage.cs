﻿//
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
using Heiflow.Models.Properties;
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
    public class SSMPackage : MFPackage
    {
        public static string PackageName = "SSM";
        public SSMPackage()
        {
            Name = SSMPackage.PackageName;
            _FullName = "Sink Source Mixing Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".ssm";
            _PackageInfo.ModuleName = "SSM";
            Description = "The SSM Package";
            Version = "SSM";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
            Category = Resources.MT3DCategory; 
            ResetToDefault();
        }

        [Category("Sink & Source Package")]
        public bool FWEL
        {
            get;
            set;
        }
        [Category("Sink & Source Package")]
        public bool FDRN
        {
            get;
            set;
        }
        [Category("Sink & Source Package")]
        public bool FRCH
        {
            get;
            set;
        }
        [Category("Sink & Source Package")]
        public bool FEVT
        {
            get;
            set;
        }
        [Category("Sink & Source Package")]
        public bool FRIV
        {
            get;
            set;
        }
        [Category("Sink & Source Package")]
        public bool FGHB
        {
            get;
            set;
        }
        [Category("Sink & Source Package")]
        public int MXSS
        {
            get;
            set;
        }
        [Category("RCH Package")]
        [Browsable(false)]
        [StaticVariableArrayItem]
        public DataCube<float>[] CRCH
        {
            get;
            set;
        }

        [Category("EVT Package")]
        [Browsable(false)]
        [StaticVariableArrayItem]
        public DataCube<float>[] CEVT
        {
            get;
            set;
        }
        [Category("Point Sources")]
        [Description("the number of point sources whose concentrations need to be specified")]
        [Browsable(false)]
        public int[] NSS
        {
            get;
            set;
        }
        [Category("Point Sources")]
        [Description("KSS, ISS, JSS, CSS, ITYPE, (CSSMS(n), n=1,NCOMP)")]
        [Browsable(false)]
        [StaticVariableArrayItem]
        public DataCube2DLayout<float>[] PointSources
        {
            get;
            set;
        }


        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override void ResetToDefault()
        {
            FWEL = false;
            FDRN = false;
            FRCH = false;
            FEVT = false;
            FRIV = false;
            FGHB = false;
        }

        public override void New()
        {
            ResetToDefault();
            base.New();
        }
        public void InitArrays()
        {
            var grid = Owner.Grid as MFGrid;
            var mf = Owner as Modflow;
            var nsp = TimeService.StressPeriods.Count;
            var btnpck = mf.GetPackage(BTNPackage.PackageName) as BTNPackage;
            CRCH = new DataCube<float>[btnpck.NCOMP];
            CEVT = new DataCube<float>[btnpck.NCOMP];
            PointSources = new DataCube2DLayout<float>[nsp];
            NSS = new int[nsp];

            for (int i = 0; i < btnpck.NCOMP; i++)
            {
                CRCH[i] = new DataCube<float>(nsp, 1, grid.ActiveCellCount)
                {
                    Name = "Spiece " + (i + 1),
                    ZeroDimension = DimensionFlag.Time
                };
                for (int j = 0; j < nsp; j++)
                {
                    CRCH[i].Variables[j] = "Concentration in Stress Period " + (j + 1);
                }
                CEVT[i] = new DataCube<float>(nsp, 1, grid.ActiveCellCount)
                {
                    Name = "Spiece " + (i + 1),
                    ZeroDimension = DimensionFlag.Time
                };
                for (int j = 0; j < nsp; j++)
                {
                    CEVT[i].Variables[j] = "Concentration in Stress Period " + (j + 1);
                }
            }
        }

        public void InitArrays(int nsp, int ncomp)
        {
            var grid = Owner.Grid as MFGrid;
            CRCH = new DataCube<float>[ncomp];
            CEVT = new DataCube<float>[ncomp];
            PointSources = new DataCube2DLayout<float>[nsp];
            NSS = new int[nsp];

            for (int i = 0; i < ncomp; i++)
            {
                CRCH[i] = new DataCube<float>(nsp, 1, grid.ActiveCellCount)
                {
                    Name = "Spiece " + (i + 1),
                    ZeroDimension = DimensionFlag.Time
                };
                for (int j = 0; j < nsp; j++)
                {
                    CRCH[i].Variables[j] = "Concentration in Stress Period " + (j + 1);
                }
                CEVT[i] = new DataCube<float>(nsp, 1, grid.ActiveCellCount)
                {
                    Name = "Spiece " + (i + 1),
                    ZeroDimension = DimensionFlag.Time
                };
                for (int j = 0; j < nsp; j++)
                {
                    CEVT[i].Variables[j] = "Concentration in Stress Period " + (j + 1);
                }
            }
        }
        
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    var grid = Owner.Grid as MFGrid;
                    var mf = Owner as Modflow;
                    string line = sr.ReadLine();
                    var bufs = TypeConverterEx.Split<string>(line);
                    var nsp= TimeService.StressPeriods.Count;
                    var btnpck =  mf.GetPackage(BTNPackage.PackageName) as BTNPackage;
                    int[] intbuf = null;

                    InitArrays();
                    FWEL = bufs[0].ToUpper() == "T";
                    FDRN = bufs[1].ToUpper() == "T";
                    FRCH = bufs[2].ToUpper() == "T";
                    FEVT = bufs[3].ToUpper() == "T";
                    FRIV = bufs[4].ToUpper() == "T";
                    FGHB = bufs[5].ToUpper() == "T";
                    line = sr.ReadLine();
                    MXSS = int.Parse(line.Trim());

                    for (int i = 0; i < nsp; i++)
                    {
                        if (FRCH)
                        {
                            line = sr.ReadLine();
                            intbuf = TypeConverterEx.Split<int>(line);

                            if (intbuf[0] >= 0)
                            {
                                for (int j = 0; j < btnpck.NCOMP; j++)
                                {
                                    ReadSerialArray<float>(sr, CRCH[j], i, 0);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < btnpck.NCOMP; j++)
                                {
                                    CRCH[j].Flags[i] = TimeVarientFlag.Repeat;
                                    CRCH[j].Multipliers[i] = 1;
                                    CRCH[j].IPRN[i] = -1;
                                }
                            }
                        }

                        if (FEVT)
                        {
                            line = sr.ReadLine();
                           intbuf = TypeConverterEx.Split<int>(line);

                            if (intbuf[0] >= 0)
                            {
                                for (int j = 0; j < btnpck.NCOMP; j++)
                                {
                                    ReadSerialArray<float>(sr, CEVT[j], i, 0);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < btnpck.NCOMP; j++)
                                {
                                    CEVT[j].Flags[i] = TimeVarientFlag.Repeat;
                                    CEVT[j].Multipliers[i] = 1;
                                    CEVT[j].IPRN[i] = -1;
                                }
                            }
                        }

                        line = sr.ReadLine();
                        intbuf = TypeConverterEx.Split<int>(line);
                        NSS[i] = intbuf[0];
                        var ncol = 5 + btnpck.NCOMP;
                      
                        if (NSS[i] > 0)
                        {
                            PointSources[i] = new DataCube2DLayout<float>(1, NSS[i], ncol);
                            PointSources[i].Name = "Point Source in Stress Period " + (i + 1);
                            PointSources[i].ColumnNames[0]= "Layer";
                            PointSources[i].ColumnNames[1]= "Row";
                            PointSources[i].ColumnNames[2] = "Column";
                            PointSources[i].ColumnNames[3] = "Source Conc.";
                            PointSources[i].ColumnNames[4] = "Type";
                            for (int j = 0; j < btnpck.NCOMP; j++)
                            {
                                PointSources[i].ColumnNames[j + 5] = "Conc. " + (j + 1);
                            }
                            for (int j = 0; j < NSS[i]; j++)
                            {
                                line = sr.ReadLine();
                                var ffbuf = TypeConverterEx.Split<float>(line);
                                if(ffbuf.Length == 5)
                                {
                                    for (int k = 0; k < 5; k++)
                                    {
                                        PointSources[i].ILArrays[0][j, k] = ffbuf[k];
                                    }
                                }
                                else if (ffbuf.Length == ncol)
                                {
                                    PointSources[i].ILArrays[0][j, ":"] = ffbuf;
                                }
                            }
                        }
                        else
                        {
                            PointSources[i] = new DataCube2DLayout<float>(1, 1, 1);
                            PointSources[i][0, 0, 0] = -999;
                            PointSources[i].Name = "Point Source in Stress Period " + (i + 1);
                        }
                    }


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

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            var grid = (Owner.Grid as IRegularGrid);
            var nsp = TimeService.StressPeriods.Count;
            var mf = Owner as Modflow;
            var btnpck = mf.GetPackage(BTNPackage.PackageName) as BTNPackage;

            StreamWriter sw = new StreamWriter(filename);
            string line = string.Format("{0}{1}{2}{3}{4}{5} F F F F", FWEL ? " T" : " F", FDRN ? " T" : " F", FRCH ? " T" : " F", FEVT ? " T" : " F", FRIV ? " T" : " F", FGHB ? " T" : " F");
            sw.WriteLine(line);
            sw.WriteLine(MXSS.ToString().PadLeft(10, ' '));
            for (int i = 0; i < nsp; i++)
            {
                if (FRCH)
                {
                    if (CRCH[0].Flags[i] == TimeVarientFlag.Repeat)
                    {
                        sw.WriteLine("        -1");
                    }
                    else
                    {
                        sw.WriteLine("         1");
                        for (int j = 0; j < btnpck.NCOMP; j++)
                        {
                            WriteSerialFloatArrayMT3D(sw, CRCH[j], i, 0, "F6", 15, 10, "G15.6");
                        }
                    }
                }
                if (FEVT)
                {
                    if (CEVT[0].Flags[i] == TimeVarientFlag.Repeat)
                    {
                        sw.WriteLine("        -1");
                    }
                    else
                    {
                        sw.WriteLine("         1");
                        for (int j = 0; j < btnpck.NCOMP; j++)
                        {
                            WriteSerialFloatArrayMT3D(sw, CEVT[j], i, 0, "F6", 15, 10, "G15.6");
                        }
                    }
                }
                sw.WriteLine(NSS[i].ToString().PadLeft(10, ' '));
                if (NSS[i] > 0)
                {
                    for (int k = 0; k < NSS[i]; k++)
                    {
                        line = string.Format("{0}{1}{2}{3}{4}", PointSources[i][0, k, 0].ToString("F0").PadLeft(10, ' '), PointSources[i][0, k, 1].ToString("F0").PadLeft(10, ' '), PointSources[i][0, k, 2].ToString("F0").PadLeft(10, ' '),
                            PointSources[i][0, k, 3].ToString("F6").PadLeft(10, ' '), PointSources[i][0, k, 4].ToString("F0").PadLeft(10, ' '));

                        for (int j = 0; j < btnpck.NCOMP; j++)
                        {
                            line += PointSources[i][0, k, 5+j].ToString("F6").PadLeft(10, ' ');
                        }
                        sw.WriteLine(line);
                    }
                }
            }
            sw.Close();
            OnSaved(progress);
        }

        public void SaveAsFile(string filename,int nsp, int ncomp)
        {
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            string line = string.Format("{0}{1}{2}{3}{4}{5} F F F F", FWEL ? " T" : " F", FDRN ? " T" : " F", FRCH ? " T" : " F", FEVT ? " T" : " F", FRIV ? " T" : " F", FGHB ? " T" : " F");
            sw.WriteLine(line);
            sw.WriteLine(MXSS.ToString().PadLeft(10, ' '));
            for (int i = 0; i < nsp; i++)
            {
                if (FRCH)
                {
                    if (CRCH[0].Flags[i] == TimeVarientFlag.Repeat)
                    {
                        sw.WriteLine("        -1");
                    }
                    else
                    {
                        sw.WriteLine("         1");
                        for (int j = 0; j < ncomp; j++)
                        {
                            WriteSerialFloatArrayMT3D(sw, CRCH[j], i, 0, "F6", 15, 10, "G15.6");
                        }
                    }
                }
                if (FEVT)
                {
                    if (CEVT[0].Flags[i] == TimeVarientFlag.Repeat)
                    {
                        sw.WriteLine("        -1");
                    }
                    else
                    {
                        sw.WriteLine("         1");
                        for (int j = 0; j < ncomp; j++)
                        {
                            WriteSerialFloatArrayMT3D(sw, CEVT[j], i, 0, "F6", 15, 10, "G15.6");
                        }
                    }
                }
                sw.WriteLine(NSS[i].ToString().PadLeft(10, ' '));
                if (NSS[i] > 0)
                {
                    for (int k = 0; k < NSS[i]; k++)
                    {
                        line = string.Format("{0}{1}{2}{3}{4}", PointSources[i][0, k, 0].ToString("F0").PadLeft(10, ' '), PointSources[i][0, k, 1].ToString("F0").PadLeft(10, ' '), PointSources[i][0, k, 2].ToString("F0").PadLeft(10, ' '),
                            PointSources[i][0, k, 3].ToString("F6").PadLeft(10, ' '), PointSources[i][0, k, 4].ToString("F0").PadLeft(10, ' '));

                        for (int j = 0; j < ncomp; j++)
                        {
                            line += PointSources[i][0, k, 5 + j].ToString("F6").PadLeft(10, ' ');
                        }
                        sw.WriteLine(line);
                    }
                }
            }
            sw.Close();
        }
        public override void OnGridUpdated(IGrid sender)
        {
            if (this.TimeService.StressPeriods.Count == 0)
                return;
            InitArrays();
            base.OnGridUpdated(sender);
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            if (ModflowInstance.Grid == null)
                return;
            InitArrays();
       
            base.OnTimeServiceUpdated(time);
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
    }
}