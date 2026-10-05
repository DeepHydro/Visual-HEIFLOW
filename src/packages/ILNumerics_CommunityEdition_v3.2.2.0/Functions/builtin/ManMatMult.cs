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
using ILNumerics.Misc;



namespace ILNumerics {

    public partial class ILMath {

        private static readonly uint ALIGN = 1024 * 4;

        internal struct MatMultArguments {
            public IntPtr pArr;
            public IntPtr pBrr;
            public IntPtr pCrr;
            public IntPtr pAPack;
            public IntPtr pBPack;
            public int m_start;
            public int m_end;
            public int n_start;
            public int n_end;
        }


        unsafe public static void MMultBlockedThreaded(double[] A, double[] B, double[] C, int m, int n, int k, int kc) {

            // block parameters
            int mc = 512;
            int mr = 4;
            int nr = 4;

            
            double[] CAux = ILMemoryPool.Pool.New<double>(mc + ALIGN / sizeof(double));
            
            double[] Bpack1 = ILMemoryPool.Pool.New<double>(kc * (n) + ALIGN / sizeof(double));
            
            double[] Apack1 = ILMemoryPool.Pool.New<double>(kc * mc + ALIGN / sizeof(double));
            
            double[] Bpack2 = ILMemoryPool.Pool.New<double>(kc * (n) + ALIGN / sizeof(double));
            
            double[] Apack2 = ILMemoryPool.Pool.New<double>(kc * mc + ALIGN / sizeof(double));
            fixed (double* pAArr = A)
            fixed (double* pBArr = B)
            fixed (double* pCArr = C)
            fixed (double* pBpack1 = Bpack1)
            fixed (double* pApack1 = Apack1)
            fixed (double* pBpack2 = Bpack2)
            fixed (double* pApack2 = Apack2) {


                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && m * n / 2 > Settings.s_minParallelElement3Count) {
                    if (m * n / workItemCount > Settings.s_minParallelElement3Count) {
                        workItemLength = n / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = n / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = n;
                    workItemCount = 1;
                }
                Action<object> func = data => {
                    MatMultArguments args = (MatMultArguments)data; 
                    inner_k_loop_managed(m, n, k, kc, mc, mr, nr,
                                 (double*)args.pArr, (double*)args.pBrr, (double*)args.pCrr, 
                                 (double*)args.pBPack, (double*)args.pAPack, 
                                 args.n_start, args.n_end, args.m_start, args.m_end);
                    //inner_k_loop(args.Item1, args.Item2, args.Item3, args.Item4, args.Item5, args.Item6, args.Item7,
                    //             args.Rest.Item1, args.Rest.Item2, args.Rest.Item3, args.Rest.Item4, args.Rest.Item5,
                    //             args.Rest.Item6, args.Rest.Item7);
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                workerCount = workItemCount - 1; 
                for (; i < workItemCount - 1; i++) {
                    MatMultArguments args4Thread; // = new MatMultArguments();
                    args4Thread.pArr = (IntPtr)pAArr;
                    args4Thread.pBrr = (IntPtr)pBArr;
                    args4Thread.pCrr = (IntPtr)pCArr;
                    args4Thread.pAPack = (IntPtr)pApack1;
                    args4Thread.pBPack = (IntPtr)pBpack1;
                    args4Thread.n_start = i * workItemLength;
                    args4Thread.n_end = (i + 1) * workItemLength;
                    args4Thread.m_start = 0;
                    args4Thread.m_end = m;
                    //System.Threading.Interlocked.Increment(ref workerCount);
                    ILThreadPool.QueueUserWorkItem(i, func, args4Thread); 
                }
                // the last (or may the only) chunk is done right here
                //inner_k_loop(m, n, k, kc, mc, mr, nr, (IntPtr)pAArr, (IntPtr)pBArr, (IntPtr)pCArr, (IntPtr)pBpack2, (IntPtr)pApack2, n / 2, n);
                inner_k_loop_managed(m, n, k, kc, mc, mr, nr, pAArr, pBArr, pCArr, pBpack2, pApack2, i * workItemLength, n, 0, m);
                ILThreadPool.Wait4Workers(ref workerCount);
            }

            ILMemoryPool.Pool.Free(CAux);
            ILMemoryPool.Pool.Free(Bpack1);
            ILMemoryPool.Pool.Free(Apack1);
            ILMemoryPool.Pool.Free(Bpack2);
            ILMemoryPool.Pool.Free(Apack2);
        }
        unsafe public static void MMultBlocked(double[] A, double[] B, double[] C, int m, int n, int k, int kc) {

            // block parameters
            int mc = 512;
            int mr = 4;
            int nr = 4;

            
            double[] CAux = ILMemoryPool.Pool.New<double>(mc + ALIGN / sizeof(double));
            
            double[] Bpack1 = ILMemoryPool.Pool.New<double>(kc * (n) + ALIGN / sizeof(double));
            
            double[] Apack1 = ILMemoryPool.Pool.New<double>(kc * mc + ALIGN / sizeof(double));
            
            double[] Bpack2 = ILMemoryPool.Pool.New<double>(kc * (n) + ALIGN / sizeof(double));
            
            double[] Apack2 = ILMemoryPool.Pool.New<double>(kc * mc + ALIGN / sizeof(double));
            fixed (double* pAArr = A)
            fixed (double* pBArr = B)
            fixed (double* pCArr = C)
            fixed (double* pBpack1 = Bpack1)
            fixed (double* pApack1 = Apack1)
            fixed (double* pBpack2 = Bpack2)
            fixed (double* pApack2 = Apack2) {
                inner_k_loop_managed(m, n, k, kc, mc, mr, nr, pAArr, pBArr, pCArr, pBpack2, pApack2, 0, n, 0, m);
            }
            ILMemoryPool.Pool.Free(CAux);
            ILMemoryPool.Pool.Free(Bpack1);
            ILMemoryPool.Pool.Free(Apack1);
            ILMemoryPool.Pool.Free(Bpack2);
            ILMemoryPool.Pool.Free(Apack2);
        }

        unsafe private static void inner_k_loop_managed(int m, int n, int k, int kc, int mc, int mr, int nr,
                                            double* pAArr, double* pBArr, double* pCArr, double* pBpack, double* pApack,
                                            int n_start, int n_end, int m_start, int m_end) {

            double* pApackTmp = (double*)((byte*)pApack + (ALIGN - ((uint)pApack % ALIGN)));
            double* pBpackTmp = (double*)((byte*)pBpack + (ALIGN - ((uint)pBpack % ALIGN)));
            double* pCArrTmp, pBArrTmp, pAArrTmp;

            int n_len = n_end - n_start;
            for (int ki = 0; ki < k; ki += kc) {
                if (k - ki < kc) kc = k - ki;

                #region pack B
                pBpackTmp = pBpack;
                for (int nb = 0; nb < n_len; nb++) {
                    pBArrTmp = pBArr + ki + k * (nb + n_start);
                    int c = 0;
                    for (; c < kc - 8; c += 8) {
                        pBpackTmp[0] = pBArrTmp[0];
                        pBpackTmp[1] = pBArrTmp[1];
                        pBpackTmp[2] = pBArrTmp[2];
                        pBpackTmp[3] = pBArrTmp[3];
                        pBpackTmp[4] = pBArrTmp[4];
                        pBpackTmp[5] = pBArrTmp[5];
                        pBpackTmp[6] = pBArrTmp[6];
                        pBpackTmp[7] = pBArrTmp[7];
                        pBpackTmp += 8; pBArrTmp += 8;
                    }
                    for (; c < kc; c++) {
                        *pBpackTmp++ = *pBArrTmp++;
                    }
                }
                //pack(BArr, Bpack, r, 0, kc, n, k);
                #endregion

                int mcc = mc;
                int m_len = m_end - m_start; 
                for (int ai = 0; ai < m_len; ai += mcc) {
                    if (m_len - ai < mcc) mcc = m_len - ai;

                    #region pack A
                    for (int ca = 0; ca < kc; ca++) {
                        pApackTmp = pApack + ca;
                        pAArrTmp = pAArr + ai + m * (m_start + ki + ca);
                        int ra = 0;
                        for (; ra < mcc - 8; ra += 8) {
                            pApackTmp[(ra) * kc] = pAArrTmp[0];
                            pApackTmp[(ra + 1) * kc] = pAArrTmp[1];
                            pApackTmp[(ra + 2) * kc] = pAArrTmp[2];
                            pApackTmp[(ra + 3) * kc] = pAArrTmp[3];
                            pApackTmp[(ra + 4) * kc] = pAArrTmp[4];
                            pApackTmp[(ra + 5) * kc] = pAArrTmp[5];
                            pApackTmp[(ra + 6) * kc] = pAArrTmp[6];
                            pApackTmp[(ra + 7) * kc] = pAArrTmp[7];
                            pAArrTmp += 8;
                        }
                        for (; ra < mcc; ra++) {
                            pApackTmp[ra * kc] = *pAArrTmp++;
                        }
                    }
                    #endregion

                    #region subblocked
                    int nrLen = nr;
                    for (int nri = 0; nri < n_len; nri += nrLen) {
                        if (n_len - nri < nrLen) nrLen = n_len - nri;
                        int mrLen = mr;
                        for (int mri = 0; mri < mcc; mri += mrLen) {
                            if (mcc - mri < mrLen) mrLen = mcc - mri;

                            // prefetch CAux 


                            if (false && mrLen == 4 && nrLen == 4) {

                            } else {
                                for (int nii = 0; nii < nrLen; nii++) {
                                    pCArrTmp = pCArr + ai + mri + (nri + nii + n_start) * m;
                                    //for (int mii = 0; mii < mrLen; mii++) {
                                    //    pCAux[mii] = pCArrTmp[mii];
                                    //}
                                    for (int mii = 0; mii < mrLen; mii++) {
                                        pApackTmp = pApack + (mri + mii) * kc; // <-- transposed packed!
                                        pBpackTmp = pBpack + (nri + nii) * kc;
                                        double sum = 0;
                                        int jj = 0;
                                        for (; jj < kc - 8; jj += 8) {
                                            sum += pApackTmp[0] * pBpackTmp[0]
                                                + pApackTmp[1] * pBpackTmp[1]
                                                + pApackTmp[2] * pBpackTmp[2]
                                                + pApackTmp[3] * pBpackTmp[3]
                                                + pApackTmp[4] * pBpackTmp[4]
                                                + pApackTmp[5] * pBpackTmp[5]
                                                + pApackTmp[6] * pBpackTmp[6]
                                                + pApackTmp[7] * pBpackTmp[7];
                                            pApackTmp += 8;
                                            pBpackTmp += 8;
                                        }
                                        for (; jj < kc; jj++) {
                                            sum += *pApackTmp++ * *pBpackTmp++;
                                        }
                                        //CAux[ra] = sum;
                                        pCArrTmp[mii] += sum;
                                    }
                                    //for (int mii = 0; mii < mrLen; mii++) {
                                    //    pCArrTmp[mii] = pCAux[mii];
                                    //}

                                }

                            }
                        }

                    }
                    #endregion
                }
            }
        }


#region HYCALPER AUTO GENERATED CODE

        unsafe public static void MMultBlockedThreaded(float[] A, float[] B, float[] C, int m, int n, int k, int kc) {

            // block parameters
            int mc = 512;
            int mr = 4;
            int nr = 4;

           
            float[] CAux = ILMemoryPool.Pool.New<float>(mc + ALIGN / sizeof(float));
           
            float[] Bpack1 = ILMemoryPool.Pool.New<float>(kc * (n) + ALIGN / sizeof(float));
           
            float[] Apack1 = ILMemoryPool.Pool.New<float>(kc * mc + ALIGN / sizeof(float));
           
            float[] Bpack2 = ILMemoryPool.Pool.New<float>(kc * (n) + ALIGN / sizeof(float));
           
            float[] Apack2 = ILMemoryPool.Pool.New<float>(kc * mc + ALIGN / sizeof(float));
            fixed (float* pAArr = A)
            fixed (float* pBArr = B)
            fixed (float* pCArr = C)
            fixed (float* pBpack1 = Bpack1)
            fixed (float* pApack1 = Apack1)
            fixed (float* pBpack2 = Bpack2)
            fixed (float* pApack2 = Apack2) {


                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && m * n / 2 > Settings.s_minParallelElement3Count) {
                    if (m * n / workItemCount > Settings.s_minParallelElement3Count) {
                        workItemLength = n / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = n / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = n;
                    workItemCount = 1;
                }
                Action<object> func = data => {
                    MatMultArguments args = (MatMultArguments)data; 
                    inner_k_loop_managed(m, n, k, kc, mc, mr, nr,
                                 (float*)args.pArr, (float*)args.pBrr, (float*)args.pCrr, 
                                 (float*)args.pBPack, (float*)args.pAPack, 
                                 args.n_start, args.n_end, args.m_start, args.m_end);
                    //inner_k_loop(args.Item1, args.Item2, args.Item3, args.Item4, args.Item5, args.Item6, args.Item7,
                    //             args.Rest.Item1, args.Rest.Item2, args.Rest.Item3, args.Rest.Item4, args.Rest.Item5,
                    //             args.Rest.Item6, args.Rest.Item7);
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                workerCount = workItemCount - 1; 
                for (; i < workItemCount - 1; i++) {
                    MatMultArguments args4Thread; // = new MatMultArguments();
                    args4Thread.pArr = (IntPtr)pAArr;
                    args4Thread.pBrr = (IntPtr)pBArr;
                    args4Thread.pCrr = (IntPtr)pCArr;
                    args4Thread.pAPack = (IntPtr)pApack1;
                    args4Thread.pBPack = (IntPtr)pBpack1;
                    args4Thread.n_start = i * workItemLength;
                    args4Thread.n_end = (i + 1) * workItemLength;
                    args4Thread.m_start = 0;
                    args4Thread.m_end = m;
                    //System.Threading.Interlocked.Increment(ref workerCount);
                    ILThreadPool.QueueUserWorkItem(i, func, args4Thread); 
                }
                // the last (or may the only) chunk is done right here
                //inner_k_loop(m, n, k, kc, mc, mr, nr, (IntPtr)pAArr, (IntPtr)pBArr, (IntPtr)pCArr, (IntPtr)pBpack2, (IntPtr)pApack2, n / 2, n);
                inner_k_loop_managed(m, n, k, kc, mc, mr, nr, pAArr, pBArr, pCArr, pBpack2, pApack2, i * workItemLength, n, 0, m);
                ILThreadPool.Wait4Workers(ref workerCount);
            }

            ILMemoryPool.Pool.Free(CAux);
            ILMemoryPool.Pool.Free(Bpack1);
            ILMemoryPool.Pool.Free(Apack1);
            ILMemoryPool.Pool.Free(Bpack2);
            ILMemoryPool.Pool.Free(Apack2);
        }
        unsafe public static void MMultBlocked(float[] A, float[] B, float[] C, int m, int n, int k, int kc) {

            // block parameters
            int mc = 512;
            int mr = 4;
            int nr = 4;

           
            float[] CAux = ILMemoryPool.Pool.New<float>(mc + ALIGN / sizeof(float));
           
            float[] Bpack1 = ILMemoryPool.Pool.New<float>(kc * (n) + ALIGN / sizeof(float));
           
            float[] Apack1 = ILMemoryPool.Pool.New<float>(kc * mc + ALIGN / sizeof(float));
           
            float[] Bpack2 = ILMemoryPool.Pool.New<float>(kc * (n) + ALIGN / sizeof(float));
           
            float[] Apack2 = ILMemoryPool.Pool.New<float>(kc * mc + ALIGN / sizeof(float));
            fixed (float* pAArr = A)
            fixed (float* pBArr = B)
            fixed (float* pCArr = C)
            fixed (float* pBpack1 = Bpack1)
            fixed (float* pApack1 = Apack1)
            fixed (float* pBpack2 = Bpack2)
            fixed (float* pApack2 = Apack2) {
                inner_k_loop_managed(m, n, k, kc, mc, mr, nr, pAArr, pBArr, pCArr, pBpack2, pApack2, 0, n, 0, m);
            }
            ILMemoryPool.Pool.Free(CAux);
            ILMemoryPool.Pool.Free(Bpack1);
            ILMemoryPool.Pool.Free(Apack1);
            ILMemoryPool.Pool.Free(Bpack2);
            ILMemoryPool.Pool.Free(Apack2);
        }

        unsafe private static void inner_k_loop_managed(int m, int n, int k, int kc, int mc, int mr, int nr,
                                           float* pAArr, float* pBArr, float* pCArr, float* pBpack, float* pApack,
                                            int n_start, int n_end, int m_start, int m_end) {

           float* pApackTmp = (float*)((byte*)pApack + (ALIGN - ((uint)pApack % ALIGN)));
           float* pBpackTmp = (float*)((byte*)pBpack + (ALIGN - ((uint)pBpack % ALIGN)));
           float* pCArrTmp, pBArrTmp, pAArrTmp;

            int n_len = n_end - n_start;
            for (int ki = 0; ki < k; ki += kc) {
                if (k - ki < kc) kc = k - ki;

                #region pack B
                pBpackTmp = pBpack;
                for (int nb = 0; nb < n_len; nb++) {
                    pBArrTmp = pBArr + ki + k * (nb + n_start);
                    int c = 0;
                    for (; c < kc - 8; c += 8) {
                        pBpackTmp[0] = pBArrTmp[0];
                        pBpackTmp[1] = pBArrTmp[1];
                        pBpackTmp[2] = pBArrTmp[2];
                        pBpackTmp[3] = pBArrTmp[3];
                        pBpackTmp[4] = pBArrTmp[4];
                        pBpackTmp[5] = pBArrTmp[5];
                        pBpackTmp[6] = pBArrTmp[6];
                        pBpackTmp[7] = pBArrTmp[7];
                        pBpackTmp += 8; pBArrTmp += 8;
                    }
                    for (; c < kc; c++) {
                        *pBpackTmp++ = *pBArrTmp++;
                    }
                }
                //pack(BArr, Bpack, r, 0, kc, n, k);
                #endregion

                int mcc = mc;
                int m_len = m_end - m_start; 
                for (int ai = 0; ai < m_len; ai += mcc) {
                    if (m_len - ai < mcc) mcc = m_len - ai;

                    #region pack A
                    for (int ca = 0; ca < kc; ca++) {
                        pApackTmp = pApack + ca;
                        pAArrTmp = pAArr + ai + m * (m_start + ki + ca);
                        int ra = 0;
                        for (; ra < mcc - 8; ra += 8) {
                            pApackTmp[(ra) * kc] = pAArrTmp[0];
                            pApackTmp[(ra + 1) * kc] = pAArrTmp[1];
                            pApackTmp[(ra + 2) * kc] = pAArrTmp[2];
                            pApackTmp[(ra + 3) * kc] = pAArrTmp[3];
                            pApackTmp[(ra + 4) * kc] = pAArrTmp[4];
                            pApackTmp[(ra + 5) * kc] = pAArrTmp[5];
                            pApackTmp[(ra + 6) * kc] = pAArrTmp[6];
                            pApackTmp[(ra + 7) * kc] = pAArrTmp[7];
                            pAArrTmp += 8;
                        }
                        for (; ra < mcc; ra++) {
                            pApackTmp[ra * kc] = *pAArrTmp++;
                        }
                    }
                    #endregion

                    #region subblocked
                    int nrLen = nr;
                    for (int nri = 0; nri < n_len; nri += nrLen) {
                        if (n_len - nri < nrLen) nrLen = n_len - nri;
                        int mrLen = mr;
                        for (int mri = 0; mri < mcc; mri += mrLen) {
                            if (mcc - mri < mrLen) mrLen = mcc - mri;

                            // prefetch CAux 


                            if (false && mrLen == 4 && nrLen == 4) {

                            } else {
                                for (int nii = 0; nii < nrLen; nii++) {
                                    pCArrTmp = pCArr + ai + mri + (nri + nii + n_start) * m;
                                    //for (int mii = 0; mii < mrLen; mii++) {
                                    //    pCAux[mii] = pCArrTmp[mii];
                                    //}
                                    for (int mii = 0; mii < mrLen; mii++) {
                                        pApackTmp = pApack + (mri + mii) * kc; // <-- transposed packed!
                                        pBpackTmp = pBpack + (nri + nii) * kc;
                                       float sum = 0;
                                        int jj = 0;
                                        for (; jj < kc - 8; jj += 8) {
                                            sum += pApackTmp[0] * pBpackTmp[0]
                                                + pApackTmp[1] * pBpackTmp[1]
                                                + pApackTmp[2] * pBpackTmp[2]
                                                + pApackTmp[3] * pBpackTmp[3]
                                                + pApackTmp[4] * pBpackTmp[4]
                                                + pApackTmp[5] * pBpackTmp[5]
                                                + pApackTmp[6] * pBpackTmp[6]
                                                + pApackTmp[7] * pBpackTmp[7];
                                            pApackTmp += 8;
                                            pBpackTmp += 8;
                                        }
                                        for (; jj < kc; jj++) {
                                            sum += *pApackTmp++ * *pBpackTmp++;
                                        }
                                        //CAux[ra] = sum;
                                        pCArrTmp[mii] += sum;
                                    }
                                    //for (int mii = 0; mii < mrLen; mii++) {
                                    //    pCArrTmp[mii] = pCAux[mii];
                                    //}

                                }

                            }
                        }

                    }
                    #endregion
                }
            }
        }

        unsafe public static void MMultBlockedThreaded(fcomplex[] A, fcomplex[] B, fcomplex[] C, int m, int n, int k, int kc) {

            // block parameters
            int mc = 512;
            int mr = 4;
            int nr = 4;

           
            fcomplex[] CAux = ILMemoryPool.Pool.New<fcomplex>(mc + ALIGN / sizeof(fcomplex));
           
            fcomplex[] Bpack1 = ILMemoryPool.Pool.New<fcomplex>(kc * (n) + ALIGN / sizeof(fcomplex));
           
            fcomplex[] Apack1 = ILMemoryPool.Pool.New<fcomplex>(kc * mc + ALIGN / sizeof(fcomplex));
           
            fcomplex[] Bpack2 = ILMemoryPool.Pool.New<fcomplex>(kc * (n) + ALIGN / sizeof(fcomplex));
           
            fcomplex[] Apack2 = ILMemoryPool.Pool.New<fcomplex>(kc * mc + ALIGN / sizeof(fcomplex));
            fixed (fcomplex* pAArr = A)
            fixed (fcomplex* pBArr = B)
            fixed (fcomplex* pCArr = C)
            fixed (fcomplex* pBpack1 = Bpack1)
            fixed (fcomplex* pApack1 = Apack1)
            fixed (fcomplex* pBpack2 = Bpack2)
            fixed (fcomplex* pApack2 = Apack2) {


                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && m * n / 2 > Settings.s_minParallelElement3Count) {
                    if (m * n / workItemCount > Settings.s_minParallelElement3Count) {
                        workItemLength = n / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = n / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = n;
                    workItemCount = 1;
                }
                Action<object> func = data => {
                    MatMultArguments args = (MatMultArguments)data; 
                    inner_k_loop_managed(m, n, k, kc, mc, mr, nr,
                                 (fcomplex*)args.pArr, (fcomplex*)args.pBrr, (fcomplex*)args.pCrr, 
                                 (fcomplex*)args.pBPack, (fcomplex*)args.pAPack, 
                                 args.n_start, args.n_end, args.m_start, args.m_end);
                    //inner_k_loop(args.Item1, args.Item2, args.Item3, args.Item4, args.Item5, args.Item6, args.Item7,
                    //             args.Rest.Item1, args.Rest.Item2, args.Rest.Item3, args.Rest.Item4, args.Rest.Item5,
                    //             args.Rest.Item6, args.Rest.Item7);
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                workerCount = workItemCount - 1; 
                for (; i < workItemCount - 1; i++) {
                    MatMultArguments args4Thread; // = new MatMultArguments();
                    args4Thread.pArr = (IntPtr)pAArr;
                    args4Thread.pBrr = (IntPtr)pBArr;
                    args4Thread.pCrr = (IntPtr)pCArr;
                    args4Thread.pAPack = (IntPtr)pApack1;
                    args4Thread.pBPack = (IntPtr)pBpack1;
                    args4Thread.n_start = i * workItemLength;
                    args4Thread.n_end = (i + 1) * workItemLength;
                    args4Thread.m_start = 0;
                    args4Thread.m_end = m;
                    //System.Threading.Interlocked.Increment(ref workerCount);
                    ILThreadPool.QueueUserWorkItem(i, func, args4Thread); 
                }
                // the last (or may the only) chunk is done right here
                //inner_k_loop(m, n, k, kc, mc, mr, nr, (IntPtr)pAArr, (IntPtr)pBArr, (IntPtr)pCArr, (IntPtr)pBpack2, (IntPtr)pApack2, n / 2, n);
                inner_k_loop_managed(m, n, k, kc, mc, mr, nr, pAArr, pBArr, pCArr, pBpack2, pApack2, i * workItemLength, n, 0, m);
                ILThreadPool.Wait4Workers(ref workerCount);
            }

            ILMemoryPool.Pool.Free(CAux);
            ILMemoryPool.Pool.Free(Bpack1);
            ILMemoryPool.Pool.Free(Apack1);
            ILMemoryPool.Pool.Free(Bpack2);
            ILMemoryPool.Pool.Free(Apack2);
        }
        unsafe public static void MMultBlocked(fcomplex[] A, fcomplex[] B, fcomplex[] C, int m, int n, int k, int kc) {

            // block parameters
            int mc = 512;
            int mr = 4;
            int nr = 4;

           
            fcomplex[] CAux = ILMemoryPool.Pool.New<fcomplex>(mc + ALIGN / sizeof(fcomplex));
           
            fcomplex[] Bpack1 = ILMemoryPool.Pool.New<fcomplex>(kc * (n) + ALIGN / sizeof(fcomplex));
           
            fcomplex[] Apack1 = ILMemoryPool.Pool.New<fcomplex>(kc * mc + ALIGN / sizeof(fcomplex));
           
            fcomplex[] Bpack2 = ILMemoryPool.Pool.New<fcomplex>(kc * (n) + ALIGN / sizeof(fcomplex));
           
            fcomplex[] Apack2 = ILMemoryPool.Pool.New<fcomplex>(kc * mc + ALIGN / sizeof(fcomplex));
            fixed (fcomplex* pAArr = A)
            fixed (fcomplex* pBArr = B)
            fixed (fcomplex* pCArr = C)
            fixed (fcomplex* pBpack1 = Bpack1)
            fixed (fcomplex* pApack1 = Apack1)
            fixed (fcomplex* pBpack2 = Bpack2)
            fixed (fcomplex* pApack2 = Apack2) {
                inner_k_loop_managed(m, n, k, kc, mc, mr, nr, pAArr, pBArr, pCArr, pBpack2, pApack2, 0, n, 0, m);
            }
            ILMemoryPool.Pool.Free(CAux);
            ILMemoryPool.Pool.Free(Bpack1);
            ILMemoryPool.Pool.Free(Apack1);
            ILMemoryPool.Pool.Free(Bpack2);
            ILMemoryPool.Pool.Free(Apack2);
        }

        unsafe private static void inner_k_loop_managed(int m, int n, int k, int kc, int mc, int mr, int nr,
                                           fcomplex* pAArr, fcomplex* pBArr, fcomplex* pCArr, fcomplex* pBpack, fcomplex* pApack,
                                            int n_start, int n_end, int m_start, int m_end) {

           fcomplex* pApackTmp = (fcomplex*)((byte*)pApack + (ALIGN - ((uint)pApack % ALIGN)));
           fcomplex* pBpackTmp = (fcomplex*)((byte*)pBpack + (ALIGN - ((uint)pBpack % ALIGN)));
           fcomplex* pCArrTmp, pBArrTmp, pAArrTmp;

            int n_len = n_end - n_start;
            for (int ki = 0; ki < k; ki += kc) {
                if (k - ki < kc) kc = k - ki;

                #region pack B
                pBpackTmp = pBpack;
                for (int nb = 0; nb < n_len; nb++) {
                    pBArrTmp = pBArr + ki + k * (nb + n_start);
                    int c = 0;
                    for (; c < kc - 8; c += 8) {
                        pBpackTmp[0] = pBArrTmp[0];
                        pBpackTmp[1] = pBArrTmp[1];
                        pBpackTmp[2] = pBArrTmp[2];
                        pBpackTmp[3] = pBArrTmp[3];
                        pBpackTmp[4] = pBArrTmp[4];
                        pBpackTmp[5] = pBArrTmp[5];
                        pBpackTmp[6] = pBArrTmp[6];
                        pBpackTmp[7] = pBArrTmp[7];
                        pBpackTmp += 8; pBArrTmp += 8;
                    }
                    for (; c < kc; c++) {
                        *pBpackTmp++ = *pBArrTmp++;
                    }
                }
                //pack(BArr, Bpack, r, 0, kc, n, k);
                #endregion

                int mcc = mc;
                int m_len = m_end - m_start; 
                for (int ai = 0; ai < m_len; ai += mcc) {
                    if (m_len - ai < mcc) mcc = m_len - ai;

                    #region pack A
                    for (int ca = 0; ca < kc; ca++) {
                        pApackTmp = pApack + ca;
                        pAArrTmp = pAArr + ai + m * (m_start + ki + ca);
                        int ra = 0;
                        for (; ra < mcc - 8; ra += 8) {
                            pApackTmp[(ra) * kc] = pAArrTmp[0];
                            pApackTmp[(ra + 1) * kc] = pAArrTmp[1];
                            pApackTmp[(ra + 2) * kc] = pAArrTmp[2];
                            pApackTmp[(ra + 3) * kc] = pAArrTmp[3];
                            pApackTmp[(ra + 4) * kc] = pAArrTmp[4];
                            pApackTmp[(ra + 5) * kc] = pAArrTmp[5];
                            pApackTmp[(ra + 6) * kc] = pAArrTmp[6];
                            pApackTmp[(ra + 7) * kc] = pAArrTmp[7];
                            pAArrTmp += 8;
                        }
                        for (; ra < mcc; ra++) {
                            pApackTmp[ra * kc] = *pAArrTmp++;
                        }
                    }
                    #endregion

                    #region subblocked
                    int nrLen = nr;
                    for (int nri = 0; nri < n_len; nri += nrLen) {
                        if (n_len - nri < nrLen) nrLen = n_len - nri;
                        int mrLen = mr;
                        for (int mri = 0; mri < mcc; mri += mrLen) {
                            if (mcc - mri < mrLen) mrLen = mcc - mri;

                            // prefetch CAux 


                            if (false && mrLen == 4 && nrLen == 4) {

                            } else {
                                for (int nii = 0; nii < nrLen; nii++) {
                                    pCArrTmp = pCArr + ai + mri + (nri + nii + n_start) * m;
                                    //for (int mii = 0; mii < mrLen; mii++) {
                                    //    pCAux[mii] = pCArrTmp[mii];
                                    //}
                                    for (int mii = 0; mii < mrLen; mii++) {
                                        pApackTmp = pApack + (mri + mii) * kc; // <-- transposed packed!
                                        pBpackTmp = pBpack + (nri + nii) * kc;
                                       fcomplex sum = 0;
                                        int jj = 0;
                                        for (; jj < kc - 8; jj += 8) {
                                            sum += pApackTmp[0] * pBpackTmp[0]
                                                + pApackTmp[1] * pBpackTmp[1]
                                                + pApackTmp[2] * pBpackTmp[2]
                                                + pApackTmp[3] * pBpackTmp[3]
                                                + pApackTmp[4] * pBpackTmp[4]
                                                + pApackTmp[5] * pBpackTmp[5]
                                                + pApackTmp[6] * pBpackTmp[6]
                                                + pApackTmp[7] * pBpackTmp[7];
                                            pApackTmp += 8;
                                            pBpackTmp += 8;
                                        }
                                        for (; jj < kc; jj++) {
                                            sum += *pApackTmp++ * *pBpackTmp++;
                                        }
                                        //CAux[ra] = sum;
                                        pCArrTmp[mii] += sum;
                                    }
                                    //for (int mii = 0; mii < mrLen; mii++) {
                                    //    pCArrTmp[mii] = pCAux[mii];
                                    //}

                                }

                            }
                        }

                    }
                    #endregion
                }
            }
        }

        unsafe public static void MMultBlockedThreaded(complex[] A, complex[] B, complex[] C, int m, int n, int k, int kc) {

            // block parameters
            int mc = 512;
            int mr = 4;
            int nr = 4;

           
            complex[] CAux = ILMemoryPool.Pool.New<complex>(mc + ALIGN / sizeof(complex));
           
            complex[] Bpack1 = ILMemoryPool.Pool.New<complex>(kc * (n) + ALIGN / sizeof(complex));
           
            complex[] Apack1 = ILMemoryPool.Pool.New<complex>(kc * mc + ALIGN / sizeof(complex));
           
            complex[] Bpack2 = ILMemoryPool.Pool.New<complex>(kc * (n) + ALIGN / sizeof(complex));
           
            complex[] Apack2 = ILMemoryPool.Pool.New<complex>(kc * mc + ALIGN / sizeof(complex));
            fixed (complex* pAArr = A)
            fixed (complex* pBArr = B)
            fixed (complex* pCArr = C)
            fixed (complex* pBpack1 = Bpack1)
            fixed (complex* pApack1 = Apack1)
            fixed (complex* pBpack2 = Bpack2)
            fixed (complex* pApack2 = Apack2) {


                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && m * n / 2 > Settings.s_minParallelElement3Count) {
                    if (m * n / workItemCount > Settings.s_minParallelElement3Count) {
                        workItemLength = n / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = n / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = n;
                    workItemCount = 1;
                }
                Action<object> func = data => {
                    MatMultArguments args = (MatMultArguments)data; 
                    inner_k_loop_managed(m, n, k, kc, mc, mr, nr,
                                 (complex*)args.pArr, (complex*)args.pBrr, (complex*)args.pCrr, 
                                 (complex*)args.pBPack, (complex*)args.pAPack, 
                                 args.n_start, args.n_end, args.m_start, args.m_end);
                    //inner_k_loop(args.Item1, args.Item2, args.Item3, args.Item4, args.Item5, args.Item6, args.Item7,
                    //             args.Rest.Item1, args.Rest.Item2, args.Rest.Item3, args.Rest.Item4, args.Rest.Item5,
                    //             args.Rest.Item6, args.Rest.Item7);
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                workerCount = workItemCount - 1; 
                for (; i < workItemCount - 1; i++) {
                    MatMultArguments args4Thread; // = new MatMultArguments();
                    args4Thread.pArr = (IntPtr)pAArr;
                    args4Thread.pBrr = (IntPtr)pBArr;
                    args4Thread.pCrr = (IntPtr)pCArr;
                    args4Thread.pAPack = (IntPtr)pApack1;
                    args4Thread.pBPack = (IntPtr)pBpack1;
                    args4Thread.n_start = i * workItemLength;
                    args4Thread.n_end = (i + 1) * workItemLength;
                    args4Thread.m_start = 0;
                    args4Thread.m_end = m;
                    //System.Threading.Interlocked.Increment(ref workerCount);
                    ILThreadPool.QueueUserWorkItem(i, func, args4Thread); 
                }
                // the last (or may the only) chunk is done right here
                //inner_k_loop(m, n, k, kc, mc, mr, nr, (IntPtr)pAArr, (IntPtr)pBArr, (IntPtr)pCArr, (IntPtr)pBpack2, (IntPtr)pApack2, n / 2, n);
                inner_k_loop_managed(m, n, k, kc, mc, mr, nr, pAArr, pBArr, pCArr, pBpack2, pApack2, i * workItemLength, n, 0, m);
                ILThreadPool.Wait4Workers(ref workerCount);
            }

            ILMemoryPool.Pool.Free(CAux);
            ILMemoryPool.Pool.Free(Bpack1);
            ILMemoryPool.Pool.Free(Apack1);
            ILMemoryPool.Pool.Free(Bpack2);
            ILMemoryPool.Pool.Free(Apack2);
        }
        unsafe public static void MMultBlocked(complex[] A, complex[] B, complex[] C, int m, int n, int k, int kc) {

            // block parameters
            int mc = 512;
            int mr = 4;
            int nr = 4;

           
            complex[] CAux = ILMemoryPool.Pool.New<complex>(mc + ALIGN / sizeof(complex));
           
            complex[] Bpack1 = ILMemoryPool.Pool.New<complex>(kc * (n) + ALIGN / sizeof(complex));
           
            complex[] Apack1 = ILMemoryPool.Pool.New<complex>(kc * mc + ALIGN / sizeof(complex));
           
            complex[] Bpack2 = ILMemoryPool.Pool.New<complex>(kc * (n) + ALIGN / sizeof(complex));
           
            complex[] Apack2 = ILMemoryPool.Pool.New<complex>(kc * mc + ALIGN / sizeof(complex));
            fixed (complex* pAArr = A)
            fixed (complex* pBArr = B)
            fixed (complex* pCArr = C)
            fixed (complex* pBpack1 = Bpack1)
            fixed (complex* pApack1 = Apack1)
            fixed (complex* pBpack2 = Bpack2)
            fixed (complex* pApack2 = Apack2) {
                inner_k_loop_managed(m, n, k, kc, mc, mr, nr, pAArr, pBArr, pCArr, pBpack2, pApack2, 0, n, 0, m);
            }
            ILMemoryPool.Pool.Free(CAux);
            ILMemoryPool.Pool.Free(Bpack1);
            ILMemoryPool.Pool.Free(Apack1);
            ILMemoryPool.Pool.Free(Bpack2);
            ILMemoryPool.Pool.Free(Apack2);
        }

        unsafe private static void inner_k_loop_managed(int m, int n, int k, int kc, int mc, int mr, int nr,
                                           complex* pAArr, complex* pBArr, complex* pCArr, complex* pBpack, complex* pApack,
                                            int n_start, int n_end, int m_start, int m_end) {

           complex* pApackTmp = (complex*)((byte*)pApack + (ALIGN - ((uint)pApack % ALIGN)));
           complex* pBpackTmp = (complex*)((byte*)pBpack + (ALIGN - ((uint)pBpack % ALIGN)));
           complex* pCArrTmp, pBArrTmp, pAArrTmp;

            int n_len = n_end - n_start;
            for (int ki = 0; ki < k; ki += kc) {
                if (k - ki < kc) kc = k - ki;

                #region pack B
                pBpackTmp = pBpack;
                for (int nb = 0; nb < n_len; nb++) {
                    pBArrTmp = pBArr + ki + k * (nb + n_start);
                    int c = 0;
                    for (; c < kc - 8; c += 8) {
                        pBpackTmp[0] = pBArrTmp[0];
                        pBpackTmp[1] = pBArrTmp[1];
                        pBpackTmp[2] = pBArrTmp[2];
                        pBpackTmp[3] = pBArrTmp[3];
                        pBpackTmp[4] = pBArrTmp[4];
                        pBpackTmp[5] = pBArrTmp[5];
                        pBpackTmp[6] = pBArrTmp[6];
                        pBpackTmp[7] = pBArrTmp[7];
                        pBpackTmp += 8; pBArrTmp += 8;
                    }
                    for (; c < kc; c++) {
                        *pBpackTmp++ = *pBArrTmp++;
                    }
                }
                //pack(BArr, Bpack, r, 0, kc, n, k);
                #endregion

                int mcc = mc;
                int m_len = m_end - m_start; 
                for (int ai = 0; ai < m_len; ai += mcc) {
                    if (m_len - ai < mcc) mcc = m_len - ai;

                    #region pack A
                    for (int ca = 0; ca < kc; ca++) {
                        pApackTmp = pApack + ca;
                        pAArrTmp = pAArr + ai + m * (m_start + ki + ca);
                        int ra = 0;
                        for (; ra < mcc - 8; ra += 8) {
                            pApackTmp[(ra) * kc] = pAArrTmp[0];
                            pApackTmp[(ra + 1) * kc] = pAArrTmp[1];
                            pApackTmp[(ra + 2) * kc] = pAArrTmp[2];
                            pApackTmp[(ra + 3) * kc] = pAArrTmp[3];
                            pApackTmp[(ra + 4) * kc] = pAArrTmp[4];
                            pApackTmp[(ra + 5) * kc] = pAArrTmp[5];
                            pApackTmp[(ra + 6) * kc] = pAArrTmp[6];
                            pApackTmp[(ra + 7) * kc] = pAArrTmp[7];
                            pAArrTmp += 8;
                        }
                        for (; ra < mcc; ra++) {
                            pApackTmp[ra * kc] = *pAArrTmp++;
                        }
                    }
                    #endregion

                    #region subblocked
                    int nrLen = nr;
                    for (int nri = 0; nri < n_len; nri += nrLen) {
                        if (n_len - nri < nrLen) nrLen = n_len - nri;
                        int mrLen = mr;
                        for (int mri = 0; mri < mcc; mri += mrLen) {
                            if (mcc - mri < mrLen) mrLen = mcc - mri;

                            // prefetch CAux 


                            if (false && mrLen == 4 && nrLen == 4) {

                            } else {
                                for (int nii = 0; nii < nrLen; nii++) {
                                    pCArrTmp = pCArr + ai + mri + (nri + nii + n_start) * m;
                                    //for (int mii = 0; mii < mrLen; mii++) {
                                    //    pCAux[mii] = pCArrTmp[mii];
                                    //}
                                    for (int mii = 0; mii < mrLen; mii++) {
                                        pApackTmp = pApack + (mri + mii) * kc; // <-- transposed packed!
                                        pBpackTmp = pBpack + (nri + nii) * kc;
                                       complex sum = 0;
                                        int jj = 0;
                                        for (; jj < kc - 8; jj += 8) {
                                            sum += pApackTmp[0] * pBpackTmp[0]
                                                + pApackTmp[1] * pBpackTmp[1]
                                                + pApackTmp[2] * pBpackTmp[2]
                                                + pApackTmp[3] * pBpackTmp[3]
                                                + pApackTmp[4] * pBpackTmp[4]
                                                + pApackTmp[5] * pBpackTmp[5]
                                                + pApackTmp[6] * pBpackTmp[6]
                                                + pApackTmp[7] * pBpackTmp[7];
                                            pApackTmp += 8;
                                            pBpackTmp += 8;
                                        }
                                        for (; jj < kc; jj++) {
                                            sum += *pApackTmp++ * *pBpackTmp++;
                                        }
                                        //CAux[ra] = sum;
                                        pCArrTmp[mii] += sum;
                                    }
                                    //for (int mii = 0; mii < mrLen; mii++) {
                                    //    pCArrTmp[mii] = pCAux[mii];
                                    //}

                                }

                            }
                        }

                    }
                    #endregion
                }
            }
        }


#endregion HYCALPER AUTO GENERATED CODE

#region obsolete stubs (archive)
        
                    #region standard mmult
                    //for (int bj = 0; bj < n_len; bj++) {
                    //    pCArrTmp = pCArr + m_start + ai + (bj + n_start) * m;
                    //    for (int ra = 0; ra < mcc; ra++) {
                    //        pApackTmp = pApack + ra * kc;
                    //        pBpackTmp = pBpack + bj * kc;
                    //        double sum = 0;
                    //        int jj = 0;
                    //        for (; jj < kc - 24; jj += 24) {
                    //            sum += pApackTmp[0] * pBpackTmp[0]
                    //                + pApackTmp[1] * pBpackTmp[1]
                    //                + pApackTmp[2] * pBpackTmp[2]
                    //                + pApackTmp[3] * pBpackTmp[3]
                    //                + pApackTmp[4] * pBpackTmp[4]
                    //                + pApackTmp[5] * pBpackTmp[5]
                    //                + pApackTmp[6] * pBpackTmp[6]
                    //                + pApackTmp[7] * pBpackTmp[7]
                    //                + pApackTmp[8] * pBpackTmp[8]
                    //                + pApackTmp[9] * pBpackTmp[9]
                    //                + pApackTmp[10] * pBpackTmp[10]
                    //                + pApackTmp[11] * pBpackTmp[11]
                    //                + pApackTmp[12] * pBpackTmp[12]
                    //                + pApackTmp[13] * pBpackTmp[13]
                    //                + pApackTmp[14] * pBpackTmp[14]
                    //                + pApackTmp[15] * pBpackTmp[15]
                    //                + pApackTmp[16] * pBpackTmp[16]
                    //                + pApackTmp[17] * pBpackTmp[17]
                    //                + pApackTmp[18] * pBpackTmp[18]
                    //                + pApackTmp[19] * pBpackTmp[19]
                    //                + pApackTmp[20] * pBpackTmp[20]
                    //                + pApackTmp[21] * pBpackTmp[21]
                    //                + pApackTmp[22] * pBpackTmp[22]
                    //                + pApackTmp[23] * pBpackTmp[23];
                    //            pApackTmp += 24; pBpackTmp += 24; 
                    //        }
                    //        for (; jj < kc; jj++) {
                    //            sum += *pApackTmp++ * *pBpackTmp++;
                    //        }
                    //        pCArrTmp[ra] += sum;
                    //    }
                    //}
                    #endregion

                    #region unblocked fast mult  (NOT CORRECT! would require A to be NOT transposed!)
                    //double* pYA, pXA, pZA;
                    //for (int i = 0; i < n; i++) {
                    //    pYA = pBpack + kc * i;
                    //    pXA = pApack;
                    //    for (int kcl = 0; kcl < kc; kcl++) {
                    //        double r = *pYA++;
                    //        //double r = Y[k + N * i];
                    //        pZA = pCArr + ai + m * i;
                    //        int j = 0;
                    //        for (; j < mcc - 16; j += 16) {
                    //            //double c0 = pZA[0], c1 = pZA[1], c2 = pZA[2], c3 = pZA[3], c4 = pZA[4], c5 = pZA[5], c6 = pZA[6], c7 = pZA[7];
                    //            pZA[0] += r * pXA[0];
                    //            pZA[1] += r * pXA[1];
                    //            pZA[2] += r * pXA[2];
                    //            pZA[3] += r * pXA[3];
                    //            pZA[4] += r * pXA[4];
                    //            pZA[5] += r * pXA[5];
                    //            pZA[6] += r * pXA[6];
                    //            pZA[7] += r * pXA[7];
                    //            pZA[8] += r * pXA[8];
                    //            pZA[9] += r * pXA[9];
                    //            pZA[10] += r * pXA[10];
                    //            pZA[11] += r * pXA[11];
                    //            pZA[12] += r * pXA[12];
                    //            pZA[13] += r * pXA[13];
                    //            pZA[14] += r * pXA[14];
                    //            pZA[15] += r * pXA[15];
                    //            pZA += 16; pXA += 16;
                    //        }
                    //        while (j++ < mcc) {
                    //            *pZA++ += *pXA++ * r;
                    //            //Z[j + N * i] += X[j + N * k] * r;
                    //        }
                    //    }
                    //}
                    #endregion

#endregion

    }
}
