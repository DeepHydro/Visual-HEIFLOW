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

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents BIFF BOF record
	/// </summary>
	internal class XlsBiffBOF : XlsBiffRecord
	{
		internal XlsBiffBOF(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Version
		/// </summary>
		public ushort Version
		{
			get { return ReadUInt16(0x0); }
		}

		/// <summary>
		/// Type of BIFF block
		/// </summary>
		public BIFFTYPE Type
		{
			get { return (BIFFTYPE) ReadUInt16(0x2); }
		}

		/// <summary>
		/// Creation ID
		/// </summary>
		/// <remarks>Not used before BIFF5</remarks>
		public ushort CreationID
		{
			get
			{
				if (RecordSize < 6) return 0;
				return ReadUInt16(0x4);
			}
		}

		/// <summary>
		/// Creation year
		/// </summary>
		/// <remarks>Not used before BIFF5</remarks>
		public ushort CreationYear
		{
			get
			{
				if (RecordSize < 8) return 0;
				return ReadUInt16(0x6);
			}
		}

		/// <summary>
		/// File history flag
		/// </summary>
		/// <remarks>Not used before BIFF8</remarks>
		public uint HistoryFlag
		{
			get
			{
				if (RecordSize < 12) return 0;
				return ReadUInt32(0x8);
			}
		}

		/// <summary>
		/// Minimum Excel version to open this file
		/// </summary>
		/// <remarks>Not used before BIFF8</remarks>
		public uint MinVersionToOpen
		{
			get
			{
				if (RecordSize < 16) return 0;
				return ReadUInt32(0xC);
			}
		}
	}
}