// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
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
    public class CSVWatcher : ArrayWatcher
    {
        private StreamReader _StreamReader;
        private FileStream _FileStream;
        private ArrayWatchObject<double> _WatchObject;

        public CSVWatcher()
        {
            State = RunningState.Stopped;
            _WatchObject = new ArrayWatchObject<double>();
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
            if (State == RunningState.Stopped)
                return;
            var line = _StreamReader.ReadLine();
            if (TypeConverterEx.IsNull(line))
                return;
            if (line.Contains("Date"))
            {
                var ss = TypeConverterEx.Split<string>(line, TypeConverterEx.Comma);
                _DataSource = new ListTimeSeries<double>(ss.Length - 1);
                return;
            }
            var strs = TypeConverterEx.SkipSplit<string>(line);
            var buf = TypeConverterEx.SkipSplit<double>(line, 1);
            _WatchObject.Current = DateTime.Parse(strs[0]);
            _WatchObject.Values = buf;
            _DataSource.Add(_WatchObject.Current, buf);
            OnUpdated(this, _WatchObject);
        }

        public override void Load(string filename)
        {
            if (this.State == RunningState.Busy)
                return;
            if (File.Exists(filename))
            {
                var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var sr = new StreamReader(fs, Encoding.Default);
                string line = sr.ReadLine();
                int nvar = TypeConverterEx.Split<string>(line, TypeConverterEx.Comma).Length - 1;
                 _DataSource =new ListTimeSeries<double>(nvar);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (!TypeConverterEx.IsNull(line))
                    {
                        var strs = TypeConverterEx.Split<string>(line);
                        var buf = TypeConverterEx.SkipSplit<double>(line, 1);
                        _DataSource.Add(DateTime.Parse(strs[0]), buf);
                    }
                }
                fs.Close();
                sr.Close();
            }
        }
    }
}
