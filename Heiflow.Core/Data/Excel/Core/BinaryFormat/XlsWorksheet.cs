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

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents Worksheet section in workbook
	/// </summary>
	internal class XlsWorksheet
	{
		private readonly uint m_dataOffset;
		private readonly int m_Index;
		private readonly string m_Name = string.Empty;
		private XlsBiffSimpleValueRecord m_CalcCount;
		private XlsBiffSimpleValueRecord m_CalcMode;
		private XlsBiffRecord m_Delta;
		private XlsBiffDimensions m_Dimensions;
		private XlsBiffSimpleValueRecord m_Iteration;
		private XlsBiffSimpleValueRecord m_RefMode;
		private XlsBiffRecord m_Window;

		public XlsWorksheet(int index, XlsBiffBoundSheet refSheet)
		{
			m_Index = index;
			m_Name = refSheet.SheetName;
			m_dataOffset = refSheet.StartOffset;
		}

		/// <summary>
		/// Name of worksheet
		/// </summary>
		public string Name
		{
			get { return m_Name; }
		}

		/// <summary>
		/// Zero-based index of worksheet
		/// </summary>
		public int Index
		{
			get { return m_Index; }
		}

		/// <summary>
		/// Offset of worksheet data
		/// </summary>
		public uint DataOffset
		{
			get { return m_dataOffset; }
		}

		public XlsBiffSimpleValueRecord CalcMode
		{
			get { return m_CalcMode; }
			set { m_CalcMode = value; }
		}

		public XlsBiffSimpleValueRecord CalcCount
		{
			get { return m_CalcCount; }
			set { m_CalcCount = value; }
		}

		public XlsBiffSimpleValueRecord RefMode
		{
			get { return m_RefMode; }
			set { m_RefMode = value; }
		}

		public XlsBiffSimpleValueRecord Iteration
		{
			get { return m_Iteration; }
			set { m_Iteration = value; }
		}

		public XlsBiffRecord Delta
		{
			get { return m_Delta; }
			set { m_Delta = value; }
		}

		/// <summary>
		/// Dimensions of worksheet
		/// </summary>
		public XlsBiffDimensions Dimensions
		{
			get { return m_Dimensions; }
			set { m_Dimensions = value; }
		}

		public XlsBiffRecord Window
		{
			get { return m_Window; }
			set { m_Window = value; }
		}

	}
}