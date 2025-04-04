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
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Project
{
    [Export(typeof(IExplorerNodeCreator))]
    public class VariablesFolderNodeCreator : ExplorerNodeCreator
    {

        public override Type ItemType
        {
            get
            {
                return typeof(VariablesFolderItem);
            }
        }

        public override IExplorerNode Creat(object sender, Models.Generic.IExplorerItem item_attribute)
        {
            var pck = sender as IPackage;
            if (item_attribute.PropertyInfo != null)
            {
                var pck_prop = pck.GetType().GetProperty(item_attribute.PropertyInfo.Name).GetValue(pck) as IDataPackage;
                var mat_menu = ContextMenuFactory.Creat(item_attribute) as IPackageContextMemu;
                mat_menu.Package = pck_prop;
                Node node_mat = new Node(item_attribute.PropertyInfo.Name)
                {
                    Image = Resources.LayoutDataDrivenPagesChangeToPage16,
                    Tag = mat_menu
                };
                return node_mat;
            }
            else if (item_attribute.Tag is DynamicVariable)
            {
                var dp = item_attribute.Tag as DynamicVariable;
                var mat_menu = new VariablesFolderContextMenu();
                mat_menu.Initialize();
                mat_menu.Package = dp.Parent;
                Node node_mat = new Node(dp.Name)
                {
                    Image = Resources.LayoutDataDrivenPagesChangeToPage16,
                    Tag = mat_menu
                };
                return node_mat;
            }
            else
                return null;
        }
    }
}
