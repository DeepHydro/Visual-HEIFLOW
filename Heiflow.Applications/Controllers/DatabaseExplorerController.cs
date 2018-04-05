// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Applications.Controllers
{
    [Export]
    public class DatabaseExplorerController
    {
        private DatabaseExplorerViewModel _ViewModel;

        [ImportingConstructor]
        public DatabaseExplorerController(DatabaseExplorerViewModel vm)
        {
            _ViewModel = vm;
        }

        public DatabaseExplorerViewModel ViewModel
        {
            get
            {
                return _ViewModel;
            }
        }
    }
}
