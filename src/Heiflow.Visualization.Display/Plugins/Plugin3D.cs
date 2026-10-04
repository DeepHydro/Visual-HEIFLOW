using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUST.WREIS.Dot3D.NewWidgets;
using Heiflow.Core.Plugin;

namespace HUST.WREIS.Dot3D
{
    public class Plugin3D : Plugin
    {
        public SceneWindow SceneWindow { get; private set; }

        protected FormWidget m_form = null;
      public  System.Windows.Controls.MenuItem MenuItem { get; set; }

        public Plugin3D(SceneWindow sw)
        {
            SceneWindow = sw;
        }

        public override void Hide()
        {
            if (m_form != null)
                m_form.Hide();
            this.Visible = false;
        }

        public override void Show()
        {
            if (m_form != null)
                m_form.Show();
            this.Visible = true;
        }

        protected virtual void OnFormVisibleChanged(object o, VisibleState state)
        {
            if (state == VisibleState.Visible)
            {    
                this.Visible = true;
                if (MenuItem != null)
                    MenuItem.IsChecked = true;
            }
            else
            { 
                this.Visible = false;
                if (MenuItem != null)
                    MenuItem.IsChecked = false;
            }
        }
    }
}
