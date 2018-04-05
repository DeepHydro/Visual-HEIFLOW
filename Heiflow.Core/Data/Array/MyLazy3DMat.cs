// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
