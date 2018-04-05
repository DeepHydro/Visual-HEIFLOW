// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using DevExpress.XtraSplashScreen;
using DotSpatial.Extensions.SplashScreens;
using System.IO;
using System.Reflection;
using System.Configuration;
using Heiflow.Plugins.SplashScreen.Properties;

namespace DotSpatial.Plugins.SplashScreenManager
{
    public partial class BrandedSplashScreen : DevExpress.XtraSplashScreen.SplashScreen
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public BrandedSplashScreen()
        {
            InitializeComponent();
            LoadCustomBranding(Settings.Default);
        }

        private void LoadCustomBranding(Settings settings)
        {
            if (!String.IsNullOrWhiteSpace(settings.CustomSplashImagePath) &&
                System.IO.File.Exists(settings.CustomSplashImagePath))
            {
                var image = System.Drawing.Image.FromFile(settings.CustomSplashImagePath);
                uxSplashImage.EditValue = image;
            }
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            var command = (SplashScreenCommand)cmd;

            switch (command)
            {
                case SplashScreenCommand.SetDisplayText:
                    uxMessage.Text = arg.ToString();
                    break;

                default:
                    break;
            }

            base.ProcessCommand(cmd, arg);
        }

        #endregion

        private void BrandedSplashScreen_Load(object sender, EventArgs e)
        {
            // TopMost would prevent Message Boxes from being seen.
         //   SetForegroundWindow(this.Handle);
        }


    }
}