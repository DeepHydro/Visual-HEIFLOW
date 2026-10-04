using DotSpatial.Projections;
using Heiflow.Core;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.UI;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Presentation.Animation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Controls.Project;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable;
using Heiflow.Visualization.Renderable.Grid;
using Heiflow.Visualization.Studio.Controls;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Heiflow.Visualization.Studio
{
    public class VisAppManager : IApplication
    {
        public static readonly ProjectionInfo Wgs84Proj = ProjectionInfo.FromEsriString(KnownCoordinateSystems.Geographic.World.WGS1984.ToEsriString());

        public VisAppManager(World world)
        {
            ProjectManager = new ProjectManager()
            {
                AppMode = AppMode.HF3D
            };
            CurrentWorld = world;
            ChildWindows = new List<IWindow>();
        }

        private RenderableObjectList _GridLayers;
        private GridAnimation _GridAnimation;
        private List<IModelRender> _GridRenders = new List<IModelRender>();
        private IModelRenderDX _ModelRenderDX;

        public static VisAppManager Default
        {
            get;
            set;
        }
        //public IProjectManager ProjectManager
        //{
        //    get;
        //    private set;
        //}
        public World CurrentWorld
        {
            get;
            private set;
        }

        public RenderableObjectList GridLayers
        {
            get
            {
                return _GridLayers;
            }
        }
        public string ApplicationPath
        {
            get;
            set;
        }

        public ODMSource ODMSource
        {
            get;
            private set;
        }

        public IModelRenderDX SelectedRender
        {
            get
            {
                return _ModelRenderDX;
            }
            set
            {
                _ModelRenderDX = value;
                _GridAnimation = new GridAnimation(_ModelRenderDX);
                MainWindow.SceneWindow.ModelRenderDX = _ModelRenderDX;
            }
        }

        public RenderableSitesLayer RenderableSitesLayer
        {
            get;
            private set;
        }

        public MFVectorRender RenderableMFVector
        {
            get;
            private set;
        }

        public MainWindow MainWindow
        {
            get;
            set;
        }

        public List<IWindow> ChildWindows
        {
            get;
            set;
        }

        public bool CheckLicense()
        {
            bool licensed = false;
            if (RegisterOperator.IsExist("HEIFLOW3D"))
            {
                string key = RegisterOperator.GetRegistData("HEIFLOW3D");
                if (key == "ptgx5-ecio8-3dsd5-rpge4")
                {
                    licensed = true;
                }
            }
            return licensed;
        }

        public void OpenODMDB()
        {
            var prj = ProjectManager.Project as Heiflow3DProject;
            foreach (var ly in prj.ODMProvider)
            {
                string filename = "";
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
                    source.Open(filename);
                    ODMSource = source;
                    RenderableSitesLayer.ODMSource = source;
                }
            }
        }

        public void CloseDB()
        {
            if (ODMSource != null && ODMSource.ODMDB.DbConnection != null)
            {
                ODMSource.Close();
            }
        }

        public void LoadFeatureLayers()
        {
            var prj = ProjectManager.Project as Heiflow3DProject;
            foreach (var ly in prj.ShapeFeatureProvider)
            {
                string filename = "";
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                    ly.FullFileName = filename;
                    var ro = ly.Load(filename, CurrentWorld);
                    ro.RenderPriority = RenderPriority.SurfaceImages;
                    CurrentWorld.DataLayerList.Add(ro);
                }
            }
            foreach (var ly in prj.FlagFeatureProvider)
            {
                string filename = "";
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                    ly.FullFileName = filename;
                    var ro = ly.Load(filename, CurrentWorld);
                    ro.RenderPriority = RenderPriority.Icons;
                    CurrentWorld.DataLayerList.Add(ro);
                }
            }
        }

        public void LoadRasterLayers()
        {
            var prj = ProjectManager.Project as Heiflow3DProject;
            foreach (var ly in prj.RenderableRasterProvider)
            {
                string filename = "";
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                    ly.FullFileName = filename;
                    var ro = ly.Load(filename, CurrentWorld);
                    CurrentWorld.DataLayerList.Add(ro);
                }
            }
        }

        public void Load3DModelFeatures()
        {
            var prj = ProjectManager.Project as Heiflow3DProject;
            foreach (var ly in prj.ModelFeatureProvider)
            {
                string filename = "";
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                    ly.FullFileName = filename;
                    var ro = ly.Load(filename, CurrentWorld);
                    CurrentWorld.DataLayerList.Add(ro);
                }
            }
        }

        public void LoadModelGrid()
        {
            //TODO: use composition method to find renderable grid layers rather than hard coding

            _GridLayers = new RenderableObjectList(ProjectManager.Project.Name)
            {
                RenderPriority = RenderPriority.LinePaths
            };
            _GridRenders.Clear();

            var model = ProjectManager.Project.Model;

            if (model is Heiflow.Models.Integration.HeiflowModel)
            {
                LoadHeiflowModelLayers(model as Heiflow.Models.Integration.HeiflowModel);
            }
            else if (model is Heiflow.Models.Subsurface.Modflow)
            {
                LoadMFModelLayers(model as Heiflow.Models.Subsurface.Modflow);
            }
            else if (model is Heiflow.Models.GHM.GHModel)
            {
                LoadGHModelLayers(model as Heiflow.Models.GHM.GHModel);
            }
            //Initialize
            foreach (var ly in _GridRenders)
            {
                ly.Initilize();
            }

            CurrentWorld.DataLayerList.Add(_GridLayers);

        }

        private void LoadHeiflowModelLayers(Heiflow.Models.Integration.HeiflowModel heiflow)
        {
            System.Drawing.Color color = System.Drawing.Color.Blue;
            var prms = heiflow.PRMSModel;
            var mf = heiflow.ModflowModel;
            LoadMFModelLayers(mf);
            LoadPRMSModelLayers(prms);
        }

        private void LoadMFModelLayers(Heiflow.Models.Subsurface.Modflow mf)
        {
            int utm_zone=(ProjectManager.Project as Heiflow3DProject).UTMZone;
            double distanceAbove = 200;
            (mf.Grid as MFGrid).UTMZone = (ProjectManager.Project as Heiflow3DProject).UTMZone;
            SelectedRender = new MFGridRender()
            {
                EquatorialRadius = CurrentWorld.EquatorialRadius,
                Owner = mf,
                Manager = ProjectManager,
                UTMZone = utm_zone
            };

            var renderableGridLayer = new RenderableModelLayer("Grid", CurrentWorld, 0, 10000, distanceAbove, SelectedRender);
            _GridRenders.Add(SelectedRender);
            _GridLayers.Add(renderableGridLayer);
            _GridAnimation = new GridAnimation(ProjectManager, SelectedRender);

            //GridValueRender
            //MODFLOW
            foreach (var pck in mf.Packages)
            {
                if (pck.Value is IRenderablePackage)
                {
                    (pck.Value as IRenderablePackage).GridRender = SelectedRender;
                }
                foreach (var chi in pck.Value.Childs)
                {
                    if (chi is IRenderablePackage)
                    {
                        (chi as IRenderablePackage).GridRender = SelectedRender;
                    }
                }
            }
            foreach (var pck in mf.Packages)
            {
                if (pck.Value is SFRDataPackage)
                {
                    var sfrw = new SFRWindow(pck.Value as SFRDataPackage)
                    {
                        Owner = MainWindow
                    };
                    (pck.Value as IShowable).Window = sfrw;
                    ChildWindows.Add(sfrw);
                }
            }
            
            //Animator
            foreach (var pck in mf.Packages)
            {
                if (pck.Value is IAnimatablePackage)
                {
                    (pck.Value as IAnimatablePackage).Animator = _GridAnimation;
                }
                foreach (var chi in pck.Value.Childs)
                {
                    if (chi is IAnimatablePackage)
                    {
                        (chi as IAnimatablePackage).Animator = _GridAnimation;
                    }
                }
            }

            //SFR
            if (mf.Packages.ContainsKey("SFR"))
            { 
                var rn = (mf.Packages["SFR"] as SFRPackage).RiverNetwork;
                var riverGrid = new RiverGridRender(rn)
                {
                    EquatorialRadius = CurrentWorld.EquatorialRadius,
                    Owner = mf,
                    Manager = ProjectManager,
                    UTMZone = utm_zone
                };
                var riverGridLayer = new RenderableModelLayer("River Network", CurrentWorld, 0, 10000, distanceAbove+20, riverGrid);
                _GridRenders.Add(riverGrid);
                foreach (var pck in mf.Packages)
                {
                    if (pck.Value is SFRDataPackage)
                    {
                        (pck.Value as SFRDataPackage).GridRender = riverGrid;
                        break;
                    }
                }
                var riverGridAni = new GridAnimation(ProjectManager, riverGrid);
                foreach (var pck in mf.Packages)
                {
                    if (pck.Value is SFRDataPackage)
                    {
                        (pck.Value as IAnimatablePackage).Animator = riverGridAni;
                        break;
                    }
                }
                _GridLayers.Add(riverGridLayer);
            }

            //Velocity
            if (mf.Packages.ContainsKey(CBCPackage.CBCName))
            {
                var vel = (mf.Packages[VelocityPackage.VelocityPackageName] as VelocityPackage);
                var velGrid = new MFVectorRender()
                {
                    EquatorialRadius = CurrentWorld.EquatorialRadius,
                    Owner = mf,
                    Manager = ProjectManager,
                    UTMZone = utm_zone
                };
                var velGridLayer = new RenderableModelLayer("Velocity Field", CurrentWorld, 0, 10000, distanceAbove, velGrid);
                RenderableMFVector = velGrid;
                _GridRenders.Add(velGrid);
                _GridLayers.Add(velGridLayer);

                foreach (var pck in mf.Packages)
                {
                    if (pck.Value is VelocityPackage)
                    {
                        (pck.Value as VelocityPackage).GridRender = velGrid;
                        break;
                    }
                }
                var veclGridAni = new GridAnimation(ProjectManager, velGrid);
                foreach (var pck in mf.Packages)
                {
                    if (pck.Value is VelocityPackage)
                    {
                        (pck.Value as IAnimatablePackage).Animator = veclGridAni;
                        break;
                    }
                }
            }

            //Profile
            GridProfileLayer profile = new GridProfileLayer(SelectedRender);      
            _GridLayers.Add(profile);

            // FHB
            if (mf.Packages.ContainsKey("FHB"))
            {
                foreach (var pck in mf.Packages)
                {
                    if (pck.Value is FHBPackage)
                    {
                        RenderableMFWellLayer fhb_layer = new RenderableMFWellLayer()
                        {
                            Name = "FHB",
                            World = CurrentWorld,
                            MaxDisplayAltitude = 10000,
                            MinDisplayAltitude = 0,
                            DistanceAboveSurface = distanceAbove,
                        };

                        MFWellRender fhb_render = new MFWellRender()
                        {
                            FHBPackage = pck.Value as FHBPackage,
                            MFGrid = (mf.Grid as MFGrid),
                            Manager = ProjectManager,
                            EquatorialRadius=CurrentWorld.EquatorialRadius,
                            Layer= fhb_layer,
                            UTMZone = utm_zone,
                            Scale=0.2f,
                            MaxBarHeight=5000
                        };
                        fhb_layer.RenderDX = fhb_render;
                        _GridRenders.Add(fhb_render);
                        _GridLayers.Add(fhb_layer);
                        break;
                    }
                }
            }

            //WEL
            if (mf.Packages.ContainsKey("WEL"))
            {
                foreach (var pck in mf.Packages)
                {
                    if (pck.Value is WELPackage)
                    {
                        RenderableMFWellLayer wel_layer = new RenderableMFWellLayer()
                        {
                            Name = "WEL",
                            World = CurrentWorld,
                            MaxDisplayAltitude = 10000,
                            MinDisplayAltitude = 0,
                            DistanceAboveSurface = distanceAbove,
                        };
                        MFWellRender wel_render = new MFWellRender()
                        {
                            WELPackage = pck.Value as WELPackage,
                            MFGrid = (mf.Grid as MFGrid),
                            Manager = ProjectManager,
                            EquatorialRadius = CurrentWorld.EquatorialRadius,
                            Layer = wel_layer,
                            UTMZone = utm_zone,
                            Scale = 0.0f,
                            MaxBarHeight = 5000
                        };
                        wel_layer.RenderDX = wel_render;
                        _GridRenders.Add(wel_render);
                        _GridLayers.Add(wel_layer);
                        break;
                    }
                }
            }
        }

        private void LoadGHModelLayers(Heiflow.Models.GHM.GHModel mf)
        {
            string assembly = Path.Combine(ApplicationPath, "Heiflow.Visualization.dll");

            foreach(var layer in mf.MasterPackage.Serializer.Layers)
            {
                var render = Activator.CreateInstanceFrom(assembly, layer.RenderName).Unwrap() as IModelRenderDX;
                render.EquatorialRadius = CurrentWorld.EquatorialRadius;
                render.Owner = mf;
                render.Manager = ProjectManager;
                render.UTMZone = (ProjectManager.Project as Heiflow3DProject).UTMZone;

                var obj_render_layer = Activator.CreateInstanceFrom(assembly, render.SupportedLayer);
                var render_layer = obj_render_layer.Unwrap() as RenderableModelObject;
                render_layer.Name = layer.Name;
                render_layer.DistanceAboveSurface = layer.DistanceAboveSurface;
                render_layer.MaxDisplayAltitude = layer.MaxDisplayAltitude;
                render_layer.MinDisplayAltitude = layer.MinDisplayAltitude;
                render_layer.World = CurrentWorld;
                render_layer.RenderDX = render;
                render.Layer = render_layer;

                foreach(var mem in layer.Members)
                {
                    foreach (var item in mem.Items)
                    {
                        item.ModelRender = render;
                    }
                }
            
                _GridRenders.Add(render);
                _GridLayers.Add(render_layer);
                SelectedRender = render;
            }
            _GridAnimation = new GridAnimation(ProjectManager, SelectedRender);

        }

        private void LoadPRMSModelLayers(PRMS prms)
        {
            //PRMS
            foreach (var pck in prms.Packages)
            {
                if (pck.Value is IRenderablePackage)
                {
                    (pck.Value as IRenderablePackage).GridRender = SelectedRender;
                }
                foreach (var chi in pck.Value.Childs)
                {
                    if (chi is IRenderablePackage)
                    {
                        (chi as IRenderablePackage).GridRender = SelectedRender;
                    }
                }
            }


            foreach (var pck in prms.Packages)
            {
                if (pck.Value is IAnimatablePackage)
                {
                    (pck.Value as IAnimatablePackage).Animator = _GridAnimation;
                    break;
                }
                foreach (var chi in pck.Value.Childs)
                {
                    if (chi is IAnimatablePackage)
                    {
                        (chi as IAnimatablePackage).Animator = _GridAnimation;
                    }
                }
            }
        }

        public void LoadSitesLayer()
        {
            var icons = new ClickableIcons("Observation Sites")
            {
                IsOn = true
            };
            RenderableSitesLayer = new RenderableSitesLayer(CurrentWorld, icons);
            CurrentWorld.DataLayerList.Add(icons);
        }

        public void LoadLanguageFile(string languagefileName)
        {
            System.Windows.Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
            {
                Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
            };
        }
    }
}