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
    public class ModelFeatureProvider:RenderableProvider
    {
        public ModelFeatureProvider()
        {

        }
         [XmlElement]
        public float Latitude { get; set; }
        [XmlElement]
        public float Longitude { get; set; }
        [XmlElement]
        public float Scale { get; set; }
        [XmlElement]
        public float Altitude { get; set; }
        [XmlElement]
        public float Rotx { get; set; }
        [XmlElement]
        public float Roty { get; set; }
        [XmlElement]
        public float Rotz { get; set; }

        public override HUST.WREIS.Dot3D.Renderable.RenderableObject Load(string filename, HUST.WREIS.Dot3D.World world)
        {
          //  FullFileName = filename;
            BaseDirectory = Path.GetDirectoryName(filename);
            RelativeFileName = filename;
            string name=Path.GetFileNameWithoutExtension(filename);
           var  _ModelFeature = new ModelFeature(name, world,
                     filename, 40, 100, 0, 1.0f, 0, 0, 0);
           return _ModelFeature;
        }

    }
}
