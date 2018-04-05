// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications.ViewModels;
using Heiflow.Applications.Views;
using Heiflow.Models.Generic.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Applications.Controllers
{
    [Export]
    public class LookupTableController
    {
        private LookupTableViewModel _ViewModel;

        [ImportingConstructor]
        public LookupTableController(LookupTableViewModel vm)
        {
            _ViewModel = vm;
        }

        public LookupTableViewModel ViewModel
        {
            get
            {
                return _ViewModel;
            }
        }

        public void Initialize()
        {

        }

        public void Shutdown()
        {
        }
    
    }
}
