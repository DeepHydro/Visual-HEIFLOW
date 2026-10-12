using Heiflow.Models.UI;
using Heiflow.Presentation.Services;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Display;
using System.Collections.Generic;

namespace Heiflow.Visualization.Applications
{
    public interface IVGSShellService : IShellService
    {
        /// <summary>
        /// Lets the scene draw while it is set. The render loop runs whenever the dispatcher runs out
        /// of work, so this is what stops it rather than a timer that is stopped.
        /// </summary>
        bool IsRenderingEnabled
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
