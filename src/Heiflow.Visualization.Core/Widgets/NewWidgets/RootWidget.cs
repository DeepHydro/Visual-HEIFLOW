using System;

using HUST.WREIS.Dot3D;

namespace HUST.WREIS.Dot3D.NewWidgets
{
	/// <summary>
	/// Summary description for Widget.
	/// </summary>
	public class RootWidget : WidgetCollection, HUST.WREIS.Dot3D.NewWidgets.IWidget, HUST.WREIS.Dot3D.NewWidgets.IInteractive
	{
		HUST.WREIS.Dot3D.NewWidgets.IWidget m_parentWidget = null;
		HUST.WREIS.Dot3D.NewWidgets.IWidgetCollection m_ChildWidgets = new WidgetCollection();
		System.Windows.Forms.Control m_ParentControl;
		bool m_Initialized = false;

		public RootWidget(System.Windows.Forms.Control parentControl) 
		{
			m_ParentControl = parentControl;
		}

		#region Methods

		public void Initialize(DrawArgs drawArgs)
		{
		}

		public void Render(DrawArgs drawArgs)
		{
			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return;

			for(int index = m_ChildWidgets.Count - 1; index >= 0; index--)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;
				if(currentWidget != null)
				{
					if(currentWidget.ParentWidget == null || currentWidget.ParentWidget != this)
						currentWidget.ParentWidget = this;

					currentWidget.Render(drawArgs);
				}
			}
		}
		#endregion

		#region Properties

		System.Drawing.Point m_location = new System.Drawing.Point(0,0);
		System.Drawing.Point m_ClientLocation = new System.Drawing.Point(0,0);

		public System.Drawing.Point AbsoluteLocation
		{
			get { return m_location; }
			set { m_location = value; }		
		}

		public string Name
		{
			get { return "Main Frame"; }
			set { }
		}
		
		public HUST.WREIS.Dot3D.NewWidgets.IWidget ParentWidget
		{
			get { return m_parentWidget; }
			set { m_parentWidget = value; }
		}

		public HUST.WREIS.Dot3D.NewWidgets.IWidgetCollection ChildWidgets
		{
			get { return m_ChildWidgets; }
			set { m_ChildWidgets = value; }
		}		

		bool m_enabled = true;
		bool m_visible = true;
		object m_tag = null;

		public System.Drawing.Point ClientLocation
		{
			get { return m_ClientLocation; }
			set { }
		}

		public System.Drawing.Size ClientSize
		{
			get
			{
				System.Drawing.Size mySize = m_ParentControl.Size;
				return mySize;
			}
            set
            {
                // ignore attempts to resize the root widget
            }
		}

		public System.Drawing.Size WidgetSize
		{
			get { return m_ParentControl.Size; }
			set { }
		}

		public bool Enabled
		{
			get { return m_enabled; }
			set { m_enabled = value; }
		}

		public bool Visible
		{
			get { return m_visible; }
			set { m_visible = value; }
		}

		protected bool m_countHeight = true;
		protected bool m_countWidth = true;
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
		public System.Drawing.Point Location
		{
			get { return m_location; }
			set { m_location = value; }
		}

		public object Tag
		{
			get { return m_tag; }
			set { m_tag = value; }
		}
		public bool IsInitialized
		{
			get { return m_Initialized;}
			set { m_Initialized = value; }
		}
		#endregion

		#region IInteractive Members

		MouseClickAction m_leftClickAction;
		public MouseClickAction LeftClickAction
		{
			get { return m_leftClickAction; }
			set { m_leftClickAction = value; }
		}	

		MouseClickAction m_rightClickAction;
		public MouseClickAction RightClickAction
		{
			get { return m_rightClickAction; }
			set { m_rightClickAction = value; }
		}	

		public bool OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return false;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;

				if(currentWidget != null && currentWidget is HUST.WREIS.Dot3D.NewWidgets.IInteractive)
				{
					HUST.WREIS.Dot3D.NewWidgets.IInteractive currentInteractive = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IInteractive;

					handled = currentInteractive.OnMouseDown(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return false;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;

				if(currentWidget != null && currentWidget is HUST.WREIS.Dot3D.NewWidgets.IInteractive)
				{
					HUST.WREIS.Dot3D.NewWidgets.IInteractive currentInteractive = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IInteractive;

					handled = currentInteractive.OnMouseUp(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return false;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;

				if(currentWidget != null && currentWidget is HUST.WREIS.Dot3D.NewWidgets.IInteractive)
				{
					HUST.WREIS.Dot3D.NewWidgets.IInteractive currentInteractive = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IInteractive;

					handled = currentInteractive.OnKeyDown(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return false;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;

				if(currentWidget != null && currentWidget is HUST.WREIS.Dot3D.NewWidgets.IInteractive)
				{
					HUST.WREIS.Dot3D.NewWidgets.IInteractive currentInteractive = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IInteractive;

					handled = currentInteractive.OnKeyUp(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

        public bool OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            for (int index = 0; index < m_ChildWidgets.Count; index++)
            {
                IWidget currentWidget = m_ChildWidgets[index] as IWidget;

                if (currentWidget != null && currentWidget is IInteractive)
                {
                    IInteractive currentInteractive = m_ChildWidgets[index] as IInteractive;

                    bool handled = currentInteractive.OnKeyPress(e);
                    if (handled)
                        return handled;
                }
            }
            return false;
        }

		public bool OnMouseEnter(EventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return false;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;

				if(currentWidget != null && currentWidget is HUST.WREIS.Dot3D.NewWidgets.IInteractive)
				{
					HUST.WREIS.Dot3D.NewWidgets.IInteractive currentInteractive = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IInteractive;

					handled = currentInteractive.OnMouseEnter(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return false;

            for (int index = m_ChildWidgets.Count - 1; index >= 0; index--)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;

				if(currentWidget != null && currentWidget is HUST.WREIS.Dot3D.NewWidgets.IInteractive)
				{
					HUST.WREIS.Dot3D.NewWidgets.IInteractive currentInteractive = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IInteractive;

					handled = currentInteractive.OnMouseMove(e);
					if(handled)
						return handled;
				}
			}

			return handled;
		}

		public bool OnMouseLeave(EventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return false;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;

				if(currentWidget != null && currentWidget is HUST.WREIS.Dot3D.NewWidgets.IInteractive)
				{
					HUST.WREIS.Dot3D.NewWidgets.IInteractive currentInteractive = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IInteractive;

					handled = currentInteractive.OnMouseLeave(e);
					if(handled)
						return handled;
				}
			}
			
			return handled;
		}

		public bool OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
		{
			bool handled = false;

			// if we aren't active do nothing.
			if ((!m_visible) || (!m_enabled))
				return false;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;

				if(currentWidget != null && currentWidget is HUST.WREIS.Dot3D.NewWidgets.IInteractive)
				{
					HUST.WREIS.Dot3D.NewWidgets.IInteractive currentInteractive = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IInteractive;

					handled = currentInteractive.OnMouseWheel(e);
					if(handled)
						return handled;
				}
			}
			
			return handled;
		}

		#endregion

		new public void Add(HUST.WREIS.Dot3D.NewWidgets.IWidget widget)
		{
			m_ChildWidgets.Add(widget);
			widget.ParentWidget = this;
		}		
		
		new public void Remove(HUST.WREIS.Dot3D.NewWidgets.IWidget widget)
		{
			m_ChildWidgets.Remove(widget);
		}
	}
}
