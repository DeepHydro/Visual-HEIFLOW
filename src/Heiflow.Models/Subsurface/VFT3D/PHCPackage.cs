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
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface.MT3DMS;
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
using System.Windows.Forms;

namespace Heiflow.Models.Subsurface.VFT3D
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
            //ResetToDefault();
            Category = Resources.PHTCategory; 
        }

        #region 
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
        [Category("Scheme")]
        [Description("the activation/deactivation criterion. If the value is set to 0, PHREEQC-2 will be executed for all grid-cells")]
        public float EPS_AQU
        {
            get;
            set;
        }
        [Category("Scheme")]
        [Description("")]
        public float EPS_PH
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

        [Category("Solve")]
        [Description("a number that acts as a flag to indicate if the charge imbalance carried by a solution is to be transported. If CB OFFSET = 0, which is the recommended setting, then the charge imbalance is not transported")]
        public int CB_OFFSET
        {
            get;
            set;
        }

        [Category("Components")]
        [Description("NR_SOL_MST_SPEC_EQU: Defines the number of aqueous components that are assumed to be in chemical equilibrium and included in the simulation")]
        public int NumAquComponents
        {
            get
            {
                var mf = Owner as Modflow;
                return mf.MobileSpeciesManager.NumSelectedSpecies;
            }
        }
        [Category("Components")]
        [Description("the number of minerals included in the simulation and for which the local equilibrium assumption (LEA) is assumed to be valid")]
        public int NR_MIN_EQU
        {
            get;
            set;
        }
        [Category("Components")]
        [Description("the number of surface master species")]
        public int NR_SURF
        {
            get;
            set;
        }
     
        [Category("Components")]
        [Description("the number of ")]
        public int NR_MOB_KIN
        {
            get;
            set;
        }
        [Category("Components")]
        [Description("the number of ")]
        public int NR_MIN_KIN
        {
            get;
            set;
        }
        [Category("Components")]
        [Description("the number of ")]
        public int NR_SURF_KIN
        {
            get;
            set;
        }
        [Category("Components")]
        [Description("the number of ")]
        public int NR_IMOB_KIN
        {
            get;
            set;
        }

        [Category("Components")]
        [Description("EXCHANGE_SPECIES: Defines the number of exchaning components")]
        public int NumExchSpecies
        {
            get
            {
                var mf = Owner as Modflow;
                return mf.ExchangeSpeciesManager.NumSelectedSpecies;
            }
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
            get
            {
                var mf = Owner as Modflow;
                var buf = from sp in mf.MobileSpeciesManager.SpeciesCollection where sp.Selected select sp.Name;
                return buf.ToArray();
            }
        }
        [Category("Components")]
        [Description("")]
        public string[] ExchSpeciesNames
        {
            get
            {
                var mf = Owner as Modflow;
                var buf = from sp in mf.ExchangeSpeciesManager.SpeciesCollection where sp.Selected select sp.Name;
                return buf.ToArray();
            }
        }
        [Category("Components")]
        [Description("")]
        public string[] AllComponentsNames
        {
            get
            {
                var mf = Owner as Modflow;
                _AllComponentsNames = new string[TotalNumComponents];
                for (int i = 0; i < NumAquComponents; i++)
                {
                    _AllComponentsNames[i] = AquComponentsNames[i];
                }
                for (int i = 0; i < NumExchSpecies; i++)
                {
                    _AllComponentsNames[NumAquComponents + i] = ExchSpeciesNames[i];
                }
                return _AllComponentsNames;
            }
        }
        #endregion

        public override void ResetToDefault()
        {
            TEMP = 25;
            OS = 2;
            ASBIN = 1;
        
            //AquComponentsNames = new string[9];
            //for (int i = 0; i < 7; i++)
            //    AquComponentsNames[i] = SpeciesManager._default_mobile_species[i];
            //AquComponentsNames[7] = "pH";
            //AquComponentsNames[8] = "pe";

            
            //ExchSpeciesNames = new string[3];
            //SpeciesManager._default_exchange_species.CopyTo(ExchSpeciesNames, 0);

            string dbfile = Path.Combine(Application.StartupPath, "data\\pht3d_datab.dat");
            var mf = Owner as Modflow;
            if (mf != null)
            {
                mf.MobileSpeciesManager.LoadCollectionFromDB(dbfile);
                mf.ExchangeSpeciesManager.LoadCollectionFromDB(dbfile);
                mf.MineralSpeciesManager.LoadCollectionFromDB(dbfile);
                mf.MobileSpeciesManager.SetDefaultMobileSpecies();
                mf.MobileSpeciesManager.CheckMandaryMobileSpecies();
                mf.ExchangeSpeciesManager.SetDefaultExchangeSpecies();
            }
        }

        private void SelectSpecies()
        {
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            //this.TimeService.PopulateTimelineFromSP(DateTime.Now);
            base.Initialize();
        }
        public override void New()
        {
            ResetToDefault();
            this._FileName = ".\\pht3d_ph.dat";
            var info = (Owner as Modflow).NameManager.GetPckInfo(PackageName);
            info.SetRelativeFileName(this._FileName);
            base.New();
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                string dbfile = Path.Combine(Application.StartupPath, "data\\pht3d_datab.dat");

                StreamReader sr = new StreamReader(FileName);
                try
                {
                    var mf = Owner as Modflow;
                    mf.MobileSpeciesManager.LoadCollectionFromDB(dbfile);
                    mf.ExchangeSpeciesManager.LoadCollectionFromDB(dbfile);
                    mf.MineralSpeciesManager.LoadCollectionFromDB(dbfile);

                    string line = sr.ReadLine();
                    var strs = TypeConverterEx.Split<string>(line);
                    OS = int.Parse(strs[0]);
                    TEMP = float.Parse(strs[1]);
                    ASBIN = int.Parse(strs[2]);
                    EPS_AQU = float.Parse(strs[3]);
                    EPS_PH = float.Parse(strs[4]);

                    line = sr.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    CB_OFFSET = int.Parse(strs[0]);

                    line = sr.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                   var  numAquComponents = int.Parse(strs[0]);

                    line = sr.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    NR_MIN_EQU = int.Parse(strs[0]);

                    line = sr.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                  var  numExchSpecies = int.Parse(strs[0]);

                    line = sr.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    NR_SURF = int.Parse(strs[0]);

                    line = sr.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    NR_MOB_KIN = int.Parse(strs[0]);
                    NR_MIN_KIN = int.Parse(strs[1]);
                    NR_SURF_KIN = int.Parse(strs[2]);
                    NR_IMOB_KIN = int.Parse(strs[3]);

                    //AquComponentsNames = new string[NumAquComponents];
                    //ExchSpeciesNames = new string[NumExchSpecies];

                    for (int i = 0; i < numAquComponents; i++)
                    {
                        line = sr.ReadLine();
                        strs = TypeConverterEx.Split<string>(line);
                        //AquComponentsNames[i] = strs[0];

                        var sp = mf.MobileSpeciesManager.Select(strs[0]);
                        if (sp != null)
                            sp.Selected = true;
                    }
                    mf.MobileSpeciesManager.CheckMandaryMobileSpecies();

                    for (int i = 0; i < numExchSpecies; i++)
                    {
                        line = sr.ReadLine();
                        strs = TypeConverterEx.Split<string>(line);
                        //ExchSpeciesNames[i] = strs[0];
                        var sp = mf.ExchangeSpeciesManager.Select(strs[0]);
                        if (sp != null)
                        {
                            sp.Selected = true;
                            var ion = 1;
                            int.TryParse(strs[1], out ion); 
                            sp.LonNum = ion;
                        }
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
            var mf = Owner as Modflow;
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            string line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t# OS, TEMP, ASBIN, EPS_AQU, EPS_PH", OS, TEMP, ASBIN, EPS_AQU, EPS_PH);
            sw.WriteLine(line);

            line = string.Format("{0}\t# CB_OFFSET", CB_OFFSET);
            sw.WriteLine(line);

            line = string.Format("{0}\t# NR_SOL_MST_SPEC_EQU", NumAquComponents);
            sw.WriteLine(line);

            line = string.Format("{0}\t# NR_MIN_EQU", NR_MIN_EQU);
            sw.WriteLine(line);

            line = string.Format("{0}\t# NR_ION_EX", NumExchSpecies);
            sw.WriteLine(line);

            line = string.Format("{0}\t# NR_SURF", NR_SURF);
            sw.WriteLine(line);

            line = string.Format("{0}\t{1}\t{2}\t{3}\t# NR_MOB_KIN, NR_MIN_KIN, NR_SURF_KIN, NR_IMOB_KIN", NR_MOB_KIN, NR_MIN_KIN, NR_SURF_KIN, NR_IMOB_KIN);
            sw.WriteLine(line);

            //foreach (var sp in mf.MobileSpeciesManager.SpeciesCollection)
            //{
            //    if (sp.Selected)
            //        sw.WriteLine(sp.Name);
            //}
            //foreach (var sp in mf.ExchangeSpeciesManager.SpeciesCollection)
            //{
            //    if (sp.Selected)
            //        sw.WriteLine(sp.Name + " " + sp.LonNum);
            //}
            var lines = new string[] { 
"Ca",
"Cl",
"Mg",
"Na", 
"S(6)",
"C(4)",
"K", 
"pH", 
"pe", 
"Ca             2",  
"Na             1",  
"Mg             2" 
            };
            foreach (var str in lines)
                sw.WriteLine(str);

            sw.Close();
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
