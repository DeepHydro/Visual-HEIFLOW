using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public class BilFile
    {
        public static void Write(string filename, RegularGrid grid, DataCube<float> mat, int var_index)
        {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            StreamWriter sw = new StreamWriter(filename + ".hdr");
            int progress = 0;
            int nsteps = mat.Size[1];
            var lonlat = grid.GetLonLatAxis();
            var nrow = grid.RowCount;
            var ncol = grid.ColumnCount;
            var times = new float[nsteps];
            if (mat.DateTimes != null)
            {
                for (int t = 0; t < nsteps; t++)
                {
                    times[t] = (float)mat.DateTimes[t].ToOADate();
                }
            }
            else
            {
                for (int t = 0; t < nsteps; t++)
                {
                    times[t] = (float)DateTime.Now.AddDays(t).ToOADate();
                }
            }

            sw.WriteLine(ncol + "," + nrow + "," + nsteps);
            var timestr = string.Join("\n", times);
            sw.WriteLine(timestr);
            sw.Close();

            for (int t = 0; t < nsteps; t++)
            {
                var mat_step = grid.ToMatrix<float>(mat[var_index, t.ToString(), ":"], 0);
                for (int r = 0; r < nrow; r++)
                {
                    for (int c = 0; c < ncol; c++)
                    {
                        //var bb= (byte) mat_step[r][c];
                        //byte hi = (byte)(bb >> 8);
                        //byte low = (byte)(bb - (short)(hi << 8));
                        var a = (int)mat_step[r][c];
                        byte hi = (byte)(a >> 8);
                        byte low = (byte)(a - (a << 8));

                        bw.Write(low);
                        bw.Write(hi);
                    }
                }
                progress = t * 100 / nsteps;
            }
            sw.Close();
            fs.Close();
            bw.Close();
        }
    }
}
