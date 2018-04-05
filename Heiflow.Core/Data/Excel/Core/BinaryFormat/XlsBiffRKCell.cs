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

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents an RK number cell
	/// </summary>
	internal class XlsBiffRKCell : XlsBiffBlankCell
	{
		internal XlsBiffRKCell(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Returns value of this cell
		/// </summary>
		public double Value
		{
			get { return NumFromRK(base.ReadUInt32(0x6)); }
		}

		/// <summary>
		/// Decodes RK-encoded number
		/// </summary>
		/// <param name="rk">Encoded number</param>
		/// <returns></returns>
		public static double NumFromRK(uint rk)
		{
			double num;
			if ((rk & 0x2) == 0x2)
			{
                num = (int)(rk >> 2 | ((rk & 0x80000000) == 0 ? 0 : 0xC0000000));
			}
			else
			{
				// hi words of IEEE num
				num = Helpers.Int64BitsToDouble(((long)(rk & 0xfffffffc) << 32));
			}
			if ((rk & 0x1) == 0x1)
				num /= 100; // divide by 100

			return num;
		}
	}
}