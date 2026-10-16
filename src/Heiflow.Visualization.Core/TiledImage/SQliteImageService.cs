using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.IO;
using System.Drawing;
using System.Collections;
using HUST.WREIS.Dot3D.Renderable;
using Heiflow.Spatial;

namespace HUST.WREIS.Dot3D
{
    public interface IImageStorageService
    {
        Microsoft.DirectX.Direct3D.Texture LoadFile(Renderable.QuadTile qt);

        /// <summary>
        /// Is the image for this tile there, and if it is not, has a download been queued for it?
        /// This one must not touch the DirectX device: it is called from the update thread, while
        /// LoadFile is only called from the thread that owns the device.
        /// </summary>
        bool IsImageAvailable(Renderable.QuadTile qt);

        void SaveImage(MemoryStream ms, QuadTile qt);
    }

    public class SQLiteImageService:IImageStorageService
    {
       
        /// <summary>
        /// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.ImageTileService"/> class.
        /// </summary>
        /// <param name="dataSetName"></param>
        /// <param name="serverUri"></param>
        public SQLiteImageService(ImageStore parent, string dbPath, int imagetype)
        {
            mDbPath = dbPath;
            mImageStore = parent;
            imageType = imagetype;
            Heiflow.Spatial.Internals.Cache.Instance.CacheLocation = ConfigurationManager.Engine3DSettings.DataPath;
            mCache = TileCacheDb.Get(dbPath);
        }

        private string mDbPath;
        private ImageStore mImageStore;
        private int imageType = 0;
        private PureImageCache mCache;

        public void SaveImage(MemoryStream ms, QuadTile qt)
        {
            if (ms != null)
            {
                GPoint pt = new GPoint(qt.Row, qt.Col);
                if(!mCache.Exists(imageType,pt,qt.Level))
                    mCache.PutImageToCache(ms.ToArray(), imageType, pt, qt.Level);
            }
        }

        /// <summary>
        /// Reads a tile that was cached as a file before the layer was switched over to a database,
        /// copies it into the database on the way and returns its texture, or null when there is no
        /// such file. Without it, switching the cache over would throw away everything the layer has
        /// already downloaded and fetch it again.
        /// </summary>
        private Texture LoadFromCachedFile(QuadTile qt)
        {
            if (string.IsNullOrEmpty(qt.ImageFilePath) || !File.Exists(qt.ImageFilePath))
                return null;

            byte[] bytes = File.ReadAllBytes(qt.ImageFilePath);
            if (bytes.Length == 0)
                return null;

            GPoint pt = new GPoint(qt.Row, qt.Col);
            if (!mCache.Exists(imageType, pt, qt.Level))
                mCache.PutImageToCache(bytes, imageType, pt, qt.Level);

            return ImageHelper.LoadTexture(new MemoryStream(bytes));
        }

        public bool IsImageAvailable(Renderable.QuadTile qt)
        {
            string filePath = mImageStore.GetLocalPath(qt);
            qt.ImageFilePath = filePath;

            if (World.Settings.UsePseudoColor || mImageStore.IsDebug == true)
                return true;

            GPoint pt = new GPoint(qt.Row, qt.Col);
            if (mCache.Exists(imageType, pt, qt.Level))
                return true;

            // A tile that is on disk from the days this layer cached to files is used as it is, see
            // LoadFromCachedFile, so no download is queued for it either.
            if (File.Exists(filePath))
                return true;

            mImageStore.QueueDownload(qt, "");
            return false;
        }

        public Microsoft.DirectX.Direct3D.Texture LoadFile(Renderable.QuadTile qt)
        {
            Texture texture = null;
            string filePath = mImageStore.GetLocalPath(qt);
            qt.ImageFilePath = filePath;

            if(World.Settings.UsePseudoColor)
                texture = ImageHelper.LoadTexture(qt.ImageFilePath);
            else
            {
                if (mImageStore.IsDebug == true)
                {
                    return mImageStore.GenerateDebugTexture(qt);
                }
                else
                {
                    // Use color key
                    GPoint pt = new GPoint(qt.Row, qt.Col);

                    if (qt.QuadTileSet.HasTransparentRange)
                    {
                        PureImage pimage = mCache.GetImageFromCache(imageType, pt, qt.Level);
                        if (pimage != null)
                        {
                            texture = ImageHelper.LoadTexture(pimage.Data);
                        }
                        else if ((texture = LoadFromCachedFile(qt)) == null)
                        {
                            mImageStore.QueueDownload(qt, filePath);
                        }
                    }
                    else
                    {
                        MemoryStream pimage = mCache.GetImageMemoryStream(imageType, pt, qt.Level);
                        if (pimage != null)
                        {
                            texture = ImageHelper.LoadTexture(pimage);
                        }
                        else if ((texture = LoadFromCachedFile(qt)) == null)
                        {
                            mImageStore.QueueDownload(qt, "");
                        }
                    }

                    //if (texture == null)
                    //    texture = mImageStore.GenerateDebugTexture(qt);

                }
            }
            return texture;
        }
    }
}
