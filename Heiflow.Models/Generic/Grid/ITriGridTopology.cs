// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
namespace Heiflow.Models.Generic
{
    public interface ITriGridTopology
    {
        float GetVertexValue(float[] vector, int i);
         ITriangularGrid Grid { get; set; }
        uint[][] NeigbouringCells { get; set; }
        uint[][] NodeConnectedCells { get; set; }
        uint[] SuccessiveVertexIndices { get; }
        uint[][] VertexIndices { get; set; }
    }
}
