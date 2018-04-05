// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Runtime.Serialization;
using System.ServiceModel;


namespace Heiflow.Spatial.Geography
{
    [ServiceContract(Namespace = "http://www.wreis.org")]
    public class GeographyArea
    {
        public GeographyArea()
        {
        }

        public GeographyArea(BoundingBox bbox)
        {
            MinX = bbox.MinX;
            MinY = bbox.MinY;
            MaxX = bbox.MaxX;
            MaxY = bbox.MaxY;
        }


        public double MinX { get; set; }
    
        public double MinY { get; set; }

        public double MaxX { get; set; }

        public double MaxY { get; set; }

        public override string ToString()
        {
            return MinX.ToString() + "," + MinY.ToString() + "," + MaxX.ToString() + "," + MaxY.ToString();
        }
    }
}
