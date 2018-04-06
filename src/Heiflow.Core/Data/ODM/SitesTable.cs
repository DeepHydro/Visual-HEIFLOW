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
