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

using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Core.MyMath;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.TempoSpatialAnalysis
{
    public class ZonalStatisticsAsTableByMappingFile : ModelTool
    {
        private string _zoneFileName;
        private Dictionary<int, int[]> _zoneCache; // 缓存映射关系
        public enum DataFilters { GreaterThan, LessThan, NotEqualTo, EqualTo, None };
        public enum DataOperations { Sum, Average}
        
        public ZonalStatisticsAsTableByMappingFile()
        {
            Name = "Zonal Statistics As Table By Mapping File";
            Category = "Tempo-Spatial Analysis";
            Description = "Zonal As Table By Mapping File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            Output = "zonal";
            ThreshholdValue = -999;
            BatchSize = 0;
            EnableParallel=true;
            FilterOperation = ZonalStatisticsAsTableByMappingFile.DataFilters.None;
        }

        [Category("Input")]
        [Description("Input datacube. The matrix name should be written as A[0][:][:]")]
        public string DataCube { get; set; }
 
        [Category("Parameter")]
        [Description("Value used to applied on the Data Operation")]
        public float ThreshholdValue { get; set; }

        [Category("Parameter")]
        [Description("Filter operation")]
        public DataFilters FilterOperation { get; set; }

        [Category("Parameter")]
        [Description("Data operation")]
        public DataOperations DataOperation { get; set; }

        [Category("Output")]
        [Description("The name of output statistics table")]
        public string Output { get; set; }

        [Category("Performance")]
        [Description("Enable parallel processing for better performance")]
        [DefaultValue(true)]
        public bool EnableParallel { get; set; }

        [Category("Performance")]
        [Description("Batch size for parallel processing (0 for auto)")]
        [DefaultValue(0)]
        public int BatchSize { get; set; }



        public override void Initialize()
        {
            var mat = Get3DMat(DataCube);
            Initialized = mat != null;
        }

        [Category("Input")]
        [Description("The zone filename. It contains two columns: zone id and hru id")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ZoneFileName
        {
            get { return _zoneFileName; }
            set { _zoneFileName = value; }
        }

        // 优化版本：使用数组替代List，减少内存分配
        public Dictionary<int, int[]> GetZoneOptimized()
        {
            var dic = new Dictionary<int, List<int>>();
            
            // 使用一次性读取替代逐行读取
            var allLines = System.IO.File.ReadAllLines(_zoneFileName);
            
            // 跳过标题行
            for (int i = 1; i < allLines.Length; i++)
            {
                var parts = allLines[i].Split(',');
                if (parts.Length >= 2)
                {
                    int zoneId = int.Parse(parts[0]);
                    int hruId = int.Parse(parts[1]);
                    List<int> hruList;
                    if (!dic.TryGetValue(zoneId, out hruList))
                    {
                        hruList = new List<int>();
                        dic[zoneId] = hruList;
                    }
                    hruList.Add(hruId);
                }
            }
            
            // 转换为数组，提高访问速度
            var result = new Dictionary<int, int[]>();
            foreach (var kvp in dic)
            {
                result[kvp.Key] = kvp.Value.ToArray();
            }
            
            return result;
        }

        // 缓存数据，避免重复计算
        private void CacheData(DataCube<float> mat)
        {
            float[, ,] _matDataCache = null;
            if (_matDataCache == null)
            {
                int var_indexA = 0;
                int nstep = mat.Size[1];
                int ncell = mat.Size[2];
                
                // 缓存为三维数组，提高访问速度
                _matDataCache = new float[1, nstep, ncell];
                for (int t = 0; t < nstep; t++)
                {
                    for (int c = 0; c < ncell; c++)
                    {
                        _matDataCache[0, t, c] = mat[var_indexA, t, c];
                    }
                }
            }
        }

        // 并行计算版本
        private void CalculateParallel(DataCube<float> mat, DataCube<float> mat_out, 
                                       Dictionary<int, int[]> dic, int nstep, int nzone,
                                       ICancelProgressHandler cancelProgressHandler, int var_indexA)
        {
            int batchSize = BatchSize > 0 ? BatchSize : System.Math.Max(1, nstep / Environment.ProcessorCount);
            var zoneKeys = dic.Keys.ToArray();
            
            // 使用并行循环
            Parallel.For(0, nstep, new ParallelOptions 
            { 
                MaxDegreeOfParallelism = Environment.ProcessorCount 
            }, (t, state) =>
            {
                if(FilterOperation == DataFilters.EqualTo)
                {
                    for (int c = 0; c < nzone; c++)
                    {
                        float value = mat[var_indexA, t, subIdsArray[j] - 1];
                        if (value != NoDataValue)
                        {
                            float value = mat[var_indexA, t, subIdsArray[j] - 1];
                            if (value == ThreshholdValue)
                            {
                                sum += value;
                                len++;
                            }
                        }
                        if(DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }
                }
                else if (FilterOperation == DataFilters.GreaterThan)
                {
                    for (int c = 0; c < nzone; c++)
                    {
                        var zoneId = zoneKeys[c];
                        var subIds = dic[zoneId];
                        int len = 0;
                        float sum = 0;

                        // 使用本地变量提高性能
                        var subIdsArray = subIds;
                        int subIdCount = subIdsArray.Length;

                        for (int j = 0; j < subIdCount; j++)
                        {
                            float value = mat[var_indexA, t, subIdsArray[j] - 1];
                            if (value > ThreshholdValue)
                            {
                                sum += value;
                                len++;
                            }
                        }

                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }
                }
                else if (FilterOperation == DataFilters.LessThan)
                {
                    for (int c = 0; c < nzone; c++)
                    {
                        var zoneId = zoneKeys[c];
                        var subIds = dic[zoneId];
                        int len = 0;
                        float sum = 0;

                        // 使用本地变量提高性能
                        var subIdsArray = subIds;
                        int subIdCount = subIdsArray.Length;

                        for (int j = 0; j < subIdCount; j++)
                        {
                            float value = mat[var_indexA, t, subIdsArray[j] - 1];
                            if (value < ThreshholdValue)
                            {
                                sum += value;
                                len++;
                            }
                        }

                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }
                }
                else if (FilterOperation == DataFilters.None)
                {
                    for (int c = 0; c < nzone; c++)
                    {
                        var zoneId = zoneKeys[c];
                        var subIds = dic[zoneId];
                        int len = 0;
                        float sum = 0;

                        // 使用本地变量提高性能
                        var subIdsArray = subIds;
                        int subIdCount = subIdsArray.Length;

                        for (int j = 0; j < subIdCount; j++)
                        {
                            float value = mat[var_indexA, t, subIdsArray[j] - 1];
                            sum += value;
                            len++;
                        }
                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }
                }
                else if (FilterOperation == DataFilters.NotEqualTo)
                {
                    for (int c = 0; c < nzone; c++)
                    {
                        var zoneId = zoneKeys[c];
                        var subIds = dic[zoneId];
                        int len = 0;
                        float sum = 0;

                        // 使用本地变量提高性能
                        var subIdsArray = subIds;
                        int subIdCount = subIdsArray.Length;

                        for (int j = 0; j < subIdCount; j++)
                        {
                            float value = mat[var_indexA, t, subIdsArray[j] - 1];
                            if (value != ThreshholdValue)
                            {
                                sum += value;
                                len++;
                            }
                        }

                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }
                }

                // 进度报告（减少报告频率）
                if (t % 100 == 0)
                {
                    cancelProgressHandler.Progress("Package_Tool", 
                        (int)((t + 1) * 100.0 / nstep), 
                        string.Format( "Calculating Step: {0}", t+1));
                }
            });
        }

        // 串行计算版本（优化循环顺序）
        private void CalculateSerial(DataCube<float> mat, DataCube<float> mat_out,
                                     Dictionary<int, int[]> dic, int nstep, int nzone,
                                     ICancelProgressHandler cancelProgressHandler, int var_indexA)
        {
            var zoneKeys = dic.Keys.ToArray();
            
            // 优化循环顺序：外层遍历zone，内层遍历time
            // 这样可以更好地利用CPU缓存
            if (FilterOperation == DataFilters.EqualTo)
            {
                for (int c = 0; c < nzone; c++)
                {
                    var zoneId = zoneKeys[c];
                    var subIds = dic[zoneId];
                    int subIdCount = subIds.Length;

                    for (int t = 0; t < nstep; t++)
                    {
                        float value = mat[var_indexA, t, subIds[j] - 1];
                        if (value != NoDataValue)
                        {
                            float value = mat[var_indexA, t, subIds[j] - 1];
                            if (value == ThreshholdValue)
                            {
                                sum += value;
                                len++;
                            }
                        }
                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }

                    // 进度报告
                    cancelProgressHandler.Progress("Package_Tool",
                        (int)((c + 1) * 100.0 / nzone),
                      string.Format("Processing Zone: {0}/{1}", c + 1, nzone));
                }
            }
            else if (FilterOperation == DataFilters.GreaterThan)
            {
                for (int c = 0; c < nzone; c++)
                {
                    var zoneId = zoneKeys[c];
                    var subIds = dic[zoneId];
                    int subIdCount = subIds.Length;

                    for (int t = 0; t < nstep; t++)
                    {
                        int len = 0;
                        float sum = 0;

                        for (int j = 0; j < subIdCount; j++)
                        {
                            float value = mat[var_indexA, t, subIds[j] - 1];
                            if (value > ThreshholdValue)
                            {
                                sum += value;
                                len++;
                            }
                        }
                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }

                    // 进度报告
                    cancelProgressHandler.Progress("Package_Tool",
                        (int)((c + 1) * 100.0 / nzone),
                      string.Format("Processing Zone: {0}/{1}", c + 1, nzone));
                }
            }
            else if (FilterOperation == DataFilters.LessThan)
            {
                for (int c = 0; c < nzone; c++)
                {
                    var zoneId = zoneKeys[c];
                    var subIds = dic[zoneId];
                    int subIdCount = subIds.Length;

                    for (int t = 0; t < nstep; t++)
                    {
                        int len = 0;
                        float sum = 0;

                        for (int j = 0; j < subIdCount; j++)
                        {
                            float value = mat[var_indexA, t, subIds[j] - 1];
                            if (value < ThreshholdValue)
                            {
                                sum += value;
                                len++;
                            }
                        }
                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }

                    // 进度报告
                    cancelProgressHandler.Progress("Package_Tool",
                        (int)((c + 1) * 100.0 / nzone),
                      string.Format("Processing Zone: {0}/{1}", c + 1, nzone));
                }
            }
            else if (FilterOperation == DataFilters.None)
            {
                for (int c = 0; c < nzone; c++)
                {
                    var zoneId = zoneKeys[c];
                    var subIds = dic[zoneId];
                    int subIdCount = subIds.Length;

                    for (int t = 0; t < nstep; t++)
                    {
                        int len = 0;
                        float sum = 0;

                        for (int j = 0; j < subIdCount; j++)
                        {
                            float value = mat[var_indexA, t, subIds[j] - 1];
                            sum += value;
                            len++;
                        }
                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }

                    // 进度报告
                    cancelProgressHandler.Progress("Package_Tool",
                        (int)((c + 1) * 100.0 / nzone),
                      string.Format("Processing Zone: {0}/{1}", c + 1, nzone));
                }
            }
            else if (FilterOperation == DataFilters.NotEqualTo)
            {
                for (int c = 0; c < nzone; c++)
                {
                    var zoneId = zoneKeys[c];
                    var subIds = dic[zoneId];
                    int subIdCount = subIds.Length;

                    for (int t = 0; t < nstep; t++)
                    {
                        int len = 0;
                        float sum = 0;

                        for (int j = 0; j < subIdCount; j++)
                        {
                            float value = mat[var_indexA, t, subIds[j] - 1];
                            if (value != ThreshholdValue)
                            {
                                sum += value;
                                len++;
                            }
                        }
                        if (DataOperation == DataOperations.Average)
                            mat_out[0, t, c] = len > 0 ? sum / len : 0;
                        else if (DataOperation == DataOperations.Sum)
                            mat_out[0, t, c] = sum;
                    }

                    // 进度报告
                    cancelProgressHandler.Progress("Package_Tool",
                        (int)((c + 1) * 100.0 / nzone),
                      string.Format("Processing Zone: {0}/{1}", c + 1, nzone));
                }
            }
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            int var_indexA = 0;
            var mat = Get3DMat(DataCube, ref var_indexA);
            
            if (mat == null) return false;
            
            // 缓存数据
            //CacheData(mat);
            
            // 获取区域映射（使用优化版本）
            var dic = GetZoneOptimized();
            _zoneCache = dic; // 缓存映射关系

            var keys = string.Join(",", dic.Keys);
            cancelProgressHandler.Progress("Package_Tool", 1, keys);

            int nzone = dic.Count;
            int nstep = mat.Size[1];
            var dickeys = string.Join(",", dic.Keys);
            cancelProgressHandler.Progress("Package_Tool", 1, dickeys);
            // 预分配输出矩阵
            var mat_out = new DataCube<float>(1, nstep, nzone)
            {
                Name = Output,
                Variables = new[] { "Mean" }
            };
            
            // 选择计算方法
            if (EnableParallel && nstep > 1000) // 只有数据量较大时并行才有优势
            {
                CalculateParallel(mat, mat_out, dic, nstep, nzone, cancelProgressHandler, var_indexA);
            }
            else
            {
                CalculateSerial(mat, mat_out, dic, nstep, nzone, cancelProgressHandler, var_indexA);
            }
            
            Workspace.Add(mat_out);
            return true;
        }
    }
}