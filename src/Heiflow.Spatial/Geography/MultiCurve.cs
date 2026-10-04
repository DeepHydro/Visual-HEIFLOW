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
    /// A MultiCurve is a one-dimensional GeometryCollection whose elements are Curves
    /// </summary>
    public abstract class MultiCurve : GeometryCollection
    {
        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public override int Dimension
        {
            get { return 1; }
        }

        /// <summary>
        /// Returns true if this MultiCurve is closed (StartPoint=EndPoint for each curve in this MultiCurve)
        /// </summary>
        public abstract bool IsClosed { get; }

        /// <summary>
        /// The Length of this MultiCurve which is equal to the sum of the lengths of the element Curves.
        /// </summary>
        public abstract double Length { get; }

        /// <summary>
        /// Returns the number of geometries in the collection.
        /// </summary>
        public new abstract int NumGeometries { get; }

        /// <summary>
        /// Returns an indexed geometry in the collection
        /// </summary>
        /// <param name="N">Geometry index</param>
        /// <returns>Geometry at index N</returns>
        public new abstract Geometry Geometry(int N);
    }
}