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
#define VFT3D
namespace Heiflow.Models.Studio
{
    using System.Windows.Forms;
    using System.ComponentModel.Composition;
    using DotSpatial.Controls.Docking;
    using System.ComponentModel.Composition.Hosting;
    using System.Reflection;
    using Heiflow.Applications;
    using Heiflow.Presentation.Controls.Project;
    using Heiflow.Presentation.Controls;
    using Heiflow.Presentation;
    using Heiflow.Tools.Conversion;
    using System.ComponentModel;
    using Heiflow.AI;
    using Heiflow.Models.IO;
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// The main form.
    /// </summary>

    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm, IWin32Window
    {
        [Export("Shell", typeof(ContainerControl))]
        private static ContainerControl Shell;

        [Export("ProjectController", typeof(IProjectController))]
        private static IProjectController _ProjectController;

        [Export("VHFManager", typeof(VHFAppManager))]
        private static VHFAppManager _VHFAppManager;
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MainForm" /> class.
        /// </summary>
        public MainForm()
        {
         // SecurityFile.Generate("e:\\vgs.dll", "cfgt-2lp5-cwp3-drkm", "VHF");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
            this.InitializeComponent();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

            Shell = this;
            appManager1.InitializeContainer();
         
            Assembly modelDll = typeof(Heiflow.Models.Integration.HeiflowModel).Assembly;
            appManager1.Catalog.Catalogs.Add(new AssemblyCatalog(modelDll));
            Assembly cntlDll = typeof(NewProjectionForm).Assembly;
            appManager1.Catalog.Catalogs.Add(new AssemblyCatalog(cntlDll));
            Assembly presentDll = typeof(IProjectExplorer).Assembly;
            appManager1.Catalog.Catalogs.Add(new AssemblyCatalog(presentDll));
            Assembly appDll = typeof(VHFAppManager).Assembly;
            appManager1.Catalog.Catalogs.Add(new AssemblyCatalog(appDll));
            Assembly toolDll = typeof(ToRasterList).Assembly;
            appManager1.Catalog.Catalogs.Add(new AssemblyCatalog(toolDll));

            _VHFAppManager = new VHFAppManager();
#if VHF
            _VHFAppManager.AppName = "Visual HEIFLOW";
            
#elif VFT3D
            _VHFAppManager.AppName = "Visual VFT3D";
#endif
            //_VHFAppManager.AppName = "山洪模拟及水情预报系统";
            //_VHFAppManager.Icon = ((System.Drawing.Icon)(resources.GetObject("heiflow")));

            _VHFAppManager.ApplicationPath = Application.StartupPath;
            _VHFAppManager.MapAppManager = appManager1;
            appManager1.CompositionContainer.ComposeParts(_VHFAppManager);

            VHFAppManager.Instance = _VHFAppManager;
            VHFAppManager.Instance.CompositionContainer = appManager1.CompositionContainer;

            _VHFAppManager.ProjectController.ShellService.MainForm = this;
            _ProjectController = _VHFAppManager.ProjectController;
            _VHFAppManager.Initialize();

            this.appManager1.LoadExtensions();
            this.appManager1.DockManager.Add(new DockablePanel("kMap",Resources.Map_panel, map1, DockStyle.Fill));

            // Add default buttons
            new DotSpatial.Controls.DefaultMenuBars(appManager1).Initialize(appManager1.HeaderControl);
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            //appManager1.Map
            _VHFAppManager.ShellService.AddNativeChildren();
            _VHFAppManager.ShellService.InitChildViewService();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _VHFAppManager.IsExiting = true;
            _VHFAppManager.Shutdown();           
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            //_license = new MyLicense();
            //_license.Init();
            //if (!_VHFAppManager.CheckLicense())
            //{
            //    LicenseForm form = new LicenseForm();
            //    if(form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {

            //    }
            //    else
            //    {
            //        MessageBox.Show("软件授权失败，程序将退出", "许可", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        Application.Exit();
            //    }
  
            //}
         
            this.Text = _VHFAppManager.AppName;
     
        }

        #endregion Constructors and Destructors
    }
}