// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Heiflow.Controls.Tree
{
	internal class FixedRowHeightLayout : IRowLayout
	{
		private TreeViewAdv _treeView;

		public FixedRowHeightLayout(TreeViewAdv treeView, int rowHeight)
		{
			_treeView = treeView;
			PreferredRowHeight = rowHeight;
		}

		private int _rowHeight;
		public int PreferredRowHeight
		{
			get { return _rowHeight; }
			set { _rowHeight = value; }
		}

		public Rectangle GetRowBounds(int rowNo)
		{
			return new Rectangle(0, rowNo * _rowHeight, 0, _rowHeight);
		}

		public int PageRowCount
		{
			get
			{
				return Math.Max((_treeView.DisplayRectangle.Height - _treeView.ActualColumnHeaderHeight) / _rowHeight, 0);
			}
		}

		public int CurrentPageSize
		{
			get
			{
				return PageRowCount;
			}
		}

		public int GetRowAt(Point point)
		{
			point = new Point(point.X, point.Y + (_treeView.FirstVisibleRow * _rowHeight) - _treeView.ActualColumnHeaderHeight);
			return point.Y / _rowHeight;
		}

		public int GetFirstRow(int lastPageRow)
		{
			return Math.Max(0, lastPageRow - PageRowCount + 1);
		}

		public void ClearCache()
		{
		}
	}
}
