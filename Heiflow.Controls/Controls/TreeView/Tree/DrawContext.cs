// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Heiflow.Controls.Tree.NodeControls;

namespace Heiflow.Controls.Tree
{
	public struct DrawContext
	{
		private Graphics _graphics;
		public Graphics Graphics
		{
			get { return _graphics; }
			set { _graphics = value; }
		}

		private Rectangle _bounds;
		public Rectangle Bounds
		{
			get { return _bounds; }
			set { _bounds = value; }
		}

		private Font _font;
		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		private DrawSelectionMode _drawSelection;
		public DrawSelectionMode DrawSelection
		{
			get { return _drawSelection; }
			set { _drawSelection = value; }
		}

		private bool _drawFocus;
		public bool DrawFocus
		{
			get { return _drawFocus; }
			set { _drawFocus = value; }
		}

		private NodeControl _currentEditorOwner;
		public NodeControl CurrentEditorOwner
		{
			get { return _currentEditorOwner; }
			set { _currentEditorOwner = value; }
		}

		private bool _enabled;
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}
	}
}
