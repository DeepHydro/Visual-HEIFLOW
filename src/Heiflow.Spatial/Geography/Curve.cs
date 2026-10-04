//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// A Curve is a one-dimensional geometric object usually stored as a sequence of points,
    /// with the subtype of Curve specifying the form of the interpolation between points.
    /// </summary>
    public abstract class Curve : Geometry
    {
        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public override int Dimension
        {
            get { return 1; }
        }

        /// <summary>
        /// The length of this Curve in its associated spatial reference.
        /// </summary>
        public abstract double Length { get; }

        /// <summary>
        /// The start point of this Curve.
        /// </summary>
        public abstract Point StartPoint { get; }

        /// <summary>
        /// The end point of this Curve.
        /// </summary>
        public abstract Point EndPoint { get; }

        /// <summary>
        /// Returns true if this Curve is closed (StartPoint = EndPoint).
        /// </summary>
        public bool IsClosed
        {
            get { return (StartPoint.Equals(EndPoint)); }
        }

        /// <summary>
        /// true if this Curve is closed (StartPoint = EndPoint) and
        /// this Curve is simple (does not pass through the same point more than once).
        /// </summary>
        public abstract bool IsRing { get; }

        /// <summary>
        /// The position of a point on the line, parameterised by length.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract Point Value(double t);
    }
}