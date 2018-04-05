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
using Heiflow.Controls.Tree.NodeControls;

namespace Heiflow.Controls.Tree
{
	public class TreeNodeAdvMouseEventArgs : MouseEventArgs
	{
		private TreeNodeAdv _node;
		public TreeNodeAdv Node
		{
			get { return _node; }
			internal set { _node = value; }
		}

		private NodeControl _control;
		public NodeControl Control
		{
			get { return _control; }
			internal set { _control = value; }
		}

		private Point _viewLocation;
		public Point ViewLocation
		{
			get { return _viewLocation; }
			internal set { _viewLocation = value; }
		}

		private Keys _modifierKeys;
		public Keys ModifierKeys
		{
			get { return _modifierKeys; }
			internal set { _modifierKeys = value; }
		}

		private bool _handled;
		public bool Handled
		{
			get { return _handled; }
			set { _handled = value; }
		}

		private Rectangle _controlBounds;
		public Rectangle ControlBounds
		{
			get { return _controlBounds; }
			internal set { _controlBounds = value; }
		}

		public TreeNodeAdvMouseEventArgs(MouseEventArgs args)
			: base(args.Button, args.Clicks, args.X, args.Y, args.Delta)
		{
		}
	}
}
