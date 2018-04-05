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
	internal class XlsxDimension
	{
		public XlsxDimension(string value)
		{
			ParseDimensions(value);
		}

		public XlsxDimension(int rows, int cols)
		{
			this.FirstRow = 1;
			this.LastRow = rows;
			this.FirstCol = 1;
			this.LastCol = cols;
		}

		private int _FirstRow;

		public int FirstRow
		{
			get { return _FirstRow; }
			set { _FirstRow = value; }
		}

		private int _LastRow;

		public int LastRow
		{
			get { return _LastRow; }
			set { _LastRow = value; }
		}

		private int _FirstCol;

		public int FirstCol
		{
			get { return _FirstCol; }
			set { _FirstCol = value; }
		}

		private int _LastCol;

		public int LastCol
		{
			get { return _LastCol; }
			set { _LastCol = value; }
		}

		public void ParseDimensions(string value)
		{
			string[] parts = value.Split(':');

			int col;
			int row;

			XlsxDim(parts[0], out col, out row);
			FirstCol = col;
			FirstRow = row;

			if (parts.Length == 1)
			{
				LastCol = FirstCol;
				LastRow = FirstRow;
			}
			else
			{
				XlsxDim(parts[1], out col, out row);
				LastCol = col;
				LastRow = row;
			}
			
		}


		/// <summary>
		/// Logic for the Excel dimensions. Ex: A15
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="val1">out val1.</param>
		/// <param name="val2">out val2.</param>
		public static void XlsxDim(string value, out int val1, out int val2)
		{//INFO: Check for a simple Solution
			int index = 0;
			val1 = 0;
			int[] arr = new int[value.Length - 1];

			while (index < value.Length)
			{
				if (char.IsDigit(value[index])) break;
				arr[index] = value[index] - 'A' + 1;
				index++;
			}

			for (int i = 0; i < index; i++)
			{
				val1 += (int)(arr[i] * Math.Pow(26, index - i - 1));
			}

			val2 = int.Parse(value.Substring(index));
		}
	}
}
