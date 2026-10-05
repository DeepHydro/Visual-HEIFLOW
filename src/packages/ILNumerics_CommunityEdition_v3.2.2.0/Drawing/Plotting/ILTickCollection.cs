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
using System.Globalization; 
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Security;

namespace ILNumerics.Drawing.Plotting {
    /// <summary>
    /// The class represents the collection of ticks for axis objects and is used in ILPlotCube
    /// </summary>
    [Serializable]
    public class ILTickCollection : ILGroup, IEnumerable<ILTick> {

        #region attributes
        /// <summary>
        /// Font creation function used for initial fonts for tick labels; default: 
        /// gives the default font for all labels (Helvetica, 10pt) 
        /// </summary>
        public static Func<Font> DefaultFontFunc = () => ILLabel.DefaultFont; 
        /// <summary>
        /// Tag identifying the tick lines within the scene graph 
        /// </summary>
        public readonly static string TickLinesTag = "TickLines";
        /// <summary>
        /// Tag identifying the tick label within the scene graph
        /// </summary>
        public readonly static string TickLabelTag = "TickLabel"; 

        private ILLabel m_defaultLabel; 
        private SizeF? m_defaultTickLabelSize;
        private int m_maxNumberDigits = 4; 
        private TickMode m_mode;
        private float m_tickLength;
        private List<ILTick> m_ticks; 
        #endregion

        #region creation functions 
        /// <summary>
        /// Default tick creation function for all axis objects with <see cref="TickMode"/> set to Automatic. 
        /// </summary>
        public static readonly Func<float, float, int, List<float>> DefaultTickCreationFunc = CreateTicksAuto;
        /// <summary>
        /// Default function used to transform tick positions into tick label texts. It applies a simple number transform, 
        /// taking the current culture into account.
        /// </summary>
        /// <remarks><para>The Func defines the following input parameters:</para>
        /// <list type="bullets">
        /// <item>Index (int) - the index of the tick within the collection of all visible ticks; 0-based indexing</item>
        /// <item>The position value (float) - number used to position the tick along the axis scale</item>
        /// </list>
        /// <para>The function returns a string which is directly used to display the tick. It is per default used by <code>LabelTransformFunc</code>.</para>
        /// <para>In order to control the creation of tick labels and/or to implement custom tick filters, one may assign a
        /// custom (anonymous) function to <code>LabelTransformFunc</code>.</para>
        /// </remarks>
        public static readonly Func<int, float, string> DefaultLabelTransformFunc =
              (index, value) => { return value.ToString(CultureInfo.CurrentCulture.NumberFormat); };
        /// <summary>
        /// Function used to control the creation of ticks for this axis. Default: <see cref="DefaultTickCreationFunc"/>
        /// </summary>
        public Func<float, float, int, List<float>> TickCreationFunc = DefaultTickCreationFunc;
        /// <summary>
        /// Function used to control the creation of tick labels from position values. Default: <see cref="DefaultLabelTransformFunc"/>
        /// </summary>
        /// <remarks><para>The Func defines the following input parameters:</para>
        /// <list type="bullets">
        /// <item>Index (int) - the index of the tick within the collection of all visible ticks; 0-based indexing</item>
        /// <item>The position value (float) - number used to position the tick along the axis scale</item>
        /// </list>
        /// <para>The function returns a string which is directly used to display the tick.</para>
        /// <para>In order to control the creation of tick labels and/or to implement custom tick filters, one may assign 
        /// custom (anonymous) functions to <code>LabelTransformFunc</code>.</para>
        /// </remarks>
        public Func<int, float, string> LabelTransformFunc = DefaultLabelTransformFunc;
        #endregion

        #region properties

        /// <summary>
        /// The label template used to create new tick labels; provides all settings (color, font, ...)
        /// </summary>
        /// <remarks>Changes to the default label instance will immediately affect all existing labels in the tick collection!</remarks>
        public ILLabel DefaultLabel {
            get {
                if (m_defaultLabel == null) {
                    //m_defaultLabel = new ILLabel() {
                    //    Font = DefaultFontFunc()
                    //};
                }
                return m_defaultLabel;
            }
        }

        /// <summary>
        /// Threshold on number of digits allowed for a tick label before it gets abbreviated and the scale label is shown. Default: 5
        /// </summary>
        [XmlAttribute]
        public int MaxNumberDigitsShowFull {
            get {
                return m_maxNumberDigits;
            }
            set {
                if (value >= 0 && value != m_maxNumberDigits) {
                    m_defaultTickLabelSize = null;
                    m_maxNumberDigits = value;
                    OnPropertyChanged("MaxNumberDigitsShowFull"); 
                }
            }
        }

        /// <summary>
        /// Gets the default size of tick labels in pixels or sets it. Default: actual size of <see cref="TickSizeMeasureDefaultTemplate"/> with <see cref="DefaultLabel.Font"/>.
        /// </summary>
        /// <remarks><para>The property is used for axis configuration and layout. It serves as a placeholder for the size of a tick label and helps 
        /// positioning other components of the axis relative to that size. One example is the axis main label which per default is placed <b>outside</b>
        /// of the ticks.</para>
        /// <para>With custom ticks it might be necessary to modify this value in order to better reflect the actual tick label size.</para></remarks>
        /// <seealso cref="MaxNumberDigitsShowFull"/>
        [ILXmlSerializeAs("{Width},{Height}")]
        public SizeF DefaultTickLabelSize {
            get {
                if (!m_defaultTickLabelSize.HasValue) {
                    string template = String.Format("{0:F" + (MaxNumberDigitsShowFull) + "}", (-1 / Math.Pow(10f, (MaxNumberDigitsShowFull))));
                    m_defaultTickLabelSize = ILAxisCollection.Graphics.MeasureString(template, DefaultLabel.Font);
                }
                return m_defaultTickLabelSize.Value;
            }
            set {
                if (m_defaultTickLabelSize != value) {
                    m_defaultTickLabelSize = value;
                    OnPropertyChanged("DefaultTickLabelSize"); 
                }
            }
        }

        /// <summary>
        /// Gets the current number of ticks in the collection (readonly)
        /// </summary>
        [XmlAttribute]
        public int Count { get; private set; }
        /// <summary>
        /// Access to the <see cref="ILLines"/> shape used for drawing the tick lines
        /// </summary>
        [XmlIgnore]
        public ILLines Lines { 
            get {
                var ret = First<ILLines>(TickLinesTag); 
                return ret; 
            }
        }
        [XmlArray]
        internal List<ILTick> Ticks {
            [SecuritySafeCritical]
            get { return m_ticks; }
            private set { m_ticks = value; } 
        }
        /// <summary>
        /// Gets the tick creation mode or sets it
        /// </summary>
        /// <remarks><para>For TickMode.Auto the ticks for the axis are created automatically (default). TickMode.Manual allows for custom configuration of the ticks.</para></remarks>
        [XmlAttribute]
        public TickMode Mode { 
            get { return m_mode; } 
            set { 
                if (m_mode != value) {
                    m_mode = value; 
                    OnPropertyChanged("Mode");
                }
            }
        }

        /// <summary>
        /// Gets the color for the lines of the ticks and the main axis line or sets it.
        /// </summary>
        /// <remarks><para>This property is a shortcut to the Color property of the Lines member which allows further configuration of the lines.</para></remarks>
        [ILXmlSerializeAs("{R},{G},{B},{A}")]
        public Color Color { 
            get { return Lines.Color ?? Color.Empty; } 
            set { Lines.Color = value; } }
        /// <summary>
        /// Gets the thickness of the tick lines or sets it
        /// </summary>
        /// <remarks><para>The tickness of the lines is measured in pixels on screen.</para>
        /// <para>This property is a shortcut to the Width property of the Lines member which allows further configuration of the tick lines.</para>
        /// </remarks>
        [XmlAttribute]
        public int Width {
            get { return Lines.Width; }
            set { Lines.Width = value; }
        }
        /// <summary>
        /// Gets/sets the length of the ticks; negative values: flip ticks inside plot cube; unit: fraction of DefaultFont height; default: 0.5f
        /// </summary>
        /// <remarks><para>The length is measured in fractions of the current DefaultFont.Height, i.e. a value of 1 will produce a tick length of 
        /// exactly the height of the DefaultFont.</para>
        /// <para>Negative values for TickLength will draw the ticks in opposite direction. For 2D plots and colorbars this produces ticks reaching 'inside' the content area.</para></remarks>
        [XmlAttribute]
        public float TickLength {
            get {
                return m_tickLength;
            }
            set {
                if (m_tickLength != value) {
                    m_tickLength = value;
                    OnPropertyChanged("TickLength");
                }
            }
        }

        #endregion

        #region constructors
        public ILTickCollection(object tag = null) : base(tag) {
            Add(new ILLines(TickLinesTag) { PickingID = this.ID }); 
            Ticks = new List<ILTick>(); 
            Color = Color.Black; 
            Width = 1; 
            TickLength = 0.5f; 
            Mode = TickMode.Auto;
            m_defaultLabel = new ILLabel("DefaultLabel") {
                Font = DefaultFontFunc()
            }; 
            m_defaultLabel.PropertyChanged += defaultLabelChanged; 
        }
        private ILTickCollection() {
            Ticks = new List<ILTick>(); 
        }
        internal ILTickCollection(ILTickCollection source) : base(source) {
            Ticks = new List<ILTick>();
            // regular labels have been copied in ILGroup(source)! 
            int ind = 0;
            var labels = Find<ILLabel>();

            foreach (var a in source.Ticks) {
                var tick = new ILTick() {
                    AutoLabel = a.AutoLabel,
                    Label = labels.ElementAt(ind++),
                    Level = a.Level,
                    Position = a.Position
                }; 
                Ticks.Add(tick);
                Count++;
                //OnPropertyChanged("Ticks"); 
            }
            Color = source.Color;
            Width = source.Width; 
            TickLength = source.TickLength;
            Mode = source.Mode; 
            m_defaultLabel = (ILLabel)source.DefaultLabel.Copy(); 
            TickCreationFunc = source.TickCreationFunc; 
            LabelTransformFunc = source.LabelTransformFunc; 
        }
        #endregion

        #region public interface 
        /// <summary>
        /// Retrieve a single tick by index; to be used in Manual Tick Mode only!
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ILTick this[int index] {
            get { return Ticks[index]; }
        }
        //public new IList<ILTick> Childs {
        //    // TODO: change this to be consistent with all scene guidelines (ticks are nodes!, Ticks not needed!)
        //    get { return Ticks; }
        //}
        /// <summary>
        /// Add a new tick to the tick collection
        /// </summary>
        /// <param name="tick">new tick object</param>
        /// <returns>the newly added tick object</returns>
        /// <remarks><para>The tick object returned is not guaranteed to be identical to the one given to <paramref name="tick"/></para></remarks>
        public ILTick Add(ILTick tick) {
            if (tick != null && tick.Label != null) {
                Ticks.Add(tick);
                Add(tick.Label);
                Mode = TickMode.Manual;
                Count++;
                OnPropertyChanged("Ticks");
            }
            return tick;
        }
        internal void AddInternal(ILTick tick) {
            if (tick != null && tick.Label != null) {
                Ticks.Add(tick);
                Add(tick.Label);
                Count++;
            }
        }
        /// <summary>
        /// Add a new tick to the tick collection by providing position and a label instance
        /// </summary>
        /// <param name="position">tick position</param>
        /// <param name="label">label object to be used for rendering the tick label</param>
        /// <returns>newly created tick object</returns>
        public ILTick Add(float position, ILLabel label) {
            if (label == null)
                label = (ILLabel)DefaultLabel.Copy(); 

            ILTick ret = new ILTick(position, label);
            if (String.IsNullOrWhiteSpace(label.Text)) {
                ret.AutoLabel = true;
            } else {
                ret.AutoLabel = false;
            }
            return Add(ret);
        }
        /// <summary>
        /// Add a new tick to the tick collection by providing position and a label text
        /// </summary>
        /// <param name="position">tick position</param>
        /// <param name="label">text to be used as tick label</param>
        /// <returns>newly created tick object</returns>
        public ILTick Add(float position, string tictext) {
            ILTick ret = new ILTick(position, (ILLabel)DefaultLabel.Copy()); 
            ret.Label.Tag = TickLabelTag;
            ret.Label.Text = tictext; 
            ret.AutoLabel = false; 
            return Add(ret);
        }
        /// <summary>
        /// Remove all ticks from the tick collection
        /// </summary>
        public void Clear() {
            Count = 0; 
            var all = Find<ILLabel>();
            foreach (var a in all) {
                base.Remove(a); 
            }
            Ticks.Clear(); 
            Lines.Positions.Update(ILMath.empty<float>(3,0)); 
            OnPropertyChanged("Ticks"); 
        }
        /// <summary>
        /// Replace all existing ticks with a collection of new ticks; does not change current tick Mode
        /// </summary>
        /// <param name="ticks">collection of position values</param>
        /// <remarks><para>Every item in <paramref name="ticks"/> creates a new tick object with the value of the item as position for the new tick.</para>
        /// <para>Tick labels are automatically created by transforming the position value into a string. Therefore, the <see cref="LabelTransformFunc"/>
        /// function is used (which by default references <see cref="DefaultLabelTransformFunc"/>). </para>
        /// <para>In order to control the label text creation process, set the <see cref="LabelTransformFunc"/> to a custom function.</para></remarks>
        public void Replace(IEnumerable<float> ticks) {
            TickMode oldMode = m_mode;
            try {
                while (Ticks.Count < ticks.Count()) {
                    ILTick tick = new ILTick();
                    tick.Label = (ILLabel)DefaultLabel.Copy();
                    tick.Label.Tag = TickLabelTag;
                    AddInternal(tick);
                }
            } finally {
                m_mode = oldMode; 
            }

            int i = 0; 
            foreach (var tick in ticks) {
                Ticks[i].Position = tick;
                // auto ticks are labeld in AxisConfigure
                //Ticks[i].Label.Text = LabelTransformFunc(i, tick);
                Ticks[i].Label.Visible = true;
                i++;
            }
            Count = i++;
            for (; i < m_children.Count; i++) {
                ILLabel l = m_children[i] as ILLabel;
                if (l != null) {
                    l.Visible = false;
                }
            }
            OnPropertyChanged("Ticks"); 
        }

        internal override ILNode Copy() {
            return new ILTickCollection(this); 
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILTickCollection();
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILTickCollection ret = (ILTickCollection)base.Synchronize(copy, syncParams);
            if (copy == null) {
                if (ret.Lines == null) {
                    ret.Add(new ILLines(TickLinesTag) { PickingID = ID });
                }
            }
            if (ret.SynchedVersion != Version) {
                ret.Lines.Markable = Markable;
                ret.Lines.Marked = Marked;
                ret.Mode = Mode; 
                if (Mode == TickMode.Manual) {
                    // synchronize manual ticks 
                    while (ret.Ticks.Count < Count) {
                        ret.Ticks.Add(new ILTick());
                    }
                    for (int i = 0; i < Count; i++) {
                        ILTick myTick = Ticks[i];
                        ILTick copyTick = ret.Ticks[i];
                        copyTick.Label = ret.FindById<ILLabel>(myTick.Label.ID);
                        copyTick.Position = myTick.Position;
                        copyTick.AutoLabel = myTick.AutoLabel;
                        copyTick.Level = myTick.Level;
                    }
                    //ret.Ticks = Ticks;
                    ret.Count = Count;
                }
                ret.Color = Color;
                ret.Width = Width;
                ret.TickLength = TickLength;
                ret.m_defaultTickLabelSize = m_defaultTickLabelSize; 
                ret.TickCreationFunc = TickCreationFunc;
                ret.LabelTransformFunc = LabelTransformFunc;
                
                // submit changes to default label; may fires ProperyChanged event:
                ret.m_defaultLabel = (ILLabel)m_defaultLabel.Synchronize(ret.m_defaultLabel, syncParams);
                if (copy == null) {
                    // first time setup only
                     ret.m_defaultLabel.PropertyChanged += ret.defaultLabelChanged; 
                }
            }
            return ret; 
        }
        protected override void SynchronizeChildren(ILSyncParams syncParams, ILGroup copyGroup) {
            if (Mode == TickMode.Manual) {
                base.SynchronizeChildren(syncParams, copyGroup);
            }
        }
        private void defaultLabelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            Version++;
            if (IsSynchedNode) {
                // populate changed to all ticks
                foreach (var tick in Ticks) {
                    switch (e.PropertyName) {
                        case "Color":
                            tick.Label.Color = DefaultLabel.Color;
                            break;
                        case "Anchor":
                            tick.Label.Anchor = DefaultLabel.Anchor;
                            break;
                        case "Font":
                            tick.Label.Font = (Font)DefaultLabel.Font.Clone();
                            break;
                        case "Markable":
                            tick.Label.Markable = DefaultLabel.Markable;
                            break;
                        case "Marked":
                            tick.Label.Marked = DefaultLabel.Marked;
                            break;
                        case "Text":
                            tick.Label.Text = DefaultLabel.Text;
                            break;
                        case "Visible":
                            tick.Label.Visible = DefaultLabel.Visible;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        #region nice numbers
        /// <summary>
        /// create "nice" number in fractions of 2 or 5
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>This code was adopted from Paul Heckbert
        /// from "Graphics Gems", Academic Press, 1990. </returns>
        static double niceNumber(double val, bool round) {
            int expv;
            double f;                                /* fractional part of x */
            double nf;                                /* nice, rounded fraction */

            expv = (int)Math.Floor(Math.Log10(val));
            f = val / Math.Pow(10, expv);                /* between 1 and 10 */
            if (round) {
                if (f < 1.5) nf = 1;
                else if (f < 3) nf = 2;
                else if (f < 7) nf = 5;
                else nf = 10;
            } else {
                if (f <= 1) nf = 1;
                else if (f <= 2) nf = 2;
                else if (f <= 5) nf = 5;
                else nf = 10;
            }
            return (nf * Math.Pow(10, expv));
        }
        /// <summary>
        /// Default ticks creation function, produce nice numbers for linear axes, does not alter this tick collection
        /// </summary>
        /// <param name="min">minimal axis range</param>
        /// <param name="max">maximum axis range</param>
        /// <param name="numberTicks">number of ticks (hint only)</param>
        /// <returns>list of nice ticks positions</returns>
        public static List<float> CreateTicksAuto(float min, float max, int numberTicks) {
            double d;                                /* tick mark spacing */
            double graphmin;                /* graph range min and max */
            double range, x;

            /* we expect min!=max */
            range = niceNumber(max - min, false);
            d = niceNumber(range / (Math.Min(numberTicks, 10) - 1), true);
            double exp = Math.Pow(10, Math.Floor(Math.Log10(max - min)));
            graphmin = Math.Floor(min / exp) * exp;

            // Math.Min(niceNumber(min - d,true),niceNumber(min - d,false));
            //graphmax = Math.Min(niceNumber(max - d, true), niceNumber(max + d, false));
            List<float> ticks = new List<float>();
            //ticks.Add(nfrac);
            for (x = graphmin; x <= max; x = (float)(Math.Round((x + d) / d) * d)) {
                //x = (float)(Math.Round(x/d)*d); 
                if (x >= min) {
                    ticks.Add((float)x);
                    if (ticks.Count > 20) break; //emergency exit if range is in floating point range
                }
            }
            return ticks;
        }

        #endregion

        #region IEnumerable<ILTick> Members
        /// <summary>
        /// Give an enumerator for this tick collection, allows the use inside foreach constructs
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ILTick> GetEnumerator() {
            for (int i = 0; i < Count; i++) {
                yield return Ticks[i];
            }
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Give an enumerator for this tick collection, allows the use inside foreach constructs
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            for (int i = 0; i < Count; i++) {
                yield return Ticks[i]; 
            }
        }

        #endregion
    }
}
