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
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Applications.Spatial
{
    public class MapFunctionActiveIdentify : MapFunction
    {
        private bool _standBy;
        private AppManager _AppManager;
        private VHFAppManager _vhf;

        public MapFunctionActiveIdentify(AppManager app, VHFAppManager vhf)
            : base(app.Map)
        {
            _AppManager = app;
            _vhf = vhf;
            Configure();
        }

        public IFeatureSet GridFeature
        {
            get;
            set;
        }

        private void Configure()
        {
            YieldStyle = YieldStyles.LeftButton | YieldStyles.Scroll;        
            Control map = Map as Control;
            if (map != null) map.MouseLeave += map_MouseLeave;
            this.Name = "MapFunctionActiveIdentify";
        }

        private void map_MouseLeave(object sender, EventArgs e)
        {
            Map.Invalidate();
        }

        protected override void OnMouseUp(GeoMouseArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            Rectangle rtol = new Rectangle(e.X - 8, e.Y - 8, 16, 16);
            Rectangle rstr = new Rectangle(e.X - 1, e.Y - 1, 2, 2);
            Extent tolerant = e.Map.PixelToProj(rtol);
            Extent strict = e.Map.PixelToProj(rstr);
            var _chart = _vhf.ShellService.WinChart;
            var data_service = _vhf.ProjectController.ActiveDataService;

            if (GridFeature != null && _chart != null && data_service.Source != null)
            {
                var grid = _vhf.ProjectController.Project.Model.Grid;
                var selected = GridFeature.Select(strict, out tolerant);
                if (selected.Count > 0)
                {
                    var fea = selected[0];
                    var hru = int.Parse(fea.DataRow["HRU_ID"].ToString());
                    int ntime = data_service.Source.Size[1];
                    int ncell = data_service.Source.Size[2];
                    int nhru = GridFeature.NumRows();
                    float[] yy = new float[ntime];
                    var sery_title = "";
                    if (ncell >= nhru * (grid.SelectedLayerToShown+1))
                    {
                        for (int i = 0; i < ntime; i++)
                        {
                            yy[i] = data_service.Source[data_service.Source.SelectedVariableIndex, i, (hru - 1) + grid.SelectedLayerToShown * nhru];
                        }
                        sery_title = string.Format("CELL {0} in Layer {1}", hru, grid.SelectedLayerToShown + 1);
                    }
                    else
                    {
                        for (int i = 0; i < ntime; i++)
                        {
                            yy[i] = data_service.Source[data_service.Source.SelectedVariableIndex, i, hru - 1];
                        }
                        sery_title = string.Format("CELL {0} ", hru);
                    }
                    _chart.Plot<float>(data_service.Source.DateTimes, yy, sery_title, SeriesChartType.FastLine);
                }
            }
        }

        /// <summary>
        /// Forces this function to begin collecting points for building a new shape.
        /// </summary>
        protected override void OnActivate()
        {
            if (_standBy == false)
            {
            }
            _vhf.ShellService.WinChart.ShowView(_vhf.ShellService.MainForm);

            _standBy = false;
            base.OnActivate();
        }

        /// <summary>
        /// Allows for new behavior during deactivation.
        /// </summary>
        protected override void OnDeactivate()
        {
            if (_standBy)
            {
                return;
            }
            // Don't completely deactivate, but rather go into standby mode
            // where we draw only the content that we have actually locked in.
            _standBy = true;
            _vhf.ShellService.WinChart.CloseView();
            Map.Invalidate();
        }

    }
}
