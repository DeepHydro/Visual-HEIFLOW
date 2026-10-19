using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HUST.WREIS.Dot3D.Renderable;


using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

namespace HUST.WREIS.Dot3D
{
    public enum RenderFillMode {Point, Solid,WireFrame }
    public class LayerStyle
    {
        public LayerStyle(RenderableObject ro)
        {
            FillMode = FillMode.Solid;
            Opacity = 255;
            mRenderableObject = ro;
        }

        byte opacity = 255;

        private RenderableObject mRenderableObject;

        public byte Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                if (value < 0)
                    opacity = 0;
                else if (value > 255)
                    opacity = 255;
                else
                    opacity = value;
                if(mRenderableObject !=null)
                    mRenderableObject.Update(ConfigurationManager.DrawArgs);
            }
        }

        public FillMode FillMode { get; set; } 
    }
}
