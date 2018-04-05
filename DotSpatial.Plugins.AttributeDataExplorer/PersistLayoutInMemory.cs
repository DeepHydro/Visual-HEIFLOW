// -----------------------------------------------------------------------
// <copyright file="PersistFilterInMemory.cs" company="">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// -----------------------------------------------------------------------

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using DevExpress.XtraGrid.Views.Grid;
    using DotSpatial.Controls;
    using DotSpatial.Symbology;

    /// <summary>
    /// Keep the current filter and layout in memory so that we can restore them after
    /// layer selection changes.
    /// </summary>
    public class PersistLayoutInMemory : GridViewMapBase
    {
        private IFeatureLayer _LastFeatureLayer;
        private Dictionary<IFeatureLayer, MemoryStream> _PersistedSettings = new Dictionary<IFeatureLayer, MemoryStream>();

        /// <summary>
        /// Initializes a new instance of the GridViewMapGlue class.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="gridView"></param>aram>
        public PersistLayoutInMemory(IMap map, GridView gridView)
            : base(map, gridView)
        {
            _LayerManager.ActiveLayerChanged += new EventHandler(LayerManager_ActiveLayerChanged);
            _GridView.GridControl.TextChanged += new EventHandler(GridControl_TextChanged);
        }

        // Hack: We use the TextChanged event as a "I'm about to databind" event.
        private void GridControl_TextChanged(object sender, EventArgs e)
        {
            PersistLayout();
        }

        private void LayerManager_ActiveLayerChanged(object sender, EventArgs e)
        {
            // Determine if the user previously filtered this layer.
            IFeatureLayer layer = _LayerManager.ActiveFeatureLayer;
            _LastFeatureLayer = layer;
            if (layer == null)
            {
                return;
            }

            MainForm.IsLayoutRestoring = true;

            if (_PersistedSettings.ContainsKey(layer))
            {
                // We could alternately store the layout in an xml file with the shapefile.
                MemoryStream stream = _PersistedSettings[layer];
                _GridView.RestoreLayoutFromStream(stream);
                stream.Position = 0;
            }

            MainForm.IsLayoutRestoring = false;
        }

        /// <summary>
        /// Persists the filter expression so that we can restore it if the user changes layers.
        /// </summary>
        private void PersistLayout()
        {
            IFeatureLayer layer = _LastFeatureLayer;
            if (layer == null)
            {
                return;
            }

            // Todo: remove reference to layer and this entry in our dicitonary when the layer is removed from the map.
            MemoryStream stream = new MemoryStream();
            _GridView.SaveLayoutToStream(stream);
            stream.Position = 0;
            _PersistedSettings[layer] = stream;
        }
    }
}