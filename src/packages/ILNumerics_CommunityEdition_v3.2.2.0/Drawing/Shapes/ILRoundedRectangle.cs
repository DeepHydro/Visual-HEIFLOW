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
using System.Windows.Forms; 
using System.Drawing;
using System.Text;

namespace ILNumerics.Drawing {
    public class ILRoundedRectangle : ILGroup {

        public event MouseEventHandler MouseClick;
        protected void OnMouseClick(MouseEventArgs e) {
            if (MouseClick != null) {
                MouseClick(this, e); 
            }
        }

        protected ILTrianglesFan m_triangles;
        protected ILLineStrip m_lines;

        public ILTrianglesFan Triangles { get { return m_triangles; } }
        public ILLineStrip Border { get { return m_lines; } }
        public double Radius { get; set; }
        public int Resolution { get; set; }
        public bool RoundUpperLeftCorner { get; set; }
        public bool RoundLowerLeftCorner { get; set; }
        public bool RoundUpperRightCorner { get; set; }
        public bool RoundLowerRightCorner { get; set; }

        public ILRoundedRectangle(
            object tag = null,
            float radius = 0.2f,
            int resolution = 30,
            bool roundUL = true, bool roundUR = true, bool roundLR = true, bool roundLL = true) {
            if (object.Equals(tag,null))
                tag = "RoundedRectangle";
            RoundUpperLeftCorner = roundUL;
            RoundLowerLeftCorner = roundLL;
            RoundUpperRightCorner = roundUR;
            RoundLowerRightCorner = roundLR;
            Radius = radius;
            Resolution = resolution; 
            m_triangles = new ILTrianglesFan(Tag + "_area");
            m_lines = new ILLineStrip(Tag + "_border");
            Add(new ILGroup() { Transform = Matrix4.Rotation(new Vector3(1, 0, 0), 0) }).Add(m_triangles);
            Add(new ILGroup() { Transform = Matrix4.Translation(0, 0, -0.1) }).Add(m_lines);
            m_triangles.MouseClick += m_triangles_MouseClick;
            m_triangles.Color = Color.Yellow;
            m_triangles.Markable = false;

            m_lines.Positions = m_triangles.Positions;
            m_lines.Color = Color.DarkGray;
            m_lines.Antialiasing = true;
            m_lines.Markable = false;
            Recreate(); 
        }

        public void Recreate() {
            m_triangles.Positions.Update(Computation.Compute((float)Radius, Resolution,RoundUpperLeftCorner,RoundUpperRightCorner,RoundLowerRightCorner,RoundLowerLeftCorner));
            Configure(); 
        }
        void m_triangles_MouseClick(object sender, MouseEventArgs e) {
            OnMouseClick(e);
        }

        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {

            internal static ILRetArray<float> Compute(float radius, int resolution, 
                    bool ul, bool ur, bool lr, bool ll) {
                using (ILScope.Enter()) {
                    int roundedCount = 0; 
                    if (ul) roundedCount++; 
                    if (ur) roundedCount++; 
                    if (ll) roundedCount++; 
                    if (lr) roundedCount++; 
                    if (roundedCount == 0) {
                        // early exit
                        return array<float>(new float[] { 1f, -1, 0, -1f, -1f, 0, -1f, 1f, 0, 1f, 1f, 0, 1f, -1f, 0 },
                                            size(3,5)); 
                    }
                    int resol = resolution / 4; 
                    if (resol < 3) resol = 3; 
                    // first create a single corner (lower right shaped) 
                    ILArray<float> angles = linspace<float>(0, ILMath.pif / 2, resol);
                    ILArray<float> round = zeros<float>(3, resol);
                    round[0, full] = cos(angles);
                    round[1, full] = -sin(angles);
                    round *= radius;
                    // assembly each corner seperately 
                    ILArray<float> ret = zeros<float>(3,roundedCount * resol + (4 - roundedCount) + 1); 
                    int curPos = 0;
                    if (lr) {
                        ret[full, r(curPos, curPos + resol - 1)] = round + array<float>(1f - radius, -1f + radius, 0);
                        curPos += resol; 
                    } else {
                        ret[full, curPos] = array<float>(1f, -1f, 0);
                        curPos ++; 
                    }
                    if (ll) {
                        ret[full, r(curPos, curPos + resol - 1)] = round[full, r(end,-1, 0)] * array<float>(-1f, 1f, 1) + array<float>(-1f + radius, -1f + radius, 0);
                        curPos += resol; 
                    } else {
                        ret[full, curPos] = array<float>(-1f, -1f, 0);
                        curPos ++; 
                    }
                    if (ul) {
                        ret[full, r(curPos, curPos + resol - 1)] = round * array<float>(-1f, -1f, 1) + array<float>(-1f + radius, 1f - radius, 0);
                        curPos += resol; 
                    } else {
                        ret[full, curPos] = array<float>(-1f, 1f, 0);
                        curPos ++; 
                    }
                    if (ur) {
                        ret[full, r(curPos, curPos + resol - 1)] = round[full, r(end,-1, 0)] * array<float>(1f, -1f, 1) + array<float>(1f - radius, 1f - radius, 0);
                        curPos += resol; 
                    } else {
                        ret[full, curPos] = array<float>(1f, 1f, 0);
                        curPos ++; 
                    }
                    // close the circle
                    ret[full,end] = ret[full,0]; 
                    return ret;
                }
            }
        }
    }
}
