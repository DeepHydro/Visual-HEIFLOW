using Microsoft.DirectX.Direct3D;
using HUST.WREIS.Dot3D.Net.Wms;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.IO;
using System.Collections;
using System.Drawing;
using Heiflow.Spatial;

namespace HUST.WREIS.Dot3D
{
    public enum ImageCacheLocation {LocalFileSys,SQLite}
	/// <summary>
	/// Base class for calculating local image paths and remote download urls
	/// </summary>
	public class ImageStore
	{
		#region Private Members

		protected string	m_dataDirectory;
		protected double m_levelZeroTileSizeDegrees = 36;
		protected int m_levelCount = 1;
		protected string m_imageFileExtension;
		protected string m_cacheDirectory;
		protected string m_duplicateTexturePath;
        protected string m_serverlogo;

		#endregion

		#region Properties

        public ImageCacheLocation ImageCacheLocation { get; set; }

        public IImageStorageService ImageService { get; set; }

        public DrawArgs DrawArgs { get; set; }
		/// <summary>
		/// Coverage of outer level 0 bitmaps (decimal degrees)
		/// Level 1 has half the coverage, level 2 half of level 1 (1/4) etc.
		/// </summary>
		public double LevelZeroTileSizeDegrees
		{
			get
			{
				return m_levelZeroTileSizeDegrees;
			}
			set
			{
				m_levelZeroTileSizeDegrees = value;
			}
		}
        
        /// <summary>
        /// The user agent the tiles of this store are requested with. Empty means the shared one of
        /// WebDownload is used. Some servers refuse that one: the WAF of the Chinese national service
        /// answers "您的请求疑似攻击行为" to it and only lets requests through that look like they
        /// come from a browser, so a layer can name its own agent in Layers.xml.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Server Logo path for Downloadable layers
        /// </summary>
        public string ServerLogo
        {
            get
            {
                return m_serverlogo;
            }
            set
            {
                m_serverlogo = value;
            }
        }

		/// <summary>
		/// Number of detail levels
		/// </summary>
		public int LevelCount
		{
			get
			{
				return m_levelCount;
			}
			set
			{
				m_levelCount = value;
			}
		}

		/// <summary>
		/// File extension of the source image file format
		/// </summary>
		public string ImageExtension
		{
			get
			{
				return m_imageFileExtension;
			}
			set
			{
				// Strip any leading dot
				m_imageFileExtension = value.Replace(".", "");
			}
		}

		/// <summary>
		/// Cache subdirectory for this layer
		/// </summary>
		public string CacheDirectory
		{
			get
			{
				return m_cacheDirectory;
			}
			set
			{
				m_cacheDirectory = value;
			}
		}

		/// <summary>
		/// Data directory for this layer (permanently stored images)
		/// </summary>
		public string DataDirectory
		{
			get
			{
				return m_dataDirectory;
			}
			set
			{
				m_dataDirectory = value;
			}
		}

		/// <summary>
		/// Default texture to be used (always ocean?)
		/// Can be either file or url
		/// </summary>
		public string DuplicateTexturePath
		{
			get
			{
				return m_duplicateTexturePath;
			}
			set
			{
				m_duplicateTexturePath = value;
			}
		}

        public virtual bool IsDownloadableLayer
        {
            set;
            get;
        }

		#endregion

        protected double earthRadius = 6378137;
        protected double earthCircum = 40075016.685578488;
        protected double earthHalfCirc = 20037508;
        protected const int pixelsPerTile = 512;
        protected Projection proj;

        public string DataSetName { get; set; }

        public ImageStore()
        {
            IsDownloadableLayer = false;
            DataSetName = "h";
            string[] projectionParameters = new string[] { "proj=merc", "ellps=sphere", "a=" + earthRadius.ToString(), "es=0.0", "no.defs" };
            proj = new Projection(projectionParameters);
            _font = new System.Drawing.Font("Verdana", 15, FontStyle.Bold);
            _brush = new System.Drawing.SolidBrush(Color.Red);
            ImageCacheLocation = Dot3D.ImageCacheLocation.LocalFileSys;
        }

        public bool IsDebug = true;
        protected System.Drawing.Font _font;
        protected System.Drawing.Brush _brush;

        public virtual string GetLocalPath(QuadTile qt)
        {
            if (qt.Level >= m_levelCount)
                throw new ArgumentException(string.Format("Level {0} not available.",
                    qt.Level));
            string cacheFullPath = "";
            if (World.Settings.UsePseudoColor)
            {
               cacheFullPath = Path.Combine(ConfigurationManager.Engine3DSettings.DataPath, World.Settings.PseudoColorImagePath);
            }
            else
            {
                string relativePath = String.Format(@"{0}\{1:D4}\{1:D4}_{2:D4}.{3}",
                    qt.Level, qt.Row, qt.Col, m_imageFileExtension);
                //  relativePath = @"1\0513\0051_0123.jpeg";
                if (m_dataDirectory != null)
                {
                    // Search data directory first
                    string rawFullPath = Path.Combine(m_dataDirectory, relativePath);
                    if (File.Exists(rawFullPath))
                        return rawFullPath;
                }

                // If cache doesn't exist, fall back to duplicate texture path.
                if (m_cacheDirectory == null)
                    return m_duplicateTexturePath;

                // Try cache with default file extension
                cacheFullPath = Path.Combine(m_cacheDirectory, relativePath);
                if (File.Exists(cacheFullPath))
                    return cacheFullPath;

                // Try cache but accept any valid image file extension
                const string ValidExtensions = ".bmp.dds.dib.hdr.jpg.jpeg.pfm.png.ppm.tga.gif.tif";

                string cacheSearchPath = Path.GetDirectoryName(cacheFullPath);
                if (Directory.Exists(cacheSearchPath))
                {
                    foreach (string imageFile in Directory.GetFiles(
                        cacheSearchPath,
                        Path.GetFileNameWithoutExtension(cacheFullPath) + ".*"))
                    {
                        string extension = Path.GetExtension(imageFile).ToLower();
                        if (ValidExtensions.IndexOf(extension) < 0)
                            continue;

                        return imageFile;
                    }
                }
            }
            return cacheFullPath;
        }

		/// <summary>
		/// Figure out how to download the image.
		/// TODO: Allow subclasses to have control over how images are downloaded, 
		/// not just the download url.
		/// </summary>
        public virtual string GetDownloadUrl(QuadTile qt)
		{
			// No local image, return our "duplicate" tile if any
			if(m_duplicateTexturePath != null && File.Exists(m_duplicateTexturePath))
				return m_duplicateTexturePath;

			// No image available anywhere, give up
			return "";
		}

		/// <summary>
		/// Deletes the cached copy of the tile.
		/// </summary>
		/// <param name="qt"></param>
		public virtual void DeleteLocalCopy(QuadTile qt)
		{
			string filename = GetLocalPath(qt);
			if(File.Exists(filename))
				File.Delete(filename);
		}


		/// <summary>
		/// Converts image file to DDS
		/// </summary>
        public virtual void ConvertImage(Texture texture, string filePath)
		{
			if(filePath.ToLower().EndsWith(".dds"))
				// Image is already DDS
				return;

			// User has selected to convert downloaded images to DDS
			string convertedPath = Path.Combine(
				Path.GetDirectoryName(filePath),
				Path.GetFileNameWithoutExtension(filePath)+".dds");

			TextureLoader.Save(convertedPath, ImageFileFormat.Dds, texture );

			// Delete the old file
			try
			{
				File.Delete(filePath);
			}
			catch
			{
			}
		}

		public virtual Texture LoadFile(QuadTile qt)
		{
            return ImageService.LoadFile(qt);
		}

        public Texture GenerateDebugTexture(QuadTile qt)
        {
            Texture texture = null;

            MemoryStream ms;
            Bitmap b = new Bitmap(pixelsPerTile, pixelsPerTile);
            ArrayList alMetaData = new ArrayList();
            System.Drawing.Imaging.ImageFormat imageFormat;

            alMetaData.Add("row: " + qt.Row.ToString());
            alMetaData.Add("col: " + qt.Col.ToString());
            alMetaData.Add("level : " + qt.Level.ToString());

            imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
            b = DecorateBitmap(b, _font, _brush, alMetaData);
            ms = new MemoryStream();
            b.Save(ms, imageFormat);
            ms.Position = 0;
            texture = TextureLoader.FromStream(DrawArgs.device, ms);
            ms.Close();
            ms = null;
            b.Dispose();
            b = null;
            return texture;
        }

        public void QueueDownload(QuadTile qt, string filePath)
        {
            string url = GetDownloadUrl(qt);
            qt.QuadTileSet.AddToDownloadQueue(qt.QuadTileSet.Camera,
                new GeoImageDownloadRequest(qt, this, filePath, url));
        }

        public void GetRowColumnLevel(out int row, out int col, out int zoomLevel, double lat, double lon, int basicLevel, DrawArgs drawArgs)
        {

            double alt = drawArgs.WorldCamera.Altitude;

            Angle tvr = drawArgs.WorldCamera.TrueViewRange; //off of altitude     
            zoomLevel = basicLevel + 0;

            double metersY;
            double yMeters;
            int yMetersPerPixel;

            double metersX = earthRadius * DegToRad(lon); //0
            double xMeters = earthHalfCirc + metersX; //20037508.342789244
            double xmpp = xMeters / MetersPerPixel(zoomLevel);
            int xMetersPerPixel = (int)Math.Round(xmpp);
            col = xMetersPerPixel / pixelsPerTile;

            UV uvCurrent = new UV(DegToRad(lon), DegToRad(lat));
            uvCurrent = proj.Forward(uvCurrent);
            metersY = uvCurrent.V;
            yMeters = earthHalfCirc - metersY;
            double ympp = yMeters / MetersPerPixel(zoomLevel);
            yMetersPerPixel = (int)Math.Round(ympp);
            row = yMetersPerPixel / pixelsPerTile;
        }

        //for generating the debug bitmap
        public Bitmap DecorateBitmap(Bitmap b, System.Drawing.Font font, Brush brush, ArrayList alMetadata)
        {
            if (alMetadata.Count > 0)
            {
                //if(b.RawFormat == System.Drawing.Imaging.ImageFormat.Png)
                if (b.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    MemoryStream ms = new MemoryStream();
                    b.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    b.Dispose();
                    b = null;
                    b = new Bitmap(256, 256);
                    b = (Bitmap)Bitmap.FromStream(ms);
                    ms.Close();
                    ms = null;
                }
                Graphics g = Graphics.FromImage(b); //fails for png files

                g.Clear(Color.Gainsboro);
                g.DrawLine(Pens.Red, 0, 0, b.Width, 0);
                g.DrawLine(Pens.Red, 0, 0, 0, b.Height);
                string s = (string)alMetadata[0];
                SizeF sizeF = g.MeasureString(s, font);
                for (int i = 0; i < alMetadata.Count; i++)
                {
                    s = (string)alMetadata[i];
                    int x = 0;
                    int y = (int)(sizeF.Height * (i + 0));
                    g.DrawString(s, font, brush, x, y);
                }
                g.Dispose();
            }
            return b;
        }

        public string CreateLevelDir(int level, string cacheDirectoryRoot)
        {
            string levelDir = null;
            //VirtualEarth.m_WorldWindow.Cache.CacheDirectory
            string cacheDirectory = cacheDirectoryRoot;// String.Format("{0}\\Virtual Earth", cacheDirectoryRoot);
            if (Directory.Exists(cacheDirectory) == false)
            {
                Directory.CreateDirectory(cacheDirectory);
            }
            levelDir = cacheDirectory + @"\" + level.ToString();
            if (Directory.Exists(levelDir) == false)
            {
                Directory.CreateDirectory(levelDir);
            }
            return levelDir;
        }

        public string CreateMapTypeDir(string levelDir, string mapType)
        {
            string mapTypeDir = "";
            if (mapType != "none")
                mapTypeDir = levelDir + @"\" + mapType;
            else
                mapTypeDir = levelDir;
            if (Directory.Exists(mapTypeDir) == false)
            {
                Directory.CreateDirectory(mapTypeDir);
            }
            return mapTypeDir;
        }

        public string CreateRowDir(string mapTypeDir, int row)
        {
            string rowDir = mapTypeDir + @"\" + row.ToString("0000");
            if (Directory.Exists(rowDir) == false)
            {
                Directory.CreateDirectory(rowDir);
            }
            return rowDir;
        }

        public string GetTextureName(string rowDir, int row, int col, string textureExtension)
        {
            string textureName = rowDir + @"\" + row.ToString("0000") + "_" + col.ToString("0000") + "." + textureExtension;
            return textureName;
        }

        private static double DegToRad(double d)
        {
            return d * Math.PI / 180.0;
        }

        private static double RadToDeg(double d)
        {
            return d * 180 / Math.PI;
        }


        private double MetersPerTile(int zoom)
        {
            return MetersPerPixel(zoom) * pixelsPerTile;
        }

        private double MetersPerPixel(int zoom)
        {
            double arc;
            arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            return arc;
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
	}
}
