// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using Heiflow.Controls.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic.Attributes;

namespace Heiflow.Controls.WinForm.Project
{
    [Export(typeof(IExplorerNodeCreator))]
    public class DataPackageCollectionNodeCreator : ExplorerNodeCreator
    {
        public DataPackageCollectionNodeCreator()
        {

        }


        public override Type ItemType
        {
            get
            {
                return typeof(DataPackageCollectionItem);
            }
        }

        public override IExplorerNode Creat(object sender, IExplorerItem item_attribute)
        {
            var pck = sender as DataPackageCollection;
            var pck_menu = ContextMenuFactory.Creat(item_attribute);
            Node node_pck = new Node(pck.Name)
            {
                Image = Resources.LayoutDataDrivenPagesChangeToPage16,
                Tag = pck_menu
            };

            foreach(var pck_child in pck.Children)
            {
                VariablesFolderItem atr_pck_child = new VariablesFolderItem();
                atr_pck_child.PropertyInfo = new SimplePropertyInfo(pck_child.Name, pck_child.GetType());
                var child_menu = ContextMenuFactory.Creat(atr_pck_child) as IPackageContextMemu;        
                child_menu.Package = pck_child;
                Node node_child = new Node(pck_child.Name)
                {
                    Image = Resources.FolderWithGISData16,
                    Tag = child_menu
                };
                node_pck.Nodes.Add(node_child);
            }

            return node_pck;
        }
    }
}
