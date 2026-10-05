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
using System.Windows.Forms; 
using ILNumerics.Exceptions; 
 
using ILNumerics.Algorithms;

namespace ILNumerics.Misc {

    /// <summary>
    /// Bucket sort algorithm (for internal use)
    /// </summary>
    /// <remarks>This class is not intended to be used directly. Sorting functionality is supplied by <see cref="ILMath.sort(ILInArray{double})"/></remarks>
    [System.Security.SecuritySafeCritical]
    public class ILBucketSort {
        /// <summary>
        /// Sort method for bucket sorts
        /// </summary>
        public enum SortMethod {
            /// <summary>
            /// Constant length
            /// </summary>
            ConstantLength,
            /// <summary>
            /// Variable length
            /// </summary>
            VariableLenth
        }

        /// <summary>
        /// Bucket sort algorithm 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="indices">Return corresponding source element indices</param>
        /// <param name="mapper"></param>
        /// <param name="method"></param>
        public static ILQueueList<ElementType,IndexType> BucketSort<ElementType,SubelementType,IndexType> (
                                            IEnumerable<ElementType> input,
                                            IEnumerable<IndexType> indices,
                                            ILKeyMapper<ElementType,SubelementType> mapper,
                                            SortMethod method) {
            if (mapper == null) 
                throw new ILInvalidOperationException("ILBucketSort: key mapper must not be null!");
            if (input == null) {
                return new ILQueueList<ElementType,IndexType>(); 
            }
            // do the sort now
            switch (method) {
                case SortMethod.VariableLenth: 
                    return bucketSort_variableLength<ElementType,SubelementType,IndexType>(input,mapper); 
                case SortMethod.ConstantLength:
                    if (indices != null) 
                        return bucketSort_constantLength(input,indices,mapper);
                    else 
                        return bucketSort_constantLength<ElementType,SubelementType,IndexType>(input,mapper);
                default: 
                    throw new ILArgumentException("ILBucketSort: unknown sort method specified."); 
            }
        }

        private static ILQueueList<ElementType, IndexType>
            bucketSort_variableLength<ElementType, SubelementType, IndexType>(
                                    IEnumerable<ElementType> input,
                                    ILKeyMapper<ElementType, SubelementType> mapper) {
            using (ILScope.Enter()) {
                int m = mapper.NumberOfKeys;
                ILQueueList<ElementType, IndexType>[] buckets = new ILQueueList<ElementType, IndexType>[m];
                ILQueueList<ElementType, IndexType> Q = new ILQueueList<ElementType, IndexType>();
                IEnumerable<ElementType> inp = input;

                #region compute lmax
                int maxLen = 0, tmp = 0;
                foreach (ElementType elem in inp) {
                    tmp = mapper.SubelementsCount(elem);
                    if (tmp > maxLen)
                        maxLen = tmp;
                }
                #endregion
                #region create Lengths array
                ILQueueList<ElementType, IndexType>[] Lengths = new ILQueueList<ElementType, IndexType>[maxLen];
                foreach (ElementType elem in inp) {
                    int len = mapper.SubelementsCount(elem) - 1;
                    if (Lengths[len] == null) {
                        Lengths[len] = new ILQueueList<ElementType, IndexType>();
                    }
                    Lengths[len].Enqueue(elem);
                }
                #endregion
                #region create bucket indices for each position: Noempty array
                ILQueueList<int, byte>[] Noempty = new ILQueueList<int, byte>[maxLen];
                ILLogical alreadyFound = new ILLogical(new ILSize(maxLen, m));
                byte tr = (byte)1;
                foreach (ElementType s in inp) {
                    for (int l = mapper.SubelementsCount(s); l-- > 0; ) {
                        int hpos = mapper.Map(s, l, 0);
                        if (alreadyFound.GetValue(l, hpos) == 0) {
                            alreadyFound.SetValue(tr, l, hpos);
                            if (Noempty[l] == null) {
                                // create list and init with new value
                                Noempty[l] = new ILQueueList<int, byte>();
                            }
                            Noempty[l].Enqueue(hpos);
                        }
                    }
                }
                alreadyFound.Dispose();
                for (int i = 0; i < maxLen; i++) {
                    // sort lists for each length
                    Noempty[i] = bucketSort_constantLength<int, int, byte>(Noempty[i], new ILIntLimitedKeyMapper(m));
                }
                #endregion
                #region sort
                ILListItem<ElementType, IndexType> curElement;
                for (int l = maxLen; l-- > 0; ) {
                    // sort Length[l] list
                    if (Lengths[l] != null) {
                        Q.AddToStart(Lengths[l]);
                    }
                    // sort from last sorting loop (l+1)
                    while (Q.Count > 0) {
                        curElement = Q.Dequeue();
                        int hpos = mapper.Map(curElement.Data, l, 0);
                        if (buckets[hpos] == null) {
                            buckets[hpos] = new ILQueueList<ElementType, IndexType>();
                        }
                        buckets[hpos].Enqueue(curElement);
                    }
                    // collect queues
                    foreach (int c in Noempty[l]) {
                        Q.Enqueue(buckets[c]);
                        buckets[c].Clear();
                    }
                }
                #endregion
                return Q;
            }
        }
        internal static ILQueueList<ElementType,IndexType> 
            bucketSort_constantLength<ElementType,SubelementType,IndexType>(
                                    IEnumerable<ElementType> input,
                                    ILKeyMapper<ElementType,SubelementType> mapper) {
            int m = mapper.NumberOfKeys;
            int tmp = 0; 
            ILQueueList<ElementType,IndexType>[] buckets = new ILQueueList<ElementType,IndexType>[m];
            ILQueueList<ElementType,IndexType> Q = new ILQueueList<ElementType,IndexType>(); 
            // find longest element 
            int maxLen = 0;
            foreach (ElementType elem in input) {
                tmp = mapper.SubelementsCount( elem );  
                if (tmp > maxLen)
                    maxLen = tmp;
            }
            // sort into buckets 
            tmp = 0; 
            for (int k = maxLen; k-- > 0; ) {
                // walk along the input 
                if (k == maxLen - 1) {
                    foreach (ElementType elem in input) {
                        // put current into bucket
                        int bpos = mapper.Map(elem,k,0);
                        if (buckets[bpos] == null) {
                            // must create list first
                            buckets[bpos] = new ILQueueList<ElementType,IndexType>();
                        } 
                        // append to list 
                        buckets[bpos].Enqueue(elem); 
                    }
                } else {
                    while (Q.Count > 0) {
                        // put current into bucket
                        ILListItem<ElementType,IndexType> elem = Q.Dequeue(); 
                        int bpos = mapper.Map(elem.Data,k,0);
                        if (buckets[bpos] == null) {
                            // must create list first
                            buckets[bpos] = new ILQueueList<ElementType,IndexType>();
                        } 
                        // append to list 
                        buckets[bpos].Enqueue(elem); 
                    }
                }
                // concatenate all buckets 
                for (int i = 0; i < buckets.Length; i++) {
                    if (buckets[i] != null && buckets[i].Count > 0) {
                        Q.Enqueue(buckets[i]); 
                        buckets[i].Clear();
                    }
                }
                // goto previous position in strings 
            }
            return Q;
        }

        internal static ILQueueList<ElementType,IndexType> 
            bucketSort_constantLength<ElementType,SubelementType,IndexType>(
                                            IEnumerable<ElementType> input,
                                            IEnumerable<IndexType> indices, 
                                            ILKeyMapper<ElementType,SubelementType> mapper) {
            int m = mapper.NumberOfKeys;
            int tmp = 0; 
            ILQueueList<ElementType,IndexType>[] buckets = new ILQueueList<ElementType,IndexType>[m];
            ILQueueList<ElementType,IndexType> Q = new ILQueueList<ElementType,IndexType>(); 
            // find longest element 
            int maxLen = 0;
            foreach (ElementType elem in input) {
                tmp = mapper.SubelementsCount( elem );  
                if (tmp > maxLen)
                    maxLen = tmp;
            }
            // sort into buckets 
            IEnumerator<IndexType> indIt = indices.GetEnumerator();
            indIt.MoveNext(); 
            for (int k = maxLen; k-- > 0; ) {
                // walk along the input 
                if (k == maxLen - 1) {
                    foreach (ElementType elem in input) {
                        // put current into bucket
                        int bpos = mapper.Map(elem,k,0);
                        if (buckets[bpos] == null) {
                            // must create list first
                            buckets[bpos] = new ILQueueList<ElementType,IndexType>();
                        } 
                        // append to list 
                        buckets[bpos].Enqueue(elem,indIt.Current);
                        indIt.MoveNext(); 
                    }
                } else {
                    while (Q.Count > 0) {
                        // put current into bucket
                        ILListItem<ElementType,IndexType> elem = Q.Dequeue(); 
                        int bpos = mapper.Map(elem.Data,k,0);
                        if (buckets[bpos] == null) {
                            // must create list first
                            buckets[bpos] = new ILQueueList<ElementType,IndexType>();
                        } 
                        // append to list 
                        buckets[bpos].Enqueue(elem); 
                    }
                }
                // concatenate all buckets 
                for (int i = 0; i < buckets.Length; i++) {
                    if (buckets[i] != null && buckets[i].Count > 0) {
                        Q.Enqueue(buckets[i]); 
                        buckets[i].Clear();
                    }
                }
                // goto previous position in strings 
            }
            return Q;
        }

    }
}
