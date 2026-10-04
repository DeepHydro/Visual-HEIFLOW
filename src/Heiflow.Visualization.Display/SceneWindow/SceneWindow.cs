using Heiflow.Core;
using Heiflow.Core.Drawing;
using Heiflow.Core.MyMath;
using Heiflow.Models.Generic;
using Heiflow.Visualization.Controls;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D.Camera;
using HUST.WREIS.Dot3D.Display;
using HUST.WREIS.Dot3D.Display.Properties;
using HUST.WREIS.Dot3D.Interop;
using HUST.WREIS.Dot3D.Menu;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using Heiflow.Presentation.Services;
using Heiflow.Presentation.Controls;

namespace HUST.WREIS.Dot3D
{
    public partial class SceneWindow : Control, IGlobe
    {
        /// <summary>
        /// Direct3D rendering m_Device3d
        /// </summary>
        public Device m_Device3d;
        public PresentParameters m_presentParams;
        public DrawArgs drawArgs;
        public World m_World;
        public Cache m_Cache;
        public Thread m_WorkerThread;
        public Thread m_TerrainThread;
        public bool showDiagnosticInfo;
        public string _caption = "";
        public long lastFpsUpdateTime;
        public int frameCounter;
        public float fps;
        public string saveScreenShotFilePath;
        public ImageFileFormat saveScreenShotImageFileFormat = ImageFileFormat.Bmp;
        public bool m_WorkerThreadRunning;
        public bool m_TerrainThreadRunning;

        private LayerManagerButton layerManagerButton;
        public MenuBar _menuBar = new MenuBar(MenuAnchor.Top, 90);
        public bool m_isRenderDisabled; // True when 3D isn't active - CPU saver
        public bool isMouseDragging;
        public Point mouseDownStartPosition = Point.Empty;
        public bool renderWireFrame;
        public System.Timers.Timer m_FpsTimer = new System.Timers.Timer(250);
        Microsoft.DirectX.Direct3D.Font captionTextFont;
        Microsoft.DirectX.Direct3D.Font infoTextFont;
        Microsoft.DirectX.Direct3D.Font titleTextFont;
        private HUST.WREIS.Dot3D.VisualControl.ProgressBar mProgressBar;
        private LineGraph m_FpsGraph = new LineGraph();
        private const int positionAlphaStep = 20;
        private int positionAlpha = 255;
        private int positionAlphaMin = 40;
        private int positionAlphaMax = 205;

        private WavingFlagLayer mSelectedFlag = null;
        private ICell mSelectedCell = null;
        private IDX3DLayerRender _GridRenderDX;
        private ProfileRender _ProfileRender;
        private bool isRenderFlagInfo = false;
        private Point mCurrentMousePosition;
        private List<IDX3DLayer> _DX3DLayers;
        /// <summary>
        /// Draws a small cross hairs for the user to pinpoint the exact lat/lon
        /// TODO: Make this user-resizeable and color customizable
        /// </summary>
        Line crossHairs;
        int crossHairColor = Color.GhostWhite.ToArgb();
        public static System.Resources.ResourceManager ResMan;
        public bool ShowPerformanceInfo { get; set; }
        public bool ShowStatisticsInfo { get; set; }
        public bool ShowTitleInfo { get; set; }
        public int infoForeColor3 { get; set; }
        public string Title { get; set; }

        bool m_FpsUpdate = false;
        public StatisticsInfo StatisticsInfo
        {
            get;
            set;
        }

        public GraphChart3D Chart
        {
            get;
            set;
        }

        public List<IDX3DLayer> DX3DLayers
        {
            get
            {
                return _DX3DLayers;
            }
            set
            {
                _DX3DLayers = value;
                InitializeDX3DLayers();
            }
        }

        public IActiveDataService ActiveDataService
        {
            get;
            set;
        }

        public IShellService ShellService
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.WorldWindow"/> class.
        /// </summary>
        public SceneWindow()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            // The m_Device3d can't be created unless the control is at least 1 x 1 pixels in size
            this.Size = new Size(1, 1);

            try
            {
                // Now perform the rendering m_Device3d initialization
                // Skip DirectX initialization in design mode
                if (!IsInDesignMode())
                    this.InitializeGraphics();

                //Post m_Device3d creation initialization
                this.drawArgs = new DrawArgs(m_Device3d, this);
                this.m_RootWidget = new HUST.WREIS.Dot3D.Widgets.RootWidget(this);
                this.m_NewRootWidget = new HUST.WREIS.Dot3D.NewWidgets.RootWidget(this);

                //this.m_RootWidget.ChildWidgets.Add(layerManager);
                DrawArgs.RootWidget = this.m_RootWidget;
                DrawArgs.NewRootWidget = this.m_NewRootWidget;

                m_FpsTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_FpsTimer_Elapsed);
                m_FpsTimer.Start();

                TimeKeeper.Start();

                mProgressBar = new VisualControl.ProgressBar(200, 10);

                captionTextFont = drawArgs.CreateFont("宋体,Arial", 8.5f, FontStyle.Bold);
                infoTextFont = drawArgs.CreateFont("宋体,Arial", 9.0f, FontStyle.Bold);
                titleTextFont = drawArgs.CreateFont("Arial", 14.0f, FontStyle.Bold);
                infoBackgourdColor = Color.FromArgb(100, 54, 54, 54).ToArgb();
                infoForeColor1 = Color.FromArgb(255, 255, 255, 0).ToArgb();
                infoForeColor2 = Color.FromArgb(255, 255, 255, 255).ToArgb();
                infoForeColor3 = Color.FromArgb(255, 255, 0, 0).ToArgb();
                ShowTitleInfo = true;

            }
            catch (InvalidCallException caught)
            {
                throw new InvalidCallException(
                    "Unable to locate a compatible graphics adapter. Make sure you are running the latest version of DirectX.", caught);
            }
            catch (NotAvailableException caught)
            {
                throw new NotAvailableException(
                    "Unable to locate a compatible graphics adapter. Make sure you are running the latest version of DirectX.", caught);
            }
        }

        public void SetResManager()
        {
            if (World.Settings.Language == "zh-CN")
                ResMan = new System.Resources.ResourceManager("HUST.WREIS.Dot3D.Display.Properties.Resource-zh-CN", System.Reflection.Assembly.GetExecutingAssembly());
            else if (World.Settings.Language == "en-US")
                ResMan = new System.Resources.ResourceManager("HUST.WREIS.Dot3D.Display.Properties.Resource-en-US", System.Reflection.Assembly.GetExecutingAssembly());

            var addhotp = ResMan.GetString("AddHotPt");
            var delhotp = ResMan.GetString("DelHotPt");
            var clchotp = ResMan.GetString("ClearHotPt");

            var viewCellVal = ResMan.GetString("ViewCellVal");
            var viewCurrentLayerVal = ResMan.GetString("ViewCurrentLayerVal");
            var viewAllLayerVal = ResMan.GetString("ViewAllLayerVal");
            var viewAvLayerVal = ResMan.GetString("ViewAvLayerVal");
            var viewHistLayerVal = ResMan.GetString("ViewHistLayerVal");
            var viewActiveTimeSeries = ResMan.GetString("ViewActiveTimeSeries");
            var viewProfile = ResMan.GetString("ViewProfile");
            var viewRowProfile = ResMan.GetString("ViewRowProfile");
            var viewColProfile = ResMan.GetString("ViewColProfile");
            var clearColProfile = ResMan.GetString("ClearColProfile");
            var selectionCell = ResMan.GetString("SelectionCell");
            var clcSelectionCell = ResMan.GetString("ClcSelectionCell");
            var hideSelectionCell = ResMan.GetString("HideSelectionCell");

            ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            ToolStripMenuItem tsi = new ToolStripMenuItem(addhotp, Resources.flag, new EventHandler(Menuitem_addHotPoint_Click), "tsiAddHotPoint");
            tsi.Enabled = false;
            ContextMenuStrip.Items.Add(tsi);
            tsi = new ToolStripMenuItem(delhotp, Resources.flag__arrow, new EventHandler(Menuitem_DeleteHotPoint_Click), "tsiDelHotPoint");
            tsi.Enabled = false;
            ContextMenuStrip.Items.Add(tsi);
            tsi = new ToolStripMenuItem(clchotp, Resources.draw_eraser, new EventHandler(Menuitem_ClearHotPoint_Click), "tsiClearHotPoint");
            ContextMenuStrip.Items.Add(tsi);

            ContextMenuStrip.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsiViewHotPoint = new ToolStripMenuItem(viewCellVal, Resources.grid_snap);
            tsiViewHotPoint.Name = "tsiViewHotPoint";
            tsiViewHotPoint.Enabled = false;
            //ToolStripMenuItem tsiVHP1 = new ToolStripMenuItem(viewCurrentLayerVal, Resources.layer_grid,
            //    new EventHandler(Menuitem_ViewHotPoint_Click), "tsiViewHPInCurrentLayer");
       //     ToolStripMenuItem tsiVHP2 = new ToolStripMenuItem(viewAllLayerVal, Resources.layers_map,
       //        new EventHandler(Menuitem_ViewHotPoint_Click), "tsiViewHPInAllLayers");
       //     ToolStripMenuItem tsiVHP3 = new ToolStripMenuItem(viewAvLayerVal, Resources.layers_map,
       //new EventHandler(Menuitem_ViewHotPoint_Click), "tsiViewHPInAverageLayer");
       //     tsiViewHotPoint.DropDownItems.AddRange(new ToolStripItem[] { tsiVHP1, tsiVHP2, tsiVHP3 });
            //tsiViewHotPoint.DropDownItems.AddRange(new ToolStripItem[] { tsiVHP1});
            //ContextMenuStrip.Items.Add(tsiViewHotPoint);

            ToolStripMenuItem tsiViewActiveTimeSeries = new ToolStripMenuItem(viewActiveTimeSeries, Resources.layer_histogram,
                new EventHandler(Menuitem_ViewActiveTimeSeries_Click), "tsiViewActiveTimeSeries");
            tsiViewActiveTimeSeries.Enabled = true;
            ContextMenuStrip.Items.Add(tsiViewActiveTimeSeries);

            ContextMenuStrip.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsiViewProfile = new ToolStripMenuItem(viewProfile, Resources.grid_snap);
            tsiViewProfile.Name = "tsiViewProfile";
            tsiViewProfile.Enabled = false;
            ToolStripMenuItem tsiVPRow = new ToolStripMenuItem(viewRowProfile, Resources.layer_his,
              new EventHandler(Menuitem_ViewProfile_Click), "tsiVPRow");
            ToolStripMenuItem tsiVPCol = new ToolStripMenuItem(viewColProfile, Resources.layer_raster,
               new EventHandler(Menuitem_ViewProfile_Click), "tsiVPCol");
            ToolStripMenuItem tsiClcProfile = new ToolStripMenuItem(clearColProfile, Resources.layer_remove,
             new EventHandler(Menuitem_ViewProfile_Click), "tsiClcProfile");
            tsiViewProfile.DropDownItems.AddRange(new ToolStripItem[] { tsiVPRow, tsiVPCol, tsiClcProfile });
            ContextMenuStrip.Items.Add(tsiViewProfile);

            //ContextMenuStrip.Items.Add(new ToolStripSeparator());

            //ToolStripMenuItem tsiSelections = new ToolStripMenuItem(selectionCell, Resources.select);
            //tsiSelections.Name = "tsiSelections";
            //tsiSelections.Enabled = true;
            //ToolStripMenuItem tsiClearSelections = new ToolStripMenuItem(clcSelectionCell, Resources.xhtml_delete,
            //    new EventHandler(Menuitem_ClearSelections_Click), "tsiClearSelections");
            //tsiClearSelections.Enabled = true;
            //ToolStripMenuItem tsiHideSelections = new ToolStripMenuItem(hideSelectionCell, Resources.monitor_wallpaper,
            // new EventHandler(Menuitem_HideSelections_Click), "tsiHideSelections");
            //tsiHideSelections.Enabled = true;
            //tsiSelections.DropDownItems.Add(tsiClearSelections);
            //tsiSelections.DropDownItems.Add(tsiHideSelections);
            //ContextMenuStrip.Items.Add(tsiSelections);

            ContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);
            ContextMenuStrip.Opened += new EventHandler(ContextMenuStrip_Opened);
        }

        #region CFD rendering
   
        private void InitializeDX3DLayers()
        {
            var grid_layer = SelectLayer("Grid");
            if (grid_layer != null)
            {
                _GridRenderDX = grid_layer.RenderDX;
                _GridRenderDX.DataSourceChanged += RenderableMFGrid_DataSourceChanged;
                _GridRenderDX.GridValuesChanged += RenderableMFGrid_GridValuesChanged;
            }

            var profile = SelectLayer("Profile");
            if(profile != null)
                _ProfileRender = profile.RenderDX as ProfileRender;
        }

        public IDX3DLayer SelectLayer(string name)
        {
            var buf = from lay in DX3DLayers where lay.Name == name select lay;
            if (buf.Any())
                return buf.First();
            else
                return null;
        }

        private void ContextMenuStrip_Opened(object sender, EventArgs e)
        {
            mShiftPressed = false;
            if (_GridRenderDX != null)
            {
                Point pt = ContextMenuStrip.MousePosition;
                double lat;
                double lon;
                drawArgs.WorldCamera.PickingRayIntersection(pt.X, pt.Y, out lat, out lon);

                mSelectedCell = _GridRenderDX.SelectCell(lat, lon);             
                ContextMenuStrip.Items["tsiAddHotPoint"].Enabled = mSelectedCell != null;
                ContextMenuStrip.Items["tsiViewActiveTimeSeries"].Enabled = mSelectedCell != null;
            }
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (!mShiftPressed)
            {
                ContextMenuStrip.Items["tsiAddHotPoint"].Enabled = false;
                ContextMenuStrip.Items["tsiViewActiveTimeSeries"].Enabled = false;
                mSelectedCell = null;
                e.Cancel = true;
            }
        }

        private void Menuitem_addHotPoint_Click(object sender, EventArgs e)
        {
            Point pt = ContextMenuStrip.MousePosition;
            Angle lat;
            Angle lon;
            this.drawArgs.WorldCamera.PickingRayIntersection(pt.X, pt.Y, out lat, out lon);

            string name = pt.X.ToString() + pt.Y.ToString();
            RenderableObjectList hotPoints = (m_World.DefaultLayerList["HotPoints"] as RenderableObjectList);
            if (hotPoints[name] == null)
            {
                //WavingFlagLayer flag = new WavingFlagLayer(name, m_World, lat.Degrees, lon.Degrees, ConfigurationManager.Engine3DSettings.IconPath + "red-flag.png");
                WavingFlagLayer flag = new WavingFlagLayer(name, m_World, mSelectedCell.CentralLatitude, mSelectedCell.CentralLongitude, ConfigurationManager.Engine3DSettings.IconPath + "red-flag.png");
                flag.IsOn = true;
                flag.ScaleX = World.Settings.FlagScale;
                flag.ScaleY = World.Settings.FlagScale;
                flag.ScaleZ = World.Settings.FlagScale;
                flag.ScreenPoint = pt;
                flag.Tag = mSelectedCell;
                flag.ShowHighlight = true;
                flag.RenderPriority = RenderPriority.Custom;
                flag.OnMouseEnterEvent += new EventHandler(flag_OnMouseEnterEvent);
                flag.OnMouseLeaveEvent += new EventHandler(flag_OnMouseLeaveEvent);
                flag.isSelectable = true;
                hotPoints.Add(flag);
                if (!_GridRenderDX.SelectedCells.Contains(mSelectedCell))
                {
                    _GridRenderDX.SelectedCells.Add(mSelectedCell);
                }
            }
        }

        private void Menuitem_DeleteHotPoint_Click(object sender, EventArgs e)
        {
            if (mSelectedFlag != null)
            {
                RenderableObjectList hotPoints = (m_World.DefaultLayerList["HotPoints"] as RenderableObjectList);
                hotPoints.ChildObjects.Remove(mSelectedFlag);
                isRenderFlagInfo = false;
                _GridRenderDX.SelectedCells.Remove(mSelectedFlag.Tag as ICell);
            }
        }

        private void Menuitem_ClearHotPoint_Click(object sender, EventArgs e)
        {
            RenderableObjectList hotPoints = (m_World.DefaultLayerList["HotPoints"] as RenderableObjectList);
            isRenderFlagInfo = false;
            hotPoints.ChildObjects.Clear();
            _GridRenderDX.SelectedCells.Clear();
        }

        private void Menuitem_ViewHotPoint_Click(object sender, EventArgs e)
        {
            if (mSelectedFlag != null)
            {
                Cell c = mSelectedFlag.Tag as Cell;
                ToolStripMenuItem tsi = sender as ToolStripMenuItem;
                Chart.Visible = true;
                Chart.GraphPane.XAxis.Type = ZedGraph.AxisType.Linear;
                string celllable = "Cell [" + c.I + "," + c.J + "]:  ";
                //string lable = "";
                //if (c.Render.DataSource != null)
                //{
          
                //}
            }
        }

        private void Menuitem_ClearSelections_Click(object sender, EventArgs e)
        {
            if (_GridRenderDX != null)
            {
                _GridRenderDX.SelectedVertexes = null;
            }
        }

        private void Menuitem_HideSelections_Click(object sender, EventArgs e)
        {
            if (_GridRenderDX != null)
            {
                (_GridRenderDX.RenderableObject as RegularGridLayer).HighLightSelectedCell =
                    !(_GridRenderDX.RenderableObject as RegularGridLayer).HighLightSelectedCell;
                (sender as ToolStripMenuItem).Checked =
                    (_GridRenderDX.RenderableObject as RegularGridLayer).HighLightSelectedCell;
            }
        }

        private void Menuitem_ViewProfile_Click(object sender, EventArgs e)
        {
            if (mSelectedFlag != null)
            {
                Cell c = mSelectedFlag.Tag as Cell;
                ProfileType type = ProfileType.Column;
                if ((sender as ToolStripMenuItem).Name == "tsiVPRow")
                {
                    type = ProfileType.Row;
                    _ProfileRender.Profiles.Add(new GridProfile(_ProfileRender, type, c.I));
                }
                else if ((sender as ToolStripMenuItem).Name == "tsiVPCol")
                {
                    type = ProfileType.Column;
                    //c.Render.GridProfile.Add(new GridProfile(c.Render, type, c.J));
                    _ProfileRender.Profiles.Add(new GridProfile(_ProfileRender, type, c.J));
                    // c.Grid.Layer.DrawProfile = true;
                }
                else
                {
                    _ProfileRender.Profiles.Clear();
                    //   c.Grid.Layer.DrawProfile = false;
                }
            }
        }

        private void Menuitem_ViewActiveTimeSeries_Click(object sender, EventArgs e)
        {
            if (ActiveDataService.Source != null)
            {
                ShellService.SelectPanel(DockPanelNames.WinChartPanel);
                int ntime = ActiveDataService.Source.Size[1];
                float[] yy = new float[ntime];
                if(ActiveDataService.Source.DateTimes == null)
                {
                    ActiveDataService.Source.DateTimes = new DateTime[ntime];
                    for (int i = 0; i < ntime; i++)
                    {
                        ActiveDataService.Source.DateTimes[i] = DateTime.Now.AddDays(i);
                    }
                }
                for (int i = 0; i < ntime; i++)
                {
                    yy[i] = ActiveDataService.Source[ActiveDataService.Source.SelectedVariableIndex,i,mSelectedCell.SerialIndex];
                }
                var name = string.Format("{0} at Cell [{1},{2}]", ActiveDataService.Source.Name, mSelectedCell.I + 1, mSelectedCell.J + 1);
                ShellService.WinChart.Plot<float>(ActiveDataService.Source.DateTimes, yy, name, System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine);
            }
        }

        private void flag_OnMouseLeaveEvent(object sender, EventArgs e)
        {
            (sender as WavingFlagLayer).ShowHighlight = false;
            mSelectedFlag = null;
            ContextMenuStrip.Items["tsiDelHotPoint"].Enabled = false;
            ContextMenuStrip.Items["tsiViewHotPoint"].Enabled = false;
            ContextMenuStrip.Items["tsiViewProfile"].Enabled = false;
            isRenderFlagInfo = false;
        }

        private void flag_OnMouseEnterEvent(object sender, EventArgs e)
        {
            mSelectedFlag = (sender as WavingFlagLayer);
            mSelectedFlag.ShowHighlight = true;
            mSelectedFlag.ScreenPoint = mCurrentMousePosition;
            ContextMenuStrip.Items["tsiDelHotPoint"].Enabled = true;
            ContextMenuStrip.Items["tsiViewHotPoint"].Enabled = true;
            ContextMenuStrip.Items["tsiViewProfile"].Enabled = true;
            isRenderFlagInfo = true;
        }

        private void RenderFlagInfo(ICell c)
        {
            if (c != null)
            {
                var str1 = ResMan.GetString("CellIndex");
                var str2 = ResMan.GetString("CellValue");

                string captionText = str1 + c.ToString() + "\n";
                captionText += str2 + c.CurrentValue.ToString("0.0000");
                DrawTextFormat dtf = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Left;
                int x = mSelectedFlag.ScreenPoint.X;
                int y = mSelectedFlag.ScreenPoint.Y;
                Rectangle textRect = Rectangle.FromLTRB(x + 10, y + 10, x + 200, y + 100);
                MenuUtils.DrawBox(x, y, 200, 100, 0.0f, Color.FromArgb(100, 0, 0, 0).ToArgb(), drawArgs.device);
                captionTextFont.DrawText(null, captionText, textRect, dtf, Color.Wheat.ToArgb());
            }
        }


        private void RenderableMFGrid_GridValuesChanged(object sender, EventArgs e)
        {
            _GridRenderDX.UpdateCellValues();
        }

        private void RenderableMFGrid_DataSourceChanged(object sender, EventArgs e)
        {
            _GridRenderDX.UpdateCellValues();
        }

        public void Animator_CurrentChanged(object sender, int e)
        {
            if (!ShellService.ShowAnimationMonitor)
                return;
            var lable = ActiveDataService.Source.Name;
            if (!Chart.Visible)
            {
                Chart.Visible = true;
                Chart.Show();
                Chart.GraphPane.XAxis.Title.Text = "Date and Time";
                Chart.GraphPane.YAxis.Title.Text = lable;
            }

            if (ActiveDataService.SourceStatistics != null)
            {
                ZedGraph.PointPairList plist = new ZedGraph.PointPairList();
                for (int i = 0; i <= e; ++i)
                {
                    plist.Add(i, ActiveDataService.SourceStatistics[0, i, 0]);
                }
                Chart.DrawLine(lable, 1.0f, plist, Color.Blue, ZedGraph.SymbolType.Diamond);
            }
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Determine whether any window messages is queued.
        /// </summary>
        private static bool IsAppStillIdle
        {
            get
            {
                NativeMethods.Message msg;
                return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        public World CurrentWorld
        {
            get
            {
                return m_World;
            }
            set
            {
                m_World = value;
                if (m_World != null)
                {
                    MomentumCamera camera = new MomentumCamera(m_World.Position, m_World.EquatorialRadius);
                    if (!World.Settings.CameraResetsAtStartup)
                    {
                        camera.SetPosition(
                            World.Settings.CameraLatitude.Degrees,
                            World.Settings.CameraLongitude.Degrees,
                            World.Settings.CameraHeading.Degrees,
                            World.Settings.CameraAltitude,
                            World.Settings.CameraTilt.Degrees,
                            0
                            );
                    }
                    this.drawArgs.WorldCamera = camera;

                    this.drawArgs.CurrentWorld = value;
                    this.layerManagerButton = new LayerManagerButton(
                        Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Resources\Images\layer-manager.png"),
                        m_World);

                    this._menuBar.AddToolsMenuButton(this.layerManagerButton, 0);
                    this._menuBar.AddToolsMenuButton(new PositionMenuButton(Path.GetDirectoryName(Application.ExecutablePath) + "\\Resources\\Images\\coordinates.png"), 1);
                    this._menuBar.AddToolsMenuButton(new LatLonMenuButton(Path.GetDirectoryName(Application.ExecutablePath) + "\\Resources\\Images\\latlong.png", m_World), 2);
                    this.layerManagerButton.SetPushed(World.Settings.ShowLayerManager);

                    // TODO: Decide how to load grids
                    m_World.RenderableObjects.Add(new Renderable.LatLongGrid(m_World));
                    ProgressPercent = -1.0f;
                }
            }
        }

        public string Caption
        {
            get
            {
                return this._caption;
            }
            set
            {
                this._caption = value;
            }
        }

        public DrawArgs DrawArgs
        {
            get { return this.drawArgs; }
        }


        public MenuBar MenuBar
        {
            get
            {
                return this._menuBar;
            }
        }

        public bool ShowLayerManager
        {
            get
            {
                if (this.layerManagerButton != null)
                    return this.layerManagerButton.IsPushed();
                else
                    return false;
            }
            set
            {
                if (this.layerManagerButton != null)
                    this.layerManagerButton.SetPushed(value);
            }
        }

        public Cache Cache
        {
            get
            {
                return m_Cache;
            }
            set
            {
                m_Cache = value;
            }
        }

        /// <summary>
        /// Disables rendering (CPU tick saver)
        /// </summary>
        public bool IsRenderDisabled
        {
            get
            {
                return m_isRenderDisabled;
            }
            set
            {
                m_isRenderDisabled = value;
            }
        }

        public float ProgressPercent { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Moves to specified location.
        /// </summary>
        /// <param name="latitude">Latitude in degrees of target position. (-90 - 90).</param>
        /// <param name="longitude">Longitude in degrees of target position. (-180 - 180).</param>
        /// <param name="heading">Camera heading in degrees (0-360) or double.NaN for no change.</param>
        /// <param name="altitude">Camera altitude in meters or double.NaN for no change.</param>
        /// <param name="perpendicularViewRange"></param>
        /// <param name="tilt">Camera tilt in degrees (-90 - 90) or double.NaN for no change.</param>
        public void GotoLatLon(double latitude, double longitude, double heading, double altitude, double perpendicularViewRange, double tilt)
        {
            if (!double.IsNaN(perpendicularViewRange))
                altitude = m_World.EquatorialRadius * Math.Sin(MathEngine.DegreesToRadians(perpendicularViewRange * 0.5));
            if (altitude < 1)
                altitude = 1;
            this.drawArgs.WorldCamera.SetPosition(latitude, longitude, heading, altitude, tilt);
        }

        public void GotoLatLon(double latitude, double longitude)
        {
            this.drawArgs.WorldCamera.SetPosition(latitude, longitude,
                this.drawArgs.WorldCamera.Heading.Degrees,
                this.drawArgs.WorldCamera.Altitude,
                this.drawArgs.WorldCamera.Tilt.Degrees);
        }

        public void GotoLatLonAltitude(double latitude, double longitude, double altitude)
        {
            this.drawArgs.WorldCamera.SetPosition(latitude, longitude,
                this.drawArgs.WorldCamera.Heading.Degrees,
                altitude,
                this.drawArgs.WorldCamera.Tilt.Degrees);
        }

        public void GotoLatLonHeadingViewRange(double latitude, double longitude, double heading, double perpendicularViewRange)
        {
            double altitude = m_World.EquatorialRadius * Math.Sin(MathEngine.DegreesToRadians(perpendicularViewRange * 0.5));
            this.GotoLatLonHeadingAltitude(latitude, longitude, heading, altitude);
        }

        public void GotoLatLonViewRange(double latitude, double longitude, double perpendicularViewRange)
        {
            double altitude = m_World.EquatorialRadius * Math.Sin(MathEngine.DegreesToRadians(perpendicularViewRange * 0.5));
            this.GotoLatLonHeadingAltitude(latitude, longitude, this.drawArgs.WorldCamera.Heading.Degrees, altitude);
        }

        public void GotoLatLonHeadingAltitude(double latitude, double longitude, double heading, double altitude)
        {
            this.drawArgs.WorldCamera.SetPosition(latitude, longitude,
                heading,
                altitude,
                this.drawArgs.WorldCamera.Tilt.Degrees);
        }

        /// <summary>
        /// Saves the current view to file.
        /// </summary>
        /// <param name="filePath">Path and filename of output file.  
        /// Extension is used to determine the image format.</param>
        public void SaveScreenshot(string filePath)
        {
            if (m_Device3d == null)
                return;

            FileInfo saveFileInfo = new FileInfo(filePath);
            string ext = saveFileInfo.Extension.Replace(".", "");
            try
            {
                this.saveScreenShotImageFileFormat = (ImageFileFormat)Enum.Parse(typeof(ImageFileFormat), ext, true);
            }
            catch (ArgumentException)
            {
                throw new ApplicationException("Unknown file type/file extension for file '" + filePath + "'.  Unable to save.");
            }

            if (!saveFileInfo.Directory.Exists)
                saveFileInfo.Directory.Create();

            this.saveScreenShotFilePath = filePath;

            try
            {
                using (Surface backbuffer = m_Device3d.GetBackBuffer(0, 0, BackBufferType.Mono))
                    SurfaceLoader.Save(saveScreenShotFilePath, saveScreenShotImageFileFormat, backbuffer);
                saveScreenShotFilePath = null;
            }
            catch (InvalidCallException caught)
            {
                MessageBox.Show(caught.Message, "Screenshot save failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// The world render loop.  
        /// Borrowed from FlightGear and Tom Miller's blog
        /// </summary>
        public void OnApplicationIdle(object sender, EventArgs e)
        {
            // Sleep will always overshoot by a bit so under-sleep by
            // 2ms in the hopes of never oversleeping.
            const float SleepOverHeadSeconds = 2e-3f;

            // Overhead associated with displaying the frame
            const float PresentOverheadSeconds = 0;//3e-4f;

            try
            {
                if (Parent.Focused && !Focused)
                    Focus();

                while (IsAppStillIdle)
                {
                    if (!World.Settings.AlwaysRenderWindow && m_isRenderDisabled && !World.Settings.CameraHasMomentum)
                        return;

                    Render();

                    if (World.Settings.ThrottleFpsHz > 0)
                    {
                        // optionally throttle the frame rate (to get consistent frame
                        // rates or reduce CPU usage.
                        float frameSeconds = 1.0f / World.Settings.ThrottleFpsHz - PresentOverheadSeconds;

                        // Sleep for remaining period of time until next render
                        float sleepSeconds = frameSeconds - SleepOverHeadSeconds - DrawArgs.SecondsSinceLastFrame;
                        if (sleepSeconds > 0)
                        {
                            // Don't sleep too long. We don't know the accuracy of Thread.Sleep
                            Thread.Sleep((int)(1000 * sleepSeconds));

                            // Burn off what little time still remains at 100% CPU load
                            while (DrawArgs.SecondsSinceLastFrame < frameSeconds)
                            {
                                // Patience
                            }
                        }
                    }
                    // Flip
                    drawArgs.Present();
                }
            }
            catch (DeviceLostException)
            {
                AttemptRecovery();
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }

        #endregion

        #region Rendering
        /// <summary>
        /// Occurs when the control is redrawn and m_isRenderDisabled=true.
        /// All other painting is handled in WndProc.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Paint the last active scene if rendering is disabled to keep the ui responsive
            try
            {
                if (m_Device3d == null)
                {
                    e.Graphics.Clear(SystemColors.Control);
                    return;
                }

                // to prevent screen garbage when resizing
                Render();
                m_Device3d.Present();
            }
            catch (DeviceLostException)
            {
                try
                {
                    AttemptRecovery();

                    // Our surface was lost, force re-render
                    Render();

                    m_Device3d.Present();
                }
                catch (DirectXException)
                {
                    // Ignore a 2nd failure
                }
            }

        }

        public List<float> m_FrameTimes = new List<float>();
        public HUST.WREIS.Dot3D.Widgets.RootWidget m_RootWidget = null;
        public HUST.WREIS.Dot3D.NewWidgets.RootWidget m_NewRootWidget = null;
        /// <summary>
        /// Render the scene.
        /// </summary>
        public void Render()
        {
            long startTicks = 0;
            PerformanceTimer.QueryPerformanceCounter(ref startTicks);

            try
            {
                this.drawArgs.BeginRender();

                // Render the sky according to view - example, close to earth, render sky blue, render space as black
                System.Drawing.Color backgroundColor = System.Drawing.Color.Black;

                /*if(drawArgs.WorldCamera != null && 
                    drawArgs.WorldCamera.Altitude < 1000000f &&
                    m_World != null &&
                    m_World.Name.IndexOf("Earth") >= 0)
                {
                    float percent = 1 - (float)(drawArgs.WorldCamera.Altitude / 1000000);
                    if(percent > 1.0f)
                        percent = 1.0f;
                    else if(percent < 0.0f)
                        percent = 0.0f;

                    backgroundColor = System.Drawing.Color.FromArgb(
                        (int)(World.Settings.SkyColor.R*percent),
                        (int)(World.Settings.SkyColor.G*percent),
                        (int)(World.Settings.SkyColor.B*percent));
                }*/

                m_Device3d.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backgroundColor, 1.0f, 0);

                if (m_World == null)
                {
                    m_Device3d.BeginScene();
                    m_Device3d.EndScene();
                    m_Device3d.Present();
                    Thread.Sleep(25);
                    return;
                }

                if (m_WorkerThread == null)
                {
                    m_WorkerThreadRunning = true;
                    m_WorkerThread = new Thread(new ThreadStart(WorkerThreadFunc));
                    m_WorkerThread.Name = "SceneWindow.WorkerThreadFunc";
                    m_WorkerThread.IsBackground = true;
                    if (World.Settings.UseBelowNormalPriorityUpdateThread)
                    {
                        m_WorkerThread.Priority = ThreadPriority.BelowNormal;
                    }
                    else
                    {
                        m_WorkerThread.Priority = ThreadPriority.Highest;
                    }
                    // BelowNormal makes rendering smooth, but on slower machines updates become slow or stops
                    // TODO: Implement dynamic FPS limiter (or different solution)
                    m_WorkerThread.Start();
                }
                //if (m_TerrainThread == null)
                //{
                //    m_TerrainThreadRunning = true;
                //    m_TerrainThread = new Thread(new ThreadStart(TerrainThreadFunc));
                //    m_TerrainThread.Name = "SceneWindow.TerrainThreadFunc";
                //    m_TerrainThread.IsBackground = true;
                //    m_TerrainThread.Priority = ThreadPriority.Normal;
                //    m_TerrainThread.Start();
                //}

                this.drawArgs.WorldCamera.Update(m_Device3d);

                m_Device3d.BeginScene();

                // Set fill mode
                if (renderWireFrame)
                    m_Device3d.RenderState.FillMode = FillMode.WireFrame;
                else
                    m_Device3d.RenderState.FillMode = FillMode.Solid;

                drawArgs.RenderWireFrame = renderWireFrame;

                // Render the current planet
                m_World.Render(this.drawArgs);

                if (World.Settings.ShowCrosshairs)
                    this.DrawCrossHairs();

                frameCounter++;
                if (frameCounter == 30)
                {
                    fps = frameCounter / (float)(DrawArgs.CurrentFrameStartTicks - lastFpsUpdateTime) * PerformanceTimer.TicksPerSecond;
                    frameCounter = 0;
                    lastFpsUpdateTime = DrawArgs.CurrentFrameStartTicks;
                }

                m_RootWidget.Render(drawArgs);
                m_NewRootWidget.Render(drawArgs);
                drawArgs.device.RenderState.ZBufferEnable = false;

                // 3D rendering complete, switch to 2D for UI rendering

                // Restore normal fill mode
                if (renderWireFrame)
                    m_Device3d.RenderState.FillMode = FillMode.Solid;

                // Disable fog for UI
                m_Device3d.RenderState.FogEnable = false;

                if (ProgressPercent > 0.0f)
                {
                    RenderProgressBar(ProgressPercent);
                }

                RenderPositionInfo();
                RenderPerformanceInfo();
                RenderStatisticsInfo();
                ReanderTitle();
                //_menuBar.Render(drawArgs);
                //m_FpsGraph.Render(drawArgs);
                //if (saveScreenShotFilePath != null)
                //    SaveScreenShot();

                if (isRenderFlagInfo && mSelectedFlag != null)
                {
                    RenderFlagInfo(mSelectedFlag.Tag as ICell);
                }

                if (m_World.OnScreenMessages != null)
                {
                    try
                    {
                        foreach (OnScreenMessage dm in m_World.OnScreenMessages)
                        {
                            int xPos = (int)Math.Round(dm.X * this.Width);
                            int yPos = (int)Math.Round(dm.Y * this.Height);
                            Rectangle posRect = new Rectangle(xPos, yPos, this.Width, this.Height);
                            this.drawArgs.defaultDrawingFont.DrawText(null, dm.Message, posRect,
                                DrawTextFormat.NoClip | DrawTextFormat.WordBreak, Color.White);
                        }
                    }
                    catch (Exception)
                    {
                        // Don't let a script error cancel the frame.
                    }
                }

                m_Device3d.EndScene();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            finally
            {
                // World.Settings.ShowFpsGraph = true;
                if (World.Settings.ShowFpsGraph)
                {
                    long endTicks = 0;
                    PerformanceTimer.QueryPerformanceCounter(ref endTicks);
                    float elapsedMilliSeconds = 1000.0f / (1000.0f * (float)(endTicks - startTicks) / PerformanceTimer.TicksPerSecond);
                    m_FrameTimes.Add(elapsedMilliSeconds);
                }
                this.drawArgs.EndRender();
            }
            drawArgs.UpdateMouseCursor(this);
        }

        private int infoBackgourdColor = 0;
        private int infoForeColor1 = 0;
        private int infoForeColor2 = 0;

        private int infoPositionX = 10;
        private int infoPositionY = 15;
        private int infoColWidth = 120;
        private int infoRowHeight = 200;
        private int infoMargin = 5;
        private int infoPadding = 10;
        DrawTextFormat dtf = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Left;

        private void RenderInfoCell(int col, int row, string text, int textColor)
        {
            infoPositionY = this.Height - 20 - infoRowHeight;
            if (infoRowHeight <= 0)
                infoPositionY = 15;
            int x = infoPositionX + (infoColWidth + infoMargin) * col;
            int y = infoPositionY + (infoRowHeight + infoMargin) * row;
            MenuUtils.DrawBox(x, y, infoColWidth, infoRowHeight, 0.0f, infoBackgourdColor, drawArgs.device);

            Rectangle textRect = Rectangle.FromLTRB(x + infoPadding, y + infoPadding, x + infoColWidth - infoPadding, y + infoRowHeight - infoPadding);
            infoTextFont.DrawText(null, text, textRect, dtf, textColor);
        }
        public void ReanderTitle()
        {
            if (ShowTitleInfo)
            {
                infoPositionY = this.Height - 20 - 100;

                int x = this.Width / 2 - 50;
                int y = 30;
                // MenuUtils.DrawBox(x, y, 150, 30, 0.0f, infoBackgourdColor, drawArgs.device);

                Rectangle textRect = Rectangle.FromLTRB(x + infoPadding, y + infoPadding, x + 500 - infoPadding, y + 100 - infoPadding);
                titleTextFont.DrawText(null, Title, textRect, dtf, infoForeColor2);
            }
        }

        public void RenderPerformanceInfo()
        {
            if (ShowPerformanceInfo)
            {
                drawArgs.Present();
                //string captionText =
                //          "可用纹理内存: " +
                //           "\n每秒帧数FPS: " +
                //          "\n边界点: " +
                //          "\n瓦片数: " +
                //          "\n绘制对象数目: ";
                string captionText =
          "Memory: " +
           "\nFPS: " +
          "\nBoun Points: " +
          "\nTiles: " +
          "\nTiles Drawn: ";
                RenderInfoCell(0, 0, captionText, infoForeColor1);

                captionText = (m_Device3d.AvailableTextureMemory / 1024).ToString("N0") + " kB" +
                   "\n " + this.fps.ToString("f1") +
                  "\n " + this.drawArgs.numBoundaryPointsRendered.ToString() + " / " + this.drawArgs.numBoundaryPointsTotal.ToString() + " : " + this.drawArgs.numBoundariesDrawn.ToString() +
                  "\n " + (this.drawArgs.numberTilesDrawn * 0.25f).ToString() +
                  "\n " + m_World.RenderableObjects.Count.ToString("f0");
                RenderInfoCell(1, 0, captionText, infoForeColor2);
            }
        }

        public void RenderStatisticsInfo()
        {
            if (ShowStatisticsInfo && StatisticsInfo!= null)
            {
                //string captionText =
                //          "最大值: " + "\n" +
                //           "最小值: " + "\n" +
                //           "平均值: " + "\n" +
                //           "标准差: ";
                string captionText =
                        "Maximum: " + "\n" +
                         "Minimum: " + "\n" +
                         "Average: " + "\n" +
                         "SD: ";
                RenderInfoCell(2, 0, captionText, infoForeColor1);

                captionText =
                         StatisticsInfo.Max.ToString("0.000") + "\n" +
                         StatisticsInfo.Min.ToString("0.000") + "\n" +
                         StatisticsInfo.Average.ToString("0.000") + "\n" +
                         StatisticsInfo.StandardDeviation.ToString("0.000");
                RenderInfoCell(3, 0, captionText, infoForeColor2);
            }
        }

        protected void RenderPositionInfo()
        {
            // Render some Development information to screen
            string captionText = _caption;

            captionText += "\n" + this.drawArgs.UpperLeftCornerText;

            if (World.Settings.ShowPosition)
            {
                string alt = null;
                double agl = this.drawArgs.WorldCamera.AltitudeAboveTerrain;
                /*if(agl>100000)
                    alt = string.Format("{0:f2}km", agl/1000);
                else
                    alt = string.Format("{0:f0}m", agl);*/
                alt = ConvertUnits.GetDisplayString(agl);

                string dist = null;
                double dgl = this.drawArgs.WorldCamera.Distance;
                /*if(dgl>100000)
                    dist = string.Format("{0:f2}km", dgl/1000);
                else
                    dist = string.Format("{0:f0}m", dgl);*/
                dist = ConvertUnits.GetDisplayString(dgl);

                // Heading from 0 - 360
                double heading = this.drawArgs.WorldCamera.Heading.Degrees;
                if (heading < 0)
                    heading += 360;
                var posinfo = ResMan.GetString("PosInfo");//"经度: {0:f4}    纬度: {1:f4}    海拔: {2}m    方向: {3:f2}                                     视角海拔高度: {4}"
                captionText += String.Format(posinfo,
                    this.drawArgs.WorldCamera.Longitude.Degrees,
                    this.drawArgs.WorldCamera.Latitude.Degrees,
                    this.drawArgs.WorldCamera.TerrainElevation,
                    heading,
                    dist);

                if (this.showDiagnosticInfo)
                    captionText +=
                        "\nAvailable Texture Memory: " + (m_Device3d.AvailableTextureMemory / 1024).ToString("N0") + " kB" +
                        "\nBoundary Points: " + this.drawArgs.numBoundaryPointsRendered.ToString() + " / " + this.drawArgs.numBoundaryPointsTotal.ToString() + " : " + this.drawArgs.numBoundariesDrawn.ToString() +
                        "\nTiles Drawn: " + (this.drawArgs.numberTilesDrawn * 0.25f).ToString() +
                        "\n" + this.drawArgs.WorldCamera +
                        "\nFPS: " + this.fps.ToString("f1") +
                        "\nRO: " + m_World.RenderableObjects.Count.ToString("f0") +
                        "\nmLat: " + this.cLat.Degrees.ToString() +
                        "\nmLon: " + this.cLon.Degrees.ToString() +
                        "\n" + TimeKeeper.CurrentTimeUtc.ToLocalTime().ToLongTimeString();

                captionText = captionText.Trim();
                DrawTextFormat dtf = DrawTextFormat.NoClip | DrawTextFormat.WordBreak | DrawTextFormat.Center;
                int x = 7;
                int y = _menuBar != null && World.Settings.ShowToolbar ? 65 : this.Height - 14;
                Rectangle textRect = Rectangle.FromLTRB(x, y, this.Width - 8, this.Height - 4);

                // Hide position info when toolbar is open
                if (_menuBar.IsActive)
                {
                    positionAlpha -= positionAlphaStep;
                    if (positionAlpha < positionAlphaMin)
                    {
                        positionAlpha = positionAlphaMin;
                    }
                }
                else
                {
                    positionAlpha += positionAlphaStep;
                    if (positionAlpha > positionAlphaMax)
                        positionAlpha = positionAlphaMax;
                }
                MenuUtils.DrawBox(0, y - 4, this.Width, 18, 0.0f, Color.FromArgb(100, 0, 0, 0).ToArgb(), drawArgs.device);

                int positionBackColor = positionAlpha << 24;
                int positionForeColor = (int)((uint)(positionAlpha << 24) + 0xffffffu);
                captionTextFont.DrawText(null, captionText, textRect, dtf, positionBackColor);
                textRect.Offset(-1, -1);
                captionTextFont.DrawText(null, captionText, textRect, dtf, positionForeColor);
            }
        }

        public void RenderProgressBar(float percent)
        {
            Vector3 projectedPoint = new Vector3(DrawArgs.ParentControl.Width / 4, DrawArgs.ParentControl.Height / 2, 0.5f);
            mProgressBar.Draw(drawArgs, projectedPoint.X, projectedPoint.Y, percent, World.Settings.DownloadProgressColor.ToArgb());
        }

        protected void DrawCrossHairs()
        {
            int crossHairSize = 10;

            if (this.crossHairs == null)
            {
                crossHairs = new Line(m_Device3d);
            }

            Vector2[] vertical = new Vector2[2];
            Vector2[] horizontal = new Vector2[2];

            horizontal[0].X = this.Width / 2 - crossHairSize;
            horizontal[0].Y = this.Height / 2;
            horizontal[1].X = this.Width / 2 + crossHairSize;
            horizontal[1].Y = this.Height / 2;

            vertical[0].X = this.Width / 2;
            vertical[0].Y = this.Height / 2 - crossHairSize;
            vertical[1].X = this.Width / 2;
            vertical[1].Y = this.Height / 2 + crossHairSize;

            crossHairs.Begin();
            crossHairs.Draw(horizontal, crossHairColor);
            crossHairs.Draw(vertical, crossHairColor);
            crossHairs.End();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Attempt to restore the 3D m_Device3d
        /// </summary>
        public void AttemptRecovery()
        {
            try
            {
                m_Device3d.TestCooperativeLevel();
            }
            catch (DeviceLostException)
            {
            }
            catch (DeviceNotResetException)
            {
                try
                {
                    m_Device3d.Reset(m_presentParams);
                }
                catch (DeviceLostException)
                {
                    // If it's still lost or lost again, just do
                    // nothing
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_WorkerThread != null && m_WorkerThread.IsAlive)
                {
                    m_WorkerThreadRunning = false;
                    m_WorkerThread.Abort();
                }
                if (m_TerrainThread != null && m_TerrainThread.IsAlive)
                {
                    m_TerrainThreadRunning = false;
                    m_TerrainThread.Abort();
                }

                m_FpsTimer.Stop();
                if (m_World != null)
                {
                    m_World.Dispose();
                    m_World = null;
                }
                if (this.drawArgs != null)
                {
                    this.drawArgs.Dispose();
                    this.drawArgs = null;
                }
                if (this._menuBar != null)
                {
                    this._menuBar.Dispose();
                    this._menuBar = null;
                }

                m_Device3d.Dispose();
                //if (m_downloadIndicator != null)
                //{
                //    m_downloadIndicator.Dispose();
                //    m_downloadIndicator = null;
                //}
            }

            base.Dispose(disposing);
            GC.SuppressFinalize(this);
        }

        private void m_Device3d_DeviceResizing(object sender, CancelEventArgs e)
        {
            if (this.Size.Width == 0 || this.Size.Height == 0)
            {
                e.Cancel = true;
                return;
            }

            this.drawArgs.screenHeight = this.Height;
            this.drawArgs.screenWidth = this.Width;
        }

        /// <summary>
        /// Returns true if executing in Design mode (inside IDE)
        /// </summary>
        /// <returns></returns>
        private static bool IsInDesignMode()
        {
            return Application.ExecutablePath.ToUpper(CultureInfo.InvariantCulture).EndsWith("DEVENV.EXE");
        }

        private void InitializeGraphics()
        {

            // Set up our presentation parameters
            m_presentParams = new PresentParameters();

            m_presentParams.Windowed = true;
            m_presentParams.SwapEffect = SwapEffect.Discard;
            m_presentParams.AutoDepthStencilFormat = DepthFormat.D16;
            m_presentParams.EnableAutoDepthStencil = true;

            if (!World.Settings.VSync)
                // Disable wait for vertical retrace (higher frame rate at the expense of tearing)
                m_presentParams.PresentationInterval = PresentInterval.Immediate;

            int adapterOrdinal = 0;
            try
            {
                // Store the default adapter
                adapterOrdinal = Manager.Adapters.Default.Adapter;
            }
            catch
            {
                // User probably needs to upgrade DirectX or install a 3D capable graphics adapter
                throw new NotAvailableException();
            }

            DeviceType dType = DeviceType.Hardware;

            foreach (AdapterInformation ai in Manager.Adapters)
            {
                if (ai.Information.Description.IndexOf("NVPerfHUD") >= 0)
                {
                    adapterOrdinal = ai.Adapter;
                    dType = DeviceType.Reference;
                }
            }
            CreateFlags flags = CreateFlags.SoftwareVertexProcessing;

            // Check to see if we can use a pure hardware m_Device3d
            Caps caps = Manager.GetDeviceCaps(adapterOrdinal, DeviceType.Hardware);

            // Do we support hardware vertex processing?
            if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
                //	// Replace the software vertex processing
                flags = CreateFlags.HardwareVertexProcessing;

            // Use multi-threading for now - TODO: See if the code can be changed such that this isn't necessary (Texture Loading for example)
            flags |= CreateFlags.MultiThreaded | CreateFlags.FpuPreserve;

            try
            {
                // Create our m_Device3d
                m_Device3d = new Device(adapterOrdinal, dType, this, flags, m_presentParams);
            }
            catch (Microsoft.DirectX.DirectXException)
            {
                throw new NotSupportedException("Unable to create the Direct3D m_Device3d.");
            }

            // Hook the m_Device3d reset event
            m_Device3d.DeviceReset += new EventHandler(OnDeviceReset);
            m_Device3d.DeviceResizing += new CancelEventHandler(m_Device3d_DeviceResizing);
            OnDeviceReset(m_Device3d, null);
        }

        private void OnDeviceReset(object sender, EventArgs e)
        {
            // Can we use anisotropic texture minify filter?
            if (m_Device3d.DeviceCaps.TextureFilterCaps.SupportsMinifyAnisotropic)
            {
                m_Device3d.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
            }
            else if (m_Device3d.DeviceCaps.TextureFilterCaps.SupportsMinifyLinear)
            {
                m_Device3d.SamplerState[0].MinFilter = TextureFilter.Linear;
            }

            // What about magnify filter?
            if (m_Device3d.DeviceCaps.TextureFilterCaps.SupportsMagnifyAnisotropic)
            {
                m_Device3d.SamplerState[0].MagFilter = TextureFilter.Anisotropic;
            }
            else if (m_Device3d.DeviceCaps.TextureFilterCaps.SupportsMagnifyLinear)
            {
                m_Device3d.SamplerState[0].MagFilter = TextureFilter.Linear;
            }

            m_Device3d.SamplerState[0].AddressU = TextureAddress.Clamp;
            m_Device3d.SamplerState[0].AddressV = TextureAddress.Clamp;

            m_Device3d.RenderState.Clipping = true;
            m_Device3d.RenderState.CullMode = Cull.Clockwise;
            m_Device3d.RenderState.Lighting = false;
            m_Device3d.RenderState.Ambient = World.Settings.StandardAmbientColor;

            m_Device3d.RenderState.ZBufferEnable = true;
            m_Device3d.RenderState.AlphaBlendEnable = true;
            m_Device3d.RenderState.SourceBlend = Blend.SourceAlpha;
            m_Device3d.RenderState.DestinationBlend = Blend.InvSourceAlpha;
        }

        /// <summary>
        /// Background worker thread loop (updates UI)
        /// </summary>
        private void WorkerThreadFunc()
        {
            const int refreshIntervalMs = 150; // Max 6 updates per seconds
            while (m_WorkerThreadRunning)
            {
                try
                {
                    if (World.Settings.UseBelowNormalPriorityUpdateThread && m_WorkerThread.Priority == System.Threading.ThreadPriority.Normal)
                    {
                        m_WorkerThread.Priority = System.Threading.ThreadPriority.BelowNormal;
                    }
                    else if (!World.Settings.UseBelowNormalPriorityUpdateThread && m_WorkerThread.Priority == System.Threading.ThreadPriority.BelowNormal)
                    {
                        m_WorkerThread.Priority = System.Threading.ThreadPriority.Normal;
                    }
                    m_WorkerThread.Priority = System.Threading.ThreadPriority.Highest;
                    long startTicks = 0;
                    PerformanceTimer.QueryPerformanceCounter(ref startTicks);

                    m_World.Update(this.drawArgs);

                    long endTicks = 0;
                    PerformanceTimer.QueryPerformanceCounter(ref endTicks);
                    float elapsedMilliSeconds = 1000 * (float)(endTicks - startTicks) / PerformanceTimer.TicksPerSecond;
                    float remaining = refreshIntervalMs - elapsedMilliSeconds;
                    if (remaining > 0)
                        Thread.Sleep((int)remaining);
                }
                catch (Exception caught)
                {
                    Log.Write(caught);
                }
            }
        }

        private void TerrainThreadFunc()
        {
            while (m_TerrainThreadRunning)
            {
                foreach (var ro in m_World.DataLayerList.ChildObjects)
                {
                    if (ro is RenderableObjectList)
                    {
                        foreach (var roo in ro.ChildObjects)
                        {
                            if (roo.Name == "Landsat7 Image")
                                roo.Update(drawArgs);
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        #endregion

        #region Event handlers

        public void ResetToolbar()
        {
            lock (this._menuBar.LayersMenuButtons.SyncRoot)
            {
                foreach (IMenu m in this._menuBar.LayersMenuButtons)
                {
                    m.Dispose();
                }
                this._menuBar.LayersMenuButtons.Clear();
            }

            lock (this._menuBar.ToolsMenuButtons.SyncRoot)
            {

                for (int i = 0; i < this._menuBar.ToolsMenuButtons.Count; i++)
                {
                    IMenu m = (IMenu)this._menuBar.ToolsMenuButtons[i];
                    if (m != null)
                    {
                        m.Dispose();
                    }
                }

                this._menuBar.ToolsMenuButtons.Clear();
            }
        }

        public void HandleMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
        }

        /// <summary>
        /// Occurs when the mouse wheel moves while the control has focus.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                if (this._menuBar.OnMouseWheel(e))
                    return;

                this.drawArgs.WorldCamera.ZoomStepped(e.Delta / 120.0f);
            }
            finally
            {
                if (m_NewRootWidget != null)
                {
                    try
                    {
                        m_NewRootWidget.OnMouseWheel(e);
                    }
                    finally
                    {
                    }
                }

                // Call the base class's OnMouseWheel method so that registered delegates receive the event.
                base.OnMouseWheel(e);
            }
        }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
            {
                e.Handled = HandleKeyDown(e);
                base.OnKeyDown(e);
            }
            catch (Exception caught)
            {
                MessageBox.Show(caught.Message, "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            try
            {
                e.Handled = HandleKeyUp(e);
                base.OnKeyUp(e);
            }
            catch (Exception caught)
            {
                MessageBox.Show(caught.Message, "Operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (m_RootWidget != null)
            {
                bool handled = m_RootWidget.OnKeyPress(e);
                e.Handled = handled;
            }
            if (m_NewRootWidget != null)
            {
                bool handled = m_NewRootWidget.OnKeyPress(e);
                e.Handled = handled;
            }
            base.OnKeyPress(e);
        }

        private bool mShiftPressed = false;
        /// <summary>
        /// Preprocess keyboard or input messages within the message loop before they are dispatched.
        /// </summary>
        /// <param name="msg">A Message, passed by reference, that represents the message to process. 
        /// The possible values are WM_KEYDOWN, WM_SYSKEYDOWN, WM_CHAR, and WM_SYSCHAR.</param>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true), SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        public override bool PreProcessMessage(ref Message msg)
        {
            const int WM_KEYDOWN = 0x0100;

            // it's the only way to handle arrow keys in OnKeyDown
            if (msg.Msg == WM_KEYDOWN)
            {
                Keys key = (Keys)msg.WParam.ToInt32();
                switch (key)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Right:
                    case Keys.ShiftKey:
                    case Keys.Down:
                        OnKeyDown(new KeyEventArgs(key));
                        // mark message as processed
                        msg.Result = (IntPtr)1;
                        // When overriding PreProcessMessage, a control should return true to indicate that it has processed the message.
                        return true;
                }
            }

            return base.PreProcessMessage(ref msg);
        }


        /// <summary>
        /// Handles key down events.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Returns true if the key is handled.</returns>
        public bool HandleKeyDown(KeyEventArgs e)
        {

            bool handled = this.m_RootWidget.OnKeyDown(e);
            if (handled)
                return handled;

            handled = this.m_NewRootWidget.OnKeyDown(e);
            if (handled)
                return handled;

            // Alt key down
            if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        World.Settings.ShowCrosshairs = !World.Settings.ShowCrosshairs;
                        return true;
                    case Keys.Add:
                    case Keys.Oemplus:
                    case Keys.Home:
                    case Keys.NumPad7:
                        this.drawArgs.WorldCamera.Fov -= Angle.FromDegrees(5);
                        return true;
                    case Keys.Subtract:
                    case Keys.OemMinus:
                    case Keys.End:
                    case Keys.NumPad1:
                        this.drawArgs.WorldCamera.Fov += Angle.FromDegrees(5);
                        return true;
                }
            }
            // Control key down
            else if (e.Control)
            {
            }
            else if (e.Shift)
            {
                mShiftPressed = true;
            }
            // Other and no control key
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.ShiftKey:
                        mShiftPressed = true;
                        return true;
                    // rotate left
                    case Keys.A:
                        if (!e.Shift)
                        {
                            Angle rotateClockwise = Angle.FromRadians(0.01f);
                            this.drawArgs.WorldCamera.Heading += rotateClockwise;
                            this.drawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, Angle.Zero, rotateClockwise);
                        }
                        return true;
                    // rotate right
                    case Keys.D:
                        Angle rotateCounterclockwise = Angle.FromRadians(-0.01f);
                        this.drawArgs.WorldCamera.Heading += rotateCounterclockwise;
                        this.drawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, Angle.Zero, rotateCounterclockwise);
                        return true;
                    // rotate up
                    case Keys.W:
                        this.drawArgs.WorldCamera.Tilt += Angle.FromDegrees(-1.0f);
                        return true;
                    // rotate down
                    case Keys.S:
                        if (!e.Shift)
                        {
                            this.drawArgs.WorldCamera.Tilt += Angle.FromDegrees(1.0f);
                        }
                        return true;
                    // pan left
                    case Keys.Left:
                    case Keys.H:
                    case Keys.NumPad4:
                        // TODO: pan n pixels
                        Angle panLeft = Angle.FromRadians((float)-1 * (this.drawArgs.WorldCamera.Altitude) * (1 / (300 * this.CurrentWorld.EquatorialRadius)));
                        this.drawArgs.WorldCamera.RotationYawPitchRoll(panLeft, Angle.Zero, Angle.Zero);
                        return true;
                    // pan down
                    case Keys.Down:
                    case Keys.J:
                    case Keys.NumPad2:
                        Angle panDown = Angle.FromRadians((float)-1 * (this.drawArgs.WorldCamera.Altitude) * (1 / (300 * this.CurrentWorld.EquatorialRadius)));
                        this.drawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, panDown, Angle.Zero);
                        return true;
                    // pan right
                    case Keys.Right:
                    case Keys.K:
                    case Keys.NumPad6:
                        Angle panRight = Angle.FromRadians((float)1 * (this.drawArgs.WorldCamera.Altitude) * (1 / (300 * this.CurrentWorld.EquatorialRadius)));
                        this.drawArgs.WorldCamera.RotationYawPitchRoll(panRight, Angle.Zero, Angle.Zero);
                        return true;
                    // pan up
                    case Keys.Up:
                    case Keys.U:
                    case Keys.NumPad8:
                        // TODO: Pan n pixels
                        Angle panUp = Angle.FromRadians((float)1 * (this.drawArgs.WorldCamera.Altitude) * (1 / (300 * this.CurrentWorld.EquatorialRadius)));
                        this.drawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, panUp, Angle.Zero);
                        return true;
                    // zoom in
                    case Keys.Add:
                    case Keys.Oemplus:
                    case Keys.Home:
                    case Keys.NumPad7:
                        this.drawArgs.WorldCamera.ZoomStepped(World.Settings.CameraZoomStepKeyboard);
                        return true;
                    // zoom out
                    case Keys.Subtract:
                    case Keys.OemMinus:
                    case Keys.End:
                    case Keys.NumPad1:
                        this.drawArgs.WorldCamera.ZoomStepped(-World.Settings.CameraZoomStepKeyboard);
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handles key up events.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Returns true if the key is handled.</returns>
        public bool HandleKeyUp(KeyEventArgs e)
        {

            bool handled = m_RootWidget.OnKeyUp(e);
            if (handled)
            {
                e.Handled = handled;
                return handled;
            }
            handled = m_NewRootWidget.OnKeyUp(e);
            if (handled)
            {
                e.Handled = handled;
                return handled;
            }

            // Alt key down
            if (e.Alt)
            {
            }
            // Control key down
            else if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.D:
                        this.showDiagnosticInfo = !this.showDiagnosticInfo;
                        return true;
                    case Keys.W:
                        renderWireFrame = !renderWireFrame;
                        return true;
                }
            }
            else if (e.Shift)
            {
                mShiftPressed = false;
            }
            // Other and no control key
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Space:
                    case Keys.Clear:
                        this.drawArgs.WorldCamera.Reset();
                        return true;
                    case Keys.ShiftKey:
                        mShiftPressed = false;
                        return true;
                }
            }
            return false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();  //fixes mousewheel not working problem

            DrawArgs.LastMousePosition.X = e.X;
            DrawArgs.LastMousePosition.Y = e.Y;

            mouseDownStartPosition.X = e.X;
            mouseDownStartPosition.Y = e.Y;


            try
            {
                bool handled = false;
                handled = m_RootWidget.OnMouseDown(e);

                if (!handled)
                {
                    handled = m_NewRootWidget.OnMouseDown(e);
                }

                if (!handled)
                {
                    if (!this._menuBar.OnMouseDown(e))
                    {

                    }
                }
            }
            finally
            {
                if (e.Button == MouseButtons.Left)
                    DrawArgs.IsLeftMouseButtonDown = true;

                if (e.Button == MouseButtons.Right)
                {
                    DrawArgs.IsRightMouseButtonDown = true;
                    //if (mShiftPressed)
                    //    ContextMenuStrip.Show(this, e.Location);              
                }
                // Call the base class method so that registered delegates receive the event.
                base.OnMouseDown(e);
            }

        }

        bool isDoubleClick = false;
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            isDoubleClick = true;
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            DrawArgs.LastMousePosition.X = e.X;
            DrawArgs.LastMousePosition.Y = e.Y;

            try
            {
                bool handled = false;

                handled = m_RootWidget.OnMouseUp(e);

                if (!handled)
                {
                    handled = m_NewRootWidget.OnMouseUp(e);
                }

                if (!handled)
                {
                    // Mouse must have been clicked outside our window and released on us, ignore
                    if (mouseDownStartPosition == Point.Empty)
                        return;

                    mouseDownStartPosition = Point.Empty;

                    if (!this.isMouseDragging)
                    {
                        if (this._menuBar.OnMouseUp(e))
                            return;
                    }

                    if (m_World == null)
                        return;

                    if (isDoubleClick)
                    {
                        isDoubleClick = false;
                        if (e.Button == MouseButtons.Left)
                        {
                            drawArgs.WorldCamera.Zoom(World.Settings.CameraDoubleClickZoomFactor);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            drawArgs.WorldCamera.Zoom(-World.Settings.CameraDoubleClickZoomFactor);
                        }
                    }
                    else
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            if (this.isMouseDragging)
                            {
                                this.isMouseDragging = false;
                            }
                            else
                            {
                                if (!m_World.PerformSelectionAction(this.drawArgs))
                                {

                                    Angle targetLatitude;
                                    Angle targetLongitude;
                                    //Quaternion targetOrientation = new Quaternion();
                                    this.drawArgs.WorldCamera.PickingRayIntersection(
                                        DrawArgs.LastMousePosition.X,
                                        DrawArgs.LastMousePosition.Y,
                                        out targetLatitude,
                                        out targetLongitude);
                                    if (!Angle.IsNaN(targetLatitude))
                                        this.drawArgs.WorldCamera.PointGoto(targetLatitude, targetLongitude);
                                }
                            }
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            if (this.isMouseDragging)
                                this.isMouseDragging = false;
                            else
                            {
                                if (!m_World.PerformSelectionAction(this.drawArgs))
                                {
                                    //nothing at the moment
                                }
                            }

                        }
                    }
                }
            }
            finally
            {
                if (e.Button == MouseButtons.Left)
                    DrawArgs.IsLeftMouseButtonDown = false;

                if (e.Button == MouseButtons.Right)
                    DrawArgs.IsRightMouseButtonDown = false;
                // Call the base class method so that registered delegates receive the event.
                base.OnMouseUp(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Default to default cursor
            mCurrentMousePosition = e.Location;
            DrawArgs.MouseCursor = CursorType.Arrow;
            try
            {
                bool handled = false;
                if (!isMouseDragging)
                {
                    handled = m_RootWidget.OnMouseMove(e);

                    if (!handled)
                    {
                        handled = m_NewRootWidget.OnMouseMove(e);
                    }
                }

                if (!handled)
                {
                    int deltaX = e.X - DrawArgs.LastMousePosition.X;
                    int deltaY = e.Y - DrawArgs.LastMousePosition.Y;
                    float deltaXNormalized = (float)deltaX / drawArgs.screenWidth;
                    float deltaYNormalized = (float)deltaY / drawArgs.screenHeight;

                    if (!this.isMouseDragging)
                    {
                        if (this._menuBar.OnMouseMove(e))
                        {
                            base.OnMouseMove(e);
                            return;
                        }
                    }

                    if (mouseDownStartPosition == Point.Empty)
                        return;

                    bool isMouseLeftButtonDown = ((int)e.Button & (int)MouseButtons.Left) != 0;
                    bool isMouseRightButtonDown = ((int)e.Button & (int)MouseButtons.Right) != 0;
                    if (isMouseLeftButtonDown || isMouseRightButtonDown)
                    {
                        int dx = this.mouseDownStartPosition.X - e.X;
                        int dy = this.mouseDownStartPosition.Y - e.Y;
                        int distanceSquared = dx * dx + dy * dy;
                        if (distanceSquared > 3 * 3)
                            // Distance > 3 = drag
                            this.isMouseDragging = true;
                    }

                    if (isMouseLeftButtonDown && !isMouseRightButtonDown)
                    {
                        // Left button (pan)
                        // Store start lat/lon for drag
                        Angle prevLat, prevLon;
                        this.drawArgs.WorldCamera.PickingRayIntersection(
                            DrawArgs.LastMousePosition.X,
                            DrawArgs.LastMousePosition.Y,
                            out prevLat,
                            out prevLon);

                        Angle curLat, curLon;
                        this.drawArgs.WorldCamera.PickingRayIntersection(
                            e.X,
                            e.Y,
                            out curLat,
                            out curLon);

                        if (World.Settings.CameraTwistLock)
                        {
                            if (Angle.IsNaN(curLat) || Angle.IsNaN(prevLat))
                            {
                                // Old style pan
                                Angle deltaLat = Angle.FromRadians((double)deltaY * (this.drawArgs.WorldCamera.Altitude) / (800 * this.CurrentWorld.EquatorialRadius));
                                Angle deltaLon = Angle.FromRadians((double)-deltaX * (this.drawArgs.WorldCamera.Altitude) / (800 * this.CurrentWorld.EquatorialRadius));
                                this.drawArgs.WorldCamera.Pan(deltaLat, deltaLon);
                            }
                            else
                            {
                                //Picking ray pan
                                Angle lat = prevLat - curLat;
                                Angle lon = prevLon - curLon;
                                this.drawArgs.WorldCamera.Pan(lat, lon);
                            }
                        }
                        else
                        {
                            double factor = (this.drawArgs.WorldCamera.Altitude) / (1500 * this.CurrentWorld.EquatorialRadius);
                            drawArgs.WorldCamera.RotationYawPitchRoll(
                                Angle.FromRadians(DrawArgs.LastMousePosition.X - e.X) * factor,
                                Angle.FromRadians(e.Y - DrawArgs.LastMousePosition.Y) * factor,
                                Angle.Zero);
                        }
                    }
                    else if (!isMouseLeftButtonDown && isMouseRightButtonDown)
                    {
                        //Right mouse button

                        // Heading
                        Angle deltaEyeDirection = Angle.FromRadians(-deltaXNormalized * World.Settings.CameraRotationSpeed);
                        this.drawArgs.WorldCamera.RotationYawPitchRoll(Angle.Zero, Angle.Zero, deltaEyeDirection);

                        // tilt
                        this.drawArgs.WorldCamera.Tilt += Angle.FromRadians(deltaYNormalized * World.Settings.CameraRotationSpeed);
                    }
                    else if (isMouseLeftButtonDown && isMouseRightButtonDown)
                    {
                        // Both buttons (zoom)
                        if (Math.Abs(deltaYNormalized) > float.Epsilon)
                            this.drawArgs.WorldCamera.Zoom(-deltaYNormalized * World.Settings.CameraZoomAnalogFactor);

                        if (!World.Settings.CameraBankLock)
                            this.drawArgs.WorldCamera.Bank -= Angle.FromRadians(deltaXNormalized * World.Settings.CameraRotationSpeed);
                    }
                }
            }
            catch
            {
            }
            finally
            {

                this.drawArgs.WorldCamera.PickingRayIntersection(
                    e.X,
                    e.Y,
                    out cLat,
                    out cLon);

                DrawArgs.LastMousePosition.X = e.X;
                DrawArgs.LastMousePosition.Y = e.Y;
                base.OnMouseMove(e);
            }
        }

        Angle cLat, cLon;

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_menuBar != null)
                // reset menu bar mouse hover state.
                _menuBar.OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, -1, -1, 0));
            base.OnMouseLeave(e);
        }

        #endregion

        #region IGlobe Members

        public void SetDisplayMessages(IList messages)
        {
            m_World.OnScreenMessages = messages;
        }

        public void SetLatLonGridShow(bool show)
        {
            World.Settings.ShowLatLonLines = show;
        }

        public void SetLayers(IList layers)
        {
            if (layers != null)
            {
                foreach (LayerDescriptor ld in layers)
                {
                    this.CurrentWorld.SetLayerOpacity(ld.Category, ld.Name, (float)ld.Opacity * 0.01f);
                }
            }
        }

        public void SetVerticalExaggeration(double exageration)
        {
            World.Settings.VerticalExaggeration = (float)exageration;
        }

        public void SetViewDirection(String type, double horiz, double vert, double elev)
        {
            this.drawArgs.WorldCamera.SetPosition(this.drawArgs.WorldCamera.Latitude.Degrees, this.drawArgs.WorldCamera.Longitude.Degrees, horiz,
                this.drawArgs.WorldCamera.Altitude, vert);
        }

        public void SetViewPosition(double degreesLatitude, double degreesLongitude,
            double metersElevation)
        {
            this.drawArgs.WorldCamera.SetPosition(degreesLatitude, degreesLongitude, this.drawArgs.WorldCamera.Heading.Degrees,
                metersElevation, this.drawArgs.WorldCamera.Tilt.Degrees);
        }

        #endregion

        private void m_FpsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            World.Settings.ShowFpsGraph = true;
            if (m_FpsUpdate)
                return;

            m_FpsUpdate = true;
          
            try
            {
                //World.Settings.ShowFpsGraph = false;
                if (World.Settings.ShowFpsGraph)
                {
                    if (!m_FpsGraph.Visible)
                    {
                        m_FpsGraph.Visible = true;
                    }

                    if (m_FrameTimes.Count > World.Settings.FpsFrameCount)
                    {
                        m_FrameTimes.RemoveRange(0, m_FrameTimes.Count - World.Settings.FpsFrameCount);
                    }

                    m_FpsGraph.Size = new Size((int)(Width * .5), (int)(Height * .1));
                    m_FpsGraph.Location = new Point((int)(Width * .35), (int)(Height * .895));
                    m_FpsGraph.Values = m_FrameTimes.ToArray();
                }
                else
                {
                    if (m_FpsGraph.Visible)
                    {
                        m_FpsGraph.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

            m_FpsUpdate = false;
        }

       
    }
}