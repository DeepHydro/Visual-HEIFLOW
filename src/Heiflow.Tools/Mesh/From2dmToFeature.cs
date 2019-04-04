using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using NetTopologySuite.Geometries;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Mesh
{
    public class From2dmToFeature : ModelTool
    {
        public From2dmToFeature()
        {
            Name = "From2dmToFeature";
            Category = "Mesh";
            Description = "Convert 2dm file to shapefile. The 2dm is a file format used by SM to store unstructured mesh";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The 2dm filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string InputFileName
        {
            get;
            set;
        }
        public override void Initialize()
        {
            if (TypeConverterEx.IsNotNull(InputFileName) && File.Exists(InputFileName))
            {
                this.Initialized = true;
            }
            else
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            List<int[]> list_poly = new List<int[]>();
            List<float[]> list_nodes = new List<float[]>();
            StreamReader sr = new StreamReader(InputFileName);
            var line = sr.ReadLine();
            line = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (TypeConverterEx.IsNotNull(line))
                {
                    if (line.Contains("BEGPARAMDEF"))
                        break;
                    if (line.Contains("E3T"))
                    {
                        var buf = TypeConverterEx.Split<string>(line.Trim());
                        int[] num = new int[3];
                        num[0] = int.Parse(buf[2]);
                        num[1] = int.Parse(buf[3]);
                        num[2] = int.Parse(buf[4]);
                        list_poly.Add(num);
                    }
                    else if (line.Contains("ND"))
                    {
                        var buf = TypeConverterEx.Split<string>(line);
                        float[] num = new float[3];
                        num[0] = float.Parse(buf[2]);
                        num[1] = float.Parse(buf[3]);
                        num[2] = float.Parse(buf[4]);
                        list_nodes.Add(num);
                    }
                }
            }
            sr.Close();
            cancelProgressHandler.Progress("Package_Tool", 10, "2dm file loaded.");
            var num_nodes = list_nodes.Count;
            var num_polys = list_poly.Count;
            var node_shp = InputFileName.Replace(".2dm", "_pt.shp");
            var poly_shp = InputFileName.Replace(".2dm", "_poly.shp");
            FeatureSet fs_poly = new FeatureSet(FeatureType.Polygon);
            fs_poly.Name = "Mesh_Elements";
            fs_poly.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
            fs_poly.DataTable.Columns.Add(new DataColumn("Temp", typeof(double)));
            for (int i = 0; i < num_polys; i++)
            {
                GeoAPI.Geometries.Coordinate[] vertices = new GeoAPI.Geometries.Coordinate[4];
                var temp=list_poly[i];
                vertices[0] = new Coordinate();
                vertices[0].X = list_nodes[temp[0] - 1][0];
                vertices[0].Y = list_nodes[temp[0] - 1][1];
                vertices[0].Z = list_nodes[temp[0] - 1][2];
                vertices[1] = new Coordinate();
                vertices[1].X = list_nodes[temp[1] - 1][0];
                vertices[1].Y = list_nodes[temp[1] - 1][1];
                vertices[1].Z = list_nodes[temp[1] - 1][2];
                vertices[2] = new Coordinate();
                vertices[2].X = list_nodes[temp[2] - 1][0];
                vertices[2].Y = list_nodes[temp[2] - 1][1];
                vertices[2].Z = list_nodes[temp[2] - 1][2];
                vertices[3] = new Coordinate(vertices[0]);
                GeoAPI.Geometries.ILinearRing ring = new LinearRing(vertices);
                Polygon geom = new Polygon(ring);
                DotSpatial.Data.IFeature feature = fs_poly.AddFeature(geom);
                feature.DataRow.BeginEdit();
                feature.DataRow["ID"] = i+1;
                feature.DataRow["Temp"] = System.Math.Round((vertices[0].Z + vertices[1].Z + vertices[2].Z) / 3, 2);
                feature.DataRow.EndEdit();
            }
            fs_poly.SaveAs(poly_shp, true);
            cancelProgressHandler.Progress("Package_Tool", 80, "Mesh elements shapefile created.");
            FeatureSet fs_pt = new FeatureSet(FeatureType.Point);
            fs_pt.Name = "Mesh_Nodes";
            fs_pt.DataTable.Columns.Add(new DataColumn("ID", typeof(int)));
            fs_pt.DataTable.Columns.Add(new DataColumn("Temp", typeof(double)));
            for (int i = 0; i < num_nodes; i++)
            {
                Point pt = new Point(list_nodes[i][0], list_nodes[i][1], list_nodes[i][2]);
                DotSpatial.Data.IFeature feature = fs_pt.AddFeature(pt);
                feature.DataRow.BeginEdit();
                feature.DataRow["ID"] = i + 1;
                feature.DataRow["Temp"] = list_nodes[i][2];
                feature.DataRow.EndEdit();
            }
            fs_pt.SaveAs(node_shp,true);
            cancelProgressHandler.Progress("Package_Tool", 90, "Mesh nodes shapefile created.");
            cancelProgressHandler.Progress("Package_Tool", 100, "Finished.");
            return true;
        }
    }
}