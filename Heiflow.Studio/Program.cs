// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Heiflow.Models.Studio
{
    using System;
    using System.Windows.Forms;

    using DevExpress.LookAndFeel;

    using DotSpatial;
    using System.Diagnostics;

    /// <summary>
    /// The program.
    /// </summary>
    internal static class Program
    {
        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            //float a=0.001f;
            //Debug.WriteLine(a.ToString("E5"));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        #endregion Methods
    }
}