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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heiflow.Core;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.Collections;

namespace Heiflow.Core.Data
{
    /// <summary>
    /// Access the database using OelDB method
    /// </summary>
    public class OleDBase : IDBase
    {
        private string mConnectString;
        protected OleDbConnection mOledbConnc;
        private string[] mPrimaryKey;
        private OleDbDataAdapter adapter = null;
        private DBkind mDBkind;
        private OleDbCommand sqlCommand;
        public OleDBase(DBkind dbkind, DBConnectInfo info)
        {
            mDBkind = dbkind;
            if (dbkind == DBkind.Access2003 || dbkind == DBkind.Access2007 || dbkind == DBkind.Access2013)
            {
                mConnectString = string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {0};Persist Security Info=False; ", info.DataSource);
            //    mConnectString = string.Format("Provider = Microsoft.ACE.OleDB.15.0; Data Source = {0};Persist Security Info=False; ", info.DataSource);
            }
            else if (dbkind == DBkind.Oracle)
            {
                mConnectString = "Provider=msdaora; data source =  " + info.DataSource + "; user id = " + info.UserName + "; PassWord = " + info.Password;
            }
            else if (dbkind == DBkind.Excel)
            {
                mConnectString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + info.DataSource + "; Extended Properties=Excel 8.0";
            }
            else if (dbkind == DBkind.Sqlserver)
            {
                mConnectString = "Provider=SQLOLEDB;Data Source=" + info.DataSource + ";Initial Catalog= " + info.DatabaseName + ";UserId=" +
                    info.UserName + ";Password=" + info.Password + ";";
            }
            OpenConnection(mConnectString);
        }

        public DBkind DBKind
        {
            get { return mDBkind; }
        }

        public OleDbConnection OleDbConnect
        {
            get
            {
                return mOledbConnc;
            }
        }

        public ConnectionState  ConnectionState
        {
            get
            {
                if (mOledbConnc != null)
                    return mOledbConnc.State;
                else
                    return System.Data.ConnectionState.Broken;
            }
        }

        public void OpenConnection()
        {
            if (mOledbConnc.State == ConnectionState.Closed)
            {
                mOledbConnc.Open();
            }
        }

        public void CloseConnection()
        {

            if (mOledbConnc.State == ConnectionState.Open)
            {
                mOledbConnc.Close();
            }
        }

        #region IDBase 成员
        public IDbConnection DbConnection
        {
            get
            {
                return mOledbConnc;
            }
        }
        public string[] PrimaryKey
        {
            get
            {
                return mPrimaryKey;
            }
        }
        public void OpenConnection(string conncstr)
        {
            try
            {
                mOledbConnc = new OleDbConnection(conncstr);
               // mOledbConnc.Open();
                sqlCommand = new OleDbCommand();
                sqlCommand.Connection = mOledbConnc;
                 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connecting to database error! Message:" + ex.Message.ToString());
            }
        }

        public string[] GetFieldsName(string tableName)
        {
            string[] result = null;
            ArrayList ar = new ArrayList();
            DataTable dt = mOledbConnc.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName, null });
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ar.Add(dr.ItemArray[3].ToString());
                }
                result = (string[])ar.ToArray(Type.GetType("System.String"));
            }
            return result;
        }

        public DataSet QueryDataSet(string CommandString)
        {
            try
            {
                adapter = new OleDbDataAdapter(CommandString, mOledbConnc);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    adapter.FillSchema(ds, SchemaType.Source);
                    ArrayList ar = new ArrayList();
                    if (ds.Tables[0].PrimaryKey != null)
                    {
                        foreach (DataColumn dc in ds.Tables[0].PrimaryKey)
                        {
                            ar.Add(dc.ColumnName);
                        }
                        mPrimaryKey = (string[])ar.ToArray(Type.GetType("System.String"));
                    }
                    return ds;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connecting to database error! " + ex.Message.ToString());
                return null;
            }
        }

        public DataTable QueryDataTable(string CommandString)
        {
            try
            {
                if (mOledbConnc != null)
                {
                    adapter = new OleDbDataAdapter(CommandString, mOledbConnc);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        return ds.Tables[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    MessageBox.Show("Database connection has not been intialized!");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error!" + ex.Message.ToString());
                return new DataTable();
            }
        }

        public bool CreateNonQueryCommand(string sqlstr)
        {
            //sqlstr——非查询的SQL语句
            try
            {
                //OleDbCommand command = mOledbConnc.CreateCommand();
                sqlCommand.CommandText = sqlstr;
                sqlCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool Exists(string CommandString)
        {
            try
            {
                sqlCommand.CommandText = CommandString;
                var dt = sqlCommand.ExecuteScalar();
                return dt != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error!" + ex.Message.ToString());
                return false;
            }
        }
        public void Update(DataSet ds)
        {
            if (adapter != null)
            {
                OleDbCommandBuilder cmd = new OleDbCommandBuilder(adapter);
                adapter.Update(ds, ds.Tables[0].TableName);
            }
        }
        #endregion
    }
}
