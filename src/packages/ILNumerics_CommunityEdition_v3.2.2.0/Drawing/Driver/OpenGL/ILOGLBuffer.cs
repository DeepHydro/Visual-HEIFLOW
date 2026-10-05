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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using OpenTK; 
using OpenTK.Graphics; 
using OpenTK.Graphics.OpenGL; 

namespace ILNumerics.Drawing {
    [System.Security.SecuritySafeCritical]
    public class ILOGLBuffer {
        public event EventHandler Deleted;
        public event EventHandler Recreated;


        public int? GLID { get; set; }
        public int DataCount { get; set; }
        public int DataLength { get; set; }
        public ILBufferBase SourceBuffer { get; set; }
        protected ConcurrentQueue<ILChangeQueueItem> m_changes = new ConcurrentQueue<ILChangeQueueItem>();
        internal ConcurrentQueue<ILChangeQueueItem> ChangeQueue {
            get {
                return m_changes; 
            }
        }

        public ILOGLBuffer(ILBufferBase buffer) {
            int id = -1;
            GL.GenBuffers(1, out id);
            GLID = id;
            SourceBuffer = buffer;
            DataCount = 0;
            DataLength = buffer.DataLength; 
            System.Diagnostics.Debug.WriteLine("Upload Buffer: {0} SourceID:{1}", buffer.BufferType, buffer.ID);
            buffer.Changed += new EventHandler<ILBufferChangedEventArgs>(buffer_Changed);
            using (ILScope.Enter()) {
                if (buffer.BufferType == BufferType.IndexBuffer) {
                    ILArray<int> IndBuff = (buffer as ILIndicesBuffer).Storage;
                    UpdateOrReplace(IndBuff); 
                } else {
                    ILArray<float> bufferArray;
                    if (buffer.BufferType == BufferType.PositionBuffer) {
                        bufferArray = (buffer as ILPositionsBuffer).Storage;
                    } else if (buffer.BufferType == BufferType.NormalBuffer) {
                        bufferArray = (buffer as ILNormalsBuffer).Storage;
                    } else {
                        System.Diagnostics.Debug.Assert(buffer.BufferType == BufferType.ColorBuffer);
                        bufferArray = (buffer as ILColorsBuffer).Storage;
                    }
                    UpdateOrReplace(bufferArray); 
                }
            }
        }
        public ILOGLBuffer() {
            int id = -1;
            GL.GenBuffers(1, out id);
            GLID = id;
        }

        public void OnRecreated() {
            if (Recreated != null)
                Recreated(this, EventArgs.Empty);
        }
        public void OnDeleted() {
            if (Deleted != null)
                Deleted(this, EventArgs.Empty);
        }
        public void PopulateChanges() {
            ILChangeQueueItem item; 
            while (m_changes.Count > 0 && m_changes.TryDequeue(out item)) {
                if (!uploadChanges2GLBuffer(item)) { // recreated, exit the loop
                    break; 
                }
            }
        }
        public void Delete() {
            System.Diagnostics.Trace.WriteLine(string.Format("Delete Buffer: {0} #{1}", this, this.GLID)); 
            
            if (GLID.HasValue) {
                int oldId = GLID.Value; 
                GL.DeleteBuffers(1,ref oldId);
                if (SourceBuffer != null) 
                    SourceBuffer.Changed -= new EventHandler<ILBufferChangedEventArgs>(buffer_Changed);
                OnDeleted(); 
            }
        }
        public void UpdateOrReplace(ILInArray<int> data) {
            using (ILScope.Enter(data)) {
                int[] arr = data.GetArrayForRead();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLID.Value);
                if (data.S.NumberOfElements > DataCount * DataLength) {
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(int) * data.S.NumberOfElements), arr, BufferUsageHint.DynamicDraw);
                    DataCount = data.S[1];
                    DataLength = data.S[0]; 
                } else {
                    GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)0, (IntPtr)(sizeof(int) * data.S.NumberOfElements), arr);
                }
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
        }
        public void UpdateOrReplace(ILInArray<float> data) {
            using (ILScope.Enter(data)) {
                float[] arr = data.GetArrayForRead();
                GL.BindBuffer(BufferTarget.ArrayBuffer, GLID.Value);
                if (data.S.NumberOfElements > DataCount * DataLength) {
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * data.S.NumberOfElements), arr, BufferUsageHint.StaticDraw);
                    DataCount = data.S[1];
                    DataLength = data.S[0];
                } else {
                    GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)(sizeof(float) * data.S.NumberOfElements), arr);
                }
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                DataCount = Math.Min(DataCount,SourceBuffer.DataCount); 
            }
        }

        #region private helper

        // this expects DataLength to stay the same! 
        private unsafe bool uploadChanges2GLBuffer(ILChangeQueueItem change) {
            //System.Diagnostics.Trace.WriteLine(string.Format("Upload Changes to Buffer: {0} #{1} Start:{2} Length:{3} First:{4}", this.SourceBuffer.BufferType, this.GLID, change.Start, change.Length,
            //                                                                                                                     (this.SourceBuffer.BufferType == BufferType.IndexBuffer) ?
            //                                                                                                                     (SourceBuffer as ILIndicesBuffer).Storage[":", ILMath.r(change.Start,change.Start+change.Length)].ToString() :
            //                                                                                                                     (SourceBuffer as ILBuffer<float>).Storage[":", ILMath.r(change.Start, change.Start + change.Length)].ToString()));
            if (change.Action != ChangeQueueActions.Update) return true; 
            using (ILScope.Enter()) {
                if (SourceBuffer.BufferType == BufferType.IndexBuffer) {
                    ILArray<int> IndBuff = (SourceBuffer as ILIndicesBuffer).Storage;
                    if (change.Length + change.Start > this.DataCount) {
                        UpdateOrReplace(IndBuff); 
                        return false; // do not continue
                    }
                    // proceed incrementally
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLID.Value);
                    int[] arr = IndBuff.GetArrayForRead();
                    int offset = change.Start * SourceBuffer.DataLength;
                    int size = change.Length * sizeof(int) * SourceBuffer.DataLength;
                    if (offset != 0) {
                        fixed (int* bufferArrP = arr) {
                            GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)(offset * sizeof(int)), (IntPtr)size, (IntPtr)(bufferArrP + offset));
                        }
                    } else {
                        GL.BufferSubData(BufferTarget.ElementArrayBuffer, (IntPtr)offset, (IntPtr)size, arr);
                    }
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                } else {
                    ILArray<float> bufferArray;
                    if (SourceBuffer.BufferType == BufferType.PositionBuffer) {
                        bufferArray = (SourceBuffer as ILPositionsBuffer).Storage;
                    } else if (SourceBuffer.BufferType == BufferType.NormalBuffer) {
                        bufferArray = (SourceBuffer as ILNormalsBuffer).Storage;
                    } else {
                        System.Diagnostics.Debug.Assert(SourceBuffer.BufferType == BufferType.ColorBuffer);
                        bufferArray = (SourceBuffer as ILColorsBuffer).Storage;
                    }
                    if (change.Length + change.Start > this.DataCount) {
                        UpdateOrReplace(bufferArray); 
                        return false; // do not continue
                    }
                    // proceed incrementally
                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLID.Value);
                    float[] bufferArr = bufferArray.GetArrayForRead();
                    int offset = change.Start * SourceBuffer.DataLength;
                    int size = change.Length * sizeof(float) * SourceBuffer.DataLength;
                    if (offset != 0) {
                        fixed (float* bufferArrP = bufferArr) {
                            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(offset * sizeof(float)), (IntPtr)size, (IntPtr)(bufferArrP + offset));
                        }
                    } else {
                        GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)offset, (IntPtr)size, bufferArr);
                    }
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    DataCount = Math.Min(DataCount, SourceBuffer.DataCount); 

                }
            }
            return true; 
        }

        void buffer_Changed(object sender, ILBufferChangedEventArgs e) {
            m_changes.Enqueue(e.ChangeQueueItem);
        }
        #endregion

    }
}
