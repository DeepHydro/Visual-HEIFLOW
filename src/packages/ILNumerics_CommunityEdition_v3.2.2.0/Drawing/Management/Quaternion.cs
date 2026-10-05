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


#region --- License ---
/*
Copyright (c) 2006 - 2008 The Open Toolkit library.

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using ILNumerics.Drawing;
using System.Drawing; 

namespace ILNumerics.Drawing
{
    /// <summary>
    /// Represents a Quaternion.
    /// </summary>
    [Serializable]
    public struct Quaternion : IEquatable<Quaternion>
    {
        #region Fields

        Vector3 xyz;
        float w;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Quaternion from vector and w components
        /// </summary>
        /// <param name="v">The vector part</param>
        /// <param name="w">The w part</param>
        public Quaternion(Vector3 v, double w)
        {
            this.xyz = v;
            this.w = (float)w;
        }

        /// <summary>
        /// Construct a new Quaternion
        /// </summary>
        /// <param name="x">The x component</param>
        /// <param name="y">The y component</param>
        /// <param name="z">The z component</param>
        /// <param name="w">The w component</param>
        public Quaternion(float x, float y, float z, float w)
            : this(new Vector3(x, y, z), w)
        { }

        #endregion

        #region Public Members

        #region Properties


        /// <summary>
        /// Gets or sets an OpenTK.Vector3 with the X, Y and Z components of this instance.
        /// </summary>
        public Vector3 Xyz { get { return xyz; } set { xyz = value; } }

        /// <summary>
        /// Gets or sets the X component of this instance.
        /// </summary>
        public float X { get { return xyz.X; } set { xyz.X = value; } }

        /// <summary>
        /// Gets or sets the Y component of this instance.
        /// </summary>
        public float Y { get { return xyz.Y; } set { xyz.Y = value; } }

        /// <summary>
        /// Gets or sets the Z component of this instance.
        /// </summary>
        public float Z { get { return xyz.Z; } set { xyz.Z = value; } }

        /// <summary>
        /// Gets or sets the W component of this instance.
        /// </summary>
        public float W { get { return w; } set { w = value; } }

        #endregion

        #region Instance

        #region ToAxisAngle

        /// <summary>
        /// Convert the current quaternion to axis angle representation
        /// </summary>
        /// <param name="axis">The resultant axis</param>
        /// <param name="angle">The resultant angle</param>
        public void ToAxisAngle(out Vector3 axis, out float angle)
        {
            Vector4 result = ToAxisAngle();
            axis = result.Xyz;
            angle = result.W;
        }

        /// <summary>
        /// Convert this instance to an axis-angle representation.
        /// </summary>
        /// <returns>A Vector4 that is the axis-angle representation of this quaternion.</returns>
        public Vector4 ToAxisAngle()
        {
            Quaternion q = this;
            if (q.W > 1.0f)
                q.Normalize();

            Vector4 result = new Vector4();

            result.W = 2.0f * (float)System.Math.Acos(q.W); // angle
            float den = (float)System.Math.Sqrt(1.0 - q.W * q.W);
            if (den > 0.0001f)
            {
                result.Xyz = q.Xyz / den;
            }
            else
            {
                // This occurs when the angle is zero. 
                // Not a problem: just set an arbitrary normalized axis.
                result.Xyz = Vector3.UnitX;
            }

            return result;
        }

        #endregion

        #region public float Length
        // points: normalized screen coords, range [-1:1]
        public static Quaternion MoveSphere(System.Drawing.Point p1, System.Drawing.Point p2, Size scalingFact) {
            Vector3 v1 = getPointOnSphereNormalized(p1, scalingFact); 
            Vector3 v2 = getPointOnSphereNormalized(p2, scalingFact); 
            //System.Diagnostics.Debug.WriteLine("p1:{0} p2:{1}", v1,v2); 
            Vector3 n = Vector3.Normalize(Vector3.Cross(v1,v2)); 
            double theta05 = Math.Acos(Vector3.Dot(v1,v2)) / 4; 
            Quaternion ret = Quaternion.FromAxisAngle(n, (float)theta05); 
            return ret; 
        }
        private static Vector3 getPointOnSphereNormalized(System.Drawing.Point p, Size scalingFact) {
            double x = p.X / (double)scalingFact.Width * 2 - 1;
            double y = (scalingFact.Height - p.Y) / (double)scalingFact.Height * 2 - 1; 
            double x2y2 = x * x + y * y; 
            double z = (x2y2 <= 0.5) ? Math.Sqrt ( 1 - x2y2) : 0.5 / Math.Sqrt(x2y2); 
            Vector3 v1 = new Vector3(x,y,z); 
            v1.NormalizeFast(); 
            return v1; 
        }

        /// <summary>
        /// Gets the length (magnitude) of the quaternion.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(W * W + Xyz.LengthSquared);
            }
        }

        #endregion

        #region public float LengthSquared

        /// <summary>
        /// Gets the square of the quaternion length (magnitude).
        /// </summary>
        public float LengthSquared
        {
            get
            {
                return W * W + Xyz.LengthSquared;
            }
        }

        #endregion

        #region public void Normalize()

        /// <summary>
        /// Scales the Quaternion to unit length.
        /// </summary>
        public void Normalize()
        {
            float scale = 1.0f / this.Length;
            Xyz *= scale;
            W *= scale;
        }

        #endregion

        #region public void Conjugate()

        /// <summary>
        /// Convert this quaternion to its conjugate
        /// </summary>
        public void Conjugate()
        {
            Xyz = -Xyz;
        }

        #endregion

        #endregion

        #region Static

        #region Fields

        /// <summary>
        /// Defines the identity quaternion.
        /// </summary>
        public static Quaternion Identity = new Quaternion(0, 0, 0, 1);

        #endregion

        #region Add

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <returns>The result of the addition</returns>
        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        /// <summary>
        /// Add two quaternions
        /// </summary>
        /// <param name="left">The first operand</param>
        /// <param name="right">The second operand</param>
        /// <param name="result">The result of the addition</param>
        public static void Add(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        #endregion

        #region Sub

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>The result of the operation.</returns>
        public static Quaternion Sub(Quaternion left, Quaternion right)
        {
            return  new Quaternion(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <param name="result">The result of the operation.</param>
        public static void Sub(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        #endregion

        #region Mult

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        [Obsolete("Use Multiply instead.")]
        public static Quaternion Mult(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                right.W * left.Xyz + left.W * right.Xyz + Vector3.Cross(left.Xyz, right.Xyz),
                left.W * right.W - Vector3.Dot(left.Xyz, right.Xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        [Obsolete("Use Multiply instead.")]
        public static void Mult(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                right.W * left.Xyz + left.W * right.Xyz + Vector3.Cross(left.Xyz, right.Xyz),
                left.W * right.W - Vector3.Dot(left.Xyz, right.Xyz));
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            Quaternion result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref Quaternion left, ref Quaternion right, out Quaternion result)
        {
            result = new Quaternion(
                right.W * left.Xyz + left.W * right.Xyz + Vector3.Cross(left.Xyz, right.Xyz),
                left.W * right.W - Vector3.Dot(left.Xyz, right.Xyz));
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <param name="result">A new instance containing the result of the calculation.</param>
        public static void Multiply(ref Quaternion quaternion, float scale, out Quaternion result)
        {
            result = new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion Multiply(Quaternion quaternion, float scale)
        {
            return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        #endregion

        #region Conjugate

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <returns>The conjugate of the given quaternion</returns>
        public static Quaternion Conjugate(Quaternion q)
        {
            return new Quaternion(-q.Xyz, q.W);
        }

        /// <summary>
        /// Get the conjugate of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion</param>
        /// <param name="result">The conjugate of the given quaternion</param>
        public static void Conjugate(ref Quaternion q, out Quaternion result)
        {
            result = new Quaternion(-q.Xyz, q.W);
        }

        #endregion

        #region Invert

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The inverse of the given quaternion</returns>
        public static Quaternion Invert(Quaternion q)
        {
            Quaternion result;
            Invert(ref q, out result);
            return result;
        }

        /// <summary>
        /// Get the inverse of the given quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <param name="result">The inverse of the given quaternion</param>
        public static void Invert(ref Quaternion q, out Quaternion result)
        {
            float lengthSq = q.LengthSquared;
            if (lengthSq != 0.0)
            {
                float i = 1.0f / lengthSq;
                result = new Quaternion(q.Xyz * -i, q.W * i);
            }
            else
            {
                result = q;
            }
        }

        #endregion

        #region Normalize

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The normalized quaternion</returns>
        public static Quaternion Normalize(Quaternion q)
        {
            Quaternion result;
            Normalize(ref q, out result);
            return result;
        }

        /// <summary>
        /// Scale the given quaternion to unit length
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The normalized quaternion</param>
        public static void Normalize(ref Quaternion q, out Quaternion result)
        {
            float scale = 1.0f / q.Length;
            result = new Quaternion(q.Xyz * scale, q.W * scale);
        }

        #endregion

        #region FromAxisAngle

        /// <summary>
        /// Build a quaternion from the given axis and angle
        /// </summary>
        /// <param name="axis">The axis to rotate about</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns></returns>
        public static Quaternion FromAxisAngle(Vector3 axis, double angle)
        {
            if (axis.LengthSquared == 0.0f)
                return Identity;

            Quaternion result = Identity;

            angle *= 0.5f;
            axis.Normalize();
            result.Xyz = axis * (float)System.Math.Sin(angle);
            result.W = (float)System.Math.Cos(angle);

            return Normalize(result);
        }
        public static Quaternion FromAxisAngle(double x, double y, double z, double rad) {
            return FromAxisAngle(new Vector3((float)x, (float)y, (float)z), (float)rad); 
        }

        #endregion

        #region Slerp

        /// <summary>
        /// Do Spherical linear interpolation between two quaternions 
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second quaternion</param>
        /// <param name="blend">The blend factor</param>
        /// <returns>A smooth blend between the given quaternions</returns>
        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float blend)
        {
            // if either input is zero, return the other.
            if (q1.LengthSquared == 0.0f)
            {
                if (q2.LengthSquared == 0.0f)
                {
                    return Identity;
                }
                return q2;
            }
            else if (q2.LengthSquared == 0.0f)
            {
                return q1;
            }


            float cosHalfAngle = q1.W * q2.W + Vector3.Dot(q1.Xyz, q2.Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                // angle = 0.0f, so just return one input.
                return q1;
            }
            else if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                // do proper slerp for big angles
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                // do lerp if angle is really small.
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaternion result = new Quaternion(blendA * q1.Xyz + blendB * q2.Xyz, blendA * q1.W + blendB * q2.W);
            if (result.LengthSquared > 0.0f)
                return Normalize(result);
            else
                return Identity;
        }

        #endregion

        #region FromMatrix
         /// <summary>
        /// Build a quaternion from the specified rotation matrix.
        /// </summary>
        /// <param name="m">Matrix to translate</param>
        /// <param name="q">Quaternion result</param>
        public static void CreateFromMatrix(ref Matrix4 m, ref Quaternion q)
        {
            float trace = 1 + m.M11 + m.M22 + m.M33;
			float S = 0;
			float X = 0;
			float Y = 0;
			float Z = 0;
			float W = 0;
			
			if (trace > 0.0000001) 
			{
				S = (float)Math.Sqrt(trace) * 2;
				X = (m.M23 - m.M32) / S;
				Y = (m.M31 - m.M13) / S;
				Z = (m.M12 - m.M21) / S;
				W = 0.25f * S;
			} 
			else 
			{
				if (m.M11 > m.M22 && m.M11 > m.M33) 
				{
					// Column 0: 
					S = (float)Math.Sqrt(1.0 + m.M11 - m.M22 - m.M33) * 2;
					X = 0.25f * S;
					Y = (m.M12 + m.M21) / S;
					Z = (m.M31 + m.M13) / S;
					W = (m.M23 - m.M32) / S;
				} 
				else if (m.M22 > m.M33) 
				{
					// Column 1: 
					S = (float)Math.Sqrt(1.0 + m.M22 - m.M11 - m.M33) * 2;
					X = (m.M12 + m.M21) / S;
					Y = 0.25f * S;
					Z = (m.M23 + m.M32) / S;
					W = (m.M31 - m.M13) / S;
				} 
				else 
				{
					// Column 2:
					S = (float)Math.Sqrt(1.0 + m.M33 - m.M11 - m.M22) * 2;
					X = (m.M31 + m.M13) / S;
					Y = (m.M23 + m.M32) / S;
					Z = 0.25f * S;
					W = (m.M12 - m.M21) / S;
				}
			}
			q = new Quaternion(X, Y, Z, W);
        }

        /// <summary>
        /// Build a quaternion from the specified rotation matrix.
        /// </summary>
        /// <param name="m">Matrix to translate.</param>
        /// <returns>A quaternion</returns>
        public static Quaternion CreateFromMatrix(ref Matrix4 m)
        {
        	Quaternion ret = default(Quaternion);
            CreateFromMatrix(ref m, ref ret);
            return ret; 
        }
        #endregion     

        #endregion

        #region Operators

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            left.Xyz += right.Xyz;
            left.W += right.W;
            return left;
        }

        /// <summary>
        /// Subtracts two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            left.Xyz -= right.Xyz;
            left.W -= right.W;
            return left;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>The result of the calculation.</returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            Multiply(ref left, ref right, out left);
            return left;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(Quaternion quaternion, float scale)
        {
            Multiply(ref quaternion, scale, out quaternion);
            return quaternion;
        }

        /// <summary>
        /// Multiplies an instance by a scalar.
        /// </summary>
        /// <param name="quaternion">The instance.</param>
        /// <param name="scale">The scalar.</param>
        /// <returns>A new instance containing the result of the calculation.</returns>
        public static Quaternion operator *(float scale, Quaternion quaternion)
        {
            return new Quaternion(quaternion.X * scale, quaternion.Y * scale, quaternion.Z * scale, quaternion.W * scale);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Quaternion.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("V: {0}, W: {1}", Xyz, W);
        }

        #endregion

        #region public override bool Equals (object o)

        /// <summary>
        /// Compares this object instance to another object for equality. 
        /// </summary>
        /// <param name="other">The other object to be used in the comparison.</param>
        /// <returns>True if both objects are Quaternions of equal value. Otherwise it returns false.</returns>
        public override bool Equals(object other)
        {
            if (other is Quaternion == false) return false;
               return this == (Quaternion)other;
        }

        #endregion

        #region public override int GetHashCode ()

        /// <summary>
        /// Provides the hash code for this object. 
        /// </summary>
        /// <returns>A hash code formed from the bitwise XOR of this objects members.</returns>
        public override int GetHashCode()
        {
            return Xyz.GetHashCode() ^ W.GetHashCode();
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Quaternion> Members

        /// <summary>
        /// Compares this Quaternion instance to another Quaternion for equality. 
        /// </summary>
        /// <param name="other">The other Quaternion to be used in the comparison.</param>
        /// <returns>True if both instances are equal; false otherwise.</returns>
        public bool Equals(Quaternion other)
        {
            return Xyz == other.Xyz && W == other.W;
        }

        #endregion

    }
}
