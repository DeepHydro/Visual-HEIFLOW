// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
