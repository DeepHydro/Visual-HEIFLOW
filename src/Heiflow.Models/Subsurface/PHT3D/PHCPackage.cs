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

namespace Heiflow.Models.Subsurface.PHT3D
{
    [PackageItem]
    [PackageCategory("Basic", true)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class PHCPackage : MFPackage
    {
        public static string PackageName = "PHC";
        private string[] _AllComponentsNames;
        public PHCPackage()
        {
            Name = "PHC";
            _FullName = "Reactive Multicomponent Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".dat";
            _PackageInfo.ModuleName = "PHC";
            Description = "Simulate a wide range of mixed equilibrium/kinetic geochemical reactions";
            Version = "PHC";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
            TEMP = 25;
            OS = 2;
        }

        [Category("Scheme")]
        [Description("a flag for the operator-splitting scheme used: 1 = iterative operator-splitting scheme; 2 = sequential operator-splitting scheme with reactions calculated after flow time steps only; 3 = sequential operator-splitting scheme with reactions calculated after each transport step")]
        public int OS
        {
            get;
            set;
        }
        [Category("Scheme")]
        [Description("the temperature in ◦C used in chemical reactions for which a temperature dependence is defined in the database file")]
        public float TEMP
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("A flag that determines if ASCII output files (extension .ACN) that contain the computed concentrations for all grid-cells and for all output times that were defined in the BTN file; 0 = Output to binary files only; 1 = Output to both binary and ASCII files")]
        public int ASBIN
        {
            get;
            set;
        }

        [Category("Components")]
        [Description("NR_SOL_MST_SPEC_EQU: Defines the number of aqueous components that are assumed to be in chemical equilibrium and included in the simulation")]
        public int NumAquComponents
        {
            get;
            set;
        }
        [Category("Components")]
        [Description("EXCHANGE_SPECIES: Defines the number of exchaning components")]
        public int NumExchSpecies
        {
            get;
            set;
        }
        [Category("Components")]
        [Description("EXCHANGE_SPECIES: Defines the number of exchaning components")]
        public int TotalNumComponents
        {
            get
            {
                return NumAquComponents + NumExchSpecies;
            }
        }
        [Category("Components")]
        [Description("")]
        public string[] AquComponentsNames
        {
            get;
            private set;
        }
        [Category("Components")]
        [Description("")]
        public string[] ExchSpeciesNames
        {
            get;
            private set;
        }
        [Category("Components")]
        [Description("")]
        public string[] AllComponentsNames
        {
            get
            {
                return _AllComponentsNames;
            }
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            this.TimeService.PopulateTimelineFromSP(DateTime.Now);
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
                    string line = sr.ReadLine();
                  var  strs = TypeConverterEx.Split<string>(line);
                    OS = int.Parse(strs[0]);
                    TEMP = float.Parse(strs[1]);
                    ASBIN = int.Parse(strs[2]);

                    line = sr.ReadLine();
                    line = sr.ReadLine();
                     strs = TypeConverterEx.Split<string>(line);
                    NumAquComponents = int.Parse(strs[0]);
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    NumExchSpecies = int.Parse(strs[0]);
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    AquComponentsNames = new string[NumAquComponents];
                    ExchSpeciesNames = new string[NumExchSpecies];
                    _AllComponentsNames = new string[TotalNumComponents];
                    for (int i = 0; i < NumAquComponents; i++)
                    {
                        line = sr.ReadLine();
                        strs = TypeConverterEx.Split<string>(line);
                        AquComponentsNames[i] = strs[0];
                        _AllComponentsNames[i] = strs[0];
                    }
                    for (int i = 0; i < NumExchSpecies; i++)
                    {
                        line = sr.ReadLine();
                        strs = TypeConverterEx.Split<string>(line);
                        ExchSpeciesNames[i] = strs[0] + "-X";
                        _AllComponentsNames[NumAquComponents + i] = ExchSpeciesNames[i];
                    }
                  
                    result = LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progress);
                    result = LoadingState.FatalError;
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
                result = LoadingState.FatalError;
            }
            OnLoaded(progress, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }

        public override void SaveAs(string filename,ICancelProgressHandler progress)
        {
            var grid = (Owner.Grid as IRegularGrid);
            //StreamWriter sw = new StreamWriter(filename);
            //WriteDefaultComment(sw, this.Name);         
            //sw.Close();
            OnSaved(progress);
        }

        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var mf = Owner as Modflow;
            ACNPackage acn = new ACNPackage()
                 {
                     Owner = mf,
                     Parent = this,
                     FileName = this.FileName
                 };
            mfout.AddChild(acn);
        }

        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;
            base.Clear();
        }

        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
    }
}
