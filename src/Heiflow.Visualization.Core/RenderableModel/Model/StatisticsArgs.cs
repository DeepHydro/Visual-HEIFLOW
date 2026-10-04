using Heiflow.Core.MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class StatisticsArgs : EventArgs
    {
        public StatisticsArgs(StatisticsInfo info)
        {
            StatisticsInfo = info;
        }

        public StatisticsInfo StatisticsInfo { get; private set; }
    }

    public delegate void StatisticsInfoHandler(object sender, StatisticsArgs args);
}
