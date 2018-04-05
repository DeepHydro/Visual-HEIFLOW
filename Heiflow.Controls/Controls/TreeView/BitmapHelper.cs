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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Heiflow.Controls
{
	public static class BitmapHelper
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct PixelData
		{
			public byte B;
			public byte G;
			public byte R;
			public byte A;
		}

		public static void SetAlphaChanelValue(Bitmap image, byte value)
		{
			if (image == null)
				throw new ArgumentNullException("image");
			if (image.PixelFormat != PixelFormat.Format32bppArgb)
				throw new ArgumentException("Wrong PixelFormat");

			BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
									 ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			unsafe
			{
				PixelData* pPixel = (PixelData*)bitmapData.Scan0;
				for (int i = 0; i < bitmapData.Height; i++)
				{
					for (int j = 0; j < bitmapData.Width; j++)
					{
						pPixel->A = value;
						pPixel++;
					}
					pPixel += bitmapData.Stride - (bitmapData.Width * 4);
				}
			}
			image.UnlockBits(bitmapData);
		}
	}
}
