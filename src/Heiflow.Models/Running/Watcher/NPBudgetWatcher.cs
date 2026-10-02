//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
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

using Heiflow.Core.Data;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Running
{
    /// <summary>
    /// 读取 N / P 质量平衡文件（sz_N_budget.csv、sz_P_budget.csv）。
    /// 与 CSVWatcher 的区别在于：显式解析列标题，并采用与区域无关的日期/数值格式。
    /// </summary>
    public class NPBudgetWatcher : CSVWatcher
    {
        private static readonly string[] _DateFormats = new string[]
        {
            "MM/dd/yyyy", "M/d/yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy", "yyyyMMdd"
        };

        /// <summary>
        /// CSV 列标题，第一列为 Date
        /// </summary>
        public string[] Headers
        {
            get;
            private set;
        }

        public override void Load(string filename, bool convertToStrepRate, bool[] var_index_isconvert)
        {
            if (this.State == RunningState.Busy)
                return;
            if (TypeConverterEx.IsNull(filename) || !File.Exists(filename))
                return;

            Headers = null;

            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(fs, Encoding.Default);
            try
            {
                var headerLine = sr.ReadLine();
                if (TypeConverterEx.IsNull(headerLine))
                    return;

                Headers = headerLine.Split(',').Select(p => p.Trim()).ToArray();
                int nvar = Headers.Length - 1;
                if (nvar <= 0)
                    return;

                _DataSource = new ListTimeSeries<double>(nvar);

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (TypeConverterEx.IsNull(line))
                        continue;

                    var strs = line.Split(',');
                    DateTime date;
                    if (!TryParseDate(strs[0], out date))
                        continue;

                    var buf = new double[nvar];
                    for (int i = 0; i < nvar; i++)
                    {
                        if (i + 1 >= strs.Length)
                            continue;
                        double value;
                        if (double.TryParse(strs[i + 1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                            buf[i] = value;
                    }
                    _DataSource.Add(date, buf);
                }
            }
            finally
            {
                sr.Close();
                fs.Close();
            }

            if (convertToStrepRate && var_index_isconvert != null)
                ConvertToStepRate(var_index_isconvert);
        }

        /// <summary>
        /// 累积量转步长值（相邻时间步差分），保留首个时间步的原值
        /// </summary>
        private void ConvertToStepRate(bool[] var_index_isconvert)
        {
            int nrow = _DataSource.Dates.Count;
            int ncol = _DataSource.Values.Length;
            if (nrow == 0)
                return;

            for (int i = 0; i < ncol && i < var_index_isconvert.Length; i++)
            {
                if (!var_index_isconvert[i])
                    continue;
                var vec = new double[nrow];
                for (int j = 0; j < nrow; j++)
                {
                    vec[j] = _DataSource.Values[i][j];
                }
                for (int j = 1; j < nrow; j++)
                {
                    _DataSource.Values[i][j] = vec[j] - vec[j - 1];
                }
            }
        }

        private static bool TryParseDate(string text, out DateTime date)
        {
            date = DateTime.Now;
            if (TypeConverterEx.IsNull(text))
                return false;

            var value = text.Trim();
            if (DateTime.TryParseExact(value, _DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return true;
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
