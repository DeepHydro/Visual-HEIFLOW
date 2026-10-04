using Heiflow.Core.Data;
using Heiflow.Models.Atmosphere;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class CalcPET
    {
        public static void Calc()
        {
            PenmanMonteithET pet = new PenmanMonteithET();
            string line = "";
            float[] buf;

            string tav = @"E:\气象\TmpMonth.txt";
            StreamReader srtav = new StreamReader(tav);
            double[][] mtav = new double[15600][];
            for (int i = 0; i < 15600; i++)
            {
                line = srtav.ReadLine();
                mtav[i] = TypeConverterEx.Split<double>(line);
            }
            srtav.Close();

            for (int y = 2000; y < 2013; y++)
            {
                string filename = @"E:\气象\Climate_" + y + ".txt";
                string fn_pet = filename.Replace(".txt", ".et0");

                StreamReader sr = new StreamReader(filename);
                StreamWriter sw = new StreamWriter(fn_pet);
                int days = DateTime.IsLeapYear(y) ? 366 : 365;
                for (int d = 0; d < days;d++ )
                {
                    for (int i = 0; i < 15600; i++)
                    {
                        pet.MonthTemperature = mtav[i];
                         line = sr.ReadLine();
                         buf = TypeConverterEx.Split<float>(line);
                         DateTime date = new DateTime((int)buf[9], (int)buf[10], (int)buf[11]);
                          var et0 = pet.ET0(buf[0], buf[1], buf[2], buf[3], buf[4], buf[5], buf[6], buf[7], buf[8], date, buf[12]);
                          sw.WriteLine(et0);
                    }
                }
                Console.WriteLine("Finished: " + y);
                sr.Close();
                sw.Close();
            }

          
        }
    }
}
