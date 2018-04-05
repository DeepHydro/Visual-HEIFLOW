// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic
{

    public enum IOState { OLD, REPLACE };
    public class PackageInfo
    {
        private string _FileName;
        public PackageInfo()
        {

        }
        public string ModuleName { get; set; }
        public int FID { get; set; }
        public string FileExtension { get; set; }
        /// <summary>
        /// filename without path
        /// </summary>
        public string Name { get; set; }
        public string WorkDirectory  { get; set; }
        /// <summary>
        /// get absouloutly full filename or set as relative filename
        /// </summary>
        public string FileName 
        { 
            get
            {
                if (TypeConverterEx.IsNull(WorkDirectory))
                    WorkDirectory = ModelService.WorkDirectory;
                return Path.Combine(WorkDirectory, _FileName);
            }
            set
            {
                _FileName = value;
            }
        }

        public string RelativeFileName
        {
            get
            {
                return _FileName;
            }
        }
        public IOState IOState { get; set; }
        public FileFormat Format { get; set; }

    }
}
