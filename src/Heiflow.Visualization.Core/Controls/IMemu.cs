using Heiflow.Core.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Heiflow.Visualization.Controls
{
    public interface IMenu
    {
        void OnKeyUp(KeyEventArgs keyEvent);
        void OnKeyDown(KeyEventArgs keyEvent);
        bool OnMouseUp(MouseEventArgs e);
        bool OnMouseDown(MouseEventArgs e);
        bool OnMouseMove(MouseEventArgs e);
        bool OnMouseWheel(MouseEventArgs e);
        void Render(IDrawArgs drawArgs);
        void Dispose();
    }

}
