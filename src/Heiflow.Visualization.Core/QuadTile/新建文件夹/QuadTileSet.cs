using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Camera;
using HUST.WREIS.Dot3D.Configuration;
using HUST.WREIS.Dot3D.Net;
using HUST.WREIS.Dot3D.Terrain;
using HUST.WREIS.Dot3D.VisualControl;
using System;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using Utility;
using Heiflow.Models.Component; 


namespace HUST.WREIS.Dot3D.Renderable
{
    /// <summary>
    /// Main class for image tile rendering.  Uses the Terrain Manager to query height values for 3D
    /// terrain rendering.
    /// Relies on an Update thread to refresh the "tiles" based on lat/lon/view range
    /// </summary>
    public class QuadTileSet : RenderableObject
    {
        #region Private Members

        bool m_RenderStruts = true;
        protected string m_ServerLogoFilePath;
        protected Image m_ServerLogoImage;

        protected Hashtable m_topmostTiles = new Hashtable();
        protected double m_north;
        protected double m_south;
        protected double m_west;
        protected double m_east;
        bool renderFileNames = false;

        protected Texture m_iconTexture;
        protected Sprite sprite;
        protected Rectangle m_spriteSize;
        protected ProgressBar progressBar;

        protected Blend m_sourceBlend = Blend.BlendFactor;
        protected Blend m_destinationBlend = Blend.InvBlendFactor;

        // If this value equals CurrentFrameStartTicks the Z buffer needs to be cleared
        protected static long lastRenderTime;

        //public static int MaxConcurrentDownloads = 3;
        protected double m_layerRadius;
        protected bool m_alwaysRenderBaseTiles;
        protected float m_tileDrawSpread;
        protected float m_tileDrawDistance;
        protected bool m_isDownloadingElevation;
        protected int m_numberRetries;
        protected Hashtable m_downloadRequests = new Hashtable();
        protected int m_maxQueueSize = 400;
        protected bool m_terrainMapped;
        protected ImageStore[] m_imageStores;
        protected Camera.CameraBase m_camera;
        protected GeoImageDownloadRequest[] m_activeDownloads = new GeoImageDownloadRequest[20];
        protected DateTime[] m_downloadStarted = new DateTime[20];
        protected TimeSpan m_connectionWaitTime = TimeSpan.FromMinutes(2);
        protected DateTime m_connectionWaitStart;
        protected bool m_isConnectionWaiting;
        protected bool m_enableColorKeying;

        protected Effect m_effect = null;
        protected string m_effectPath = null;
        protected string m_effectTechnique = null;
        static protected EffectPool m_effectPool = new EffectPool();


		protected TimeSpan m_cacheExpirationTime = TimeSpan.MaxValue;

        #endregion

        #region public members

        /// <summary>
        /// Texture showing download in progress
        /// </summary>
        public static Texture DownloadInProgressTexture;

        /// <summary>
        /// Texture showing queued download
        /// </summary>
        public static Texture DownloadQueuedTexture;

        /// <summary>
        /// Texture showing terrain download in progress
        /// </summary>
        public static Texture DownloadTerrainTexture;



        public int ColorKey; // default: 100% transparent black = transparent

        /// <summary>
        /// If a color range is to be transparent this specifies the brightest transparent color.
        /// The darkest transparent color is set using ColorKey.
        /// </summary>
        public int ColorKeyMax;

        #endregion

        private bool m_renderGrayscale;
        private float m_grayscaleBrightness;
      
        /// <summary>
        /// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.QuadTileSet"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentWorld"></param>
        /// <param name="distanceAboveSurface"></param>
        /// <param name="north"></param>
        /// <param name="south"></param>
        /// <param name="west"></param>
        /// <param name="east"></param>
        /// <param name="terrainAccessor"></param>
        /// <param name="imageAccessor"></param>
        public QuadTileSet(
                string name,
                World parentWorld,
                double distanceAboveSurface,
                double north,
                double south,
                double west,
                double east,
                bool terrainMapped,
                                        ImageStore[] imageStores)
            : base(name, parentWorld)
        {
            float layerRadius = (float)(parentWorld.EquatorialRadius + distanceAboveSurface);
            m_north = north;
            m_south = south;
            m_west = west;
            m_east = east;

            // Layer center position
            Position = MathEngine.SphericalToCartesian(
                    (north + south) * 0.5f,
                    (west + east) * 0.5f,
                    layerRadius);

            m_layerRadius = layerRadius;
            m_tileDrawDistance = 3.5f;
            //m_tileDrawDistance = 6f;
            m_tileDrawSpread = 2.9f;
            m_imageStores = imageStores;
            m_terrainMapped = terrainMapped;

            // Default terrain mapped imagery to terrain mapped priority
            if (terrainMapped)
                m_renderPriority = RenderPriority.TerrainMappedImages;
        }

        #region Public properties
        [Browsable(true), Category("Render")]
        [Description("")]
        public float GrayscaleBrightness
        {
            get { return m_grayscaleBrightness; }
            set { m_grayscaleBrightness = value; }
        }
        [Browsable(true), Category("Render")]
        [Description("")]
        public bool RenderGrayscale
        {
            get { return m_renderGrayscale; }
            set { m_renderGrayscale = value; }
        }
        [Browsable(true), Category("Render")]
        [Description("")]
        public bool RenderStruts
        {
            get { return m_RenderStruts; }
            set { m_RenderStruts = value; }
        }

       
		/// <summary>
		/// If images in cache are older than expration time a refresh
		/// from server will be attempted.
		/// </summary>
        [Browsable(false)]
		public TimeSpan CacheExpirationTime
		{
			get
			{
				return this.m_cacheExpirationTime;
			}
			set
			{
				this.m_cacheExpirationTime = value;
			}
		}

        /// <summary>
        /// Path to a thumbnail image (e.g. for use as a download indicator).
        /// </summary>
        /// 
        [Browsable(true), Category("General")]
        [Description("")]
        public virtual string ServerLogoFilePath
        {
            get
            {
                return m_ServerLogoFilePath;
            }
            set
            {
                m_ServerLogoFilePath = value;
            }
        }
        [Browsable(true), Category("Render")]
        [Description("")]
        public bool RenderFileNames
        {
            get
            {
                return renderFileNames;
            }
            set
            {
                renderFileNames = value;
            }
        }
        [Browsable(false), Category("Render")]
        [Description("")]
        /// <summary>
        /// The image referenced by ServerLogoFilePath.
        /// </summary>
        public virtual Image ServerLogoImage
        {
            get
            {
                if (m_ServerLogoImage == null)
                {
                    if (m_ServerLogoFilePath == null)
                        return null;
                    try
                    {
                        if (File.Exists(m_ServerLogoFilePath))
                            m_ServerLogoImage = ImageHelper.LoadImage(m_ServerLogoFilePath);
                    }
                    catch { }
                }
                return m_ServerLogoImage;
            }
        }
          [Browsable(false)]
        /// <summary>
        /// Path to a thumbnail image (or download indicator if none available)
        /// </summary>
        public override Image ThumbnailImage
        {
            get
            {
                if (base.ThumbnailImage != null)
                    return base.ThumbnailImage;

                return ServerLogoImage;
            }
        }
          [Browsable(true), Category("Effect")]
          [Description("")]
        /// <summary>
        /// Path to a thumbnail image (e.g. for use as a download indicator).
        /// </summary>
        public virtual bool HasTransparentRange
        {
            get
            {
                return (ColorKeyMax != 0);
            }
        }
          [Browsable(true), Category("Effect")]
          [Description("")]
        /// <summary>
        /// Source blend when rendering non-opaque layer
        /// </summary>
        public Blend SourceBlend
        {
            get
            {
                return m_sourceBlend;
            }
            set
            {
                m_sourceBlend = value;
            }
        }
          [Browsable(true), Category("Effect")]
          [Description("")]
        /// <summary>
        /// Destination blend when rendering non-opaque layer
        /// </summary>
        public Blend DestinationBlend
        {
            get
            {
                return m_destinationBlend;
            }
            set
            {
                m_destinationBlend = value;
            }
        }
          [Browsable(true), Category("GeoSpatial")]
          [Description("")]
        /// <summary>
        /// North bound for this QuadTileSet
        /// </summary>
        public double North
        {
            get
            {
                return m_north;
            }
        }
          [Browsable(true), Category("GeoSpatial")]
          [Description("")]
        /// <summary>
        /// West bound for this QuadTileSet
        /// </summary>
        public double West
        {
            get
            {
                return m_west;
            }
        }
          [Browsable(true), Category("GeoSpatial")]
          [Description("")]
        /// <summary>
        /// South bound for this QuadTileSet
        /// </summary>
        public double South
        {
            get
            {
                return m_south;
            }
        }
          [Browsable(true), Category("GeoSpatial")]
          [Description("")]
        /// <summary>
        /// East bound for this QuadTileSet
        /// </summary>
        public double East
        {
            get
            {
                return m_east;
            }
        }
          [Browsable(true), Category("General")]
          [Description("")]
        /// <summary>
        /// Controls if images are rendered using ColorKey (transparent areas)
        /// </summary>
        public bool EnableColorKeying
        {
            get
            {
                return m_enableColorKeying;
            }
            set
            {
                m_enableColorKeying = value;
            }
        }
          [Browsable(true), Category("Web")]
          [Description("")]
        public DateTime ConnectionWaitStart
        {
            get
            {
                return m_connectionWaitStart;
            }
        }
          [Browsable(true), Category("Web")]
          [Description("")]
        public bool IsConnectionWaiting
        {
            get
            {
                return m_isConnectionWaiting;
            }
        }
          [Browsable(true), Category("GeoSpatial")]
          [Description("")]
        public double LayerRadius
        {
            get
            {
                return m_layerRadius;
            }
            set
            {
                m_layerRadius = value;
            }
        }
          [Browsable(true), Category("Render")]
          [Description("")]
        public bool AlwaysRenderBaseTiles
        {
            get
            {
                return m_alwaysRenderBaseTiles;
            }
            set
            {
                m_alwaysRenderBaseTiles = value;
            }
        }
          [Browsable(true), Category("GeoSpatial")]
          [Description("")]
        public float TileDrawSpread
        {
            get
            {
                return m_tileDrawSpread;
            }
            set
            {
                m_tileDrawSpread = value;
            }
        }
          [Browsable(true), Category("GeoSpatial")]
          [Description("")]
        public float TileDrawDistance
        {
            get
            {
                return m_tileDrawDistance;
            }
            set
            {
                m_tileDrawDistance = value;
            }
        }
          [Browsable(true), Category("Web")]
          [Description("")]
        public bool IsDownloadingElevation
        {
            get
            {
                return m_isDownloadingElevation;
            }
            set
            {
                m_isDownloadingElevation = value;
            }
        }
          [Browsable(true), Category("Web")]
          [Description("")]
        public int NumberRetries
        {
            get
            {
                return m_numberRetries;
            }
            set
            {
                m_numberRetries = value;
            }
        }
          [Browsable(true), Category("GeoSpatial")]
          [Description("")]
        /// <summary>
        /// Controls rendering (flat or terrain mapped)
        /// </summary>
        public bool TerrainMapped
        {
            get { return m_terrainMapped; }
            set { m_terrainMapped = value; }
        }
          [Browsable(false)]
        public ImageStore[] ImageStores
        {
            get
            {
                return m_imageStores;
            }
        }
          [Browsable(false)]
        /// <summary>
        /// Tiles in the request for download queue
        /// </summary>
        public Hashtable DownloadRequests
        {
            get
            {
                return m_downloadRequests;
            }
        }
          [Browsable(false)]
        /// <summary>
        /// The camera controlling the layers update logic
        /// </summary>
        public CameraBase Camera
        {
            get
            {
                return m_camera;
            }
            set
            {
                m_camera = value;
            }
        }
          [Browsable(true), Category("Effect")]
          [Description("")]
        /// <summary>
        /// Path to the effect used to render this tileset; if null, use fixed function pipeline
        /// </summary>
        public string EffectPath
        {
            get
            {
                return m_effectPath;
            }
            set
            {
                m_effectPath = value;
                // can't reload here because we need a valid DX device for that and EffectPath
                // may be set before DX is initialized, so set null and reload in Update()
                m_effect = null;
            }
        }
        [Browsable(false)]
        /// <summary>
        /// The effect used to render this tileset.
        /// </summary>
        public Effect Effect
        {
            get
            {
                return m_effect;
            }
        }

        #endregion

        override public void Initialize(DrawArgs drawArgs)
        {
            Camera = DrawArgs.Camera;

            // Initialize download rectangles
            if (DownloadInProgressTexture == null)
                DownloadInProgressTexture = CreateDownloadRectangle(
                        DrawArgs.Device, World.Settings.DownloadProgressColor, 0);
            if (DownloadQueuedTexture == null)
                DownloadQueuedTexture = CreateDownloadRectangle(
                        DrawArgs.Device, World.Settings.DownloadQueuedColor, 0);
            if (DownloadTerrainTexture == null)
                DownloadTerrainTexture = CreateDownloadRectangle(
                        DrawArgs.Device, World.Settings.DownloadTerrainRectangleColor, 0);

            try
            {
                lock (m_topmostTiles.SyncRoot)
                {
                    foreach (QuadTile qt in m_topmostTiles.Values)
                        qt.Initialize();
                }
            }
            catch(Exception ex)
            {
                Log.Write(ex);
            }
            isInitialized = true;


            if (MetaData.ContainsKey("EffectPath"))
            {
                m_effectPath = MetaData["EffectPath"] as string;
            }
            else
            {
                m_effectPath = null;
            }
            m_effect = null;
            if (m_imageStores != null)
            {
                foreach(ImageStore ims in m_imageStores)
                {
                    ims.DrawArgs = drawArgs;
                }
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!isInitialized)
                Initialize(drawArgs);

            if (m_effectPath != null && m_effect == null)
            {
                string errs = string.Empty;
                m_effect = Effect.FromFile(DrawArgs.Device, m_effectPath, null, "", ShaderFlags.None, m_effectPool, out errs);

                if (errs != null && errs != string.Empty)
                {
                    Log.Write(Log.Levels.Warning, "Could not load effect " + m_effectPath + ": " + errs);
                    Log.Write(Log.Levels.Warning, "Effect has been disabled.");
                    m_effectPath = null;
                    m_effect = null;
                }
            }

            if (ImageStores[0].LevelZeroTileSizeDegrees < 180)
            {
                // Check for layer outside view
                double vrd = DrawArgs.Camera.ViewRange.Degrees;
                double latitudeMax = DrawArgs.Camera.Latitude.Degrees + vrd;
                double latitudeMin = DrawArgs.Camera.Latitude.Degrees - vrd;
                double longitudeMax = DrawArgs.Camera.Longitude.Degrees + vrd;
                double longitudeMin = DrawArgs.Camera.Longitude.Degrees - vrd;
                if (latitudeMax < m_south || latitudeMin > m_north || longitudeMax < m_west || longitudeMin > m_east)
                    return;
            }

            if (DrawArgs.Camera.ViewRange * 0.5f >
                    Angle.FromDegrees(TileDrawDistance * ImageStores[0].LevelZeroTileSizeDegrees))
            {
                lock (m_topmostTiles.SyncRoot)
                {
                    foreach (QuadTile qt in m_topmostTiles.Values)
                        qt.Dispose();
                    m_topmostTiles.Clear();
                    ClearDownloadRequests();
                }

                return;
            }

            RemoveInvisibleTiles(DrawArgs.Camera);
            try
            {
               
                int middleRow = MathEngine.GetRowFromLatitude(DrawArgs.Camera.Latitude, ImageStores[0].LevelZeroTileSizeDegrees);
                int middleCol = MathEngine.GetColFromLongitude(DrawArgs.Camera.Longitude, ImageStores[0].LevelZeroTileSizeDegrees);

                double middleSouth = -90.0f + middleRow * ImageStores[0].LevelZeroTileSizeDegrees;
                double middleNorth = -90.0f + middleRow * ImageStores[0].LevelZeroTileSizeDegrees + ImageStores[0].LevelZeroTileSizeDegrees;
                double middleWest = -180.0f + middleCol * ImageStores[0].LevelZeroTileSizeDegrees;
                double middleEast = -180.0f + middleCol * ImageStores[0].LevelZeroTileSizeDegrees + ImageStores[0].LevelZeroTileSizeDegrees;

                double middleCenterLat = 0.5f * (middleNorth + middleSouth);
                double middleCenterLon = 0.5f * (middleWest + middleEast);

                int tileSpread = 4;
                for (int i = 0; i < tileSpread; i++)
                {
                    for (double j = middleCenterLat - i * ImageStores[0].LevelZeroTileSizeDegrees; j < middleCenterLat + i * ImageStores[0].LevelZeroTileSizeDegrees; j += ImageStores[0].LevelZeroTileSizeDegrees)
                    {
                        for (double k = middleCenterLon - i * ImageStores[0].LevelZeroTileSizeDegrees; k < middleCenterLon + i * ImageStores[0].LevelZeroTileSizeDegrees; k += ImageStores[0].LevelZeroTileSizeDegrees)
                        {
                            int curRow = MathEngine.GetRowFromLatitude(Angle.FromDegrees(j), ImageStores[0].LevelZeroTileSizeDegrees);
                            int curCol = MathEngine.GetColFromLongitude(Angle.FromDegrees(k), ImageStores[0].LevelZeroTileSizeDegrees);
                            long key = ((long)curRow << 32) + curCol;

                            QuadTile qt = (QuadTile)m_topmostTiles[key];
                            if (qt != null)
                            {
                                qt.Update(drawArgs);
                                continue;
                            }

                            // Check for tile outside layer boundaries
                            double west = -180.0f + curCol * ImageStores[0].LevelZeroTileSizeDegrees;
                            if (west > m_east)
                                continue;

                            double east = west + ImageStores[0].LevelZeroTileSizeDegrees;
                            if (east < m_west)
                                continue;

                            double south = -90.0f + curRow * ImageStores[0].LevelZeroTileSizeDegrees;
                            if (south > m_north)
                                continue;

                            double north = south + ImageStores[0].LevelZeroTileSizeDegrees;
                            if (north < m_south)
                                continue;

                            qt = new QuadTile(south, north, west, east, 0, this);
                            if (DrawArgs.Camera.ViewFrustum.Intersects(qt.BoundingBox))
                            {
                                lock (m_topmostTiles.SyncRoot)
                                    m_topmostTiles.Add(key, qt);
                                qt.Update(drawArgs);
                            }
                        }
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }

        protected void RemoveInvisibleTiles(CameraBase camera)
        {
            List<long> deletionList = new List<long>();

            lock (m_topmostTiles.SyncRoot)
            {
                foreach (long key in m_topmostTiles.Keys)
                {
                    QuadTile qt = (QuadTile)m_topmostTiles[key];
                    if (!camera.ViewFrustum.Intersects(qt.BoundingBox))
                        deletionList.Add(key);
                }

                foreach (long deleteThis in deletionList)
                {
                    QuadTile qt = (QuadTile)m_topmostTiles[deleteThis];
                    if (qt != null)
                    {
                        m_topmostTiles.Remove(deleteThis);
                        qt.Dispose();
                    }
                }
            }
        }

        public override void Render(DrawArgs drawArgs)
        {
            try
            {
                lock (m_topmostTiles.SyncRoot)
                {
                    if (m_topmostTiles.Count <= 0)
                    {
                        return;
                    }

                    Device device = DrawArgs.Device;

                    // Temporary fix: Clear Z buffer between rendering
                    // terrain mapped layers to avoid Z buffer fighting
                    //if (lastRenderTime == DrawArgs.CurrentFrameStartTicks)
                    device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
                    device.RenderState.ZBufferEnable = true;
                    lastRenderTime = DrawArgs.CurrentFrameStartTicks;

                    if (!World.Settings.EnableSunShading)
                    {
                        // Set the render states for rendering of quad tiles.
                        // Any quad tile rendering code that adjusts the state should restore it to below values afterwards.
                        device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                        device.SetTextureStageState(0, TextureStageStates.ColorOperation, (int)TextureOperation.SelectArg1);
                        device.SetTextureStageState(0, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
                        device.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
                        device.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.SelectArg1);

                        // Be prepared for multi-texturing
                        device.SetTextureStageState(1, TextureStageStates.ColorArgument2, (int)TextureArgument.Current);
                        device.SetTextureStageState(1, TextureStageStates.ColorArgument1, (int)TextureArgument.TextureColor);
                        device.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, 0);
                    }
                    device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                    FillMode fillmode = device.RenderState.FillMode;
                   device.RenderState.FillMode = FillMode;

                    foreach (QuadTile qt in m_topmostTiles.Values)
                        qt.Render(drawArgs);

                    // Restore device states
                    device.SetTextureStageState(1, TextureStageStates.TextureCoordinateIndex, 1);

                    if (m_renderPriority < RenderPriority.TerrainMappedImages)
                        device.RenderState.ZBufferEnable = true;

                    //if (m_opacity < 255 || EnableColorKeying)
                    //{
                    //    // Restore alpha blend state
                    //    device.RenderState.SourceBlend = Blend.SourceAlpha;
                    //    device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                    //}

                    device.RenderState.FillMode = fillmode;
                }
            }
            catch(Exception ex)
            {
                Log.Write(ex);
            }
            finally
            {
                if (IsConnectionWaiting)
                {
                    if (DateTime.Now.Subtract(TimeSpan.FromSeconds(15)) < ConnectionWaitStart)
                    {
                        string s = "Problem connecting to server... Trying again in 2 minutes.\n";
                        drawArgs.UpperLeftCornerText += s;
                    }
                }

                int i = 0;
                foreach (GeoImageDownloadRequest request in m_activeDownloads)
                {
                    if (request != null)
                    {
                        if (!request.IsComplete && i < 10)
                            RenderDownloadProgress(drawArgs, request, i++);
                        // Only render the first
                        //break;
                    }
                }
            }
        }

        public void RenderDownloadProgress(DrawArgs drawArgs, GeoImageDownloadRequest request, int offset)
        {
            int halfIconHeight = 24;
            int halfIconWidth = 24;

            //Vector3 projectedPoint = new Vector3(DrawArgs.ParentControl.Width - halfIconWidth - 10, DrawArgs.ParentControl.Height - 34 - 4 * offset, 0.5f);
            Vector3 projectedPoint = new Vector3(50, DrawArgs.ParentControl.Height - 50, 0.5f);

            // Render progress bar
            if (progressBar == null)
                progressBar = new ProgressBar(150, 10);
            progressBar.Draw(drawArgs, projectedPoint.X, projectedPoint.Y + 24, request.ProgressPercent, World.Settings.DownloadProgressColor.ToArgb());
            DrawArgs.Device.RenderState.ZBufferEnable = true;

            // Render server logo
            if (ServerLogoFilePath == null)
                return;

            if (m_iconTexture == null)
                m_iconTexture = ImageHelper.LoadIconTexture(ServerLogoFilePath);

            if (sprite == null)
            {
                using (Surface s = m_iconTexture.GetSurfaceLevel(0))
                {
                    SurfaceDescription desc = s.Description;
                    m_spriteSize = new Rectangle(0, 0, desc.Width, desc.Height);
                }

                this.sprite = new Sprite(DrawArgs.Device);
            }

            float scaleWidth = (float)2.0f * halfIconWidth / m_spriteSize.Width;
            float scaleHeight = (float)2.0f * halfIconHeight / m_spriteSize.Height;

            this.sprite.Begin(SpriteFlags.AlphaBlend);
            this.sprite.Transform = Matrix.Transformation2D(new Vector2(0.0f, 0.0f), 0.0f, new Vector2(scaleWidth, scaleHeight),
                    new Vector2(0, 0), 0.0f, new Vector2(projectedPoint.X, projectedPoint.Y));

            this.sprite.Draw(m_iconTexture, m_spriteSize,
                    new Vector3(1.32f * 48, 1.32f * 48, 0), new Vector3(0, 0, 0),
                    World.Settings.DownloadLogoColor);
            this.sprite.End();
        }

        public override void Dispose()
        {
            isInitialized = false;

            // flush downloads
            for (int i = 0; i < World.Settings.MaxSimultaneousDownloads; i++)
            {
                if (m_activeDownloads[i] != null)
                {
                    m_activeDownloads[i].Dispose();
                    m_activeDownloads[i] = null;
                }
            }

            foreach (QuadTile qt in m_topmostTiles.Values)
                qt.Dispose();

            if (m_iconTexture != null)
            {
                m_iconTexture.Dispose();
                m_iconTexture = null;
            }

            if (this.sprite != null)
            {
                this.sprite.Dispose();
                this.sprite = null;
            }
        }

        public virtual void ResetCacheForCurrentView(HUST.WREIS.Dot3D.Camera.CameraBase camera)
        {
            //                      if (!ImageStore.IsDownloadableLayer)
            //                              return;

            ArrayList deletionList = new ArrayList();
            //reset "root" tiles that intersect current view
            lock (m_topmostTiles.SyncRoot)
            {
                foreach (long key in m_topmostTiles.Keys)
                {
                    QuadTile qt = (QuadTile)m_topmostTiles[key];
                    if (camera.ViewFrustum.Intersects(qt.BoundingBox))
                    {
                        qt.ResetCache();
                        deletionList.Add(key);
                    }
                }

                foreach (long deletionKey in deletionList)
                    m_topmostTiles.Remove(deletionKey);
            }
        }

        public void ClearDownloadRequests()
        {
            lock (m_downloadRequests.SyncRoot)
            {
                m_downloadRequests.Clear();
            }
        }

        public virtual void AddToDownloadQueue(CameraBase camera, GeoImageDownloadRequest newRequest)
        {
            QuadTile key = newRequest.QuadTile;
            key.WaitingForDownload = true;
            lock (m_downloadRequests.SyncRoot)
            {
                if (m_downloadRequests.Contains(key))
                    return;

                m_downloadRequests.Add(key, newRequest);

                if (m_downloadRequests.Count >= m_maxQueueSize)
                {
                    //remove spatially farthest request
                    GeoImageDownloadRequest farthestRequest = null;
                    Angle curDistance = Angle.Zero;
                    Angle farthestDistance = Angle.Zero;
                    foreach (GeoImageDownloadRequest curRequest in m_downloadRequests.Values)
                    {
                        curDistance = MathEngine.SphericalDistance(
                                        curRequest.QuadTile.CenterLatitude,
                                        curRequest.QuadTile.CenterLongitude,
                                        camera.Latitude,
                                        camera.Longitude);

                        if (curDistance > farthestDistance)
                        {
                            farthestRequest = curRequest;
                            farthestDistance = curDistance;
                        }
                    }

                    farthestRequest.Dispose();
                    farthestRequest.QuadTile.DownloadRequest = null;
                    m_downloadRequests.Remove(farthestRequest.QuadTile);
                }
            }

            ServiceDownloadQueue();
        }

        /// <summary>
        /// Removes a request from the download queue.
        /// </summary>
        public virtual void RemoveFromDownloadQueue(GeoImageDownloadRequest removeRequest)
        {
            lock (m_downloadRequests.SyncRoot)
            {
                QuadTile key = removeRequest.QuadTile;
                GeoImageDownloadRequest request = (GeoImageDownloadRequest)m_downloadRequests[key];
                if (request != null)
                {
                    m_downloadRequests.Remove(key);
                    request.QuadTile.DownloadRequest = null;
                }
            }
        }

        /// <summary>
        /// Starts downloads when there are threads available
        /// </summary>
        public virtual void ServiceDownloadQueue()
        {
            Log.Write(Log.Levels.Verbose, "QTS", "ServiceDownloadQueue: " + m_downloadRequests.Count + " requests waiting");
            lock (m_downloadRequests.SyncRoot)
            {
                for (int i = 0; i < World.Settings.MaxSimultaneousDownloads; i++)
                {
                    if (m_activeDownloads[i] == null)
                        continue;

                    if (!m_activeDownloads[i].IsComplete)
                        continue;

                    m_activeDownloads[i].Cancel();
                    m_activeDownloads[i].Dispose();
                    m_activeDownloads[i] = null;
                }

                if (NumberRetries >= 5 || m_isConnectionWaiting)
                {
                    // Anti hammer in effect
                    if (!m_isConnectionWaiting)
                    {
                        m_connectionWaitStart = DateTime.Now;
                        m_isConnectionWaiting = true;
                    }

                    if (DateTime.Now.Subtract(m_connectionWaitTime) > m_connectionWaitStart)
                    {
                        NumberRetries = 0;
                        m_isConnectionWaiting = false;
                    }
                    return;
                }

                // Queue new downloads
                for (int i = 0; i < World.Settings.MaxSimultaneousDownloads; i++)
                {
                    if (m_activeDownloads[i] != null)
                        continue;

                    if (m_downloadRequests.Count <= 0)
                        continue;

                    m_activeDownloads[i] = GetClosestDownloadRequest();
                    if (m_activeDownloads[i] != null)
                    {
                        m_downloadStarted[i] = DateTime.Now;
                        m_activeDownloads[i].StartDownload();
                    }
                }
            }
        }

        /// <summary>
        /// Finds the "best" tile from queue
        /// </summary>
        public virtual GeoImageDownloadRequest GetClosestDownloadRequest()
        {
            GeoImageDownloadRequest closestRequest = null;
            float largestArea = float.MinValue;

            lock (m_downloadRequests.SyncRoot)
            {
                foreach (GeoImageDownloadRequest curRequest in m_downloadRequests.Values)
                {
                    if (curRequest.IsDownloading)
                        continue;

                    QuadTile qt = curRequest.QuadTile;
                    if (!m_camera.ViewFrustum.Intersects(qt.BoundingBox))
                        continue;

                    float screenArea = qt.BoundingBox.CalcRelativeScreenArea(m_camera);
                    if (screenArea > largestArea)
                    {
                        largestArea = screenArea;
                        closestRequest = curRequest;
                    }
                }
            }

            return closestRequest;
        }

        /// <summary>
        /// Creates a tile download indication texture
        /// </summary>
        static protected Texture CreateDownloadRectangle(Device device, Color color, int padding)
        {
            int mid = 128;
            using (Bitmap i = new Bitmap(2 * mid, 2 * mid))
            using (Graphics g = Graphics.FromImage(i))
            using (Pen pen = new Pen(color))
            {
                int width = mid - 1 - 2 * padding;
                g.DrawRectangle(pen, padding, padding, width, width);
                g.DrawRectangle(pen, mid + padding, padding, width, width);
                g.DrawRectangle(pen, padding, mid + padding, width, width);
                g.DrawRectangle(pen, mid + padding, mid + padding, width, width);

                Texture texture = new Texture(device, i, Usage.None, Pool.Managed);
                return texture;
            }
        }

       
    }
}

