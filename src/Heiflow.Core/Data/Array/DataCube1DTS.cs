using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ILNumerics;

namespace Heiflow.Core.Data
{
    public class DataCube1DTS<T> : DataCube<T>
    {
        public DataCube1DTS(T[] values, DateTime[] dates)
            : base(1, 1, values.Length,false)
        {
            _arrays[0]["0", ":"] = values;
            DateTimes = dates;
            Name = "Time Series";
            ColumnNames = new string[] { Variables[0] };
        }
        public override System.Data.DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("Date", typeof(DateTime));
            dt.Columns.Add(dc);
            dc = new DataColumn(ColumnNames[0], typeof(T));
            dt.Columns.Add(dc);
            for (int i = 0; i < Size[1]; i++)
            {
                var dr = dt.NewRow();
                dr[0] = DateTimes[i];
                dr[1] = this[0, i, 0];
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public override void FromDataTable(DataTable dt, int var_index, int time_index, int cell_index)
        {
            for (int r = 0; r < Size[1]; r++)
            {
                DataRow dr = dt.Rows[r];
                DateTimes[r] = DateTime.Parse(dr[0].ToString());
                this[0, r, 1] = (T)dr[1];
            }
        }
    }
}