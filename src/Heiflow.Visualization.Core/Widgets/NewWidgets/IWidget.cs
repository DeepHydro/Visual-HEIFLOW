
using System;
using System.Collections;
using System.Windows.Forms;

using HUST.WREIS.Dot3D;

namespace HUST.WREIS.Dot3D.NewWidgets
{	
	/// <summary>
	/// Base Interface for DirectX GUI Widgets
	/// </summary>
	public interface IWidget
	{
		#region Properties

		/// <summary>
		/// Name of this widget
		/// </summary>
		string Name{get;set;}

		/// <summary>
		/// Location of this widget relative to the client area of the parent
		/// </summary>
		System.Drawing.Point Location {get;set;}

		/// <summary>
		/// Where this widget is on the window
		/// </summary>
		System.Drawing.Point AbsoluteLocation{get;}

		/// <summary>
		/// The top left corner of this widget's client area
		/// </summary>
		System.Drawing.Point ClientLocation{get;}

		/// <summary>
		/// Size of widget in pixels
		/// </summary>
		System.Drawing.Size WidgetSize{get;set;}

		/// <summary>
		/// Size of the client area in pixels
		/// </summary>
        System.Drawing.Size ClientSize { get;set;}

		/// <summary>
		/// Whether this widget is enabled
		/// </summary>
		bool Enabled{get;set;}

		/// <summary>
		/// Whether this widget is visible
		/// </summary>
		bool Visible{get;set;}

		/// <summary>
		/// Whether this widget should count for height calculations - HACK until we do real layout
		/// </summary>
		bool CountHeight{get; set;}

		/// <summary>
		/// Whether this widget should count for width calculations - HACK until we do real layout
		/// </summary>
		bool CountWidth{get; set;}

		/// <summary>
		/// The parent widget of this widget
		/// </summary>
		IWidget ParentWidget{get;set;}

		/// <summary>
		/// List of children widgets
		/// </summary>
		IWidgetCollection ChildWidgets{get;set;}

		/// <summary>
		/// A link to an object.
		/// </summary>
		object Tag{get;set;}
		#endregion

		#region Methods

		/// <summary>
		/// The render method to draw this widget on the screen.
		/// 
		/// Called on the GUI thread.
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		void Render(DrawArgs drawArgs);


		/// <summary>
		/// Initializes the button by loading the texture, creating the sprite and figure out the scaling.
		/// 
		/// Called on the GUI thread.
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		void Initialize(DrawArgs drawArgs);

		#endregion
	}
}
