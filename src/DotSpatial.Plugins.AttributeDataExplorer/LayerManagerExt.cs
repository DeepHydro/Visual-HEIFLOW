// --------------------------------------------------------------------------------------------------------------------
// <copyright file="" company="">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// -----------------------------------------------------------------------

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DotSpatial.Controls;

    public static class LayerManagerExt
    {
        private static Dictionary<IMap, LayerManager> _LayerManagers = new Dictionary<IMap, LayerManager>();
        private static object syncLock = new object();

        /// <summary>
        /// Gets the layer manager for this map (returns a shared instance).
        /// </summary>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        public static LayerManager GetLayerManager(this IMap map)
        {
            lock (syncLock)
            {
                if (_LayerManagers.ContainsKey(map))
                    return _LayerManagers[map];

                var layerManager = new LayerManager(map);
                _LayerManagers.Add(map, layerManager);
                return layerManager;
            }
        }

        /// <summary>
        /// Clears the cache. You would not want to call this method while objects still have a reference to a layer manager that is cached.
        /// </summary>
        public static void ClearCache()
        {
            _LayerManagers.Clear();
        }
    }
}