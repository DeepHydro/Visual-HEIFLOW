using Microsoft.DirectX.Direct3D;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Serialization;
using HUST.WREIS.Dot3D.Configuration;

namespace HUST.WREIS.Dot3D
{

    public enum MeasureMode
    {
        Single,
        Multi
    }

    public enum ScaleMethod { None, Linear, StandardDeviation }
    public enum ArrowStyle { HeadOnly, LineArrow }
    /// <summary>
    /// World user configurable settings
    /// TODO: Group settings
    /// </summary>
    public class WorldSettings : SettingsBase
    {
        public WorldSettings()
        {
            RenderCarsianGridOnly = false;
            TerrainScaleMethod = ScaleMethod.StandardDeviation;
            StandardDeviationNumber = 2;
        }

        #region Atmosphere
        private bool enableAtmosphericScattering = false;

        [Browsable(true), Category("Atmosphere")]
        [Description("Enable Atmospheric Scattering")]
        public bool EnableAtmosphericScattering
        {
            get { return enableAtmosphericScattering; }

            set { enableAtmosphericScattering = value; }
        }

        private bool forceCpuAtmosphere = true;
        [Browsable(true), Category("Atmosphere")]
        [Description("Forces CPU calculation instead of GPU for Atmospheric Scattering")]
        public bool ForceCpuAtmosphere
        {
            get { return forceCpuAtmosphere; }
            set { forceCpuAtmosphere = value; }
        }

        #endregion

        #region UI

        /// <summary>
        /// Show the top tool button bar
        /// </summary>
        private bool showToolbar = false;

        /// <summary>
        /// Display the layer manager window
        /// </summary>
        private bool showLayerManager = false;

        /// <summary>
        /// Display cross-hair symbol on screen
        /// </summary>
        private bool showCrosshairs = false;

        /// <summary>
        /// Font name for the default font used in UI
        /// </summary>
        private string defaultFontName = "Tahoma";

        /// <summary>
        /// Font size (em) for the default font used in UI
        /// </summary>
        private float defaultFontSize = 9.0f;

        /// <summary>
        /// Font style for the default font used in UI
        /// </summary>
        private FontStyle defaultFontStyle = FontStyle.Regular;

        /// <summary>
        /// Font name used in the toolbar 
        /// </summary>
        private string toolbarFontName = "Tahoma";

        /// <summary>
        /// Font size (em) for the font used in UI
        /// </summary>
        private float toolbarFontSize = 8;

        /// <summary>
        /// Font style for the font used in UI
        /// </summary>
        private FontStyle toolbarFontStyle = FontStyle.Bold;

        /// <summary>
        /// Menu bar background color
        /// </summary>
        private int menuBarBackgroundColor = Color.FromArgb(128, 128, 128, 128).ToArgb();

        /// <summary>
        /// Font name used in the layer manager 
        /// </summary>
        private string layerManagerFontName = "Tahoma";

        /// <summary>
        /// Font size (em) for the font used in UI
        /// </summary>
        private float layerManagerFontSize = 9;

        /// <summary>
        /// Font style for the font used in layer manager
        /// </summary>
        private FontStyle layerManagerFontStyle = FontStyle.Regular;

        /// <summary>
        /// Layer manager width (pixels)
        /// </summary>
        private int layerManagerWidth = 200;

        /// <summary>
        /// Draw anti-aliased text
        /// </summary>
        private bool antiAliasedText = false;

        /// <summary>
        /// Maximum frames-per-second setting
        /// </summary>
        private int throttleFpsHz = 50;

        /// <summary>
        /// Vsync on/off (Wait for vertical retrace)
        /// </summary>
        private bool vSync = true;

        /// <summary>
        /// Rapid Fire MODIS icon size
        /// </summary>
        private int modisIconSize = 60;

        private int m_FpsFrameCount = 300;
        private bool m_ShowFpsGraph = false;

        private int downloadTerrainRectangleColor = Color.FromArgb(50, 0, 0, 255).ToArgb();
        private int downloadProgressColor = Color.FromArgb(100, 0, 255, 0).ToArgb();
        private int downloadLogoColor = Color.FromArgb(180, 255, 255, 255).ToArgb();
        private int menuBackColor = Color.FromArgb(170, 40, 40, 40).ToArgb();
        private int menuOutlineColor = Color.FromArgb(150, 160, 160, 160).ToArgb();
        private int widgetBackgroundColor = Color.FromArgb(0, 0, 0, 255).ToArgb();
        private int scrollbarColor = System.Drawing.Color.FromArgb(170, 100, 100, 100).ToArgb();
        private int scrollbarHotColor = System.Drawing.Color.FromArgb(170, 255, 255, 255).ToArgb();
        private int toolBarBackColor = System.Drawing.Color.FromArgb(100, 255, 255, 255).ToArgb();
        private bool showDownloadIndicator = true;
        private bool outlineText = false;
        private bool showCompass = true;
  //      private bool showChart = true;
 //       private bool showGridLengend = false;
        private bool browserVisible = false;
        private bool browserOrientationHorizontal = false;
        private int browserSize = 300;
        private bool useInternalBrowser = true;
        private bool usePseudoColor = false;
        private bool useOfflineSearch = false;
        private string pseudoColorImagePath = "ColorRamp\\Spectrum_1.png";

        private bool inversePseudoColor = true;
        private int pseudoColorStrechLevel = 256;

        private bool mshowGridLengend;
        private bool showChart;
        private bool showHistrogramChart;

        [Browsable(true), Category("UI")]
        [Description("Show Compass Indicator.")]
        public bool ShowCompass
        {
            get { return showCompass; }
            set
            {
                if (NotifyPropertyChanged)
                {
                    OnPropertyChanged(new SettingsEventArgs() { SettingName = "ShowCompass", OldValue = showCompass, NewValue = value });
                }
                showCompass = value;                
            }
        }

        public bool NotifyPropertyChanged
        {
            get;
            set;
        }

        [Browsable(true), Category("UI")]
        [Description("Show ShowLengend Bar")]
        public bool ShowLengendBar
        {
            get
            {
                return mshowGridLengend;
            }
            set
            {
                if (NotifyPropertyChanged)
                {
                    OnPropertyChanged(new SettingsEventArgs() { SettingName = "ShowGridLengend", OldValue = mshowGridLengend, NewValue = value });
                }
                mshowGridLengend=value;
            }
        }

        [Browsable(true), Category("UI")]
        [Description("Use Pseudo Color.")]
        public bool UsePseudoColor
        {
            get { return usePseudoColor; }
            set
            {
                OnPropertyChanged(new SettingsEventArgs() { SettingName = "UsePseudoColor", OldValue = usePseudoColor, NewValue = value });
                usePseudoColor = value;
            }
        }

        [Browsable(true), Category("UI")]
        [Description("Relative Path of Pseudo Color Image .")]
        public string PseudoColorImagePath
        {
            get { return pseudoColorImagePath; }
            set
            {
                OnPropertyChanged(new SettingsEventArgs() { SettingName = "PseudoColorImagePath", OldValue = pseudoColorImagePath, NewValue = value });
                pseudoColorImagePath = value;
            }
        }

        [Browsable(true), Category("UI")]
        [Description("Relative Path of Pseudo Color Image .")]
        public int PseudoColorStrechLevel
        {
            get { return pseudoColorStrechLevel; }
            set
            {
                OnPropertyChanged(new SettingsEventArgs() { SettingName = "PseudoColorStrechLevel", OldValue = pseudoColorStrechLevel, NewValue = value });
                pseudoColorStrechLevel = value;
            }
        }

        [Browsable(true), Category("UI")]
        [Description("Inverse Pseudo Color")]
        public bool InversePseudoColor
        {
            get { return inversePseudoColor; }
            set
            {
                OnPropertyChanged(new SettingsEventArgs() { SettingName = "InversePseudoColor", OldValue = inversePseudoColor, NewValue = value });
                inversePseudoColor = value;
            }
        }

        [Browsable(true), Category("UI")]
        [Description("Show Chart Child Window.")]
        public bool ShowChart
        {
            get { return showChart; }
            set
            {
                if(NotifyPropertyChanged)
                    OnPropertyChanged(new SettingsEventArgs() { SettingName = "ShowChart", OldValue = showChart, NewValue = value });
                showChart = value;
            }
        }

        [Browsable(true), Category("UI")]
        [Description("Show Chart Child Window.")]
        public bool ShowHistrogramChart
        {
            get { return showHistrogramChart; }
            set
            {
                if (NotifyPropertyChanged)
                    OnPropertyChanged(new SettingsEventArgs() { SettingName = "ShowHistrogramChart", OldValue = showHistrogramChart, NewValue = value });
                showHistrogramChart = value;
            }
        }

        // [Browsable(true), Category("UI")]
        // [Description("Show  Lengend Child Window.")]
        //public bool ShowGridLengend
        //{
        //    get { return showGridLengend; }
        //    set
        //    {
        //        OnPropertyChanged(new SettingsEventArgs() { SettingName = "ShowGridLengend", OldValue = showCompass, NewValue = value });
        //        showGridLengend = value;
        //    }
        //}


        [Browsable(true), Category("UI")]
        [Description("Draw outline around text to improve visibility.")]
        public bool OutlineText
        {
            get { return outlineText; }
            set { outlineText = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Display download progress and rectangles.")]
        public bool ShowDownloadIndicator
        {
            get { return showDownloadIndicator; }
            set { showDownloadIndicator = value; }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Toolbar Background color.")]
        public Color ToolBarBackColor
        {
            get { return Color.FromArgb(toolBarBackColor); }
            set { toolBarBackColor = value.ToArgb(); }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of scrollbar when scrolling.")]
        public Color ScrollbarHotColor
        {
            get { return Color.FromArgb(scrollbarHotColor); }
            set { scrollbarHotColor = value.ToArgb(); }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of scrollbar.")]
        public Color ScrollbarColor
        {
            get { return Color.FromArgb(scrollbarColor); }
            set { scrollbarColor = value.ToArgb(); }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Menu border color.")]
        public Color MenuOutlineColor
        {
            get { return Color.FromArgb(menuOutlineColor); }
            set { menuOutlineColor = value.ToArgb(); }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Widget background color.")]
        public Color WidgetBackgroundColor
        {
            get { return Color.FromArgb(widgetBackgroundColor); }
            set { widgetBackgroundColor = value.ToArgb(); }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Background color of the menu.")]
        public Color MenuBackColor
        {
            get { return Color.FromArgb(menuBackColor); }
            set { menuBackColor = value.ToArgb(); }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color/transparency of the download progress icon.")]
        public Color DownloadLogoColor
        {
            get { return Color.FromArgb(downloadLogoColor); }
            set { downloadLogoColor = value.ToArgb(); }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of the download progress bar.")]
        public Color DownloadProgressColor
        {
            get { return Color.FromArgb(downloadProgressColor); }
            set { downloadProgressColor = value.ToArgb(); }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of the terrain download in progress rectangle.")]
        public Color DownloadTerrainRectangleColor
        {
            get { return Color.FromArgb(downloadTerrainRectangleColor); }
            set { downloadTerrainRectangleColor = value.ToArgb(); }
        }

        [Browsable(true), Category("UI")]
        [Description("Show the top tool button bar.")]
        public bool ShowToolbar
        {
            get { return showToolbar; }
            set { showToolbar = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Display the layer manager window.")]
        public bool ShowLayerManager
        {
            get { return showLayerManager; }
            set { showLayerManager = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Display cross-hair symbol on screen.")]
        public bool ShowCrosshairs
        {
            get { return showCrosshairs; }
            set { showCrosshairs = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Font name for the default font used in UI.")]
        public string DefaultFontName
        {
            get { return defaultFontName; }
            set { defaultFontName = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Font size for the default font used in UI.")]
        public float DefaultFontSize
        {
            get { return defaultFontSize; }
            set { defaultFontSize = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Font style for the default font used in UI.")]
        public FontStyle DefaultFontStyle
        {
            get { return defaultFontStyle; }
            set { defaultFontStyle = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Font name for the toolbar font used in UI.")]
        public string ToolbarFontName
        {
            get { return toolbarFontName; }
            set { toolbarFontName = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Font size (em) for the toolbar font used in UI.")]
        public float ToolbarFontSize
        {
            get { return toolbarFontSize; }
            set { toolbarFontSize = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Font style for the toolbar font used in UI.")]
        public FontStyle ToolbarFontStyle
        {
            get { return toolbarFontStyle; }
            set { toolbarFontStyle = value; }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Menu bar background color.")]
        public Color MenuBarBackgroundColor
        {
            get { return Color.FromArgb(menuBarBackgroundColor); }
            set { menuBarBackgroundColor = value.ToArgb(); }
        }

        [Browsable(true), Category("UI")]
        [Description("Font name for the layer manager font.")]
        public string LayerManagerFontName
        {
            get { return layerManagerFontName; }
            set { layerManagerFontName = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Font size for the layer manager font.")]
        public float LayerManagerFontSize
        {
            get { return layerManagerFontSize; }
            set { layerManagerFontSize = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Font style for the layer manager font used in UI.")]
        public FontStyle LayerManagerFontStyle
        {
            get { return layerManagerFontStyle; }
            set { layerManagerFontStyle = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Layer manager width (pixels)")]
        public int LayerManagerWidth
        {
            get { return layerManagerWidth; }
            set { layerManagerWidth = value; }
        }

        /// <summary>
        /// Draw anti-aliased text
        /// </summary>
        [Browsable(true), Category("UI")]
        [Description("Enable anti-aliased text rendering. Change active only after program restart.")]
        public bool AntiAliasedText
        {
            get { return antiAliasedText; }
            set { antiAliasedText = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Maximum frames-per-second setting. Optionally throttles the frame rate (to get consistent frame rates or reduce CPU usage. 0 = Disabled")]
        public int ThrottleFpsHz
        {
            get { return throttleFpsHz; }
            set { throttleFpsHz = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Synchronize render buffer swaps with the monitor's refresh rate (vertical retrace). Change active only after program restart.")]
        public bool VSync
        {
            get { return vSync; }
            set { vSync = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Changes the size of the Rapid Fire Modis icons.")]
        public int ModisIconSize
        {
            get { return modisIconSize; }
            set { modisIconSize = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Enables the Frames Per Second Graph")]
        public bool ShowFpsGraph
        {
            get { return m_ShowFpsGraph; }
            set { m_ShowFpsGraph = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Changes length of the Fps Graph History")]
        public int FpsFrameCount
        {
            get { return m_FpsFrameCount; }
            set { m_FpsFrameCount = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Initial visiblity of browser.")]
        public bool BrowserVisible
        {
            get { return browserVisible; }
            set { browserVisible = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Browser orientation.")]
        public bool BrowserOrientationHorizontal
        {
            get { return browserOrientationHorizontal; }
            set { browserOrientationHorizontal = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Size of browser panel.")]
        public int BrowserSize
        {
            get { return browserSize; }
            set { browserSize = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Use Internal Browser?")]
        public bool UseInternalBrowser
        {
            get { return useInternalBrowser; }
            set { useInternalBrowser = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Use Offline Placename Search?")]
        public bool UseOfflineSearch
        {
            get { return useOfflineSearch; }
            set { useOfflineSearch = value; }
        }



        #endregion

        #region Grid

        /// <summary>
        /// Display the latitude/longitude grid
        /// </summary>
        private bool showLatLonLines = false;

        /// <summary>
        /// The color of the latitude/longitude grid
        /// </summary>
        private int latLonLinesColor = System.Drawing.Color.FromArgb(200, 160, 160, 160).ToArgb();

        /// <summary>
        /// The color of the equator latitude line
        /// </summary>
        private int equatorLineColor = System.Drawing.Color.FromArgb(160, 64, 224, 208).ToArgb();

        /// <summary>
        /// Display the tropic of capricorn/cancer lines
        /// </summary>
        private bool showTropicLines = true;

        /// <summary>
        /// The color of the latitude/longitude grid
        /// </summary>
        private int tropicLinesColor = System.Drawing.Color.FromArgb(160, 176, 224, 230).ToArgb();

        [Browsable(true), Category("Grid Lines")]
        [Description("Display the latitude/longitude grid.")]
        public bool ShowLatLonLines
        {
            get { return showLatLonLines; }
            set { showLatLonLines = value; }
        }

        [XmlIgnore]
        [Browsable(true), Category("Grid Lines")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("The color of the latitude/longitude grid.")]
        public int LatLonLinesColor
        {
            get { return latLonLinesColor; }
            set { latLonLinesColor = value; }
        }

        [XmlIgnore]
        [Browsable(true), Category("Grid Lines")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("The color of the equator latitude line.")]
        public int EquatorLineColor
        {
            get { return equatorLineColor; }
            set { equatorLineColor = value; }
        }

        [Browsable(true), Category("Grid Lines")]
        [Description("Display the tropic latitude lines.")]
        public bool ShowTropicLines
        {
            get { return showTropicLines; }
            set { showTropicLines = value; }
        }

        [XmlIgnore]
        [Browsable(true), Category("Grid Lines")]
        [Description("The color of the latitude/longitude grid.")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        public int TropicLinesColor
        {
            get { return tropicLinesColor; }
            set { tropicLinesColor = value; }
        }

        #endregion

        #region World

        /// <summary>
        /// Whether to display the planet axis line (through poles)
        /// </summary>
        private bool showPlanetAxis = false;

        /// <summary>
        /// Whether place name labels should display
        /// </summary>
        private bool showPlacenames = true;

        /// <summary>
        /// Whether country borders and other boundaries should display
        /// </summary>
        private bool showBoundaries = true;

        /// <summary>
        /// Displays coordinates of current position
        /// </summary>
        private bool showPosition = true;


        /// <summary>
        /// Color of the sky at sea level
        /// </summary>
        private int skyColor = Color.FromArgb(115, 155, 185).ToArgb();

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of the sky at sea level.")]
        public Color SkyColor
        {
            get { return Color.FromArgb(skyColor); }
            set { skyColor = value.ToArgb(); }
        }

        /// <summary>
        /// Keep the original (unconverted) NASA SVS image files on disk (in addition to converted files). 
        /// </summary>
        private bool keepOriginalSvsImages = false;

        [Browsable(true), Category("World")]
        [Description("Whether to display the planet axis line (through poles).")]
        public bool ShowPlanetAxis
        {
            get { return showPlanetAxis; }
            set { showPlanetAxis = value; }
        }

        private bool showClouds = true;
        [Browsable(true), Category("World")]
        [Description("Whether to show clouds.")]
        public bool ShowClouds
        {
            get { return showClouds; }
            set { showClouds = value; }
        }

        [Browsable(true), Category("World")]
        [Description("Whether place name labels should display")]
        public bool ShowPlacenames
        {
            get { return showPlacenames; }
            set { showPlacenames = value; }
        }

        [Browsable(true), Category("World")]
        [Description("Whether country borders and other boundaries should display")]
        public bool ShowBoundaries
        {
            get { return showBoundaries; }
            set { showBoundaries = value; }
        }

        [Browsable(true), Category("World")]
        [Description("Displays coordinates of current position.")]
        public bool ShowPosition
        {
            get { return showPosition; }
            set { showPosition = value; }
        }

        [Browsable(true), Category("World")]
        [Description("Keep the original (unconverted) NASA SVS image files on disk (in addition to converted files). ")]
        public bool KeepOriginalSvsImages
        {
            get { return keepOriginalSvsImages; }
            set { keepOriginalSvsImages = value; }
        }

        #endregion

        #region Camera

        private bool cameraResetsAtStartup = true;
        private Angle cameraLatitude = Angle.FromDegrees(0.0);
        private Angle cameraLongitude = Angle.FromDegrees(0.0);
        private double cameraAltitudeMeters = 20000000;
        private Angle cameraHeading = Angle.FromDegrees(0.0);
        private Angle cameraTilt = Angle.FromDegrees(0.0);

        private bool cameraIsPointGoto = true;
        private bool cameraHasInertia = true;
        private bool cameraSmooth = true;
        private bool cameraHasMomentum = false;
        private bool cameraTwistLock = true;
        private bool cameraBankLock = true;
        private float cameraSlerpStandard = 0.35f;
        private float cameraSlerpInertia = 0.05f;

        // Set to either Inertia or Standard slerp value
        private float cameraSlerpPercentage = 0.05f;

        private Angle cameraFov = Angle.FromRadians(Math.PI * 0.25f);
        private Angle cameraFovMin = Angle.FromDegrees(5);
        private Angle cameraFovMax = Angle.FromDegrees(150);
        private float cameraZoomStepFactor = 0.015f;
        private float cameraZoomAcceleration = 10f;
        private float cameraZoomAnalogFactor = 1f;
        private float cameraZoomStepKeyboard = 0.15f;
        private float cameraRotationSpeed = 3.5f;
        private bool elevateCameraLookatPoint = true;

        [Browsable(true), Category("Camera")]
        public bool ElevateCameraLookatPoint
        {
            get { return elevateCameraLookatPoint; }
            set { elevateCameraLookatPoint = value; }
        }

        [Browsable(true), Category("Camera")]
        public bool CameraResetsAtStartup
        {
            get { return cameraResetsAtStartup; }
            set { cameraResetsAtStartup = value; }
        }

        //[Browsable(true),Category("Camera")]
        public Angle CameraLatitude
        {
            get { return cameraLatitude; }
            set { cameraLatitude = value; }
        }

        //[Browsable(true),Category("Camera")]
        public Angle CameraLongitude
        {
            get { return cameraLongitude; }
            set { cameraLongitude = value; }
        }

        public double CameraAltitude
        {
            get { return cameraAltitudeMeters; }
            set { cameraAltitudeMeters = value; }
        }

        //[Browsable(true),Category("Camera")]
        public Angle CameraHeading
        {
            get { return cameraHeading; }
            set { cameraHeading = value; }
        }

        public Angle CameraTilt
        {
            get { return cameraTilt; }
            set { cameraTilt = value; }
        }

        [Browsable(true), Category("Camera")]
        [Description("")]
        public float DefalutLatitude
        {
            get;
            set;
        }

        [Browsable(true), Category("Camera")]
        [Description("")]
        public float DefalutLongitude
        {
            get;
            set;
        }

        [Browsable(true), Category("Widgets")]
        [Description("")]
        public bool ShowScaleBar
        {
            get;
            set;
        }

        [Browsable(true), Category("Widgets")]
        [Description("")]
        public bool EnableFog
        {
            get;
            set;
        }

        [Browsable(true), Category("Widgets")]
        [Description("")]
        public bool EnableSky
        {
            get;
            set;
        }

        [Browsable(true), Category("Widgets")]
        [Description("")]
        public bool EnableAtmosphere
        {
            get;
            set;
        }

        [Browsable(true), Category("Camera")]
        public bool CameraIsPointGoto
        {
            get { return cameraIsPointGoto; }
            set { cameraIsPointGoto = value; }
        }

        [Browsable(true), Category("Camera")]
        [Description("Smooth camera movement.")]
        public bool CameraSmooth
        {
            get { return cameraSmooth; }
            set { cameraSmooth = value; }
        }

        [Browsable(true), Category("Camera")]
        [Description("See CameraSlerp settings for responsiveness adjustment.")]
        public bool CameraHasInertia
        {
            get { return cameraHasInertia; }
            set
            {
                cameraHasInertia = value;
                cameraSlerpPercentage = cameraHasInertia ? cameraSlerpInertia : cameraSlerpStandard;
            }
        }

        [Browsable(true), Category("Camera")]
        public float CameraSlerpPercentage
        {
            get { return cameraSlerpPercentage; }
            set
            {
                cameraSlerpPercentage = value;
            }
        }

        [Browsable(true), Category("Camera")]
        public bool CameraHasMomentum
        {
            get { return cameraHasMomentum; }
            set { cameraHasMomentum = value; }
        }

        [Browsable(true), Category("Camera")]
        public bool CameraTwistLock
        {
            get { return cameraTwistLock; }
            set { cameraTwistLock = value; }
        }

        [Browsable(true), Category("Camera")]
        public bool CameraBankLock
        {
            get { return cameraBankLock; }
            set { cameraBankLock = value; }
        }

        [Browsable(true), Category("Camera")]
        [Description("Responsiveness of movement when inertia is enabled.")]
        public float CameraSlerpInertia
        {
            get { return cameraSlerpInertia; }
            set
            {
                cameraSlerpInertia = value;
                if (cameraHasInertia)
                    cameraSlerpPercentage = cameraSlerpInertia;
            }
        }

        [Browsable(true), Category("Camera")]
        [Description("Responsiveness of movement when inertia is disabled.")]
        public float CameraSlerpStandard
        {
            get { return cameraSlerpStandard; }
            set
            {
                cameraSlerpStandard = value;
                if (!cameraHasInertia)
                    cameraSlerpPercentage = cameraSlerpStandard;
            }
        }

        [Browsable(true), Category("Camera")]
        public Angle CameraFov
        {
            get { return cameraFov; }
            set { cameraFov = value; }
        }

        [Browsable(true), Category("Camera")]
        public Angle CameraFovMin
        {
            get { return cameraFovMin; }
            set { cameraFovMin = value; }
        }

        [Browsable(true), Category("Camera")]
        public Angle CameraFovMax
        {
            get { return cameraFovMax; }
            set { cameraFovMax = value; }
        }

        [Browsable(true), Category("Camera")]
        public float CameraZoomStepFactor
        {
            get { return cameraZoomStepFactor; }
            set
            {
                const float maxValue = 0.3f;
                const float minValue = 1e-4f;

                if (value >= maxValue)
                    value = maxValue;
                if (value <= minValue)
                    value = minValue;
                cameraZoomStepFactor = value;
            }
        }

        [Browsable(true), Category("Camera")]
        public float CameraZoomAcceleration
        {
            get { return cameraZoomAcceleration; }
            set
            {
                const float maxValue = 50f;
                const float minValue = 1f;

                if (value >= maxValue)
                    value = maxValue;
                if (value <= minValue)
                    value = minValue;

                cameraZoomAcceleration = value;
            }
        }

        [Browsable(true), Category("Camera")]
        [Description("Analog zoom factor (Mouse LMB+RMB)")]
        public float CameraZoomAnalogFactor
        {
            get { return cameraZoomAnalogFactor; }
            set { cameraZoomAnalogFactor = value; }
        }

        [Browsable(true), Category("Camera")]
        public float CameraZoomStepKeyboard
        {
            get { return cameraZoomStepKeyboard; }
            set
            {
                const float maxValue = 0.3f;
                const float minValue = 1e-4f;

                if (value >= maxValue)
                    value = maxValue;
                if (value <= minValue)
                    value = minValue;

                cameraZoomStepKeyboard = value;
            }
        }

        float m_cameraDoubleClickZoomFactor = 2.0f;
        [Browsable(true), Category("Camera")]
        public float CameraDoubleClickZoomFactor
        {
            get { return m_cameraDoubleClickZoomFactor; }
            set
            {
                m_cameraDoubleClickZoomFactor = value;
            }
        }

        [Browsable(true), Category("Camera")]
        public float CameraRotationSpeed
        {
            get { return cameraRotationSpeed; }
            set { cameraRotationSpeed = value; }
        }

        #endregion

        #region Time

        [Browsable(true), Category("Time")]
        [Description("Controls the time multiplier for the Time Keeper.")]
        [XmlIgnore]
        public float TimeMultiplier
        {
            get { return TimeKeeper.TimeMultiplier; }
            set { TimeKeeper.TimeMultiplier = value; }
        }

        #endregion

        #region 3D

        private Format textureFormat = Format.Dxt3;
        private bool m_UseBelowNormalPriorityUpdateThread = false;
        private bool m_AlwaysRenderWindow = false;

        private bool convertDownloadedImagesToDds = true;
        [Browsable(true), Category("3D settings")]
        [Description("Enables image conversion to DDS files when loading images. TextureFormat controls the sub-format of the DDS file.")]
        public bool ConvertDownloadedImagesToDds
        {
            get
            {
                return convertDownloadedImagesToDds;
            }
            set
            {
                convertDownloadedImagesToDds = value;
            }
        }

        [Browsable(true), Category("3D settings")]
        [Description("Always Renders the 3D window even form is unfocused.")]
        public bool AlwaysRenderWindow
        {
            get
            {
                return m_AlwaysRenderWindow;
            }
            set
            {
                m_AlwaysRenderWindow = value;
            }
        }

        [Browsable(true), Category("3D settings")]
        [Description("In-memory texture format.  Also used for converted files on disk when image conversion is enabled.")]
        public Format TextureFormat
        {
            get
            {
                //	return Format.Dxt3;
                return textureFormat;
            }
            set
            {
                textureFormat = value;
            }
        }

        private bool m_enableSunShading = false;
        [Browsable(true), Category("3D settings")]
        [Description("Shade the Earth according to the Sun's position at a certain time.")]
        public bool EnableSunShading
        {
            get
            {
                return m_enableSunShading;
            }
            set
            {
                m_enableSunShading = value;
            }
        }

        private bool m_sunSynchedWithTime = true;
        [Browsable(true), Category("3D settings")]
        [Description("Sun position is computed according to time.")]
        public bool SunSynchedWithTime
        {
            get
            {
                return m_sunSynchedWithTime;
            }
            set
            {
                m_sunSynchedWithTime = value;
            }
        }

        private double m_sunElevation = Math.PI / 4;
        [Browsable(true), Category("3D settings")]
        [Description("Sun elevation when not synched to time.")]
        public double SunElevation
        {
            get
            {
                return m_sunElevation;
            }
            set
            {
                m_sunElevation = value;
            }
        }

        private double m_sunHeading = -Math.PI / 4;
        [Browsable(true), Category("3D settings")]
        [Description("Sun direction when not synched to time.")]
        public double SunHeading
        {
            get
            {
                return m_sunHeading;
            }
            set
            {
                m_sunHeading = value;
            }
        }

        private double m_sunDistance = 150000000000;
        [Browsable(true), Category("3D settings")]
        [Description("Sun distance in meter.")]
        public double SunDistance
        {
            get
            {
                return m_sunDistance;
            }
            set
            {
                m_sunDistance = value;
            }
        }

        private int m_shadingAmbientColor = System.Drawing.Color.FromArgb(50, 50, 50).ToArgb();
        [Browsable(true), Category("3D settings")]
        [Description("The background ambient color when sun shading is enabled.")]
        [XmlIgnore]
        public System.Drawing.Color ShadingAmbientColor
        {
            get
            {
                return System.Drawing.Color.FromArgb(m_shadingAmbientColor);
            }
            set
            {
                m_shadingAmbientColor = value.ToArgb();
            }
        }

        private int m_standardAmbientColor = System.Drawing.Color.FromArgb(64, 64, 64).ToArgb();
        [Browsable(true), Category("3D settings")]
        [Description("The background ambient color only ambient lighting is used.")]
        [XmlIgnore]
        public System.Drawing.Color StandardAmbientColor
        {
            get
            {
                 return System.Drawing.Color.FromArgb(m_standardAmbientColor);
            }
            set
            {
                m_standardAmbientColor = value.ToArgb();
            }
        }

        [Browsable(true), Category("3D settings")]
        [Description("Use lower priority update thread to allow smoother rendering at the expense of data update frequency.")]
        public bool UseBelowNormalPriorityUpdateThread
        {
            get
            {
                return m_UseBelowNormalPriorityUpdateThread;
            }
            set
            {
                m_UseBelowNormalPriorityUpdateThread = value;
            }
        }

        float mFogNearFactor = 2.0f;
        float mFogFarFactor = 60.0f;
        int fogColor = Color.FromArgb(208, 208, 208).ToArgb();
        [Browsable(true), Category("3D settings")]
        [Description("")]
        public float FogNearFactor
        {
            get
            {
                return mFogNearFactor;
            }
            set
            {
                mFogNearFactor = value;
            }
        }

        [Browsable(true), Category("3D settings")]
        [Description("")]
        public float FogFarFactor
        {
            get
            {
                return mFogFarFactor;
            }
            set
            {
                mFogFarFactor = value;
            }
        }
        [Browsable(true), Category("3D settings")]
        [Description("")]
        public int FogColor
        {
            get
            {
                return fogColor;
            }
            set
            {
                fogColor = value;
            }
        }
        #endregion

        #region Terrain

        private float minSamplesPerDegree = 3.0f;

        [Browsable(true), Category("Terrain")]
        [Description("Sets the minimum samples per degree for which elevation is applied.")]
        public float MinSamplesPerDegree
        {
            get
            {
                return minSamplesPerDegree;
            }
            set
            {
                minSamplesPerDegree = value;
            }
        }

        private bool useWorldSurfaceRenderer = true;

        [Browsable(true), Category("Terrain")]
        [Description("Use World Surface Renderer for the visualization of multiple terrain-mapped layers.")]
        public bool UseWorldSurfaceRenderer
        {
            get
            {
                return useWorldSurfaceRenderer;
            }
            set
            {
                useWorldSurfaceRenderer = value;
            }
        }

        private float verticalExaggeration = 3.0f;

        [Browsable(true), Category("Terrain")]
        [Description("Terrain height multiplier.")]
        public float VerticalExaggeration
        {
            get
            {
                return verticalExaggeration;
            }
            set
            {
                if (value > 20)
                    throw new ArgumentException("Vertical exaggeration out of range: " + value);
                if (value <= 0)
                    verticalExaggeration = Single.Epsilon;
                else
                    verticalExaggeration = value;
            }
        }

        #endregion

        #region Measure tool

        private MeasureMode measureMode;

        private bool measureShowGroundTrack;

        private int measureLineGroundColor = Color.FromArgb(222, 0, 255, 0).ToArgb();
        private int measureLineLinearColor = Color.FromArgb(255, 255, 0, 0).ToArgb();

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of the linear distance measure line.")]
        public Color MeasureLineLinearColor
        {
            get { return Color.FromArgb(measureLineLinearColor); }
            set { measureLineLinearColor = value.ToArgb(); }
        }

        [Browsable(false)]
        public int MeasureLineLinearColorXml
        {
            get { return measureLineLinearColor; }
            set { measureLineLinearColor = value; }
        }

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of the ground track measure line.")]
        public Color MeasureLineGroundColor
        {
            get { return Color.FromArgb(measureLineGroundColor); }
            set { measureLineGroundColor = value.ToArgb(); }
        }

        [Browsable(false)]
        public int MeasureLineGroundColorXml
        {
            get { return measureLineGroundColor; }
            set { measureLineGroundColor = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Display the ground track column in the measurement statistics table.")]
        public bool MeasureShowGroundTrack
        {
            get { return measureShowGroundTrack; }
            set { measureShowGroundTrack = value; }
        }

        [Browsable(true), Category("UI")]
        [Description("Measure tool operation mode.")]
        public MeasureMode MeasureMode
        {
            get { return measureMode; }
            set { measureMode = value; }
        }

        #endregion

        #region Units
        private UnitsLength m_displayUnits = UnitsLength.Metric;
        [Browsable(true), Category("Units")]
        [Description("The target display units for measurements.")]
        public UnitsLength DisplayUnits
        {
            get
            {
                return m_displayUnits;
            }
            set
            {
                m_displayUnits = value;
            }
        }
        #endregion

        public ScaleMethod TerrainScaleMethod { get; set;}

        public int StandardDeviationNumber { get; set; }

        private TimeSpan terrainTileRetryInterval = TimeSpan.FromMinutes(30);

        [Browsable(true), Category("Terrain")]
        [Description("Retry Interval for missing terrain tiles.")]
        [XmlIgnore]
        public TimeSpan TerrainTileRetryInterval
        {
            get
            {
                return terrainTileRetryInterval;
            }
            set
            {
                TimeSpan minimum = TimeSpan.FromMinutes(1);
                if (value < minimum)
                    value = minimum;
                terrainTileRetryInterval = value;
            }
        }

        private int downloadQueuedColor = Color.FromArgb(50, 128, 168, 128).ToArgb();

        [XmlIgnore]
        [Browsable(true), Category("UI")]
        [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        [Description("Color of queued for download image tile rectangles.")]
        public Color DownloadQueuedColor
        {
            get { return Color.FromArgb(downloadQueuedColor); }
            set { downloadQueuedColor = value.ToArgb(); }
        }

         [XmlIgnore]
         [Browsable(true), Category("UI")]
        public bool RenderCarsianGridOnly
        {
            get;
            set;
        }

        #region Layers
        public System.Collections.ArrayList loadedLayers = new System.Collections.ArrayList();
        private bool useDefaultLayerStates = true;
        private int maxSimultaneousDownloads = 1;

        [Browsable(true), Category("Layers")]
        public bool UseDefaultLayerStates
        {
            get { return useDefaultLayerStates; }
            set { useDefaultLayerStates = value; }
        }

        [Browsable(false), Category("Layers")]
        public int MaxSimultaneousDownloads
        {
            get { return maxSimultaneousDownloads; }
            set
            {
                if (value > 20)
                    maxSimultaneousDownloads = 20;
                else if (value < 1)
                    maxSimultaneousDownloads = 1;
                else
                    maxSimultaneousDownloads = value;
            }
        }

        [Browsable(true), Category("Layers")]
        public System.Collections.ArrayList LoadedLayers
        {
            get { return loadedLayers; }
            set { loadedLayers = value; }
        }
        #endregion

        [Browsable(true), Category("Logging")]
        public bool Log404Errors
        {
            get { return HUST.WREIS.Dot3D.Net.WebDownload.Log404Errors; }
            set { HUST.WREIS.Dot3D.Net.WebDownload.Log404Errors = value; }
        }

        string language = "en-US";
        [Browsable(true), Category("Language")]
        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        float _FlagScale = 50.0f;
        [Browsable(true), Category("Identification")]
        public float FlagScale
        {
            get { return _FlagScale; }
            set { _FlagScale = value; }
        }

        float _FlagAboveSurface = 50.0f;
        [Browsable(true), Category("Identification")]
        public float FlagAboveSurface
        {
            get { return _FlagAboveSurface; }
            set { _FlagAboveSurface = value; }
        }


        bool _EnableHighPerfomance = false;
        [Browsable(true), Category("Performance")]
        public bool EnableHighPerfomance
        {
            get { return _EnableHighPerfomance; }
            set { _EnableHighPerfomance = value; }
        }

        // comment out ToString() to have namespace+class name being used as filename
        public override string ToString()
        {
            return "SceneWorld";
        }

      
        
    }
}