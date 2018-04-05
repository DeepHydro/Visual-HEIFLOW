// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
        public My2DMat<T> ReadInternalMatrix<T>(StreamReader sr)
        {
            string line = sr.ReadLine().ToUpper();
            var strs = TypeConverterEx.Split<string>(line);
            var grid = Owner.Grid as IRegularGrid;
            int row = grid.RowCount;
            int col = grid.ColumnCount;

            // Read constant matrix
            if (strs[0].ToUpper() == "CONSTANT")
            {
                var matrix = new My2DMat<T>(row, col);
                var ar = TypeConverterEx.Split<string>(line);
                T conv = TypeConverterEx.ChangeType<T>(ar[1]);
                matrix.Constant(conv);
                return matrix;
            }
            // Read internal matrix
            else
            {
                var matrix = new My2DMat<T>(row, col);

                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);

                if (values.Length == col)
                {
                    matrix.SetBy(values, 0, MyMath.full);
                    for (int r = 1; r < row; r++)
                    {
                        line = sr.ReadLine();
                        values = TypeConverterEx.Split<T>(line);
                        matrix.SetBy(values, r, MyMath.full);
                    }

                }
                else
                {
                    int colLine = (int)Math.Ceiling(col / 10.0);
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
                        //matrix[r, ILMath.full] = values;
                        matrix.SetBy(values, r, MyMath.full);
                    }
                }
                return matrix;
            }
        }
        public void ReadSerialArray<T>(StreamReader sr, MyVarient3DMat<T> mat, int var_index, int time_index)
        {
            string line = sr.ReadLine().ToUpper();
            var strs = TypeConverterEx.Split<string>(line);
            var grid = Owner.Grid as IRegularGrid;

            // Read constant matrix
            if (strs[0].ToUpper() == "CONSTANT")
            {
                var ar = TypeConverterEx.Split<string>(line);
                T conv = TypeConverterEx.ChangeType<T>(ar[1]);
                mat.Flags[var_index,time_index] = TimeVarientFlag.Constant;
                mat.Constants[var_index,time_index] = TypeConverterEx.ChangeType<float>( conv);
            }
            // Read internal matrix
            else
            {
                int row = grid.RowCount;
                int col = grid.ColumnCount;
                int activeCount = grid.ActiveCellCount;
                mat.Value[var_index][time_index] = new T[activeCount];
                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                int iprn = -1;
                int.TryParse(strs[3], out iprn);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);
                int r = 1;
                if (values.Length == col)
                {
                    int index = 0;
                    for (int c = 0; c < col; c++)
                    {
                        if (grid.IBound[0, 0, c] != 0)
                        {
                            mat.Value[var_index][time_index][index] = values[c];
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
                                    mat.Value[var_index][time_index][index] = values[c];
                                    index++;
                                }
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
                                mat.Value[var_index][time_index][index] = values[c];
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
                                    mat.Value[var_index][time_index][index] = values[c];
                                    index++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = string.Format("Failed to read Array when reading Line {0}. Error: {1}", r, ex.Message);
                    }
                }
                mat.Flags[var_index,time_index] = TimeVarientFlag.Individual;
                mat.Multipliers[var_index,time_index] = TypeConverterEx.ChangeType<float>( multiplier);
                mat.IPRN[var_index,time_index] = iprn;
            }
        }
        public void ReadSerialArray<T>(StreamReader sr, MyVarient3DMat<T> mat, int var_index, int time_index, int ncell)
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
                mat.Flags[var_index, 0] = TimeVarientFlag.Constant;
                mat.Constants[var_index, 0] = TypeConverterEx.ChangeType<float>(conv);
            }
            // Read internal matrix
            else
            {
                mat.Value[var_index][time_index] = new T[ncell];
                T multiplier = TypeConverterEx.ChangeType<T>(strs[1]);
                int iprn = -1;
                int.TryParse(strs[3], out iprn);
                line = sr.ReadLine();
                var values = TypeConverterEx.Split<T>(line);
                
                if (values.Length == ncell)
                {
                    for(int i=0;i<ncell;i++)
                    {
                        mat.Value[var_index][time_index][i] = values[i];
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

                        for (int i = 0; i < ncell; i++)
                        {
                            mat.Value[var_index][time_index][i] = values[i];
                        }
                    }
                    catch (Exception ex)
                    {
                        Message = string.Format("Failed to read Array when reading Line. Error: {0}",  ex.Message);
                    }
                }
                mat.Flags[var_index, time_index] = TimeVarientFlag.Individual;
                mat.Multipliers[var_index, time_index] = TypeConverterEx.ChangeType<float>(multiplier);
                mat.IPRN[var_index, time_index] = iprn;
            }
        } 
        public void WriteSerialFloatArray(StreamWriter sw, MyVarient3DMat<float> mat, int var_index, int time_index, string format, string comment)
        {
            if (mat.Flags[var_index,time_index] == TimeVarientFlag.Constant)
            {
                string line = string.Format("CONSTANT\t{0}\t{1}", mat.Constants[var_index,time_index], comment);
                sw.WriteLine(line);
            }
            else if (mat.Flags[var_index,time_index] == TimeVarientFlag.Repeat)
            {

            }
            else if (mat.Flags[var_index,time_index] == TimeVarientFlag.Individual)
            {
                string line = string.Format("INTERNAL\t{0}\t(FREE)\t{1}\t{2}", mat.Multipliers[var_index,time_index], mat.IPRN[var_index,time_index], comment);
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
                            vv = mat.Value[var_index][time_index][index];
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
        public void WriteRegularArray<T>(StreamWriter sw, MyVarient3DMat<T> mat, int var_index, string format, string comment)
        {
            if (mat.Flags[var_index,0] == TimeVarientFlag.Constant)
            {
                string line = string.Format("CONSTANT\t{0}\t{1}", mat.Constants[var_index,0], comment);
                sw.WriteLine(line);
            }
            else if (mat.Flags[var_index,0] == TimeVarientFlag.Repeat)
            {

            }
            else if (mat.Flags[var_index,0] == TimeVarientFlag.Individual)
            {
                string line = string.Format("INTERNAL\t{0}\t(FREE)\t{1}\t{2}", mat.Multipliers[var_index,0], mat.IPRN[var_index,0], comment);
                var grid = Owner.Grid as MFGrid;
                int row = grid.RowCount;
                int col = grid.ColumnCount;

                sw.WriteLine(line);
                for (int r = 0; r < row; r++)
                {
                    line = string.Join(StreamReaderSequence.stab, mat.Value[var_index][r]);
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
    }
}
