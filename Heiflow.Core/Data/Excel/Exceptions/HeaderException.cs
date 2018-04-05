// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;

namespace Excel.Exceptions
{
	public class HeaderException : Exception
	{
		public HeaderException()
		{
		}

		public HeaderException(string message)
			: base(message)
		{
		}

		public HeaderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
