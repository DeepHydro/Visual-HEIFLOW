// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Spatial
{
    [Serializable]
    public enum CoordinateSystem {Beijing1954,Xian1980,WGS1984,UTM}

    public class CoordinateConvertor
    {
        public static int ZoneWide =3; ////6度带宽 
        public static double iPI = 0.0174532925199433; ////3.1415926535898/180.0; 
        //public static int ZoneSerial { get; set; }
        public static double False_Easting = 500000.000000;

        static CoordinateConvertor()
        {
           
        }
        /// <summary>
        /// Get the number of projection zone
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static int GetProjectionNo(double longitude)
        {
          
            int ProjNo = (int)(longitude / ZoneWide);
            return ProjNo;
        }


        /// <summary>
        /// Get the central meridian longitude of the projection zone
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static double GetCentralMeridianLongitude(double longitude)
        {
            int ProjNo = GetProjectionNo(longitude);
            double longitude0 = ProjNo * ZoneWide + ZoneWide / 2;
            longitude0 = longitude0 * iPI;
            return longitude0;
        }

        /// <summary>
        /// 高斯投影由经纬度(Unit:DD)反算大地坐标(含带号，Unit:Metres) 
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public static void GaussProjCal(double longitude, double latitude, ref double X, ref double Y, CoordinateSystem cs)
        {
            int ProjNo = 0; 
            double longitude1, latitude1, longitude0,  X0, Y0, xval, yval;
            double a, f, e2, ee, NN, T, C, A, M;
            a = 0;
            f = 0;
            if (cs == CoordinateSystem.Xian1980)
            {
                a = 6378245.0; f = 1.0 / 298.3; //80年西安坐标系参数 
            }
            else if (cs == CoordinateSystem.Beijing1954)
            {
                a = 6378140.0; f = 1 / 298.257;//54年北京坐标系参数    
            }
      
            ProjNo = (int)(longitude / ZoneWide);
            longitude0 = ProjNo * ZoneWide + ZoneWide / 2;
            longitude0 = longitude0 * iPI;
            longitude1 = longitude * iPI; //经度转换为弧度
            latitude1 = latitude * iPI; //纬度转换为弧度
            e2 = 2 * f - f * f;
            ee = e2 * (1.0 - e2);
            NN = a / Math.Sqrt(1.0 - e2 * Math.Sin(latitude1) * Math.Sin(latitude1));
            T = Math.Tan(latitude1) * Math.Tan(latitude1);
            C = ee * Math.Cos(latitude1) * Math.Cos(latitude1);
            A = (longitude1 - longitude0) * Math.Cos(latitude1);
            M = a * ((1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256) * latitude1 - (3 * e2 / 8 + 3 * e2 * e2 / 32 + 45 * e2 * e2
            * e2 / 1024) * Math.Sin(2 * latitude1)
            + (15 * e2 * e2 / 256 + 45 * e2 * e2 * e2 / 1024) * Math.Sin(4 * latitude1) - (35 * e2 * e2 * e2 / 3072) * Math.Sin(6 * latitude1));
            xval =
            NN * (A + (1 - T + C) * A * A * A / 6 + (5 - 18 * T + T * T + 72 * C - 58 * ee) * A * A * A * A * A / 120);
            yval = M + NN * Math.Tan(latitude1) * (A * A / 2 + (5 - T + 9 * C + 4 * C * C) * A * A * A * A / 24
            + (61 - 58 * T + T * T + 600 * C - 330 * ee) * A * A * A * A * A * A / 720);
            X0 = 1000000L * (ProjNo + 1) + 500000L;
            Y0 = 0;
            xval = xval + X0; yval = yval + Y0;
            X = xval;
            Y = yval;
        }

        /// <summary>
        /// 高斯投影由大地坐标(Unit:Metres)反算经纬度(Unit:DD)
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        public static void GaussProjInvCal(double X, double Y, ref double longitude, ref double latitude, CoordinateSystem cs)
        {
            X = X + False_Easting;
            int ProjNo;// = CurrentProjNo;
            double longitude1, latitude1, longitude0,  X0, Y0, xval, yval;
            double e1, e2, f, a, ee, NN, T, C, M, D, R, u, fai;
            a = 0;
            f = 0;
            if (cs == CoordinateSystem.Xian1980)
            {
                a = 6378245.0; f = 1.0 / 298.3; //80年西安坐标系参数 
            }
            else if (cs == CoordinateSystem.Beijing1954)
            {
                a = 6378140.0; f = 1 / 298.257;//54年北京坐标系参数    
            }
            ProjNo = (int)(X / 1000000L); //查找带号
            if(ZoneWide == 6)
                longitude0 = (ProjNo - 1) * ZoneWide + ZoneWide / 2;
            else
                longitude0 = ProjNo * 3;//6度带宽
            //longitude0 = longitude0 * iPI; //中央经线
            X0 = ProjNo * 1000000L + 500000L;
            Y0 = 0;
            xval = X - X0; yval = Y - Y0; //带内大地坐标
            e2 = 2 * f - f * f;
            e1 = (1.0 - Math.Sqrt(1 - e2)) / (1.0 + Math.Sqrt(1 - e2));
            ee = e2 / (1 - e2);
            M = yval;
            u = M / (a * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256));
            fai =
            u + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * u) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(
            4 * u)
            + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * u) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * u);
            C = ee * Math.Cos(fai) * Math.Cos(fai);
            T = Math.Tan(fai) * Math.Tan(fai);
            NN = a / Math.Sqrt(1.0 - e2 * Math.Sin(fai) * Math.Sin(fai));
            R =
            a * (1 - e2) / Math.Sqrt((1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin
            (fai) * Math.Sin(fai)));
            D = xval / NN;
            //计算经度(Longitude) 纬度(Latitude)
            longitude1 =
            longitude0 + (D - (1 + 2 * T + C) * D * D * D / 6 + (5 - 2 * C + 28 * T - 3 * C * C + 8 * ee + 24 * T * T) * D
            * D * D * D * D / 120) / Math.Cos(fai);
            latitude1 = fai
            - (NN * Math.Tan(fai) / R) * (D * D / 2 - (5 + 3 * T + 10 * C - 4 * C * C - 9 * ee) * D * D * D * D / 24
            + (61 + 90 * T + 298 * C + 45 * T * T - 256 * ee - 3 * C * C) * D * D * D * D * D * D / 720);
            //转换为度 DD
            longitude = longitude1 / iPI;
            latitude = latitude1 / iPI;
        }

        public static void GaussToBL(double X, double Y, int ZoneWide, ref double longitude, ref double latitude, CoordinateSystem cs,int zoneSerial)//, double *longitude, double *latitude)
        {

            int ProjNo; ; ////带宽

         //   double[] output = new double[2];

            double longitude1, latitude1, longitude0, X0, Y0, xval, yval;//latitude0,

            double e1, e2, ee, NN, T, C, M, D, R, u, fai, iPI;
            double a = 0, f = 0;

            iPI = 0.0174532925199433; ////3.1415926535898/180.0;

            if (cs == CoordinateSystem.Beijing1954)
            {
                //54年北京坐标系参数
                a = 6378245.0; 
                f = 1.0 / 298.3; 
            }
            else if (cs == CoordinateSystem.Xian1980)
            {
                //80年西安坐标系参数
                a = 6378140.0;
                f = 1 / 298.257; 
            }

            ProjNo = (int)(X / 1000000L); //查找带号
       //     ProjNo = CurrentProjNo;
            if (ZoneWide == 6)
                longitude0 = (ProjNo - 1) * ZoneWide + ZoneWide / 2;//6度带宽
            else
                longitude0 = ProjNo * 3;//6度带宽

            longitude0 = longitude0 * iPI; //中央经线

            X0 = ProjNo * 1000000L + 500000L;

            Y0 = 0;

            xval = X - X0; yval = Y - Y0; //带内大地坐标

            e2 = 2 * f - f * f;

            e1 = (1.0 - Math.Sqrt(1 - e2)) / (1.0 + Math.Sqrt(1 - e2));

            ee = e2 / (1 - e2);

            M = yval;

            u = M / (a * (1 - e2 / 4 - 3 * e2 * e2 / 64 - 5 * e2 * e2 * e2 / 256));

            fai = u + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * u) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(

            4 * u) + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * u) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * u);

            C = ee * Math.Cos(fai) * Math.Cos(fai);

            T = Math.Tan(fai) * Math.Tan(fai);

            NN = a / Math.Sqrt(1.0 - e2 * Math.Sin(fai) * Math.Sin(fai));

            R = a * (1 - e2) / Math.Sqrt((1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin(fai) * Math.Sin(fai)) * (1 - e2 * Math.Sin

            (fai) * Math.Sin(fai)));

            D = xval / NN;

            //计算经度(Longitude) 纬度(Latitude)

            longitude1 = longitude0 + (D - (1 + 2 * T + C) * D * D * D / 6 + (5 - 2 * C + 28 * T - 3 * C * C + 8 * ee + 24 * T * T) * D

            * D * D * D * D / 120) / Math.Cos(fai);

            latitude1 = fai - (NN * Math.Tan(fai) / R) * (D * D / 2 - (5 + 3 * T + 10 * C - 4 * C * C - 9 * ee) * D * D * D * D / 24

            + (61 + 90 * T + 298 * C + 45 * T * T - 256 * ee - 3 * C * C) * D * D * D * D * D * D / 720);

            //转换为度 DD
            if( zoneSerial >110)
                longitude = zoneSerial + longitude1 / iPI;
            else
                longitude = longitude1 / iPI;

            latitude = latitude1 / iPI;
        }
    }
}
