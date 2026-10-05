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

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public class ILPlotCubeDataGroup : ILGroup, IILAxisDataProvider {

        #region attributes
        #endregion

        #region properties
        public ILLimits Limits { get; private set; }
        internal bool AutoScaleOnAdd { get; set; }
        public ILScaleModes ScaleModes { get; private set; }
        #endregion

        private ILPlotCubeDataGroup() {
            // we want the limits to be local to the driver...
            Limits = new ILLimits();
            Limits.Changed += (send, args) => {
                OnPropertyChanged("Limits"); 
            };
        }
        internal ILPlotCubeDataGroup(ILPlotCubeDataGroup source) : base(source) {
            Limits = source.Limits.Clone(); 
            AutoScaleOnAdd = source.AutoScaleOnAdd;
            ScaleModes = source.ScaleModes.Copy();
            ScaleModes.PropertyChanged += (sender, args) => { if (AutoScaleOnAdd) Reset(); }; 
        }
        public ILPlotCubeDataGroup(object tag = null) : base(tag) {
            Limits = new ILLimits();
            AutoScaleOnAdd = true;
            ScaleModes = new ILScaleModes();
            ScaleModes.PropertyChanged += (sender,args) => { if (AutoScaleOnAdd) Reset(); };
            Clipping = new ILClipParams(); 
            Children.NodeAdded += (sender, args) => { if (AutoScaleOnAdd) Reset(); };
        }
        internal override ILNode Copy() {
            return new ILPlotCubeDataGroup(this); 
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILPlotCubeDataGroup(); 
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILPlotCubeDataGroup ret = (ILPlotCubeDataGroup)base.Synchronize(copy, syncParams);
            if (copy == null) {
                Limits.Changed += (s,arg) => {
                    OnPropertyChanged("Limits"); 
                }; 
            }
            if (copy == null || copy.SynchedVersion != Version) {
                ret.AutoScaleOnAdd = AutoScaleOnAdd;
                ret.Limits.Set(Limits.Min,Limits.Max);
                ret.ScaleModes = ScaleModes.Copy();
                ret.ScaleModes.PropertyChanged += (sender, args) => { if (ret.AutoScaleOnAdd) ret.Reset(); };
            }
            return ret;
        }
        public override T Add<T>(T node, object tag = null, bool shareBuffers = true) {
            T ret = base.Add<T>(node, tag, shareBuffers);
            if (AutoScaleOnAdd) {
                Reset();
            }
            return ret;
        }
        /// <summary>
        /// Reset current view limits to show all content
        /// </summary>
        public void Reset() {
            Vector3? bounds = ScaleModes.GetBoundsForLimits(); 
            ILLimits lim = GetLimits(bounds);
            #region some heuristics to ensure non-empty volumes
            if (Math.Abs(lim.DepthF) < ILMath.epsf) {
                float min, max;
                ILHelper.EnsurePlotCubeExtend(lim.ZMin, out min, out max);
                lim.ZMin = min; lim.ZMax = max;
            }
            if (Math.Abs(lim.WidthF) < ILMath.epsf) {
                float min, max;
                ILHelper.EnsurePlotCubeExtend(lim.XMin, out min, out max);
                lim.XMin = min; lim.XMax = max;
            }
            if (Math.Abs(lim.HeightF) < ILMath.epsf) {
                float min, max;
                ILHelper.EnsurePlotCubeExtend(lim.YMin, out min, out max);
                lim.YMin = min; lim.YMax = max;
            }
            #endregion
            if (bounds.HasValue) { 
                // log mode
                if (!float.IsNaN(bounds.Value.X)) {
                    lim.XMin = (float)Math.Log10(lim.XMin);
                    lim.XMax = (float)Math.Log10(lim.XMax);
                }
                if (!float.IsNaN(bounds.Value.Y)) {
                    lim.YMin = (float)Math.Log10(lim.YMin);
                    lim.YMax = (float)Math.Log10(lim.YMax);
                }
                if (!float.IsNaN(bounds.Value.Z)) {
                    lim.ZMin = (float)Math.Log10(lim.ZMin);
                    lim.ZMax = (float)Math.Log10(lim.ZMax);
                }
            }
            lim.Update(lim.CenterF, 1.05f);
            Limits.Set(lim.Min, lim.Max);
        }
        public override T Insert<T>(int index, T node, object tag = null) {
            T ret = base.Insert<T>(index, node, tag);
            if (AutoScaleOnAdd) {
                Reset();
            }
            return ret;
        }
        protected override bool BeginVisit(ILRenderParameter parameter) {
            Vector4 logState = ScaleModes.GetLogState();
            parameter.LogState.Push(logState);
            if (parameter.CurrentPassCount == 0) {
                Vector3 lengths = new Vector3(
                    1 / (Limits.WidthF == 0 ? 1 : Limits.WidthF),
                    1 / (Limits.HeightF == 0 ? 1 : Limits.HeightF),
                    1 / (Limits.DepthF == 0 ? 1 : Limits.DepthF));
                //Vector3 lengths = new Vector3(
                //    1 / (Limits.WidthF == 0 ? 1 : (logState.X != 0 ? Math.Log10( Limits.WidthF) : Limits.WidthF)),
                //    1 / (Limits.HeightF == 0 ? 1 : (logState.Y != 0 ? Math.Log10( Limits.HeightF) : Limits.HeightF)),
                //    1 / (Limits.DepthF == 0 ? 1 : (logState.Z != 0 ? Math.Log10( Limits.DepthF) : Limits.DepthF))); 
                Vector3 mins = new Vector3(-Limits.XMin, -Limits.YMin, -Limits.ZMin);
                Transform = Limits.IsEmpty ? Matrix4.Identity : Matrix4.ScaleTransform(lengths.X, lengths.Y, lengths.Z)
                            * Matrix4.Translation(-Limits.XMin, -Limits.YMin, -Limits.ZMin);
                // handle clipping
                if (Clipping != null) {
                    Clipping.Update(Limits);
                }
            }
            return base.BeginVisit(parameter);
        }
        protected override void EndVisit(ILRenderParameter parameter) {
            parameter.LogState.Pop(); 
            base.EndVisit(parameter);
        }
        //internal void GetLogLimits(AxisNames axisName, out float min, out float max, ref int configureVersion) {
        //    if (m_configureVersion == configureVersion) {
        //        min = m_lastLogMin[(int)axisName];
        //        max = m_lastLogMax[(int)axisName]; 
        //        return; 
        //    }
        //    configureVersion = m_configureVersion;
        //    m_lastLogMin = new Vector3(ScaleModes[AxisNames.XAxis] == AxisScale.Logarithmic ? float.MaxValue : float.NaN,
        //                               ScaleModes[AxisNames.YAxis] == AxisScale.Logarithmic ? float.MaxValue : float.NaN,
        //                               ScaleModes[AxisNames.ZAxis] == AxisScale.Logarithmic ? float.MaxValue : float.NaN);
        //    m_lastLogMax = new Vector3(ScaleModes[AxisNames.XAxis] == AxisScale.Logarithmic ? float.MinValue : float.NaN,
        //                               ScaleModes[AxisNames.YAxis] == AxisScale.Logarithmic ? float.MinValue : float.NaN,
        //                               ScaleModes[AxisNames.ZAxis] == AxisScale.Logarithmic ? float.MinValue : float.NaN);
        //    Stack<Matrix4> transforms = new Stack<Matrix4>();
        //    transforms.Push(Matrix4.Identity);
        //    getLogLimitsInternal(transforms, ref m_lastLogMin, ref m_lastLogMax);
        //    if (!float.IsNaN(m_lastLogMax.X) && m_lastLogMax.X < 0) {
        //        m_lastLogMax.X = float.Epsilon;
        //    }
        //    if (!float.IsNaN(m_lastLogMax.Y) && m_lastLogMax.Y < 0) {
        //        m_lastLogMax.Y = float.Epsilon;
        //    }
        //    if (!float.IsNaN(m_lastLogMax.Z) && m_lastLogMax.Z < 0) {
        //        m_lastLogMax.Z = float.Epsilon;
        //    }
        //    if (!float.IsNaN(m_lastLogMin.X) && m_lastLogMin.X < 0) {
        //        m_lastLogMin.X = float.Epsilon;
        //    }
        //    if (!float.IsNaN(m_lastLogMin.Y) && m_lastLogMin.Y < 0) {
        //        m_lastLogMin.Y = float.Epsilon;
        //    }
        //    if (!float.IsNaN(m_lastLogMin.Z) && m_lastLogMin.Z < 0) {
        //        m_lastLogMin.Z = float.Epsilon;
        //    }
        //    if (m_lastLogMax.X == m_lastLogMin.X) {
        //        m_lastLogMax.X = m_lastLogMin.X + ILMath.epsf;
        //    }
        //    if (m_lastLogMax.Y == m_lastLogMin.Y) {
        //        m_lastLogMax.Y = m_lastLogMin.Y + ILMath.epsf;
        //    }
        //    if (m_lastLogMax.Z == m_lastLogMin.Z) {
        //        m_lastLogMax.Z = m_lastLogMin.Z + ILMath.epsf;
        //    }
        //    min = m_lastLogMin[(int)axisName];
        //    max = m_lastLogMax[(int)axisName]; 
        //    return;
        //}

        #region IILAxisDataProvider Members

        public float GetRangeMinValue(AxisNames AxisName) {
            return Limits.Min[(int)AxisName]; 
        }

        public float GetRangeMaxValue(AxisNames AxisName) {
            return Limits.Max[(int)AxisName];
        }

        public AxisScale ScaleMode(AxisNames AxisName) {
            return ScaleModes[AxisName]; 
        }

        #endregion

        //#region IILLegendDataProvider Members

        //public IDictionary<int, string> GetAllEntries(IEnumerable<int> predefined) {
        //    if (predefined != null) {
        //        return Find<ILGroup>(predicate: g => g is IILLegendItemDataProvider && predefined.Contains(g.ID))
        //                .ToDictionary<ILGroup,int, string>(
        //                                g => g.ID, 
        //                                g => (g.Tag != null) ? g.Tag.ToString() : g.GetType().Name + " " + g.ID); 
        //    } else {
        //        return Find<ILGroup>(predicate: g => g is IILLegendItemDataProvider)
        //                .ToDictionary<ILGroup, int, string>(
        //                                g => g.ID, 
        //                                g => (g.Tag != null) ? g.Tag.ToString() : g.GetType().Name + " " + g.ID);
        //    }
        //}

        //public IILLegendItemDataProvider GetEntry(int id) {
        //    return First<ILGroup>(predicate: g => g is IILLegendItemDataProvider && g.ID == id) as IILLegendItemDataProvider; 
        //}

        //#endregion
    }
}
