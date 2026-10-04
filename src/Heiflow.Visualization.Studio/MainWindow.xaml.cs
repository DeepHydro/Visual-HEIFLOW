using Heiflow.Core;
using Heiflow.Core.Plugin;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Surface.MIKE;
using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable;
using Heiflow.Visualization.Studio.Controls;
using Heiflow.Visualization.Tool;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Configuration;
using HUST.WREIS.Dot3D.Display;
using HUST.WREIS.Dot3D.Display.Plugins;
using HUST.WREIS.Dot3D.Renderable;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Utility;

namespace Heiflow.Visualization.Studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static Engine3DSettings _Settings = new Engine3DSettings();
        private string _SettingsDirectory;
        private string _ApplicationPath = "";
        private PluginCompiler _PluginCompiler;
        private SplashScreen _SplashScreen;
        private Timer _Timer3d;

        public MainWindow()
        {
            _SplashScreen = new SplashScreen("Images\\SplashScreen.png");
            _SplashScreen.Show(false);

            InitializeComponent();

            _Timer3d = new Timer();
            _Timer3d.Interval = 15;
            _Timer3d.Tick += new EventHandler(timer3d_Tick);

            System.Threading.Thread.CurrentThread.Name = "Main Thread";
            Uri uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            _ApplicationPath = (uri.LocalPath);
            _ApplicationPath = System.IO.Path.GetDirectoryName(_ApplicationPath);

            _SettingsDirectory = _ApplicationPath + "\\Config";
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;          
        }
  
        /// <summary>
        /// Application Path
        /// </summary>
        public string ApplicationPath
        {
            get
            {
                return _ApplicationPath;
            }
        }
        /// <summary>
        /// The 3d rendering window
        /// </summary>
        public SceneWindow SceneWindow
        {
            get
            {
                return sceneWindow;
            }
        }

        public GridLengendBar GridLengend
        {
            get
            {
                return _PluginCompiler["GridLengendBar"] as GridLengendBar;
            }
        }

        #region Loading & Initializing
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            this.winhostMain.Loaded += new RoutedEventHandler(winhostMain_Loaded);
            long CacheUpperLimit = (long)_Settings.CacheSizeMegaBytes * 1024L * 1024L;
            long CacheLowerLimit = (long)_Settings.CacheSizeMegaBytes * 768L * 1024L;	//75% of upper limit
            sceneWindow.Cache = new Cache(_Settings.CachePath, CacheLowerLimit, CacheUpperLimit, _Settings.CacheCleanupInterval, _Settings.TotalRunTime);
        }

        void winhostMain_Loaded(object sender, RoutedEventArgs e)
        {
            sceneWindow.SetResManager();
            sceneWindow.CurrentWorld = ConfigurationManager.Load(_SettingsDirectory + "\\Layers.xml", sceneWindow.Cache, _Settings);
            LoadDefaultLayers();
            sceneWindow.Render();
            SceneWindow.Focus();
            SceneWindow.GotoLatLonAltitude(World.Settings.DefalutLatitude, World.Settings.DefalutLongitude,750000.0f);
            ConfigurationManager.DrawArgs = sceneWindow.DrawArgs;

            InitializeApp();
            _Timer3d.Start();
            InitializePluginCompiler();

            //SceneWindow.CurrentWorld.DefaultLayerList.FindChild("Sky").IsOn = World.Settings.EnableSky;
            SceneWindow.CurrentWorld.DefaultLayerList.FindChild("SkyGradient").IsOn = World.Settings.EnableAtmosphere;

            miGridLengend.IsChecked = World.Settings.ShowLengendBar;
            miShowScaleBar.IsChecked = World.Settings.ShowScaleBar;
            miHasCompass.IsChecked = World.Settings.ShowCompass;
            miShowChart.IsChecked = World.Settings.ShowChart;

            _PluginCompiler.SetPluginVisible("Compass", World.Settings.ShowCompass);
            _PluginCompiler.SetPluginVisible("ScaleBar", World.Settings.ShowScaleBar);
            _PluginCompiler.SetPluginVisible("GridLengendBar", World.Settings.ShowLengendBar);
            _PluginCompiler.SetPluginVisible("HistogramChart", World.Settings.ShowHistrogramChart);
            _PluginCompiler.SetPluginVisible("Chart", World.Settings.ShowChart);

            _SplashScreen.Close(new TimeSpan(100));
        }

        private void LoadSettings()
        {
            try
            {
                World.LoadSettings(_SettingsDirectory);
                _Settings = (Engine3DSettings)SettingsBase.Load(_Settings, SettingsBase.LocationType.Application);
                _Settings.SceneWindow = SceneWindow;
                ConfigurationManager.Engine3DSettings = _Settings;
                string cfdpath = System.IO.Path.Combine(_ApplicationPath, "Config\\Settings.xml");
                ConfigurationManager.LoadCFDSettings(cfdpath);

                // decrypt encoded user credentials
                DataProtector dp = new DataProtector(DataProtector.Store.USE_USER_STORE);
                if (_Settings.ProxyUsername.Length > 0) _Settings.ProxyUsername = dp.TransparentDecrypt(_Settings.ProxyUsername);
                if (_Settings.ProxyPassword.Length > 0) _Settings.ProxyPassword = dp.TransparentDecrypt(_Settings.ProxyPassword);


                miPositionInfo.IsChecked = World.Settings.ShowPosition;
                miHasCrossHairs.IsChecked = World.Settings.ShowCrosshairs;
                miCameraHasInertia.IsChecked = World.Settings.CameraHasInertia;
                miDisplayFog.IsChecked = World.Settings.EnableFog;
                World.Settings.EnableSky = false;
                miHasSky.IsChecked = World.Settings.EnableSky;
                miHasAtmosphere.IsChecked = World.Settings.EnableAtmosphere;
                miSunshading.IsChecked = World.Settings.EnableSunShading;
                miDisplayFog.IsChecked = World.Settings.EnableFog;
                miShowLatLong.IsChecked = World.Settings.ShowLatLonLines;
                World.Settings.NotifyPropertyChanged = true;
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }
        private void LoadDefaultLayers()
        {
            string imgpath = _ApplicationPath + @"\Resources\Images\wsiearth.JPG";
            string PluginDirectory = _ApplicationPath + @"\Plugins\stars3d\";
            RenderableObjectList roList = SceneWindow.CurrentWorld.DefaultLayerList;

            Stars3DLayer starLayer = new Stars3DLayer("Starfield", PluginDirectory, SceneWindow.CurrentWorld, SceneWindow.DrawArgs);
            starLayer.IsOn = true;
            roList.Add(starLayer);

            PluginDirectory = _ApplicationPath + @"\Plugins\SkyGradient\";
            FileInfo SettingsFile = new FileInfo(System.IO.Path.Combine(PluginDirectory, SceneWindow.CurrentWorld.Name + ".ini"));
            if (SettingsFile.Exists)
            {
                Murris.Plugins.SkyGradientLayer skyGradienLayer = new Murris.Plugins.SkyGradientLayer("SkyGradient", PluginDirectory, SceneWindow.CurrentWorld, SceneWindow.DrawArgs);
                skyGradienLayer.IsOn = false;
                roList.Add(skyGradienLayer);
            }

            PluginDirectory = _ApplicationPath + @"\Plugins\LensFlare\";
            LensFlareEffect flare = new LensFlareEffect(PluginDirectory);
            roList.Add(flare);

            SceneWindow.CurrentWorld.RenderableObjects.ChildObjects.Add(roList);

            ImageLayer ilayer = new ImageLayer("Land Surface", sceneWindow.CurrentWorld, 0, imgpath, -90, 90, -180, 180, 1.0, null);
            ilayer.RenderPriority = RenderPriority.SurfaceImages;
            ilayer.IsOn = true;
            ilayer.IconImagePath = _Settings.IconPath + "DefaultLayerIcon.png";
            SceneWindow.CurrentWorld.DataLayerList.ChildObjects.Insert(0, ilayer);

            RefreshLayerManager();
            SceneWindow.CurrentWorld.DataLayerList.PropertyChanged += DataLayerList_PropertyChanged;
        }

        void DataLayerList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ChildObjects")
            {
                RefreshLayerManager();
            }
        }

        private void InitializePluginCompiler()
        {
            Log.Write(Log.Levels.Debug, "CONF", "initializing plugin compiler...");
            //this.splashScreen.SetText("Initializing plugins...");
            string pluginRoot = System.IO.Path.Combine(_ApplicationPath, "Plugins");
            _PluginCompiler = new PluginCompiler(VGSManager.Instance as IApplication, pluginRoot);

            //#if DEBUG
            // Search for plugins in this application (plugin development/debugging aid)
            //  compiler.FindPlugins(Assembly.GetExecutingAssembly());
            //#endif
            PluginInfo compass = new PluginInfo()
            {
                Name = "Compass",
                Plugin = new Compass3D(SceneWindow),
                IsLoadedAtStartup = true,
            };
            //PluginInfo sky = new PluginInfo()
            //{
            //    Name = "Sky",
            //    Plugin = new Sky(SceneWindow),
            //    IsLoadedAtStartup = true,
            //};
            PluginInfo fog = new PluginInfo()
            {
                Name = "Fog",
                Plugin = new Fog(SceneWindow),
                IsLoadedAtStartup = true,
            };

            PluginInfo chart = new PluginInfo()
            {
                Name = "Chart",
                Plugin = new GraphChart3D(SceneWindow)
                {
                    Name = "Chart",
                    Caption = "Chart Viewer",
                    Location = new System.Drawing.Point(10, SceneWindow.Height - 320),
                    MenuItem = miShowChart
                },
                IsLoadedAtStartup = true,
            };


            PluginInfo lengend = new PluginInfo()
            {
                Name = "GridLengendBar",
                Plugin = new GridLengendBar(SceneWindow),
                IsLoadedAtStartup = true,
            };

            PluginInfo scaleBar = new PluginInfo()
            {
                Name = "ScaleBar",
                Plugin = new ScaleBarLegend(SceneWindow) { MenuItem = miShowScaleBar },
                IsLoadedAtStartup = true
            };

            PluginInfo toolBar = new PluginInfo()
            {
                Name = "ToolBar",
                Plugin = new HUST.WREIS.Dot3D.Display.ToolBar(SceneWindow),
                IsLoadedAtStartup = true,
            };
            PluginInfo measure = new PluginInfo()
            {
                Name = "MeasureTool",
                Plugin = new HUST.WREIS.Dot3D.Display.MeasureTool(SceneWindow),
                IsLoadedAtStartup = true,
            };


            _PluginCompiler.Plugins.Add(compass);
            //    pluginCompiler.Plugins.Add(sky);
            _PluginCompiler.Plugins.Add(fog);
            _PluginCompiler.Plugins.Add(chart);
            _PluginCompiler.Plugins.Add(lengend);
            _PluginCompiler.Plugins.Add(scaleBar);
            //  pluginCompiler.Plugins.Add(toolBar);
            _PluginCompiler.Plugins.Add(measure);
            _PluginCompiler.LoadStartupPlugins();

            SceneWindow.Chart = chart.Plugin as GraphChart3D;
        }


        private void InitializeApp()
        {
            //VisAppManager.Default = new VisAppManager(sceneWindow.CurrentWorld)
            //{
            //    ApplicationPath = _ApplicationPath,
            //    MainWindow = this
            //};
            //_ProjectManager = VisAppManager.Default.ProjectManager;
            //_ProjectManager.Composite();
            //_ProjectManager.ConfigManager.SetPath(ApplicationPath);
            //_ProjectManager.Initialize();
            //_ProjectManager.ProjectExplorer = _ProjectExplorer;
            //_ProjectManager.PropertyGrid = _PropertyGrid;
            //_ProjectManager.AnimationPanel = _AnimationPlayer;
            //_ProjectManager.SymbologyControl = _SymbologyControl;
            //_ProjectManager.SerializationManager.ProjectOpened += SerializationManager_ProjectOpened;
            //(_ProjectManager.SerializationManager.OpenProjectFileProviders as List<IOpenProjectFileProvider>).Add(new OpenH3DProjectFileProvider());
            //(_ProjectManager.SerializationManager.SaveProjectFileProviders as List<ISaveProjectFileProvider>).Add(new SaveH3DProjectFileProvider());
            //if (!VisAppManager.Default.CheckLicense())
            //{
            //    MainGrid.Visibility = System.Windows.Visibility.Hidden;
            //    ActivicationPanel.Visibility = System.Windows.Visibility.Visible;
            //}
            //var window = new DataGridWindow();
            //window.Owner = this;
            ////_ProjectManager.DataGridPanel = window;
            //VisAppManager.Default.LoadSitesLayer();
        }


        public void RefreshLayerManager()
        {
           // _LayerManager.SetDataSource(SceneWindow.CurrentWorld.DataLayerList.ChildObjects);
        }
        #endregion

        #region 3D rendering
        void timer3d_Tick(object sender, EventArgs e)
        {
            MainWindow_ContentRendered(null, null);
        }

        void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                if (sceneWindow.m_Device3d == null)
                {
                    //  e.Graphics.Clear(SystemColors.ControlColor);
                    return;
                }

                // to prevent screen garbage when resizing
                sceneWindow.Render();
                sceneWindow.m_Device3d.Present();
            }
            catch (DeviceLostException)
            {
                try
                {
                    sceneWindow.AttemptRecovery();
                    // Our surface was lost, force re-render
                    sceneWindow.Render();
                    sceneWindow.m_Device3d.Present();
                }
                catch (DirectXException)
                {
                    // Ignore a 2nd failure
                }
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (sceneWindow != null)
                sceneWindow.Focus();
            base.OnGotFocus(e);
        }
        #endregion

        #region Menu
        private void FileLoadHeiflow_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "project file|*.ihmx";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //_ProjectManager.SerializationManager.ProjectOpened += SerializationManager_ProjectOpend;
                //_ProjectManager.SerializationManager.OpenProject(ofd.FileName);
            }
        }

        private void SerializationManager_ProjectOpend(object sender, EventArgs e)
        {
            var prj = (sender as ProjectSerialization).CurrentProject as Heiflow3DProject;

            var grid = prj.Model.Grid;
            cmbLayer.Items.Clear();
            for (int i = 1; i <= grid.ActualLayerCount; i++)
            {
                cmbLayer.Items.Add(i.ToString());
            }
        }

        private void cmbLayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_ProjectManager.CurrentGridLayer = int.Parse(cmbLayer.SelectedValue.ToString());
        }
        private void SerializationManager_ProjectOpened(object sender, EventArgs e)
        {
            //VisAppManager.Default.OpenODMDB();
            //VisAppManager.Default.LoadModelGrid();
            //VisAppManager.Default.LoadFeatureLayers();
            //VisAppManager.Default.LoadRasterLayers();
            //VisAppManager.Default.Load3DModelFeatures();
            //SceneWindow.ModelRenderDX = VisAppManager.Default.SelectedRender;
            //VisAppManager.Default.SelectedRender.ValueRangeChanged += RenderableMFGrid_ValueRangeChanged;
            //GridLengend.Grid = VisAppManager.Default.SelectedRender;
        }

       private   void RenderableMFGrid_ValueRangeChanged(object sender, Renderable.Grid.StatisticsArgs args)
        {
            SceneWindow.StatisticsInfo = args.StatisticsInfo;
        }

        private void FileSaveHeiflow_Click(object sender, RoutedEventArgs e)
        {
            //if (_ProjectManager.Project == null)
            //{
            //    System.Windows.Forms.MessageBox.Show("You cann't save since no project has been created!", "Warning",
            //        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //_ProjectManager.SerializationManager.SaveProject(_ProjectManager.Project.FullProjectFilePath, _ProjectManager.Project);
        }

        private void OnLoadEnglishClick(object sender, RoutedEventArgs e)
        {
            //VisAppManager.Default.LoadLanguageFile("Resources/Langs/en-US.xaml");
        }

        private void OnLoadChineseClick(object sender, RoutedEventArgs e)
        {
            //VisAppManager.Default.LoadLanguageFile("Resources/Langs/zh-CN.xaml");
        }

        private void StyleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as System.Windows.Controls.MenuItem;
            var style = menu.Header.ToString();
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);
            var accent = ThemeManager.GetAccent(style);

            // ThemeManager.ChangeAppStyle(System.Windows.Application.Current, accent, theme.Item1);
        }
        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem)
            {
                System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;//miCameraHasInertia
                switch (item.Name)
                {
                    case "miCameraHasInertia":
                        World.Settings.CameraHasInertia = !World.Settings.CameraHasInertia;
                       // (sender as System.Windows.Controls.MenuItem).IsChecked = World.Settings.CameraHasInertia;
                        break;
                    case "miCameraHasMoment":
                        World.Settings.CameraHasMomentum = !World.Settings.CameraHasMomentum;
                        //(sender as System.Windows.Controls.MenuItem).IsChecked = World.Settings.CameraHasMomentum;
                        break;
                    case "miSunshading":
                        World.Settings.EnableSunShading = !World.Settings.EnableSunShading;
                        //miSunshading.IsChecked = World.Settings.EnableSunShading;
                        break;
                    case "miHasSky":
                        //World.Settings.EnableSky = !World.Settings.EnableSky;
                        //  SceneWindow.CurrentWorld.DefaultLayerList.FindChild("Sky").IsOn = false;// World.Settings.EnableSky;
                        //miHasSky.IsChecked = World.Settings.EnableSky;
                        break;
                    case "miHasAtmosphere":
                        World.Settings.EnableAtmosphere = !World.Settings.EnableAtmosphere;
                        SceneWindow.CurrentWorld.DefaultLayerList.FindChild("SkyGradient").IsOn = World.Settings.EnableAtmosphere;
                      //  miHasAtmosphere.IsChecked = World.Settings.EnableAtmosphere;
                        break;
                    case "miPositionInfo":
                        World.Settings.ShowPosition = !World.Settings.ShowPosition;
                        break;
                    case "miHasCrossHairs":
                        World.Settings.ShowCrosshairs = !World.Settings.ShowCrosshairs;
                        break;
                    case "miDisplayFog":
                        World.Settings.EnableFog = !World.Settings.EnableFog;
                        SceneWindow.CurrentWorld.RenderableObjects.FindChild("Fog").IsOn = World.Settings.EnableFog;
                        //miDisplayFog.IsChecked = World.Settings.EnableFog;
                        break;
                    case "miViewProperty":
                        this.ToggleFlyout(0);
                        break;
                    case "miHasCompass":
                        World.Settings.ShowCompass = !World.Settings.ShowCompass;
                       // miHasCompass.IsChecked = World.Settings.ShowCompass;
                        _PluginCompiler.SetPluginVisible("Compass", World.Settings.ShowCompass);
                        break;
                    case "miShowScaleBar":
                        World.Settings.ShowScaleBar = !World.Settings.ShowScaleBar;
                        //miShowScaleBar.IsChecked = World.Settings.ShowScaleBar;
                        _PluginCompiler.SetPluginVisible("ScaleBar", World.Settings.ShowScaleBar);
                        break;
                    case "miGridLengend":
                        World.Settings.ShowLengendBar = !World.Settings.ShowLengendBar;
                        //miGridLengend.IsChecked = World.Settings.ShowLengendBar;
                        _PluginCompiler.SetPluginVisible("GridLengendBar", World.Settings.ShowLengendBar);
                        break;
                    case "miShowChart":
                        World.Settings.ShowChart = !World.Settings.ShowChart;
                        //miShowChart.IsChecked = World.Settings.ShowChart;
                        _PluginCompiler.SetPluginVisible("Chart", World.Settings.ShowChart);
                        break;
                    case "miHasAtmosphereScattering":
                        World.Settings.EnableAtmosphericScattering = !World.Settings.EnableAtmosphericScattering;
                        //miHasAtmosphereScattering.IsChecked = World.Settings.EnableAtmosphericScattering;
                        break;
                    case "miSaveViewSetting":
                        World.Settings.Save();
                        break;
                    case "miShowLatLong":
                        World.Settings.ShowLatLonLines = !World.Settings.ShowLatLonLines;
                        //miShowLatLong.IsChecked = World.Settings.ShowLatLonLines;
                        break;
                    case "_EnableHighPerfomance":
                        World.Settings.EnableHighPerfomance = !World.Settings.EnableHighPerfomance;
                       // miShowLatLong.IsChecked = World.Settings.EnableHighPerfomance;
                        break;
                }
            }
        }

        private void miGotoDefault_Click(object sender, RoutedEventArgs e)
        {
            SceneWindow.GotoLatLonAltitude(World.Settings.DefalutLatitude, World.Settings.DefalutLongitude, 1500000.0f);
        }

        private void miCurrentPosAsDefault_Click(object sender, RoutedEventArgs e)
        {
            World.Settings.DefalutLatitude = (float)this.SceneWindow.DrawArgs.WorldCamera.Latitude.Degrees;
            World.Settings.DefalutLongitude = (float)this.SceneWindow.DrawArgs.WorldCamera.Longitude.Degrees;
            World.Settings.Save();
        }
        private void btnVisSetting_Click(object sender, RoutedEventArgs e)
        {
            var index = int.Parse((sender as System.Windows.Controls.Button).Tag.ToString());
            ToggleFlyout(index);
        }
        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void miLoadShp_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "shape file|*.shp";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ShapeFeatureProvider layer = new ShapeFeatureProvider();
                var ly = layer.Load(ofd.FileName, SceneWindow.CurrentWorld);
                SceneWindow.CurrentWorld.DataLayerList.Add(ly);
            }
        }
        private void miLoadRaster_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //GdalRasterProvider gp = new GdalRasterProvider();
            //ofd.Filter = gp.DialogReadFilter;
            //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string name = Path.GetFileNameWithoutExtension(ofd.FileName);
            //    RenderableRasterProvider provider = new RenderableRasterProvider();
            //    var ras = provider.Load(ofd.FileName, SceneWindow.CurrentWorld);
            //    SceneWindow.CurrentWorld.DataLayerList.ChildObjects.Add(ras);
            //    RefreshLayerManager();

                //var prj = VisAppManager.Default.ProjectManager.Project as Heiflow3DProject;
                //if (prj != null)
                //{
                //    if (prj.RenderableRasterProvider == null)
                //        prj.RenderableRasterProvider = new List<RenderableRasterProvider>();
                //    if (!prj.RenderableRasterProvider.Contains(provider))
                //    {
                //        prj.RenderableRasterProvider.Add(provider);
                //    }
                //    provider.RelativeFileName = provider.GetRelativeFileName(prj.AbsolutePathToProjectFile, provider.FullFileName);
                //}
            //}
        }
        private void miLoad3DModel_Click(object sender, RoutedEventArgs e)
        {
            Add3DModelWindow window = new Add3DModelWindow(SceneWindow);
            window.ShowDialog();
        }
        private void miLoadODM_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Access Database|*.mdb";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //var prj = VisAppManager.Default.ProjectManager.Project as Heiflow3DProject;
                //if (prj != null)
                //{
                //    ODMProvider provider = new ODMProvider();

                //    if (prj.ODMProvider == null)
                //        prj.ODMProvider = new List<ODMProvider>();
                //    if (!prj.ODMProvider.Contains(provider))
                //    {
                //        prj.ODMProvider.Add(provider);
                //    }
                //    provider.FullFileName = ofd.FileName;
                //    provider.RelativeFileName = provider.GetRelativeFileName(prj.AbsolutePathToProjectFile, provider.FullFileName);
                //    VisAppManager.Default.OpenODMDB();
                //}
            }
        }

        private void miSearchSites_Click(object sender, RoutedEventArgs e)
        {
            SeachSitesWindow sw = new SeachSitesWindow();
            sw.ShowInTaskbar = false;
            sw.Owner = this;
            sw.Show();
        }

        private void ToolMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            switch (item.Name)
            {
                case "miDis3DRenderInfo":
                    sceneWindow.ShowPerformanceInfo = miDis3DRenderInfo.IsChecked;
                    break;
                case "miMeasure":
                    (_PluginCompiler["MeasureTool"] as MeasureTool).TurnOn();
                    miMeasure.IsChecked = (_PluginCompiler["MeasureTool"] as MeasureTool).IsOn;
                    break;
                case "miScreenShot":
                    SaveFileDialog ofd = new SaveFileDialog();
                    ofd.Filter = "bmp file | *.bmp | jpg file | *.jpg| png file(*.png) | *.png";

                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        sceneWindow.SaveScreenshot(ofd.FileName);
                    }
                    break;
                case "miExtractTerrain":
                    ExtractTerrain terrain = new ExtractTerrain();
                    OpenFileDialog ofdlg = new OpenFileDialog();
                    ofdlg.Filter = "shp file|*.shp";
                    if (ofdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        terrain.ExtractTo(ofdlg.FileName);
                    }
                    break;
                case "miExtractMike":
                   OpenFileDialog ofdlg1 = new OpenFileDialog();
                   ofdlg1.Filter = "dbf file|*.dbf";
                   ofdlg1.Multiselect = true;
                   if (ofdlg1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        MikeShpProvider mike = new MikeShpProvider();
                        string ac_file = ofdlg1.FileNames[0].Replace(".dbf", ".ac");
                        //mike.Export(VisAppManager.Default.SelectedRender.Grid as ITriangularGrid, ac_file, ofdlg1.FileNames, new int[] { 1, 2 });
                        mike.Export( ac_file, ofdlg1.FileNames, new int[] { 1, 2 });
                    }
                    break;
                case "miImportMikeMesh":
                    string node_shp = @"E:\科研项目\洪水风险\算例\model5_3kuikou\2d\Results\Nodes.shp";
                    string elem_shp = @"E:\科研项目\洪水风险\算例\model5_3kuikou\2d\Results\Elements.shp";
                    string mesh = @"E:\科研项目\洪水风险\算例\model5_3kuikou\2d\Results\grid.mesh";
                    MikeShpProvider mike1 = new MikeShpProvider();
                    mike1.Import(node_shp, elem_shp, mesh);
                    break;
            }
        }

        private void miShowStatInfo_Click(object sender, RoutedEventArgs e)
        {
            sceneWindow.ShowStatisticsInfo = !sceneWindow.ShowStatisticsInfo;
        }
        #endregion

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //VisAppManager.Default.CloseDB();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow();
            window.ShowDialog();
        }


    }
}