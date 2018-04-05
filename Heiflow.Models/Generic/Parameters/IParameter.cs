// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Heiflow.Models.Generic
{
    public enum ParameterType { Dimension, Parameter, Control }
    public enum ParameterDimension { Single, Array}
    public enum Modules
    {
        basin,climateflow, check_nhru_params, cascade, obs, prms_time, soltab,
        climate_hru, ddsolrad, ccsolrad, potet_jh, potet_hamon, potet_pan, potet_hs, potet_pt, potet_pm, transp_frost,
        frost_date, transp_tindex, intcp, snowcomp, srunoff_smidx, srunoff_carea, soilzone, gwflow, subbasin,
        routing, strmflow, muskingum, strmflow_lake, lake_route, strmflow_in_out, water_balance, nhru_summar,
        prms_summary, basin_sum, map_results, write_climate_hru, prms_only, xyz_dist, gsflow_prms2mf, precip_prms, precip_laps_prms,
        precip_dist2_prms, hru_sum_prms, gsflow_setconv, temp_dist2_prms, gsflow_sum
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
        ParameterDimension ParameterDimension
        {
            get;
            set;
        }
        Array ArrayObject { get; }
        My3DMat<float> DataCubeObject { get; }
        IEnumerable<double> ToDouble();

        IEnumerable<float> ToFloat();

        IEnumerable<int> ToInt32();

        string[] ToStrings();

        Type GetVariableType();

        void AlterDimLength( int new_length);
    
        void SetValue(object vv, int index);

        void SetValues<T>(T[] vv);
        void Constant(object vv);
        void ResetToDefault();
        void UpdateFrom(IParameter new_para);
    }
}
