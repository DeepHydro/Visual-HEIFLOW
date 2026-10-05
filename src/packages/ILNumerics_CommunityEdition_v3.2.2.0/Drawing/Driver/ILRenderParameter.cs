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
using OpenTK;

namespace ILNumerics.Drawing {
    public class ILRenderParameter {

        public byte CurrentPassCount { get; internal set; }
        protected byte? NextRenderPass { get; private set; }
        protected Stack<Matrix4> m_matrixStack { get; private set; }
        private Stack<Matrix4> ProjectionTransforms { get; set; }
        public Stack<Matrix4> ViewTransforms { get; private set; }
        public Stack<Matrix4> CameraPositionTransforms { get; private set; }
        public Stack<RenderTarget> RenderTargets { get; private set; }
        private Vector4[] m_frustumPlanes;
        
        /// <summary>
        /// Frustum planes for the current projection matrix, camera coords
        /// </summary>
        public Vector4[] FrustumPlanes {
            get {
                if (m_frustumPlanes == null) {
                    // create frustum planes from current projection matrix
                    m_frustumPlanes = CalculateFrustumPlanes(); 
                }
                return m_frustumPlanes; 
            }
        }

        public SizeF ViewportScaleFactor {
            get {
                Size driverSizeView = Driver.Size; 
                return new SizeF( Math.Abs(2 * ViewTransform.M11 / driverSizeView.Width), Math.Abs(2 * ViewTransform.M22 / driverSizeView.Height)); 
            }
        }

        public ILDriver Driver { get; private set; }
        public List<ILPreprocessedShape> Transparent3DNodes = new List<ILPreprocessedShape>();
        public List<ILPreprocessedShape> Screen2DNearNodes = new List<ILPreprocessedShape>();
        public List<ILPreprocessedShape> TransparentScreen2DNearNodes = new List<ILPreprocessedShape>();
        public List<ILPreprocessedShape> Screen2DFarNodes = new List<ILPreprocessedShape>();
        public List<ILPreprocessedShape> TransparentScreen2DFarNodes = new List<ILPreprocessedShape>();
        public List<Light> Lights = new List<Light>();
        internal ILPickingContext PickingContext;
        public bool DepthTestEnabled { get; set; }
        public bool BlendingEnabled { get; set; }
        public Stack<Color?> ColorOverride { get; private set; }
        public Stack<float> Alpha { get; private set; }
        public Stack<Vector4> LogState { get; private set; }
        public long Time_ms { get; set; }
        private Stack<ILClipParams> m_clipping;
        private Matrix4? m_inverseProjCamModelMat = null;
        internal Matrix4 InverseProjCamModelMat {
            get {
                if (!m_inverseProjCamModelMat.HasValue) {
                    m_inverseProjCamModelMat = calculateInverseProjCamMat();
                }
                return m_inverseProjCamModelMat.Value;
            }
            set {
                m_inverseProjCamModelMat = value;
            }
        }

        private Matrix4? calculateInverseProjCamMat() {
            return Matrix4.Invert(PeekClipTransform());
        }

        public Matrix4 ProjectionTransform { get { return ProjectionTransforms.Peek(); } }
        public Matrix4 ViewTransform { get { return ViewTransforms.Peek(); } }
        public Matrix4 WorldToCameraTransform {
            get {
                if (CameraPositionTransforms.Count > 0) {
                    return CameraPositionTransforms.Peek();
                }
                return Matrix4.Identity; 
            }
        }


        public ILRenderParameter(ILDriver driver) {
            Driver = driver;
            CurrentPassCount = 0;
            m_matrixStack = new Stack<Matrix4>(10);

            ViewTransforms = new Stack<Matrix4>();
            ViewTransforms.Push(Driver.ViewTransform);
            ProjectionTransforms = new Stack<Matrix4>();
            ProjectionTransforms.Push(Matrix4.OrthographicTransform(-1,1,1,-1,-1,1));

            CameraPositionTransforms = new Stack<Matrix4>(); 

            ColorOverride = new Stack<Color?>();
            ColorOverride.Push(null); 
            Alpha = new Stack<float>();
            Alpha.Push(1); // eases queries   
            BlendingEnabled = false;
            DepthTestEnabled = true;

            LogState = new Stack<Vector4>(); 
            LogState.Push(new Vector4(0,0,0,0)); 

            RenderTargets = new Stack<RenderTarget>();
            RenderTargets.Push(RenderTarget.World3D); 

            m_clipping = new Stack<ILClipParams>();
            m_clipping.Push(null);
            
        }
        /// <summary>
        /// takes model coord clipping planes, stores them in camera coords
        /// </summary>
        /// <param name="clipping"></param>
        public void PushClipping(ILClipParams clipping, bool takeasCameraCoords = false) {
            if (!takeasCameraCoords) {
                if (clipping == null) {
                    m_clipping.Push(clipping);
                } else {
                    ILClipParams newClip = new ILClipParams();
                    Matrix4 invModelMat = Matrix4.Invert(CurrentModel2CameraTransform);
                    //invModelMat.M43 = 1; 
                    m_clipping.Push(newClip);
                    for (int i = 0; i < 6; i++) {
                        newClip[i] = clipping[i] * invModelMat;
                    }
                }
            } else {
                m_clipping.Push(clipping);
            }
        }
        public void PopClipping() {
            m_clipping.Pop();
        }
        public ILClipParams PeekClipping() {
            return m_clipping.Peek();
        }
        internal Matrix4 Pop() {
            System.Diagnostics.Debug.Assert(m_matrixStack.Count > 0);
            m_inverseProjCamModelMat = null;
            return m_matrixStack.Pop();
        }

        internal void Push(Matrix4 transform) {
            m_inverseProjCamModelMat = null;
            m_matrixStack.Push(CurrentModel2CameraTransform * transform);
            //System.Diagnostics.Debug.WriteLine("Stack New Transform" + CurrentTransform); 
            //System.Diagnostics.Debug.WriteLine(""); 

        }
        internal void PushNew(Matrix4 transform) {
            m_inverseProjCamModelMat = null;
            m_matrixStack.Push(transform);
        }
        //internal Matrix4 CameraTransform {
        //    System.Diagnostics.Debug.Assert(m_matrixStack.Count > 0);
        //    return m_matrixStack.Peek();
        //}
        internal Matrix4 PeekClipTransform() {
            System.Diagnostics.Debug.Assert(m_matrixStack.Count > 0);
            return ProjectionTransform * m_matrixStack.Peek();
        }
        public Matrix4 CurrentModel2CameraTransform {
            get {
                if (m_matrixStack.Count > 0)
                    return m_matrixStack.Peek();
                else {
                    return Matrix4.Identity;
                }
            }
        }


        internal ILRetArray<float> ToScreen(ILInArray<float> A) {
            using (ILScope.Enter(A)) {
                ILArray<float> locA = ProjectionTransform * m_matrixStack.Peek() * A;
                locA.a = ViewTransform * (locA / locA["end;:"]);
                return locA;
            }
        }
        internal ILRetArray<float> ToClipWithDivide(ILInArray<float> A) {
            using (ILScope.Enter(A)) {
                ILArray<float> locA = ProjectionTransform * m_matrixStack.Peek() * A;
                locA.a = locA / locA["end;:"];
                return locA;
            }
        }
        internal ILRetArray<float> ToClip(ILInArray<float> A) {
            using (ILScope.Enter(A)) {
                ILArray<float> locA = ProjectionTransform * m_matrixStack.Peek() * A;
                return locA;
            }
        }
        internal Vector4 ToClip(Vector4 A1) {
            using (ILScope.Enter()) {
                return ProjectionTransform * m_matrixStack.Peek() * A1;
            }
        }
        internal void ToScreen(ref Vector3 A1, ref Vector3 A2) {
            using (ILScope.Enter()) {
                Matrix4 clipTransform = ProjectionTransform * m_matrixStack.Peek();
                A1 = ViewTransform * (clipTransform * A1); // <- makes persp. divide
                A2 = ViewTransform * (clipTransform * A2);
            }
        }
        internal Vector3 ToScreen(Vector3 A1) {
            using (ILScope.Enter()) {
                Matrix4 clipTransform = ProjectionTransform * m_matrixStack.Peek();
                return ViewTransform * (clipTransform * A1); // <- makes persp. divide
            }
        }
        internal Vector4 ToScreen(Vector4 A1) {
            using (ILScope.Enter()) {
                Matrix4 clipTransform = ProjectionTransform * m_matrixStack.Peek();
                return ViewTransform * (clipTransform * A1); // <- makes persp. divide
            }
        }

        internal Vector4 ToModel(Vector3 screen) {
            Matrix4 vt = Matrix4.Invert(ViewTransform);
            //Vector4 view = new Vector4((screen.X - (vt.M14 - vt.M11)) / (vt.M11 * 2f) - 1f,
            //                           (screen.Y - (vt.M24 - vt.M22)) / (vt.M22 * 2f) - 1f,
            //                            2f * screen.Z - 1f,
            //                            1f);
            Vector4 view = vt * new Vector4(screen, 1);
            Vector4 model = InverseProjCamModelMat * view;
            if (model.W != 0) {
                model = model / model.W;
            }
            return model;
        }

        internal Vector3 Cam2Screen(Vector4 a) {
            Vector4 ret = ProjectionTransform * a;
            ret = ret / ret.W;
            ret = ViewTransform * ret;
            return ret.Xyz;
        }
        internal Vector3 Cam2Screen(Vector3 a) {
            return ViewTransform * (ProjectionTransform * a);
        }
        /// <summary>
        /// returns custom clip planes from stack (if any) or frustum planes  
        /// </summary>
        /// <param name="planeID">plane index: 0..5 -> frustum planes; 6 ... 12 (currently) -> custom planes from stack</param>
        /// <returns></returns>
        internal Vector4 GetClipping(int planeID) {
            if (planeID < 6) {
                return FrustumPlanes[planeID]; 
            } else {
                if (m_clipping.Peek() == null || planeID > 11) {
                    return new Vector4(-1,0,0,float.MaxValue); 
                }
                return m_clipping.Peek()[planeID - 6]; 
            }
        }

        private Vector4[] CalculateFrustumPlanes() {
            Vector4[] ret = new Vector4[6]; 
            System.Diagnostics.Debug.Assert(ProjectionTransforms.Count > 0); 
            Matrix4 p = ProjectionTransform;
            // this is taken from: Mathematics for 3D Game Programming and Computer Graphics, Eric Lengyel, 2nd Edition
            ret[0] = new Vector4(p.M41 + p.M31, p.M42 + p.M32, p.M43 + p.M33, p.M44 + p.M34); 
            ret[1] = new Vector4(p.M41 - p.M31, p.M42 - p.M32, p.M43 - p.M33, p.M44 - p.M34); 
            ret[2] = new Vector4(p.M41 + p.M11, p.M42 + p.M12, p.M43 + p.M13, p.M44 + p.M14); 
            ret[3] = new Vector4(p.M41 - p.M11, p.M42 - p.M12, p.M43 - p.M13, p.M44 - p.M14); 
            ret[4] = new Vector4(p.M41 + p.M21, p.M42 + p.M22, p.M43 + p.M23, p.M44 + p.M24); 
            ret[5] = new Vector4(p.M41 - p.M21, p.M42 - p.M22, p.M43 - p.M23, p.M44 - p.M24); 
            return ret;     
        }

        internal void PushProjectionTransform(Matrix4 ProjectionTransform) {
            ProjectionTransforms.Push(ProjectionTransform); 
            m_frustumPlanes = null; 
        }


        internal void PopProjectionTransform() {
            ProjectionTransforms.Pop(); 
            m_frustumPlanes = null; 
        }
    }
}
