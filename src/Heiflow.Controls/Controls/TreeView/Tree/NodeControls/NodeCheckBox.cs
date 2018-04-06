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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;
using Heiflow.Controls.WinForm.Properties;

namespace Heiflow.Controls.Tree.NodeControls
{
	public class NodeCheckBox : InteractiveControl
	{
		public const int ImageSize = 13;

		private Bitmap _check;
		private Bitmap _uncheck;
		private Bitmap _unknown;

		#region Properties

		private bool _threeState;
		[DefaultValue(false)]
		public bool ThreeState
		{
			get { return _threeState; }
			set { _threeState = value; }
		}

		bool _reverseCheckOrder;
		/// <summary>
		/// Reverses the Order. 
		/// From Indeterminate, Unchecked, Checked to Indeterminate, Checked, Unchecked.
		/// </summary>
		[DefaultValue(false)]
		public bool ReverseCheckOrder
		{
			get { return _reverseCheckOrder; }
			set { _reverseCheckOrder = value; }
		}

		#endregion

		public NodeCheckBox()
			: this(string.Empty)
		{
		}

		public NodeCheckBox(string propertyName)
		{
			_check = Resources.check;
			_uncheck = Resources.uncheck;
			_unknown = Resources.unknown;
			DataPropertyName = propertyName;
			LeftMargin = 0;
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return new Size(ImageSize, ImageSize);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			Rectangle bounds = GetBounds(node, context);
			CheckState state = GetCheckState(node);
			if (Application.RenderWithVisualStyles)
			{
				VisualStyleRenderer renderer;
				if (state == CheckState.Indeterminate)
					renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.MixedNormal);
				else if (state == CheckState.Checked)
					renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.CheckedNormal);
				else
					renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedNormal);
				renderer.DrawBackground(context.Graphics, new Rectangle(bounds.X, bounds.Y, ImageSize, ImageSize));
			}
			else
			{
				Image img;
				if (state == CheckState.Indeterminate)
					img = _unknown;
				else if (state == CheckState.Checked)
					img = _check;
				else
					img = _uncheck;
				context.Graphics.DrawImage(img, bounds.Location);
			}
		}

		protected virtual CheckState GetCheckState(TreeNodeAdv node)
		{
			object obj = GetValue(node);
			if (obj is CheckState)
				return (CheckState)obj;
			else if (obj is bool)
				return (bool)obj ? CheckState.Checked : CheckState.Unchecked;
			else
				return CheckState.Unchecked;
		}

		protected virtual void SetCheckState(TreeNodeAdv node, CheckState value)
		{
			if (VirtualMode)
			{
				SetValue(node, value);
				OnCheckStateChanged(node);
			}
			else
			{
				Type type = GetPropertyType(node);
				if (type == typeof(CheckState))
				{
					SetValue(node, value);
					OnCheckStateChanged(node);
				}
				else if (type == typeof(bool))
				{
					SetValue(node, value != CheckState.Unchecked);
					OnCheckStateChanged(node);
				}
			}
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			if (args.Button == MouseButtons.Left && IsEditEnabled(args.Node))
			{
				DrawContext context = new DrawContext();
				context.Bounds = args.ControlBounds;
				Rectangle rect = GetBounds(args.Node, context);
				if (rect.Contains(args.ViewLocation))
				{
					CheckState state = GetCheckState(args.Node);
					state = GetNewState(state);
					SetCheckState(args.Node, state);
					Parent.UpdateView();
					args.Handled = true;
				}
			}
		}

		public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			args.Handled = true;
		}

		protected virtual CheckState GetNewState(CheckState state)
		{
			if (ReverseCheckOrder)
			{
				if (state == CheckState.Indeterminate)
					return CheckState.Checked;
				else if (state == CheckState.Checked)
					return CheckState.Unchecked;
				else
					return ThreeState ? CheckState.Indeterminate : CheckState.Checked;
			}
			else
			{
				if (state == CheckState.Indeterminate)
					return CheckState.Unchecked;
				else if (state == CheckState.Unchecked)
					return CheckState.Checked;
				else
					return ThreeState ? CheckState.Indeterminate : CheckState.Unchecked;
			}
		}

		public override void KeyDown(KeyEventArgs args)
		{
			if (args.KeyCode == Keys.Space && EditEnabled)
			{
				Parent.BeginUpdate();
				try
				{
					if (Parent.CurrentNode != null)
					{
						CheckState value = GetNewState(GetCheckState(Parent.CurrentNode));
						foreach (TreeNodeAdv node in Parent.Selection)
							if (IsEditEnabled(node))
								SetCheckState(node, value);
					}
				}
				finally
				{
					Parent.EndUpdate();
				}
				args.Handled = true;
			}
		}

		public event EventHandler<TreePathEventArgs> CheckStateChanged;
		protected void OnCheckStateChanged(TreePathEventArgs args)
		{
			if (CheckStateChanged != null)
				CheckStateChanged(this, args);
		}

		protected void OnCheckStateChanged(TreeNodeAdv node)
		{
			TreePath path = this.Parent.GetPath(node);
			OnCheckStateChanged(new TreePathEventArgs(path));
		}

	}
}
