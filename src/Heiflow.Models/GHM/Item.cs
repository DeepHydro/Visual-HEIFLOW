//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

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
