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

namespace Heiflow.Core.Data.ODM
{
    public  class DendritiRecord<T>:IDendritiRecord<T>
    {
        public DendritiRecord()
        {
            Children = new List<IDendritiRecord<T>>();
        }

        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IDendritiRecord<T> Parent
        {
            get;
            set;
        }

        public List<IDendritiRecord<T>> Children
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public T Value
        {
            get;
            set;
        }

        public object Tag
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public bool CanDelete
        {
            get;
            set;
        }


        public bool CanExport2Shp
        {
            get;
            set;
        }

        public bool CanExport2Excel
        {
            get;
            set;
        }
    }
}
