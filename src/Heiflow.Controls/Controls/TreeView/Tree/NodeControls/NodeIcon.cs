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
using System.Windows.Forms;
using System.ComponentModel;

namespace Heiflow.Controls.Tree.NodeControls
{
	public class NodeIcon : BindableControl
	{
		public NodeIcon()
		{
			LeftMargin = 1;
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			Image image = GetIcon(node);
			if (image != null)
				return image.Size;
			else
				return Size.Empty;
		}


		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			Image image = GetIcon(node);
			if (image != null)
			{
				Rectangle r = GetBounds(node, context);
				if ( image.Width > 0 && image.Height > 0 )
				{
					switch (_scaleMode)
					{
						case ImageScaleMode.Fit:
							context.Graphics.DrawImage(image, r);
							break;
						case ImageScaleMode.ScaleDown:
							{
								float factor = Math.Min((float)r.Width / (float)image.Width, (float)r.Height / (float)image.Height);
								if (factor < 1)
									context.Graphics.DrawImage(image, r.X, r.Y, image.Width * factor, image.Height * factor);
								else
									context.Graphics.DrawImage(image, r.X, r.Y, image.Width, image.Height);
							} break;
						case ImageScaleMode.ScaleUp:
							{
								float factor = Math.Max((float)r.Width / (float)image.Width, (float)r.Height / (float)image.Height);
								if (factor > 1)
									context.Graphics.DrawImage(image, r.X, r.Y, image.Width * factor, image.Height * factor);
								else
									context.Graphics.DrawImage(image, r.X, r.Y, image.Width, image.Height);
							} break;
						case ImageScaleMode.AlwaysScale:
							{
								float fx = (float)r.Width / (float)image.Width;
								float fy = (float)r.Height / (float)image.Height;
								if (Math.Min(fx, fy) < 1)
								{ //scale down
									float factor = Math.Min(fx, fy);
									context.Graphics.DrawImage(image, r.X, r.Y, image.Width * factor, image.Height * factor);
								}
								else if (Math.Max(fx, fy) > 1)
								{
									float factor = Math.Max(fx, fy);
									context.Graphics.DrawImage(image, r.X, r.Y, image.Width * factor, image.Height * factor);
								}
								else
									context.Graphics.DrawImage(image, r.X, r.Y, image.Width, image.Height);
							} break;
						case ImageScaleMode.Clip:
						default: 
							context.Graphics.DrawImage(image, r.X, r.Y, image.Width, image.Height);
							break;
					}
				}

			}
		}

		protected virtual Image GetIcon(TreeNodeAdv node)
		{
			return GetValue(node) as Image;
		}

        private ImageScaleMode _scaleMode = ImageScaleMode.Clip;
        [DefaultValue("Clip"), Category("Appearance")]
        public ImageScaleMode ScaleMode
        {
            get { return _scaleMode; }
            set { _scaleMode = value; }
        }


	}
}
