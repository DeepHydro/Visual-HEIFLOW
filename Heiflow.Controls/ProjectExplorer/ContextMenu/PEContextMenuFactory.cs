// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.MenuItems
{
    [Export(typeof(IPEContextMenuFactory))]
    public class PEContextMenuFactory : IPEContextMenuFactory
    {
        [ImportMany]
        public IEnumerable<IPEContextMenu> Menus
        {
            get;
            set;
        }

        public IPEContextMenu Creat(Models.Generic.IExplorerItem item)
        {
            var creator = from cc in Menus where cc.ItemType == item.GetType() select cc;
            if (creator.Count() > 0)
            {
                var menu_type= creator.First().GetType();
                var menu = Activator.CreateInstance(menu_type) as IPEContextMenu;
                menu.ExplorerItem = item;
                menu.Initialize();
                return menu;
            }
            else
                return null;
        }
    }
}
