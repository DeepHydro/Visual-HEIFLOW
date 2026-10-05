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
using System.Diagnostics;
using System.Xml; 
namespace ILNumerics.Drawing {
    
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    [DebuggerDisplay("[4 x 4] Matrix4")]
    [Serializable]
    public struct Matrix4 {
        // column major storage!!
        public float M11, M21, M31, M41;
        public float M12, M22, M32, M42;
        public float M13, M23, M33, M43;
        public float M14, M24, M34, M44;
        

        //public static Vector3 operator *(Matrix4 mat, Vector3 vec) {
        //    return new Vector3(
        //        mat.M11 * vec.X + mat.M12 * vec.Y + mat.M13 * vec.Z + mat.M14,
        //        mat.M21 * vec.X + mat.M22 * vec.Y + mat.M23 * vec.Z + mat.M24,
        //        mat.M31 * vec.X + mat.M32 * vec.Y + mat.M33 * vec.Z + mat.M34); 
        //}
        public static Matrix4 operator *(Matrix4 left, Matrix4 right) {
            Matrix4 result;
            Mult(ref left, ref right, out result);
            return result;
        }
        public static Vector4 operator *(Matrix4 left, Vector4 right) {
            Vector4 result;
            Mult(ref left, ref right, out result);
            return result;
        }
        public static Vector4 operator *(Vector4 left, Matrix4 right) {
            Vector4 result;
            Mult(ref left, ref right, out result);
            return result;
        }
        /// <summary>
        /// create new matrix ROW MAJOR definition! 
        /// </summary>
        /// <param name="m11"></param>
        /// <param name="m12"></param>
        /// <param name="m13"></param>
        /// <param name="m14"></param>
        /// <param name="m21"></param>
        /// <param name="m22"></param>
        /// <param name="m23"></param>
        /// <param name="m24"></param>
        /// <param name="m31"></param>
        /// <param name="m32"></param>
        /// <param name="m33"></param>
        /// <param name="m34"></param>
        /// <param name="m41"></param>
        /// <param name="m42"></param>
        /// <param name="m43"></param>
        /// <param name="m44"></param>
        public Matrix4(float m11, float m12, float m13, float m14,
                       float m21, float m22, float m23, float m24,
                       float m31, float m32, float m33, float m34,
                       float m41, float m42, float m43, float m44) {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }

        public Matrix4(Matrix3 m3) {
            M11 = m3.M11; M12 = m3.M12; M13 = m3.M13; M14 = 0;
            M21 = m3.M21; M22 = m3.M22; M23 = m3.M23; M24 = 0;
            M31 = m3.M31; M32 = m3.M32; M33 = m3.M33; M34 = 0;
            M41 = 0; M42 = 0; M43 = 0; M44 = 1;
        }

        public static readonly Matrix4 Identity = new Matrix4(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);
        #region public interface 

        public static Matrix4 LookAtTransformation(Vector3 camPos, Vector3 lookAt, Vector3 upVec) {
            Vector3 lookDir = Vector3.Normalize(lookAt - camPos);
            upVec.Normalize(); 

            Vector3 x = Vector3.CrossN(lookDir, upVec);
            Vector3 y = Vector3.Cross(x, lookDir);

            Matrix4 rotMat = Matrix4.Identity;

            rotMat.M11 = x.X;
            rotMat.M12 = x.Y;
            rotMat.M13 = x.Z;

            rotMat.M21 = y.X;
            rotMat.M22 = y.Y;
            rotMat.M23 = y.Z;

            rotMat.M31 = -lookDir.X;
            rotMat.M32 = -lookDir.Y;
            rotMat.M33 = -lookDir.Z;

            return rotMat * Matrix4.Translation(-camPos.X, -camPos.Y, -camPos.Z);
        }
        /// <summary>
        /// Calculate the inverse of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to invert</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Matrix4 is singular.</exception>
        public static Matrix4 Invert(Matrix4 mat) {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            float[,] inverse = {{mat.M11, mat.M12, mat.M13, mat.M14}, 
                                {mat.M21, mat.M22, mat.M23, mat.M24}, 
                                {mat.M31, mat.M32, mat.M33, mat.M34}, 
                                {mat.M41, mat.M42, mat.M43, mat.M44} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++) {
                // Find the largest pivot value
                float maxPivot = 0.0f;
                for (int j = 0; j < 4; j++) {
                    if (pivotIdx[j] != 0) {
                        for (int k = 0; k < 4; ++k) {
                            if (pivotIdx[k] == -1) {
                                float absVal = System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot) {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            } else if (pivotIdx[k] > 0) {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol) {
                    for (int k = 0; k < 4; ++k) {
                        float f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                float pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0.0f) {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                float oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j) {
                    // check this isn't on the diagonal
                    if (icol != j) {
                        float f = inverse[j, icol];
                        inverse[j, icol] = 0.0f;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j) {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k) {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            mat.M11 = inverse[0, 0]; mat.M12 = inverse[0, 1]; mat.M13 = inverse[0, 2]; mat.M14 = inverse[0, 3];
            mat.M21 = inverse[1, 0]; mat.M22 = inverse[1, 1]; mat.M23 = inverse[1, 2]; mat.M24 = inverse[1, 3];
            mat.M31 = inverse[2, 0]; mat.M32 = inverse[2, 1]; mat.M33 = inverse[2, 2]; mat.M34 = inverse[2, 3];
            mat.M41 = inverse[3, 0]; mat.M42 = inverse[3, 1]; mat.M43 = inverse[3, 2]; mat.M44 = inverse[3, 3];
            return mat;
        }
        internal static Matrix4 Transpose(Matrix4 a) {
            return new Matrix4(
            a.M11, a.M21, a.M31, a.M41,
            a.M12, a.M22, a.M32, a.M42,
            a.M13, a.M23, a.M33, a.M43,
            a.M14, a.M24, a.M34, a.M44
                ); 
        }
        public Matrix4 Transpose() {
            return Matrix4.Transpose(this); 
        }

        public Matrix4 Translate(double x, double y, double z) {
            return Translation(x, y, z) * this; 
        }
        public static Matrix4 Translation(Vector3 offset) {
            return Translation(offset.X,offset.Y,offset.Z); 
        }
        public static Matrix4 Translation(double x, double y, double z) {
            Matrix4 ret = Identity;
            ret.M14 = (float)x;
            ret.M24 = (float)y;
            ret.M34 = (float)z;
            return ret;
        }
        public Matrix4 Scale(double x, double y, double z) {
            return ScaleTransform(x,y,z) * this;
        }
        public static Matrix4 ScaleTransform(double x, double y, double z) {
            Matrix4 ret = Identity;
            ret.M11 = (float)x;
            ret.M22 = (float)y;
            ret.M33 = (float)z;
            return ret;
        }
        public static Matrix4 PerspectiveTransform(float frustumscale, float zNear, float zFar, float aspectRatio = 1f) {
            Matrix4 ret = Identity;
            ret.M11 = frustumscale / aspectRatio; 
            ret.M22 = frustumscale; 
            float NminF = zNear - zFar; 
            ret.M33 = (zNear + zFar) / NminF; 
            ret.M43 = -1; 
            ret.M34 = 2 * zFar * zNear / NminF; 
            return ret; 
        }
        public static Matrix4 OrthographicTransform(float left, float right, float top, float bottom, float zNear, float zFar) {
            //float tx = (right + left)/(right - left); 
            //float ty = (top + bottom)/(top - bottom);
            //float tz = (zFar + zNear) / (zFar - zNear); 
            //ret = new Matrix4(2 / (right - left), 0, 0, tx,
            //                          0, 2 / (top - bottom), 0, ty,
            //                          0, 0, -2 / (zFar - zNear), tz,
            //                          0, 0, 0, 1);
            //Matrix4 ret = Matrix4.ScaleTransform(2f / (right - left), 2f / (top - bottom), 2f / (zFar - zNear))
            //            * Translation(-(right + left) / 2f, -(top + bottom) / 2f, -(zFar + zNear) / 2f);
            Matrix4 ret = new Matrix4();

            float invRL = 1 / (right - left);
            float invTB = 1 / (top - bottom);
            float invFN = 1 / (zFar - zNear);

            ret.M11 = 2 * invRL;
            ret.M22 = 2 * invTB;
            ret.M33 = -2 * invFN;

            ret.M14 = -(right + left) * invRL;
            ret.M24 = -(top + bottom) * invTB;
            ret.M34 = -(zFar + zNear) * invFN;
            ret.M44 = 1;
            return ret; 
        }
        /// <summary>
        /// Creates a perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static Matrix4 CreatePerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar) {
            if (fovy <= 0 || fovy > Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float yMax = zNear * (float)System.Math.Tan(0.5f * fovy);
            float yMin = -yMax;
            float xMin = yMin * aspect;
            float xMax = yMax * aspect;
            Matrix4 result; 
            CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out result);
            return result; 
        }
        /// <summary>
        /// Creates an perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4 result) {
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float x = (2.0f * zNear) / (right - left);
            float y = (2.0f * zNear) / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(zFar + zNear) / (zFar - zNear);
            float d = -(2.0f * zFar * zNear) / (zFar - zNear);

            result = new Matrix4(x,  0,  a,  0,
                                 0,  y,  b,  0,
                                 0,  0,  c,  d,
                                 0,  0, -1,  0);
        }

        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <param name="result">A matrix instance.</param>
        public static Matrix4 Rotation(Vector3 axis, double angle) {
            float cos = (float)System.Math.Cos(-angle);
            float sin = (float)System.Math.Sin(-angle);
            float t = 1.0f - cos;

            axis.Normalize();

            return new Matrix4(t * axis.X * axis.X + cos, t * axis.X * axis.Y - sin * axis.Z, t * axis.X * axis.Z + sin * axis.Y, 0.0f,
                                 t * axis.X * axis.Y + sin * axis.Z, t * axis.Y * axis.Y + cos, t * axis.Y * axis.Z - sin * axis.X, 0.0f,
                                 t * axis.X * axis.Z - sin * axis.Y, t * axis.Y * axis.Z + sin * axis.X, t * axis.Z * axis.Z + cos, 0.0f,
                                 0, 0, 0, 1);
        }

        public static void Mult(ref Matrix4 left, ref Vector4 right, out Vector4 result) {
            result = new Vector4(
                left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14 * right.W,
                left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24 * right.W,
                left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34 * right.W,
                left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z + left.M44 * right.W);
        }
        public static Vector4 Mult(Matrix4 left, ref Vector3 right) {
            var result = new Vector4(
                left.M11 * right.X + left.M12 * right.Y + left.M13 * right.Z + left.M14,
                left.M21 * right.X + left.M22 * right.Y + left.M23 * right.Z + left.M24,
                left.M31 * right.X + left.M32 * right.Y + left.M33 * right.Z + left.M34,
                left.M41 * right.X + left.M42 * right.Y + left.M43 * right.Z + left.M44);
            return result; 
        }
        public static void Mult(ref Vector4 left, ref Matrix4 right, out Vector4 result) {
            result = new Vector4(
                left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41,
                left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42,
                left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43,
                left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34 + left.W * right.M44);
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref Matrix4 left, ref Matrix4 right, out Matrix4 result) {
            result = new Matrix4(
                left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41,
                left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42,
                left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43,
                left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44,
                left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41,
                left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42,
                left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43,
                left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44,
                left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41,
                left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42,
                left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43,
                left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44,
                left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41,
                left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42,
                left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43,
                left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44);
        }

        public override string ToString() {
            return String.Format(
                "{1} {2} {3} {4} {0}" +
                "{5} {6} {7} {8} {0}" +
                "{9} {10} {11} {12} {0}" +
                "{13} {14} {15} {16} {0}"
                ,Environment.NewLine
                , M11, M12, M13, M14
                , M21, M22, M23, M24
                , M31, M32, M33, M34
                , M41, M42, M43, M44); 
        }
        public void ToXML(XmlWriter writer) {
            string val = String.Format(
                "{1},{2},{3},{4}," +
                "{5},{6},{7},{8}," +
                "{9},{10},{11},{12}," +
                "{13},{14},{15},{16}"
                , Environment.NewLine
                , M11, M21, M31, M41
                , M12, M22, M32, M42
                , M13, M23, M33, M43
                , M14, M24, M34, M44);
            writer.WriteElementString("ColumnMajor", val);  

        }

        #endregion

        private class DebuggerProxy {

            ILNumerics.Misc.ILArrayDebuggerProxy<float> m_proxy;
            public DebuggerProxy(Matrix4 mat) {
                m_proxy = new ILNumerics.Misc.ILArrayDebuggerProxy<float>(
                ILMath.array<float>(
                        ILMath.size(4, 4),
                        mat.M11, mat.M21, mat.M31, mat.M41,
                        mat.M12, mat.M22, mat.M32, mat.M42,
                        mat.M13, mat.M23, mat.M33, mat.M43,
                        mat.M14, mat.M24, mat.M34, mat.M44));
            }


            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ILNumerics.Misc.RowVisualizer[] Rows {
                get {
                    return m_proxy.Rows; 
                }
            }
        }


        public Matrix4 Rotate(Quaternion offset) {
            Vector3 axis;
            float angle;
            offset.ToAxisAngle(out axis, out angle);
            return Rotation(axis, angle) * this;
        }
        public Matrix4 Rotate(Vector3 direction, double angle) {
            Matrix4 ret = Matrix4.Rotation(direction, (float)angle) * this; 
            return ret;
        }
        public static Matrix4 Rotation(Quaternion offset) {
            Vector3 axis;
            float angle;
            offset.ToAxisAngle(out axis, out angle);
            return Rotation(axis, angle);
        }
        internal ILRetArray<float> ToArray() {
            using (ILScope.Enter()) {
                float[] retArr = ILMath.New<float>(16); 
                ILArray<float> ret = ILMath.array<float>(retArr,ILMath.size(4,4));
                ret[0] = M11; ret[4] = M12; ret[08] = M13; ret[12] = M14;
                ret[1] = M21; ret[5] = M22; ret[09] = M23; ret[13] = M24;
                ret[2] = M31; ret[6] = M32; ret[10] = M33; ret[14] = M34;
                ret[3] = M41; ret[7] = M42; ret[11] = M43; ret[15] = M44;
                return ret; 
            }
        }

        public static ILRetArray<float> operator *(Matrix4 left, ILInArray<float> right) {
            using (ILScope.Enter(right)) {
                if (right.S[0] == 3) {
                    ILArray<float> ret = ILMath.zeros<float>(ILMath.size(4, right.S[1]));
                    for (int i = 0; i < right.S[1]; i++) {
                        float r1 = right.GetValue(0, i), r2 = right.GetValue(1, i), r3 = right.GetValue(2, i);
                        ret.SetValue(left.M11 * r1 + left.M12 * r2 + left.M13 * r3 + left.M14, 0, i);
                        ret.SetValue(left.M21 * r1 + left.M22 * r2 + left.M23 * r3 + left.M24, 1, i);
                        ret.SetValue(left.M31 * r1 + left.M32 * r2 + left.M33 * r3 + left.M34, 2, i);
                        ret.SetValue(left.M41 * r1 + left.M42 * r2 + left.M43 * r3 + left.M44, 3, i);
                    }
                    return ret;
                } else {
                    ILArray<float> tmp = ILMath.multiply(left.ToArray(), right);
                    return tmp; // / tmp[ILMath.end, ILMath.full];  <- we do not do perspective divide here! 
                }

            }
        }
        /// <summary>
        /// Transforms clip positions into view coordinates, includes perspective divide
        /// </summary>
        /// <param name="ViewTransform">View Transformation Matrix</param>
        /// <param name="A">clip positions matrix, 3xn or 4xn</param>
        /// <returns>view coordinates</returns>
        public static ILRetArray<float> ViewTransformWithPerspDivide(Matrix4 ViewTransform, ILInArray<float> A) {
            using (ILScope.Enter(A)) {
                if (A.S[0] == 3) {
                    ILArray<float> ret = ILMath.array<float>(ILMath.New<float>(A.S.NumberOfElements), A.Size);
                    float[] retArr = ret.GetArrayForWrite();
                    float[] rigArr = A.GetArrayForRead();
                    for (int i = 0; i < A.S.NumberOfElements; i += 3) {
                        float r1 = rigArr[i], r2 = rigArr[i + 1], r3 = rigArr[i + 2];
                        float w = ViewTransform.M41 * r1 + ViewTransform.M42 * r2 + ViewTransform.M43 * r3 + ViewTransform.M44;
                        retArr[i + 0] = (ViewTransform.M11 * r1 + ViewTransform.M12 * r2 + ViewTransform.M13 * r3 + ViewTransform.M14) / w;
                        retArr[i + 1] = (ViewTransform.M21 * r1 + ViewTransform.M22 * r2 + ViewTransform.M23 * r3 + ViewTransform.M24) / w;
                        retArr[i + 2] = (ViewTransform.M31 * r1 + ViewTransform.M32 * r2 + ViewTransform.M33 * r3 + ViewTransform.M34) / w;
                    }
                    return ret;
                } else {
                    return ILMath.multiply(ViewTransform.ToArray(), A / A[ILMath.end, ILMath.full]);
                }
            }
        }
        public static Matrix4 FromViewRectangleF(float left, float right, float top, float bottom) {
            return Matrix4.ScaleTransform((right - left), (bottom - top), 1).Translate(left, top, 0);
        }

        #region CreateFromQuaternion
        /// <summary>
        /// Build a rotation matrix from the specified quaternion.
        /// </summary>
        /// <param name="q">Quaternion to translate.</param>
        /// <param name="m">Matrix result.</param>
        public static void CreateFromQuaternion(ref Quaternion q,ref Matrix4 m)
        {
            m = Matrix4.Identity;

			float X = q.X;
			float Y = q.Y;
			float Z = q.Z;
			float W = q.W;
			
			float xx = X * X;
			float xy = X * Y;
			float xz = X * Z;
			float xw = X * W;
			float yy = Y * Y;
			float yz = Y * Z;
			float yw = Y * W;
			float zz = Z * Z;
			float zw = Z * W;
			
			m.M11 = 1 - 2 * (yy + zz);
			m.M21 = 2 * (xy - zw);
			m.M31 = 2 * (xz + yw);
			m.M12 = 2 * (xy + zw);
			m.M22 = 1 - 2 * (xx + zz);
			m.M32 = 2 * (yz - xw);
			m.M13 = 2 * (xz - yw);
			m.M23 = 2 * (yz + xw);
			m.M33 = 1 - 2 * (xx + yy);
        }
        public static Matrix4 CreateFromQuaternion(Quaternion q) {
            Matrix4 ret = default(Matrix4); 
            CreateFromQuaternion(ref q, ref ret); 
            return ret; 
        }
        #endregion

        #region ViewTransforms 
        public static Matrix4 ViewportFromScreen0_1ToClip {
            get {
                return Matrix4.ScaleTransform(2,-2,1).Translate(-1,1,0); 
            }
        }
        #endregion


    }

}
