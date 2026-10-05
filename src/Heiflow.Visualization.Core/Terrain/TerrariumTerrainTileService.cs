using System;

namespace HUST.WREIS.Dot3D.Terrain
{
    /// <summary>
    /// Elevation taken from terrain tiles in the terrarium encoding: PNG tiles that carry the height of
    /// every pixel in metres in their RGB bytes.
    ///
    /// The configuration asked the NASA WorldWind WMS for NASA_SRTM30_900m_Tiled as application/bil16,
    /// and that server has been shut down. Of the elevation servers that are still up, none answers a
    /// WMS request with bil16, so the WMS service cannot be repaired by pointing it somewhere else. The
    /// tiles this service reads are global, need no key, and are written into the same int16 terrain
    /// cache the old WMS service filled, so tiles that were downloaded from the old server are still
    /// used and only the missing ones are fetched again.
    /// </summary>
    public class TerrariumTerrainTileService : TerrainTileService
    {
        public TerrariumTerrainTileService(string serverUrl,
            string dataSet,
            double levelZeroTileSizeDegrees,
            int samplesPerTile,
            string fileExtension,
            int numberLevels,
            string terrainTileDirectory,
            TimeSpan terrainTileRetryInterval,
            string dataType,
            int zoomOffset,
            int maxZoom)
            : base(serverUrl, dataSet, levelZeroTileSizeDegrees, samplesPerTile, fileExtension, numberLevels,
                   terrainTileDirectory, terrainTileRetryInterval, dataType)
        {
            ZoomOffset = zoomOffset;
            MaxZoom = maxZoom;
        }

        /// <summary>
        /// Added to the level of a terrain tile to get the zoom of the tiles that cover it. One terrain
        /// tile spans 20/2^level degrees, one tile of the server spans 360/2^zoom degrees, so an offset
        /// of 4 leaves a tile of the server a little wider than a terrain tile and one to four of them
        /// cover it.
        /// </summary>
        public int ZoomOffset
        {
            get;
            protected set;
        }

        /// <summary>Highest zoom the server has tiles for.</summary>
        public int MaxZoom
        {
            get;
            protected set;
        }
    }
}
