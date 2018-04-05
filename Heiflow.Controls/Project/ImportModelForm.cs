// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications;
using Heiflow.Applications.Services;
using Heiflow.Models.Generic.Project;
using Heiflow.Presentation;
using Heiflow.Presentation.Services;
using System;
using System.IO;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Project
{

    public partial class ImportModelForm : Form
    {
        private IProject _Project;
        private IImportProperty _ImportProperty;

        public ImportModelForm(IProject project)
        {
            InitializeComponent();
            _Project = project;
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            var import = MyAppManager.Instance.CompositionContainer.GetExportedValue<IImportModelService>();
            foreach (var prop in import.ImportProperties)
            {
                if (prop.Token == _Project.Token)
                {
                    propertyGrid1.SelectedObject = prop;
                    _ImportProperty = prop;
                    break;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_ImportProperty != null)
            {
                var service = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
                var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
                var prjcntl = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectController>();
                if (prjcntl.Import.CanExecute(_ImportProperty))
                {
                    prjcntl.Import.Execute(_ImportProperty);
                }
                else
                {
                    var msg = MyAppManager.Instance.CompositionContainer.GetExportedValue<IMessageService>();
                    msg.ShowError(shell.MainForm, "The work directory is wrong.");
                }
            }

        }
    }
}

