// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.Database
{
    public abstract class DatabaseSource : IDatabaseSource
    {
        public event EventHandler<int> ProgressChanged;
        public event EventHandler Started;
        public event EventHandler Finished;

        public DatabaseSource()
        {

        }

        public abstract bool Open();

        public abstract bool Open(string dbpath, ref string msg);

        public abstract void Close();

        protected virtual void OnProgressChanged(int current)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, current);
        }

        protected virtual void OnStarted()
        {
            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        protected virtual void OnFinished()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }

    }
}
