// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel.Log
{
	public static class StringExtensions
	{
		/// <summary>
		/// Formats string with the formatting passed in. This is a shortcut to string.Format().
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="formatting">The formatting.</param>
		/// <returns>A formatted string.</returns>
		public static string FormatWith(this string input, params object[] formatting)
		{
			return string.Format(input, formatting);
		}

	}
}
