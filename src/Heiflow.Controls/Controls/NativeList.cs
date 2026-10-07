//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
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

using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Controls
{
    /// <summary>
    /// Fills a plain WinForms list view from a collection of objects. The value shown in a column
    /// is taken from the property whose name is stored in ColumnHeader.Name, so the column
    /// definitions in the designer double as the binding. This replaces ObjectListView's
    /// SetObjects() and its DataSource binding, keeping the project free of third party controls.
    /// </summary>
    public static class NativeList
    {
        /// <summary>Rebuilds the rows of <paramref name="view"/> from <paramref name="items"/>.
        /// Each row keeps the object it was built from in its Tag.</summary>
        public static void Fill(ListView view, IEnumerable items)
        {
            if (view == null)
                return;

            view.BeginUpdate();
            view.Items.Clear();
            if (items != null)
            {
                var properties = new string[view.Columns.Count];
                for (int i = 0; i < properties.Length; i++)
                    properties[i] = view.Columns[i].Name;

                foreach (var item in items)
                {
                    if (item == null)
                        continue;
                    var values = new string[properties.Length];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var property = string.IsNullOrEmpty(properties[i])
                            ? null
                            : item.GetType().GetProperty(properties[i]);
                        var value = property == null ? null : property.GetValue(item, null);
                        values[i] = value == null ? string.Empty : value.ToString();
                    }
                    view.Items.Add(new ListViewItem(values) { Tag = item });
                }
            }
            view.EndUpdate();
        }

        /// <summary>The object behind the selected row, or null when nothing is selected.</summary>
        public static T Selected<T>(ListView view) where T : class
        {
            if (view == null || view.SelectedItems.Count == 0)
                return null;
            return view.SelectedItems[0].Tag as T;
        }

        /// <summary>The object behind the selected row, or null when nothing is selected.</summary>
        public static object Selected(ListView view)
        {
            if (view == null || view.SelectedItems.Count == 0)
                return null;
            return view.SelectedItems[0].Tag;
        }

        /// <summary>The object behind the selected row of a data grid, or null when nothing is
        /// selected. Data grids are used where the values have to stay editable.</summary>
        public static T SelectedRow<T>(DataGridView view) where T : class
        {
            if (view == null || view.SelectedRows.Count == 0)
                return null;
            return view.SelectedRows[0].DataBoundItem as T;
        }

        /// <summary>The object behind the selected row of a data grid, or null when nothing is
        /// selected.</summary>
        public static object SelectedRow(DataGridView view)
        {
            if (view == null || view.SelectedRows.Count == 0)
                return null;
            return view.SelectedRows[0].DataBoundItem;
        }

        /// <summary>Shows the objects of <paramref name="items"/> in <paramref name="grid"/>. The
        /// columns of the grid supply the property names, and editing a cell writes straight back
        /// onto the object, which is what the ObjectListView based grids did before.</summary>
        public static void Bind(DataGridView grid, IEnumerable items)
        {
            if (grid == null)
                return;

            // A data grid needs a list, so anything else is materialised first. The rows still
            // hold the very same objects, so edits made in the grid reach them.
            object source = items as IList;
            if (source == null && items != null)
            {
                var list = new List<object>();
                foreach (var item in items)
                    list.Add(item);
                source = list;
            }
            grid.DataSource = source;
        }
    }
}
