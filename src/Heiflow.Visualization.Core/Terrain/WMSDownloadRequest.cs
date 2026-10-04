using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Net;

namespace HUST.WREIS.Dot3D.Terrain
{
    public class WMSDownloadRequest : TerrainDownloadRequest
    {
        public WMSDownloadRequest(TerrainTile tile, TerrainTileService owner, int row, int col, int targetLevel)
            : base(tile
                , owner, row, col, targetLevel)
        {
            WMSTerrainTileService wms = owner as WMSTerrainTileService;

            TerrainTile = tile;
            string bbox = tile.West + "," + tile.South + "," + tile.East + "," + tile.North;
            download.Url = String.Format(CultureInfo.InvariantCulture,
                "{0}?SERVICE=WMS&REQUEST=GetMap&STYLES=&VERSION={1}&LAYERS={2}&format={3}&WIDTH={4}&HEIGHT={5}&BBOX={6}&CRS={7}",
                owner.ServerUrl, wms.Version, wms.DataSet, wms.Format, wms.SamplesPerTile, wms.SamplesPerTile,
                bbox, wms.SRS);
        }

        protected override void DownloadComplete(Net.WebDownload downloadInfo)
        {
             (Owner as WMSTerrainTileService).TerrainStorageService.SaveTerrain(TerrainTile,downloadInfo.ContentStream as MemoryStream);
        }
       
        protected override void ProcessFile()
        {
         
        }


        // http://www.nasa.network.com/elev?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&LAYERS=srtm30&STYLES=&SRS=EPSG:4326
        //&format=application/bil16&BBOX=-124,21,-66,49&WIDTH=150&HEIGHT=150

    }

}
