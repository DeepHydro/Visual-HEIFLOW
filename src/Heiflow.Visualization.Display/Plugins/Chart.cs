using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUST.WREIS.Dot3D.NewWidgets;

namespace HUST.WREIS.Dot3D.Display
{
    public class Chart3D : Plugin3D
    {
        string basePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

        //FormWidget m_form = null;
        LineGraph m_LineGraph = null;

        public LineGraph LineGraph
        {
            get
            {
                return m_LineGraph;
            }
        }

        public Chart3D(SceneWindow sw)
            : base(sw)
        {

        }

        public override void Load()
        {
            m_form = new FormWidget("水流速度曲线");
            m_form.Location = new System.Drawing.Point(10, SceneWindow.Height -310);
            m_form.ClientSize = new System.Drawing.Size(400, 300);
            m_form.BackgroundColor = World.Settings.WidgetBackgroundColor;

            m_form.AutoHideHeader = true;
            m_form.VerticalScrollbarEnabled = true;
            m_form.HorizontalScrollbarEnabled = true;
            m_form.BorderEnabled = true;
            m_form.AutoHideHeader = false;
            m_form.OnResizeEvent += new FormWidget.ResizeHandler(m_form_OnResizeEvent);
        
            m_form.Initialize(SceneWindow.DrawArgs);
            m_LineGraph = new  LineGraph();
            m_LineGraph.Location = new System.Drawing.Point(0, 0);
            //m_LineGraph.Font = new System.Drawing.Font("Ariel", 10.0f, System.Drawing.FontStyle.Bold);
            m_LineGraph.ParentWidget = m_form;
            m_LineGraph.Size = new System.Drawing.Size(m_form.ClientSize.Width, m_form.ClientSize.Height - m_form.HeaderHeight);
            m_LineGraph.Visible = true;
            m_form_OnResizeEvent(m_form, m_form.WidgetSize);
            float[] values = new float[500];
            for (int i = 0; i < 500; i++)
            {
                values[i] = (float)Math.Sin(i);
            }
            m_LineGraph.Values = values;
            m_form.ChildWidgets.Add(m_LineGraph);
            m_form.Visible = true;

            DrawArgs.NewRootWidget.ChildWidgets.Add(m_form);

            base.Load();
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
    
        void m_form_OnResizeEvent(object IWidget, System.Drawing.Size size)
        {
            if (m_form != null && m_LineGraph != null)
            {
                int height = m_form.WidgetSize.Height - m_form.HeaderHeight;
                if (height < 0)
                    height = 0;
                int width=m_form.WidgetSize.Width;
                if (width < 0)
                    width = 0;
                m_LineGraph.ClientSize = new System.Drawing.Size(width, height);
            }
        }
    }
}
