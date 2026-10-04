using DotSpatial.Data;
using Heiflow.Core.Drawing;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Visualization.Renderable
{
     [Serializable]
    public class FlagFeatureProvider : RenderableProvider
    {
        public FlagFeatureProvider()
        {

        }
        [XmlElement]
        public string LabelField
        {
            get;
            set;
        }

        [XmlElement]
        public string FlagTexture
        {
            get;
            set;
        }

        [XmlElement]
        public float Scale
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

        [XmlIgnore]
        public RenderableObjectList RenderableObject
        {
            get;
            protected set;
        }

        public override RenderableObject Load(string filename, HUST.WREIS.Dot3D.World world)
        {  
            RelativeFileName = filename;
            string lyname = Path.GetFileNameWithoutExtension(filename);
            this.Name = lyname;
            IFeatureSet fs = FeatureSet.Open(filename);
            RenderableObjectList ro = new RenderableObjectList(this.Name);

            int i = 1;
            foreach (var f in fs.Features)
            {
                var points = (from coor in f.Geometry.Coordinates select new Point3d(coor.X, coor.Y, 0)).ToArray();

                foreach (var p in points)
                {
                    string name = "Flag" + i;
                    if (f.DataRow.Table.Columns.Contains(DescriptionField))
                    {
                        name = f.DataRow[DescriptionField].ToString();
                        if (name.Length > 5)
                            name = name.Substring(0, 5);
                    }
                    WavingFlagLayer flag = new WavingFlagLayer(name, world, p.Y, p.X, ConfigurationManager.Engine3DSettings.IconPath + FlagTexture);
                    flag.IsOn = true;
                    flag.ScaleX = Scale;
                    flag.ScaleY = Scale;
                    flag.ScaleZ = Scale;
                    flag.ShowHighlight = true;
                    flag.RenderPriority = RenderPriority.Custom;
                    flag.isSelectable = true;
                    flag.DistanceAboveSurface = this.DistanceAboveSurface;
                    ro.Add(flag);
                    i++;
                }
            }

            return ro;
        }


    }
}
