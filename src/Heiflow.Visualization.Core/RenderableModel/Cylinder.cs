using System;
using System.Collections.Generic;
using System.Text;
using HUST.WREIS.Dot3D;
using Utility;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;

namespace HUST.WREIS.Dot3D.Renderable
{
    public class Cylinder : RenderableObject
    {
        double m_distanceAboveSurface = 0;
        double m_latitude = 0;
        double m_longitude = 0;
        double m_height = 0;
        int m_color = 0;
        float m_currentVerticalExaggeration = World.Settings.VerticalExaggeration;
        Point3d m_cartesianPoint = null;
        private bool m_useScaling = false;
        private double m_scalarMinimum = 0;
        private double m_scalarMaximum = 1;
        private double m_targetScalar = 1;
        private float m_scaleX = 1;
        private float m_scaleY = 1;
        public double RenderedHeight = 0;
      //  int[] m_extrudeIndices = null;
    //    CustomVertex.PositionColored[] m_extrudeVertices = null;
     //   short[] m_outlineIndices = null;
     //   CustomVertex.PositionColored[] m_outlineVertices = null;
        Mesh mesh;

        public double Latitude
        {
            get { return m_latitude; }
            set
            {
                m_latitude = value;
                UpdateCartesianPoint();
            }
        }
        public double Longitude
        {
            get { return m_longitude; }
            set
            {
                m_longitude = value;
                UpdateCartesianPoint();
            }
        }
        public new double DistanceAboveSurface
        {
            get { return m_distanceAboveSurface; }
            set
            {
                m_distanceAboveSurface = value;
                UpdateCartesianPoint();
            }
        }
        public double Height
        {
            get { return m_height; }
            set { m_height = value; }
        }
        public bool UseScaling
        {
            get { return m_useScaling; }
            set { m_useScaling = value; }
        }
        public double ScalarMinimum
        {
            get { return m_scalarMinimum; }
            set { m_scalarMinimum = value; }
        }
        public double ScalarMaximum
        {
            get { return m_scalarMaximum; }
            set { m_scalarMaximum = value; }
        }
        public double ScalarValue
        {
            get { return m_targetScalar; }
            set { m_targetScalar = value; }
        }
        public float ScaleX
        {
            get { return m_scaleX; }
            set { m_scaleX = value; }
        }
        public float ScaleY
        {
            get { return m_scaleY; }
            set { m_scaleY = value; }
        }
        public Cylinder(
            string name,
            World parentWorld,
            double latitude,
            double longitude,
            double distanceAboveSurface,
            double height,
            System.Drawing.Color color)
            : base(name, parentWorld)
        {
            m_latitude = latitude;
            m_longitude = longitude;
            m_distanceAboveSurface = distanceAboveSurface;
            m_height = height;
            m_color = color.ToArgb();
            UpdateCartesianPoint();
        }
        private void UpdateCartesianPoint()
        {
            m_cartesianPoint = MathEngine.SphericalToCartesianD(
                Angle.FromDegrees(m_latitude), Angle.FromDegrees(m_longitude), m_world.EquatorialRadius + m_distanceAboveSurface * World.Settings.VerticalExaggeration);
            m_currentVerticalExaggeration = World.Settings.VerticalExaggeration;
        }
        public override void Initialize(DrawArgs drawArgs)
        {
            isInitialized = true;
        }
        public override void Update(DrawArgs drawArgs)
        {
            if (!isInitialized)
                Initialize(drawArgs);
        }
        public override void Render(DrawArgs drawArgs)
        {
            RenderBar(drawArgs);
        }
        public override void Dispose()
        {
        }
        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }


        private void RenderBar(DrawArgs drawArgs)
        {
            bool lighting = drawArgs.device.RenderState.Lighting;
            drawArgs.device.RenderState.Lighting = false;
            var ele = World.TerrainAccessor.GetElevationAt(m_latitude, m_longitude) * World.Settings.VerticalExaggeration;
            Vector3 surfacePos = MathEngine.SphericalToCartesian(m_latitude, m_longitude, World.EquatorialRadius + ele);
            Vector3 rc = new Vector3(
              (float)drawArgs.WorldCamera.ReferenceCenter.X,
              (float)drawArgs.WorldCamera.ReferenceCenter.Y,
              (float)drawArgs.WorldCamera.ReferenceCenter.Z
              );

            Matrix translation = Matrix.Translation(
                    (float)(m_cartesianPoint.X - drawArgs.WorldCamera.ReferenceCenter.X),
                    (float)(m_cartesianPoint.Y - drawArgs.WorldCamera.ReferenceCenter.Y),
                    (float)(m_cartesianPoint.Z - drawArgs.WorldCamera.ReferenceCenter.Z)
                        );
  
            if(mesh == null)
            {
                mesh = Mesh.Cylinder(drawArgs.device, 10000, 10000, 10000, 50, 50);
            }

            translation = Matrix.Translation(surfacePos - rc);

            drawArgs.device.Transform.World *= Matrix.RotationY((float)-MathEngine.DegreesToRadians(90));
            drawArgs.device.Transform.World *= Matrix.RotationY((float)-MathEngine.DegreesToRadians(m_latitude));
            drawArgs.device.Transform.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(m_longitude));
            drawArgs.device.Transform.World *= translation;
            //drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
            //drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;
            //drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.Diffuse;
            //drawArgs.device.TextureState[0].AlphaArgument1 = TextureArgument.Diffuse;
            //drawArgs.device.TextureState[0].AlphaOperation = TextureOperation.SelectArg1;
            drawArgs.device.RenderState.Lighting = true;
            Material boxMaterial = new Material();
            boxMaterial.Ambient = Color.Red;
            boxMaterial.Diffuse = Color.Red;
            drawArgs.device.Material = boxMaterial;
            mesh.DrawSubset(0);

            drawArgs.device.RenderState.Lighting = lighting;
        }
    }
}
