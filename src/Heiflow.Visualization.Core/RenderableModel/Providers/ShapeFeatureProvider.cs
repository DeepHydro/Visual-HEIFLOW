using DotSpatial.Data;
using Heiflow.Core.Drawing;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX.Direct3D;
using System;
using System.ComponentModel;
//using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Heiflow.Visualization.Renderable
{
    [Serializable]
    public class ShapeFeatureProvider:RenderableProvider
    {
        public ShapeFeatureProvider()
        {
            IsOn = true;
            CoordinateSystem = "WGS84";
            FeatureColor  = "Blue";
            FillMode = Microsoft.DirectX.Direct3D.FillMode.Solid;
            Opacity = 255;
            DistanceAboveSurface = 10;
            MinimumDisplayAltitude = 0;
            MaximumDisplayAltitude = 1000000;
            RenderPriority = HUST.WREIS.Dot3D.Renderable.RenderPriority.SurfaceImages;
          
        }

        [XmlElement]
        [Browsable(false)]
        public string CoordinateSystem
        {
            get;
            set;
        }
        [XmlElement]
        public string FeatureColor
        {
            get;
            set;
        }

        [XmlElement]
        public string LabelField
        {
            get;
            set;
        }

        [XmlIgnore]
        public RenderableObject RenderableObject
        {
            get;
            protected set;
        }

        public override RenderableObject Load(string filename, World world)
        {
            RelativeFileName = filename;
            var color = RandomColor.Next();
            var mcolor = MediaColor.From(color);
            string lyname = Path.GetFileNameWithoutExtension(filename);
            RenderableShp shp = null;
            ShapeFeatureProvider shpf = new ShapeFeatureProvider();
            IFeatureSet fs = FeatureSet.Open(filename);

            this.Name = lyname;
            try
            {
                if (FeatureColor != "" && FeatureColor != null)
                {
                    mcolor = (Color)ColorConverter.ConvertFromString(FeatureColor);
                    color = System.Drawing.Color.FromArgb(mcolor.R, mcolor.G, mcolor.B);
                }
            }
            catch
            {
                color = RandomColor.Next();
                mcolor = MediaColor.From(color);
            }
            finally
            {

            }
            if (fs.FeatureType == FeatureType.Line)
            {
                shp = new RenderableLine(filename, world, color);
                shp.Name = lyname;
                shp.DistanceAboveSurface = DistanceAboveSurface;

                shp.FeatureColor = mcolor;
                shp.FillMode = this.FillMode;
                shp.IsOn = this.IsOn;
                shp.MaxDisplayAltitude = this.MaximumDisplayAltitude;
                shp.MinDisplayAltitude = this.MinimumDisplayAltitude;
                shp.Opacity = this.Opacity;
                shp.RenderPriority = this.RenderPriority;
                shp.isSelectable = this.IsSelectable;
                shp.UpdateVertices();
                RenderableObject = shp;
            }
            else if (fs.FeatureType == FeatureType.Polygon)
            {
                shp = new RenderablePolygon(filename, world, color);
                shp.Name = lyname;
                shp.DistanceAboveSurface = DistanceAboveSurface;
                shp.FeatureColor = mcolor;
                shp.FillMode = this.FillMode;
                shp.IsOn = this.IsOn;
                shp.MaxDisplayAltitude = this.MaximumDisplayAltitude;
                shp.MinDisplayAltitude = this.MinimumDisplayAltitude;
                shp.Opacity = this.Opacity;
                shp.RenderPriority = this.RenderPriority;
                shp.UpdateVertices();
                RenderableObject = shp;
            }
            else if (fs.FeatureType == FeatureType.Point || fs.FeatureType == FeatureType.MultiPoint)
            {
                PointFeatureLayer layer = new PointFeatureLayer();
                RenderableObject = layer.Load(filename, world);
            }
         
            return RenderableObject;
        }

    }
}
