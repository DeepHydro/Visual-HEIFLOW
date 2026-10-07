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

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Controls
{
    /// <summary>
    /// Fills a plain WinForms tree view from a table that describes a hierarchy through an ID and a
    /// ParentID column, which is how the budget tables are built. This replaces the
    /// DataTreeListView binding so that no third party control is needed.
    /// </summary>
    public static class NativeTree
    {
        /// <summary>Value used by the budget tables to mark a row as a top level row.</summary>
        public const int RootKeyValue = 9999;

        /// <summary>Rebuilds <paramref name="view"/> from <paramref name="table"/>: the text of a
        /// node comes from <paramref name="textColumn"/>, the rest of the columns are shown behind
        /// it and repeated in the node tooltip. The tree is left expanded.</summary>
        public static void Fill(TreeView view, DataTable table, string textColumn,
            params string[] detailColumns)
        {
            if (view == null)
                return;

            view.BeginUpdate();
            view.Nodes.Clear();
            if (table != null && table.Columns.Contains("ID") && table.Columns.Contains("ParentID"))
            {
                var nodes = new Dictionary<int, TreeNode>();
                foreach (DataRow row in table.Rows)
                {
                    var node = new TreeNode(NodeText(row, textColumn, detailColumns));
                    node.ToolTipText = NodeToolTip(row, detailColumns);
                    node.Tag = row;
                    nodes[Key(row)] = node;
                }

                foreach (DataRow row in table.Rows)
                {
                    var node = nodes[Key(row)];
                    var parentId = ParentKey(row);
                    TreeNode parent;
                    if (parentId == RootKeyValue || !nodes.TryGetValue(parentId, out parent))
                        view.Nodes.Add(node);
                    else
                        parent.Nodes.Add(node);
                }
            }
            view.EndUpdate();
            if (view.Nodes.Count > 0)
                view.ExpandAll();
        }

        private static int Key(DataRow row)
        {
            return Convert.ToInt32(row["ID"]);
        }

        private static int ParentKey(DataRow row)
        {
            return row["ParentID"] == DBNull.Value ? RootKeyValue : Convert.ToInt32(row["ParentID"]);
        }

        private static string NodeText(DataRow row, string textColumn, string[] detailColumns)
        {
            var text = textColumn != null && row.Table.Columns.Contains(textColumn)
                ? Convert.ToString(row[textColumn])
                : string.Empty;

            var details = new List<string>();
            foreach (var column in detailColumns)
            {
                if (column == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                    continue;
                var value = row[column];
                details.Add(value is double || value is float
                    ? Convert.ToDouble(value).ToString("0.###")
                    : Convert.ToString(value));
            }
            return details.Count == 0 ? text : text + "   [" + string.Join(",  ", details) + "]";
        }

        private static string NodeToolTip(DataRow row, string[] detailColumns)
        {
            var lines = new List<string>();
            foreach (var column in detailColumns)
            {
                if (column == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                    continue;
                lines.Add(column + " = " + Convert.ToString(row[column]));
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
