// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Heiflow.Controls
{
	public static class TextHelper
	{
		public static StringAlignment TranslateAligment(HorizontalAlignment alignment)
		{
			if (alignment == HorizontalAlignment.Left)
				return StringAlignment.Near;
			else if (alignment == HorizontalAlignment.Right)
				return StringAlignment.Far;
			else
				return StringAlignment.Center;
		}

        public static TextFormatFlags TranslateAligmentToFlag(HorizontalAlignment alignment)
        {
            if (alignment == HorizontalAlignment.Left)
                return TextFormatFlags.Left;
            else if (alignment == HorizontalAlignment.Right)
                return TextFormatFlags.Right;
            else
                return TextFormatFlags.HorizontalCenter;
        }

		public static TextFormatFlags TranslateTrimmingToFlag(StringTrimming trimming)
		{
			if (trimming == StringTrimming.EllipsisCharacter)
				return TextFormatFlags.EndEllipsis;
			else if (trimming == StringTrimming.EllipsisPath)
				return TextFormatFlags.PathEllipsis;
			if (trimming == StringTrimming.EllipsisWord)
				return TextFormatFlags.WordEllipsis;
			if (trimming == StringTrimming.Word)
				return TextFormatFlags.WordBreak;
			else
				return TextFormatFlags.Default;
		}
	}
}
