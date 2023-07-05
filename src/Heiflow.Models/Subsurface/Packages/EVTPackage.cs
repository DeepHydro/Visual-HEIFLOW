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
    [PackageCategory("Boundary Conditions,Specified Head", true)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class EVTPackage : MFPackage
    {
        public static string PackageName = "EVT";
        public EVTPackage()
        {
            Name = "EVT";
            _FullName = "Evapotranspiration Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".evt";
            _PackageInfo.ModuleName = "EVT";
            Description = "EVT is used to simulate a head-dependent flux out of the model distributed over the top of the model and specified in units of length/time";
            Version = "EVT";
            IsMandatory = false;
            _Layer3DToken = "EVT";
            NEVTOP = 1;
            Category = Resources.HeadDependentCategory;
        }
        [Category("General")]
        [Description("1—ET is calculated only for cells in the top grid layer.  2—The cell for each vertical column is specified by the user in variable IEVT. 3—Evapotranspiration is applied to the highest active cell in each vertical column.")]
        /// <summary>
        /// 1—ET is calculated only for cells in the top grid layer.  2—The cell for each vertical column is specified by the user in variable IEVT. 3—Evapotranspiration is applied to the highest active cell in each vertical column.
        /// </summary>
        public int NEVTOP { get; set; }

        [Category("Output")]
        [Description("If IEVTCB > 0, it is the unit number to which cell-by-cell flow terms will be written when SAVE BUDGET or a non-zero value for ICBCFL is specified in Output Control. If IEVTCB ≤ 0, cell-by-cell flow terms will not be written.")]
        /// <summary>
        ///If IEVTCB > 0, it is the unit number to which cell-by-cell flow terms will be written when SAVE BUDGET or a non-zero value for ICBCFL is specified in Output Control. If IEVTCB ≤ 0, cell-by-cell flow terms will not be written.
        /// </summary>
        public int IEVTCB { get; set; }

        /// <summary>
        /// The ET surface elevation; [NSP,1,ActCellCount]
        /// </summary>
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube<float> SURF { get; set; }
        /// <summary>
        /// The extinction depth; [NSP,1,ActCellCount]
        /// </summary>
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube<float> EXDP { get; set; }
        /// <summary>
        /// [NSP,1,ActCellCount]
        /// </summary>
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube<float> EVTR { get; set; }

        /// <summary>
        /// [NSP,1,ActCellCount]
        /// </summary>
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube<int> IEVT { get; set; }

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
            var mf = Owner as Modflow;
            NEVTOP = 1;
            this.IEVTCB = mf.NameManager.GetFID(".cbc");
            base.New();
        }
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
        public override void CompositeOutput(MFOutputPackage mfout)
        {

        }

        private void InitArrays()
        {
            int nsp = TimeService.StressPeriods.Count;
            SURF = new DataCube<float>(nsp, 1, this.Grid.ActiveCellCount)
            {
                ZeroDimension = DimensionFlag.Time
            };
            EXDP = new DataCube<float>(nsp, 1, this.Grid.ActiveCellCount)
            {
                ZeroDimension = DimensionFlag.Time
            };
            EVTR = new DataCube<float>(nsp, 1, this.Grid.ActiveCellCount)
            {
                ZeroDimension = DimensionFlag.Time
            };
            IEVT = new DataCube<int>(nsp, 1, this.Grid.ActiveCellCount)
            {
                ZeroDimension = DimensionFlag.Time
            };
            for (int i = 0; i < nsp; i++)
            {
                SURF.Variables[i] = "Layer numbers in Stress Period " + (i + 1);
                EVTR.Variables[i] = "ET in Stress Period " + (i + 1);
                EXDP.Variables[i] = "ET in Stress Period " + (i + 1);
                IEVT.Variables[i] = "ET Layer in Stress Period " + (i + 1);

                if (i > 0)
                {
                    SURF.Flags[i] = TimeVarientFlag.Repeat;
                    SURF.Multipliers[i] = 1;
                    SURF.IPRN[i] = -1;

                    EVTR.Flags[i] = TimeVarientFlag.Repeat;
                    EVTR.Multipliers[i] = 1;
                    EVTR.IPRN[i] = -1;

                    EXDP.Flags[i] = TimeVarientFlag.Repeat;
                    EXDP.Multipliers[i] = 1;
                    EXDP.IPRN[i] = -1;

                    IEVT.Flags[i] = TimeVarientFlag.Repeat;
                    IEVT.Multipliers[i] = 1;
                    IEVT.IPRN[i] = -1;
                }
                else
                {
                    SURF.Flags[i] = TimeVarientFlag.Individual;
                    SURF.Multipliers[i] = 1;
                    SURF.IPRN[i] = -1;
                    SURF.ILArrays[0][0, ":"] = 0.0f;

                    EVTR.Flags[i] = TimeVarientFlag.Individual;
                    EVTR.Multipliers[i] = 1;
                    EVTR.IPRN[i] = -1;
                    EVTR.ILArrays[0][0, ":"] = 0.0f;

                    EXDP.Flags[i] = TimeVarientFlag.Individual;
                    EXDP.Multipliers[i] = 1;
                    EXDP.IPRN[i] = -1;
                    EXDP.ILArrays[0][0, ":"] = 0.0f;

                    IEVT.Flags[i] = TimeVarientFlag.Individual;
                    IEVT.Multipliers[i] = 1;
                    IEVT.IPRN[i] = -1;
                    IEVT.ILArrays[0][0, ":"] = 1;
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
                    var mf = Owner as Modflow;
                    var nsp = mf.TimeService.StressPeriods.Count;
                    //Data Set 1: # OPTIONS
                    string newline = ReadComment(sr);
                    var intbuf = TypeConverterEx.Split<int>(newline);
                    int INSURF, INEVTR, INEXDP ,INIEVT;
                    NEVTOP = intbuf[0];
                    IEVTCB = intbuf[1];

                    SURF = new DataCube<float>(nsp, 1, this.Grid.ActiveCellCount)
                    {
                        ZeroDimension = DimensionFlag.Time
                    };
                    EXDP = new DataCube<float>(nsp, 1, this.Grid.ActiveCellCount)
                    {
                        ZeroDimension = DimensionFlag.Time
                    };
                    EVTR = new DataCube<float>(nsp, 1, this.Grid.ActiveCellCount)
                    {
                        ZeroDimension = DimensionFlag.Time
                    };
                    IEVT = new DataCube<int>(nsp, 1, this.Grid.ActiveCellCount)
                    {
                        ZeroDimension = DimensionFlag.Time
                    };
                    for (int i = 0; i < nsp; i++)
                    {

                        SURF.Variables[i] = "SURF in Stress Period " + (i + 1);
                        EXDP.Variables[i] = "EXDP in Stress Period " + (i + 1);
                        EVTR.Variables[i] = "EVTR in Stress Period " + (i + 1);
                        newline = sr.ReadLine();
                        //INSURF INEVTR INEXDP INIEVT
                        intbuf = TypeConverterEx.Split<int>(newline);
                        INSURF = intbuf[0];
                        INEVTR = intbuf[1];
                        INEXDP = intbuf[2];
                        INIEVT = intbuf[3];
                        if (INSURF >= 0)
                        {
                            ReadSerialArray<float>(sr, SURF, i, 0);
                        }
                        else
                        {
                            SURF.Flags[i] = TimeVarientFlag.Repeat;
                            SURF.Multipliers[i] = 1;
                            SURF.IPRN[i] = -1;
                        }

                        if (INEVTR >= 0)
                        {
                            ReadSerialArray<float>(sr, EVTR, i, 0);
                        }
                        else
                        {
                            EVTR.Flags[i] = TimeVarientFlag.Repeat;
                            EVTR.Multipliers[i] = 1;
                            EVTR.IPRN[i] = -1;
                        }

                        if (INEXDP >= 0)
                        {
                            ReadSerialArray<float>(sr, EXDP, i, 0);
                        }
                        else
                        {
                            EXDP.Flags[i] = TimeVarientFlag.Repeat;
                            EXDP.Multipliers[i] = 1;
                            EXDP.IPRN[i] = -1;
                        }

                        if ( NEVTOP == 2)
                        {
                            if (INIEVT >= 0)
                            {
                                ReadSerialArray<int>(sr, IEVT, i, 0);
                            }
                            else
                            {
                                IEVT.Flags[i] = TimeVarientFlag.Repeat;
                                IEVT.Multipliers[i] = 1;
                                IEVT.IPRN[i] = -1;
                            }
                        }
                    }

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
            string line = string.Format("{0}  {1} # NEVTOP, IEVTCB", NEVTOP, IEVTCB);
            sw.WriteLine(line);
            sw.WriteLine("1 1 1 0 # INSURF INEVTR INEXDP INIEVT for Stress Period 1");
            WriteSerialFloatArray(sw, SURF, 0, 0, "F6", " # SURF");
            WriteSerialFloatArray(sw, EVTR, 0, 0, "F6", " # EVTR");
            WriteSerialFloatArray(sw, EXDP, 0, 0, "F6", " # EXDP");
            if (NEVTOP == 2)
            {
                WriteSerialArray<int>(sw, IEVT, 0, 0, "F0", " # IEVT");
            }

            for (int i = 1; i < nsp; i++)
            {
                sw.WriteLine("-1 -1 -1 0 # INSURF INEVTR INEXDP INIEVT for Stress Period 1");
                sw.WriteLine(line);
            }

            sw.Close();
            OnSaved(progress);
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            if (ModflowInstance.Grid == null)
                return;
            InitArrays();
            base.OnTimeServiceUpdated(time);
        }
        public override void OnGridUpdated(IGrid sender)
        {
            if (this.TimeService.StressPeriods.Count == 0)
                return;
            InitArrays();
            base.OnGridUpdated(sender);
        }
        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;
            SURF = null;
            EVTR = null;
            EXDP = null;
            IEVT = null;
            base.Clear();
        }

    }
}
