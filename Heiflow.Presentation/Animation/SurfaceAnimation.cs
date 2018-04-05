// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heiflow.Models.Subsurface;
using ILNumerics;
using Heiflow.Models.Generic;
using Heiflow.Core.Animation;
using Heiflow.Core.Data;
using Heiflow.Presentation.Controls;
using Heiflow.Models.Generic.Project;

namespace Heiflow.Presentation.Animation
{
    public class SurfaceAnimation : DataCubeAnimation
    {
        private ISurfacePlotView _Plot;

        public SurfaceAnimation( )
        {
            _Name = "3D Animation";
        }


        public ISurfacePlotView SurfacePlot
        {
            get
            {
                return _Plot;
            }
            set
            {
                _Plot = value;
            }
        }

        protected override void Plot(int time_index)
        {
            var mat = _DataSource.ToILBaseArray(_DataSource.SelectedVariableIndex, time_index) as ILArray<float>;
            if (mat != null)
            {
                mat.Name = string.Format("{0}[{1}]", _DataSource.Name, _DataSource.Variables[_DataSource.SelectedVariableIndex]);
                _Plot.PlotSurface(mat);
                _Current++;
                if (Current >= Maximum)
                    Pause();
            }
        }
    }
}
