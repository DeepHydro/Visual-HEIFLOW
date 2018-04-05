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
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public abstract  class ArrayWatcher:WatcherBase, IArrayWatcher
    {
        public event EventHandler<ArrayWatchObject<double>> Updated;
        protected ListTimeSeries<double> _DataSource;
        protected string _FileName;

        public ArrayWatcher()
        {

        }
        public string FileName
        {
            get
            {
                if (TypeConverterEx.IsNull(_FileName))
                    return _FileName;
                else
                    return Path.Combine(ModelService.WorkDirectory, _FileName);
            }
            set
            {
                _FileName = value;
            }
        }
        public ListTimeSeries<double> DataSource
        {
            get
            {
                return _DataSource;
            }
        }

        public string[] Variables { get; protected set; }

        public abstract void Load(string filename);

        public void OnUpdated(object sender, ArrayWatchObject<double> e)
        {
            if (Updated != null)
                Updated(sender, e);
        }

        public virtual void Clear()
        {
            if (State == RunningState.Busy)
                Stop();
            if (_DataSource != null)
                _DataSource.Clear();
        }
    }
}
