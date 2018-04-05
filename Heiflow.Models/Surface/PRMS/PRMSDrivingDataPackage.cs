// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.IO;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.PRMS
{
    [DataPackageCollectionItem("Surface Driving")]
    public class PRMSDrivingDataPackage : DataPackageCollection
    {
        private PRMSDataPackage _data;
        private MasterPackage _master;

        public PRMSDrivingDataPackage()
        {
            Name = "Surface Driving";
            IsMandatory = true;
        }
        [Browsable(false)]
        public MasterPackage MasterPackage
        {
            get
            {
                return _master;
            }
            set
            {
                _master = value;
            }
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;

            _data = new PRMSDataPackage()
            {
                Owner = this.Owner
            };
            _data.FileName = MasterPackage.DataFile;
            _data.Initialize();
            AddChild(_data);

            State = ModelObjectState.Ready;
            _Initialized = true;
        }

        public override bool Scan()
        {
            return true;
        }

        public override bool Load(int var_index)
        {
            return true;
        }

        public override bool New()
        {
            return _data.New();
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

        public override bool Save(IProgress progress)
        {
            return _data.Save(progress);
        }

        public override void Clear()
        {
            _data.Clear();
            if(_Initialized)
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            State = ModelObjectState.Standby;
            _Initialized = false;
        }

    }
}
