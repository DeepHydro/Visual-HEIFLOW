// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Heiflow.Controls.Tree
{
	public class ListModel : TreeModelBase
	{
		private IList _list;

		public int Count
		{
			get { return _list.Count; }
		}

		public ListModel()
		{
			_list = new List<object>();
		}

		public ListModel(IList list)
		{
			_list = list;
		}

		public override IEnumerable GetChildren(TreePath treePath)
		{
			return _list;
		}

		public override bool IsLeaf(TreePath treePath)
		{
			return true;
		}

		public void AddRange(IEnumerable items)
		{
			foreach (object obj in items)
				_list.Add(obj);
			OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
		}

		public void Add(object item)
		{
			_list.Add(item);
			OnNodesInserted(new TreeModelEventArgs(TreePath.Empty, new int[] { _list.Count - 1 }, new object[] { item }));
		}

		public void Clear()
		{
			_list.Clear();
			OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
		}
	}
}
