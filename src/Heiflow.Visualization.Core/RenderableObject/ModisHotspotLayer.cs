using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;

namespace HUST.WREIS.Dot3D.Renderable
{
    /// <summary>
    /// NASA FIRMS 发布的一条 MODIS 活跃火点记录。
    /// </summary>
    public class ModisHotspot
    {
        public double Latitude;
        public double Longitude;
        public double Brightness;
        public double Frp;
        public int Confidence;
        public string AcquiredDate;
        public string AcquiredTime;
        public bool IsNight;
    }

    /// <summary>
    /// 把 NASA FIRMS 的 MODIS 活跃火点做成一个 DownloadableImageFromIconSet 图层。
    /// 数据分两级：图标位置来自 FIRMS 的真实火点（联网抓取一次）；
    /// 而每个图标对应的卫星影像 URL 指向 NASA GIBS，只有在用户点击图标时才按需下载。
    /// 这样避免了一次性拉取上万张影像。
    /// </summary>
    public static class ModisHotspotLayer
    {
        public const string LayerName = "MODIS Active Fires";

        /// <summary>
        /// NASA FIRMS：全球最近 24 小时的 MODIS 活跃火点（CSV，公开数据，无需 token）。
        /// 旧版 WorldWind 用的 Rapid Fire subsets 接口（lance-modis/imagery/subsets）已下线，故改用 FIRMS。
        /// </summary>
        public const string FirmsCsvUrl =
            "https://firms.modaps.eosdis.nasa.gov/data/active_fire/modis-c6.1/csv/MODIS_C6_1_Global_24h.csv";

        private const string GibsWmsBase = "https://gibs.earthdata.nasa.gov/wms/epsg4326/best/wms.cgi";
        private const string GibsLayer = "MODIS_Terra_CorrectedReflectance_TrueColor";

        /// <summary>单个火点的取图半跨度（度）。</summary>
        public const double DefaultHalfSpan = 2.0;

        /// <summary>
        /// 影像边长（像素）。取 512（2 的幂）是因为影像最终要转成 DXT1，
        /// 非 4 的倍数尺寸会导致转换失败。
        /// </summary>
        public const int DefaultImageSize = 512;

        /// <summary>
        /// 下载并解析 FIRMS 火点列表，按置信度从高到低排序，便于调用方只取最强的若干个。
        /// </summary>
        public static List<ModisHotspot> FetchHotspots(int timeoutMs = 60000)
        {
            var result = new List<ModisHotspot>();

            var request = (HttpWebRequest)WebRequest.Create(FirmsCsvUrl);
            request.Timeout = timeoutMs;
            request.UserAgent = "Visual-HEIFLOW";

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string header = reader.ReadLine();
                if (string.IsNullOrEmpty(header))
                    return result;

                // 按列名定位，避免列顺序变动导致解析错位
                string[] columns = header.Split(',');
                int iLat = IndexOf(columns, "latitude");
                int iLon = IndexOf(columns, "longitude");
                int iBright = IndexOf(columns, "brightness");
                int iFrp = IndexOf(columns, "frp");
                int iConf = IndexOf(columns, "confidence");
                int iDate = IndexOf(columns, "acq_date");
                int iTime = IndexOf(columns, "acq_time");
                int iDayNight = IndexOf(columns, "daynight");

                if (iLat < 0 || iLon < 0)
                    throw new InvalidDataException("FIRMS CSV 中未找到 latitude/longitude 列，接口格式可能已变化。");

                int maxIndex = Math.Max(Math.Max(iLat, iLon), Math.Max(iDate, iDayNight));

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] f = line.Split(',');
                    if (f.Length <= maxIndex)
                        continue;

                    double lat, lon;
                    if (!double.TryParse(f[iLat], NumberStyles.Float, CultureInfo.InvariantCulture, out lat))
                        continue;
                    if (!double.TryParse(f[iLon], NumberStyles.Float, CultureInfo.InvariantCulture, out lon))
                        continue;

                    var spot = new ModisHotspot();
                    spot.Latitude = lat;
                    spot.Longitude = lon;
                    spot.Brightness = ParseDouble(f, iBright);
                    spot.Frp = ParseDouble(f, iFrp);
                    spot.Confidence = ParseInt(f, iConf);
                    spot.AcquiredDate = Field(f, iDate);
                    spot.AcquiredTime = Field(f, iTime);
                    spot.IsNight = string.Equals(Field(f, iDayNight), "N", StringComparison.OrdinalIgnoreCase);
                    result.Add(spot);
                }
            }

            result.Sort(delegate(ModisHotspot a, ModisHotspot b) { return b.Confidence.CompareTo(a.Confidence); });
            return result;
        }

        /// <summary>
        /// 依据火点列表构建图标集图层。调用方需自行把返回的图层加入 World.RenderableObjects。
        /// </summary>
        public static DownloadableImageFromIconSet Build(
            World world,
            DrawArgs drawArgs,
            IList<ModisHotspot> hotspots,
            string iconFilePath,
            string cacheDirectory,
            int maxIcons = 300,
            double halfSpan = DefaultHalfSpan,
            int imageSize = DefaultImageSize)
        {
            if (world == null) throw new ArgumentNullException("world");
            if (drawArgs == null) throw new ArgumentNullException("drawArgs");
            if (hotspots == null || hotspots.Count == 0)
                throw new ArgumentException("没有可用的 MODIS 火点数据。", "hotspots");

            var layer = new DownloadableImageFromIconSet(LayerName, world, 0f, drawArgs, world.TerrainAccessor);
            layer.RenderPriority = RenderPriority.Icons;

            int count = 0;
            foreach (ModisHotspot spot in hotspots)
            {
                if (count >= maxIcons)
                    break;

                double west = Math.Max(-180.0, spot.Longitude - halfSpan);
                double east = Math.Min(180.0, spot.Longitude + halfSpan);
                double south = Math.Max(-90.0, spot.Latitude - halfSpan);
                double north = Math.Min(90.0, spot.Latitude + halfSpan);

                string name = string.Format(CultureInfo.InvariantCulture, "MODIS Fire {0}", count);
                string savePath = Path.Combine(cacheDirectory, "MODISFires",
                    string.Format(CultureInfo.InvariantCulture, "fire{0}.dds", count));
                string imageUrl = BuildImageUrl(west, south, east, north, spot.AcquiredDate, imageSize);
                string caption = string.Format(CultureInfo.InvariantCulture,
                    "MODIS fire  lat {0:0.###}  lon {1:0.###}  confidence {2}%  {3} {4}",
                    spot.Latitude, spot.Longitude, spot.Confidence, spot.AcquiredDate, spot.AcquiredTime);

                layer.AddDownloadableIcon(name, 0f, (float)west, (float)south, (float)east, (float)north,
                    imageUrl, savePath, iconFilePath, 32, caption);
                count++;
            }

            // 图层是在 World 初始化之后才加入的，不会被自动初始化；
            // DownloadableImageFromIconSet.Render 首行会检查 isInitialized，故必须手动调用。
            layer.Initialize(drawArgs);
            return layer;
        }

        /// <summary>
        /// 生成 NASA GIBS 的取图地址。
        /// </summary>
        public static string BuildImageUrl(double west, double south, double east, double north, string date, int imageSize)
        {
            // WMS 1.3.0 + EPSG:4326 的 BBOX 轴序是 纬度,经度（与常见的 x,y 相反）
            return string.Format(CultureInfo.InvariantCulture,
                "{0}?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&LAYERS={1}&CRS=EPSG:4326&BBOX={2},{3},{4},{5}&WIDTH={6}&HEIGHT={7}&FORMAT=image/jpeg&TIME={8}",
                GibsWmsBase, GibsLayer,
                south, west, north, east,
                imageSize, imageSize,
                string.IsNullOrEmpty(date)
                    ? DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : date);
        }

        private static int IndexOf(string[] columns, string name)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (string.Equals(columns[i].Trim(), name, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        private static string Field(string[] f, int index)
        {
            if (index < 0 || index >= f.Length) return string.Empty;
            return f[index].Trim();
        }

        private static double ParseDouble(string[] f, int index)
        {
            double value;
            string s = Field(f, index);
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return value;
            return 0.0;
        }

        private static int ParseInt(string[] f, int index)
        {
            int value;
            string s = Field(f, index);
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                return value;
            return 0;
        }
    }
}
