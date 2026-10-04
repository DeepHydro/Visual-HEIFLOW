using System;
using System.Collections;
using System.Windows.Forms;

using HUST.WREIS.Dot3D;

namespace HUST.WREIS.Dot3D.NewWidgets
{
	/// <summary>
	/// Delegate for mouse click events
	/// </summary>
	public delegate void MouseClickAction(System.Windows.Forms.MouseEventArgs e);

	/// <summary>
	/// Interface must be implemented in order to recieve user input.
	/// </summary>
	public interface IInteractive
	{
		#region Properties

		/// <summary>
		/// Action to perform when the left mouse button is clicked
		/// </summary>
		MouseClickAction LeftClickAction{set; get;}

		/// <summary>
		/// Action to perform when the right mouse button is clicked
		/// </summary>
		MouseClickAction RightClickAction{set; get;}

		#endregion

		#region Methods

		/// <summary>
		/// Mouse down event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseDown(MouseEventArgs e);

		/// <summary>
		/// Mouse up event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseUp(MouseEventArgs e);

		/// <summary>
		/// Mouse move event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseMove(MouseEventArgs e);

		/// <summary>
		/// Mouse wheel event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>				
		bool OnMouseWheel(MouseEventArgs e);
		
		/// <summary>
		/// Mouse entered this widget event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseEnter(EventArgs e);
		
		/// <summary>
		/// Mouse left this widget event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseLeave(EventArgs e);

		/// <summary>
		/// Key down event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnKeyDown(KeyEventArgs e);
		
		/// <summary>
		/// Key up event handler.
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnKeyUp(KeyEventArgs e);

        /// <summary>
        /// Key press event handler.
        /// </summary>
        /// <param name="e">Event Args</param>
        /// <returns></returns>
        bool OnKeyPress(KeyPressEventArgs e);

		#endregion
	}
}
