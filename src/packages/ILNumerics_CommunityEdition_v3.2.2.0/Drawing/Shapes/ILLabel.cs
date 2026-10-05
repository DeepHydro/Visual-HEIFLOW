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
using ILNumerics.Data;
using System.Xml.Serialization;  

namespace ILNumerics.Drawing {

    [Serializable]
    public class ILLabel : ILDrawable {

        #region attributes
        Vector3 m_position; 
        Font m_font;
        IILTextInterpreter m_interpreter;
        private string m_text;
        private double m_rotation;
        private System.Drawing.PointF m_anchor;
        private static Font s_defaultFont = new Font("Helvetica", SystemFonts.MessageBoxFont.SizeInPoints); // SystemFonts.MessageBoxFont;
        #endregion

        #region properties
           
        /// <summary>
        /// Default font for new label objects
        /// </summary>
        [ILXmlSerializeAs("{Name},{Size},{Style}")]
        public static Font DefaultFont {
            get {
                return (Font)s_defaultFont.Clone();
            }
            set {
                if (s_defaultFont != value && value != null) {
                    s_defaultFont = (Font)value.Clone();
                }
            }
        }

        /// <summary>
        /// Defines a hint for rendering font sizes &lt; 16 points. Default: AntiAliasGridFit
        /// </summary>
        /// <remarks>Small font sizes may look blurry, when rendered with antialiasing. Per default, AntiAliasGridFit is selected. Setting this property to 
        /// <see cref="System.Drawing.Text.TextRenderingHint.AntiAlias"/> will generate a more smooth output for small fonts.</remarks>
        public static System.Drawing.Text.TextRenderingHint SmallFontTextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

        [XmlIgnore]
        public IILTextInterpreter Interpreter {
            get {
                if (m_interpreter == null) {
                    m_interpreter = new ILSimpleTexInterpreter(); 
                }
                return m_interpreter; 
            }
            set {
                if (value == null)
                    throw new ArgumentNullException();
                m_interpreter = value;
            }
        }

        [XmlAttribute]
        public double Rotation {
            get {
                return m_rotation;
            }
            set {
                if (m_rotation != value) {
                    m_rotation = value;
                    OnPropertyChanged("Rotation");
                }
            }
        }
        
        public ILFringe Fringe { get; set; }

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

        public Vector3 Position {
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

        [ILXmlSerializeAs("{X},{Y}")]
        public System.Drawing.PointF Anchor {
            get {
                return m_anchor;
            }
            set {
                if (m_anchor != value) {
                    m_anchor = value;
                    OnPropertyChanged("Anchor");
                }
            }
        }

        [ILXmlSerializeAs("{Name},{Size},{Style}")]
        public System.Drawing.Font Font {
            get {
                return m_font;
            }
            set {
                if (m_font != value && value != null) {
                    m_font = value;
                    OnPropertyChanged("Font"); 
                }
            }
        }
        #endregion

        #region constructors
        /// <summary>
        ///  Copy constructor
        /// </summary>
        /// <param name="source"></param>
        protected ILLabel(ILLabel source) : base (source) {
            this.m_anchor = source.m_anchor; 
            this.m_font = (Font)source.m_font.Clone();
            this.m_position = source.m_position; 
            this.m_text = source.m_text; 
            this.Fringe = source.Fringe.Clone();
            this.m_rotation = source.m_rotation; 
            this.m_interpreter = source.m_interpreter;  // todo: check! reuse or clone necessary?
        }
        public ILLabel(string text = "", object tag = null) : base(tag) { 
            this.Color = System.Drawing.Color.Black;
            this.m_anchor = new PointF(0.5f, 0.5f);
            this.m_text = text; 
            this.Fringe = new ILFringe();
            this.m_rotation = 0; 
            this.m_font = DefaultFont; 
        }
        #endregion

        #region public interface 
        public override void Dispose() {
            base.Dispose(); 
            if (m_font != null)
                m_font.Dispose();
        }

        public override ILNode Detach() {
            return this; 
        }

        protected override void VisitInternal(ILRenderParameter parameter) {
            base.VisitInternal(parameter); 
            parameter.Driver.VisitNode(this, parameter);
        }
        internal override ILNode Copy() {
            return new ILLabel (this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILLabel();
        }
        public override Vector3 GetPosition() {
            return Position; 
        }
        internal override bool IsTransparent {
            get {
                return true;
            }
        }
        public override string ToString() {
            return String.Format("{0} Text:'{1}'", base.ToString(), Text);
        }
        /// <summary>
        /// Measures the size in screen pixels of the current label state
        /// </summary>
        /// <returns>size in pixels as SizeF struct</returns>
        public SizeF MeasureSize() {
            float h = Font.GetHeight(); 
            if (String.IsNullOrEmpty(Text)) return new SizeF(0, h); 
            ILRenderQueue queue = Interpreter.Transform(this, new ILDummyTextureStorage());
            
            return queue.Size;
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILLabel label = (ILLabel)base.Synchronize(copy, syncParams);
            if (copy == null || label.SynchedVersion != Version) {
                // we must utilize the property setter in order for the PropertyChangedEvent to fire! (needed for tick labels)
                label.Anchor = this.m_anchor;
                label.Font = (Font)this.m_font.Clone();
                label.Position = this.m_position;
                label.Text = this.m_text;
                label.Interpreter = this.Interpreter;
                label.Fringe = this.Fringe.Clone();
                label.Rotation = this.m_rotation;
            }
            return label; 
        }

        #endregion

        #region BSP tree related

        //internal override void ToBSPBuildPrimitives<T>(IList<BSPPrimitive<T>> primitives, Matrix4 model2CameraTransform, Matrix4 camera2ClipTransform, T data) {
        //    var ret = new BSPPrimitive<T>() {
        //        Positions = new Vector3[1] { model2CameraTransform * Position },
        //        Data = data,
        //    }; 
        //    primitives.Add(ret); 
        //}
        #endregion


        //internal override void FromBSPBuildPrimitives(IEnumerable<BSPPrimitive<ILDrawable>> primitive) {
        //    System.Diagnostics.Debug.Assert(primitive.Count() == 1); 
        //    // we dont need to update anything for labels here
        //    return; 
        //}
    }
}
