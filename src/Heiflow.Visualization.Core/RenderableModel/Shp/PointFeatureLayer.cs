using DotSpatial.Data;
using DotSpatial.Projections;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Heiflow.Visualization.Renderable
{
    [Serializable]
    public class PointFeatureLayer : ShapeFeatureProvider
    {
        public PointFeatureLayer():base()
        {
            IsOn = true;
            IconWidthPixels = 16;
            IconHeightPixels = 16;
            OnClickZoomAltitude = 100000;
            OnClickZoomHeading = 100;
            OnClickZoomTilt = 10;
            TextureFilePath = ConfigurationManager.Engine3DSettings.IconPath + "marker_gold.png";
            DescriptionField = "";
        }

        public bool NameAlwaysVisible { get; set; }

        [XmlElement]
        public string TextureFilePath
        {
            get;
            set;
        }
        [XmlElement]
        public int IconWidthPixels
        {
            get;
            set;
        }
        [XmlElement]
        public int IconHeightPixels
        {
            get;
            set;
        }

        [XmlElement]
        public float OnClickZoomAltitude
        {
            get;
            set;
        }
        [XmlElement]
        public float OnClickZoomTilt
        {
            get;
            set;
        }
        [XmlElement]
        public float OnClickZoomHeading
        {
            get;
            set;
        }
        [XmlElement]
        public string DescriptionField
        {
            get;
            set;
        }

        public override RenderableObject Load(string filename, HUST.WREIS.Dot3D.World world)
        {
            string lyname = Path.GetFileNameWithoutExtension(filename);

            IFeatureSet fs = FeatureSet.Open(filename);
            if(this.DescriptionField == "")
            {
                DescriptionField = fs.DataTable.Columns[0].ColumnName;
            }
            System.Windows.Forms.MenuItem[] mitems = new System.Windows.Forms.MenuItem[2];
            mitems[0] = new System.Windows.Forms.MenuItem("View Data");
            mitems[1] = new System.Windows.Forms.MenuItem("Property");
            ClickableIcons icons = new ClickableIcons(lyname);
            double[] xy = new double[2];
            double[] z = new double[1];

            foreach (var f in fs.Features)
            {
                var points = (from coor in f.Geometry.Coordinates select new Point3d(coor.X, coor.Y, 0)).ToArray();
                foreach (var p in points)
                {
                    string textureName = this.TextureFilePath;
                    int width = this.IconWidthPixels;
                    int height = this.IconHeightPixels;
                    string DescriptionField = this.DescriptionField;
                    string name = "";

                    if (!fs.Projection.IsLatLon)
                    {
                        xy[0] = p.X;
                        xy[1] = p.Y;
                        Reproject.ReprojectPoints(xy, z, fs.Projection, KnownCoordinateSystems.Geographic.World.WGS1984, 0, 1);
                        p.X = xy[0];
                        p.Y = xy[1];
                    }

                    if (f.DataRow.Table.Columns.Contains(DescriptionField))
                    {
                        name = f.DataRow[DescriptionField].ToString();
                        if (name.Length > 5)
                            name = name.Substring(0, 5);
                    }
                    double terrainHeight = 0;
                    if (world.TerrainAccessor != null)
                    {
                        terrainHeight = world.TerrainAccessor.GetElevationAt(p.Y, p.X);
                    }
                    Icon ic = new Icon(name, name, p.Y, p.X, this.DistanceAboveSurface, world, textureName, width, height, "");
                    ic.IsOn = this.IsOn;
                    ic.Altitude = terrainHeight;
                    ic.isSelectable = this.IsSelectable;
                    ic.NameAlwaysVisible = this.NameAlwaysVisible;
                    ic.OnClickZoomAltitude = this.OnClickZoomAltitude;
                    ic.OnClickZoomHeading = this.OnClickZoomHeading;
                    ic.OnClickZoomTilt = this.OnClickZoomTilt;
                    ic.MinimumDisplayDistance = this.MinimumDisplayAltitude;
                    ic.MaximumDisplayDistance = this.MaximumDisplayAltitude;
                    ic.ParentControl = ConfigurationManager.Engine3DSettings.SceneWindow;
                    ic.ContextMenu = new System.Windows.Forms.ContextMenu(mitems);
                    if (f.DataRow.Table.Columns.Contains("SiteID"))
                        ic.Tag = f.DataRow["SiteID"].ToString();
                    icons.Add(ic);
                }

            }
            icons.DataTable = fs.DataTable;
            fs.Close();
            RenderableObject = icons;
            return icons;
        }
    
    }
}
