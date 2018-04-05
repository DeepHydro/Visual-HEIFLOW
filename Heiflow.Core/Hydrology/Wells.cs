// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Hydrology
{
   public class Wells:HydroPoint
    {
       public Wells(int id)
           : base(id)
       {
           
       }
    }

   public class ModflowWells : Wells
   {
       public ModflowWells(int id)
           : base(id)
       {
           
       }

       public int LAYER { get; set; }
       public int ROW { get; set; }
       public int COLUMN { get; set; }
       public int IREFSP { get; set; }

       public double TOFFSET { get; set; }
       public double ROFF { get; set; }
       public double COFF { get; set; }
       public double HOBS { get; set; }

        //OBSNAM LAYER ROW COLUMN IREFSP TOFFSET ROFF COFF HOBS
   }
}
