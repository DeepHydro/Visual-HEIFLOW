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
using System.Text;
using System.Threading; 
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;



namespace ILNumerics  {
	public partial class ILMath {

		/// <summary>
		/// Sum elements along first non singleton dimension ignoring NaN values
		/// </summary>
		/// <param name="A">Input array</param>
		/// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
		/// <returns>Sum of elements along specified of first non singleton dimension, ignoring nan values</returns>
		/// <remarks><para>The array returned will have the same size as <paramref name="A"/>, with the specified or first non singleton dimension 
		/// reduced to the length 1. It will contain the sum of all elements along that dimension after removing NaN values respectively. </para>
		/// <para>If A contains an all NaN vector along <paramref name="dim"/> , 
		/// the resulting sum will be 0 - not NaN! This corresponds to the sum of an empty vector.</para></remarks> 
		public static ILRetArray<double> nansum(ILInArray<double> A, int dim = -1)
		{
			if (dim < 0)
				dim = A.Size.WorkingDimension();
			return nansum_internal(A, dim, false);
		}
		
		/// <summary>
		/// Depending on settings, calculate nansum or nanmean
		/// </summary>
		private static ILRetArray<double> nansum_internal (ILInArray<double> A, int dim, bool computeMean) {
			using (ILScope.Enter(A)) {
				if (dim >= A.Size.NumberOfDimensions)
				   return A.C;
				
				if (A.IsScalar) {
					return array<double>(new double[1] { A.GetValue(0) }, 1, 1);
				}
				
				if (A.S[dim] == 1) return A.C;

				int[] newDims = A.S.ToIntArray();
				newDims[dim] = 1; 
				ILSize retDimension = new ILSize(newDims);

				if (A.IsEmpty)
					return empty<double>(retDimension);

				double[] retArr = ILMemoryPool.Pool.New<double>(retDimension.NumberOfElements);

				int inc = A.Size.SequentialIndexDistance(dim);
				int dimLen = A.Size[dim];
				int maxRuns = retDimension.NumberOfElements;
				int modHelp = A.Size.NumberOfElements - 1;
				int modOut = retDimension.NumberOfElements - 1;
				int incOut = retDimension.SequentialIndexDistance(dim); 
				int numelA = A.S.NumberOfElements;
				
				double[] aArray = A.GetArrayForRead();
				if (maxRuns == 1) {
					
					double tmp = 0;
					if (computeMean)
					{
						int cnt = 0;
						for (int j = 0; j < dimLen; j++)
						{
							if (!/*HC:inArr1*/double.IsNaN(aArray[j]))
							{
								tmp += aArray[j];
								cnt++;
							}
						}
						if (cnt == 0)
							retArr[0] = double.NaN;
						else
							retArr[0] = tmp / cnt;
					}
					else
					{
						for (int j = 0; j < dimLen; j++)
						{
							if (!/*HC:inArr1*/double.IsNaN(aArray[j]))
								tmp += aArray[j];
						}
						retArr[0] = tmp;
					}
				} else {
					#region may run parallel 
					int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
					if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
						&& numelA / 2 >= Settings.s_minParallelElement1Count) {

						if (maxRuns >= Settings.s_maxNumberThreads
							&& numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
							workItemLength = maxRuns / workItemCount;
						} else {
							workItemLength = maxRuns / 2;
							workItemCount = 2;
						}
						
					} else {
						workItemLength = maxRuns;
						workItemCount = 1;
					}
					Action<object> action = (data) => {
						Tuple<int, int> range = (Tuple<int, int>)data;
						int from = range.Item1, to = range.Item2;
						for (int c = from; c < to; c++) {
							int pos = (int)(((long)dimLen * c * inc) % modHelp);
							long posOut = ((long)c * incOut);
							if (posOut > modOut)
								posOut = ((posOut - 1) % modOut) + 1;
							
							double tmp = 0;
							int end = pos + dimLen * inc;
							if (computeMean)
							{
								int cnt = 0;
								for (int j = pos; j < end; j += inc)
								{
									if (!/*HC:inArr1*/double.IsNaN(aArray[j]))
									{ 
										tmp += aArray[j];
										cnt++;
									}
								}
								if (cnt == 0)
									retArr[posOut] = double.NaN;
								else
									retArr[posOut] = tmp / cnt;
							}
							else
							{
							   
								for (int j = pos; j < end; j += inc)
								{
									if (!/*HC:inArr1*/double.IsNaN(aArray[j]))
										tmp += aArray[j];
								}
								retArr[posOut] = tmp;
							}
						}
						System.Threading.Interlocked.Decrement(ref workerCount);
					};
					for (; i < workItemCount - 1; i++) {
						Interlocked.Increment(ref workerCount);

						ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
					}
					action(Tuple.Create(i * workItemLength, maxRuns));
					ILThreadPool.Wait4Workers(ref workerCount); 
					#endregion
				}
				return array(retArr, newDims);
			}
		}

#region HYCALPER AUTO GENERATED CODE

		/// <summary>
		/// Sum elements along first non singleton dimension ignoring NaN values
		/// </summary>
		/// <param name="A">Input array</param>
		/// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
		/// <returns>Sum of elements along specified of first non singleton dimension, ignoring nan values</returns>
		/// <remarks><para>The array returned will have the same size as <paramref name="A"/>, with the specified or first non singleton dimension 
		/// reduced to the length 1. It will contain the sum of all elements along that dimension after removing NaN values respectively. </para>
		/// <para>If A contains an all NaN vector along <paramref name="dim"/> , 
		/// the resulting sum will be 0 - not NaN! This corresponds to the sum of an empty vector.</para></remarks> 
		public static ILRetArray<fcomplex> nansum(ILInArray<fcomplex> A, int dim = -1)
		{
			if (dim < 0)
				dim = A.Size.WorkingDimension();
			return nansum_internal(A, dim, false);
		}
		
		/// <summary>
		/// Depending on settings, calculate nansum or nanmean
		/// </summary>
		private static ILRetArray<fcomplex> nansum_internal (ILInArray<fcomplex> A, int dim, bool computeMean) {
			using (ILScope.Enter(A)) {
				if (dim >= A.Size.NumberOfDimensions)
				   return A.C;
				
				if (A.IsScalar) {
					return array<fcomplex>(new fcomplex[1] { A.GetValue(0) }, 1, 1);
				}
				
				if (A.S[dim] == 1) return A.C;

				int[] newDims = A.S.ToIntArray();
				newDims[dim] = 1; 
				ILSize retDimension = new ILSize(newDims);

				if (A.IsEmpty)
					return empty<fcomplex>(retDimension);

				fcomplex[] retArr = ILMemoryPool.Pool.New<fcomplex>(retDimension.NumberOfElements);

				int inc = A.Size.SequentialIndexDistance(dim);
				int dimLen = A.Size[dim];
				int maxRuns = retDimension.NumberOfElements;
				int modHelp = A.Size.NumberOfElements - 1;
				int modOut = retDimension.NumberOfElements - 1;
				int incOut = retDimension.SequentialIndexDistance(dim); 
				int numelA = A.S.NumberOfElements;
				
				fcomplex[] aArray = A.GetArrayForRead();
				if (maxRuns == 1) {
					
					fcomplex tmp = 0;
					if (computeMean)
					{
						int cnt = 0;
						for (int j = 0; j < dimLen; j++)
						{
							if (!/*HC:*/fcomplex.IsNaN(aArray[j]))
							{
								tmp += aArray[j];
								cnt++;
							}
						}
						if (cnt == 0)
							retArr[0] = fcomplex.NaN;
						else
							retArr[0] = tmp / cnt;
					}
					else
					{
						for (int j = 0; j < dimLen; j++)
						{
							if (!/*HC:*/fcomplex.IsNaN(aArray[j]))
								tmp += aArray[j];
						}
						retArr[0] = tmp;
					}
				} else {
					#region may run parallel 
					int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
					if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
						&& numelA / 2 >= Settings.s_minParallelElement1Count) {

						if (maxRuns >= Settings.s_maxNumberThreads
							&& numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
							workItemLength = maxRuns / workItemCount;
						} else {
							workItemLength = maxRuns / 2;
							workItemCount = 2;
						}
						
					} else {
						workItemLength = maxRuns;
						workItemCount = 1;
					}
					Action<object> action = (data) => {
						Tuple<int, int> range = (Tuple<int, int>)data;
						int from = range.Item1, to = range.Item2;
						for (int c = from; c < to; c++) {
							int pos = (int)(((long)dimLen * c * inc) % modHelp);
							long posOut = ((long)c * incOut);
							if (posOut > modOut)
								posOut = ((posOut - 1) % modOut) + 1;
							
							fcomplex tmp = 0;
							int end = pos + dimLen * inc;
							if (computeMean)
							{
								int cnt = 0;
								for (int j = pos; j < end; j += inc)
								{
									if (!/*HC:*/fcomplex.IsNaN(aArray[j]))
									{ 
										tmp += aArray[j];
										cnt++;
									}
								}
								if (cnt == 0)
									retArr[posOut] = fcomplex.NaN;
								else
									retArr[posOut] = tmp / cnt;
							}
							else
							{
							   
								for (int j = pos; j < end; j += inc)
								{
									if (!/*HC:*/fcomplex.IsNaN(aArray[j]))
										tmp += aArray[j];
								}
								retArr[posOut] = tmp;
							}
						}
						System.Threading.Interlocked.Decrement(ref workerCount);
					};
					for (; i < workItemCount - 1; i++) {
						Interlocked.Increment(ref workerCount);

						ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
					}
					action(Tuple.Create(i * workItemLength, maxRuns));
					ILThreadPool.Wait4Workers(ref workerCount); 
					#endregion
				}
				return array(retArr, newDims);
			}
		}
		/// <summary>
		/// Sum elements along first non singleton dimension ignoring NaN values
		/// </summary>
		/// <param name="A">Input array</param>
		/// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
		/// <returns>Sum of elements along specified of first non singleton dimension, ignoring nan values</returns>
		/// <remarks><para>The array returned will have the same size as <paramref name="A"/>, with the specified or first non singleton dimension 
		/// reduced to the length 1. It will contain the sum of all elements along that dimension after removing NaN values respectively. </para>
		/// <para>If A contains an all NaN vector along <paramref name="dim"/> , 
		/// the resulting sum will be 0 - not NaN! This corresponds to the sum of an empty vector.</para></remarks> 
		public static ILRetArray<float> nansum(ILInArray<float> A, int dim = -1)
		{
			if (dim < 0)
				dim = A.Size.WorkingDimension();
			return nansum_internal(A, dim, false);
		}
		
		/// <summary>
		/// Depending on settings, calculate nansum or nanmean
		/// </summary>
		private static ILRetArray<float> nansum_internal (ILInArray<float> A, int dim, bool computeMean) {
			using (ILScope.Enter(A)) {
				if (dim >= A.Size.NumberOfDimensions)
				   return A.C;
				
				if (A.IsScalar) {
					return array<float>(new float[1] { A.GetValue(0) }, 1, 1);
				}
				
				if (A.S[dim] == 1) return A.C;

				int[] newDims = A.S.ToIntArray();
				newDims[dim] = 1; 
				ILSize retDimension = new ILSize(newDims);

				if (A.IsEmpty)
					return empty<float>(retDimension);

				float[] retArr = ILMemoryPool.Pool.New<float>(retDimension.NumberOfElements);

				int inc = A.Size.SequentialIndexDistance(dim);
				int dimLen = A.Size[dim];
				int maxRuns = retDimension.NumberOfElements;
				int modHelp = A.Size.NumberOfElements - 1;
				int modOut = retDimension.NumberOfElements - 1;
				int incOut = retDimension.SequentialIndexDistance(dim); 
				int numelA = A.S.NumberOfElements;
				
				float[] aArray = A.GetArrayForRead();
				if (maxRuns == 1) {
					
					float tmp = 0;
					if (computeMean)
					{
						int cnt = 0;
						for (int j = 0; j < dimLen; j++)
						{
							if (!/*HC:*/float.IsNaN(aArray[j]))
							{
								tmp += aArray[j];
								cnt++;
							}
						}
						if (cnt == 0)
							retArr[0] = float.NaN;
						else
							retArr[0] = tmp / cnt;
					}
					else
					{
						for (int j = 0; j < dimLen; j++)
						{
							if (!/*HC:*/float.IsNaN(aArray[j]))
								tmp += aArray[j];
						}
						retArr[0] = tmp;
					}
				} else {
					#region may run parallel 
					int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
					if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
						&& numelA / 2 >= Settings.s_minParallelElement1Count) {

						if (maxRuns >= Settings.s_maxNumberThreads
							&& numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
							workItemLength = maxRuns / workItemCount;
						} else {
							workItemLength = maxRuns / 2;
							workItemCount = 2;
						}
						
					} else {
						workItemLength = maxRuns;
						workItemCount = 1;
					}
					Action<object> action = (data) => {
						Tuple<int, int> range = (Tuple<int, int>)data;
						int from = range.Item1, to = range.Item2;
						for (int c = from; c < to; c++) {
							int pos = (int)(((long)dimLen * c * inc) % modHelp);
							long posOut = ((long)c * incOut);
							if (posOut > modOut)
								posOut = ((posOut - 1) % modOut) + 1;
							
							float tmp = 0;
							int end = pos + dimLen * inc;
							if (computeMean)
							{
								int cnt = 0;
								for (int j = pos; j < end; j += inc)
								{
									if (!/*HC:*/float.IsNaN(aArray[j]))
									{ 
										tmp += aArray[j];
										cnt++;
									}
								}
								if (cnt == 0)
									retArr[posOut] = float.NaN;
								else
									retArr[posOut] = tmp / cnt;
							}
							else
							{
							   
								for (int j = pos; j < end; j += inc)
								{
									if (!/*HC:*/float.IsNaN(aArray[j]))
										tmp += aArray[j];
								}
								retArr[posOut] = tmp;
							}
						}
						System.Threading.Interlocked.Decrement(ref workerCount);
					};
					for (; i < workItemCount - 1; i++) {
						Interlocked.Increment(ref workerCount);

						ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
					}
					action(Tuple.Create(i * workItemLength, maxRuns));
					ILThreadPool.Wait4Workers(ref workerCount); 
					#endregion
				}
				return array(retArr, newDims);
			}
		}
		/// <summary>
		/// Sum elements along first non singleton dimension ignoring NaN values
		/// </summary>
		/// <param name="A">Input array</param>
		/// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
		/// <returns>Sum of elements along specified of first non singleton dimension, ignoring nan values</returns>
		/// <remarks><para>The array returned will have the same size as <paramref name="A"/>, with the specified or first non singleton dimension 
		/// reduced to the length 1. It will contain the sum of all elements along that dimension after removing NaN values respectively. </para>
		/// <para>If A contains an all NaN vector along <paramref name="dim"/> , 
		/// the resulting sum will be 0 - not NaN! This corresponds to the sum of an empty vector.</para></remarks> 
		public static ILRetArray<complex> nansum(ILInArray<complex> A, int dim = -1)
		{
			if (dim < 0)
				dim = A.Size.WorkingDimension();
			return nansum_internal(A, dim, false);
		}
		
		/// <summary>
		/// Depending on settings, calculate nansum or nanmean
		/// </summary>
		private static ILRetArray<complex> nansum_internal (ILInArray<complex> A, int dim, bool computeMean) {
			using (ILScope.Enter(A)) {
				if (dim >= A.Size.NumberOfDimensions)
				   return A.C;
				
				if (A.IsScalar) {
					return array<complex>(new complex[1] { A.GetValue(0) }, 1, 1);
				}
				
				if (A.S[dim] == 1) return A.C;

				int[] newDims = A.S.ToIntArray();
				newDims[dim] = 1; 
				ILSize retDimension = new ILSize(newDims);

				if (A.IsEmpty)
					return empty<complex>(retDimension);

				complex[] retArr = ILMemoryPool.Pool.New<complex>(retDimension.NumberOfElements);

				int inc = A.Size.SequentialIndexDistance(dim);
				int dimLen = A.Size[dim];
				int maxRuns = retDimension.NumberOfElements;
				int modHelp = A.Size.NumberOfElements - 1;
				int modOut = retDimension.NumberOfElements - 1;
				int incOut = retDimension.SequentialIndexDistance(dim); 
				int numelA = A.S.NumberOfElements;
				
				complex[] aArray = A.GetArrayForRead();
				if (maxRuns == 1) {
					
					complex tmp = 0;
					if (computeMean)
					{
						int cnt = 0;
						for (int j = 0; j < dimLen; j++)
						{
							if (!/*HC:*/complex.IsNaN(aArray[j]))
							{
								tmp += aArray[j];
								cnt++;
							}
						}
						if (cnt == 0)
							retArr[0] = complex.NaN;
						else
							retArr[0] = tmp / cnt;
					}
					else
					{
						for (int j = 0; j < dimLen; j++)
						{
							if (!/*HC:*/complex.IsNaN(aArray[j]))
								tmp += aArray[j];
						}
						retArr[0] = tmp;
					}
				} else {
					#region may run parallel 
					int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
					if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
						&& numelA / 2 >= Settings.s_minParallelElement1Count) {

						if (maxRuns >= Settings.s_maxNumberThreads
							&& numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
							workItemLength = maxRuns / workItemCount;
						} else {
							workItemLength = maxRuns / 2;
							workItemCount = 2;
						}
						
					} else {
						workItemLength = maxRuns;
						workItemCount = 1;
					}
					Action<object> action = (data) => {
						Tuple<int, int> range = (Tuple<int, int>)data;
						int from = range.Item1, to = range.Item2;
						for (int c = from; c < to; c++) {
							int pos = (int)(((long)dimLen * c * inc) % modHelp);
							long posOut = ((long)c * incOut);
							if (posOut > modOut)
								posOut = ((posOut - 1) % modOut) + 1;
							
							complex tmp = 0;
							int end = pos + dimLen * inc;
							if (computeMean)
							{
								int cnt = 0;
								for (int j = pos; j < end; j += inc)
								{
									if (!/*HC:*/complex.IsNaN(aArray[j]))
									{ 
										tmp += aArray[j];
										cnt++;
									}
								}
								if (cnt == 0)
									retArr[posOut] = complex.NaN;
								else
									retArr[posOut] = tmp / cnt;
							}
							else
							{
							   
								for (int j = pos; j < end; j += inc)
								{
									if (!/*HC:*/complex.IsNaN(aArray[j]))
										tmp += aArray[j];
								}
								retArr[posOut] = tmp;
							}
						}
						System.Threading.Interlocked.Decrement(ref workerCount);
					};
					for (; i < workItemCount - 1; i++) {
						Interlocked.Increment(ref workerCount);

						ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
					}
					action(Tuple.Create(i * workItemLength, maxRuns));
					ILThreadPool.Wait4Workers(ref workerCount); 
					#endregion
				}
				return array(retArr, newDims);
			}
		}

#endregion HYCALPER AUTO GENERATED CODE
}
}