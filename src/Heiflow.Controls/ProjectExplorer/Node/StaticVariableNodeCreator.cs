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
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
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
    public class StaticVariableNodeCreator : ExplorerNodeCreator
    {
         public override Type ItemType
         {
             get
             {
                 return typeof(StaticVariableItem);
             }
         }

        public override IExplorerNode Creat(object sender, IExplorerItem item_attribute)
        {
            var mat_atr = item_attribute as StaticVariableItem;
            var pck = sender as IPackage;
            var folder_item = new VariablesFolderItem()
            {
                PropertyInfo = item_attribute.PropertyInfo
            };
            var folder_menu = ContextMenuFactory.Creat(folder_item);
            var value = pck.GetType().GetProperty(item_attribute.PropertyInfo.Name).GetValue(pck);
            var dc = value as IDataCubeObject;
            folder_menu.EneableAll(false);
            folder_menu.Enable(VariablesFolderContextMenu._AT, true);
            folder_menu.Enable(VariablesFolderContextMenu._OP, false);          
            (folder_menu as IPackageContextMemu).Package = pck;
           
            Node node_folder = new Node(item_attribute.PropertyInfo.Name)
            {
                Image = Resources.FolderWithGISData16,
                Tag = folder_menu
            };
            if (dc != null)
            {  
                folder_item.Variables = new string[dc.Size[0]];
                for (int i = 0; i < dc.Size[0]; i++)
                {
                    StaticVariableItem item = new StaticVariableItem("")
                    {
                        VariableIndex = i,
                        VariableName = dc.Variables[i],  //string.Format("{0} {1}{2}", item_attribute.PropertyInfo.Name, mat_atr.Prefix, i + 1),
                        PropertyInfo = item_attribute.PropertyInfo
                    };
                    folder_item.Variables[i] = item.VariableName;
                    StaticVariableContextMenu elei = new StaticVariableContextMenu()
                    {
                        Package = pck,
                        VariableIndex = i,
                        ExplorerItem = item
                    };
                    elei.Initialize();
                    Node ndmat = new Node(item.VariableName)
                    {
                        Image = Resources.LayerRaster_B_16,
                        Tag = elei
                    };
                    node_folder.Nodes.Add(ndmat);
                }
            }
            else
            {

            }
            return node_folder;
        }
    }
}
