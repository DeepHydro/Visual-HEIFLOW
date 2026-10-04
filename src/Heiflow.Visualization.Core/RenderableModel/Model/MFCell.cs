using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heiflow.Models.Subsurface;
using GeoAPI.Geometries;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class MFCell : Cell
    {
        /// <summary>
        /// mfcell 
        /// </summary>
        /// <param name="i">starting from 0</param>
        /// <param name="j">starting from 0</param>
        public MFCell(int i, int j)
            : base(i, j)
        {

        }

        public void CalcCoordinate(MFGridRender render)
        {
            Render = render;
            var mfgrid = Render.Grid as MFGrid;
            LatitudeRadianRect = new double[4];
            LongitudeRadianRect = new double[4];

            var utm = Render.GCSConverter;
            var upleft = new Coordinate(mfgrid.BBox.MinX, mfgrid.BBox.MaxY);
            var cor = mfgrid.LocateCentroid(J + 1, I + 1);
            var inteval = mfgrid.DELR[0, 0, 0];
           utm.ToLatLon(cor.X + inteval, cor.Y - inteval);
            double lng = utm.Longitude;
            double lat = utm.Latitude;
            double centalLat = 0;
            double centalLng = 0;
            LatitudeRadianRect[0] = lat; //MathEngine.DegreesToRadians(lat);
            LongitudeRadianRect[0] = lng;//MathEngine.DegreesToRadians(lng);
            centalLng += lng;
            centalLat += lat;

            utm.ToLatLon(cor.X, cor.Y - inteval);
            lng = utm.Longitude;
            lat = utm.Latitude;
            LatitudeRadianRect[1] = lat; //MathEngine.DegreesToRadians(lat);
            LongitudeRadianRect[1] = lng;// MathEngine.DegreesToRadians(lng);

            utm.ToLatLon(cor.X, cor.Y);
            lng = utm.Longitude;
            lat = utm.Latitude;
            centalLng += lng;
            centalLat += lat;
            LatitudeRadianRect[2] = lat; //MathEngine.DegreesToRadians(lat);
            LongitudeRadianRect[2] = lng;//MathEngine.DegreesToRadians(lng);

            utm.ToLatLon(cor.X + inteval, cor.Y);
            lng = utm.Longitude;
            lat = utm.Latitude;
            LatitudeRadianRect[3] = lat; //MathEngine.DegreesToRadians(lat);
            LongitudeRadianRect[3] = lng;//MathEngine.DegreesToRadians(lng);

            CentralLatitude = 0.5 * centalLat;
            CentralLongitude = 0.5 * centalLng;

        }
        public override float[] GetTimeSeries()
        {
            return Render.GetCellTimeSeries(this.I, this.J);
        }
    }
}
