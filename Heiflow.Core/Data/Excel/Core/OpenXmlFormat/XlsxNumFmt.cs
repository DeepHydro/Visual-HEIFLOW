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
	internal class XlsxNumFmt
	{
		public const string N_numFmt = "numFmt";
		public const string A_numFmtId = "numFmtId";
		public const string A_formatCode = "formatCode";

		private int _Id;

		public int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		private string _FormatCode;

		public string FormatCode
		{
			get { return _FormatCode; }
			set { _FormatCode = value; }
		}

		public XlsxNumFmt(int id, string formatCode)
		{
			_Id = id;
			_FormatCode = formatCode;
		}
	}
}
