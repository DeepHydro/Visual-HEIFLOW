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
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Heiflow.Models.Subsurface
{
    public enum OCCommand { HEAD_PRINT_FORMAT, HEAD_SAVE_FORMAT }

    [Export(typeof(IMFPackage))]
    [PackageItem]
    public class OCPackage : MFPackage
    {
        public static string PackageName = "OC";

        public OCPackage()
        {
            IsBanary = true;
            IsSaveBudget = true;
            IsSaveDrawdwon = false;
            IsSaveHead = true;
            IsPrintBudget = true;
            IsMandatory = true;
            Inteval = 1;

            HEAD_PRINT_FORMAT = 10;
            HEAD_SAVE_FORMAT = "10(1X1PE13.5)";
            HEAD_SAVE_UNIT = 37;
            DRAWDOWN_PRINT_FORMAT = 10;
            DRAWDOWN_SAVE_FORMAT = "10(1X1PE13.5)";
            DRAWDOWN_SAVE_UNIT = 38;
            IBOUND_SAVE_FORMAT = "10(1X1PE3.1)";
            IBOUND_SAVE_UNIT = 39;
            COMPACT_BUDGET = "false";

            Name = PackageName;
            _FullName = "Output Control Option";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".oc";
            _PackageInfo.ModuleName = PackageName;
            Version = "OC1";
           
        }

        private string[] cmds = new string[] 
        { 
            "PRINT HEAD",
            "PRINT DRAWDOWN",
            "PRINT BUDGET",
            "SAVE HEAD",
            "SAVE DRAWDOWN", 
            "SAVE IBOUND",
            "SAVE BUDGET", 
        };

        private string[] _Records = new string[] 
        {
            "HEAD PRINT FORMAT",
            "HEAD SAVE FORMAT",
            "HEAD SAVE UNIT",
            "DRAWDOWN PRINT FORMAT",
            "DRAWDOWN SAVE FORMAT",
            "DRAWDOWN SAVE UNIT",
            "IBOUND SAVE FORMAT",
            "IBOUND SAVE UNIT",
            "COMPACT BUDGET"
        };


        [Category("File Option")]
        public bool IsBanary { get; set; }
        [Category("Time")]
        public int Inteval { get; set; }

        [Category("Head")]
        public bool IsSaveHead { get; set; }
        [Category("Head")]
        /// <summary>
        /// The format in which heads will be printed. Value range: [0,20]
        /// </summary>
        public int HEAD_PRINT_FORMAT
        {
            get;
            set;
        }
        [Category("Head")]
        /// <summary>
        /// a character value that specifies the format for saving heads, and can only be specified if the word method of output control is used. 
        /// The format must contain 20 characters or less and must be a valid Fortran format that is enclosed in parentheses. 
        /// </summary>
        public string HEAD_SAVE_FORMAT
        {
            get;
            set;
        }
        [Category("Head")]
        /// <summary>
        /// the unit number on which heads will be saved
        /// </summary>
        public int HEAD_SAVE_UNIT
        {
            get;
            set;
        }
        [Category("Drawdwon")]
        public bool IsSaveDrawdwon { get; set; }

        [Category("Drawdown")]
        /// <summary>
        /// The format in which drawdown will be printed. Value range: [0,20]
        /// </summary>
        public int DRAWDOWN_PRINT_FORMAT
        {
            get;
            set;
        }
        [Category("Drawdown")]
        /// <summary>
        ///  a character value that specifies the format for saving drawdown,
        /// </summary>
        public string DRAWDOWN_SAVE_FORMAT
        {
            get;
            set;
        }
        [Category("Drawdown")]
        /// <summary>
        /// the unit number on which drawdowns will be saved.
        /// </summary>
        public int DRAWDOWN_SAVE_UNIT
        {
            get;
            set;
        }
        [Category("IBOUND")]
        /// <summary>
        /// a character value that specifies the format for saving IBOUND
        /// </summary>
        public string IBOUND_SAVE_FORMAT
        {
            get;
            set;
        }
        [Category("IBOUND")]
        /// <summary>
        /// the unit number on which the IBOUND array will be saved.
        /// </summary>
        public int IBOUND_SAVE_UNIT
        {
            get;
            set;
        }
        [Category("Budget")]
        public string COMPACT_BUDGET
        {
            get;
            set;
        }

        [Category("Budget")]
        public bool IsSaveBudget
        {
            get;
            set;
        }
        [Category("Budget")]
        public bool IsPrintBudget
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
             
        }
        public override void New()
        {
            IsSaveHead = true;
            IsSaveDrawdwon = false;
            IsSaveBudget = true;

            HEAD_SAVE_UNIT = ModflowInstance.NameManager.NextFID();
            var oc_info = new PackageInfo()
            {
                FID = HEAD_SAVE_UNIT,
                FileExtension = ".fhd",
                FileName = string.Format("{0}{1}{2}", Modflow.OutputDic, ModflowInstance.Project.Name, ".fhd"),
                Format = FileFormat.Binary,
                IOState = IOState.REPLACE,
                ModuleName = "DATA",
                WorkDirectory = ModflowInstance.WorkDirectory
            };
            oc_info.Name = Path.GetFileName(oc_info.FileName);
            ModflowInstance.NameManager.Add(oc_info);
            base.New();
            State = ModelObjectState.Ready;
        }
        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            var result = LoadingState.Normal;
            var mf = Owner as Modflow;
            var sp_start = Owner.TimeService.Start;
            var _Dic_SP = Owner.TimeService.StressPeriods;

            int nrec = LoadRecords();
            StreamReader sr = new StreamReader(FileName);
            StepOption stepoption = null;
            string line = "";
            for (int i = 0; i < nrec; i++)
            {
                line = sr.ReadLine();
            }
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine().Trim().ToUpper();

                if (line.Contains("PERIOD"))
                {
                    var tt = ParseSP(line);
                    if (Owner.TimeService.ContainsSP(tt.Item1))
                    {
                        var sp = _Dic_SP[tt.Item1 - 1];
                        stepoption = new StepOption(tt.Item1, tt.Item2);
                        sp.StepOptions.Add(stepoption);
                    }
                }
                else
                {
                    var cmd = ParseCommand(line);
                    if (cmd == "SAVE HEAD")
                        stepoption.SaveHead = true;
                    else if (cmd == "SAVE DRAWDOWN")
                        stepoption.SaveDarwdown = true;
                    else if (cmd == "SAVE IBOUND")
                        stepoption.SaveIbound = true;
                    else if (cmd == "SAVE BUDGET")
                        stepoption.SaveBudget = true;
                    else if (cmd == "PRINT HEAD")
                        stepoption.PrintHead = true;
                    else if (cmd == "PRINT DRAWDOWN")
                        stepoption.PrintDarwdown = true;
                    else if (cmd == "PRINT BUDGET")
                        stepoption.PrintBudget = true;
                }
            }
            int index = _Dic_SP.Count - 1;
            if (_Dic_SP[index].StepOptions.Count > 1)
                Inteval = _Dic_SP[index].StepOptions[1].Step - _Dic_SP[index].StepOptions[0].Step;
            else
                Inteval = 1;
            sr.Close();
            Owner.TimeService.UpdateStressPeriodTimeLine();
            OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }
        public override void SaveAs(string filename, ICancelProgressHandler prg)
        {
            var mf = Owner as Modflow;
            var sp = mf.TimeService.StressPeriods;
            StreamWriter sw = new StreamWriter(filename);
            string line = string.Format("# Output Control file created on {0} by Visual HEIFLOW", DateTime.Now);
            sw.WriteLine(line);
            if (IsBanary)
            {
                line = "HEAD SAVE FORMAT  \n HEAD SAVE UNIT  " + HEAD_SAVE_UNIT;
                sw.WriteLine(line);
                if (IsSaveDrawdwon)
                {
                    line = "DRAWDOWN SAVE FORMAT \n DRAWDOWN SAVE UNIT " + DRAWDOWN_SAVE_UNIT;
                    sw.WriteLine(line);
                }
            }
            else
            {
                line = string.Format("HEAD SAVE FORMAT ({0}) LABEL \n HEAD SAVE UNIT {1}", HEAD_SAVE_FORMAT, HEAD_SAVE_UNIT);
                sw.WriteLine(line);
                if (IsSaveDrawdwon)
                {
                    line = string.Format("DRAWDOWN SAVE FORMAT ({0}) LABEL \n DRAWDOWN SAVE UNIT {1}", DRAWDOWN_SAVE_FORMAT, DRAWDOWN_SAVE_UNIT);
                    sw.WriteLine(line);
                }
            }
          
            for (int n = 0; n < sp.Count; n++)
            {
                int step = 1;
                foreach(var op in sp[n].StepOptions)
                {
                    if (op.Step >= step)
                    {
                        line = string.Format("PERIOD {0} STEP {1}  ", n + 1, op.Step);
                        sw.WriteLine(line);
                        WriteCmds(sw);
                        step += Inteval;
                    }
                }
            }
            sw.Close();
            OnSaved(prg);
        }
        public override void Clear()
        {
            base.Clear();
        }

        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var mf = Owner as Modflow;
            if (this.HEAD_SAVE_UNIT > 0)
            {
                var fhd_info = (from pck in mf.NameManager.MasterList where pck.FID == this.HEAD_SAVE_UNIT select pck).First();
                fhd_info.Name = FHDPackage.PackageName;

                var FHD = new FHDPackage()
                  {
                      Owner = mf,
                      Parent = this,
                      PackageInfo = fhd_info,
                      FileName = fhd_info.FileName
                  };
                mfout.AddChild(FHD);
            }
        }
        private int LoadRecords()
        {
            StreamReader sr = new StreamReader(FileName);
            string line = "";
            int recods_line = 0;
            int comment_line = 0;

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine().Trim().ToUpper();
                if (line.StartsWith("#"))
                {
                    comment_line++;
                    continue;
                }
                else
                {
                    if (line.Contains("PERIOD"))
                    {
                        break;
                    }
                    else
                    {
                        recods_line++;
                    }
                }
            }
            sr.Close();

            sr = new StreamReader(FileName);
            for (int i = 0; i < comment_line; i++)
                sr.ReadLine();

            for (int i = 0; i < recods_line; i++)
            {
                line = sr.ReadLine().Trim().ToUpper();
                ParseRecord(line);
            }

            sr.Close();
            return comment_line + recods_line;
        }
        private void ParseRecord(string line)
        {
            var strs = TypeConverterEx.Split<string>(line);
            if (strs.Length >= 3)
            {
                strs = TypeConverterEx.Split<string>(line, 3);
            }
            else
            {
                strs = TypeConverterEx.Split<string>(line, 2);
            }
            var cmd = string.Join(" ", strs);
            if (_Records.Contains(cmd))
            {
                switch (cmd)
                {
                    case "HEAD PRINT FORMAT":
                        HEAD_PRINT_FORMAT = ParseUnit(line);
                        break;
                    case "HEAD SAVE FORMAT":
                        HEAD_SAVE_FORMAT = ParseFomat(line);
                        break;
                    case "HEAD SAVE UNIT":
                        HEAD_SAVE_UNIT = ParseUnit(line);
                        break;
                    case "DRAWDOWN PRINT FORMAT UNIT":
                        DRAWDOWN_PRINT_FORMAT = ParseUnit(line);
                        break;
                    case "DRAWDOWN SAVE FORMAT":
                        DRAWDOWN_SAVE_FORMAT = ParseFomat(line);
                        break;
                    case "DRAWDOWN SAVE UNIT":
                        DRAWDOWN_SAVE_UNIT = ParseUnit(line);
                        break;
                    case "IBOUND SAVE FORMAT":
                        IBOUND_SAVE_FORMAT = ParseFomat(line);
                        break;
                    case "IBOUND SAVE UNIT":
                        IBOUND_SAVE_UNIT = ParseUnit(line);
                        break;
                    case "COMPACT BUDGET":
                        COMPACT_BUDGET = "true";
                        break;
                }
            }
        }
        private int ParseUnit(string line)
        {
            int unit = 0;
            var strs = TypeConverterEx.Split<string>(line);
            if (strs.Length == 4)
            {
                unit = int.Parse(strs[3]);
            }
            else
            {
                unit = 0;
            }
            return unit;
        }
        private string ParseFomat(string line)
        {
            string format = "";
            var strs = TypeConverterEx.Split<string>(line);
            if (strs.Length == 4)
            {
                format = strs[3];
            }
            else
            {
                format = "";
            }
            return format;
        }
        private Tuple<int, int> ParseSP(string line)
        {
            var strs = TypeConverterEx.Split<string>(line);
            Tuple<int, int> sp = new Tuple<int, int>(int.Parse(strs[1]), int.Parse(strs[3]));
            return sp;
        }
        private string ParseCommand(string line)
        {
            var strs = TypeConverterEx.Split<string>(line);
            var cmd = string.Join(" ", strs);
            return cmd;
        }
        private void WriteCmds(StreamWriter sw)
        {
            if (IsSaveHead)
                sw.WriteLine("\t    SAVE HEAD");
            if (IsSaveDrawdwon)
                sw.WriteLine("\t    SAVE DRAWDOWN");
            if (IsSaveBudget)
                sw.WriteLine("\t    SAVE BUDGET");
            if (IsPrintBudget)
                sw.WriteLine("\t    PRINT BUDGET");
        }
    }
}
