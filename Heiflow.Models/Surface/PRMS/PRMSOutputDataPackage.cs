// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Models.Integration;
using Heiflow.Core.IO;
using Heiflow.Core.Data;
using System.ComponentModel;
using System.IO;

namespace Heiflow.Models.Surface.PRMS
{
    [DataPackageCollectionItem("Surface Output")]
    public class PRMSOutputDataPackage : DataPackageCollection
    {
        private MasterPackage _master;
        private AnimationOutPackage _animation;

        public PRMSOutputDataPackage()
        {
            Name = "Surface Output";
            _animation = new AnimationOutPackage();
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
            _animation.FileName = _master.AniOutFileName;
            _animation.MasterPackage = _master;
            _animation.Owner = this.Owner;
            _animation.Initialize();
            AddChild(_animation);

            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            _Initialized = true;
        }

        public override bool New()
        {

            return true;
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
        public override void Clear()
        {
            _animation.Clear();
            if (_Initialized)
            {
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            }
            State = ModelObjectState.Standby;
            _Initialized = false;
        }

    }
}