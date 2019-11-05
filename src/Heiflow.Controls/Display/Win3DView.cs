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

using Heiflow.Controls.WinForm.Controls;
using Heiflow.Presentation.Controls;
using ILNumerics;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Display
{
    [Export(typeof(ISurfacePlotView))]
    public partial class Win3DView : Form, ISurfacePlotView
    {

        public Win3DView()
        {
            InitializeComponent();
            this.FormClosing += Win3DView_FormClosing;
        }
        public View3DControl View3DControl
        {
            get
            {
                return view3DControl1;
            }
        }
        public string ChildName
        {
            get { return ChildWindowNames.Win3DView; }
        }

        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
                this.Show(pararent);
        }
       private void Win3DView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

       public void PlotSurface(ILArray<float> array)
       {
           view3DControl1.PlotSurface(array);
           this.Text = string.Format("3D View - {0}", array.Name);
       }

       public void ClearContent()
       {
           view3DControl1.ClearContent();
       }
       public void InitService()
       {

       }
    }
}
