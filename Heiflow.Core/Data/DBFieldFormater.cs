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
