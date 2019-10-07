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
using Heiflow.Presentation.Services;

namespace Heiflow.Controls.Options
{
    [Export(typeof(IOptionControl))]
    public partial class MMSOptionPanel : UserControl, IOptionControl
    {
        private MMSPackage _DefaultMMSPackage;
        private MMSPackage _ImportedMMSPackage;
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
        public string SelectedVersion
        {
            get;
            set;
        }
        public string SelectedExportSource
        {
            get;
            set;
        }
        private void MMSOptionPanel_Load(object sender, EventArgs e)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            _Configfile = Path.Combine(VHFAppManager.Instance.ConfigManager.ConfigPath, "mms_config_" + SelectedVersion + ".xml");
            LoadDefault(_Configfile);
        }
        public void LoadDefault(string filename)
        {
            if (File.Exists(filename))
            {
                _DefaultMMSPackage = new MMSPackage("PRMS");
                _DefaultMMSPackage.Deserialize(filename);
                foreach (var para in _DefaultMMSPackage.Parameters.Values)
                {
                    para.Dimension = para.DimensionNames.Length;
                }
                propertyGrid1.SelectedObject = _DefaultMMSPackage.Parameters;
            }
        }

        public void ImportFromXml(string filename)
        {
            _ImportedMMSPackage = new MMSPackage();
            _ImportedMMSPackage.Deserialize(filename);
            foreach (var para in _ImportedMMSPackage.Parameters.Values)
            {
                para.Dimension = para.DimensionNames.Length;
            }
            propertyGrid1.SelectedObject = _ImportedMMSPackage.Parameters;
        }

        public void ImportFromParameter(string filename)
        {
            _ImportedMMSPackage = new MMSPackage();
            _ImportedMMSPackage.FileName = filename;
            _ImportedMMSPackage.Parameters.Clear();
            _ImportedMMSPackage.Load(filename, null);
            propertyGrid1.SelectedObject = _ImportedMMSPackage.Parameters;
        }
        public void Save()
        {
            if (File.Exists(_Configfile))
            {
                _DefaultMMSPackage.Serialize(_Configfile);
            }
        }
        public void SaveAsCsv(string filename)
        {
            var pck = _DefaultMMSPackage;
            if (SelectedExportSource != "Default")
                pck = _ImportedMMSPackage;

            if (pck != null)
            {
                StreamWriter sw = new StreamWriter(filename);
                string line = "Parameter Name,Module Name,Default Value,Maximum,Minimum,Description,Dimension Name,Dimension";
                sw.WriteLine(line);
                foreach (var para in pck.Parameters.Values)
                {
                    line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", para.Name, para.ModuleName, para.DefaultValue, para.Maximum, para.Minimum, para.Description, string.Join(" ", para.DimensionNames), para.Dimension);
                    sw.WriteLine(line);
                }
                sw.Close();
            }
        }

        public void SaveAsXml(string filename)
        {
            var pck = _DefaultMMSPackage;
            if (SelectedExportSource != "Default")
                pck = _ImportedMMSPackage;
            if (pck != null)
            {
                pck.Serialize(filename);
            }
        }

        public void SaveAsParameter(string filename)
        {
            var pck = _DefaultMMSPackage;
            if (SelectedExportSource != "Default")
                pck = _ImportedMMSPackage;
            if (pck != null)
            {
                string[] dims = new string[] { "nhru", "nhrucell", "ngwcell", "ncascade", "nreach", "nsegment", "nlake" };
                foreach (var dim in dims)
                {
                    _ImportedMMSPackage.AlterLength(dim, 1);
                }
                pck.SaveAs(filename, null);
            }
        }


        public void Compare()
        {
            if(_ImportedMMSPackage != null)
            {
                foreach(var impara in _ImportedMMSPackage.Parameters.Values)
                {
                    var para = _DefaultMMSPackage.Select(impara.Name);
                     if(para != null)
                     {
                         impara.DefaultValue = para.DefaultValue;
                         impara.Description = para.Description;
                         impara.Maximum = para.Maximum;
                         impara.Minimum = para.Minimum;
                         impara.ModuleName = para.ModuleName;
                         impara.Units = para.Units;
                     }
                    else
                     {
                         impara.ModuleName = Models.Generic.Modules.ecology_general;
                     }
                }
            }
        }
    }
}