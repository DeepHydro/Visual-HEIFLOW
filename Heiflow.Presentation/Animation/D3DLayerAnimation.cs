// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Animation;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Heiflow.Models.Visualization;
namespace Heiflow.Presentation.Animation
{
    public class D3DLayerAnimation : DataCubeAnimation
    {
        private I3DLayerRender _render;
        public D3DLayerAnimation()
        {
        }

        public I3DLayerRender Render
        {
            get
            {
                return _render;
            }
            set
            {
                _render = value;
            }
        }

        public override void Initialize()
        {
            //_CurrentRender.DataSource = _DataSource;
            //_CurrentRender.VarIndex = VariableIndex;
           
        }

        public override void Cache()
        {
            _render.CacheColor();
        }

        protected override void Plot(int time_index)
        {
            var pck = _DataSource.DataOwner as IPackage;
            if (pck != null)
            {
                var vector = _DataSource.GetByTime(_DataSource.SelectedVariableIndex, time_index);
                _render.Render(vector as float []);
            }
        }
    }
}
