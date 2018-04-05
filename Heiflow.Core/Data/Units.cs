// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core
{
    [Serializable]
    public class Units:IUnits
    {
        public Units()
        {
        }

        public Units(int id)
        {
            mId = id;
            Name = "Unknown";
            AliasName = "Unknown";
            UnitType = "Unknown";
            Abbreviation = "Unknown";

        }
        private int mId;

        #region IUnits 成员

        public int ID
        {
            get
            {
                return mId;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string AliasName
        {
            get;
            set;
        }

        public string UnitType
        {
            get;
            set;
        }

        public string Abbreviation
        {
            get;
            set;
        }

        #endregion
    }
}
