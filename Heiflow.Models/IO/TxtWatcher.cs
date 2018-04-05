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
