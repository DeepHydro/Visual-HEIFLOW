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
using Heiflow.Core.MyMath;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Spatial.SpatialAnalyst;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Parameters
{
    [Serializable]
    public abstract class PackageCoverage:IPackageCoverage
    {
        public static  string HRUID_NAME = "HRU_ID";
        public static  string ZONEID_NAME = "ZONE_ID";
        public static  string LAYERID_NAME = "LAYER_ID";
        private const string _ID_COL_NAME = "ID";
        public event EventHandler<int> Processing;
        public event EventHandler<string> Processed;
        protected IPackage _Package;
        private Dictionary<double, int> _RowIndex;
        private Dictionary<string, int> _ColIndex;

        public PackageCoverage()
        {
            LegendText = "New_Coverage";
            FieldName = "";
            State = ModelObjectState.Standby;
            _RowIndex = new Dictionary<double, int>();
            _ColIndex = new Dictionary<string, int>();
            AveragingMethod = AveragingMethod.PseudoMedian;
        }

        [XmlArrayItem]
        public List<ArealPropertyInfo> ArealProperties
        {
            get;
            set;
        }

        [XmlElement]
        public string LegendText
        {
            get;
            set;
        }
        [XmlElement]
        public string ID
        {
            get;
            set;
        }

        [XmlElement]
        public string PackageName
        {
            get;
            set;
        }
        /// <summary>
        /// layer index starting from 0
        /// </summary>
        [XmlElement]
        public int GridLayer
        {
            get;
            set;
        }
        [XmlElement]
        public string FieldName
        {
            get;
            set;
        }
        /// <summary>
        /// Full filename of the coverage file
        /// </summary>
        [XmlIgnore]
        public string FullCoverageFileName
        {
            get
            {
                if (DirectoryHelper.IsRelativePath(CoverageFilePath))
                    return Path.GetFullPath(Path.Combine(ModelService.ProjectDirectory, CoverageFilePath));
                else
                    return CoverageFilePath;
            }
        }
        /// <summary>
        /// Relative filename of the coverage file
        /// </summary>
        [XmlElement]
        public string CoverageFilePath
        {
            get;
            set;
        }
        /// <summary>
        /// Full filename of the lookup table
        /// </summary>
         [XmlIgnore]
         public string FullLookupTableFileName
         {
             get
             {
                 if (DirectoryHelper.IsRelativePath(LookupTableFilePath))
                     return Path.GetFullPath(Path.Combine(ModelService.ProjectDirectory, LookupTableFilePath));
                 else
                     return LookupTableFilePath;
             }
         }
        /// <summary>
         /// Relative filename of the lookup table
        /// </summary>
         [XmlElement]
         public string LookupTableFilePath
         {
             get;
             set;
         }

        [XmlIgnore]
        public IPackage Package
        {
            get
            {
                return _Package;
            }
            set
            {
                _Package = value;
                _Package.Coverage = this;
            }
        }

        [XmlIgnore]
        public IFeatureSet TargetFeatureSet
        {
            get;
            set;
        }
        [XmlIgnore]
        public IDataSet Source
        {
            get;
            set;
        }

        [XmlIgnore]
        public DataTable LookupTable
        {
            get;
            set;
        }
         [XmlIgnore]
        public ModelObjectState State
        {
            get;
            set;
        }
          [XmlIgnore]
         public AveragingMethod AveragingMethod
         {
             get;
             set;
         }
          public bool UseDefaultValue
          {
              get;
              set;
          }

        protected virtual void OnProcessing(int percent)
        {
            if (Processing != null)
                Processing(this, percent);
        }

        protected virtual void OnProcessed(string arg)
        {
            if (Processed != null)
                Processed(this, arg);
        }

        public float GetValue(string col_name, double row_id)
        {
            //if (_ColIndex.Keys.Contains(col_name) && _RowIndex.Keys.Contains(row_id))
            //    return LookupTable.Rows[_RowIndex[row_id]][col_name].ToString();
            //else
            //    return ZonalStatastics.NoDataValueString;
            if (_ColIndex.Keys.Contains(col_name) && _RowIndex.Keys.Contains(row_id))
                return float.Parse(LookupTable.Rows[_RowIndex[row_id]][col_name].ToString());
            else
                return ZonalStatastics.NoDataValue;
        }
        public string GetValue(string col_name, int row_index)
        {
            if (_ColIndex.Keys.Contains(col_name))
                return LookupTable.Rows[row_index][col_name].ToString(); 
            else
                return ZonalStatastics.NoDataValueString;
        }

        public void LoadLookTable()
        {
            if (File.Exists(FullLookupTableFileName))
            {
                NewLookupTable();
                _RowIndex.Clear();
                LookupTable<float> lt = new LookupTable<float>();
                lt.NoValue = -9999;
                lt.FromTextFile(FullLookupTableFileName);
                for (int i = 0; i < lt.RowNames.Length; i++)
                {
                    var dr = LookupTable.NewRow();
                    dr[_ID_COL_NAME] = lt.RowNames[i];
                    foreach (var ap in ArealProperties)
                    {
                        if (lt.ColNames.Contains(ap.AliasName))
                        {
                            dr[ap.AliasName] = lt.GetValue(ap.AliasName, i);
                        }
                        else
                        {
                            dr[ap.AliasName] = ap.DefaultValue;
                        }
                    }
                    _RowIndex.Add(double.Parse(lt.RowNames[i]), i);
                    LookupTable.Rows.Add(dr);
                }
            }
        }

        private void NewLookupTable()
        {
            LookupTable = new DataTable();
            DataColumn dc_id = new DataColumn(_ID_COL_NAME, typeof(double));
            LookupTable.Columns.Add(dc_id);
            _ColIndex.Clear();
            int i = 0;
            foreach (var ap in ArealProperties)
            {
                DataColumn dc = new DataColumn(ap.AliasName, typeof(float));
                LookupTable.Columns.Add(dc);
                _ColIndex.Add(ap.AliasName, i);
                i++;
            }
        }
        public void InitLookupTable()
        {
            NewLookupTable();
            _RowIndex.Clear();
            if (Source is FeatureSet)
            {
                var dt = (Source as FeatureSet).DataTable;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var id = double.Parse(dt.Rows[i][FieldName].ToString());
                    var dr = LookupTable.NewRow();
                    dr[_ID_COL_NAME] = id;
                    foreach (var ap in ArealProperties)
                    {
                        dr[ap.AliasName] = ap.DefaultValue;
                    }
                    _RowIndex.Add(id, i);
                    LookupTable.Rows.Add(dr);
                }
            }
            else
            {
                bool outov = false;
                var uval = RasterEx.GetUniqueValues(Source as IRaster, 100, out outov);
                var uid = from uu in uval select uu.ToString();
                for (int i = 0; i < uval.Count; i++)
                {
                    var dr = LookupTable.NewRow();
                    var id = double.Parse(uval.ElementAt(i).ToString());
                    dr[_ID_COL_NAME] = id;
                    foreach (var ap in ArealProperties)
                    {
                        dr[ap.AliasName] = ap.DefaultValue;
                    }
                    _RowIndex.Add(id, i);
                    LookupTable.Rows.Add(dr);
                }
            }
        }
        public void SaveLookupTable()
        {
            SaveLookupTable(FullLookupTableFileName);
        }

        public void SaveLookupTable(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            var buf = from DataColumn dc in LookupTable.Columns select dc.ColumnName;
            string cols = string.Join(",", buf);
            string line = "";
            sw.WriteLine(cols);
            string[] strs = new string[LookupTable.Columns.Count];
            for (int i = 0; i < LookupTable.Rows.Count; i++)
            {
                for (int j = 0; j < LookupTable.Columns.Count; j++)
                {
                    strs[j] = LookupTable.Rows[i][j].ToString();
                }
                line = string.Join(",", strs);
                sw.WriteLine(line);
            }
            sw.Close();
        }

        public void ImportFrom(string filename)
        {
            LookupTable<float> lt = new LookupTable<float>();
            lt.FromTextFile(filename);
            lt.NoValue = -9999;
            for (int i = 0; i < LookupTable.Rows.Count; i++)
            {
                var dr = LookupTable.Rows[i];
                var id = double.Parse(dr[_ID_COL_NAME].ToString());
                for (int j = 1; j < LookupTable.Columns.Count; j++)
                {
                    var col = LookupTable.Columns[j].ColumnName;
                    if (lt.ColNames.Contains(col))
                    {
                        dr[j] = lt.GetValue(col, id);
                    }
                }
            }
        }

        public abstract void Map();
        /// <summary>
        /// map parameters by zone id and lookup table
        /// </summary>
        /// <param name="zone_tb"> [Layer ID, HRU ID, Zone ID]</param>
        /// <param name="lookup_table">[Para Name][Layer ID, Zone ID, Para Value]</param>
        public void MapByZone(Tuple<int,int,int>[] zone_tb, Dictionary<string, Tuple<int, int, float>[]> lookup_table)
        {
            int progress = 0;
            int index_ap = 0;
            try
            {
                OnProcessing(progress);
                var ncell = zone_tb.Length;
                foreach (var ap in ArealProperties)
                {
                    if (ap.IsParameter )
                    {
                        if (lookup_table.Keys.Contains(ap.ParameterName))
                        {
                            for (int i = 0; i < ncell; i++)
                            {
                                var vv = GetMappedValue(lookup_table, zone_tb[i].Item1, zone_tb[i].Item3, ap.ParameterName);
                                if (vv != ZonalStatastics.NoDataValue)
                                    ap.Parameter.SetValue(0, zone_tb[i].Item2 - 1, 0, vv);
                                else
                                    ap.Parameter.SetValue(0, zone_tb[i].Item2 - 1, 0, ap.DefaultValue);
                            }
                        }
                    }
                    else
                    {
                        if (lookup_table.Keys.Contains(ap.PropertyName))
                        {
                            var mat = Package.GetType().GetProperty(ap.PropertyName).GetValue(Package) as DataCube<float>;
                            if (mat != null)
                            {
                                for (int i = 0; i < ncell; i++)
                                {
                                    var vv = GetMappedValue(lookup_table, zone_tb[i].Item1, zone_tb[i].Item3, ap.PropertyName);
                                    if (vv != ZonalStatastics.NoDataValue && mat.IsAllocated(zone_tb[i].Item1 - 1))
                                        mat[zone_tb[i].Item1 - 1, 0, zone_tb[i].Item2 - 1] = vv;
                                    else
                                        mat[zone_tb[i].Item1 - 1, 0, zone_tb[i].Item2 - 1] = (float)ap.DefaultValue;
                                }
                            }
                        }
                    }
                    progress = (int)(index_ap * 100.0 / ArealProperties.Count);
                    OnProcessing(progress);
                    index_ap++;
                }
                OnProcessing(100);
                OnProcessed(ConstantWords.Successful);
            }
            catch (Exception ex)
            {
                OnProcessing(100);
                OnProcessed("Failed. Error message: " + ex.Message);
            }
        }
      
        public float GetMappedValue(Dictionary<string, Tuple<int, int, float>[]> lookup_table, int layer_id,int zone_id,string paraname)
        {
            var buf = from rc in lookup_table[paraname] where rc.Item1 == layer_id && rc.Item2 == zone_id select rc;
            if (buf.Any())
                return buf.First().Item3;
            else
                return ZonalStatastics.NoDataValue;
        }
        /// <summary>
        /// Zone Table is converted as Tuple array. A tuple has three items: [Layer ID, HRU ID, Zone ID]
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Tuple<int,int,int>[] ConvertZoneTable(DataTable dt)
        {
            var nrow= dt.Rows.Count;
            var records = new Tuple<int, int, int>[nrow];
            for (int i = 0; i < nrow;i++ )
            {
                var dr= dt.Rows[i];
                records[i] = new Tuple<int, int, int>(int.Parse(dr[LAYERID_NAME].ToString()), int.Parse(dr[HRUID_NAME].ToString()), int.Parse(dr[ZONEID_NAME].ToString()));
            }
            return records;
        }
        /// <summary>
        /// Lookup table is converted as a dictionary. The key is Parameter Name, the value is a Tuple[Layer ID, Zone ID, Para Value]
        /// </summary>
        /// <param name="dt">The first and second columns of the datatable are Layer ID and Zone ID</param>
        /// <returns></returns>
        public Dictionary<string, Tuple<int, int, float>[]> ConvertLookupTable(DataTable dt)
        {
            var nrow = dt.Rows.Count;
            var npara = dt.Columns.Count - 2;
            var nrecord = nrow * npara;
            var records = new Dictionary<string, Tuple<int, int, float>[]>();
            for (int i = 2; i < dt.Columns.Count; i++)
            {
                var paraname = dt.Columns[i].ColumnName;
                records.Add(paraname, new Tuple<int, int, float>[nrow]);
                for (int j = 0; j < nrow; j++)
                {
                    var dr = dt.Rows[j];
                    records[paraname][j] = new Tuple<int, int, float>(int.Parse(dr[LAYERID_NAME].ToString()), int.Parse(dr[ZONEID_NAME].ToString()), float.Parse(dr[paraname].ToString()));
                }
            }
            return records;
        }

        public virtual void Clear()
        {
            if (File.Exists(this.FullLookupTableFileName))
            {
                File.Delete(this.FullLookupTableFileName);
            }
        }
    }

 
}
