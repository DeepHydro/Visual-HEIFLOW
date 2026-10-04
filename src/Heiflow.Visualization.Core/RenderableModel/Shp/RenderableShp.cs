using DotSpatial.Data;
using Heiflow.Core;
using Heiflow.Visualization.Geometry.Triangulation;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable
{
    public class RenderableShp : RenderableModelObject
    {
        public RenderableShp(string filename, World parentWorld)
            : base("Shape layer", parentWorld)
        {
            _Filename = filename;
            m_world = parentWorld;
        }

        protected string _Filename;
        protected IFeatureSet _IFeatureSet;

        protected System.Drawing.Color _FeatureColor = System.Drawing.Color.Black;
      

        /// <summary>
        /// Boolean indicating whether or not the line needs rebuilding.
        /// </summary>
        protected bool NeedsUpdate = true;

        public System.Windows.Media.Color FeatureColor
        {
            get
            {
                return MediaColor.From(_FeatureColor); 
            }
            set
            {
                _FeatureColor= MediaColor.From(value);
                _FeatureColor = System.Drawing.Color.FromArgb(Opacity, _FeatureColor);
                NeedsUpdate = true;
            }
        }
              
        public override byte Opacity
        {
            get
            {
                return m_opacity;
            }
            set
            {
                m_opacity = value;
                UpdateVertices();
            }
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (_IFeatureSet == null)
            {
                isInitialized = true;
                return;
            }
            isInitialized = true;
        }

        public virtual void UpdateVertices()
        {
 
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (drawArgs.WorldCamera.Altitude >= MinDisplayAltitude && drawArgs.WorldCamera.Altitude <= MaxDisplayAltitude)
            {
                if (!isInitialized)
                    Initialize(drawArgs);
            }
        }

        public override void Render(DrawArgs drawArgs)
        {
           
        }


        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }

        public override void Dispose()
        {
             
        }
    }
}
