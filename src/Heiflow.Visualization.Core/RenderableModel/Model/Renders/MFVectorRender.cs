using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.ComponentModel;
using Heiflow.Models.Subsurface;
using Heiflow.Core.MyMath;
using Heiflow.Spatial.Geography;
using HUST.WREIS.Dot3D;
using Heiflow.Core.Data;
using Heiflow.Visualization.RenderableModel.Model.Renders;


namespace Heiflow.Visualization.Renderable.Grid
{
    public class MFVectorRender : MFGridRender, IVectorRender, IStreamlineRender
    {
        /// <summary>
        /// How long a degree of latitude is, which is what turns the size of a cell into metres for
        /// the length of a dash.
        /// </summary>
        private const double MetresPerDegree = 111320.0;

        /// <summary>
        /// How many vertices the lines of one layer are allowed to take.
        /// </summary>
        private const int MaxStreamlineVertices = 300000;

        protected VectorStyle _VectorStyle;

        /// <summary>
        /// Three floats for every vertex while the mesh holds lines: how far along its line the
        /// vertex lies, how fast the flow is there, and which side of the line it lies on. Null while
        /// the mesh holds arrows.
        /// </summary>
        private float[] _StreamlineFlow;

        /// <summary>
        /// The rows and the columns of the grid, written into an array so that the cell of a step
        /// can be looked up without asking the topology for it, and the size of a cell.
        /// </summary>
        private int[] _CellAt;
        private int _CellRows;
        private int _CellColumns;
        private double _LonPerColumn;
        private double _LatPerRow;

        /// <summary>
        /// The cells one line walks through and where their centres lie, then the points of the curve
        /// that is put through those centres and how far along the line each of them is. They are
        /// kept between two lines, a run of tens of thousands of them asks the collector for nothing.
        /// </summary>
        private int[] _Path;
        private double[] _PathX;
        private double[] _PathY;
        private double[] _PathZ;
        private float[] _PathSpeed;
        private int[] _PathColour;
        private double[] _CurveX;
        private double[] _CurveY;
        private double[] _CurveZ;
        private double[] _CurveArc;

        /// <summary>
        /// Which line has taken a cell of the grid, one above the number of the line, and nothing
        /// where no line runs there. It is what keeps two lines from lying over each other, see
        /// TooClose, and it is wiped before every rebuild.
        /// </summary>
        private int[] _Taken;

        public MFVectorRender(World world):base(world)
        {
            RequiredUpdated = true;
            _VectorStyle = new VectorStyle();
        }

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
        public bool RequiredUpdated
        {
            get;
            set;
        }

        public bool StreamlinesActive
        {
            get { return _VectorStyle.ShowStreamlines && _StreamlineFlow != null; }
        }

        public float[] StreamlineFlow
        {
            get { return _StreamlineFlow; }
        }

        /// <summary>
        /// How long a cell of this grid is, in metres, which is what turns the settings that are
        /// asked for in cells and in seconds into the units the scene is drawn in.
        /// </summary>
        private double CellMetres
        {
            get
            {
                double cell = Math.Abs(_LatPerRow) * MetresPerDegree;
                return cell > 0 ? cell : 1000.0;
            }
        }

        /// <summary>
        /// How fast a dash runs along a line, in metres per second. It is asked for in cells a
        /// second, so that the same number looks the same on a grid of any size.
        /// </summary>
        public float FlowSpeed
        {
            get { return (float)(Math.Max(0.0, _VectorStyle.FlowSpeed) * CellMetres); }
        }

        public StreamlineLook Look
        {
            get
            {
                var look = new StreamlineLook();
                double cell = CellMetres;

                look.Width = _VectorStyle.StreamlineWidth;
                look.TangentOffset = (float)Math.Max(1.0, cell);

                // How long a dash stays lit is how far it gets in the time it is given, and two
                // heads are a dark stretch of the line apart from that.
                look.Trail = (float)(FlowSpeed * Math.Max(0.1, _VectorStyle.FadeTime));
                look.Spacing = look.Trail + (float)(Math.Max(0.0, _VectorStyle.DashCells) * cell);
                if (look.Trail <= 0)
                    look.Trail = (float)cell;
                if (look.Spacing <= look.Trail)
                    look.Spacing = look.Trail + 1f;

                return look;
            }
        }

        public override void Initilize()
        {
            base.Initilize();
            _Project = LayerObject.Project;
            _MFGrid = _Project.Model.Grid as MFGrid;
            _VertexIndexList = new int[_MFGrid.ActiveCellCount * 6];
            _VertexList = new CustomVertex.PositionNormalColored[_MFGrid.ActiveCellCount * 4];
            _VectorStyle.StyleChanged -= VectorStyle_StyleChanged;
            _VectorStyle.HeadSize = 600;
            _VectorStyle.TileCount = 1;
            _VectorStyle.AutoColor = true;
            _VectorStyle.ScaleMethod = ScaleMethod.StandardDeviation;
            CreatMeshes();
            _VectorStyle.StyleChanged += VectorStyle_StyleChanged;
        }
        public override void CacheColor()
        {
            //do nothing because we have to receate meshes every time step.
        }

        public override void CreatMeshes()
        {
            if (DataSource == null)
                return;

            var metric_array = DataSource.GetVector(0, CurrentTimeStep.ToString(), ":");
            var anl_array = DataSource.GetVector(1, CurrentTimeStep.ToString(), ":");
            if (metric_array == null || anl_array == null || metric_array.Length == 0)
                return;

            double maxValue = metric_array.Max();
            double minValue = metric_array.Min();
            double avValue = metric_array.Average();
            var std = MyStatisticsMath.StandardDeviation(metric_array);
            var levels = _DataColor.GetLevels(metric_array, this.ColourRampCount, this.ClassificationMethod);

            // Both ways of laying the flow out read the same values and differ in what they put in
            // the mesh: an arrow on every cell, or a line traced through the field.
            if (_VectorStyle.ShowStreamlines)
            {
                CreatStreamlines(metric_array, anl_array, levels, maxValue);
            }
            else
            {
                _StreamlineFlow = null;
                CreatArrows(metric_array, anl_array, levels, maxValue, minValue, avValue, std);

                // Only the arrows are lit. A line is drawn by the shader, which has no use for a
                // normal, and the rows of its segments would be read as triangles if the normals
                // were worked out over them.
                CalculateNormals(ref _VertexList, _VertexIndexList);
            }

            NotifyMeshChanged();
        }

        /// <summary>
        /// An arrow on every active cell of the grid.
        /// </summary>
        private void CreatArrows(float[] metric_array, float[] anl_array, int[] levels,
            double maxValue, double minValue, double avValue, double std)
        {
            double radius = EquatorialRadius + this.DistanceAboveSurface *World.Settings.VerticalExaggeration;
            double tanAngle = Math.Tan(_VectorStyle.ArrowAngle);

            double headSize = _VectorStyle.HeadSize;
            int colorIndex = _VectorStyle.ArrowColor;

            // The mesh holds four points for a cell while the arrows are drawn and two points for a
            // segment of a line while they are not, so the arrays are made to fit whichever of the
            // two the flow is laid out as this time.
            int cellCount = _MFGrid.ActiveCellCount;
            if (_VertexList == null || _VertexList.Length != cellCount * 4
                || _VertexIndexList == null || _VertexIndexList.Length != cellCount * 6)
            {
                _VertexList = new CustomVertex.PositionNormalColored[cellCount * 4];
                _VertexIndexList = new int[cellCount * 6];
            }

            // An arrow is written for every active cell of the grid, not only for the cells the tile
            // count keeps. The count decides which of them have a length: every tileCount-th cell is
            // drawn, the cells in between get an arrow whose four points sit on their own centre,
            // which is three lines of no length and so draws nothing. Writing every cell is what lets
            // a larger count take hold, a cell the new count leaves out used to keep the arrow the
            // count before it had given it, so only part of the grid answered the setting.
            int tileCount = Math.Max(1, _VectorStyle.TileCount);

            for (int c = 0; c < cellCount; c++)
            {
                bool hasData = c < metric_array.Length && c < anl_array.Length;
                if (_VectorStyle.AutoColor && hasData)
                {
                    colorIndex = _DataColor.GetVertexColor(levels[c], Opacity);
                }

                double angle = hasData ? anl_array[c] : 0;

                double centralLon = MathEngine.DegreesToRadians(_MFGrid.CentralPoint[0, 0, c]);
                double centralLat = MathEngine.DegreesToRadians(_MFGrid.CentralPoint[1, 0, c]);
                double centralZ = radius + _MFGrid.CentralPoint[2, 0, c] * World.Settings.VerticalExaggeration;
                Point3d center = MathEngine.SphericalRadianToCartesianD(centralLat, centralLon, centralZ);

                double x1 = center.X;
                double y1 = center.Y;
                double z1 = center.Z;
                double x2 = center.X;
                double y2 = center.Y;
                double z2 = center.Z;
                double x3 = center.X;
                double y3 = center.Y;
                double z3 = center.Z;
                double x4 = center.X;
                double y4 = center.Y;
                double z4 = center.Z;

                if (hasData && c % tileCount == 0)
                {
                    headSize = Style.ScaleHeadSize(metric_array[c], maxValue, minValue, avValue, std);

                    Heiflow.Spatial.Geography.Point mp1 = Geodesic.GetPointFromHeading(centralLon, centralLat, headSize, angle);
                    Point3d p31 = MathEngine.SphericalRadianToCartesianD(mp1.Y, mp1.X, centralZ);
                    x1 = p31.X;
                    y1 = p31.Y;
                    z1 = p31.Z;

                    double r1 = headSize * tanAngle * _VectorStyle.Dehisce;

                    Heiflow.Spatial.Geography.Point mp2 = Geodesic.GetPointFromHeading(centralLon, centralLat, r1, angle - MathEngine.PIHalf);
                    Point3d p32 = MathEngine.SphericalRadianToCartesianD(mp2.Y, mp2.X, centralZ);
                    x2 = p32.X;
                    y2 = p32.Y;
                    z2 = p32.Z;

                    Heiflow.Spatial.Geography.Point mp3 = Geodesic.GetPointFromHeading(centralLon, centralLat, r1, angle + MathEngine.PIHalf);
                    Point3d p33 = MathEngine.SphericalRadianToCartesianD(mp3.Y, mp3.X, centralZ);
                    x3 = p33.X;
                    y3 = p33.Y;
                    z3 = p33.Z;

                    Heiflow.Spatial.Geography.Point mp4 = Geodesic.GetPointFromHeading(centralLon, centralLat, headSize * _VectorStyle.TailSize, angle + Math.PI);
                    Point3d p34 = MathEngine.SphericalRadianToCartesianD(mp4.Y, mp4.X, centralZ);
                    x4 = p34.X;
                    y4 = p34.Y;
                    z4 = p34.Z;
                }

                CustomVertex.PositionNormalColored p1 = new CustomVertex.PositionNormalColored()
                {
                    X = (float)x1,
                    Y = (float)y1,
                    Z = (float)z1
                };

                CustomVertex.PositionNormalColored p2 = new CustomVertex.PositionNormalColored()
                {
                    X = (float)x2,
                    Y = (float)y2,
                    Z = (float)z2
                };
                CustomVertex.PositionNormalColored p3 = new CustomVertex.PositionNormalColored()
                {
                    X = (float)x3,
                    Y = (float)y3,
                    Z = (float)z3
                };
                CustomVertex.PositionNormalColored p4 = new CustomVertex.PositionNormalColored()
                {
                    X = (float)x4,
                    Y = (float)y4,
                    Z = (float)z4
                };

                p1.Color = colorIndex;
                p2.Color = colorIndex;
                p3.Color = colorIndex;
                p4.Color = colorIndex;

                _VertexList[c * 4 + 0] = p1;
                _VertexList[c * 4 + 1] = p2;
                _VertexList[c * 4 + 2] = p3;
                _VertexList[c * 4 + 3] = p4;
                _VertexIndexList[c * 6 + 0] = c * 4;
                _VertexIndexList[c * 6 + 1] = c * 4 + 1;
                _VertexIndexList[c * 6 + 2] = c * 4;
                _VertexIndexList[c * 6 + 3] = c * 4 + 2;
                _VertexIndexList[c * 6 + 4] = c * 4;
                _VertexIndexList[c * 6 + 5] = c * 4 + 3;
            }

        }

        /// <summary>
        /// A line traced through the flow from every tileCount-th cell, laid out as a ribbon of two
        /// triangles for every piece of it, which is what Streamline.fx runs its dashes along.
        /// </summary>
        /// <remarks>
        /// A line follows the direction a cell carries, the same one an arrow on that cell points in,
        /// from cell to cell. The direction is turned into a step in rows and columns with the size
        /// of a cell, and the cell the step lands in gives the next point of the line. A line that
        /// walks out of the grid, or into a cell that carries no flow, ends early, and the room that
        /// was kept for the rest of it is filled with triangles of no area, which draw nothing.
        ///
        /// The chain of cells is then smoothed into a curve, see <see cref="Curve"/>, and every point
        /// of it keeps how far along the line it lies, which is what the shader moves the dashes by,
        /// and how fast the flow is there, which is what sets a quick part of the field apart from a
        /// slow one.
        ///
        /// A line also keeps the cells it comes to for itself, see <see cref="TooClose"/>, so that
        /// two of them neither lie over each other nor cross. That is what says which lines are
        /// drawn and how far each of them gets, and with it how many of them there are: a line is
        /// not started where one already runs close by, and one that is already running stops there
        /// as well, which is why the room kept for them is filled as they are drawn rather than one
        /// room to a cell that was asked for a line.
        /// </remarks>
        private void CreatStreamlines(float[] magnitude, float[] angle, int[] levels, double maxValue)
        {
            if (_MFGrid == null || _MFGrid.Topology == null || _MFGrid.Topology.ActiveCellLocation == null
                || _MFGrid.CentralPoint == null)
            {
                _StreamlineFlow = null;
                return;
            }

            var locations = _MFGrid.Topology.ActiveCellLocation;
            int cellCount = Math.Min(magnitude.Length, Math.Min(angle.Length, locations.Length));
            if (cellCount <= 0)
            {
                _StreamlineFlow = null;
                return;
            }

            int steps = Math.Max(1, _VectorStyle.StreamlineSteps);
            int smooth = Math.Max(1, _VectorStyle.StreamlineSmooth);

            // A line every tileCount cells, as the arrows are, but never so many of them that the
            // mesh outgrows the room a layer may ask for. A grid of a hundred thousand cells asked
            // for a line each would be millions of vertices, so the spacing is widened until the
            // count fits, which is what the tile count would have been set to anyway.
            int tileCount = Math.Max(1, _VectorStyle.TileCount);
            int verticesPerLine = (steps * smooth + 1) * 2;
            int widest = (int)Math.Ceiling((double)cellCount * verticesPerLine / MaxStreamlineVertices);
            if (widest > tileCount)
                tileCount = Math.Min(widest, cellCount);

            int seeds = (cellCount + tileCount - 1) / tileCount;

            // A line is a ribbon: two vertices on every point of it, one for each side, and two
            // triangles for every piece between two points.
            int vertexCount = seeds * verticesPerLine;
            int indexCount = seeds * steps * smooth * 6;
            if (_VertexList == null || _VertexList.Length != vertexCount)
                _VertexList = new CustomVertex.PositionNormalColored[vertexCount];
            if (_VertexIndexList == null || _VertexIndexList.Length != indexCount)
                _VertexIndexList = new int[indexCount];
            if (_StreamlineFlow == null || _StreamlineFlow.Length != vertexCount * 3)
                _StreamlineFlow = new float[vertexCount * 3];

            if (_Path == null || _Path.Length < steps + 1
                || _CurveX == null || _CurveX.Length < steps * smooth + 1)
            {
                _Path = new int[steps + 1];
                _PathX = new double[steps + 1];
                _PathY = new double[steps + 1];
                _PathZ = new double[steps + 1];
                _PathSpeed = new float[steps + 1];
                _PathColour = new int[steps + 1];
                _CurveX = new double[steps * smooth + 1];
                _CurveY = new double[steps * smooth + 1];
                _CurveZ = new double[steps * smooth + 1];
                _CurveArc = new double[steps * smooth + 1];
            }

            double lonPerColumn;
            double latPerRow;
            if (!CellSteps(cellCount, out lonPerColumn, out latPerRow))
            {
                _StreamlineFlow = null;
                return;
            }

            double radius = EquatorialRadius + this.DistanceAboveSurface *World.Settings.VerticalExaggeration;
            double exaggeration = World.Settings.VerticalExaggeration;
            double fastest = maxValue > 0 ? maxValue : 1.0;
            int spacing = Math.Max(0, Math.Min(4, _VectorStyle.StreamlineSpacing));

            // No line has taken a cell yet. The map is kept between two rebuilds so that a grid of
            // a hundred thousand cells does not ask the collector for one every step.
            if (_Taken == null || _Taken.Length < cellCount)
                _Taken = new int[cellCount];
            Array.Clear(_Taken, 0, _Taken.Length);

            // The room kept for the lines is filled from the front with the ones that were drawn,
            // so a line that is not started, or one that stops at once, leaves its room to the
            // line that comes after it rather than leaving a hole.
            int vertex = 0;
            int index = 0;
            for (int seed = 0; seed < seeds; seed++)
            {
                int cell = seed * tileCount;
                var location = locations[cell];
                if (location == null || location.Length < 2)
                    continue;

                int row = location[0];
                int column = location[1];

                // No line is started where one already runs close by, which is where two of them
                // would have lain over each other.
                if (TooClose(row, column, spacing, 0))
                    continue;

                int length = Trace(cell, row, column, magnitude, angle, levels, fastest,
                    cellCount, radius, exaggeration, lonPerColumn, latPerRow, spacing, seed + 1);
                if (length < 2)
                    continue;

                int points = (length - 1) * smooth + 1;
                if (vertex + points * 2 > vertexCount)
                    break;

                Curve(length, smooth);

                int lineVertex = vertex;
                for (int i = 0; i < points; i++)
                {
                    int at = i / smooth;
                    double part = (i % smooth) / (double)smooth;
                    float speed = at + 1 < length
                        ? (float)(_PathSpeed[at] + (_PathSpeed[at + 1] - _PathSpeed[at]) * part)
                        : _PathSpeed[length - 1];
                    int colour = part < 0.5 ? _PathColour[at] : _PathColour[Math.Min(at + 1, length - 1)];

                    WriteRibbon(vertex, i, points, colour, speed);
                    vertex += 2;
                }

                for (int i = 0; i < points - 1; i++)
                {
                    int a = lineVertex + i * 2;
                    _VertexIndexList[index++] = a;
                    _VertexIndexList[index++] = a + 1;
                    _VertexIndexList[index++] = a + 2;
                    _VertexIndexList[index++] = a + 2;
                    _VertexIndexList[index++] = a + 1;
                    _VertexIndexList[index++] = a + 3;
                }
            }

            // What is left of the room is filled with triangles whose three corners are the same
            // vertex, which is a triangle of no area and so draws nothing at all.
            while (index < indexCount)
            {
                _VertexIndexList[index++] = 0;
                _VertexIndexList[index++] = 0;
                _VertexIndexList[index++] = 0;
                _VertexIndexList[index++] = 0;
                _VertexIndexList[index++] = 0;
                _VertexIndexList[index++] = 0;
            }
        }

        /// <summary>
        /// True where another line already runs within spacing cells of a place on the grid, which
        /// is what keeps two lines from lying over each other or from crossing.
        /// </summary>
        /// <param name="lineId">The line that is asking, which never counts as being in its own
        /// way. Nothing is taken by a line of no number, so 0 asks for every other line.</param>
        private bool TooClose(int row, int column, int spacing, int lineId)
        {
            if (_Taken == null)
                return false;

            for (int dr = -spacing; dr <= spacing; dr++)
            {
                for (int dc = -spacing; dc <= spacing; dc++)
                {
                    int at = CellAt(row + dr, column + dc);
                    if (at < 0 || at >= _Taken.Length)
                        continue;

                    int taken = _Taken[at];
                    if (taken != 0 && taken != lineId)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Walks a line through the flow from one cell and writes the cells it comes to, with the
        /// centre, the speed and the colour of each. Returns how many of them there are.
        /// </summary>
        /// <remarks>
        /// The cells the line comes to are taken for it as it goes, and it stops where another line
        /// already runs within spacing cells of where it would step next, which is where the two
        /// would have crossed. It stops as well when it comes back to a cell of its own, so a line
        /// that runs into a turn in the flow does not lie over itself.
        /// </remarks>
        private int Trace(int cell, double row, double column, float[] magnitude, float[] angle,
            int[] levels, double fastest, int cellCount, double radius, double exaggeration,
            double lonPerColumn, double latPerRow, int spacing, int lineId)
        {
            Point3d centre = CellCentre(cell, radius, exaggeration);
            _Taken[cell] = lineId;
            _Path[0] = cell;
            _PathX[0] = centre.X;
            _PathY[0] = centre.Y;
            _PathZ[0] = centre.Z;
            _PathSpeed[0] = (float)(magnitude[cell] / fastest);
            _PathColour[0] = _DataColor.GetVertexColor(levels[cell], Opacity);

            int length = 1;
            int steps = _Path.Length - 1;
            while (length <= steps)
            {
                int at = CellAt((int)Math.Round(row), (int)Math.Round(column));
                if (at < 0 || at >= cellCount)
                    break;

                // Where the flow of this cell points, turned into a step over the rows and the
                // columns of the grid. A degree of longitude is shorter than a degree of latitude
                // away from the equator, which is what the cosine is for.
                double centralLat = MathEngine.DegreesToRadians(_MFGrid.CentralPoint[1, 0, at]);
                double east = Math.Sin(angle[at]) / Math.Max(0.05, Math.Cos(centralLat));
                double north = Math.Cos(angle[at]);
                double stepColumn = east / lonPerColumn;
                double stepRow = north / latPerRow;

                double reach = Math.Sqrt(stepColumn * stepColumn + stepRow * stepRow);
                if (reach < 1e-12)
                    break;

                row += stepRow / reach;
                column += stepColumn / reach;

                int next = CellAt((int)Math.Round(row), (int)Math.Round(column));
                if (next < 0 || next >= cellCount)
                    break;

                // Back on a cell the line has taken itself, which is a line that has turned round
                // on itself, or within reach of another line, which is where the two would cross.
                if (_Taken[next] == lineId)
                    break;
                if (TooClose((int)Math.Round(row), (int)Math.Round(column), spacing, lineId))
                    break;

                Point3d point = CellCentre(next, radius, exaggeration);
                _Taken[next] = lineId;
                _Path[length] = next;
                _PathX[length] = point.X;
                _PathY[length] = point.Y;
                _PathZ[length] = point.Z;
                _PathSpeed[length] = (float)(magnitude[next] / fastest);
                _PathColour[length] = _DataColor.GetVertexColor(levels[next], Opacity);
                length++;
            }

            return length;
        }

        /// <summary>
        /// Puts a curve through the centres the line walked through and writes how far along the line
        /// every point of it lies.
        /// </summary>
        /// <remarks>
        /// The walk goes from cell to cell, so what it leaves is a chain of straight pieces with a
        /// corner at every cell it came to. The curve is a Catmull-Rom spline: it runs through every
        /// one of those centres and takes the same way out of a point as it came in, so the corners
        /// are gone and the line keeps the shape the flow gave it.
        /// </remarks>
        private void Curve(int length, int smooth)
        {
            int last = length - 1;
            int points = last * smooth + 1;

            for (int k = 0; k < last; k++)
            {
                int before = Math.Max(0, k - 1);
                int after = Math.Min(last, k + 2);
                for (int s = 0; s < smooth; s++)
                    Spline(before, k, k + 1, after, s / (double)smooth, k * smooth + s);
            }

            _CurveX[points - 1] = _PathX[last];
            _CurveY[points - 1] = _PathY[last];
            _CurveZ[points - 1] = _PathZ[last];

            _CurveArc[0] = 0;
            for (int i = 1; i < points; i++)
            {
                double dx = _CurveX[i] - _CurveX[i - 1];
                double dy = _CurveY[i] - _CurveY[i - 1];
                double dz = _CurveZ[i] - _CurveZ[i - 1];
                _CurveArc[i] = _CurveArc[i - 1] + Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
        }

        /// <summary>
        /// One point of the curve between two centres it runs through, written from the two centres
        /// on either side of them.
        /// </summary>
        private void Spline(int before, int from, int to, int after, double t, int target)
        {
            double t2 = t * t;
            double t3 = t2 * t;
            double c0 = -0.5 * t3 + t2 - 0.5 * t;
            double c1 = 1.5 * t3 - 2.5 * t2 + 1.0;
            double c2 = -1.5 * t3 + 2.0 * t2 + 0.5 * t;
            double c3 = 0.5 * t3 - 0.5 * t2;

            _CurveX[target] = c0 * _PathX[before] + c1 * _PathX[from] + c2 * _PathX[to] + c3 * _PathX[after];
            _CurveY[target] = c0 * _PathY[before] + c1 * _PathY[from] + c2 * _PathY[to] + c3 * _PathY[after];
            _CurveZ[target] = c0 * _PathZ[before] + c1 * _PathZ[from] + c2 * _PathZ[to] + c3 * _PathZ[after];
        }

        /// <summary>
        /// Writes the two vertices that hold one point of a line up, one on either side of it, with
        /// the way the line runs there, which is what the width is put on across.
        /// </summary>
        private void WriteRibbon(int vertex, int i, int points, int colour, float speed)
        {
            double x = _CurveX[i];
            double y = _CurveY[i];
            double z = _CurveZ[i];

            // The way the line runs is taken over the points on either side where there are two of
            // them, so that the two pieces of the ribbon that meet here point the same way and the
            // ribbon does not part at the join.
            double dx;
            double dy;
            double dz;
            if (i <= 0)
            {
                dx = _CurveX[1] - x;
                dy = _CurveY[1] - y;
                dz = _CurveZ[1] - z;
            }
            else if (i >= points - 1)
            {
                dx = x - _CurveX[i - 1];
                dy = y - _CurveY[i - 1];
                dz = z - _CurveZ[i - 1];
            }
            else
            {
                dx = _CurveX[i + 1] - _CurveX[i - 1];
                dy = _CurveY[i + 1] - _CurveY[i - 1];
                dz = _CurveZ[i + 1] - _CurveZ[i - 1];
            }

            double reach = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            if (reach > 0)
            {
                dx /= reach;
                dy /= reach;
                dz /= reach;
            }
            else
            {
                dx = 0;
                dy = 0;
                dz = 1;
            }

            var left = new CustomVertex.PositionNormalColored();
            left.X = (float)x;
            left.Y = (float)y;
            left.Z = (float)z;
            left.Nx = (float)dx;
            left.Ny = (float)dy;
            left.Nz = (float)dz;
            left.Color = colour;

            var right = left;

            _VertexList[vertex] = left;
            _VertexList[vertex + 1] = right;

            float arc = (float)_CurveArc[i];
            _StreamlineFlow[vertex * 3] = arc;
            _StreamlineFlow[vertex * 3 + 1] = speed;
            _StreamlineFlow[vertex * 3 + 2] = 1f;
            _StreamlineFlow[(vertex + 1) * 3] = arc;
            _StreamlineFlow[(vertex + 1) * 3 + 1] = speed;
            _StreamlineFlow[(vertex + 1) * 3 + 2] = -1f;
        }

        /// <summary>
        /// The centre of a cell, where the scene is drawn, which is where a point of a line lies.
        /// </summary>
        private Point3d CellCentre(int cell, double radius, double exaggeration)
        {
            double lon = MathEngine.DegreesToRadians(_MFGrid.CentralPoint[0, 0, cell]);
            double lat = MathEngine.DegreesToRadians(_MFGrid.CentralPoint[1, 0, cell]);
            double height = radius + _MFGrid.CentralPoint[2, 0, cell] * exaggeration;
            return MathEngine.SphericalRadianToCartesianD(lat, lon, height);
        }

        /// <summary>
        /// The cell of the grid that sits at a row and a column, or -1 where there is none. The grid
        /// is asked for it through a dictionary, which is too slow to do for every step of every
        /// line, so the rows and the columns are written into an array once.
        /// </summary>
        private int CellAt(int row, int column)
        {
            if (_CellAt == null)
                return -1;
            if (row < 0 || row >= _CellRows || column < 0 || column >= _CellColumns)
                return -1;
            return _CellAt[row * _CellColumns + column];
        }

        /// <summary>
        /// Fills the array the rows and the columns of the grid are looked up in.
        /// </summary>
        private void BuildLookup(int cellCount)
        {
            int rows = _MFGrid.RowCount;
            int columns = _MFGrid.ColumnCount;
            if (rows <= 0 || columns <= 0)
                return;
            if (_CellAt != null && _CellRows == rows && _CellColumns == columns)
                return;

            var lookup = new int[rows * columns];
            for (int i = 0; i < lookup.Length; i++)
                lookup[i] = -1;

            var locations = _MFGrid.Topology.ActiveCellLocation;
            for (int i = 0; i < cellCount && i < locations.Length; i++)
            {
                var location = locations[i];
                if (location == null || location.Length < 2)
                    continue;
                if (location[0] >= 0 && location[0] < rows && location[1] >= 0 && location[1] < columns)
                    lookup[location[0] * columns + location[1]] = i;
            }

            _CellAt = lookup;
            _CellRows = rows;
            _CellColumns = columns;
        }

        /// <summary>
        /// How much of a degree of longitude one column takes and how much of a degree of latitude
        /// one row takes, both signed, which is what turns a direction into a step over the grid.
        /// They are worked out once and kept, a grid does not change its cells between two steps.
        /// </summary>
        private bool CellSteps(int cellCount, out double lonPerColumn, out double latPerRow)
        {
            lonPerColumn = _LonPerColumn;
            latPerRow = _LatPerRow;
            if (lonPerColumn != 0 && latPerRow != 0)
                return true;

            BuildLookup(cellCount);

            double lon = 0;
            double lat = 0;
            int across = 0;
            int down = 0;
            var locations = _MFGrid.Topology.ActiveCellLocation;
            var centre = _MFGrid.CentralPoint;

            for (int i = 0; i < cellCount && i < locations.Length; i++)
            {
                var location = locations[i];
                if (location == null || location.Length < 2)
                    continue;

                int right = CellAt(location[0], location[1] + 1);
                if (right >= 0 && right < cellCount)
                {
                    lon += centre[0, 0, right] - centre[0, 0, i];
                    across++;
                }

                int below = CellAt(location[0] + 1, location[1]);
                if (below >= 0 && below < cellCount)
                {
                    lat += centre[1, 0, below] - centre[1, 0, i];
                    down++;
                }
            }

            if (across > 0)
                lon /= across;
            if (down > 0)
                lat /= down;

            if (Math.Abs(lon) < 1e-12 || Math.Abs(lat) < 1e-12)
                return false;

            _LonPerColumn = lon;
            _LatPerRow = lat;
            lonPerColumn = lon;
            latPerRow = lat;
            return true;
        }

        public override void UpdateVertexColor()
        {
            DataSource = (Owner.GetPackage(VelocityPackage.PackageName) as VelocityPackage).DataCube;
            if (DataSource != null && RequiredUpdated)
                CreatMeshes();
            NotifyColorsChanged();
        }

        public override void UpdateCachedColor()
        {
            UpdateVertexColor();
        }

        protected void VectorStyle_StyleChanged(object sender, EventArgs e)
        {
            UpdateVertexColor();
        }

    }
}
