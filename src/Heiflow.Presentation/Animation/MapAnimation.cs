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
using DotSpatial.Symbology;
using Heiflow.Core.Animation;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics;
using Heiflow.Models.Generic.Project;

namespace Heiflow.Presentation.Animation
{
    public class MapAnimation : DataCubeAnimation
    {
        public MapAnimation()
        {
            _Name = "Map Animation";
        }


        protected override void Plot(int time_index )
        {
            var pck = _DataSource.DataOwner as IPackage;
            if (pck != null && time_index > -1)
            {
                var vector = _DataSource.GetVectorAsArray(_DataSource.SelectedVariableIndex, time_index.ToString(), ":");
                if (pck.Feature != null && vector != null)
                {
                    var dt = pck.Feature.DataTable;
                    if (vector.Length == dt.Rows.Count)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dt.Rows[i][RegularGrid.ParaValueField] = vector.GetValue(i);
                        }
                    }
                    else
                    {
                        if(vector.Length == dt.Rows.Count* _DataSource.Layers)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                dt.Rows[i][RegularGrid.ParaValueField] = vector.GetValue(i + dt.Rows.Count * _DataSource.SelectedLayerToShown);
                            }
                        }
                    }
                    ApplyScheme(pck.FeatureLayer, RegularGrid.ParaValueField);
                }
            }
        }

        protected void ApplyScheme(IMapFeatureLayer gridLayer, string fieldName)
        {
            IFeatureScheme newScheme = gridLayer.Symbology;
            newScheme.EditorSettings.NumBreaks = 5;
            newScheme.EditorSettings.UseGradient = true;
            newScheme.EditorSettings.ClassificationType = ClassificationType.Quantities;
            newScheme.EditorSettings.FieldName = fieldName;
            newScheme.CreateCategories(gridLayer.DataSet.DataTable);
            newScheme.ResumeEvents();
            gridLayer.ApplyScheme(newScheme);
        }

    }
}
