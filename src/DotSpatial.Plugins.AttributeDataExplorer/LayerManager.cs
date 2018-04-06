// -----------------------------------------------------------------------
// <copyright file="LayerManager.cs" company="">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// -----------------------------------------------------------------------

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using DotSpatial.Controls;
    using DotSpatial.Symbology;

    /// <summary>
    /// Since DotSpatial doesn't consistently select a layer when started, this class determines
    /// what layer should be considered active and notifies of any changes to the active layer.
    /// </summary>
    public sealed class LayerManager
    {
        /// <summary>
        /// Initializes a new instance of the LayerManager class.
        /// </summary>
        public LayerManager(IMap map)
        {
            _Map = map;
            WireUpMapEvents();

            // In case we are activated after the user loaded layers.
            LayerSelected();
        }

        /// <summary>
        /// Wires up events. When a project is opened, the map we had a reference to is sort of destroyed and all of its properties are serialized,
        /// so we need to wireup our events again by calling this method.
        /// </summary>
        public void WireUpMapEvents()
        {
            _Map.MapFrame.LayerSelected += MapFrame_LayerSelected;
            _Map.MapFrame.LayerAdded += MapFrame_LayerAdded;
            _Map.MapFrame.Layers.LayerSelected += Layers_LayerSelected;
            _Map.MapFrame.Layers.LayerRemoved += Layers_LayerRemoved;
        }

        private void Layers_LayerRemoved(object sender, LayerEventArgs e)
        {
            // this didn't work without introducing a new field
            // because a drag and drop operation calls LayerRemoved and then LayerAdded would leave us without our _ActiveLayer.
            if (e.Layer == _ActiveLayer)
            {
                // because you have to right click on a layer to remove it, they have
                // changed the active layer to the one that was removed.
                _RemovedLayer = _ActiveLayer;
                _ActiveLayer = null;
                OnActiveLayerChanged(EventArgs.Empty);
            }
        }

        private void MapFrame_LayerAdded(object sender, DotSpatial.Symbology.LayerEventArgs e)
        {
            // The first layer that is added is not automatically selected, so we show the data for it anyway.
            if (_Map.Layers.SelectedLayer == null && _Map.Layers.Count == 1)
            {
                _ActiveLayer = _Map.Layers[0];
                OnActiveLayerChanged(EventArgs.Empty);
            }
            else if (_Map.Layers.SelectedLayer == null && _RemovedLayer != null)
            {
                _ActiveLayer = _RemovedLayer;
                _RemovedLayer = null;
                OnActiveLayerChanged(EventArgs.Empty);
            }
        }

        private void Layers_LayerSelected(object sender, DotSpatial.Symbology.LayerSelectedEventArgs e)
        {
            LayerSelected();
        }

        private void MapFrame_LayerSelected(object sender, DotSpatial.Symbology.LayerSelectedEventArgs e)
        {
            LayerSelected();
        }

        private void LayerSelected()
        {
            if (_Map.Layers.SelectedLayer != _ActiveLayer)
            {
                _ActiveLayer = _Map.Layers.SelectedLayer;
                OnActiveLayerChanged(EventArgs.Empty);
            }
        }

        private ILayer _ActiveLayer;
        private ILayer _RemovedLayer;
        private readonly IMap _Map;

        public ILayer ActiveLayer
        {
            get
            {
                return _ActiveLayer;
            }
        }

        public IFeatureLayer ActiveFeatureLayer { get { return ActiveLayer as IFeatureLayer; } }

        public event EventHandler ActiveLayerChanged;

        #region OnActiveLayerChanged

        /// <summary>
        /// Triggers the ActiveLayerChanged event.
        /// </summary>
        public void OnActiveLayerChanged(EventArgs ea)
        {
            if (ActiveLayerChanged != null)
                ActiveLayerChanged(null, ea);
        }

        #endregion

        public EventHandler<LayerSelectedEventArgs> Layer_LayerSelected { get; set; }
    }
}