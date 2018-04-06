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
