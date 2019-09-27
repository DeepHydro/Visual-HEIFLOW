using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public class DataCube2DLayout<T> : DataCube<T>
    {
        public DataCube2DLayout(int nvar, int nrow, int ncol, bool islazy = false)
            : base(nvar, nrow, ncol, islazy)
        {
            Layout = DataCubeLayout.TwoD;
            ColumnNames = new string[ncol];
            for (int i = 0; i < ncol; i++)
            {
                ColumnNames[i] = "Col " + i;
            }
        }
        /// <summary>
        /// Default values are provided.
        /// </summary>
        public string[] ColumnNames { get; set; }
    }
}
