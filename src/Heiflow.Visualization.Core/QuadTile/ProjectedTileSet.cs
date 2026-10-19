using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

using Heiflow.Spatial;
using Heiflow.Spatial.Projections;
using Heiflow.Core;

using SharpDX;
using Rectangle = System.Drawing.Rectangle;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

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

           // StartZoomLevel = 7;
        }


         public override void Update(DrawArgs drawArgs)
         {
             if (!isInitialized)
                 Initialize(drawArgs);

             if (m_effectPath != null && m_effect == null)
             {
                 string errs = string.Empty;
                 m_effect = ShaderLoader.FromFile(DrawArgs.Device, m_effectPath, ShaderFlags.None, m_effectPool, out errs);

                 if (errs != null && errs != string.Empty)
                 {
                     m_effectPath = null;
                     m_effect = null;
                 }
             }
             double vrd = DrawArgs.Camera.ViewRange.Degrees;
             if (ImageStores[0].LevelZeroTileSizeDegrees <= 180)
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
                 // One budget per sweep, see QuadTile.MaxTileInitsPerSweep. The counter is static and
                 // shared by every layer, and QuadTileSet.Update is the only other caller of this. A
                 // projected layer that left the call out therefore kept adding to a total that
                 // nothing cleared: after the first eight tiles the budget was spent for good and no
                 // tile of this layer was ever initialized again, so the layer kept downloading tiles
                 // it never turned into textures and drew nothing beyond the few it started with.
                 QuadTile.BeginUpdateSweep();

                 // The first argument is the top edge, not the bottom one, see RectLatLng.FromLTRB:
                 // it takes the northern latitude and measures the height downwards from it. Given the
                 // southern edge instead, the rectangle reaches from there down and out of the range of
                 // the projection, and the tile list then covers only the southernmost row, so most of
                 // the layer is never built.
                 RectLatLng rectLatlng = new RectLatLng(m_north, m_west, m_east - m_west, m_north - m_south);
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

                     //if (DrawArgs.Camera.ViewFrustum.Intersects(qt.BoundingBox))
                     //{
                         // long key = ((long)gp.Y << 32) + gp.X;
                         lock (m_topmostTiles.SyncRoot)
                         {
                             if (!m_topmostTiles.ContainsKey(key))
                                    m_topmostTiles.Add(key, qt);
                             }
                             // The update has to happen in the sweep that created the tile. Left to
                            // the next sweep it never ran: every sweep starts by removing the tiles
                            // the frustum does not intersect, so a tile that had just been built and
                            // then judged invisible was disposed before anything asked it to
                            // initialize. It was rebuilt as new on the following sweep and thrown
                            // away again, and the layer kept a set of topmost tiles that were never
                            // updated at all - no tile counts, nothing drawn, which is what zooming
                            // below about 80 km looked like.
                            qt.Update(drawArgs);
                             //}
                    //}
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
