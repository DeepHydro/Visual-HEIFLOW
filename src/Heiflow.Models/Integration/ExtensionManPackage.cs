using Heiflow.Core.Data;
using Heiflow.Models.Generic;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Integration
{
    public class ExtensionManPackage : Package
    {
        public static string PackageName = "Extension Manager";
       // private string _BudgetsSumExFile;
        private string _MFOutputExFile;
        private string _SFRExFile;
         private string   _SFRWQFile;
         private string _SFROutEXFile;
         private string _LakeExFile;
         private string _LAIDAExFile;
         private string _MF_IOLOG_File;
         private string _GWHDA_File;
         private string _ABM_MODEL_File;
         private string _CHDEX_File;

        public ExtensionManPackage()
        {
            this.Name = PackageName;
            TimeService = new TimeService("Base Timeline");
            IsMandatory = true;
            FullName = "Extension Manager";
            InitValues();
        }
        [Category("Time")]
        [Description("Start date time represented by Julian")]
        public int StartInJulian
        {
            get;
            set;
        }
        [Category("Solver Extension")]
        [Description("Enable solver extension")]
        public int SolverExFlag
        {
            get;
            set;
        }
        [Category("Solver Extension")]
        [Description("Maximum MODFLOW iteration in transit peroid")]
        public int Max_TS_ITER
        {
            get;
            set;
        }
        [Category("Solver Extension")]
        [Description("HCLOSER in transit peroid")]
        public float HCLOSER
        {
            get;
            set;
        }
        [Category("Solver Extension")]
        [Description("HCLOSER in transit peroid")]
        public float Prep_Frac
        {
            get;
            set;
        }
        [Category("Modflow Output Extension")]
        [Description("Enable Modflow Output Extension")]
        public bool EnableMFOutputEx
        {
            get;
            set;
        }
        [Category("Modflow Output Extension")]
        [Description("Control file for the Modflow output extension")]
        public string MFOutputExFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _MFOutputExFile);
            }
            set
            {
                _MFOutputExFile = value;
            }
        }
        [Category("Modflow Output Extension")]
        [Description("Log file for Modflow")]
        public string MF_IOLOG_File
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _MF_IOLOG_File);
            }
            set
            {
                _MF_IOLOG_File = value;
            }
        }
        [Category("Modflow SFR Package Extension")]
        [Description("")]
        public bool EnableSFREx
        {
            get;
            set;
        }
        [Category("SFR WQ Module")]
        [Description("")]
        public bool EnableSFRWQ
        {
            get;
            set;
        }
        [Category("Modflow SFR Package Extension")]
        [Description("")]
        public bool EnableSFROutEx
        {
            get;
            set;
        }
        [Category("Modflow SFR Package Extension")]
        [Description("")]
        public string SFRExFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _SFRExFile);
            }
            set
            {
                _SFRExFile = value;
            }
        }
        [Category("Modflow SFR Package Extension")]
        [Description("")]
        public string  SFRWQFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _SFRWQFile);
            }
            set
            {
                _SFRWQFile = value;
            }
        }
        [Category("Modflow SFR Package Extension")]
        [Description("")]
        public string SFROutExFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _SFROutEXFile);
            }
            set
            {
                _SFROutEXFile = value;
            }
        }
        [Category("Modflow LAK Package Extension")]
        [Description("")]
        public bool EnableLakeEx
        {
            get;
            set;
        }
        [Category("Modflow LAK Package Extension")]
        [Description("")]
        public string LakeExFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _LakeExFile);
            }
            set
            {
                _LakeExFile = value;
            }
        }
        [Category("LAI DA Module Extension")]
        [Description("")]
        public bool EnableLAIDAEx
        {
            get;
            set;
        }
        [Category("LAI DA Module Extension")]
        [Description("")]
        public string LAIDAExFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _LAIDAExFile);
            }
            set
            {
                _LAIDAExFile = value;
            }
        }
        [Category("GWH DA Extension")]
        [Description("")]
        public bool EnableGWHDA
        {
            get;
            set;
        }
        [Category("GWH DA Extension")]
        [Description("")]
        public string GWHDAFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _GWHDA_File);
            }
            set
            {
                _GWHDA_File = value;
            }
        }
        [Category("Agent Based Model Extension")]
        [Description("")]
        public bool EnableABM
        {
            get;
            set;
        }
        [Category("CHD Package Extension")]
        [Description("")]
        public bool EnableCHDEx
        {
            get;
            set;
        }
        [Category("Agent Based Model Extension")]
        [Description("")]
        public string ABM_MODEL_File
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _ABM_MODEL_File);
            }
            set
            {
                _ABM_MODEL_File = value;
            }
        }
        [Category("CHD Package Extension")]
        [Description("")]
        public string CHD_EX_File
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _CHDEX_File);
            }
            set
            {
                _CHDEX_File = value;
            }
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            base.Initialize();
        }

        public override LoadingState Load(DotSpatial.Data.ICancelProgressHandler progess)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                try
                {
                    string newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    StartInJulian = int.Parse(newline.Trim());
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    SolverExFlag = int.Parse(newline.Trim());
                    newline = sr.ReadLine();
                    var buf = TypeConverterEx.Split<int>(newline, 2);
                    Max_TS_ITER = buf[0];
                    HCLOSER = buf[1];
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnableMFOutputEx = TypeConverterEx.String2Bool(newline.Trim());
                    MFOutputExFile = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    buf = TypeConverterEx.Split<int>(newline, 3);
                    EnableSFREx = TypeConverterEx.Int2Bool(buf[0]);
                    EnableSFRWQ = TypeConverterEx.Int2Bool(buf[1]);
                    EnableSFROutEx = TypeConverterEx.Int2Bool(buf[2]);
                    SFRExFile = sr.ReadLine().Trim();
                    SFRWQFile = sr.ReadLine().Trim();
                    SFROutExFile = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnableLakeEx = TypeConverterEx.String2Bool(newline.Trim());
                    LakeExFile = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnableLAIDAEx = TypeConverterEx.String2Bool(newline.Trim());
                    LAIDAExFile = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    MF_IOLOG_File = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnableGWHDA = TypeConverterEx.String2Bool(newline.Trim());
                    GWHDAFile = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnableABM = TypeConverterEx.String2Bool(newline.Trim());
                    ABM_MODEL_File = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnableCHDEx = TypeConverterEx.String2Bool(newline.Trim());
                    CHD_EX_File = sr.ReadLine().Trim();

                    State = ModelObjectState.Ready;
                    result = LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    State = ModelObjectState.Standby;
                    Message = string.Format("Failed to load extension control file: {0}. Error message: {1}", FileName, ex.Message);
                    ShowWarning(Message, progess);
                    result = LoadingState.Warning;
                }
                finally
                {
                    fs.Close();
                    sr.Close();
                }
            }
            else
            {
                Message = "The extension manager file dose not exist: " + FileName;
                ShowWarning(Message, progess);
                result = LoadingState.Warning;
            }
            OnLoaded(progess, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }
        public override void New()
        {
            InitValues();
            base.New();
            State = ModelObjectState.Ready;
        }

        public override void SaveAs(string filename, DotSpatial.Data.ICancelProgressHandler progress)
        {
            if(TypeConverterEx.IsNull(filename))
            {
                progress.Progress("ex_man_pck", 100, "the file for extension man pakage is Null.");
                return;
            }
            StreamWriter sw = new StreamWriter(filename);
            string newline = "# extension modules";
            sw.WriteLine(newline);
            sw.WriteLine("## Julian Start");
            sw.WriteLine(StartInJulian.ToString());
            sw.WriteLine("## SolverEx");
            sw.WriteLine(SolverExFlag);
            if (SolverExFlag < 2)
                newline = string.Format("{0}  {1} #Max_TS_ITER, HCLOSER", Max_TS_ITER, HCLOSER);
            else
                newline = string.Format("{0}  {1} {2} #Max_TS_ITER, HCLOSER, Precp_Frac", Max_TS_ITER, HCLOSER,Prep_Frac);
            sw.WriteLine(newline);
            sw.WriteLine("## MFOutputEx");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableMFOutputEx));
            sw.WriteLine(_MFOutputExFile);
            sw.WriteLine("## SFREx");
            newline = string.Format("{0}  {1}   {2}  #SFREx  SFRWQ SFROutEX", TypeConverterEx.Bool2String(EnableSFREx), TypeConverterEx.Bool2String(EnableSFRWQ), TypeConverterEx.Bool2String(EnableSFROutEx));
            sw.WriteLine(newline);
            sw.WriteLine(_SFRExFile);
            sw.WriteLine(_SFRWQFile);
            sw.WriteLine(_SFROutEXFile);
            sw.WriteLine("## LakeEX");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableLakeEx));
            sw.WriteLine(_LakeExFile);
            sw.WriteLine("## LAI DA");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableLAIDAEx));
            sw.WriteLine(_LAIDAExFile);
            sw.WriteLine("## MF IO LOG FILE");
            sw.WriteLine(_MF_IOLOG_File);
            sw.WriteLine("## GWH DA");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableGWHDA));
            sw.WriteLine(_GWHDA_File);
            sw.WriteLine("## PET DA");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableABM));
            sw.WriteLine(_ABM_MODEL_File);
            sw.WriteLine("## CHD Extension");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableCHDEx));
            sw.WriteLine(_CHDEX_File);
            sw.Close();

            SaveSFROutEx();
            OnSaved(progress);
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            StartInJulian = (int)(TimeService.Start.ToOADate() + 2415018.5);
        }
        private void InitValues()
        {
            StartInJulian = 36526;
            SolverExFlag = 1;
            Max_TS_ITER = 20;
            HCLOSER = 10;
            EnableMFOutputEx = false;
            EnableSFREx = false;
            EnableSFROutEx = true;
            EnableSFRWQ = false;
            EnableLakeEx = false;
            EnableLAIDAEx = false;
            EnableCHDEx = false;
            EnableGWHDA = false;
            Prep_Frac = 0.1f;
            _MFOutputExFile = ".\\Input\\Extension\\mfoutput.ex";
            _SFRExFile = ".\\Input\\Extension\\sfr.ex";
            _SFRWQFile = ".\\Input\\WQ\\sfrwq.ex";
            _SFROutEXFile = ".\\Input\\Extension\\sfrout_oc.ex";
            _LakeExFile = ".\\Input\\Extension\\lake.ex";
            _LAIDAExFile = ".\\Input\\Extension\\lai_da.ex";
            _MF_IOLOG_File = ".\\Output\\mf_io_log.csv";
            _GWHDA_File = ".\\Input\\Extension\\gwh_da.ex";
            _ABM_MODEL_File = ".\\Input\\Extension\\abm.ex";
            _CHDEX_File = ".\\Input\\Extension\\chd.ex";
        }

        private void SaveSFROutEx()
        {
            StreamWriter sw = new StreamWriter(SFROutExFile);
            string line = "5 #num_sfrout_vars\n" +
                                "2 3 4 7 13\n" +
                                "sfr_out_vars(1) = 'flow in'\n" +
                                "sfr_out_vars(2) = 'stream loss'\n" +
                                "sfr_out_vars(3) = 'flow out'\n" +
                                "sfr_out_vars(4) = 'overland flow'\n" +
                                "sfr_out_vars(5) = 'precipitation'\n" +
                                "sfr_out_vars(6) = 'et'\n" +
                                "sfr_out_vars(7) = 'stream head'\n" +
                                "sfr_out_vars(8) = 'stream depth'\n" +
                                "sfr_out_vars(9) = 'stream width'\n" +
                                "sfr_out_vars(10) = 'streambed conductance'\n" +
                                "sfr_out_vars(11) = 'flow to water table'\n" +
                                "sfr_out_vars(12) = 'change in unsat storage'\n" +
                                "sfr_out_vars(13) = 'gw head'";
            sw.WriteLine(line);
            sw.Close();
        }

    }
}
