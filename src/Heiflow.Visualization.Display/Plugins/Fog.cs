//----------------------------------------------------------------------------
// NAME: Fog
// VERSION: 1.0
// DESCRIPTION: Add range fog. Adds itself as a layer in Layer Manager (key: L). Right click on layer for settings. 
// DEVELOPER: Patrick Murris
// WEBSITE: http://www.alpix.com/worldwin
//----------------------------------------------------------------------------
// Based on Bjorn Reppen 'Atmosphere' and 'RangeFog' plugin
// 1.1 June 19, 2005	Settings dialog, intensity select, read/save settings
// 1.0 June 15, 2005	First version
//----------------------------------------------------------------------------

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Drawing;


using System.IO;
using System;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Camera;
using Utility;

namespace HUST.WREIS.Dot3D.Display.Plugins
{
    /// <summary>
	/// The plugin (main class)
	/// </summary>
	public class Fog:Plugin3D
	{
		/// <summary>
		/// Name displayed in layer manager
		/// </summary>
		public static string LayerName = "Fog";

        public Fog(SceneWindow sw)
            : base(sw)
        {
        }

		/// <summary>
		/// Plugin entry point - All plugins must implement this function
		/// </summary>
		public override void Load() 
		{
            FogLayer layer = new FogLayer(LayerName, PluginDirectory, SceneWindow);
            layer.IsOn = World.Settings.EnableFog;
            SceneWindow.CurrentWorld.RenderableObjects.ChildObjects.Insert(0, layer);
           // SceneWindow.CurrentWorld.DefaultLayerList.Add(layer);
		}

		/// <summary>
		/// Unloads our plugin
		/// </summary>
		public override void Unload() 
		{
            SceneWindow.CurrentWorld.RenderableObjects.Remove(LayerName);
		}

	}

	/// <summary>
	/// Fog layer
	/// </summary>
	public class FogLayer : RenderableObject
	{
		private World world;
        private DrawArgs drawArgs;

		Color fogColor = Color.FromArgb(208, 208, 208);
		
		/// <summary>
		/// Constructor
		/// </summary>
        public FogLayer(string LayerName, string pluginPath, SceneWindow worldWindow)
            : base(LayerName)
		{
			this.world = worldWindow.CurrentWorld;
			this.drawArgs = worldWindow.DrawArgs;
			//this.RenderPriority = (RenderPriority)-1; // rendered before all others
			this.RenderPriority = RenderPriority.SurfaceImages;       
		}

		#region RenderableObject

		/// <summary>
		/// This is where we do our rendering 
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override void Render(DrawArgs drawArgs)
		{
			if(!isInitialized)
				return;
			// Camera & Device shortcuts ;)
			CameraBase camera = drawArgs.WorldCamera;
			Device device = drawArgs.device;

			// Set fog params
			float a = (float)camera.Altitude;
            //device.RenderState.ZBufferEnable = true;
            //device.RenderState.Ambient = Color.FromArgb(255, 200, 200, 200);
			device.RenderState.FogEnable = true;
			device.RenderState.FogColor = Color.FromArgb(World.Settings.FogColor);
			device.RenderState.FogTableMode = FogMode.Linear;
            device.RenderState.FogStart = a * World.Settings.FogNearFactor;
            device.RenderState.FogEnd = a * World.Settings.FogFarFactor; ;
		}

		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
			isInitialized = true;
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
			if(!isInitialized)
				Initialize(drawArgs);
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Dispose()
		{
			isInitialized = false;
		}

		/// <summary>
		/// Gets called when user left clicks.
		/// RenderableObject abstract member (needed)
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

 		/// <summary>
 		/// Fills the context menu with menu items specific to the layer.
 		/// </summary>
 		public override void BuildContextMenu( ContextMenu menu )
 		{
  			menu.MenuItems.Add("Properties", new System.EventHandler(OnPropertiesClick));
 		}

 		/// <summary>
 		/// Properties context menu clicked.
 		/// </summary>
 		protected override void  OnPropertiesClick(object sender, EventArgs e)
 		{
 		}
		#endregion
	}
}
