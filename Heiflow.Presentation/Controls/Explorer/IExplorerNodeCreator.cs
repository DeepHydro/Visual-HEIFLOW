// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Presentation.Controls
{
    public interface IExplorerNodeCreator
    {
        IExplorerNodeFactory NodeFactory { get; set; }
        IPEContextMenuFactory ContextMenuFactory { get; set; }
        IExplorerNode Creat(object sender, IExplorerItem item_attribute);
        Type ItemType { get; }
    }
}
