using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable
{
    public class MediaColor
    {
        public static System.Windows.Media.Color From(System.Drawing.Color color)
        {
            return new System.Windows.Media.Color()
            {
                A = color.A,
                B = color.B,
                R = color.R,
                G = color.G,
            };
        }
        public static System.Drawing.Color From( System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A,color.R,color.G,color.B);
        }

        public static int ToArgb(System.Windows.Media.Color color)
        {
            return From(color).ToArgb();
        }

    }
}
