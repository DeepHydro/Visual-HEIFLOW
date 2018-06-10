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
                    this.Close();
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

