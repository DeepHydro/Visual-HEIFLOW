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

using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Heiflow.Models.Generic
{
    public enum ParameterType { Dimension, Parameter, Control }
    public enum Modules
    {
        basin, climateflow, check_nhru_params, cascade, obs, prms_time, soltab,
        climate_hru, ddsolrad, ccsolrad, potet_jh, potet_hamon, potet_pan, potet_hs, potet_pt, potet_pm, transp_frost,
        frost_date, transp_tindex, intcp, snowcomp, srunoff_smidx, srunoff_carea, srunoff_module, soilzone, gwflow, subbasin,
        routing, strmflow, muskingum, strmflow_lake, lake_route, strmflow_in_out, water_balance, nhru_summar,
        prms_summary, basin_sum, map_results, write_climate_hru, prms_only, xyz_dist, sw2gw, precip_prms, precip_laps_prms,
        precip_dist2_prms, hru_sum_prms, swgw_setconv, temp_dist2_prms, gsflow_sum, ecology_general, ecopopulus
    }

    public interface IParameter
    {
        string Name
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        IPackage Owner
        {
            get;
            set;
        }
        /// <summary>
        ///0 is for short; 1 is for integer;2 is for real; 3 is for double; 4 is for character string; 5 is for object; 6 is for bool
        /// </summary>
        /// 
        int ValueType
        {
            get;
            set;
        }

        int ValueCount
        {
            get;
        }

        ParameterType VariableType
        {
            get;
            set;
        }

        int Dimension
        {
            get;
            set;
        }

        string[] DimensionNames
        {
            get;
            set;
        }

        string DimensionCat
        {
            get;
        }
        //int[] DimensionLengh
        //{
        //    get;
        //}
        object Tag
        {
            get;
            set;
        }

        Modules ModuleName
        {
            get;
            set;
        }

        double DefaultValue
        {
            get;
            set;
        }

        float Maximum
        {
            get;
            set;
        }

        float Minimum
        {
            get;
            set;
        }

        string Units
        {
            get;
            set;
        }
        /// <summary>
        /// parameter that has fixed value
        /// </summary>
        bool IsFixed
        {
            get;set;
        }
        DataCube2DLayout<float> FloatDataCube
        {
            get;
        }
        string[] ToStringVector();
        float[] GetColumnVector(int col_index);
        Type GetVariableType();
        object GetValue(int var_index, int time_index, int cell_index);
        void SetValue(int var_index, int time_index, int cell_index, object new_value);
        void AlterDimLength(string dim_name, int new_length);
        void Constant(object vv);
        void SetToDefault();
        void UpdateFrom(IParameter new_para);
        void UpdateFromFloatDataCube();
    }
}
