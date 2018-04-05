// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heiflow.Spatial.Geography;
using System.IO;
using Heiflow.Spatial;

namespace Heiflow.Spatial.Geography
{
    public class Raster : IRaster, IGeometry
    {
        public Raster(string filename)
        {
            SamplesPerTile = 3600;
            NodataValue = -9999;
            FileName = filename;
            FileInfo info = new FileInfo(filename);

            if( info.Extension == ".dem")
                Read(filename);
            else if (info.Extension == ".tif")
                ReadTif(filename);
        }

        BoundingBox _boundingBox;
        public string FileName { get; private set; }
        public byte[] Data
        {
            get;
            private set;
        }
        /// <summary>
        /// Transform parameter
        /// </summary>
        private double[] gt;
       public double NodataValue { get; private set; }
        /// <summary>
        /// Width of the raster
        /// </summary>
        public int XSize { get; private set; }
        /// <summary>
        /// Height of the raster
        /// </summary>
        public int YSize { get; private set; }
        /// <summary>
        /// Count of pixel in the raster
        /// </summary>
        public int Count { get; private set; }

        public PointLatLng NorthWest { get; private set; }
        public PointLatLng SouthEast { get; private set; }

        public double SamplesPerDegree { get; private set; }

        public short[,] PixelValueArray { get; private set; }

        public int SamplesPerTile { get; private set; }

        public float MaximumAlt { get;private set; }
        public float MinimumAlt { get; private set; }
        public float Mean { get; private set; }
        public float StandardDeviation { get; private set; }

        public void ReadTif(string filename)
        {
            DotSpatial.Data.IRaster raster = DotSpatial.Data.Raster.Open(filename);
            gt = new double[6];
            gt[0] = raster.Extent.MinX;
            gt[1] = raster.CellWidth;
            gt[2] = 0;
            gt[3] = raster.Extent.MaxY;
            gt[4] = raster.CellHeight;
            gt[5] = 0;
            XSize = raster.NumColumns;
            YSize = raster.NumRows;
            //PixelValueArray = new float[XSize, YSize];
            PixelValueArray = new short[YSize, XSize];

        }

        public  void Read(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader bw = new BinaryReader(fs);
            gt = new double[6];
            for (int i = 0; i < 6; i++)
            {
                gt[i] = bw.ReadDouble();
            }
            XSize = bw.ReadInt32();
            YSize = bw.ReadInt32();
            //PixelValueArray = new float[XSize, YSize];
            PixelValueArray = new short[YSize, XSize];

            for (int y = 0; y < YSize; y++)
            {
                for (int x = 0; x < XSize; x++)
                {
                    PixelValueArray[y, x] = bw.ReadInt16();
                    if (x == 0 && y == 0)
                    {
                        MinimumAlt = PixelValueArray[y, x];
                        MaximumAlt = PixelValueArray[y, x];
                    }
                    else
                    {
                        MaximumAlt = PixelValueArray[y, x] > MaximumAlt ? PixelValueArray[y, x] : MaximumAlt;
                        MinimumAlt = PixelValueArray[y, x] < MinimumAlt ? PixelValueArray[y, x] : MinimumAlt;
                    }
                }
            }

            Mean = (float) Average(PixelValueArray);
            StandardDeviation = (float)SD(PixelValueArray);

            //NorthWest = RasterCoorinateConvertor.GetGeoCoorinate(1,1,gt);
            //SouthEast = RasterCoorinateConvertor.GetGeoCoorinate(XSize, YSize, gt);
            double xgeo = gt[0] + XSize * gt[1] + YSize * gt[2];
            double ygeo = gt[3] + XSize * gt[4] + YSize * gt[5];

            NorthWest = new PointLatLng(gt[3], gt[0]);
            SouthEast = new PointLatLng(ygeo, xgeo);
            SamplesPerDegree = (XSize - 1) / (SouthEast.Lng - NorthWest.Lng);

            var SamplesPerDegree1 = (YSize - 1) / (NorthWest.Lat - SouthEast.Lat);
        }

        public void WriteToDem(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            if (PixelValueArray != null)
            {
                for (int i = 0; i < gt.Length; i++)
                {
                    bw.Write(gt[i]);
                }
                bw.Write(XSize);
                bw.Write(YSize);

                for (int y = 0; y < YSize; y++)
                {
                    for (int x = 0; x < XSize; x++)
                    {
                        bw.Write(PixelValueArray[y, x]);
                    }
                }
            }
            bw.Close();
            fs.Close();
        }

        private  float SD(short[,] numl)
        {
            float Sum = 0.0f, SumOfSqrs = 0.0f;
            int len = 0;
            foreach (short d in numl)
            {
                Sum += d;
                SumOfSqrs += (float)Math.Pow(d, 2);
                len++;
            }
            float topSum = (float)((len * SumOfSqrs) - (Math.Pow(Sum, 2)));
            float n = (float)len;
            return (float)Math.Sqrt(topSum / (n * (n - 1)));
        }

        private  double Average(short[,] num)
        {
            double sum = 0.0;
            int len = 0;
            foreach (short d in num)
            {
                sum += d;
                len++;
            }
            double avg = sum / len;
            return avg;
        }
        public float GetPixelValueAt(double latitude, double longitude)
        {
            if (latitude < SouthEast.Lat || latitude > NorthWest.Lat || longitude < NorthWest.Lng || longitude > SouthEast.Lng)
                return 0;

            float result = 0;
            double deltaLat = NorthWest.Lat - latitude;
            double deltaLon = longitude - NorthWest.Lng;

            double df2 = SamplesPerDegree;
            float lat_pixel = (float)(deltaLat * df2);
            float lon_pixel = (float)(deltaLon * df2);

            int lat_min = (int)lat_pixel;
            int lat_max = (int)Math.Ceiling(lat_pixel);
            int lon_min = (int)lon_pixel;
            int lon_max = (int)Math.Ceiling(lon_pixel);

            //if (lat_min >= SamplesPerTile)
            //    lat_min = SamplesPerTile - 1;
            //if (lat_max >= SamplesPerTile)
            //    lat_max = SamplesPerTile - 1;
            //if (lon_min >= SamplesPerTile)
            //    lon_min = SamplesPerTile - 1;
            //if (lon_max >= SamplesPerTile)
            //    lon_max = SamplesPerTile - 1;

            if (lat_min < 0)
                lat_min = 0;
            if (lat_max < 0)
                lat_max = 0;
            if (lon_min < 0)
                lon_min = 0;
            if (lon_max < 0)
                lon_max = 0;
            if (lat_max >= YSize)
                lat_max = YSize -1;
            if (lon_max >= XSize)
                lon_max = XSize -1;

            float delta = lat_pixel - lat_min;

            float westElevation = PixelValueArray[lat_min, lon_min] * (1 - delta) + PixelValueArray[lat_max, lon_min] * delta;

            float eastElevation = PixelValueArray[lat_min, lon_max] * (1 - delta) + PixelValueArray[lat_max, lon_max] * delta;

            //float westElevation = PixelValueArray[lon_min, lat_min] * (1 - delta) + PixelValueArray[lon_min, lat_max] * delta;

            //float eastElevation = PixelValueArray[lon_min, lat_min] * (1 - delta) + PixelValueArray[lon_min, lat_max] * delta;

            delta = lon_pixel - lon_min;
            float interpolatedElevation =  westElevation * (1 - delta) + eastElevation * delta;

            GPoint gp = RasterCoorinateConvertor.GetPixelCoorinate(latitude, latitude, gt);
            if (gp.X >= 0 && gp.X < XSize && gp.Y >= 0 && gp.Y < YSize)
                result = PixelValueArray[gp.Y, gp.X];
            result = interpolatedElevation;

            return result;
        }

        public Raster(byte[] data, BoundingBox boundingBox)
        {
            this.Data = data;
            _boundingBox = boundingBox;
        }

        public BoundingBox GetBoundingBox()
        {
            return _boundingBox;
        }

        #region IGeometry Members

        public int Dimension
        {
            get { return 2; }
        }

        public int SRID
        {
            get;
            set;
        }

        public Geometry Envelope()
        {
            throw new NotImplementedException();
        }

        public string AsText()
        {
            throw new NotImplementedException();
        }

        public byte[] AsBinary()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public bool IsSimple()
        {
            throw new NotImplementedException();
        }

        public Geometry Boundary()
        {
            throw new NotImplementedException();
        }

        public bool Relate(Geometry other, string intersectionPattern)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Disjoint(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Touches(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Crosses(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Within(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public double Distance(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public Geometry Buffer(double d)
        {
            throw new NotImplementedException();
        }

        public Geometry ConvexHull()
        {
            throw new NotImplementedException();
        }

        public Geometry Intersection(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public Geometry Union(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public Geometry Difference(Geometry geom)
        {
            throw new NotImplementedException();
        }

        public Geometry SymDifference(Geometry geom)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
