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
