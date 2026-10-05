using Microsoft.DirectX.Direct3D;
using HUST.WREIS.Dot3D.Net.Wms;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.IO;
using System.Drawing;
using System.Collections;
using Utility;
using Heiflow.Spatial;
using Heiflow.Spatial.MapProviders;


namespace HUST.WREIS.Dot3D
{

    public class ProjectedMapImageStore:ImageStore
    {
        //WW _levelZeroTileSizeDegrees
        //180 90 45 22.5 11.25 5.625 2.8125 1.40625 .703125 .3515625 
        //.17578125 .087890625 0.0439453125 0.02197265625 0.010986328125 0.0054931640625
        public ProjectedMapImageStore(string mapProviderName)
            : this(mapProviderName, "")
        {
        }

        /// <summary>
        /// The key of the service, only used by the providers that need one, e.g. Tianditu.
        /// </summary>
        public ProjectedMapImageStore(string mapProviderName, string token)
        {
            switch (mapProviderName)
            {
                case "GoogleSatelliteMap":
                    MapProvider = GoogleSatelliteMapProvider.Instance;
                    break;
                case "BingSatelliteMap":
                    MapProvider = BingSatelliteMapProvider.Instance;
                    break;
                case "OpenStreetMapProvider":
                    MapProvider = OpenStreetMapProvider.Instance;
                    break;
                case "GoogleTerrainMap":
                    MapProvider = GoogleTerrainMapProvider.Instance;
                    break;
                case "GoogleChinaHybridMap":
                    MapProvider = GoogleChinaHybridMapProvider.Instance;
                    break;
                case "GoogleChinaSatelliteMap":
                    MapProvider = GoogleChinaSatelliteMapProvider.Instance;
                    break;
                case "ArcGIS_Imagery_World_2D_MapProvider":
                    MapProvider = ArcGIS_Imagery_World_2D_MapProvider.Instance;
                    break;
                case "ArcGIS_World_Imagery_MapProvider":
                    MapProvider = ArcGIS_World_Imagery_MapProvider.Instance;
                    break;
                case "GoogleChinaTerrainMap":
                    MapProvider = GoogleChinaTerrainMapProvider.Instance;
                    break;
                case "TiandituSatelliteMapProvider":
                    MapProvider = TiandituSatelliteMapProvider.Instance;
                    break;
                default:
                    MapProvider = GoogleSatelliteMapProvider.Instance;
                    break;
            }
            Token = token;
        }

        public GMapProvider MapProvider { get; private set; }

        /// <summary>
        /// The key a provider that needs one, Tianditu today, has to put in every request.
        /// </summary>
        public string Token { get; private set; }

        public override string GetDownloadUrl(QuadTile qt)
        {
            GPoint centerTileXYLocation = new GPoint(qt.Col, qt.Row);
            var tianditu = MapProvider as TiandituSatelliteMapProvider;
            if (tianditu != null)
            {
                return tianditu.MakeTileImageUrl(centerTileXYLocation, qt.Level, "en", Token);
            }
            return MapProvider.MakeTileImageUrl(centerTileXYLocation, qt.Level, "en");
        }
    }
}
