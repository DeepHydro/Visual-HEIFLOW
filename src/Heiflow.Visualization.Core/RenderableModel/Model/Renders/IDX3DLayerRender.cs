using Heiflow.Core.Drawing;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Visualization;
using Heiflow.Presentation.Controls;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    public interface IDX3DLayerRender : I3DLayerRender
    {
        event EventHandler ColorRampChanged;
        event StatisticsInfoHandler ValueRangeChanged;
        event EventHandler GridValuesChanged;
        event EventHandler DataSourceChanged;


        CustomVertex.PositionNormalColored[] VertexList { get; }

        int[] VertexIndexList { get; }

        /// <summary>
        /// Two indices for every edge of the mesh, for a layer that is drawn as a wire frame. Null for
        /// a render that keeps none, and the layer then draws the indices of the triangles as lines,
        /// which is how it was done before.
        /// </summary>
        int[] LineIndexList { get; }

        /// <summary>
        /// Bumped whenever the geometry is rebuilt, the layers upload the buffers again when it changed.
        /// </summary>
        int MeshVersion { get; }

        /// <summary>
        /// Bumped whenever the vertex colours change.
        /// </summary>
        int ColorVersion { get; }

        int[] SelectedVertexIndexes { get; set; }

        CustomVertex.PositionColored[] SelectedVertexes { get; set; }

        Ramp ColorRamp { get; }

        double MaxCellValue { get; }

        double MinCellValue { get; }

        List<IProfile> Profiles { get; }

        bool UniqueColor { get; set; }

        bool InvertColor { get; set; }

        /// <summary>
        /// Scale the values over one range for the whole run instead of putting them in classes, see
        /// ModelRenderDX.ContinuousColor.
        /// </summary>
        bool ContinuousColor { get; set; }

        /// <summary>
        /// Let the sun and the ambient take a shade off the colours, see ModelRenderDX.ShadedLighting.
        /// </summary>
        bool ShadedLighting { get; set; }

        int Opacity { get; set; }

        int ColorRampID { get; set; }

        int ColourRampCount { get; set; }

        /// <summary>
        /// Draws the mesh with the colours read out of textures rather than out of the vertices, see
        /// ModelRenderDX.DrawShaded. False leaves the layer to draw the mesh as it always did.
        /// </summary>
        bool DrawShaded(Device device, ModelMeshBuffer mesh, PrimitiveType primitiveType,
            Matrix worldViewProjection, Vector3 sunDirection, float sunLight, float[] ambient);
        float DistanceAboveSurface { get; set; }
        ClassificationMethod ClassificationMethod { get; set; }

        string SupportedLayerObject { get; set; }

        List<ICell> SelectedCells { get; }

        ICell SelectCell(double lat, double lon);

        IBasicModel Owner { get; set; }

        GCSConverter GCSConverter { get; set; }

        IProject Project { get; set; }
        RenderableObject RenderableObject { get; set; }
        void UpdateCellValues();

    }
}
