// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Applications.Views
{
    public interface IShellView : IView
    {
        double Left { get; set; }

        double Top { get; set; }

        double Width { get; set; }

        double Height { get; set; }

        bool IsMaximized { get; set; }


        event CancelEventHandler Closing;

        event EventHandler Closed;


        void Show();

        void Close();
    }
}
