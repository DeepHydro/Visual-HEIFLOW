// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.IO
{
    public abstract class BaseDataCube:IArrayStream
    {
        public event EventHandler<MyLazy3DMat<float>> Loaded;
        public event EventHandler<int> Loading;
        public event EventHandler<string> LoadFailed;
        public int StepsToLoad
        {
            get;
            set;
        }

        public int NumTimeStep
        {
            get;
            protected set;
        }

        public string[] Variables
        {
            get;
            set;
        }

        public MyLazy3DMat<float> Source
        {
            get;
            set;
        }

        public abstract void Scan();


        public abstract Data.My3DMat<float> Load();


        public abstract Data.My3DMat<float> Load(int var_index);

        public void SetDataSource(MyLazy3DMat<float> source)
        {
            this.Source = source;
        }

        protected virtual void OnLoading(int percent)
        {
            if (Loading != null)
                Loading(this, percent);
        }

        protected virtual void OnLoaded(MyLazy3DMat<float> e)
        {
            if (Loaded != null)
                Loaded(this, e);
        }

        protected virtual void OnLoadFailed(string msg)
        {
            if (LoadFailed != null)
                LoadFailed(this, msg);
        }

    }
}
