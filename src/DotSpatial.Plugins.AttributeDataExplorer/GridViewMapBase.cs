// --------------------------------------------------------------------------------------------------------------------
// <copyright file="" company="">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraGrid.Views.Grid;
using DotSpatial.Controls;

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    /// <summary>
    /// Used to store a reference to LayerManager, map, and GridView.
    /// </summary>
    public class GridViewMapBase
    {
        protected readonly LayerManager _LayerManager;
        protected readonly GridView _GridView;
        protected readonly IMap _Map;

        /// <summary>
        /// Initializes a new instance of the GridViewMapGlue class.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="gridView"></param>aram>
        public GridViewMapBase(IMap map, GridView gridView)
        {
            _GridView = gridView;
            _Map = map;
            _LayerManager = _Map.GetLayerManager();
        }
    }
}