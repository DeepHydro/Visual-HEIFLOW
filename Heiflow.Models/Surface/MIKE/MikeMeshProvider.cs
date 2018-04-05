// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.MIKE
{
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

                line = sr.ReadLine();
                cstr = TypeConverterEx.Split<string>(line);
                grid.CellCount = int.Parse(cstr[0]);
                grid.Centroids = new Coordinate[grid.CellCount];
                topology.VertexIndices = new uint[grid.CellCount][];
                topology.NodeConnectedCells = new uint[grid.VertexCount][];

                for (uint i = 0; i < grid.CellCount; i++)
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
                grid.Elevations = new My3DMat<float>(1,1, grid.VertexCount);
                grid.Elevations[0, 0] = elev;
                topology.Grid = grid;
                grid.Topology = topology;
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
