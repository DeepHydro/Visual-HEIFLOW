using System;
using System.Collections.Generic;
using System.IO;
using Heiflow.Spatial;
using HUST.WREIS.Dot3D.Configuration;

namespace HUST.WREIS.Dot3D
{
    /// <summary>
    /// The one cache there is to manage: the database every layer keeps its tiles in, see
    /// World.Settings.TileCacheDbPath, and what is still on disk from the days the layers cached
    /// their tiles as files, which nothing reads any more once the tiles go into the database.
    /// </summary>
    public static class TileCacheInventory
    {
        /// <summary>The database every layer shares.</summary>
        public static string DatabasePath
        {
            get { return ConfigurationManager.TileCacheDbPath(); }
        }

        public static bool DatabaseExists
        {
            get { return File.Exists(DatabasePath); }
        }

        /// <summary>Size of the database, 0 while it has not been created yet.</summary>
        public static long DatabaseSize
        {
            get { return DatabaseExists ? new FileInfo(DatabasePath).Length : 0; }
        }

        /// <summary>Size of the tiles that are still cached as files under the cache path.</summary>
        public static long FileCacheSize
        {
            get
            {
                long bytes = 0;
                foreach (FileInfo file in Files())
                    bytes += file.Length;

                return bytes;
            }
        }

        /// <summary>
        /// Clears the database. The file itself goes when the scene does not have it open; when it
        /// does, only the tiles in it can be deleted, and that is what the result tells apart.
        /// </summary>
        public static ClearResult ClearDatabase()
        {
            ClearResult result = new ClearResult();
            string path = DatabasePath;

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    result.DatabaseDeleted = true;
                    return result;
                }
                catch (Exception caught)
                {
                    result.Problem = caught.Message;
                }
            }

            // Still there, so the running scene has it open. The tiles can go without the file going,
            // but only the ones with a time on them, which is what the tiles of the old cache lack.
            if (File.Exists(path))
                result.RowsDeleted = TileCacheDb.Get(path).DeleteOlderThan(DateTime.MaxValue);

            return result;
        }

        /// <summary>Deletes the tiles that are still cached as files, and names what would not go.</summary>
        public static int ClearFileCache(List<string> failures)
        {
            int deleted = 0;
            foreach (FileInfo file in Files())
            {
                try
                {
                    File.Delete(file.FullName);
                    deleted++;
                }
                catch (Exception caught)
                {
                    failures.Add(file.FullName + ": " + caught.Message);
                }
            }

            return deleted;
        }

        /// <summary>
        /// Every tile file under the cache path. A database in there is left alone: it is the shared
        /// one or one that was left behind, and it is managed on its own.
        /// </summary>
        static IEnumerable<FileInfo> Files()
        {
            string cachePath = ConfigurationManager.Engine3DSettings.CachePath;
            if (!Directory.Exists(cachePath))
                return new List<FileInfo>();

            List<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in new DirectoryInfo(cachePath).EnumerateFiles("*", SearchOption.AllDirectories))
            {
                if (!file.Name.EndsWith(".gmdb", StringComparison.OrdinalIgnoreCase))
                    files.Add(file);
            }

            return files;
        }
    }

    /// <summary>What clearing the database managed to do, see TileCacheInventory.ClearDatabase.</summary>
    public class ClearResult
    {
        /// <summary>The file itself is gone, and is created again empty when it is needed.</summary>
        public bool DatabaseDeleted { get; set; }

        /// <summary>How many tiles were deleted out of a database that is still open.</summary>
        public int RowsDeleted { get; set; }

        /// <summary>Why the file could not be deleted.</summary>
        public string Problem { get; set; }
    }
}
