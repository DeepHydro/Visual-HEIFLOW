using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
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
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    [Export]
    public class LayerController
    {
        private Lazy<VGSShellViewModel> _ShellViewModel;
        private Lazy<VirtualGlobeViewModel> _VirtualGlobeViewModel;
        private LayerService _LayerService;
        private VGSProjectService _VGSProjectService;
        private LayerManagerViewModel _LayerManagerViewModel;
        private IVGSShellService _ShellService;
        private SiteViewModel _SiteViewModel;
        private SFRViewModel _SFRViewModel;

        [ImportingConstructor]
        public LayerController(Lazy<VGSShellViewModel> shellViewModel, Lazy<VirtualGlobeViewModel> vgvm,
            LayerService layerserv, VGSProjectService prj, LayerManagerViewModel layervm, IVGSShellService shell, SiteViewModel sitevm, SFRViewModel sfr)
        {
            _ShellViewModel = shellViewModel;
            _VirtualGlobeViewModel = vgvm;
            _LayerService = layerserv;
            _VGSProjectService = prj;
            _LayerManagerViewModel = layervm;
            _ShellService = shell;
            _SiteViewModel = sitevm;
            _SFRViewModel = sfr;
        }

        public VGSShellViewModel ShellViewModel
        {
            get
            {
                return _ShellViewModel.Value;
            }
        }

        public SiteViewModel SiteViewModel
        {
            get
            {
                return _SiteViewModel;
            }
        }

        public SFRViewModel SFRViewModel
        {
            get
            {
                return _SFRViewModel;
            }
        }

        public void Initialize()
        {
            _ShellService.LayerManager = _LayerManagerViewModel.View;
            ShellViewModel.LoadShpFile = new DelegateCommand(LoadShpFile);
            ShellViewModel.LoadWaterFile = new DelegateCommand(LoadWaterFile);
            ShellViewModel.LoadGridWater = new DelegateCommand(LoadGridWater);
            ShellViewModel.LoadRasterFile = new DelegateCommand(LoadRasterFile);
            ShellViewModel.Load3DModel = new DelegateCommand(Load3DModel);
            ShellViewModel.LoadODM = new DelegateCommand(LoadODM);
            _LayerManagerViewModel.RefreshCommand = new DelegateCommand(Refresh);
            _LayerManagerViewModel.RemoveCommand = new DelegateCommand(Remove);
        }

        public void Run()
        {
            var icons = new ClickableIcons("Observation Sites")
            {
                IsOn = true
            };
            var world = _VirtualGlobeViewModel.Value.VirtualGlobe.CurrentWorld;
            var layer = new RenderableSitesLayer(world, icons);
            layer.ViewSiteDataClicked += layer_SiteClicked;
            layer.SitePropertyClicked += layer_SitePropertyClicked;
            _LayerService.SitesLayer = layer;
            world.DataLayerList.Add(icons);
            _LayerService.RootLayers = world.DataLayerList;
        }

        /// <summary>
        /// Takes the layer the layer manager points at out of the scene. The layers that may go are the
        /// ones put into the Feature Layers group, which is where the layers added to the project go;
        /// the rest of the scene, the terrain and the model among it, belongs to nobody's choice. The
        /// entry of the layer in the project goes with it, so that saving the project afterwards does
        /// not write the layer back into it.
        /// </summary>
        private void Remove(object obj)
        {
            var ro = obj as RenderableObject;
            if (ro == null)
                return;

            var project = _VGSProjectService.Project as Heiflow3DProject;
            var features = _LayerService.FeatureLayerGroup;
            if (features != null && !_LayerService.IsInFeatureLayerGroup(ro))
                return;

            var provider = _LayerService.ProviderOf(ro);
            var owner = ro.ParentList;
            if (owner != null)
                owner.Remove(ro);
            else if (features != null)
                features.Remove(ro);

            if (project != null)
            {
                project.RemoveProvider(provider);
                _LayerService.ForgetProvider(ro);
            }
            Refresh(null);
        }


        private void Refresh(object obj)
        {
            (_ShellService.LayerManager as ILayerManagerView).Refresh();
        }
        private void layer_SiteClicked(object sender, Core.Data.ODM.Site e)
        {
            _ShellService.SelectPanel(DockPanelNames.SitePanel);
            _ShellService.SiteView.Site = e;
        }

        private void layer_SitePropertyClicked(object sender, Site e)
        {
            _ShellService.SelectPanel(DockPanelNames.PropertyPanel);
            _ShellService.PropertyView.SelectedObject = e;
        }

        /// <summary>
        /// The same shapefile LoadShpFile takes, drawn as water with waves running across it, see
        /// WaterSurfaceLayer. A shapefile of lines or of points is drawn the way it always is.
        /// </summary>
        private void LoadWaterFile(object para)
        {
            ShapeFeatureProvider provider = new ShapeFeatureProvider();
            provider.WaterSurface = true;
            // Water lies on its own level and not ten metres above the ground, which is what a
            // vector is given so that it does not sink into the terrain.
            provider.DistanceAboveSurface = 0;
            var ro = provider.Load(para.ToString(), _ShellService.VirtualGlobeView.VirtualGlobe.CurrentWorld);
            AddLayer(ro, provider, para.ToString());
            Refresh(null);
        }

        /// <summary>
        /// The triangular grid of the model of the project that is open, drawn once more as a sheet
        /// of water with waves running across it, see TriangularWaterLayer.
        ///
        /// It is a copy of the grid and not the grid itself, the geometry being read out of the model
        /// and kept by the new layer, so the mesh the model is drawn with is left as it is and the
        /// water is a layer of its own beside it. No file is asked for, the grid being the one the
        /// model already holds.
        /// </summary>
        private void LoadGridWater(object para)
        {
            var project = _VGSProjectService.Project;
            var grid = project != null && project.Model != null
                ? project.Model.Grid as TriangularGrid
                : null;
            if (grid == null)
            {
                // Nothing at all happens without a word otherwise, and a click on the menu that
                // brings up nothing looks like a menu that is broken.
                Heiflow.Core.Log.Write(Heiflow.Core.Log.Levels.Warning, "WATR",
                    "There is no triangular grid to draw as water: either no project is open, or the"
                    + " grid of the model of it is not a triangular one.");
                return;
            }

            var world = _ShellService.VirtualGlobeView.VirtualGlobe.CurrentWorld;
            var layer = new Heiflow.Visualization.Renderable.Grid.TriangularWaterLayer(
                "Water surface", world, grid);
            // Water lies on its own level and not ten metres above the ground, which is what a
            // vector is given so that it does not sink into the terrain.
            layer.DistanceAboveSurface = 0;

            if (_LayerService.ModelLayerGroup != null)
            {
                _LayerService.ModelLayerGroup.Add(layer);
            }
            else
            {
                _LayerService.RootLayers.Add(layer);
            }
            Refresh(null);
        }

        private void LoadShpFile(object para)
        {
            ShapeFeatureProvider provider = new ShapeFeatureProvider();
            var ro = provider.Load(para.ToString(), _ShellService.VirtualGlobeView.VirtualGlobe.CurrentWorld);
            AddLayer(ro, provider, para.ToString());
            Refresh(null);
        }

        /// <summary>
        /// Puts a layer that has just been loaded into the scene, and, a 3D project being open, into
        /// the project as well, so that saving the project writes the layer into the project file and
        /// opening the project brings the layer back. The file of the layer is kept as a path relative
        /// to the project file, and the way the layer is drawn goes into the project with it.
        /// </summary>
        private void AddLayer(RenderableObject ro, ShapeFeatureProvider provider, string fileName)
        {
            var project = _VGSProjectService.Project as Heiflow3DProject;
            if (project != null)
            {
                provider.BaseDirectory = project.AbsolutePathToProjectFile;
                provider.RelativeFileName = project.ToRelativeFileName(fileName);
                provider.ShowAtStartup = true;
                provider.IsOn = true;
                project.AddShapeFeatureProvider(provider);
                _LayerService.RememberProvider(ro, provider);
            }
            if (ro == null)
                return;

            if (_VGSProjectService.Project != null && _LayerService.FeatureLayerGroup != null)
            {
                _LayerService.FeatureLayerGroup.Add(ro);
            }
            else
            {
                _LayerService.RootLayers.Add(ro);
            }
        }

        private void Load3DModel(object para)
        {
            ModelFeatureProvider provider = new ModelFeatureProvider();
            var ro = provider.Load(para.ToString(), _ShellService.VirtualGlobeView.VirtualGlobe.CurrentWorld);
            if (_VGSProjectService.Project != null)
            {
                _LayerService.FeatureLayerGroup.Add(ro);
            }
            else
            {
                _LayerService.RootLayers.Add(ro);
            }
            Refresh(null);
        }
        private void LoadODM(object para)
        {
            ODMSource source = new ODMSource();
            string msg = "";
            if (source.Open(para.ToString(), ref msg))
            {
                _LayerService.ODMSource = source;
                var sites = source.GetSites(new QueryCriteria());
                _LayerService.SitesLayer.ShowSites(sites);
            }
        }
        /// <summary>
        /// Draws a raster file, a GeoTIFF or a picture with a world file among them, on the ground of
        /// the scene. What comes out of the file is a picture and the patch of ground it belongs to,
        /// see RasterDrawing, and those two are what an ImageLayer stretches over that patch: the
        /// picture as its texture, its four sides as the four sides of the layer. It is given no
        /// altitude above the ground, and it is handed the terrain accessor of the world, so it lies
        /// on the terrain and follows it.
        ///
        /// Unlike the other layers that are added, a raster is not written into the project file: no
        /// provider of the project draws one yet, and a layer put into the project without a provider
        /// would come back as nothing the next time the project is opened. The layer is therefore one
        /// of the session, and goes when the scene goes.
        /// </summary>
        private void LoadRasterFile(object para)
        {
            var fileName = para as string;
            if (string.IsNullOrEmpty(fileName))
                return;

            string whyNot;
            var drawing = RasterDrawing.Load(fileName, out whyNot);
            if (drawing == null)
            {
                // The menu is the only way in, and a click on it that brings up nothing looks like a
                // menu that is broken, hence the word in the log.
                Heiflow.Core.Log.Write(Heiflow.Core.Log.Levels.Warning, "RAST",
                    "The raster was not drawn. " + whyNot);
                return;
            }

            var world = _ShellService.VirtualGlobeView.VirtualGlobe.CurrentWorld;
            var layer = new ImageLayer(Path.GetFileNameWithoutExtension(fileName), world, 0,
                drawing.BitmapFile, drawing.South, drawing.North, drawing.West, drawing.East,
                1.0, world.TerrainAccessor)
            {
                RenderPriority = RenderPriority.SurfaceImages,
                IsOn = true
            };

            if (_VGSProjectService.Project != null && _LayerService.FeatureLayerGroup != null)
            {
                _LayerService.FeatureLayerGroup.Add(layer);
            }
            else
            {
                _LayerService.RootLayers.Add(layer);
            }
            Refresh(null);
        }
    }
}
