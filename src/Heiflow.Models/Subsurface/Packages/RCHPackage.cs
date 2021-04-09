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
    public class RCHPackage : MFPackage
    {
        public static string PackageName = "CHD";
        public RCHPackage()
        {
            Name = "RCH";
            _FullName = "Recharge Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".rch";
            _PackageInfo.ModuleName = "RCH";
            Description = "The Recharge package is used to simulate a specified flux distributed over the top of the model and specified in units of length/time.  Within MODFLOW, these rates are multiplied by the horizontal area of the cells to which they are applied to calculate the volumetric flux rates";
            Version = "RCH";
            IsMandatory = false;
            _Layer3DToken = "RCH";
            NRCHOP = 3;
            Category = Modflow.FluxCategory;
        }
        [Category("General")]
        [Description("the recharge option code. Recharge fluxes are defined in a layer variable.  1—Recharge is only to the top grid layer. 2—Vertical distribution of recharge is specified in layer variable IRCH. 3—Recharge is applied to the highest active cell in each vertical column. A constant-head node intercepts recharge and prevents deeper infiltration.")]
        /// <summary>
        /// the recharge option code. Recharge fluxes are defined in a layer variable.  1—Recharge is only to the top grid layer. 2—Vertical distribution of recharge is specified in layer variable IRCH. 3—Recharge is applied to the highest active cell in each vertical column. A constant-head node intercepts recharge and prevents deeper infiltration.
        /// </summary>
        public int NRCHOP { get; set; }

        [Category("Output")]
        [Description("If IRCHCB > 0, it is the unit number to which cell-by-cell flow terms will be written when SAVE BUDGET or a non-zero value for ICBCFL is specified in Output Control. If IRCHCB ≤ 0, cell-by-cell flow terms will not be written.")]
        /// <summary>
        /// If IRCHCB > 0, it is the unit number to which cell-by-cell flow terms will be written when "SAVE BUDGET" or a non-zero value for ICBCFL is specified in Output Control. If IRCHCB ≤ 0, cell-by-cell flow terms will not be written.
        /// </summary>
        public int IRCHCB { get; set; }

        /// <summary>
        /// [NSP,1,ActCellCount]
        /// </summary>
        /// 
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube<float> RECH { get; set; }

        /// <summary>
        /// [NSP,1,ActCellCount]
        /// </summary>
        /// 
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube<int> IRCH { get; set; }

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
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
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
                    NRCHOP = intbuf[0];
                    IRCHCB = intbuf[1];
                    RECH = new DataCube<float>(nsp, 1, this.Grid.ActiveCellCount)
                    {
                        ZeroDimension = DimensionFlag.Time
                    };
                    IRCH = new DataCube<int>(nsp, 1, this.Grid.ActiveCellCount)
                    {
                        ZeroDimension = DimensionFlag.Time
                    };
                    for (int i = 0; i < nsp; i++)
                    {
                        RECH.Variables[i] = "Recharge in Stress Period " + (i + 1);
                        IRCH.Variables[i] = "Layer numbers in Stress Period " + (i + 1);
                        var ss = TypeConverterEx.Split<int>(sr.ReadLine(), 2);
                        if (ss[0] >= 0)
                        {
                            ReadSerialArray<float>(sr, RECH, i, 0);
                        }
                        else
                        {
                            RECH.Flags[i] = TimeVarientFlag.Repeat;
                            RECH.Multipliers[i] = 1;
                            RECH.IPRN[i] = -1;
                        }
                        if (ss[1] >= 0 && NRCHOP == 2)
                        {
                            ReadSerialArray<int>(sr, IRCH, i, 0);
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
            string line = string.Format("{0}  {1} # NRCHOP, IRCHCB", NRCHOP, IRCHCB);
            sw.WriteLine(line);
            sw.WriteLine("1 1 # INRECH, INIRCH for Stress Period 1");
            WriteSerialFloatArray(sw, RECH, 0, 0, "F6", " # RECH");
            if (NRCHOP == 2)
            {
                WriteSerialArray<int>(sw, IRCH, 0, 0, "F0", " # IRCH");
            }

            for (int i = 1; i < nsp; i++)
            {
                line = RECH.Flags[i] == TimeVarientFlag.Repeat ? "-1 " : "1 ";
                line += IRCH.Flags[i] == TimeVarientFlag.Repeat ? "-1 " : "1 ";
                line += " # INRECH, INIRCH for Stress Period " + (i + 1);
                sw.WriteLine(line);
                if (RECH.Flags[i] != TimeVarientFlag.Repeat)
                {
                    WriteSerialFloatArray(sw, RECH, i, 0, "F6", " # RECH");
                }
                if (IRCH.Flags[i] != TimeVarientFlag.Repeat)
                {
                    WriteSerialArray<int>(sw, IRCH, i, 0, "F0", " # IRCH");
                }
            }

            sw.Close();
            OnSaved(progress);
        }
    
        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;
            RECH = null;
            base.Clear();
        }
     
    }
}
