using Heiflow.Controls.WinForm.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Heiflow.Controls.WinForm.Project
{
    /// <summary>
    /// The two icons a variable node carries, which tell whether its data has been loaded. The icon of
    /// the model is drawn in grey while there is no data behind the node yet, so the tree shows which
    /// of the variables are loaded and which still have to be.
    /// </summary>
    public static class VariableIcons
    {
        private static readonly Lazy<Bitmap> _NotLoaded = new Lazy<Bitmap>(() => ToGrey(Resources.UiLayer16));

        /// <summary>
        /// The icon of a variable whose data is loaded.
        /// </summary>
        public static Bitmap Loaded
        {
            get { return Resources.UiLayer16; }
        }

        /// <summary>
        /// The icon of a variable whose data is not loaded.
        /// </summary>
        public static Bitmap NotLoaded
        {
            get { return _NotLoaded.Value; }
        }

        /// <summary>
        /// Draws the icon with the colour taken out of it. The shares are the ones that say how bright
        /// a pixel of these three channels is seen, so the shade the eye reads the icon as is kept.
        /// </summary>
        private static Bitmap ToGrey(Bitmap icon)
        {
            if (icon == null)
                return null;

            var grey = new Bitmap(icon.Width, icon.Height);
            var matrix = new ColorMatrix(new float[][]
            {
                new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f },
                new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f },
                new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f },
                new float[] { 0f, 0f, 0f, 1f, 0f },
                new float[] { 0f, 0f, 0f, 0f, 1f }
            });

            using (var attributes = new ImageAttributes())
            using (var surface = Graphics.FromImage(grey))
            {
                attributes.SetColorMatrix(matrix);
                surface.DrawImage(icon, new Rectangle(0, 0, icon.Width, icon.Height),
                    0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel, attributes);
            }
            return grey;
        }
    }
}
