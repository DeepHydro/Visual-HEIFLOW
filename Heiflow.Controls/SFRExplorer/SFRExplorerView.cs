// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Controls.WinForm.TimeSeriesExplorer;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using Heiflow.Presentation.Services;
using Microsoft.Research.Science.Data.NetCDF4;
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

namespace Heiflow.Controls.WinForm.SFRExplorer
{
    [Export(typeof(IPackageOptionalView))]
    public partial class SFRExplorerView : Form, IPackageOptionalView
    {
        private IPackage _Package;
        private IProjectService _ProjectService;

        public SFRExplorerView()
        {
            InitializeComponent();
            this.FormClosing += SFRExplorerView_FormClosing;
            this.Load += SFRExplorerView_Load;
        }

        public object DataContext
        {
            get;
            set;
        }
        public IPackage Package
        {
            get
            {
                return _Package;
            }
            set
            {
                _Package = value;
                sfrExplorer1.SFROutput = _Package as SFROutputPackage;
            }
        }
        public string PackageName
        {
            get { return SFROutputPackage.PackageName; }
        }
        public string ChildName
        {
            get { return "SFRExplorerView"; }
        }
        private void SFRExplorerView_Load(object sender, EventArgs e)
        {
            _ProjectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            sfrExplorer1.ODM = _ProjectService.Project.ODMSource;
        }

        public void ShowView(IWin32Window pararent)
        {          
            if(!this.Visible)
                this.Show(pararent);
        }
        private  void SFRExplorerView_FormClosing(object sender, FormClosingEventArgs e)
        {
           if(e.CloseReason== CloseReason.UserClosing)
           {
               this.Hide();
               e.Cancel = true;
           }
            else
           {
               e.Cancel = false;
           }
        }
        public void ClearContent()
        {
            this.sfrExplorer1.ClearContent();
        }
        public void InitService()
        {
        
        }
    }  
}
