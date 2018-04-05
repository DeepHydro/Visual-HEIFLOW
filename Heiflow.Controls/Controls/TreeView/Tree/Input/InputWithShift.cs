// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;

namespace Heiflow.Controls.Tree
{
	internal class InputWithShift: NormalInputState
	{
		public InputWithShift(TreeViewAdv tree): base(tree)
		{
		}

		protected override void FocusRow(TreeNodeAdv node)
		{
			Tree.SuspendSelectionEvent = true;
			try
			{
				if (Tree.SelectionMode == TreeSelectionMode.Single || Tree.SelectionStart == null)
					base.FocusRow(node);
				else if (CanSelect(node))
				{
					SelectAllFromStart(node);
					Tree.CurrentNode = node;
					Tree.ScrollTo(node);
				}
			}
			finally
			{
				Tree.SuspendSelectionEvent = false;
			}
		}

		protected override void DoMouseOperation(TreeNodeAdvMouseEventArgs args)
		{
			if (Tree.SelectionMode == TreeSelectionMode.Single || Tree.SelectionStart == null)
			{
				base.DoMouseOperation(args);
			}
			else if (CanSelect(args.Node))
			{
				Tree.SuspendSelectionEvent = true;
				try
				{
					SelectAllFromStart(args.Node);
				}
				finally
				{
					Tree.SuspendSelectionEvent = false;
				}
			}
		}

		protected override void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
		}

		private void SelectAllFromStart(TreeNodeAdv node)
		{
			Tree.ClearSelectionInternal();
			int a = node.Row;
			int b = Tree.SelectionStart.Row;
			for (int i = Math.Min(a, b); i <= Math.Max(a, b); i++)
			{
				if (Tree.SelectionMode == TreeSelectionMode.Multi || Tree.RowMap[i].Parent == node.Parent)
					Tree.RowMap[i].IsSelected = true;
			}
		}
	}
}
