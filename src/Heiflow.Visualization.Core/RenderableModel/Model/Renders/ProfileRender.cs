using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class ProfileRender : MFGridRender
    {
        public ProfileRender(World world)
            : base(world)
        {

        }

        public override void Initilize()
        {
            // must be called
            base.Initilize();
            _Topology = _MFGrid.Topology;
        }

        public override Cell[] RetrieveCellsInRow(int row)
        {
            List<Cell> list = new List<Cell>();
            var ibound = MFGrid.IBound;
            for (int c = 0; c < MFGrid.ColumnCount; c++)
            {
                if (ibound[0,row,c] != 0)
                {
                    MFCell cell = new MFCell(row, c);
                    cell.CalcCoordinate(this);
                    var index = _Topology.GetSerialIndex(row, c);
                    cell.Elevation = MFGrid.Elevations[0,0,index];
                    cell.Elevations = new double[MFGrid.LayerCount];
                    for (int l = 0; l < MFGrid.LayerCount; l++)
                    {
                        cell.Elevations[l] = MFGrid.Elevations[l,0,index];
                    }
                    list.Add(cell);
                }
            }
            return list.ToArray();
        }

        public override Cell[] RetrieveCellsInColumn(int col)
        {
            List<Cell> list = new List<Cell>();
            var ibound = MFGrid.IBound;
            for (int r = 0; r < MFGrid.RowCount; r++)
            {
                if (ibound[0,r,col] != 0)
                {
                    MFCell cell = new MFCell(r, col);
                    cell.CalcCoordinate(this);
                    var index = _Topology.GetSerialIndex(r, col);
                    cell.Elevation = MFGrid.Elevations[0,0,index];
                    cell.Elevations = new double[MFGrid.LayerCount];
                    for (int l = 0; l < MFGrid.LayerCount; l++)
                    {
                        cell.Elevations[l] = MFGrid.Elevations[l,0,index];
                    }
                    list.Add(cell);
                }
            }
            return list.ToArray();
        }

        public override void CreatMeshes()
        {
             
        }
    }
}
