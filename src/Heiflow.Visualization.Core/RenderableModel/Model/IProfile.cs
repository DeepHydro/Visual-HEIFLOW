using System;
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
        Microsoft.DirectX.Direct3D.CustomVertex.PositionNormalColored[] VertexList { get; }
    }
}
