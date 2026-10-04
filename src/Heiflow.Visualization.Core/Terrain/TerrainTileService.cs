
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using HUST.WREIS.Dot3D.Net;
using System.Windows.Forms;
using HUST.WREIS.Dot3D.Renderable;
using System.Collections;
using Heiflow.Spatial.Geography;

namespace HUST.WREIS.Dot3D.Terrain
{
 	/// <summary>
	/// Provides elevation data (BIL format).
	/// </summary>
	public class TerrainTileService : IDisposable
	{
		#region Private Members
		string m_serverUrl;
		string m_dataSet;
		double m_levelZeroTileSizeDegrees;
		int m_samplesPerTile;
		int m_numberLevels;
		string m_fileExtension;
		string m_terrainTileDirectory;
		TimeSpan m_terrainTileRetryInterval;
		string m_dataType;
        protected Hashtable m_downloadRequests = new Hashtable();
        protected int m_maxQueueSize = 400;

		#endregion

		#region Properties
		public string ServerUrl
		{
			get
			{
				return m_serverUrl;
			}
		}

		public string DataSet
		{
			get
			{
				return m_dataSet;
			}
		}

		public double LevelZeroTileSizeDegrees
		{
			get
			{
				return m_levelZeroTileSizeDegrees;
			}
		}

		public int SamplesPerTile
		{
			get
			{
				return m_samplesPerTile;
			}
		}

		public string FileExtension
		{
			get
			{
				return m_fileExtension;
			}
		}

		public string TerrainTileDirectory
		{
			get
			{
				return m_terrainTileDirectory;
			}
		}

		public TimeSpan TerrainTileRetryInterval
		{
			get
			{
				return m_terrainTileRetryInterval;
			}
		}

		public string DataType
		{
			get
			{
				return m_dataType;
			}
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Terrain.TerrainTileService"/> class.
		/// </summary>
		/// <param name="serverUrl"></param>
		/// <param name="dataSet"></param>
		/// <param name="levelZeroTileSizeDegrees"></param>
		/// <param name="samplesPerTile"></param>
		/// <param name="fileExtension"></param>
		/// <param name="numberLevels"></param>
		/// <param name="terrainTileDirectory"></param>
		/// <param name="terrainTileRetryInterval"></param>
		/// <param name="dataTyle">Terrain Tiles Data type</param>
        /// <param name="localFile">local dem File </param>
		public TerrainTileService(
			string serverUrl,
			string dataSet,
			double levelZeroTileSizeDegrees,
			int samplesPerTile,
			string fileExtension,
			int numberLevels,
			string terrainTileDirectory,
			TimeSpan terrainTileRetryInterval,
			string dataType,string localFile)
		{
			m_serverUrl = serverUrl;
			m_dataSet = dataSet;
			m_levelZeroTileSizeDegrees = levelZeroTileSizeDegrees;
			m_samplesPerTile = samplesPerTile;
			m_numberLevels = numberLevels;
			m_fileExtension = fileExtension.Replace(".","");
			m_terrainTileDirectory = terrainTileDirectory;
			if(!Directory.Exists(m_terrainTileDirectory))
				Directory.CreateDirectory(m_terrainTileDirectory);
			m_terrainTileRetryInterval = terrainTileRetryInterval;
			m_dataType = dataType;
            string filename = m_terrainTileDirectory + "\\" + localFile;
            RasterSet = new Raster(filename);
		}


        /// <summary>
        /// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Terrain.TerrainTileService"/> class.
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="dataSet"></param>
        /// <param name="levelZeroTileSizeDegrees"></param>
        /// <param name="samplesPerTile"></param>
        /// <param name="fileExtension"></param>
        /// <param name="numberLevels"></param>
        /// <param name="terrainTileDirectory"></param>
        /// <param name="terrainTileRetryInterval"></param>
        /// <param name="dataTyle">Terrain Tiles Data type</param>
        public TerrainTileService(
            string serverUrl,
            string dataSet,
            double levelZeroTileSizeDegrees,
            int samplesPerTile,
            string fileExtension,
            int numberLevels,
            string terrainTileDirectory,
            TimeSpan terrainTileRetryInterval,
            string dataType)
        {
            m_serverUrl = serverUrl;
            m_dataSet = dataSet;
            m_levelZeroTileSizeDegrees = levelZeroTileSizeDegrees;
            m_samplesPerTile = samplesPerTile;
            m_numberLevels = numberLevels;
            m_fileExtension = fileExtension.Replace(".", "");
            m_terrainTileDirectory = terrainTileDirectory;
            if (!Directory.Exists(m_terrainTileDirectory))
                Directory.CreateDirectory(m_terrainTileDirectory);
            m_terrainTileRetryInterval = terrainTileRetryInterval;
            m_dataType = dataType;
        }

        public Raster RasterSet
        {
            get;
            set;
        }

        public ITerrainStorageService TerrainStorageService { get; set; }

        /// <summary>
        /// Builds terrain tile containing the specified coordinates.
        /// </summary>
        /// <param name="latitude">Latitude in decimal degrees.</param>
        /// <param name="longitude">Longitude in decimal degrees.</param>
        /// <param name="samplesPerDegree"></param>
        /// <returns>Uninitialized terrain tile (no elevation data)</returns>
        public TerrainTile GetTerrainTile( double latitude, double longitude, double samplesPerDegree)
        {
            TerrainTile tile = new TerrainTile(this);

            tile.TargetLevel = m_numberLevels - 1;
            for (int i = 0; i < m_numberLevels; i++)
            {
                if (samplesPerDegree <= m_samplesPerTile / (m_levelZeroTileSizeDegrees * Math.Pow(0.5, i)))
                {
                    tile.TargetLevel = i;
                    break;
                }
            }

            tile.Row = GetRowFromLatitude(latitude, m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel));
            tile.Col = GetColFromLongitude(longitude, m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel));

            //tile.TerrainTileFilePath = string.Format( CultureInfo.InvariantCulture,
            //    @"{0}\{4}\{1:D4}\{1:D4}_{2:D4}.{3}",
            //    m_terrainTileDirectory, tile.Row, tile.Col, m_fileExtension, tile.TargetLevel);

            //if (File.Exists(tile.TerrainTileFilePath))
            //{
            tile.TerrainTileFilePath = string.Format(CultureInfo.InvariantCulture,
    @"{0}\{4}\{1}\{1}_{2}.{3}", m_terrainTileDirectory, tile.Row, tile.Col, m_fileExtension, tile.TargetLevel);

            tile.SamplesPerTile = m_samplesPerTile;
            tile.TileSizeDegrees = m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel);
            tile.North = -90.0 + tile.Row * tile.TileSizeDegrees + tile.TileSizeDegrees;
            tile.South = -90.0 + tile.Row * tile.TileSizeDegrees;
            tile.West = -180.0 + tile.Col * tile.TileSizeDegrees;
            tile.East = -180.0 + tile.Col * tile.TileSizeDegrees + tile.TileSizeDegrees;

            return tile;
            //}
            //else
            //{
            //    return GetTerrainTile(latitude, longitude, samplesPerDegree / 2);
            //}
        }

		/// <summary>
		/// Builds terrain tile containing the specified coordinates.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="samplesPerDegree"></param>
		/// <returns>Uninitialized terrain tile (no elevation data)</returns>
        public TerrainTile GetTerrainTile(QuadTile qt, double latitude, double longitude, double samplesPerDegree)
        {
            TerrainTile tile = new TerrainTile(this);

            tile.TargetLevel = m_numberLevels - 1;
            for (int i = 0; i < m_numberLevels; i++)
            {
                if (samplesPerDegree <= m_samplesPerTile / (m_levelZeroTileSizeDegrees * Math.Pow(0.5, i)))
                {
                    tile.TargetLevel = i;
                    break;
                }
            }

            tile.Row = GetRowFromLatitude(latitude, m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel));
            tile.Col = GetColFromLongitude(longitude, m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel));

            //tile.TerrainTileFilePath = string.Format( CultureInfo.InvariantCulture,
            //    @"{0}\{4}\{1:D4}\{1:D4}_{2:D4}.{3}",
            //    m_terrainTileDirectory, tile.Row, tile.Col, m_fileExtension, tile.TargetLevel);
            tile.TerrainTileFilePath = string.Format(CultureInfo.InvariantCulture,
    @"{0}\{4}\{1}\{1}_{2}.{3}", m_terrainTileDirectory, tile.Row, tile.Col, m_fileExtension, tile.TargetLevel);

            //if (File.Exists(tile.TerrainTileFilePath))
            //{
                tile.SamplesPerTile = m_samplesPerTile;
                tile.TileSizeDegrees = m_levelZeroTileSizeDegrees * Math.Pow(0.5, tile.TargetLevel);
                tile.North = -90.0 + tile.Row * tile.TileSizeDegrees + tile.TileSizeDegrees;
                tile.South = -90.0 + tile.Row * tile.TileSizeDegrees;
                tile.West = -180.0 + tile.Col * tile.TileSizeDegrees;
                tile.East = -180.0 + tile.Col * tile.TileSizeDegrees + tile.TileSizeDegrees;

                return tile;
            //}
            //else
            //{
            //    return GetTerrainTile(latitude, longitude, samplesPerDegree / 2);
            //}
        }

		// Hack: newer methods in MathEngine class cause problems
		public static int GetColFromLongitude(double longitude, double tileSize)
		{
			return (int)System.Math.Floor((System.Math.Abs(-180.0 - longitude)%360)/tileSize);
		}

		public static int GetRowFromLatitude(double latitude, double tileSize)
		{
			return (int)System.Math.Floor((System.Math.Abs(-90.0 - latitude)%180)/tileSize);
		}

        public virtual bool AddToDownloadQueue(TerrainDownloadRequest newRequest)
        {
            if (m_downloadRequests.Count > m_maxQueueSize)
                return false;
            TerrainTile key = newRequest.TerrainTile;
            //key.WaitingForDownload = true;
            lock (m_downloadRequests.SyncRoot)
            {
                if (m_downloadRequests.Contains(key))
                    return false;

                m_downloadRequests.Add(key, newRequest);
                newRequest.StartDownload();
                return true;
            }
        }

		#region IDisposable Members

		public void Dispose()
		{
			if(DrawArgs.DownloadQueue!=null)
				DrawArgs.DownloadQueue.Clear(this);
		}

		#endregion
	}
}
