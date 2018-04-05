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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using DotSpatial.Controls;
using Heiflow.Models.UI;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Presentation.Services
{
    public interface IShellService : INotifyPropertyChanged
    {
        object ShellView { get; set; }
        List<IChildView> ChildViews { get; }
        IAnimationView AnimationPlayer { get; set; }
        IDataGridView DataGridView { get; set; }
        IModelSymbologyControl SymbologyControl { get; set; }
        IProgressView ProgressWindow { get; set; }
        IProjectExplorer ProjectExplorer { get; set; }
        IPropertyView PropertyView { get; set; } 
        ISurfacePlotView SurfacePlot { get; set; }     
        INewProject NewProjectWindow { get; set; }
        IModelToolManager PackageToolManager { get; set; }
        IWin32Window MainForm { get; set; }
        IWinChartView WinChart { get; set; }
        IFigureService FigureService { get; set; }
        IMessageService MessageService { get; set; }
        AppManager MapAppManager { get; set; }
        IDataCubeEditor TV3DMatEditor { get; set; }
        bool ShowAnimationMonitor { get; set; }
        void SelectPanel(string key);
        void ClearContents();
        IChildView SelectChild(string child_name);
        void ShowChildWindow(string child_name);
        void AddChild(IChildView child);
        void AddNativeChildren();
        void InitChildViewService();
    }
}
