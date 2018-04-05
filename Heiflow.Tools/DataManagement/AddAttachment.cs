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