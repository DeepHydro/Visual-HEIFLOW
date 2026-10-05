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

namespace ILNumerics.Drawing {
    internal class ILNormalVisualizer : ILLines {

        ILShape m_shape;
        bool m_isDirty;

        public ILNormalVisualizer(ILShape shape) {
            m_shape = shape;
            m_isDirty = true; 
            m_shape.Positions.Changed += new EventHandler<ILBufferChangedEventArgs>((a, e) => { m_isDirty = true; });
            m_shape.Indices.Changed += new EventHandler<ILBufferChangedEventArgs>((a, e) => { m_isDirty = true; });
            m_shape.Normals.Changed += new EventHandler<ILBufferChangedEventArgs>((a, e) => { m_isDirty = true; });
            Color = System.Drawing.Color.White; 
            Width = 1; 
            Tag = "NORMAL_VISUALIZER" + ILShape.s_nodesCount;
        }

        public override void Configure(bool configureDown = true, bool configureUp = true) {
            Visible = m_shape.Visible; 
            if (!m_isDirty) return;
            Computation.create(m_shape, this);
            m_isDirty = false; 
            base.Configure(false, true);
        }

        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {
            public static void create(ILShape shape, ILLines lines) {
                if (shape.Normals.IsEmpty) return; 
                ILArray<float> pos = shape.Positions.Storage;
                if (!shape.Indices.IsEmpty) {
                    ILArray<int> used = shape.Indices.Storage[full]; 
                    pos.a = pos[full,used];
                    pos.a = pos.Concat(pos + shape.Normals.Storage[full, used], 0);
                } else {
                    pos.a = pos.Concat(pos + shape.Normals.Storage, 0);
                }
                pos.a = reshape(pos,3,pos.S[1] * 2);
                if (lines.Positions.DataCount > pos.S[1]) {
                    lines.Positions = new ILPositionsBuffer(); 
                }
                lines.Positions.Update(pos); 
            }

        }
    }
}
