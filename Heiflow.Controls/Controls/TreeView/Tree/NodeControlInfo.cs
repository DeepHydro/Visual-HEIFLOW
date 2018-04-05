// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using Heiflow.Controls.Tree.NodeControls;
using System.Drawing;

namespace Heiflow.Controls.Tree
{
	public struct NodeControlInfo
	{
		public static readonly NodeControlInfo Empty = new NodeControlInfo(null, Rectangle.Empty, null);

		private NodeControl _control;
		public NodeControl Control
		{
			get { return _control; }
		}

		private Rectangle _bounds;
		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		private TreeNodeAdv _node;
		public TreeNodeAdv Node
		{
			get { return _node; }
		}

		public NodeControlInfo(NodeControl control, Rectangle bounds, TreeNodeAdv node)
		{
			_control = control;
			_bounds = bounds;
			_node = node;
		}
	}
}
