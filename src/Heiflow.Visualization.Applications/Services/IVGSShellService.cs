using Heiflow.Models.UI;
using Heiflow.Presentation.Services;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Display;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Heiflow.Visualization.Applications
{
    public interface IVGSShellService : IShellService
    {
        DispatcherTimer Timer
        {
            get;
            set;
        }

        IVirtualGlobeView VirtualGlobeView
        {
            get;
            set;
        }

        object LayerManager
        {
            get;
            set;
        }
        IAboutView AboutView
        {
            get;
            set;
        }
        IProjectExplorerView ProjectExplorerView
        {
            get;
            set;
        }
        ISymbologyView SymbologyView
        {
            get;
            set;
        }
        GridLengendBar Legend
        {
            get;
            set;
        }

        List<IChildWPFWindow> ChildWPFWindows
        {
            get;
        }
        ISiteView SiteView
        {
            get;
            set;
        }
        void Initialize();

    }
}
