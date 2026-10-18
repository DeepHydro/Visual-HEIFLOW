using Heiflow.Core.IO;
using Heiflow.Models.Integration;
using Heiflow.Presentation.Controls.Project;
using Heiflow.Visualization.Renderable;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D.Terrain;
using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// A full file name as one relative to the directory the project file lies in, which is what
        /// is written to the project file. A project kept together with its layers then still finds
        /// them after it has been moved somewhere else.
        /// </summary>
        public string ToRelativeFileName(string fullFileName)
        {
            if (string.IsNullOrEmpty(fullFileName) || string.IsNullOrEmpty(AbsolutePathToProjectFile))
                return fullFileName;
            try
            {
                return FileHelper.GetRelativePath(Path.GetFullPath(fullFileName), AbsolutePathToProjectFile);
            }
            catch (Exception)
            {
                return fullFileName;
            }
        }

        /// <summary>
        /// Remembers a shapefile layer in the project, so that it is written to the project file when
        /// the project is saved and read back when the project is opened again. A layer of the same
        /// file and of the same kind, water or plain, is replaced rather than added a second time.
        /// </summary>
        public void AddShapeFeatureProvider(ShapeFeatureProvider provider)
        {
            if (provider == null)
                return;
            if (ShapeFeatureProvider == null)
                ShapeFeatureProvider = new List<ShapeFeatureProvider>();

            var old = ShapeFeatureProvider.FirstOrDefault(p => p != null
                && string.Equals(p.RelativeFileName, provider.RelativeFileName, StringComparison.OrdinalIgnoreCase)
                && p.WaterSurface == provider.WaterSurface);
            if (old != null)
                ShapeFeatureProvider.Remove(old);

            provider.BaseDirectory = AbsolutePathToProjectFile;
            ShapeFeatureProvider.Add(provider);
            IsDirty = true;
        }

        /// <summary>
        /// Forgets a layer that has been taken out of the scene, whichever of the lists of the project
        /// carries it, so that saving the project does not bring the layer back.
        /// </summary>
        public void RemoveProvider(RenderableProvider provider)
        {
            if (provider == null)
                return;
            bool gone = false;
            var shape = provider as ShapeFeatureProvider;
            var flag = provider as FlagFeatureProvider;
            var model = provider as ModelFeatureProvider;
            var odm = provider as ODMProvider;
            if (shape != null && ShapeFeatureProvider != null)
                gone = ShapeFeatureProvider.Remove(shape);
            else if (flag != null && FlagFeatureProvider != null)
                gone = FlagFeatureProvider.Remove(flag);
            else if (model != null && ModelFeatureProvider != null)
                gone = ModelFeatureProvider.Remove(model);
            else if (odm != null && ODMProvider != null)
                gone = ODMProvider.Remove(odm);
            if (gone)
                IsDirty = true;
        }

    }
}
