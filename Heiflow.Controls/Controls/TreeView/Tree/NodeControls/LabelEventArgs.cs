// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;

namespace Heiflow.Controls.Tree.NodeControls
{
	public class LabelEventArgs : EventArgs
	{
		private object _subject;
		public object Subject
		{
			get { return _subject; }
		}

		private string _oldLabel;
		public string OldLabel
		{
			get { return _oldLabel; }
		}

		private string _newLabel;
		public string NewLabel
		{
			get { return _newLabel; }
		}

		public LabelEventArgs(object subject, string oldLabel, string newLabel)
		{
			_subject = subject;
			_oldLabel = oldLabel;
			_newLabel = newLabel;
		}
	}
}
