using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Visualization.Renderable
{
    [Serializable]
    public class ShapeFeatureSetProvider: RenderableProvider
    {
        public ShapeFeatureSetProvider()
        {

        }
        [XmlElement]
        public ShapeFeatureProvider[] ShapeFeatures
        {
            get;
            set;
        }
    }
}
