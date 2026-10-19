using DotSpatial.Data;
using DotSpatial.Projections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Heiflow.Visualization.Renderable
{
    /// <summary>
    /// A raster file read out into the two things a layer of the scene is made of: a bitmap file that
    /// ImageLayer draws as a texture, and the patch of ground, given in degrees of latitude and
    /// longitude, that the bitmap covers. A raster drawn without knowing where it lies would cover the
    /// whole planet or none of it, so a file whose position cannot be found is not drawn at all.
    ///
    /// Two kinds of file come through here:
    ///
    ///   * a raster of numbers, a GeoTIFF of elevations or of land use among them, read through
    ///     DotSpatial. Its numbers have to be turned into colours first - this is what ReadValueRaster
    ///     does - and where the file lies is written in the file itself, together with the coordinate
    ///     system the numbers are in.
    ///
    ///   * a picture, a satellite image or a scanned map among them, whose own pixels are the colours
    ///     to draw, and whose position is not in the picture but in the small world file that goes with
    ///     it, see ReadPicture and ReadWorldFile.
    ///
    /// Both end up as a bitmap in the raster cache of this application, and as the four sides of the
    /// ground they belong to.
    /// </summary>
    public sealed class RasterDrawing
    {
        /// <summary>The bitmap file the layer reads its texture out of.</summary>
        public string BitmapFile { get; private set; }

        public double West { get; private set; }

        public double East { get; private set; }

        public double South { get; private set; }

        public double North { get; private set; }

        /// <summary>
        /// Reads the file, whichever of the two kinds it is. Null comes back, with the reason written
        /// into whyNot, when the file cannot be drawn.
        /// </summary>
        public static RasterDrawing Load(string fileName, out string whyNot)
        {
            whyNot = null;
            string rasterFailure = null;
            string pictureFailure = null;

            try
            {
                var fromNumbers = ReadValueRaster(fileName, out rasterFailure);
                if (fromNumbers != null)
                    return fromNumbers;
            }
            catch (Exception ex)
            {
                rasterFailure = ex.Message;
            }

            try
            {
                var fromPicture = ReadPicture(fileName, out pictureFailure);
                if (fromPicture != null)
                    return fromPicture;
            }
            catch (Exception ex)
            {
                pictureFailure = ex.Message;
            }

            whyNot = "It could not be read as a raster of numbers" +
                     (rasterFailure == null ? "" : ": " + rasterFailure) +
                     ", and not as a picture with a world file beside it" +
                     (pictureFailure == null ? "" : ": " + pictureFailure) + ".";
            return null;
        }

        /// <summary>
        /// Reads a raster of numbers: one grid of values, whose band the example of DotSpatial hands
        /// us, stretched from black at the smallest value to white at the largest, the cells that carry
        /// the "no data" value of the file being left transparent so that what lies under them shows
        /// through.
        /// </summary>
        private static RasterDrawing ReadValueRaster(string fileName, out string whyNot)
        {
            whyNot = null;
            using (var raster = Raster.Open(fileName))
            {
                if (raster == null)
                {
                    whyNot = "no reader of the file could be found";
                    return null;
                }

                int rows = raster.NumRows;
                int columns = raster.NumColumns;
                if (rows <= 0 || columns <= 0)
                {
                    whyNot = "the file carries no grid";
                    return null;
                }

                double noData = raster.NoDataValue;
                double lowest, highest;
                ReadRange(raster, rows, columns, noData, out lowest, out highest);
                if (lowest == double.MaxValue)
                {
                    whyNot = "no cell of the grid carries a value";
                    return null;
                }
                if (highest <= lowest)
                {
                    // A file of one value has no spread to colour by, and drawing it as one flat
                    // rectangle of grey would hide what is underneath for no gain.
                    whyNot = "every cell carries the same value";
                    return null;
                }

                var bitmap = PaintValues(raster, rows, columns, noData, lowest, highest);
                return FromBitmap(bitmap, fileName,
                    raster.Extent.MinX, raster.Extent.MaxX, raster.Extent.MinY, raster.Extent.MaxY,
                    raster.Projection, out whyNot);
            }
        }

        /// <summary>
        /// Reads a picture, a satellite image or a scanned map among them, together with the world file
        /// that says where on the globe it lies.
        /// </summary>
        private static RasterDrawing ReadPicture(string fileName, out string whyNot)
        {
            whyNot = null;
            double[] worldFile;
            if (!TryReadWorldFile(fileName, out worldFile))
            {
                whyNot = "there is no world file beside it to say where it lies";
                return null;
            }

            using (var picture = new Bitmap(fileName))
            {
                // A world file holds six numbers: the width and the height of a cell in ground units,
                // how far the grid is turned - nothing here deals with a turned grid, so a grid that is
                // turned is refused - and where the middle of the upper left cell lies. The sides come
                // out of those: move half a cell out from the middle of that first cell.
                if (Math.Abs(worldFile[1]) > 1e-10 || Math.Abs(worldFile[2]) > 1e-10)
                {
                    whyNot = "the grid of the picture is turned, which nothing here draws";
                    return null;
                }

                double west = worldFile[4] - worldFile[0] / 2;
                double north = worldFile[5] - worldFile[3] / 2;
                double east = west + picture.Width * worldFile[0];
                double south = north + picture.Height * worldFile[3];

                return FromBitmap(picture, fileName, west, east, south, north, null, out whyNot);
            }
        }

        /// <summary>
        /// Writes the bitmap into the raster cache of this application, moves its four sides into
        /// degrees of latitude and longitude, and hands back the drawing that comes out of it. The
        /// projection the sides are given in - a world file names none, and its numbers are taken to
        /// already be degrees - decides whether they have to be moved at all.
        /// </summary>
        private static RasterDrawing FromBitmap(Bitmap bitmap, string sourceFileName,
            double west, double east, double south, double north, ProjectionInfo projection, out string whyNot)
        {
            whyNot = null;
            if (projection != null)
            {
                // The corners rather than merely the sides: a box drawn in a projection that is not
                // WGS84 comes out turned, or bent, once it has been moved, so its new sides are taken
                // around what the four corners became.
                var geographic = KnownCoordinateSystems.Geographic.World.WGS1984;
                double[] xy = { west, south, east, south, east, north, west, north };
                double[] z = { 0, 0, 0, 0 };
                Reproject.ReprojectPoints(xy, z, projection, geographic, 0, 4);

                west = Smallest(xy[0], xy[2], xy[4], xy[6]);
                east = Largest(xy[0], xy[2], xy[4], xy[6]);
                south = Smallest(xy[1], xy[3], xy[5], xy[7]);
                north = Largest(xy[1], xy[3], xy[5], xy[7]);
            }

            if (!LooksLikeDegrees(west, east, south, north))
            {
                whyNot = "where it lies, once read, is not a box that can be degrees of latitude and longitude";
                return null;
            }

            return new RasterDrawing
            {
                BitmapFile = WriteBitmap(bitmap, sourceFileName),
                West = west,
                East = east,
                South = south,
                North = north
            };
        }

        /// <summary>
        /// Walks the grid twice, once to find the smallest and the largest value of it, which is what
        /// decides how the values are coloured, and once to colour them.
        /// </summary>
        private static void ReadRange(IRaster raster, int rows, int columns, double noData, out double lowest, out double highest)
        {
            lowest = double.MaxValue;
            highest = double.MinValue;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    double value = raster.Value[row, column];
                    if (double.IsNaN(value) || value == noData)
                        continue;
                    if (value < lowest)
                        lowest = value;
                    if (value > highest)
                        highest = value;
                }
            }
        }

        private static Bitmap PaintValues(IRaster raster, int rows, int columns, double noData, double lowest, double highest)
        {
            double spread = highest - lowest;
            var pixels = new byte[4 * rows * columns];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    int at = 4 * (row * columns + column);
                    double value = raster.Value[row, column];
                    if (double.IsNaN(value) || value == noData)
                    {
                        pixels[at + 3] = 0;
                        continue;
                    }
                    int shade = (int)(255.0 * (value - lowest) / spread);
                    if (shade < 0)
                        shade = 0;
                    if (shade > 255)
                        shade = 255;
                    pixels[at] = (byte)shade;
                    pixels[at + 1] = (byte)shade;
                    pixels[at + 2] = (byte)shade;
                    pixels[at + 3] = 255;
                }
            }

            var bitmap = new Bitmap(columns, rows, PixelFormat.Format32bppArgb);
            var bits = bitmap.LockBits(new Rectangle(0, 0, columns, rows), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                Marshal.Copy(pixels, 0, bits.Scan0, pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bits);
            }
            return bitmap;
        }

        /// <summary>
        /// Puts the bitmap in the raster cache as a PNG, the one picture format a texture of DirectX is
        /// sure to read, whatever shape the file it came out of was in.
        /// </summary>
        private static string WriteBitmap(Bitmap bitmap, string sourceFileName)
        {
            var folder = Path.Combine(Path.GetTempPath(), "HydroEarth", "Rasters");
            Directory.CreateDirectory(folder);
            var name = Path.GetFileNameWithoutExtension(sourceFileName);
            string file;
            int attempts = 0;
            do
            {
                var suffix = attempts == 0 ? "" : "_" + attempts.ToString(CultureInfo.InvariantCulture);
                file = Path.Combine(folder, name + suffix + ".png");
                attempts++;
            }
            while (File.Exists(file) && attempts < 100);

            bitmap.Save(file, ImageFormat.Png);
            return file;
        }

        /// <summary>
        /// Looking for the small text file of six numbers that says where a picture lies. Its name is
        /// made of the picture's own: the first and the last letter of the extension and a w, as in
        /// Neighbourhood.tif beside Neighbourhood.tfw, though .tifw and .wld are written too.
        /// </summary>
        private static bool TryReadWorldFile(string fileName, out double[] worldFile)
        {
            worldFile = null;
            foreach (var candidate in WorldFileNames(fileName))
            {
                if (!File.Exists(candidate))
                    continue;
                var lines = File.ReadAllLines(candidate);
                if (lines.Length < 6)
                    continue;
                var numbers = new double[6];
                for (int i = 0; i < 6; i++)
                {
                    if (!double.TryParse(lines[i], NumberStyles.Float, CultureInfo.InvariantCulture, out numbers[i]))
                        return false;
                }
                worldFile = numbers;
                return true;
            }
            return false;
        }

        private static IEnumerable<string> WorldFileNames(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName) ?? "";
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            if (extension.Length > 1)
            {
                var letters = extension.Substring(1);
                yield return Path.Combine(folder, name + "." + letters[0] + letters[letters.Length - 1] + "w");
                yield return Path.Combine(folder, name + "." + letters + "w");
            }
            yield return Path.Combine(folder, name + ".wld");
        }

        /// <summary>
        /// True for a box that could be degrees: within the reach of longitude and of latitude, and
        /// with its sides on the right ends of each other.
        /// </summary>
        private static bool LooksLikeDegrees(double west, double east, double south, double north)
        {
            return Math.Abs(west) <= 180 && Math.Abs(east) <= 180
                && Math.Abs(south) <= 90 && Math.Abs(north) <= 90
                && east > west && north > south;
        }

        private static double Smallest(params double[] values)
        {
            double smallest = double.MaxValue;
            foreach (var value in values)
            {
                if (value < smallest)
                    smallest = value;
            }
            return smallest;
        }

        private static double Largest(params double[] values)
        {
            double largest = double.MinValue;
            foreach (var value in values)
            {
                if (value > largest)
                    largest = value;
            }
            return largest;
        }
    }
}
