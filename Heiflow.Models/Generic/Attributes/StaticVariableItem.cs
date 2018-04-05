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
    ///  Indicates a property (which is a My3DMat) needs to be shown
    /// </summary>
    public class StaticVariableItem : DisplayablePropertyItem
    {
        public StaticVariableItem( string prefix)
        {
            Prefix = prefix;
        }
        public StaticVariableItem()
        {

        }
        public string VariableName
        {
            get;
            set;
        }

        public string Prefix
        {
            get;
            private set;
        }

        public int VariableIndex
        {
            get;
            set;
        }

    }
}
