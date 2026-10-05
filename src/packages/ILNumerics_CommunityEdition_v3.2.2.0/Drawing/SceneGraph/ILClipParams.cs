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
using System.Xml;

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILClipParams {

        public Vector4 Plane0; // XMin
        public Vector4 Plane1; // XMax
        public Vector4 Plane2; // YMin
        public Vector4 Plane3; // YMax
        public Vector4 Plane4; // ZMin
        public Vector4 Plane5; // ZMax

        public ILClipParams() {
            Plane0 = new Vector4(-1, 0, 0, float.MaxValue);
            Plane1 = new Vector4(1, 0, 0, float.MaxValue);
            Plane2 = new Vector4(0, -1, 0, float.MaxValue);
            Plane3 = new Vector4(0, 1, 0, float.MaxValue);
            Plane4 = new Vector4(0, 0, -1, float.MaxValue);
            Plane5 = new Vector4(0, 0, 1, float.MaxValue); 
        }

        /// <summary>
        /// A set of clipping planes which does apply clipping at infinity; effectively causes no clipping
        /// </summary>
        public static ILClipParams Infinity {
            get { return new ILClipParams(); }
        }

        public static bool operator !=(ILClipParams left, ILClipParams right) {
            return !(left == right);
        }
        public static bool operator ==(ILClipParams left, ILClipParams right) {
            if (object.Equals(left,null)) {
                if (object.Equals(right, null)) return true;
                return false;
            }
            if (object.Equals(right, null)) {
                if (object.Equals(left, null)) return true;
                return false;
            }
            return 
                left.Plane0 == right.Plane0 &&
                left.Plane1 == right.Plane1 &&
                left.Plane2 == right.Plane2 &&
                left.Plane3 == right.Plane3 &&
                left.Plane4 == right.Plane4 &&
                left.Plane5 == right.Plane5;
        }
        public Vector4 this[int i] {
            get {
                switch (i) {
                    case 0:
                        return Plane0;
                    case 1:
                        return Plane1;
                    case 2:
                        return Plane2;
                    case 3:
                        return Plane3;
                    case 4:
                        return Plane4;
                    case 5:
                    default:
                        return Plane5;
                }
            }
            internal set {
                switch (i) {
                    case 0:
                        Plane0 = value;
                        break;
                    case 1:
                        Plane1 = value;
                        break;
                    case 2:
                        Plane2 = value;
                        break;
                    case 3:
                        Plane3 = value;
                        break;
                    case 4:
                        Plane4 = value;
                        break;
                    case 5:
                    default:
                        Plane5 = value;
                        break;
                }
            }
        }

        internal void Update(ILLimits Limits) {
            Vector3 max = Limits.Max; 
            Vector3 min = Limits.Min; 
            if (Limits.WidthF > 0) {
                if (max.X != Plane0.W || Plane0.X != -1) {
                    Plane0.X = -1; Plane0.W = max.X;
                }
                if (min.X != -Plane1.W || Plane1.X != 1) {
                    Plane1.X = 1; Plane1.W = -min.X;
                }
            }
            if (Limits.HeightF > 0) {
                if (max.Y != Plane2.W || Plane2.Y != -1) {
                    Plane2.Y = -1; Plane2.W = max.Y;
                }
                if (min.Y != -Plane3.W || Plane3.Y != 1) {
                    Plane3.Y = 1; Plane3.W = -min.Y;
                }
            }
            if (Limits.DepthF > 0) {
                if (max.Z != Plane4.W || Plane4.Z != -1) {
                    Plane4.Z = -1; Plane4.W = max.Z;
                }
                if (min.Z != -Plane5.W || Plane5.Z != 1) {
                    Plane5.Z = 1; Plane5.W = -min.Z;
                }
            }
        }
        public ILClipParams Copy() {
            return (ILClipParams)MemberwiseClone(); 
        }

        public override string ToString() {
            return String.Format("0:{0} -> 1:{1} | 2:{2} -> 3:{3} | 4:{4} -> 5:{5}"
                                , Plane0, Plane1, Plane2, Plane3, Plane4, Plane5); 
        }
        public void ToXML(XmlWriter writer) {
            Action<Vector4, int> writePlane = (plane, index) => {
                writer.WriteElementString("Plane" + index.ToString(),plane.ToXMLAttrString()); 
            };

            for (int i = 0; i < 6; i++) {
                writePlane(this[i],i); 
            }
        }
    }
}
