// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Heiflow.Controls.Threading
{
	public sealed class WorkItem
	{
		private WaitCallback _callback;
		private object _state;
		private ExecutionContext _ctx;

		internal WorkItem(WaitCallback wc, object state, ExecutionContext ctx)
		{
			_callback = wc; 
			_state = state; 
			_ctx = ctx;
		}

		internal WaitCallback Callback
		{
			get
			{
				return _callback;
			}
		}

		internal object State
		{
			get
			{
				return _state;
			}
		}

		internal ExecutionContext Context
		{
			get
			{
				return _ctx;
			}
		}
	}
}
