// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
