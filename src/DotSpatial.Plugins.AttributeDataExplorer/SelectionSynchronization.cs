// -----------------------------------------------------------------------
// <copyright file="SelectionSynchronization.cs" company="">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// -----------------------------------------------------------------------

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DevExpress.XtraGrid.Views.Grid;
    using DotSpatial.Controls;
    using DotSpatial.Symbology;

    /// <summary>
    /// Updating the map selection or rows selected in the table when the other is modified.
    /// </summary>
    public class SelectionSynchronization : GridViewMapBase
    {
        /// <summary>
        /// Initializes a new instance of the GridViewMapGlue class.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="gridView"></param>aram>
        public SelectionSynchronization(IMap map, GridView gridView)
            : base(map, gridView)
        {
            _GridView.SelectionChanged += GridView_SelectionChanged;
            WireUpMapEvents();

            _LayerManager.ActiveLayerChanged += LayerManager_ActiveLayerChanged;
        }

        /// <summary>
        /// Wires up events. When a project is opened, the map we had a reference to is sort of destroyed and all of its properties are serialized,
        /// so we need to wireup our events again.
        /// </summary>
        public void WireUpMapEvents()
        {
            _Map.MapFrame.SelectionChanged += MapFrame_SelectionChanged;
        }

        private void LayerManager_ActiveLayerChanged(object sender, EventArgs e)
        {
            ShowSelectionFromCurrentLayer(_Map);
        }

        private void MapFrame_SelectionChanged(object sender, EventArgs e)
        {
            ShowSelectionFromCurrentLayer(_Map);
        }

        public bool IsUserMakingSelection { get; set; }

        /// <summary>
        /// Shows the selection from current layer in the grid.
        /// </summary>
        /// <param name="map">The map.</param>
        public void ShowSelectionFromCurrentLayer(IMap map)
        {
            if (IsUserMakingSelection) return;
            if (_GridView.RowCount == 0) return;
            var _featureLayer = _LayerManager.ActiveFeatureLayer;
            if (_featureLayer == null) return;

            // Perhaps Selection should be changed in DotSpatial so that it isn't null for a moment when the selection is cleared.
            if (_featureLayer.Selection == null)
                return;

            var selection = _featureLayer.Selection.ToFeatureList();

            _GridView.BeginSelection();
            _GridView.ClearSelection();
            try
            {
                foreach (var sel in selection)
                {
                    // if the layer is polygon it is 1 based
                    // if the layer is point it is 0 based.
                    // line layer is 1 based.
                    // Record number is inconsistent as shown so we can't use sel.RecordNumber - 1, but the index of the datarow holds true for DS
                    // int index = sel.DataRow.Table.Rows.IndexOf(sel.DataRow);

                    // Now the FDO way
                    int index = sel.RecordNumber - 1;
                    _GridView.SelectRow(_GridView.GetRowHandle(index));
                }
            }
            finally
            {
                _GridView.SelectionChanged -= GridView_SelectionChanged;
                _GridView.EndSelection();
                _GridView.SelectionChanged += GridView_SelectionChanged;
            }
        }

        private int FindRowHandleByRowObject(DevExpress.XtraGrid.Views.Grid.GridView view, object row)
        {
            if (row != null)

                for (int i = 0; i < view.DataRowCount; i++)

                    if (row.Equals(view.GetRow(i)))

                        return i;

            return DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        }

        private void GridView_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (e.Action == System.ComponentModel.CollectionChangeAction.Add && e.ControllerRow == 0)
            {
                // Selection changed is fired to automatically select the first row of the grid view when a
                // new dataset is loaded or when the layer is selected.
                // we shouldn't do anything in that case.
                return;
            }
            if (MainForm.IsLayoutRestoring) return;
            IsUserMakingSelection = true;
            var _featureLayer = _LayerManager.ActiveFeatureLayer;
            if (_featureLayer == null) return;

            _featureLayer.Selection.SuspendChanges();
            _featureLayer.UnSelectAll();
            if (_GridView.SelectedRowsCount > 0)
            {
                foreach (System.Int32 row in _GridView.GetSelectedRows())
                {
                    _featureLayer.Select(_GridView.GetDataSourceRowIndex(row));
                }
            }
            _featureLayer.Selection.ResumeChanges();
            IsUserMakingSelection = false;
        }
    }
}