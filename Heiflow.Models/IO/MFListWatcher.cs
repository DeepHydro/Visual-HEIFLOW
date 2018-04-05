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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public class MFListWatcher : ArrayWatcher
    {
        private WatchDirectory _directoryToWatch;
        private StreamReader _StreamReader;
        private FileStream _FileStream;
        private ArrayWatchObject<double> _WatchObject;
        private MFMonitor _MFMonitor;

        public MFListWatcher(WatchDirectory directory)
        {
            _WatchObject = new ArrayWatchObject<double>();
            _directoryToWatch = directory;
            FileName = _directoryToWatch.FilePath;
            State = RunningState.Stopped;
        }

        public MFListWatcher(MFMonitor monitor)
        {
            _MFMonitor = monitor;
            State = RunningState.Stopped;
            _WatchObject = new ArrayWatchObject<double>();
        }

        public WatchDirectory DirectoryToWatch
        {
            get
            {
                return _directoryToWatch;
            }
        }

        public override void Start()
        {
            _FileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _StreamReader = new StreamReader(_FileStream, Encoding.Default);
            State = RunningState.Busy;
        }

        public override void Pause()
        {

        }

        public override void Continue()
        {

        }

        public override void Stop()
        {
            if (_StreamReader != null)
            {
                _StreamReader.Close();
                _FileStream.Close();
            }
            State = RunningState.Stopped;
        }

        public override void Update()
        {

        }

        public void InitMonitor(string filename)
        {
            if (File.Exists(filename))
            {
                var root = new MonitorItemCollection("Modflow Water Budgets");
                var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var sr = new StreamReader(fs, Encoding.Default);
                string line = "";
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.Contains("VOLUMETRIC BUDGET FOR ENTIRE MODEL AT END OF TIME STEP"))
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                sr.ReadLine();
                            }
                            for (int i = 0; i < 30; i++)
                            {
                                line = sr.ReadLine();
                                if (TypeConverterEx.IsNull(line))
                                {
                                    goto Found;
                                }
                                else
                                {
                                    var buf = TypeConverterEx.Split<string>(line);
                                    string var_nm = buf[0] + " IN";
                                    if (buf.Length == 8)
                                    {
                                        var_nm = buf[0] + " " + buf[1] + " IN";
                                    }
                                    MonitorItem item = new MonitorItem(var_nm)
                                    {
                                        VariableIndex = i,
                                        Group = FileMonitor._In_Group,
                                        SequenceType = SequenceType.StepbyStep
                                    };
                                    root.Children.Add(item);
                                }
                            }
                        }
                    }
                }
            Found:
                fs.Close();
                sr.Close();

                var nvar = root.Children.Count;
                for (int i = nvar; i < nvar * 2; i++)
                {
                    var nn = root.Children[i - nvar].Name.Replace("IN", "OUT");
                    MonitorItem item = new MonitorItem(nn)
                    {
                        VariableIndex = i,
                        Group = FileMonitor._Out_Group,
                        SequenceType = SequenceType.StepbyStep
                    };
                    root.Children.Add(item);
                }
                //"Total SAT IN"
                var total_in = new MonitorItem(FileMonitor.SAT_IN)
                {
                    VariableIndex = nvar * 2,
                    Group = FileMonitor._Total_Group,
                    SequenceType = SequenceType.StepbyStep
                };
               // "Total SAT OUT"
                var total_out = new MonitorItem(FileMonitor.SAT_OUT)
                {
                    VariableIndex = nvar * 2 + 1,
                    Group = FileMonitor._Total_Group,
                    SequenceType = SequenceType.StepbyStep
                };
                //"Total Storage Change"
                var ds = new MonitorItem(FileMonitor.SAT_DS)
                {
                    VariableIndex = nvar * 2 + 2,
                    Group = FileMonitor._Total_Group,
                    SequenceType = SequenceType.StepbyStep
                };
                //"Total Budget Error"
                var error = new MonitorItem(FileMonitor.SAT_ERROR)
                {
                    VariableIndex = nvar * 2 + 3,
                    Group = FileMonitor._Total_Group,
                    SequenceType = SequenceType.StepbyStep
                };
                root.Children.Add(total_in);
                root.Children.Add(total_out);
                root.Children.Add(ds);
                root.Children.Add(error);

                foreach (var item in root.Children)
                {
                    item.SequenceType = SequenceType.StepbyStep;
                    item.Monitor = _MFMonitor;
                }

                _MFMonitor.Root.Clear();
                _MFMonitor.Root.Add(root);
            }

        }

        public override void Load(string filename)
        {
            if (this.State == RunningState.Busy)
                return;

            var back_file = filename + ".csv";
            if (File.Exists(back_file) && File.Exists(filename))
            {
                InitMonitor(filename);
                _DataSource = new ListTimeSeries<double>(_MFMonitor.Root[0].Children.Count);
                var fs = new FileStream(back_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var sr = new StreamReader(fs, Encoding.Default);

                string line = sr.ReadLine();
                int t = 0;
                while(!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var buf = TypeConverterEx.SkipSplit<double>(line, 1);
                        var date = ModelService.Start.AddDays(t);
                        _DataSource.Add(date, buf);
                        t++;
                    }
                }
                fs.Close();
                sr.Close();
            }
            else
            {
                if (File.Exists(filename))
                {
                    InitMonitor(filename);
                    int total_var = _MFMonitor.Root[0].Children.Count;
                    _DataSource = new ListTimeSeries<double>(total_var);

                    var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var sr = new StreamReader(fs, Encoding.Default);
                    string line = "";
                    int t = 0;
                    int nvar = (total_var - 4) / 2;
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line.Contains("VOLUMETRIC BUDGET FOR ENTIRE MODEL AT END OF TIME STEP"))
                            {
                                var vector = new double[total_var];
                                double total_in = 0, total_out = 0, ds = 0, error = 0;
                                for (int i = 0; i < 7; i++)
                                {
                                    sr.ReadLine();
                                }

                                for (int i = 0; i < nvar; i++)
                                {
                                    line = sr.ReadLine();
                                    var buf = TypeConverterEx.Split<string>(line);
                                    if (buf.Length == 6)
                                    {
                                        vector[i] = double.Parse(buf[5]);
                                    }
                                    else if (buf.Length == 8)
                                    {
                                        vector[i] = double.Parse(buf[7]);
                                    }
                                    total_in += vector[i];
                                }
                                for (int i = 0; i < 5; i++)
                                {
                                    sr.ReadLine();
                                }
                                for (int i = nvar; i < nvar * 2; i++)
                                {
                                    line = sr.ReadLine();
                                    var buf = TypeConverterEx.Split<string>(line);
                                    if (buf.Length == 6)
                                    {
                                        vector[i] = double.Parse(buf[5]);
                                    }
                                    else if (buf.Length == 8)
                                    {
                                        vector[i] = double.Parse(buf[7]);
                                    }
                                    total_out += vector[i];
                                }

                                ds = vector[2 * nvar] - vector[0];
                                error = total_in - total_out;

                                vector[2 * nvar] = total_in;
                                vector[2 * nvar + 1] = total_out;
                                vector[2 * nvar + 2] = ds;
                                vector[2 * nvar + 3] = error;
             
                                if (t > 0)
                                {
                                    var date = ModelService.Start.AddDays(t - 1);
                                    _DataSource.Add(date, vector);      
                                }
                                t++;
                            }
                        }
                    }
                    fs.Close();
                    sr.Close();

                    var csv_file = filename + ".csv";
                    StreamWriter sw = new StreamWriter(csv_file);
                    var head = from item in _MFMonitor.Root[0].Children select item.Name;
                    line = "Date" + "," + string.Join(",", head);
                    sw.WriteLine(line);
                    var row = new double[total_var];
                    for (int i = 0; i < _DataSource.Dates.Count; i++)
                    {
                        for (int n = 0; n < total_var; n++)
                        {
                            row[n] = _DataSource.Values[n][i];
                        }
                        line = _DataSource.Dates[i].ToString("yyyy-MM-dd") + "," + string.Join(",", row);
                        sw.WriteLine(line);
                    }
                    sw.Close();
                }
            }
        }
    }
}
