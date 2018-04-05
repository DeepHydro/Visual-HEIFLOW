// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.IO;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System;
using System.Xml;
using System.Xml.Serialization;
using Heiflow.Core.Animation;
using Heiflow.Core;
using Heiflow.Models.Visualization;

namespace Heiflow.Models.GHM
{

    [Serializable]
    public class Item
    {
        public Item()
        {

        }

        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// RelativePath
        /// </summary>
        [XmlAttribute]
        public string Path
        {
            get;
            set;
        }

        [XmlIgnore]
        public string FullPath
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Description
        {
            get;
            set;
        }


        [XmlIgnore]
        public IGrid Grid
        {
            get;
            set;
        }
        [XmlIgnore]
        public I3DLayerRender ModelRender
        {
            get;
            set;
        }

        public MyArray<float> LoadArray()
        {
            MyArray<float> mat = null;
            var provider = FileProviderFactory.GetProvider(FullPath);
            if (provider is IMatFileProvider)
                mat = (provider as IMatFileProvider).LoadSerial(FullPath, Grid);
            return mat;
        }

        public IVectorTimeSeries<double> LoadTimeSeries()
        {
            IVectorTimeSeries<double> ts = null;
            var provider = FileProviderFactory.GetProvider(FullPath);
            if (provider is ITimeSeriesFileProvider)
                ts = (provider as ITimeSeriesFileProvider).Load();
            return ts;
        }
    }
}
