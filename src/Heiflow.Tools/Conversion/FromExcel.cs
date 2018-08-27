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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.IO;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;

namespace Heiflow.Tools.Conversion
{
    public class FromExcel : ModelTool
    {
        public FromExcel()
        {
            Name = "From Excel";
            Category = "Conversion";
            Description = "Load data cube from excel file";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The  input data filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Filename
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            //Save2DB();
            StreamReader sr = new StreamReader(Filename);
            StreamWriter sw_stat = new StreamWriter(Filename + "_stat.csv");
            StreamWriter sw_records = new StreamWriter(Filename + "_record.csv");
            Dictionary<string, SiteInfo> sites = new Dictionary<string, SiteInfo>();
            Dictionary<string, SiteInfo> filtered_sites = new Dictionary<string, SiteInfo>();
            var head = sr.ReadLine();
            sw_records.WriteLine(head);
            DateTime start = new DateTime(2000, 1, 1);
            while (!sr.EndOfStream)
            {
                var str = sr.ReadLine().Trim();
                var buf = TypeConverterEx.Split<string>(str, TypeConverterEx.Comma);
                var name = buf[1].Trim();
                var date = DateTime.Parse(buf[2].Trim());
                if (sites.Keys.Contains(name))
                {
                    var site = sites[name];
                    site.Dates.Add(new DateTime(date.Year, date.Month, 1));
                    if (date > start)
                        site.Records.Add(str);
                }
                else
                {
                    var site = new SiteInfo()
                    {
                        Name = name
                    };
                    site.Dates.Add(new DateTime(date.Year, date.Month, 1));
                    sites.Add(name, site);
                    if(date>start)
                        site.Records.Add(str);
                }
            }
            List<DateTime> basedates = new List<DateTime>();
            for (int i = 2000; i < 2017; i++)
            {
                for (int j = 1; j < 13; j++)
                {
                    basedates.Add(new DateTime(i, j, 1));
                }
            }
            var temp = (from dt in basedates select dt.ToString("yyyy-MM-dd")).ToArray();
            int ndate = temp.Count();
            var header = "Name," + string.Join(",", temp);
            sw_stat.WriteLine(header);
            for (int i = 0; i < sites.Count; i++)
            {
                var flags = new int[ndate];
                var ss = sites.Values.ElementAt(i);
                for (int j = 0; j < ndate; j++)
                {      
                    if (ss.Dates.Contains(basedates[j]))
                    {
                        flags[j] = 1;
                    }
                }
                 ss.Flags = flags;
                var line = sites.Keys.ElementAt(i) + "," + string.Join(",", flags);
                sw_stat.WriteLine(line);
            }

            for (int i = 0; i < sites.Count; i++)
            {
                var ss = sites.Values.ElementAt(i);
                if (ss.Flags.Sum() > 200)
                {
                    filtered_sites.Add(ss.Name, ss);
                }
            }
            for (int i = 0; i < filtered_sites.Count; i++)
            {
                var ss = filtered_sites.Values.ElementAt(i);
                for (int j = 0; j < ndate;j++ )
                {
                    if (ss.Flags[j] == 0)
                    {
                        var strs = TypeConverterEx.Split<string>(ss.Records[j - 1], TypeConverterEx.Comma);
                        strs[2] = basedates[j].ToString("yyyy-MM-dd");
                        strs[3] = basedates[j].Month.ToString();
                        var sline = string.Join(",", strs);
                        ss.Records.Insert(j, sline);
                    }
                }
                for (int j = 0; j < ndate; j++)
                {
                    sw_records.WriteLine(ss.Records[j]);
                }
            }
            sr.Close();
            sw_stat.Close();
            sw_records.Close();
            return true;
        }
  
        private void Save2DB()
        {
            ODMSource odm = new ODMSource();
            string msg="";
            odm.Open(@"F:\System\Database\Pearl River Basin.mdb", ref msg);
            var sites = odm.GetSites("select * from Sites");
            StreamReader sr = new StreamReader(Filename);
            Dictionary<string, Site> dic = new Dictionary<string, Site>();
            foreach (var ss in sites)
                dic.Add(ss.Name, ss);
            var line = sr.ReadLine();
            line = sr.ReadLine();
            var varid = TypeConverterEx.SkipSplit<int>(line,6);
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                var buf = TypeConverterEx.Split<string>(line, TypeConverterEx.Comma);
                for (int i = 0; i < varid.Length; i++)
                {
                    odm.SaveDataValue(dic[buf[1].Trim()], varid[i], DateTime.Parse(buf[2].Trim()), double.Parse(buf[6 + i].Trim()));
                }
            }
            odm.UpdateSeriesCatalog();
            sr.Close();
            odm.Close();
        }
    }

    internal class SiteInfo
    {
        public SiteInfo()
        {
            Dates = new List<DateTime>();
            Records = new List<string>();
        }
        public string Name { get; set; }
        public List<DateTime> Dates { get; set; }
        public List<string> Records { get; set; }
        public int[] Flags { get; set; }
    }
}
