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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Heiflow.Controls.Tree
{
	using System.Drawing;

	public class AutoHeaderHeightLayout : IHeaderLayout
	{
		DrawContext _measureContext;
		TreeViewAdv _treeView;

		public AutoHeaderHeightLayout(TreeViewAdv treeView, int headerHeight)
		{
			_treeView = treeView;
			PreferredHeaderHeight = headerHeight;
			_measureContext = new DrawContext();
			_measureContext.Graphics = Graphics.FromImage(new Bitmap(1, 1));
		}

		int? _headerHeight;

		bool _computed;

		#region Implementation of IHeaderLayout

		public int PreferredHeaderHeight
		{
			get { return GetHeaderHeight(); }
			set
			{
				_headerHeight = value;
				_computed = false;
			}
		}

		public void ClearCache()
		{
			_computed = false;
		}

		int GetHeaderHeight()
		{
			if (!_computed)
			{
				int res = 0;
				_measureContext.Font = _treeView.Font;
				foreach (TreeColumn nc in _treeView.Columns)
				{
					int h = nc.GetActualSize(_measureContext).Height;
					if (h > res) res = h;
				}
				_headerHeight = res;

				_computed = true;
			}

			return _headerHeight.Value;
		}

		#endregion
	}
}
