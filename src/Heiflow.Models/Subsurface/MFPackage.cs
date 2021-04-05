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
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.Visualization;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    public class MFPackage : Package, IExtractMFPackage, IAssignParameter,IMFPackage
    {
        public MFPackage()
        {
            _PackageInfo = new PackageInfo();
            Options = "";
            NoDataValue = 0;
        }

       [Category("Data")]
        public float NoDataValue { get; set; }

        /// <summary>
        /// 2D array [np,3]
        /// </summary>
        [Browsable(false)]
        public int[,] SPInfo
        {
            get;
            set;
        }
        [Category("Option")]
        public string Options { get; set; }

         [Browsable(false)]
        public Modflow ModflowInstance
        {
            get
            {
                return Owner as Modflow;
            }
        }
         [Browsable(false)]
         public MFGrid MFGridInstance
        {
            get
            {
                return Owner.Grid as MFGrid;
            }
        }
        public override void Initialize()
        {
            //this.LoadFailed += ModflowInstance.OnLoadFailed;
            base.Initialize();
        }
        /// <summary>
        /// return the first occurrence of a line that is not comment
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public string ReadComment(StreamReader sr)
        {
            string lastline = "";
            while (!sr.EndOfStream)
            {
                lastline = sr.ReadLine();
                if (!lastline.StartsWith("#"))
                    break;
            }
            return lastline;
        }
        public int GetNumSP()
        {
            var mf = Owner as Modflow;
            var nsp = mf.TimeService.StressPeriods.Count;
            return nsp;
        }
        public DataCube<T> ReadInternalMatrix<T>(StreamReader sr)
        {
            string line = sr.ReadLine().ToUpper();
            var strs = TypeConverterEx.Split<string>(line);
            var grid = Owner.Grid as IRegularGrid;
            int row = grid.RowCount;
            int col = grid.ColumnCount;

            // Read constant matrix
            if (strs[0].ToUpper() == "CONSTANT")
            {
                var matrix = new DataCube<T>(1, row, col);
                var ar = TypeConverterEx.Split<string>(line);
                T conv = TypeConverterEx.ChangeType<T>(ar[1]);
                matrix.ILArrays[0][":,", ":"] = conv;
                return matrix;
            }
            // Read internal matrix
            else
            {
                var matrix = new DataCube<T>(1, row, col);

                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);

                if (values.Length == col)
                {
                    matrix[0][0, ":"] = values;
                    for (int r = 1; r < row; r++)
                    {
                        line = sr.ReadLine();
                        values = TypeConverterEx.Split<T>(line);
                        matrix[0][r, ":"] = values;
                    }

                }
                else
                {
                    int colLine = (int)Math.Ceiling(col / (float)values.Length);
                    for (int r = 0; r < row; r++)
                    {
                        int i = 0;
                        if (r == 0)
                        {
                            i = 1;
                        }
                        else
                        {
                            i = 0;
                            line = "";
                        }
                        for (; i < colLine; i++)
                        {
                            line += sr.ReadLine() + " ";
                        }

                        values = TypeConverterEx.Split<T>(line);
                        matrix[0][r, ":"] = values;
                    }
                }
                return matrix;
            }
        }

        public int GetFormatLen()
        {
            int len = 10;

            return len;
        }

        public void ReadSerialArray<T>(StreamReader sr, DataCube<T> mat, int var_index, int time_index)
        {
            string line = sr.ReadLine().ToUpper();
            var strs = TypeConverterEx.Split<string>(line);
            var grid = Owner.Grid as IRegularGrid;
            var integerform = new char[] {'i','I'};
            // Read constant matrix
            if (strs[0].ToUpper() == "CONSTANT" || strs[0]=="0")
            {
                var ar = TypeConverterEx.Split<string>(line);
                T conv = TypeConverterEx.ChangeType<T>(ar[1]);
                mat.Flags[var_index] = TimeVarientFlag.Constant;
                mat.Constants[var_index] = TypeConverterEx.ChangeType<float>( conv);
                mat.ILArrays[var_index][time_index,":"] = conv;
            }
            // Read internal matrix
            else
            {
                int row = grid.RowCount;
                int col = grid.ColumnCount;
                int activeCount = grid.ActiveCellCount;
//                mat.Value[var_index][time_index] = new T[activeCount];
                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                int iprn = -1;

                var format= strs[2].Replace('(',' ');
                format= format.Replace(')',' ');
                format = format.Trim();

                var collen=10;
                var pos =strs[2].IndexOfAny(integerform, 0);
                var format = strs[2].Substring(1, );
                int.TryParse(strs[3], out iprn);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);
                int r = 1;
                T[] vector = new T[activeCount];
                if (values.Length == col)
                {
                    int index = 0;
                    for (int c = 0; c < col; c++)
                    {
                        if (grid.IBound[0, 0, c] != 0)
                        {
                            //mat[var_index,time_index,index] = values[c];
                            vector[index] = values[c];
                            index++;
                        }
                    }
                    try
                    {
                        for (r = 1; r < row; r++)
                        {
                            line = sr.ReadLine();
                            values = TypeConverterEx.Split<T>(line);
                            for (int c = 0; c < col; c++)
                            {
                                if (grid.IBound[0, r, c] != 0)
                                {
                                    //mat[var_index, time_index, index] = values[c];
                                    vector[index] = values[c];
                                    index++;
                                }
                            }
                        }
                        mat[var_index, time_index.ToString(), ":"] = vector;
                    }
                    catch (Exception ex)
                    {
                        Message = string.Format("Failed to read Array when reading Line {0}. Error: {1}", r, ex.Message);
                    }
                }
                else
                {
                    int index = 0;
                    int colLine = (int)Math.Ceiling(col / 10.0);
                    try
                    {
                        line += "\t";
                        for (int c = 1; c < colLine; c++)
                        {
                            line += sr.ReadLine() + "\t";
                        }
                        values = TypeConverterEx.Split<T>(line);
                        for (int c = 0; c < col; c++)
                        {
                            if (grid.IBound[0, 0, c] != 0)
                            {
                                //mat[var_index,time_index,index] = values[c];
                                vector[index] = values[c];
                                index++;
                            }
                        }

                        for (r = 1; r < row; r++)
                        {
                            line = "";
                            for (int c = 0; c < colLine; c++)
                            {
                                line += sr.ReadLine() + "\t";
                            }

                            values = TypeConverterEx.Split<T>(line);
                            for (int c = 0; c < col; c++)
                            {
                                if (grid.IBound[0, r, c] != 0)
                                {
                                    //mat[var_index,time_index,index] = values[c];
                                    vector[index] = values[c];
                                    index++;
                                }
                            }
                        }
                        mat[var_index, time_index.ToString(), ":"] = vector;
                    }
                    catch (Exception ex)
                    {
                        Message = string.Format("Failed to read Array when reading Line {0}. Error: {1}", r, ex.Message);
                    }
                }
                mat.Flags[var_index] = TimeVarientFlag.Individual;
                mat.Multipliers[var_index] = TypeConverterEx.ChangeType<float>( multiplier);
                mat.IPRN[var_index] = iprn;
            }
        }
        public void ReadSerialArray<T>(StreamReader sr, DataCube<T> mat, int var_index, int time_index, int ncell)
        {
            string line = sr.ReadLine().ToUpper();
            var strs = TypeConverterEx.Split<string>(line);
            var grid = Owner.Grid as IRegularGrid;
            int row = grid.RowCount;
            int col = grid.ColumnCount;
            int activeCount = grid.ActiveCellCount;

            // Read constant matrix
            if (strs[0].ToUpper() == "CONSTANT")
            {
                var ar = TypeConverterEx.Split<string>(line);
                T conv = TypeConverterEx.ChangeType<T>(ar[1]);
                mat.Flags[var_index] = TimeVarientFlag.Constant;
                mat.Constants[var_index] = TypeConverterEx.ChangeType<float>(conv);
                mat.ILArrays[var_index][time_index, ":"] = conv;
            }
            // Read internal matrix
            else
            {
               // mat.Value[var_index][time_index] = new T[ncell];
                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                int iprn = -1;
                int.TryParse(strs[3], out iprn);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);
                
                if (values.Length == ncell)
                {
                    //for(int i=0;i<ncell;i++)
                    //{
                    //    mat[var_index,time_index,i] = values[i];
                    //}
                    mat[var_index, time_index.ToString(), ":"] = values;
                }
                else
                {
                    int colLine = (int)Math.Ceiling(col / 10.0);
                    try
                    {
                        line += "\t";
                        for (int c = 1; c < colLine; c++)
                        {
                            line += sr.ReadLine() + "\t";
                        }
                        values = TypeConverterEx.Split<T>(line);

                        //for (int i = 0; i < ncell; i++)
                        //{
                        //    mat[var_index,time_index,i] = values[i];
                        //}
                        mat[var_index, time_index.ToString(), ":"] = values;
                    }
                    catch (Exception ex)
                    {
                        Message = string.Format("Failed to read Array when reading Line. Error: {0}",  ex.Message);
                    }
                }
                mat.Flags[var_index] = TimeVarientFlag.Individual;
                mat.Multipliers[var_index] = TypeConverterEx.ChangeType<float>(multiplier);
                mat.IPRN[var_index] = iprn;
            }
        }
        public void ReadRegularArray<T>(StreamReader sr, DataCube<T> mat, int var_index)
        {
            string line = sr.ReadLine().ToUpper();
            var strs = TypeConverterEx.Split<string>(line);
            var grid = Owner.Grid as IRegularGrid;

            // Read constant matrix
            if (strs[0].ToUpper() == "CONSTANT")
            {
                var ar = TypeConverterEx.Split<string>(line);
                T conv = TypeConverterEx.ChangeType<T>(ar[1]);
                mat.Flags[var_index] = TimeVarientFlag.Constant;
                mat.Constants[var_index] = TypeConverterEx.ChangeType<float>(conv);
                mat.ILArrays[var_index][":", ":"] = conv;
            }
            // Read internal matrix
            else
            {
                int row = grid.RowCount;
                int col = grid.ColumnCount;
                int activeCount = grid.ActiveCellCount;
                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                int iprn = -1;
                int.TryParse(strs[3], out iprn);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);
                int r = 1;
                if (values.Length == col)
                {
                    for (int c = 0; c < col; c++)
                    {
                        mat[var_index][0, c] = values[c];
                    }
                    try
                    {
                        for (r = 1; r < row; r++)
                        {
                            line = sr.ReadLine();
                            values = TypeConverterEx.Split<T>(line);
                            for (int c = 0; c < col; c++)
                            {
                                mat[var_index][r, c] = values[c];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = string.Format("Failed to read Array when reading Line {0}. Error: {1}", r, ex.Message);
                    }
                }
                else
                {
                    int colLine = (int)Math.Ceiling(col / 10.0);
                    try
                    {
                        line += "\t";
                        for (int c = 1; c < colLine; c++)
                        {
                            line += sr.ReadLine() + "\t";
                        }
                        values = TypeConverterEx.Split<T>(line);
                        for (int c = 0; c < col; c++)
                        {
                            mat[var_index][0, c] = values[c];
                        }

                        for (r = 1; r < row; r++)
                        {
                            line = "";
                            for (int c = 0; c < colLine; c++)
                            {
                                line += sr.ReadLine() + "\t";
                            }

                            values = TypeConverterEx.Split<T>(line);
                            for (int c = 0; c < col; c++)
                            {
                                mat[var_index][r, c] = values[c];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = string.Format("Failed to read Array when reading Line {0}. Error: {1}", r, ex.Message);
                    }
                }
                mat.Flags[var_index] = TimeVarientFlag.Individual;
                mat.Multipliers[var_index] = TypeConverterEx.ChangeType<float>(multiplier);
                mat.IPRN[var_index] = iprn;
            }
        }
        public void WriteSerialArray<T>(StreamWriter sw, DataCube<T> mat, int var_index, int time_index, string format, string comment)
        {
            if (mat.Flags[var_index] == TimeVarientFlag.Constant)
            {
                string line = string.Format("CONSTANT\t{0}\t{1}", mat.Constants[var_index], comment);
                sw.WriteLine(line);
            }
            else if (mat.Flags[var_index] == TimeVarientFlag.Repeat)
            {

            }
            else if (mat.Flags[var_index] == TimeVarientFlag.Individual)
            {
                string line = string.Format("INTERNAL\t{0}\t(FREE)\t{1}\t{2}", mat.Multipliers[var_index], mat.IPRN[var_index], comment);
                var grid = Owner.Grid as MFGrid;
                int row = grid.RowCount;
                int col = grid.ColumnCount;
                int index = 0;

                sw.WriteLine(line);
                for (int r = 0; r < row; r++)
                {
                    line = "";
                    for (int c = 0; c < col; c++)
                    {
                        if (grid.IBound[0, r, c] != 0)
                        {
                            line += string.Format("{0}", mat[var_index, time_index, index]) + StreamReaderSequence.stab;
                            index++;
                        }
                        else
                        {
                            line += string.Format("{0}", NoDataValue.ToString(format)) + StreamReaderSequence.stab;
                        }
                    }
                    line = line.Trim(StreamReaderSequence.ctab);
                    sw.WriteLine(line);
                }
            }
        }
        public void WriteSerialFloatArray(StreamWriter sw, DataCube<float> mat, int var_index, int time_index, string format, string comment)
        {
            if (mat.Flags[var_index] == TimeVarientFlag.Constant)
            {
                string line = string.Format("CONSTANT\t{0}\t{1}", mat.Constants[var_index], comment);
                sw.WriteLine(line);
            }
            else if (mat.Flags[var_index] == TimeVarientFlag.Repeat)
            {

            }
            else if (mat.Flags[var_index] == TimeVarientFlag.Individual)
            {
                string line = string.Format("INTERNAL\t{0}\t(FREE)\t{1}\t{2}", mat.Multipliers[var_index], mat.IPRN[var_index], comment);
                var grid = Owner.Grid as MFGrid;
                int row = grid.RowCount;
                int col = grid.ColumnCount;
                int index = 0;

                sw.WriteLine(line);
                for (int r = 0; r < row; r++)
                {
                    line = "";
                    for (int c = 0; c < col; c++)
                    {
                        float vv = NoDataValue;
                        if (grid.IBound[0, r, c] != 0)
                        {
                            vv = mat[var_index,time_index,index];
                            index++;
                        }
                        line += string.Format("{0}", vv.ToString(format)) + StreamReaderSequence.stab;
                    }
                    line = line.Trim(StreamReaderSequence.ctab);
                    sw.WriteLine(line);
                }
            }
        }
        
        /// <summary>
        /// write 3d mat for given variable
        /// </summary>
        /// <param name="sw">stream writer</param>
        /// <param name="mat">the layout of the mat is [nvar][nrow][ncol]</param>
        /// <param name="var_index"></param>
        /// <param name="format"></param>
        /// <param name="comment"></param>
        public void WriteRegularArray<T>(StreamWriter sw, DataCube<T> mat, int var_index, string format, string comment)
        {
            if (mat.Flags[var_index] == TimeVarientFlag.Constant)
            {
                string line = string.Format("CONSTANT\t{0}\t{1}", mat.Constants[var_index], comment);
                sw.WriteLine(line);
            }
            else if (mat.Flags[var_index] == TimeVarientFlag.Repeat)
            {

            }
            else if (mat.Flags[var_index] == TimeVarientFlag.Individual)
            {
                string line = string.Format("INTERNAL\t{0}\t(FREE)\t{1}\t{2}", mat.Multipliers[var_index], mat.IPRN[var_index], comment);
                var grid = Owner.Grid as MFGrid;
                int row = grid.RowCount;
                int col = grid.ColumnCount;

                sw.WriteLine(line);
                for (int r = 0; r < row; r++)
                {
                    line = string.Join(StreamReaderSequence.stab, mat[var_index, r.ToString(), ":"]);
                    sw.WriteLine(line);
                }
            }
        }
        public void WriteDefaultComment(StreamWriter sw, string package)
        {
            string defaultcm = string.Format("#{0}: created on {1} by Visual HEIFLOW", package, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            sw.WriteLine(defaultcm);
        }
        public virtual IPackage Extract(Modflow newmf)
        {
            return null;
        }

        public virtual void Assign(DotSpatial.Data.IFeatureSet feature, params object[] paras)
        {
            
        }

        public virtual void CompositeOutput(MFOutputPackage mfout)
        {
             
        }
        public override void Clear()
        {
            var mf = Owner as Modflow;
            //this.LoadFailed -= mf.OnLoadFailed;
            base.Clear();
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
           
        }

        public override LoadingState Load(ICancelProgressHandler progess)
        {
            return LoadingState.Normal;
        }
    }
}
