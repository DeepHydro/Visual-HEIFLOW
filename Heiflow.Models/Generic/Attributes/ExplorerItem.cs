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

namespace Heiflow.Models.Generic
{
    /// <summary>
    /// Indiate that the annoated property is  a part of a model and couble be shown in the Project Explorer
    /// </summary>
    public class ExplorerItem : Attribute, IExplorerItem
    {
        //public string PropertyName
        //{
        //    get;
        //    set;
        //}
        public System.Reflection.PropertyInfo PropertyInfo
        {
            get;
            set;
        }
    }
}
