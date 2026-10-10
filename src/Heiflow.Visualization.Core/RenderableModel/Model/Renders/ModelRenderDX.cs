using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using HUST.WREIS.Dot3D;
using Heiflow.Core.Drawing;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Generic;
using Heiflow.Core.MyMath;
using HUST.WREIS.Dot3D.Renderable;
using Heiflow.Core.Data;
using Heiflow.Presentation.Controls;
using Heiflow.Models.Generic.Project;
using GeoAPI.Geometries;
using Heiflow.Models.Visualization;

namespace Heiflow.Visualization.Renderable.Grid
{
    public abstract class ModelRenderDX : INotifyPropertyChanged, IDX3DLayerRender
    {
        protected CustomVertex.PositionNormalColored[] _VertexList;
        protected int[] _VertexIndexList;

        protected float _VerticalExaggeration = 3;
        protected Point3d localOrigin; // Add this offset to get world coordinates
        public int[] SelectedVertexIndexes { get; set; }
       // protected Ramp _ColorRamp;
        protected int mOpacity = 255;
        protected int _ColourRampCount = 5;
        protected bool _UniqueColor = false;
        protected bool _InvertColor = false;
        protected ClassificationMethod _ClassificationMethod = Core.Drawing.ClassificationMethod.Natural_Breaks_Jenks;

        /// <summary>
        /// Bumped by every setting that decides how a value turns into a colour. The cached colours
        /// remember the version they were built under, so a cache built under an older scheme is
        /// rebuilt instead of being copied over the vertices again, which is what used to leave a
        /// cached layer showing the colours of the ramp it had before the change.
        /// </summary>
        protected int _SymbologyVersion;
        protected int _CachedSymbologyVersion = -1;
        protected DataCube<int> cachedColor;
        protected float[] cachedMaxValues;
        protected float[] cachedMinValues;
        protected StatisticsInfo[] cachedStatisticsInfo;
        protected StatisticsInfo _StatisticsInfo;
        protected Cell[] _Cells = null;
        protected float[] _GridValues;
        protected DataCube<float> _DataSource;
        protected RegularGridTopology _Topology;
        protected int _UTMZone;
        protected IProject _Project;
        protected World _World;
        protected DataColor _DataColor;
        protected bool _Initialized = false;

        public event EventHandler ColorRampChanged;
        public event EventHandler VariableChanged;
        public event EventHandler LayerChanged;
        public event EventHandler TimeStepChanged;
        public event StatisticsInfoHandler ValueRangeChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler GridValuesChanged;
        public event EventHandler DataSourceChanged;
        public event EventHandler<int> CachingColorProgressChanged;
        public event EventHandler CachingColorFinished;

        public ModelRenderDX( World world)
        {
            SelectedCells = new List<ICell>();
            _World = world;
            this.EquatorialRadius = world.EquatorialRadius;
            Profiles = new List<IProfile>();
            ClassificationMethod = Core.Drawing.ClassificationMethod.Natural_Breaks_Jenks;
            _DataColor = new DataColor();
            DistanceAboveSurface = 10;
        }

        public ModelRenderDX()
        {
            SelectedCells = new List<ICell>();
            Profiles = new List<IProfile>();
            ClassificationMethod = Core.Drawing.ClassificationMethod.Natural_Breaks_Jenks;
            _DataColor = new DataColor();
            DistanceAboveSurface = 10;
        }

        public IBasicModel Owner
        {
            get;
            set;
        }

        public IGrid Grid
        {
            get;
            set;
        }

        public IProject Project
        {
            get
            {
                return _Project;
            }
            set
            {
                _Project = value;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public RenderableObject RenderableObject
        {
            get;
            set;
        }
        public I3DLayer LayerObject
        {
            get;
            set;
        }
        public string SupportedLayerObject
        {
            get;
            set;
        }
        public GCSConverter GCSConverter
        {
            get;
            set;
        }
        #region Grid Properties
        public double EquatorialRadius
        {
            get;
            set;
        }

        public double NoDataValue
        {
            get;
            set;
        }


        public float VerticalExaggeration
        {
            get
            {
                return _VerticalExaggeration;
            }
            set
            {
                _VerticalExaggeration = value;
                OnPropertyChanged("VerticalExaggeration");
            }
        }

        public CustomVertex.PositionNormalColored[] VertexList
        {
            get
            {
                return _VertexList;
            }
        }

        public int[] VertexIndexList
        {
            get
            {
                return _VertexIndexList;
            }
        }

        /// <summary>
        /// Raised every time the geometry of the mesh is rebuilt. The layers compare it with the version
        /// they uploaded and upload again only when it changed.
        /// </summary>
        public int MeshVersion { get; private set; }

        /// <summary>
        /// Raised every time the vertex colours change, a new time step for example. The colours live in
        /// the vertex array, so they are uploaded again as well.
        /// </summary>
        public int ColorVersion { get; private set; }

        /// <summary>
        /// Call at the end of CreatMeshes, the geometry has to be uploaded again.
        /// </summary>
        protected void NotifyMeshChanged()
        {
            MeshVersion++;
            ColorVersion++;
        }

        /// <summary>
        /// Call at the end of UpdateVertexColor, the vertex array has to be uploaded again.
        /// </summary>
        protected void NotifyColorsChanged()
        {
            ColorVersion++;
        }

        public CustomVertex.PositionColored[] SelectedVertexes
        {
            get;
            set;
        }
        public float DistanceAboveSurface
        {
            get;
            set;
        }

        public HUST.WREIS.Dot3D.Angle CenterLatitude
        {
            get;
            protected set;
        }
        public HUST.WREIS.Dot3D.Angle CenterLongitude
        {
            get;
            protected set;
        }

        public Point3d LocalOringin
        {
            get
            {
                return localOrigin;
            }
        }

        public List<ICell> SelectedCells
        {
            get;
            protected set;
        }

        public Envelope BBox { get; set; }
        #endregion

        #region Color Properties
        public int VertexUnifiedColor
        {
            get;
            set;
        }

        public int ColorRampID
        {
            get
            {
                return _DataColor.ColorRamp.RampId;
            }
            set
            {
                if (_DataColor.ColorRamp.RampId != value)
                {
                    _DataColor.ColorRamp = BuildColorRamp(value, _ColourRampCount);
                    _SymbologyVersion++;
                    OnPropertyChanged("ColorRampID");
                }
            }
        }
        public Ramp ColorRamp
        {
            get
            {
                return _DataColor.ColorRamp;
            }
        }

        /// <summary>
        /// A ramp comes out of Ramp(id) with the full set of colours it was defined with, so the
        /// class count the user picked has to be applied to it again. Both the id and the count go
        /// through here, which keeps the two in step, otherwise picking a ramp drops the classes
        /// back to whatever that ramp happens to hold.
        /// </summary>
        protected Ramp BuildColorRamp(int rampId, int classCount)
        {
            var ramp = new Ramp(rampId);
            ramp.Colors = DataColor.Resample(ramp.Colors, classCount);
            return ramp;
        }

        public int ColourRampCount
        {
            get
            {
                return _ColourRampCount;
            }
            set
            {
                if (value > 0 && _ColourRampCount != value)
                {
                    _ColourRampCount = value;
                    _DataColor.ColorRamp = BuildColorRamp(ColorRampID, _ColourRampCount);
                    _SymbologyVersion++;
                    OnPropertyChanged("ColourRampCount");
                }
            }
        }

        /// <summary>
        /// Decides how the values are split into the classes that the ramp colours. This used to be
        /// a plain auto property, so picking another method changed the field and nothing else, and
        /// the colours kept coming from the method that was current when they were last computed.
        /// </summary>
        public ClassificationMethod ClassificationMethod
        {
            get
            {
                return _ClassificationMethod;
            }
            set
            {
                if (_ClassificationMethod != value)
                {
                    _ClassificationMethod = value;
                    _SymbologyVersion++;
                    OnPropertyChanged("ClassificationMethod");
                }
            }
        }
        /// <summary>
        /// 0 表示颜色完全透明，而值255 表示颜色完全不透明
        /// </summary>
        public int Opacity
        {
            get
            {
                return mOpacity;
            }
            set
            {
                mOpacity = value;
                _SymbologyVersion++;
                OnPropertyChanged("Opacity");
            }
        }
        public bool InvertColor
        {
            get
            {
                return _InvertColor;
            }
            set
            {
                _InvertColor = value;
                _SymbologyVersion++;
                OnPropertyChanged("InvertColor");
            }
        }
        public bool UniqueColor
        {
            get
            {
                return _UniqueColor;
            }
            set
            {
                _UniqueColor = value;
                _SymbologyVersion++;
                OnPropertyChanged("UniqueColor");
            }
        }

        #endregion

        #region Data Properties
        public double MaxCellValue
        {
            get;
            protected set;
        }

        public double MinCellValue
        {
            get;
            protected set;
        }

        public float[] GridValues
        {
            get
            {
                return _GridValues;
            }
            protected set
            {
                _GridValues = value;
                if (GridValuesChanged != null)
                    GridValuesChanged(this, EventArgs.Empty);
            }
        }

        public DataCube<float> DataSource
        {
            get
            {
                return _DataSource;
            }
            set
            {
                _DataSource = value;
                if (DataSourceChanged != null)
                {
                    DataSourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public int CurrentTimeStep
        {
            get;
            set;
        }

        public int CurrentLayerIndex
        {
            get
            {
                return ModelService.CurrentGridLayer;
            }
        }
        public int VarIndex
        {
            get;
            set;
        }

        public string CurrentVarName
        {
            get;
            set;
        }


        public StatisticsInfo StatisticsInfo
        {
            get
            {
                return _StatisticsInfo;
            }
            set
            {
                _StatisticsInfo = value;
                OnValueRangeChanged(new StatisticsArgs(_StatisticsInfo));
            }
        }

        public bool UseCache
        {
            get;
            set;
        }
        #endregion

        #region External Object Properties
        public List<IProfile> Profiles
        {
            get;
            protected set;
        }

        #endregion

        public abstract void CreatMeshes();
        public abstract float GetCellValue(int p1, int p2);

        public abstract ICell SelectCell(double lat, double lon);

        public abstract void UpdateVertexColor();

        public abstract void CacheColor();
        public virtual void ClearCachedColor()
        {
            cachedColor = null;
            _CachedSymbologyVersion = -1;
        }

        /// <summary>
        /// The cached colours were computed with the ramp, the class count, the method and the
        /// opacity that were current when the cache was filled. Once any of those moved on the
        /// cache describes a scheme that is no longer the one on screen, and copying it over the
        /// vertices would put the colours of the old scheme back.
        /// </summary>
        protected bool IsCachedColorStale
        {
            get { return cachedColor == null || _CachedSymbologyVersion != _SymbologyVersion; }
        }

        /// <summary>
        /// Call once the cache has been filled under the scheme that is current now.
        /// </summary>
        protected void MarkCachedColorFresh()
        {
            _CachedSymbologyVersion = _SymbologyVersion;
        }

        public abstract void UpdateCachedColor();

        public abstract void UpdateCellValues();
        public virtual void LoadDataSource(string path)
        {

        }
        public virtual Cell[] RetrieveCellsInColumn(int ColumnIndex)
        {
            return null;
        }

        public virtual Cell[] RetrieveCellsInRow(int RowIndex)
        {
            return null;
        }

        public abstract float[] GetCellTimeSeries(int p1, int p2);
        public virtual void Render(float[] vector)
        {
            GridValues = vector;
            UpdateVertexColor();
        }

        public virtual void Initilize()
        {
            VertexUnifiedColor = System.Drawing.Color.Blue.ToArgb();
            _ColourRampCount = 5;
            Opacity = 255;
            NoDataValue = -9999;
            Profiles = new List<IProfile>();
            _Initialized = true;
        }

        public virtual void OnValueRangeChanged(StatisticsArgs args)
        {
            if (ValueRangeChanged != null)
            {
                ValueRangeChanged(this, args);
            }
        }

        public virtual void OnColorRampChanged()
        {
            if (ColorRampChanged != null)
                ColorRampChanged(this, new EventArgs());
        }

        protected virtual void OnVariableChanged()
        {
            if (VariableChanged != null)
                VariableChanged(this, new EventArgs());
        }

        protected virtual void OnLayerChanged()
        {
            if (LayerChanged != null)
                LayerChanged(this, new EventArgs());
        }

        protected virtual void OnTimeStepChanged()
        {
            if (TimeStepChanged != null)
                TimeStepChanged(this, new EventArgs());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (!_Initialized)
                return;
            switch (propertyName)
            {
                case "VerticalExaggeration":
                    CreatMeshes();
                    break;
                case "CurrentLayerIndex":
                    CreatMeshes();
                    break;
                case "ColorRampID":
                    UpdateVertexColor();
                    OnColorRampChanged();
                    break;
                case "Opacity":
                    UpdateVertexColor();
                    break;
                case "ColourRampCount":
                    UpdateVertexColor();
                    OnColorRampChanged();
                    break;
                // the legend draws the class breaks, so moving the class count or the method has to
                // reach it the same way a new ramp does
                case "ClassificationMethod":
                    UpdateVertexColor();
                    OnColorRampChanged();
                    break;
                case "UniqueColor":
                    UpdateVertexColor();
                    break;
                case "InvertColor":
                    UpdateVertexColor();
                    break;
                default:
                    UpdateVertexColor();
                    break;
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual float[] GetGridValues(DataCube<float> array, int timestep)
        {
            float[] vec = DataSource.GetVector(VarIndex, timestep.ToString(), ":");
            return vec;
        }

        public void ComputeLocalOrigin(double lat, double lng)
        {
            float layerRadius = (float)(EquatorialRadius + RenderableObject.DistanceAboveSurface);
            CenterLatitude = new HUST.WREIS.Dot3D.Angle() { Degrees = lat };
            CenterLongitude = new HUST.WREIS.Dot3D.Angle() { Degrees = lng };
            localOrigin = MathEngine.SphericalToCartesianD(CenterLatitude, CenterLongitude, layerRadius);

            // To avoid gaps between neighbouring tiles truncate the origin to 
            // a number that doesn't get rounded. (nearest 10km)
            localOrigin.X = (float)(Math.Round(localOrigin.X / 100000) * 100000);
            localOrigin.Y = (float)(Math.Round(localOrigin.Y / 100000) * 100000);
            localOrigin.Z = (float)(Math.Round(localOrigin.Z / 100000) * 100000);
        }

        protected void CalculateNormals(ref CustomVertex.PositionNormalColored[] vertices, int[] indices)
        {
            ModelNormals.Calculate(vertices, indices);
        }

        public int GetVertexColor(double max, double min, double averagedValue, int alpha)
        {
            double level = 0;
            return GetVertexColor(max, min, averagedValue, alpha, out level);
        }

        public virtual int GetVertexColor(double max, double min, double averagedValue, int alpha, out double level)
        {
            level = 0;
            if (averagedValue == NoDataValue)
            {
                return System.Drawing.Color.Transparent.ToArgb();
            }
            System.Drawing.Color color = System.Drawing.Color.White;
            if (max == min)
                return color.ToArgb();
            else
            {
                level = (averagedValue - min) / (max - min) * _DataColor.ColorRamp.Colors.Length - 1;

                if (level < 0)
                {
                    color = _DataColor.ColorRamp.Colors[0];
                    alpha = 0;
                }
                else
                {
                    if (level < _DataColor.ColorRamp.Colors.Length)
                        color = _DataColor.ColorRamp.Colors[(int)level];
                    else
                    {
                        color = _DataColor.ColorRamp.Colors[_DataColor.ColorRamp.Colors.Length - 1];
                    }
                }
                return System.Drawing.Color.FromArgb(alpha, color).ToArgb();
            }
        }

        protected void OnCachingColorPrgChanged(int prg)
        {
            if(CachingColorProgressChanged!= null)
            {
                CachingColorProgressChanged(this, prg);
            }
        }

        protected void OnCachingColorFinished(EventArgs e)
        {
            if (CachingColorProgressChanged != null)
            {
                CachingColorFinished(this, e);
            }
        }
    }
}
