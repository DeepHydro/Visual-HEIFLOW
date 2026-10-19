using Heiflow.Core;
using Heiflow.Core.Plugin;
using Heiflow.Presentation.Services;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Display;
using HUST.WREIS.Dot3D.Display.Plugins;
using HUST.WREIS.Dot3D.Renderable;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Threading;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

namespace Heiflow.Visualization.Applications
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class VirtualGlobeController
    {
        private Lazy<VGSShellViewModel> _ShellViewModel;
        private AnimationPlayerViewModel _AnimViewModel;
        private VirtualGlobeViewModel _VirtualGlobeViewModel;
        private SymbologyViewModel _SymbologyViewModel;
        private RunModelViewModel _RunModelViewModel;
        private IVGSShellService _VGSShellService;
        private LayerService _LayerService;
        private PluginCompiler _PluginCompiler;
        private HUST.WREIS.Dot3D.SceneWindow _VirtualGlobe;
        private DispatcherTimer _Timer3d;
        private bool _RenderLoopRunning = false;
        private IActiveDataService _ActiveDataService;
        // private System.Windows.Forms.Timer _Timer3d;

        [ImportingConstructor]
        public VirtualGlobeController(VirtualGlobeViewModel vgvm, IVGSShellService shell, LayerService ls, 
            Lazy<VGSShellViewModel> shellViewModel,AnimationPlayerViewModel anim_vm, SymbologyViewModel sym_vm, RunModelViewModel rm_vm,
            IActiveDataService dataservice)
        {
            _VirtualGlobeViewModel = vgvm;
            _VGSShellService = shell;
            _LayerService = ls;
            _ShellViewModel = shellViewModel;
            _AnimViewModel = anim_vm;
            _SymbologyViewModel = sym_vm;
            _RunModelViewModel = rm_vm;
            _ActiveDataService = dataservice;
        }

        public void Initialize()
        {
            _Timer3d = new DispatcherTimer();
            _Timer3d.Interval = new TimeSpan(0, 0, 0, 0, 15);
            _Timer3d.Tick += Timer3d_Tick;
            _VirtualGlobeViewModel.ViewLoaded += VirtualGlobeViewModel_ViewLoaded;
            // The shell service no longer carries a timer, it carries this flag instead, and it
            // turns it off when the shell closes, so the loop has to honour it.
            _VGSShellService.IsRenderingEnabled = false;
            _ShellViewModel.Value.ViewCommand = new DelegateCommand(OnViewCommand);
            _ShellViewModel.Value.MeasureCommand = new DelegateCommand(OnMeasureCommand);
            _ShellViewModel.Value.ScreenShotCommand = new DelegateCommand(OnScreenShotCommand);
        }

        public void Run()
        {

        }

        public void ShutDown()
        {
            _RenderLoopRunning = false;
            _VGSShellService.IsRenderingEnabled = false;
            System.Windows.Media.CompositionTarget.Rendering -= OnWpfRendering;
            _Timer3d.Stop();
        }

        private void VirtualGlobeViewModel_ViewLoaded(object sender, EventArgs e)
        {
            _VirtualGlobe = (_VGSShellService.VirtualGlobeView as IVirtualGlobeView).VirtualGlobe;
            _VirtualGlobe.ActiveDataService = _ActiveDataService;
            _VirtualGlobe.ShellService = _VGSShellService;
            _VirtualGlobe.SetResManager();
            _VirtualGlobe.CurrentWorld = ConfigurationManager.Load(ConfigurationManager.SettingsPath + "\\Layers.xml", _VirtualGlobe.Cache, ConfigurationManager.Engine3DSettings);
            LoadDefaultLayers();
            _VirtualGlobe.Render();
            _VirtualGlobe.Focus();
            _VirtualGlobe.GotoLatLonAltitude(World.Settings.DefalutLatitude, World.Settings.DefalutLongitude, 19000000.0f);
            ConfigurationManager.DrawArgs = _VirtualGlobe.DrawArgs;

            // see class LayerProfiler in World.cs, remove when the profiling is done
            if (HUST.WREIS.Dot3D.LayerProfiler.On)
                HookUiProfiling();

            _RenderLoopRunning = true;
            _VGSShellService.IsRenderingEnabled = true;
            System.Windows.Media.CompositionTarget.Rendering += OnWpfRendering;
            // Heartbeat for when WPF stops composing, see OnWpfRendering.
            _Timer3d.Start();
            InitializePluginCompiler();

            _VirtualGlobe.CurrentWorld.DefaultLayerList.FindChild("SkyGradient").IsOn = World.Settings.EnableAtmosphere;

            _LayerService.Layers = _VirtualGlobe.CurrentWorld.DataLayerList;
            (_VGSShellService.LayerManager as ILayerManagerView).Refresh();

            _ShellViewModel.Value.WorldSettings = World.Settings;
        }

        /// <summary>see class LayerProfiler in World.cs, remove when the profiling is done</summary>
        static long lastFrameStartTicks = 0;

        /// <summary>How far apart two CompositionTarget.Rendering ticks are, see RenderFrame.</summary>
        static long lastWpfFrameTicks = 0;

        /// <summary>see class LayerProfiler in World.cs, remove when the profiling is done</summary>
        static long wpfFrameIntervalTicks = 0;

        /// <summary>see class LayerProfiler in World.cs, remove when the profiling is done</summary>
        static long uiOpStartTicks = 0;

        /// <summary>see class LayerProfiler in World.cs, remove when the profiling is done</summary>
        static int uiOpPriority = -1;

        /// <summary>see class LayerProfiler in World.cs, remove when the profiling is done</summary>
        static long lastCompositionTicks = 0;

        /// <summary>
        /// see class LayerProfiler in World.cs, remove when the profiling is done.
        /// The render loop is a DispatcherTimer at Normal priority, and Normal comes after DataBind,
        /// Render, Loaded and Input. If the shell keeps the dispatcher busy with any of those the
        /// tick is late and the globe slows down, so every dispatcher operation is timed and filed
        /// under its priority: 5 Input, 6 Loaded, 7 Render, 8 DataBind, 9 Normal (our tick).
        /// </summary>
        void HookUiProfiling()
        {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.Hooks.OperationStarted += delegate (object s, DispatcherHookEventArgs e)
            {
                uiOpPriority = (int)e.Operation.Priority;
                uiOpStartTicks = Stopwatch.GetTimestamp();
            };
            dispatcher.Hooks.OperationCompleted += delegate (object s, DispatcherHookEventArgs e)
            {
                if (uiOpStartTicks == 0)
                    return;
                HUST.WREIS.Dot3D.LayerProfiler.RecordRender("UI dispatcher priority " + uiOpPriority,
                    Stopwatch.GetTimestamp() - uiOpStartTicks);
                uiOpStartTicks = 0;
            };
            dispatcher.Hooks.OperationAborted += delegate (object s, DispatcherHookEventArgs e)
            {
                uiOpStartTicks = 0;
            };

            // How often does WPF compose a frame of its own? One line per WPF frame, so ms/call is
            // the WPF frame interval: ~16 ms means WPF is animating at 60 fps on this thread.
            System.Windows.Media.CompositionTarget.Rendering += delegate
            {
                long now = Stopwatch.GetTimestamp();
                if (lastCompositionTicks != 0)
                    HUST.WREIS.Dot3D.LayerProfiler.RecordRender("WPF: ms between composition frames",
                        now - lastCompositionTicks);
                lastCompositionTicks = now;
            };
        }

        /// <summary>
        /// This is the real render loop. A DispatcherTimer at 15 ms managed 9 frames a second and
        /// ComponentDispatcher.ThreadIdle managed 11, while a frame costs 4 to 30 ms: neither is
        /// reliably woken while WPF keeps composing. CompositionTarget.Rendering is WPF's own frame
        /// clock and does fire every time, ~55 Hz here, so that is what drives rendering now, with
        /// the timer kept as a heartbeat for when WPF stops composing. ThrottleFpsHz caps the rate
        /// and is checked here, so several drivers can call this without rendering twice.
        /// </summary>
        private void OnWpfRendering(object sender, EventArgs e)
        {
            long now = Stopwatch.GetTimestamp();
            if (lastWpfFrameTicks != 0)
                wpfFrameIntervalTicks = now - lastWpfFrameTicks;
            lastWpfFrameTicks = now;

            RenderFrame();
        }

        private void Timer3d_Tick(object sender, EventArgs e)
        {
            RenderFrame();
        }

        private void RenderFrame()
        {
            if (!_RenderLoopRunning || !_VGSShellService.IsRenderingEnabled)
                return;

            int fps = World.Settings.ThrottleFpsHz;
            long now = Stopwatch.GetTimestamp();
            if (fps > 0 && lastFrameStartTicks != 0)
            {
                // CompositionTarget.Rendering paces itself to the display, ~18 ms here, and
                // waiting for the whole target on top of that drops every other frame: a 20 ms
                // target with an 18 ms tick renders every 36 ms, 27 fps instead of 50. So a
                // frame is only held back while the next tick would still arrive early.
                long target = Stopwatch.Frequency / fps;
                if (now - lastFrameStartTicks + wpfFrameIntervalTicks / 2 < target)
                    return;
            }

            try
            {
                if (_VirtualGlobe.m_Device3d == null)
                {
                    //  e.Graphics.Clear(SystemColors.ControlColor);
                    return;
                }

                // see class LayerProfiler in World.cs, remove the profiling when the question is answered.
                bool profiled = HUST.WREIS.Dot3D.LayerProfiler.On;
                long frameStart = now;
                if (lastFrameStartTicks != 0 && profiled)
                    HUST.WREIS.Dot3D.LayerProfiler.RecordRender("frame: wall clock since last frame",
                        frameStart - lastFrameStartTicks);
                lastFrameStartTicks = frameStart;

                // to prevent screen garbage when resizing
                _VirtualGlobe.Render();

                long presentStart = profiled ? Stopwatch.GetTimestamp() : 0;
                _VirtualGlobe.m_Device3d.Present();
                if (profiled)
                    HUST.WREIS.Dot3D.LayerProfiler.RecordRender("device.Present (after Render)",
                        Stopwatch.GetTimestamp() - presentStart);

                // Everything measured above adds up to a few ms, yet a frame takes far longer. The
                // rest is other work the UI thread does between frames, so time the frame itself and
                // attribute whatever the dispatcher runs, see HookUiProfiling.
                if (profiled)
                    HUST.WREIS.Dot3D.LayerProfiler.RecordRender("frame: render frame total",
                        Stopwatch.GetTimestamp() - frameStart);
            }
            catch (Exception)
            {
                try
                {
                    _VirtualGlobe.AttemptRecovery();
                    // Our surface was lost, force re-render
                    _VirtualGlobe.Render();
                    _VirtualGlobe.m_Device3d.Present();
                }
                catch (Exception)
                {
                    // Ignore a 2nd failure
                }
            }
        }

        private void LoadDefaultLayers()
        {
            var ww = (_VGSShellService.VirtualGlobeView as IVirtualGlobeView).VirtualGlobe;
            string imgpath = ConfigurationManager.ApplicationPath + @"\Resources\Images\wsiearth.JPG";
            string pluginDirectory = ConfigurationManager.ApplicationPath + @"\Plugins\stars3d\";
            RenderableObjectList roList = ww.CurrentWorld.DefaultLayerList;

            Stars3DLayer starLayer = new Stars3DLayer("Starfield", pluginDirectory, ww.CurrentWorld, ww.DrawArgs);
            starLayer.IsOn = true;
            roList.Add(starLayer);

            pluginDirectory = ConfigurationManager.ApplicationPath + @"\Plugins\SkyGradient\";
            FileInfo SettingsFile = new FileInfo(System.IO.Path.Combine(pluginDirectory, ww.CurrentWorld.Name + ".ini"));
            if (SettingsFile.Exists)
            {
                Murris.Plugins.SkyGradientLayer skyGradienLayer = new Murris.Plugins.SkyGradientLayer("SkyGradient", pluginDirectory, ww.CurrentWorld, ww.DrawArgs);
                skyGradienLayer.IsOn = true;
                roList.Add(skyGradienLayer);
            }

            pluginDirectory = ConfigurationManager.ApplicationPath + @"\Plugins\LensFlare\";
            LensFlareEffect flare = new LensFlareEffect(pluginDirectory);
            roList.Add(flare);

            pluginDirectory = ConfigurationManager.ApplicationPath + "\\Plugins";
            SkyLayer sky = new SkyLayer("Sky",pluginDirectory,ww);
            sky.IsOn = World.Settings.EnableSky;
            roList.Add(sky);

            ww.CurrentWorld.RenderableObjects.ChildObjects.Add(roList);

            ImageLayer ilayer = new ImageLayer("Land Surface", ww.CurrentWorld, 0, imgpath, -90, 90, -180, 180, 1.0, null);
            ilayer.RenderPriority = RenderPriority.SurfaceImages;
            ilayer.IsOn = true;
            ilayer.IconImagePath = ConfigurationManager.Engine3DSettings.IconPath + "DefaultLayerIcon.png";
            ww.CurrentWorld.DataLayerList.ChildObjects.Insert(0, ilayer);

            //World.Settings.CameraHasMomentum = false;
            //World.Settings.CameraHasInertia = false;
            ww.CurrentWorld.DataLayerList.PropertyChanged += DataLayerList_PropertyChanged;
        }

        private void DataLayerList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ChildObjects")
            {
                //RefreshLayerManager();
            }
        }

        private void InitializePluginCompiler()
        {
               var ww = (_VGSShellService.VirtualGlobeView as IVirtualGlobeView).VirtualGlobe;
            Log.Write(Log.Levels.Debug, "CONF", "initializing plugin compiler...");
            //this.splashScreen.SetText("Initializing plugins...");
            string pluginRoot = System.IO.Path.Combine(ConfigurationManager.ApplicationPath, "Plugins");
            _PluginCompiler = new PluginCompiler(VGSManager.Instance as IApplication, pluginRoot);

            //#if DEBUG
            // Search for plugins in this application (plugin development/debugging aid)
            //  compiler.FindPlugins(Assembly.GetExecutingAssembly());
            //#endif
            PluginInfo compass = new PluginInfo()
            {
                Name = "Compass",
                Plugin = new Compass3D(_VirtualGlobe),
                IsLoadedAtStartup = true,
            };
            //PluginInfo sky = new PluginInfo()
            //{
            //    Name = "Sky",
            //    Plugin = new Sky(ww),
            //    IsLoadedAtStartup = true,
            //};
            PluginInfo fog = new PluginInfo()
            {
                Name = "Fog",
                Plugin = new Fog(_VirtualGlobe),
                IsLoadedAtStartup = true,
            };

            PluginInfo chart = new PluginInfo()
            {
                Name = "Monitor",
                Plugin = new GraphChart3D(_VirtualGlobe)
                {
                    Name = "Monitor",
                    Caption = "Monitor",
                    Location = new System.Drawing.Point(10, _VirtualGlobe.Height - 320),
                    //  MenuItem = miShowChart
                },
                IsLoadedAtStartup = true,
            };


            PluginInfo lengend = new PluginInfo()
            {
                Name = "GridLengendBar",
                Plugin = new GridLengendBar(_VirtualGlobe),
                IsLoadedAtStartup = true,
            };
            _ShellViewModel.Value.ShellService.Legend = lengend.Plugin as GridLengendBar;

            PluginInfo scaleBar = new PluginInfo()
            {
                Name = "ScaleBar",
                Plugin = new ScaleBarLegend(_VirtualGlobe)
                {
                    //MenuItem = miShowScaleBar 
                },
                IsLoadedAtStartup = true
            };

            PluginInfo toolBar = new PluginInfo()
            {
                Name = "ToolBar",
                Plugin = new HUST.WREIS.Dot3D.Display.ToolBar(_VirtualGlobe),
                IsLoadedAtStartup = true,
            };
            PluginInfo measure = new PluginInfo()
            {
                Name = "MeasureTool",
                Plugin = new HUST.WREIS.Dot3D.Display.MeasureTool(_VirtualGlobe),
                IsLoadedAtStartup = true,
            };


            _PluginCompiler.Plugins.Add(compass);
       //     _PluginCompiler.Plugins.Add(sky);
            _PluginCompiler.Plugins.Add(fog);
            _PluginCompiler.Plugins.Add(chart);
            _PluginCompiler.Plugins.Add(lengend);
            _PluginCompiler.Plugins.Add(scaleBar);
            //  pluginCompiler.Plugins.Add(toolBar);
            _PluginCompiler.Plugins.Add(measure);
            _PluginCompiler.LoadStartupPlugins();

            _PluginCompiler.SetPluginVisible("Compass", World.Settings.ShowCompass);
            _PluginCompiler.SetPluginVisible("ScaleBar", World.Settings.ShowScaleBar);
            _PluginCompiler.SetPluginVisible("GridLengendBar", World.Settings.ShowLengendBar);
           
            _PluginCompiler.SetPluginVisible("Monitor", false);
            _PluginCompiler.SetPluginVisible("Fog", World.Settings.EnableFog);

            _VirtualGlobe.Chart = chart.Plugin as GraphChart3D;
        }
        private void OnMeasureCommand(object ischecked)
        {
            var tool = (_PluginCompiler["MeasureTool"] as MeasureTool);
            if((bool)ischecked)
                tool.TurnOn();
        }
        private void OnScreenShotCommand(object filename)
        {
            _VirtualGlobe.SaveScreenshot(filename.ToString());
        }
        private void OnViewCommand(object para)
        {
            var menu = para.ToString();
            switch (menu)
            {
                case "miCameraHasInertia":
                    World.Settings.CameraHasInertia = !World.Settings.CameraHasInertia;
                    break;
                case "miCameraHasMoveMo":
                    World.Settings.CameraHasMomentum = !World.Settings.CameraHasMomentum;
                    break;
                case "miSunshading":
                    World.Settings.EnableSunShading = !World.Settings.EnableSunShading;
                    break;
                case "miHasSky":
                    World.Settings.EnableSky = !World.Settings.EnableSky;
                    _VirtualGlobeViewModel.VirtualGlobe.CurrentWorld.DefaultLayerList.FindChild("Sky").IsOn =  World.Settings.EnableSky;
                    break;
                case "miHasAtmosphere":
                    World.Settings.EnableAtmosphere = !World.Settings.EnableAtmosphere;
                    _VirtualGlobeViewModel.VirtualGlobe.CurrentWorld.DefaultLayerList.FindChild("SkyGradient").IsOn = World.Settings.EnableAtmosphere;
                    break;
                case "miPositionInfo":
                    World.Settings.ShowPosition = !World.Settings.ShowPosition;
                    break;
                case "miHasCrossHairs":
                    World.Settings.ShowCrosshairs = !World.Settings.ShowCrosshairs;
                    break;
                case "miDisplayFog":
                    World.Settings.EnableFog = !World.Settings.EnableFog;
                    _VirtualGlobeViewModel.VirtualGlobe.CurrentWorld.RenderableObjects.FindChild("Fog").IsOn = World.Settings.EnableFog;
                    break;
                case "miHasCompass":
                    World.Settings.ShowCompass = !World.Settings.ShowCompass;
                    _PluginCompiler.SetPluginVisible("Compass", World.Settings.ShowCompass);
                    break;
                case "miShowScaleBar":
                    World.Settings.ShowScaleBar = !World.Settings.ShowScaleBar;
                    _PluginCompiler.SetPluginVisible("ScaleBar", World.Settings.ShowScaleBar);
                    break;
                case "miGridLengend":
                    World.Settings.ShowLengendBar = !World.Settings.ShowLengendBar;
                    _PluginCompiler.SetPluginVisible("GridLengendBar", World.Settings.ShowLengendBar);
                    break;
                case "miMonitor":
                    _PluginCompiler.SwitchPluginVisible("Monitor");
                    break;
                case "miHasAtmosphereScattering":
                    World.Settings.EnableAtmosphericScattering = !World.Settings.EnableAtmosphericScattering;
                    break;
                case "miShowLatLong":
                    World.Settings.ShowLatLonLines = !World.Settings.ShowLatLonLines;
                    break;
                case "miEnableHighPerfomance":
                    World.Settings.EnableHighPerfomance = !World.Settings.EnableHighPerfomance;
                    break;

            }
        }

    }
}
