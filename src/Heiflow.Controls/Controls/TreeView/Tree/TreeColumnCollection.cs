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
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Heiflow.Controls.Tree
{
	internal class TreeColumnCollection : Collection<TreeColumn>
	{
		private TreeViewAdv _treeView;

		public TreeColumnCollection(TreeViewAdv treeView)
		{
			_treeView = treeView;
		}

		protected override void InsertItem(int index, TreeColumn item)
		{
			base.InsertItem(index, item);
			BindEvents(item);
			_treeView.UpdateColumns();
		}

		protected override void RemoveItem(int index)
		{
			UnbindEvents(this[index]);
			base.RemoveItem(index);
			_treeView.UpdateColumns();
		}

		protected override void SetItem(int index, TreeColumn item)
		{
			UnbindEvents(this[index]);
			base.SetItem(index, item);
			item.Owner = this;
			BindEvents(item);
			_treeView.UpdateColumns();
		}

		protected override void ClearItems()
		{
			foreach (TreeColumn c in Items)
				UnbindEvents(c);
			Items.Clear();
			_treeView.UpdateColumns();
		}

		private void BindEvents(TreeColumn item)
		{
			item.Owner = this;
			item.HeaderChanged += HeaderChanged;
			item.IsVisibleChanged += IsVisibleChanged;
			item.WidthChanged += WidthChanged;
			item.SortOrderChanged += SortOrderChanged;
		}

		private void UnbindEvents(TreeColumn item)
		{
			item.Owner = null;
			item.HeaderChanged -= HeaderChanged;
			item.IsVisibleChanged -= IsVisibleChanged;
			item.WidthChanged -= WidthChanged;
			item.SortOrderChanged -= SortOrderChanged;
		}

		void SortOrderChanged(object sender, EventArgs e)
		{
			TreeColumn changed = sender as TreeColumn;
			//Only one column at a time can have a sort property set
			if (changed.SortOrder != SortOrder.None)
			{
				foreach (TreeColumn col in this)
				{
					if (col != changed)
						col.SortOrder = SortOrder.None;
				}
			}
			_treeView.UpdateHeaders();
		}

		void WidthChanged(object sender, EventArgs e)
		{
			_treeView.ChangeColumnWidth(sender as TreeColumn);
		}

		void IsVisibleChanged(object sender, EventArgs e)
		{
			_treeView.FullUpdate();
		}

		void HeaderChanged(object sender, EventArgs e)
		{
			_treeView.UpdateView();
		}
	}
}
