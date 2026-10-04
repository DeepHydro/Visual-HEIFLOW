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

            BoundingBox = new BoundingBox((float)south, (float)north, (float)west, (float)east,
                                (float)quadTileSet.LayerRadius,(float)quadTileSet.LayerRadius + 300000f);
            //localOrigin = BoundingBox.CalculateCenter();
            localOrigin = MathEngine.SphericalToCartesianD(CenterLatitude, CenterLongitude, quadTileSet.LayerRadius);

            // To avoid gaps between neighbouring tiles truncate the origin to 
            // a number that doesn't get rounded. (nearest 10km)
            localOrigin.X = (float)(Math.Round(localOrigin.X / 100000) * 100000);
            localOrigin.Y = (float)(Math.Round(localOrigin.Y / 100000) * 100000);
            localOrigin.Z = (float)(Math.Round(localOrigin.Z / 100000) * 100000);

            TileSet = quadTileSet as ProjectedTileSet;         
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
