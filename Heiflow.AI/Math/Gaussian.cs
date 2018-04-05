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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

namespace  Heiflow.AI.Math
{
    using System;

    /// <summary>
    /// Gaussian function.
    /// </summary>
    /// 
    /// <remarks><para>The class is used to calculate 1D and 2D Gaussian functions for
    /// specified <see cref="Sigma"/> (s) value:</para>
    /// 
    /// <code lang="none">
    /// 1-D: f(x) = exp( x * x / ( -2 * s * s ) ) / ( s * sqrt( 2 * PI ) )
    /// 
    /// 2-D: f(x, y) = exp( x * x + y * y / ( -2 * s * s ) ) / ( s * s * 2 * PI )
    /// </code>
    /// 
    /// </remarks>
    /// 
    public class Gaussian
    {
        // sigma value
        private double sigma = 1.0;
        // squared sigma
        private double sqrSigma = 1.0;

        /// <summary>
        /// Sigma value.
        /// </summary>
        /// 
        /// <remarks><para>Sigma property of Gaussian function.</para>
        /// 
        /// <para>Default value is set to <b>1</b>. Minimum allowed value is <b>0.00000001</b>.</para>
        /// </remarks>
        /// 
        public double Sigma
        {
            get { return sigma; }
            set
            {
                sigma = Math.Max( 0.00000001, value );
                sqrSigma = sigma * sigma;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gaussian"/> class.
        /// </summary>
        /// 
        public Gaussian( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gaussian"/> class.
        /// </summary>
        /// 
        /// <param name="sigma">Sigma value.</param>
        /// 
        public Gaussian( double sigma )
        {
            Sigma = sigma;
        }

        /// <summary>
        /// 1-D Gaussian function.
        /// </summary>
        /// 
        /// <param name="x">x value.</param>
        /// 
        /// <returns>Returns function's value at point <paramref name="x"/>.</returns>
        /// 
        /// <remarks><para>The function calculates 1-D Gaussian function:</para>
        /// 
        /// <code lang="none">
        /// f(x) = exp( x * x / ( -2 * s * s ) ) / ( s * sqrt( 2 * PI ) )
        /// </code>
        /// </remarks>
        /// 
        public double Function( double x )
        {
            return Math.Exp( x * x / ( -2 * sqrSigma ) ) / ( Math.Sqrt( 2 * Math.PI ) * sigma );
        }

        /// <summary>
        /// 2-D Gaussian function.
        /// </summary>
        /// 
        /// <param name="x">x value.</param>
        /// <param name="y">y value.</param>
        /// 
        /// <returns>Returns function's value at point (<paramref name="x"/>, <paramref name="y"/>).</returns>
        /// 
        /// <remarks><para>The function calculates 2-D Gaussian function:</para>
        /// 
        /// <code lang="none">
        /// f(x, y) = exp( x * x + y * y / ( -2 * s * s ) ) / ( s * s * 2 * PI )
        /// </code>
        /// </remarks>
        /// 
        public double Function2D( double x, double y )
        {
            return Math.Exp( ( x * x + y * y ) / ( -2 * sqrSigma ) ) / ( 2 * Math.PI * sqrSigma );
        }

        /// <summary>
        /// 1-D Gaussian kernel.
        /// </summary>
        /// 
        /// <param name="size">Kernel size (should be odd), [3, 101].</param>
        /// 
        /// <returns>Returns 1-D Gaussian kernel of the specified size.</returns>
        /// 
        /// <remarks><para>The function calculates 1-D Gaussian kernel, which is array
        /// of Gaussian function's values in the [-r, r] range of x value, where
        /// r=floor(<paramref name="size"/>/2).
        /// </para></remarks>
        /// 
        /// <exception cref="ArgumentException">Wrong kernel size.</exception>
        /// 
        public double[] Kernel( int size )
        {
            // check for evem size and for out of range
            if ( ( ( size % 2 ) == 0 ) || ( size < 3 ) || ( size > 101 ) )
            {
                throw new ArgumentException( "Wrong kernal size." );
            }

            // raduis
            int r = size / 2;
            // kernel
            double[] kernel = new double[size];

            // compute kernel
            for ( int x = -r, i = 0; i < size; x++, i++ )
            {
                kernel[i] = Function( x );
            }

            return kernel;
        }

        /// <summary>
        /// 2-D Gaussian kernel.
        /// </summary>
        /// 
        /// <param name="size">Kernel size (should be odd), [3, 101].</param>
        /// 
        /// <returns>Returns 2-D Gaussian kernel of specified size.</returns>
        /// 
        /// <remarks><para>The function calculates 2-D Gaussian kernel, which is array
        /// of Gaussian function's values in the [-r, r] range of x,y values, where
        /// r=floor(<paramref name="size"/>/2).
        /// </para></remarks>
        /// 
        /// <exception cref="ArgumentException">Wrong kernel size.</exception>
        /// 
        public double[,] Kernel2D( int size )
        {
            // check for evem size and for out of range
            if ( ( ( size % 2 ) == 0 ) || ( size < 3 ) || ( size > 101 ) )
            {
                throw new ArgumentException( "Wrong kernal size." );
            }

            // raduis
            int r = size / 2;
            // kernel
            double[,] kernel = new double[size, size];

            // compute kernel
            for ( int y = -r, i = 0; i < size; y++, i++ )
            {
                for ( int x = -r, j = 0; j < size; x++, j++ )
                {
                    kernel[i, j] = Function2D( x, y );
                }
            }

            return kernel;
        }
    }
}
