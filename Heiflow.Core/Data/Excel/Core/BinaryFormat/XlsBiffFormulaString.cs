// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Text;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents a string value of formula
	/// </summary>
	internal class XlsBiffFormulaString : XlsBiffRecord
	{
		private Encoding m_UseEncoding = Encoding.Default;
		private const int LEADING_BYTES_COUNT = 3;

		internal XlsBiffFormulaString(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Encoding used to deal with strings
		/// </summary>
		public Encoding UseEncoding
		{
			get { return m_UseEncoding; }
			set { m_UseEncoding = value; }
		}

		/// <summary>
		/// Length of the string
		/// </summary>
		public ushort Length
		{
			get { return base.ReadUInt16(0x0); }
		}

		/// <summary>
		/// String text
		/// </summary>
		public string Value
		{
			get
			{
				//is unicode?
				if (base.ReadUInt16(0x01) != 0)
				{
					return Encoding.Unicode.GetString(m_bytes, m_readoffset + LEADING_BYTES_COUNT, Length * 2);
				}

				return m_UseEncoding.GetString(m_bytes, m_readoffset + LEADING_BYTES_COUNT, Length);
			}
		}
	}
}