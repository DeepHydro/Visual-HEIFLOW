// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Packages
{
    [Serializable]
    public class FeatureElement
    {
        public FeatureElement()
        {

        }
        /// <summary>
        /// the name shown in Lengend
        /// </summary>
        [XmlElement]
        public string FeatureName
        {
            get;
            set;
        }
        [XmlElement]
        public string PackageName
        {
            get;
            set;
        }
        [XmlIgnore]
        public string FeatureFilename
        {
            get;
            set;
        }
        [XmlElement]
        public string FeatureFilePath
        {
            get
            {    
                if (String.IsNullOrEmpty(FeatureFilename))
                    return null;

                return DirectoryHelper.RelativePathTo(FeatureFilename);
            }
            set
            {
                FeatureFilename = DirectoryHelper.AbsolutePathTo(value);
            }
        }

    }
}
