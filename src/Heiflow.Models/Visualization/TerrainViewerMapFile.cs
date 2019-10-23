using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Heiflow.Models.Visualization
{
    public class TerrainViewerMapFile
    {
        public TerrainViewerMapFile()
        {

        }

        public static string Mapfile
        {
            get
            {
                return Path.Combine(Application.StartupPath, "External\\TerrainViewer\\maps.xml");
            }
        }

        public static void Save(string [] mapnames, string [] bilfile)
        {
            StreamWriter sw = new StreamWriter(Mapfile);
            string line = "<?xml version='1.0' encoding=\"ISO-8859-1\" ?>";
            sw.WriteLine(line);
            line = "<!-- List of 3D maps -->";
            sw.WriteLine(line);
            line = "<maps>";
            sw.WriteLine(line);
            for (int i = 0; i < mapnames.Length; i++)
            {
                line = string.Format( "<map name=\"{0}\">", mapnames[i]);
                sw.WriteLine(line);
                line = string.Format("<terrain demfile=\"{0}\" texturefile=\"colors\\Geo_1.png\" altscale=\"0.015\"/>", bilfile[i]);
                sw.WriteLine(line);
                line = " <sky skyfile=\"skys\\Sky_Gradian_Day1.jpg\" visible=\"1\"/>";
                sw.WriteLine(line);
                line = "</map>";
                sw.WriteLine(line);
            }
            line = "</maps>";
            sw.WriteLine(line);
            sw.Close();
        }
    }
}
