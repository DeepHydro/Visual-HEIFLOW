using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HUST.WREIS.Dot3D.Terrain
{
    public class WMSTerrainTileService:TerrainTileService
    {
        public WMSTerrainTileService(string serverUrl,
            string dataSet,
            double levelZeroTileSizeDegrees,
            int samplesPerTile,
            string fileExtension,
            int numberLevels,
            string terrainTileDirectory,
            TimeSpan terrainTileRetryInterval,
            string dataType, string version,string format,string srs)
            : base(serverUrl, dataSet, levelZeroTileSizeDegrees, samplesPerTile, fileExtension,numberLevels,
             terrainTileDirectory, terrainTileRetryInterval, dataType)
        {
            Version=version;
            LayerName= dataSet;
            Format=format;
            SRS=srs;
        }

        public string Version 
        {get; protected set;}

            public string LayerName 
            {get; protected set;}

            public string Format 
            {get; protected set;}
       
        public string SRS 
        {get;set;}
    }

}
