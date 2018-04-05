// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace Heiflow.Controls.Tree.NodeControls
{

	public class NodeIntegerTextBox : NodeTextBox
	{
		private bool _allowNegativeSign = true;
		[DefaultValue(true)]
		public bool AllowNegativeSign
		{
			get { return _allowNegativeSign; }
			set { _allowNegativeSign = value; }
		}

		public NodeIntegerTextBox()
		{
		}

		protected override TextBox CreateTextBox()
		{
			NumericTextBox textBox = new NumericTextBox();
			textBox.AllowDecimalSeparator = false;
			textBox.AllowNegativeSign = AllowNegativeSign;
			return textBox;
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			SetValue(node, (editor as NumericTextBox).IntValue);
		}
	}
}
