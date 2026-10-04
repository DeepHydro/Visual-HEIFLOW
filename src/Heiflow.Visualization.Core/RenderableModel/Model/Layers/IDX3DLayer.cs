using Heiflow.Models.Generic;
using Heiflow.Models.Visualization;
using System;
namespace Heiflow.Visualization.Renderable.Grid
{
    public interface IDX3DLayer:I3DLayer
    {
        IDX3DLayerRender RenderDX { get; set; }
        bool HighLightSelectedCell { get; set; }
        bool IsUsed { get; set; }
    }
}
