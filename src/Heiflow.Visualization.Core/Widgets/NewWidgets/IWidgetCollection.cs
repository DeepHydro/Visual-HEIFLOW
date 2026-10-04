
using System;
using System.Collections;
using System.Windows.Forms;

using HUST.WREIS.Dot3D;

namespace HUST.WREIS.Dot3D.NewWidgets
{
	/// <summary>
	/// Collection of IWidgets
	/// </summary>
	public interface IWidgetCollection
	{
		#region Methods
		void BringToFront(int index);
		void BringToFront(IWidget widget);
		void Add(IWidget widget);
		void Clear();
		void Insert(IWidget widget, int index);
		IWidget RemoveAt(int index);
		void Remove (IWidget widget);
		#endregion

		#region Properties
		int Count{get;}
		#endregion

		#region Indexers
		IWidget this[int index] {get;set;}
		#endregion
	}

}
