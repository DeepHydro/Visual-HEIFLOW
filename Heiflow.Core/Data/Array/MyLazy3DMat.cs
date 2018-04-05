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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    /// <summary>
    ///  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyLazy3DMat<T> : My3DMat<T>
    {
        /// <summary>
        ///  Initially only the zero dimension is allocated.
        /// </summary>
        /// <param name="size"></param>
        public MyLazy3DMat(int size0, int size1, int size2)
        {
            Size = new int[] { size0, size1, size2 };
            Value = new T[Size[0]][][];

            InitFlags(Size[0], Size[1]);
            PopulateVariables();
            DataCubeType = Data.DataCubeType.General;
            TimeBrowsable = false;
            AllowTableEdit = false; 
        }

        public void Allocate(int var_index, int time_len, int space_len)
        {
            Value[var_index] = new T[time_len][];
            for(int i=0; i<time_len;i++)
            {
                Value[var_index][i] = new T[space_len];
            }
            Size[2] = space_len;
        }

        public void Allocate(int var_index)
        {
            Value[var_index] = new T[Size[1]][];
            for (int i = 0; i < Size[1]; i++)
            {
                Value[var_index][i] = new T[Size[2]];
            }
        }

        public void Allocate(int var_index, int time_len)
        {
            Value[var_index] = new T[time_len][];
        }

    }
}
