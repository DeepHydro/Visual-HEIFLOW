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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Heiflow.Controls.Tree
{
	internal class ReorderColumnState : ColumnState
	{
		#region Properties

		private Point _location;
		public Point Location
		{
			get { return _location; }
		}

		private Bitmap _ghostImage;
		public Bitmap GhostImage
		{
			get { return _ghostImage; }
		}

		private TreeColumn _dropColumn;
		public TreeColumn DropColumn
		{
			get { return _dropColumn; }
		}

		private int _dragOffset;
		public int DragOffset
		{
			get { return _dragOffset; }
		}

		#endregion

		public ReorderColumnState(TreeViewAdv tree, TreeColumn column, Point initialMouseLocation)
			: base(tree, column)
		{
			_location = new Point(initialMouseLocation.X + Tree.OffsetX, 0);
			_dragOffset = tree.GetColumnX(column) - initialMouseLocation.X;
			_ghostImage = column.CreateGhostImage(new Rectangle(0, 0, column.Width, tree.ActualColumnHeaderHeight), tree.Font);
		}

		public override void KeyDown(KeyEventArgs args)
		{
			args.Handled = true;
			if (args.KeyCode == Keys.Escape)
				FinishResize();
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			FinishResize();
		}

		public override bool MouseMove(MouseEventArgs args)
		{
			_dropColumn = null;
			_location = new Point(args.X + Tree.OffsetX, 0);
			int x = 0;
			foreach (TreeColumn c in Tree.Columns)
			{
				if (c.IsVisible)
				{
					if (_location.X < x + c.Width / 2)
					{
						_dropColumn = c;
						break;
					}
					x += c.Width;
				}
			}
			Tree.UpdateHeaders();
			return true;
		}

		private void FinishResize()
		{
			Tree.ChangeInput();
			if (Column == DropColumn)
				Tree.UpdateView();
			else
			{
				Tree.Columns.Remove(Column);
				if (DropColumn == null)
					Tree.Columns.Add(Column);
				else
					Tree.Columns.Insert(Tree.Columns.IndexOf(DropColumn), Column);

				Tree.OnColumnReordered(Column);
			}
		}
	}
}