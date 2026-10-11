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

        /// <summary>
        /// Off by default, which leaves the values in classes. On, they are scaled over one range for
        /// the whole run instead, see ContinuousColor.
        /// </summary>
        protected bool _ContinuousColor = false;

        /// <summary>
        /// Whether the sun and the ambient are allowed to take a shade off the colours, see
        /// ShadedLighting. Off by default, a layer that shows values is read from the ramp and not
        /// from how the sun happens to fall on it.
        /// </summary>
        protected bool _ShadedLighting = false;
        protected ClassificationMethod _ClassificationMethod = Core.Drawing.ClassificationMethod.Natural_Breaks_Jenks;

        /// <summary>
        /// Bumped by a setting that decides what colour a class or a shade is. The palette remembers
        /// the version it was built under, so it is made again when the ramp, the class count, the
        /// opacity or the inversion moved on, and stays a plain lookup the rest of the time.
        /// </summary>
        protected int _SymbologyVersion;

        /// <summary>
        /// Bumped by a setting that decides which class or which shade a value falls into, which is
        /// not every setting that decides what colour that class or shade is. What a cache holds is
        /// the class or the shade of every vertex and not its colour, so another ramp or another
        /// opacity leaves the cache standing and only the palette is made again, while another class
        /// count, another method or the switch to shades throws it away. Keeping the two apart is
        /// what stops picking a ramp from refilling the cache of every step.
        /// </summary>
        protected int _SchemeVersion;
        protected int _CachedSchemeVersion = -1;

        /// <summary>
        /// True on a render whose cache holds colours rather than the class or the shade of every
        /// vertex, which a layer that draws its values by interpolating the ramp does. Such a cache
        /// has to be filled again when the ramp, the opacity or the inversion moves, since what it
        /// holds is the colour of the ramp it was built under and not a key into it.
        /// </summary>
        protected bool _CacheHoldsColor = false;

        protected DataCube<int> cachedColor;

        /// <summary>
        /// The class a vertex falls into, one byte for every vertex of every step, laid out step by
        /// step. A layer that classifies its values keeps this rather than a colour, a byte being a
        /// quarter of what an integer would take, and the colour comes from the table below.
        /// </summary>
        protected byte[] cachedLevel;

        /// <summary>
        /// The range the shades are scaled over while ContinuousColor is on, the extremes of every
        /// step taken together, so that a shade stands for the same value on every step. Empty until
        /// the cache has been filled under that setting.
        /// </summary>
        protected float _ScaleMin;
        protected float _ScaleMax;

        /// <summary>
        /// The colour of every class, so that a class index is turned into a colour by looking it up
        /// instead of asking the ramp again for every vertex of every frame.
        /// </summary>
        private int[] _LevelPalette;
        private int _CachedPaletteVersion = -1;

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
                    // Which class or shade a value falls into does not depend on what colour that
                    // class or shade is, so a cache of classes or shades holds and only the palette is
                    // made again. This is the setting that is picked the most often, and refilling the
                    // cache for it was what made picking a ramp cost the whole run.
                    BumpColorScheme();
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
                    BumpClassScheme();
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
                    BumpClassScheme();
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
                // The opacity is how the colour is drawn and not which colour a value is given, so
                // the cache holds and only the palette is made again. Dragging the slider sends a
                // value for every step of the way, and none of them pay for the cache.
                BumpColorScheme();
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
                // Inverting takes the colours the other way round and leaves the class or the shade
                // of every vertex where it is, so the cache holds.
                BumpColorScheme();
                OnPropertyChanged("InvertColor");
            }
        }

        /// <summary>
        /// A layer is coloured in classes while this is off, a handful of colours for the whole grid,
        /// and the classes are found again for every step of the animation, so the colour a value
        /// stands for moves from one step to the next and the picture flickers as it plays.
        ///
        /// While it is on the values are scaled over one range that covers every step at once and
        /// take every shade of the ramp rather than one of a few. A step then only changes the shade
        /// of a vertex, it no longer moves the scale underneath it, and shades that are next to each
        /// other on the ramp are next to each other on the grid. The cached byte per vertex holds a
        /// shade just as well as it held a class, so this costs no more memory.
        /// </summary>
        public bool ContinuousColor
        {
            get
            {
                return _ContinuousColor;
            }
            set
            {
                if (_ContinuousColor != value)
                {
                    _ContinuousColor = value;
                    _SymbologyVersion++;
                    // A shade and a class are not the same thing, so a cache that holds one of them
                    // cannot be asked for the other.
                    _SchemeVersion++;
                    OnPropertyChanged("ContinuousColor");
                }
            }
        }

        /// <summary>
        /// Bump the colours, and the classes behind them while the values are still put in classes.
        /// Under continuous shading neither the class count nor the method has a say in which shade a
        /// value falls into, they only decide what the shades look like, so there they leave the cache
        /// standing and only the palette is made again.
        /// </summary>
        private void BumpClassScheme()
        {
            _SymbologyVersion++;
            if (!_ContinuousColor)
            {
                _SchemeVersion++;
            }
        }

        /// <summary>
        /// Bump the colours, and the cache with them only where the cache is one of colours. A cache
        /// of classes or shades is asked again for a key and reads the colour off the palette that
        /// was just made, so it needs no filling.
        /// </summary>
        private void BumpColorScheme()
        {
            _SymbologyVersion++;
            if (_CacheHoldsColor)
            {
                _SchemeVersion++;
            }
        }

        /// <summary>
        /// On draws the colour of a vertex as the sun and the ambient leave it, which is what the
        /// fixed pipeline did with a layer it lit. Off draws it as the ramp gives it, with nothing
        /// taken off, which is what the fixed pipeline did with a layer it did not light. It is off
        /// by default, the sun falling on a layer that shows values only gets in the way of reading
        /// them. It is the drawing that changes and not the scheme, so no cache is thrown away.
        /// </summary>
        public bool ShadedLighting
        {
            get
            {
                return _ShadedLighting;
            }
            set
            {
                if (_ShadedLighting != value)
                {
                    _ShadedLighting = value;
                    OnPropertyChanged("ShadedLighting");
                }
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
                // A unique value is looked up rather than found, so what a cache holds for every
                // vertex is no longer what the layer is asking for.
                _SchemeVersion++;
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

        private int _CurrentTimeStep;

        /// <summary>
        /// The step the picture shows. Setting it is what starts the mix from the step before to this
        /// one, see StepBlend, and what tells the step textures they have to be filled again.
        /// </summary>
        public int CurrentTimeStep
        {
            get { return _CurrentTimeStep; }
            set
            {
                if (_CurrentTimeStep != value)
                {
                    _CurrentTimeStep = value;
                    _StepStarted = DateTime.UtcNow;
                }
            }
        }

        private int _StepDuration = 500;

        /// <summary>
        /// How long one step of the animation takes, in milliseconds. The animation hands it over so
        /// that the mix between two steps can be taken from the clock instead of from a timer of its
        /// own, which would have to be kept in step with the one that drives the playback.
        /// </summary>
        public int StepDuration
        {
            get { return _StepDuration; }
            set { _StepDuration = value; }
        }

        private DateTime _StepStarted = DateTime.UtcNow;

        /// <summary>
        /// How far the picture has come from the step before to the one it is at, 0 at the moment the
        /// step changed and 1 once the step has had its full length. Read from the clock on every
        /// frame, so showing the steps in between is a matter of handing one float to the card.
        /// </summary>
        public float StepBlend
        {
            get
            {
                if (_StepDuration <= 0)
                    return 1f;
                var elapsed = (DateTime.UtcNow - _StepStarted).TotalMilliseconds / _StepDuration;
                if (elapsed <= 0)
                    return 0f;
                if (elapsed >= 1)
                    return 1f;
                return (float)elapsed;
            }
        }

        /// <summary>
        /// Which texel of the step texture carries which vertex, see ModelColorShader. The vertices of
        /// a grid that is laid out in rows and columns do not arrive in the order the texels of such
        /// a texture do, so the renderer says where each of them goes. Without one the mesh is drawn
        /// the way it always was.
        /// </summary>
        protected int[] _VertexTexel;
        protected int _DataTextureWidth;
        protected int _DataTextureHeight;

        /// <summary>
        /// Whether the texels round a vertex can be mixed into its colour. Only a grid that is laid out
        /// in rows and columns can say yes, see the two techniques of ModelColor.fx.
        /// </summary>
        protected bool _SmoothStepTexture;

        /// <summary>
        /// The widest a texture may be on a card of this age, which is how wide the step texture of a
        /// grid that has no shape is cut.
        /// </summary>
        protected const int MaxStepTextureWidth = 2048;

        private bool _ShadingActive;

        /// <summary>
        /// True while the mesh is drawn from the step textures. The renderer then leaves the colours in
        /// the vertices alone, and the card is not handed the whole vertex array again every time the
        /// step moves on.
        /// </summary>
        public bool ShadingActive
        {
            get { return _ShadingActive; }
        }

        public int[] VertexTexel
        {
            get { return _VertexTexel; }
        }

        public int DataTextureWidth
        {
            get { return _DataTextureWidth; }
        }

        public int DataTextureHeight
        {
            get { return _DataTextureHeight; }
        }

        private ModelColorShader _shader;
        private int _shadedStep = -1;
        private int _rampVersion = -1;

        /// <summary>
        /// Off sends every layer back to the fixed pipeline, which is how a shader that misbehaves on
        /// one card is told apart from a shader that is wrong everywhere.
        /// </summary>
        public static bool AllowShadedColors = true;

        /// <summary>
        private static readonly float[] WhiteAmbient = { 1f, 1f, 1f };

        /// <summary>
        /// Draws the mesh with the colours taken from the step textures. Returns false whenever that
        /// cannot be done, a card without shaders, no cache to fill the textures from, or a grid that
        /// never said where its vertices go, and the layer then draws the mesh as it always did.
        /// </summary>
        public bool DrawShaded(Device device, ModelMeshBuffer mesh, PrimitiveType primitiveType,
            Matrix worldViewProjection, Vector3 sunDirection, float sunLight, float[] ambient)
        {
            if (!AllowShadedColors || mesh == null || _VertexTexel == null || cachedLevel == null
                || _DataTextureWidth <= 0 || _DataTextureHeight <= 0)
            {
                _ShadingActive = false;
                return false;
            }

            if (_shader == null)
                _shader = new ModelColorShader();

            if (!_shader.Ensure(device, _VertexTexel, _DataTextureWidth, _DataTextureHeight))
            {
                _ShadingActive = false;
                return false;
            }

            var vertexCount = _VertexTexel.Length;

            // The step the animation came from and the one it is at. They are filled when the step
            // moves on and not on every frame, which is the whole point of the two textures.
            if (_shadedStep != _CurrentTimeStep)
            {
                var before = _CurrentTimeStep > 0 ? _CurrentTimeStep - 1 : _CurrentTimeStep;
                _shader.UploadStep(device, cachedLevel, before, vertexCount, true);
                _shader.UploadStep(device, cachedLevel, _CurrentTimeStep, vertexCount, false);
                _shadedStep = _CurrentTimeStep;
            }

            // The ramp only moves when the scheme behind it does.
            if (_rampVersion != _SymbologyVersion)
            {
                _shader.UploadRamp(LevelPalette);
                _rampVersion = _SymbologyVersion;
            }

            // Nothing of the sun and nothing of the ambient is taken off the colour while the lighting
            // is off, which leaves the ramp as it was drawn.
            var sun = ShadedLighting ? sunLight : 0f;
            var floor = ShadedLighting ? ambient : WhiteAmbient;

            _ShadingActive = _shader.Draw(device, mesh.VertexBuffer, mesh.IndexBuffer, mesh.VertexCount,
                mesh.IndexCount, primitiveType, worldViewProjection, sunDirection, sun, floor,
                Opacity / 255f, StepBlend, _SmoothStepTexture);
            return _ShadingActive;
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
            cachedLevel = null;
            _CachedSchemeVersion = -1;
        }

        /// <summary>
        /// What the cache holds is the class or the shade of every vertex, so it is the scheme behind
        /// those that decides whether it still holds, and not the colours they are drawn in. The
        /// colours come from the palette, which is made again on its own.
        /// </summary>
        protected bool IsCachedColorStale
        {
            get { return (cachedColor == null && cachedLevel == null) || _CachedSchemeVersion != _SchemeVersion; }
        }

        /// <summary>
        /// The colour of every class under the scheme that is current now. The ramp, the class count,
        /// the opacity and whether the ramp is inverted each bump the version, so the table is made
        /// again whenever one of them moved on and is a plain lookup the rest of the time.
        /// </summary>
        protected int[] LevelPalette
        {
            get
            {
                if (_LevelPalette == null || _CachedPaletteVersion != _SymbologyVersion)
                {
                    _LevelPalette = ContinuousColor ? BuildShadedPalette() : BuildClassPalette();
                    _CachedPaletteVersion = _SymbologyVersion;
                }
                return _LevelPalette;
            }
        }

        /// <summary>
        /// One colour for every class of the ramp, which is what a layer drawn in classes takes.
        /// </summary>
        private int[] BuildClassPalette()
        {
            var count = _DataColor.ColorRamp.Colors.Length;
            var palette = new int[count];
            for (int l = 0; l < count; l++)
            {
                palette[l] = _DataColor.GetVertexColor(l, Opacity, InvertColor);
            }
            return palette;
        }

        /// <summary>
        /// The ramp spread over as many shades as a byte can name. The ramp of the render keeps the
        /// classes the user picked, so that is what the legend shows, and only what is drawn takes
        /// the fine one.
        /// </summary>
        private int[] BuildShadedPalette()
        {
            var shades = DataColor.Resample(_DataColor.ColorRamp.Colors, 256);
            var palette = new int[256];
            for (int l = 0; l < 256; l++)
            {
                palette[l] = System.Drawing.Color.FromArgb(Opacity, shades[InvertColor ? 255 - l : l]).ToArgb();
            }
            return palette;
        }

        /// <summary>
        /// Which of the two hundred and fifty six shades a value stands for, given how far it sits
        /// above the bottom of the range and how much of that range one shade covers. A value outside
        /// the range ends up on the shade at that end.
        /// </summary>
        protected static int ShadeOf(float aboveMin, float scale)
        {
            var shade = (int)(aboveMin * scale);
            if (shade < 0)
            {
                shade = 0;
            }
            else if (shade > 255)
            {
                shade = 255;
            }
            return shade;
        }

        /// <summary>
        /// Call once the cache has been filled under the scheme that is current now.
        /// </summary>
        protected void MarkCachedColorFresh()
        {
            _CachedSchemeVersion = _SchemeVersion;
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
            if (CachingColorFinished != null)
            {
                CachingColorFinished(this, e);
            }
        }
    }
}
