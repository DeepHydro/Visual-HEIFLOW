// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Heiflow.Core.Hydrology
{
    /// <summary>
    ///  Basin
    /// </summary>
    public class Basin : HydroArea
    {
        public Basin(int id)
            : base(id)
        {
            HydroFeatureType = HydroFeatureType.Basin;
        }

        public Watershed[] SubWatersheds
        {
            get
            {
                var ws = from w in SubFeatures where w is Watershed select w;
                return ws.Cast<Watershed>().ToArray();
            }
        }
    }

    /// <summary>
    ///  A watershed refers to a divide that separates one drainage area from another drainage area.
    ///  However, in the United States and Canada, the term is often used to mean a drainage basin 
    ///  or catchment area itself 
    /// </summary>
    public class Watershed : HydroArea
    {
        public Watershed(int id)
            : base(id)
        {
            HydroFeatureType = HydroFeatureType.Watershed;
        }
    }

    public class IrrigationSystem : HydroArea
    {
        public IrrigationSystem(int id)
            : base(id)
        {
            HydroFeatureType = HydroFeatureType.IrrigationSystem;
        }
    }
}
