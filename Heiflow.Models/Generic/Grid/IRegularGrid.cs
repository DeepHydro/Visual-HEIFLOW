// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Generic
{
    public interface IRegularGrid:IGrid
    {
        int RowCount
        {
            set;
            get;
        }

        int ColumnCount
        {
            set;
            get;
        }


        int ActiveCellCount
        {
            set;
            get;
        }

        /// <summary>
        /// 3DMat[LayerCount,1,ActiveCellCount]
        /// </summary>
        MyVarient3DMat<float> Elevations
        {
            get;
            set;
        }
        /// <summary>
        /// 3DMat[ ActualLayerCount,RowCount, ColumnCount,]:
        /// </summary>
        MyVarient3DMat<float> IBound
        {
            get;
            set;
        }
        /// <summary>
        ///  mat[0][0][NCOL]. The cell width along rows. Read one value for each of the NCOL columns.
        /// </summary>
        MyVarient3DMat<float> DELR
        {
            get;
            set;
        }
        /// <summary>
        ///  mat[0][0][NROW]. the cell width along columns. Read one value for each of the NROW rows. 
        /// </summary>
        MyVarient3DMat<float> DELC
        {
            get;
            set;
        }
    }
}
