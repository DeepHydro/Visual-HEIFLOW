using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Applications
{
    public interface IChildWPFWindow:IWindow
    {
        System.Windows.Window Owner { get; set; }
      
    }
}
