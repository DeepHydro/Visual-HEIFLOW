// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Project
{
      [Export(typeof(IExplorerNodeCreator))]
    public class MapLayerNodeCreator : ExplorerNodeCreator
    {
          public MapLayerNodeCreator()
          {

          }

        public override Type ItemType
        {
            get 
            {
                return typeof(MapLayerItem);
            }
        }

        public override IExplorerNode Creat(object sender, Models.Generic.IExplorerItem item_attribute)
        {
            var layer_item = item_attribute as MapLayerItem;
            var layer_menu = ContextMenuFactory.Creat(layer_item) as IMapLayerContextMenu;
            layer_menu.Coverage = layer_item.Coverage;
            Node layer_node = new Node(layer_item.Coverage.LegendText)
            {
                Image = Resources.MapPackageTiledTPKFile16,
                Tag = layer_menu
            };
            layer_menu.Coverage.State = ModelObjectState.Ready;
            if(!File.Exists(layer_menu.Coverage.FullCoverageFileName))
            {
                layer_node.Image = Resources.PkgInfo_File16;
                layer_menu.Coverage.State = ModelObjectState.Error;
            }
            return layer_node;
        }
    }
}
