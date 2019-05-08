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

        public ExtensionManPackage()
        {
            this.Name = PackageName;
            TimeService = new TimeService("Base Timeline");
            IsMandatory = true;
            FullName = "Extension Manager";
            InitValues();
        }

        public int StartInJulian
        {
            get;
            set;
        }

        public bool EnableSolverEx
        {
            get;
            set;
        }

        public int Max_TS_ITER
        {
            get;
            set;
        }

        public float HCLOSER
        {
            get;
            set;
        }

        public bool EnableMFOutputEx
        {
            get;
            set;
        }

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

        public bool EnableSFREx
        {
            get;
            set;
        }

        public bool EnableSFRReport
        {
            get;
            set;
        }

        public bool EnableSFROutEx
        {
            get;
            set;
        }

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

        public bool EnableLakeEx
        {
            get;
            set;
        }

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

        public bool EnableAllocCurveEx
        {
            get;
            set;
        }
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
        public bool EnablePET_CONSTRAINT
        {
            get;
            set;
        }

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
        public bool EnableABM
        {
            get;
            set;
        }
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
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            base.Initialize();
        }

        public override bool Load(DotSpatial.Data.ICancelProgressHandler progess)
        {
            if (File.Exists(FileName))
            {
                FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                string newline = sr.ReadLine();
                newline = sr.ReadLine();
                newline = sr.ReadLine();
                StartInJulian = int.Parse(newline.Trim());
                newline = sr.ReadLine();
                newline = sr.ReadLine();
                EnableSolverEx = String2Bool(newline.Trim());
                newline = sr.ReadLine();
                var buf = TypeConverterEx.Split<int>(newline, 2);
                Max_TS_ITER = buf[0];
                HCLOSER = buf[1];
                newline = sr.ReadLine();
                newline = sr.ReadLine();
                EnableMFOutputEx = String2Bool(newline.Trim());
                MFOutputExFile = sr.ReadLine().Trim();
                newline = sr.ReadLine();
                newline = sr.ReadLine();
                buf = TypeConverterEx.Split<int>(newline, 3);
                EnableSFREx = Int2Bool(buf[0]);
                EnableSFRReport = Int2Bool(buf[1]);
                EnableSFROutEx = Int2Bool(buf[2]);
                SFRExFile = sr.ReadLine().Trim();
                SFRReportFile = sr.ReadLine().Trim();
                SFROutExFile = sr.ReadLine().Trim();
                newline = sr.ReadLine();
                newline = sr.ReadLine();
                EnableLakeEx = String2Bool(newline.Trim());
                LakeExFile = sr.ReadLine().Trim();
                newline = sr.ReadLine();
                newline = sr.ReadLine();
                EnableAllocCurveEx = String2Bool(newline.Trim());
                AllocCurveExFile = sr.ReadLine().Trim();
                newline = sr.ReadLine();
                MF_IOLOG_File = sr.ReadLine().Trim();
                newline = sr.ReadLine();
                newline = sr.ReadLine();
                EnablePET_CONSTRAINT = String2Bool(newline.Trim());
                PET_CONSTRAINT_File = sr.ReadLine().Trim();
                newline = sr.ReadLine();
                newline = sr.ReadLine();
                EnableABM = String2Bool(newline.Trim());
                ABM_MODEL_File = sr.ReadLine().Trim();

                fs.Close();
                sr.Close();
                OnLoaded(progess);
                return true;
            }
            else
            {
                Message = "The extension manager file dose not exist: " + FileName;
                OnLoadFailed(Message, progess);
                return false;
            }
        }
        public override bool New()
        {
            InitValues();
            base.New();
            State = ModelObjectState.Ready;
            return true;
        }

        public override bool SaveAs(string filename, DotSpatial.Data.ICancelProgressHandler progress)
        {
            StreamWriter sw = new StreamWriter(FileName);
            string newline = "# extension modules";
            sw.WriteLine(newline);
            sw.WriteLine("## Julian Start");
            sw.WriteLine(StartInJulian.ToString());
            sw.WriteLine("## SolverEx");
            sw.WriteLine(Bool2String(EnableSolverEx));
            newline = string.Format("{0}  {1} #Max_TS_ITER, HCLOSER", Max_TS_ITER, HCLOSER);
            sw.WriteLine(newline);
            sw.WriteLine("## MFOutputEx");
            sw.WriteLine(Bool2String(EnableMFOutputEx));
            sw.WriteLine(_MFOutputExFile);
            sw.WriteLine("## SFREx");
            newline = string.Format("{0}  {1}   {2}  #SFREx  SFRReport SFROutEX", Bool2String(EnableSFREx), Bool2String(EnableSFRReport), Bool2String(EnableSFROutEx));
            sw.WriteLine(newline);
            sw.WriteLine(_SFRExFile);
            sw.WriteLine(_SFRReportFile);
            sw.WriteLine(_SFROutEXFile);
            sw.WriteLine("## LakeEX");
            sw.WriteLine(Bool2String(EnableLakeEx));
            sw.WriteLine(_LakeExFile);
            sw.WriteLine("## AllocationCurveEX");
            sw.WriteLine(Bool2String(EnableAllocCurveEx));
            sw.WriteLine(_AllocCurveExFile);
            sw.WriteLine("## MF IO LOG FILE");
            sw.WriteLine(_MF_IOLOG_File);
            sw.WriteLine("## PET Constraint");
            sw.WriteLine(Bool2String(EnablePET_CONSTRAINT));
            sw.WriteLine(_PET_CONSTRAINT_File);
            sw.WriteLine("## ABM Model");
            sw.WriteLine(Bool2String(EnableABM));
            sw.WriteLine(ABM_MODEL_File);

            sw.Close();
            OnSaved(progress);
            return true;
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            StartInJulian = (int)(TimeService.Start.ToOADate() + 2415018.5);
        }
        private void InitValues()
        {
            StartInJulian = 36526;
            EnableSolverEx = true;
            Max_TS_ITER = 20;
            HCLOSER = 10;
            EnableMFOutputEx = false;
            EnableSFREx = false;
            EnableSFROutEx = false;
            EnableSFRReport = false;
            EnableLakeEx = false;
            EnableAllocCurveEx = false;
            _MFOutputExFile = ".\\Input\\Extension\\mfoutput.ex";
            _SFRExFile = ".\\Input\\Extension\\sfr.ex";
            _SFRReportFile = ".\\Output\\sfr_report.txt";
            _SFROutEXFile = ".\\Input\\Extension\\sfrout_oc.ex";
            _LakeExFile = ".\\Input\\Extension\\lake.ex";
            _AllocCurveExFile = ".\\Input\\Extension\\allocation_curve.ex";
            _MF_IOLOG_File = ".\\Output\\mf_io_log.csv";
            _PET_CONSTRAINT_File = ".\\Input\\Extension\\pet_constraint.ex";
            _ABM_MODEL_File = ".\\Input\\Extension\\abm.ex";
        }
        private bool String2Bool(string str)
        {
            var buf = int.Parse(str);
            return buf > 0;
        }
        private bool Int2Bool(int vv)
        {
            return vv > 0;
        }
        private string Bool2String(bool vv)
        {
            if (vv)
                return "1";
            else
                return "0";
        }
    }
}
