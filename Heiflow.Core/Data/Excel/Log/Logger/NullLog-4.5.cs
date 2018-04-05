// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;

namespace Excel.Log.Logger
{
	/// <summary>
	/// The default logger until one is set.
	/// </summary>
	public partial class NullLog : ILog, ILog<NullLog>
	{
		public void Debug(Func<string> message)
		{
		}

		public void Info(Func<string> message)
		{
		}

		public void Warn(Func<string> message)
		{
		}

		public void Error(Func<string> message)
		{
		}

		public void Fatal(Func<string> message)
		{
		}

	}
}
