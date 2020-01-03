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
            List<IDendritiRecord<ObservationSeries>> records = new List<IDendritiRecord<ObservationSeries>>();
            if (_Series != null)
            {
                var group = from re in _Series group re by re.Site.State into vv_cat select new { cat = vv_cat.Key, series = vv_cat };
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
        private List<IDendritiRecord<ObservationSeries>> GroupByVariable()
        {       
            List<IDendritiRecord<ObservationSeries>> records = new List<IDendritiRecord<ObservationSeries>>();
            if (_Series != null)
            {
                var group = from re in _Series group re by re.Variable.GeneralCategory into vv_cat select new { cat = vv_cat.Key, series = vv_cat };
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
            }
            return records;

        }
    }
}