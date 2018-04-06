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

namespace  Heiflow.AI.Genetic
{
	using System;
	using  Heiflow.AI;

	/// <summary>
	/// Fitness function for times series prediction problem
	/// </summary>
	/// 
	/// <remarks><para>The fitness function calculates fitness value of
	/// <see cref="GPTreeChromosome">GP</see> and <see cref="GEPChromosome">GEP</see>
	/// chromosomes with the aim of solving times series prediction problem using
	/// sliding window method. The fitness function's value is computed as:
	/// <code>100.0 / ( error + 1 )</code>
	/// where <b>error</b> equals to the sum of absolute differences between predicted value
    /// and actual future value.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // number of points from the past used to predict new one
    /// int windowSize = 5;
    ///	// time series to predict
    ///	double[] data = new double[13] { 1, 2, 4, 7, 11, 16, 22, 29, 37, 46, 56, 67, 79 };
    ///	// constants
    ///	double[] constants = new double[10] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23 };
    ///	// create population
    ///	Population population = new Population( 100,
    ///	new GPTreeChromosome( new SimpleGeneFunction( windowSize + constants.Length ) ),
    ///	new TimeSeriesPredictionFitness( data, windowSize, 1, constants ),
    ///	new EliteSelection( ) );
    ///	// run one epoch of the population
    ///	population.RunEpoch( );
    /// </code>
    /// </remarks>
	/// 
	public class TimeSeriesPredictionFitness : IFitnessFunction
	{
		// time series data
		private double[]	data;
		// varibles
		private double[]	variables;
		// window size
		private int			windowSize;
		// prediction size
		private int			predictionSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeSeriesPredictionFitness"/> class.
		/// </summary>
		/// 
		/// <param name="data">Time series to be predicted.</param>
		/// <param name="windowSize">Window size - number of past samples used
		/// to predict future value.</param>
		/// <param name="predictionSize">Prediction size - number of values to be predicted. These
		/// values are excluded from training set.</param>
		/// <param name="constants">Array of constants to be used as additional
		/// paramters for genetic expression.</param>
		/// 
		/// <remarks><para>The <paramref name="data"/> parameter is a one dimensional array, which defines times
		/// series to predict. The amount of learning samples is equal to the number of samples
		/// in the provided time series, minus window size, minus prediction size.</para>
        /// 
        /// <para>The <paramref name="predictionSize"/> parameter specifies the amount of samples, which should
		/// be excluded from training set. This set of samples may be used for future verification
		/// of the prediction model.</para>
        /// 
        /// <para>The <paramref name="constants"/> parameter is an array of constants, which can be used as
		/// additional variables for a genetic expression. The actual amount of variables for
		/// genetic expression equals to the amount of constants plus the window size.</para>
		/// </remarks>
		/// 
		public TimeSeriesPredictionFitness( double[] data, int windowSize, int predictionSize, double[] constants )
		{
			// check for correct parameters
			if ( windowSize >= data.Length )
				throw new ArgumentException( "Window size should be less then data amount" );
			if ( data.Length - windowSize - predictionSize < 1 )
				throw new ArgumentException( "Data size should be enough for window and prediction" );
			// save parameters
			this.data			= data;
			this.windowSize		= windowSize;
			this.predictionSize	= predictionSize;
			// copy constants
			variables = new double[constants.Length + windowSize];
			Array.Copy( constants, 0, variables, windowSize, constants.Length );
		}

        /// <summary>
        /// Evaluates chromosome.
        /// </summary>
        /// 
        /// <param name="chromosome">Chromosome to evaluate.</param>
        /// 
        /// <returns>Returns chromosome's fitness value.</returns>
        ///
        /// <remarks>The method calculates fitness value of the specified
        /// chromosome.</remarks>
        ///
		public double Evaluate( IChromosome chromosome )
		{
			// get function in polish notation
			string function = chromosome.ToString( );

			// go through all the data
			double error = 0.0;
			for ( int i = 0, n = data.Length - windowSize - predictionSize; i < n; i++ )
			{
				// put values from current window as variables
				for ( int j = 0, b = i + windowSize - 1; j < windowSize; j++ )
				{
					variables[j] = data[b - j];
				}

				// avoid evaluation errors
				try
				{
					// evaluate the function
					double y = PolishExpression.Evaluate( function, variables );
					// check for correct numeric value
					if ( double.IsNaN( y ) )
						return 0;
					// get the difference between evaluated value and
					// next value after the window, and sum error
					error += Math.Abs( y - data[i + windowSize] );
				}
				catch
				{
					return 0;
				}
			}

			// return optimization function value
			return 100.0 / ( error + 1 );
		}

		/// <summary>
		/// Translates genotype to phenotype.
		/// </summary>
		/// 
		/// <param name="chromosome">Chromosome, which genoteype should be
		/// translated to phenotype.</param>
		///
		/// <returns>Returns chromosome's fenotype - the actual solution
		/// encoded by the chromosome.</returns> 
		/// 
		/// <remarks><para>The method returns string value, which represents prediction
        /// expression written in polish postfix notation.</para>
        /// 
        /// <para>The interpretation of the prediction expression is very simple. For example, let's
		/// take a look at sample expression, which was received with window size equal to 5:
		/// <code lang="none">$0 $1 - $5 / $2 *</code>
		/// The above expression in postfix polish notation should be interpreted as a next expression:
		/// <code lang="none">( ( x[t - 1] - x[t - 2] ) / const1 ) * x[t - 3]</code>
        /// </para>
		/// </remarks>
		///
		public string Translate( IChromosome chromosome )
		{
			// return polish notation for now ...
			return chromosome.ToString( );
		}
	}
}
