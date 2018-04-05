// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Project
{
    public abstract class ExplorerNodeCreator : IExplorerNodeCreator
    {
        public ExplorerNodeCreator()
        {

        }

        public IExplorerNodeFactory NodeFactory
        {
            get;
            set;
        }

        public IPEContextMenuFactory ContextMenuFactory
        {
            get;
            set;
        }

        public abstract Type ItemType { get; }
   
        public abstract IExplorerNode Creat(object sender, Models.Generic.IExplorerItem item_attribute);


    }
}
