using System;
using System.Collections.Generic;
using System.IO;
using Heiflow.Spatial;
using Heiflow.Spatial.CacheProviders;
using Heiflow.Spatial.Internals;

namespace HUST.WREIS.Dot3D
{
    /// <summary>
    /// One SQLite cache per database file, shared by the tile services that use that file.
    /// </summary>
    /// <remarks>
    /// Heiflow.Spatial.Internals.Cache.Instance.ImageCache is a single field for the whole process, and
    /// every SQLite service used to put its own cache there, so the layer built last won and all the
    /// other layers read their tiles out of the database of that one layer. That is harmless while
    /// every layer names the same database, which is what Layers.xml used to do, but not once a layer
    /// is allowed a database of its own. Keeping one instance per file fixes it; the singleton is
    /// pointed at the first one so that code going through it still finds a cache.
    /// </remarks>
    public static class TileCacheDb
    {
        private static readonly Dictionary<string, PureImageCache> Caches =
            new Dictionary<string, PureImageCache>(StringComparer.OrdinalIgnoreCase);

        private static readonly object Sync = new object();

        /// <summary>
        /// The cache for this database file, opened and created if it is not there yet.
        /// </summary>
        public static PureImageCache Get(string dbPath)
        {
            lock (Sync)
            {
                PureImageCache cache;
                if (!Caches.TryGetValue(dbPath, out cache))
                {
                    // SQLitePureImageCache creates the database when it is given the file name, and
                    // that fails when the directory is not there, which it is not for a layer whose
                    // tiles have never been cached before.
                    string directory = Path.GetDirectoryName(dbPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    cache = new SQLitePureImageCache
                    {
                        CacheFileName = dbPath
                    };
                    Caches.Add(dbPath, cache);

                    if (Heiflow.Spatial.Internals.Cache.Instance.ImageCache == null)
                        Heiflow.Spatial.Internals.Cache.Instance.ImageCache = cache;
                }
                return cache;
            }
        }
    }
}
