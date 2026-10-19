using System;
using System.IO;
using System.Drawing;
using HUST.WREIS.Dot3D;
using System.Xml;

using Utility;
using Heiflow.Core;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;
using Color = System.Drawing.Color;

namespace HUST.WREIS.Dot3D.Renderable
{
    /// <summary>
    /// This class Loads and Renders at a specific lat,lon,alt a given
    /// Model(ie Textured Mesh) in Direct X or Other supported Format
    /// </summary>
    public class ModelFeature:HUST.WREIS.Dot3D.Renderable.RenderableObject
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Scale { get; set; }
        public float Altitude { get; set; }
        public float Rotx { get; set; }
       public float Roty  { get; set; }
        public float Rotz { get; set; }

        public float vertExaggeration = 1;

        public bool isVertExaggerable { get; set; }
		public float currentElevation = 0;
        public bool isElevationRelative2Ground { get; set; }

		Vector3 worldXyz; // XYZ World coordinates
		string meshFileName;
		Mesh mesh;
		Texture[] meshTextures;            // Textures for the mesh
		static Material[] meshMaterials;
		string errorMsg;

        public ModelFeature(string name,World parentWorld,string fileName,float Latitude,
            float Longitude,float Altitude,float Scale,float rotX,float rotY,float rotZ)
            : base(name,parentWorld) 
		{
			meshFileName = fileName;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Altitude = Altitude;
            this.Scale = Scale;
            this.Rotx = rotX;
            this.Roty = rotY;
            this.Rotz = rotZ;
		}

		/// <summary>
		/// Determine if the object is visible
		/// </summary>
		bool IsVisible(HUST.WREIS.Dot3D.Camera.CameraBase camera)
		{
			if(worldXyz == Vector3.Zero)
				worldXyz = MathEngine.SphericalToCartesian(Latitude, Longitude, camera.WorldRadius );
			return camera.ViewFrustum.ContainsPoint(worldXyz);
		}

		public override void Render(DrawArgs drawArgs)
		{
			if(errorMsg != null)
			{
				//System.Windows.Forms.MessageBox.Show( errorMsg, "Model failed to load.", MessageBoxButtons.OK, MessageBoxIcon.Error);
				errorMsg = null;
				IsOn = false;
				isInitialized = false;
				return;
			}

            //if(!IsVisible(drawArgs.WorldCamera))
            //{
            //    // Mesh is not in view, unload it to save memory
            //    if(isInitialized)
            //        Dispose();
            //    return;
            //}

			if(!isInitialized)
				return;
            drawArgs.device.SetRenderState(RenderState.ZEnable, true);
			drawArgs.device.SetRenderState(RenderState.CullMode, (int)(Cull.None));
			drawArgs.device.SetRenderState(RenderState.Lighting, true);
			drawArgs.device.SetRenderState(RenderState.Ambient, 0x808080);
			drawArgs.device.SetRenderState(RenderState.NormalizeNormals, true);
			
			LightHub.Get(drawArgs.device, 0).Diffuse = ColorUtil.ToColor4(Color.FromArgb(255, 255, 255));
			LightHub.Get(drawArgs.device, 0).Type = LightType.Directional; 
			LightHub.Get(drawArgs.device, 0).Direction = new Vector3(1f,1f,1f);
			LightHub.Get(drawArgs.device, 0).Enabled = true; 

			drawArgs.device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
			drawArgs.device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);

			drawArgs.device.SetRenderState(RenderState.AlphaBlendEnable, true);
			drawArgs.device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
			drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);

			// Put the light somewhere up in space
			LightHub.Get(drawArgs.device, 0).Position = new Vector3(
				(float)worldXyz.X*2f,
				(float)worldXyz.Y*1f,
				(float)worldXyz.Z*1.5f);

            Matrix currentWorld = drawArgs.device.GetTransform(TransformState.World);
            drawArgs.device.SetTransform(TransformState.World, Matrix.RotationX((float)MathEngine.DegreesToRadians(Rotx)));
            
            
            

            // Move the mesh to desired location on earth
            //if (isVertExaggerable == true)
            //    vertExaggeration = World.Settings.VerticalExaggeration;
            //else vertExaggeration = 1;
            //
            //
            //


            //

            if (isVertExaggerable == true)
                vertExaggeration = World.Settings.VerticalExaggeration;
            else vertExaggeration = 1;
            double radius = drawArgs.WorldCamera.WorldRadius + (currentElevation * Convert.ToInt16(isElevationRelative2Ground) + Altitude) * vertExaggeration;
            double radCosLat = radius * Math.Cos(MathEngine.DegreesToRadians(Latitude));
            Point3d absPosition = new Point3d(radCosLat * Math.Cos(MathEngine.DegreesToRadians(Longitude)),
                                radCosLat * Math.Sin(MathEngine.DegreesToRadians(Longitude)),
                                radius * Math.Sin(MathEngine.DegreesToRadians(Latitude)));
            Point3d refPosition = absPosition - drawArgs.WorldCamera.ReferenceCenter;

            
            
             

            // 2.�豸ͶӰ�任����   
            // ģ���������ת������   
            //Matrix currentWorld = drawArgs.device.GetTransform(TransformState.World);
            //drawArgs.device.SetTransform(TransformState.World, Matrix.RotationX((float)MathEngine.DegreesToRadians(Rotx)));
            //
            //
            //

            //// ��ģ������������Ӧλ��   
            //// Move the mesh to desired location on earth   
            //if (isVertExaggerable == true)
            //    vertExaggeration = World.Settings.VerticalExaggeration;
            //else vertExaggeration = 1;
            //
            //
            //

            //// ���豸����������������׼   
            //  

            int i = 0;
            try
            {
                // 3.����ģ��   
                for ( i = 0; i < meshMaterials.Length; i++)
                {
                    // Set the material and texture for this subset
                    drawArgs.device.Material = meshMaterials[i];
                    if (i < meshTextures.Length)
                    {
                        drawArgs.device.SetTexture(0, meshTextures[i]);
                    }
                    // Draw the mesh subset
                    mesh.DrawSubset(i);
                }
            }
            catch
            {
                throw new Exception(this.name + ":meshMaterials length" + meshMaterials.Length + ";meshTextures length:" + meshTextures .Length+ "; current index:" + i);
            }

			drawArgs.device.SetTransform(TransformState.World, currentWorld);
			drawArgs.device.SetRenderState(RenderState.Lighting, false);
		}
	
		
		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
			if(!IsVisible(drawArgs.WorldCamera))
				return;

			try
			{
                if(meshFileName.EndsWith(".x"))
                    LoadDirectXMesh(drawArgs);
                else if(meshFileName.EndsWith(".dae")||meshFileName.EndsWith(".xml"))
                    LoadColladaMesh(drawArgs);
                if (mesh == null)
                    throw new SharpDXException();
				
				vertExaggeration = World.Settings.VerticalExaggeration;
				if (isElevationRelative2Ground==true)
					currentElevation = World.TerrainAccessor.GetElevationAt(Latitude, Longitude);
				

				isInitialized = true;
			}
			catch(Exception caught)
			{
				Log.Write( caught );
				errorMsg = "Failed to read mesh from " + meshFileName;
			}
		}

        /// <summary>
        /// Method to load Native Direct X Meshes
        /// </summary>
        private void LoadDirectXMesh(DrawArgs drawArgs)
        {
            Heiflow.Visualization.DirectX.ExtendedMaterial[] materials;
			//DataStream adj;
            //drawArgs.device.SetRenderState(RenderState.ZEnable, true);
			mesh = NativeD3DX.LoadMeshFromX(drawArgs.device, meshFileName, MeshFlags.Managed, out materials);

			// Extract the material properties and texture names.
			meshTextures  = new Texture[materials.Length];					
			meshMaterials = new Material[materials.Length];
			string xFilePath = Path.GetDirectoryName(meshFileName);
			for(int i = 0; i < materials.Length; i++)
			{
				meshMaterials[i] = materials[i].Material3D;
				// Set the ambient color for the material (D3DX does not do this)
				meshMaterials[i].Ambient = meshMaterials[i].Diffuse;

				// Create the texture.
				if(materials[i].TextureFilename!=null)
				{
					string textureFilePath = Path.Combine(xFilePath, materials[i].TextureFilename);
					meshTextures[i] = TextureLoader.FromFile(drawArgs.device, textureFilePath);				
				}
			}
        }

        private void LoadColladaMesh(DrawArgs drawArgs)
        {

            XmlDocument colladaDoc = new XmlDocument();
            colladaDoc.Load(meshFileName);
            XmlNodeList geometryNodes = colladaDoc.SelectNodes("geometry");
            if (geometryNodes != null)
            {
                foreach (XmlNode geometryNode in geometryNodes)
                {
                    //Get Mesh Source Points
                    XmlNodeList sourceNodes = geometryNode.SelectNodes("mesh/source");
                    //Get Means to parse Mesh vertices
                    XmlNode vertexNode = geometryNode.SelectSingleNode("mesh/vertices");
                    //Get Means to parse Mesh edges
                    XmlNode lineNode = geometryNode.SelectSingleNode("mesh/lines");
                    //Get Faces/Triangles
                    XmlNodeList triangleNodes = geometryNode.SelectNodes("mesh/triangles");

                    //Store is a new Mesh object
                }
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
			else if ((vertExaggeration != World.Settings.VerticalExaggeration)&&(isVertExaggerable==true))
				Initialize(drawArgs);
			else if ((isElevationRelative2Ground==true)&&(currentElevation != World.TerrainAccessor.GetElevationAt(Latitude, Longitude)))
				Initialize(drawArgs);

		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Dispose()
		{
			isInitialized = false;
			if(mesh!=null)
				mesh.Dispose();
			if(meshTextures!=null)
				foreach(Texture t in meshTextures)
					if(t!=null)
						t.Dispose();
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

    }
}
