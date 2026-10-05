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
using System.Drawing;
using System.Linq;
using System.Text;

namespace ILNumerics.Drawing.Plotting {
    /// <summary>
    /// The class scales the plot cube data area to always fit into the PlotCubeScreeRect rectangle.
    /// </summary>
    [Serializable]
    public class ILPlotCubeScaleGroup : ILGroup {

        #region attributes
        public static string DefaultTag = "PlotCubeScale";
        private RectangleF? m_plotCubeScreenRect = null;
        private Matrix4 m_rotation;
        /// <summary>
        /// Default tag for the plot cube bounding box lines node
        /// </summary>
        public static readonly string LinesTag = "PlotCubeLines";
        /// <summary>
        /// Tag for the Plots group node
        /// </summary>
        public static string PlotsTag = "PlotsData";
        /// <summary>
        /// Tag used to identify the axes collection within the scene graph node
        /// </summary>
        public static string AxesTag = "Axes";
        #endregion

        #region properties
        internal ILAxisCollection Axes { get { return First<ILAxisCollection>(AxesTag); } }
        internal ILLines Lines { get { return First<ILLines>(LinesTag); } }
        internal ILPlotCubeDataGroup Plots { get { return First<ILPlotCubeDataGroup>(PlotsTag); } }
        /// <summary>
        /// If set: the extend of the screen area used by the plot cube internals, relative viewport coordinates [0,0] -> [1,1]. See: ILPlotCube.DataScreeRect
        /// </summary>
        internal RectangleF DataScreenRect {
            get {
                return m_plotCubeScreenRect.HasValue ? m_plotCubeScreenRect.Value : RectangleF.Empty; 
            }
            set {
                if (value.IsEmpty) {
                    m_plotCubeScreenRect = null; 
                } else {
                    m_plotCubeScreenRect = value;
                }
            }
        }

        public Matrix4 Rotation {
            get {
                return m_rotation;
            }
            set {
                if (!m_rotation.Equals(value)) {
                    m_rotation = value;
                    OnPropertyChanged("Rotation");
                }
            }
        }

        #endregion

        public ILPlotCubeScaleGroup() { }
        public ILPlotCubeScaleGroup(object tag = null)
            : base(tag ?? DefaultTag) {
            //DataScreenRect = new RectangleF(0.5f,0.5f,0.5f,0.5f);
            Rotation = Matrix4.Identity;
            Add(new ILPlotCubeDataGroup(PlotsTag));
            Add(new ILLines(LinesTag));
            Lines.Positions = ILPositionsBuffer.UnitCube;
            Lines.Indices = ILIndicesBuffer.UnitCube;
            Add(new ILAxisCollection(Plots, AxesTag));
            Lines.Color = Color.DarkGray;
            Lines.Configure(); 

        }
        public ILPlotCubeScaleGroup(ILPlotCubeScaleGroup source) 
            : base(source) {
            m_plotCubeScreenRect = source.m_plotCubeScreenRect;
            Rotation = source.Rotation;

            var sourcePlots = source.Find<ILGroup>(predicate: g => g is IILAxisDataProvider).Select(
                    (dg,i) => {
                        return new { dg.ID, Index = i }; 
                    });

            var myPlots = Find<ILGroup>(predicate: g => g is IILAxisDataProvider)
                         .Select(a => a as IILAxisDataProvider).ToList(); 
            foreach (ILAxis axis in Axes) {
                // Note: ILPlotCube does not restrict the number of 
                // plot cube data groups added to it. So we need to rewire 
                // the data range sources (-> ILPlotCubeDataGroup ILAxis.Plots) of every axis 
                // to the _COPY_ of the data groups in the new sub tree. 
                // The match is done by index in the Childs collection, since the order 
                // is be retained during Copy().
                axis.DataProviderID = myPlots[sourcePlots.First(a => a.ID == axis.DataProviderID).Index].ID;  
            }
        }

        #region synched members
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILPlotCubeScaleGroup(); 
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILPlotCubeScaleGroup ret = (ILPlotCubeScaleGroup)base.Synchronize(copy, syncParams);
            ret.DataScreenRect = DataScreenRect;
            if (copy == null || ret.SynchedVersion != Version) {

                // Note: rewiring axes to PCDP is not necessary since we link to the data provider by IDs instead of instances! 

                //ret.Axes = ret.FindById<ILAxisCollection>(Axes.ID);
                //if (ret.Axes != null) {
                //    foreach (ILAxis axis in ret.Axes) {
                //        axis.DataProviderID = ret.FindById<ILPlotCubeDataGroup>(axis.DataProviderID).ID;
                //    }
                //}
                ret.Rotation = Rotation; 
            }
            return ret; 
        }
        internal override ILNode Copy() {
            return new ILPlotCubeScaleGroup(this); 
        }
        #endregion

        #region rendering
        protected override bool BeginVisit(ILRenderParameter parameter) {
            if (parameter.CurrentPassCount == 0) {
                RectangleF rect = m_plotCubeScreenRect.HasValue ? m_plotCubeScreenRect.Value : CalculateDefaultSpaceUsedbyAxes(parameter);
                CalculateTransform(parameter, rect);
            }
            base.BeginVisit(parameter); // populates calculated transform to parameters matrix stack
            if (parameter.CurrentPassCount == 0) {
                Axes.ConfigureAxes_BeginVisit(parameter, Plots.Limits);
            }
            return true; 
        }
        #endregion

        #region private helpers
        private RectangleF CalculateDefaultSpaceUsedbyAxes(ILRenderParameter parameter) {
            SizeF pixelsSize = Axes.CalculateDefaultSize(parameter);
            float margin = Math.Max(pixelsSize.Width, pixelsSize.Height);
            if (margin < 0) margin = 0;
            // convert to relative viewport range (0..1)
            RectangleF vp = parameter.ViewTransform.ToViewRectangle();
            float margX = margin / vp.Width;
            float margY = margin / vp.Height;
            if (margX > 0.3f) margX = 0.3f;
            if (margY > 0.3f) margY = 0.3f; 
            return new RectangleF(margX, margY, 1 - (margX * 2), 1 - (margY * 2));
        }
        /// <summary>
        /// scales and translates the the screen extends of the group node according to the DataScreenRect property
        /// </summary>
        /// <param name="parameter">current render parameter</param>
        /// <param name="screenRect">current setting of DataScreeRect or default/ auto screen rect</param>
        internal void CalculateTransform(ILRenderParameter parameter, RectangleF screenRect) {
            // called from base.BeginVisit(parameter)
            if (parameter.CurrentPassCount == 0) {
                // go from 0..1 -> -1..1 space
                Matrix4 cam = parameter.PeekClipTransform();
                Matrix4 camInv = Matrix4.Invert(cam);
                cam = cam *
                    Matrix4.Translation(-1, -1, -1) *
                    Matrix4.ScaleTransform(2, 2, 2);
                using (ILScope.Enter()) {
                    Matrix4 rot = Matrix4.Translation(0.5f, 0.5f, 0.5f) * Rotation * Matrix4.Translation(-0.5f, -0.5f, -0.5f);
                    ILArray<float> unit = (cam * rot) * ILPositionsBuffer.UnitCube.Storage;
                    // do perspective divide to really know the true extend
                    unit.a = unit / unit["3;:"];
                    ILArray<float> maxUnit = ILMath.max(ILMath.abs(unit), dim: 1);
                    float maxX = maxUnit.GetValue(0);
                    float maxY = maxUnit.GetValue(1);
                    float maxZ = maxUnit.GetValue(2);
                    maxX = (maxX <= 0) ? maxX = 1 : 1f / maxX;
                    maxY = (maxY <= 0) ? maxY = 1 : 1f / maxY;
                    maxZ = (maxZ <= 0) ? maxZ = 1 : 1f / maxZ;
                    Matrix4 CT =
                        Matrix4.Translation(-1, 1, 0) *
                        Matrix4.ScaleTransform(2, -2, 1) *
                        Matrix4.Translation(screenRect.Left, screenRect.Top, 0) *
                        Matrix4.ScaleTransform(screenRect.Width, screenRect.Height, 1) *
                        Matrix4.ScaleTransform(0.5f, -0.5f, 1) *
                        Matrix4.Translation(1, -1, 0) *
                        Matrix4.ScaleTransform(maxX, maxY, maxZ);
                    Transform = camInv * CT * cam * rot;
                }
            }
        }
        //internal void CalculateTransform(ILRenderParameter parameter, RectangleF screenRect) {
        /*  This was an early attempt to utilize 'clear' model matrices only. If done the other way (now in use) 
         *  the perspective correction pollutes the model matrix with perspective components. 
         *  These introduced problems for clipping in a later stage. However, the advantage of 'correct' 
         *  screen rect transforms without perspective distortion pays off. So this version is not used anymore.
         */
        //    // called from base.BeginVisit(parameter)
        //    if (parameter.CurrentPassCount == 0) {
        //        // go from 0..1 -> -1..1 space
        //        Matrix4 Zero1ToMinus1Rotate2CamSpace = Rotation * Matrix4.ScaleTransform(2, 2, 2) * Matrix4.Translation(-0.5f, -0.5f, -0.5f); 
        //        using (ILScope.Enter()) {
        //             ILArray<float> unit = parameter.PeekClipTransform()
        //                         * Zero1ToMinus1Rotate2CamSpace
        //                        * ILPositionsBuffer.UnitCube.Storage; 
        //            // do perspective divide to really know the true extend
        //            unit.a = unit / unit["3;:"];
        //            // shrink / expand in NDC, than transform back and measure
        //            ILArray<float> maxUnitNDC = ILMath.max(ILMath.abs(unit), dim: 1);
        //            maxUnitNDC[2] = 1;  // do not correct Z any further 
        //            unit.a =
        //                Matrix4.Translation(-1, 1, 0) *
        //                Matrix4.ScaleTransform(2, -2, 1) *
        //                Matrix4.Translation(screenRect.Left, screenRect.Top, 0) *
        //                Matrix4.ScaleTransform(screenRect.Width, screenRect.Height, 1) *
        //                Matrix4.ScaleTransform(0.5f, -0.5f, 1) *
        //                Matrix4.Translation(1, -1, 0) *
        //                (unit / maxUnitNDC);
        //            ILArray<float> offsetModel = Matrix4.Invert(parameter.PeekClipTransform() ) * unit;
        //            offsetModel.a = offsetModel / offsetModel["end;:"];
        //            ILArray<float> translate = ILMath.min(offsetModel, dim: 1) - ILMath.min((Zero1ToMinus1Rotate2CamSpace * ILPositionsBuffer.UnitCube.Storage),dim:1);
        //            ILArray<float> scale = (ILMath.max(offsetModel, dim: 1) - ILMath.min(offsetModel, dim: 1)) 
        //                                / (ILMath.max((Zero1ToMinus1Rotate2CamSpace * ILPositionsBuffer.UnitCube.Storage),dim:1) - ILMath.min((Zero1ToMinus1Rotate2CamSpace * ILPositionsBuffer.UnitCube.Storage),dim:1)); 
        //                /// ILMath.max((Zero1ToMinus1Rotate2CamSpace * ILPositionsBuffer.UnitCube.Storage)["0:2;:"], dim: 1); // - ILMath.min(ILPositionsBuffer.UnitCube.Storage,dim: 1);
        //            Transform =
        //                Matrix4.ScaleTransform(scale.GetValue(0), scale.GetValue(1), scale.GetValue(2)) *
        //                Matrix4.Translation(translate.GetValue(0), translate.GetValue(1), translate.GetValue(2)) *
        //                        Zero1ToMinus1Rotate2CamSpace; 
        //        }
        //    }
        //}
        #endregion

    }
}
