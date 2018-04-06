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
using System.Windows.Forms;
using System.Drawing;

namespace Heiflow.Controls.Tree
{
	internal class ClickColumnState : ColumnState
	{
		private Point _location;

		public ClickColumnState(TreeViewAdv tree, TreeColumn column, Point location)
			: base(tree, column)
		{
			_location = location;
		}

		public override void KeyDown(KeyEventArgs args)
		{
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
		}

		public override bool MouseMove(MouseEventArgs args)
		{
			if (TreeViewAdv.Dist(_location, args.Location) > TreeViewAdv.ItemDragSensivity
				&& Tree.AllowColumnReorder)
			{
				Tree.Input = new ReorderColumnState(Tree, Column, args.Location);
				Tree.UpdateView();
			}
			return true;
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			Tree.ChangeInput();
			Tree.UpdateView();
			Tree.OnColumnClicked(Column);
		}
	}
}
