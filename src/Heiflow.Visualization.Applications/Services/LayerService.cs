using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic.Project;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Applications
{
    /// <summary>
    /// This class is specially used to handle layers contained in a Project file
    /// </summary>
    [Export]
    public class LayerService
    {
        /// <summary>
        /// Which entry of the project file each layer of the scene came from. A layer is drawn by a
        /// renderable object and written to the project file by a provider, and nothing else carries
        /// the way from one to the other, so taking a layer out of the scene would otherwise leave its
        /// entry behind and the project would bring it back the next time it is opened.
        /// </summary>
        private readonly Dictionary<RenderableObject, RenderableProvider> _Providers
            = new Dictionary<RenderableObject, RenderableProvider>();

        public LayerService()
        {

        }

        public void RememberProvider(RenderableObject layer, RenderableProvider provider)
        {
            if (layer == null || provider == null)
                return;
            lock (_Providers)
            {
                _Providers[layer] = provider;
            }
        }

        public RenderableProvider ProviderOf(RenderableObject layer)
        {
            if (layer == null)
                return null;
            lock (_Providers)
            {
                RenderableProvider provider;
                return _Providers.TryGetValue(layer, out provider) ? provider : null;
            }
        }

        public void ForgetProvider(RenderableObject layer)
        {
            if (layer == null)
                return;
            lock (_Providers)
            {
                _Providers.Remove(layer);
            }
        }

        /// <summary>
        /// True for a layer that hangs in the Feature Layers group, or in a group within it. Those are
        /// the layers put into the project by hand, an added shapefile or water body among them, and
        /// being put in by hand is what makes a layer one that may be taken out again.
        /// </summary>
        public bool IsInFeatureLayerGroup(RenderableObject layer)
        {
            if (layer == null || FeatureLayerGroup == null)
                return false;
            for (RenderableObjectList list = layer.ParentList; list != null; list = list.ParentList)
            {
                if (ReferenceEquals(list, FeatureLayerGroup))
                    return true;
            }
            return false;
        }

        public RenderableSitesLayer SitesLayer
        {
            get;
            set;
        }

        public ODMSource ODMSource
        {
            get;
            set;
        }

        public RenderableObjectList Layers
        {
            get;
            set;
        }

        public RenderableObjectList ModelLayerGroup
        {
            get;
            set;
        }

        /// <summary>
        /// The layers that are read out of files belonging to the project: the shapefiles and the water
        /// bodies added to it, the flags among them, together with whatever else the project file
        /// carries. In the layer manager they hang under the group of the same name, the other group
        /// being the one that carries the layers of the model being simulated.
        /// </summary>
        public RenderableObjectList FeatureLayerGroup
        {
            get;
            set;
        }

        public RenderableObjectList RootLayers
        {
            get;
            set;
        }

        public void Present(IProject project, World world)
        {
            var prj = project as Heiflow3DProject;
            LoadShpFile(prj, world);
            LoadRasterFile(prj, world);
            Load3DModel(prj, world);
            LoadODM(prj, world);
        }

        public void Clear()
        {
            Layers.Remove(ModelLayerGroup);
            Layers.Remove(FeatureLayerGroup);
            SitesLayer.SiteIcons.RemoveAll();
        }

        private void LoadShpFile(Heiflow3DProject prj, World world)
        {
            if (prj.ShapeFeatureProvider == null)
                return;
            foreach (var ly in prj.ShapeFeatureProvider)
            {        
                string filename = "";
                ly.BaseDirectory = prj.AbsolutePathToProjectFile;
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                    //ly.FullFileName = filename;
                    var ro = ly.Load(filename, world);
                    ro.RenderPriority = RenderPriority.SurfaceImages;
                    FeatureLayerGroup.Add(ro);
                    RememberProvider(ro, ly);
                }
            }
            if (prj.FlagFeatureProvider == null)
                return;
            foreach (var ly in prj.FlagFeatureProvider)
            {
                string filename = "";
                ly.BaseDirectory = prj.AbsolutePathToProjectFile;
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                //    ly.FullFileName = filename;
                    var ro = ly.Load(filename, world);
                    ro.RenderPriority = RenderPriority.Icons;
                    FeatureLayerGroup.Add(ro);
                    RememberProvider(ro, ly);
                }
            }
        }
        private void LoadRasterFile(Heiflow3DProject prj, World world)
        {
            //foreach (var ly in prj.RenderableRasterProvider)
            //{
            //    string filename = "";
            //    if (File.Exists(ly.FullFileName))
            //        filename = ly.FullFileName;
            //    else
            //    {
            //        string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
            //        filename = Path.Combine(folder, ly.RelativeFileName);
            //    }
            //    if (File.Exists(filename))
            //    {
            //        ly.FullFileName = filename;
            //        var ro = ly.Load(filename, world);
            //        world.DataLayerList.Add(ro);
            //    }
            //}
        }
        private void Load3DModel(Heiflow3DProject prj, World world)
        {
            if (prj.ModelFeatureProvider == null)
                return;
            foreach (var ly in prj.ModelFeatureProvider)
            {
                string filename = "";
                ly.BaseDirectory = prj.AbsolutePathToProjectFile;
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                  //  ly.FullFileName = filename;
                    var ro = ly.Load(filename, world);
                    FeatureLayerGroup.Add(ro);
                    RememberProvider(ro, ly);
                }
            }
        }

        private void LoadODM(Heiflow3DProject prj, World world)
        {
            if (prj.ODMProvider == null)
                return;
            foreach (var ly in prj.ODMProvider)
            {
                string filename = "";
                ly.BaseDirectory = prj.AbsolutePathToProjectFile;
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                    ODMSource source = new ODMSource();
                    string msg="";
                    if (source.Open(filename, ref msg))
                    {
                        this.ODMSource = source;
                        var sites = source.GetSites(new QueryCriteria());
                        this.SitesLayer.ShowSites(sites);
                    }
                }
            }
        }
    }
}
