// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
