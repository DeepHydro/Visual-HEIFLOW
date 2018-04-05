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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.PRMS
{
    [DataPackageCollectionItem("Surface Input")]
    public class PRMSInputDataPackage : DataPackageCollection
    {
        private ClimateDataPackage _climate;
        private MasterPackage _master;

        public PRMSInputDataPackage()
        {
            Name = "Surface Input";
            IsMandatory = true;
        }

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

            _climate = new ClimateDataPackage()
            {
                Owner = this.Owner,
                MasterPackage=_master
            };
            _climate.Initialize();
           
            AddChild(_climate);

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
            return _climate.New();
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

        public override void OnGridUpdated(IGrid sender)
        {
            this.Grid = sender;
            this.FeatureLayer = this.Grid.FeatureLayer;
            this.Feature = this.Grid.FeatureSet;
        }

        public override bool Save(IProgress progress)
        {
            return _climate.Save(progress);
        }

        public override void Clear()
        {
            _climate.Clear();
            if(_Initialized)
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            State = ModelObjectState.Standby;
            _Initialized = false;
        }

    }
}
