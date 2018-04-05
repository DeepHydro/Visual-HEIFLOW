// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Heiflow.Core.IO;

namespace Heiflow.Models.Generic.Project
{
     [InheritedExport]
    public interface ISaveProjectFileProvider : IFileProvider
    {
        /// <summary>
        /// Saves the specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="graph">The control graph.</param>
        void Save(string fileName, IProject project);
    }
}
