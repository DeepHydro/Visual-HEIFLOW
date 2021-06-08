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

using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.MIKE
{
    [Export(typeof(IGridFileProvider))]
    public class MikeMeshProvider : IGridFileProvider
    {
        public MikeMeshProvider()
        {
      
        }

        public string FileTypeDescription
        {
            get
            {
                return "grid file used by Mike 2D";
            }
        }

        public string Extension
        {
            get
            {
                return ".mesh";
            }
        }

        public string FileName
        {
            get;
            set;
        }

        public IGrid Provide(string filename)
        {
            TriangularGrid grid = new TriangularGrid();
            var topology = new TriGridTopology();

            if (File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                string line = sr.ReadLine();
                string[] cstr = TypeConverterEx.Split<string>(line);
                grid.VertexCount = int.Parse(cstr[2]);
                grid.Vertex = new Coordinate[grid.VertexCount];
                Dictionary<uint, List<uint>> map = new Dictionary<uint, List<uint>>();


                for (uint i = 0; i < grid.VertexCount; i++)
                {
                    line = sr.ReadLine();
                    cstr = TypeConverterEx.Split<string>(line);
                    grid.Vertex[i] = new Coordinate(double.Parse(cstr[1]), double.Parse(cstr[2]), double.Parse(cstr[3]));
                    map[i] = new List<uint>();
                }

                double xxmin = (from vert in grid.Vertex select vert.X).Min();
                double xxmax = (from vert in grid.Vertex select vert.X).Max();
                double yymin = (from vert in grid.Vertex select vert.Y).Min();
                double yymax = (from vert in grid.Vertex select vert.Y).Max();

                line = sr.ReadLine();
                cstr = TypeConverterEx.Split<string>(line);
                grid.ActiveCellCount = int.Parse(cstr[0]);
                grid.Centroids = new Coordinate[grid.ActiveCellCount];
                topology.VertexIndices = new uint[grid.ActiveCellCount][];
                topology.NodeConnectedCells = new uint[grid.VertexCount][];

                for (uint i = 0; i < grid.ActiveCellCount; i++)
                {
                    double bathymetry = 0;
                    topology.VertexIndices[i] = new uint[3];

                    line = sr.ReadLine();
                    cstr = TypeConverterEx.Split<string>(line);
                    topology.VertexIndices[i][0] = uint.Parse(cstr[1]) -1 ;
                    topology.VertexIndices[i][1] = uint.Parse(cstr[2]) - 1;
                    topology.VertexIndices[i][2] = uint.Parse(cstr[3]) - 1;

                    map[topology.VertexIndices[i][0]].Add(i);
                    map[topology.VertexIndices[i][1]].Add(i);
                    map[topology.VertexIndices[i][2]].Add(i);

                    bathymetry += (grid.Vertex[topology.VertexIndices[i][0]].Z + grid.Vertex[topology.VertexIndices[i][1]].Z
                        + grid.Vertex[topology.VertexIndices[i][2]].Z) / 3.0;

                    Triangle trig = new Triangle(grid.Vertex[topology.VertexIndices[i][0]], grid.Vertex[topology.VertexIndices[i][1]],
                        grid.Vertex[topology.VertexIndices[i][2]]);
                    grid.Centroids[i] = new Coordinate(trig.InCentre());
                    grid.Centroids[i].Z = bathymetry;
                }

                topology.NodeConnectedCells = new uint[grid.VertexCount][];
                for (uint i = 0; i < grid.VertexCount; i++)
                {
                    topology.NodeConnectedCells[i] = map[i].ToArray();
                }

                var elev = (from vert in grid.Vertex select (float)vert.Z).ToArray();
                grid.Elevations = new DataCube<float>(1, 1, grid.VertexCount);
                grid.Elevations[0, "0",":"] = elev;
                topology.Grid = grid;
                grid.Topology = topology;

                grid.BBox = new Envelope(xxmin,xxmax,yymin,yymax);
                grid.BBoxCentroid = new Coordinate((xxmin + xxmax) * 0.5, (yymin + yymax) * 0.5);
                sr.Close();
                map.Clear();
            }
            return grid;
        }

        public void Save(string filename, IGrid grid)
        {

        }
    }
}
