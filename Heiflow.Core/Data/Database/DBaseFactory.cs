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

using Heiflow.Core;


namespace Heiflow.Core.Data
{
      /// <summary>
    /// Creates proper object that provides access to the database
    /// </summary>
    public class DBaseFactory
    {
        private  IDBase oledbOptDbase;
#if UseOracle
       private static IDBase oralclientOptDbase;
#endif

        public DBaseFactory()
        {

        }

        public  IDBase CreateInitialDbClass(DBkind dbkind, DbOptKind optkind, DBConnectInfo info)
        {
            switch (optkind)
            {
                case DbOptKind.Oledb:
                    oledbOptDbase = new OleDBase(dbkind, info);
                    break;
                case DbOptKind.SqlClient:
                    oledbOptDbase = new SqlServerDatabase(info);
                    break;
#if UseOracle
               case DbOptKind.OracleClient:
                    oralclientOptDbase = new OracleDbase(info);
                    break;
#endif
                default:
                    break;
            }
            IDBase result = null;
            if (optkind == DbOptKind.Oledb)
            {
                result= oledbOptDbase;
            }
#if UseOracle
            else
            {
                result = oralclientOptDbase;
            }
#endif
            return result;
        }

        public  IDBase GetOleDboptClass()
        {
            if (oledbOptDbase != null)
            {
                return oledbOptDbase;
            }
            else
            {
                throw new ArgumentException("The IDBase object has not been instantiated!");
            }
        }
 #if UseOracle
        public static IDBase GetOralDboptClass()
        {
            if (DBaseFactory.oralclientOptDbase != null)
            {
                return DBaseFactory.oralclientOptDbase;
            }
            else
            {
                throw new ArgumentException("未实例化数据库访问类");
            }

        }
#endif
    }

}
