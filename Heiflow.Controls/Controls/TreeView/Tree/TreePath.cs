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
using System.Text;
using System.Collections.ObjectModel;

namespace Heiflow.Controls.Tree
{
	public class TreePath
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly TreePath Empty = new TreePath();

		private object[] _path;
		public object[] FullPath
		{
			get { return _path; }
		}

		public object LastNode
		{
			get
			{
				if (_path.Length > 0)
					return _path[_path.Length - 1];
				else
					return null;
			}
		}

		public object FirstNode
		{
			get
			{
				if (_path.Length > 0)
					return _path[0];
				else
					return null;
			}
		}

		public TreePath()
		{
			_path = new object[0];
		}

		public TreePath(object node)
		{
			_path = new object[] { node };
		}

		public TreePath(object[] path)
		{
			_path = path;
		}

		public TreePath(TreePath parent, object node)
		{
			_path = new object[parent.FullPath.Length + 1];
			for (int i = 0; i < _path.Length - 1; i++)
				_path[i] = parent.FullPath[i];
			_path[_path.Length - 1] = node;
		}

		public bool IsEmpty()
		{
			return (_path.Length == 0);
		}
	}
}
