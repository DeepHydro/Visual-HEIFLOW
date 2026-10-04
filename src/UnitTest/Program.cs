using System;
using Heiflow.Models.Integration;
using Heiflow.Models.Surface;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Surface.MIKE;
using Heiflow.Models.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using MathNet.Numerics.Random;
using Heiflow.Core.Utility;
using System.IO;
namespace UnitTest
{
    class Program
    {

        static void Main(string[] args)
        {
            string path1 = @"C:\mydirectory\anotherdirectory\filename.txt";
            string path2 = @"d:\mydirectory\";

            System.Uri uri1 = new Uri(path1);
            System.Uri uri2 = new Uri(path2);
   
            var uri = DirectoryHelper.RelativePathTo( uri1.ToString(),uri2.ToString());

            var path = Path.Combine(path2, path1);

            string vv = "head[e][1][-1]";
            var pattern = @"\[(.*?)\]";
            Regex regex = new Regex(pattern);
           var cdf_sr = Palf.Doubles(100,1);
             var cdf=   MathNet.Numerics.Statistics.Statistics.EmpiricalCDFFunc(cdf_sr);
            Array.Sort<double>(cdf_sr);
            var cdf_value = new double[cdf_sr.Length];
            for(int i=0;i<cdf_sr.Length;i++)
            {
                cdf_value[i] = cdf(cdf_sr[i]);
            }

            var match = regex.Matches(vv);

            pattern = @"\d+";
            regex = new Regex(pattern);
            var ism= regex.IsMatch(match[1].Value);
            var num = regex.Match(match[0].Value);

            //LoadMatFromASC();
            //return;
            //string dbffile = @"E:\Heihe\HRB\GeoData\GBHM\IHRB\GIS\grid.dbf";
            string dbffile_hru = @"E:\Heihe\HRB\GeoData\GBHM\IHRB\GIS\Grid_IHRB_active.dbf";
            //AscWriter asc=new AscWriter();
            //string ascfile=@"C:\Users\Administrator\Documents\GBHM\IHRB\dem\elevation.asc";
            //asc.Export(dbffile,ascfile,560,430,1000,337028,4753638);

            MergeACFile mac = new MergeACFile();
            string ppt_hrb = @"E:\Heihe\HRB\Processes\AC\ppt_month_2000-2012.ac";
            string ppt_gbhm = @"E:\Heihe\HRB\Processes\AC\ppt_month_2000-2012.ac";
            string ppt_ihrb = @"C:\Users\Administrator\Documents\GBHM\IHRB\result\ppt.ac";
            // mac.Export(dbffile_hru, ppt_ihrb, ppt_hrb, ppt_gbhm);

            string lai_hrb = @"E:\Heihe\HRB\Processes\AC\LAI_FWJ_2000-2012.ac";
            string lai_gbhm = @"E:\Heihe\HRB\Processes\AC\LAI_FWJ_2000-2012.ac";
            string lai_ihrb = @"C:\Users\Administrator\Documents\GBHM\IHRB\result\lai.ac";

           // mac.Export(dbffile_hru, lai_ihrb, lai_hrb, lai_gbhm);

            //string et_hrb = @"E:\Heihe\HRB\Processes\AC\et_month_2000-2012_v1.ac";
            //string et_gbhm = @"E:\Heihe\HRB\Processes\AC\et_month_2000-2012_gbhm.ac";
            //string et_ihrb = @"C:\Users\Administrator\Documents\GBHM\IHRB\result\et.ac";

            string et_hrb = @"E:\Heihe\HRB\Processes\AC\sm_month_2000-2012.ac";
            string et_gbhm = @"E:\Heihe\HRB\Processes\AC\sm_month_2000-2012_gbhm.ac";
            string et_ihrb = @"C:\Users\Administrator\Documents\GBHM\IHRB\result\sm.ac";

            //mac.Determin2();

         //  mac.Determin();
         //  mac.Export(dbffile_hru, et_ihrb, et_hrb, et_gbhm);
            mac.Runoff_GBHM();

            Console.WriteLine("Press any key to continue!");
            Console.ReadKey();
            }
    }
}
