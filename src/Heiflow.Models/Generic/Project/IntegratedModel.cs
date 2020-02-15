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
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic
{
    [Serializable]
    public class IntegratedModel : BaseModel
    {
        public IntegratedModel()
        {
            Name = "Integrated Model";

        }

        public override bool Validate()
        {
            return false;
        }

        public override void Initialize()
        {

        }


        public override bool LoadGrid(ICancelProgressHandler progress)
        {
            return true;
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            return LoadingState.Normal;
        }

        public override bool New(ICancelProgressHandler progress)
        {
            return true;
        }
        public override IPackage GetPackage(string pck_name)
        {
            IPackage pck = null;
            foreach (var mm in Children.Values)
            {
                pck = mm.GetPackage(pck_name);
                if (pck != null)
                    break;
            }
            if(pck == null)
            {
                var buf = from pp in Packages.Values where pp.Name == pck_name select pp;
                if (buf.Any())
                    pck = buf.First();
            }
            return pck;
        }
        public override void Clear()
        {
            foreach (var model in Children.Values)
            {
                model.Clear();
            }
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            foreach (var model in Children.Values)
            {
                model.Attach(map,  directory);
            }
        }

        public override void Save(ICancelProgressHandler progress)
        {
            foreach (var mod in Children.Values)
            {
                mod.Save(progress);
            }
        }
    }
}
