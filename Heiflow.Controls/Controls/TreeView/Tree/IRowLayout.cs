// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Heiflow.Controls.Tree
{
	internal interface IRowLayout
	{
		int PreferredRowHeight
		{
			get;
			set;
		}

		int PageRowCount
		{
			get;
		}

		int CurrentPageSize
		{
			get;
		}

		Rectangle GetRowBounds(int rowNo);

		int GetRowAt(Point point);

		int GetFirstRow(int lastPageRow);

		void ClearCache();
	}
}
