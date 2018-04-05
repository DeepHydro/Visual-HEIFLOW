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
    public interface IGridTopology
    {
        int RowCount { get; }
        int ColumnCount { get; }
        int ActiveCellCount { get; }
        /// <summary>
        /// a 2d mat[num_act_cell][2]. the second dimension stores row and column indexes that start from 0
        /// </summary>
        int[][] ActiveCell { get;  }
        /// <summary>
        /// hold active cell id starting from 1
        /// </summary>
        int[] ActiveCellIDs { get; }
        /// <summary>
        /// mapping between Cell ID  (starts from 1) and Cell Serial Index (starts from 0)
        /// </summary>
        Dictionary<int, int> CellID2CellIndex { get;}
    }
}
