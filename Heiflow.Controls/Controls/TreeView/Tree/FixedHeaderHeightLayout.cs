// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Heiflow.Controls.Tree
{
	class FixedHeaderHeightLayout : IHeaderLayout
	{
		TreeViewAdv _treeView;
		int _headerHeight;

		public FixedHeaderHeightLayout(TreeViewAdv treeView, int headerHeight)
		{
			_treeView = treeView;
			PreferredHeaderHeight = headerHeight;
		}

		#region Implementation of IHeaderLayout

		public int PreferredHeaderHeight
		{
			get { return _headerHeight; }
			set { _headerHeight = value; }
		}

		public void ClearCache()
		{
		}

		#endregion
	}
}
