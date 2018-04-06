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
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using Heiflow.Core;

namespace Heiflow.Core.Data
{
    /// <summary>
    /// 数据库助手类
    /// </summary>
    public  class SqlServerDatabase:IDBase
    {
        public  string connectionString ;
        public SqlServerDatabase(DBConnectInfo info)
        {
            connectionString = "Provider=System.Data.SqlClient;Data Source=" + info.DataSource + ";Initial Catalog= " + info.DatabaseName + ";User Id=" +
                    info.UserName + ";Password=" + info.Password + ";";
        }

        public ConnectionState ConnectionState
        {
            get
            {
                return mSqlConnection.State;
            }
        }

        /// <summary>
        /// 是否存在一条记录
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public  bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public  bool Exists(string strSql)
        {
            DataTable dt = QueryDataTable(strSql);
            return (dt != null);
        }

        /// <summary>
        /// 执行简单的SQL语句
        /// </summary>
        /// <param name="SQLString">执行的SQL语句</param>
        /// <returns>返回的影响的记录</returns>
        public  int ExecuteSql(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 根据多长时间执行sql
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public  int ExecuteSqlByTime(string SQLString, int Times)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandTimeout = Times;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        conn.Close();
                        throw e;
                    }
                }
            }

        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务
        /// </summary>
        /// <param name="SQLStringList">SQL语句</param>
        /// <returns>影响行数</returns>
        public  int ExecuteSqlTran(List<string> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;

                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
            }
        }
        /// <summary>
        ///执行带一个参数的SQL语句
        /// </summary>
        /// <param name="SQLString">将要执行的SQL语句</param>
        /// <param name="content">参数的内容</param>
        /// <returns>影响的记录数</returns>
        public  int ExecuteSql(string SQLString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                System.Data.SqlClient.SqlParameter myParameter = new SqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 执行带参数的SQL语句
        /// </summary>
        /// <param name="SQLString">将要执行的SQL语句</param>
        /// <param name="content">参数的内容</param>
        /// <returns>具体查找到的内容</returns>
        public  object ExecuteSqlGet(string SQLString, string content)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(SQLString, connection);
                System.Data.SqlClient.SqlParameter myParameter = new SqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((object.Equals(obj, null)) || (object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// 向数据库里插入图像格式的内容
        ///  
        /// </summary>
        /// <param name="StrSql">SQL语句</param>
        /// <param name="fs">图像字节，数据库中的字段类型必须是image的情况</param>
        /// <returns>影响的记录行数</returns>
        public  int ExecuteSqlInserImg(string StrSql, byte[] fs)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(StrSql, connection);
                System.Data.SqlClient.SqlParameter myParameter = new SqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }

        }
        /// <summary>
        /// 执行一条SQL，返回它的查询结果
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>结果集</returns>
        public  object GetSingle(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if (object.Equals(obj, null) || (object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }

                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        public  object GetSingle(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        object obj = cmd.ExecuteScalar();
                        if (object.Equals(obj, null) || (object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回SqlDataReader(注意：调用该方法后，一定要对SqlDataReader进行close())
        /// </summary>
        /// <param name="StrSQL">将要执行的查询语句</param>
        /// <returns>SqlDataReader</returns>
        public  SqlDataReader ExecuteReader(string StrSQL)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(StrSQL, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">将要执行的查询语句</param>
        /// <returns>DataSet</returns>
        public  DataSet Query(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");

                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">将要执行的查询语句</param>
        /// <returns>DataSet</returns>
        public  DataTable QueryDataTable(string SQLString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataTable result = null;
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        result = ds.Tables[0];
                    }
                    connection.Close();

                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);

                }
                finally
                {

                }
                return result;
            }
        }


        public  DataSet Query(string SQLString, int Times)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = Times;
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="cmdparms">参数列表</param>
        /// <returns></returns>
        public  int ExecuteSql(string SQLString, params SqlParameter[] cmdparms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        connection.Open();
                        prepareCommand(cmd, connection, null, SQLString, cmdparms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;

                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }



        }

        /// <summary>
        /// 返回count(*)或sum(expression)查询类型的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns></returns>
        public  int GetRecordsCountOrSumValues(string SQLString)
        {
            DataTable dt = QueryDataTable(SQLString);
            if (dt != null)
            {
                int count = 0;
                int.TryParse(dt.Rows[0][0].ToString(), out count);
                return count;
            }
            else
            {
                return 0;
            }
        }


        private  void prepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParams)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParams != null)
            {
                foreach (SqlParameter parameter in cmdParams)
                {
                    if ((parameter.Direction == ParameterDirection.Input || parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);

                }
            }

        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库的事务
        /// </summary>
        /// <param name="SQLStringList">Sql语句的哈希表(key,表示的是SQL语句，value:表示该语句的SqlParameter[])</param>
        public  void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdparams = (SqlParameter[])myDE.Value;
                            prepareCommand(cmd, conn, trans, cmdText, cmdparams);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();


                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

        }
        /// <summary>
        ///  执行多条SQL语句，实现数据库的事务
        /// </summary>
        /// <param name="SQLStringList">Sql语句的哈希表(key,表示的是SQL语句，value:表示该语句的SqlParameter[])</param>

        public  void ExecuteSqlTranWith(Hashtable SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int indentity = 0;
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            SqlParameter[] cmdParams = (SqlParameter[])myDE.Value;
                            foreach (SqlParameter q in cmdParams)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            prepareCommand(cmd, conn, trans, cmdText, cmdParams);
                            int val = cmd.ExecuteNonQuery();
                            foreach (SqlParameter q in cmdParams)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 执行一条SQL语句，返回它的结果集
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <param name="cmdParams">参数</param>
        /// <returns>查询到结果集</returns>

        public  object GetSingle(string SQLString, params SqlParameter[] cmdParams)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        prepareCommand(cmd, connection, null, SQLString, cmdParams);
                        object obj = cmd.ExecuteScalar();
                        if ((object.Equals(obj, null)) || (object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }

                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;

                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回SqlDataReader(注意：调用该方法以后，一定要对SqlDataReader进行close)
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="cmParams"></param>
        /// <returns></returns>
        public  SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmParams)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                prepareCommand(cmd, connection, null, SQLString, cmParams);
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">要执行的SQL语句</param>
        /// <param name="cmdParams">SQL语句所需要的参数</param>
        /// <returns>返回DataSet</returns>
        public  DataSet Query(string SQLString, params SqlParameter[] cmdParams)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                prepareCommand(cmd, connection, null, SQLString, cmdParams);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }
        /// <summary>
        /// 执行存储过程，返回SqlDataReader(注意)
        /// </summary>
        /// <param name="StoredProcName">存储过程名称</param>
        /// <param name="parameters">存储过程需要的参数</param>
        /// <returns>SqlDataReader</returns>
        public  SqlDataReader RunProcedure(string StoredProcName, IDataParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlDataReader returnReader;
            connection.Open();
            SqlCommand command = BuildQuerycommand(connection, StoredProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;

        }
        /// <summary>
        /// 构建SqlCommand对象(返回一个结果集)
        /// </summary>
        /// <param name="connection">数据连接</param>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">存储过程需要的参数</param>
        /// <returns></returns>
        private  SqlCommand BuildQuerycommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;

        }
        /// <summary>
        /// 执行存储过程，返回DataSet
        /// </summary>
        /// <param name="storedProcName">存储过程的名称</param>
        /// <param name="parameters">存储过程需要的参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public  DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                conn.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQuerycommand(conn, storedProcName, parameters);
                sqlDA.Fill(ds, tableName);
                conn.Close();
                return ds;
            }
        }
        public  DataSet RunProcedure(string stroredProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                conn.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQuerycommand(conn, stroredProcName, parameters);
                sqlDA.SelectCommand.CommandTimeout = Times;
                sqlDA.Fill(ds, tableName);
                conn.Close();
                return ds;
            }
        }
        SqlConnection mSqlConnection;
        #region IDBase 成员

        public DBkind DBKind
        {
            get { return DBkind.Sqlserver; }
        }

        public IDbConnection DbConnection
        {
            get { return mSqlConnection; }
        }

        public void OpenConnection(string conncstr)
        {
            mSqlConnection = new SqlConnection(conncstr);
        }

        public DataSet QueryDataSet(string CommandString)
        {
            return this.Query(CommandString);
        }

        public bool CreateNonQueryCommand(string sqlstr)
        {
            return this.ExecuteSql(sqlstr) != 0;
        }

        public void Update(DataSet ds)
        {
            
        }

        #endregion

    }
}
