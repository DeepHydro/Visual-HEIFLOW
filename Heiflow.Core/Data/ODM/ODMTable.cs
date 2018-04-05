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

using Heiflow.Core.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.ODM
{
    public abstract class ODMTable:IODMTable
    {
        protected string _Message;
        protected string _TableName;
        protected IODMExportSetting _ExportSetting;
        public event EventHandler<int> ProgressChanged;
        public event EventHandler Started;
        public event EventHandler Finished;

        public ODMTable()
        {
            _Message = "";
        }


        public ODMSource ODM
        {
            get;
            set;
        }


        public string Message
        {
            get { return _Message; }
        }

        public string TableName
        {
            get { return _TableName; }
        }

        public IODMExportSetting ExportSetting
        {
            get { return _ExportSetting; }
        }

        public virtual bool Check(System.Data.DataTable dt)
        {
            bool succ = true;
            var dt_source = ODM.GetDataTable(TableName, 1);
            var col_external = from DataColumn dc in dt.Columns select dc.ColumnName;
            var col_source = from DataColumn dc in dt_source.Columns select dc.ColumnName;

            foreach (var str in col_source)
            {
                if (!col_external.Contains(str))
                {
                    _Message = string.Format("The neccessary field {0} is missing", str);
                    succ = false;
                    break;
                }
            }
            if (!succ)
            {
                return false;
            }
            else
            {
                return succ;
            }
        }

        public abstract bool Save(System.Data.DataTable dt);

        public virtual void Export(string filename, System.Data.DataTable source)
        {
            if (source != null)
            {
                CSVFileStream csv = new CSVFileStream(filename);
                csv.Save(source);
            }
        }
        public virtual void CustomExport(DataTable source)
        {

        }
        protected virtual void OnProgressChanged(int current)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, current);
        }

        protected virtual void OnStarted()
        {
            if (Started != null)
                Started(this, EventArgs.Empty);
        }

        protected virtual void OnFinished()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }


    }
}
