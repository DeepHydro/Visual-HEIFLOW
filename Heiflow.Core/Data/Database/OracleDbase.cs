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
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using System.Collections;
#if UseOracle
using System.Data.OracleClient;
#endif
namespace Heiflow.Core.Data
{
#if UseOracle
    public class OracleDbase : IDBase
    {
        protected static OracleConnection mOralConnection;
        private string mConnectString;
        private string[] mPrimaryKey;
        public OracleDbase(dbConnectInfo info)
        {
            mConnectString = "data source =  " + info.dataSource + "; user id = " + info.user + "; PassWord = " + info.password;
            this.OpenConnection(mConnectString);
        }

    #region IDBase 成员
        public string[] GetFieldsName(string tableName)
        {
            string[] result = null;
            DataTable dt = mOralConnection.GetSchema("Columns", null);
            if (dt != null)
            {
                result = new string[dt.Rows.Count];
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    result[i] = dr[0].ToString();
                    i++;
                }
            }
            return result;
        }
        public IDbConnection DbConnection
        {
            get
            {
                return mOralConnection;
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
                mOralConnection = new OracleConnection(conncstr);
                mOralConnection.Open();

            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接错误!" + ex.Message.ToString());
            }
        }

        public DataSet QueryDataSet(string CommandString)
        {
            try
            {
                OracleDataAdapter adapter = new OracleDataAdapter(CommandString, OracleDbase.mOralConnection);
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
                MessageBox.Show("数据库连接错误!" + ex.Message.ToString());
                return null;
            }
        }

        public DataTable QueryDataTable(string CommandString)
        {
            try
            {
                if (OracleDbase.mOralConnection != null)
                {

                    OracleDataAdapter adapter = new OracleDataAdapter(CommandString, OracleDbase.mOralConnection);
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
                    MessageBox.Show("数据库连接没有初始化");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接错误!" + ex.Message.ToString());
                return new DataTable();
            }
        }

        public bool CreateNonQueryCommand(string sqlstr)
        {
            try
            {
                OracleCommand command = OracleDbase.mOralConnection.CreateCommand();
                command.CommandText = sqlstr;
                command.ExecuteNonQuery();
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
                if (OracleDbase.mOralConnection != null)
                {

                    OracleDataAdapter adapter = new OracleDataAdapter(CommandString, OracleDbase.mOralConnection);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("数据库连接没有初始化");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接错误!" + ex.Message.ToString());
                return false;
            }
        }
        public void Update(DataSet ds)
        {
            //if (adapter != null)
            //{
            //    OleDbCommandBuilder cmd = new OleDbCommandBuilder(adapter);
            //    adapter.Update(ds, ds.Tables[0].TableName);
            //}
        }
        #endregion
    }
#endif
}
