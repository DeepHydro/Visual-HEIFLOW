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

using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Symbology;
using Heiflow.Plugins.Default.Properties;
using Heiflow.Controls.WinForm.Spatial;
using DotSpatial.Controls.Docking;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    public class TableEditorPlugin : Extension
    {
        //context menu item name
        //TODO: make this localizable
        private const string contextMenuItemName = "Attribute Table Editor";
        //private TableEditorControl _TableEditorControl;

        public override void Activate()
        {
            //App.HeaderControl.Add(new SimpleActionItem(HeaderControl.HomeRootItemKey, "View Attribute Table", AttributeTable_Click) { GroupCaption = "Map Tool",
            //                                                                                                                          SmallImage = Resources.table_green_48,
            //                                                                                                                          LargeImage = Resources.table_green_48
            //});
            //App.Map.LayerAdded += Map_LayerAdded;
            //App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            //// TODO: if layers were loaded before this plugin, do something about adding them to the context menu.
        
            //_TableEditorControl = new TableEditorControl();
            //_TableEditorControl.Size = new System.Drawing.Size(800, 600);
            //var dock = new DockablePanel("kAttributeTable", "Attribute Table", _TableEditorControl, DockStyle.None)
            //{
            //    SmallImage = Properties.Resources.table_green_48,
            //};

            //App.DockManager.Add(dock);

            //base.Activate();
        }

        private void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            // context menu items are added to layers when opening a project
            // this call is necessary because the LayerAdded event doesn't fire when a project is opened.
            foreach (ILayer layer in App.Map.MapFrame.GetAllLayers())
            {
                IFeatureLayer fl = layer as IFeatureLayer;
                if (fl != null)
                {
                    if (!fl.ContextMenuItems.Exists(item => item.Name == contextMenuItemName))
                    {
                        // add context menu item.
                        var menuItem = new SymbologyMenuItem(contextMenuItemName, delegate { ShowAttributes(fl); });
                        menuItem.Image = Resources.CatalogShowTree16;
                        fl.ContextMenuItems.Insert(2, menuItem);
                    }
                }
            }
            //attach layer added events to existing groups
            foreach (IGroup grp in App.Map.MapFrame.GetAllGroups())
            {
                grp.LayerAdded += Map_LayerAdded;
            }
        }

        private void Map_LayerAdded(object sender, LayerEventArgs e)
        {
            if (e.Layer == null)
                return;

            AddContextMenuItems(e.Layer);
        }

        private void AddContextMenuItems(ILayer addedLayer)
        {
            IMapGroup grp = addedLayer as IMapGroup;
            if (grp != null)
            {
                // map.layerAdded event doesn't fire for groups. Therefore, it's necessary
                // to handle this event separately for groups.
                grp.LayerAdded += Map_LayerAdded;
            }

            if (addedLayer.ContextMenuItems.Exists(item => item.Name == contextMenuItemName))
            {
                // assume menu item already exists. Do nothing.
                return;
            }

            // add context menu item.
            var menuItem = new SymbologyMenuItem(contextMenuItemName, delegate { ShowAttributes(addedLayer as IFeatureLayer); });
            menuItem.Image = Resources.table_green_48;
            var cmi = addedLayer.ContextMenuItems;
            if (cmi.Count > 2)
            {
                addedLayer.ContextMenuItems.Insert(2, menuItem);
            }
            else
            {
                addedLayer.ContextMenuItems.Add(menuItem);
            }
       }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            // detach events
            DetachLayerAddedEvents();
            App.SerializationManager.Deserializing -= SerializationManager_Deserializing;
            // remove context menu items.
            RemoveContextMenuItems();
            this.App.DockManager.Remove("kAttributeTable");
            base.Deactivate();
        }

        private void DetachLayerAddedEvents()
        {
            App.Map.LayerAdded -= Map_LayerAdded;
            foreach (IGroup grp in App.Map.MapFrame.GetAllGroups())
            {
                grp.LayerAdded -= Map_LayerAdded;
            }
        }

        private void RemoveContextMenuItems()
        {
            foreach (ILayer lay in App.Map.MapFrame.GetAllLayers())
            {
                if (lay.ContextMenuItems.Exists(item => item.Name == contextMenuItemName))
                {
                    lay.ContextMenuItems.Remove(lay.ContextMenuItems.First(item => item.Name == contextMenuItemName));
                    return;
                }
            }
        }

        private  void ShowAttributes(IFeatureLayer layer)
        {
            App.DockManager.ShowPanel("kAttributeTable");
            //_TableEditorControl.FeatureLayer = layer;
            //if (layer != null)
            //    layer.ShowAttributes();
        }

        /// <summary>
        /// Open attribute table
        /// </summary>
        private void AttributeTable_Click(object sender, EventArgs e)
        {
            IMapFrame mainMapFrame = App.Map.MapFrame;
            List<ILayer> layers = mainMapFrame.GetAllLayers();

            foreach (ILayer layer in layers.Where(l => l.IsSelected))
            {
                IFeatureLayer fl = layer as IFeatureLayer;

                if (fl == null)
                    continue;
                ShowAttributes(fl);
            }
        }
    }
}