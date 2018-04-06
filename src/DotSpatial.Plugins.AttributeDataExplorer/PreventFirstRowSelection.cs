// -----------------------------------------------------------------------
// <copyright file="PreventFirstRowSelection.cs" company="">
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

    /// <summary>
    /// By default the grid selects the first row if all selected rows become invisible after filtering.
    /// We override this behavior so that the selection is cleared in that particular case.
    /// </summary>
    public class PreventFirstRowSelection
    {
        protected readonly GridView _GridView;
        private int _FilteredSelectedRowsCount = -1;

        /// <summary>
        /// Initializes a new instance of the PreventFirstRowSelection class.
        /// </summary>
        /// <param name="gridView"></param>
        public PreventFirstRowSelection(GridView gridView)
        {
            _GridView = gridView;
            _GridView.ColumnFilterChanged += GridView_ColumnFilterChanged;
            _GridView.SelectionChanged += GridView_SelectionChanged;
        }

        private void GridView_ColumnFilterChanged(object sender, EventArgs e)
        {
            // This event will occur
            // (1) if the find text has been changed
            // (2) if the column filter has been changed or
            // (3) when PopulateColumns() is called after the datasource is changed and there was a filter
            // to prevent the last case we use the IsDataBinding static field.
            if (MainForm.IsDataBinding) return;
            if (MainForm.IsLayoutRestoring) return;

            // http://www.devexpress.com/Support/Center/p/Q280595.aspx
            _FilteredSelectedRowsCount = _GridView.SelectedRowsCount;
        }

        private void GridView_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            // By default the grid selects the first row if all selected rows become invisible after filtering.
            // when the FindFilter is being set, the _FilteredSelectedRowsCount can become 0 when _GridView.SelectedRowsCount is 1 here and the selection
            // will still be modified at at later point, so we can ignore this right now.
            if (_FilteredSelectedRowsCount == 0)
            {
                _FilteredSelectedRowsCount = -1;

                _GridView.ClearSelection();
            }
        }
    }
}