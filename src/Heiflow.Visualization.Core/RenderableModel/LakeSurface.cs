using System;

using System.Collections.Generic;
using System.Text;
using HUST.WREIS.Dot3D;
using Utility;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


namespace HUST.WREIS.Dot3D.Renderable
{
    public struct togglestate
    {
        public bool animation,  view_normals;
    };

    // A structure for our custom vertex type
     public struct CUSTOMVERTEX
    {
        public float x, y, z;      // The untransformed, 3D position for the vertex
        public int color;        // The vertex color
    };

    public struct SURFACEVERTEX
    {
        public Vector3 position;
        public float displacement;
    };

    public struct DISPLACEMENT
    {
        public int displacement;
    };

    public enum rendermode
    {
        RM_POINTS = 0,
        RM_WIREFRAME,
        RM_SOLID
    };

    public struct SOFTWARESURFACEVERTEX
    {
      public  float x, y, z;
      public float nx, ny, nz;
      public float tu, tv;
    };

    public class SeaSurface : RenderableObject
    {
        public SeaSurface(Vector3 position, Vector3 normal, int size_x, int size_y, string name, World parentWorld, SeaParameter para)
            : base(name, parentWorld)
        {
            plane = Plane.FromPointNormal(position, normal);
            normal.Normalize();

            // calculate the u and v-vectors
            // take one of two vectors (the one further away from the normal) and force it into the plane
            Vector3 x;
            if (Math.Abs(Vector3.Dot(new Vector3(1, 0, 0), normal)) < Math.Abs(Vector3.Dot(new Vector3(1, 0, 0), normal)))
            {
                x = new Vector3(1, 0, 0);
            }
            else
            {
                x = new Vector3(0, 0, 1);
            }
            u = x - normal * Vector3.Dot(normal, x);
            u.Normalize();
            // get v (cross)
            v = Vector3.Cross(u, normal);

            mPara = para;
            pos = position;
            gridsize_x = size_x + 1;
            gridsize_y = size_y + 1;
            boxfilter = false;
            if (boxfilter)
            {

            }
          
            mPara = para;

            set_displacement_amplitude(0.0f);
            
        }

        public string debugdata;
        public float efficiency;
        public Matrix range;
        public Plane plane, upper_bound, lower_bound;
        public Texture surf_refraction, surf_reflection;
        public Surface depthstencil;

        protected Vector3 normal, u, v, pos;
        protected float min_height, max_height;
        protected int gridsize_x, gridsize_y, rendermode;
        protected bool plane_within_frustum;
        VertexBuffer surf_software_vertices;
        IndexBuffer surf_indicies;

        //Texture surf_texture, surf_fresnel, underwater_fresnel, noise2D;
        Effect surf_software_effect = null;
        Effect underwater_software_effect = null;


     //   VertexBuffer skybox_vertices;
   //     IndexBuffer skybox_indicies;
        Effect skybox_effect = null;

       // CubeTexture sky_cubemap;

        private SeaParameter mPara;
        private bool initialized, boxfilter;

        public override void Initialize(DrawArgs drawArgs)
        {
            if (!initbuffers(drawArgs.device))
                initialized = false;
            if (initialized && skybox_effect != null && underwater_software_effect != null && surf_software_effect != null)
            {
            }
            isInitialized = true;

        }

        private void set_displacement_amplitude(float amplitude)
        {
            upper_bound = Plane.FromPointNormal((pos + amplitude * normal), normal);
            lower_bound = Plane.FromPointNormal((pos - amplitude * normal), normal);
        }

        /// <summary>
        /// prepare the vertex and indexbuffer with a uniform grid (dependant on the size parameter)	
        /// </summary>
        /// <returns></returns>
        private bool initbuffers(Device device)
        {
            // create the vertexbuffer used in the softwaremode (it can be empty as it'll be memcpy-ed to)
            SOFTWARESURFACEVERTEX ver = new SOFTWARESURFACEVERTEX();
            int size = gridsize_x * gridsize_y * System.Runtime.InteropServices.Marshal.SizeOf(ver);
            surf_software_vertices = new VertexBuffer(device, size, Usage.Dynamic, VertexFormats.Normal, Pool.Default);

            int num1 = (gridsize_x - 1) * (gridsize_y - 1);
            surf_indicies = new IndexBuffer(typeof(int), num1, device, Usage.WriteOnly, Pool.Default);
            //    (device,size1, Usage.WriteOnly, Pool.Default,false);

            int[] indexbuffer = (int[])surf_indicies.Lock(0, 0);
            int i = 0;

            for (int v = 0; v < gridsize_y - 1; v++)
            {
                for (int u = 0; u < gridsize_x - 1; u++)
                {
                    // face 1 |/
                    indexbuffer[i++] = v * gridsize_x + u;
                    indexbuffer[i++] = v * gridsize_x + u + 1;
                    indexbuffer[i++] = (v + 1) * gridsize_x + u;

                    // face 2 /|
                    indexbuffer[i++] = (v + 1) * gridsize_x + u;
                    indexbuffer[i++] = v * gridsize_x + u + 1;
                    indexbuffer[i++] = (v + 1) * gridsize_x + u + 1;
                }
            }
            surf_indicies.Unlock();
            return true;
        }

        public override void Update(DrawArgs drawArgs)
        {

            if (!isInitialized)

                Initialize(drawArgs);

        }



        public override void Render(DrawArgs drawArgs)
        {
            
        }



        public override void Dispose()
        {
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }


    }
}
