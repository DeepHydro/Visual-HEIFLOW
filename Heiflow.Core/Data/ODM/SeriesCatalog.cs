// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.ODM
{
    public enum GroupMethod { SiteCategory, SiteState, VariableMedium, DataTable }
    public class SeriesCatalog
    {
        private ODMSource _ODM;
        private IEnumerable<ObservationSeries> _Series;

        public SeriesCatalog(ODMSource odm)
        {
            _ODM = odm;
            GroupMethod = GroupMethod.SiteCategory;
        }

        public IEnumerable<ObservationSeries> Series
        {
            get
            {
                return _Series;
            }
        }

        public GroupMethod GroupMethod
        {
            get;
            set;
        }


        public ODMSource ODM
        {
            get
            {
                return _ODM;
            }
        }

        public void Retrieve()
        {
            _Series = _ODM.GetSeriesCatalog();
        }

        public List<IDendritiRecord<ObservationSeries>> Group()
        {
            List<IDendritiRecord<ObservationSeries>> list = null;
            switch (GroupMethod)
            {
                case GroupMethod.SiteCategory:
                    list = GroupBySiteCategory();
                    break;
                case GroupMethod.SiteState:
                    list = GroupBySiteState();
                    break;
                case GroupMethod.VariableMedium:
                    list = GroupByVariable();
                    break;
            }
            return list;
        }
        private List<IDendritiRecord<ObservationSeries>> GroupBySiteCategory()
        {
            List<IDendritiRecord<ObservationSeries>> records = new List<IDendritiRecord<ObservationSeries>>();

            if (_Series != null)
            {
                var group = from re in _Series group re by re.Site.SiteType into vv_cat select new { cat = vv_cat.Key, series = vv_cat };

                foreach (var gp in group)
                {
                    DendritiRecord<ObservationSeries> root = new DendritiRecord<ObservationSeries>()
                    {
                        Level = 0,
                        Name = gp.cat,
                        Parent = null,
                        Value = null,
                        Tag = null,
                        CanDelete = false,
                        CanExport2Excel = false,
                        CanExport2Shp = false
                    };

                    var uni_sites = gp.series.GroupBy(g => g.SiteID).Select(o => o.First());
                    foreach (var site in uni_sites)
                    {
                        DendritiRecord<ObservationSeries> site_record = new DendritiRecord<ObservationSeries>()
                        {
                            Level = 1,
                            Name = site.Site.Name,
                            Parent = root,
                            Value = null,
                            Tag = site.Site,
                            CanDelete = true,
                            CanExport2Excel = true,
                            CanExport2Shp = true
                        };
                        root.Children.Add(site_record);

                        var site_vars = from ss in _Series where ss.SiteID == site.SiteID select ss;
                        foreach (var site_var in site_vars)
                        {
                            DendritiRecord<ObservationSeries> variable_record = new DendritiRecord<ObservationSeries>()
                            {
                                Level = 2,
                                Name = site_var.Variable.Name,
                                Parent = site_record,
                                Value = site_var,
                                Tag = site_var,
                                CanDelete = true,
                                CanExport2Excel = true,
                                CanExport2Shp = true
                            };
                            site_record.Children.Add(variable_record);
                        }
                    }
                    records.Add(root);
                }
            }

            return records;

        }
        private List<IDendritiRecord<ObservationSeries>> GroupBySiteState()
        {
            var group = from re in _Series group re by re.Site.State into vv_cat select new { cat = vv_cat.Key, series = vv_cat };
            List<IDendritiRecord<ObservationSeries>> records = new List<IDendritiRecord<ObservationSeries>>();

            foreach (var gp in group)
            {
                DendritiRecord<ObservationSeries> root = new DendritiRecord<ObservationSeries>()
                {
                    Level = 0,
                    Name = gp.cat,
                    Parent = null,
                    Value = null,
                    Tag = null,
                    CanDelete = false,
                    CanExport2Excel = false,
                    CanExport2Shp = false
                };

                var uni_sites = gp.series.GroupBy(g => g.SiteID).Select(o => o.First());
                foreach (var site in uni_sites)
                {
                    DendritiRecord<ObservationSeries> site_record = new DendritiRecord<ObservationSeries>()
                    {
                        Level = 1,
                        Name = site.Site.Name,
                        Parent = root,
                        Value = null,
                        Tag = site.Site,
                        CanDelete = true,
                        CanExport2Excel = true,
                        CanExport2Shp = true
                    };
                    root.Children.Add(site_record);

                    var site_vars = from ss in _Series where ss.SiteID == site.SiteID select ss;
                    foreach (var site_var in site_vars)
                    {
                        DendritiRecord<ObservationSeries> variable_record = new DendritiRecord<ObservationSeries>()
                        {
                            Level = 2,
                            Name = site_var.Variable.Name,
                            Parent = site_record,
                            Value = site_var,
                            Tag = site_var,
                            CanDelete = true,
                            CanExport2Excel = true,
                            CanExport2Shp = true
                        };
                        site_record.Children.Add(variable_record);
                    }
                }
                records.Add(root);
            }

            return records;

        }
        private List<IDendritiRecord<ObservationSeries>> GroupByVariable()
        {
            var group = from re in _Series group re by re.Variable.GeneralCategory into vv_cat select new { cat = vv_cat.Key, series = vv_cat };
            List<IDendritiRecord<ObservationSeries>> records = new List<IDendritiRecord<ObservationSeries>>();

            foreach (var gp in group)
            {
                DendritiRecord<ObservationSeries> root = new DendritiRecord<ObservationSeries>()
                {
                    Level = 0,
                    Name = gp.cat,
                    Parent = null,
                    Value = null,
                    Tag = null,
                    CanDelete = false,
                    CanExport2Excel=false,
                    CanExport2Shp=false
                };

                var uni_variables = gp.series.GroupBy(g => g.Variable.Name).Select(o => o.First());
                foreach (var variable in uni_variables)
                {
                    DendritiRecord<ObservationSeries> site_record = new DendritiRecord<ObservationSeries>()
                    {
                        Level = 1,
                        Name = variable.Variable.Name,
                        Parent = root,
                        Value = null,
                        Tag = variable.Variable,
                        CanDelete = true,
                        CanExport2Excel = true,
                        CanExport2Shp = true
                    };
                    root.Children.Add(site_record);

                    var site_vars = from ss in _Series where ss.Variable.ID == variable.Variable.ID select ss;
                    foreach (var site_var in site_vars)
                    {
                        DendritiRecord<ObservationSeries> variable_record = new DendritiRecord<ObservationSeries>()
                        {
                            Level = 2,
                            Name = site_var.Site.Name,
                            Parent = site_record,
                            Value = site_var,
                            Tag = site_var,
                            CanDelete = true,
                            CanExport2Excel = true,
                            CanExport2Shp = true
                        };
                        site_record.Children.Add(variable_record);
                    }
                }
                records.Add(root);
            }

            return records;

        }
    }
}