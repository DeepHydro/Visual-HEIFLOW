// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace Heiflow.Controls
{
    public static class ResourceHelper
    {
        // VSpilt Cursor with Innerline (symbolisize hidden column)
        private static Cursor _dVSplitCursor = GetCursor(Heiflow.Controls.WinForm.Properties.Resources.DVSplit);
        public static Cursor DVSplitCursor
        {
            get { return _dVSplitCursor; }
        }

		private static GifDecoder _loadingIcon = GetGifDecoder(Heiflow.Controls.WinForm.Properties.Resources.loading_icon);
		public static GifDecoder LoadingIcon
		{
			get { return _loadingIcon; }
		}

        /// <summary>
        /// Help function to convert byte[] from resource into Cursor Type 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Cursor GetCursor(byte[] data)
        {
            using (MemoryStream s = new MemoryStream(data))
                return new Cursor(s);
        }

		/// <summary>
		/// Help function to convert byte[] from resource into GifDecoder Type 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static GifDecoder GetGifDecoder(byte[] data)
		{
			using(MemoryStream ms = new MemoryStream(data))
				return new GifDecoder(ms, true);
		}

    }
}
