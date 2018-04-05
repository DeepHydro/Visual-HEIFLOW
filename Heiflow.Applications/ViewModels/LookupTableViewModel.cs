// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications.Views;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Waf.Applications;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using Heiflow.Core.Data;
using System.IO;

namespace Heiflow.Applications.ViewModels
{
    [Export]
    public class LookupTableViewModel : ViewModel<ILookupTableView>
    {
        private IWindowService _WindowService;
        private PackageCoverage _Coverage;
        private IProjectService _ProjectService;
        private IShellService _ShellService;
        [ImportingConstructor]
        public LookupTableViewModel(ILookupTableView view, IWindowService win, IProjectService prj, IShellService shell)
            : base(view)
        {
            _WindowService = win;
            _ProjectService = prj;
            _ShellService = shell;
        }


        public IProjectService ProjectService
        {
            get
            {
                return _ProjectService;
            }
        }

        public IShellService ShellService
        {
            get
            {
                return _ShellService;
            }
        }

        public PackageCoverage Coverage
        {
            get
            {
                return _Coverage;
            }
            set
            {
                _Coverage = value;
                //LookupTable = Derieve(_Coverage);
                OnPropertyChanged(new PropertyChangedEventArgs("Coverage"));
            }
        }

        //public DataTable LookupTable
        //{
        //    get
        //    {
        //        return _LookupTable;
        //    }
        //    set
        //    {
        //        _LookupTable = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("LookupTable"));
        //    }
        //}

        public void ShowView()
        {
            if (ViewCore == null)
            {
                var view = _WindowService.NewParameterTableWindow();
                view.ShowView(null);
            }
            else
            {
                ViewCore.ShowView(null);
            }
        }

        public void ImportFrom(string excel, DataTable source)
        {
            if (source != null && source.Columns.Contains("ID"))
            {
                var mp = new LookupTable<float>();
                mp.NoValue = -1;
                mp.FromTextFile(excel);
                if (mp != null)
                {
                    foreach (DataRow dr in source.Rows)
                    {
                        var id = dr["ID"].ToString();
                        foreach (DataColumn dc in source.Columns)
                        {
                            if (dc.ColumnName != "ID")
                            {
                                var buf = mp.GetValue(dc.ColumnName, id);
                                if (buf != mp.NoValue)
                                    dr[dc] = buf;
                            }
                        }
                    }
                    source.AcceptChanges();
                }
            }
        }

        private DataTable Derieve(PackageCoverage coverage)
        {
            if (coverage is FeatureCoverage)
            {
                var lm = coverage as FeatureCoverage;
                var dt = (lm.Source as FeatureSet).DataTable;
                List<DataColumn> new_dcs = new List<DataColumn>();
                foreach (var ap in lm.ArealProperties)
                {
                    if (!dt.Columns.Contains(ap.PropertyName))
                    {
                        DataColumn dc = new DataColumn(ap.PropertyName, Type.GetType(ap.TypeName));
                        dt.Columns.Add(dc);
                        dc.DefaultValue = ap.DefaultValue;
                        new_dcs.Add(dc);
                    }
                }

                if (!dt.Columns.Contains("ID"))
                {
                    DataColumn dc = new DataColumn("ID", Type.GetType("System.Int32"));
                    dt.Columns.Add(dc);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["ID"] = i + 1;
                    }
                }
                if (new_dcs.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        foreach (var dc in new_dcs)
                        {
                            dr[dc.ColumnName] = dc.DefaultValue;
                        }
                    }
                }
                //lm.Source.Save();
                return dt;
            }
            else
            {
                bool outov = false;
                var raster = coverage as RasterCoverage;
                bool require_newfile = true;
                DataTable dt = null;
                if (File.Exists(raster.FullLookupTableFileName))
                {
                    LookupTable<double> mt = new LookupTable<double>();
                    mt.FromTextFile(raster.FullLookupTableFileName);
                    dt = mt.ToDataTable();
                    require_newfile = false;

                    foreach (var ap in raster.ArealProperties)
                    {
                        var buf = mt.ColNames.Where(p => p == ap.PropertyName);
                        if (buf.Count() == 0)
                        {
                            require_newfile = true;
                            break;
                        }
                    }
                    if(!require_newfile)
                    {
                        //raster.Converter = mt;
                    }
                }
                if (require_newfile)
                {
                    var para_names = from pr in raster.ArealProperties select pr.PropertyName;
                    var uval = RasterEx.GetUniqueValues(raster.Source as IRaster, 100, out outov);
                    var uid = from uu in uval select uu.ToString();
                    var default_values = from pr in raster.ArealProperties select double.Parse(pr.DefaultValue.ToString());
                    var mapping = new LookupTable<double>(para_names.ToArray(), uid.ToArray(), default_values.ToArray());
                    mapping.Save(raster.FullLookupTableFileName);
                    dt = mapping.ToDataTable();
                    //raster.Converter = mapping;
                }

                return dt;
            }
        }
    }
}