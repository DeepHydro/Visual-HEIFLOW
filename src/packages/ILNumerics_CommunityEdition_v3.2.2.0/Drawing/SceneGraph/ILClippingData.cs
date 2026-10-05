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
using System.Text;
using System.Xml.Serialization;

namespace ILNumerics.Drawing {
    /// <summary>
    /// Class holding and managing limits for a 3 dimensional cube
    /// </summary>
    [Serializable]
    public sealed class ILLimits {

        #region eventing 
        /// <summary>
        /// fires if the data range have changed
        /// </summary>
        public event ILClippingDataChangedEvent Changed; 
        /// <summary>
        /// called if the limits have changed
        /// </summary>
        private void OnChange() {
            if (m_eventingActive && Changed != null) {
                Changed ( this, new ClippingChangedEventArgs(this)); 
            }
            m_isDirty = false; 
        }
        #endregion

        #region attributes 
        private bool m_eventingActive = true;
        private float m_xMin = float.MaxValue;
        private float m_yMin = float.MaxValue;
        private float m_zMin = float.MaxValue;
        private float m_xMax = float.MinValue;
        private float m_yMax = float.MinValue;
        private float m_zMax = float.MinValue;
        private float m_sphereRadius; 
        private bool m_isDirty = false;
        private bool m_allowZeroVolume = true;
        #endregion

        #region properties 
        /// <summary>
        /// the radius of a sphere tightly enclosing the box determined by this clipping data limits (readonly)
        /// </summary>
        [XmlIgnore]
        public float SphereRadius {
            get {
                return m_sphereRadius;  
            }
        }
        /// <summary>
        /// minimum value for x axis
        /// </summary>
        [XmlIgnore]
        public float XMin {
            get {
                return m_xMin; 
            }
            set {
                if (m_xMin != value) {
                    m_isDirty = true; 
                    m_xMin = value;
                    if (!m_allowZeroVolume) ensureVolumeNotZero();
                    m_sphereRadius = getSphereRadius(); 
                    if (m_eventingActive && Changed != null)
                        OnChange();
                }
            }
        }
        /// <summary>
        /// minimum value for y axis
        /// </summary>
        [XmlIgnore]
        public float YMin {
            get {
                return m_yMin; 
            }
            set {
                if (m_yMin != value) {
                    m_isDirty = true; 
                    m_yMin = value;
                    if (!m_allowZeroVolume) ensureVolumeNotZero();
                    m_sphereRadius = getSphereRadius();
                    if (m_eventingActive && Changed != null)
                        OnChange();
                }
            }
        }
        /// <summary>
        /// minimum value for z axis
        /// </summary>
        [XmlIgnore]
        public float ZMin {
            get {
                return m_zMin; 
            }
            set {
                if (m_zMin != value) {
                    m_isDirty = true;
                    m_zMin = value;
                    if (!m_allowZeroVolume) ensureVolumeNotZero();
                    m_sphereRadius = getSphereRadius();
                    if (m_eventingActive && Changed != null)
                        OnChange(); 
                }
            }
        }
        /// <summary>
        /// maximum value for x axis
        /// </summary>
        [XmlIgnore]
        public float XMax {
            get {
                return m_xMax; 
            }
            set {
                if (m_xMax != value) {
                    m_isDirty = true; 
                    m_xMax = value;
                    if (!m_allowZeroVolume) ensureVolumeNotZero();
                    m_sphereRadius = getSphereRadius();
                    if (m_eventingActive && Changed != null)
                        OnChange(); 
                }
            }
        }
        /// <summary>
        /// maximum value for y axis
        /// </summary>
        [XmlIgnore]
        public float YMax {
            get {
                return m_yMax; 
            }
            set {
                if (m_yMax != value) {
                    m_isDirty = true; 
                    m_yMax = value;
                    if (!m_allowZeroVolume) ensureVolumeNotZero();
                    m_sphereRadius = getSphereRadius();
                    if (m_eventingActive && Changed != null)
                        OnChange(); 
                }
            }
        }
        /// <summary>
        /// maximum value for z axis
        /// </summary>
        [XmlIgnore]
        public float ZMax {
            get {
                return m_zMax; 
            }
            set {
                if (m_zMax != value) {
                    m_isDirty = true; 
                    m_zMax = value;
                    if (!m_allowZeroVolume) ensureVolumeNotZero();
                    m_sphereRadius = getSphereRadius();
                    if (m_eventingActive && Changed != null)
                        OnChange(); 
                }
            }
        }
        /// <summary>
        /// minimum (coordinate)
        /// </summary>
        [ILXmlSerializeAs("{X},{Y},{Z}")]
        public Vector3 Min {
            get {
                return new Vector3(XMin,YMin,ZMin);
            }
        }
        /// <summary>
        /// maximum (coordinate)
        /// </summary>
        [ILXmlSerializeAs("{X},{Y},{Z}")]
        public Vector3 Max {
            get {
                return new Vector3(XMax,YMax,ZMax); 
            }
        }
        /// <summary>
        /// get center of this clipping range
        /// </summary>
        [XmlIgnore]
        public Vector3 CenterF {
            get { 
                Vector3 ret = new Vector3(
                    (XMax + XMin) / 2.0f,
                    (YMax + YMin) / 2.0f,
                    (ZMax + ZMin) / 2.0f);
                return ret; 
            }
        }
        /// <summary>
        /// get width (x-direction) of this clipping range
        /// </summary>
        [XmlIgnore]
        public float WidthF {
            get {
                return (XMax - XMin);
            }
        }
        /// <summary>
        /// get height (y-direction) of this clipping range
        /// </summary>
        [XmlIgnore]
        public float HeightF {
            get {
                return (YMax - YMin);
            }
        }
        /// <summary>
        /// get depth (z-direction) of this clipping range
        /// </summary>
        [XmlIgnore]
        public float DepthF {
            get {
                return (ZMax - ZMin);
            }
        }
        /// <summary>
        /// marks the limits as altered, without having fired a changed event yet
        /// </summary>
        [XmlIgnore]
        public bool IsDirty {
            get {
                return m_isDirty; 
            }
        }
        /// <summary>
        /// true: this clipping data always ensures a non-zero volume
        /// </summary>
        /// <remarks>'NonZeroVolumne' means, non of Depth,Width nor Heigth are allowed to be zero. If some edge of the cube is set to zero, the class expands this edge by 1 in each direction.</remarks>
        [XmlIgnore]
        public bool AllowZeroVolume {
            get {
                return m_allowZeroVolume;
            }
            set {
                if (m_allowZeroVolume != value && value == true) {
                    ensureVolumeNotZero(); 
                }
                m_allowZeroVolume = value; 
            }
        }
        /// <summary>
        /// Gets if this limits object marks an empty volume [readonly]
        /// </summary>
        [XmlIgnore]
        public bool IsEmpty { get { return WidthF <= 0 && HeightF <= 0 && DepthF <= 0; } }
        #endregion

        #region public interface
        /// <summary>
        /// suspend the firing of events until EventingResume has been called
        /// </summary>
        public void EventingSuspend() {
            m_eventingActive = false;
            m_isDirty = false; 
        }
        /// <summary>
        /// Resume previously suspended eventing. Start sending events again.
        /// </summary>
        public void EventingResume() {
            m_eventingActive = true;
            if (m_isDirty && Changed != null) {
                OnChange();
            }
        }
        /// <summary>
        /// enable eventing, discarding pending events
        /// </summary>
        public void EventingStart() {
            m_eventingActive = true;
            m_isDirty = false; 
        }
        /// <summary>
        /// update ranges for this object with union of both ranges. 
        /// </summary>
        /// <param name="clipData">clipping ranges to create union with</param>
        public void Update (ILLimits clipData) {
            if (clipData.XMin < XMin) { m_isDirty = true; m_xMin = clipData.XMin; }
            if (clipData.YMin < YMin) { m_isDirty = true; m_yMin = clipData.YMin; }
            if (clipData.ZMin < ZMin) { m_isDirty = true; m_zMin = clipData.ZMin; }
            if (clipData.XMax > XMax) { m_isDirty = true; m_xMax = clipData.XMax; }
            if (clipData.YMax > YMax) { m_isDirty = true; m_yMax = clipData.YMax; }
            if (clipData.ZMax > ZMax) { m_isDirty = true; m_zMax = clipData.ZMax; }
            if (!m_allowZeroVolume) ensureVolumeNotZero();
            m_sphereRadius = getSphereRadius();
            if (m_isDirty && m_eventingActive && Changed != null) 
                OnChange(); 
            }
        /// <summary>
        /// update ranges for this object with point coords for specific axes
        /// </summary>
        /// <param name="point">point with coords to update ranges with</param>
        /// <param name="updateBitFlags">bitflag combination to specify axis to be recognized: 1,2,4 -> x,y,z</param>
        public void Update (Vector3 point, int updateBitFlags) {
            if ((updateBitFlags & 1) != 0) {
                if (point.X < XMin) { m_isDirty = true; m_xMin = point.X; }
                if (point.X > XMax) { m_isDirty = true; m_xMax = point.X; }
            }
            if ((updateBitFlags & 2) != 0) {
                if (point.Y < YMin) { m_isDirty = true; m_yMin = point.Y; }
                if (point.Y > YMax) { m_isDirty = true; m_yMax = point.Y; }
            }
            if ((updateBitFlags & 4) != 0) {
                if (point.Z < ZMin) { m_isDirty = true; m_zMin = point.Z; }
                if (point.Z > ZMax) { m_isDirty = true; m_zMax = point.Z; }
            }
            if (!m_allowZeroVolume) ensureVolumeNotZero();
            m_sphereRadius = getSphereRadius();
            if (m_isDirty && m_eventingActive && Changed != null) 
                OnChange(); 
        }
        /// <summary>
        /// update clipping data for this object with union of this and rectangle specified
        /// </summary>
        /// <param name="luCorner">left upper corner</param>
        /// <param name="rbCorner">right lower corner</param>
        public void Update (Vector3 luCorner, Vector3 rbCorner) {
            bool oldeventState = m_eventingActive; 
            m_eventingActive = false; 
            Update(luCorner,7);
            m_eventingActive = oldeventState; 
            Update(rbCorner,7);
        }
        public void Update (Vector3 center, float zoomFactor) {
            if (zoomFactor == 1.0f && center == CenterF) return; 
            m_isDirty = true; 
            float s = WidthF * zoomFactor / 2; 
            m_xMin = center.X - s; 
            m_xMax = center.X + s; 
            s = HeightF * zoomFactor / 2; 
            m_yMin = center.Y - s; 
            m_yMax = center.Y + s; 
            s = DepthF * zoomFactor / 2; 
            m_zMin = center.Z - s; 
            m_zMax = center.Z + s;
            if (!m_allowZeroVolume) ensureVolumeNotZero();
            m_sphereRadius = getSphereRadius();
            if (m_eventingActive && Changed != null) 
                OnChange();  
        }
        /// <summary>
        /// Set clipping limits to volume inside the box specified 
        /// </summary>
        /// <param name="lunCorner">left-upper-near corner of the volume box</param>
        /// <param name="rbfCorner">right-bottom-far corner of the volume box</param>
        public void Set(Vector3 lunCorner, Vector3 rbfCorner) {
            m_isDirty = true; 
            m_xMin = Math.Min(lunCorner.X,rbfCorner.X); 
            m_xMax = Math.Max(lunCorner.X,rbfCorner.X); 
            m_yMin = Math.Min(lunCorner.Y,rbfCorner.Y); 
            m_yMax = Math.Max(lunCorner.Y,rbfCorner.Y); 
            m_zMin = Math.Min(lunCorner.Z,rbfCorner.Z); 
            m_zMax = Math.Max(lunCorner.Z,rbfCorner.Z);
            if (!m_allowZeroVolume) ensureVolumeNotZero();
            m_sphereRadius = getSphereRadius();
            if (m_eventingActive && Changed != null) 
                OnChange(); 
        }
        /// <summary>
        /// copy this from other clipping data
        /// </summary>
        /// <param name="m_clippingData"></param>
        internal void CopyFrom(ILLimits m_clippingData) {
            m_xMin = m_clippingData.XMin;
            m_yMin = m_clippingData.YMin;
            m_zMin = m_clippingData.ZMin;
            m_xMax = m_clippingData.XMax;
            m_yMax = m_clippingData.YMax;
            m_zMax = m_clippingData.ZMax;
            if (!m_allowZeroVolume) ensureVolumeNotZero();
            m_sphereRadius = getSphereRadius();
            m_isDirty = true; 
            if (m_eventingActive && Changed != null) 
                OnChange();  
        }
        /// <summary>
        /// creates clone of this clipping data
        /// </summary>
        /// <returns>clone</returns>
        public ILLimits Clone() {
            ILLimits ret = new ILLimits(); 
            ret.Update(this); 
            return ret;
        }

        public override string ToString() {
            return String.Format("Min:{0} Max:{1}",Min,Max); 
        }
        #endregion

        #region private helper

        private float getSphereRadius() {
            return ((Max - Min) / 2f).Length; 
        }

        private void ensureVolumeNotZero() {
            if (HeightF == 0) {
                m_isDirty = true;
                m_yMin = m_yMin - 1f;
                m_yMax = m_yMin + 2f;
            }
            if (WidthF == 0) {
                m_isDirty = true;
                m_xMin = m_xMin - 1f;
                m_xMax = m_xMin + 2f;
            }
            if (DepthF == 0) {
                m_isDirty = true;
                m_zMin = m_zMin - 1f;
                m_zMax = m_zMin + 2f;
            }
        }
        #endregion

        #region operator overloads 
        /// <summary>
        /// Equalty operator overload, true if both cubes span the same region in 3D space 
        /// </summary>
        /// <param name="limit1">cube 1</param>
        /// <param name="limit2">cube 2</param>
        /// <returns>true if both cubes span the same 3D space, false otherwise</returns>
        public static bool operator == (ILLimits limit1, ILLimits limit2) {
            return (limit1.m_xMax == limit2.m_xMax && 
                    limit1.m_yMax == limit2.m_yMax && 
                    limit1.m_zMax == limit2.m_zMax && 
                    limit1.m_xMin == limit2.m_xMin && 
                    limit1.m_yMin == limit2.m_yMin && 
                    limit1.m_zMin == limit2.m_zMin); 
        }
        /// <summary>
        /// unequalty operator
        /// </summary>
        /// <param name="limit1">cube 1</param>
        /// <param name="limit2">cube 2</param>
        /// <returns>false if both cubes span the same 3D space, true otherwise</returns>
        public static bool operator !=(ILLimits limit1, ILLimits limit2) {
            return (limit1.m_xMax != limit2.m_xMax || 
                    limit1.m_yMax != limit2.m_yMax ||
                    limit1.m_zMax != limit2.m_zMax || 
                    limit1.m_xMin != limit2.m_xMin || 
                    limit1.m_yMin != limit2.m_yMin || 
                    limit1.m_zMin != limit2.m_zMin); 
        }
        /// <summary>
        /// Returns hash code for this ILClippingData
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }
        /// <summary>
        /// Compares to cube objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if obj references this class instance, false otherwise</returns>
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
        #endregion


        internal float GetMin(AxisNames AxisName) {
            switch (AxisName) {
                case AxisNames.XAxis:
                    return XMin;
                case AxisNames.YAxis:
                    return YMin;
                default:
                    return ZMin;
            }
        }
        internal float GetMax(AxisNames AxisName) {
            switch (AxisName) {
                case AxisNames.XAxis:
                    return XMax;
                case AxisNames.YAxis:
                    return YMax;
                default:
                    return ZMax;
            }
        }

    }
}
