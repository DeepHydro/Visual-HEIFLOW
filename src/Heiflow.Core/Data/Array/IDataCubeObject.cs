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
        DataCubeLayout Layout { get; set; }
        bool AllowTableEdit { get; }
        IGridTopology Topology { get; set; }
        Array GetByTime(int var_index, int time_index);
        Array GetBySpace(int var_index, int space_index);
        int GetSpaceDimLength(int var_index, int time_index);
        Array GetSerialArrayByTime(int p1, int p2);
        Array GetRegularlArrayByTime(int p1, int p2);
        void FromRegularArray(int p1, int p2, Array array);
        void FromSerialArray(int p1, int p2, Array array);
        void AllocateVariable(int var_index,int ntime, int ncell);
        bool IsAllocated(int var_index);
        ILBaseArray ToILBaseArray(int var_index, int time_index);
        string SizeString();
    }

    public enum DataCubeType { General, Varient,Vector}
}
