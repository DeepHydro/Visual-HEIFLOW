// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.MenuItems;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic;
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
    public class MMSPackageNodeCreator : ExplorerNodeCreator
    {
        public MMSPackageNodeCreator()
        {

        }

        public override Type ItemType
        {
            get
            {
                return typeof(MMSPackageItem);
            }
        }

        public override IExplorerNode Creat(object sender, IExplorerItem item_attribute)
        {
            var pck = sender as IPackage;
            PackageContextMenu pcki = new PackageContextMenu()
            {
                Package = pck
            };
            pcki.Initialize();
            Node node = new Node(pck.Name)
            {
                Image = pck.Icon,
                Tag = pcki
            };

            var categories = from optc in pck.Parameters.Values
                             group optc by optc.DimensionNames[0] into cat
                             select new { Category = cat.Key, Items = cat.ToArray() };
            foreach (var cat in categories)
            {
                var menu_cat = new VariablesFolderContextMenu()
                {
                    Package = pck,
                    Tag = cat.Items
                };
                menu_cat.Initialize();
                menu_cat.Enable(VariablesFolderContextMenu._LA, false);
                menu_cat.Enable(VariablesFolderContextMenu._LAD, false);
                menu_cat.Enable(VariablesFolderContextMenu._OP, false);
                Node node_cat = new Node(cat.Category)
                {
                    Image = Resources.FolderWithGISData16,
                    Tag = menu_cat
                };
                foreach (var pr in cat.Items)
                {
                    DisplayablePropertyItem attribute = new DisplayablePropertyItem();
                    MMSParaContextMenu mmsi = new MMSParaContextMenu()
                    {
                        Package = pck,
                        ExplorerItem = attribute,
                        Parameter = pr
                    };
                    mmsi.Initialize();
                    if (pr.ValueCount == ModelService.NHRU)
                    {
                        mmsi.Enable(DisplayablePropertyContextMenu._LD, false);
                        Node ndpara = new Node(pr.Name)
                        {
                            Image = Resources.LayerRaster_B_16,
                            Tag = mmsi
                        };
                        node_cat.Nodes.Add(ndpara);
                    }
                    else
                    {
                        mmsi.Enable(DisplayablePropertyContextMenu._LD, false);
                        mmsi.Enable(DisplayablePropertyContextMenu._SOM, false);
                        mmsi.Enable(DisplayablePropertyContextMenu._VI3, false);
                        mmsi.Enable(DisplayablePropertyContextMenu._AN, false);
                        mmsi.Enable(DisplayablePropertyContextMenu._EX, false);

                        Node ndpara = new Node(pr.Name)
                        {
                            Image = Resources.single_object,
                            Tag = mmsi
                        };
                        node_cat.Nodes.Add(ndpara);
                    }
                    mmsi.Enable(DisplayablePropertyContextMenu._A2DC, true);
                }
                node.Nodes.Add(node_cat);
            }

            return node;
        }

    }
}
