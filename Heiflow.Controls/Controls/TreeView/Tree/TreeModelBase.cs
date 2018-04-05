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
	public abstract class TreeModelBase: ITreeModel
	{
		public abstract System.Collections.IEnumerable GetChildren(TreePath treePath);
		public abstract bool IsLeaf(TreePath treePath);


		public event EventHandler<TreeModelEventArgs> NodesChanged;
		protected void OnNodesChanged(TreeModelEventArgs args)
		{
			if (NodesChanged != null)
				NodesChanged(this, args);
		}

		public event EventHandler<TreePathEventArgs> StructureChanged;
		protected void OnStructureChanged(TreePathEventArgs args)
		{
			if (StructureChanged != null)
				StructureChanged(this, args);
		}

		public event EventHandler<TreeModelEventArgs> NodesInserted;
		protected void OnNodesInserted(TreeModelEventArgs args)
		{
			if (NodesInserted != null)
				NodesInserted(this, args);
		}

		public event EventHandler<TreeModelEventArgs> NodesRemoved;
		protected void OnNodesRemoved(TreeModelEventArgs args)
		{
			if (NodesRemoved != null)
				NodesRemoved(this, args);
		}

		public virtual void Refresh()
		{
			OnStructureChanged(new TreePathEventArgs(TreePath.Empty));
		}
	}
}
