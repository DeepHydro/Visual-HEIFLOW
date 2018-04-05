// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
            if (pck != null)
            {
                var vector = _DataSource.GetByTime(_DataSource.SelectedVariableIndex, time_index);
                if (pck.Feature != null)
                {
                    var dt = pck.Feature.DataTable;
                    if (vector.Length == dt.Rows.Count)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dt.Rows[i][RegularGrid.ParaValueField] = vector.GetValue(i);
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
