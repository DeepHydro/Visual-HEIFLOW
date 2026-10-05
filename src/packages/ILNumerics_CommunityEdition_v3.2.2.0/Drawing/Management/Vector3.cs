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
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using OpenTK; 

/* Disclaimer: a lot of the code on Vector3 (and other Vector and Matrix structs) 
 * has shamelessly been stolen from OpenTK: http://opentk.com and adapted for ILNumerics.
 */

namespace ILNumerics.Drawing {

    /// <summary>
    /// single precision 3D point structure
    /// </summary>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3 : IEquatable<Vector3> {

        #region attributes
        /// <summary>
        /// X coordinate
        /// </summary>
        internal float m_x;
        /// <summary>
        /// Y coordinate
        /// </summary>
        internal float m_y;
        /// <summary>
        /// Z coordinate
        /// </summary>
        internal float m_z;
        //private float padding; 
        #endregion

        #region properties
        /// <summary>
        /// X coordinate
        /// </summary>
        public float X {
            get { return m_x; }
            set { m_x = value; }
        }
        /// <summary>
        /// Y coordinate
        /// </summary>
        public float Y {
            get { return m_y; }
            set { m_y = value; }
        }
        /// <summary>
        /// Z coordinate
        /// </summary>
        public float Z {
            get { return m_z; }
            set { m_z = value; }
        }
        #endregion

        #region constructors
        /// <summary>
        /// create vector explicitly
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <param name="z">Z coord</param>
        public Vector3(float x, float y, float z) {
            m_x = x;
            m_y = y;
            m_z = z;
            //padding = -1; 
        }
        /// <summary>
        /// create explicitly
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <param name="z">Z coors</param>
        public Vector3(double x, double y, double z) {
            m_x = (float)x;
            m_y = (float)y;
            m_z = (float)z;
            //padding = -1;
        }
        #endregion

        /// <summary>
        /// convert this point to string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "X:" + X.ToString() + " Y:" + Y.ToString() + " Z:" + Z.ToString();
        }
        /// <summary>
        /// Access to coords by index
        /// </summary>
        /// <param name="index">index number: 0=x, 1=y, 2=z</param>
        /// <returns>float value of coord specified</returns>
        public float this[int index] {
            get {
                switch (index) {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        return Z;
                }
            }
            set {
                switch (index) {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        break;
                }
            }
        }
        public static bool operator ==(Vector3 p1, Vector3 p2) {
            return (p1.X == p2.X &&
                    p1.Y == p2.Y &&
                    p1.Z == p2.Z);
        }
        public static bool operator !=(Vector3 p1, Vector3 p2) {
            return (p1.X != p2.X ||
                    p1.Y != p2.Y ||
                    p1.Z != p2.Z);
        }
        public static Vector3 operator *(Vector3 p1, Vector3 p2) {
            Vector3 ret = new Vector3();
            ret.X = p1.X * p2.X;
            ret.Y = p1.Y * p2.Y;
            ret.Z = p1.Z * p2.Z;
            return ret;
        }
        public static Vector3 operator +(Vector3 p1, Vector3 p2) {
            Vector3 ret = new Vector3();
            ret.X = p1.X + p2.X;
            ret.Y = p1.Y + p2.Y;
            ret.Z = p1.Z + p2.Z;
            return ret;
        }
        public static Vector3 operator -(Vector3 p1, Vector3 p2) {
            Vector3 ret = new Vector3();
            ret.X = p1.X - p2.X;
            ret.Y = p1.Y - p2.Y;
            ret.Z = p1.Z - p2.Z;
            return ret;
        }
        public static Vector3 operator -(Vector3 vec) {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            vec.Z = -vec.Z;
            return vec;
        }
        public static Vector3 operator -(Vector3 vec, float a) {
            vec.X -= a;
            vec.Y -= a;
            vec.Z -= a;
            return vec;
        }
        public static Vector3 operator +(Vector3 vec, float a) {
            vec.X += a;
            vec.Y += a;
            vec.Z += a;
            return vec;
        }

        public static Vector3 operator *(Vector3 p1, float factor) {
            Vector3 ret = new Vector3();
            ret.X = p1.X * factor;
            ret.Y = p1.Y * factor;
            ret.Z = p1.Z * factor;
            return ret;
        }
        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="scale">The scalar.</param>
        /// <param name="vec">The instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Vector3 operator *(float scale, Vector3 vec) {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }
        /// <summary>
        /// Calculate the dot (scalar) product of two vectors
        /// </summary>
        /// <param name="left">First operand</param>
        /// <param name="right">Second operand</param>
        /// <returns>The dot product of the two inputs</returns>
        public static float Dot(Vector3 left, Vector3 right) {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }
        /// <summary>
        /// elementwise power for vector elements 
        /// </summary>
        /// <param name="vec">vector</param>
        /// <param name="exp">exponent</param>
        /// <returns>new vector with elementwise power of 'vec's elements</returns>
        public static Vector3 Pow(Vector3 vec, float exp) {
            return new Vector3(Math.Pow(vec.X,exp), Math.Pow(vec.Y, exp), Math.Pow(vec.Z, exp)); 
        }
        /// <summary>
        /// Vector transformation and perspective divide
        /// </summary>
        /// <param name="mat">transformation matrix</param>
        /// <param name="vec">vector</param>
        /// <returns>transformed vector in normalized device coordinates</returns>
        public static Vector3 operator *(Matrix4 mat, Vector3 vec) {
            return new Vector3(
                mat.M11 * vec.X + mat.M12 * vec.Y + mat.M13 * vec.Z + mat.M14,
                mat.M21 * vec.X + mat.M22 * vec.Y + mat.M23 * vec.Z + mat.M24,
                mat.M31 * vec.X + mat.M32 * vec.Y + mat.M33 * vec.Z + mat.M34)
                /
                (mat.M41 * vec.X + mat.M42 * vec.Y + mat.M43 * vec.Z + mat.M44);
        }
        /// <summary>
        /// Vector transformation (left side)
        /// </summary>
        /// <param name="mat">transformation matrix</param>
        /// <param name="vec">vector</param>
        /// <returns>transformed vector in normalized device coordinates</returns>
        public static Vector3 operator *(Vector3 vec, Matrix4 mat) {
            return new Vector3(
                vec.X * mat.M11 + vec.Y * mat.M21 + vec.Z * mat.M31 + mat.M41,
                vec.X * mat.M12 + vec.Y * mat.M22 + vec.Z * mat.M32 + mat.M42,
                vec.X * mat.M13 + vec.Y * mat.M23 + vec.Z * mat.M33 + mat.M43); 
        }
        public static Vector3 operator /(Vector3 p1, float factor) {
            Vector3 ret = new Vector3();
            ret.X = p1.X / factor;
            ret.Y = p1.Y / factor;
            ret.Z = p1.Z / factor;
            return ret;
        }
        public static readonly Vector3 Empty = new Vector3(float.NaN, float.NaN, float.NaN);
        
        public bool IsEmtpy() {
            return float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z);
        }

        /// <summary>
        /// cross product
        /// </summary>
        /// <param name="a">vector 1</param>
        /// <param name="b">vector 2</param>
        /// <returns>normalized cross product between a x b</returns>
        public static Vector3 Cross(Vector3 a, Vector3 b) {
            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
        }
        /// <summary>
        /// normalized cross product
        /// </summary>
        /// <param name="a">vector 1</param>
        /// <param name="b">vector 2</param>
        /// <returns>normalized cross product: a x b</returns>
        public static Vector3 CrossN(Vector3 a, Vector3 b) {
            Vector3 ret = new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
            float len = (float)Math.Sqrt(ret.X * ret.X + ret.Z * ret.Z + ret.Y * ret.Y);
            if (len != 0)
                return (ret / len);
            return ret;
        }
        #region public float Length

        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <see cref="LengthFast"/>
        /// <seealso cref="LengthSquared"/>
        public float Length {
            get {
                return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        #endregion

        #region public float LengthFast

        /// <summary>
        /// Gets an approximation of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property uses an approximation of the square root function to calculate vector magnitude, with
        /// an upper error bound of 0.001.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthSquared"/>
        public float LengthFast {
            get {
                return 1.0f / InverseSqrtFast(X * X + Y * Y + Z * Z);
            }
        }

        #endregion


        #region InverseSqrtFast

        /// <summary>
        /// Returns an approximation of the inverse square root of left number.
        /// </summary>
        /// <param name="x">A number.</param>
        /// <returns>An approximation of the inverse square root of the specified number, with an upper error bound of 0.001</returns>
        /// <remarks>
        /// This is an improved implementation of the the method known as Carmack's inverse square root
        /// which is found in the Quake III source code. This implementation comes from
        /// http://www.codemaestro.com/reviews/review00000105.html. For the history of this method, see
        /// http://www.beyond3d.com/content/articles/8/
        /// </remarks>
        [SecuritySafeCritical]
        public static float InverseSqrtFast(float x) {
            unsafe {
                float xhalf = 0.5f * x;
                int i = *(int*)&x;              // Read bits as integer.
                i = 0x5f375a86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation
                x = *(float*)&i;                // Convert bits back to float
                x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
                return x;
            }
        }

        #endregion


        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        /// <see cref="Length"/>
        /// <seealso cref="LengthFast"/>
        public float LengthSquared {
            get {
                return X * X + Y * Y + Z * Z;
            }
        }

        #endregion

        public static readonly Vector3 UnitX = new Vector3(1,0,0); 
        public static readonly Vector3 UnitY = new Vector3(0,1,0); 
        public static readonly Vector3 UnitZ = new Vector3(0,0,1); 

        public static Vector3 MaxValue {
            get { return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue); }
        }
        public static Vector3 MinValue {
            get { return new Vector3(float.MinValue, float.MinValue, float.MinValue); }
        }
        public static Vector3 Min(Vector3 val1, Vector3 val2) {
            Vector3 ret = val1;
            if (val2.X < val1.X) ret.X = val2.X;
            if (val2.Y < val1.Y) ret.Y = val2.Y;
            if (val2.Z < val1.Z) ret.Z = val2.Z;
            return ret;
        }
        public static Vector3 Min(Vector3 val1, Vector3 val2, ref bool changed) {
            Vector3 ret = val1;
            if (val2.X < val1.X) {
                ret.X = val2.X;
                changed = true;
            }
            if (val2.Y < val1.Y) {
                ret.Y = val2.Y;
                changed = true;
            }
            if (val2.Z < val1.Z) {
                ret.Z = val2.Z;
                changed = true;
            }
            return ret;
        }
        public static Vector3 Max(Vector3 val1, Vector3 val2) {
            Vector3 ret = val1;
            if (val2.X > val1.X) ret.X = val2.X;
            if (val2.Y > val1.Y) ret.Y = val2.Y;
            if (val2.Z > val1.Z) ret.Z = val2.Z;
            return ret;
        }
        public static Vector3 Max(Vector3 val1, Vector3 val2, ref bool changed) {
            Vector3 ret = val1;
            if (val2.X > val1.X) {
                ret.X = val2.X;
                changed = true;
            }
            if (val2.Y > val1.Y) {
                ret.Y = val2.Y;
                changed = true;
            }
            if (val2.Z > val1.Z) {
                ret.Z = val2.Z;
                changed = true;
            }
            return ret;
        }


        public static Vector3 Normalize(Vector3 p) {
            float length = (float)Math.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);
            return p / length;
        }

        public static Vector3 Normalize(float x, float y, float z) {
            float length = (float)Math.Sqrt(x * x + y * y + z * z);
            return new Vector3(x, y, z) / length;
        }

        public Vector3 ToPolar() {
            return new Vector3( (float)Math.Atan2(Y, X),
                                (float)Math.Atan2(Math.Sqrt(X * X + Y * Y), Z), 
                                (float)Math.Sqrt(X * X + Y * Y + Z * Z)); 
        }

        /// <summary>
        /// rotate the vector, keep length
        /// </summary>
        /// <param name="normal">axis as rotation normal</param>
        /// <param name="angleDeg">angle to move (radian)</param>
        /// <returns>rotated version of this vector, does not change original vector</returns>
        public Vector3 Spin(Vector3 normal, float angleDeg) {
            float a = angleDeg * (float)Math.PI / 180f;
            float cosa = (float)Math.Cos(a);
            float sina = (float)Math.Sin(a);
            normal = Vector3.Normalize(normal);
            float omincosa = 1 - cosa;
            Vector3 ret = new Vector3(
                (cosa + X * X * omincosa) * normal.X
                + (X * Y * omincosa - Z * sina) * normal.Y
                + (X * Z * omincosa + Y * sina) * normal.Z,

                (Y * X * omincosa + Z * sina) * normal.X
                + (cosa + Y * Y * omincosa) * normal.Y
                + (Y * Z * omincosa - X * sina) * normal.Z,

                (Z * X * omincosa - Y * sina) * normal.X
                + (Z * Y * omincosa + X * sina) * normal.Y
                + (cosa + Z * Z * omincosa) * normal.Z);
            return ret;
        }

        #region public void Normalize()

        /// <summary>
        /// Scales the Vector3 to unit length.
        /// </summary>
        public void Normalize() {
            float scale = 1.0f / this.Length;
            X *= scale;
            Y *= scale;
            Z *= scale;
        }

        #endregion

        #region public void NormalizeFast()

        /// <summary>
        /// Scales the Vector3 to approximately unit length.
        /// </summary>
        public void NormalizeFast() {
            float scale = InverseSqrtFast(X * X + Y * Y + Z * Z);
            X *= scale;
            Y *= scale;
            Z *= scale;
        }

        #endregion


        /// <summary>
        /// Compares obj's coordinate values to those of this class instance
        /// </summary>
        /// <param name="obj">Vector3 to compare</param>
        /// <returns>true, if X,Y and Z coordinates are equal</returns>
        public override bool Equals(object obj) {
            if (obj is Vector3) 
                return false; 
            return Equals((Vector3)obj);
        }
        /// <summary>
        /// get a hash code for this Vector3 object
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static Vector3 Round(Vector3 a) {
            return new Vector3(Math.Round(a.X, MidpointRounding.AwayFromZero), Math.Round(a.Y, MidpointRounding.AwayFromZero), Math.Round(a.Z, MidpointRounding.AwayFromZero)); 
        }

        #region IEquatable<Vector3> Members

        public bool Equals(Vector3 other) {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z; 
        }

        #endregion

        /// <summary>
        /// Convert this vector into a short string representation, suitable for xml attribute serialization
        /// </summary>
        /// <returns>short string representing this vectors content</returns>
        public string ToXMLAttrString() {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2}", X,Y,Z); 
        }
    }
}
