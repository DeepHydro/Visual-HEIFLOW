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

namespace Heiflow.Controls.WinForm.Editors
{
    public class DropdownListSource : Attribute
    {

        public DropdownListSource(string source_name)
        {
            SourceName = source_name;
        }
        /// <summary>
        /// name of the property that holds the list source
        /// </summary>
        public string SourceName { get; protected set; }
    }
}
