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
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Project
{
      [Export(typeof(IExplorerNodeCreator))]
    public class MapDataNodeCreator : ExplorerNodeCreator
    {
          private Node root_mapdata;
          public MapDataNodeCreator()
          {

          }

        public override Type ItemType
        {
            get 
            {
                return typeof(MapDataItem);
            }
        }

        public override IExplorerNode Creat(object sender, Models.Generic.IExplorerItem item_attribute)
        {
            var model = sender as IBasicModel;
            var root_menu = ContextMenuFactory.Creat(item_attribute);

            root_mapdata = new Node("Map Coverage")
            {
                Image = Resources.DataFrame16,
                Tag = root_menu
            };

            model.Project.FeatureCoverages.CollectionChanged += LayerParameters_CollectionChanged;
            model.Project.RasterLayerCoverages.CollectionChanged += LayerParameters_CollectionChanged;
            foreach (var lp in model.Project.FeatureCoverages)
            {
                MapLayerItem layer_item = new MapLayerItem();
                layer_item.Coverage = lp;
                var creator = NodeFactory.Select(layer_item);
                var layer_node = creator.Creat(model, layer_item) as Node;
                root_mapdata.Nodes.Add(layer_node);
            }
            foreach (var lp in model.Project.RasterLayerCoverages)
            {
                MapLayerItem layer_item = new MapLayerItem();
                layer_item.Coverage = lp;
                var creator = NodeFactory.Select(layer_item);
                var layer_node = creator.Creat(model, layer_item) as Node;
                root_mapdata.Nodes.Add(layer_node);
            }

            return root_mapdata;
        }

        private void LayerParameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var lp = e.NewItems[0] as PackageCoverage;
                MapLayerItem layer_item = new MapLayerItem();           
                layer_item.Coverage = lp;
               var creator =  NodeFactory.Select(layer_item);
               var layer_node = creator.Creat(sender, layer_item) as Node; 
                root_mapdata.Nodes.Add(layer_node);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var lp = e.OldItems[0];// as IFeatureLayerMapping;
                var node = from nn in root_mapdata.Nodes where (nn.Tag as MapLayerContextMenu).Coverage.Equals(lp) select nn;
                if (node.Count() == 1)
                    root_mapdata.Nodes.Remove(node.First());
            }
        }
    }
}
