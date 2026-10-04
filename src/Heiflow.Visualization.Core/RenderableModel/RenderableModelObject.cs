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
            ModelNormals.Calculate(vertices, indices);
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
