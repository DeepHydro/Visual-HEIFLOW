// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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


        public override bool LoadGrid(IProgress progress)
        {
            return true;
        }
        public override bool Load(IProgress progress)
        {
            return true;
        }

        public override bool New(IProgress progress)
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

        public override void Save(IProgress progress)
        {
            foreach (var mod in Children.Values)
            {
                mod.Save(progress);
            }
        }
    }
}
