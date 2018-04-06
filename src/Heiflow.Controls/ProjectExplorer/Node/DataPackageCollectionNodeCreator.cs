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
