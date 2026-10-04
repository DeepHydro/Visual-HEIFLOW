using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUST.WREIS.Dot3D.Renderable;
using System.IO;
using Heiflow.Spatial.Geography;

namespace HUST.WREIS.Dot3D.Terrain
{
    class TifTerrainAccessor : TerrainAccessor
    {
        /// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Terrain.NltTerrainAccessor"/> class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="west"></param>
		/// <param name="south"></param>
		/// <param name="east"></param>
		/// <param name="north"></param>
		/// <param name="terrainTileService"></param>
        public TifTerrainAccessor(string name, double west, double south, double east, double north,
			TerrainTileService terrainTileService,string localfile)
		{
			m_name = name;
			m_west = west;
			m_south = south;
			m_east = east;
			m_north = north;
			m_terrainTileService = terrainTileService;
            string filename = Path.Combine(ConfigurationManager.Engine3DSettings.ApplicationDirectory, localfile);
            if(File.Exists(filename))
                RasterSet = new Raster(filename);
		}

        public Raster RasterSet { get; private set; }

        #region Public Methods
		/// <summary>
		/// Get terrain elevation at specified location.  
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="targetSamplesPerDegree"></param>
		/// <returns>Returns 0 if the tile is not available on disk.</returns>
		public override float GetElevationAt(double latitude, double longitude, double targetSamplesPerDegree)
		{
			try
			{			
                return RasterSet.GetPixelValueAt(latitude, longitude);
			}
			catch (Exception)
			{
			}
			return 0;
		}

		/// <summary>
		/// Get terrain elevation at specified location.  
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <returns>Returns 0 if the tile is not available on disk.</returns>
		public override float GetElevationAt(double latitude, double longitude)
		{
            try
            {
                return RasterSet.GetPixelValueAt(latitude, longitude);
            }
            catch (Exception)
            {
            }
            return 0;
		}

        public override float GetElevationAt(QuadTile qt, double latitude, double longitude, double targetSamplesPerDegree)
        {
            try
            {
                return RasterSet.GetPixelValueAt(latitude, longitude);
            }
            catch (Exception)
            {
            }
            return 0;
        }

		/// <summary>
		/// Builds a terrain array with specified boundaries
		/// </summary>
		/// <param name="north">North edge in decimal degrees.</param>
		/// <param name="south">South edge in decimal degrees.</param>
		/// <param name="west">West edge in decimal degrees.</param>
		/// <param name="east">East edge in decimal degrees.</param>
		/// <param name="samples"></param>
        public override TerrainTile GetElevationArray(QuadTile qt, double north, double south, double west, double east,
			int samples)
		{
			TerrainTile res = null;
			
			res = new TerrainTile(m_terrainTileService);
			res.North = north;
			res.South = south;
			res.West = west;
			res.East = east;
			res.SamplesPerTile = samples;
			res.IsInitialized = true;
			res.IsValid = true;

			double samplesPerDegree = (double)samples / (double)(north - south);
			double latrange = Math.Abs(north - south);
			double lonrange = Math.Abs(east - west);
			//TerrainTileCacheEntry ttce = null;

			float[,] data = new float[samples, samples];

			if(samplesPerDegree < World.Settings.MinSamplesPerDegree)
			{
				res.ElevationData = data;
				return res;
			}

			double scaleFactor = (double)1 / (samples - 1);
			for (int x = 0; x < samples; x++)
			{
				for (int y = 0; y < samples; y++)
				{
					double curLat = north - scaleFactor * latrange * x;
					double curLon = west + scaleFactor * lonrange * y;

                    // Wrap lat/lon to fit range 90/-90 and -180/180 (PM 2006-11-17)
                    if (curLat > 90)
                    {
                        curLat = 90 - (curLat - 90);
                        curLon += 180;
                    }
                    if (curLat < -90)
                    {
                        curLat = -90 - (curLat + 90);
                        curLon += 180;
                    }
                    if (curLon > 180)
                    {
                        curLon -= 360;
                    }
                    if (curLon < -180)
                    {
                        curLon += 360;
                    }
             
                    data[x, y] = RasterSet.GetPixelValueAt(curLat, curLon);
				}
			}
			res.ElevationData = data;
       //     res.ElevationData = new float[samples, samples];
			return res;
		}
        public override Raster GetRaster()
        {
            return RasterSet;
        }
		#endregion
    }
}
