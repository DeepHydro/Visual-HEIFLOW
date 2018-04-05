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
	public interface ITreeModel
	{
		IEnumerable GetChildren(TreePath treePath);
		bool IsLeaf(TreePath treePath);

		event EventHandler<TreeModelEventArgs> NodesChanged; 
		event EventHandler<TreeModelEventArgs> NodesInserted;
		event EventHandler<TreeModelEventArgs> NodesRemoved; 
		event EventHandler<TreePathEventArgs> StructureChanged;
	}
}
