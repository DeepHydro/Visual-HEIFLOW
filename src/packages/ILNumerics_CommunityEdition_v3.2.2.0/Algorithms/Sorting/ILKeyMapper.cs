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

namespace ILNumerics.Algorithms {
    /// <summary>
    /// Key mapper class, to be overriden for user defined classes to be sorted with bucket sort
    /// </summary>
    /// <typeparam name="ElementType">Type of elements. Elements are constructed out of any number of subelements</typeparam>
    /// <typeparam name="SubelementType">Type of subelements</typeparam>
    /// <remarks>This class can be extended to enable sorting (bucket sort) for arbitrary types. The elements of those types may be devidable into subelements.
    /// <para>Examples of sortable classes:
    /// <list>
    /// <item>colors: number/type of subelements: 1/any (e.g. the color code). One should write a <![CDATA[ILKeyMapper<Color,int>]]>.</item>
    /// <item>strings: number/type of subelements: arbitrary/char. Here a sample ILASCIKeyMapper implementation exists already. This implementation is the default implementation used for bucket sort via ILMath.sort().</item>
    /// <item>trees: number/type of subelements: arbitrary/tree nodes. One should write a key mapper to map a node of a tree to a bucket number</item>
    /// <item>...</item></list></para></remarks>
    public abstract class ILKeyMapper<ElementType, SubelementType> {
        /// <summary>
        /// Maps subelement types to bucket index
        /// </summary>
        /// <param name="inSubelement">Item</param>
        /// <returns>Bucket index</returns>
        public abstract int Map (SubelementType inSubelement); 
        /// <summary>
        /// Map subelemt - provide fallback on error
        /// </summary>
        /// <param name="element">Element item</param>
        /// <param name="position">Position of subelement in element item to be mapped</param>
        /// <param name="fallback">If position is out of range, give back fallback</param>
        /// <returns>Mapped bucket for subelement or fallback on error</returns>
        public virtual int Map(ElementType element, int position, int fallback) {
            if (SubelementsCount(element) > position) 
                return Map(GetSubelement(element,position));
            else 
                return fallback; 
        }
        /// <summary>
        /// Count subelements in an element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public abstract int SubelementsCount (ElementType element); 
        /// <summary>
        /// Get subelement from element item
        /// </summary>
        /// <param name="element">Element item</param>
        /// <param name="idx">Position of subitem in element</param>
        /// <returns>Subitem referenced</returns>
        public abstract SubelementType GetSubelement (ElementType element, int idx); 
        
        private int m_numberOfKeys = 0; 
        /// <summary>
        /// Maximum number of keys (different subitems)
        /// </summary>
        public int NumberOfKeys {
            get {
                return m_numberOfKeys; 
            }
        }
        /// <summary>
        /// Construct key mapper
        /// </summary>
        /// <param name="NumberOfKeys">Maximm number of different subitems (keys)</param>
        public ILKeyMapper (int NumberOfKeys) {
            m_numberOfKeys = NumberOfKeys; 
        }
    }
    /// <summary>
    /// Concrete implementation of a key mapper for strings
    /// </summary>
    /// <remarks>this class is the default key mapper, used for bucket sort on strings</remarks>
    public class ILASCIIKeyMapper : ILKeyMapper<string, char> {
        /// <summary>
        /// map subelement to bucket 
        /// </summary>
        /// <param name="inSubelement">subelement to be mapped</param>
        /// <returns>ASCII code of the subelement character</returns>
        public override int  Map(char inSubelement) {
            return (int)inSubelement; 
        }
        /// <summary>
        /// Map char out of string with fallback
        /// </summary>
        /// <param name="element">full string item</param>
        /// <param name="position">position of character in string</param>
        /// <param name="fallback">fallback bucket number, if position is out of range</param>
        /// <returns>ASCII code for character specified, fallback on error</returns>
        public override int Map(string element, int position, int fallback) {
            if (element.Length > position)
                return (int)element[position]; 
            else 
                return fallback; 
        }
        /// <summary>
        /// give one char from string
        /// </summary>
        /// <param name="element">full string item</param>
        /// <param name="idx">character position in string</param>
        /// <returns>character in string</returns>
        /// <exception cref="IndexOutOfRangeException"> if idx is not within element ranges</exception>
        public override char  GetSubelement(string element, int idx) {
 	           return element[idx];
        }
        /// <summary>
        /// Count numer of characters in string
        /// </summary>
        /// <param name="element">element item</param>
        /// <returns>number of characters in string - length of string</returns>
        public override int SubelementsCount(string element) {
            if (object.Equals(element,null))
                return 0; 
            return element.Length;             
        }
        /// <summary>
        /// construct ASCII key mapper for 256 buckets
        /// </summary>
        public ILASCIIKeyMapper() : base(256) {}
    }
    /// <summary>
    /// Integer key mapper - sample implementation for bucket sort
    /// </summary>
    /// <remarks>This mapper may be used for sorting integers with bucketsort. 
    /// <para>The integers to be sorted must be positive and limited. It corresponds to the number of buckets to be created.</para>
    /// <para>This implementation serves as a sample implementation for bucket sort. You should consider using quicksort instead, which is implemented for ILMath.sort()</para></remarks>
    internal class ILIntLimitedKeyMapper : ILKeyMapper<int, int> {
        /// <summary>
        /// Gives subelement - i.e. the element itself
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="idx">(ignored)</param>
        /// <returns>element</returns>
        public override int GetSubelement(int element, int idx) {
            return element; 
        }
        /// <summary>
        /// map element - ignoring position &amp; fallback
        /// </summary>
        /// <param name="element">integer element</param>
        /// <param name="position">(ignored)</param>
        /// <param name="fallback">(ignored)</param>
        /// <returns>integer element</returns>
        public override int Map(int element, int position, int fallback) {
            return element; 
        }
        /// <summary>
        /// map (copy) subelement
        /// </summary>
        /// <param name="inSubelement">subelement</param>
        /// <returns>subelement</returns>
        public override int Map(int inSubelement) {
            return inSubelement; 
        }
        /// <summary>
        /// number of subelements in an element (Here: always 1)
        /// </summary>
        /// <param name="element">element</param>
        /// <returns>1</returns>
        public override int SubelementsCount(int element) {
            return 1; 
        }
        /// <summary>
        /// construct integer key mapper
        /// </summary>
        /// <param name="limit">maximum number of buckets to be used</param>
        public ILIntLimitedKeyMapper (int limit) : base(limit) {} 
    }
}

