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
         private string   _SFRReportFile;
         private string _SFROutEXFile;
         private string _LakeExFile;
         private string _AllocCurveExFile;
         private string _MF_IOLOG_File;
         private string _PET_CONSTRAINT_File;
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
        [Category("Modflow SFR Package Extension")]
        [Description("")]
        public bool EnableSFRReport
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
        public string  SFRReportFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _SFRReportFile);
            }
            set
            {
                _SFRReportFile = value;
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
        [Category("Allocation Curve Module Extension")]
        [Description("")]
        public bool EnableAllocCurveEx
        {
            get;
            set;
        }
        [Category("Allocation Curve Module Extension")]
        [Description("")]
        public string AllocCurveExFile
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _AllocCurveExFile);
            }
            set
            {
                _AllocCurveExFile = value;
            }
        }
        [Category("PET Extension")]
        [Description("")]
        public bool EnablePET_CONSTRAINT
        {
            get;
            set;
        }
        [Category("PET Extension")]
        [Description("")]
        public string PET_CONSTRAINT_File
        {
            get
            {
                return Path.Combine(Owner.WorkDirectory, _PET_CONSTRAINT_File);
            }
            set
            {
                _PET_CONSTRAINT_File = value;
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
                    EnableSFRReport = TypeConverterEx.Int2Bool(buf[1]);
                    EnableSFROutEx = TypeConverterEx.Int2Bool(buf[2]);
                    SFRExFile = sr.ReadLine().Trim();
                    SFRReportFile = sr.ReadLine().Trim();
                    SFROutExFile = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnableLakeEx = TypeConverterEx.String2Bool(newline.Trim());
                    LakeExFile = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnableAllocCurveEx = TypeConverterEx.String2Bool(newline.Trim());
                    AllocCurveExFile = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    MF_IOLOG_File = sr.ReadLine().Trim();
                    newline = sr.ReadLine();
                    newline = sr.ReadLine();
                    EnablePET_CONSTRAINT = TypeConverterEx.String2Bool(newline.Trim());
                    PET_CONSTRAINT_File = sr.ReadLine().Trim();
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
            newline = string.Format("{0}  {1}   {2}  #SFREx  SFRReport SFROutEX", TypeConverterEx.Bool2String(EnableSFREx), TypeConverterEx.Bool2String(EnableSFRReport), TypeConverterEx.Bool2String(EnableSFROutEx));
            sw.WriteLine(newline);
            sw.WriteLine(_SFRExFile);
            sw.WriteLine(_SFRReportFile);
            sw.WriteLine(_SFROutEXFile);
            sw.WriteLine("## LakeEX");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableLakeEx));
            sw.WriteLine(_LakeExFile);
            sw.WriteLine("## AllocationCurveEX");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableAllocCurveEx));
            sw.WriteLine(_AllocCurveExFile);
            sw.WriteLine("## MF IO LOG FILE");
            sw.WriteLine(_MF_IOLOG_File);
            sw.WriteLine("## PET Constraint");
            sw.WriteLine(TypeConverterEx.Bool2String(EnablePET_CONSTRAINT));
            sw.WriteLine(_PET_CONSTRAINT_File);
            sw.WriteLine("## ABM Model");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableABM));
            sw.WriteLine(_ABM_MODEL_File);
            sw.WriteLine("## CHD Extension");
            sw.WriteLine(TypeConverterEx.Bool2String(EnableCHDEx));
            sw.WriteLine(_CHDEX_File);
            sw.Close();
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
            EnableSFROutEx = false;
            EnableSFRReport = false;
            EnableLakeEx = false;
            EnableAllocCurveEx = false;
            EnableCHDEx = false;
            Prep_Frac = 0.1f;
            _MFOutputExFile = ".\\Input\\Extension\\mfoutput.ex";
            _SFRExFile = ".\\Input\\Extension\\sfr.ex";
            _SFRReportFile = ".\\Output\\sfr_report.txt";
            _SFROutEXFile = ".\\Input\\Extension\\sfrout_oc.ex";
            _LakeExFile = ".\\Input\\Extension\\lake.ex";
            _AllocCurveExFile = ".\\Input\\Extension\\allocation_curve.ex";
            _MF_IOLOG_File = ".\\Output\\mf_io_log.csv";
            _PET_CONSTRAINT_File = ".\\Input\\Extension\\pet_constraint.ex";
            _ABM_MODEL_File = ".\\Input\\Extension\\abm.ex";
            _CHDEX_File = ".\\Input\\Extension\\chd.ex";
        }
    }
}
