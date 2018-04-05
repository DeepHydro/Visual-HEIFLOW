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
	/// <summary>
	/// Custom interface for logging messages
	/// </summary>
	public partial interface ILog
	{
		/// <summary>
		/// Debug level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Debug(Func<string> message);

		/// <summary>
		/// Info level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Info(Func<string> message);

		/// <summary>
		/// Warn level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Warn(Func<string> message);

		/// <summary>
		/// Error level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Error(Func<string> message);

		/// <summary>
		/// Fatal level of the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		void Fatal(Func<string> message);
	}


}
