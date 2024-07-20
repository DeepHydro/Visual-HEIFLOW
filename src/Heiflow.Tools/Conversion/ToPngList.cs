//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using ColorPalettes.Colors;
using ColorPalettes.PaletteGeneration;
using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Core.Data.Classification;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using ImageProcessor;
using ImageProcessor.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Conversion
{
    public class ToPngList : MapLayerRequiredTool
    {
        public enum TimeIntevalunits { Second, Hour, Day, Month, Year }
        public ToPngList()
        {
            Name = "To png imag list";
            Category = "Conversion";
            SubCategory = "Raster";
            Description = "Convert data cube  to png image";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            DateFormat = "yyyy-MM-dd";
            VariableName = "dc";
            Direcotry = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TimeInteval = 86400;
            Start = new DateTime(2000, 1, 1);
            TimeIntevalunit = TimeIntevalunits.Day;
            NumBreaks = 10;
            IntervalMethod = Core.Data.Classification.IntervalMethod.EqualFrequency;

            DPI = 300;
            Hue = 150;
            Contrast = 0.88;
            Saturation = 0.6;
            Brightness = 0.75;
            ImageSizeScale = 1;
            InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            ResizeMode = ImageProcessor.Processing.ResizeMode.Stretch;

            ClassificationMethod = Core.Data.Classification.ClassificationMethod.Category;
        }

        private IFeatureSet _grid_layer;
        private ClassificationMethod _ClassificationMethod;

        [Category("Input")]
        [Description("The name of the datacube being exported. The Source should be mat[0][0][:]")]
        public string Source
        {
            get;
            set;
        }


        [Category("Input")]
        [Description("Model grid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }



        [Category("Input")]
        [Description("The name of exported variable")]
        public string VariableName
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The file folder directory for output")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Direcotry
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Specify the date format used to generate the output files automatically. Its style should be yyyy-MM-dd or yyyy-MM")]
        public string DateFormat
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Specify the start date time.")]
        public DateTime Start
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Specify the time inteval in the coresponding time unit.")]
        public int TimeInteval
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("The time inteval unit")]
        public TimeIntevalunits TimeIntevalunit
        {
            get;
            set;
        }

      [Category("Symbology")]
        [Description("The DPI of image")]
        public double DPI
        {
            get;
            set;
        }
      [Category("Symbology")]
      public int ImageSizeScale
      {
          get;
          set;
      }
      [Category("Symbology")]
      public InterpolationMode InterpolationMode
      {
          get;
          set;
      }
      [Category("Symbology")]
      public ResizeMode ResizeMode
      {
          get;
          set;
      }

      [Category("Symbology")]
      public ClassificationMethod ClassificationMethod
      {
          get
          {
              return _ClassificationMethod;
          }
          set
          {
              _ClassificationMethod = value;
              if (_ClassificationMethod == Core.Data.Classification.ClassificationMethod.Strech || _ClassificationMethod == Core.Data.Classification.ClassificationMethod.CatBand)
              {
                  ColorBandFiles = ColorBandMapper.GetColorBandFile();
              }
          }
      }
      [Category("Cateogry Color")]
      [Description("The number of breaks")]
      public int NumBreaks { get; set; }

        [Category("Cateogry Color")]
        [Description("The inteval method")]
        public IntervalMethod IntervalMethod
        {
            get;
            set;
        }

        [Category("Cateogry Color")]
        public double Hue
        {
            get;
            set;
        }
        [Category("Cateogry Color")]
        public double Saturation
        {
            get;
            set;
        }
        [Category("Cateogry Color")]
        public double Brightness
        {
            get;
            set;
        }
        [Category("Cateogry Color")]
        public double Contrast
        {
            get;
            set;
        }
        
        [Browsable(false)]
        [Category("Strech Color")]
        public string[] ColorBandFiles
        {
            get;
            set;
        }
        private string _ColorBand;

        [Category("Strech Color")]
        [Description("Color band file")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("ColorBandFiles")]
        public string ColorBand
        {
            get
            {
                return _ColorBand;
            }
            set
            {
                _ColorBand = value;
            }
        }


        public override void Initialize()
        {
            this.Initialized = true;
            var mat = Get3DMat(Source);
            Initialized = mat != null;

            if (GridFeatureLayer == null)
            {
                this.Initialized = false;
                return;
            }

            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            if (_grid_layer == null)
            {
                this.Initialized = false;
            }

            if (TypeConverterEx.IsNull(Direcotry))
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                var ntime = mat.Size[1];
                var grid = mf.Grid as RegularGrid;
                if (mat.DateTimes == null)
                {
                    mat.DateTimes = new DateTime[ntime];
                    mat.DateTimes[0] = Start;
                    for (int t = 1; t < ntime; t++)
                    {
                        if (TimeIntevalunit == TimeIntevalunits.Second)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddSeconds(TimeInteval);
                        }
                        else if (TimeIntevalunit == TimeIntevalunits.Hour)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddHours(TimeInteval);
                        }
                        else if (TimeIntevalunit == TimeIntevalunits.Day)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddDays(TimeInteval);
                        }
                        else if (TimeIntevalunit == TimeIntevalunits.Month)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddMonths(TimeInteval);
                        }
                        else if (TimeIntevalunit == TimeIntevalunits.Year)
                        {
                            mat.DateTimes[t] = mat.DateTimes[t - 1].AddYears(TimeInteval);
                        }
                    }
                }
          
                List<int[]> colorindex_list = new List<int[]>();
                List<Color> colors;
                if (ClassificationMethod == Core.Data.Classification.ClassificationMethod.Category)
                {
                    colors = GetCategoryColors();
                    for (int t = 0; t < ntime; t++)
                    {
                        var Filename = Path.Combine(Direcotry, string.Format("{0}_{1}_buf.png", VariableName, mat.DateTimes[t].ToString(DateFormat)));
                        var newFilename = Path.Combine(Direcotry, string.Format("{0}_{1}.png", VariableName, mat.DateTimes[t].ToString(DateFormat)));
                        var vec = mat.GetVector(var_index, t.ToString(), ":");
                        var ci = CreateCatImage(Filename, vec, colors, grid);
                        ReSizeImage(Filename, newFilename);
                        progress = t * 100 / ntime;
                        colorindex_list.Add(ci);
                        if (progress > count)
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Processing step: " + t);
                            count++;
                        }
                    }
                }
                else if (ClassificationMethod == Core.Data.Classification.ClassificationMethod.CatBand)
                {
                    var cbfile = Path.Combine(ColorBandMapper.GetColorBandFileFolder(), ColorBand);
                    ColorBandMapper mapper = new ColorBandMapper(cbfile);
                    colors = mapper.GetCatPalette(NumBreaks);
                    for (int t = 0; t < ntime; t++)
                    {
                        var Filename = Path.Combine(Direcotry, string.Format("{0}_{1}_buf.png", VariableName, mat.DateTimes[t].ToString(DateFormat)));
                        var newFilename = Path.Combine(Direcotry, string.Format("{0}_{1}.png", VariableName, mat.DateTimes[t].ToString(DateFormat)));
                        var vec = mat.GetVector(var_index, t.ToString(), ":");
                        var ci = CreateCatImage(Filename, vec, colors, grid);
                        ReSizeImage(Filename, newFilename);
                        colorindex_list.Add(ci);
                        progress = t * 100 / ntime;
                        if (progress > count)
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Processing step: " + t);
                            count++;
                        }
                    }
                }
                else if (ClassificationMethod == Core.Data.Classification.ClassificationMethod.Strech)
                {
                    var cbfile = Path.Combine(ColorBandMapper.GetColorBandFileFolder(), ColorBand);
                    ColorBandMapper mapper = new ColorBandMapper(cbfile);
                    colors = mapper.GetFullPalette();
                    for (int t = 0; t < ntime; t++)
                    {
                        var Filename = Path.Combine(Direcotry, string.Format("{0}_{1}_buf.png", VariableName, mat.DateTimes[t].ToString(DateFormat)));
                        var newFilename = Path.Combine(Direcotry, string.Format("{0}_{1}.png", VariableName, mat.DateTimes[t].ToString(DateFormat)));
                        var vec = mat.GetVector(var_index, t.ToString(), ":");
                        var ci = GetStrechColorIndex(vec);
                        CreateStrechImage(Filename, ci, colors, grid);
                        ReSizeImage(Filename, newFilename);
                        progress = t * 100 / ntime;
                        if (progress > count)
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Processing step: " + t);
                            count++;
                        }
                    }

                }

                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }
        }

        private List<Color> GetCategoryColors()
        {
            var calculationParameters = new CalculationParameters(NumBreaks, Hue % 360.0, Contrast, Saturation, Brightness, RgbModel.AdobeRgbD65);
            var paletteGeneratorFactory = new PaletteGeneratorFactory();
            var paletteGenerator = paletteGeneratorFactory.CreatePaletteGenerator();
            var palette = paletteGenerator.GeneratePalette(calculationParameters).ToList();

            var count = palette.Count();
            var colors = new List<Color>();
            for (var i = 0; i < count; i++)
            {
                var vector3 = palette[i];
                var color = System.Drawing.Color.FromArgb(255, ToColorByte(vector3.X), ToColorByte(vector3.Y), ToColorByte(vector3.Z));
                colors.Add(color);
            }
            return colors;
        }
        private byte ToColorByte(double component)
        {
            var max = System.Math.Min(1.0, component);
            return (byte)(max * 255);
        }

        private void ReSizeImage(string file, string newfile)
        {
            using (var factory = new ImageFactory())
            {
                factory.Load(file)
                       .Resize(new ResizeOptions
                       {
                           Size = new Size(factory.Image.Width * ImageSizeScale, (factory.Image.Height * ImageSizeScale) + 40),
                           InterpolationMode = InterpolationMode,
                           ResizeMode = ResizeMode,
                       })
                       .Save(newfile);
            }
            File.Delete(file);
        }

        //private List<Color> GetStrechColorIndex(float[] vec)
        //{
        //    Color[] colors = new Color[vec.Length];
        //    if (!string.IsNullOrEmpty(ColorBand))
        //    {
        //        var max = vec.Max();
        //        var min = vec.Min();
        //        ColorBandMapper mapper = new ColorBandMapper(ColorBand);
        //        for (int i = 0; i < vec.Length; i++)
        //        {
        //            if (max != min)
        //                colors[i] = mapper.MapValueToColor(vec[i], min, max);
        //            else
        //                colors[i] = SystemColors.ButtonFace;
        //        }

        //    }
        //    return colors.ToList();
        //}

        private int[] GetStrechColorIndex(float[] vec)
        {
            int[] colors = new int[vec.Length];
            if (!string.IsNullOrEmpty(ColorBand))
            {
                var max = vec.Max();
                var min = vec.Min();
                var del = max - min;
                var cbfile = Path.Combine(ColorBandMapper.GetColorBandFileFolder(), ColorBand);
                ColorBandMapper mapper = new ColorBandMapper(cbfile);
                for (int i = 0; i < vec.Length; i++)
                {
                    int pixelY = (int)((vec[i] - min) / (del) * (mapper.ImageHeight - 1));
                    if (del != 0)
                        colors[i] = pixelY;
                    else
                        colors[i] = 0;
                }

            }
            return colors;
        }

        private int[] CreateCatImage(string filename, float[] vec, List<Color> colors, RegularGrid grid)
        {
            int cols = grid.ColumnCount;
            int rows = grid.RowCount;
            ImageInfo imi = new ImageInfo(cols, rows, 8, true); // 8 bits per channel, no alpha 
            // open image for writing 
            PngWriter png = FileHelper.CreatePngWriter(filename, imi, true);
            // add some optional metadata (chunks)
            png.GetMetadata().SetDpi(DPI);
            png.GetMetadata().SetTimeNow(0); // 0 seconds fron now = now
            png.GetMetadata().SetText(PngChunkTextVar.KEY_Title, "HEIFLOW");
            PngChunk chunk = png.GetMetadata().SetText("output", "output image");
            chunk.Priority = true; // this chunk will be written as soon as possible

            var transpcolor = Color.Transparent;
            var colorindex = GetCatColorIndex(vec);

            for (int row = 0; row < imi.Rows; row++)
            {
                byte[] rowint = new byte[cols * 4];
                for (int col = 0; col < imi.Cols; col++)
                {
                    var cellindex = grid.Topology.GetSerialIndex(row, col);
                    var col4 = col * 4;
                    if (cellindex == -1)
                    {
                        rowint[col4] = transpcolor.R;
                        rowint[col4 + 1] = transpcolor.G;
                        rowint[col4 + 2] = transpcolor.B;
                        rowint[col4 + 3] = transpcolor.A;
                    }
                    else
                    {
                        var color = colors[colorindex[cellindex]];
                        rowint[col4] = color.R;
                        rowint[col4 + 1] = color.G;
                        rowint[col4 + 2] = color.B;
                        rowint[col4 + 3] = color.A;
                    }
                }
                png.WriteRowByte(rowint, row);
            }

            png.End();

            return colorindex;
        }

        private void CreateStrechImage(string filename, int[] color_index, List<Color> colors, RegularGrid grid)
        {
            int cols = grid.ColumnCount;
            int rows = grid.RowCount;
            ImageInfo imi = new ImageInfo(cols, rows, 8, true); // 8 bits per channel, no alpha 
            // open image for writing 
            PngWriter png = FileHelper.CreatePngWriter(filename, imi, true);
            // add some optional metadata (chunks)
            png.GetMetadata().SetDpi(DPI);
            png.GetMetadata().SetTimeNow(0); // 0 seconds fron now = now
            png.GetMetadata().SetText(PngChunkTextVar.KEY_Title, "HEIFLOW");
            PngChunk chunk = png.GetMetadata().SetText("output", "output image");
            chunk.Priority = true; // this chunk will be written as soon as possible

            var transpcolor = Color.Transparent;
            int k = 0;
            for (int row = 0; row < imi.Rows; row++)
            {
                byte[] rowint = new byte[cols * 4];
                for (int col = 0; col < imi.Cols; col++)
                {
                    var cellindex = grid.Topology.GetSerialIndex(row, col);
                    var col4 = col * 4;
                    if (cellindex == -1)
                    {
                        rowint[col4] = transpcolor.R;
                        rowint[col4 + 1] = transpcolor.G;
                        rowint[col4 + 2] = transpcolor.B;
                        rowint[col4 + 3] = transpcolor.A;
                    }
                    else
                    {
                        var color = colors[color_index[k]];
                        rowint[col4] = color.R;
                        rowint[col4 + 1] = color.G;
                        rowint[col4 + 2] = color.B;
                        rowint[col4 + 3] = color.A;
                        k++;
                    }
                }
                png.WriteRowByte(rowint, row);
            }

            png.End();

        }

        private int[] GetCatColorIndex(float[] vec)
        {
            int[] index;
            if (IntervalMethod == IntervalMethod.NaturalBreaks)
            {
                index = JenksFisher.CreateJenksFisherIndex(vec.ToList(), NumBreaks);
            }
            else
            {
                var scheme = new Scheme();
                scheme.EditorSettings.NumBreaks = NumBreaks;
                scheme.EditorSettings.IntervalMethod = IntervalMethod;
                var veccopy = Array.ConvertAll(vec, x => (double)x);
                Array.Sort(veccopy);
                scheme.Values = veccopy.ToList();
                scheme.CreateBreakCategories();
                index = scheme.GetBreakIndex(vec);
            }
            return index;
        }
    }
}