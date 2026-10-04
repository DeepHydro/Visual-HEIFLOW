// <copyright   company="Environment Modeling & Software Studio"> 
// Copyright (c) 2009, 2010 All Right Reserved, http://www.wreis.org/ 
// 
// This source is subject to the EMSS Permissive License. 
// Please see the License.txt file for more information. 
// All other rights reserved. 
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY  
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// </copyright> 
//
// <author>Yong Tian</author> 
// <email>ytian.world@gmail.com</email> 
// <date>2011-11-1</date> 
// <summary></summary> 
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using HUST.WREIS.Dot3D.Net;
using System.Windows.Forms;

namespace HUST.WREIS.Dot3D.Terrain
{
    public class TerrainTile : IDisposable
    {
        public string TerrainTileFilePath { get; set; }
        public double TileSizeDegrees { get; set; }
        public int SamplesPerTile { get; set; }
        public double South { get; set; }
        public double North { get; set; }
        public double West { get; set; }
        public double East { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int TargetLevel { get; set; }
        public TerrainTileService Owner { get; set; }
        public bool IsInitialized { get; set; }
        public bool IsValid { get; set; }

        public float[,] ElevationData { get; set; }
        public TerrainDownloadRequest Request { get; set; }

        public TerrainTile(TerrainTileService owner)
        {
            Owner = owner;
        }
        /// <summary>
        /// This method initializes the terrain tile add switches to
        /// Initialize floating point/int 16 tiles
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
                return;

            ElevationData = Owner.TerrainStorageService.LoadTerrain(this);

            //if (ElevationData == null)
            //    ElevationData = new float[SamplesPerTile, SamplesPerTile];           
        }

        public void StartDownload()
        {
            if (Request == null)
            {
                if (Owner.GetType() == typeof(TerrainTileService))
                {
                    using (Request = new TerrainDownloadRequest(this, Owner, Row, Col, TargetLevel))
                    {
                        if (!(Owner.TerrainStorageService is FileTerrainStorageService))
                            TerrainTileFilePath = "";
                        if (!Owner.AddToDownloadQueue(Request))
                            Request = null;
                      //  Request.StartDownload();
                    }
                }
                else if (Owner.GetType() == typeof(WMSTerrainTileService))
                {
                    using (Request = new WMSDownloadRequest(this, Owner, Row, Col, TargetLevel))
                    {
                        if (!(Owner.TerrainStorageService is FileTerrainStorageService))
                            TerrainTileFilePath = "";
                        if (!Owner.AddToDownloadQueue(Request))
                            Request = null;
                       // Request.StartDownload();
                    }
                }
            }
        }

        public float GetElevationAt(double latitude, double longitude)
        {
            try
            {
                double deltaLat = North - latitude;
                double deltaLon = longitude - West;

                double df2 = (SamplesPerTile - 1) / TileSizeDegrees;
                float lat_pixel = (float)(deltaLat * df2);
                float lon_pixel = (float)(deltaLon * df2);

                int lat_min = (int)lat_pixel;
                int lat_max = (int)Math.Ceiling(lat_pixel);
                int lon_min = (int)lon_pixel;
                int lon_max = (int)Math.Ceiling(lon_pixel);

                if (lat_min >= SamplesPerTile)
                    lat_min = SamplesPerTile - 1;
                if (lat_max >= SamplesPerTile)
                    lat_max = SamplesPerTile - 1;
                if (lon_min >= SamplesPerTile)
                    lon_min = SamplesPerTile - 1;
                if (lon_max >= SamplesPerTile)
                    lon_max = SamplesPerTile - 1;

                if (lat_min < 0)
                    lat_min = 0;
                if (lat_max < 0)
                    lat_max = 0;
                if (lon_min < 0)
                    lon_min = 0;
                if (lon_max < 0)
                    lon_max = 0;

                float delta = lat_pixel - lat_min;
                float westElevation =
                    ElevationData[lon_min, lat_min] * (1 - delta) +
                    ElevationData[lon_min, lat_max] * delta;

                float eastElevation =
                    ElevationData[lon_max, lat_min] * (1 - delta) +
                    ElevationData[lon_max, lat_max] * delta;

                delta = lon_pixel - lon_min;
                float interpolatedElevation =
                    westElevation * (1 - delta) +
                    eastElevation * delta;

                return interpolatedElevation;
            }
            catch
            {
            }
            return 0;
        }
        
        #region IDisposable Members

        public void Dispose()
        {
            if (Request != null)
            {
                Request.Dispose();
                Request = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
