// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data.Database;
using Heiflow.Core.MyMath;
using Heiflow.Core.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Heiflow.Core.Data.ODM
{
    [Serializable]
    public class ODMSource : DatabaseSource
    {
        private Dictionary<string, IODMTable> _ODMTables ;
        private Dictionary<int, Site> _sites;
        private Dictionary<int, Variable> _variables;

        public ODMSource()
        {
            _variables = new Dictionary<int, Variable>();
            _sites = new Dictionary<int, Site>();
            _ODMTables = new Dictionary<string, IODMTable>();

            SitesTable sites = new SitesTable()
            {
                ODM = this
            };
            _ODMTables.Add(sites.TableName, sites);

            VariablesTable variables = new VariablesTable()
            {
                ODM = this
            };
            _ODMTables.Add(variables.TableName, variables);

            DataValuesTable dataValues = new DataValuesTable()
            {
                ODM = this
            };
            _ODMTables.Add(dataValues.TableName, dataValues);

            CVTable DataTypeCV = new CVTable("DataTypeCV")
            {
                ODM = this
            };
            _ODMTables.Add(DataTypeCV.TableName, DataTypeCV);
            CVTable SampleMediumCV = new CVTable("SampleMediumCV")
            {
                ODM = this
            };
            _ODMTables.Add(SampleMediumCV.TableName, SampleMediumCV);
            CVTable SpeciationCV = new CVTable("SpeciationCV")
            {
                ODM = this
            };
            _ODMTables.Add(SpeciationCV.TableName, SpeciationCV);
            CVTable ValueTypeCV = new CVTable("ValueTypeCV")
            {
                ODM = this
            };
            _ODMTables.Add(ValueTypeCV.TableName, ValueTypeCV);
            CVTable VariableNameCV = new CVTable("VariableNameCV")
            {
                ODM = this
            };
            _ODMTables.Add(VariableNameCV.TableName, VariableNameCV);
        }
        [XmlElement]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// relative file path
        /// </summary>
        [XmlElement]
        public string DatabaseFilePath
        {
            get;
            set;
        }
        [XmlIgnore]
        public IDBase ODMDB { get; private set; }

         [XmlIgnore]
        public Dictionary<string, IODMTable> ODMTables
        {
            get
            {
                return _ODMTables;
            }
        }
        [XmlIgnore]
         public string WorkDirectory
         {
             get;
             set;
         }


         public override bool Open()
         {
             string dbpath = DatabaseFilePath;
             string msg = "";
             if(DirectoryHelper.IsRelativePath(DatabaseFilePath))
             {
                 dbpath = Path.Combine(WorkDirectory, DatabaseFilePath);
             }
             return Open(DatabaseFilePath,ref msg);
         }

         public override bool Open(string dbpath, ref string msg)
         {
             try
             {
                 DBaseFactory factory = new DBaseFactory();
                 DBConnectInfo info = new DBConnectInfo()
                 {
                     DataSource = dbpath
                 };
                 ODMDB = factory.CreateInitialDbClass(DBkind.Access2013, DbOptKind.Oledb, info);
                 if (ODMDB.ConnectionState == ConnectionState.Closed)
                     ODMDB.DbConnection.Open();
                 DatabaseFilePath = dbpath;
                 Name = Path.GetFileNameWithoutExtension(dbpath);
                 msg = "successful";
                 return true;
             }
             catch(Exception ex)
             {
                 msg = ex.Message;
                 return false;
             }
         }

        public override void Close()
        {
            if (ODMDB != null && ODMDB.DbConnection != null)
                ODMDB.DbConnection.Close();
        }

        public string[] GetDBTableNames()
        {
            string[] names = null;
            string sql = "SELECT MSysObjects.*, MSysObjects.Type FROM MSysObjects where (((MSysObjects.Type)=1)) and (((MSysObjects.flags)=0));";
            var dt = ODMDB.QueryDataTable(sql).AsEnumerable();
            if (dt != null)
            {
                names = (from dr in dt select dr.Field<string>("Name")).ToArray();
            }
            return names;
        }

        public string[] GetODMTableNames()
        {
            return new string[]
            {
                "Sites",
                "Variables",                
                "DataValues",
                "DataTypeCV",
                "SampleMediumCV",
                 "SpeciationCV",
                 "ValueTypeCV",
                "VariableNameCV",
            };
        }

        public Site[] GetSites(QueryCriteria qc)
        {
            string sql = "";
            if (qc==null || qc.VariableName == "" || qc.VariableName == null)
            {
                sql = "select * from Sites";
                var dt = ODMDB.QueryDataTable(sql);
                if (dt != null)
                {
                    var sites = from r in dt.AsEnumerable()
                                select new Site()
                                {
                                    ID = r.Field<int>("SiteID"),
                                    Code = r.Field<string>("SiteCode"),
                                    Elevation = r.Field<double>("Elevation_m"),
                                    Name = r.Field<string>("SiteName"),
                                    Comments = r.Field<string>("Comments"),
                                    Latitude = r.Field<double>("Latitude"),
                                    Longitude = r.Field<double>("Longitude"),
                                    SiteType = r.Field<string>("SiteType"),
                                    State = r.Field<string>("State")
                                   // Variables = GetVariables(r.Field<int>("SiteID"))
                                };
                    return sites.ToArray();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var variable = GetVariable(qc.VariableName);
                if (variable != null)
                {
                    qc.VariableID = variable.ID;
                    sql = string.Format("select * from SeriesCatalog where  VariableID={0} and (BeginDateTime <= #{1}# and EndDateTime >= #{2}#)",
                        variable.ID, qc.End.ToString("yyyy/MM/dd"), qc.Start.ToString("yyyy/MM/dd"));
                    var dt = ODMDB.QueryDataTable(sql);
                    if (dt != null)
                    {
                        List<Site> result = new List<Site>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            int id = int.Parse(dr["SiteID"].ToString());
                            var site = GetSite(id);
                            if (site.Latitude <= qc.BBox.North && site.Latitude >= qc.BBox.South && site.Longitude <= qc.BBox.East
                                 && site.Longitude >= qc.BBox.West)
                                result.Add(site);
                        }
                        return result.ToArray();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public Site[] GetSites(string sql)
        {
            var dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var sites = from r in dt.AsEnumerable()
                            select new Site()
                            {
                                ID = r.Field<int>("SiteID"),
                                Code = r.Field<string>("SiteCode"),
                                Elevation = r.Field<double>("Elevation_m"),
                                Name = r.Field<string>("SiteName"),
                                Comments = r.Field<string>("Comments"),
                                Latitude = r.Field<double>("Latitude"),
                                Longitude = r.Field<double>("Longitude"),
                                SiteType = r.Field<string>("SiteType"),
                                State = r.Field<string>("State")
                                // Variables = GetVariables(r.Field<int>("SiteID"))
                            };
                return sites.ToArray();
            }
            else
            {
                return null;
            }
        }

        public Site GetSite(int id)
        {
            string sql = "select * from Sites where SiteID=" + id;
            var dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var sites = from r in dt.AsEnumerable()
                            select new Site()
                            {
                                ID = r.Field<int>("SiteID"),
                                Code = r.Field<string>("SiteCode"),
                                Elevation = r.Field<double>("Elevation_m"),
                                Name = r.Field<string>("SiteName"),
                                Comments = r.Field<string>("Comments"),
                                Latitude = r.Field<double>("Latitude"),
                                Longitude = r.Field<double>("Longitude"),
                                //  Variables = GetVariables(r.Field<int>("SiteID"))
                            };
                return sites.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public Site GetSite(string name)
        {
            string sql = "select * from Sites where SiteName='" + name + "'";
            var dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var dr = dt.Rows[0];
                var site = new Site()
                {
                    ID = (int)dr["SiteID"],
                    Code = dr["SiteCode"].ToString(),
                    Elevation = (double)dr["Elevation_m"],
                    Name = dr["SiteName"].ToString(),
                    Comments = dr["Comments"].ToString(),
                    Latitude = (double)dr["Latitude"],
                    Longitude = (double)dr["Longitude"],
                    Variables = GetVariables((int)dr["SiteID"])
                };
                return site;
            }
            else
            {
                return null;
            }
        }

        public Variable[] GetVariables(int siteid)
        {
            string sql = string.Format("select * from SeriesCatalog where SiteID={0}", siteid);
            var dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var vv = (from r in dt.AsEnumerable() select r.Field<int>("variableID")).ToArray();
                Variable[] vars = new Variable[vv.Length];
                for (int i = 0; i < vars.Length; i++)
                {
                    vars[i] = GetVariable(vv[i]);
                }
                return vars;
            }
            else
            {
                return null;
            }
        }

        public Variable[] GetVariables()
        {
            string sql = "select * from Variables";
            var dt = ODMDB.QueryDataTable(sql);
            var vars = from dr in dt.AsEnumerable()
                       select new Variable()
            {
                ID = dr.Field<int>("VariableID"),
                Name = dr.Field<string>("VariableName"),
                GeneralCategory = dr.Field<string>("GeneralCategory")
            };
            return vars.ToArray();
        }

        public Variable GetVariable(int varID)
        {
            string sql = string.Format("select * from Variables where variableID={0}", varID);
            var dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var dr = dt.Rows[0];
                var vb = new Variable()
                {
                    ID = (int)dr["variableID"],
                    Code = dr["VariableCode"].ToString(),
                    Name = dr["VariableName"].ToString(),
                };
                return vb;
            }
            else
            {
                return null;
            }
        }

        public Variable GetVariable(string varName)
        {
            string sql = string.Format("select * from Variables where variableName='{0}'", varName);
            var dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var dr = dt.Rows[0];
                var vb = new Variable()
                {
                    ID = (int)dr["variableID"],
                    Code = dr["VariableCode"].ToString(),
                    Name = dr["VariableName"].ToString(),
                };
                return vb;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// retrieve time series
        /// </summary>
        /// <param name="qc">if qc.VariableID ==0, qc.VariableName will be used</param>
        /// <returns></returns>
        public IVectorTimeSeries<double> GetTimeSeries(QueryCriteria qc)
        {
            if (qc.VariableID == 0)
            {
                var variable = GetVariable(qc.VariableName);
                if(variable != null)
                    qc.VariableID = variable.ID;
            }
            string sql = "";
            if (qc.AllTime)
            {
                sql = string.Format("select DateTimeUTC, DataValue from DataValues where SiteID={0} and VariableID={1} order by DateTimeUTC",
                    qc.SiteID, qc.VariableID);
            }
            else
            {
                sql = string.Format("select DateTimeUTC, DataValue from DataValues where SiteID={0} and VariableID={1} and DateTimeUTC >=#{2}# and DateTimeUTC <=#{3}# order by DateTimeUTC",
                    qc.SiteID, qc.VariableID, qc.Start.ToString("yyyy/MM/dd"), qc.End.ToString("yyyy/MM/dd"));
            }
            DataTable dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var dates = from dr in dt.AsEnumerable() select dr.Field<DateTime>("DateTimeUTC");
                var values = from dr in dt.AsEnumerable() select dr.Field<double>("DataValue");
                return new DoubleTimeSeries()
                {
                    DateTimes = dates.ToArray(),
                    Value = values.ToArray()
                };
            }
            else
            {
                return null;
            }
        }

        public DataTable GetValues(ObservationSeries series)
        {
            string sql = string.Format("select DateTimeUTC, DataValue from DataValues where SiteID={0} and VariableID={1} order by DateTimeUTC",
         series.SiteID, series.VariableID);
            DataTable dt = ODMDB.QueryDataTable(sql);
            return dt;
        }

        public DoubleTimeSeries GetTimeSereis(ObservationSeries series)
        {
            string sql = string.Format("select DateTimeUTC, DataValue from DataValues where SiteID={0} and VariableID={1} order by DateTimeUTC",
         series.SiteID, series.VariableID);
            var dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var ds = dt.AsEnumerable();
                var dates = from dr in ds select dr.Field<DateTime>("DateTimeUTC");
                var values = from dr in ds select dr.Field<double>("DataValue");
                DoubleTimeSeries ts = new DoubleTimeSeries(values.ToArray(), dates.ToArray());
                return ts;
            }
            else
            {
                return null;
            }
        }

        public StatisticsInfo GetValueStatistics(ObservationSeries series)
        {
            string sql = string.Format("select DateTimeUTC, DataValue from DataValues where SiteID={0} and VariableID={1} order by DateTimeUTC",
         series.SiteID, series.VariableID);
            var dt = ODMDB.QueryDataTable(sql).AsEnumerable();
            var dates = from dr in dt select dr.Field<DateTime>("DateTimeUTC");
            var values = from dr in dt select dr.Field<double>("DataValue");

            var filtered = from vv in values where vv != 0 select vv;
            var info = MyStatisticsMath.SimpleStatistics(filtered.ToArray());
            return info;
        }

        public DataTable GetDataTable(string table_name, int top_num=2000)
        {
            string sql = string.Format("select top {0} * from {1}", top_num, table_name);
            DataTable dt = ODMDB.QueryDataTable(sql);
            return dt;
        }

        public DataTable Execute(string sql)
        {
            DataTable dt = ODMDB.QueryDataTable(sql);
            return dt;
        }

        public string[] GetKeyWords()
        {
            string sql = " SELECT distinct( Variables.VariableName) FROM Variables INNER JOIN SeriesCatalog ON Variables.VariableID = SeriesCatalog.VariableID";
            DataTable dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                var words = from dr in dt.AsEnumerable() select dr.Field<string>(0);
                return words.ToArray();
            }
            else
                return null;
        }

        public IEnumerable<ObservationSeries> GetSeriesCatalog()
        {
            string sql = "select * from SeriesCatalog";
            var dt = ODMDB.QueryDataTable(sql);
            if (dt != null)
            {
                if (_variables.Count == 0)
                    UpdateVariableList();
                if (_sites.Count == 0)
                    UpdateSiteList();

                var records = (from dr in dt.AsEnumerable()
                               select new ObservationSeries()
                               {
                                   SiteID = dr.Field<int>("SiteID"),
                                   VariableID = dr.Field<int>("VariableID"),
                                   Start = dr.Field<DateTime>("BeginDateTime"),
                                   End = dr.Field<DateTime>("EndDateTime"),
                               }).ToArray();

                for (int i = 0; i < records.Length; i++)
                {
                    var rd = records[i];
                    rd.Site = _sites[rd.SiteID];
                    rd.Variable = _variables[rd.VariableID];
                }
                return records;
            }
            else
            {
                return null;
            }
        }

        public void SaveSite(IObservationsSite s)
        {
            string sql = "";
            sql = string.Format("select * from Sites where SiteID={0}", s.ID);
            if (ODMDB.Exists(sql))
            {
                sql = string.Format("update Sites set SiteName='{1}', SiteCode='{2}',  SiteType='{3}',Longitude={4},Latitude={5},Elevation_m={6}," +
                "State='{7}',Country='{8}',MonitorType={9},Comments='{10}' where SiteID={0}" ,
                   s.ID,  s.Name, s.Code, s.SiteType, s.Longitude, s.Latitude, s.Elevation, s.State, s.Country, s.MonitorType, s.Comments
                    );
            }
            else
            {
                sql = string.Format("insert into Sites (SiteID, SiteName,SiteCode,SiteType,Longitude,Latitude,Elevation_m,State,Country,MonitorType,Comments)" +
             "values ({0}, '{1}', '{2}','{3}', {4}, {5},{6}, '{7}', '{8}','{9}', '{10}')",
                s.ID, s.Name, s.Code, s.SiteType, s.Longitude, s.Latitude, s.Elevation, s.State, s.Country, s.MonitorType, s.Comments);
            }
         
            ODMDB.CreateNonQueryCommand(sql);
        }

        public void SaveSites(IObservationsSite[] sites)
        {
            OnStarted();
            int progress = 0;
            int total = sites.Length;
            int i = 1;
            foreach (var site in sites)
            {
                SaveSite(site);
                progress = i * 100 / total;
                i++;
                OnProgressChanged(progress);
            }
            OnFinished();
        }

        public void SaveVariable(IVariable varb)
        {
            string sql = "";
            sql = string.Format("select * from Variables where VariableID={0}", varb.VariableID);
            if (ODMDB.Exists(sql))
            {
                sql = string.Format("update Variables set VariableCode='{1}',	VariableName='{2}',	Specification='{3}',	ValueType='{4}',	DataType='{5}', SampleMedium='{6}'," +
                "TimeUnitsID={7},	VariableUnitsID={8},	TimeSupport={9},	GeneralCategory='{10}',	NoDataValue='{11}')" +
                " where VariableID={0}" ,
                 varb.VariableID, varb.Code, varb.Name, varb.Specification, varb.ValueType, varb.DataType, varb.SampleMedium, varb.TimeUnitsID,
                       varb.VariableUnitsID, varb.TimeSupport, varb.GeneralCategory, varb.NoDataValue
                 );
            }
            else
            {
                //VariableID	VariableCode	VariableName	Specification	ValueType	DataType	SampleMedium	 TimeUnitsID	VariableUnitsID	TimeSupport	GeneralCategory	NoDataValue
                sql = string.Format("insert into Variables (VariableID	VariableCode	VariableName	Specification	ValueType	DataType	 SampleMedium" +
                "TimeUnitsID	VariableUnitsID	TimeSupport	GeneralCategory	NoDataValue)" +
                    "values ({0}, '{1}', '{2}','{3}', '{4}', '{5}','{6}', {7}, {8},{9}, '{10}',{11})",
                       varb.VariableID, varb.Code, varb.Name, varb.Specification, varb.ValueType, varb.DataType, varb.SampleMedium, varb.TimeUnitsID,
                       varb.VariableUnitsID, varb.TimeSupport, varb.GeneralCategory, varb.NoDataValue);
            }
            ODMDB.CreateNonQueryCommand(sql);
        }

        public void SaveVariables(IVariable[] varbs)
        {
            int progress = 0;
            int total = varbs.Length;
            int i = 1;
            foreach (var varb in varbs)
            {
                SaveVariable(varb);
                progress = i * 100 / total;
                i++;
                OnProgressChanged(progress);
            }
            OnFinished();
        }

        public void UpdateSeriesCatalog()
        {
            string sql = "SELECT DISTINCT siteid AS sid, variableid AS vid, datetimeutc AS dt FROM datavalues;";
            var dt = ODMDB.QueryDataTable(sql);
            var dten = dt.AsEnumerable();
            var buf = (from dr in dten select dr.Field<int>("sid")).Distinct();

            int progress = 0;
            int total = buf.Count();
            int i = 1;

            foreach (var s in buf)
            {
                var varb = from dr in dten
                           where dr.Field<int>("sid") == s
                           group dr by dr.Field<int>("vid") into m
                           select new
                           {
                               vid = m.Key,
                               min = m.Select(n => n.Field<DateTime>("dt")).Min(),
                               max = m.Select(n => n.Field<DateTime>("dt")).Max(),
                               count = m.Select(n => n.Field<DateTime>("dt")).Count(),
                           };
                foreach (var vb in varb)
                {
                    sql = string.Format("select * from SeriesCatalog where SiteID={0} and VariableID = {1}", s, vb.vid);
                    if (ODMDB.Exists(sql))
                    {
                        sql = string.Format("delete * from SeriesCatalog where SiteID={0} and VariableID = {1}", s, vb.vid);
                        ODMDB.CreateNonQueryCommand(sql);
                    }
                    sql = string.Format("insert into SeriesCatalog (SiteID,VariableID,BeginDateTime,EndDateTime,ValueCount)  values ({0}, {1},  #{2}#, #{3}#, {4})",
                    s, vb.vid, vb.min.ToString("yyyy/MM/dd HH:mm:ss"), vb.max.ToString("yyyy/MM/dd HH:mm:ss"), vb.count);
                    ODMDB.CreateNonQueryCommand(sql);
                }
                progress = i * 100 / total;
                i++;
                OnProgressChanged(progress);
            }
            OnFinished();
        }

        public virtual void SaveDataValues(IObservationsSite[] site, int varID)
        {
            string sql = "";

            foreach (var s in site)
            {
                var ts = s.TimeSeries as IVectorTimeSeries<double>;
                // update DataValue table
                for (int d = 0; d < s.TimeSeries.DateTimes.Length; d++)
                {

                    sql = string.Format("select * from DataValues where SiteID={0} and VariableID = {1} and DateTimeUTC= #{2}#", s.ID, varID, s.TimeSeries.DateTimes[d].ToString("yyyy/MM/dd"));
                    if (ODMDB.Exists(sql))
                    {
                        //  sql = string.Format("delete * from DataValues  where SiteID={0} and VariableID = {1} and DateTimeUTC= #{2}#", s.ID, varID, s.TimeSeries.DateTimeVector[d].ToString("yyyy/MM/dd"));
                        sql = string.Format("update DataValues set datavalue={0} where SiteID={1} and variableid={2} and datetimeutc=#{3}#", ts.Value[d],
                             s.ID, varID, s.TimeSeries.DateTimes[d].ToString("yyyy/MM/dd HH:mm:ss"));
                        ODMDB.CreateNonQueryCommand(sql);
                    }
                    else
                    {
                        sql = string.Format("insert into DataValues (SiteID,VariableID,DateTimeUTC,DataValue)  values ({0}, {1},  #{2}#,{3})",
                               s.ID, varID, s.TimeSeries.DateTimes[d].ToString("yyyy/MM/dd HH:mm:ss"), ts.Value[d]);
                        ODMDB.CreateNonQueryCommand(sql);
                    }
                }

                // update SeriesCatalog table
                sql = string.Format("select * from SeriesCatalog where SiteID={0} and VariableID = {1}", s.ID, varID);
                if (ODMDB.Exists(sql))
                {
                    sql = string.Format("delete * from SeriesCatalog where SiteID={0} and VariableID = {1}", s.ID, varID);
                    ODMDB.CreateNonQueryCommand(sql);
                }

                sql = string.Format("select DateTimeUTC from DataValues where SiteID={0} and VariableID = {1}", s.ID, varID);
                var dt = ODMDB.QueryDataTable(sql);
                if (dt != null)
                {
                    var dates = from r in dt.AsEnumerable() select r.Field<DateTime>("DateTimeUTC");
                    sql = string.Format("insert into SeriesCatalog (SiteID,VariableID,BeginDateTime,EndDateTime,ValueCount)  values ({0}, {1},  #{2}#, #{3}#, {4})",
                    s.ID, varID, dates.Min().ToString("yyyy/MM/dd"), dates.Max().ToString("yyyy/MM/dd"), dates.Count());
                    ODMDB.CreateNonQueryCommand(sql);
                }

            }
        }

        public IDendritiRecord<object> GetParameters()
        {
            ParameterRecord root = new ParameterRecord("Parameters")
            {
                Parent = null,
                ID = 9999,
                Description = "Root"
            };

            string sql = "";
            sql = string.Format("select * from [Parameters] ");
            var dt = ODMDB.QueryDataTable(sql);

            var records = from dr in dt.AsEnumerable()
                          select new
                          {
                              ID = dr.Field<int>("ID"),
                              Term = dr.Field<string>("Term"),
                              //Def = dr.Field<string>("Definition"),
                              Module = dr.Field<string>("Module"),
                              //Value = dr.Field<double>("Values"),
                              //Descrip = dr.Field<string>("Description"),
                          };

            var modules = from rc in records group rc by rc.Module into cat select new { Module = cat.Key, Items = cat.ToArray() };

            foreach (var mm in modules)
            {
                var module = new ParameterRecord(mm.Module)
                {
                    Parent = root
                };
                var groups = from pa in mm.Items group pa by pa.Term into cat select new { Term = cat.Key, Items = cat.ToArray() };

                foreach (var pp in groups)
                {
                    var term_group = new ParameterRecord(pp.Term)
                    {
                        Parent = module
                    };
                    //foreach (var p in pp.Items)
                    //{
                    //    var para = new ParameterRecord(p.Descrip)
                    //    {
                    //        Description = p.Descrip,
                    //        Parent = term_group,
                    //        ID = p.ID,
                    //        Value= p.Value
                    //    };
                    //    term_group.Children.Add(para);
                    //}
                    module.Children.Add(term_group);
                }
                root.Children.Add(module);
            }
            return root;
        }

        public DataTable GetParametersTable(IDendritiRecord<object> record)
        {
            string sql = "";
            sql = string.Format("select Description,Min, Average, Max from [Parameters] where Term='{0}'", record.Name);
            var dt = ODMDB.QueryDataTable(sql);
            return dt;
        }

        public void UpdateSiteList()
        {
            _sites.Clear();
            var sites = GetSites(new QueryCriteria());
            foreach (var s in sites)
            {
                _sites.Add(s.ID, s);
            }
        }

        public void UpdateVariableList()
        {
            _variables.Clear();
            var variables = GetVariables();
            foreach (var vab in variables)
            {
                _variables.Add(vab.ID, vab);
            }
        }
    }
}
