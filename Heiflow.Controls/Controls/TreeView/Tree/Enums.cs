// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;

namespace Heiflow.Controls.Tree
{

    public enum BackgroundPaintMode
    {
        Default,Gradiant
    }

	public enum DrawSelectionMode
	{
		None, Active, Inactive, FullRowSelect
	}

	public enum TreeSelectionMode
	{
		Single, Multi, MultiSameParent
	}

	public enum NodePosition
	{
		Inside, Before, After
	}

	public enum VerticalAlignment
	{
		Top, Bottom, Center
	}

	public enum IncrementalSearchMode
	{
		None, Standard, Continuous
	}

	[Flags]
    public enum GridLineStyle
    {
		None = 0, 
		Horizontal = 1, 
		Vertical = 2, 
		HorizontalAndVertical = 3
    }

	public enum ImageScaleMode
	{
		/// <summary>
		/// Don't scale
		/// </summary>
		Clip,
		/// <summary>
		/// Scales image to fit the display rectangle, aspect ratio is not fixed.
		/// </summary>
		Fit,
		/// <summary>
		/// Scales image down if it is larger than display rectangle, taking aspect ratio into account
		/// </summary>
		ScaleDown,
		/// <summary>
		/// Scales image up if it is smaller than display rectangle, taking aspect ratio into account
		/// </summary>
		ScaleUp,
		/// <summary>
		/// Scales image to match the display rectangle, taking aspect ratio into account
		/// </summary>
		AlwaysScale,

	}
}
