﻿//
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
using Heiflow.Core.Data;
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
        protected string[] _removedHeaderItemKeys;
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
        [XmlIgnore]
        [Browsable(false)]
        public string[] RemovedHeaderItemKeys
        {
            get
            {
                return _removedHeaderItemKeys;
            }
            set
            {
                _removedHeaderItemKeys = value;
            }
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
        public string WQDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input\\WQ");
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
        public string WRAInputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input\\WRA");
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
        [XmlIgnore]
        [Browsable(false)]
        public IGridFileFactory GridFileFactory
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public IDataCubeFileFactory DataCubeFileFactory
        {
            get;
            set;
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
        public virtual void Clear()
        {
            if (this.Model != null)
                this.Model.Clear();
        }
        public abstract void AttachFeatures();
        public abstract void CreateGridFeature();
        protected abstract void SaveBatchRunFile();

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

        protected virtual void CheckBatchRunFile()
        {
            var filename = Path.Combine(AbsolutePathToProjectFile, "run.bat");
            if (File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                var line = sr.ReadLine();
                var strs = TypeConverterEx.Split<string>(line);
                bool need_fix = false;
                if (strs.Length == 2)
                {
                    if (!File.Exists(strs[0]))
                    {
                        need_fix = true;
                    }

                }
                else
                {
                    need_fix = true;
                }
                sr.Close();
                if (need_fix)
                    SaveBatchRunFile();
            }
            else
            {
                SaveBatchRunFile();
            }
        }

        protected virtual void SaveIHMProjectFile()
        {
            var filename = Path.Combine(AbsolutePathToProjectFile, this.Name+ ".ihmx");
            if (!File.Exists(filename))
            {
                StreamWriter sw = new StreamWriter(filename);
                string line = "<?xml version=\"1.0\"?>";
                sw.WriteLine(line);
                line = "<Heiflow3DProject xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
                sw.WriteLine(line);
  //                <Name>HEIFLOW</Name>
  //<Description>HEIFLOW model version 1.0.0</Description>
  //<RelativeMapFileName>ghm.dspx</RelativeMapFileName>
  //<RelativeControlFileName>ghm.control</RelativeControlFileName>
  //<RelativeModelWorkDirectory>.\</RelativeModelWorkDirectory>
  //<GridFeatureFilePath>GeoSpatial\Grid.shp</GridFeatureFilePath>
  //<CentroidFeatureFilePath>GeoSpatial\Centroid.shp</CentroidFeatureFilePath>
  //<RelativeModelGeoPrjFile>GeoSpatial\Centroid.prj</RelativeModelGeoPrjFile> 
                line = string.Format("\t<Name>{0}</Name>", this.Name);
                sw.WriteLine(line);
                line = string.Format("\t<Description>{0}</Description>", "HydroEarth");
                sw.WriteLine(line);
                line = string.Format("\t<RelativeControlFileName>{0}.control</RelativeControlFileName>", this.Name);
                sw.WriteLine(line);
                line = "\t<RelativeModelWorkDirectory>.\\</RelativeModelWorkDirectory>";
                sw.WriteLine(line);
                line = "\t<RelativeModelGeoPrjFile>GeoSpatial\\Centroid.prj</RelativeModelGeoPrjFile> ";
                sw.WriteLine(line);

                sw.WriteLine("<ODMProvider>");
                sw.WriteLine("\t<Name>IHM ODM</Name>");
                sw.WriteLine("\t<Opacity>255</Opacity>");
                sw.WriteLine("\t<DistanceAboveSurface>10</DistanceAboveSurface>");
                sw.WriteLine("\t<MinimumDisplayAltitude>0</MinimumDisplayAltitude>");
                sw.WriteLine("\t<MaximumDisplayAltitude>1000000</MaximumDisplayAltitude>");
                sw.WriteLine("\t<RenderPriority>LinePaths</RenderPriority>");
                sw.WriteLine("\t<IsOn>false</IsOn>");
                sw.WriteLine("\t<IsSelectable>false</IsSelectable>");
                sw.WriteLine("\t<RelativeFileName>.\\database\\odm.accdb</RelativeFileName>");
                sw.WriteLine("\t<ShowAtStartup>false</ShowAtStartup>");
                sw.WriteLine("</ODMProvider>");

                sw.WriteLine("<ShapeFeatureProvider>");
                sw.WriteLine("\t<Name>Grid Layer</Name>");
                sw.WriteLine("\t<Opacity>255</Opacity>");
                sw.WriteLine("\t<DistanceAboveSurface>10</DistanceAboveSurface>");
                sw.WriteLine("\t<MinimumDisplayAltitude>0</MinimumDisplayAltitude>");
                sw.WriteLine("\t<MaximumDisplayAltitude>1000000</MaximumDisplayAltitude>");
                sw.WriteLine("\t<RenderPriority>LinePaths</RenderPriority>");
                sw.WriteLine("\t<IsOn>true</IsOn>");
                sw.WriteLine("\t<IsSelectable>false</IsSelectable>");
                sw.WriteLine("\t<RelativeFileName>.\\Geospatial\\grid.shp</RelativeFileName>");
                sw.WriteLine("\t<ShowAtStartup>true</ShowAtStartup>");
                sw.WriteLine("\t<CoordinateSystem>WGS84</CoordinateSystem>");
                sw.WriteLine("\t<LabelField>ID</LabelField>");
                sw.WriteLine("\t<FeatureColor>Blue</FeatureColor>");
                sw.WriteLine("</ShapeFeatureProvider>");

                sw.Write("</Heiflow3DProject>");
                sw.Close();
            }
        }
    }
}
