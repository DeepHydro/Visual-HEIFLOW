// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Visualization
{
    public interface I3DLayerRender
    {
        IGrid Grid { get; set; }
        string Name { get; set; }
        void Render(float [] vector);

        MyArray<float> DataSource { get; set; }

        int VarIndex { get; set; }
        
        double EquatorialRadius { get; set; }

        float VerticalExaggeration { get; set; }

        void CacheColor();

        void UpdateCachedColor();

        void Initilize();

        float GetCellValue(int row, int col);

        I3DLayer LayerObject { get; set; }
    }
}
