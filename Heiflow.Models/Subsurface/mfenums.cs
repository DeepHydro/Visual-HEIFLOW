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

namespace Heiflow.Models.Subsurface
{
    public enum ModelState { SS,TR};
    public enum LAYTYP { Confined = 0, Convertable = 1 };
    public enum LAYAVG { Harmonic_Mean = 0, Logarithmic_Mean = 1, Arithmetic_logarithmic = 2 };
    public enum LAYVKA { Vertical_hydraulic_conductivity = 0, Ratio_of_horizontal_to_vertical_hydraulic_conductivity = 1};
    public enum LAYWET { Inactive =0, Active=1};
    public enum CHANI { Define = -1, NotDefine =1};
}
