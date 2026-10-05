///
///    This file is part of ILNumerics Community Edition.
///
///    ILNumerics Community Edition - high performance computing for applications.
///    Copyright (C) 2006 - 2013 Haymo Kutschbach, http://ilnumerics.net
///
///    ILNumerics Community Edition is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License version 3 as published by
///    the Free Software Foundation.
///
///    ILNumerics Community Edition is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with ILNumerics Community Edition. See the file License.txt in the root
///    of your distribution package. If not, see <http://www.gnu.org/licenses/>.
///
///    In addition this software uses the following components and/or licenses: 
///
///    =================================================================================
///    The Open Toolkit Library License
///    
///    Copyright (c) 2006 - 2009 the Open Toolkit library.
///    
///    Permission is hereby granted, free of charge, to any person obtaining a copy
///    of this software and associated documentation files (the "Software"), to deal
///    in the Software without restriction, including without limitation the rights to 
///    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
///    the Software, and to permit persons to whom the Software is furnished to do
///    so, subject to the following conditions:
///
///    The above copyright notice and this permission notice shall be included in all
///    copies or substantial portions of the Software.
///
///    =================================================================================
///    Intel® Math Kernel Library 11.1 for Windows
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Intel® Math Kernel Library 10.3 for Linux
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Products / Software which is implicitly used by ILNumerics due to the inclusion 
///    of 3rd party components: 
///  
///        BLAS/ LAPACK; see: http://netlib.org
///        FFT Functions; see: http://www.spiral.net, http://fftw.org
///        OpenGL; see: http://opengl.org
///
///    =================================================================================


using System;
using System.Collections.Generic;
using System.Text;
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging; 

namespace ILNumerics  {
    public partial class ILMath {

        /// <summary>
        /// Load single array from matfile file
        /// </summary>
        /// <typeparam name="T">Element type of the array to return</typeparam>
        /// <param name="filename">Path of the matfile on disk</param>
        /// <param name="arrayname">[Optional] name of the requested array in the matfile (default: empty string)</param>
        /// <returns>The array requested</returns>
        /// <remarks><para>If <paramref name="arrayname"/> is ommited, the first array is returned. </para></remarks>
        public static ILRetArray<T> loadArray<T>(string filename, string arrayname = "") {
            using (ILScope.Enter()) {
                using (ILMatFile matfile = new ILMatFile(filename)) {
                    if (String.IsNullOrEmpty(arrayname) && matfile.Count > 0) {
                        return matfile.GetArray<T>(0);
                    } else {
                        return matfile.GetArray<T>(arrayname); 
                    }
                }
            }
        }
        public static ILRetArray<int> loadImage(string filename, Rectangle? rect = null) {
            using (var fs = File.OpenRead(filename)) {
                return loadImage(fs);
            }
        }
        public static ILRetArray<int> loadImage(Stream inputStream, Rectangle? rect = null) {
            using (Bitmap bmp = new Bitmap(inputStream)) {
                return loadImage(bmp);
            }
        }
        public unsafe static ILRetArray<int> loadImage(Image image, Rectangle? rect = null) {
            using (ILScope.Enter()) {
                // create transposed: bmp stores row major order
                ILArray<int> ret = zeros<int>(image.Size.Width, image.Size.Height);
                Bitmap bmp = new Bitmap(image);
                if (!rect.HasValue) rect = new Rectangle(new Point(), image.Size);
                BitmapData data = bmp.LockBits(rect.GetValueOrDefault(), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                fixed (int* retArr = ret.GetArrayForWrite()) {
                    int pos = ret.S.SequentialIndexDistance(2);
                    int* dataP = (int*)data.Scan0;
                    int* retPA = retArr;
                    while (pos > 8) {
                        retPA[0] = dataP[0];
                        retPA[1] = dataP[1];
                        retPA[2] = dataP[2];
                        retPA[3] = dataP[3];
                        retPA[4] = dataP[4];
                        retPA[5] = dataP[5];
                        retPA[6] = dataP[6];
                        retPA[7] = dataP[7];
                        pos -= 8;
                        dataP += 8;
                        retPA += 8;
                    }
                    while (pos > 0) {
                        retPA[0] = dataP[0];
                        pos--;
                        dataP += 1;
                        retPA += 1;
                    }
                }
                return ret.T;
            }
        }
        public static ILRetArray<byte> loadChannels(string filename, Rectangle? rect = null) {
            using (var fs = File.OpenRead(filename)) {
                return loadChannels(fs);
            }
        }
        public static ILRetArray<byte> loadChannels(Stream inputStream, Rectangle? rect = null) {
            using (Bitmap bmp = new Bitmap(inputStream)) {
                return loadChannels(bmp);
            }
        }
        public unsafe static ILRetArray<byte> loadChannels(Image image, Rectangle? rect = null) {
            using (ILScope.Enter()) {
                if (!rect.HasValue) rect = new Rectangle(new Point(), image.Size);
                // create transposed: bmp stores row major order
                ILArray<byte> ret = zeros<byte>(rect.GetValueOrDefault().Size.Width, rect.GetValueOrDefault().Size.Height, 4);
                Bitmap bmp = new Bitmap(image);
                BitmapData data = bmp.LockBits(rect.GetValueOrDefault(), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                fixed (byte* retArr = ret.GetArrayForWrite()) {
                    int pixpos = ret.S.SequentialIndexDistance(2);
                    byte* dataP = (byte*)data.Scan0;
                    byte* retPA = retArr;
                    byte* retPR = retArr + ret.S.SequentialIndexDistance(2);
                    byte* retPG = retArr + ret.S.SequentialIndexDistance(2) * 2;
                    byte* retPB = retArr + ret.S.SequentialIndexDistance(2) * 3;
                    //while (pixpos > 8) {
                    //    retPA[0] = dataP[0];
                    //    retPA[1] = dataP[1];
                    //    retPA[2] = dataP[2];
                    //    retPA[3] = dataP[3];
                    //    retPA[4] = dataP[4];
                    //    retPA[5] = dataP[5];
                    //    retPA[6] = dataP[6];
                    //    retPA[7] = dataP[7];
                    //    pixpos -= 8;
                    //    dataP += 8;
                    //    retPA += 8;
                    //}
                    while (pixpos > 0) {
                        retPA[0] = dataP[0];
                        retPR[0] = dataP[1];
                        retPG[0] = dataP[2];
                        retPB[0] = dataP[3];
                        pixpos--;
                        dataP += 4;
                        retPA += 1;
                        retPR += 1;
                        retPG += 1;
                        retPB += 1;
                    }
                }
                return ret;
            }
        }

        public static ILRetArray<T> loadBinary<T>(Stream stream, int leadDimLen, int height, int width, int offsetWidth = 0, int offsetHeight = 0, Action<Array, int, Array, int, int> convertScanLine = null) where T : struct {
            using (ILScope.Enter()) {
                int sizeofT = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)); 
                ILArray<T> ret = ILMath.zeros<T>(height,width); 
                T[] retArr = ret.GetArrayForWrite();

                if (convertScanLine == null) {
                    convertScanLine = Buffer.BlockCopy; 
                }

                byte[] buffer = new byte[height * sizeofT]; 
                int curRetArrPos = 0;
                int curBufLen; 
                for (int c = 0; c < width; c++) {
                    long pos = (leadDimLen * (long)(c + offsetWidth) + offsetHeight) * sizeofT; 
                    stream.Seek(pos, SeekOrigin.Begin);
                    curBufLen = stream.Read(buffer, 0, buffer.Length);
                    if (curBufLen != buffer.Length)
                        throw new InvalidDataException("Could not read the file. Unexpected EOF detected. Invalid dimensions specified?"); 
                    convertScanLine(buffer, 0, retArr, curRetArrPos, curBufLen);
                    curRetArrPos += buffer.Length; 
                } 
                return ret; 
            }
        }

    }
}
