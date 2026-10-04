//----------------------------------------------------------------------------
// NAME: Sky
// VERSION: 1.3
// DESCRIPTION: Renders a textured sky dome above the camera. Adds itself as a layer in Layer Manager (key: L). Right click on layer for settings.
// DEVELOPER: Patrick Murris
// WEBSITE: http://www.alpix.com/3d/worldwin
//----------------------------------------------------------------------------
// Based on Bjorn Reppen 'Atmosphere' plugin
// 1.3 Nov   5, 2006	WW 1.4 : added 'recenter' fix in Render()
// 1.2 Nov  13, 2005	uncommented mashi's clipping code, raised trigger alt to 200Km
// 1.1 June 18, 2005	Settings dialog, bitmap select, read/save settings
// 1.0 June 15, 2005	First version, one bitmap
//----------------------------------------------------------------------------

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;

using System.IO;
using System;
using HUST.WREIS.Dot3D.Renderable;
using HUST.WREIS.Dot3D.Camera;
using Utility;
using Heiflow.Core;

namespace HUST.WREIS.Dot3D.Display.Plugins
{
	/// <summary>
	/// The plugin (main class)
	/// </summary>
	public class Sky :Plugin3D
	{
		/// <summary>
		/// Name displayed in layer manager
		/// </summary>
		public static string LayerName = "Sky";
        public Sky(SceneWindow sw)
            : base(sw)
        {
        }
		/// <summary>
		/// Plugin entry point - All plugins must implement this function
		/// </summary>
		public override void Load() 
		{
            SkyLayer layer = new SkyLayer(LayerName, PluginDirectory, SceneWindow);
         //   SceneWindow.CurrentWorld.RenderableObjects.ChildObjects.Insert(1, layer);
       //     SceneWindow.CurrentWorld.PlugInList.Add(layer);
            layer.IsOn = true;
            SceneWindow.CurrentWorld.DefaultLayerList.Add(layer);
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
	/// Sky dome
	/// </summary>
	public class SkyLayer : RenderableObject
	{
		static string version = "1.3";
		string settingsFileName = "sky.ini";
		string pluginPath;
		public World world;
		public DrawArgs drawArgs;
		Texture texture;
		Form pDialog;

		// default sky bitmap
		public string textureFileName = "Sky_Day4.jpg";

		/// <summary>
		/// Constructor
		/// </summary>
        public SkyLayer(string LayerName, string pluginPath, SceneWindow worldWindow)
            : base(LayerName)
		{
            this.pluginPath = pluginPath + "\\SkyAndFog\\";
			this.world = worldWindow.CurrentWorld;
			this.drawArgs = worldWindow.DrawArgs;
			this.RenderPriority = RenderPriority.SurfaceImages;
			ReadSettings();
		}
		
		/// <summary>
		/// Read saved settings from ini file
		/// </summary>
		public void ReadSettings()
		{
			string line = "";
			try 
			{
				TextReader tr = File.OpenText(Path.Combine(pluginPath, settingsFileName));
				line = tr.ReadLine();
				tr.Close();
			}
            catch (Exception caught) 
            {
                Log.Write(caught); 
            }
			if(line != "")
			{
				string[] settingsList = line.Split(';');
				string saveVersion = settingsList[1];	// version when settings where saved
				if(settingsList[1] != null) textureFileName = settingsList[1];
			}
		}

		/// <summary>
		/// Save settings in ini file
		/// </summary>
		public void SaveSettings()
		{
			string line = version + ";" + textureFileName;
			try
			{
				StreamWriter sw = new StreamWriter(Path.Combine(pluginPath, settingsFileName));
				sw.Write(line);
				sw.Close();
			}
			catch(Exception caught) 
            {
                Log.Write(caught);
            }
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

			if(camera.Altitude > 200e3) return;

			double distToCenterOfPlanet = (camera.Altitude + camera.WorldRadius);
			double tangentalDistance  = Math.Sqrt( distToCenterOfPlanet*distToCenterOfPlanet - camera.WorldRadius*camera.WorldRadius);
			double domeRadius = tangentalDistance*0.5;
			double domeToCenterOfPlanet = Math.Sqrt(camera.WorldRadius*camera.WorldRadius - domeRadius*domeRadius) - 2e3;

			// Create sky dome
			Mesh skyMesh = TexturedDome(device, (float)domeRadius, 24, 12);
			// set texture
			device.SetTexture(0,texture);
			device.TextureState[0].ColorOperation = TextureOperation.BlendCurrentAlpha;
                	drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
                	drawArgs.device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
                	drawArgs.device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
                	drawArgs.device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
 			device.VertexFormat = CustomVertex.PositionTextured.Format;
			
			// save world and projection transform
			Matrix origWorld = device.Transform.World;
			Matrix origProjection = device.Transform.Projection;

			// move sky dome
			Matrix skyTrans;
			Angle camLat = camera.Latitude;
			Angle camLon = camera.Longitude;
			//Vector3 groundPos = MathEngine.SphericalToCartesian(camLat, camLon, this.world.EquatorialRadius);
			skyTrans = Matrix.Translation(0,0,(float)domeToCenterOfPlanet);
			skyTrans = Matrix.Multiply(skyTrans, Matrix.RotationY(-(float)camLat.Radians+(float)Math.PI/2));
			skyTrans = Matrix.Multiply(skyTrans, Matrix.RotationZ((float)camLon.Radians));

			device.Transform.World = skyTrans;

			// Recenter world
			Recenter(drawArgs);
			
			// Save fog status
			bool origFog = device.RenderState.FogEnable;
			device.RenderState.FogEnable = false;

			// Set new one (to avoid being clipped) - probably better ways of doing this?
			float aspectRatio =  (float)device.Viewport.Width / device.Viewport.Height;
			device.Transform.Projection = Matrix.PerspectiveFovRH((float)camera.Fov.Radians, aspectRatio, 1, float.MaxValue );

			// draw
			skyMesh.DrawSubset(0);

			// Restore device states
			device.Transform.World = origWorld;
			device.Transform.Projection = origProjection;
			device.RenderState.FogEnable = origFog;
			// dispose of sky - for now
			skyMesh.Dispose();
		}

		// Recenter world projection in WW 1.4
		public void Recenter(DrawArgs drawArgs) 
        {
			drawArgs.device.Transform.World *= Matrix.Translation(
   				(float)-drawArgs.WorldCamera.ReferenceCenter.X,
   				(float)-drawArgs.WorldCamera.ReferenceCenter.Y,
   				(float)-drawArgs.WorldCamera.ReferenceCenter.Z
   				);
		}

		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
				texture = TextureLoader.FromFile(drawArgs.device, Path.Combine(pluginPath, textureFileName));
				isInitialized = true;	
			}
			catch
			{
				isOn = false;
				MessageBox.Show("Error loading texture " + Path.Combine(pluginPath, textureFileName) + ".","Layer initialization failed.", MessageBoxButtons.OK, 
					MessageBoxIcon.Error );
			}
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
			if(texture!=null)
			{
				texture.Dispose();
				texture = null;
			}
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
        protected override void OnPropertiesClick(object sender, EventArgs e)
 		{
			if(pDialog != null && ! pDialog.IsDisposed)
				// Already open
				return;

			// Display the dialog
			pDialog = new propertiesDialog(this);
			pDialog.Show();

 		}

		/// <summary>
		/// Properties Dialog
		/// </summary>
		public class propertiesDialog : System.Windows.Forms.Form
		{
			private System.Windows.Forms.Label lblTexture;
			private System.Windows.Forms.ComboBox cboTexture;
			private System.Windows.Forms.Button btnOK;
			private System.Windows.Forms.Button btnCancel;
			private SkyLayer layer;

			public propertiesDialog( SkyLayer layer )
			{
				InitializeComponent();
				//this.Icon = WorldWind.PluginEngine.Plugin.Icon;
				this.layer = layer;
				// Init texture list with *.jpg and *.png
				DirectoryInfo di = new DirectoryInfo(layer.pluginPath);
				FileInfo[] imgFiles = di.GetFiles("*.jpg");
				cboTexture.Items.AddRange(imgFiles);
				imgFiles = di.GetFiles("*.png");
				cboTexture.Items.AddRange(imgFiles);
				// select current bitmap
				int i = cboTexture.FindString(layer.textureFileName);
				if(i != -1) cboTexture.SelectedIndex = i;
			}

			#region Windows Form Designer generated code
			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this.btnCancel = new System.Windows.Forms.Button();
				this.btnOK = new System.Windows.Forms.Button();
				this.lblTexture = new System.Windows.Forms.Label();
				this.cboTexture = new System.Windows.Forms.ComboBox();
				this.SuspendLayout();
				// 
				// btnCancel
				// 
				this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				this.btnCancel.Location = new System.Drawing.Point(311, 59);
				this.btnCancel.Name = "btnCancel";
				this.btnCancel.TabIndex = 0;
				this.btnCancel.Text = "Cancel";
				this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
				// 
				// btnOK
				// 
				this.btnOK.Location = new System.Drawing.Point(224, 59);
				this.btnOK.Name = "btnOK";
				this.btnOK.TabIndex = 1;
				this.btnOK.Text = "OK";
				this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
				// 
				// lblTexture
				// 
				this.lblTexture.AutoSize = true;
				this.lblTexture.Location = new System.Drawing.Point(16, 28);
				this.lblTexture.Name = "lblTexture";
				this.lblTexture.Size = new System.Drawing.Size(82, 16);
				this.lblTexture.TabIndex = 2;
				this.lblTexture.Text = "Sky texture:";
				// 
				// cboTexture
				// 
				this.cboTexture.Location = new System.Drawing.Point(96, 25);
				this.cboTexture.Name = "cboTexture";
				this.cboTexture.Size = new System.Drawing.Size(296, 21);
				this.cboTexture.TabIndex = 3;
				this.cboTexture.Text = "Select texture file";
				this.cboTexture.DropDownStyle = ComboBoxStyle.DropDownList;
				this.cboTexture.MaxDropDownItems = 10;
				// 
				// frmFavorites
				// 
				this.AcceptButton = this.btnOK;
				this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
				this.CancelButton = this.btnCancel;
				this.ClientSize = new System.Drawing.Size(406, 94);
				this.ControlBox = false;
				this.Controls.Add(this.cboTexture);
				this.Controls.Add(this.lblTexture);
				this.Controls.Add(this.btnOK);
				this.Controls.Add(this.btnCancel);
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Name = "pDialog";
				this.ShowInTaskbar = false;
				this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
				//this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
				//this.Location = new System.Drawing.Point(layer.drawArgs.CurrentMousePosition.X + 10, layer.drawArgs.CurrentMousePosition.Y - 10);
				this.Text = "Sky properties";
				this.TopMost = true;
				this.ResumeLayout(false);

			}
			#endregion

			private void btnOK_Click(object sender, System.EventArgs e)
			{
				if(cboTexture.SelectedItem != null) 
				{
  					//System.Windows.Forms.MessageBox.Show("Texture : " + cboTexture.SelectedItem.ToString());
					layer.Dispose();
					layer.textureFileName = cboTexture.SelectedItem.ToString();
					layer.Initialize(layer.drawArgs);
					layer.SaveSettings();
				}
				// Close this form
				this.Close();
			}

			private void btnCancel_Click(object sender, System.EventArgs e)
			{

				// Close this form
				this.Close();
			}





		}



		/// <summary>
		/// Creates a PositionNormalTextured dome above X-Y plane
		/// </summary>
		/// <param name="device">The current direct3D drawing device.</param>
		/// <param name="radius">The dome's radius</param>
		/// <param name="slices">Number of slices (Horizontal resolution).</param>
		/// <param name="stacks">Number of stacs. (Vertical resolution)</param>
		/// <returns></returns>
		/// <remarks>
		/// Number of vertices in the dome will be (slices+1)*(stacks+1)<br/>
		/// Number of faces	:slices*stacks*2
		/// Number of Indexes	: Number of faces * 3;
		/// </remarks>
		private Mesh TexturedDome(Device device, float radius, int slices, int stacks)
		{
			int numVertices = (slices+1)*(stacks+1);
			int numFaces	= slices*stacks*2;
			int indexCount	= numFaces * 3;

			Mesh mesh = new Mesh(numFaces,numVertices,MeshFlags.Managed,CustomVertex.PositionNormalTextured.Format,device);

			// Get the original sphere's vertex buffer.
			int [] ranks = new int[1];
			ranks[0] = mesh.NumberVertices;
			System.Array arr = mesh.VertexBuffer.Lock(0,typeof(CustomVertex.PositionNormalTextured),LockFlags.None,ranks);

			// Set the vertex buffer
			int vertIndex=0;
			for(int stack=0;stack<=stacks;stack++)
			{
				double latitude = (float)stack/stacks*(float)90.0;
				for(int slice=0;slice<=slices;slice++)
				{
					CustomVertex.PositionNormalTextured pnt = new CustomVertex.PositionNormalTextured();
					double longitude = 180 - ((float)slice/slices*(float)360);
					Vector3 v = MathEngine.SphericalToCartesian( latitude, longitude, radius);
					pnt.X = v.X;
					pnt.Y = v.Y;
					pnt.Z = v.Z;
					pnt.Tu = (float)slice/slices;
					pnt.Tv = 1.0f-(float)stack/stacks;
					arr.SetValue(pnt,vertIndex++);
				}
			}

			mesh.VertexBuffer.Unlock();
			ranks[0]=indexCount;
			arr = mesh.LockIndexBuffer(typeof(short),LockFlags.None,ranks);
			int i=0;
			short bottomVertex = 0;
			short topVertex = 0;
			for(short x=0;x<stacks;x++)
			{
				bottomVertex = (short)((slices+1)*x);
				topVertex = (short)(bottomVertex + slices + 1);
				for(int y=0;y<slices;y++)
				{
					arr.SetValue(bottomVertex,i++);
					arr.SetValue((short)(topVertex+1),i++);
					arr.SetValue(topVertex,i++);
					arr.SetValue(bottomVertex,i++);
					arr.SetValue((short)(bottomVertex+1),i++);
					arr.SetValue((short)(topVertex+1),i++);
					bottomVertex++;
					topVertex++;
				}
			}
			mesh.IndexBuffer.SetData(arr,0,LockFlags.None);
			mesh.ComputeNormals();

			return mesh;
		}


		#endregion
	}
}
