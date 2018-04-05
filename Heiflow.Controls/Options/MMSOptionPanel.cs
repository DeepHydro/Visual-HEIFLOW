// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
