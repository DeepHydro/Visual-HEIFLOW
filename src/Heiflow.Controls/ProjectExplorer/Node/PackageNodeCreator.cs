﻿//
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
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
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

            if (pck is GHMPackage)
            {
                var ghmpck = pck as GHMPackage;
                foreach (var pr in ghmpck.StaticVariables)
                {
                    var dp_item = new StaticVariableItem();
                    dp_item.Tag = pr;
                    dp_item.VariableName = pr.Name;
                    var prop_nodecreator = this.NodeFactory.Select(dp_item);
                    if (prop_nodecreator != null)
                    {
                        Node prop_node = prop_nodecreator.Creat(pck, dp_item) as Node;
                        pck_node.Nodes.Add(prop_node);
                    }
                }
                foreach (var pr in ghmpck.DynamicVariables)
                {
                    var dp_item = new VariablesFolderItem();
                    dp_item.Tag = pr;
                    var prop_nodecreator = this.NodeFactory.Select(dp_item);
                    if (prop_nodecreator != null)
                    {
                        Node prop_node = prop_nodecreator.Creat(pck, dp_item) as Node;
                        pck_node.Nodes.Add(prop_node);
                    }
                }
            }
            else
            {
                foreach (var pr in propinfos)
                {
                    var atr = pr.GetCustomAttributes(typeof(DisplayablePropertyItem), true);
                    if (atr.Length > 0)
                    {
                        var dp_item = atr[0] as DisplayablePropertyItem;
                        dp_item.PropertyInfo = pr;
                        var prop_nodecreator = this.NodeFactory.Select(dp_item);
                        if (prop_nodecreator != null)
                        {
                            Node prop_node = prop_nodecreator.Creat(pck, dp_item) as Node;
                            pck_node.Nodes.Add(prop_node);
                        }
                    }
                }
            }
            return pck_node;
        }
    }
}
