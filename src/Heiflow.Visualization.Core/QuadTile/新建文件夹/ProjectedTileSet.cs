using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;
using Microsoft.DirectX.Direct3D;
using Heiflow.Spatial;
using Heiflow.Spatial.Projections;
using Heiflow.Models.Component;

namespace HUST.WREIS.Dot3D.Renderable
{ 
    public class ProjectedTileSet : QuadTileSet
    {
        private Projection _proj;
        private  double earthRadius; //6378137;
        private  double earthCircum; //40075016.685578488
        private  double earthHalfCirc; //20037508.
        private const int pixelsPerTile = 256;

        readonly MercatorProjection projection = new MercatorProjection();
        public  PureProjection Projection
        {
            get
            {
                return projection;
            }
        }

        protected int _StartZoomLevel;

        public int StartZoomLevel
        {
            get
            {
                return _StartZoomLevel;
            }
            set
            {
                _StartZoomLevel = value;
            }
        }


         public ProjectedTileSet(
        string name,
        World parentWorld,
        double distanceAboveSurface,
        double north,
        double south,
        double west,
        double east,
        bool terrainMapped, ImageStore[] imageStores)
            : base(name, parentWorld,distanceAboveSurface,north,south,west,east,terrainMapped,imageStores)
        {
            earthRadius = parentWorld.EquatorialRadius;
            earthCircum = earthRadius * 2.0 * Math.PI; //40075016.685578488
            earthHalfCirc = earthCircum / 2; //20037508.

            //NOTE tiles did not line up properly with ellps=WGS84
            string[] args = {"+proj=lcc", "+lat_1=33", 
							"+lat_2=45", "+datum=NAD27", "+nodefs"};
            string[] projectionParameters = new string[] { "proj=merc", "ellps=sphere", "a=" + earthRadius.ToString(), "es=0.0", "no.defs" };
            _proj = new Projection(projectionParameters);

            StartZoomLevel = 7;
        }


         public override void Update(DrawArgs drawArgs)
         {
             if (!isInitialized)
                 Initialize(drawArgs);

             if (m_effectPath != null && m_effect == null)
             {
                 string errs = string.Empty;
                 m_effect = Effect.FromFile(DrawArgs.Device, m_effectPath, null, "", ShaderFlags.None, m_effectPool, out errs);

                 if (errs != null && errs != string.Empty)
                 {
                     m_effectPath = null;
                     m_effect = null;
                 }
             }
             double vrd = DrawArgs.Camera.ViewRange.Degrees;
             if (ImageStores[0].LevelZeroTileSizeDegrees < 180)
             {
                 // Check for layer outside view

                 double latitudeMax = DrawArgs.Camera.Latitude.Degrees + vrd;
                 double latitudeMin = DrawArgs.Camera.Latitude.Degrees - vrd;
                 double longitudeMax = DrawArgs.Camera.Longitude.Degrees + vrd;
                 double longitudeMin = DrawArgs.Camera.Longitude.Degrees - vrd;
                 if (latitudeMax < m_south || latitudeMin > m_north || longitudeMax < m_west || longitudeMin > m_east)
                     return;
             }

             if (DrawArgs.Camera.ViewRange * 0.5f > Angle.FromDegrees(TileDrawDistance * ImageStores[0].LevelZeroTileSizeDegrees))
             {
                 lock (m_topmostTiles.SyncRoot)
                 {
                     foreach (QuadTile qt in m_topmostTiles.Values)
                         qt.Dispose();
                     m_topmostTiles.Clear();
                     ClearDownloadRequests();
                 }

                 return;
             }

             RemoveInvisibleTiles(DrawArgs.Camera);
             try
             {
                 RectLatLng rectLatlng = new RectLatLng(m_south, m_west, m_east - m_west, m_north - m_south);
                 List<GPoint> topPoints = projection.GetAreaTileList(rectLatlng, StartZoomLevel, 0);

                 foreach (GPoint gp in topPoints)
                 {
                     long key = ((long)gp.Y << 32) + gp.X;
                     QuadTile qt = (QuadTile)m_topmostTiles[key];
                     if (qt != null)
                     {
                         qt.Update(drawArgs);
                         continue;
                     }

                     LatLngRect rect = GetLatLngBounds(gp.Y, gp.X, StartZoomLevel);
                     if (rect.West > m_east)
                         continue;
                     if (rect.East < m_west)
                         continue;
                     if (rect.South > m_north)
                         continue;
                     if (rect.North < m_south)
                         continue;

                     qt = new ProjectedQuadTile(rect.South, rect.North, rect.West, rect.East, StartZoomLevel, this);
                     qt.Row = gp.Y;
                     qt.Col = gp.X;

                     if (DrawArgs.Camera.ViewFrustum.Intersects(qt.BoundingBox))
                     {
                         // long key = ((long)gp.Y << 32) + gp.X;
                         lock (m_topmostTiles.SyncRoot)
                         {
                             if (!m_topmostTiles.ContainsKey(key))
                                 m_topmostTiles.Add(key, qt);
                         }
                         qt.Update(drawArgs);
                     }
                 }
             }
             catch (Exception caught)
             {
                 Log.Write(caught);
             }
         }

        public LatLngRect GetLatLngBounds(int row, int col, int level)
        {
            double metersPerPixel = MetersPerPixel(level);
            double totalTilesPerEdge = Math.Pow(2, level);
            double totalMeters = totalTilesPerEdge * pixelsPerTile * metersPerPixel;
            double halfMeters = totalMeters / 2;

            //do meters calculation in VE space
            //the 0,0 origin for VE is in upper left
            double N = row * (pixelsPerTile * metersPerPixel);
            double W = col * (pixelsPerTile * metersPerPixel);
            //now convert it to +/- meter coordinates for Proj.4
            //the 0,0 origin for Proj.4 is 0 lat, 0 lon
            //-22 to 22 million, -11 to 11 million
            N = halfMeters - N;
            W = W - halfMeters;
            double E = W + (pixelsPerTile * metersPerPixel);
            double S = N - (pixelsPerTile * metersPerPixel);

            UV UL = new UV(W, N);
            UV UR = new UV(E, N);
            UV LL = new UV(W, S);
            UV LR = new UV(E, S);
            UV geoUL = _proj.Inverse(UL);
            UV geoLR = _proj.Inverse(LR);
            double latRange = (geoUL.U - geoLR.U) * 180 / Math.PI;

            double north = geoUL.V * 180 / Math.PI;
            double south = geoLR.V * 180 / Math.PI;
            double west = geoUL.U * 180 / Math.PI;
            double east = geoLR.U * 180 / Math.PI;

            //double west = ProjectionMercator.xToLon(W);
            //double east = ProjectionMercator.xToLon(E);
            //double south = ProjectionMercator.yToLat(S);
            //double north = ProjectionMercator.yToLat(N);

            return new LatLngRect(west,east,south,north);
        }

        public  double MetersPerTile(int zoom)
        {
            return MetersPerPixel(zoom) * pixelsPerTile;
        }

        public double MetersPerPixel(int zoom)
        {
            double arc;
            arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            return arc;
        }

        public double DegToRad(double d)
        {
            return d * Math.PI / 180.0;
        }

        public double RadToDeg(double d)
        {
            return d * 180 / Math.PI;
        }
    }

}
