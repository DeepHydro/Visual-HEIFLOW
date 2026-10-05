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
using System.Xml.Serialization;

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public class ILLegendItem : ILGroup, IILPositionProvider {

        #region attributes
        /// <summary>
        /// Tag used to identiy the entry area within the scene graph
        /// </summary>
        public static readonly string EntryAreaTag = "EntryArea";
        /// <summary>
        /// Tag used to identiy the label area within the scene graph
        /// </summary>
        public static readonly string LabelAreaTag = "LabelArea";
        /// <summary>
        /// Tag used to identiy the label within the scene graph
        /// </summary>
        public static readonly string LabelTag = "Label";
        /// <summary>
        /// Tag used to identiy the whole legend item (entry area, label area, label) within the scene graph
        /// </summary>
        public static readonly string LegendItemTag = "LegendItem";
        private string m_text;
        private float m_splitPosition;
        private int m_providerID; 
        private ILArray<float> m_markerPosition = ILMath.localMember<float>(); 

        #endregion

        #region ctor
        /// <summary>
        /// Create new legend item for manual mode, provide provider reference and [optionally] a label string 
        /// </summary>
        /// <param name="providerID">A valid object ID to an provider object for the new legend item</param>
        /// <param name="text">[optional] legend item label text, default: the Tag property of the provider object </param>
        /// <param name="tag">[optional] a tag identifying the item in the scene</param>
        public ILLegendItem(int providerID, string text = null, object tag = null)
            : base(tag ?? LegendItemTag) {
            Add(new ILGroup(LabelAreaTag)); 
            Add(new ILGroup(EntryAreaTag)); 
            LabelArea.Add(new ILLabel() {
                Position = new Vector3(0, 0.5f, 0),
                Anchor = new System.Drawing.PointF(0, 0.5f)
            }); 
            m_text = text; 
            m_providerID = providerID; 
        }
        /// <summary>
        /// Create new legend item for manual mode, provide provider reference and [optionally] a label string 
        /// </summary>
        /// <param name="plotSource">provider object, the source for the new legend item, can not be null</param>
        /// <param name="text">[optional] legend item label text, default: the Tag property of the provider object </param>
        /// <param name="tag">[optional] a tag identifying the item in the scene</param>
        public ILLegendItem(IILLegendItemDataProvider plotSource, string text = null, object tag = null)
            : this(plotSource.GetID(), text, tag) { }

        internal ILLegendItem(ILLegendItem source)
            : base(source) {
            m_text = source.m_text; 
            m_splitPosition = source.m_splitPosition; 
            m_providerID = -source.m_providerID; 
        }
        private ILLegendItem() { }
        #endregion

        #region properties
        /// <summary>
        /// Legend item label text override, default: null -> builds label text from source plot properties
        /// </summary>
        public string Text {
            get {
                return m_text;
            }
            set {
                if (m_text != value) {
                    m_text = value;
                    OnPropertyChanged("Text");
                }
            }
        }
        /// <summary>
        /// The position of the marker within the legend entry area; (X,Y,Z) coordinates; default: (.375, .5, 0)
        /// </summary>
        public ILRetArray<float> MarkerPositions {
            get {
                if (m_markerPosition.IsEmpty) {
                    m_markerPosition.a = new float[] {0.75f / 2f, 0.5f, 0}; 
                }
                return m_markerPosition;
            }
        }

        [XmlIgnore]
        public ILLabel Label {
            get {
                return LabelArea.First<ILLabel>();
            }
        }
        [XmlAttribute]
        internal float SplitPosition {
            get {
                return m_splitPosition;
            }
            set {
                if (m_splitPosition != value) {
                    m_splitPosition = value;
                    OnPropertyChanged("SplitPosition");
                }
            }
        }
        [XmlIgnore]
        public ILGroup LabelArea {
            get {
                return First<ILGroup>(LabelAreaTag);
            }
        }
        [XmlIgnore]
        public ILGroup EntryArea {
            get {
                return First<ILGroup>(EntryAreaTag); ;
            }
        }
        /// <summary>
        /// Gives a reference to the plot object for this legend item or sets it
        /// </summary>
        [XmlAttribute]
        public int ProviderID {
            get {
                return m_providerID;
            }
            set {
                if (m_providerID != value) {
                    m_providerID = value;
                    OnPropertyChanged("ProviderID");
                }
            }
        }
        #endregion

        #region public interface 
        internal override ILNode Copy() {
            return new ILLegendItem(this); 
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILLegendItem ret = (ILLegendItem)base.Synchronize(copy, syncParams);
            if (copy == null || ret.SynchedVersion != Version) {
                ret.m_splitPosition = m_splitPosition; 
                ret.m_text = m_text; 
                ret.m_providerID = m_providerID; 
            }
            return ret; 
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILLegendItem(); 
        }
        protected override bool BeginVisit(ILRenderParameter parameter) {
            var ret = base.BeginVisit(parameter);
            if (parameter.CurrentPassCount == 0) {
                // configure area groups 
                EntryArea.Transform = Matrix4.ScaleTransform(SplitPosition, 1, 1);
                LabelArea.Transform = Matrix4.ScaleTransform(1 - SplitPosition, 1, 1)
                                      .Translate(SplitPosition, 0, 0);
            }
            return ret; 
        }

        internal void UpdateContent() {
            var idp = GetProvider();

            if (idp == null) return; 
            
            idp.ConfigureLegendVisual(EntryArea);

            if (m_text == null) {
                idp.ConfigureLegendLabel(LabelArea);
            } else {
                ILLabel label = LabelArea.First<ILLabel>();
                if (label != null) {
                    label.Text = m_text;
                } else {
                    LabelArea.Add(new ILLabel(text: m_text));
                }
            }
        }

        public IILLegendItemDataProvider GetProvider() {
            ILGroup root = Parent;
            while (root != null && root.Parent != null) {
                root = root.Parent;
            }
            // fetch the corresponding node to my provider id
            if (m_providerID < 0) {
                // This node was created by copying from another scene. 
                // We must find and translate the new provider id ...
                var idp = root.First<ILGroup>(predicate: g => {
                    return (g is IILLegendItemDataProvider) && g.SourceID == -m_providerID;
                }) as IILLegendItemDataProvider;
                m_providerID = idp.GetID(); 
                return idp;
            } else {
                var idp = root.First<ILGroup>(predicate: g => {
                    return (g is IILLegendItemDataProvider) && g.ID == m_providerID;
                }) as IILLegendItemDataProvider;
                return idp;
            }
        }
        #endregion


        #region IILPositionProvider Members
        [XmlIgnore]
        public ILRetArray<float> Positions {
            get { return MarkerPositions;  }
        }
        [XmlIgnore]
        public ILRetArray<int> Indices {
            get { return null; }
        }

        #endregion
    }
}
