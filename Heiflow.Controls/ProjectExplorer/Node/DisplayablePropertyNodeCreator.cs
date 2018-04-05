// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Project
{
    [Export(typeof( IExplorerNodeCreator))]
    public class DisplayablePropertyNodeCreator : ExplorerNodeCreator
    {
        public DisplayablePropertyNodeCreator()
        {

        }

        public override Type ItemType
        {
            get 
            {
                return typeof(DisplayablePropertyItem);
            }
        }

        public override IExplorerNode Creat(object sender, Models.Generic.IExplorerItem item_attribute)
        {
            var pck = sender as IPackage;
            var mat_menu = ContextMenuFactory.Creat(item_attribute) as IPackageContextMemu;
            mat_menu.Package=pck;
            var name = item_attribute.PropertyInfo.Name;
            if (item_attribute.PropertyInfo != null)
                name = item_attribute.PropertyInfo.Name;

            Node node_mat = new Node(name)
            {
                Image = Resources.LayerRaster_B_16,
                Tag = mat_menu
            };

            return node_mat;
        }
    }
}
