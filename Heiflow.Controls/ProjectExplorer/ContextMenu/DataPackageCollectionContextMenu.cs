// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.MenuItems
{
    [Export(typeof(IPEContextMenu))]
    public class DataPackageCollectionContextMenu:PackageContextMenu
    {
        public DataPackageCollectionContextMenu()
        {

        }
        public override Type ItemType
        {
            get
            {
                return typeof(DataPackageCollectionItem);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            base.Enable(_Save, false);
            base.Enable(_SaveAs, false);
            base.Enable(_CS, false);
            base.Enable(_RM, false);
            base.Enable(_EX, false);
        }


    }
}
