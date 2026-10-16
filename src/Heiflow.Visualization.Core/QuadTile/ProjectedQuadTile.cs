using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using HUST.WREIS.Dot3D.Terrain;
using HUST.WREIS.Dot3D.Configuration;
using HUST.WREIS.Dot3D.Net;

using Utility;
using System.Drawing;
using System.Collections;
using Heiflow.Spatial;


namespace HUST.WREIS.Dot3D.Renderable
{
    public class ProjectedQuadTile:QuadTile
    {

        public ProjectedTileSet TileSet
        {
            get;
            private set;
        }

        public ProjectedQuadTile(double south, double north, double west, double east, int level, QuadTileSet quadTileSet)
            : base(south,north,west,east,level,quadTileSet)
        {
            this.Level = level;
            this.QuadTileSet = quadTileSet;

            CenterLatitude = Angle.FromDegrees(0.5f * (North + South));
            CenterLongitude = Angle.FromDegrees(0.5f * (West + East));
            LatitudeSpan = Math.Abs(North - South);
            LongitudeSpan = Math.Abs(East - West);

            // As in QuadTile: the box is spanned by the four corners of the tile, the sphere
            // sags away from the plane through them, and that sag is all the slack the box
            // needs. It replaces a height of 30000000000 times the vertical exaggeration,
            // which stretched the box past the whole scene, so that every tile in this tree
            // answered the frustum test with yes.
            // The vertices sit on the sphere, but between two of them the mesh is a straight
            // triangle, and that triangle cuts inside the sphere. So the drawn surface reaches
            // from LayerRadius down to LayerRadius minus the sag of the chord, and the box has
            // to allow the sag both ways - allowing it only outwards left the tile's own
            // geometry outside its own box, and a tile whose box fails the frustum test takes
            // its whole subtree with it, which gave this layer patches of blank ground that
            // moved with the view and with latitude. The sag is what separates it from a plain
            // QuadTileSet: at StartZoomLevel 5 a tile spans 5.6 degrees and the sag is about
            // 7 km, but at StartZoomLevel 1 it spans 180 degrees and the sag is thousands.
            double layerRadius = quadTileSet.LayerRadius;
            double chordSag = ChordSag(layerRadius, LatitudeSpan, LongitudeSpan);

            BoundingBox = new BoundingBox((float)south, (float)north, (float)west, (float)east,
                                (float)(layerRadius - chordSag - BoundingBoxSlack), (float)(layerRadius + chordSag + BoundingBoxSlack));
            //localOrigin = BoundingBox.CalculateCenter();
            localOrigin = MathEngine.SphericalToCartesianD(CenterLatitude, CenterLongitude, quadTileSet.LayerRadius);

            // To avoid gaps between neighbouring tiles truncate the origin to 
            // a number that doesn't get rounded. (nearest 10km)
            localOrigin.X = (float)(Math.Round(localOrigin.X / 100000) * 100000);
            localOrigin.Y = (float)(Math.Round(localOrigin.Y / 100000) * 100000);
            localOrigin.Z = (float)(Math.Round(localOrigin.Z / 100000) * 100000);

            TileSet = quadTileSet as ProjectedTileSet;         
        }

        /// <summary>
        /// The tiles of this layer are in web mercator, so the rows of the image are evenly
        /// spaced in projected metres and not in latitude. Asking the projection where a
        /// latitude falls inside the tile is what keeps the image on the ground: on a level 1
        /// tile, which covers the quarter of the globe between the equator and 85 degrees, the
        /// middle row of the image belongs at 66 degrees of latitude and not at 42, so laying
        /// the rows out evenly in latitude shifted the image by thousands of kilometres.
        /// </summary>
        protected override double TextureVForLatitude(double latitude)
        {
            double northY = MercatorY(North);
            double southY = MercatorY(South);
            double span = northY - southY;
            if (span == 0)
                return 0;

            return (northY - MercatorY(latitude)) / span;
        }

        /// <summary>
        /// Where a latitude sits in the projected metres of a mercator map. The unit cancels
        /// out, only the ratio between two of these is used. Clipped the way the projection
        /// that produced the tiles clips, so that a tile edge at the limit asks for the same
        /// row the image has.
        /// </summary>
        static double MercatorY(double latitude)
        {
            const double maxLatitude = 85.0511287798066;
            double phi = latitude;
            if (phi > maxLatitude)
                phi = maxLatitude;
            if (phi < -maxLatitude)
                phi = -maxLatitude;

            phi = phi * Math.PI / 180.0;
            return Math.Log(Math.Tan(Math.PI / 4.0 + phi / 2.0));
        }

        public override void ComputeChildren(DrawArgs drawArgs)
        {
            PureProjection projection = TileSet.Projection;
            if (Level + 1 >= QuadTileSet.ImageStores[0].LevelCount)
                return;

            double CenterLat = 0.5f * (South + North);
            double CenterLon = 0.5f * (East + West);

            int level = Level + 1;
            double deltaLat=LatitudeSpan/4;
              double deltaLng=LongitudeSpan/4;
              GPoint nwP;
              GPoint nwTile;
            if (northWestChild == null)
            {
                PointLatLng pll = new PointLatLng(CenterLat + deltaLat, CenterLon - deltaLng);
                nwP = projection.FromLatLngToPixel(pll, level);
                nwTile = projection.FromPixelToTileXY(nwP);

                LatLngRect rect = TileSet.GetLatLngBounds(nwTile.Y, nwTile.X, level);
                northWestChild = ComputeChild(rect.South, rect.North, rect.West, rect.East);
                northWestChild.Row = nwTile.Y;
                northWestChild.Col = nwTile.X;
            }

            if (northEastChild == null)
            {
                PointLatLng pll = new PointLatLng(CenterLat + deltaLat, CenterLon + deltaLng);
                nwP = projection.FromLatLngToPixel(pll, level);
                nwTile = projection.FromPixelToTileXY(nwP);

                LatLngRect rect = TileSet.GetLatLngBounds(nwTile.Y, nwTile.X, level);
                northEastChild = ComputeChild(rect.South, rect.North, rect.West, rect.East);
                northEastChild.Row = nwTile.Y;
                northEastChild.Col = nwTile.X;
            }

            if (southWestChild == null)
            {
                PointLatLng pll = new PointLatLng(CenterLat - deltaLat, CenterLon - deltaLng);
                nwP = projection.FromLatLngToPixel(pll, level);
                nwTile = projection.FromPixelToTileXY(nwP);

                LatLngRect rect = TileSet.GetLatLngBounds(nwTile.Y, nwTile.X, level);
                southWestChild = ComputeChild(rect.South, rect.North, rect.West, rect.East);
                southWestChild.Row = nwTile.Y;
                southWestChild.Col = nwTile.X;
            }

            if (southEastChild == null)
            {
                PointLatLng pll = new PointLatLng(CenterLat - deltaLat, CenterLon + deltaLng);
                nwP = projection.FromLatLngToPixel(pll, level);
                nwTile = projection.FromPixelToTileXY(nwP);

                LatLngRect rect = TileSet.GetLatLngBounds(nwTile.Y, nwTile.X, level);
                southEastChild = ComputeChild(rect.South, rect.North, rect.West, rect.East);
                southEastChild.Row = nwTile.Y;
                southEastChild.Col = nwTile.X;
            }
        }

        protected override QuadTile ComputeChild(double childSouth, double childNorth, double childWest, double childEast)
        {
            ProjectedQuadTile child = new ProjectedQuadTile(
                 childSouth,
                 childNorth,
                 childWest,
                 childEast,
                 this.Level + 1,
                 QuadTileSet);
            return child;
        }
    }
}
