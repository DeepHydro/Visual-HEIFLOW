using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Heiflow.Visualization.Studio.Views
{
    /// <summary>
    /// SymbologyWindow.xaml 的交互逻辑
    /// </summary>
    /// 
   [Export(typeof(ISymbologyView))]
    public partial class SymbologyWindow : MetroWindow, ISymbologyView
    {
       private Lazy<SymbologyViewModel> viewModel;
        public SymbologyWindow()
        {
            InitializeComponent();
            this.Name = DockPanelNames.SymbologyPanel;
            this.Closing += Window_Closing;
            viewModel = new Lazy<SymbologyViewModel>(() => ViewHelper.GetViewModel<SymbologyViewModel>(this));
            symbology.ViewModel = viewModel;

        }
        public bool CloseAllowed
        {
            get;
            set;
        }
        public Renderable.Grid.IDX3DLayerRender SelectedRender
        {
            get
            {
                return symbology.SelectedRender;
            }
            set
            {
                symbology.SelectedRender = value;
            }
        }


        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseAllowed)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }


        public void ClearContents()
        {
             
        }
    }
}
