//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.MenuItems;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.GHM;
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
    [Export(typeof(IExplorerNodeCreator))]
    public class ModelNodeCreator : ExplorerNodeCreator
    {
        public class PckCat
        {
            public string Category { get; set; }
            public List<IPackage> Packages { get; set; }
        }
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

                    var pckcats = from par in mm.Packages.Values
                                  group par by par.Category into pp
                                  select new PckCat
                               {
                                   Category = pp.Key.ToString(),
                                   Packages = pp.ToList()
                               };

                    if (pckcats.Count() == 1)
                    {
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
                    }
                    else
                    {
                        var ordered = pckcats.OrderBy(key => key.Category);
                        foreach (var cat in ordered)
                        {
                            Node node_cat = new Node(cat.Category)
                            {
                                Image = mm.Icon,
                                Tag = null
                            };
                            foreach (var pck in cat.Packages)
                            {
                                var buf = pck.GetType().GetCustomAttributes(typeof(PackageItem), true);
                                if (buf.Length != 0)
                                {
                                    var pck_atr = buf[0] as IExplorerItem;
                                    var pck_node_creator = NodeFactory.Select(pck_atr);
                                    Node node_pck = pck_node_creator.Creat(pck, pck_atr) as Node;
                                    node_cat.Nodes.Add(node_pck);
                                }
                            }
                            node_child.Nodes.Add(node_cat);
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
                var pckcats = from par in ihm.Packages.Values
                              group par by par.Category into pp
                              select new PckCat
                              {
                                  Category = pp.Key.ToString(),
                                  Packages = pp.ToList()
                              };

                if (pckcats.Count() == 1)
                {
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
                }
                else
                {
                    var ordered = pckcats.OrderBy(key => key.Category);
                    foreach (var cat in ordered)
                    {
                        Node node_cat = new Node(cat.Category)
                        {
                            Image = ihm.Icon,
                            Tag = null
                        };
                        foreach (var pck in cat.Packages)
                        {
                            var buf = pck.GetType().GetCustomAttributes(typeof(PackageItem), true);
                            if (buf.Length != 0)
                            {
                                var pck_atr = buf[0] as IExplorerItem;
                                var pck_node_creator = NodeFactory.Select(pck_atr);
                                Node node_pck = pck_node_creator.Creat(pck, pck_atr) as Node;
                                node_cat.Nodes.Add(node_pck);
                            }
                        }
                        node_model.Nodes.Add(node_cat);
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
