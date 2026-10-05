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

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILSelectionBox : ILGroup {

        public static Func<ILSelectionBox,string> DefaultLabelText = StandardLabelText; 
        public static string LinesTag = "SelectionLines"; 
        public static string LabelTag = "SelectionLabel"; 

        public ILLines Lines { 
            get {
                return First<ILLines>(LinesTag); 
            }
        }
        public ILLabel Label { 
            get {
                return First<ILLabel>(LabelTag); ; 
            }
        }
        public ILShape Shape { get; private set; }
        private ILSelectionBox() { }
        internal ILSelectionBox(ILSelectionBox source) : base (source) {
            Shape = source.Shape; 
        }
        public ILSelectionBox(ILShape shape, object tag = null) : base(tag: tag) {
            Shape = shape; 
            CreateLines();
            Add(new ILLabel { 
                Anchor = new PointF(0, 0),
                Text = DefaultLabelText(this)
            }); 

            shape.Limits.Changed += (s, arg) => { Update(s as ILLimits); };
            shape.PropertyChanged += (s, arg) => {
                if (arg != null && arg.PropertyName == "Tag") {
                    Label.Text = DefaultLabelText(this); 
                }
            };
            base.Visible = false; 
            Update(shape.Limits); 
            Configure(); 
        }

        public static string StandardLabelText(ILSelectionBox selectionBox) {
            return object.Equals(selectionBox.Shape.Tag, null) ?
                "(" + selectionBox.Shape.Type.ToString() + ")" : String.Format("{0} ({1})", selectionBox.Shape.Tag.ToString(), selectionBox.Shape.Type.ToString());
        }
        protected override bool BeginVisit(ILRenderParameter parameter) {
            // this function is called in the context of the renderer driver. 
            base.BeginVisit(parameter);
            if (Lines != null) {
                using (ILScope.Enter()) {
                    ILArray<float> transformedCorners = parameter.CurrentModel2CameraTransform * Lines.Positions.Storage;
                    ILArray<int> indices = 0;
                    transformedCorners.a = ILMath.min(ILMath.sum(transformedCorners * transformedCorners, 0), indices, 1);
                    int i = (int)indices[0];
                    Label.Position = Lines.Positions.GetPositionAt(i);
                }
            }
            return true; 
        }
        public override object Tag {
            get {
                return base.Tag;
            }
            set {
                base.Tag = value;
                if (Label != null) 
                    Label.Tag = value; 
            }
        }
        public override bool Visible {
            get {
                return base.Visible;
            }
            set {
                base.Visible = value;
                if (Lines != null)
                    Lines.Visible = value;
                if (Label != null)
                    Label.Visible = value; 
            }
        }
        private void CreateLines() {
            Add(new ILLines("SelectionBox_lines") {
                Positions = ILPositionsBuffer.UnitCube, 
                Indices = ILIndicesBuffer.UnitCube, 
                Color = System.Drawing.Color.Red,
                Selectable = false, 
                Markable = false 
            }); 
            Configure(); 
        }
        internal void Update(ILLimits limits) {
            Transform = Matrix4.ScaleTransform(limits.WidthF <= 0 ? 0.1f : limits.WidthF, limits.HeightF <= 0 ? 0.1f : limits.HeightF, limits.DepthF <= 0 ? 0.1f : limits.DepthF)
                                   .Translate(limits.XMin, limits.YMin, limits.ZMin);
            //Label.Position = limits.Min; // todo: change this to make the label always visible
        }
        internal override ILNode Copy() {
            return new ILSelectionBox(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILSelectionBox();
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILSelectionBox ret = (ILSelectionBox)base.Synchronize(copy, syncParams);
            if (copy == null) {
                // nothing to do
            }
            return ret;
        }
    }
}
