// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.Tree;
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
    public class ModelNodeCreator : ExplorerNodeCreator
    {

        public ModelNodeCreator()
        {

        }

        public override Type ItemType
        {
            get
            {
                return typeof(ModelItem);
            }
        }

        public override IExplorerNode Creat(object sender, Models.Generic.IExplorerItem item_attribute)
        {
            if (sender is IntegratedModel)
            {
                var ihm = sender as IntegratedModel;
                var ihm_menu = ContextMenuFactory.Creat(item_attribute) as IModelContextMenu;
                ihm_menu.Model = ihm;

                Node node_ihm = new Node(ihm.Name)
                {
                    Image = ihm.Icon,
                    Tag = ihm_menu,
                };

                foreach (var mm in ihm.Children.Values)
                {
                    var model_menu = ContextMenuFactory.Creat(item_attribute) as IModelContextMenu;
                    model_menu.Model = mm;

                    Node node_child = new Node(mm.Name)
                    {
                        Image = mm.Icon,
                        Tag = model_menu
                    };

                    foreach (var pck in mm.Packages.Values)
                    {
                        var buf = pck.GetType().GetCustomAttributes(typeof(PackageItem), true);
                        if (buf.Length != 0)
                        {
                            var pck_atr = buf[0] as IExplorerItem;
                            var pck_node_creator = NodeFactory.Select(pck_atr);
                            Node node_pck = pck_node_creator.Creat(pck, pck_atr) as Node;
                            node_child.Nodes.Add(node_pck);
                        }
                    }

                    node_ihm.Nodes.Add(node_child);
                }
                return node_ihm;
            }
            else if (sender is IBasicModel)
            {
                var ihm = sender as IBasicModel;
                var model_menu = ContextMenuFactory.Creat(item_attribute) as IModelContextMenu;
                model_menu.Model = ihm;

                Node node_model = new Node(ihm.Name)
                {
                    Image = ihm.Icon,
                    Tag = model_menu
                };
                foreach (var pck in ihm.Packages.Values)
                {
                    var buf = pck.GetType().GetCustomAttributes(typeof(PackageItem), true);
                    if (buf.Length != 0)
                    {
                        var pck_atr = buf[0] as IExplorerItem;
                        var pck_node_creator = NodeFactory.Select(pck_atr);
                        Node node_pck = pck_node_creator.Creat(pck, pck_atr) as Node;
                        node_model.Nodes.Add(node_pck);
                    }
                }
                return node_model;
            }
            else
            {
                return null;
            }
        }
    }
}
