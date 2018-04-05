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

namespace Heiflow.Models.Generic.Attributes
{
    public  class PackageCategory:Attribute
    {
        public PackageCategory(string cat, bool mandatory)
        {
            Category = cat;
            Mandatory = mandatory;
        }

        public string Category { get; protected set; }

        public bool Mandatory { get; protected set; }

        public int Depth
        {
            get
            {
                if (string.IsNullOrEmpty(Category))
                    return 0;
                var strs = TypeConverterEx.Split<string>(Category, TypeConverterEx.Comma);
                return strs.Length;
            }
        }

        public string Root
        {
            get
            {
                if (string.IsNullOrEmpty(Category))
                    return "";
                var strs = TypeConverterEx.Split<string>(Category, TypeConverterEx.Comma);
                return strs[0];
            }
        }

        public string [] Tokens
        {
            get
            {
                if (string.IsNullOrEmpty(Category))
                    return null;
                var strs = TypeConverterEx.Split<string>(Category, TypeConverterEx.Comma);
                return strs;
            }
        }
    }
}
