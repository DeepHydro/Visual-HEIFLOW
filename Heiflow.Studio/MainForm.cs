// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

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
        private MyLicense _license;
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MainForm" /> class.
        /// </summary>
        public MainForm()
        {
          //  SecurityFile.Generate("e:\\vgs.dll", "cfgt-2lp5-cwp3-drkm", "VHF");
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
            Assembly toolDll = typeof(ToTIFSets).Assembly;
            appManager1.Catalog.Catalogs.Add(new AssemblyCatalog(toolDll));

            _VHFAppManager = new VHFAppManager();
            _VHFAppManager.AppName = "Visual HEIFLOW";
           // _VHFAppManager.Icon = ((System.Drawing.Icon)(resources.GetObject("heiflow")));

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
            this.appManager1.DockManager.Add(new DockablePanel("kMap", "Map", map1, DockStyle.Fill));

            // Add default buttons
            new DotSpatial.Controls.DefaultMenuBars(appManager1).Initialize(appManager1.HeaderControl);
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
            DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;

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