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

namespace MiscUtil.Collections.Extensions
{
    /// <summary>
    /// Extensions to IDictionary
    /// </summary>
    public static class DictionaryExt
    {
        /// <summary>
        /// Returns the value associated with the specified key if there
        /// already is one, or inserts a new value for the specified key and
        /// returns that.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value, which must either have
        /// a public parameterless constructor or be a value type</typeparam>
        /// <param name="dictionary">Dictionary to access</param>
        /// <param name="key">Key to lookup</param>
        /// <returns>Existing value in the dictionary, or new one inserted</returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                       TKey key)
            where TValue : new()
        {
            TValue ret;
            if (!dictionary.TryGetValue(key, out ret))
            {
                ret = new TValue();
                dictionary[key] = ret;
            }
            return ret;
        }

        /// <summary>
        /// Returns the value associated with the specified key if there already
        /// is one, or calls the specified delegate to create a new value which is
        /// stored and returned.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">Dictionary to access</param>
        /// <param name="key">Key to lookup</param>
        /// <param name="valueProvider">Delegate to provide new value if required</param>
        /// <returns>Existing value in the dictionary, or new one inserted</returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                       TKey key,
                                                       Func<TValue> valueProvider)
        {
            TValue ret;
            if (!dictionary.TryGetValue(key, out ret))
            {
                ret = valueProvider();
                dictionary[key] = ret;
            }
            return ret;
        }

        /// <summary>
        /// Returns the value associated with the specified key if there
        /// already is one, or inserts the specified value and returns it.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">Dictionary to access</param>
        /// <param name="key">Key to lookup</param>
        /// <param name="missingValue">Value to use when key is missing</param>
        /// <returns>Existing value in the dictionary, or new one inserted</returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                       TKey key,
                                                       TValue missingValue)
        {
            TValue ret;
            if (!dictionary.TryGetValue(key, out ret))
            {
                ret = missingValue;
                dictionary[key] = ret;
            }
            return ret;
        }
    }
}
