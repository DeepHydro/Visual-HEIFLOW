using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Applications
{
    public interface IProjectExplorerView : IChildWPFWindow
    {
        /// <summary>
        /// The explorer shown by this window. It is the interface and not a concrete control, so the
        /// view can be implemented either with Windows Forms or with WPF.
        /// </summary>
        IProjectExplorer ProjectExplorer { get; set; }
    }
}
