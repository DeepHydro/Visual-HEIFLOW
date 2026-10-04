using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace HUST.WREIS.Dot3D.Renderable
{
	/// <summary>
	/// Contains one texture for our icon texture cache
	/// </summary>
	public class IconTexture : IDisposable
	{
		public Texture Texture;
		public int Width;
		public int Height;
        public int ReferenceCount;

        /// <summary>
        /// Base Save path for any downloaded images.  Set to CachePath\IconTextures by default.
        /// TODO: Need to get the real CachePath rather than faking it.
        /// TODO: Should make this directory settable.
        /// TODO: Should have some mechanism to clear the cache out.
        /// </summary>
        public static string BaseSavePath = Path.GetDirectoryName(Application.ExecutablePath) + @"\Cache\IconTextures";

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.IconTexture"/> class 
		/// from a texture file on disk.
		/// </summary>
		public IconTexture(Device device, string textureFileName)
		{
            if ((textureFileName != null) && textureFileName.Length > 0)
            {
                if (textureFileName.ToLower().StartsWith("http://") && BaseSavePath != null)
                {
                    // download it
                    try
                    {
                        Uri uri = new Uri(textureFileName);

                        // Set the subdirectory path to the hostname and replace . with _
                        string savePath = uri.Host;
                        savePath = savePath.Replace('.', '_');

                        // build the save file name from the component pieces
                        savePath = BaseSavePath + @"\" + savePath + uri.AbsolutePath;
                        savePath = savePath.Replace('/', '\\');

                        HUST.WREIS.Dot3D.Net.WebDownload webDownload = new HUST.WREIS.Dot3D.Net.WebDownload(textureFileName);
                        webDownload.DownloadType = HUST.WREIS.Dot3D.Net.DownloadType.Unspecified;
                        webDownload.DownloadFile(savePath);

                        // reset the texture file name for later use.
                        textureFileName = savePath;
                    }
                    catch { }
                }
            }

            if(ImageHelper.IsGdiSupportedImageFormat(textureFileName))
			{
				// Load without rescaling source bitmap
				using(Image image = ImageHelper.LoadImage(textureFileName))
					LoadImage(device, image);
			}
			else
			{
				// Only DirectX can read this file, might get upscaled depending on input dimensions.
				Texture = ImageHelper.LoadIconTexture( textureFileName );
				// Read texture level 0 size
				using(Surface s = Texture.GetSurfaceLevel(0))
				{
					SurfaceDescription desc = s.Description;
					Width = desc.Width;
					Height = desc.Height;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Renderable.IconTexture"/> class 
		/// from a bitmap.
		/// </summary>
		public IconTexture(Device device, Bitmap image)
		{
			LoadImage(device, image);
		}

		protected void LoadImage(Device device, Image image)
		{
			Width = (int)Math.Round(Math.Pow(2, (int)(Math.Ceiling(Math.Log(image.Width)/Math.Log(2)))));
			if(Width>device.DeviceCaps.MaxTextureWidth)
				Width = device.DeviceCaps.MaxTextureWidth;

			Height = (int)Math.Round(Math.Pow(2, (int)(Math.Ceiling(Math.Log(image.Height)/Math.Log(2)))));
			if(Height>device.DeviceCaps.MaxTextureHeight)
				Height = device.DeviceCaps.MaxTextureHeight;

			using(Bitmap textureSource = new Bitmap(Width, Height))
			using(Graphics g = Graphics.FromImage(textureSource))
			{
				g.DrawImage(image, 0,0,Width,Height);
				if(Texture!=null)
					Texture.Dispose();
				Texture = new Texture(device, textureSource, Usage.None, Pool.Managed);
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			if(Texture!=null)
			{
				Texture.Dispose();
				Texture = null;
			}
			
			GC.SuppressFinalize(this);
		}

		#endregion
	}
	
}
