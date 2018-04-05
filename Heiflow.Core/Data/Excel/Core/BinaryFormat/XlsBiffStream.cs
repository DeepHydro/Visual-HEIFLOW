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

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents a BIFF stream
	/// </summary>
	internal class XlsBiffStream : XlsStream
	{
		private readonly ExcelBinaryReader reader;
		private readonly byte[] bytes;
		private readonly int m_size;
		private int m_offset;

		public XlsBiffStream(XlsHeader hdr, uint streamStart, bool isMini, XlsRootDirectory rootDir, ExcelBinaryReader reader)
			: base(hdr, streamStart, isMini, rootDir)
		{
			this.reader = reader;
			bytes = base.ReadStream();
			m_size = bytes.Length;
			m_offset = 0;

		}

		/// <summary>
		/// Returns size of BIFF stream in bytes
		/// </summary>
		public int Size
		{
			get { return m_size; }
		}

		/// <summary>
		/// Returns current position in BIFF stream
		/// </summary>
		public int Position
		{
			get { return m_offset; }
		}

		//TODO:Remove ReadStream
		/// <summary>
		/// Always returns null, use biff-specific methods
		/// </summary>
		/// <returns></returns>
		[Obsolete("Use BIFF-specific methods for this stream")]
		public new byte[] ReadStream()
		{
			return bytes;
		}

		/// <summary>
		/// Sets stream pointer to the specified offset
		/// </summary>
		/// <param name="offset">Offset value</param>
		/// <param name="origin">Offset origin</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Seek(int offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					m_offset = offset;
					break;
				case SeekOrigin.Current:
					m_offset += offset;
					break;
				case SeekOrigin.End:
					m_offset = m_size - offset;
					break;
			}
			if (m_offset < 0)
				throw new ArgumentOutOfRangeException(string.Format("{0} On offset={1}", Errors.ErrorBIFFIlegalBefore, offset));
			if (m_offset > m_size)
				throw new ArgumentOutOfRangeException(string.Format("{0} On offset={1}", Errors.ErrorBIFFIlegalAfter, offset));
		}

		/// <summary>
		/// Reads record under cursor and advances cursor position to next record
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public XlsBiffRecord Read()
		{
            if ((uint)m_offset >= bytes.Length)
                return null;

			XlsBiffRecord rec = XlsBiffRecord.GetRecord(bytes, (uint)m_offset, reader);
			m_offset += rec.Size;
			if (m_offset > m_size)
				return null;
			return rec;
		}

		/// <summary>
		/// Reads record at specified offset, does not change cursor position
		/// </summary>
		/// <param name="offset"></param>
		/// <returns></returns>
		public XlsBiffRecord ReadAt(int offset)
		{
            if ((uint)offset >= bytes.Length)
                return null;

			XlsBiffRecord rec = XlsBiffRecord.GetRecord(bytes, (uint)offset, reader);

			//choose ReadOption.Loose to skip this check (e.g. sql reporting services)
			if (reader.ReadOption == ReadOption.Strict)
			{
				if (m_offset + rec.Size > m_size)
					return null;
			}
			
			return rec;
		}
	}
}
