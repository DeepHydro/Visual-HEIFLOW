// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Serialization;

namespace Heiflow.Core
{
    public enum DBkind { Oracle, Access2003, Access2007,Access2013, Sqlserver, Excel };
    public enum DbOptKind { Oledb, OracleClient,SqlClient };
    [Serializable]
    public class DBConnectInfo
    {
        [XmlElement]
        public string Password { get; set; }
        [XmlElement]
        public string UserName { get; set; }
        [XmlElement]
        public string DataSource { get; set; }
        [XmlElement]
        public string DatabaseName { get; set; }
    }
    /// <summary>
    /// 数据库访问接口
    /// </summary>
    public interface IDBase
    {
        DBkind DBKind { get; }
        IDbConnection DbConnection { get; }
        ConnectionState ConnectionState { get; }
        void OpenConnection(string conncstr);
        DataSet QueryDataSet(string CommandString);
        DataTable QueryDataTable(string CommandString);
        bool CreateNonQueryCommand(string sqlstr);
        bool Exists(string sqlstr);
        void Update(DataSet ds);
    }
    public interface IDTable
    {
        bool Exists();
        void Add();
        void Update();
        void Delete();
        void GetModel();
    }
}
