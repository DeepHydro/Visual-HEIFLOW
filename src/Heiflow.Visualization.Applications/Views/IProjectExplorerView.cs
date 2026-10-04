using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Applications
{
    public interface IProjectExplorerView : IChildWPFWindow
    {
        Heiflow.Presentation.Controls.Project.ProjectExplorerControl ProjectExplorer { get; set; }
    }
}
