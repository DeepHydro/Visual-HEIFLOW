// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Project
{
    [Export(typeof(IExplorerNodeFactory))]
    public class ExplorerNodeFactory : IExplorerNodeFactory
    {
        [ImportMany]
        public IEnumerable<IExplorerNodeCreator> Creators
        {
            get;
            set;
        }
        public IExplorerNodeCreator Select(IExplorerItem item)
        {
            var creator = from cc in Creators where cc.ItemType == item.GetType() select cc;
            if (creator.Count() > 0)
                return creator.First();
            else
                return null;
        }
    }
}
