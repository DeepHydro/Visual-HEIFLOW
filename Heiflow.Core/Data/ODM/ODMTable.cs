// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
