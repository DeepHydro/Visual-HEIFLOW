// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace DotSpatial.Plugins.SplashScreenManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.Windows.Forms;
    using DevExpress.XtraSplashScreen;
    using DotSpatial.Extensions.SplashScreens;
    using System.ComponentModel.Composition;
    using Heiflow.Plugins.SplashScreen;

    //[Export(typeof(ISplashScreenManager))]
    public class SplashScreenPlugin : ISplashScreenManager
    {
        public void ProcessCommand(Enum cmd, object arg)
        {
            if (SplashScreenManager.Default != null)
                SplashScreenManager.Default.SendCommand(cmd, arg);
        }

        public void Activate()
        {
            SplashScreenManager.ShowForm(typeof(BrandedSplashScreen), true, true);
        //    SplashScreenManager.ShowForm(typeof(Form1), true, true);
        }

        public void Deactivate()
        {
            SplashScreenManager.CloseForm();
        }
    }
}
