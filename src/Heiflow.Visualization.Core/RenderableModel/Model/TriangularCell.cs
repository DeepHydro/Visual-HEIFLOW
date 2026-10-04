using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class TriangularCell : Cell
    {
        public TriangularCell(int id)
            : base(id)
        {

        }

        public uint[] VertexID { get; set; }


        public override float[] GetTimeSeries()
        {
            if (Render != null)
            {
                var DataSource = Render.DataSource;
                if (DataSource is DataCube<float>)
                {
                    var vec = new float[DataSource.Size[1]];
                    var id = (int)VertexID[0];
                    for (int t = 0; t < DataSource.Size[1]; t++)
                    {
                        vec[t] = DataSource[Render.VarIndex, t,id];
                    }
                    return vec;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
