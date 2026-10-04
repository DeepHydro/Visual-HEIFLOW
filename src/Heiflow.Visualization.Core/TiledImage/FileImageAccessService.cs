using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.DirectX.Direct3D;
using HUST.WREIS.Dot3D.Renderable;

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

        public Microsoft.DirectX.Direct3D.Texture LoadFile(Renderable.QuadTile qt)
        {
            if (mImageStore.IsDebug == true)
            {
                return mImageStore.GenerateDebugTexture(qt);
            }
            else
            {
                string filePath = mImageStore.GetLocalPath(qt);
                qt.ImageFilePath = filePath;
                if (!File.Exists(filePath))
                {
                    string badFlag = filePath + ".txt";
                    if (File.Exists(badFlag))
                    {
                        FileInfo fi = new FileInfo(badFlag);
                        if (DateTime.Now - fi.LastWriteTime < TimeSpan.FromDays(1))
                        {
                            return null;
                        }
                        // Timeout period elapsed, retry
                        File.Delete(badFlag);
                    }

                    if (mImageStore.IsDownloadableLayer)
                    {
                        mImageStore.QueueDownload(qt, filePath);
                        return null;
                    }

                    if (mImageStore.DuplicateTexturePath == null)
                        // No image available, neither local nor online.
                        return null;

                    filePath = mImageStore.DuplicateTexturePath;
                }

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
