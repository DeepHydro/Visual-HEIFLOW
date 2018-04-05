using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
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

namespace Heiflow.Models.Visualization
{
    public interface I3DLayer
    {
        string Name { get; set; }
        string Token { get; set; }
        I3DLayerRender RenderObject { get; set; }
        /// <summary>
        /// A layer can has multiple packages, but a package can only have one layer.
        /// </summary>
        List<IPackage> Packages { get; }
        IProject Project { get; set; }
        void AddPackage(IPackage pck);
        IPackage Select(string pck_name);
    }
}
