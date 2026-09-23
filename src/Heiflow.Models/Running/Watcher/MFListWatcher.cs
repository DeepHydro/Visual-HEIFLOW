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

using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
{
    public class MFListWatcher : ArrayWatcher
    {
        private WatchDirectory _directoryToWatch;
        private StreamReader _StreamReader;
        private FileStream _FileStream;
        private ArrayWatchObject<double> _WatchObject;
        private MFMonitor _MFMonitor;
        private string _cache_file;
        private bool _has_lakpck = false;
        private int _num_lakes = 0;
        private const string _timestep_key = "VOLUMETRIC BUDGET FOR ENTIRE MODEL AT END OF TIME STEP";
        private const string _lak_step_key = "ALL FLUID FLUXES ARE VOLUMES ADDED TO THE LAKE DURING PRESENT TIME STEP";
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

        private void InitLakeMonitor(int nvar)
        {
            var lak = new MonitorItemCollection("Lake Water Budgets");

            MonitorItem item = new MonitorItem(FileMonitor.LAK_PPT)
              {
                  VariableIndex = nvar + 3,
                  Group = FileMonitor._In_Group,
                  SequenceType = SequenceType.StepbyStep
              };
            lak.Children.Add(item);

            item = new MonitorItem(FileMonitor.LAK_Runoff)
            {
                VariableIndex = nvar + 7,
                Group = FileMonitor._In_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(item);

            item = new MonitorItem(FileMonitor.LAK_Gaining)
            {
                VariableIndex = nvar + 8,
                Group = FileMonitor._In_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(item);

            item = new MonitorItem(FileMonitor.LAK_INFLOW)
            {
                VariableIndex = nvar + 10,
                Group = FileMonitor._In_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(item);

            item = new MonitorItem(FileMonitor.LAKET)
            {
                VariableIndex = nvar + 4,
                Group = FileMonitor._Out_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(item);

            item = new MonitorItem(FileMonitor.LAK_Losing)
            {
                VariableIndex = nvar + 9,
                Group = FileMonitor._Out_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(item);

            item = new MonitorItem(FileMonitor.LAK_Outflow)
            {
                VariableIndex = nvar + 11,
                Group = FileMonitor._Out_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(item);

            item = new MonitorItem(FileMonitor.LAK_Uzf_Infil)
            {
                VariableIndex = nvar + 13,
                Group = FileMonitor._Out_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(item);

            item = new MonitorItem(FileMonitor.LAK_Water_Use)
            {
                VariableIndex = nvar + 12,
                Group = FileMonitor._Out_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(item);

            MonitorItem lak_stor = new MonitorItem(FileMonitor.LAK_Storage)
            {
                VariableIndex = nvar + 1,
                Group = FileMonitor._Storage_Group,
                SequenceType = SequenceType.StepbyStep,
            };
            lak.Children.Add(lak_stor);

            MonitorItem lak_in = new MonitorItem(FileMonitor.LAK_In)
            {
                VariableIndex = -1,
                Group = FileMonitor._Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { nvar + 3, nvar + 7, nvar + 8, nvar + 10 }
            };
            lak.Children.Add(lak_in);

            MonitorItem lak_out = new MonitorItem(FileMonitor.LAK_Out)
            {
                VariableIndex = -1,
                Group = FileMonitor._Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { nvar + 4, nvar + 9, nvar + 11, nvar + 12, nvar + 13 }
            };
            lak.Children.Add(lak_out);

            MonitorItem lak_ds = new MonitorItem(FileMonitor.LAK_Storage_Change)
            {
                VariableIndex = nvar + 2,
                Group = FileMonitor._Total_Group,
                SequenceType = SequenceType.StepbyStep,
            };
            lak.Children.Add(lak_ds);
             

            AggregatedMonitorItem lak_error = new AggregatedMonitorItem(FileMonitor.LAK_Error)
            {
                VariableIndex = -1,
                Group = FileMonitor._Total_Group,
                Derivable = true
            };
            lak_error.Source.AddRange(new MonitorItem[] { lak_in, lak_out, lak_ds });
            lak_error.SourceSign.AddRange(new int[] { -1, 1, 1 });
            lak.Children.Add(lak_error);

            MonitorItem lake_percdisp = new MonitorItem("Laek Error Percent")
            {
                VariableIndex = nvar + 13,
                Group = FileMonitor._Total_Group,
                SequenceType = SequenceType.StepbyStep
            };
            lak.Children.Add(lake_percdisp);

            foreach (var cl in lak.Children)
            {
                cl.Monitor = _MFMonitor;
                cl.SequenceType = SequenceType.StepbyStep;
            }

            _MFMonitor.Root.Add(lak);
        }

        public void InitMonitor(string filename)
        {
            if (File.Exists(filename))
            {
                var root = new MonitorItemCollection("Saturated Zone Water Budgets");
                var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var sr = new StreamReader(fs, Encoding.Default);
                string line = "";
                // Find steady state budgets and skip it
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if(line.Contains("LAK7"))
                    {
                        _has_lakpck = true;
                    }
                    if (line.Contains("MAXIMUM NUMBER OF LAKES"))
                    {
                        var matches = Regex.Matches(line, @"\d+");
                        foreach (Match match in matches)
                        {
                            _num_lakes = int.Parse(match.Value);
                        }
                    }
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.Contains(_timestep_key))
                        {
                            break;
                        }
                    }
                }
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.Contains(_timestep_key))
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
                                    else if (buf.Length ==10)
                                    {
                                        var_nm = buf[0] + " " + buf[1] + " " + buf[2] + " IN";
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
                var ds_cum = new MonitorItem(FileMonitor.SAT_DS_CUM)
                {
                    VariableIndex = nvar * 2 + 4,
                    Group = FileMonitor._Storage_Group,
                    SequenceType = SequenceType.Accumulative
                };
                var disp_cum = new MonitorItem(FileMonitor.SAT_PERD_CUM)
                {
                    VariableIndex = nvar * 2 + 5,
                    Group = FileMonitor._Total_Group,
                    SequenceType = SequenceType.Accumulative
                };
                var disp_step = new MonitorItem(FileMonitor.SAT_PERD_Step)
                {
                    VariableIndex = nvar * 2 + 6,
                    Group = FileMonitor._Total_Group,
                    SequenceType = SequenceType.StepbyStep
                };
               
                root.Children.Add(ds_cum);

                root.Children.Add(total_in);
                root.Children.Add(total_out);
                root.Children.Add(ds);
                root.Children.Add(error);
                root.Children.Add(disp_step);
                root.Children.Add(disp_cum);

                foreach (var item in root.Children)
                {
                  //  item.SequenceType = SequenceType.StepbyStep;
                    item.Monitor = _MFMonitor;
                }

                _MFMonitor.Root.Clear();
                _MFMonitor.Root.Add(root);

                if(_has_lakpck)
                {
                    InitLakeMonitor(root.Children.Count);
                }
            }
        }

        public override void Load(string filename, bool convertToStrepRate, bool[] var_index_isconvert)
        {
            if (this.State == RunningState.Busy)
                return;

            _cache_file = filename + ".csv";
            var percent_token = @"PERCENT DISCREPANCY\s*=\s*(-?\d+\.?\d*)";
            var num_lakvar = 19;
            //if (File.Exists(_cache_file) && File.Exists(filename))
            //{
            //    InitMonitor(filename);
            //    _DataSource = new ListTimeSeries<double>(_MFMonitor.Root[0].Children.Count);
            //    var fs = new FileStream(_cache_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            //    var sr = new StreamReader(fs, Encoding.Default);

            //    string line = sr.ReadLine();
            //    int t = 0;
            //    while(!sr.EndOfStream)
            //    {
            //        line = sr.ReadLine();
            //        if (!string.IsNullOrEmpty(line))
            //        {
            //            var buf = TypeConverterEx.SkipSplit<double>(line, 1);
            //            var date = ModelService.Start.AddDays(t);
            //            _DataSource.Add(date, buf);
            //            t++;
            //        }
            //    }
            //    fs.Close();
            //    sr.Close();
            //}
            //else
            //{
                if (File.Exists(filename))
                {
                    InitMonitor(filename);
                    var nvar = (from it in _MFMonitor.Root[0].Children where it.Group == FileMonitor._In_Group select it).Count();
                    int total_var = _MFMonitor.Root[0].Children.Count;
                    if (_has_lakpck)
                        _DataSource = new ListTimeSeries<double>(total_var + num_lakvar);
                    else
                        _DataSource = new ListTimeSeries<double>(total_var);

                    var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var sr = new StreamReader(fs, Encoding.Default);
                    string line = "";
                    int t = 0;
                    var lak_vec = new double[num_lakvar];

                    var ds_cum = 0.0;
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line.Contains(_timestep_key))
                            {
                                break;
                            }
                        }
                    }
                    while (!sr.EndOfStream)
                    {
                      
                        line = sr.ReadLine();
                        //if (!string.IsNullOrEmpty(line))
                        //{
                        if(line.Contains(_lak_step_key))
                        {
                            ParseLakeBudget(sr, _num_lakes, ref lak_vec);
                            line = sr.ReadLine();
                        }
                        if (line.Contains(_timestep_key))
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
                                line = line.Replace("=", " ");
                                var buf = TypeConverterEx.Split<string>(line);
                                if (buf.Length == 4)
                                {
                                    vector[i] = double.Parse(buf[3]);
                                }
                                else if (buf.Length == 6)
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
                                line = line.Replace("=", " ");
                                var buf = TypeConverterEx.Split<string>(line);
                                if (buf.Length == 4)
                                {
                                    vector[i] = double.Parse(buf[3]);
                                }
                                else if (buf.Length == 6)
                                {
                                    vector[i] = double.Parse(buf[5]);
                                }
                                else if (buf.Length == 8)
                                {
                                    vector[i] = double.Parse(buf[7]);
                                }
                                total_out += vector[i];
                            }

                            ds = vector[nvar] - vector[0];
                            error = total_in - total_out;

                            vector[2 * nvar] = total_in;
                            vector[2 * nvar + 1] = total_out;
                            vector[2 * nvar + 2] = ds;
                            vector[2 * nvar + 3] = error;
                            ds_cum += ds;
                            vector[2 * nvar + 4] += ds_cum;

                            for (int i = 0; i < 5; i++)
                            {
                                sr.ReadLine();
                            }
                            line = sr.ReadLine();
                            var matches = Regex.Matches(line, percent_token);
                            var ii = 0;
                            foreach (Match match in matches)
                            {
                                vector[2 * nvar + 5 + ii] = double.Parse(match.Groups[1].Value);
                                ii++;
                            }

                            var date = ModelService.Start.AddDays(t);

                            if (_has_lakpck)
                            {
                                var new_vec = vector.Concat(lak_vec).ToArray();
                                _DataSource.Add(date, new_vec);
                            }
                            else
                            {
                                _DataSource.Add(date, vector);
                            }
                            t++;
                        }
                        //}
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
            //}
        }

        public void ParseLakeBudget(StreamReader sr, int lakeCount, ref double[] vec)
        {
            var line = sr.ReadLine();
            line = sr.ReadLine();

            for (int i = 0; i < vec.Length; i++)
            {
                vec[i] = 0;
            }

            for (int i = 0; i < lakeCount; i++)
            {
                line = sr.ReadLine();
                var tokens = Regex.Split(line.Trim(), @"\s+");
                for (int j = 0; j < 8; j++)
                {
                    vec[j] += double.Parse(tokens[j + 1]);
                }
            }

            line = sr.ReadLine();
            line = sr.ReadLine();
            line = sr.ReadLine();
            for (int i = 0; i < lakeCount; i++)
            {
                line = sr.ReadLine();
                var tokens = Regex.Split(line.Trim(), @"\s+");
                for (int j = 0; j < 6; j++)
                {
                    vec[8 + j] += double.Parse(tokens[j + 1]);
                }
            }

            line = sr.ReadLine();
            line = sr.ReadLine();
            line = sr.ReadLine();
            for (int i = 0; i < lakeCount; i++)
            {
                line = sr.ReadLine();
                var tokens = Regex.Split(line.Trim(), @"\s+");
                for (int j = 0; j < 5; j++)
                {
                    vec[14 + j] += double.Parse(tokens[j + 1]);
                }
            }
        }

        public override void Clear()
        {
            if(TypeConverterEx.IsNotNull(_cache_file) && File.Exists(_cache_file))
            {
                File.Delete(_cache_file);
            }
            base.Clear();
        }
    }
}
