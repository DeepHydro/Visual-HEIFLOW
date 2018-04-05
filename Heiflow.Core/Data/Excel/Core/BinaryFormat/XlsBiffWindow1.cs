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

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents Workbook's global window description
	/// </summary>
	internal class XlsBiffWindow1 : XlsBiffRecord
	{
		#region Window1Flags enum

		[Flags]
		public enum Window1Flags : ushort
		{
			Hidden = 0x1,
			Minimized = 0x2,
			//(Reserved) = 0x4,

			HScrollVisible = 0x8,
			VScrollVisible = 0x10,
			WorkbookTabs = 0x20
			//(Other bits are reserved)
		}

		#endregion

		internal XlsBiffWindow1(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}


		/// <summary>
		/// Returns X position of a window
		/// </summary>
		public ushort Left
		{
			get { return base.ReadUInt16(0x0); }
		}

		/// <summary>
		/// Returns Y position of a window
		/// </summary>
		public ushort Top
		{
			get { return base.ReadUInt16(0x2); }
		}

		/// <summary>
		/// Returns width of a window
		/// </summary>
		public ushort Width
		{
			get { return base.ReadUInt16(0x4); }
		}

		/// <summary>
		/// Returns height of a window
		/// </summary>
		public ushort Height
		{
			get { return base.ReadUInt16(0x6); }
		}

		/// <summary>
		/// Returns window flags
		/// </summary>
		public Window1Flags Flags
		{
			get { return (Window1Flags) base.ReadUInt16(0x8); }
		}

		/// <summary>
		/// Returns active workbook tab (zero-based)
		/// </summary>
		public ushort ActiveTab
		{
			get { return base.ReadUInt16(0xA); }
		}

		/// <summary>
		/// Returns first visible workbook tab (zero-based)
		/// </summary>
		public ushort FirstVisibleTab
		{
			get { return base.ReadUInt16(0xC); }
		}

		/// <summary>
		/// Returns number of selected workbook tabs
		/// </summary>
		public ushort SelectedTabCount
		{
			get { return base.ReadUInt16(0xE); }
		}

		/// <summary>
		/// Returns workbook tab width to horizontal scrollbar width
		/// </summary>
		public ushort TabRatio
		{
			get { return base.ReadUInt16(0x10); }
		}
	}
}