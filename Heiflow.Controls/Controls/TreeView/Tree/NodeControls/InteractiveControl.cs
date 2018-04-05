// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Heiflow.Controls.Tree.NodeControls
{
	public abstract class InteractiveControl : BindableControl
	{
		private bool _editEnabled = false;
		[DefaultValue(false)]
		public bool EditEnabled
		{
			get { return _editEnabled; }
			set { _editEnabled = value; }
		}

		protected bool IsEditEnabled(TreeNodeAdv node)
		{
			if (EditEnabled)
			{
				NodeControlValueEventArgs args = new NodeControlValueEventArgs(node);
				args.Value = true;
				OnIsEditEnabledValueNeeded(args);
				return Convert.ToBoolean(args.Value);
			}
			else
				return false;
		}

		public event EventHandler<NodeControlValueEventArgs> IsEditEnabledValueNeeded;
		private void OnIsEditEnabledValueNeeded(NodeControlValueEventArgs args)
		{
			if (IsEditEnabledValueNeeded != null)
				IsEditEnabledValueNeeded(this, args);
		}
	}
}
