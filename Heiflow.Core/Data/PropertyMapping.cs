// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;

namespace Heiflow.Core.Data
{
    public class PropertyMapping<T>
    {
        public Dictionary<string, T[]> Mappings { get; set; }
        public Dictionary<string, string> Variables { get; private set; }
        private string[] mVarNames;
        private string[] mIDs;
        private T[,] mVarValues;

        public PropertyMapping(string[] ids, string[] var, T[,] values)
        {
            mVarNames = var;
            mVarValues = values;
            mIDs = ids;
            Build();
        }

        public PropertyMapping(string[] ids, string var, T[] values)
        {
            mVarNames = new string[] { var };
            mVarValues = new T[ids.Length, 1];
            MatrixExtension<T>.AssignColumn(mVarValues, 0, values);
            mIDs = ids;
            Build();
        }

        public int MappingCount { get; private set; }
        public int VariableCount { get; private set; }

        private void Build()
        {
            MappingCount = mIDs.Length;
            VariableCount = mVarNames.Length;
            Mappings = new Dictionary<string, T[]>();
            Variables = new Dictionary<string, string>();

            for (int i = 0; i < VariableCount; i++)
            {
                Variables.Add(mIDs[i], mVarNames[i]);
            }
            for (int i = 0; i < MappingCount; i++)
            {
                Mappings.Add(mIDs[i], MatrixExtension<T>.GetRow(i, mVarValues));
            }
        }

        public static PropertyMapping<T>[] Read(StreamReader sr)
        {
            //read head line that tells mapping count, variable names
            string line = sr.ReadLine();
            var strs = TypeConverterEx.Split<string>(line, 1);
            int layer =  int.Parse(strs[0]);
            PropertyMapping<T> []maps=new PropertyMapping<T>[layer];

            for (int l = 0; l < layer; l++)
            {
                line = sr.ReadLine();
                strs = TypeConverterEx.Split<string>(line, 1);
                int num = int.Parse(strs[0]);
                line = sr.ReadLine();
                var varnam = TypeConverterEx.SkipSplit<string>(line, 1);
                var ids = new string[num];
                var values = new T[num, varnam.Length];
                for (int i = 0; i < num; i++)
                {
                    line = sr.ReadLine();
                    strs = TypeConverterEx.Split<string>(line);
                    ids[i] = strs[0];
                    var row = TypeConverterEx.SkipSplit<T>(line, 1);
                    MatrixExtension<T>.AssignRow(values, i, row);
                }
                PropertyMapping<T> map = new PropertyMapping<T>(ids, varnam, values);
                maps[l] = map;
            }
            return maps; 
        }

        public static PropertyMapping<float>[] Read(string excelfile, string sheet, int nlayer)
        {
            FileStream stream = File.Open(excelfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelReader.IsFirstRowAsColumnNames = true;
            System.Data.DataSet result = excelReader.AsDataSet();
            var dt = result.Tables[sheet];
            var dtEnum = dt.AsEnumerable();
            int num = dt.Rows.Count;
            PropertyMapping<float>[] maps = new PropertyMapping<float>[nlayer];
            for (int l = 0; l < nlayer; l++)
            {
                var ids = from r in dtEnum select r.Field<double>("ID" + (l + 1)).ToString();
                string[] varnam = new string[5];
                var values = new float[num, varnam.Length];
                for (int i = 1; i < 6; i++)
                {
                    varnam[i - 1] = dt.Columns[l * 6 + i].ColumnName;
                    var colvec = from r in dtEnum select float.Parse(r.Field<double>(varnam[i - 1]).ToString());
                    MatrixExtension<float>.AssignColumn(values, i-1, colvec.ToArray());
                }
                PropertyMapping<float> map = new PropertyMapping<float>(ids.ToArray(), varnam, values);
                maps[l] = map;
            }

            return maps; 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappedMatrix"></param>
        /// <param name="variableIndex">starting from 0</param>
        public void Map(float[,] mappedMatrix, int variableIndex)
        {
            int row = mappedMatrix.GetLength(0);
            int col = mappedMatrix.GetLength(1);
            var map = Mappings;

            if (map != null)
            {
                var line = map.Keys.Cast<string>().ToArray();
                float[] ids = TypeConverterEx.ChangeType<float>(line);
                var values = (from dic in map select float.Parse(dic.Value[variableIndex].ToString())).ToArray(); 
                
                for (int r = 0; r < row; r++)
                {
                    for (int c = 0; c < col; c++)
                    {
                        int t = 0;
                        foreach(var kk in ids)
                        {
                            if(mappedMatrix[r,c] == kk)
                            {
                                mappedMatrix[r, c] = values[t];
                                break;
                            }
                            t++;
                        }
                        if(t == ids.Count())
                        {
                            mappedMatrix[r, c] = 9999;
                        }
                    }
                }
            }
        }
    
    }
}
