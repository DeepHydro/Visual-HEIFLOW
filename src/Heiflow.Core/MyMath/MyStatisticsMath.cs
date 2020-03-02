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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;

namespace Heiflow.Core.MyMath
{
    public enum DeviationType
    {
        Population,
        Sample
    }

    ///<Summary>
    ///C# Math Class for Computing Standard Deviation, Normal Distribution, Probability Density
    ///Will Be Used In C# Naive Bayes Data Mining Algorithm for Classifying Numeric or Continuous Data
    ///</Summary>
    ///<Remarks>
    ///C# Probability Density Function or Normal Distrubution from 
    ///http://www.experts-exchange.com/Programming/Programming_Languages/C_Sharp/Q_20936306.html 
    ///C# Mean and C# Standard Deviation Functions from Daniel Olson at
    ///http://authors.aspalliance.com/olson/
    ///</Remarks>
    public class MyStatisticsMath
    {
        ///<Summary>
        ///Main function used to test or execute class functions from the command prompt
        ///</Summary>
        public static void Mainx()
        {
            //Test Standard Deviation Computation
            double[] arrayOfDoubles = { 2.5, 2.6, 2.8, 3.2, 3.8, 3.9, 4.0, 4.3, 4.4, 4.8 };

            double stdDev = StandardDeviation(arrayOfDoubles);

            Console.WriteLine("Standard Deviation Of Array Of Doubles Is : {0}", stdDev.ToString());

            //Test Probability Density or Normal Distribution Computation
            Console.WriteLine("Normal Distribution or Probability Density Is : {0}", NormalDistribution(42, 40, 1.5));

            Console.ReadLine();
        }

        public static StatisticsInfo SimpleStatistics(float[, ,] values, int layerIndex, int zindex, float noDataValue)
        {
            StatisticsInfo info = new StatisticsInfo();
            info.Max = float.NegativeInfinity;
            info.Min = float.PositiveInfinity;
            info.NoDataValue = noDataValue;
            info.Average = 0;
            double sum = 0;
            int count = 0;
            int row = values.GetLength(0);
            int col = values.GetLength(1);
            for (int i = 0; i < row; i++)
            {

                float f = values[i, layerIndex, zindex];
                if (f == noDataValue)
                {
                    continue;
                }
                if (f > info.Max)
                {
                    info.Max = f;
                }
                if (f < info.Min)
                {
                    info.Min = f;
                }
                sum += f;
                count++;
            }

            if (count > 0)
                info.Average = sum / count;
            return info;
        }

        public static StatisticsInfo ComplexStatistics(float[, ,] values, int layerIndex, int zindex, float noDataValue)
        {
            StatisticsInfo info = SimpleStatistics(values, layerIndex, zindex, noDataValue);
            info.StandardDeviation = StandardDeviation(values, layerIndex, zindex);
            return info;
        }

        public static StatisticsInfo ComplexStatistics(float[,] values)
        {
            StatisticsInfo info = SimpleStatistics(values, float.NaN);
            info.StandardDeviation = StandardDeviation(values);
            return info;
        }

        public static StatisticsInfo SimpleStatistics(float[,] values, float noDataValue)
        {
            StatisticsInfo info = new StatisticsInfo();
            info.Max = float.PositiveInfinity;
            info.Min = float.NegativeInfinity;
            info.NoDataValue = noDataValue;
            info.Average = 0;
            double sum = 0;
            int count = 0;
            foreach (float f in values)
            {
                if (f == noDataValue)
                {
                    continue;
                }
                if (f > info.Max)
                {
                    info.Max = f;
                }
                if (f < info.Min)
                {
                    info.Min = f;
                }
                sum += f;
                count++;
            }
            if (count > 0)
                info.Average = sum / count;
            return info;
        }

        public static StatisticsInfo SimpleStatistics(double[,] values, double noDataValue)
        {
            StatisticsInfo info = new StatisticsInfo();
            info.Max = float.PositiveInfinity;
            info.Min = float.NegativeInfinity;
            info.NoDataValue = noDataValue;
            info.Average = 0;
            double sum = 0;
            int count = 0;
            foreach (double f in values)
            {
                if (f == noDataValue)
                {
                    continue;
                }
                if (f > info.Max)
                {
                    info.Max = f;
                }
                if (f < info.Min)
                {
                    info.Min = f;
                }
                sum += f;
                count++;
            }
            if (count > 0)
                info.Average = sum / count;
            return info;
        }

        ///<Summary>
        ///Calculates standard deviation of numbers of doubles data type in an array
        ///</Summary>  
        public static double StandardDeviation(double[] num)
        {
            double Sum = 0.0, SumOfSqrs = 0.0;
            for (int i = 0; i < num.Length; i++)
            {
                Sum += num[i];
                SumOfSqrs += Math.Pow(num[i], 2);
            }
            double topSum = (num.Length * SumOfSqrs) - (Math.Pow(Sum, 2));
            double n = (double)num.Length;
            return Math.Sqrt(topSum / (n * (n - 1)));
        }

        public static float StandardDeviation(float[] num)
        {
            double Sum = 0.0, SumOfSqrs = 0.0;
            for (int i = 0; i < num.Length; i++)
            {
                Sum += num[i];
                SumOfSqrs += Math.Pow(num[i], 2);
            }
            double topSum = (num.Length * SumOfSqrs) - (Math.Pow(Sum, 2));
            double n = (double)num.Length;
            return (float)Math.Sqrt(topSum / (n * (n - 1)));
        }


        ///<Summary>
        ///Calculates standard deviation of numbers of doubles data type in a column of an array
        ///</Summary>  
        public static double StandardDeviation(double[,] num, int col)
        {
            double Sum = 0.0, SumOfSqrs = 0.0;
            int len = num.GetLength(0);
            for (int i = 0; i < len; i++)
            {
                Sum += num[i, col];
                SumOfSqrs += Math.Pow(num[i, col], 2);
            }
            double topSum = (len * SumOfSqrs) - (Math.Pow(Sum, 2));
            double n = System.Convert.ToDouble(len);
            return Math.Sqrt(topSum / (n * (n - 1)));
        }

        ///<Summary>
        ///Calculates standard deviation of numbers of doubles data type in a column of an array
        ///</Summary>  
        public static double StandardDeviation(double[,] numl)
        {
            double Sum = 0.0, SumOfSqrs = 0.0;
            int len = 0;
            foreach (double d in numl)
            {
                Sum += d;
                SumOfSqrs += Math.Pow(d, 2);
                len++;
            }
            double topSum = (len * SumOfSqrs) - (Math.Pow(Sum, 2));
            double n = System.Convert.ToDouble(len);
            return Math.Sqrt(topSum / (n * (n - 1)));
        }

        ///<Summary>
        ///Calculates standard deviation of numbers of doubles data type in a column of an array
        ///</Summary>  
        public static float StandardDeviation(float[,] numl)
        {
            float Sum = 0.0f, SumOfSqrs = 0.0f;
            int len = 0;
            foreach (float d in numl)
            {
                Sum += d;
                SumOfSqrs += (float)Math.Pow(d, 2);
                len++;
            }
            float topSum = (float)((len * SumOfSqrs) - (Math.Pow(Sum, 2)));
            float n = (float)len;
            return (float)Math.Sqrt(topSum / (n * (n - 1)));
        }


        ///<Summary>
        ///Calculates standard deviation of numbers of doubles data type in a column of an array
        ///</Summary>  
        public static float StandardDeviation(float[, ,] numl, int y, int z)
        {
            float Sum = 0.0f, SumOfSqrs = 0.0f;
            int len = 0;
            int x = numl.GetLength(0);
            for (int i = 0; i < x; i++)
            {
                Sum += numl[i, y, z];
                SumOfSqrs += (float)Math.Pow(numl[i, y, z], 2);
                len++;
            }

            float topSum = (float)((len * SumOfSqrs) - (Math.Pow(Sum, 2)));
            float n = (float)len;
            return (float)Math.Sqrt(topSum / (n * (n - 1)));
        }

        ///<Summary>
        ///Calculates standard deviation of numbers of doubles data type in a column of an array
        ///</Summary>  
        public static float StandardDeviation(short[,] numl)
        {
            float Sum = 0.0f, SumOfSqrs = 0.0f;
            int len = 0;
            foreach (short d in numl)
            {
                Sum += d;
                SumOfSqrs += (float)Math.Pow(d, 2);
                len++;
            }
            float topSum = (float)((len * SumOfSqrs) - (Math.Pow(Sum, 2)));
            float n = (float)len;
            return (float)Math.Sqrt(topSum / (n * (n - 1)));
        }

        ///<Summary>
        ///Calculates average of numbers of doubles data type in an array 
        ///</Summary>  
        public static double Average(double[,] num)
        {

            double sum = 0.0;
            int len = 0;
            foreach (double d in num)
            {
                sum += d;
                len++;
            }
            double avg = sum / len;

            return avg;
        }

        ///<Summary>
        ///Calculates average of numbers of doubles data type in an array 
        ///</Summary>  
        public static double Average(short[,] num)
        {
            double sum = 0.0;
            int len = 0;
            foreach (short d in num)
            {
                sum += d;
                len++;
            }
            double avg = sum / len;
            return avg;
        }

        ///<Summary>
        ///Calculates average of numbers of doubles data type in an array 
        ///</Summary>  
        public static float Average(float[,] num)
        {

            float sum = 0.0f;
            int len = 0;
            foreach (float d in num)
            {
                sum += d;
                len++;
            }
            float avg = sum / len;

            return avg;
        }

        ///<Summary>
        ///Calculates average of numbers of doubles data type in an array 
        ///</Summary>  
        public static double Average(double[] num)
        {
            double sum = 0.0;
            for (int i = 0; i < num.Length; i++)
            {
                sum += num[i];
            }
            double avg = sum / System.Convert.ToDouble(num.Length);

            return avg;
        }

        ///<Summary>
        ///Calculates average of numbers of integer data type in an array 
        ///</Summary>    
        public static double Average(int[] num)
        {
            double sum = 0.0;
            for (int i = 0; i < num.Length; i++)
            {
                sum += num[i];
            }
            double avg = sum / System.Convert.ToDouble(num.Length);

            return avg;
        }

        /// <summary> 
        /// Calculates Normal Distribution or Probability Density given the mean, and standard deviation 
        /// </summary> 
        /// <param name="x">The value for which you want the distribution.</param> 
        /// <param name="mean">The arithmetic mean of the distribution.</param> 
        /// <param name="deviation">The standard deviation of the distribution.</param> 
        /// <returns>Returns the normal distribution for the specified mean and standard deviation.</returns> 
        public static double NormalDistribution(double x, double mean, double deviation)
        {
            return NormalDensity(x, mean, deviation);
        }
        private static double NormalDensity(double x, double mean, double deviation)
        {
            return Math.Exp(-(Math.Pow((x - mean) / deviation, 2) / 2)) / Math.Sqrt(2 * Math.PI) / deviation;
        }

        public static StatisticsInfo SimpleStatistics(float[] vector)
        {
            StatisticsInfo info = new StatisticsInfo();
            info.Max = vector.Max();
            info.Min = vector.Min();
            info.Average = vector.Average();
            info.StandardDeviation = StandardDeviation(vector);
            info.Count = vector.Length;
            info.Sum = vector.Sum();
            return info;
        }

        public static StatisticsInfo SimpleStatistics(double[] vector)
        {
            StatisticsInfo info = new StatisticsInfo();
            info.Max = vector.Max();
            info.Min = vector.Min();
            info.Average = vector.Average();
            info.StandardDeviation = StandardDeviation(vector);
            info.Count = vector.Length;
            return info;
        }


        public static double Mse(double[] x, double[] y)
        {
            double result = 0;
            if (x != null && y != null && x.Length == y.Length)
            {
                int n = x.Length;
                double sum = 0;
                for (int i = 0; i < n; i++)
                {
                    sum += (x[i] - y[i]) * (x[i] - y[i]);
                }
                result = sum / n;
            }
            return result;
        }

        public static double RMse(double[] x, double[] y)
        {
            double result = 0;
            if (x != null && y != null && x.Length == y.Length)
            {
                double mse = Mse(x, y);
                result = Math.Sqrt(mse);
            }
            return result;
        }

        /// <summary>
        /// For each data point, this method calculates the value of  y from the formula. 
        /// It subtracts this from the data's y-value and squares the difference. 
        /// All these squares are added up and the sum is divided by the number of data.
        /// </summary>
        /// <returns></returns>
        public static double RootMeanSquaredError(double[] x, double[] y)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += (x[i] - y[i]) * (x[i] - y[i]);
            }
            sum = sum / x.Length;
            return Math.Sqrt(sum);
        }

        public static double Var(double[] list)
        {
            try
            {
                double s = 0;
                for (int i = 0; i <= list.Length - 1; i++)
                    s += Math.Pow(list[i], 2);
                return (s - list.Length * Math.Pow(list.Average(), 2)) / (list.Length - 1);
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }

        public static double S(double[] list)
        {
            return Math.Sqrt(Var(list));
        }

        /// <summary>
        /// compute the covariance
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static double Cov(double[] s1, double[] s2)
        {
            try
            {
                if (s1.Length != s2.Length) return double.NaN;
                int len = s1.Length;
                double sum_mul = 0;
                for (int i = 0; i <= len - 1; i++)
                    sum_mul += (s1[i] * s2[i]);
                return (sum_mul - len * s1.Average() * s2.Average()) / (len - 1);
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }
        /// <summary>
        /// Compute the correlation coefficient
        /// </summary>
        /// <param name="design1"></param>
        /// <param name="design2"></param>
        /// <returns></returns>
        public static double Correlation(double[] design1, double[] design2)
        {
            try
            {
                return Cov(design1, design2) / (S(design1) * S(design2));
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Returns the pearson correlation for two N-dimensional double vectors. 
        /// </summary>
        /// 
        /// <param name="p">1st point vector.</param>
        /// <param name="q">2nd point vector.</param>
        /// 
        /// <returns>Returns Pearson correlation between two supplied vectors.</returns>
        /// 
        /// <exception cref="ArgumentException">Thrown if the two vectors are of different dimensions (if specified
        /// array have different length).</exception>
        /// 
        public static double GetSimilarityScore(double[] p, double[] q)
        {
            double pSum = 0, qSum = 0, pSumSq = 0, qSumSq = 0, productSum = 0;
            double pValue, qValue;
            int n = p.Length;

            if (n != q.Length)
                throw new ArgumentException("Input vectors must be of the same dimension.");

            for (int x = 0; x < n; x++)
            {
                pValue = p[x];
                qValue = q[x];

                pSum += pValue;
                qSum += qValue;
                pSumSq += pValue * pValue;
                qSumSq += qValue * qValue;
                productSum += pValue * qValue;
            }

            double numerator = productSum - ((pSum * qSum) / (double)n);
            double denominator = Math.Sqrt((pSumSq - (pSum * pSum) / (double)n) * (qSumSq - (qSum * qSum) / (double)n));

            return (denominator == 0) ? 0 : numerator / denominator;
        }
        /// <summary>
        /// Compute Nash-Stucliffe model efficiency
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double NashStucliffeR(double[] x, double[] y)
        {
            double result = 0;
            if (x != null && y != null && x.Length == y.Length)
            {
                double average = x.Average();
                double sumX = 0;
                double sumY = 0;
                int n = x.Length;
                for (int i = 0; i < n; i++)
                {
                    sumX += (x[i] - y[i]) * (x[i] - y[i]);
                    sumY += (x[i] - average) * (x[i] - average);
                }
                result = 1 - sumX / sumY;
            }
            return result;
        }

        /// <summary>
        /// Mean absolute relative errors. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double MARE(double[] observed, double[] estimated)
        {
            double mare = 0;
            double are = 0;
            for (int i = 0; i < observed.Length; i++)
            {
                if (observed[i] != 0)
                    are += Math.Abs(observed[i] - estimated[i]) / observed[i];
            }
            mare = are / observed.Length;
            return mare;
        }

        /// <summary>
        /// Calculate fist order distance
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] GetFirstOrderDistance(double[] data)
        {
            double mean = data.Average();
            double[] result = new double[data.Length];
            int i = 0;
            foreach (double d in data)
            {
                result[i] = d - mean;
                i++;
            }
            return result;
        }

        public double GetCorrelation(double[] num1, double[] num2)
        {
            double denominator = (num1.Length - 1) * GetStandardDeviation(num1) *
            GetStandardDeviation(num2);
            double sumxy = 0;
            for (int i = 0; i < num1.Length; i++)
            {
                sumxy += num1[i] * num2[i];
            }
            double numerator = sumxy - num1.Length * num1.Average() * num2.Average();
            return numerator / denominator;
        }

        public double GetStandardDeviation(double[] num)
        {
            double SumOfSqrs = 0;
            double avg = num.Average();
            for (int i = 0; i < num.Length; i++)
            {
                SumOfSqrs += Math.Pow(((double)num[i] - avg), 2);
            }
            double n = (double)num.Length;
            return Math.Sqrt(SumOfSqrs / (n - 1));
        }

        public static double Deviation(double[] Values, DeviationType CalculationType)
        {
            double SumOfValuesSquared = 0;
            double SumOfValues = 0;
            //Calculate the sum of all the values
            foreach (double item in Values)
            {
                SumOfValues += item;
            }
            //Calculate the sum of all the values squared
            foreach (double item in Values)
            {
                SumOfValuesSquared += Math.Pow(item, 2);
            }
            if (CalculationType == DeviationType.Sample)
            {
                return Math.Sqrt((SumOfValuesSquared - Math.Pow(SumOfValues, 2) / Values.Length) / (Values.Length - 1));
            }
            else
            {
                return Math.Sqrt((SumOfValuesSquared - Math.Pow(SumOfValues, 2) / Values.Length) / Values.Length);
            }
        }

        /// <summary>
        /// Fits a line to a collection of (x,y) points.
        /// </summary>
        /// <param name="xVals">The x-axis values.</param>
        /// <param name="yVals">The y-axis values.</param>
        /// <param name="inclusiveStart">The inclusive inclusiveStart index.</param>
        /// <param name="exclusiveEnd">The exclusive exclusiveEnd index.</param>
        /// <param name="rsquared">The r^2 value of the line.</param>
        /// <param name="yintercept">The y-intercept value of the line (i.e. y = ax + b, yintercept is b).</param>
        /// <param name="slope">The slop of the line (i.e. y = ax + b, slope is a).</param>
        public static void LinearRegression(double[] xVals, double[] yVals,
                                            int inclusiveStart, int exclusiveEnd,
                                            out double rsquared, out double yintercept,
                                            out double slope)
        {
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = exclusiveEnd - inclusiveStart;

            for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
             * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }

        public static float PseudoMedian(float[] array)
        {
            int length = array.Length;
            float[] sort = new float[length];
            Array.Copy(array, 0, sort, 0, sort.Length);
            Array.Sort(sort);
            return sort[sort.Length / 2];
        }
        public static float Median(float[] array)
        {
            int length = array.Length;
            float[] sort = new float[length];
            Array.Copy(array, 0, sort, 0, sort.Length);
            Array.Sort(sort);
            
            if (length % 2 == 0)
            {
                return (sort[(sort.Length / 2) - 1] + sort[sort.Length / 2]) / 2;
            }
            else
            {
                return sort[sort.Length / 2];
            }
        }
        public static int Median(int[] array)
        {
            int length = array.Length;
            int[] sort = new int[length];
            Array.Copy(array, 0, sort, 0, sort.Length);
            Array.Sort(sort);

            if (length % 2 == 0)
            {
                return (sort[(sort.Length / 2) - 1] + sort[sort.Length / 2]) / 2;
            }
            else
            {
                return sort[sort.Length / 2];
            }
        }
        public static float Majority(params float[] x)
        {
            Dictionary<float, float> d = new Dictionary<float, float>();
            int majority = x.Length / 2;

            //Stores the number of occcurences of each item in the passed array in a dictionary
            foreach (int i in x)
                if (d.ContainsKey(i))
                {
                    d[i]++;
                    //Checks if element just added is the majority element
                    if (d[i] > majority)
                        return i;
                }
                else
                    d.Add(i, 1);

            return x[0];
        }
        public static double Majority(params double[] x)
        {
            Dictionary<double, double> d = new Dictionary<double, double>();
            int majority = x.Length / 2;

            //Stores the number of occcurences of each item in the passed array in a dictionary
            foreach (int i in x)
                if (d.ContainsKey(i))
                {
                    d[i]++;
                    //Checks if element just added is the majority element
                    if (d[i] > majority)
                        return i;
                }
                else
                    d.Add(i, 1);

            return x[0];
        }
        public static int Majority(params int[] x)
        {
            Dictionary<int, int> d = new Dictionary<int, int>();
            int majority = x.Length / 2;

            //Stores the number of occcurences of each item in the passed array in a dictionary
            foreach (int i in x)
                if (d.ContainsKey(i))
                {
                    d[i]++;
                    //Checks if element just added is the majority element
                    if (d[i] > majority)
                        return i;
                }
                else
                    d.Add(i, 1);

            return x[0];
        }
    }
}

