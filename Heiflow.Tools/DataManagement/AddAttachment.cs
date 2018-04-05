// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using System.IO;
using Heiflow.Models.Generic;
using System.ComponentModel;
using Heiflow.Controls.WinForm.Editors;
using System.Windows.Forms.Design;
using GeoAPI.Geometries;
using Heiflow.Core.IO;

namespace Heiflow.Tools.DataManagement
{
    public class AddAttachment : ModelTool
    {
        public AddAttachment()
        {
            Name = "Add Attachment";
            Category = "Data Management";
            Description = "Add attachment  to a dcx file. The attachment is XML document that describs the content the dcx file";
            NVar = 1;
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The full file name of the time stamp filename. The  file is a txt file that has one column:  date")]
        public string TimeStampsFileName { get; set; }

        [Category("Input")]
        [Description("The full file name of the coordinate filename. The  file is a csv file that has two columns:  x, y")]
        public string CoordinateFileName { get; set; }

        [Category("Input")]
        [Description("The full file name of the dcx file")]
        public string DcxFileName { get; set; }

        [Category("Input")]
        [Description("The number of variables")]
        public int NVar { get; set; }

        public override void Initialize()
        {
            Initialized = File.Exists(DcxFileName) && File.Exists(TimeStampsFileName);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamReader sr = new StreamReader(TimeStampsFileName);
            var line = sr.ReadLine().Trim();

            DataCubeDescriptor descriptor = new DataCubeDescriptor();
            int steps = 0;
            int ncell = 0;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (TypeConverterEx.IsNull(line))
                    break;
                steps++;
            }
            sr.Close();

            descriptor.NTimeStep = steps;
            descriptor.NVar = this.NVar;
            descriptor.TimeStamps = new DateTime[steps];

            sr = new StreamReader(TimeStampsFileName);
            line = sr.ReadLine();
            DateTime cur = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            for (int i = 0; i < steps; i++)
            {
                line = sr.ReadLine().Trim();
                DateTime.TryParse(line, out cur);
                descriptor.TimeStamps[i] = cur;
                cur = cur.AddDays(i);
            }
            sr.Close();

            if (File.Exists(CoordinateFileName))
            {
                sr = new StreamReader(CoordinateFileName);
                line = sr.ReadLine().Trim();
                var buf = TypeConverterEx.Split<string>(line);
                if(buf.Length == 2)
                {
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        if (TypeConverterEx.IsNull(line))
                            break;
                        ncell++;
                    }
                    sr.Close();

                    descriptor.NCell = ncell;
                    descriptor.XCoor = new double[ncell];
                    descriptor.YCoor = new double[ncell];
                    sr = new StreamReader(CoordinateFileName);
                    line = sr.ReadLine();
                    for (int i = 0; i < ncell; i++)
                    {
                        line = sr.ReadLine().Trim();
                        var vv = TypeConverterEx.Split<double>(line);
                        descriptor.XCoor[i] = vv[0];
                        descriptor.YCoor[i] = vv[1];
                    }
                    sr.Close();
                }
            }
            DataCubeDescriptor.Serialize(DcxFileName + ".xml", descriptor);
            return true;
        }

    }
}