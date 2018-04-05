// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
