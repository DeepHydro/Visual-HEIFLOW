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
using System.Collections;

namespace Heiflow.Controls.Tree
{
	public class SortedTreeModel: TreeModelBase
	{
		private ITreeModel _innerModel;
		public ITreeModel InnerModel
		{
			get { return _innerModel; }
		}

		private IComparer _comparer;
		public IComparer Comparer
		{
			get { return _comparer; }
			set 
			{ 
				_comparer = value;
				OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
			}
		}

		public SortedTreeModel(ITreeModel innerModel)
		{
			_innerModel = innerModel;
			_innerModel.NodesChanged += new EventHandler<TreeModelEventArgs>(_innerModel_NodesChanged);
			_innerModel.NodesInserted += new EventHandler<TreeModelEventArgs>(_innerModel_NodesInserted);
			_innerModel.NodesRemoved += new EventHandler<TreeModelEventArgs>(_innerModel_NodesRemoved);
			_innerModel.StructureChanged += new EventHandler<TreePathEventArgs>(_innerModel_StructureChanged);
		}

		void _innerModel_StructureChanged(object sender, TreePathEventArgs e)
		{
			OnStructureChanged(e);
		}

		void _innerModel_NodesRemoved(object sender, TreeModelEventArgs e)
		{
			OnStructureChanged(new TreePathEventArgs(e.Path));
		}

		void _innerModel_NodesInserted(object sender, TreeModelEventArgs e)
		{
			OnStructureChanged(new TreePathEventArgs(e.Path));
		}

		void _innerModel_NodesChanged(object sender, TreeModelEventArgs e)
		{
			OnStructureChanged(new TreePathEventArgs(e.Path));
		}

		public override IEnumerable GetChildren(TreePath treePath)
		{
			if (Comparer != null)
			{
				ArrayList list = new ArrayList();
				IEnumerable res = InnerModel.GetChildren(treePath);
				if (res != null)
				{
					foreach (object obj in res)
						list.Add(obj);
					list.Sort(Comparer);
					return list;
				}
				else
					return null;
			}
			else
				return InnerModel.GetChildren(treePath);
		}

		public override bool IsLeaf(TreePath treePath)
		{
			return InnerModel.IsLeaf(treePath);
		}
	}
}
