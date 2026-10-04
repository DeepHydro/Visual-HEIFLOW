using Heiflow.Visualization.Renderable.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.RenderableModel.Model.Renders
{
    public interface IVectorRender
    {
        VectorStyle Style { get; set; }
    }
}
