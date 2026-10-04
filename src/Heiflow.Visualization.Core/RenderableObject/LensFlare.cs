//----------------------------------------------------------------------------
// NAME: Lens Flare
// VERSION: 1.1
// DESCRIPTION: Render a simple lens flare effect
// DEVELOPER: Stephan 'stepman' Mantler, Patrick Murris
// WEBSITE: http://www.stephanmantler.com/
//----------------------------------------------------------------------------
//  1.1 Apr  8, 2007	Settings are saved + new 'colored' flares by patmurris
//			Supports a variable number of flares - up to 12 (could be more)
//  1.0	Feb 21, 2007	First version by stepman "only took about two hours to write, and I needed a break"
//----------------------------------------------------------------------------
// This file is in the Public Domain, and comes with no warranty. 
//
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;

namespace HUST.WREIS.Dot3D
{
	/// <summary>
	/// Display a lens flare effect when sun is in view 
	/// </summary>
	public class LensFlareEffect : RenderableObject
	{
        	string version = "1.1";
        	//string pluginName = "Lens Flare";
        	string settingsFileName = "LensFlare.ini";
        	string pluginPath;

	//	MainApplication ww;
        	Texture[] m_flareTextures;
        	SurfaceDescription m_flareSurfaceDescription;
        	Sprite m_sprite;
      //  	LensFlarePlugin plugin;
     //   	LensFlareOptionsDialog m_dialog;

		public int m_flareCount = 0;
        	public string[] m_flareFiles;
        	public float[] m_flareSizes;
        	public float[] m_flarePositions;
        	public float[] m_flareOpacities;

            public LensFlareEffect(string pluginDirectory)
                : base("Lens Flare", Vector3.Empty, Quaternion.Identity)
		{
            	//	this.plugin = plugin;
		//	this.ww = app;
			this.pluginPath = pluginDirectory;
			
			// We want to be drawn on top of everything else
			this.RenderPriority = RenderPriority.Icons;

			// true to make this layer active on startup, this is equal to the checked state in layer manager
			this.IsOn = true;

			// Max 12 flares
            		m_flareFiles = new string[12];
            		m_flareSizes = new float[12];
            		m_flarePositions = new float[12];
            		m_flareOpacities = new float[12];

			ReadSettings();
            
		}

	        /// <summary>
	        /// Read saved settings from ini file
	        /// </summary>
	        public void ReadSettings()
	        {
			string line = "";
			m_flareCount = 0;
			int l = 0;
			try 
			{
				TextReader tr = File.OpenText(Path.Combine(pluginPath, settingsFileName));
				while ((line = tr.ReadLine()) != null) 
				{
					if(line.Length > 2) {
						if(l == 0) {
							// First line : general settings
							string[] settingsList = line.Split(';');
							string saveVersion = settingsList[0];	// version when settings where saved
						} else {
							// Other lines : flare list
							string[] settingsList = line.Split(';');
							m_flareFiles[m_flareCount] = settingsList[0];
							m_flareSizes[m_flareCount] = float.Parse(settingsList[1], CultureInfo.InvariantCulture);
							m_flarePositions[m_flareCount] = float.Parse(settingsList[2], CultureInfo.InvariantCulture);
							m_flareOpacities[m_flareCount] = float.Parse(settingsList[3], CultureInfo.InvariantCulture);
							m_flareCount++;
						}
						l++;
					}
				}
				tr.Close();
			}
			catch(Exception caught) 
			{
				MessageBox.Show(line + " : " + caught.ToString(), "Error reading settings", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

	        /// <summary>
	        /// Save settings in ini file
	        /// </summary>
	        public void SaveSettings()
	        {
	            string line = "";
	            try
	            {
				line = version;
				StreamWriter sw = new StreamWriter(Path.Combine(pluginPath, settingsFileName));
				sw.WriteLine(line);
				if(m_flareCount > 0) {
					for(int flare = 0; flare < m_flareCount; flare++) {
						line = m_flareFiles[flare] + ";"
						+ m_flareSizes[flare] + ";"
						+ m_flarePositions[flare] + ";"
						+ m_flareOpacities[flare];
						sw.WriteLine(line);
					}
				}
				sw.Close();
	            }
	            catch { }
	        }


		/// <summary>
		/// Plugin entry point - All plugins must implement this function
		/// </summary>
		public void Load()
		{
		}

		public void ShowOptions( object sender, EventArgs e )
        	{
		//	m_dialog = new LensFlareOptionsDialog(this);
		//	m_dialog.Icon = ww.Icon;
		//	m_dialog.Show();
        	}


		/// <summary>
		/// This is where we do our rendering 
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override void Render(DrawArgs drawArgs)
		{
			if (!isInitialized) 
				return;

			Point3d sunPosition = -SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);

			Point3d sunSpherical = MathEngine.CartesianToSphericalD(sunPosition.X, sunPosition.Y, sunPosition.Z);
			sunPosition = MathEngine.SphericalToCartesianD(
				Angle.FromRadians(sunSpherical.Y),
				Angle.FromRadians(sunSpherical.Z),
				150000000000);

			Vector3 sunVector = new Vector3((float)sunPosition.X, (float)sunPosition.Y, (float)sunPosition.Z);

            		// don't draw if sun below horizon
			Frustum viewFrustum = new Frustum();

			float aspectRatio = (float)drawArgs.WorldCamera.Viewport.Width / drawArgs.WorldCamera.Viewport.Height;
			Matrix projectionMatrix = Matrix.PerspectiveFovRH((float)drawArgs.WorldCamera.Fov.Radians, aspectRatio, 10000.0f, 300000000000);

			viewFrustum.Update(
				Matrix.Multiply(drawArgs.WorldCamera.AbsoluteWorldMatrix,
				Matrix.Multiply(drawArgs.WorldCamera.AbsoluteViewMatrix,
					projectionMatrix)));

			if (!viewFrustum.ContainsPoint(sunVector))
				return;

			Vector3 translationVector = new Vector3(
				(float)(sunPosition.X - drawArgs.WorldCamera.ReferenceCenter.X),
				(float)(sunPosition.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
				(float)(sunPosition.Z - drawArgs.WorldCamera.ReferenceCenter.Z));

			Vector3 projectedPoint = drawArgs.WorldCamera.Project(translationVector);

            // abuse view pick ray to see if sun position is above horizon
            Angle prLon,prLat;
            drawArgs.WorldCamera.PickingRayIntersection((int)projectedPoint.X, (int)projectedPoint.Y, out prLon, out prLat);

            if(!Angle.IsNaN(prLon) || !Angle.IsNaN(prLat))
                return;
            
            BlendOperation abo = drawArgs.device.RenderState.AlphaBlendOperation;
            Blend sb = drawArgs.device.RenderState.SourceBlend;
            Blend db = drawArgs.device.RenderState.DestinationBlend;

			m_sprite.Begin(SpriteFlags.AlphaBlend);

            drawArgs.device.RenderState.AlphaBlendOperation = BlendOperation.Add;
            drawArgs.device.RenderState.SourceBlend = Blend.SourceAlpha;
            drawArgs.device.RenderState.DestinationBlend = Blend.One;

            int m_flareWidth = 144;
            int m_flareHeight = 144;

			// Render icon
			float xscale = (float)m_flareWidth / m_flareSurfaceDescription.Width;
			float yscale = (float)m_flareHeight / m_flareSurfaceDescription.Height;

            Vector3 sunScreen = new Vector3(projectedPoint.X, projectedPoint.Y, 0);
            Vector3 center = new Vector3(drawArgs.screenWidth/2.0f, drawArgs.screenHeight/2.0f, 0);
            Vector3 flareDir = center - sunScreen;

            for(int flare=0; flare < m_flareCount; flare++)
            {
    			m_sprite.Transform = Matrix.Scaling(xscale * m_flareSizes[flare], yscale * m_flareSizes[flare], 0);
    			m_sprite.Transform *= Matrix.Translation(sunScreen + m_flarePositions[flare] * flareDir);
    			m_sprite.Draw(m_flareTextures[flare],
    				new Vector3(m_flareSurfaceDescription.Width >> 1, m_flareSurfaceDescription.Height >> 1, 0),
    				Vector3.Empty,
    				System.Drawing.Color.FromArgb((int)m_flareOpacities[flare],253, 253, 253).ToArgb());
            }

			// Reset transform to prepare for text rendering later
			m_sprite.Transform = Matrix.Identity;
			m_sprite.End();

            drawArgs.device.RenderState.AlphaBlendOperation = abo;
            drawArgs.device.RenderState.SourceBlend = sb;
            drawArgs.device.RenderState.DestinationBlend = db;
        }

		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
	        public override void Initialize(DrawArgs drawArgs)
	        {
	            Device device = drawArgs.device;
		    int i = 0;
	            try
	            {
			if (m_flareTextures == null)
			{
                		m_flareTextures= new Texture[m_flareCount];
				for(i = 0; i < m_flareCount; i++)
                    m_flareTextures[i] = ImageHelper.LoadTexture(Path.Combine(pluginPath, m_flareFiles[i]));

				// Assume all flares have same size
				m_flareSurfaceDescription = m_flareTextures[0].GetLevelDescription(0);
			}

			if (m_sprite == null)
			{
				m_sprite = new Sprite(drawArgs.device);
			}

	                isInitialized = true;
	            }
	            catch
	            {
	                isOn = false;
	                MessageBox.Show("Error loading texture " + Path.Combine(pluginPath, m_flareFiles[i]) + ".", "Layer initialization failed.", MessageBoxButtons.OK,
	                    MessageBoxIcon.Error);
	            }
	        }

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
            		if (!isInitialized)
                		Initialize(drawArgs);
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Dispose()
		{
			if (!isInitialized) return;
			if (m_flareTextures != null)
			{
                		for(int i = 0; i < m_flareCount; i++)
					m_flareTextures[i].Dispose();
			}

			if (m_sprite == null)
			{
				m_sprite.Dispose();
			}
            //if(m_dialog != null)
            //{
            //    try 
            //    {
            //        m_dialog.Close();
            //        m_dialog.Dispose();
            //    }
            //    catch {}
            //}
	                isInitialized = false;
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}
	}	
}
