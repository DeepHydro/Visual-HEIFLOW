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
        private const string _ID_COL_NAME = "ID";
        public event EventHandler<int> Processing;
        public event EventHandler<string> Processed;
        protected IPackage _Package;
        private Dictionary<string, int> _RowIndex;
        private Dictionary<string, int> _ColIndex;

        public PackageCoverage()
        {
            LegendText = "New_Coverage";
            FieldName = "";
            State = ModelObjectState.Standby;
            _RowIndex = new Dictionary<string, int>();
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

        public string GetValue(string col_name, string row_id)
        {
            if (_ColIndex.Keys.Contains(col_name) && _RowIndex.Keys.Contains(row_id))
                return LookupTable.Rows[_RowIndex[row_id]][col_name].ToString();
            else
                return ZonalStatastics.NoDataValueString;
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
                LookupTable<string> lt = new LookupTable<string>();
                lt.NoValue = "-9999";
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
                            dr[ap.AliasName] = ap.DefaultValue.ToString();
                        }
                    }
                    _RowIndex.Add(lt.RowNames[i], i);
                    LookupTable.Rows.Add(dr);
                }
            }
        }

        private void NewLookupTable()
        {
            LookupTable = new DataTable();
            DataColumn dc_id = new DataColumn(_ID_COL_NAME, typeof(string));
            LookupTable.Columns.Add(dc_id);
            _ColIndex.Clear();
            int i = 0;
            foreach (var ap in ArealProperties)
            {
                DataColumn dc = new DataColumn(ap.AliasName, typeof(string));
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
                    string id = dt.Rows[i][FieldName].ToString();
                    var dr = LookupTable.NewRow();
                    dr[_ID_COL_NAME] = id;
                    foreach (var ap in ArealProperties)
                    {
                        dr[ap.AliasName] = ap.DefaultValue.ToString();
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
                    var id =uval.ElementAt(i).ToString();
                    dr[_ID_COL_NAME] = id;
                    foreach (var ap in ArealProperties)
                    {
                        dr[ap.AliasName] = ap.DefaultValue.ToString();
                    }
                    _RowIndex.Add(id, i);
                    LookupTable.Rows.Add(dr);
                }
            }
        }
        public void SaveLookupTable()
        {
            StreamWriter sw = new StreamWriter(FullLookupTableFileName);
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
            LookupTable<string> lt = new LookupTable<string>();
            lt.FromTextFile(filename);
            lt.NoValue = "-9999";
            for (int i = 0; i < LookupTable.Rows.Count; i++)
            {
                var dr = LookupTable.Rows[i];
                var id = dr[_ID_COL_NAME].ToString();
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

        public virtual void Clear()
        {
            if (File.Exists(this.FullLookupTableFileName))
            {
                File.Delete(this.FullLookupTableFileName);
            }
        }
    }
}
