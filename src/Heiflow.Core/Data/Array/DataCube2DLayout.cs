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
        public DataCube2DLayout(int nvar, int nrow, int ncol, bool islazy = false)
            : base(nvar, nrow, ncol, islazy)
        {
            ColumnNames = new string[Size[2]];
            for (int i = 0; i < Size[2]; i++)
            {
                ColumnNames[i] = "C" + i;
            }
            Layout = DataCubeLayout.TwoD;
        }

        /// <summary>
        /// get a 2D array for the specified variable. The array size is [nrow, ncol]
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index">not required. set 0 as default</param>
        /// <returns></returns>
        public override Array GetSpatialSerialArray(int var_index, int time_index = 0)
        {
            T[,] array = new T[Size[1], Size[2]];

            for (int i = 0; i < Size[1]; i++)
            {
                for (int j = 0; j < Size[2]; j++)
                {
                    array[i,j] = this[SelectedVariableIndex, i, j];
                }
            }
            return array;
        }
        /// <summary>
        /// get a 2D array for the specified variable.
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index"></param>
        /// <returns></returns>
        public override Array GetSpatialRegularArray(int var_index, int time_index)
        {
            return GetSpatialSerialArray(var_index, time_index);
        }
        public override void FromSpatialSerialArray(int var_index, int time_index, Array array)
        {
            for (int i = 0; i < Size[1]; i++)
            {
                for (int j = 0; j < Size[2]; j++)
                {
                    this[var_index, i, j] = TypeConverterEx.ChangeType<T>(array.GetValue(i, j));
                }
            }
        }
        public override void FromSpatialRegularArray(int var_index, int time_index, Array array)
        {
            FromSpatialSerialArray(var_index, time_index, array);
        }
        public override System.Data.DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            if (SelectedVariableIndex < 0)
                SelectedVariableIndex = 0;
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
        public override DataTable ToDataTable(int var_index, int time_index, int cell_index)
        {
            return ToDataTable();
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
