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
using System.Collections;
using System.Data;
using System.Windows.Forms;
using Heiflow.Core;
using System.Data.SQLite;

namespace Heiflow.Core.Data
{
    /// <summary>
    /// Access the SQLite database using System.Data.SQLite
    /// </summary>
    public class SQLiteDatabase : IDBase
    {
        private string mConnectString;
        protected SQLiteConnection mSQLiteConn;
        private string[] mPrimaryKey;
        private SQLiteDataAdapter adapter = null;
        private DBkind mDBkind;
        private SQLiteCommand sqlCommand;

        public SQLiteDatabase(DBConnectInfo info)
        {
            mDBkind = DBkind.SQLite;
            mConnectString = "Data Source=" + info.DataSource + ";Version=3;";
            if (!string.IsNullOrEmpty(info.Password))
            {
                mConnectString += "Password=" + info.Password + ";";
            }
            OpenConnection(mConnectString);
        }

        public SQLiteDatabase(string conncstr)
        {
            mDBkind = DBkind.SQLite;
            OpenConnection(conncstr);
        }

        public DBkind DBKind
        {
            get { return mDBkind; }
        }

        public SQLiteConnection SQLiteConnect
        {
            get
            {
                return mSQLiteConn;
            }
        }

        public ConnectionState ConnectionState
        {
            get
            {
                if (mSQLiteConn != null)
                    return mSQLiteConn.State;
                else
                    return System.Data.ConnectionState.Broken;
            }
        }

        public void OpenConnection()
        {
            if (mSQLiteConn != null && mSQLiteConn.State == ConnectionState.Closed)
            {
                mSQLiteConn.Open();
            }
        }

        public void CloseConnection()
        {
            if (mSQLiteConn != null && mSQLiteConn.State == ConnectionState.Open)
            {
                mSQLiteConn.Close();
            }
        }

        #region IDBase 成员
        public IDbConnection DbConnection
        {
            get
            {
                return mSQLiteConn;
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
                mSQLiteConn = new SQLiteConnection(conncstr);
                sqlCommand = new SQLiteCommand();
                sqlCommand.Connection = mSQLiteConn;
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
            DataTable dt = QueryDataTable("PRAGMA table_info('" + tableName + "')");
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ar.Add(dr["name"].ToString());
                }
                result = (string[])ar.ToArray(Type.GetType("System.String"));
            }
            return result;
        }

        public DataSet QueryDataSet(string CommandString)
        {
            try
            {
                if (mSQLiteConn == null)
                {
                    MessageBox.Show("Database connection has not been intialized!");
                    return null;
                }
                adapter = new SQLiteDataAdapter(CommandString, mSQLiteConn);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ArrayList ar = new ArrayList();
                    DataTable schema = mSQLiteConn.GetSchema("IndexColumns");
                    if (schema != null)
                    {
                        foreach (DataRow dr in schema.Rows)
                        {
                            if (dr["table_name"].ToString() == ds.Tables[0].TableName)
                            {
                                ar.Add(dr["column_name"].ToString());
                            }
                        }
                    }
                    if (ar.Count > 0)
                    {
                        mPrimaryKey = (string[])ar.ToArray(Type.GetType("System.String"));
                        ds.Tables[0].PrimaryKey = new DataColumn[mPrimaryKey.Length];
                        for (int i = 0; i < mPrimaryKey.Length; i++)
                        {
                            ds.Tables[0].PrimaryKey[i] = ds.Tables[0].Columns[mPrimaryKey[i]];
                        }
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
                MessageBox.Show("Querying database error! " + ex.Message.ToString());
                return null;
            }
        }

        public DataTable QueryDataTable(string CommandString)
        {
            try
            {
                if (mSQLiteConn != null)
                {
                    adapter = new SQLiteDataAdapter(CommandString, mSQLiteConn);
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
                SQLiteCommandBuilder cmd = new SQLiteCommandBuilder(adapter);
                adapter.Update(ds, ds.Tables[0].TableName);
            }
        }
        #endregion
    }
}
