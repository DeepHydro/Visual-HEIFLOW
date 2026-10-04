using Heiflow.Controls.Tree;
using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Heiflow.Visualization.Studio.Controls
{
     internal class MyNode:Node
    {
         public MyNode(string text):base(text)
         {
             ImageSource = new BitmapImage(new Uri(@"./Resources/pck16.png", UriKind.Relative));
             Source = System.IO.Path.Combine( ConfigurationManager.ApplicationPath, @"Resources\pck16.png");
         }

         public ImageSource ImageSource { get; set; }

         public string Source { get; set; }
    }
}
