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

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace Heiflow.Core.IO
//{
//    public class DataCubeStreamWriter : IDisposable
//    {
//        private const int BUFFER_SIZE = 81920; // 80KB buffer
//        private const int MAX_BUFFER_FLOATS = BUFFER_SIZE / 4; // 浮点数数量
        
//        private readonly string _fileName;
//        private readonly FileStream _fileStream;
//        private readonly BinaryWriter _binaryWriter;
//        private readonly DataCubeDescriptor _descriptor;
//        private readonly byte[] _writeBuffer;
//        private int _bufferPosition;
//        private bool _disposed = false;

//        public DataCubeStreamWriter(string filename)
//        {
//            _fileName = filename;
//            _fileStream = new FileStream(_fileName, FileMode.Create, FileAccess.Write, 
//                FileShare.None, BUFFER_SIZE, FileOptions.SequentialScan);
//            _binaryWriter = new BinaryWriter(_fileStream);
//            _descriptor = new DataCubeDescriptor();
//            _writeBuffer = new byte[BUFFER_SIZE];
//            _bufferPosition = 0;
//        }

//        // 高性能缓冲区写入方法
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private void WriteToBuffer(float value)
//        {
//            if (_bufferPosition + 4 > BUFFER_SIZE)
//            {
//                FlushBuffer();
//            }
            
//            byte[] bytes = BitConverter.GetBytes(value);
//            Buffer.BlockCopy(bytes, 0, _writeBuffer, _bufferPosition, 4);
//            _bufferPosition += 4;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private void WriteToBuffer(int value)
//        {
//            if (_bufferPosition + 4 > BUFFER_SIZE)
//            {
//                FlushBuffer();
//            }
            
//            byte[] bytes = BitConverter.GetBytes(value);
//            Buffer.BlockCopy(bytes, 0, _writeBuffer, _bufferPosition, 4);
//            _bufferPosition += 4;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private void WriteToBuffer(char[] chars)
//        {
//            int byteLength = Encoding.UTF8.GetByteCount(chars);
//            if (_bufferPosition + byteLength > BUFFER_SIZE)
//            {
//                FlushBuffer();
//            }
            
//            Encoding.UTF8.GetBytes(chars, 0, chars.Length, _writeBuffer, _bufferPosition);
//            _bufferPosition += byteLength;
//        }

//        private void FlushBuffer()
//        {
//            if (_bufferPosition > 0)
//            {
//                _fileStream.Write(_writeBuffer, 0, _bufferPosition);
//                _bufferPosition = 0;
//            }
//        }

//        public void WriteHeader(string[] varNames, int feaNum)
//        {
//            int varnum = varNames.Length;
//            WriteToBuffer(varnum);
            
//            foreach (var varName in varNames)
//            {
//                WriteToBuffer(varName.Length);
//                WriteToBuffer(varName.ToCharArray());
//                WriteToBuffer(feaNum);
//            }
            
//            FlushBuffer();
//        }

//        // 使用 Buffer.BlockCopy 替代 MemoryCopy 的优化方法
//        private void WriteFloatArrayBatch(float[] data)
//        {
//            int dataLength = data.Length;
//            int dataIndex = 0;
            
//            while (dataIndex < dataLength)
//            {
//                // 计算本次可以写入缓冲区的浮点数数量
//                int floatsToWrite = Math.Min((BUFFER_SIZE - _bufferPosition) / 4, 
//                                            dataLength - dataIndex);
                
//                if (floatsToWrite > 0)
//                {
//                    // 使用 Buffer.BlockCopy 进行批量复制
//                    int byteLength = floatsToWrite * 4;
//                    Buffer.BlockCopy(data, dataIndex * 4, _writeBuffer, _bufferPosition, byteLength);
                    
//                    _bufferPosition += byteLength;
//                    dataIndex += floatsToWrite;
//                }
                
//                if (_bufferPosition >= BUFFER_SIZE - 4)
//                {
//                    FlushBuffer();
//                }
//            }
//        }

//        // 优化的写入方法 - 使用不安全代码但兼容 .NET 4.5
//        public unsafe void WriteStep(int varnum, int feaNum, DataCube<float> data)
//        {
//            int totalFloats = varnum * feaNum;
//            float[] buffer = new float[totalFloats];
            
//            // 使用固定内存块进行快速填充
//            fixed (float* bufferPtr = buffer)
//            {
//                int index = 0;
//                for (int s = 0; s < feaNum; s++)
//                {
//                    for (int v = 0; v < varnum; v++)
//                    {
//                        bufferPtr[index++] = data[v, 0, s];
//                    }
//                }
//            }
            
//            // 批量写入缓冲区
//            WriteFloatArrayBatch(buffer);
//        }

//        // 完全重写的WriteAll方法，使用最优化内存访问模式
//        public void WriteAll(DataCube<float> mat)
//        {
//            var varNames = mat.Variables;
//            int steps = mat.Size[1];
//            int feaNum = mat.Size[2];
//            int varnum = varNames.Length;
//            int floatsPerStep = varnum * feaNum;
            
//            // 写入头部
//            WriteHeader(varNames, feaNum);
            
//            // 预分配大缓冲区用于批量处理
//            float[] stepBuffer = new float[floatsPerStep];
            
//            for (int step = 0; step < steps; step++)
//            {
//                // 快速填充缓冲区
//                int bufferIndex = 0;
//                for (int s = 0; s < feaNum; s++)
//                {
//                    for (int v = 0; v < varnum; v++)
//                    {
//                        stepBuffer[bufferIndex++] = mat[v, step, s];
//                    }
//                }
                
//                // 批量写入
//                WriteFloatArrayBatch(stepBuffer);
//            }
            
//            FlushBuffer();
//            WriteDescriptor(mat, varnum, steps, feaNum);
//        }

//        // 针对特定变量索引的优化版本
//        public void WriteAll(DataCube<float> mat, int[] varIndex)
//        {
//            var varNames = mat.Variables;
//            int steps = mat.Size[1];
//            int feaNum = mat.Size[2];
//            int varnum = varIndex.Length;
//            int floatsPerStep = varnum * feaNum;
            
//            // 优化头部写入
//            WriteToBuffer(varnum);
//            foreach (int index in varIndex)
//            {
//                string varName = varNames[index];
//                WriteToBuffer(varName.Length);
//                WriteToBuffer(varName.ToCharArray());
//                WriteToBuffer(feaNum);
//            }
//            FlushBuffer();
            
//            // 批量数据处理
//            float[] stepBuffer = new float[floatsPerStep];
            
//            for (int step = 0; step < steps; step++)
//            {
//                int bufferIndex = 0;
//                for (int s = 0; s < feaNum; s++)
//                {
//                    foreach (int v in varIndex)
//                    {
//                        stepBuffer[bufferIndex++] = mat[v, step, s];
//                    }
//                }
//                WriteFloatArrayBatch(stepBuffer);
//            }
            
//            FlushBuffer();
//            WriteDescriptor(mat, varnum, steps, feaNum);
//            Close();
//        }

//        // 不使用不安全代码的安全版本
//        public void WriteAllSafe(DataCube<float> mat)
//        {
//            var varNames = mat.Variables;
//            int steps = mat.Size[1];
//            int feaNum = mat.Size[2];
//            int varnum = varNames.Length;
            
//            WriteHeader(varNames, feaNum);
            
//            // 使用更大的缓冲区减少刷新次数
//            int optimalBufferSize = Math.Min(varnum * feaNum * 10, 100000); // 最多10个时间步或100KB
//            float[] largeBuffer = new float[optimalBufferSize];
//            int bufferFloatsUsed = 0;
            
//            for (int step = 0; step < steps; step++)
//            {
//                for (int s = 0; s < feaNum; s++)
//                {
//                    for (int v = 0; v < varnum; v++)
//                    {
//                        largeBuffer[bufferFloatsUsed++] = mat[v, step, s];
                        
//                        // 缓冲区满了就写入
//                        if (bufferFloatsUsed >= optimalBufferSize)
//                        {
//                            WriteFloatArrayBatch(largeBuffer, bufferFloatsUsed);
//                            bufferFloatsUsed = 0;
//                        }
//                    }
//                }
//            }
            
//            // 写入剩余数据
//            if (bufferFloatsUsed > 0)
//            {
//                WriteFloatArrayBatch(largeBuffer, bufferFloatsUsed);
//            }
            
//            FlushBuffer();
//            WriteDescriptor(mat, varnum, steps, feaNum);
//        }

//        // 支持部分数组写入的重载
//        private void WriteFloatArrayBatch(float[] data, int count)
//        {
//            if (count <= 0) return;
            
//            int dataIndex = 0;
            
//            while (dataIndex < count)
//            {
//                int floatsToWrite = Math.Min((BUFFER_SIZE - _bufferPosition) / 4, count - dataIndex);
                
//                if (floatsToWrite > 0)
//                {
//                    int byteLength = floatsToWrite * 4;
//                    Buffer.BlockCopy(data, dataIndex * 4, _writeBuffer, _bufferPosition, byteLength);
                    
//                    _bufferPosition += byteLength;
//                    dataIndex += floatsToWrite;
//                }
                
//                if (_bufferPosition >= BUFFER_SIZE - 4)
//                {
//                    FlushBuffer();
//                }
//            }
//        }

//        // 并行写入版本（兼容.NET 4.5）
//        public void WriteAllParallel(DataCube<float> mat)
//        {
//            var varNames = mat.Variables;
//            int steps = mat.Size[1];
//            int feaNum = mat.Size[2];
//            int varnum = varNames.Length;
            
//            WriteHeader(varNames, feaNum);
            
//            // 为每个线程创建临时文件，最后合并
//            string tempDir = Path.GetDirectoryName(_fileName);
//            string baseName = Path.GetFileNameWithoutExtension(_fileName);
            
//            // 并行处理
//            Parallel.For(0, steps, new ParallelOptions { 
//                MaxDegreeOfParallelism = Environment.ProcessorCount 
//            }, step =>
//            {
//                string tempFile = Path.Combine(tempDir, string.Format("{0}_temp_{1}.bin", baseName, step));
//                using (var tempWriter = new DataCubeStreamWriter(tempFile))
//                {
//                    float[] stepData = new float[varnum * feaNum];
//                    int index = 0;
                    
//                    for (int s = 0; s < feaNum; s++)
//                    {
//                        for (int v = 0; v < varnum; v++)
//                        {
//                            stepData[index++] = mat[v, step, s];
//                        }
//                    }
                    
//                    // 写入临时文件
//                    tempWriter.WriteFloatArrayBatch(stepData);
//                    tempWriter.FlushBuffer();
//                }
//            });
            
//            // 合并临时文件
//            MergeTempFiles(tempDir, baseName, steps);
//            WriteDescriptor(mat, varnum, steps, feaNum);
//        }

//        private void MergeTempFiles(string tempDir, string baseName, int steps)
//        {
//            // 按顺序合并所有临时文件
//            for (int step = 0; step < steps; step++)
//            {
//                string tempFile = Path.Combine(tempDir, string.Format("{0}_temp_{1}.bin", baseName, step));
//                if (File.Exists(tempFile))
//                {
//                    byte[] tempData = File.ReadAllBytes(tempFile);
//                    _fileStream.Write(tempData, 0, tempData.Length);
//                    File.Delete(tempFile); // 清理临时文件
//                }
//            }
//        }

//        private void WriteDescriptor(DataCube<float> mat, int varnum, int steps, int feaNum)
//        {
//            if (mat.DateTimes != null)
//            {
//                _descriptor.NVar = varnum;
//                _descriptor.NTimeStep = steps;
//                _descriptor.NCell = feaNum;
//                _descriptor.TimeStamps = mat.DateTimes;
//                DataCubeDescriptor.Serialize(_fileName + ".xml", _descriptor);
//            }
//        }

//        public void Close()
//        {
//            _binaryWriter.Close();
//            _fileStream.Close(); 
//            Dispose();
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!_disposed)
//            {
//                if (disposing)
//                {
//                    FlushBuffer();
//                    _binaryWriter.Dispose();
//                    _fileStream.Dispose();
//                }
//                _disposed = true;
//            }
//        }

//        ~DataCubeStreamWriter()
//        {
//            Dispose(false);
//        }
//    }
//}

using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.IO
{
    public class DataCubeStreamWriter : IDisposable
    {
        private const int BUFFER_SIZE = 81920; // 80KB buffer
        private const int FLOAT_SIZE = 4; // sizeof(float)
        
        private readonly string _fileName;
        private readonly FileStream _fileStream;
        private readonly BinaryWriter _binaryWriter;
        private readonly DataCubeDescriptor _descriptor;
        private readonly byte[] _writeBuffer;
        private int _bufferPosition;
        private bool _disposed = false;

        public DataCubeStreamWriter(string filename)
        {
            _fileName = filename;
            _fileStream = new FileStream(_fileName, FileMode.Create, FileAccess.Write, 
                FileShare.None, BUFFER_SIZE, FileOptions.SequentialScan);
            _binaryWriter = new BinaryWriter(_fileStream);
            _descriptor = new DataCubeDescriptor();
            _writeBuffer = new byte[BUFFER_SIZE];
            _bufferPosition = 0;
        }

        #region 缓冲区管理方法

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteToBuffer(float value)
        {
            if (_bufferPosition + FLOAT_SIZE > BUFFER_SIZE)
            {
                FlushBuffer();
            }
            
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, _writeBuffer, _bufferPosition, FLOAT_SIZE);
            _bufferPosition += FLOAT_SIZE;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteToBuffer(int value)
        {
            if (_bufferPosition + FLOAT_SIZE > BUFFER_SIZE)
            {
                FlushBuffer();
            }
            
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, _writeBuffer, _bufferPosition, FLOAT_SIZE);
            _bufferPosition += FLOAT_SIZE;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteToBuffer(char[] chars)
        {
            int byteLength = Encoding.UTF8.GetByteCount(chars);
            if (_bufferPosition + byteLength > BUFFER_SIZE)
            {
                FlushBuffer();
            }
            
            Encoding.UTF8.GetBytes(chars, 0, chars.Length, _writeBuffer, _bufferPosition);
            _bufferPosition += byteLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlushBuffer()
        {
            if (_bufferPosition > 0)
            {
                _fileStream.Write(_writeBuffer, 0, _bufferPosition);
                _bufferPosition = 0;
            }
        }

        #endregion

        #region 公共写入方法

        public void WriteHeader(string[] varNames, int feaNum)
        {
            int varnum = varNames.Length;
            WriteToBuffer(varnum);
            
            foreach (var varName in varNames)
            {
                WriteToBuffer(varName.Length);
                WriteToBuffer(varName.ToCharArray());
                WriteToBuffer(feaNum);
            }
            
            FlushBuffer();
        }

        public void WriteStep(int varnum, int feaNum, float[][][] data)
        {
            for (int s = 0; s < feaNum; s++)
            {
                for (int v = 0; v < varnum; v++)
                {
                    WriteToBuffer(data[v][0][s]);
                }
            }
        }

        public void WriteStep(int varnum, int feaNum, DataCube<float> data)
        {
            for (int s = 0; s < feaNum; s++)
            {
                for (int v = 0; v < varnum; v++)
                {
                    WriteToBuffer(data[v, 0, s]);
                }
            }
        }

        #endregion

        #region 优化的WriteAll方法

        /// <summary>
        /// 写入所有数据（原始方法，保持兼容性）
        /// </summary>
        public void WriteAll(float[][][] data, string[] varNames)
        {        
            int steps = data[0].Length;
            int feaNum = data[0][0].Length;
            int varnum = varNames.Length;
            
            WriteHeader(varNames, feaNum);

            for (int i = 0; i < steps; i++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        WriteToBuffer(data[v][i][s]);
                    }
                }
            }

            FlushBuffer();
        }

        /// <summary>
        /// 写入DataCube所有数据
        /// </summary>
        public void WriteAll(DataCube<float> mat)
        {
            var varNames = mat.Variables;
            int steps = mat.Size[1];
            int feaNum = mat.Size[2];
            int varnum = varNames.Length;
            
            WriteHeader(varNames, feaNum);

            for (int i = 0; i < steps; i++)
            {
                for (int s = 0; s < feaNum; s++)
                {
                    for (int v = 0; v < varnum; v++)
                    {
                        WriteToBuffer(mat[v, i, s]);
                    }
                }
            }

            FlushBuffer();
            WriteDescriptor(mat, varnum, steps, feaNum);
        }

        /// <summary>
        /// 深度优化的WriteAll方法 - 针对变量索引进行专门优化
        /// </summary>
        public void WriteAll(DataCube<float> mat, int[] varIndex)
        {
            // 参数验证
            if (mat == null) throw new ArgumentNullException("mat");
            if (varIndex == null) throw new ArgumentNullException("varIndex");
            if (varIndex.Length == 0) throw new ArgumentException("变量索引数组不能为空");

            var varNames = mat.Variables;
            int steps = mat.Size[1];
            int feaNum = mat.Size[2];
            int varnum = varIndex.Length;
            
            // 验证变量索引有效性
            ValidateVariableIndices(varIndex, varNames.Length);

            // 根据数据特征选择最优写入策略
            long totalDataSize = (long)steps * feaNum * varnum;
            if (totalDataSize < 100000) // 小数据量
            {
                WriteSmallDataOptimized(mat, varIndex, steps, feaNum, varnum);
            }
            else if (feaNum > 1000 && varnum > 5) // 大数据量且特征多
            {
                WriteLargeDataWithCache(mat, varIndex, steps, feaNum, varnum);
            }
            else // 中等数据量
            {
                WriteStandardOptimized(mat, varIndex, steps, feaNum, varnum);
            }

            WriteDescriptor(mat, varnum, steps, feaNum);
        }

        /// <summary>
        /// 并行写入版本 - 适合多核CPU和大数据量
        /// </summary>
        public void WriteAllParallel(DataCube<float> mat, int[] varIndex)
        {
            if (mat == null || varIndex == null || varIndex.Length == 0)
                throw new ArgumentException("参数验证失败");

            var varNames = mat.Variables;
            int steps = mat.Size[1];
            int feaNum = mat.Size[2];
            int varnum = varIndex.Length;

            ValidateVariableIndices(varIndex, varNames.Length);
            WriteOptimizedHeader(varNames, varIndex, feaNum);

            // 使用并行处理
            int degreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1);
            object lockObject = new object();
            
            Parallel.For(0, steps, new ParallelOptions 
            { 
                MaxDegreeOfParallelism = degreeOfParallelism 
            }, step =>
            {
                // 每个线程处理一个时间步
                float[] stepData = ExtractStepData(mat, varIndex, step, feaNum, varnum);
                byte[] byteData = ConvertToByteArray(stepData);
                
                lock (lockObject)
                {
                    _fileStream.Write(byteData, 0, byteData.Length);
                }
            });

            WriteDescriptor(mat, varnum, steps, feaNum);
        }

        #endregion

        #region 私有优化方法

        /// <summary>
        /// 验证变量索引有效性
        /// </summary>
        private void ValidateVariableIndices(int[] varIndex, int maxVarCount)
        {
            for (int i = 0; i < varIndex.Length; i++)
            {
                if (varIndex[i] < 0 || varIndex[i] >= maxVarCount)
                {
                    throw new ArgumentOutOfRangeException("varIndex", 
                        "变量索引 {varIndex[i]} 超出有效范围 [0, {maxVarCount - 1}]");
                }
            }
        }

        /// <summary>
        /// 优化的头部写入
        /// </summary>
        private void WriteOptimizedHeader(string[] varNames, int[] varIndex, int feaNum)
        {
            int varnum = varIndex.Length;
            
            // 批量写入整数数据
            WriteToBuffer(varnum);
            foreach (int index in varIndex)
            {
                WriteToBuffer(varNames[index].Length);
                WriteToBuffer(feaNum);
            }
            
            // 批量写入变量名
            foreach (int index in varIndex)
            {
                WriteToBuffer(varNames[index].ToCharArray());
            }
            
            FlushBuffer();
        }

        /// <summary>
        /// 小数据量优化策略
        /// </summary>
        private void WriteSmallDataOptimized(DataCube<float> mat, int[] varIndex, int steps, int feaNum, int varnum)
        {
            WriteOptimizedHeader(mat.Variables, varIndex, feaNum);
            
            int totalFloatsPerStep = varnum * feaNum;
            float[] stepBuffer = new float[totalFloatsPerStep];
            
            for (int step = 0; step < steps; step++)
            {
                int bufferIndex = 0;
                for (int s = 0; s < feaNum; s++)
                {
                    for (int v = 0; v < varIndex.Length; v++)
                    {
                        stepBuffer[bufferIndex++] = mat[varIndex[v], step, s];
                    }
                }
                
                WriteFloatArrayBatch(stepBuffer);
            }
            
            FlushBuffer();
        }

        /// <summary>
        /// 标准优化策略
        /// </summary>
        private void WriteStandardOptimized(DataCube<float> mat, int[] varIndex, int steps, int feaNum, int varnum)
        {
            WriteOptimizedHeader(mat.Variables, varIndex, feaNum);
            
            int totalFloatsPerStep = varnum * feaNum;
            int optimalBatchSize = CalculateOptimalBatchSize(totalFloatsPerStep);
            
            float[] batchBuffer = new float[totalFloatsPerStep * optimalBatchSize];

            for (int step = 0; step < steps; step += optimalBatchSize)
            {
                int actualBatchSize = Math.Min(optimalBatchSize, steps - step);
                int bufferIndex = 0;
                
                for (int batchStep = 0; batchStep < actualBatchSize; batchStep++)
                {
                    int currentStep = step + batchStep;
                    
                    for (int s = 0; s < feaNum; s++)
                    {
                        for (int v = 0; v < varIndex.Length; v++)
                        {
                            batchBuffer[bufferIndex++] = mat[varIndex[v], currentStep, s];
                        }
                    }
                }
                
                WriteFloatArrayBatch(batchBuffer, bufferIndex);
            }
            
            FlushBuffer();
        }

        /// <summary>
        /// 大数据量缓存优化策略
        /// </summary>
        private void WriteLargeDataWithCache(DataCube<float> mat, int[] varIndex, int steps, int feaNum, int varnum)
        {
            WriteOptimizedHeader(mat.Variables, varIndex, feaNum);
            
            // 使用缓存优化内存访问模式
            int cacheSize = Math.Min(feaNum, 1000);
            float[][] featureCache = new float[cacheSize][];
            
            for (int i = 0; i < cacheSize; i++)
            {
                featureCache[i] = new float[varnum];
            }

            int writeBufferSize = Math.Min(varnum * feaNum, 10000);
            float[] writeBuffer = new float[writeBufferSize];
            
            for (int step = 0; step < steps; step++)
            {
                int bufferIndex = 0;
                
                for (int sStart = 0; sStart < feaNum; sStart += cacheSize)
                {
                    int sEnd = Math.Min(sStart + cacheSize, feaNum);
                    int currentCacheSize = sEnd - sStart;
                    
                    // 填充缓存
                    for (int v = 0; v < varIndex.Length; v++)
                    {
                        int varIdx = varIndex[v];
                        for (int s = sStart; s < sEnd; s++)
                        {
                            int cacheIndex = s - sStart;
                            featureCache[cacheIndex][v] = mat[varIdx, step, s];
                        }
                    }
                    
                    // 从缓存写入
                    for (int s = 0; s < currentCacheSize; s++)
                    {
                        for (int v = 0; v < varIndex.Length; v++)
                        {
                            writeBuffer[bufferIndex++] = featureCache[s][v];
                            
                            if (bufferIndex >= writeBuffer.Length)
                            {
                                WriteFloatArrayBatch(writeBuffer, bufferIndex);
                                bufferIndex = 0;
                            }
                        }
                    }
                }
                
                if (bufferIndex > 0)
                {
                    WriteFloatArrayBatch(writeBuffer, bufferIndex);
                }
            }
            
            FlushBuffer();
        }

        /// <summary>
        /// 计算最优批处理大小
        /// </summary>
        private int CalculateOptimalBatchSize(int floatsPerStep)
        {
            int maxFloatsInBuffer = BUFFER_SIZE / FLOAT_SIZE;
            int batchSize = maxFloatsInBuffer / floatsPerStep;
            return Math.Max(1, Math.Min(batchSize, 100));
        }

        /// <summary>
        /// 批量写入浮点数组
        /// </summary>
        private void WriteFloatArrayBatch(float[] data)
        {
            WriteFloatArrayBatch(data, data.Length);
        }

        /// <summary>
        /// 批量写入浮点数组（指定数量）
        /// </summary>
        private void WriteFloatArrayBatch(float[] data, int count)
        {
            if (count <= 0) return;
            
            int dataIndex = 0;
            while (dataIndex < count)
            {
                int floatsToWrite = Math.Min((BUFFER_SIZE - _bufferPosition) / FLOAT_SIZE, count - dataIndex);
                
                if (floatsToWrite > 0)
                {
                    int byteLength = floatsToWrite * FLOAT_SIZE;
                    Buffer.BlockCopy(data, dataIndex * FLOAT_SIZE, _writeBuffer, _bufferPosition, byteLength);
                    
                    _bufferPosition += byteLength;
                    dataIndex += floatsToWrite;
                }
                
                if (_bufferPosition >= BUFFER_SIZE - FLOAT_SIZE)
                {
                    FlushBuffer();
                }
            }
        }

        /// <summary>
        /// 提取单个时间步的数据
        /// </summary>
        private float[] ExtractStepData(DataCube<float> mat, int[] varIndex, int step, int feaNum, int varnum)
        {
            float[] stepData = new float[varnum * feaNum];
            int index = 0;
            
            for (int s = 0; s < feaNum; s++)
            {
                for (int v = 0; v < varIndex.Length; v++)
                {
                    stepData[index++] = mat[varIndex[v], step, s];
                }
            }
            
            return stepData;
        }

        /// <summary>
        /// 将浮点数组转换为字节数组
        /// </summary>
        private byte[] ConvertToByteArray(float[] data)
        {
            byte[] byteArray = new byte[data.Length * FLOAT_SIZE];
            Buffer.BlockCopy(data, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }

        /// <summary>
        /// 写入描述文件
        /// </summary>
        private void WriteDescriptor(DataCube<float> mat, int varnum, int steps, int feaNum)
        {
            if (mat.DateTimes != null)
            {
                _descriptor.NVar = varnum;
                _descriptor.NTimeStep = steps;
                _descriptor.NCell = feaNum;
                _descriptor.TimeStamps = mat.DateTimes;
                DataCubeDescriptor.Serialize(_fileName + ".xml", _descriptor);
            }
        }

        #endregion

        #region 资源管理

        public void Close()
        {
            _binaryWriter.Close();
            _fileStream.Close(); 
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    FlushBuffer();
                    _binaryWriter.Dispose();
                    _fileStream.Dispose();
                }
                _disposed = true;
            }
        }

        ~DataCubeStreamWriter()
        {
            Dispose(false);
        }

        #endregion
    }
}