// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Symbology;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel.Composition;

namespace Heiflow.Controls.WinForm.MenuItems
{
    [Export(typeof(IPEContextMenu))]
    public class PackageContextMenu : PEContextMenu, IPackageContextMemu
    {
        protected const string _Save = "Save";
        protected const string _SaveAs = "Save As...";
        protected const string _EX = "Export...";
        protected const string _RM = "Remove";
    
        protected const string _CS = "Coverage Setup...";
      //  protected const string _FS = "FeatureSet...";
        protected const string _AD = "Advanced...";

        public PackageContextMenu()
        {
            LegendSymbolMode = DotSpatial.Symbology.SymbolMode.Symbol;
            LegendType = DotSpatial.Symbology.LegendType.Symbol;
        }

        public override Type ItemType
        {
            get
            {
                return typeof(PackageItem);
            }
        }

        public override void AddMenuItems()
        {
            ContextMenuItems.Add(new ExplorerMenuItem(_Save, null, Save_Clicked));
            ContextMenuItems.Add(new ExplorerMenuItem(_SaveAs, Resources.GenericSave_B_16, SaveAs_Clicked));
            
            ContextMenuItems.Add(new ExplorerMenuItem(_EX, null, Export_Clicked));
            if (MyAppManager.Instance.AppMode == AppMode.VHF)
            {
         
                ContextMenuItems.Add(new ExplorerMenuItem(PEContextMenu.MenuSeparator, null, null));
                ContextMenuItems.Add(new ExplorerMenuItem(_CS, Resources.MapPackageTiledTPKFile16, CoverageSetup_Clicked));
                //ContextMenuItems.Add(new ExplorerMenuItem(_FS, null, FeatureSet_Clicked));
            }
            ContextMenuItems.Add(new ExplorerMenuItem(_AD, Resources.GenericWindowLightBlue16, Optional_Clicked));
            if (MyAppManager.Instance.AppMode == AppMode.VHF)
            {
                ContextMenuItems.Add(new ExplorerMenuItem(PEContextMenu.MenuSeparator, null, null));
                ContextMenuItems.Add(new ExplorerMenuItem(_RM, Resources.LayerRemove16, Remove_PackageClicked));
            }
        }

        protected virtual void Save_Clicked(object sender, EventArgs e)
        {
            if ( _Package.State == ModelObjectState.Ready)
            {
                Cursor.Current = Cursors.WaitCursor;
                _Package.IsDirty = true;
                _Package.Save(null);
                Cursor.Current = Cursors.Arrow;
            }
        }

        private void ProgressPanel_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            _Package.Save(_ShellService.ProgressWindow);
        }

        private void Package_Saving(object sender, int e)
        {
            string msg = string.Format("Saving {0}%", e);
            _ShellService.ProgressWindow.Progress(e, msg);
        }

        protected virtual void Package_Saved(object sender, EventArgs e)
        {
            _Package.Loading -= Package_Saving;
            _Package.Saved -= Package_Saved;
            _ShellService.ProgressWindow.DoWork -= ProgressPanel_DoWork;
        }
        protected virtual void ProgressWindow_WorkCompleted(object sender, EventArgs e)
        {
            _ShellService.ProgressWindow.WorkCompleted -= ProgressWindow_WorkCompleted;
        }

        protected virtual void SaveAs_Clicked(object sender, EventArgs e)
        {
            string extension = Path.GetExtension(_Package.FileName);
            SaveFileDialog form = new SaveFileDialog();
            form.Filter = string.Format("package file |*{0}", extension);
            if (form.ShowDialog() == DialogResult.OK)
            {
              //  _Package.FileName = form.FileName;
                _Package.SaveAs(form.FileName, null);
           //     Save_Clicked(sender, e);
            }
        }

        protected virtual void Export_Clicked(object sender, EventArgs e)
        {
            string extension = Path.GetExtension(_Package.FileName);
            SaveFileDialog form = new SaveFileDialog();
            form.Filter = string.Format("csv file |*{0}", ".csv");
            if (form.ShowDialog() == DialogResult.OK)
            {
                _Package.FileName = form.FileName;
                Save_Clicked(sender, e);
            }
        }

        protected virtual void Remove_PackageClicked(object sender, EventArgs e)
        {
            if (!this.Package.IsMandatory)
            {
                var question = Package.Name + " will be removed. Do you really want to reomove it?";
                if (_ShellService.MessageService.ShowYesNoQuestion(null, question))
                    this.Package.Owner.Remove(this.Package);
            }
            else
                _ShellService.MessageService.ShowWarning(null, "This package is mandatory. It can not be removed");
        }

        protected virtual void CoverageSetup_Clicked(object sender, EventArgs e)
        {
            var csc = (_AppManager as VHFAppManager).CoverageSetupController;
            csc.ViewModel.Coverage = _Package.Coverage;
            csc.ViewModel.ShowView();
        }

        private void FeatureSet_Clicked(object sender, EventArgs e)
        {
            PackageFeatureSet form = new PackageFeatureSet(this.Package, (_AppManager as VHFAppManager).MapAppManager.Map,null); 
            form.ShowDialog();
        }

        private void Optional_Clicked(object sender, EventArgs e)
        {
            if (Package!= null && Package.OptionalView != null)
                    Package.OptionalView.ShowView(_ShellService.MainForm);
        }

        protected override void ProjectItem_PropertyClicked(object sender, EventArgs e)
        {
            base.ProjectItem_PropertyClicked(sender, e);
            _ShellService.PropertyView.SelectedObject = _Package;
        }


    }
}
