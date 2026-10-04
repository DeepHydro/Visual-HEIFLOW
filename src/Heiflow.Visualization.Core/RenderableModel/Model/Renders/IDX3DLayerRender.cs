using Heiflow.Core.Drawing;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Visualization;
using Heiflow.Presentation.Controls;
using HUST.WREIS.Dot3D.Renderable;
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

        int[] SelectedVertexIndexes { get; set; }

        CustomVertex.PositionColored[] SelectedVertexes { get; set; }

        Ramp ColorRamp { get; }

        double MaxCellValue { get; }

        double MinCellValue { get; }

        List<IProfile> Profiles { get; }

        bool UniqueColor { get; set; }

        bool InvertColor { get; set; }

        int Opacity { get; set; }

        int ColorRampID { get; set; }

        int ColourRampCount { get; set; }
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
