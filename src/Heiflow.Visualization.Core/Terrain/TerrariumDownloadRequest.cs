using Heiflow.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace HUST.WREIS.Dot3D.Terrain
{
    /// <summary>
    /// Downloads the tiles of a <see cref="TerrariumTerrainTileService"/> that cover one terrain tile,
    /// reads the height out of their pixels and writes it to the terrain cache as the int16 grid that
    /// the rest of the terrain code expects.
    ///
    /// A terrain tile is 20/2^level degrees wide and holds SamplesPerTile x SamplesPerTile heights, while
    /// a tile of the server is a fixed 256 x 256 pixels of the web mercator grid, so the tiles that cover
    /// the terrain tile are fetched and sampled at the centre of every cell of the grid.
    /// </summary>
    public class TerrariumDownloadRequest : TerrainDownloadRequest
    {
        private const int TilePixels = 256;
        /// <summary>Most tiles one terrain tile may be built from, a guard against a wrong offset.</summary>
        private const int MaxTilesPerTerrainTile = 9;
        /// <summary>Whether the refusal of the https handshake has been written to the log already.</summary>
        private static bool tlsFailureReported;

        /// <summary>
        /// The tiles are served over https and the server refuses anything below TLS 1.2, while the
        /// protocol a request offers defaults to SSL 3 and TLS 1.0 on a 4.5 runtime. Without this switch
        /// every request dies in the handshake with "The request was aborted: Could not create SSL/TLS
        /// secure channel". It is added to whatever is set already, so an older protocol another download
        /// needs stays turned on. It is set here rather than at start up because no tile can be fetched
        /// before this class is loaded, and it is done once, not per request.
        /// </summary>
        static TerrariumDownloadRequest()
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }
            catch (Exception caught)
            {
                Log.Write(Log.Levels.Warning, "TERR",
                    "TLS 1.2 could not be turned on, https tiles may fail to load: " + caught.Message);
            }
        }

        public TerrariumDownloadRequest(TerrainTile tile, TerrainTileService owner, int row, int col, int targetLevel)
            : base(tile, owner, row, col, targetLevel)
        {
        }

        /// <summary>
        /// Builds the grid of the tile and puts it in the cache. Terrain tiles are downloaded in the
        /// foreground by AddToDownloadQueue, so this runs on the thread that asked for the elevation.
        /// </summary>
        public override void StartDownload()
        {
            var service = Owner as TerrariumTerrainTileService;
            if (service == null)
                return;

            try
            {
                var elevation = BuildElevation(service);
                if (elevation == null)
                    return;

                // The cache is read back as int16 by SQliteTerrainStorageService, two bytes per sample,
                // the lowest byte first, which is what BinaryWriter would have written too.
                var bytes = new byte[elevation.Length * 2];
                for (int i = 0; i < elevation.Length; i++)
                {
                    bytes[2 * i] = (byte)(elevation[i] & 0xff);
                    bytes[2 * i + 1] = (byte)((elevation[i] >> 8) & 0xff);
                }
                // Owner of the base class is an object, it has to be cast to reach the storage.
                service.TerrainStorageService.SaveTerrain(TerrainTile, new MemoryStream(bytes));
            }
            catch (Exception caught)
            {
                Log.Write(Log.Levels.Error, "TERR", string.Format(CultureInfo.InvariantCulture,
                    "The elevation of the terrain tile L{0} {1}x{2} was not read: {3}",
                    TerrainTile.TargetLevel, TerrainTile.Row, TerrainTile.Col, caught.Message));
            }
        }

        /// <summary>
        /// Returns the heights of the grid of the terrain tile, or null when not a single tile of the
        /// server could be read, in which case nothing is written to the cache.
        /// </summary>
        private short[] BuildElevation(TerrariumTerrainTileService service)
        {
            int zoom = Math.Min(TerrainTile.TargetLevel + service.ZoomOffset, service.MaxZoom);
            int count = 1 << zoom;

            int x0 = (int)Math.Floor(GlobalX(TerrainTile.West, count) / TilePixels);
            int x1 = (int)Math.Floor(GlobalX(TerrainTile.East, count) / TilePixels);
            int y0 = (int)Math.Floor(GlobalY(TerrainTile.North, count) / TilePixels);
            int y1 = (int)Math.Floor(GlobalY(TerrainTile.South, count) / TilePixels);
            // An eastern or southern bound that falls on the first pixel of the next tile is not on it.
            if (x1 > x0 && GlobalX(TerrainTile.East, count) == x1 * TilePixels)
                x1--;
            if (y1 > y0 && GlobalY(TerrainTile.South, count) == y1 * TilePixels)
                y1--;

            var tiles = new Dictionary<long, TileHeights>();
            for (int y = y0; y <= y1 && tiles.Count < MaxTilesPerTerrainTile; y++)
            {
                if (y < 0 || y >= count)
                    continue;
                for (int x = x0; x <= x1 && tiles.Count < MaxTilesPerTerrainTile; x++)
                {
                    var tile = LoadTile(service.ServerUrl, zoom, x, y, count);
                    if (tile != null)
                        tiles[Key(x, y)] = tile;
                }
            }
            if (tiles.Count == 0)
                return null;

            int samples = TerrainTile.SamplesPerTile;
            var elevation = new short[samples * samples];
            double spanLon = (TerrainTile.East - TerrainTile.West) / samples;
            double spanLat = (TerrainTile.North - TerrainTile.South) / samples;

            for (int row = 0; row < samples; row++)
            {
                // The grid runs from north to south, which is how TerrainTile.GetElevationAt reads it.
                double gy = GlobalY(TerrainTile.North - (row + 0.5) * spanLat, count);
                for (int col = 0; col < samples; col++)
                {
                    double gx = GlobalX(TerrainTile.West + (col + 0.5) * spanLon, count);
                    elevation[row * samples + col] = ToInt16(Sample(tiles, gx, gy));
                }
            }
            return elevation;
        }

        /// <summary>Reads one tile of the server, or null when the server has none at that place.</summary>
        private static TileHeights LoadTile(string urlTemplate, int zoom, int x, int y, int count)
        {
            // A terrain tile at the edge of the world wraps around in the longitude only.
            int tx = ((x % count) + count) % count;
            var url = string.Format(CultureInfo.InvariantCulture, urlTemplate, zoom, tx, y);
            try
            {
                byte[] bytes;
                using (var client = new WebClient())
                {
                    // The proxy the user filled in for the rest of the globe is honoured here too.
                    if (!string.IsNullOrEmpty(Net.WebDownload.proxyUrl))
                        client.Proxy = new WebProxy(Net.WebDownload.proxyUrl);
                    bytes = client.DownloadData(url);
                }
                using (var stream = new MemoryStream(bytes))
                using (var bitmap = new Bitmap(stream))
                {
                    return Decode(bitmap);
                }
            }
            catch (WebException caught)
            {
                var response = caught.Response as HttpWebResponse;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    return null;
                if (caught.Status == WebExceptionStatus.SecureChannelFailure && !tlsFailureReported)
                {
                    // Said once: there is one tile and more behind it, and the same sentence for every one
                    // of them would fill the log.
                    tlsFailureReported = true;
                    Log.Write(Log.Levels.Warning, "TERR", string.Format(CultureInfo.InvariantCulture,
                        "The https connection to {0} was refused. The tile server asks for TLS 1.2, which the machine has to allow as well: below Windows 10, or before .NET 4.6 with the strong crypto key unset, no elevation can be fetched.",
                        url));
                }
                throw;
            }
        }

        /// <summary>Height in metres of the pixel of a loaded tile that a point of the grid falls on.</summary>
        private static float Sample(Dictionary<long, TileHeights> tiles, double gx, double gy)
        {
            TileHeights tile;
            if (!tiles.TryGetValue(Key((int)Math.Floor(gx / TilePixels), (int)Math.Floor(gy / TilePixels)), out tile))
                return 0;

            int px = (int)(gx - Math.Floor(gx / TilePixels) * TilePixels);
            int py = (int)(gy - Math.Floor(gy / TilePixels) * TilePixels);
            if (px < 0) px = 0;
            if (py < 0) py = 0;
            if (px >= tile.Size) px = tile.Size - 1;
            if (py >= tile.Size) py = tile.Size - 1;
            return tile.Heights[py * tile.Size + px];
        }

        /// <summary>Reads the heights out of the pixels of a tile of the server.</summary>
        private static TileHeights Decode(Bitmap bitmap)
        {
            var tile = new TileHeights
            {
                Size = bitmap.Width,
                Heights = new float[bitmap.Width * bitmap.Height]
            };

            int depth = Image.GetPixelFormatSize(bitmap.PixelFormat);
            if (depth != 24 && depth != 32)
            {
                // A palette or a 64 bit tile is read pixel by pixel, it is not worth the bytes.
                for (int y = 0; y < bitmap.Height; y++)
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        var colour = bitmap.GetPixel(x, y);
                        tile.Heights[y * bitmap.Width + x] = Height(colour.R, colour.G, colour.B);
                    }
                return tile;
            }

            var area = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var locked = bitmap.LockBits(area, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            try
            {
                var buffer = new byte[Math.Abs(locked.Stride) * locked.Height];
                Marshal.Copy(locked.Scan0, buffer, 0, buffer.Length);
                int bytesPerPixel = depth / 8;
                for (int y = 0; y < locked.Height; y++)
                {
                    int row = y * locked.Stride;
                    for (int x = 0; x < locked.Width; x++)
                    {
                        int i = row + x * bytesPerPixel;
                        // The bytes of a GDI+ bitmap are blue, green, red.
                        tile.Heights[y * bitmap.Width + x] = Height(buffer[i + 2], buffer[i + 1], buffer[i]);
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(locked);
            }
            return tile;
        }

        /// <summary>
        /// Height of a pixel of a terrarium tile: the three bytes are the metres above the sea, biased by
        /// 32768 so that the depth of the sea is kept too.
        /// </summary>
        private static float Height(byte red, byte green, byte blue)
        {
            return (red * 256.0f + green + blue / 256.0f) - 32768.0f;
        }

        private static short ToInt16(float height)
        {
            if (height < short.MinValue)
                return short.MinValue;
            if (height > short.MaxValue)
                return short.MaxValue;
            return (short)Math.Round(height);
        }

        /// <summary>Pixel of the whole world at that longitude, at a zoom of count tiles per side.</summary>
        private static double GlobalX(double longitude, int count)
        {
            return (longitude + 180.0) / 360.0 * TilePixels * count;
        }

        /// <summary>Pixel of the whole world at that latitude, at a zoom of count tiles per side.</summary>
        private static double GlobalY(double latitude, int count)
        {
            // Mercator stops at the poles, everything above it is the last row.
            double lat = Math.Max(Math.Min(latitude, 85.05112878), -85.05112878);
            double rad = lat * Math.PI / 180.0;
            return (1.0 - Math.Log(Math.Tan(rad) + 1.0 / Math.Cos(rad)) / Math.PI) / 2.0 * TilePixels * count;
        }

        private static long Key(int x, int y)
        {
            return ((long)x << 32) | (uint)y;
        }

        private class TileHeights
        {
            public float[] Heights;
            public int Size;
        }
    }
}
