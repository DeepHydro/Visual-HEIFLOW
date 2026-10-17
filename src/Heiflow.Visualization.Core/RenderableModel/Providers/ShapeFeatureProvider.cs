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

            // What the water is drawn like when a shapefile is asked for as water, see
            // WaterSurfaceLayer. The heights are metres and the speed is how fast a crest runs.
            WaterSurface = false;
            WaveHeight = 0.6;
            WaveLength = 40;
            WaveSpeed = 1;
            ShallowColor = "#5BA8C2";
            DeepColor = "#0D3661";
            SkyColor = "#94BDE0";
        }

        /// <summary>
        /// Draws a polygon shapefile as water, with waves running across it, rather than as a flat
        /// sheet of one colour. Anything but a polygon is drawn the way it always was.
        /// </summary>
        [XmlElement]
        public bool WaterSurface
        {
            get;
            set;
        }

        /// <summary>
        /// Metres above the datum the water stands at. Nothing set means it is taken from the
        /// shapefile, and failing that from the ground under the outline.
        /// </summary>
        [XmlElement]
        public double? WaterLevel
        {
            get;
            set;
        }

        [XmlElement]
        public double WaveHeight
        {
            get;
            set;
        }

        [XmlElement]
        public double WaveLength
        {
            get;
            set;
        }

        [XmlElement]
        public double WaveSpeed
        {
            get;
            set;
        }

        [XmlElement]
        public string ShallowColor
        {
            get;
            set;
        }

        [XmlElement]
        public string DeepColor
        {
            get;
            set;
        }

        [XmlElement]
        public string SkyColor
        {
            get;
            set;
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
                shp = WaterSurface
                    ? WaterOf(filename, world, color)
                    : (RenderableShp)new RenderablePolygon(filename, world, color);

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

        /// <summary>
        /// A polygon shapefile as water, see WaterSurfaceLayer. The colours are the ones set on the
        /// provider, and one that does not read as a colour leaves the water as it comes.
        /// </summary>
        private WaterSurfaceLayer WaterOf(string filename, World world, System.Drawing.Color color)
        {
            var water = new WaterSurfaceLayer(filename, world, color);
            water.WaterLevel = WaterLevel;
            // A lake that carries no level of its own is laid in the hollow it lies in, which is what
            // the ground under its outline is. Without it the water stands at the datum, and on high
            // ground that is under the terrain, where nothing of it is ever seen.
            water.AutoTerrainHeight = true;
            water.WaveHeight = WaveHeight;
            water.WaveLength = WaveLength;
            water.WaveSpeed = WaveSpeed;

            var shallow = DrawingColorOf(ShallowColor);
            var deep = DrawingColorOf(DeepColor);
            var sky = DrawingColorOf(SkyColor);

            if (shallow.HasValue)
                water.ShallowColor = shallow.Value;
            if (deep.HasValue)
                water.DeepColor = deep.Value;
            if (sky.HasValue)
                water.SkyColor = sky.Value;

            return water;
        }

        /// <summary>
        /// A colour of the project file as a colour to draw with, and nothing where the text is not
        /// one, which leaves the layer with the colour it was made with.
        /// </summary>
        private static System.Drawing.Color? DrawingColorOf(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            try
            {
                var media = (Color)ColorConverter.ConvertFromString(text);
                return System.Drawing.Color.FromArgb(media.R, media.G, media.B);
            }
            catch
            {
                return null;
            }
        }
    }
}
