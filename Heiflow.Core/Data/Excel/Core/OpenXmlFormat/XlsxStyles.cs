// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;

namespace Excel.Core.OpenXmlFormat
{
	internal class XlsxStyles
	{
		public XlsxStyles()
		{
			_cellXfs = new List<XlsxXf>();
			_NumFmts = new List<XlsxNumFmt>();
		}

		private List<XlsxXf> _cellXfs;

		public List<XlsxXf> CellXfs
		{
			get { return _cellXfs; }
			set { _cellXfs = value; }
		}

		private List<XlsxNumFmt> _NumFmts;

		public List<XlsxNumFmt> NumFmts
		{
			get { return _NumFmts; }
			set { _NumFmts = value; }
		}
	}
}
