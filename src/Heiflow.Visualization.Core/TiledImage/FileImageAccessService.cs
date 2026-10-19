using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using HUST.WREIS.Dot3D.Renderable;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

namespace HUST.WREIS.Dot3D
{
    public class FileImageAccessService:IImageStorageService
    {
        public FileImageAccessService(ImageStore store)
        {
            mImageStore = store;
        }

        private ImageStore mImageStore;

        public void SaveImage(MemoryStream ms, QuadTile qt)
        {
        }

        /// <summary>
        /// Called from the update thread: finds out whether the image is on disk and queues a
        /// download when it is not. It must not create a texture, because every device call made
        /// from the update thread is serviced by the thread that owns the device, which is the
        /// thread the render loop runs on, and a burst of tile downloads then blocked rendering
        /// for hundreds of milliseconds. LoadFile does the texture, on the device thread.
        /// </summary>
        public bool IsImageAvailable(Renderable.QuadTile qt)
        {
            if (mImageStore.IsDebug == true)
                return true;

            string filePath = mImageStore.GetLocalPath(qt);
            if (!File.Exists(filePath))
            {
                string badFlag = filePath + ".txt";
                if (File.Exists(badFlag))
                {
                    FileInfo fi = new FileInfo(badFlag);
                    if (DateTime.Now - fi.LastWriteTime < TimeSpan.FromDays(1))
                    {
                        return false;
                    }
                    // Timeout period elapsed, retry
                    File.Delete(badFlag);
                }

                if (mImageStore.IsDownloadableLayer)
                {
                    mImageStore.QueueDownload(qt, filePath);
                    return false;
                }

                if (mImageStore.DuplicateTexturePath == null)
                    // No image available, neither local nor online.
                    return false;

                filePath = mImageStore.DuplicateTexturePath;
            }

            qt.ImageFilePath = filePath;
            return true;
        }

        public Texture LoadFile(Renderable.QuadTile qt)
        {
            if (mImageStore.IsDebug == true)
            {
                return mImageStore.GenerateDebugTexture(qt);
            }
            else
            {
                if (!IsImageAvailable(qt))
                    return null;

                string filePath = qt.ImageFilePath;

                // Use color key
                Texture texture = null;

                if (qt.QuadTileSet.HasTransparentRange)
                {
                    texture = ImageHelper.LoadTexture(filePath, qt.QuadTileSet.ColorKey, qt.QuadTileSet.ColorKeyMax);
                }
                else
                {
                    texture = ImageHelper.LoadTexture(filePath, qt.QuadTileSet.ColorKey);
                }

                if (qt.QuadTileSet.CacheExpirationTime != TimeSpan.MaxValue)
                {
                    FileInfo fi = new FileInfo(filePath);
                    DateTime expiry = fi.LastWriteTimeUtc.Add(qt.QuadTileSet.CacheExpirationTime);
                    if (DateTime.UtcNow > expiry)
                        mImageStore.QueueDownload(qt, filePath);
                }

                // Only convert images that are downloadable (don't mess with things the user put here!)
                if (World.Settings.ConvertDownloadedImagesToDds && mImageStore.IsDownloadableLayer)
                    mImageStore.ConvertImage(texture, filePath);

                return texture;
            }
        }
    }
}
