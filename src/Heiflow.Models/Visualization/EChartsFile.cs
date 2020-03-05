using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Heiflow.Models.Visualization
{
    public class EChartsFile
    {
        public static List<string> TopSection
        {
            get;
            private set;
        }
        public static List<string> EndSection
        {
            get;
            private set;
        }
        public static bool Initialized
        {
            get;
            private set;
        }
        public static void Initialize(string templatefile)
        {
            TopSection = new List<string>();
            EndSection = new List<string>();
            if (File.Exists(templatefile))
            {
                StreamReader sr = new StreamReader(templatefile);
                var line = "";
                while(!sr.EndOfStream)
                {
                   line = sr.ReadLine();
                   if (line != @"//generateData")
                    {
                        TopSection.Add(line);
                    }                 
                    else
                   {
                       break;
                   }
                }
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    EndSection.Add(line);
                }
                sr.Close();
                Initialized = false;
            }
            else
            {
                Initialized = false;
            }
        }
    }
}
