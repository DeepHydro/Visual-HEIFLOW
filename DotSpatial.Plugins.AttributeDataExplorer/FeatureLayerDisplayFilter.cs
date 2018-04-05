// -----------------------------------------------------------------------
// <copyright file="FeatureLayerFilter.cs" company="">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Data;
using DotSpatial.Symbology;

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Prevents painting features that are not displayed in in the table due to filtering.
    /// </summary>
    public sealed class FeatureLayerDisplayFilter : GridViewMapBase
    {
        private object _CurrentDataSource;

        /// <summary>
        /// Initializes a new instance of the FeatureLayerFilter class.
        /// </summary>
        /// <param name="map"></param>
        public FeatureLayerDisplayFilter(IMap map, GridView gridView)
            : base(map, gridView)
        {
            _GridView.ColumnFilterChanged += GridView_ColumnFilterChanged;
            _GridView.GridControl.DataSourceChanged += new EventHandler(GridControl_DataSourceChanged);
        }

        private void GridControl_DataSourceChanged(object sender, EventArgs e)
        {
            _CurrentDataSource = _GridView.GridControl.DataSource;
        }

        private void GridView_ColumnFilterChanged(object sender, EventArgs e)
        {
            if (_CurrentDataSource == _GridView.GridControl.DataSource)
            {
                if (MainForm.IsLayoutRestoring) return;

                ToggleFeatureVisibility();
            }
            else
            {
                _GridView.ClearColumnsFilter();
            }
        }

        /// <summary>
        /// Toggles the visibility of featuers based on whether they are visible in the gridview.
        /// </summary>
        private void ToggleFeatureVisibility()
        {
            IFeatureLayer layer = _LayerManager.ActiveFeatureLayer;
            if (layer == null)
            {
                return;
            }

            for (int i = 0; i < layer.DrawnStates.Length; i++)
            {
                int row = _GridView.GetRowHandle(i);
                layer.DrawnStates[i].Visible = row != DevExpress.XtraGrid.GridControl.InvalidRowHandle;
            }
            _Map.MapFrame.ResetBuffer();
        }

        /// <summary>
        /// Make all points visible for every single layer when exiting.
        /// </summary>
        internal void ShowAllFeatures()
        {
            foreach (IMapFeatureLayer layer in _Map.GetFeatureLayers())
            {
                for (int i = 0; i < layer.DrawnStates.Length; i++)
                {
                    layer.DrawnStates[i].Visible = true;
                }
            }
            _Map.MapFrame.ResetBuffer();
        }
    }
}