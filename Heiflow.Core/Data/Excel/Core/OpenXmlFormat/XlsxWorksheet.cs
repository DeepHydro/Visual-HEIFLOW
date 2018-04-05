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
using System.Collections.Generic;
using System.Text;

namespace Excel.Core.OpenXmlFormat
{
	internal class XlsxWorksheet
	{
		public const string N_dimension = "dimension";
		public const string N_worksheet = "worksheet";
		public const string N_row = "row";
		public const string N_col = "col";
		public const string N_c = "c"; //cell
		public const string N_v = "v";
		public const string N_t = "t";
		public const string A_ref = "ref";
		public const string A_r = "r";
		public const string A_t = "t";
		public const string A_s = "s";
		public const string N_sheetData = "sheetData";
		public const string N_inlineStr = "inlineStr";

		private XlsxDimension _dimension;

		public bool IsEmpty { get; set; }

		public XlsxDimension Dimension
		{
			get { return _dimension; }
			set { _dimension = value; }
		}

		public int ColumnsCount
		{
			get
			{
				return IsEmpty ? 0 : (_dimension == null ? -1 : _dimension.LastCol);
			}
		}

		public int RowsCount
		{
			get
			{
				return _dimension == null ? -1 : _dimension.LastRow - _dimension.FirstRow + 1;
			}
		}

		private string _Name;

		public string Name
		{
			get { return _Name; }
		}

		private int _id;

		public int Id
		{
			get { return _id; }
		}

		private string _rid;

		public string RID
		{
			get
			{
				return _rid;
			}
			set
			{
				_rid = value;
			}
		}

		private string _path;
		

		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}

		public XlsxWorksheet(string name, int id, string rid)
		{
			_Name = name;
			_id = id;
			_rid = rid;
		}

	}
}
