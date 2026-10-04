using System;
using System.Linq;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using HUST.WREIS.Dot3D.Menu;
using HUST.WREIS.Dot3D;

namespace HUST.WREIS.Dot3D
{
   


	/// <summary>
	/// Summary description for LineGraph.
	/// </summary>
    public class LineGraph : HUST.WREIS.Dot3D.NewWidgets.IWidget
	{
		float m_Min = 0f;
		float m_Max = 50.0f;

		float[] m_Values = new float[0];

		System.Drawing.Point m_Location = new System.Drawing.Point(100,100);
		System.Drawing.Size m_Size = new System.Drawing.Size(300,100);
		System.Drawing.Color m_BackgroundColor = System.Drawing.Color.FromArgb(100, 0, 0, 0);
		System.Drawing.Color m_LineColor = System.Drawing.Color.Red;

		bool m_Visible = false;
		bool m_ResetVerts = true;

		public bool Visible
		{
			get { return m_Visible; }
			set { m_Visible = value; }
		}

		public float[] Values
		{
			get
			{
				return m_Values;
			}
			set
			{
				m_Values = value;
				m_ResetVerts = true;
                if (m_Values != null && m_Values.Length > 0)
                {
                    m_Min = (from v in m_Values select v).Min();
                    m_Max = (from v in m_Values select v).Max();
                }
			}
		}

		public System.Drawing.Color BackgroundColor
		{
			get
			{
				return m_BackgroundColor;
			}
			set
			{
				m_BackgroundColor = value;
			}
		}

		public System.Drawing.Color LineColor
		{
			get
			{
				return m_LineColor;
			}
			set
			{
				m_LineColor = value;
				m_ResetVerts = true;
			}
		}

		public System.Drawing.Point Location
		{
			get{ return m_Location; }
			set
			{
				if(m_Location != value)
				{
					m_Location = value;
					m_ResetVerts = true;
				}
			 }
		}

		public System.Drawing.Size Size
		{
			get{ return m_Size; }
			set
			{
				if(m_Size != value)
				{
					m_Size = value;
					m_ResetVerts = true;
				}
			}
		}

		public LineGraph()
		{
            Name = "LineGraph";
		}

		CustomVertex.TransformedColored[] m_Verts = new Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored[0];

		public void Render(DrawArgs drawArgs)
		{
			if(!m_Visible)
				return;

            MenuUtils.DrawBox(AbsoluteLocation.X, AbsoluteLocation.Y,
            m_Size.Width,
            m_Size.Height,
            0.0f,
            m_BackgroundColor.ToArgb(),
            drawArgs.device);

			if(m_Values == null || m_Values.Length == 0)
				return;

			float xIncr = (float)m_Size.Width / (float)m_Values.Length;
            float yIncr = (float)m_Size.Height / (m_Max - m_Min);

			m_Verts = new CustomVertex.TransformedColored[m_Values.Length];

			if(m_ResetVerts)
			{
				for(int i = 0; i < m_Values.Length; i++)
				{
                    m_Verts[i].Y = AbsoluteLocation.Y+(m_Values[i] - m_Min) * yIncr;
                    m_Verts[i].X = AbsoluteLocation.X  + i * xIncr;
					m_Verts[i].Z = 0.0f;
					m_Verts[i].Color = m_LineColor.ToArgb();
				}
			}

			drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
			drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;

			drawArgs.device.VertexFormat = CustomVertex.TransformedColored.Format;
			drawArgs.device.DrawUserPrimitives(
				PrimitiveType.LineStrip,
				m_Verts.Length - 1,
				m_Verts);

		}

        bool m_Enabled = true;
        HUST.WREIS.Dot3D.NewWidgets.IWidget m_ParentWidget = null;
        //object m_Tag = null;
        //System.Drawing.Color m_ForeColor = System.Drawing.Color.White;
        //string m_Name = "";
        //System.Drawing.Font m_localFont = null;
        //Font m_drawingFont = null;
        /// <summary>
        /// CountHeight property value
        /// </summary>
        protected bool m_countHeight = true;

        /// <summary>
        /// CountWidth property value
        /// </summary>
        protected bool m_countWidth = true;

        #region IWidget ≥…‘±

        public string Name
        {
            get;
            set;
        }

        public System.Drawing.Point AbsoluteLocation
        {
            get
            {
                if (m_ParentWidget != null)
                {
                    return new System.Drawing.Point(
                        m_Location.X + m_ParentWidget.ClientLocation.X,
                        m_Location.Y + m_ParentWidget.ClientLocation.Y);

                }
                else
                {
                    return m_Location;
                }
            }
        }

        public System.Drawing.Point ClientLocation
        {
            get
            {
                return m_Location;
            }
            set
            {
                m_Location = value;
            }
        }

        public System.Drawing.Size WidgetSize
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        public System.Drawing.Size ClientSize
        {
            get
            {
                return m_Size;
            }
            set
            {
                m_Size = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
            }
        }

        public bool CountHeight
        {
            get { return m_countHeight; }
            set { m_countHeight = value; }
        }

        public bool CountWidth
        {
            get { return m_countWidth; }
            set { m_countWidth = value; }
        }

        public NewWidgets.IWidget ParentWidget
        {
            get { return m_ParentWidget; }
            set { m_ParentWidget = value; }
        }

        public NewWidgets.IWidgetCollection ChildWidgets
        {
            get;
            set;
        }

        public object Tag
        {
            get;
            set;
        }

        public void Initialize(DrawArgs drawArgs)
        {
           
        }

        #endregion
    }
}
