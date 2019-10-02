using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Heiflow.Core.Data
{
    public class DataCube2DLayout<T> : DataCube<T>
    {
        public DataCube2DLayout(int nvar, int nrow, int ncol, bool init, bool islazy = false)
            : base(nvar, nrow, ncol, init, islazy)
        {
            Layout = DataCubeLayout.TwoD;
            ColumnNames = new string[ncol];
            for (int i = 0; i < ncol; i++)
            {
                ColumnNames[i] = "C" + i;
            }
        }
        /// <summary>
        /// Default values are provided.
        /// </summary>
        public string[] ColumnNames { get; set; }

        public override System.Data.DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            if (DateTimes != null)
            {
                DataColumn dc = new DataColumn("Date", typeof(DateTime));
                dt.Columns.Add(dc);
            }
            for (int i = 0; i < Size[2]; i++)
            {
                DataColumn dc = new DataColumn(ColumnNames[i], typeof(T));
                dt.Columns.Add(dc);
            }
            if (DateTimes != null)
            {
                for (int i = 0; i < Size[1]; i++)
                {
                    var dr = dt.NewRow();
                    dr[0] = DateTimes[i];
                    for (int j = 0; j < Size[2]; j++)
                    {
                        dr[j + 1] = this[SelectedVariableIndex, i, j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                for (int i = 0; i < Size[1]; i++)
                {
                    var dr = dt.NewRow();

                    for (int j = 0; j < Size[2]; j++)
                    {
                        dr[j] = this[SelectedVariableIndex, i, j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        public override void FromDataTable(DataTable dt)
        {
            if (DateTimes != null)
            {
                for (int r = 0; r < Size[1]; r++)
                {
                    DataRow dr = dt.Rows[r];
                    DateTimes[r] = DateTime.Parse(dr[0].ToString());
                    for (int c = 1; c <= Size[2]; c++)
                    {
                        this[SelectedVariableIndex, r, c - 1] = (T)dr[c];
                    }
                }
            }
            else
            {
                for (int r = 0; r < Size[1]; r++)
                {
                    DataRow dr = dt.Rows[r];
                    for (int c = 0; c < Size[2]; c++)
                    {
                        this[SelectedVariableIndex, r, c] = (T)dr[c];
                    }
                }
            }
        }
    }
}
