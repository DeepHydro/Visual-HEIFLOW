// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Heiflow.Controls.Tree
{
	public class TreeViewRowDrawEventArgs: PaintEventArgs
	{
		TreeNodeAdv _node;
		DrawContext _context;
		int _row;
		Rectangle _rowRect;

		public TreeViewRowDrawEventArgs(Graphics graphics, Rectangle clipRectangle, TreeNodeAdv node, DrawContext context, int row, Rectangle rowRect)
			: base(graphics, clipRectangle)
		{
			_node = node;
			_context = context;
			_row = row;
			_rowRect = rowRect;
		}

		public TreeNodeAdv Node
		{
			get { return _node; }
		}

		public DrawContext Context
		{
			get { return _context; }
		}

		public int Row
		{
			get { return _row; }
		}

		public Rectangle RowRect
		{
			get { return _rowRect; }
		}
	}

}
