using Heiflow.Models.Generic;
using HUST.WREIS.Dot3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// 三维剖切设置：沿模型网格自身的 X（东）、Y（北）、Z（垂向）三个方向各用一对裁剪平面，
    /// 把落在区间外的网格体裁掉，实现沿任意方向的剖切显示。
    ///
    /// 网格顶点是以地心为原点的笛卡尔坐标（见 MFGridRender.PlaceVertex），模型自身的 X/Y 方向
    /// 在世界里是斜着的，所以每个轴存的是世界坐标系里的一条方向向量和顶点在该方向上的投影
    /// 范围。剖切窗口只给出 0~1 的相对位置，两个平面方程由这里换算。
    ///
    /// 裁剪平面由 ModelRenderScope 在每个模型图层绘制时应用、恢复，因此只对模型网格、剖面、
    /// 矢量场等模型图层生效，不影响地形影像。
    /// </summary>
    public static class ModelGridClipper
    {
        /// <summary>一个方向的剖切状态：方向向量、顶点投影范围和用户给定的相对区间。</summary>
        public class AxisClip
        {
            /// <summary>单位方向向量，顶点坐标空间（世界笛卡尔坐标）。</summary>
            public Vector3 Direction = new Vector3(1, 0, 0);

            /// <summary>顶点沿 Direction 投影的最小值，Setup 时测得。</summary>
            public double Min;

            /// <summary>顶点沿 Direction 投影的最大值，Setup 时测得。</summary>
            public double Max;

            /// <summary>这个方向是否参与剖切。</summary>
            public bool Clip = true;

            /// <summary>剖切起点，0~1，相对 Min~Max 的位置。</summary>
            public double Start = 0;

            /// <summary>剖切终点，0~1，相对 Min~Max 的位置。</summary>
            public double End = 1;

            public bool IsValid
            {
                get { return Max > Min; }
            }

            /// <summary>起点在投影范围里的绝对值。</summary>
            public double StartPosition
            {
                get { return Min + (Max - Min) * Math.Min(Start, End); }
            }

            /// <summary>终点在投影范围里的绝对值。</summary>
            public double EndPosition
            {
                get { return Min + (Max - Min) * Math.Max(Start, End); }
            }
        }

        /// <summary>X（东）方向的剖切状态。</summary>
        public static readonly AxisClip XAxis = new AxisClip();

        /// <summary>Y（北）方向的剖切状态。</summary>
        public static readonly AxisClip YAxis = new AxisClip();

        /// <summary>Z（垂向）方向的剖切状态。</summary>
        public static readonly AxisClip ZAxis = new AxisClip();

        /// <summary>剖切总开关，关掉时不设置任何裁剪平面。</summary>
        public static bool Enabled { get; set; }

        /// <summary>Setup 是否已经从网格测得了方向和范围。</summary>
        public static bool HasBounds { get; private set; }

        /// <summary>清空设置，回到未剖切状态。</summary>
        public static void Clear()
        {
            Enabled = false;
            HasBounds = false;
        }

        /// <summary>
        /// 从网格渲染器测出三个剖切方向和顶点投影范围。方向由网格的模型坐标范围经
        /// GCSConverter 转到经纬度、再转到世界笛卡尔坐标得出，这样剖切平面永远跟着模型的
        /// X/Y/Z 轴走，与模型摆在地球上的位置无关。转换不可用时退化为世界坐标轴。
        /// </summary>
        /// <returns>是否成功测得范围。</returns>
        public static bool Setup(IDX3DLayerRender render)
        {
            HasBounds = false;
            if (render == null)
                return false;

            var vertices = render.VertexList;
            if (vertices == null || vertices.Length == 0)
                return false;

            var xDir = new Vector3(1, 0, 0);
            var yDir = new Vector3(0, 1, 0);
            var zDir = new Vector3(0, 0, 1);

            var gcs = render.GCSConverter;
            var grid = render.Grid;
            if (gcs != null && grid != null && grid.BBox != null && grid.BBox.Width > 0 && grid.BBox.Height > 0)
            {
                var bbox = grid.BBox;
                double midX = (bbox.MinX + bbox.MaxX) * 0.5;
                double midY = (bbox.MinY + bbox.MaxY) * 0.5;
                const double radius = 6378137.0;

                var west = ToCartesian(gcs, bbox.MinX, midY, radius);
                var east = ToCartesian(gcs, bbox.MaxX, midY, radius);
                var south = ToCartesian(gcs, midX, bbox.MinY, radius);
                var north = ToCartesian(gcs, midX, bbox.MaxY, radius);
                var centre = ToCartesian(gcs, midX, midY, radius);

                xDir = Vector3.Normalize(Sub(east, west));
                yDir = Vector3.Normalize(Sub(north, south));
                zDir = Vector3.Normalize(centre);
            }

            Measure(XAxis, xDir, vertices);
            Measure(YAxis, yDir, vertices);
            Measure(ZAxis, zDir, vertices);

            HasBounds = XAxis.IsValid && YAxis.IsValid && ZAxis.IsValid;
            return HasBounds;
        }

        /// <summary>
        /// 在绘制一个模型图层之前设置裁剪平面。注意要在世界矩阵设为
        /// Translation(-ReferenceCenter) 之后调用（ModelRenderScope.Begin 里的顺序），
        /// 平面系数要按这个平移折算。
        /// </summary>
        public static void Apply(Device device)
        {
            if (!Enabled || !HasBounds)
            {
                device.ClipPlanes.DisableAll();
                return;
            }

            int maxPlanes = device.DeviceCaps.MaxUserClipPlanes;
            if (maxPlanes < 2)
            {
                device.ClipPlanes.DisableAll();
                return;
            }

            // ModelRenderScope 把世界矩阵设为平移(-ReferenceCenter)，顶点被挪进相机参考系；
            // D3D 的裁剪平面在世界空间里求值，平面常数要扣掉这段平移才落在顶点本来的坐标上。
            Matrix world = device.Transform.World;
            Vector3 offset = new Vector3(world.M41, world.M42, world.M43);
            int index = 0;

            AddAxis(device, XAxis, offset, ref index, maxPlanes);
            AddAxis(device, YAxis, offset, ref index, maxPlanes);
            AddAxis(device, ZAxis, offset, ref index, maxPlanes);

            for (int i = index; i < maxPlanes; i++)
                device.ClipPlanes[i].Enabled = false;
        }

        /// <summary>把裁剪平面关掉，用于图层绘制结束后的状态恢复。</summary>
        public static void Reset(Device device)
        {
            device.ClipPlanes.DisableAll();
        }

        private static void AddAxis(Device device, AxisClip axis, Vector3 offset,
            ref int index, int maxPlanes)
        {
            if (!axis.Clip || !axis.IsValid)
                return;

            // 保留 dot(v, d) >= min 的顶点：法向 d、常数 -min。
            AddPlane(device, axis.Direction, -axis.StartPosition, offset, ref index, maxPlanes);
            // 保留 dot(v, d) <= max 的顶点：法向 -d、常数 max。
            AddPlane(device, Negate(axis.Direction), axis.EndPosition, offset, ref index, maxPlanes);
        }

        private static void AddPlane(Device device, Vector3 normal, double d, Vector3 offset,
            ref int index, int maxPlanes)
        {
            if (index >= maxPlanes)
                return;

            float offsetDot = normal.X * offset.X + normal.Y * offset.Y + normal.Z * offset.Z;
            device.ClipPlanes[index].Plane = new Plane(normal.X, normal.Y, normal.Z, (float)(d - offsetDot));
            device.ClipPlanes[index].Enabled = true;
            index++;
        }

        private static void Measure(AxisClip axis, Vector3 direction,
            CustomVertex.PositionNormalColored[] vertices)
        {
            axis.Direction = direction;
            int stride = Math.Max(1, vertices.Length / 10000);
            double min = double.MaxValue;
            double max = double.MinValue;

            for (int i = 0; i < vertices.Length; i += stride)
            {
                double p = vertices[i].X * direction.X + vertices[i].Y * direction.Y + vertices[i].Z * direction.Z;
                if (p < min)
                    min = p;
                if (p > max)
                    max = p;
            }

            axis.Min = min;
            axis.Max = max;
        }

        private static Vector3 ToCartesian(GCSConverter gcs, double x, double y, double radius)
        {
            gcs.ToLatLon(x, y);
            return MathEngine.SphericalToCartesian(gcs.Latitude, gcs.Longitude, radius);
        }

        private static Vector3 Sub(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        private static Vector3 Negate(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }
    }
}
