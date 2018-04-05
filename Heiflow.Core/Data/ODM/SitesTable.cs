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
using System.Data;
using System.IO;
using Heiflow.Core.IO;

namespace Heiflow.Core.Data.ODM
{
    public class SitesTable : ODMTable
    {
        private string[] _NessearyFields = new string[] { "SiteID", "Longitude", "Latitude", "Elevation_m",  "SiteType", "SiteName",
            "State" };
        private string[] _UnNessearyFields = new string[] { "SiteCode", "Comments", "Country" };

        public SitesTable()
        {
            _TableName = "Sites";
            _ExportSetting = new SiteExportSetting()
            {
                VariableID = 1,
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
        }

        public override bool Save(DataTable dt)
        {
            string sql = "";
            var ODMDB = ODM.ODMDB;
            int progress = 0;
            int total = dt.Rows.Count;
            int i = 1;

            OnStarted();

            foreach (DataRow dr in dt.Rows)
            {
                sql = string.Format("select * from Sites where SiteID={0}", dr["SiteID"]);
                if (ODMDB.Exists(sql))
                {
                    sql = string.Format("update Sites set SiteName='{1}', SiteCode='{2}',  SiteType='{3}',Longitude={4},Latitude={5},Elevation_m={6}," +
                "State='{7}',Country='{8}',MonitorType={9},Comments='{10}' where SiteID={0}",
           dr["SiteID"],  dr["SiteName"], dr["SiteCode"], dr["SiteType"], dr["Longitude"], dr["Latitude"], dr["Elevation_m"], dr["State"],
                      dr["Country"], dr["MonitorType"], dr["Comments"]
             );
                }
                else
                {
                    sql = string.Format("insert into Sites (SiteID, SiteName,SiteCode,SiteType,Longitude,Latitude,Elevation_m,State,Country,MonitorType,Comments)" +
                    "values ({0}, '{1}', '{2}','{3}', {4}, {5},{6}, '{7}', '{8}','{9}', '{10}')",
                      dr["SiteID"], dr["SiteName"], dr["SiteCode"], dr["SiteType"], dr["Longitude"], dr["Latitude"], dr["Elevation_m"], dr["State"],
                      dr["Country"], dr["MonitorType"], dr["Comments"]);
                }
                ODMDB.CreateNonQueryCommand(sql);
                progress = i * 100 / total;
                i++;
                OnProgressChanged(progress);
            }
            OnFinished();
            return true;
        }

        public Site [] GetSites(DataTable dt)
        {
            var dc_names = from DataColumn dc in dt.Columns select dc.ColumnName;
            Site[] sites = new Site[dt.Rows.Count];
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                sites[i] = new Site()
                {
                    ID = int.Parse(dr[_NessearyFields[0]].ToString()),
                    Longitude = double.Parse(dr[_NessearyFields[1]].ToString()),
                    Latitude = double.Parse(dr[_NessearyFields[2]].ToString()),
                    Elevation = double.Parse(dr[_NessearyFields[3]].ToString()),
                    SiteType = dr[_NessearyFields[4]].ToString(),
                    Name = dr[_NessearyFields[5]].ToString(),
                    State = dr[_NessearyFields[6]].ToString(),
                    MonitorType = 0,
                    Code=  dr[_UnNessearyFields[0]].ToString(),
                     Comments=  dr[_UnNessearyFields[1]].ToString(),
                     Country = dr[_UnNessearyFields[2]].ToString()
                };
                i++;                
            }
            return sites;
        }

        public override void CustomExport( DataTable source)
        {
             if(source != null)
             {
                 var sites = GetSites(source);
                 var setting = _ExportSetting as SiteExportSetting;
                 foreach(var site  in sites)
                 {
                     var series=new ObservationSeries()
                     {
                          SiteID=site.ID,
                          VariableID= setting.VariableID
                     };             
                     var ts = ODM.GetTimeSereis(series);
                     if (ts != null)
                     {
                         var filename = Path.Combine(setting.Directory, site.Name + ".csv");
                         var var_odm = ODM.GetVariable(setting.VariableID);
                         ts.Name = var_odm.Name;
                         CSVFileStream csv = new CSVFileStream(filename);
                         csv.Save(ts);
                     }
                 }
             }
        }
    }
}
