// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;

namespace Heiflow.Controls
{
	public class StringCollectionEditor : CollectionEditor
	{
		public StringCollectionEditor(Type type): base(type)
		{
		}

		protected override Type CreateCollectionItemType()
		{
			return typeof(string);
		}

		protected override object CreateInstance(Type itemType)
		{
			return "";
		}
	}
}
