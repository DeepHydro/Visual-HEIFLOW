// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using Heiflow.Spatial.Geography;
namespace Heiflow.Spatial.Geography
{
    public interface IRaster : IGeometry
    {
        byte[] Data { get; }
        double NodataValue { get;   }
        /// <summary>
        /// Width of the raster
        /// </summary>
        int XSize { get;   }
        /// <summary>
        /// Height of the raster
        /// </summary>
        int YSize { get;   }
        /// <summary>
        /// Count of pixel in the raster
        /// </summary>
        int Count { get;   }

        short[,] PixelValueArray { get;   }

        float GetPixelValueAt(double lat, double lon);
    }
}
