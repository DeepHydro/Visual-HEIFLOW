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
using System.Linq;
using System.Drawing;
using ILNumerics;
using ILNumerics.Drawing; 

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public sealed class ILColormap {

        #region events
        /// <summary>
        /// Fires when the colormap data have changed
        /// </summary>
        public event EventHandler Changed;
        protected void OnChanged() {
            m_version++;
            if (Changed != null) {
                Changed(this, null);
            }
        }
        #endregion

        #region attributes
        public static Vector4 DefaultOutOfRangeOrNaNColor = Color.Empty.ToVector4(); 
        ILArray<float> m_map = ILMath.localMember<float>();
        Colormaps m_type;
        int m_version;
        long m_synchedVersion;
        int m_synchedHashCode; 
        #endregion

        #region properties
        internal int SynchedHashCode {
            get {
                return m_synchedHashCode; 
            }
        }
        internal long Version {
            get { return m_version; }
        }

        /// <summary>
        /// Number of colors in the colormap
        /// </summary>
        public int Length {
            get {
                return m_map.Size[0];
            }
        }
        /// <summary>
        /// The colormap type this colormap is based on (readonly)
        /// </summary>
        public Colormaps Type {
            get {
                return m_type;
            }
        }
        /// <summary>
        /// retrieve / set internal data for color indices
        /// </summary>
        /// <remarks><para>Data is a matrix with 5 columns, elements in range [0...1] !
        /// <list type="number">
        /// <item>Column 0: keypoint position</item>
        /// <item>Column 1: keypoint R color component value</item>
        /// <item>Column 2: keypoint G color component value</item>
        /// <item>Column 3: keypoint B color component value</item>
        /// <item>Column 4: keypoint alpha component value</item>
        /// </list>
        /// </para>
        /// <para>The array returned will be a <i>copy</i> of the internal data only. It cannot be used to alter the internal color table! 
        /// In order to modify the color table, one must query the table, alter it outside and store it back.</para></remarks>
        /// <seealso cref="SetData(ILInArray{float})"/>
        public ILRetArray<float> Data {
            get {
                return m_map;
            }
            set {
                SetData(value); 
            }
        }
        /// <summary>
        /// Sets internal data for color indices
        /// </summary>
        /// <remarks><para><paramref name="data"/> is a matrix with 5 columns, elements in range [0...1] 
        /// <list type="number">
        /// <item>Column 0: keypoint position</item>
        /// <item>Column 1: keypoint R color component value</item>
        /// <item>Column 2: keypoint G color component value</item>
        /// <item>Column 3: keypoint B color component value</item>
        /// <item>Column 4: keypoint alpha component value</item>
        /// </list>
        /// </para>
        /// <para>Calling this function may fire a <see cref="Changed"/> event.</para>
        /// </remarks>
        public void SetData(ILInArray<float> data) {
            using (ILScope.Enter(data)) {
                if (!object.Equals(data, null) && !data.IsEmpty && data.IsMatrix && data.Size[0] > 1 && data.Size[1] == 5) {
                    m_map.a = data.C;
                    OnChanged();
                } else {
                    throw new Exceptions.ILArgumentException("Data matrix of size [n x 5] expected, n > 1. Found: "
                        + (object.Equals(data, null) ? "null" : data.S.ToString()));
                }
            }
        }
        #endregion

        #region constructor
        /// <summary>
        /// construct new colormap, based on ILNumerics default
        /// </summary>
        internal ILColormap() {
            m_type = Colormaps.ILNumerics;
            m_map.a = MapCreator.CreateMap(m_type);
        }
        /// <summary>
        ///  create specific colormap
        /// </summary>
        /// <param name="map"></param>
        public ILColormap(Colormaps map) {
            m_type = map;
            m_map.a = MapCreator.CreateMap(m_type);
        }
        /// <summary>
        /// Creates a new colormap based on predefined colors
        /// </summary>
        /// <param name="colors">Matrix with keypoint values as rows, 
        /// 5 columns each: keypoint position (float range), R, G, B, A components in range[0...1]</param>
        public ILColormap(ILInArray<float> colors) {
            using (ILScope.Enter(colors)) {
                if (ILMath.isnull(colors))
                    throw new Exceptions.ILArgumentException("colors argument may not be null");
                ILArray<int> ind = 1; 
                ILArray<float> sorted = ILMath.sort(colors[":;0"], Indices: ind); 
                m_map.a = colors[ind,":"];
                m_type = Colormaps.ILNumerics;
            }
        }
        private ILColormap(ILColormap source) {
            m_map = source.m_map.C;
            m_type = source.m_type;
            m_synchedHashCode = source.GetHashCode();
            m_synchedVersion = source.Version; 
        }
        #endregion

        #region public interface
        /// <summary>
        /// Implicitly converts a Colormaps enumeration value to a discrete colormap
        /// </summary>
        /// <param name="val">Colormaps enum value</param>
        /// <returns>new colormap object</returns>
        public static implicit operator ILColormap(Colormaps val) {
            return new ILColormap(val); 
        }
        internal ILColormap Synchronize(ILColormap target) {
            ILColormap ret = target;
            if (target == null) {
                ret = new ILColormap(this);
            } else if (target.m_synchedVersion != m_version || target.m_synchedHashCode != GetHashCode()) {
                target.m_synchedHashCode = GetHashCode(); 
                target.m_synchedVersion = m_version;
                target.m_map = m_map.C;
                target.m_type = m_type;
            }
            return ret;
        }
        internal ILColormap Copy() {
            return new ILColormap(this);
        }
        /// <summary>
        /// Map single value within colormap data range to color 
        /// </summary>
        /// <param name="val">Value to map, range of this colormap data range (i.e. column 0 in <see cref="Data"/> matrix)</param>
        /// <param name="outOfRangeAndNaNsValue">[optional] Replacement value if no valid color could be mapped, default: black</param>
        /// <param name="minMaxRange">[optional] If null: val is mapped to the colormap from range 0..1; otherwise, the range for val is taken from minMaxRange</param>
        /// <returns>Color as Vector4 with mapped RGBA (XYZW) values, interpolated according to this colormap</returns>
        public Vector4 Map(float val, Tuple<float,float> minMaxRange = null, Vector4? outOfRangeAndNaNsValue = null) {
            Vector4 ooRVal = outOfRangeAndNaNsValue.HasValue ? outOfRangeAndNaNsValue.Value : new Vector4(0, 0, 0, 1);
            if (float.IsNaN(val)) {
                return ooRVal; 
            }
            float min, max;
            if (minMaxRange == null) {
                min = 0;
                max = 1;
            } else {
                min = minMaxRange.Item1; 
                max = minMaxRange.Item2; 
            }
            if (val <= min) {
                return new Vector4(
                    m_map.GetValue(0, 1),
                    m_map.GetValue(0, 2),
                    m_map.GetValue(0, 3),
                    m_map.GetValue(0, 4));
            }
            if (val >= max) {
                return new Vector4(
                    m_map.GetValue(m_map.S[0] - 1, 1),
                    m_map.GetValue(m_map.S[0] - 1, 2),
                    m_map.GetValue(m_map.S[0] - 1, 3),
                    m_map.GetValue(m_map.S[0] - 1, 4));
            }
            val = (val - min) / (max - min); // val -> 0..1 range
            
            int ind = (int)ILMath.find(m_map[":;0"] > val)[0] - 1;
            System.Diagnostics.Debug.Assert(ind < m_map.S[0] - 1); 
            // val -> colormap data range
            float mapMin = m_map.GetValue(0); 
            float mapMax = m_map.GetValue(m_map.S[0]-1,0); 
            val = mapMin + val * (mapMax - mapMin); 
            val = val - m_map.GetValue(ind,0);
            System.Diagnostics.Debug.Assert(val >= 0 && val <= 1);
            Vector4 c1 = new Vector4(
                    m_map.GetValue(ind, 1),
                    m_map.GetValue(ind, 2),
                    m_map.GetValue(ind, 3),
                    m_map.GetValue(ind, 4));
            Vector4 c2 = new Vector4(
                    m_map.GetValue(ind+1, 1),
                    m_map.GetValue(ind+1, 2),
                    m_map.GetValue(ind+1, 3),
                    m_map.GetValue(ind+1, 4));
            Vector4 ret = c1 + (c2 - c1) / (m_map.GetValue(ind+1,0) - m_map.GetValue(ind,0)) * val; 
            return ret; 
        }

        /// <summary>
        /// Maps all elements in A to interpolated colors from this colormap
        /// </summary>
        /// <param name="A">Array with elements to map, values will be taken in order of storage in A and lined up by their linear indices</param>
        /// <param name="dataRange">[optional] If given, dataRange marks the upper and lower limit of the color mapping data range, if null: taken from A.GetLimits()</param>
        /// <param name="outOfRangeAndNaNsValue">[optional] Color value assigned to those values in A, which do not fit inside the range given by <paramref name="dataRange"/>. Default: Color.Black</param>
        /// <returns>Colors as matrix, the i-th row represents the color from the i-th element of A (linear indexing) as RGBA quadrupel (RGB components and alpha).</returns>
        public ILRetArray<float> Map(ILInArray<float> A, Tuple<float, float> dataRange = null, Vector4? outOfRangeAndNaNsValue = null) {
            using (ILScope.Enter(A)) {
                if (ILMath.isnullorempty(A)) {
                    return ILMath.empty<float>(0,4); 
                }
                ILArray<float> ret = 1;
                Vector4 ooRVal = outOfRangeAndNaNsValue.HasValue ? outOfRangeAndNaNsValue.Value : new Vector4(0, 0, 0, 1);

                float minVal = 0, maxVal = 0;
                if (dataRange == null) {
                    if (!A.GetLimits(out minVal, out maxVal)) {
                        minVal = 0; maxVal = 1; 
                    }
                } else {
                    minVal = dataRange.Item1;
                    maxVal = dataRange.Item2;
                }
                if (minVal == maxVal) {
                    minVal = m_map.GetValue(0);
                    maxVal = (float)m_map["end;0"];
                    //// special case: all constant / nans 
                    //ILArray<float> col = new float[] { ooRVal.X, ooRVal.Y, ooRVal.Z, ooRVal.W };
                    //return ILMath.repmat<float>(col.T, ret.Size[0], 1);
                }
                // we keep m_map sorted by x vals, so here we must sort A only 
                ILArray<int> indices = 1;
                ILArray<float> AVals = ILMath.sort(A[":"], indices); 
                // map data to color range 
                AVals.a = (AVals - minVal) / (maxVal - minVal); 
                AVals.a = AVals * (m_map["end;0"] - m_map[0]) + m_map[0]; 
                
                int posA = 0, posM = 0;
                int lenA = AVals.S.NumberOfElements, lenM = m_map.S[0]; 
                ILArray<int> startBlocks = ILMath.zeros<int>(lenA,1); 
                float[] AArr = AVals.GetArrayForRead(); 
                float[] MArr = m_map.GetArrayForRead(); 
                // starting .. until inside map range
                while (posA < lenA) {
                    if (float.IsNaN(AArr[posA])) {
                        // done! (sort puts nans to the end) 
                        startBlocks.SetRange(-1, ILMath.r(posA, ILMath.end));
                        posA = lenA;
                        break;
                    }
                    if (AArr[posA] < MArr[0]) {
                        startBlocks.SetValue(0, posA); 
                        posA++; 
                    } else {
                        break; 
                    }
                }
                while (posA < lenA) {
                    if (float.IsNaN(AArr[posA])) {
                        // done! 
                        startBlocks.SetRange(-1, ILMath.r(posA, ILMath.end)); 
                        posA = lenA; 
                        break; 
                    }
                    while (true) {
                        if (posM < lenM) {
                            if (AArr[posA] >= MArr[posM]) {
                                posM++;
                            } else {
                                startBlocks.SetValue(posM - 1, posA); 
                                posA++; 
                                break; 
                            }
                        } else {
                            // end of map, go back to last element 
                            posM -= 1; 
                            // all remaining indices match end of map or are out of range
                            if (posA < lenA) {
                                startBlocks.SetRange(posM, ILMath.r(posA, ILMath.end));
                                posA = lenA;
                            }
                            break;
                        }
                    }
                }
                ILLogical nans = startBlocks == -1; 
                AVals[nans] = float.NaN;  // <- what is this for? shouldn't AVals[nan] be NaN anyway ? 
                startBlocks[nans] = 0; 
                ILArray<int> add = ILMath.ones<int>(startBlocks.S[0],1); 
                add[startBlocks == lenM - 1] = 0; 
                ILArray<float> starts = m_map[startBlocks,"1:4"]; 
                ILArray<float> dist = AVals - m_map[startBlocks,0];
                ILArray<float> denom = m_map[startBlocks + add, 0] - m_map[startBlocks, 0]; 
                dist[denom == 0] = 0; 
                dist[dist < 0] = 0; 
                
                denom[denom == 0] = 1; 
                ILArray<float> fact = (m_map[startBlocks + add, "1:4"] - starts) / denom; 
 
                ret = starts + fact * dist;

                if (ILMath.anyall(nans)) {
                    ret[nans, ":"] = ILMath.repmat(ILMath.array<float>(ooRVal.X, ooRVal.Y, ooRVal.Z, ooRVal.W).T, 1, nans.Length);
                }
                ret[indices, ":"] = ret; 
                return ret;
            }
        }
        #endregion

        #region create maps
        [System.Security.SecuritySafeCritical]
        internal class MapCreator : ILNumerics.ILMath {
            /// <summary>
            /// create colormap
            /// </summary>
            /// <param name="map">colormap type to create</param>
            /// <returns>colormap data matrix, the size depends on the type of the colormap</returns>
            internal static ILRetArray<float> CreateMap(Colormaps map) {
                using (ILScope.Enter()) {
                    ILArray<float> ret = empty<float>();
                    switch (map) {
                        case Colormaps.Autumn:
                            #region autumn
                            ret.a = new float[,] {
{0f,1f,0f,0f,1f},
{1f,1f,1f,0f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Bone:
                            #region bone
                            ret.a = new float[,] {
{0f,0f,0f,0.0052083f,1f},
{0.35938f,0.30556f,0.30556f,0.42535f,1f},
{0.73438f,0.63889f,0.75868f,0.76389f,1f},
{1f,1f,1f,1f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Colorcube:
                            #region colorcube
                            ret.a = new float[,] {
{0f,0.33333f,0.33333f,0f,1f},
{0.03125f,0.33333f,0.66667f,0f,1f},
{0.046875f,0.33333f,1f,0f,1f},
{0.078125f,0.66667f,0.66667f,0f,1f},
{0.09375f,0.66667f,1f,0f,1f},
{0.125f,1f,0.66667f,0f,1f},
{0.14063f,1f,1f,0f,1f},
{0.17188f,0f,0.66667f,0.5f,1f},
{0.1875f,0f,1f,0.5f,1f},
{0.23438f,0.33333f,0.66667f,0.5f,1f},
{0.25f,0.33333f,1f,0.5f,1f},
{0.29688f,0.66667f,0.66667f,0.5f,1f},
{0.3125f,0.66667f,1f,0.5f,1f},
{0.35938f,1f,0.66667f,0.5f,1f},
{0.375f,1f,1f,0.5f,1f},
{0.40625f,0f,0.66667f,1f,1f},
{0.42188f,0f,1f,1f,1f},
{0.46875f,0.33333f,0.66667f,1f,1f},
{0.48438f,0.33333f,1f,1f,1f},
{0.53125f,0.66667f,0.66667f,1f,1f},
{0.54688f,0.66667f,1f,1f,1f},
{0.57813f,1f,0.33333f,1f,1f},
{0.59375f,1f,0.66667f,1f,1f},
{0.67188f,0.83333f,0f,0f,1f},
{0.6875f,1f,0f,0f,1f},
{0.76563f,0f,0.83333f,0f,1f},
{0.78125f,0f,1f,0f,1f},
{0.85938f,0f,0f,0.83333f,1f},
{0.875f,0f,0f,1f,1f},
{1f,1f,1f,1f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Cool:
                            #region cool
                            ret.a = new float[,] {
{0f,0f,1f,1f,1f},
{1f,1f,0f,1f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Copper:
                            #region copper
                            ret.a = new float[,] {
{0f,0f,0f,0f,1f},
{0.78125f,0.97222f,0.6076f,0.38694f,1f},
{0.79688f,0.99206f,0.62f,0.39484f,1f},
{1f,1f,0.7812f,0.4975f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Flag:
                            #region flag
                            ret.a = new float[,] {
{0f,1f,0f,0f,1f},
{0.015625f,1f,0f,0f,1f},
{0.03125f,1f,1f,1f,1f},
{0.046875f,0f,0f,1f,1f},
{0.0625f,0f,0f,0f,1f},
{0.078125f,1f,0f,0f,1f},
{0.09375f,1f,1f,1f,1f},
{0.10938f,0f,0f,1f,1f},
{0.125f,0f,0f,0f,1f},
{0.14063f,1f,0f,0f,1f},
{0.15625f,1f,1f,1f,1f},
{0.17188f,0f,0f,1f,1f},
{0.1875f,0f,0f,0f,1f},
{0.20313f,1f,0f,0f,1f},
{0.21875f,1f,1f,1f,1f},
{0.23438f,0f,0f,1f,1f},
{0.25f,0f,0f,0f,1f},
{0.26563f,1f,0f,0f,1f},
{0.28125f,1f,1f,1f,1f},
{0.29688f,0f,0f,1f,1f},
{0.3125f,0f,0f,0f,1f},
{0.32813f,1f,0f,0f,1f},
{0.34375f,1f,1f,1f,1f},
{0.35938f,0f,0f,1f,1f},
{0.375f,0f,0f,0f,1f},
{0.39063f,1f,0f,0f,1f},
{0.40625f,1f,1f,1f,1f},
{0.42188f,0f,0f,1f,1f},
{0.4375f,0f,0f,0f,1f},
{0.45313f,1f,0f,0f,1f},
{0.46875f,1f,1f,1f,1f},
{0.48438f,0f,0f,1f,1f},
{0.5f,0f,0f,0f,1f},
{0.51563f,1f,0f,0f,1f},
{0.53125f,1f,1f,1f,1f},
{0.54688f,0f,0f,1f,1f},
{0.5625f,0f,0f,0f,1f},
{0.57813f,1f,0f,0f,1f},
{0.59375f,1f,1f,1f,1f},
{0.60938f,0f,0f,1f,1f},
{0.625f,0f,0f,0f,1f},
{0.64063f,1f,0f,0f,1f},
{0.65625f,1f,1f,1f,1f},
{0.67188f,0f,0f,1f,1f},
{0.6875f,0f,0f,0f,1f},
{0.70313f,1f,0f,0f,1f},
{0.71875f,1f,1f,1f,1f},
{0.73438f,0f,0f,1f,1f},
{0.75f,0f,0f,0f,1f},
{0.76563f,1f,0f,0f,1f},
{0.78125f,1f,1f,1f,1f},
{0.79688f,0f,0f,1f,1f},
{0.8125f,0f,0f,0f,1f},
{0.82813f,1f,0f,0f,1f},
{0.84375f,1f,1f,1f,1f},
{0.85938f,0f,0f,1f,1f},
{0.875f,0f,0f,0f,1f},
{0.89063f,1f,0f,0f,1f},
{0.90625f,1f,1f,1f,1f},
{0.92188f,0f,0f,1f,1f},
{0.9375f,0f,0f,0f,1f},
{0.95313f,1f,0f,0f,1f},
{0.96875f,1f,1f,1f,1f},
{1f,0f,0f,0f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Gray:
                            #region gray
                            ret.a = new float[,] {
{0f,0f,0f,0f,1f},
{1f,1f,1f,1f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Hot:
                            #region hot
                            ret.a = new float[,] {
{0f,0.041667f,0f,0f,1f},
{0.35938f,0.95833f,0f,0f,1f},
{0.73438f,1f,0.95833f,0f,1f},
{1f,1f,1f,1f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Hsv:
                            ret.a = new float[,] {
{0f,1f,0f,0f,1f},
{0.15625f,1f,0.84375f,0f,1f},
{0.17188f,1f,0.9375f,0f,1f},
{0.32813f,0.125f,1f,0f,1f},
{0.34375f,0.03125f,1f,0f,1f},
{0.5f,0f,1f,0.90625f,1f},
{0.65625f,0f,0.15625f,1f,1f},
{0.67188f,0f,0.0625f,1f,1f},
{0.82813f,0.875f,0f,1f,1f},
{0.84375f,0.96875f,0f,1f,1f},
{1f,1f,0f,0.09375f,1f},
};
                            break;
                        case Colormaps.ILNumerics:
                            ret.a = zeros<float>(5, 128);
                            int tmp;
                            ILColorProvider cprov = new ILColorProvider(0.0f, 0.5f, 1.0f);
                            ILArray<float> rng = linspace<float>(ILColorProvider.MAXHUEVALUE, 0.0, 128);
                            for (int i = 0; i < 128; i++) {
                                tmp = cprov.H2RGB((float)rng[i]);
                                ret.SetValue((float)(tmp >> 16 & 255), 1, i);
                                ret.SetValue((float)(tmp >> 8 & 255), 2, i);
                                ret.SetValue((float)(tmp & 255), 3, i);
                            }
                            ret.a /= 255.0f;
                            ret["0;:"] = linspace<float>(0f,1f,128); 
                            ret["4;:"] = 1; 
                            break;
                        case Colormaps.Jet:
                            #region jet
                            ret.a = new float[,] {
{0f,0f,0f,0.5625f,1f},
{0.10938f,0f,0f,0.9375f,1f},
{0.35938f,0f,0.9375f,1f,1f},
{0.60938f,0.9375f,1f,0.0625f,1f},
{0.85938f,1f,0.0625f,0f,1f},
{1f,0.5f,0f,0f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Lines:
                            #region lines
                            ret.a = new float[,] {
{0f,0f,0f,1f,1f},
{0.015625f,1f,0f,0f,1f},
{0.03125f,0f,0.5f,0f,1f},
{0.046875f,1f,0f,0f,1f},
{0.0625f,0f,0.75f,0.75f,1f},
{0.078125f,0.75f,0f,0.75f,1f},
{0.09375f,0.75f,0.75f,0f,1f},
{0.10938f,0.25f,0.25f,0.25f,1f},
{0.125f,0f,0f,1f,1f},
{0.14063f,0f,0.5f,0f,1f},
{0.15625f,1f,0f,0f,1f},
{0.17188f,0f,0.75f,0.75f,1f},
{0.1875f,0.75f,0f,0.75f,1f},
{0.20313f,0.75f,0.75f,0f,1f},
{0.21875f,0.25f,0.25f,0.25f,1f},
{0.23438f,0f,0f,1f,1f},
{0.25f,0f,0.5f,0f,1f},
{0.26563f,1f,0f,0f,1f},
{0.28125f,0f,0.75f,0.75f,1f},
{0.29688f,0.75f,0f,0.75f,1f},
{0.3125f,0.75f,0.75f,0f,1f},
{0.32813f,0.25f,0.25f,0.25f,1f},
{0.34375f,0f,0f,1f,1f},
{0.35938f,0f,0.5f,0f,1f},
{0.375f,1f,0f,0f,1f},
{0.39063f,0f,0.75f,0.75f,1f},
{0.40625f,0.75f,0f,0.75f,1f},
{0.42188f,0.75f,0.75f,0f,1f},
{0.4375f,0.25f,0.25f,0.25f,1f},
{0.45313f,0f,0f,1f,1f},
{0.46875f,0f,0.5f,0f,1f},
{0.48438f,1f,0f,0f,1f},
{0.5f,0f,0.75f,0.75f,1f},
{0.51563f,0.75f,0f,0.75f,1f},
{0.53125f,0.75f,0.75f,0f,1f},
{0.54688f,0.25f,0.25f,0.25f,1f},
{0.5625f,0f,0f,1f,1f},
{0.57813f,0f,0.5f,0f,1f},
{0.59375f,1f,0f,0f,1f},
{0.60938f,0f,0.75f,0.75f,1f},
{0.625f,0.75f,0f,0.75f,1f},
{0.64063f,0.75f,0.75f,0f,1f},
{0.65625f,0.25f,0.25f,0.25f,1f},
{0.67188f,0f,0f,1f,1f},
{0.6875f,0f,0.5f,0f,1f},
{0.70313f,1f,0f,0f,1f},
{0.71875f,0f,0.75f,0.75f,1f},
{0.73438f,0.75f,0f,0.75f,1f},
{0.75f,0.75f,0.75f,0f,1f},
{0.76563f,0.25f,0.25f,0.25f,1f},
{0.78125f,0f,0f,1f,1f},
{0.79688f,0f,0.5f,0f,1f},
{0.8125f,1f,0f,0f,1f},
{0.82813f,0f,0.75f,0.75f,1f},
{0.84375f,0.75f,0f,0.75f,1f},
{0.85938f,0.75f,0.75f,0f,1f},
{0.875f,0.25f,0.25f,0.25f,1f},
{0.89063f,0f,0f,1f,1f},
{0.90625f,0f,0.5f,0f,1f},
{0.92188f,1f,0f,0f,1f},
{0.9375f,0f,0.75f,0.75f,1f},
{0.95313f,0.75f,0f,0.75f,1f},
{0.96875f,0.75f,0.75f,0f,1f},
{1f,0f,0f,1f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Pink:
                            #region pink
                            ret.a = new float[,] {
{0f,0.11785f,0f,0f,1f},
{0.015625f,0.11785f,0f,0f,1f},
{0.03125f,0.19586f,0.10287f,0.10287f,1f},
{0.046875f,0.25066f,0.14548f,0.14548f,1f},
{0.0625f,0.29547f,0.17817f,0.17817f,1f},
{0.078125f,0.33432f,0.20574f,0.20574f,1f},
{0.09375f,0.36911f,0.23002f,0.23002f,1f},
{0.10938f,0.40089f,0.25198f,0.25198f,1f},
{0.125f,0.43033f,0.27217f,0.27217f,1f},
{0.14063f,0.45788f,0.29096f,0.29096f,1f},
{0.15625f,0.48387f,0.30861f,0.30861f,1f},
{0.17188f,0.50853f,0.3253f,0.3253f,1f},
{0.1875f,0.53204f,0.34118f,0.34118f,1f},
{0.20313f,0.55456f,0.35635f,0.35635f,1f},
{0.21875f,0.5762f,0.3709f,0.3709f,1f},
{0.23438f,0.59706f,0.3849f,0.3849f,1f},
{0.25f,0.61721f,0.39841f,0.39841f,1f},
{0.26563f,0.63673f,0.41148f,0.41148f,1f},
{0.28125f,0.65566f,0.42414f,0.42414f,1f},
{0.29688f,0.67407f,0.43644f,0.43644f,1f},
{0.3125f,0.69198f,0.4484f,0.4484f,1f},
{0.32813f,0.70944f,0.46004f,0.46004f,1f},
{0.34375f,0.72648f,0.4714f,0.4714f,1f},
{0.35938f,0.74313f,0.4825f,0.4825f,1f},
{0.375f,0.75942f,0.49334f,0.49334f,1f},
{0.39063f,0.76636f,0.51755f,0.50395f,1f},
{0.40625f,0.77323f,0.54067f,0.51434f,1f},
{0.42188f,0.78004f,0.56285f,0.52453f,1f},
{0.4375f,0.7868f,0.58418f,0.53452f,1f},
{0.45313f,0.79349f,0.60477f,0.54433f,1f},
{0.46875f,0.80013f,0.62467f,0.55397f,1f},
{0.48438f,0.80672f,0.64396f,0.56344f,1f},
{0.5f,0.81325f,0.66269f,0.57275f,1f},
{0.51563f,0.81973f,0.6809f,0.58191f,1f},
{0.53125f,0.82616f,0.69864f,0.59094f,1f},
{0.54688f,0.83254f,0.71594f,0.59982f,1f},
{0.5625f,0.83887f,0.73283f,0.60858f,1f},
{0.57813f,0.84515f,0.74934f,0.61721f,1f},
{0.59375f,0.85139f,0.76549f,0.62573f,1f},
{0.60938f,0.85758f,0.78131f,0.63413f,1f},
{0.625f,0.86373f,0.79682f,0.64242f,1f},
{0.64063f,0.86984f,0.81203f,0.6506f,1f},
{0.65625f,0.8759f,0.82696f,0.65868f,1f},
{0.67188f,0.88192f,0.84163f,0.66667f,1f},
{0.6875f,0.8879f,0.85604f,0.67456f,1f},
{0.70313f,0.89384f,0.87022f,0.68236f,1f},
{0.71875f,0.89974f,0.88416f,0.69007f,1f},
{0.73438f,0.9056f,0.8979f,0.69769f,1f},
{0.75f,0.91142f,0.91142f,0.70523f,1f},
{0.76563f,0.91721f,0.91721f,0.72717f,1f},
{0.78125f,0.92296f,0.92296f,0.74846f,1f},
{0.79688f,0.92867f,0.92867f,0.76916f,1f},
{0.8125f,0.93435f,0.93435f,0.78931f,1f},
{0.82813f,0.94f,0.94f,0.80897f,1f},
{0.84375f,0.94561f,0.94561f,0.82816f,1f},
{0.85938f,0.95119f,0.95119f,0.84691f,1f},
{0.875f,0.95674f,0.95674f,0.86526f,1f},
{0.89063f,0.96225f,0.96225f,0.88323f,1f},
{0.90625f,0.96773f,0.96773f,0.90084f,1f},
{0.92188f,0.97319f,0.97319f,0.91811f,1f},
{0.9375f,0.97861f,0.97861f,0.93506f,1f},
{0.95313f,0.984f,0.984f,0.95171f,1f},
{0.96875f,0.98936f,0.98936f,0.96808f,1f},
{1f,1f,1f,1f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Prism:
                            #region prism
                            ret.a = new float[,] {
{0f,1f,0f,0f,1f},
{0.03125f,1f,0.5f,0f,1f},
{0.046875f,1f,1f,0f,1f},
{0.0625f,0f,1f,0f,1f},
{0.078125f,0f,0f,1f,1f},
{0.09375f,0.66667f,0f,1f,1f},
{0.125f,1f,0.5f,0f,1f},
{0.14063f,1f,1f,0f,1f},
{0.15625f,0f,1f,0f,1f},
{0.17188f,0f,0f,1f,1f},
{0.1875f,0.66667f,0f,1f,1f},
{0.21875f,1f,0.5f,0f,1f},
{0.23438f,1f,1f,0f,1f},
{0.25f,0f,1f,0f,1f},
{0.26563f,0f,0f,1f,1f},
{0.28125f,0.66667f,0f,1f,1f},
{0.3125f,1f,0.5f,0f,1f},
{0.32813f,1f,1f,0f,1f},
{0.34375f,0f,1f,0f,1f},
{0.35938f,0f,0f,1f,1f},
{0.375f,0.66667f,0f,1f,1f},
{0.40625f,1f,0.5f,0f,1f},
{0.42188f,1f,1f,0f,1f},
{0.4375f,0f,1f,0f,1f},
{0.45313f,0f,0f,1f,1f},
{0.46875f,0.66667f,0f,1f,1f},
{0.5f,1f,0.5f,0f,1f},
{0.51563f,1f,1f,0f,1f},
{0.53125f,0f,1f,0f,1f},
{0.54688f,0f,0f,1f,1f},
{0.5625f,0.66667f,0f,1f,1f},
{0.59375f,1f,0.5f,0f,1f},
{0.60938f,1f,1f,0f,1f},
{0.625f,0f,1f,0f,1f},
{0.64063f,0f,0f,1f,1f},
{0.65625f,0.66667f,0f,1f,1f},
{0.6875f,1f,0.5f,0f,1f},
{0.70313f,1f,1f,0f,1f},
{0.71875f,0f,1f,0f,1f},
{0.73438f,0f,0f,1f,1f},
{0.75f,0.66667f,0f,1f,1f},
{0.78125f,1f,0.5f,0f,1f},
{0.79688f,1f,1f,0f,1f},
{0.8125f,0f,1f,0f,1f},
{0.82813f,0f,0f,1f,1f},
{0.84375f,0.66667f,0f,1f,1f},
{0.875f,1f,0.5f,0f,1f},
{0.89063f,1f,1f,0f,1f},
{0.90625f,0f,1f,0f,1f},
{0.92188f,0f,0f,1f,1f},
{0.9375f,0.66667f,0f,1f,1f},
{0.96875f,1f,0.5f,0f,1f},
{1f,0f,1f,0f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Spring:
                            #region spring
                            ret.a = new float[,] {
{0f,1f,0f,1f,1f},
{1f,1f,1f,0f,1f},
};
                            #endregion
                            break;
                        case Colormaps.Summer:
                            #region summer
                            ret.a = new float[,] {
{0f,0f,0.5f,0.4f,1f},
{1f,1f,1f,0.4f,1f},
};
                            #endregion
                            break;
                        case Colormaps.White:
                            #region white
                            ret.a = new float[,] {
{0f,1f,1f,1f,1f},
{1f,1f,1f,1f,1f},
                            };
                            #endregion
                            break;
                        case Colormaps.Winter:
                            #region winter
                            ret.a = new float[,] {
{0f,0f,0f,1f,1f},
{1f,0f,1f,0.5f,1f},
};
                            #endregion
                            break;
                    }
                    return ret.T;
                }
            }
		}
        #endregion
    }
}
