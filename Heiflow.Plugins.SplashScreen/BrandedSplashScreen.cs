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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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