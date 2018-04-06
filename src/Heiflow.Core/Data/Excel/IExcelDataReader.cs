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
using System.Data;
using System.IO;

namespace Excel
{
	public interface IExcelDataReader : IDataReader
	{
		/// <summary>
		/// Initializes the instance with specified file stream.
		/// </summary>
		/// <param name="fileStream">The file stream.</param>
		void Initialize(Stream fileStream);

		/// <summary>
		/// Read all data in to DataSet and return it
		/// </summary>
		/// <returns>The DataSet</returns>
		DataSet AsDataSet();

		/// <summary>
		///Read all data in to DataSet and return it
		/// </summary>
		/// <param name="convertOADateTime">if set to <c>true</c> [try auto convert OA date time format].</param>
		/// <returns>The DataSet</returns>
		DataSet AsDataSet(bool convertOADateTime);

		/// <summary>
		/// Gets a value indicating whether file stream is valid.
		/// </summary>
		/// <value><c>true</c> if file stream is valid; otherwise, <c>false</c>.</value>
		bool IsValid { get;}

		/// <summary>
		/// Gets the exception message in case of error.
		/// </summary>
		/// <value>The exception message.</value>
		string ExceptionMessage { get;}

		/// <summary>
		/// Gets the sheet name.
		/// </summary>
		/// <value>The sheet name.</value>
		string Name { get;}

		/// <summary>
		/// Gets the number of results (workbooks).
		/// </summary>
		/// <value>The results count.</value>
		int ResultsCount { get;}

		/// <summary>
		/// Gets or sets a value indicating whether the first row contains the column names.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the first row contains column names; otherwise, <c>false</c>.
		/// </value>
		bool IsFirstRowAsColumnNames { get;set;}

	}
}