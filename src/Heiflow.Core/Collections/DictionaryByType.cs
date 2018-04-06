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
namespace MiscUtil.Collections
{
    /// <summary>
    /// Map from types to instances of those types, e.g. int to 10 and
    /// string to "hi" within the same dictionary. This cannot be done
    /// without casting (and boxing for value types) as .NET cannot
    /// represent this relationship with generics in their current form.
    /// This class encapsulates the nastiness in a single place.
    /// </summary>
    public class DictionaryByType
    {
        private readonly IDictionary<Type, object> dictionary = new Dictionary<Type, object>();

        /// <summary>
        /// Maps the specified type argument to the given value. If
        /// the type argument already has a value within the dictionary,
        /// ArgumentException is thrown.
        /// </summary>
        public void Add<T>(T value)
        {
            dictionary.Add(typeof(T), value);
        }

        /// <summary>
        /// Maps the specified type argument to the given value. If
        /// the type argument already has a value within the dictionary, it
        /// is overwritten.
        /// </summary>
        public void Put<T>(T value)
        {
            dictionary[typeof(T)] = value;
        }

        /// <summary>
        /// Attempts to fetch a value from the dictionary, throwing a
        /// KeyNotFoundException if the specified type argument has no
        /// entry in the dictionary.
        /// </summary>
        public T Get<T>()
        {
            return (T) dictionary[typeof(T)];
        }

        /// <summary>
        /// Attempts to fetch a value from the dictionary, returning false and
        /// setting the output parameter to the default value for T if it
        /// fails, or returning true and setting the output parameter to the
        /// fetched value if it succeeds.
        /// </summary>
        public bool TryGet<T>(out T value)
        {
            object tmp;
            if (dictionary.TryGetValue(typeof(T), out tmp))
            {
                value = (T) tmp;
                return true;
            }
            value = default(T);
            return false;
        }
    }
}
