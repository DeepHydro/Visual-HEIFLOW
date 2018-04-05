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
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Project
{
    [Export(typeof(IExplorerNodeCreator))]
    public class TimieSeriesVariableNodeCreator : ExplorerNodeCreator
    {
        public TimieSeriesVariableNodeCreator()
        {

        }

        public override Type ItemType
        {
            get
            {
                return typeof(TimeSeriesVariableItem);
            }
        }

        public override IExplorerNode Creat(object sender, IExplorerItem item_attribute)
        {
            var pck = sender as IPackage;
            var atr = item_attribute as TimeSeriesVariableItem;
            var mat_menu = ContextMenuFactory.Creat(item_attribute) as IPackageContextMemu;
            mat_menu.Package = pck;
            (mat_menu as IVariableContextMenu).VariableIndex = atr.VariableIndex;

            Node node_mat = new Node(item_attribute.PropertyInfo.Name)
            {
                Image = Resources.LayerRaster_B_16_gray,
                Tag = mat_menu
            };

            return node_mat;
        }
    }
}
