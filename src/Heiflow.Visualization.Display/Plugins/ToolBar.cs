using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUST.WREIS.Dot3D.NewWidgets;

namespace HUST.WREIS.Dot3D.Display
{
    public class ToolBar : Plugin3D
    {

        public ToolBar(SceneWindow sw)
            : base(sw)
        {
        }

        public override void Load()
        {
            m_form = new FormWidget("工具栏");
            m_form.ClientSize = new System.Drawing.Size(400, 45);
            m_form.Location = new System.Drawing.Point(10, SceneWindow.Height - 320);
            m_form.BackgroundColor = World.Settings.WidgetBackgroundColor;
            m_form.AutoHideHeader = true;
            m_form.VerticalScrollbarEnabled = false;
            m_form.HorizontalScrollbarEnabled = true;
            m_form.BorderEnabled = false;
            m_form.Initialize(SceneWindow.DrawArgs);       

            m_form.OnResizeEvent += new FormWidget.ResizeHandler(m_form_OnResizeEvent);
            m_form.OnVisibleChanged += new VisibleChangedHandler(OnFormVisibleChanged);
            //  mPbox.Opacity = 220;
            m_form_OnResizeEvent(m_form, m_form.WidgetSize);
            m_form.Visible = true;
            DrawArgs.NewRootWidget.ChildWidgets.Add(m_form);
            AddButtonItem();
            base.Load();
        }

        public void AddButtonItem()
        {
            ButtonWidget buttion = new ButtonWidget()
            {
                ClientSize = new System.Drawing.Size(32, 32),
                Location = new System.Drawing.Point(2, 4),
                ImageName = ConfigurationManager.Engine3DSettings.IconPath + "modis-flood.png",
                LeftClickAction = ButtonItemClick
            };
            m_form.ChildWidgets.Add(buttion);
        }

        void ButtonItemClick(System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Hello!");
        }


        void m_form_OnResizeEvent(object IWidget, System.Drawing.Size size)
        {
        }

        public override void Unload()
        {
            if (m_form != null)
            {
                DrawArgs.NewRootWidget.ChildWidgets.Remove(m_form);
                m_form.Dispose();
                m_form = null;
            }
            base.Unload();
        }

      
    }
}
