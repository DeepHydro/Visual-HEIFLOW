//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Heiflow.Controls.Tree.NodeControls
{
	/// <summary>
	/// Displays an animated icon for those nodes, who are in expanding state. 
	/// Parent TreeView must have AsyncExpanding property set to true.
	/// </summary>
	public class ExpandingIcon: NodeControl
	{
		private static GifDecoder _gif = ResourceHelper.LoadingIcon;
		private static int _index = 0;
		private static volatile Thread _animatingThread;
        private static object _lock = new object();

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return ResourceHelper.LoadingIcon.FrameSize;
		}

		protected override void OnIsVisibleValueNeeded(NodeControlValueEventArgs args)
		{
			args.Value = args.Node.IsExpandingNow;
			base.OnIsVisibleValueNeeded(args);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			Rectangle rect = GetBounds(node, context);
			Image img = _gif.GetFrame(_index).Image;
			context.Graphics.DrawImage(img, rect.Location);
		}

		public static void Start()
		{
            lock (_lock)
            {
                if (_animatingThread == null)
                {
                    _index = 0;
                    _animatingThread = new Thread(new ThreadStart(IterateIcons));
                    _animatingThread.IsBackground = true;
                    _animatingThread.Priority = ThreadPriority.Lowest;
                    _animatingThread.Start();
                }
            }
		}

        public static void Stop()
        {
            lock (_lock)
            {
                _index = 0;
                _animatingThread = null;
            }
        }

		private static void IterateIcons()
		{
            while (_animatingThread != null)
			{
				lock (_lock)
				{
					if (_index < _gif.FrameCount - 1)
						_index++;
					else
						_index = 0;
				}

				if (IconChanged != null)
					IconChanged(null, EventArgs.Empty);

				int delay = _gif.GetFrame(_index).Delay;
				Thread.Sleep(delay);
			}
            System.Diagnostics.Debug.WriteLine("IterateIcons Stopped");
		}

		public static event EventHandler IconChanged;
	}
}
