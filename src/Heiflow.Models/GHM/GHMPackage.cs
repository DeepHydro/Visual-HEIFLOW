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

using DotSpatial.Data;
using Heiflow.Core.Animation;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.UI;
using Heiflow.Models.Visualization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.GHM
{
    [PackageItem]
    [Serializable]
    public class GHMPackage : Package
    {
        public GHMPackage()
        {
            _Name = "GHM Package";
            StaticVariables = new List<StaticVariable>();
            DynamicVariables = new List<DynamicVariable>();
        }

        [Browsable(false)]
        [XmlIgnore]
        public GHMSerializer Serializer
        {
            get;
            set;
        }
         [Browsable(false)]
         [XmlIgnore]
        public GHModel GHModel
        {
            get;
            set;
        }
        [Browsable(false)]
        [XmlArrayItem]
         public List<StaticVariable> StaticVariables
         {
             get;
             set;
         }
        [Browsable(false)]
        [XmlArrayItem]
        public List<DynamicVariable> DynamicVariables
        {
            get;
            set;
        }

        public override void Save(ICancelProgressHandler progress)
        {
            throw new NotImplementedException();
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            throw new NotImplementedException();
        }

        public override void New()
        {
            base.New();
        }

        public override void Serialize(string filename)
        {
            
        }

        public override void Deserialize(string filename)
        {
            
        }


        public override LoadingState Load(ICancelProgressHandler progess)
        {
            return LoadingState.Normal;
        }
    }
}
