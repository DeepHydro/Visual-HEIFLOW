using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using HUST.WREIS.Dot3D.NewWidgets;
using ZedGraph;
using Heiflow.Visualization.Renderable.Grid;
using Heiflow.Core;

namespace HUST.WREIS.Dot3D.Display
{

    public class GridLengendBar : Plugin3D
    {
        string basePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

        GridLengend lengend = null;
        private IDX3DLayerRender mGrid;

        public GridLengendBar(SceneWindow sw)
            : base(sw)
        {

        }

        public IDX3DLayerRender RenderObject 
        {
            get
            {
                return mGrid;
            }
            set
            {
                mGrid = value;
                lengend.RenderObject = value;
            } 
        }

        public override void Load()
        {
            string title = SceneWindow.ResMan.GetString("Lengend");
            m_form = new FormWidget(title);
            m_form.ClientSize = new System.Drawing.Size(100, 270);
            m_form.Location = new System.Drawing.Point(SceneWindow.Width - 130, SceneWindow.Height - 300);
            m_form.BackgroundColor = World.Settings.WidgetBackgroundColor;
            m_form.VerticalScrollbarEnabled = true;
            m_form.HorizontalScrollbarEnabled = true;
            m_form.BorderEnabled = true;
            m_form.AutoHideHeader = false;
            m_form.OnResizeEvent += new FormWidget.ResizeHandler(m_form_OnResizeEvent);
            m_form.FormSizeChanged += new EventHandler(m_form_FormSizeChanged);
            m_form.OnVisibleChanged += new VisibleChangedHandler(OnFormVisibleChanged);
            m_form.LocationChanged += new EventHandler(m_form_LocationChanged);
            m_form.Initialize(SceneWindow.DrawArgs);
          
            m_form.Visible = true;

            int width = m_form.ClientSize.Width - 4;
            int height = m_form.ClientSize.Height - m_form.HeaderHeight - 4;
            lengend = new  GridLengend();
            lengend.Location = new System.Drawing.Point(2, 2);
            lengend.ParentWidget = m_form;
            lengend.ClientSize = new System.Drawing.Size(width, height);
            lengend.Visible = true;
            m_form.ChildWidgets.Add(lengend);
            DrawArgs.NewRootWidget.ChildWidgets.Add(m_form);
            base.Load();
        }

        void m_form_FormSizeChanged(object sender, EventArgs e)
        {
            if (lengend != null)
            {
                lengend.CreateLengendVertex();
            }
        }

        void m_form_LocationChanged(object sender, EventArgs e)
        {
            if (lengend != null)
            {
                lengend.CreateLengendVertex();
            }
        }

        protected override void OnFormVisibleChanged(object o, VisibleState state)
        {
            World.Settings.NotifyPropertyChanged = true;
            if (state == VisibleState.Visible)
            {
                World.Settings.ShowLengendBar = true;
                this.Visible = true;
            }
            else
            {
                World.Settings.ShowLengendBar = false;
                this.Visible = false;
            }
            base.OnFormVisibleChanged(o, state);
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
            if (m_form != null && lengend != null)
            {
                int height = size.Height - m_form.HeaderHeight - 4;
                if (height < 0)
                    height = 0;

                int width = size.Width - 4;
                if (width < 0)
                    width = 0;
                if (height >= 0 && width >= 0)
                {
                    lengend.ClientSize = new System.Drawing.Size(width, height);
                }
            }
        }
    }

    public class GridLengend : NewWidget
    {
        System.Drawing.Font m_localFont = null;
        Font m_drawingFont = null;
        private CustomVertex.TransformedColored[] verts;
        private IDX3DLayerRender _RenderObject;

        public IDX3DLayerRender RenderObject
        {
            get
            {
                return _RenderObject;
            }
            set
            {
                _RenderObject = value;
                _RenderObject.ColorRampChanged += new EventHandler(mGrid_ColorRampChanged);
                CreateLengendVertex();
            }
        }

        public bool IsInverseColorRamp { get; set; }

        void mGrid_ColorRampChanged(object sender, EventArgs e)
        {
            CreateLengendVertex();
        }

        public GridLengend()
        {
            IsInverseColorRamp = false;
        
        }

        void GridLengend_LocationChanged(object sender, EventArgs e)
        {
            CreateLengendVertex();
        }

        #region Properties
        public System.Drawing.Font Font
        {
            get { return m_localFont; }
            set
            {
                m_localFont = value;
                if (m_drawingFont != null)
                {
                    m_drawingFont.Dispose();
                    m_drawingFont = new Font(DrawArgs.Device, m_localFont);
                }
            }
        }


        #endregion

        #region IWidget Members

        public override void Initialize(DrawArgs drawArgs)
        {
          
        }


        public override void Render(DrawArgs drawArgs)
        {
            try
            {
                if(m_drawingFont == null)
                    m_drawingFont = drawArgs.CreateFont("Arial,宋体", 8.5f, System.Drawing.FontStyle.Bold);

                if (m_Visible && RenderObject != null)
                {
                    if (m_localFont != null && m_drawingFont == null)
                    {
                        m_drawingFont = new Font(drawArgs.device, m_localFont);
                    }

                      DrawTextFormat drawTextFormat = DrawTextFormat.VerticalCenter;


                    drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
                    drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
                    drawArgs.device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
                    drawArgs.device.TextureState[0].AlphaArgument1 = TextureArgument.Diffuse;

                    m_drawingFont.DrawText(
                        null,
                            _RenderObject.MaxCellValue.ToString("0.000"),
                        new System.Drawing.Rectangle(AbsoluteLocation.X + 2, AbsoluteLocation.Y, m_Size.Width, 15),
                        drawTextFormat,
                        m_ForeColor);
                    m_drawingFont.DrawText(
                      null,
                      _RenderObject.MinCellValue.ToString("0.000"),
                      new System.Drawing.Rectangle(AbsoluteLocation.X + 2, AbsoluteLocation.Y + m_Size.Height -15, m_Size.Width, 15),
                      drawTextFormat,
                      m_ForeColor);
                   // CreateLengendVertex();
                    RenderBackbone(drawArgs);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public void CreateLengendVertex()
        {
            if (RenderObject != null)
            {
                int colorCount = RenderObject.ColorRamp.Colors.Length;
                float deltaY = (float)(ClientSize.Height-30) / (float)colorCount;

                if (deltaY > 0)
                {
                    verts = new CustomVertex.TransformedColored[colorCount * 4];
                    int colorIndex = 0;
                    for (int i = 0; i < colorCount; i++)
                    {
                        if(IsInverseColorRamp)
                            colorIndex=i;
                        else
                            colorIndex = colorCount - i - 1; 
                        verts[i * 4 + 0].X = AbsoluteLocation.X + 2;
                        verts[i * 4 + 0].Y = AbsoluteLocation.Y +15 + i * deltaY;
                        verts[i * 4 + 0].Z = 0;
                        verts[i * 4 + 0].Color = RenderObject.ColorRamp.Colors[colorIndex].ToArgb();

                        verts[i * 4 + 1].X = AbsoluteLocation.X + 2;
                        verts[i * 4 + 1].Y = AbsoluteLocation.Y + 15 + (i + 1.0f) * deltaY;
                        verts[i * 4 + 1].Z = 0;
                        verts[i * 4 + 1].Color = RenderObject.ColorRamp.Colors[colorIndex].ToArgb();

                        verts[i * 4 + 2].X = AbsoluteLocation.X + ClientSize.Width;
                        verts[i * 4 + 2].Y = AbsoluteLocation.Y + 15 + i * deltaY;
                        verts[i * 4 + 2].Z = 0;
                        verts[i * 4 + 2].Color = RenderObject.ColorRamp.Colors[colorIndex].ToArgb();

                        verts[i * 4 + 3].X = AbsoluteLocation.X + ClientSize.Width;
                        verts[i * 4 + 3].Y = AbsoluteLocation.Y + 15 + (i + 1.0f) * deltaY;
                        verts[i * 4 + 3].Z = 0;
                        verts[i * 4 + 3].Color = RenderObject.ColorRamp.Colors[colorIndex].ToArgb();
                    }
                }
            }
        }

        private void RenderBackbone(DrawArgs drawArgs)
        {

            bool lighting = drawArgs.device.RenderState.Lighting;
            drawArgs.device.RenderState.Lighting = false;
            Cull cull = drawArgs.device.RenderState.CullMode;
            drawArgs.device.RenderState.CullMode = Cull.CounterClockwise;

            drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;
            drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleStrip, verts.Length, verts);

            drawArgs.device.RenderState.Lighting = lighting;
            drawArgs.device.RenderState.CullMode = cull;
        }


        #endregion
    }
 
}
