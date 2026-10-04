using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using HUST.WREIS.Dot3D.Renderable;


namespace HUST.WREIS.Dot3D
{
    public enum RenderFillMode {Point, Solid,WireFrame }
    public class LayerStyle
    {
        public LayerStyle(RenderableObject ro)
        {
            FillMode = Microsoft.DirectX.Direct3D.FillMode.Solid;
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
