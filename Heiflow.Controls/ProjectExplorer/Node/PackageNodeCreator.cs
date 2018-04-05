// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.MenuItems;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Controls.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Project
{
    [Export(typeof(IExplorerNodeCreator))]

    public class PackageNodeCreator : ExplorerNodeCreator
    {
        public PackageNodeCreator()
        {

        }

        public override Type ItemType
        {
            get
            {
                return typeof(PackageItem);
            }
        }


        public override IExplorerNode Creat(object package, IExplorerItem item_attribute)
        {
            var pck = package as IPackage;
            var pck_menu = ContextMenuFactory.Creat(item_attribute) ;
            var propinfos = pck.GetType().GetProperties();

            pck_menu.ExplorerItem = item_attribute;
            (pck_menu as IPackageContextMemu).Package = pck;

            Node pck_node = new Node(pck.Name)
            {
                Image = pck.Icon,
                Tag = pck_menu
            };
            if (pck.State == ModelObjectState.Standby || pck.State == ModelObjectState.Error)
                pck_node.Image = Resources.PkgInfo_File16;
            foreach (var pr in propinfos)
            {
                var atr = pr.GetCustomAttributes(typeof(DisplayablePropertyItem), true);
                if (atr.Length > 0)
                {
                    var dp_item = atr[0] as DisplayablePropertyItem;
                    dp_item.PropertyInfo = pr;
                    var prop_nodecreator = this.NodeFactory.Select(dp_item);
                    Node prop_node = prop_nodecreator.Creat(pck, dp_item) as Node;
                    pck_node.Nodes.Add(prop_node);
                }
            }
            return pck_node;
        }
    }
}
