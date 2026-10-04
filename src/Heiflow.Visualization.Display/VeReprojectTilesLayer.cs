using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;

using System.Runtime.InteropServices;
using System.Net;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Net;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D.Terrain;
using Heiflow.Spatial;
using Heiflow.Core;

namespace HUST.WREIS.Dot3D
{
    #region VEPROJECTTILESLAYER
    public class VeReprojectTilesLayer : RenderableObject
    {
        private Projection proj;
        private static double earthRadius; //6378137;
        private static double earthCircum; //40075016.685578488
        private static double earthHalfCirc; //20037508.
        private static string ServerLogoFilePath = "Plugins\\PlaceFinder\\vejewel.png";
        private static Texture m_iconTexture;
        private static Rectangle m_spriteSize;

        private World mCurrentWorld;
        //private static Sprite sprite;


        private const int pixelsPerTile = 256;
        private int prevRow = -1;
        private int prevCol = -1;
        private int prevLvl = -1;
        private float prevVe = -1;

        public World CurrentWorld 
        {
            get
            {
                return mCurrentWorld;
            }
        }

        public SceneWindow WorldWindow
        {
            get;
            set;
        }
        public string DataSetName
        {
            get;set;
        }
        public string ImageExtension
        {
            get;set;
        }

        public int StartZoomLevel
        {
            get;set;
        }

        public bool IsDebug 
        {
            get;set;
        }
        public bool IsTerrainOn
        {
            get;
            set;
        }
        private ArrayList veTiles = new ArrayList();

        public VeReprojectTilesLayer(string name,SceneWindow ww,int startZoomLevel)
            : base(name)
        {
            DataSetName="h";
            this.name = name;
            mCurrentWorld=ww.CurrentWorld;
            WorldWindow=ww;
            StartZoomLevel=startZoomLevel;
            IsDebug=false;
            ImageExtension = "jpg";
        }

        private Sprite sprite;
        private Texture spriteTexture;
        float scaleWidth = .25f;
        float scaleHeight = .25f;
        int iconWidth = 128;
        int iconHeight = 128;
        Rectangle spriteSize;

        /// <summary>
        /// Layer initialization code
        /// </summary>
        public override void Initialize(DrawArgs drawArgs)
        {
            try
            {
                if (this.isInitialized == true)
                {
                    return;
                }

                //init the sprite for PushPins
                sprite = new Sprite(drawArgs.device);
                //#if DEBUG
                string spritePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Plugins\\VirtualEarth\\VirtualEarthPushPin.png";
                //#else
                //				string spritePath = VirtualEarthPlugin.PluginDir + @"\VirtualEarthPushPin.png";
                //
                //#endif
                if (File.Exists(spritePath) == false)
                {
                    Log.Write(new Exception("spritePath not found " + spritePath));
                }
                spriteSize = new Rectangle(0, 0, iconWidth, iconHeight);
                spriteTexture = TextureLoader.FromFile(drawArgs.device, spritePath);

                earthRadius = CurrentWorld.EquatorialRadius;
                earthCircum = earthRadius * 2.0 * Math.PI; //40075016.685578488
                earthHalfCirc = earthCircum / 2; //20037508.

                //NOTE tiles did not line up properly with ellps=WGS84
                //string [] projectionParameters = new string[]{"proj=merc", "ellps=WGS84", "no.defs"};
                //+proj=longlat +ellps=sphere +a=6370997.0 +es=0.0
                     string[] args = {"+proj=lcc", "+lat_1=33", 
							"+lat_2=45", "+datum=NAD27", "+nodefs"};
                string[] projectionParameters = new string[] { "proj=merc", "ellps=sphere", "a=" + earthRadius.ToString(), "es=0.0", "no.defs" };
                proj = new Projection(projectionParameters);

                //static
                VeTile.Init(this.proj, CurrentWorld.TerrainAccessor, CurrentWorld.EquatorialRadius);

                prevVe = World.Settings.VerticalExaggeration;

                this.isInitialized = true;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                throw;
            }
        }

        public string GetLocalLiveLink()
        {
            //http://local.live.com/default.aspx?v=2&cp=43.057723~-88.404224&style=r&lvl=12
            string lat = WorldWindow.DrawArgs.WorldCamera.Latitude.Degrees.ToString("###.#####");
            string lon = WorldWindow.DrawArgs.WorldCamera.Longitude.Degrees.ToString("###.#####");
            string link = "http://local.live.com/default.aspx?v=2&cp=" + lat + "~" + lon + "&styles=" + DataSetName + "&lvl=" + prevLvl.ToString();
            return link;
        }

        public void RemoveAllTiles()
        {
            lock (veTiles.SyncRoot)
            {
                for (int i = 0; i < veTiles.Count; i++)
                {
                    VeTile veTile = (VeTile)veTiles[i];
                    veTile.Dispose();
                    veTiles.RemoveAt(i);
                }
                veTiles.Clear();
            }
        }

        public void ForceRefresh()
        {
            prevRow = -1;
            prevCol = -1;
            prevLvl = -1;
        }

        /// <summary>
        /// Update layer (called from worker thread)
        /// </summary>
        public override void Update(DrawArgs drawArgs)
        {

            try
            {
                if (this.isOn == false)
                {
                    return;
                }

                //NOTE for some reason Initialize is not getting called from the Plugin Menu Load/Unload
                //it does get called when the plugin loads from Startup
                //not sure what is going on, so i'll just call it manually
                if (this.isInitialized == false)
                {
                    this.Initialize(drawArgs);
                    return;
                }

                //get lat, lon
                double lat = drawArgs.WorldCamera.Latitude.Degrees;
                double lon = drawArgs.WorldCamera.Longitude.Degrees;
                //determine zoom level
                double alt = drawArgs.WorldCamera.Altitude;
                //could go off distance, but this changes when view angle changes
                //Angle fov = drawArgs.WorldCamera.Fov; //stays at 45 degress
                //Angle viewRange = drawArgs.WorldCamera.ViewRange; //off of distance, same as TVR but changes when view angle changes
                Angle tvr = drawArgs.WorldCamera.TrueViewRange; //off of altitude
                //smallest altitude = 100m
                //tvr = .00179663198575926
                //start altitude = 12756273m
                //tvr = 180

                //WW _levelZeroTileSizeDegrees
                //180 90 45 22.5 11.25 5.625 2.8125 1.40625 .703125 .3515625 
                //.17578125 .087890625 0.0439453125 0.02197265625 0.010986328125 0.0054931640625
                int zoomLevel = GetZoomLevelByTrueViewRange(tvr.Degrees);
                //dont start VE tiles until a certain zoom level
                if (zoomLevel < StartZoomLevel)
                {
                    this.RemoveAllTiles();
                    return;
                }

                //WW tiles
                //double tileDegrees = GetLevelDegrees(zoomLevel);
                //int row = MathEngine.GetRowFromLatitude(lat, tileDegrees);
                //int col = MathEngine.GetColFromLongitude(lon, tileDegrees);

                //VE tiles
                double metersY;
                double yMeters;
                int yMetersPerPixel;
                int row;
                /*
                //WRONG - doesn't stay centered away from equator
                //int yMeters = LatitudeToYAtZoom(lat, zoomLevel); //1024
                double sinLat = Math.Sin(DegToRad(lat));
                metersY = earthRadius / 2 * Math.Log((1 + sinLat) / (1 - sinLat)); //0
                yMeters = earthHalfCirc - metersY; //20037508.342789244
                yMetersPerPixel = (int) Math.Round(yMeters / MetersPerPixel(zoomLevel));
                row = yMetersPerPixel / pixelsPerTile;
                */
                //CORRECT
                //int xMeters = LongitudeToXAtZoom(lon, zoomLevel); //1024
                double metersX = earthRadius * DegToRad(lon); //0
                double xMeters = earthHalfCirc + metersX; //20037508.342789244
                int xMetersPerPixel = (int)Math.Round(xMeters / MetersPerPixel(zoomLevel));
                int col = xMetersPerPixel / pixelsPerTile;

                //reproject - overrides row above
                //this correctly keeps me on the current tile that is being viewed
                UV uvCurrent = new UV(DegToRad(lon), DegToRad(lat));
                uvCurrent = proj.Forward(uvCurrent);
                metersY = uvCurrent.V;
                yMeters = earthHalfCirc - metersY;
                yMetersPerPixel = (int)Math.Round(yMeters / MetersPerPixel(zoomLevel));
                row = yMetersPerPixel / pixelsPerTile;

                //update mesh if VertEx changes
                if (prevVe != World.Settings.VerticalExaggeration)
                {
                    lock (veTiles.SyncRoot)
                    {
                        VeTile veTile;
                        for (int i = 0; i < veTiles.Count; i++)
                        {
                            veTile = (VeTile)veTiles[i];
                            if (veTile.VertEx != World.Settings.VerticalExaggeration)
                            {
                                veTile.CreateMesh(this.Opacity, World.Settings.VerticalExaggeration);
                            }
                        }
                    }
                }
                prevVe = World.Settings.VerticalExaggeration;

                //if within previous bounds and same zoom level, then exit
                if (row == prevRow && col == prevCol && zoomLevel == prevLvl)
                {
                    return;
                }

                //System.Diagnostics.Debug.WriteLine("CHANGE");

                lock (veTiles.SyncRoot)
                {
                    VeTile veTile;
                    for (int i = 0; i < veTiles.Count; i++)
                    {
                        veTile = (VeTile)veTiles[i];
                        veTile.IsNeeded = false;
                    }
                }

                //metadata
                ArrayList alMetadata = null;
                if (IsDebug == true)
                {
                    alMetadata = new ArrayList();
                    alMetadata.Add("yMeters " + yMeters.ToString());
                    alMetadata.Add("metersY " + metersY.ToString());
                    alMetadata.Add("yMeters2 " + yMeters.ToString());
                    alMetadata.Add("vLat " + uvCurrent.V.ToString());
                    //alMetadata.Add("xMeters " + xMeters.ToString());
                    //alMetadata.Add("metersX " + metersX.ToString());
                    //alMetadata.Add("uLon " + uvCurrent.U.ToString());
                }

                //add current tiles first
                AddVeTile(drawArgs, row, col, zoomLevel, alMetadata);
                //then add other tiles outwards in surrounding circles
                AddNeighborTiles(drawArgs, row, col, zoomLevel, null, 1);
                AddNeighborTiles(drawArgs, row, col, zoomLevel, null, 2);
                AddNeighborTiles(drawArgs, row, col, zoomLevel, null, 3);

                //if(prevLvl > zoomLevel) //zooming out
                //{
                //}			

                lock (veTiles.SyncRoot)
                {
                    VeTile veTile =null;
                    for (int i = 0; i < veTiles.Count; i++)
                    {
                        veTile = (VeTile)veTiles[i];
                        if (veTile.IsNeeded == false)
                        {
                            veTile.Dispose();
                            veTiles.RemoveAt(i);
                        }
                    }
                }

                prevRow = row;
                prevCol = col;
                prevLvl = zoomLevel;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void AddNeighborTiles(DrawArgs drawArgs, int row, int col, int zoomLevel, ArrayList alMetadata, int range)
        {
            int minRow = row - range;
            int maxRow = row + range;
            int minCol = col - range;
            int maxCol = col + range;
            for (int i = minRow; i <= maxRow; i++)
            {
                for (int j = minCol; j <= maxCol; j++)
                {
                    //only outer edges, inner tiles should already be added
                    if (i == minRow || i == maxRow || j == minCol || j == maxCol)
                    {
                        AddVeTile(drawArgs, i, j, zoomLevel, alMetadata);
                    }
                }
            }
        }

        private void AddVeTile(DrawArgs drawArgs, int row, int col, int zoomLevel, ArrayList alMetadata)
        {
            //TODO handle column wrap-around
            //haven't had to explicitly handle this yet

            bool tileFound = false;
            lock (veTiles.SyncRoot)
            {
                foreach (VeTile veTile in veTiles)
                {
                    if (veTile.IsNeeded == true)
                    {
                        continue;
                    }
                    if (veTile.IsEqual(row, col, zoomLevel) == true)
                    {
                        veTile.IsNeeded = true;
                        tileFound = true;
                        break;
                    }
                }
            }
            if (tileFound == false)
            {
                //exit if zoom level has changed
                int curZoomLevel = GetZoomLevelByTrueViewRange(drawArgs.WorldCamera.TrueViewRange.Degrees);
                if (curZoomLevel != zoomLevel)
                {
                    return;
                }
                VeTile newVeTile = CreateVeTile(drawArgs, row, col, zoomLevel, alMetadata);
                newVeTile.IsNeeded = true;
                lock (veTiles.SyncRoot)
                {
                    veTiles.Add(newVeTile);
                }
            }
        }

        private VeTile CreateVeTile(DrawArgs drawArgs, int row, int col, int zoomLevel, ArrayList alMetadata)
        {
            VeTile newVeTile = new VeTile(row, col, zoomLevel,WorldWindow);
            newVeTile.IsDebug = IsDebug;
            newVeTile.DatasetName = DataSetName;
            newVeTile.ImageExtension = ImageExtension;
            newVeTile.IsTerrainOn = IsTerrainOn;

            //metadata
            if (alMetadata != null)
            {
                foreach (string metadata in alMetadata)
                {
                    newVeTile.AddMetaData(metadata);
                }
            }

            //thread to download new tile(s) or just load from cache
            newVeTile.GetTexture(drawArgs, pixelsPerTile);

            //handle the diff projection
            double metersPerPixel = MetersPerPixel(zoomLevel);
            double totalTilesPerEdge = Math.Pow(2, zoomLevel);
            double totalMeters = totalTilesPerEdge * pixelsPerTile * metersPerPixel;
            double halfMeters = totalMeters / 2;

            //do meters calculation in VE space
            //the 0,0 origin for VE is in upper left
            double N = row * (pixelsPerTile * metersPerPixel);
            double W = col * (pixelsPerTile * metersPerPixel);
            //now convert it to +/- meter coordinates for Proj.4
            //the 0,0 origin for Proj.4 is 0 lat, 0 lon
            //-22 to 22 million, -11 to 11 million
            N = halfMeters - N;
            W = W - halfMeters;
            double E = W + (pixelsPerTile * metersPerPixel);
            double S = N - (pixelsPerTile * metersPerPixel);

            newVeTile.UL = new UV(W, N);
            newVeTile.UR = new UV(E, N);
            newVeTile.LL = new UV(W, S);
            newVeTile.LR = new UV(E, S);

            //create mesh
            byte opacity = this.Opacity; //from RenderableObject
            float verticalExaggeration = World.Settings.VerticalExaggeration;
            newVeTile.CreateMesh(opacity, verticalExaggeration);
            newVeTile.CreateDownloadRectangle(drawArgs, World.Settings.DownloadProgressColor.ToArgb());

            return newVeTile;
        }

        private static double MetersPerTile(int zoom)
        {
            return MetersPerPixel(zoom) * pixelsPerTile;
        }

        private static double MetersPerPixel(int zoom)
        {
            double arc;
            arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            return arc;
        }

        private static double DegToRad(double d)
        {
            return d * Math.PI / 180.0;
        }

        private static double RadToDeg(double d)
        {
            return d * 180 / Math.PI;
        }

        public double GetLevelDegrees(int level)
        {
            double metersPerPixel = MetersPerPixel(level);
            double arcDistance = metersPerPixel * pixelsPerTile;
            //double arcDistance = earthCircum * (tileRange / 360);
            double tileRange = (arcDistance / earthCircum) * 360;
            return tileRange;
        }

        public int GetZoomLevelByTrueViewRange(double trueViewRange)
        {
            int maxLevel = 3;
            int minLevel = 19;
            int numLevels = minLevel - maxLevel + 1;
            int retLevel = maxLevel;
            for (int i = 0; i < numLevels; i++)
            {
                retLevel = i + maxLevel;

                double viewAngle = 180;
                for (int j = 0; j < i; j++)
                {
                    viewAngle = viewAngle / 2.0;
                }
                if (trueViewRange >= viewAngle)
                {
                    break;
                }
            }
            return retLevel;
        }

        public int GetZoomLevelByArcDistance(double arcDistance)
        {
            //arcDistance in meters
            int totalLevels = 24;
            int level = 0;
            for (level = 1; level <= totalLevels; level++)
            {
                double metersPerPixel = MetersPerPixel(level);
                double totalDistance = metersPerPixel * pixelsPerTile;
                if (arcDistance > totalDistance)
                {
                    break;
                }
            }
            return level - 1;
        }

        private int LatitudeToYAtZoom(double lat, int zoom)
        {
            int y;
            //code VE Mobile v1 - NO LONGER VALID
            //double sinLat = Math.Sin(DegToRad(lat));
            //double metersY = 6378137 / 2 * Math.Log((1 + sinLat) / (1 - sinLat));
            //y = (int)Math.Round((20971520 - metersY) / MetersPerPixel(zoom));
            //forum - SKIPS TILES THE FURTHER YOU GET FROM EQUATOR
            double arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            double sinLat = Math.Sin(DegToRad(lat));
            double metersY = earthRadius / 2 * Math.Log((1 + sinLat) / (1 - sinLat));
            y = (int)Math.Round((earthHalfCirc - metersY) / arc);
            //HACK - THIS HANDLES THE SKIPPING OF TILES THE FURTHER YOU GET FROM EQUATOR
            //double arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            //double metersY = earthRadius * DegToRad(lat);
            //y = (int) Math.Round((earthHalfCirc - metersY) / arc);
            return y;
        }

        private int LongitudeToXAtZoom(double lon, int zoom)
        {
            int x;
            double arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            double metersX = earthRadius * DegToRad(lon);
            x = (int)Math.Round((earthHalfCirc + metersX) / arc);
            return x;
        }
      
        /// <summary>
        /// Draws the layer
        /// </summary>
        public override void Render(DrawArgs drawArgs)
        {
            try
            {
                if (this.isOn == false)
                {
                    return;
                }

                if (this.isInitialized == false)
                {
                    return;
                }

                if (drawArgs.device == null)
                    return;

                if (veTiles != null && veTiles.Count > 0)
                {
                    //render mesh and tile(s)
                    bool disableZBuffer = false; //TODO where do i get this setting
                    //foreach(VeTile veTile in veTiles)
                    //{
                    //	veTile.Render(drawArgs, disableZBuffer);
                    //}

                    // camera jitter fix
                    drawArgs.device.Transform.World = Matrix.Translation(
                           (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                        (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                    );

                    // Clear ZBuffer between layers (as in WW)
                    drawArgs.device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);

                    // Render tiles
                    VeTile.Render(drawArgs, disableZBuffer, veTiles);

                    //camera jitter fix
                    drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
                }

                //else pushpins only
                //render PushPins
                if (pushPins != null && pushPins.Count > 0)
                {

                    RenderPushPins(drawArgs);

                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public void RenderDownloadProgress(DrawArgs drawArgs, HUST.WREIS.Dot3D.Renderable.GeoImageDownloadRequest request, int offset)
        {
            int halfIconHeight = 24;
            int halfIconWidth = 24;

            Vector3 projectedPoint = new Vector3(DrawArgs.ParentControl.Width - halfIconWidth - 10, DrawArgs.ParentControl.Height - 34 - 4 * offset, 0.5f);
            /*
            // Render progress bar
            if (progressBar == null)
                progressBar = new ProgressBar(40, 4);
            progressBar.Draw(drawArgs, projectedPoint.X, projectedPoint.Y + 24, request.ProgressPercent, World.Settings.DownloadProgressColor.ToArgb());
            DrawArgs.Device.RenderState.ZBufferEnable = true;
            */
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
                    new Vector2(0, 0),
                    0.0f, new Vector2(projectedPoint.X, projectedPoint.Y));

            this.sprite.Draw(m_iconTexture, m_spriteSize,
                    new Vector3(1.32f * 48, 1.32f * 48, 0), new Vector3(0, 0, 0),
                    World.Settings.DownloadLogoColor);
            this.sprite.End();
        }

        public void GetViewPort(DrawArgs drawArgs, out double lat1, out double lon1, out double lat2, out double lon2)
        {
            double halfViewRange = drawArgs.WorldCamera.TrueViewRange.Degrees / 2;
            double lat = drawArgs.WorldCamera.Latitude.Degrees;
            double lon = drawArgs.WorldCamera.Longitude.Degrees;
            lat1 = lat + halfViewRange;
            lon1 = lon + halfViewRange;
            lat2 = lat - halfViewRange;
            lon2 = lon - halfViewRange;
        }

        private ArrayList pushPins = null;
        public ArrayList PushPins
        {
            get { return pushPins; }
            set { pushPins = value; }
        }
        //private Icons pushPins = null;
        //public Icons PushPins
        //{
        //	get{return pushPins;}
        //	set{pushPins = value;}
        //}

        public void RenderPushPins(DrawArgs drawArgs)
        {
            if (pushPins == null || pushPins.Count <= 0)
                return;

            //pushPins.Initialize(drawArgs);
            //pushPins.Render(drawArgs);

            double lat1, lon1, lat2, lon2;
            GetViewPort(drawArgs, out lat1, out lon1, out lat2, out lon2);

            Vector3 projectedPoint;

            lock (pushPins.SyncRoot)
            {
                foreach (PushPin p in pushPins)
                {
                    if (p.Latitude <= lat1 && p.Latitude >= lat2)
                    {
                        if (p.Longitude <= lon1 && p.Longitude >= lon2)
                        {
                            projectedPoint = MathEngine.SphericalToCartesian((float)p.Latitude, (float)p.Longitude, (float)earthRadius + 100);
                            projectedPoint.Project(drawArgs.device.Viewport, drawArgs.WorldCamera.ProjectionMatrix, drawArgs.WorldCamera.ViewMatrix, drawArgs.WorldCamera.WorldMatrix);

                            sprite.Begin(SpriteFlags.AlphaBlend);
                            sprite.Transform = Matrix.Transformation2D(new Vector2(0.0f, 0.0f),
                                0.0f, new Vector2(scaleWidth, scaleHeight),
                                new Vector2(0, 0), 0.0f, new Vector2(projectedPoint.X, projectedPoint.Y));
                            sprite.Draw(spriteTexture, spriteSize, new Vector3(.5f * iconWidth, .5f * iconHeight, 0), new Vector3(0, 0, 0), System.Drawing.Color.White);
                            sprite.End();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cleanup when layer is disabled
        /// </summary>
        public override void Dispose()
        {
            RemoveAllTiles();

            if (sprite != null)
            {
                sprite.Dispose();
                sprite = null;
            }
        }

        /// <summary>
        /// Handle mouse click
        /// </summary>
        /// <returns>true if click was handled.</returns>
        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }
    }
    #endregion

    #region VESETTINGS
    /// <summary>
    /// This class stores virtual earth settings
    /// </summary>
    [Serializable]
    public class VESettings
    {
        /// <summary>
        /// Layer Zoom Level
        /// </summary>
        private int zoomlevel = 8;
        /// <summary>
        /// Turn layer on
        /// </summary>
        private bool layeron = true;
        /// <summary>
        /// Turn terrain on
        /// </summary>
        private bool terrain = true;
        /// <summary>
        /// Layer types
        /// </summary>
        private bool road = true;
        private bool aerial = false;
        private bool hybrid = false;
        private bool debug = false;


        public int ZoomLevel
        {
            get
            {
                return zoomlevel;
            }
            set
            {
                zoomlevel = value;
            }
        }

        public bool LayerOn
        {
            get
            {
                return layeron;
            }
            set
            {
                layeron = value;
            }
        }

        public bool Terrain
        {
            get
            {
                return terrain;
            }
            set
            {
                terrain = value;
            }
        }

        public bool Road
        {
            get
            {
                return road;
            }
            set
            {
                road = value;
            }
        }

        public bool Aerial
        {
            get
            {
                return aerial;
            }
            set
            {
                aerial = value;
            }
        }

        public bool Hybrid
        {
            get
            {
                return hybrid;
            }
            set
            {
                hybrid = value;
            }
        }

        public bool Debug
        {
            get
            {
                return debug;
            }
            set
            {
                debug = value;
            }
        }
        /// <summary>
        /// Loads a serialized instance of the settings from the specified file
        /// returns default values if the file doesn't exist or an error occurs
        /// </summary>
        /// <returns>The persisted settings from the file</returns>
        public static VESettings LoadSettingsFromFile(string filename)
        {
            VESettings settings;
            XmlSerializer xs = new XmlSerializer(typeof(VESettings));

            if (File.Exists(filename))
            {
                FileStream fs = null;

                try
                {
                    fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                }
                catch
                {
                    return new VESettings();
                }

                try
                {
                    settings = (VESettings)xs.Deserialize(fs);
                }
                catch
                {
                    settings = new VESettings();
                }
                finally
                {
                    fs.Close();
                }
            }
            else
            {
                settings = new VESettings();
            }

            return settings;
        }

        /// <summary>
        /// Persists the settings to the specified filename
        /// </summary>
        /// <param name="file">The filename to use for saving</param>
        /// <param name="settings">The instance of the Settings class to persist</param>
        public static void SaveSettingsToFile(string file, VESettings settings)
        {
            FileStream fs = null;
            XmlSerializer xs = new XmlSerializer(typeof(VESettings));

            fs = File.Open(file, FileMode.Create, FileAccess.Write);

            try
            {
                xs.Serialize(fs, settings);
            }
            finally
            {
                fs.Close();
            }
        }
    }
    #endregion

}
