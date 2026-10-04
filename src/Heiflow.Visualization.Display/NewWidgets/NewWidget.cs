using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using HUST.WREIS.Dot3D.NewWidgets;

namespace HUST.WREIS.Dot3D.Display
{
    public class NewWidget : HUST.WREIS.Dot3D.NewWidgets.IWidget
    {
        protected System.Drawing.Point m_Location = new System.Drawing.Point(0, 0);
        protected System.Drawing.Size m_Size = new System.Drawing.Size(20, 100);
        protected bool m_Visible = true;
        protected bool m_Enabled = true;
        protected HUST.WREIS.Dot3D.NewWidgets.IWidget m_ParentWidget = null;
        protected object m_Tag = null;
        protected System.Drawing.Color m_ForeColor = System.Drawing.Color.White;
        protected string m_Name = "";

        public NewWidget()
        {
        }

        /// <summary>
        /// CountHeight property value
        /// </summary>
        protected bool m_countHeight = true;

        /// <summary>
        /// CountWidth property value
        /// </summary>
        protected bool m_countWidth = true;

        #region IWidget Members
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        public HUST.WREIS.Dot3D.NewWidgets.IWidget ParentWidget
        {
            get
            {
                return m_ParentWidget;
            }
            set
            {
                m_ParentWidget = value;
            }
        }

        public bool Visible
        {
            get
            {
                return m_Visible;
            }
            set
            {
                m_Visible = value;
            }
        }

        public object Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }

        public HUST.WREIS.Dot3D.NewWidgets.IWidgetCollection ChildWidgets
        {
            get
            {
                return null;
            }
            set
            {

            }
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
        /// <summary>
        /// / Location of this widget relative to the client area of the parent
        /// </summary>
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


        /// New IWidget properties

        /// <summary>
        /// Location of this widget relative to the client area of the parent
        /// </summary>
        public System.Drawing.Point Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        /// <summary>
        /// Size of widget in pixels
        /// </summary>
        public System.Drawing.Size WidgetSize
        {
            get { return m_Size; }
            set { m_Size = value; }
        }


        /// <summary>
        /// Whether this widget should count for height calculations - HACK until we do real layout
        /// </summary>
        public bool CountHeight
        {
            get { return m_countHeight; }
            set { m_countHeight = value; }
        }


        /// <summary>
        /// Whether this widget should count for width calculations - HACK until we do real layout
        /// </summary>
        public bool CountWidth
        {
            get { return m_countWidth; }
            set { m_countWidth = value; }
        }

        public virtual void Initialize(DrawArgs drawArgs)
        {
        }

        public virtual void Render(DrawArgs drawArgs)
        {
        }
        #endregion

        public System.Drawing.Color ForeColor
        {
            get
            {
                return m_ForeColor;
            }
            set
            {
                m_ForeColor = value;
            }
        }
    }
}
