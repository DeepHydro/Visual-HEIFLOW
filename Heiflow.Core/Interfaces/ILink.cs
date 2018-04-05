// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core
{
    public interface ILink
    {
        string ID { get; set; }
        ILinkableObject Target { get; set; }
        ILinkableObject Oringin { get; set; }
        bool Validate();
    }

    public interface ILinkableObject
    {
        string ModuleID { get; set; }
        string Descriptions { get; set; }
        void AddLink(string linkID);
        void RemoveLink(string linkID);
    }
}
