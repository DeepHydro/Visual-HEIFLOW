// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Applications.Spatial
{
    public static class RasterEx
    {
        public static ISet<double> GetUniqueValues(IRaster raster, int maxCount, out bool overMaxCount)
        {
            overMaxCount = false;
            var result = new HashSet<double>();

            var totalPossibleCount = int.MaxValue;

            // Optimization for integer types
            if (raster.DataType == typeof(byte) ||
                raster.DataType == typeof(int) ||
                raster.DataType == typeof(sbyte) ||
                raster.DataType == typeof(uint) ||
                raster.DataType == typeof(short) ||
                raster.DataType == typeof(ushort))
            {
                totalPossibleCount = (int)(raster.Maximum - raster.Minimum + 1);
            }

            // NumRows and NumColumns - virtual properties, so copy them local variables for faster access
            var numRows = raster.NumRows;
            var numCols = raster.NumColumns;
            var valueGrid = raster.Value;

            for (var row = 0; row < numRows; row++)
                for (var col = 0; col < numCols; col++)
                {
                    double val = valueGrid[row, col];
                    if (result.Add(val))
                    {
                        if (result.Count > maxCount)
                        {
                            overMaxCount = true;
                            goto fin;
                        }
                        if (result.Count == totalPossibleCount)
                            goto fin;
                    }
                }
        fin:
            return result;
        }
    }
}
