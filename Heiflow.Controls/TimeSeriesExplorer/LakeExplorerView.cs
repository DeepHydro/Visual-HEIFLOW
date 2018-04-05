// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.TimeSeriesExplorer
{
    [Export(typeof(IPackageOptionalView))]
    public partial class LakeExplorerView : Form, IPackageOptionalView
    {
        private IProjectService _ProjectService;
        public LakeExplorerView()
        {
            InitializeComponent();
            this.FormClosing += LakeExplorerView_FormClosing;
            this.Load += LakeExplorerView_Load;
        }

        public bool CloseRequired
        {
            get;
            set;
        }

        public string PackageName
        {
            get
            { 
                return LakePackage.PackageName; 
            }
        }

        public IPackage Package
        {
            get;
            set;
        }
        public object DataContext
        {
            get;
            set;
        }
        public string ChildName
        {
            get { return "LakeExplorerView"; }
        }
        public void ShowView(IWin32Window pararent)
        {
            this.Show(pararent);
        }

        private void LakeExplorerView_Load(object sender, EventArgs e)
        {
            this.timeSeriesExplorer1.Package = Package;
            _ProjectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            timeSeriesExplorer1.ODM = _ProjectService.Project.ODMSource;
        }

        private void LakeExplorerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CloseRequired)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
        public void ClearContent()
        {
            timeSeriesExplorer1.ClearContent();
        }
        public void InitService()
        {

        }
    }
}
