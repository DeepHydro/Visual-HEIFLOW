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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using Heiflow.Models.Surface;
using Heiflow.Presentation.Controls.Project;
using System.IO;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Presentation.Controls;
using Heiflow.Applications;

namespace Heiflow.Controls.Options
{
    [Export(typeof(IOptionControl))]
    public partial class MMSOptionPanel : UserControl, IOptionControl
    {
        private MMSPackage _MMSPackage;
        private string _Configfile;

        public MMSOptionPanel()
        {
            InitializeComponent();
            this.Load += MMSOptionPanel_Load;        
        }

        public UserControl OptionControl
        {
            get
            {
                return this;
            }
        }

        public string Category
        {
            get
            {
                return "Models";
            }
        }

        public string OptionName
        {
            get
            {
                return "PRMS";
            }
        }

        void MMSOptionPanel_Load(object sender, EventArgs e)
        {
            _Configfile = Path.Combine(VHFAppManager.Instance.ConfigManager.ConfigPath, "mms.config.xml");
            if (File.Exists(_Configfile))
            {
                _MMSPackage = new MMSPackage("PRMS");
                _MMSPackage.Deserialize(_Configfile);
                propertyGrid1.SelectedObject = _MMSPackage.Parameters;
            }
            else
            {

            }
        }

        public void Save()
        {
            if (File.Exists(_Configfile))
            {
                _MMSPackage.Serialize(_Configfile);
            }
        }

    }
}
