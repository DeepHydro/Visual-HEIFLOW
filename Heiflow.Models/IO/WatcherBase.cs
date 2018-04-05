// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public abstract class WatcherBase : IWatcher
    {
        public WatcherBase() 
        {
            CanPauseAndContinue = true;
        }

        public bool CanPauseAndContinue { get; set; }

       public  RunningState State { get; protected set; }
        public abstract void Start();
        public abstract void Pause();
        public abstract void Continue();
        public abstract void Stop();
        public abstract void Update();
    }
}
