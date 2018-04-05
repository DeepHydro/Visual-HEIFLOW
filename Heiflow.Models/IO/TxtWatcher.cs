// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
    public class TxtWatcher : ArrayWatcher
    {
        private WatchDirectory _directoryToWatch;
        private StreamReader _StreamReader;
        private FileStream _FileStream;
        private ArrayWatchObject<double> _WatchObject;

        public TxtWatcher(WatchDirectory directory)
        {
            _WatchObject = new ArrayWatchObject<double>();
            _directoryToWatch = directory;
            FileName = _directoryToWatch.FilePath;
            State = RunningState.Stopped;      
        }

        public TxtWatcher()
        {
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

        public override void Load(string filename)
        {
            if (this.State == RunningState.Busy)
                return;
            if (File.Exists(filename))
            {
                var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var sr = new StreamReader(fs, Encoding.Default);
                string line = sr.ReadLine().Trim(TypeConverterEx.Sharp);
                int nvar = TypeConverterEx.Split<string>(line).Length - 1;
                 _DataSource =new ListTimeSeries<double>(nvar);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (!TypeConverterEx.IsNull(line))
                    {
                        var strs = TypeConverterEx.Split<string>(line);
                        var buf = TypeConverterEx.SkipSplit<double>(line, 1);
                        var date = ModelService.Start.AddDays(int.Parse(strs[0]));                        
                        _DataSource.Add(date, buf);
                    }
                }
                fs.Close();
                sr.Close();
            }
        }
    }
}
