using Heiflow.Visualization.Renderable.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    public interface ISymbologyView : IView, IChildWPFWindow
    {
        IDX3DLayerRender SelectedRender { get; set; }
    }
}
