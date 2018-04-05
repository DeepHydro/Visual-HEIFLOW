﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.IO;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic.Project
{
    [InheritedExport]
    public interface IModelLoader
    {
        string Extension { get; }
        string FileTypeDescription { get; }

        bool Load( IProject project, IProgress progress);
        bool CanImport(IProject project);
        void Import(IProject project, IImportProperty property, IProgress progress);
    }
}