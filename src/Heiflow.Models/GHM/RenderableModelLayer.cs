﻿//
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

using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.GHM
{
    [Serializable]
    public class RenderableModelLayer
    {
        public RenderableModelLayer()
        {
            LayerIsOn = true;
        }
        private bool _LayerIsOn = true;

        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }
        [XmlIgnore]
        public bool LayerIsOn
        {
            get
            {
                if(IsOn != null)
                    _LayerIsOn = IsOn.ToUpper() == "TRUE";
                return _LayerIsOn;
            }
            set
            {
                _LayerIsOn = value;
            }
        }
        [XmlAttribute]
        public string RenderName
        {
            get;
            set;
        }
        [XmlElement]
        public double DistanceAboveSurface
        {
            get;
            set;
        }
        [XmlElement]
        public double MaxDisplayAltitude
        {
            get;
            set;
        }
        [XmlElement]
        public string IsOn
        {
            get;
            set;
        }
        [XmlElement]
        public double MinDisplayAltitude
        {
            get;
            set;
        }
        [XmlElement]
        public string DataSource
        {
            get;
            set;
        }
        [XmlIgnore]
        public string FullDataSource
        {
            get
            {
                return Path.Combine(ModelService.WorkDirectory, DataSource);
            }
        }
    }
}
