using Heiflow.Models.Integration;
using Heiflow.Presentation.Controls.Project;
using Heiflow.Visualization.Renderable;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Visualization.Project
{
    [Serializable]
    public class Heiflow3DProject : HeiflowProject
    {
        public Heiflow3DProject()
        {
            DistanceAboveSurface = 10;
        }
        [XmlElement]
        public string RelativeModelGeoPrjFile
        {
            get;
            set;
        }

         [XmlElement]
        public List<ShapeFeatureSetProvider> ShapeFeatureSetProviders
        {
            get;
            set;
        }

         [XmlElement]
         public List<ShapeFeatureProvider> ShapeFeatureProvider
         {
             get;
             set;
         }
         [XmlElement]
         public List<FlagFeatureProvider> FlagFeatureProvider
         {
             get;
             set;
         }

           [XmlElement]
         public List<ModelFeatureProvider> ModelFeatureProvider
         {
             get;
             set;
         }
           [XmlElement]
           public List<ODMProvider> ODMProvider
           {
               get;
               set;
           }
           [XmlElement]
           public float DistanceAboveSurface
           {
               get;
               set;
           }

    }
}
