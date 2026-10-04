using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    public interface IVGSShellView:IView
    {
        void Show();

        void Close();

        event CancelEventHandler Closing;

        event EventHandler Closed;
    }
}
