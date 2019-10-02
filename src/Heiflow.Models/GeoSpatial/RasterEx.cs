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

using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.GeoSpatial
{
    public class RasterEx
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
                    if (val != raster.NoDataValue)
                    {
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
                }
        fin:
            return result;
        }

        /// <summary>
        /// Executes the slope generation raster.
        /// </summary>
        /// <param name="raster">The input altitude raster.</param>
        /// <param name="inZFactor">The double precision multiplicative scaling factor for elevation values.</param>
        /// <param name="slopeInPercent">A boolean parameter that clarifies the nature of the slope values.  If this is true, the values represent percent slope.</param>
        /// <param name="cancelProgressHandler">The progress handler.</param>
        /// <returns>The output slope raster, or null if the process was unsuccessful.</returns>
        public static IRaster GetSlope(IRaster raster, double inZFactor, bool slopeInPercent, ICancelProgressHandler cancelProgressHandler)
        {
            // Validates the input and output data
            if (raster == null)
            {
                return null;
            }

            int noOfCol = raster.NumColumns;
            int noOfRow = raster.NumRows;

            // Create the new raster with the appropriate dimensions
            var result = Raster.CreateRaster("SlopeRaster.bgd", string.Empty, noOfCol, noOfRow, 1, typeof(double), new[] { string.Empty });
            result.NoDataValue = raster.NoDataValue;
            result.Bounds = raster.Bounds;
            result.Projection = raster.Projection;

            ProgressMeter progMeter = null;
            try
            {
                if (cancelProgressHandler != null) progMeter = new ProgressMeter(cancelProgressHandler, "Calculating Slope", result.NumRows);

                // Cache cell size for faster access
                var cellWidth = raster.CellWidth;
                var cellHeight = raster.CellHeight;
                for (int i = 0; i < result.NumRows; i++)
                {
                    if (cancelProgressHandler != null)
                    {
                        progMeter.Next();
                        if ((i % 100) == 0)
                        {
                            progMeter.SendProgress();
  
                        }
                    }
                    for (int j = 0; j < result.NumColumns; j++)
                    {
                        if (i > 0 && i < result.NumRows - 1 && j > 0 && j < result.NumColumns - 1)
                        {
                            double z1 = raster.Value[i - 1, j - 1];
                            double z2 = raster.Value[i - 1, j];
                            double z3 = raster.Value[i - 1, j + 1];
                            double z4 = raster.Value[i, j - 1];
                            double z5 = raster.Value[i, j + 1];
                            double z6 = raster.Value[i + 1, j - 1];
                            double z7 = raster.Value[i + 1, j];
                            double z8 = raster.Value[i + 1, j + 1];

                            // 3rd Order Finite Difference slope algorithm
                            double dZdX = inZFactor * ((z3 - z1) + (2 * (z5 - z4)) + (z8 - z6)) / (8 * cellWidth);
                            double dZdY = inZFactor * ((z1 - z6) + (2 * (z2 - z7)) + (z3 - z8)) / (8 * cellHeight);

                            double slope = Math.Atan(Math.Sqrt((dZdX * dZdX) + (dZdY * dZdY))) * (180 / Math.PI);

                            // change to radius and in percentage
                            if (slopeInPercent)
                            {
                                slope = Math.Tan(slope * Math.PI / 180) * 100;
                            }
                            result.Value[i, j] = slope;

                            var aspect = 57.29578 * Math.Atan2(dZdY, -dZdX);
                            double cell=0;
                            if (aspect < 0)
                                cell = 90.0 - aspect;
                            else if (aspect > 90.0)
                                cell = 360.0 - aspect + 90.0;
                            else
                                cell = 90.0 - aspect;

                            if (cancelProgressHandler != null && cancelProgressHandler.Cancel)
                            {
                                return null;
                            }
                        }
                        else
                        {
                            result.Value[i, j] = result.NoDataValue;
                        }

                        if (cancelProgressHandler != null && cancelProgressHandler.Cancel)
                        {
                            return null;
                        }
                    }
                }

                if (result.IsFullyWindowed())
                {
                    result.Save();
                    return result;
                }

                return null;
            }
            finally
            {
                if (progMeter != null)
                {
                    progMeter.Reset();
                }
            }
        }

        /// <summary>
        /// Executes the slope generation raster.
        /// </summary>
        /// <param name="raster">The input altitude raster.</param>
        /// <param name="inZFactor">The double precision multiplicative scaling factor for elevation values.</param>
        /// <param name="slopeInPercent">A boolean parameter that clarifies the nature of the slope values.  If this is true, the values represent percent slope.</param>
        /// <param name="cancelProgressHandler">The progress handler.</param>
        /// <returns>The output slope and aspect rasters, or null if the process was unsuccessful.</returns>
        public static IRaster[] GetSlopeAspect(IRaster raster, double inZFactor, bool slopeInPercent, ICancelProgressHandler cancelProgressHandler)
        {
            // Validates the input and output data
            if (raster == null)
            {
                return null;
            }

            int noOfCol = raster.NumColumns;
            int noOfRow = raster.NumRows;

            // Create the new raster with the appropriate dimensions
            var slopeRas = Raster.CreateRaster("SlopeRaster.bgd", string.Empty, noOfCol, noOfRow, 1, typeof(double), new[] { string.Empty });
            slopeRas.NoDataValue = raster.NoDataValue;
            slopeRas.Bounds = raster.Bounds;
            slopeRas.Projection = raster.Projection;

            var aspectRas = Raster.CreateRaster("AspectRaster.bgd", string.Empty, noOfCol, noOfRow, 1, typeof(double), new[] { string.Empty });
            aspectRas.NoDataValue = raster.NoDataValue;
            aspectRas.Bounds = raster.Bounds;
            aspectRas.Projection = raster.Projection;

            ProgressMeter progMeter = null;
            try
            {
                if (cancelProgressHandler != null) progMeter = new ProgressMeter(cancelProgressHandler, "Calculating Slope", slopeRas.NumRows);

                // Cache cell size for faster access
                var cellWidth = raster.CellWidth;
                var cellHeight = raster.CellHeight;
                for (int i = 0; i < slopeRas.NumRows; i++)
                {
                    if (cancelProgressHandler != null)
                    {
                        progMeter.Next();
                        if ((i % 100) == 0)
                        {
                            progMeter.SendProgress();

                        }
                    }
                    for (int j = 0; j < slopeRas.NumColumns; j++)
                    {
                        if (i > 0 && i < slopeRas.NumRows - 1 && j > 0 && j < slopeRas.NumColumns - 1)
                        {
                            double z1 = raster.Value[i - 1, j - 1];
                            double z2 = raster.Value[i - 1, j];
                            double z3 = raster.Value[i - 1, j + 1];
                            double z4 = raster.Value[i, j - 1];
                            double z5 = raster.Value[i, j + 1];
                            double z6 = raster.Value[i + 1, j - 1];
                            double z7 = raster.Value[i + 1, j];
                            double z8 = raster.Value[i + 1, j + 1];

                            // 3rd Order Finite Difference slope algorithm
                            double dZdX = inZFactor * ((z3 - z1) + (2 * (z5 - z4)) + (z8 - z6)) / (8 * cellWidth);
                            double dZdY = inZFactor * ((z1 - z6) + (2 * (z2 - z7)) + (z3 - z8)) / (8 * cellHeight);

                            double slope = Math.Atan(Math.Sqrt((dZdX * dZdX) + (dZdY * dZdY))) * (180 / Math.PI);

                            // change to radius and in percentage
                            if (slopeInPercent)
                            {
                                slope = Math.Tan(slope * Math.PI / 180) * 100;
                            }
                            slopeRas.Value[i, j] = slope;

                            var aspect = 57.29578 * Math.Atan2(dZdY, -dZdX);
                            double cell = 0;
                            if (aspect < 0)
                                cell = 90.0 - aspect;
                            else if (aspect > 90.0)
                                cell = 360.0 - aspect + 90.0;
                            else
                                cell = 90.0 - aspect;
                            aspectRas.Value[i, j] = cell;

                            if (cancelProgressHandler != null && cancelProgressHandler.Cancel)
                            {
                                return null;
                            }
                        }
                        else
                        {
                            slopeRas.Value[i, j] = -9999;
                        }

                        if (cancelProgressHandler != null && cancelProgressHandler.Cancel)
                        {
                            return null;
                        }
                    }
                }

                if (slopeRas.IsFullyWindowed())
                {
                    slopeRas.Save();
                    aspectRas.Save();
                    return new IRaster[] { slopeRas, aspectRas };
                }

                return null;
            }
            finally
            {
                if (progMeter != null)
                {
                    progMeter.Reset();
                }
            }
        }
    }
}
