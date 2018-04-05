// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Name
        {
            get;
            set;
        }
        [XmlIgnore]
        public string NameToShown
        {
            get;
            protected set;
        }
        [XmlElement]
        public string Description
        {
            get;
            set;
        }

        [XmlIgnore]
        public System.Drawing.Image Icon
        {
            get;
            set;
        }
        [XmlIgnore]
        public System.Drawing.Image LargeIcon
        {
            get;
            set;
        }
        [XmlIgnore]
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
        public IMap Map
        {
            get;
            set;
        }
        [XmlIgnore]
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// Full project file path without project file name
        /// </summary>
          [XmlIgnore]
        public string AbsolutePathToProjectFile
        {
            get;
            set;
        }

        /// <summary>
        /// Full map file name
        /// </summary>
        [XmlElement]
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
        public string FullProjectFileName
        {
            get;
            set;
        }
        /// <summary>
        /// Root directory for the project
        /// </summary>
        [XmlElement]
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
        public string ModelExeFileName
        {
            get;
            set;
        }

        /// <summary>
        /// Relative file name of the grid shapefile
        /// </summary>
        /// 
        [XmlElement]
        public string GridFeatureFilePath
        {
            get;
            set;
        }
        /// <summary>
        /// Relative file name of the grid centroid shapefile
        /// </summary>
        [XmlElement]
        public string CentroidFeatureFilePath
        {
            get;
            set;
        }

        [XmlArrayItem]
        public ObservableCollection<FeatureCoverage> FeatureCoverages
        {
            get;
            set;
        }

        [XmlArrayItem]
        public ObservableCollection<RasterCoverage> RasterLayerCoverages
        {
            get;
            set;
        }
   
        //[XmlArrayItem]
        //public ObservableCollection<FeatureElement> PackageFeatures
        //{
        //    get;
        //    set;
        //}

        [XmlElement]
        public ODMSource ODMSource
        {
            get;
            set;
        }

        [XmlIgnore]
        public string GeoSpatialDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Geospatial");
            }
        }

        [XmlIgnore]
        public string ProcessingDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Processing");
            }
        }

        [XmlIgnore]
        public string InputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input");
            }
        }
        [XmlIgnore]
        public string MFInputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input\\Modflow");
            }
        }
        [XmlIgnore]
        public string PRMSInputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Input\\PRMS");
            }
        }
        [XmlIgnore]
        public string OutputDirectory
        {
            get
            {
                return Path.Combine(AbsolutePathToProjectFile, "Output");
            }
        }

        [XmlIgnore]
        public MapPolygonLayer GridLayer
        {
            get
            {
                return _GridLayer;
            }
        }

        [XmlIgnore]
        public MapPointLayer CentroidLayer
        {
            get
            {
                return _CentroidLayer;
            }
        }
        [XmlIgnore]
        public List<ITimeService> TimeServices
        {
            get
            {
                return _TimeServices;
            }
        }
        [XmlIgnore]
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
        protected void OnModelChanged()
        {
            if (ModelChanged != null)
                ModelChanged(this, EventArgs.Empty);
        }

     
        public virtual void Initialize()
        {
            _IsDirty = false;
        }

        public abstract bool New(IProgress progress,bool ImportFromExistingModel);
        public abstract void Clear();
        public abstract void AttachFeatures();
        public abstract void CreateGridFeature();
    }
}
