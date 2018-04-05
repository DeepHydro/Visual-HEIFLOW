// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Generic.Project
{
    /// <summary>
    ///
    /// </summary>
    [InheritedExport]
    public interface IOpenProjectFileProvider : IFileProvider
    {
        /// <summary>
        /// Opens the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        IProject Open(string fileName);

        string ProviderName { get; }
    }
}
