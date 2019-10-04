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
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public class PRMSDataPackage : DataPackage
    {
        private int _skipped_line = 0;
        public PRMSDataPackage()
        {
            Name = "Driving Data";
            IsMandatory = true;
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            _Initialized = true;
        }
        public override bool New()
        {
            IsDirty = true;
            return true;
        }
        public override bool Scan()
        {
            NumTimeStep = 0;
            var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fileStream, Encoding.Default);
            string line = "";
            _skipped_line = 1;
            line = sr.ReadLine();
            List<string> varnames = new List<string>();
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (!TypeConverterEx.IsNull(line))
                {
                    if (line.StartsWith("#"))
                    {
                        _skipped_line++;
                        break;
                    }
                    if (!line.StartsWith("//"))
                    {
                        var buf = TypeConverterEx.Split<string>(line.Trim());
                        var nvar = int.Parse(buf[1]);
                        for (int i = 0; i < nvar; i++)
                        {
                            varnames.Add(buf[0] + (i + 1));
                        }
                    }
                }
                _skipped_line++;
            }
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (!TypeConverterEx.IsNull(line))
                {
                    NumTimeStep++;
                }
            }
            Variables = varnames.ToArray();
            sr.Close();
            fileStream.Close();

            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep;
            return true;
        }

        public override bool Load(ICancelProgressHandler cancelprogess)
        {
            OnLoading(0);
            Scan();
            int nvar = Variables.Length;
            var mat = new DataCube<float>(nvar, StepsToLoad, 1, false);
            var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fileStream, Encoding.Default);
            string line = "";
            int progress = 0;
            mat.DateTimes = new DateTime[StepsToLoad];
            for (int i = 0; i < _skipped_line; i++)
            {
                line = sr.ReadLine();
            }
            for (int j = 0; j < nvar; j++)
            {
                mat.Allocate(j, StepsToLoad, 1);
            }
            for (int i = 0; i < StepsToLoad; i++)
            {
                line = sr.ReadLine();
                var buf = TypeConverterEx.Split<int>(line, 6);
                mat.DateTimes[i] = new DateTime(buf[0], buf[1], buf[2], buf[3], buf[4], buf[5]);
                var buf1= TypeConverterEx.SkipSplit<float>(line,6);
                for (int j = 0; j < nvar; j++)
                {
                    mat[j,i,0] = buf1[j];
                }
                progress = Convert.ToInt32(i * 100 / StepsToLoad);
                OnLoading(progress);
            }
            OnLoading(100);
            mat.Variables = Variables;
            sr.Close();
            fileStream.Close();
            DataCube = mat;
            OnLoaded(cancelprogess);
            return true;
        }

        public override bool Load(int var_index, ICancelProgressHandler cancelprogess)
        {
            return Load(cancelprogess);
        }

        public override bool Save(ICancelProgressHandler progress)
        {
            if (IsDirty)
            {
                SaveAs(FileName,progress);
                IsDirty = false;
            }
            return true;
        }

        public override bool SaveAs(string filename, ICancelProgressHandler progress)
        {
            StreamWriter sw = new StreamWriter(filename);
            string line = "PRMS data file generated by Visual HEIFLOW. Created on " + DateTime.Now.ToString();
            sw.WriteLine(line);
            line = "tmax 1";
            sw.WriteLine(line);
            sw.WriteLine("#########################################################################");
            foreach (var time in TimeService.Timeline)
            {
                line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t0", time.Year, time.Month, time.Day, time.Hour, 0, 0);
                sw.WriteLine(line);
            }
            sw.Close();
            OnSaved(progress);
            return true;
        }

        public override void Clear()
        {
            if (_Initialized)
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            State = ModelObjectState.Standby;
            _Initialized = false;
        }

     
    }
}