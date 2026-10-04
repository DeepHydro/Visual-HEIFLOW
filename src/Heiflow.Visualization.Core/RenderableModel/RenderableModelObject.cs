using Heiflow.Models.Generic;
using Heiflow.Models.Visualization;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable
{
    public abstract class RenderableModelObject : RenderableObject, IDX3DLayer
    {
        protected CustomVertex.PositionNormalColored[] _VertexList;
        protected int[] _VertexIndexList;
        protected IDX3DLayerRender _RenderDX;
        protected string _token;
        private List<IPackage> _Packages = new List<IPackage>();

        public RenderableModelObject()
        {

        }
        public RenderableModelObject(string name, World world)
            : base(name, world)
        {

        }

        public RenderableModelObject(string name, Vector3 position, Quaternion quat)
            : base(name, position, quat)
        {

        }
        [Browsable(false)]
        public bool HighLightSelectedCell
        {
            get;
            set;
        }

        [Browsable(false)]
        public I3DLayerRender RenderObject
        {
            get 
            {
                return _RenderDX; 
            }
            set 
            {
                _RenderDX = value as IDX3DLayerRender;
                _RenderDX.LayerObject = this;
            }
        }

        [Browsable(false)]
        public IDX3DLayerRender RenderDX
        {
            get
            {
                return _RenderDX;
            }
            set
            {
                _RenderDX = value;
                _RenderDX.LayerObject = this;
                _RenderDX.RenderableObject = this;
            }
        }
         [Browsable(false)]
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }
        [Browsable(false)]
        public List<IPackage> Packages
        {
            get
            {
                return _Packages;
            }
        }
         [Browsable(false)]
        public bool IsUsed
        {
            get;
            set;
        }
         [Browsable(false)]
         public Models.Generic.Project.IProject Project
         {
             get;
             set;
         }
        protected void CalculateNormals(ref CustomVertex.PositionNormalColored[] vertices, int[] indices)
        {
            List<Vector3>[] normal_buffer = new List<Vector3>[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                normal_buffer[i] = new List<Vector3>();
            }
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 p1 = vertices[indices[i + 0]].Position;
                Vector3 p2 = vertices[indices[i + 1]].Position;
                Vector3 p3 = vertices[indices[i + 2]].Position;

                Vector3 v1 = p2 - p1;
                Vector3 v2 = p3 - p1;
                Vector3 normal = Vector3.Cross(v1, v2);

                normal.Normalize();

                // Store the face's normal for each of the vertices that make up the face.
                normal_buffer[indices[i + 0]].Add(normal);
                normal_buffer[indices[i + 1]].Add(normal);
                normal_buffer[indices[i + 2]].Add(normal);
            }

            // Now loop through each vertex vector, and avarage out all the normals stored.
            for (int i = 0; i < vertices.Length; ++i)
            {
                for (int j = 0; j < normal_buffer[i].Count; ++j)
                {
                    Vector3 curNormal = normal_buffer[i][j];

                    if (vertices[i].Normal == Vector3.Empty)
                        vertices[i].Normal = curNormal;
                    else
                        vertices[i].Normal += curNormal;
                }
                vertices[i].Normal.Multiply(1.0f / normal_buffer[i].Count);
            }
        }


        public void AddPackage(IPackage pck)
        {
            if (!Exist(pck))
                _Packages.Add(pck);
        }

        public bool Exist(IPackage pck)
        {
            var buf = from pp in _Packages where pp.Name == pck.Name select pp;
            return buf.Any();
        }

        public IPackage Select(string pck_name)
        {
            var buf = from pp in _Packages where pp.Name == pck_name select pp;
            if (buf.Any())
                return buf.First();
            else
                return null;
        }
    }
}
