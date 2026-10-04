using Heiflow.Controls.Project;
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
            ODMDBPath = "null";
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
         public List<ModelFeatureProvider> ModelFeatureProvider
         {
             get;
             set;
         }
           [XmlElement]
           public List<RenderableRasterProvider> IRenderableRasterProviders
           {
               get;
               set;
           }


         [XmlElement]
         public string ODMDBPath
         {
             get;
             set;
         }
    }
}
