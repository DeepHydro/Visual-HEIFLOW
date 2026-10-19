using System;
using Heiflow.Visualization.DirectX;
namespace Heiflow.Visualization.Renderable.Grid
{
    public  interface IProfile
    {
        void BuildProfile();
        int ColumnIndex { get; set; }
        double DistanceAboveSurface { get; set; }
        double MaxProfileLength { get; set; }
        ProfileType ProfileType { get; set; }
        int RowIndex { get; set; }
        double ScaleFactor { get; set; }
        int[] VertexIndexList { get; }
        CustomVertex.PositionNormalColored[] VertexList { get; }
    }
}
