using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using HUST.WREIS.Dot3D.NewWidgets;
using ZedGraph;
using Heiflow.Visualization.Renderable.Grid;
using Heiflow.Core;

using SharpDX;
using Rectangle = System.Drawing.Rectangle;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

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
                // The widget is made while the plugin loads and the layer can be handed over before
                // that, so it is given to the widget only once the widget is there.
                if (lengend != null)
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
            // The layer was kept aside if it arrived before the widget existed, and this is where it
            // reaches the widget.
            if (mGrid != null)
                lengend.RenderObject = mGrid;
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
                    m_drawingFont = FontFactory.Create(DrawArgs.Device, m_localFont);
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
                    m_drawingFont = drawArgs.CreateFont("Arial,宋体", 12f, System.Drawing.FontStyle.Bold);

                if (m_Visible && RenderObject != null)
                {
                    if (m_localFont != null && m_drawingFont == null)
                    {
                        m_drawingFont = FontFactory.Create(drawArgs.device, m_localFont);
                    }

                      FontDrawFlags FontDrawFlags = FontDrawFlags.VerticalCenter;


                    drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
                    drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Diffuse);
                    drawArgs.device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.SelectArg1);
                    drawArgs.device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Diffuse);

                    m_drawingFont.DrawText(
                        null,
                            _RenderObject.MaxCellValue.ToString("0.000"),
                        new System.Drawing.Rectangle(AbsoluteLocation.X + 2, AbsoluteLocation.Y, m_Size.Width, 15),
                        FontDrawFlags,
                        m_ForeColor);
                    m_drawingFont.DrawText(
                      null,
                      _RenderObject.MinCellValue.ToString("0.000"),
                      new System.Drawing.Rectangle(AbsoluteLocation.X + 2, AbsoluteLocation.Y + m_Size.Height -15, m_Size.Width, 15),
                      FontDrawFlags,
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
            if (RenderObject == null || RenderObject.ColorRamp == null)
            {
                // There is no ramp to show, and the corners of the last one that was built must not
                // stay behind and be drawn as if they were still the ramp.
                verts = null;
                return;
            }

            int colorCount = RenderObject.ColorRamp.Colors.Length;
            // Fifteen pixels are left above the bar for the largest value and as much below it for the
            // smallest one, which is what the text at both ends is drawn into.
            float deltaY = (float)(ClientSize.Height - 30) / (float)colorCount;

            if (deltaY <= 0 || ClientSize.Width <= 4)
                return;

            float left = AbsoluteLocation.X + 2;
            float right = AbsoluteLocation.X + ClientSize.Width - 2;

            // Two triangles for every band of the ramp, drawn as a list. A strip would carry on from
            // the band that ends into the one that begins and make two triangles of its own across the
            // join, so every band would be smeared into the one next to it.
            verts = new CustomVertex.TransformedColored[colorCount * 6];
            for (int i = 0; i < colorCount; i++)
            {
                int colorIndex = IsInverseColorRamp ? i : colorCount - i - 1;
                int color = RenderObject.ColorRamp.Colors[colorIndex].ToArgb();
                float top = AbsoluteLocation.Y + 15 + i * deltaY;
                float bottom = AbsoluteLocation.Y + 15 + (i + 1.0f) * deltaY;

                int v = i * 6;
                SetVertex(verts, v + 0, left, top, color);
                SetVertex(verts, v + 1, left, bottom, color);
                SetVertex(verts, v + 2, right, top, color);

                SetVertex(verts, v + 3, right, top, color);
                SetVertex(verts, v + 4, left, bottom, color);
                SetVertex(verts, v + 5, right, bottom, color);
            }
        }

        /// <summary>
        /// Writes one corner of a band. A vertex that is already in screen coordinates has to carry the
        /// reciprocal of the homogeneous w as well, without it the corner is thrown away and nothing of
        /// the bar reaches the screen.
        /// </summary>
        private static void SetVertex(CustomVertex.TransformedColored[] vertices, int index, float x, float y, int color)
        {
            vertices[index].X = x;
            vertices[index].Y = y;
            vertices[index].Z = 0;
            vertices[index].Rhw = 1.0f;
            vertices[index].Color = color;
        }

        private void RenderBackbone(DrawArgs drawArgs)
        {
            // Nothing is built while the ramp is missing or while there is no room for the bar, and
            // drawing would be reading corners that were never written.
            if (verts == null || verts.Length < 3)
                return;

            bool lighting = drawArgs.device.GetRenderStateBool(RenderState.Lighting);
            drawArgs.device.SetRenderState(RenderState.Lighting, false);
            // The bands are flat rectangles with no back to them, so the way their corners turn should
            // not be what decides whether they are drawn at all.
            Cull cull = drawArgs.device.GetRenderStateEnum<Cull>(RenderState.CullMode);
            drawArgs.device.SetRenderState(RenderState.CullMode, (int)(Cull.None));

            drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;
            drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
            // What is asked for is how many triangles there are and not how many corners, and three
            // corners make one.
            drawArgs.device.DrawUserPrimitives(PrimitiveType.TriangleList, verts.Length / 3, verts);

            drawArgs.device.SetRenderState(RenderState.Lighting, lighting);
            drawArgs.device.SetRenderState(RenderState.CullMode, cull);
        }


        #endregion
    }
 
}
