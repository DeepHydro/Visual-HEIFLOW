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

using DotSpatial.Controls;
using DotSpatial.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Project
{
    [Serializable]
    public abstract class BaseProject : IProject
    {
        public event EventHandler ModelChanged;
        protected IBasicModel _Model;
        protected string _RelativeMapFileName;
        protected string _RelativeControlFileName;
        protected string _RelativeModelWorkDirectory;
        protected MapPolygonLayer _GridLayer;
        protected MapPointLayer _CentroidLayer;
        protected List<ITimeService> _TimeServices = new List<ITimeService>();
        protected bool _IsDirty;
        protected string _SelectedVersion;
        public BaseProject()
        {
            this.Name = "Project";
            NameToShown = "Unknown";
            this.Icon = Resources.RasterImageAnalysisPanSharpen16;
            this.LargeIcon = Resources.RasterImageAnalysisPanSharpen32;
            Description = "This is base project";
            GridFeatureFilePath = "";
            CentroidFeatureFilePath = "";
            _IsDirty = false;
            //this.PackageFeatures = new ObservableCollection<Generic.Packages.FeatureElement>();
            this.RasterLayerCoverages = new ObservableCollection<Generic.Parameters.RasterCoverage>();
            this.FeatureCoverages = new ObservableCollection<Generic.Parameters.FeatureCoverage>();
        }


        [XmlElement]
        [Browsable(false)]
        public string Name
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public string NameToShown
        {
            get;
            protected set;
        }
        [XmlElement]
        [Category("General")]
        public string Description
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public System.Drawing.Image Icon
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public System.Drawing.Image LargeIcon
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public IBasicModel Model
        {
            get
            {
                return _Model;
            }
            set
            {
                _Model = value;
                _Model.Project = this;
                OnModelChanged();
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public IMap Map
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// Full project file path without project file name
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string AbsolutePathToProjectFile
        {
            get;
            set;
        }

        /// <summary>
        /// Full map file name
        /// </summary>
        [XmlElement]
        [Browsable(false)]
        public string RelativeMapFileName
        {
            get
            {
                return _RelativeMapFileName;
            }
            set
            {
                _RelativeMapFileName = value;
            }
        }
        /// <summary>
        /// Full map file name
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string FullMapFileName
        {
            get
            {
                if (DirectoryHelper.IsRelativePath(_RelativeMapFileName))
                    return Path.GetFullPath(Path.Combine(AbsolutePathToProjectFile, _RelativeMapFileName));
                else
                    return _RelativeMapFileName;
            }
        }
        /// <summary>
        /// Relative control file name.
        /// </summary>
        [XmlElement]
        [Browsable(false)]
        public string RelativeControlFileName
        {
            get
            {
                return _RelativeControlFileName;
            }
            set
            {
                _RelativeControlFileName = value;
            }
        }

        /// <summary>
        /// Full control file name. This includes path, file name and extension
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string FullControlFileName
        {
            get
            {
                if (DirectoryHelper.IsRelativePath(_RelativeControlFileName))
                    return Path.GetFullPath(Path.Combine(AbsolutePathToProjectFile, _RelativeControlFileName));
                else
                    return _RelativeControlFileName;
            }
        }
        /// <summary>
        /// Full project File name
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string FullProjectFileName
        {
            get;
            set;
        }
        /// <summary>
        /// Root directory for the project
        /// </summary>
        [XmlElement]
        [Browsable(false)]
        public string RelativeModelWorkDirectory
        {
            get
            {
                return _RelativeModelWorkDirectory;
            }
            set
            {
                _RelativeModelWorkDirectory = value;
            }
        }
        /// <summary>
        /// Root directory for the project
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string FullModelWorkDirectory
        {
            get
            {
                if (DirectoryHelper.IsRelativePath(_RelativeModelWorkDirectory))
                    return Path.GetFullPath(Path.Combine(AbsolutePathToProjectFile, _RelativeModelWorkDirectory));
                else
                    return _RelativeModelWorkDirectory;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string ModelExeFileName
        {
            get
            {
                string model = string.Format("Models\\{0}_{1}.exe", Token, SelectedVersion);
                return Path.Combine(Application.StartupPath, model);
            }
        }

        /// <summary>
        /// Relative file name of the grid shapefile
        /// </summary>
        /// 
        [XmlElement]
        [Browsable(false)]
        public string GridFeatureFilePath
        {
            get;
            set;
        }
        /// <summary>
        /// Relative file name of the grid centroid shapefile
        /// </summary>
        [XmlElement]
        [Browsable(false)]
        public string CentroidFeatureFilePath
        {
            get;
            set;
        }

        [XmlArrayItem]
        [Browsable(false)]
        public ObservableCollection<FeatureCoverage> FeatureCoverages
        {
            get;
            set;
        }

        [XmlArrayItem]
        [Browsable(false)]
        public ObservableCollection<RasterCoverage> RasterLayerCoverages
        {
            get;
            set;
        }

        [XmlElement]
        [Browsable(false)]
        public ODMSource ODMSource
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public string GeoSpatialDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Geospatial");
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string ProcessingDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Processing");
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string InputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input");
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string MFInputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input\\Modflow");
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string PRMSInputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input\\PRMS");
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string ExtensionInputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input\\Extension");
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string DatabaseDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Database");
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string OutputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Output");
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public MapPolygonLayer GridLayer
        {
            get
            {
                return _GridLayer;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public MapPointLayer CentroidLayer
        {
            get
            {
                return _CentroidLayer;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public List<ITimeService> TimeServices
        {
            get
            {
                return _TimeServices;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }
            set
            {
                _IsDirty = value;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public string[] SupportedVersions
        {
            get;
            protected set;
        }
        [XmlElement]
        [Browsable(false)]
        public string SelectedVersion
        {
            get
            {
                return _SelectedVersion;
            }
            set
            {
                _SelectedVersion = value;
            }
        }

        protected void OnModelChanged()
        {
            if (ModelChanged != null)
                ModelChanged(this, EventArgs.Empty);
        }

        public virtual void Initialize()
        {
            _IsDirty = false;
        }

        public abstract bool New(ICancelProgressHandler progress, bool ImportFromExistingModel);
        public abstract void Clear();
        public abstract void AttachFeatures();
        public abstract void CreateGridFeature();

        public bool ContainsCoverage(string legendtext)
        {
            var buf = from fea in FeatureCoverages where fea.LegendText == legendtext select fea;
            if (buf.Any())
            {
                return true;
            }
            else
            {
                var rasc = from ras in this.RasterLayerCoverages where ras.LegendText == legendtext select ras;
                return rasc.Any();
            }
        }
    }
}
