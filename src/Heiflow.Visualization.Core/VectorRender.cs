using Heiflow.Visualization.Renderable.Grid;
using Heiflow.Visualization.RenderableModel.Model.Renders;
using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization
{
    public abstract class VectorRender : ModelRenderDX, IVectorRender
    {
        public VectorRender(World world):base(world)
        {
            _VectorStyle = new VectorStyle()
            {
                AutoColor = true,
                TileCount = 3
            };
            _VectorStyle.StyleChanged += VectorStyle_StyleChanged;
            RequiredUpdated = true;
        }

        public VectorRender()
        {
            _VectorStyle = new VectorStyle()
            {
                AutoColor = true,
            };
            _VectorStyle.StyleChanged += VectorStyle_StyleChanged;
            RequiredUpdated = true;
        }

        protected VectorStyle _VectorStyle;

        public VectorStyle Style
        {
            get
            {
                return _VectorStyle;
            }
            set
            {
                _VectorStyle = value;
            }
        }

        public bool RequiredUpdated { get; set; }

        protected virtual void VectorStyle_StyleChanged(object sender, EventArgs e)
        {

        }

        public override void CreatMeshes()
        {
            throw new NotImplementedException();
        }

        public override float GetCellValue(int p1, int p2)
        {
            throw new NotImplementedException();
        }

        public override Models.Generic.ICell SelectCell(double lat, double lon)
        {
            throw new NotImplementedException();
        }

        public override void UpdateVertexColor()
        {
            throw new NotImplementedException();
        }

        public override void CacheColor()
        {
            throw new NotImplementedException();
        }

        public override void UpdateCachedColor()
        {
            throw new NotImplementedException();
        }

        public override void UpdateCellValues()
        {
            throw new NotImplementedException();
        }

        public override Cell[] RetrieveCellsInColumn(int ColumnIndex)
        {
            throw new NotImplementedException();
        }

        public override Cell[] RetrieveCellsInRow(int RowIndex)
        {
            throw new NotImplementedException();
        }

        public override float[] GetCellTimeSeries(int p1, int p2)
        {
            throw new NotImplementedException();
        }
    }
}
