using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Visualization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Heiflow.Models.GHM
{
    [Serializable]
    public class GHMVariable : DataPackage, INotifyPropertyChanged
    {
        private bool _LoadAllVars;
        public GHMVariable()
        {
            Start = DateTime.Now;
            TimeInteval = 86400;
            LoadAllVars = false;
        }

        [Browsable(false)]
        [XmlAttribute]
        public string RenderableModelLayer
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Source
        {
            get;
            set;
        }

        [XmlElement]
        public DateTime Start
        {
            get;
            set;
        }

        [XmlElement]
        public string LoadAll
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool LoadAllVars
        {
            get
            {
                _LoadAllVars = LoadAll.ToUpper() == "TRUE";
                return _LoadAllVars;
            }
            private set
            {
                _LoadAllVars = value;
            }
        }

        [XmlElement]
        public double TimeInteval
        {
            get;
            set;
        }

        [Browsable(false)]
        [XmlIgnore]
        public string FullPath
        {
            get
            {
                return Path.Combine(ModelService.WorkDirectory, Source);
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public I3DLayer RenderableModelObject
        {
            get;
            set;
        }

        public override bool Scan()
        {
            return false;
        }

        public override LoadingState Load(int var_index, DotSpatial.Data.ICancelProgressHandler progess)
        {
            return LoadingState.Warning;
        }

        public override LoadingState Load(DotSpatial.Data.ICancelProgressHandler progess)
        {
            return LoadingState.Warning;
        }

        public override void SaveAs(string filename, DotSpatial.Data.ICancelProgressHandler progress)
        {

        }
    }
}