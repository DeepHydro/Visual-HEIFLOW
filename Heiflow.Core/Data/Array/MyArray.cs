// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.MyMath;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public abstract class MyArray<T>
    {
         protected string[] _Variables;
         public MyArray()
         {
             Name = "Default";
         }
         
         public abstract T[] this[int i0, int i1]
         {
             get;
             set;
         }

         public abstract T this[int i0, int i1, int i2]
         {
             get;
             set;
         }

         public DateTime[] DateTimes
         {
             get;
             set;
         }

         public StatisticsInfo StatisticsInfo
         {
             get;
             set;
         }
      
         public string Name
         {
             get;
             set;
         }

         public string OwnerName
         {
             get;
             set;
         }

         public virtual string[] Variables
         {
             get
             {
                 if (_Variables == null)
                     PopulateVariables();
                 return _Variables;
             }
             set
             {
                 _Variables = value;
             }
         }

         public int[] Size
         {
             get;
             protected set;
         }

         protected virtual void Initialize()
         {

         }

         /// <summary>
         ///  Require 3 parameters for 3d mat,  2 parameters for 2d mat, 0parameters for vector and scalar.
         ///  For 3dmat: [full,c,v]; For 2dmat: [full,c,0]; 
         /// </summary>
         /// <param name="dims"></param>
         /// <returns></returns>
         public abstract T[] GetVector(int r1, int r2, int r3);

         /// <summary>
         /// Require 3 parameters for 3d mat,  2 parameters for 3d mat, 0parameters for vector and scalar.
         /// For 3dmat: [full,c,v, 0]; For 2dmat: [full,c,0,0]; 
         /// </summary>
         /// <param name="vector"></param>
         /// <param name="dims"></param>
         public abstract void SetBy(T[] vector, int r1, int r2, int r3);

         public abstract ILArray<T> ToILArray();
         
        public abstract void Constant(T cnst);

        public abstract double Sum();

        public virtual void PopulateVariables()
        {
            int nvar = Size[0];
            _Variables = new string[nvar];
            for (int i = 0; i < nvar; i++)
            {
                _Variables[i] = "V" + (i + 1);
            }
        }
        public virtual string SizeString()
        {
            if(Size != null)
            {
                string size = "";
                for (int i = 0; i < size.Length;i++ )
                {
                    size += "[" + size[i] + "]";
                }
                    return size;
            }
            else
            {
                return "Empty";
            }
        }

    }
}
