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
using System.Drawing; 
using System.Text;

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILPoints : ILShape {

        #region attributes
        public static string PointsTagDefault = "Points"; 
        float m_size; 
        #endregion

        #region properties
        /// <summary>
        /// Diameter for all the points in the shape, size in pixels 
        /// </summary>
        public float Size {
            get { return m_size; }
            set {
                if (value != m_size) {
                    m_size = value;
                    OnPropertyChanged("Size"); 
                }
            }
        }
        public override int VerticesPerPrimitive {
            get { return 1; }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Create a new points shape
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="color">[optional] color for all points, default: red</param>
        public ILPoints(string tag = "", Color? color = null)
            : base(tag ?? PointsTagDefault) {
            Type = Primitives.Points;
            Size = 6; 
            Color = color ?? System.Drawing.Color.Red; 
            Positions.Update(ILMath.zeros<float>(3,1)); 
        }
        internal ILPoints(ILPoints source)
            : base(source) {
            Type = Primitives.Points; 
            Size = source.Size; 
        }
        #endregion

        #region public interface
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILPoints();
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILPoints ret = (ILPoints)base.Synchronize(copy, syncParams);
            if (ret.SynchedVersion != Version) {
                ret.Size = Size; 
            }
            return ret; 
        }
        internal override ILNode Copy() {
            return new ILPoints(this); 
        }
        internal virtual IEnumerable<Point> GetSortedAndClippedScreen(ILRenderParameter parameters, SortingMode sorting = SortingMode.None, bool frustumClipping = false) {
            using (ILScope.Enter()) {
                if (Positions.IsEmpty) yield break;
                ILArray<float> positions_camera = 1;
                ILArray<int> indices = 1;
                ILArray<float> positions_screen = 1;
                ILArray<float> normals_camera = 1;
                bool userClipping = parameters.PeekClipping() != null;
                int minPlaneId = frustumClipping ? 0 : 6;
                int maxPlaneId = userClipping ? 12 : 6;
                StartPipeline(parameters, sorting, frustumClipping,
                                 positions_camera, indices, positions_screen, normals_camera);
                int maxIndex = GetPrimitiveCount();
                for (int curIndex = 0; curIndex < maxIndex; curIndex++) {
                    Point ret = new Point();
                    #region user clipping
                    int i0 = curIndex; 
                    if (!indices.IsEmpty) {
                        i0 = indices.GetValue(curIndex);
                    }
                    ret.PCam1 = positions_camera.GetPosition4At(i0);
                    bool skip = false;
                    for (int c = minPlaneId; c < maxPlaneId; c++) {
                        if (Vector4.Dot(parameters.GetClipping(c), ret.PCam1) < 0) {
                            skip = true;
                            break;
                        }
                    }
                    if (skip) continue;
                    #endregion

                    ret.P1 = positions_screen.GetPositionAt(i0);

                    if (float.IsNaN(ret.P1.X) || float.IsNaN(ret.P1.Y))
                        continue; 

                    if (parameters.ColorOverride.Peek().HasValue) {
                        ret.C1 = parameters.ColorOverride.Peek().GetValueOrDefault().ToVector4();
                    } else if (Color.HasValue) {
                        ret.C1 = Color.Value.ToVector4();
                    } else if (Colors.DataCount > i0) {
                        ret.C1 = Colors.GetVector4At(i0);
                    } else {
                       ret.C1 = new Vector4(1,0,0,1); 
                    }
                    if (parameters.Alpha.Peek() < 1) {
                        ret.C1.W = parameters.Alpha.Peek(); 
                    }
                    if (Normals != null && Normals.DataCount > i0) {
                        ret.N1 = Normals.GetNormalAt(i0);
                    }
                    yield return ret;
                }
            }
        }
        #endregion

    }
}
