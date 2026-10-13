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
            Heiflow.Spatial.CacheProviders.SQLitePureImageCache imgcache = new Heiflow.Spatial.CacheProviders.SQLitePureImageCache()
                  {
                      CacheFileName = dbPath
                  };
            Heiflow.Spatial.Internals.Cache.Instance.ImageCache = imgcache;
        }

        private string mDbPath;
        private ImageStore mImageStore;
        private int imageType = 0;

        public void SaveImage(MemoryStream ms, QuadTile qt)
        {
            if (ms != null)
            {
                GPoint pt = new GPoint(qt.Row, qt.Col);
                if(!Heiflow.Spatial.Internals.Cache.Instance.ImageCache.Exists(imageType,pt,qt.Level))
                    Heiflow.Spatial.Internals.Cache.Instance.ImageCache.PutImageToCache(ms.ToArray(), imageType, pt, qt.Level);
            }
        }

        public bool IsImageAvailable(Renderable.QuadTile qt)
        {
            string filePath = mImageStore.GetLocalPath(qt);
            qt.ImageFilePath = filePath;

            if (World.Settings.UsePseudoColor || mImageStore.IsDebug == true)
                return true;

            GPoint pt = new GPoint(qt.Row, qt.Col);
            if (Heiflow.Spatial.Internals.Cache.Instance.ImageCache.Exists(imageType, pt, qt.Level))
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

                    // see class LayerProfiler in World.cs, remove both when the profiling is done
                    bool profiled = LayerProfiler.On;
                    long cacheStart = profiled ? Stopwatch.GetTimestamp() : 0;

                    if (qt.QuadTileSet.HasTransparentRange)
                    {
                        PureImage pimage = Heiflow.Spatial.Internals.Cache.Instance.ImageCache.GetImageFromCache(imageType, pt, qt.Level);
                        if (profiled)
                            LayerProfiler.RecordUpdate("tile: sqlite cache read", Stopwatch.GetTimestamp() - cacheStart);

                        if (pimage != null)
                        {
                            long textureStart = profiled ? Stopwatch.GetTimestamp() : 0;
                            texture = ImageHelper.LoadTexture(pimage.Data);
                            if (profiled)
                                LayerProfiler.RecordUpdate("tile: texture from bytes", Stopwatch.GetTimestamp() - textureStart);
                        }
                        else
                        {
                            if (profiled)
                                LayerProfiler.RecordUpdate("tile: queued download", 0);
                            mImageStore.QueueDownload(qt, filePath);
                        }
                    }
                    else
                    {
                        MemoryStream pimage = Heiflow.Spatial.Internals.Cache.Instance.ImageCache.GetImageMemoryStream(imageType, pt, qt.Level);
                        if (profiled)
                            LayerProfiler.RecordUpdate("tile: sqlite cache read", Stopwatch.GetTimestamp() - cacheStart);

                        if (pimage != null)
                        {
                            long textureStart = profiled ? Stopwatch.GetTimestamp() : 0;
                            texture = ImageHelper.LoadTexture(pimage);
                            if (profiled)
                                LayerProfiler.RecordUpdate("tile: texture from bytes", Stopwatch.GetTimestamp() - textureStart);
                        }
                        else
                        {
                            if (profiled)
                                LayerProfiler.RecordUpdate("tile: queued download", 0);
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
