// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Windows.Forms;
namespace Heiflow.Controls.Tree
{
	internal abstract class InputState
	{
		private TreeViewAdv _tree;

		public TreeViewAdv Tree
		{
			get { return _tree; }
		}

		public InputState(TreeViewAdv tree)
		{
			_tree = tree;
		}

		public abstract void KeyDown(System.Windows.Forms.KeyEventArgs args);
		public abstract void MouseDown(TreeNodeAdvMouseEventArgs args);
		public abstract void MouseUp(TreeNodeAdvMouseEventArgs args);

		/// <summary>
		/// handle OnMouseMove event
		/// </summary>
		/// <param name="args"></param>
		/// <returns>true if event was handled and should be dispatched</returns>
		public virtual bool MouseMove(MouseEventArgs args)
		{
			return false;
		}

		public virtual void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
		}
	}
}
