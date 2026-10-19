using System;
using System.IO;

using Utility;
using Heiflow.Core;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

namespace HUST.WREIS.Dot3D.Renderable
{
	/// <summary>
	/// Summary description for MeshLayer.
	/// </summary>
	public class MeshLayer : RenderableObject
	{
		Mesh mesh;
		string meshFilePath;
		Heiflow.Visualization.DirectX.ExtendedMaterial[] materials;
		Material[] meshMaterials;

		float lat;
		float lon;
		float layerRadius;
		float scaleFactor;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.MeshLayer"/> class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <param name="layerRadius"></param>
		/// <param name="scaleFactor"></param>
		/// <param name="meshFilePath"></param>
		/// <param name="orientation"></param>
		public MeshLayer(string name, float latitude, float longitude, float layerRadius, float scaleFactor, string meshFilePath, Quaternion orientation) : base(name, MathEngine.SphericalToCartesian(latitude, longitude, layerRadius), orientation) 
		{
			this.meshFilePath = meshFilePath;
			this.lat = latitude;
			this.lon = longitude;
			this.layerRadius = layerRadius;
			this.scaleFactor = scaleFactor;
		}

		public override void Initialize(DrawArgs drawArgs)
		{
			try
			{
				this.mesh = NativeD3DX.LoadMeshFromX(drawArgs.device, this.meshFilePath, MeshFlags.Managed, out this.materials);
				this.meshMaterials = new Material[this.materials.Length];
				//using(StreamWriter sw = new StreamWriter("mat.txt", true, System.Text.Encoding.ASCII))
			{
				//sw.WriteLine(this.meshMaterials.Length.ToString());
				for(int i = 0; i < this.materials.Length; i++)
				{
					this.meshMaterials[i] = this.materials[i].Material3D;
					this.meshMaterials[i].Ambient = this.meshMaterials[i].Diffuse;
				}
			}

				//NativeD3DX.ComputeNormals(this.mesh);
			}
			catch(Exception caught)
			{
				Log.Write( caught );
			}
			this.isInitialized = true;
		}

		public override void Dispose()
		{
			// Don't dispose the mesh!
		}

		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		public override void Update(DrawArgs drawArgs)
		{
			if(!this.isInitialized)
				this.Initialize(drawArgs);

		}

		public override void Render(DrawArgs drawArgs)
		{
//			Vector3 here = MathEngine.SphericalToCartesian(drawArgs.WorldCamera.Latitude, drawArgs.WorldCamera.Longitude, this.layerRadius);
			Matrix currentWorld = drawArgs.device.GetTransform(TransformState.World);
			drawArgs.device.SetRenderState(RenderState.Lighting, true);
			drawArgs.device.SetRenderState(RenderState.ZEnable, true);
			LightHub.Get(drawArgs.device, 0).Diffuse = ColorUtil.ToColor4(System.Drawing.Color.White); 
			LightHub.Get(drawArgs.device, 0).Type = LightType.Point; 
			LightHub.Get(drawArgs.device, 0).Range = 100000;
			LightHub.Get(drawArgs.device, 0).Position = new Vector3(this.layerRadius, 0, 0);
			LightHub.Get(drawArgs.device, 0).Enabled = true ; 

			drawArgs.device.SetRenderState(RenderState.CullMode, (int)(Cull.None));
			drawArgs.device.SetTransform(TransformState.World, Matrix.Identity);
			
			//
			
			
			
			
			
			//
		
			drawArgs.device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);
			drawArgs.device.SetRenderState(RenderState.NormalizeNormals, true);
		
			
			for( int i = 0; i < this.meshMaterials.Length; i++ )
			{
				drawArgs.device.Material = this.meshMaterials[i];
				this.mesh.DrawSubset(i);
			}
			
			drawArgs.device.SetTransform(TransformState.World, currentWorld);
			drawArgs.device.SetRenderState(RenderState.CullMode, (int)(Cull.Clockwise));
			drawArgs.device.SetRenderState(RenderState.Lighting, false);
		}
	}
}
