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
using System.Security;
using System.Text;
using System.Xml.Serialization;

namespace ILNumerics.Drawing.Plotting {

    /// <summary>
    /// Legend object
    /// </summary>
    [Serializable]
    public class ILLegend : ILScreenObject {

        #region attributes 
        /// <summary>
        /// Tag used to identify legends in the scene graph
        /// </summary>
        public static readonly string LegendTag = "Legend";
        /// <summary>
        /// Tag used to identify legend item area group within the scene graph
        /// </summary>
        public static readonly string ItemsGroupTag = "ItemsArea"; 
        
        private float m_padding; 
        private int? m_rootID; 
        private class CreateObjects {
            public IEnumerable<string> Labels; 
            public ILGroup Root; 
        }
        private CreateObjects m_createObjects;
        private float m_legendItemSize;
        #endregion

        #region properties
        /// <summary>
        /// Get or sets the width for the area in pixels where lines &amp; markers are drawn; default: 100px
        /// </summary>
        [XmlAttribute]
        public float LegendItemSize {
            get {
                return m_legendItemSize;
            }
            set {
                if (m_legendItemSize != value) {
                    m_legendItemSize = value;
                    OnPropertyChanged("LegendItemSize");
                }
            }
        }

        /// <summary>
        /// Gets the root node of the subtree in the scene, which is considered for finding legend items automatically or sets it. Default: the plot cube containing this legend
        /// </summary>
        [XmlAttribute]
        public int? RootID {
            get {
                if (!m_rootID.HasValue) {
                    var plotcube = FirstUp<ILPlotCube>();
                    if (plotcube != null) {
                        m_rootID = plotcube.ID;
                    }
                }
                return m_rootID; 
            }
            set {
                if (m_rootID != value) {
                    m_rootID = value; 
                    OnPropertyChanged("RootID"); 
                }
            }
        }

        /// <summary>
        /// Gets the collection of legend items
        /// </summary>
        [XmlArray]
        internal IEnumerable<ILLegendItem> LegendItems {
            [SecuritySafeCritical]
            get {
                if (Items == null) 
                    return null;
                return Items.Find<ILLegendItem>();
            }
        }
        /// <summary>
        /// The collection of items for display in this legend
        /// </summary>
        [XmlIgnore]
        public ILGroup Items {
            get {
                return First<ILGroup>(ItemsGroupTag);
            }
        }
        /// <summary>
        /// Spacing between outer border of the legend and the legend items measured in pixels
        /// </summary>
        [XmlAttribute]
        public float Padding {
            get {
                return m_padding; 
            }
            set {
                if (m_padding != value) {
                    m_padding = value;
                    OnPropertyChanged("Padding"); 
                }
            }
        }

        #endregion

        #region ctors
        /// <summary>
        /// Create legend object for any suitable plot object in the plot cube; provide label text for all items
        /// </summary>
        /// <param name="labels">item label text strings</param>
        /// <remarks><para>The number of created items depends on the number of elements in labels and the number of plot objects found in the plot cube. 
        /// The iteration of suitable plot objects in the plot cube is walked. Every object gets a new legend item associated to with the label text given by the 
        /// corresponding element in <paramref name="labels"/>. If the corresponding element in labels is null, the <see cref="ILNumerics.Drawing.ILNode.Tag"/> 
        /// property of the plot object is used as label text instead. </para>
        /// <para>The iteration of suitable plot objects is acquired from the subtree of that plot cube in the scene, where this legend object is 
        /// contained within at time of rendering. If the number of suitable objects found is n, the number of legend items created is min(labels.Count, n). 
        /// If labels is string[0] (empty array) no items are created. If labels is null, items for all suitable objects are created. </para>
        /// <para>Not all plot objects are suitable objects for display in a legend. Examples of suitable objects are line plots and contour plots.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.Drawing.Plotting.ILLegend(ILNumerics.Drawing.ILGroup, System.Collections.Generic.IEnumerable{string}, System.Object)"/>
        public ILLegend(params string[] labels)
            : this(null, labels, null) { }
        /// <summary>
        /// Create legend object for automatic configuration
        /// </summary>
        /// <param name="root">[optional] root node of the subtree considered for finding legend items automatically. Default: plot cube this legends is in</param>
        /// <param name="labels">[optional] label text strings for all items to be displayed</param>
        /// <param name="tag">[optional] tag used to identify this object in the scene</param>
        /// <remarks><para>The number of created items depends on the number of elements in labels and the number of plot objects found in the plot cube. 
        /// The iteration of suitable plot objects below <paramref name="root"/> is walked. Every object gets a new legend item associated to it; the label text is taken from the 
        /// corresponding element in <paramref name="labels"/>. If the corresponding element in labels is null, the <see cref="ILNumerics.Drawing.ILNode.Tag"/> 
        /// property of the plot object is used as label text instead.</para>
        /// <para>If <paramref name="root"/> is null, the collection of suitable plot objects is acquired from the subtree of the first plot cube in the scene, which lays 
        /// on the path from the legend up to the scene root at time of rendering. If the number of suitable objects found is n, the number of legend items created is min(labels.Count, n). 
        /// If labels is string[0] (empty array) no items are created. If labels is null, items for <i>all</i> suitable objects are created. </para>
        /// <para>Not all plot objects are suitable objects for display in a legend. Examples of suitable objects are line plots and contour plots.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.Drawing.Plotting.ILLegend(string[])"/>
        public ILLegend(ILGroup root = null, IEnumerable<string> labels = null, object tag = null)
            : base(tag ?? LegendTag) {

            Add(new ILGroup(ItemsGroupTag));
            m_rootID = (root != null) ? root.ID : (int?)null;

            m_padding = 10f;
            Anchor = new System.Drawing.PointF(1.0f, 0f);

            LocationXUnit = Units.Viewport;
            LocationYUnit = Units.Viewport;
            Location = new System.Drawing.PointF(0.9f, 0.1f);
            LegendItemSize = 100; 


            WidthUnit = Units.Pixels;
            HeightUnit = Units.Pixels;
            Height = null;
            Width = null; 
            Background.Color = System.Drawing.Color.White;
            //Transform = Matrix4.Translation(0,0,0.5);
                
                // deferred creation 
            m_createObjects = new CreateObjects() {
                Labels = labels, 
                Root = root
            }; 
        }
        /// <summary>
        /// Create a new legend object, provide initial legend items 
        /// </summary>
        /// <param name="items">collection of legend items</param>
        /// <param name="tag">tag object used to identify the legend in the scene</param>
        public ILLegend(IEnumerable<ILLegendItem> items, object tag = null)
            : base(tag ?? LegendTag) {

            Add(new ILGroup(ItemsGroupTag));
            foreach (var item in items) {
                Items.Add(item); 
            }

            m_padding = 10f;
            Anchor = new System.Drawing.PointF(1.0f, 0f);
            ZCoord = 0.0f; 
            LegendItemSize = 100; 

            LocationXUnit = Units.Viewport;
            LocationYUnit = Units.Viewport;
            Location = new System.Drawing.PointF(0.9f, 0.1f);

            WidthUnit = Units.Pixels;
            HeightUnit = Units.Pixels;
            Height = null;
            Width = null;
            Background.Color = System.Drawing.Color.White;
        }
        internal ILLegend(ILLegend source)
            : base(source) {
            m_padding = source.m_padding;
            m_legendItemSize = source.m_legendItemSize;
            m_createObjects = source.m_createObjects; 
        }
        protected ILLegend() { }
        #endregion

        #region public interface
        internal override ILNode Copy() {
            var ret = new ILLegend(this); 
            return ret; 
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILLegend(); 
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILLegend ret = (ILLegend)base.Synchronize(copy, syncParams);
            if (copy == null || ret.SynchedVersion != Version) {
                ret.m_padding = m_padding; 
                ret.m_legendItemSize = m_legendItemSize; 
            }
            return ret; 
        }
        /// <summary>
        /// Configures the legend initially and after changes to child objects
        /// </summary>
        /// <param name="configureChilds">[optional] triggers childs configuration; default: true</param>
        /// <param name="configurePath2Root">[optional] triggers parents configuration; default: true</param>
        public override void Configure(bool configureChilds = true, bool configurePath2Root = true) {
            base.Configure(configureChilds, configurePath2Root);
            if (m_createObjects != null) {
                Configure(); 
            }
        }
        /// <summary>
        /// update all existing legend items
        /// </summary>
        public void Configure() {
            IList<KeyValuePair<int, string>> entries = null;
            // at first time: create all items according to constructor parameters
            if (m_createObjects != null) {
                if (m_createObjects.Root == null) {
                    m_createObjects.Root = FirstUp<ILPlotCube>(); 
                }
                if (m_createObjects.Root != null) {
                    // find ILLegendItemDataProviders below root node
                    entries = new List<KeyValuePair<int, string>>();
                    if (m_createObjects.Labels == null) {
                        // take all objects
                        m_createObjects.Root.Find<ILGroup>(predicate: g => {
                            if (g is IILLegendItemDataProvider) {
                                entries.Add(new KeyValuePair<int, string>(g.ID, null)); // <- null: let the item configure the label in UpdateContent()!
                            }
                            return false;
                        });
                    } else {
                        // take only up to (labels items).Count items
                        var labelEnum = m_createObjects.Labels.GetEnumerator();
                        m_createObjects.Root.Find<ILGroup>(predicate: g => {
                            if (g is IILLegendItemDataProvider && labelEnum.MoveNext()) {
                                entries.Add(new KeyValuePair<int, string>(g.ID, labelEnum.Current));
                            }
                            return false;
                        });
                    }
                }
                m_createObjects = null; 
            }
            // add new items
            if (entries != null) {
                foreach (var entry in entries.Reverse()) {
                    Items.Insert(0, new ILLegendItem(entry.Key, entry.Value));
                }
                float maxLabelWidthPx = 0, accumHeightPx = 0;
                // update all existing items
                foreach (var item in LegendItems) {
                    //item.Transform = Matrix4.ScaleTransform(1, height, 1).Translate(0, top * height, 0);
                    item.UpdateContent();
                    System.Drawing.SizeF size = item.Label.MeasureSize();
                    if (size.Width > maxLabelWidthPx)
                        maxLabelWidthPx = size.Width;
                    accumHeightPx += size.Height;
                    //top += 1;
                }
                // set split positions - align all items
                base.Width = maxLabelWidthPx + LegendItemSize + m_padding * 2;
                base.Height = accumHeightPx + m_padding * 2;
                int i = 0;
                float splitPos = (LegendItemSize + Padding) / ((LegendItemSize + Padding) + (maxLabelWidthPx + Padding));
                float height = 1f / LegendItems.Count();
                foreach (var item in LegendItems) {
                    item.Transform = Matrix4.ScaleTransform(1, height, 1).Translate(0, i++ * height, 0);
                    item.SplitPosition = splitPos;
                }
                float padWidthVP = Padding / Width.GetValueOrDefault();
                float padHeightVP = Padding / Height.GetValueOrDefault();
                Items.Transform = Matrix4.ScaleTransform(1 - 2 * padWidthVP, 1 - 2 * padHeightVP, 1)
                                * Matrix4.Translation(padWidthVP, padHeightVP, 0); 

            }
        }
        #endregion 

        #region private helpers
        #endregion
    }
}
