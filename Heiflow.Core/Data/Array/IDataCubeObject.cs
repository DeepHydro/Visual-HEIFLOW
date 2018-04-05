// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using ILNumerics;
using System;
using System.Data;

namespace Heiflow.Core.Data
{
    public interface IDataCubeObject: IDataTableConvertable
    {
        event EventHandler DataCubeValueChanged;
        Array ArrayObject { get; }
        string Name { get; set; }
        object DataOwner { get; set; }
        string OwnerName { get; set; }
        /// <summary>
        /// index for the zero dimension. Selecting all variables by setting to -1
        /// </summary>
        int SelectedVariableIndex { get; set; }
        /// <summary>
        /// index for the first dimension. Selecting all times by setting to -1
        /// </summary>
        int SelectedTimeIndex { get; set; }
        /// <summary>
        /// index for the second dimension. Selecting all space cells by setting to -1
        /// </summary>
        int SelectedSpaceIndex { get; set; }
        DateTime[] DateTimes { get; set; }
        int[] Size { get; }
        TimeVarientFlag[,] Flags { get; set; }
        string[] Variables { get; set; }
        float[,] Constants{ get; set; }
        float[,] Multipliers { get; set; }
        bool TimeBrowsable { get; set; }
        DataCubeType DataCubeType { get; }
        bool AllowTableEdit { get; }
        IGridTopology Topology { get; set; }
        Array GetByTime(int var_index, int time_index);
        Array GetBySpace(int var_index, int space_index);
        int GetSpaceDimLength(int var_index, int time_index);
        Array GetSerialArrayByTime(int p1, int p2);
        Array GetRegularlArrayByTime(int p1, int p2);
        void FromRegularArray(int p1, int p2, Array array);
        void FromSerialArray(int p1, int p2, Array array);
        void AllocateSpaceDim(int var_index, int time_index, int length);
        bool IsAllocated(int var_index);
        ILBaseArray ToILBaseArray(int var_index, int time_index);
        string SizeString();
    }

    public enum DataCubeType { General, Varient,Vector}
}
