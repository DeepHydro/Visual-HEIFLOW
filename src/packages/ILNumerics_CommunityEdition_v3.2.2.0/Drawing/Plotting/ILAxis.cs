///
///    This file is part of ILNumerics Community Edition.
///
///    ILNumerics Community Edition - high performance computing for applications.
///    Copyright (C) 2006 - 2013 Haymo Kutschbach, http://ilnumerics.net
///
///    ILNumerics Community Edition is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License version 3 as published by
///    the Free Software Foundation.
///
///    ILNumerics Community Edition is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with ILNumerics Community Edition. See the file License.txt in the root
///    of your distribution package. If not, see <http://www.gnu.org/licenses/>.
///
///    In addition this software uses the following components and/or licenses: 
///
///    =================================================================================
///    The Open Toolkit Library License
///    
///    Copyright (c) 2006 - 2009 the Open Toolkit library.
///    
///    Permission is hereby granted, free of charge, to any person obtaining a copy
///    of this software and associated documentation files (the "Software"), to deal
///    in the Software without restriction, including without limitation the rights to 
///    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
///    the Software, and to permit persons to whom the Software is furnished to do
///    so, subject to the following conditions:
///
///    The above copyright notice and this permission notice shall be included in all
///    copies or substantial portions of the Software.
///
///    =================================================================================
///    Intel® Math Kernel Library 11.1 for Windows
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Intel® Math Kernel Library 10.3 for Linux
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Products / Software which is implicitly used by ILNumerics due to the inclusion 
///    of 3rd party components: 
///  
///        BLAS/ LAPACK; see: http://netlib.org
///        FFT Functions; see: http://www.spiral.net, http://fftw.org
///        OpenGL; see: http://opengl.org
///
///    =================================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization; 

namespace ILNumerics.Drawing.Plotting {
    /// <summary>
    /// The class implements an axis for displaying scaling information
    /// </summary>
    [Serializable]
    public class ILAxis : ILGroup {

        #region attributes
        AxisNames m_axisName;
        /// <summary>
        /// Default node tag for new instances of ILAxis
        /// </summary>
        public static readonly string AxisGroupTag = "AxisGroup";
        /// <summary>
        /// Default node tag for new instances of axis' ticks collection, default: 'TicksCollectionGroup'
        /// </summary>
        public static readonly string TicksTag = "TicksCollectionGroup";
        /// <summary>
        /// Default node tag for new instances of axis' label, default: 'AxesLabel'
        /// </summary>
        public static readonly string LabelTag = "AxesLabel";
        /// <summary>
        /// Default node tag for new instances of axis' scale label, default: 'ScaleLabel'
        /// </summary>
        public static readonly string ScaleLabelTag = "ScaleLabel";
        /// <summary>
        /// Default node tag for new instances of axis' grid major lines, default: 'GridMajorLines'
        /// </summary>
        public static readonly string GridMajorLinesTag = "GridMajorLines";
        /// <summary>
        /// Default node tag for new instances of axis' grid minor lines, default: 'GridMinorLines'
        /// </summary>
        public static readonly string GridMinorLinesTag = "GridMinorLines";
        /// <summary>
        /// Default padding (distance) between individual ticks, default: 7
        /// </summary>
        public static int TickLabelPadding = 7;
        /// <summary>
        /// The relative position of the axis main label, if LabelPosition is null. Default: follow axis at (0.5,1) (axis center, close to tick label)
        /// </summary>
        public static PointF DefaultLabelPosition = new PointF(.5f, 1f);
        /// <summary>
        /// The relative position of the axis tick scale label, if ScaleLabelPosition is null. Default: follow axis at (1,1) (upper axis end, close to tick label)
        /// </summary>
        public static PointF DefaultScaleLabelPosition = new PointF(1, 1.1f);
        private IILAxisDataProvider m_axisDataProvider;
        private Vector3? m_position;
        private Vector3 m_direction;
        private float? m_min;
        private float? m_max;
        private PointF? m_labelAnchor;
        private PointF? m_scaleLabelAnchor;
        private Vector3? m_labelPosition;
        private Vector3? m_scaleLabelPosition;
        private float? m_labelRotation;
        private int m_dataProviderID;
        #endregion

        #region properties 
        /// <summary>
        /// Gets/ sets the id of the data provider this axis retrieves its data from
        /// </summary>
        [XmlAttribute]
        public int DataProviderID {
            get {
                return m_dataProviderID;
            }
            set {
                if (m_dataProviderID != value) {
                    m_dataProviderID = value;
                    m_axisDataProvider = null; 
                    OnPropertyChanged("DataProviderID");
                }
            }
        }

        /// <summary>
        /// The object providing neccessary data for this axis; default: first IILAxisDataProvider on the path up to the root
        /// </summary>
        [XmlIgnore]
        public IILAxisDataProvider DataProvider {
            get {
                if (m_axisDataProvider == null) {
                    if (DataProviderID != -1) {
                        // find matching data provider by ID
                        ILGroup root = this;
                        while (root.Parent != null) {
                            root = root.Parent;
                        }
                        m_axisDataProvider = root.FindById<ILGroup>(DataProviderID) as IILAxisDataProvider;
                    } else {
                        // find first IILAxisDataProvider by subsequently walking up the path to the root
                        ILGroup root = this;
                        do {
                            m_axisDataProvider = root.First<ILGroup>(predicate: g => g is IILAxisDataProvider) as IILAxisDataProvider;
                            root = root.Parent;
                        } while (m_axisDataProvider == null && root != null); 
                    }
                }
                return m_axisDataProvider; 
            }
            //set {
            //    if (value != m_axisDataProvider) {
            //        m_axisDataProvider = value;
            //        OnPropertyChanged("DataProvider"); 
            //    }
            //}
        }

        #region Auto Mode Properties
        /// <summary>
        /// Anchor for the main axis label, range: (0,0)..(1,1); null: automatic
        /// </summary>
        /// <remarks>This value overrides any setting of the Label.Anchor property.
        /// <para>Leaving this value to <code>null</code> will automatically find a good looking setting according to the current orientation and position 
        /// of the label.</para>
        /// <para>Custom settings of this property align the labels anchor point relative to the labels size. (0,0) is the upper left corner of the label, 
        /// (1,1) corresponds to the lower right corner. The anchor point is addressed by the position and used as reference point for any rotation of the 
        /// label.</para>
        /// </remarks>
        [ILXmlSerializeAs("{X},{Y}")]
        public PointF? LabelAnchor {
            get {
                return m_labelAnchor;
            }
            set {
                if (m_labelAnchor != value) {
                    m_labelAnchor = value;
                    OnPropertyChanged("LabelAnchor");
                }
            }
        }
        /// <summary>
        /// Anchor for the axis scale label, range: 0..1, null: automatic (default)
        /// </summary>
        /// <remarks>This value overrides any setting of the ScaleLabel.Anchor property!
        /// <para>Leaving this value to <code>null</code> will automatically find a good looking setting 
        /// according to the current orientation and position of the label and the current position of 
        /// the main label.</para>
        /// <para>A custom setting of this property aligns the labels anchor point relative to the 
        /// scale labels size. (0,0) is the upper left corner of the label, 
        /// (1,1) corresponds to the lower right corner. The anchor point is addressed by the position 
        /// and used as reference point for any rotation of the label. See <see cref="ILLabel"/> for <code>ILLabel.Anchor</code> usage.</para>
        /// </remarks>
        [ILXmlSerializeAs("{X},{Y}")]
        public PointF? ScaleLabelAnchor {
            get {
                return m_scaleLabelAnchor;
            }
            set {
                if (m_scaleLabelAnchor != value) {
                    m_scaleLabelAnchor = value;
                    OnPropertyChanged("ScaleLabelAnchor");
                }
            }
        }
        /// <summary>
        /// Default position of the label relative to the current axis position, size and orientation; null: automatic (default)
        /// </summary>
        /// <remarks>This value overrides any setting of the Label.Position property.
        /// <para>Leaving this value to <code>null</code> will automatically find a good looking setting according to the current orientation and position 
        /// of the axis.</para>
        /// <para>Custom settings of this property position the labels anchor point relative to the axis orientation and position. </para>
        /// <para>The X coordinate corresponds to the length of the axis with 0 being the lower axis end (i.e. the end with the lower value) and 1 being the
        /// upper axis end.</para>
        /// <para>The Y coordinate corresponds to the direction pointing outside along the ticks direction perpendicular to the axis. 0 corresponds to a 
        /// position on the axis line, 1 corresponds to the distance along that direction, which equals the tick length plus the size of the tick label.</para>
        /// <para>Note, the relevant size of the tick label depend on the current roation of the axis, for vertical axes, the width of the label is commonly 
        /// more relevant than the height. This is taken into account automatically, so a setting of (0,1) will place the label always on the lower end and 
        /// outside of the tick labels area - regardless of the axis orientation.</para>
        /// </remarks>
        public Vector3? LabelPosition {
            get {
                return m_labelPosition;
            }
            set {
                if (m_labelPosition != value) {
                    m_labelPosition = value;
                    OnPropertyChanged("LabelPosition");
                }
            }
        }
        /// <summary>
        /// Position of the scale label relative to the current axis position, size and orientation; null: automatic (default)
        /// </summary>
        /// <remarks>This value overrides any setting of the ScaleLabel.Position property.
        /// <para>Leaving this value to <code>null</code> will automatically find a good looking setting according to the current orientation and position 
        /// of the axis and the current position of the main axis label.</para>
        /// <para>Custom settings of this property position the scale labels anchor point relative to the axis orientation and position. </para>
        /// <para>The X coordinate corresponds to the length of the axis with 0 being the lower axis end (i.e. the end with the lower value) and 1 being the
        /// upper axis end.</para>
        /// <para>The Y coordinate corresponds to the direction pointing outside along the ticks direction perpendicular to the axis. 0 corresponds to a 
        /// position on the axis line, 1 corresponds to the distance along that direction, which equals the tick length plus the size of the tick label.</para>
        /// <para>Note, the relevant size of the tick label depend on the current roation of the axis, for vertical axes, the width of the label is usually 
        /// more relevant than the height. This is taken into account automatically, so a setting of (0,1.1) will place the label always on the lower end and 
        /// outside of the tick labels area - regardless of the axis orientation.</para>
        /// </remarks>
        public Vector3? ScaleLabelPosition {
            get {
                return m_scaleLabelPosition;
            }
            set {
                if (m_scaleLabelPosition != value) {
                    m_scaleLabelPosition = value;
                    OnPropertyChanged("ScaleLabelPosition");
                }
            }
        }
        /// <summary>
        /// Rotation for the main axis label; null: the label follows the axis orientation (default)
        /// </summary>
        /// <remarks>
        /// <para>If this property is null the axis main label will always follow the orientation of this axis (default). Otherwise, the value set to LabelRotation 
        /// will define the (fixed) rotation value for the main axis label.</para>
        /// <para>This property overrides any value may be configured for the Label.Rotation property.</para></remarks>
        [XmlAttribute]
        public float? LabelRotation {
            get {
                return m_labelRotation;
            }
            set {
                if (m_labelRotation != value) {
                    m_labelRotation = value;
                    OnPropertyChanged("LabelRotation");
                }
            }
        }
        #endregion

        #region common axis properties
        /// <summary>
        /// Get the type of the axis (XAxis, YAxis or ZAxis) or sets it
        /// </summary>
        [XmlAttribute]
        public AxisNames AxisName {
            get { return m_axisName; }
            set {
                if (m_axisName != value) {
                    m_axisName = value;
                    Vector3 dir = new Vector3();
                    switch (value) {
                        case AxisNames.XAxis:
                            dir.X = 1;
                            break;
                        case AxisNames.YAxis:
                            dir.Y = 1;
                            break;
                        default:
                            dir.Z = 1;
                            break;
                    }
                    Direction = dir;
                    OnPropertyChanged("AxisName"); 
                }
            }
        }

        /// <summary>
        /// Gets the axis position or sets it. The position is the start/lower end of the axis. null: automatic (default)
        /// </summary>
        public Vector3? Position {
            get {
                return m_position;
            }
            set {
                if (m_position != value) {
                    m_position = value;
                    OnPropertyChanged("Position");
                }
            }
        }
        
        /// <summary>
        /// The direction of the axis. Default: automatic setting according to the axis name (XAxis, YAxis or ZAxis)
        /// </summary>
        public Vector3 Direction {
            get {
                return m_direction;
            }
            set {
                if (m_direction != value) {
                    m_direction = value;
                    OnPropertyChanged("Direction");
                }
            }
        }

        /// <summary>
        /// The minimum value for the axis range, if this property is not null, its value will override the actual value 
        /// taken from the assigned data container within the plot cube. Default: null
        /// </summary>
        [XmlAttribute]
        public float? Min {
            get {
                return m_min;
            }
            set {
                if (m_min != value) {
                    m_min = value;
                    OnPropertyChanged("Min");
                }
            }
        }

        /// <summary>
        /// The maximum value for the axis range, if this property is not null, its value will override the actual value 
        /// taken from the assigned data container within the plot cube. Default: null
        /// </summary>
        [XmlAttribute]
        public float? Max {
            get {
                return m_max;
            }
            set {
                if (m_max != value) {
                    m_max = value;
                    OnPropertyChanged("Max");
                }
            }
        }

        /// <summary>
        /// Access the axis main label for configuration
        /// </summary>
        [XmlIgnore]
        public ILLabel Label { get { return First<ILLabel>(LabelTag); } }
        /// <summary>
        /// Access the axis scale label for configuration. 
        /// </summary>
        /// <remarks>The scale label displays the tick value scale factor for abbreviated tick values outside of the allowed display range width.
        /// Individual values for the ScaleLabel.Position will be ignored. The scale label is always displayed at the position of the tick with the lowest screen Y coordinate.</remarks>
        [XmlIgnore]
        public ILLabel ScaleLabel { get { return First<ILLabel>(ScaleLabelTag); } }
        /// <summary>
        /// Access to the ticks collection group
        /// </summary>
        [XmlIgnore]
        public ILTickCollection Ticks { get { return First<ILTickCollection>(TicksTag); } }
        /// <summary>
        /// Major grid lines for axes ticks
        /// </summary>
        [XmlIgnore]
        public ILLines GridMajor { get { return First<ILLines>(GridMajorLinesTag); } }
        /// <summary>
        /// Minor grid lines for axes ticks
        /// </summary>
        [XmlIgnore]
        public ILLines GridMinor { get { return First<ILLines>(GridMinorLinesTag); } }
        #endregion
        #endregion

        protected ILAxis() { }
        internal ILAxis(ILAxis source)
            : base(source) {
            m_axisName = source.m_axisName;
            m_direction = source.m_direction;
            m_position = source.m_position;
            m_min = source.m_min;
            m_max = source.m_max;
            m_labelPosition = source.m_labelPosition;
            m_labelRotation = source.m_labelRotation;
            m_labelAnchor = source.m_labelAnchor;
            m_scaleLabelAnchor = source.m_scaleLabelAnchor;
            m_scaleLabelPosition = source.m_scaleLabelPosition;
            m_dataProviderID = source.m_dataProviderID; // must be re-connected in ILPlotCubeScaleGroup.Copy()!
            
        }
        public ILAxis(IILAxisDataProvider dataProvider, object tag = null)
            : base(tag ?? AxisGroupTag) {

            Add(new ILTickCollection(tag: TicksTag));
            Add(new ILLabel(tag: LabelTag));
            Add(new ILLabel(tag: ScaleLabelTag));
            Add(new ILLines(tag: GridMajorLinesTag));
            Add(new ILLines(tag: GridMinorLinesTag)); 

            AxisName = AxisNames.XAxis;
            Direction = new Vector3(1, 0, 0);
            Label.Text = AxisName.ToString();
            Ticks.Color = Color.DarkGray;
            GridMajor.Color = Color.LightGray;
            GridMajor.DashStyle = DashStyle.Dotted; 
            GridMinor.Color = Color.LightGray; 
            LabelRotation = null;
            LabelAnchor = null;
            ScaleLabelAnchor = null; 
            ScaleLabelPosition = null;
            DataProviderID = (dataProvider != null) ? dataProvider.ID : -1; 
            // disable selections 
            GridMajor.Selectable = false; 
            GridMinor.Selectable = false; 
        }

        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILAxis();
        }
        internal override ILNode Copy() {
            return new ILAxis(this);
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILAxis ret = (ILAxis)base.Synchronize(copy, syncParams);
            if (copy == null || copy.SynchedVersion != Version) {
                // first time synchronize -> creates and sets up new axis
                //ret.Ticks = ret.FindById<ILTickCollection>(Ticks.ID);
                //ret.Label = ret.FindById<ILLabel>(Label.ID);
                //ret.ScaleLabel = ret.FindById<ILLabel>(ScaleLabel.ID);
                //ret.GridMajor = ret.FindById<ILLines>(GridMajor.ID);
                //ret.GridMinor = ret.FindById<ILLines>(GridMinor.ID); 
                ret.Direction = Direction;
                ret.m_axisName = AxisName;
                ret.DataProviderID = DataProviderID; // gets re-wired in ILPlotCubeScaleGroup.Synchronize()
                ret.Position = Position;
                ret.Min = Min;
                ret.Max = Max;
                ret.LabelPosition = LabelPosition;
                ret.LabelRotation = LabelRotation;
                ret.LabelAnchor = LabelAnchor;
                ret.ScaleLabelPosition = ScaleLabelPosition; 
                ret.ScaleLabelAnchor = ScaleLabelAnchor; 
            }
            return ret;
        }

        internal void ConfigureAxis(ILRenderParameter parameter, ILViewAxesParameters viewParams) {
            if (!Visible) return; 

            float min = 0; 
            float max = 0;
            if (!Max.HasValue || !Min.HasValue) {
                min = DataProvider.GetRangeMinValue(AxisName);
                max = DataProvider.GetRangeMaxValue(AxisName);
            }
            if (Max.HasValue) max = Max.Value; 
            if (Min.HasValue) min = Min.Value; 

            Vector3 startWorld, endWorld;
            System.Diagnostics.Debug.Assert(viewParams.UnitCube.ContainsKey((int)AxisName));
            startWorld = Position.HasValue ? Position.Value : viewParams.UnitCube[(int)AxisName].Item1;
            endWorld = startWorld + Direction; // startWorld + viewParams.UnitCube[(int)AxisName].Item2; 
            int tickCount = getOptimalTickNumber(viewParams);

            if (Ticks.Mode == TickMode.Auto) {
                Ticks.Replace(Ticks.TickCreationFunc(min, min < max ? max : min, tickCount));
            }
            using (ILScope.Enter()) {
                #region create / configure axis & tick lines
                ILArray<float> tickPos = ILMath.zeros<float>(3, (Ticks.Count + 1) * 2);
                float[] tickPosArr = tickPos.GetArrayForWrite();
                tickPosArr[0] = startWorld.X; tickPosArr[1] = startWorld.Y; tickPosArr[2] = startWorld.Z;
                tickPosArr[3] = endWorld.X; tickPosArr[4] = endWorld.Y; tickPosArr[5] = endWorld.Z;
                int i = 6;
                Vector3 a = (endWorld - startWorld) / (max - min);
                Vector3 tickDirWorldStart = new Vector3();
                Vector3 tickDirWorldEnd = new Vector3(); 
                Vector3 tickDirScreen = new Vector3();
                float rotatedTickSize = 0, tickDirAngleToYAxis = 0;
                PointF anchor = new PointF();
                getTicksScreenParameter(startWorld, endWorld, parameter, ref tickDirScreen, ref anchor,
                                        ref tickDirWorldStart, ref tickDirWorldEnd, ref rotatedTickSize, ref tickDirAngleToYAxis);
                //// add the user rotation setting to the tickdir angle
                //tickDirAngleToYAxis += TickLabelRotation; 

                // length of tick lines should not depend on the viewport! if viewport is scaled down -> increase length here!
                SizeF vpScale = parameter.ViewportScaleFactor;
                float vpTickLenScale = (float)(Math.Abs(Math.Cos(tickDirAngleToYAxis)) * vpScale.Height + Math.Abs(Math.Sin(tickDirAngleToYAxis)) * vpScale.Width); 
                Vector3 tickDirWorldStartLines = tickDirWorldStart * Ticks.DefaultLabel.Font.Height * Ticks.TickLength * vpTickLenScale;
                Vector3 tickDirWorldEndLines = tickDirWorldEnd * Ticks.DefaultLabel.Font.Height * Ticks.TickLength * vpTickLenScale; 
                
                Vector3 ticka = (tickDirWorldEndLines - tickDirWorldStartLines) / (max - min); 
                bool showScaleLabel = false;
                float scale = (float)Math.Truncate(Math.Max(Math.Log10(Math.Abs(min)),Math.Log10(Math.Abs(max))));
                bool needsScaleLabel = scale < 0 ? Math.Abs(scale) >= Ticks.MaxNumberDigitsShowFull - 1 : Math.Abs(scale) >= Ticks.MaxNumberDigitsShowFull; 
                int id = 0;   
                foreach (ILTick tick in Ticks) {
                    float curVal = tick.Position - min; 
                    Vector3 curPos = startWorld + a * curVal;
                    tickPosArr[i++] = curPos.X; tickPosArr[i++] = curPos.Y; tickPosArr[i++] = curPos.Z;
                    if (Ticks.TickLength < 0)
                        tick.Label.Position = curPos;
                    curPos += (tickDirWorldStartLines + ticka * curVal);
                    tickPosArr[i++] = curPos.X; tickPosArr[i++] = curPos.Y; tickPosArr[i++] = curPos.Z;
                    if (Ticks.TickLength >= 0)
                        tick.Label.Position = curPos;

                    if (tick.AutoLabel) {
                        float val = tick.Position;
                        if (needsScaleLabel) {
                            // handle scale label
                            showScaleLabel = true;
                            val = (float)(val / Math.Pow(10, scale));
                        }
                        tick.Label.Text = Ticks.LabelTransformFunc(id, val);
                        tick.Label.Anchor = anchor;
                    }
                    id++;
                }
                #endregion
                if (DataProvider != null && DataProvider.ScaleMode(AxisName) == AxisScale.Logarithmic 
                    && Ticks.Mode != TickMode.Manual) {
                    ScaleLabel.Visible = true;
                    ScaleLabel.Text = "Lg10";  

                } else if (Ticks.Mode != TickMode.Manual) {
                    if (showScaleLabel) {
                        ScaleLabel.Visible = true;
                        ScaleLabel.Text = "\u00D710^{" + scale.ToString() + "}";  // unicode 'multiply' symbol
                    } else {
                        
                        ScaleLabel.Visible = false; 
                    }

                }
                Ticks.Lines.Positions.Update(tickPos);
                Ticks.Lines.Configure();

                if (GridMajor.Visible) {
                    // configure tick level 0
                    ConfigureGrid(0, Ticks, a, min, viewParams, GridMajor);

                }
                if (GridMinor.Visible) {
                    ConfigureGrid(1, Ticks, a, min, viewParams, GridMinor);

                }

                #region configure axis main label

                PointF screenStart = Position.HasValue ? parameter.ToScreen(startWorld).ToPointF() : viewParams.Screen[(int)AxisName].Item1;
                PointF screenEnd = Position.HasValue ? parameter.ToScreen(endWorld).ToPointF() : viewParams.Screen[(int)AxisName].Item2;
                float ang = (float)Math.Atan2(screenEnd.Y - screenStart.Y, screenEnd.X - screenStart.X);

                float x = DefaultLabelPosition.X, y = DefaultLabelPosition.Y;
                if (LabelPosition.HasValue) {
                    x = LabelPosition.Value.X; y = LabelPosition.Value.Y;
                }
                Vector3 tickDirWorldStartLabel = tickDirWorldStart * (Ticks.DefaultLabel.Font.Height * Ticks.TickLength + rotatedTickSize);
                Vector3 tickDirWorldEndLabel = tickDirWorldEnd * (Ticks.DefaultLabel.Font.Height * Ticks.TickLength + rotatedTickSize);
                ticka = tickDirWorldEndLabel - tickDirWorldStartLabel;
                Label.Position = startWorld + Direction * x + (tickDirWorldStartLabel + ticka * x) * y;

                if (LabelRotation.HasValue) {
                    Label.Rotation = LabelRotation.Value;
                } else {
                    if (ang > Math.PI / 2 || ang < -Math.PI / 2) {
                        ang = (float)Math.IEEERemainder(ang + Math.PI, Math.PI * 2);
                        //flipAnchor = true;
                    }
                    Label.Rotation = ang;
                }
                if (LabelAnchor.HasValue) {
                    Label.Anchor = LabelAnchor.Value;
                } else {
                    Label.Anchor = new PointF(LabelPosition.HasValue ? LabelPosition.Value.X : DefaultLabelPosition.X,
                                               1 - (0.5f + (LabelPosition.HasValue ? LabelPosition.Value.Y : DefaultLabelPosition.Y) / 2f));
                    //Label.Anchor = new PointF(LabelPosition.HasValue ? LabelPosition.Value.X : DefaultLabelPosition.X, 0);
                    if (tickDirScreen.Y <= 0) {
                        Label.Anchor = new PointF(1 - Label.Anchor.X, 1 - Label.Anchor.Y);
                    }
                }
                #endregion
                #region scale label
                if (ScaleLabel.Visible) {
                    ILPlotCube pc = FirstUp<ILPlotCube>(); 
                    if (pc != null && pc.TwoDMode) {
                        // 2D View 
                        if (ScaleLabelPosition.HasValue) {
                            x = ScaleLabelPosition.Value.X; y = ScaleLabelPosition.Value.Y;
                            ScaleLabel.Position = startWorld + Direction * x + (tickDirWorldStartLabel + ticka * x) * y;
                        } else if (AxisName == AxisNames.YAxis) {
                            ScaleLabel.Position = endWorld; ScaleLabel.Anchor = new PointF(0, 1);
                        } else if (AxisName == AxisNames.XAxis) {
                            ScaleLabel.Position = endWorld + (tickDirWorldStartLabel + ticka); ScaleLabel.Anchor = new PointF(1, -0.1f);
                        }
                        if (ScaleLabelAnchor.HasValue) {
                            ScaleLabel.Anchor = ScaleLabelAnchor.GetValueOrDefault();
                        }
                    } else {
                        // 3D View
                        x = DefaultScaleLabelPosition.X; y = DefaultScaleLabelPosition.Y;
                        if (ScaleLabelPosition.HasValue) {
                            x = ScaleLabelPosition.Value.X; y = ScaleLabelPosition.Value.Y;
                        }
                        ScaleLabel.Position = startWorld + Direction * x + (tickDirWorldStartLabel + ticka * x) * y;
                        if (ScaleLabelAnchor.HasValue) {
                            ScaleLabel.Anchor = ScaleLabelAnchor.GetValueOrDefault(); 
                        }
                    }
                }
                #endregion
            }
        }

        private void ConfigureGrid(int level, ILTickCollection Ticks, Vector3 a, float min, ILViewAxesParameters viewParams, ILLines grid) {
            if (grid.Visible) {
                using (ILScope.Enter()) {
                    int axisId = (int)AxisName;
                    List<Tuple<Vector3, Vector3>> sides = viewParams.Grids[axisId];
                    var tickLevel = Ticks.Where<ILTick>((t) => t.Level == level);
                    ILArray<float> vertices = ILMath.zeros<float>(3, sides.Count * 2 * tickLevel.Count());
                    float[] vertPosArr = vertices.GetArrayForWrite();
                    int i = 0;
                    foreach (ILTick tick in tickLevel) {
                        float curVal = tick.Position - min;
                        foreach (var side in sides) {
                            Vector3 curPos = a * curVal + side.Item1;
                            vertPosArr[i++] = curPos.X; vertPosArr[i++] = curPos.Y; vertPosArr[i++] = curPos.Z;
                            curPos += side.Item2;
                            vertPosArr[i++] = curPos.X; vertPosArr[i++] = curPos.Y; vertPosArr[i++] = curPos.Z;
                        }
                    }
                    grid.Positions.Update(vertices);
                    grid.Configure();
                }
            }
        }

        private void getTicksScreenParameter(Vector3 startWorld, Vector3 endWorld, ILRenderParameter parameter, ref Vector3 TickDirScreen,
                                            ref PointF anchor, ref Vector3 tickDirWorldStart, ref Vector3 tickDirWorldEnd, ref float rotatedTickSize,
                                            ref float tickDirAngleToYAxis) {
            // get screen direction, rough
            Vector3 s = startWorld, endScreen = endWorld;
            parameter.ToScreen(ref s, ref endScreen);
            Vector3 centerAxisScreen = (s + endScreen) / 2f;
            Vector3 centerCube = parameter.ToScreen(new Vector3(.49f, .5f, .5f));

            // make ticks always perpendicular to axis _on screen_
            Vector3 r = endScreen - s;
            TickDirScreen = r.Y != 0 ? new Vector3(r.Y, -r.X, 0) : new Vector3(-r.Y, r.X, 0);
            TickDirScreen = Vector3.Normalize(TickDirScreen);

            // make it always point in 'outside' direction
            if (Vector3.Dot(centerCube - centerAxisScreen, TickDirScreen) > ILMath.epsf) {
                TickDirScreen = TickDirScreen * -1;
            }

            anchor.X = TickDirScreen.X * -0.7f + 0.5f;
            anchor.Y = TickDirScreen.Y * -0.5f + 0.5f;

            // where do ticks end? -> 1 for label position
            // default tick placeholder size
            tickDirAngleToYAxis = (float)Math.Atan2(r.X,r.Y);
            float y = (float)Math.Sin(tickDirAngleToYAxis) * Ticks.DefaultTickLabelSize.Height;
            float x = (float)Math.Cos(tickDirAngleToYAxis) * Ticks.DefaultTickLabelSize.Width;
            rotatedTickSize = (float)Math.Sqrt(x * x + y * y);

            //TickDirScreen = TickDirScreen * tickLen;
            // get the world coords (ticks are drawn in model coords!)
            tickDirWorldStart = parameter.ToModel(s + TickDirScreen).Xyz - startWorld;
            tickDirWorldEnd = parameter.ToModel(endScreen + TickDirScreen).Xyz - endWorld;

        }

        //private void getTicksScreenParameter(Vector3 startWorld, Vector3 endWorld, ILRenderParameter parameter, ref Vector3 TickDirScreen, ref PointF anchor, ref Vector4 tickDirWorld, float tickLen) {
        //    // get screen direction, rough
        //    Vector3 s = startWorld, endScreen = endWorld;
        //    parameter.ToScreen(ref s, ref endScreen);
        //    Vector3 T = startWorld, center = new Vector3(.5f, .5f, .5f);
        //    Vector3 tickOffset = getTicksDirection(startWorld, 0.005f);
        //    T = T + tickOffset;
        //    parameter.ToScreen(ref T, ref center);

        //    // T is some distant point on the correct side of the axis now
        //    Vector3 r = endScreen - s;
        //    // make it always perpendicular to axis _on screen_
        //    TickDirScreen = r.Y != 0 ? new Vector3(r.Y, -r.X, 0) : new Vector3(-r.Y, r.X, 0);
        //    TickDirScreen = Vector3.Normalize(TickDirScreen);
        //    // find the sign/ which of the two options is the right one? 
        //    float b = (r.Y != 0) ?
        //        (s.X - T.X + r.X * ((T.Y - s.Y) / r.Y)) / (r.X * TickDirScreen.Y / r.Y + TickDirScreen.X) :
        //        (T.Y - s.Y - (T.X - s.X) / r.X) / (r.Y * TickDirScreen.X / r.X + TickDirScreen.Y);
        //    if (b < 0) {
        //        TickDirScreen = TickDirScreen * -1;
        //    }
        //    anchor.X = TickDirScreen.X / -2f + 0.5f;
        //    anchor.Y = TickDirScreen.Y / -2f + 0.5f;

        //    TickDirScreen = TickDirScreen * tickLen;
        //    // get the world coords (ticks are drawn in model coords!)
        //    tickDirWorld = new Vector4(parameter.ToModel(s + TickDirScreen).Xyz - startWorld, 1);
        //}

        private int getOptimalTickNumber(ILViewAxesParameters viewParams) {
            PointF startScreen = viewParams.Screen[(int)AxisName].Item1;
            PointF endScreen = viewParams.Screen[(int)AxisName].Item2;
            float difX = endScreen.X - startScreen.X, difY = endScreen.Y - startScreen.Y;
            if (difX * difX + difY * difY < 2) return 0;

            float scaleX = (float)Math.Atan2(difY, difX);
            float scaleY = (float)Math.Abs(Math.Sin(scaleX));
            scaleX = (float)Math.Abs(Math.Cos(scaleX));

            float xSize = Ticks.DefaultTickLabelSize.Width * scaleX + TickLabelPadding * 2;
            float ySize = Ticks.DefaultTickLabelSize.Height * scaleY + TickLabelPadding * 2;
            int x = (int)(Math.Floor((float)Math.Abs(endScreen.X - startScreen.X) / xSize));
            int y = (int)(Math.Floor((float)Math.Abs(endScreen.Y - startScreen.Y) / ySize));
            return (int)Math.Floor(Math.Max((double)x, y) + 1);
        }

        private void getLabelAnchor(ILRenderParameter parameter, Vector3 start, Vector3 tickDir, ref PointF anchor, ref Vector3 dirScreen) {
            Vector3 end = start + tickDir;
            parameter.ToScreen(ref start, ref end);
            end = end - start;
            dirScreen = new Vector3(end.X, end.Y, 0);
            float x = -end.X, y = -end.Y;
            float len = (float)Math.Sqrt(x * x + y * y);

            anchor.X = x / len / 2f + 0.5f;
            anchor.Y = y / len / 2f + 0.5f;

        }
        private Vector3 getTicksDirection(Vector3 start, float length) {
            switch (AxisName) {
                case AxisNames.XAxis:
                    return new Vector3(
                        0,
                        (start.Y * 2f - 1f) * length,
                        (start.Z * 2f - 1f) * length);
                case AxisNames.YAxis:
                    return new Vector3(
                        (start.X * 2f - 1f) * length,
                        0,
                        (start.Z * 2f - 1f) * length);
                default:
                    return new Vector3(
                        (start.X * 2f - 1f) * length,
                        (start.Y * 2f - 1f) * length,
                        0);
            }
        }

        internal Size CalculateDefaultSize(ILRenderParameter parameter) {
            return new Size((int)(Ticks.DefaultTickLabelSize.Width + Label.Font.Height + Ticks.TickLength * Ticks.DefaultLabel.Font.Height + 5), 
                             (int)(Ticks.DefaultTickLabelSize.Height + Label.Font.Height)); 
        }
    }
}
