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
