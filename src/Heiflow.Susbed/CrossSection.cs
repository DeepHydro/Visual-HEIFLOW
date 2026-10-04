// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public class CrossSection
    {
        private int _NumNodes;
        public CrossSection()
        {

        }

        public int ID { get; set; }
        public string Name { get; set; }
        public double Length { get; set;}
        public int NumNodes
        {
            get
            {
                return _NumNodes;
            }
            set
            {
                _NumNodes = value;
                Distance = new double[_NumNodes];
                Elevation = new double[_NumNodes];
            }
        }
        public double[] Distance { get; protected set; }
        public double[] Elevation { get; protected set; }

        public int SegmentID { get; set; }

        public DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("序号", Type.GetType("System.Int32"));
            dt.Columns.Add(dc);
            dc = new DataColumn("距离", Type.GetType("System.Double"));
            dt.Columns.Add(dc);
            dc = new DataColumn("高程", Type.GetType("System.Double"));
            dt.Columns.Add(dc);
            for (int i = 0; i < NumNodes;i++ )
            {
                var dr = dt.NewRow();
                dr[0] = (i + 1);
                dr[1] = Distance[i];
                dr[2] = Elevation[i];
                dt.Rows.Add(dr);
            }
                return dt;
        }
            

    }
}
