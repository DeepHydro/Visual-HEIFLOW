using DotSpatial.Data;
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using GeoAPI.Geometries;
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

namespace Heiflow.Tools.Visualization
{
    public class WaterBodyMapping : ModelTool
    {
        private double[,] sobel_x = new double[,] {{-1, 0, 1}, 
                       {-2, 0, 2}, 
                       {-1, 0, 1}};
        private double[,] sobel_y = new double[,] {{-1, -2, -1}, 
                        {0,  0,  0}, 
                        {1,  2,  1}};
        private int senseor = 5;

        public WaterBodyMapping()
        {
            Name = "Water Body Mapping";
            Category = "Visualization";
            Description = " ";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            AlphaThreashhold = 8;
            if(senseor == 8)
                FileDirectory = @"E:\Project\HRB\Badan Jarian\Data\cluster\resample\landsat5\";
            else
                FileDirectory = @"E:\Project\HRB\Badan Jarian\Data\cluster\resample\landsat8\";
        }

        [Category("Input")]
        [Description("The input file directory")]
        public string FileDirectory
        {
            get;
            set;
        }
        [Category("Optional")]
        [Description("The Alpha Threashhold")]
        public double AlphaThreashhold
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {

            int progress = 0;
            int count = 1;
            int ndays = 174;
            if (senseor == 5)
                ndays = 174;
            else
                ndays = 56;
            //string lake_centroid = @"E:\Project\HRB\Badan Jarian\Data\Geospatial\mask_centroid.shp";
            string lake_centroid = @"E:\Project\HRB\Badan Jarian\Data\Geospatial\mask_centroid_selected.shp";
            var lake_centroid_fs = FeatureSet.Open(lake_centroid);
            DataTable centroid_dt = lake_centroid_fs.DataTable;
            //var lake_list = (from DataRow dr in centroid_dt.Rows where dr["Flag"].ToString() == "1" select int.Parse(dr["Id"].ToString())).ToList();
           // var lake_list = new int[] { 1,18,19,28,30,35,41,49,50,55,57,58,59,60,63,89,91,97,99,100,112,113,118,133,135,136,160,169,181,183};
            var lake_list = new int[] { 59 };
            int nlakes = lake_list.Length;
            double [,] water_area=new double[ndays,nlakes];
            double[,] water_bond_area = new double[ndays, nlakes];

            for (int K = 0; K < nlakes; K++)
            {
                int lake_id = lake_list[K];
                var dr_lake = (from DataRow dr in centroid_dt.Rows where dr["Id"].ToString() == lake_id.ToString() select dr).First();

                string img_dir = Path.Combine(FileDirectory, lake_id.ToString());
                StreamReader sr_date_list = new StreamReader(Path.Combine(img_dir, "date_list.txt"));
                int nfile = 0;
                while (!sr_date_list.EndOfStream)
                {
                    sr_date_list.ReadLine();
                    nfile++;
                }
                sr_date_list.Close();
                sr_date_list = new StreamReader(Path.Combine(img_dir, "date_list.txt"));
                string[] dirs = new string[nfile];
                for (int i = 0; i < nfile; i++)
                {
                    var str = sr_date_list.ReadLine();
                    dirs[i] = Path.Combine(img_dir, str + "_cmb.tif");
                }

                string class_file_list = Path.Combine(img_dir, @"class\class_list.txt");
                StreamWriter sw_area = new StreamWriter(Path.Combine(img_dir, "class\\area.csv"));
                FileStream fs_class = new FileStream(class_file_list, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr_class_file = new StreamReader(fs_class);
                var center_pt = new Coordinate(double.Parse(dr_lake["X"].ToString()), double.Parse(dr_lake["Y"].ToString()));
                int cell_area = 5 * 5;
                int date_index = 0;
                try
                {
                    foreach (var file in dirs)
                    {
                        int water_cell_count = 0;
                        int water_bound_cell_count = 0;
                        var temp = Path.GetFileNameWithoutExtension(file);
                        var daystr = temp.Remove(10);
                        var water_file = file.Replace("_cmb", "_water");
                        IRaster raster2 = Raster.OpenFile(file);
                        raster2.SaveAs(water_file);
                        IRaster raster_water = Raster.OpenFile(water_file);

                        var class_file = sr_class_file.ReadLine();
                        var class_mat = ReadClassFile(class_file, raster2.NumRows, raster2.NumColumns);

                        for (int i = 0; i < raster2.NumRows; i++)
                        {
                            for (int j = 0; j < raster2.NumColumns; j++)
                            {
                                if (raster2.Value[i, j] != raster2.NoDataValue)
                                {
                                    raster_water.Value[i, j] = class_mat[i][j];
                                }
                                else
                                {
                                    raster_water.Value[i, j] = raster2.NoDataValue;
                                }
                            }
                        }
                        var cell = raster2.ProjToCell(center_pt);
                        var center_class = raster_water.Value[cell.Row, cell.Column];
                        var center_bound_class = center_class;
                        for (int i = cell.Row + 1; i < raster2.NumRows; i++)
                        {
                            if (raster_water.Value[i, cell.Column] != center_class)
                            {
                                center_bound_class = raster_water.Value[i, cell.Column];
                                break;
                            }
                        }

                        if (center_bound_class == center_class)
                        {
                            for (int i = cell.Column + 1; i < raster2.NumColumns; i++)
                            {
                                if (raster_water.Value[cell.Row, i] != center_class)
                                {
                                    center_bound_class = raster_water.Value[cell.Row, i];
                                    break;
                                }
                            }
                        }

                        for (int i = 0; i < raster2.NumRows; i++)
                        {
                            for (int j = 0; j < raster2.NumColumns; j++)
                            {
                                if (raster2.Value[i, j] != raster2.NoDataValue)
                                {
                                    if (raster_water.Value[i, j] == center_class)
                                    {
                                        water_cell_count++;
                                    }
                                    if (raster_water.Value[i, j] == center_bound_class)
                                    {
                                        water_bound_cell_count++;
                                    }
                                }
                            }
                        }
                        water_area[date_index, K] = water_cell_count * cell_area;
                        water_bond_area[date_index, K] = water_bound_cell_count * cell_area;
                        sw_area.WriteLine(string.Format("{0},{1},{2},{3}", daystr, water_area[date_index, K], water_bond_area[date_index, K],
                            water_area[date_index, K] + water_bond_area[date_index, K]));

                        raster_water.Save();
                        raster_water.Close();

                        date_index++;
                    }
                }
                catch (Exception ex)
                {
                    cancelProgressHandler.Progress("Package_Tool", 100, "Error: " + ex.Message);
                }
                finally
                {
                    sw_area.Close();
                    sr_class_file.Close();
                    sr_date_list.Close();
                }
                progress = K * 100 / nlakes;
                //if (progress > count)
                //{
                cancelProgressHandler.Progress("Package_Tool", progress, "Processing lake " + lake_id);
                count++;
                // }
            }
            string lake_area_file = Path.Combine(FileDirectory, "water_area.csv");
            string lake_area_bond_file = Path.Combine(FileDirectory, "waterbond_area.csv");
            StreamWriter csv_water = new StreamWriter(lake_area_file);
            StreamWriter csv_bond = new StreamWriter(lake_area_bond_file);
            var line = string.Join(",", lake_list.ToArray());
            csv_water.WriteLine(line);
            csv_bond.WriteLine(line);
            for (int t = 0; t < ndays; t++)
            {
                line = "";
                for (int j = 0; j < nlakes; j++)
                {
                    line += water_area[t, j] + ",";
                }
                line = line.TrimEnd(new char[] { ',' });
                csv_water.WriteLine(line);

                line = "";
                for (int j = 0; j < nlakes; j++)
                {
                    line += water_bond_area[t, j] + ",";
                }
                line = line.TrimEnd(new char[] { ',' });
                csv_bond.WriteLine(line);
            }

            csv_water.Close();
            csv_bond.Close();
            lake_centroid_fs.Close();
            return true;
        }


        private int[][] ReadClassFile(string filename, int nrow, int ncol)
        {
            int[][] mat = new int[nrow][];
            StreamReader sr = new StreamReader(filename);
            for (int i = 0; i < nrow; i++)
            {
                var line = sr.ReadLine();
                mat[i] = TypeConverterEx.Split<int>(line);
            }
            sr.Close();
            return mat;
        }

        public  bool Execute1(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamReader sr = new StreamReader(Path.Combine(FileDirectory, "list.txt"));
            int nfile = 0;
            while (!sr.EndOfStream)
            {
                sr.ReadLine();
                nfile++;
            }
            sr.Close();
            sr = new StreamReader(Path.Combine(FileDirectory, "list.txt"));
            string[] dirs = new string[nfile];
            for (int i = 0; i < nfile; i++)
            {
                var str = sr.ReadLine();
                dirs[i] = Path.Combine(FileDirectory, str + "_2.tif");
            }

            int[] bandlist = new int[] { 4, 3, 2 };
            //string[] dirs = Directory.GetFiles(FileDirectory, "*_2.tif");
            string center_shp = @"E:\Project\HRB\Badan Jarian\Data\Geospatial\Center35.shp";
            StreamWriter sw = new StreamWriter(Path.Combine(FileDirectory, "area.csv"));
            StreamWriter sw_alpha = new StreamWriter(Path.Combine(FileDirectory, "alpha.txt"));
            IFeatureSet center_fs = FeatureSet.Open(center_shp);
            List<double> vec = new List<double>();
            try
            {
                double cell_area = 5 * 5;

                var center_pt = center_fs.Features[0].Geometry.Coordinate;
                var center_vec = new double[3];
                var pt_vec = new double[3];
                int progress = 0;
                int count = 1;
                int t = 0;
                vec.Clear();
                foreach (var file in dirs)
                {
                    long wcount = 0;
                    var temp = Path.GetFileNameWithoutExtension(file);
                    var daystr = temp.Remove(10);
                    var water_file = file.Replace("_2", "_water");
                    IRaster raster2 = Raster.OpenFile(file);
                    raster2.SaveAs(water_file);
                    IRaster raster3 = Raster.OpenFile(file.Replace("_2", "_3"));
                    IRaster raster4 = Raster.OpenFile(file.Replace("_2", "_4"));
                    IRaster raster_water = Raster.OpenFile(water_file);
                    double[,] img = new double[raster2.NumRows, raster2.NumColumns];
                    var cell = raster2.ProjToCell(center_pt);
                    center_vec[0] = raster2.Value[cell.Row, cell.Column];
                    center_vec[1] = raster3.Value[cell.Row, cell.Column];
                    center_vec[2] = raster4.Value[cell.Row, cell.Column];
                    for (int i = 0; i < raster2.NumRows; i++)
                    {
                        for (int j = 0; j < raster2.NumColumns; j++)
                        {
                            if (raster2.Value[i, j] == raster2.NoDataValue)
                            {
                                img[i, j] = 0;
                                raster_water.Value[i, j] = 0;
                                sw_alpha.WriteLine(0);
                                vec.Add(0);
                            }
                            else
                            {
                                pt_vec[0] = raster2.Value[i, j];
                                pt_vec[1] = raster3.Value[i, j];
                                pt_vec[2] = raster4.Value[i, j];
                                var alpha = sam(pt_vec, center_vec);
                                if (alpha <= AlphaThreashhold)
                                {
                                    raster_water.Value[i, j] = 1;
                                    wcount++;
                                }
                                else
                                {
                                    raster_water.Value[i, j] = 0;
                                }
                                try
                                {
                                    raster_water.Value[i, j] = alpha;
                                }
                                catch (Exception ex)
                                {
                                    cancelProgressHandler.Progress("Package_Tool", progress, "Warning: " + ex.Message + " " + alpha);
                                    alpha = 0;
                                    raster_water.Value[i, j] = 0;
                                }
                                finally
                                {
                                    img[i, j] = alpha;
                                    vec.Add(alpha);
                                    sw_alpha.WriteLine(alpha);
                                }
                            }
                        }
                    }
                    var max = vec.Max();
                    var min = vec.Min();
                    var delta = max - min;
                    int k = 0;
                    for (int i = 0; i < raster2.NumRows; i++)
                    {
                        for (int j = 0; j < raster2.NumColumns; j++)
                        {
                             img[i, j]  = (vec[k] - min) / delta * 255;
                             raster_water.Value[i, j] = img[i, j];
                            k++;
                        }
                    }

                    var sobled_img = sobel(img, raster2.NumRows, raster2.NumColumns);
                    for (int i = 0; i < raster2.NumRows; i++)
                    {
                        for (int j = 0; j < raster2.NumColumns; j++)
                        {
                            raster_water.Value[i, j] = sobled_img[i, j];
                        }
                    }
                    var water_area = wcount * cell_area;
                    sw.WriteLine(daystr + "," + water_area);
                    raster2.Close();
                    raster3.Close();
                    raster4.Close();
                    raster_water.Save();
                    raster_water.Close();
                    progress = t * 100 / dirs.Length;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing: " + file);
                        count++;
                    }
                    t++;
                }
            }
            catch (Exception ex)
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error: " + ex.Message);
            }
            finally
            {
                sw_alpha.Close();
                sr.Close();
                sw.Close();
                center_fs.Close();
            }

            return true;
        }

        private double sam(double[] bi, double[] bj)
        {
            int len = bi.Length;
            double alpha = 0;
            double temp1 = 0;
            double mi = 0;
            double mj = 0;
            bool equal = true;
            for (int i = 0; i < len; i++)
            {
                temp1 += bi[i] * bj[i];
                mi += bi[i] * bi[i];
                mj += bj[i] * bj[i];
                if (bi[i] != bj[i])
                    equal = false;
            }
            double buf = temp1 / (System.Math.Sqrt(mi) * System.Math.Sqrt(mj));
            if (equal)
                buf = 1;
            alpha = System.Math.Acos(buf) * 180 / System.Math.PI;
            return alpha;
        }

        private double[,] sobel(double[,] img, int width, int height)
        {
            double[,] sobeled = new double[width, height];
            double pixel_x = 0;
            double pixel_y = 0;
            
            for (int x = 1; x < width - 2; x++)
            {
                for (int y = 1; y < height - 2; y++)
                {
                    pixel_x = (sobel_x[0, 0] * img[x - 1, y - 1]) + (sobel_x[0, 1] * img[x, y - 1]) +   (sobel_x[0, 2] * img[x + 1, y - 1]) +
                                    (sobel_x[1, 0] * img[x - 1, y]) +        (sobel_x[1, 1] * img[x, y]) +          (sobel_x[1, 2] * img[x + 1, y]) +
                                    (sobel_x[2, 0] * img[x - 1, y + 1]) + (sobel_x[2, 1] * img[x, y + 1]) +    (sobel_x[2, 2] * img[x + 1, y + 1]);

                 pixel_y =   (sobel_y[0, 0] * img[x - 1, y - 1]) + (sobel_y[0, 1] * img[x, y - 1]) +        (sobel_y[0, 2] * img[x + 1, y - 1]) +
                                    (sobel_y[1, 0] * img[x - 1, y]) +       (sobel_y[1, 1] * img[x, y]) +              (sobel_y[1, 2] * img[x + 1, y]) +
                                    (sobel_y[2, 0] * img[x - 1, y + 1]) + (sobel_y[2, 1] * img[x, y + 1]) +       (sobel_y[2, 2] * img[x + 1, y + 1]);
                 sobeled[x, y] = System.Math.Ceiling(System.Math.Sqrt(pixel_x * pixel_x + pixel_y * pixel_y));
                }
            }
            return sobeled;
        }
    }
}
