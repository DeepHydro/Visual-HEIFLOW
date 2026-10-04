using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// Add3DModelWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IAdd3DModelView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Add3DModelWindow:IAdd3DModelView
    {
        private readonly Lazy<Add3DModelViewModel> viewModel;
        private string meshFilePath;
        private SceneWindow _SceneWindow;
        private RenderableObject _ModelFeature;
        private ModelFeatureProvider _ModelFeatureProvider;

        public Add3DModelWindow(SceneWindow sceneWindow)
        {
            InitializeComponent();
            _SceneWindow = sceneWindow;
            viewModel = new Lazy<Add3DModelViewModel>(() => ViewHelper.GetViewModel<Add3DModelViewModel>(this));
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "3D Model file|*.x";
            if (ofd.ShowDialog().Value ==  true )
            {
                meshFilePath = ofd.FileName;
                tbFile.Text = meshFilePath;
                _ModelFeatureProvider = new ModelFeatureProvider();
                 _ModelFeature=   _ModelFeatureProvider.Load(meshFilePath, _SceneWindow.CurrentWorld);    
                propertyGrid.SelectedObject = _ModelFeature;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_ModelFeature != null)
            {
                _SceneWindow.CurrentWorld.DataLayerList.ChildObjects.Add(_ModelFeature);
            //    VisAppManager.Default.MainWindow.RefreshLayerManager();
                var prj = viewModel.Value.ProjectService.Project as Heiflow3DProject;

                if (prj != null)
                {
                    if (prj.ModelFeatureProvider == null)
                        prj.ModelFeatureProvider = new List<Renderable.ModelFeatureProvider>();
                    if (!prj.ModelFeatureProvider.Contains(_ModelFeatureProvider))
                    {
                        prj.ModelFeatureProvider.Add(_ModelFeatureProvider);
                    }
                    _ModelFeatureProvider.RelativeFileName = _ModelFeatureProvider.GetRelativeFileName(prj.AbsolutePathToProjectFile, _ModelFeatureProvider.FullFileName);
                }
                this.Close();
            }
        }
    }
}
