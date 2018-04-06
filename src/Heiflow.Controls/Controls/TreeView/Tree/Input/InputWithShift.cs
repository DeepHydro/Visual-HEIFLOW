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
