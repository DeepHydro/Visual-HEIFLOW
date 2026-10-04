using Heiflow.Core.Data.ODM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    public interface ISiteView :  IView, IChildWPFWindow
    {
        Site Site { get; set; }
    }
}
