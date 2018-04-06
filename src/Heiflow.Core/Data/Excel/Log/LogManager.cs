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
using System.Text;

namespace Excel.Log
{
	/// <summary>
	/// 2.0 version of LogExtensions, not as awesome as Extension methods
	/// </summary>
	public static class LogManager
	{
		/// <summary>
		/// Concurrent dictionary that ensures only one instance of a logger for a type.
		/// </summary>
		private static readonly Dictionary<string, ILog> _dictionary = new Dictionary<string, ILog>();

		private static object _sync = new Object();

		/// <summary>
		/// Gets the logger for <see cref="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type">The type to get the logger for.</param>
		/// <returns>Instance of a logger for the object.</returns>
		public static ILog Log<T>(T type)
		{
			string objectName = typeof(T).FullName;
			return Log(objectName);
		}

		/// <summary>
		/// Gets the logger for the specified object name.
		/// </summary>
		/// <param name="objectName">Either use the fully qualified object name or the short. If used with Log&lt;T&gt;() you must use the fully qualified object name"/></param>
		/// <returns>Instance of a logger for the object.</returns>
		public static ILog Log(string objectName)
		{
			ILog result = null;

			if (_dictionary.ContainsKey(objectName))
				result = _dictionary[objectName];

			if (result == null)
			{
				lock (_sync)
				{
					result = Excel.Log.Log.GetLoggerFor(objectName);
					_dictionary.Add(objectName, result);
				}
			}
			
			return result;
		}
	}
}
