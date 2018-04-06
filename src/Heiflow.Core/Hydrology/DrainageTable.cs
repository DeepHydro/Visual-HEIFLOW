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
using Heiflow.Core.Data;

namespace Heiflow.Core.Hydrology
{
    public class DrainageTable : IDTable
    {
        private IDBase mDbase;
        public DrainageTable()
        {
            mDbase = Configuration.DataBase;
        }

        #region Model
        private string _drainage;
        private string _basin;
        /// <summary>
        /// 
        /// </summary>
        public string Drainage
        {
            set { _drainage = value; }
            get { return _drainage; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Basin
        {
            set { _basin = value; }
            get { return _basin; }
        }
        #endregion Model


        #region  成员方法

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from [Drainage]");
            strSql.Append("where Basin='" + Basin + "'and Drainage='" + Drainage + "'");
            return mDbase.Exists(strSql.ToString());
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into [Drainage](");
            strSql.Append("Drainage,Basin");
            strSql.Append(")");
            strSql.Append(" values (");
            strSql.Append("'" + Drainage + "',");
            strSql.Append("'" + Basin + "'");
            strSql.Append(")");
            mDbase.CreateNonQueryCommand(strSql.ToString());
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public void Update()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Drainage set ");
            strSql.Append("Drainage='" + Drainage + "'");
            strSql.Append(" where Basin='" + Basin + "'and Drainage='" + Drainage + "'");
            mDbase.CreateNonQueryCommand(strSql.ToString());
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public void Delete()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Drainage ");
            strSql.Append(" where Basin='" + Basin + "'and Drainage='" + Drainage + "'");
            mDbase.CreateNonQueryCommand(strSql.ToString());
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public void GetModel()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  ");
            strSql.Append("[Drainage],[Basin] ");
            strSql.Append(" from Drainage ");
            strSql.Append("where Basin='" + Basin + "' and Drainage='" + Drainage + "'");
            DataSet ds = mDbase.QueryDataSet(strSql.ToString());
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Drainage = ds.Tables[0].Rows[0]["Drainage"].ToString();
                Basin = ds.Tables[0].Rows[0]["Basin"].ToString();
            }
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select [Drainage],[Basin] ");
            strSql.Append(" FROM Drainage ");
            if (strWhere.Trim() != "")
            {

                strSql.Append(" where " + strWhere);
            }
            return mDbase.QueryDataSet(strSql.ToString());
        }

        public string[] GetBasinList()
        {
            string[] result = null;
            string strSql = "SELECT [Basin] FROM Drainage GROUP BY [Basin] HAVING count(*)>0 ";
            DataSet ds = mDbase.QueryDataSet(strSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                result = new string[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    result[i] = ds.Tables[0].Rows[i]["Basin"].ToString();
                }
            }
            return result;
        }

        public string[] GetDrainageList(string basin)
        {
            string[] result = null;
            string strSql = "SELECT [Drainage] FROM Drainage where basin='" + basin + "'";
            DataSet ds = mDbase.QueryDataSet(strSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                result = new string[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    result[i] = ds.Tables[0].Rows[i][0].ToString();
                }
            }
            return result;
        }

        #endregion  成员方法
    }
}
