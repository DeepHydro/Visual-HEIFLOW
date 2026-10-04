using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HUST.WREIS.Dot3D.Renderable
{

	/// <summary>
	/// One icon in an icon layer
	/// </summary>
	public class Icon : RenderableObject
	{
		public double OnClickZoomAltitude = double.NaN;
		public double OnClickZoomHeading = double.NaN;
		public double OnClickZoomTilt = double.NaN;
		public string SaveFilePath = null;
        public System.DateTime LastRefresh = System.DateTime.MinValue;
		public System.TimeSpan RefreshInterval = System.TimeSpan.MaxValue;
        public System.Windows.Forms.ContextMenu ContextMenu { get; set; }
        public Control ParentControl { get; set; }
		private Angle m_rotation = Angle.Zero;
		private bool m_isRotated = false;
		private Point3d m_positionD = new Point3d();

		bool m_nameAlwaysVisible = false;

     
		public bool NameAlwaysVisible
		{
			get{ return m_nameAlwaysVisible; }
			set{ m_nameAlwaysVisible = value; }
		}

		public bool IsRotated
		{
			get
			{
				return m_isRotated;
			}
			set
			{
				m_isRotated = value;
			}
		}
	
		public Angle Rotation
		{
			get
			{
				return m_rotation;
			}
			set
			{
				m_rotation = value;
			}
		}

		System.Collections.ArrayList overlays = new ArrayList();
		
		//not a good way to handle this
		public void OverlayOnOpen(object o, EventArgs e)
		{
			System.Windows.Forms.MenuItem mi = (System.Windows.Forms.MenuItem)o;

			foreach(ScreenOverlay overlay in overlays)
			{
				if(overlay == null)
					continue;

				if(overlay.Name.Equals(mi.Text))
				{
					if(!overlay.IsOn)
						overlay.IsOn = true;
				}
			}
		}

		public ScreenOverlay[] Overlays
		{
			get
			{
				if(overlays == null)
				{
					return null;
				}
				else
				{
					return (ScreenOverlay[])overlays.ToArray(typeof(ScreenOverlay));
				}
			}
		}

		public void AddOverlay(ScreenOverlay overlay)
		{
			if(overlay != null)
				overlays.Add(overlay);
		}

		public void RemoveOverlay(ScreenOverlay overlay)
		{
			for(int i = 0; i < overlays.Count; i++)
			{
				ScreenOverlay curOverlay = (ScreenOverlay)overlays[i];
				if(curOverlay.IconImagePath == overlay.IconImagePath && overlay.Name == curOverlay.Name)
				{
					overlays.RemoveAt(i);
				}
			}
		}

		#region private members

		/// <summary>
		/// On-Click browse to location
		/// </summary>
		protected string m_clickableActionURL;

		/// <summary>
		/// Latitude (North/South) in decimal degrees
		/// </summary>
		protected double m_latitude;

		/// <summary>
		/// Longitude (East/West) in decimal degrees
		/// </summary>
		protected double m_longitude;

		#endregion

		/// <summary>
		/// Longer description of icon (addition to name)
		/// </summary>
		//public string Description;

		/// <summary>
		/// The icon altitude above sea level
		/// </summary>
		public double Altitude;

		/// <summary>
		/// Icon bitmap path. (Overrides Image)
		/// </summary>
		public string TextureFileName;

		/// <summary>
		/// Icon image.  Leave TextureFileName=null if using Image.  
		/// Caller is responsible for disposing the Bitmap when the layer is removed, 
		/// either by calling Dispose on Icon or on the Image directly.
		/// </summary>
		public Bitmap Image;

		/// <summary>
		/// Icon on-screen rendered width (pixels).  Defaults to icon image width.  
		/// If source image file is not a valid GDI+ image format, width may be increased to closest power of 2.
		/// </summary>
		public int Width;

		/// <summary>
		/// Icon on-screen rendered height (pixels).  Defaults to icon image height.  
		/// If source image file is not a valid GDI+ image format, height may be increased to closest power of 2.
		/// </summary>
		public int Height;

		/// <summary>
		/// On-Click browse to location
		/// </summary>
		public string ClickableActionURL
		{
			get
			{
				return m_clickableActionURL;
			}
			set 
			{
				isSelectable = value != null;
				m_clickableActionURL = value;
			}
		}

		public Point3d PositionD
		{
			get{ return m_positionD; }
			set{ m_positionD = value; }
		}

		/// <summary>
		/// The maximum distance (meters) the icon will be visible from
		/// </summary>
		public double MaximumDisplayDistance = double.MaxValue;

		/// <summary>
		/// The minimum distance (meters) the icon will be visible from
		/// </summary>
		public double MinimumDisplayDistance;

		/// <summary>
		/// Bounding box centered at (0,0) used to calculate whether mouse is over icon/label
		/// </summary>
		public Rectangle SelectionRectangle;

		/// <summary>
		/// Latitude (North/South) in decimal degrees
		/// </summary>
		public double Latitude
		{
			get { return m_latitude; }
		}

		/// <summary>
		/// Longitude (East/West) in decimal degrees
		/// </summary>
		public double Longitude
		{
			get { return m_longitude; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		public Icon(string name,double latitude, double longitude) : base( name )
		{
			m_latitude = latitude;
			m_longitude = longitude;
			this.RenderPriority = RenderPriority.Icons;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		public Icon(string name,double latitude, double longitude,double heightAboveSurface) : base( name )
		{
			m_latitude = latitude;
			m_longitude = longitude;
			Altitude = heightAboveSurface;
			this.RenderPriority = RenderPriority.Icons;
		}

		#region Obsolete

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		[Obsolete]
		public Icon(string name,
			double latitude, 
			double longitude,
			double heightAboveSurface, 
			World parentWorld ) : base( name )
		{
			m_latitude = latitude;
			m_longitude = longitude;
			this.Altitude = heightAboveSurface;
			this.RenderPriority = RenderPriority.Icons;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="heightAboveSurface">Icon height (meters) above sea level.</param>
		[Obsolete]
		public Icon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			World parentWorld, 
			Bitmap image,
			int width,
			int height,
			string actionURL) : base( name )
		{
			this.Description = description;
			m_latitude = latitude;
			m_longitude = longitude;
			this.Altitude = heightAboveSurface;
			this.Image = image;
			this.Width = width;
			this.Height = height;
			ClickableActionURL = actionURL;
			this.RenderPriority = RenderPriority.Icons;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.Icon"/> class 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <param name="heightAboveSurface"></param>
		/// <param name="parentWorld"></param>
		/// <param name="TextureFileName"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="actionURL"></param>
		public Icon(string name, 
			string description,
			double latitude, 
			double longitude, 
			double heightAboveSurface,
			World parentWorld, 
			string TextureFileName,
			int width,
			int height,
			string actionURL) : base( name )
		{
			this.Description = description;
			m_latitude = latitude;
			m_longitude = longitude;
			this.Altitude = heightAboveSurface;
			this.TextureFileName = TextureFileName;
			this.Width = width;
			this.Height = height;
			ClickableActionURL = actionURL;
			this.RenderPriority = RenderPriority.Icons;
		}

		#endregion

		/// <summary>
		/// Sets the geographic position of the icon.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		public void SetPosition(double latitude, double longitude)
		{
			m_latitude = latitude;
			m_longitude = longitude;

			// Recalculate XYZ coordinates
			isInitialized = false;
		}

		/// <summary>
		/// Sets the geographic position of the icon.
		/// </summary>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
		/// <param name="altitude">The icon altitude above sea level.</param>
		public void SetPosition(double latitude, double longitude, double altitude)
		{
			m_latitude = latitude;
			m_longitude = longitude;
			Altitude = altitude;

			// Recalculate XYZ coordinates
			isInitialized = false;
		}

		#region RenderableObject methods

		public override void Initialize(DrawArgs drawArgs)
		{
			double samplesPerDegree = 50.0 / (drawArgs.WorldCamera.ViewRange.Degrees);
			double elevation = drawArgs.CurrentWorld.TerrainAccessor.GetElevationAt(m_latitude, m_longitude);
		//	double altitude = (World.Settings.VerticalExaggeration * Altitude + World.Settings.VerticalExaggeration * elevation);
            double altitude = + World.Settings.VerticalExaggeration * elevation;
			Position = MathEngine.SphericalToCartesian(m_latitude, m_longitude, 
				altitude + drawArgs.WorldCamera.WorldRadius);

			m_positionD = MathEngine.SphericalToCartesianD(
				Angle.FromDegrees(m_latitude),
				Angle.FromDegrees(m_longitude),
				altitude + drawArgs.WorldCamera.WorldRadius);

			isInitialized = true;
		}

		/// <summary>
		/// Disposes the icon (when disabled)
		/// </summary>
		public override void Dispose()
		{
			// Nothing to dispose
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			// Handled by parent
			return false;
		}

		Matrix lastView = Matrix.Identity;

		public override void Update(DrawArgs drawArgs)
		{
			if(drawArgs.WorldCamera.ViewMatrix != lastView && drawArgs.CurrentWorld.TerrainAccessor != null && drawArgs.WorldCamera.Altitude < 300000)
			{
				double samplesPerDegree = 50.0 / drawArgs.WorldCamera.ViewRange.Degrees;
				double elevation = drawArgs.CurrentWorld.TerrainAccessor.GetElevationAt(m_latitude, m_longitude, samplesPerDegree);
				double altitude = World.Settings.VerticalExaggeration * Altitude + World.Settings.VerticalExaggeration * elevation;
				Position = MathEngine.SphericalToCartesian(m_latitude, m_longitude, 
					altitude + drawArgs.WorldCamera.WorldRadius);

				lastView = drawArgs.WorldCamera.ViewMatrix;
			}

			if(overlays != null)
			{
				for(int i = 0; i < overlays.Count; i++)
				{
					ScreenOverlay curOverlay = (ScreenOverlay)overlays[i];
					if(curOverlay != null)
					{
						curOverlay.Update(drawArgs);
					}
				}
			}
		}

		public override void Render(DrawArgs drawArgs)
		{
			if(overlays != null)
			{
				for(int i = 0; i < overlays.Count; i++)
				{
					ScreenOverlay curOverlay = (ScreenOverlay)overlays[i];
					if(curOverlay != null && curOverlay.IsOn)
					{
						curOverlay.Render(drawArgs);
					}
				}
			}
		}

		#endregion

		private void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{

		}
	}
}
