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

using Heiflow.Core;

namespace Heiflow.Core.Data
{
   public  static class DBFieldFormater
    {
       
       /// <summary>
       /// Format sql expression to query datetime.
       /// The sql expression used to query datetime differs in different types of databases.
       /// This method formats datetime querying expression for each of the databases.
       /// The formatted expression looks like: fieldname>='2001-1-1' and fieldname = '2011-1-1'"
       /// </summary>
       /// <param name="fieldName"></param>
       /// <param name="start"></param>
       /// <param name="end"></param>
       /// <param name="dbkind"></param>
       /// <returns></returns>
       public static string FormatDateTime(string fieldName, DateTime start,DateTime end, DBkind dbkind)
       {
           string str = "";
           if (dbkind == DBkind.Sqlserver)
           {
               str = fieldName + ">='" + start.ToString() + "' and " + fieldName + "<='" + end.ToString() + "'";
           }
           else if (dbkind == DBkind.Access2003 || dbkind == DBkind.Access2007)
           {
               str = fieldName + ">=#" + start.ToString() + "# and " + fieldName + "<=#" + end.ToString() + "#";
           }
           return str;
       }
    }
}
