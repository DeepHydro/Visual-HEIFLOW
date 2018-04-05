// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Generic
{
    public interface ICell
    {
        /// <summary>
        /// The unique ID of the cell
        /// </summary>
         int ID { get; set; }
        /// <summary>
        /// Column Index starting from 0
        /// </summary>
         int I { get; set; }
        /// <summary>
         /// Row index  starting from 0
        /// </summary>
         int J { get; set; }
         int SerialIndex { get; set; }
         double CurrentValue { get; set; }

         /// <summary>
         /// Central longitude in degree
         /// </summary>
         double CentralLongitude { get; set; }
         /// <summary>
         /// Central latitude  in degree
         /// </summary>
         double CentralLatitude { get; set; }
    }
}
