using System;
using System.Collections.Generic;

namespace HUST.WREIS.Dot3D.NewWidgets
{
	/// <summary>
	/// Summary description for WidgetCollection.
	/// </summary>
	public class WidgetCollection : HUST.WREIS.Dot3D.NewWidgets.IWidgetCollection
	{
        List<IWidget> m_ChildWidgets = new List<IWidget>();
		
		public WidgetCollection()
		{
		}

		#region Methods
		public void BringToFront(int index)
		{
			HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index];
			if(currentWidget != null)
			{
				m_ChildWidgets.RemoveAt(index);
				m_ChildWidgets.Insert(0, currentWidget);
			}
		}

		public void BringToFront(HUST.WREIS.Dot3D.NewWidgets.IWidget widget)
		{
			int foundIndex = -1;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index];
				if(currentWidget != null)
				{		
					if(currentWidget == widget)
					{
						foundIndex = index;
						break;
					}
				}	
			}

			if(foundIndex > 0)
			{
				BringToFront(foundIndex);
			}	
		}

		public void Add(HUST.WREIS.Dot3D.NewWidgets.IWidget widget)
		{
			m_ChildWidgets.Add(widget);
		}

		public void Clear()
		{
			m_ChildWidgets.Clear();
		}

		public void Insert(HUST.WREIS.Dot3D.NewWidgets.IWidget widget, int index)
		{
			if(index <= m_ChildWidgets.Count)
			{
				m_ChildWidgets.Insert(index, widget);
			}
			//probably want to throw an indexoutofrange type of exception
		}

		public HUST.WREIS.Dot3D.NewWidgets.IWidget RemoveAt(int index)
		{
			if(index < m_ChildWidgets.Count)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget oldWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;
				m_ChildWidgets.RemoveAt(index);
				return oldWidget;
			}
			else
			{
				return null;
			}
		}

		public void Remove(HUST.WREIS.Dot3D.NewWidgets.IWidget widget)
		{
			int foundIndex = -1;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				HUST.WREIS.Dot3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as HUST.WREIS.Dot3D.NewWidgets.IWidget;
				if(currentWidget != null)
				{		
					if(currentWidget == widget)
					{
						foundIndex = index;
						break;
					}
				}	
			}

			if(foundIndex >= 0)
			{
				m_ChildWidgets.RemoveAt(foundIndex);
			}
		}
		#endregion

		#region Properties
		public int Count
		{
			get
			{
				return m_ChildWidgets.Count;
			}
		}

		#endregion

		#region Indexers
		public HUST.WREIS.Dot3D.NewWidgets.IWidget this[int index]
		{
			get
			{
				return m_ChildWidgets[index] ;
			}
			set
			{
				m_ChildWidgets[index] = value;
			}
		}
		#endregion

	}
}
