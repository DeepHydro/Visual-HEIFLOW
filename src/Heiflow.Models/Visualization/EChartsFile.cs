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
        public List<string> TopSection
        {
            get;
            private set;
        }
        public List<string> EndSection
        {
            get;
            private set;
        }
        public bool Initialized
        {
            get;
            private set;
        }
        public void Initialize(string templatefile)
        {
            TopSection = new List<string>();
            EndSection = new List<string>();
            if (File.Exists(templatefile))
            {
                StreamReader sr = new StreamReader(templatefile);
                var line = "";
                while (!sr.EndOfStream)
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
