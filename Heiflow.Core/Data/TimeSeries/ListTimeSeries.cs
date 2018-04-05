// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
   public class ListTimeSeries<T>
    {
       public ListTimeSeries(int nvar)
       {
           Dates = new List<DateTime>();
           Values = new List<T>[nvar];
           for (int i = 0; i < nvar; i++)
           {
               Values[i] = new List<T>();
           }
       }

       public List<DateTime> Dates
       {
           get;
           set;
       }
       /// <summary>
       /// [var][step]
       /// </summary>
       public List<T>[]  Values
       {
           get;
           set;
       }

       public void Add(DateTime date, T[] row)
       {
           Dates.Add(date);
           var len = Math.Min(row.Length, Values.Length);
           for (int i = 0; i < len; i++)
           {
               Values[i].Add(row[i]);
           }
       }

       public void Clear()
       {
           Dates.Clear();
           for (int i = 0; i < Values.Length; i++)
           {
               Values[i].Clear();
           }
       }
    }
}
