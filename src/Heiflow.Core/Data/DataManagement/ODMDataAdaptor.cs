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
using System.Data;
using Heiflow.Core.Data;
using Heiflow.Core;
using Heiflow.Core.Hydrology;
using Heiflow.Core.Schema;

namespace Heiflow.Core.Data
{
    /// <summary>
    /// Provide access to a database implementing ODB data model
    /// </summary>
    public class ODMDataAdaptor : ITimeSeriesTransform
    {
        public ODMDataAdaptor(IDBase dbase)
        {
            mDbase = dbase;
        }

        public event EventHandler<ProgressEvent> SavingTimeSeries;

        private IDBase mDbase;

        /// <summary>
        /// Given a site id, this method returns the site's information including
        ///  all the variables measured at the site
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HydroPoint GetSiteInfo(int id)
        {
            string sql = "select * from " + Configuration.StationsTableName + " where SiteID =" + id;
            DataTable dt = mDbase.QueryDataTable(sql);
            DataRow dr = dt.Rows[0];
            HydroPoint p = new HydroPoint(id)
            {
                Name = dr["SiteName"].ToString(),
                Code = dr["SiteCode"].ToString(),
                Latitude = Convert.ToDouble(dr["Latitude"].ToString()),
                Longitude = Convert.ToDouble(dr["Longitude"].ToString()),
                Comments = dr["Comments"].ToString()
            };

            sql = "select * from " + Configuration.SeriesCatalogTableName + " where SiteID =" + id;
            dt = mDbase.QueryDataTable(sql);
            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    int vid = Convert.ToInt32(r["VariableID"].ToString());
                    Variable v = GetVariableInfo(vid);
                    v.BeginDate = DateTime.Parse(r["BeginDateTime"].ToString());
                    v.EndDate = DateTime.Parse(r["EndDateTime"].ToString());
                    v.ValueCount = Int32.Parse(r["ValueCount"].ToString());
                    v.SiteID = id;
                    p.MeasuredVariables.Add(v);          
                }                
            }
            return p;
        }

        /// <summary>
        /// Given an array of site ids, this method returns the metadata for each one
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public HydroPoint [] GetSites(int [] ids)
        {
            HydroPoint[] sites = new HydroPoint[ids.Length];
            for (int i = 0; i < sites.Length; i++)
            {
                sites[i] = GetSiteInfo(ids[i]);
            }
            return sites;
        }

        /// <summary>
        /// Given a variable ID, this methods returns metadata about the variable
        /// </summary>
        /// <param name="id">Variable ID</param>
        /// <returns>An instance of the variable class</returns>
        public Variable GetVariableInfo(int id)
        {
            string sql = "select * from " + Configuration.VariablesTableName + " where VariableID =" + id;
            DataTable dt = mDbase.QueryDataTable(sql);
            DataRow dr = dt.Rows[0];
            Variable p = new Variable(id)
            {
                Name = dr["VariableName"].ToString(),
                Code = dr["VariableCode"].ToString(),
                TimeSupport = Convert.ToDouble(dr["TimeSupport"].ToString()),
                Specification = dr["Specification"].ToString(),
                SampleMedium = dr["SampleMedium"].ToString(),
                NoDataValue = Convert.ToDouble(dr["NoDataValue"].ToString()),
                GeneralCategory = dr["GeneralCategory"].ToString(),
            };
            int timeUnitId = Convert.ToInt32(dr["TimeUnitsID"].ToString());
            p.TimeUnits = (TimeUnits) timeUnitId;
            //TODO: the value type, data type of the variable is not retrieved!

            return p;
        }

        /// <summary>
        /// Given a variable ID, this method returns metadata about the variable and all the sites measuring the variable
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VirtualVariable GetVirtualVariableInfo(int id)
        {
            string sql = "select * from " + Configuration.VariablesTableName + " where VariableID =" + id;
            DataTable dt = mDbase.QueryDataTable(sql);
            DataRow dr = dt.Rows[0];
            VirtualVariable p = new VirtualVariable(id)
            {
                Name = dr["VariableName"].ToString(),
                Code = dr["VariableCode"].ToString(),
                TimeSupport = Convert.ToDouble(dr["TimeSupport"].ToString()),
                Specification = dr["Specification"].ToString(),
                SampleMedium = dr["SampleMedium"].ToString(),
                NoDataValue = Convert.ToDouble(dr["NoDataValue"].ToString()),
                GeneralCategory = dr["GeneralCategory"].ToString(),
            };
            int timeUnitId = Convert.ToInt32(dr["TimeUnitsID"].ToString());
            p.TimeUnits = (TimeUnits)timeUnitId;

             sql = "select * from " + Configuration.SeriesCatalogTableName + " where VariableID =" + id;
            dt = mDbase.QueryDataTable(sql);
            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    int sid = Convert.ToInt32(r["SiteID"].ToString());
                    p.SitesMeasured.Add(GetSiteInfo(sid));
                }
            }
            return p;
        }

        #region ITimeSeriesProvider 成员

        public IVectorTimeSeries<double> GetTimeSeries(ITimeSeriesQueryCriteria qc)
        {         
            NumericalTimeSeries ts = null;
            if (qc != null)
            {
                string sql = "select * from " + Configuration.DataValuesTableName + " where VariableID =" + qc.VariableID + " and SiteID=" + qc.SiteID
                    + " and " + DBFieldFormater.FormatDateTime("DateTimeUTC", qc.Start, qc.End, mDbase.DBKind) + " order by DateTimeUTC";
                DataTable dt = mDbase.QueryDataTable(sql);
                if (dt != null)
                {                  
                    double [] dv = dt.AsEnumerable().Select(row => row.Field<double>("DataValue")).ToArray();
                    DateTime [] dtime = dt.AsEnumerable().Select(row => row.Field<DateTime>("DateTimeUTC")).ToArray();
                    ts = new NumericalTimeSeries(dv, dtime);
                    Variable variable = GetVariableInfo(qc.VariableID);
                    DataRepairer dr = new DataRepairer(variable.NoDataValue);
                    dr.Repair(ts);
                }
            }
            return ts;
        }

        public IVectorTimeSeries<double> GetTransformedTimeSeries(ITimeSeriesQueryCriteria qc, double multiplier)
        {
            NumericalTimeSeries ts = null;
            if (qc != null)
            {
                string sql = "select * from " + Configuration.DataValuesTableName + " where VariableID =" + qc.VariableID + " and SiteID=" + qc.SiteID
                    + " and " + DBFieldFormater.FormatDateTime("DateTimeUTC", qc.Start, qc.End, mDbase.DBKind);
                DataTable dt = mDbase.QueryDataTable(sql);
                if (dt != null)
                {
                    double[] dv = dt.AsEnumerable().Select(row => row.Field<double>("DataValue")).ToArray();
                    DateTime[] dtime = dt.AsEnumerable().Select(row => row.Field<DateTime>("DateTimeUTC")).ToArray();
                    ts = new NumericalTimeSeries(dv, dtime);
                    Variable variable = GetVariableInfo(qc.VariableID);
                    DataRepairer dr = new DataRepairer(variable.NoDataValue);
                    dr.Repair(ts,multiplier);
                }
            }
            return ts;
        }

        #endregion


        public DataTable GetDataValues(ITimeSeriesQueryCriteria qc)
        {
            string sql = "";
            if (qc != null)
            {
                if (qc.Start == DateTime.MinValue || qc.End == DateTime.MinValue)
                {
                    sql = "select ValueID,DateTimeUTC,DataValue from " + Configuration.DataValuesTableName + " where VariableID =" + qc.VariableID + " and SiteID=" + qc.SiteID;
                }
                else
                {
                    sql = "select ValueID,DateTimeUTC,DataValue from " + Configuration.DataValuesTableName + " where VariableID =" + qc.VariableID + " and SiteID=" + qc.SiteID
                    + " and " + DBFieldFormater.FormatDateTime("DateTimeUTC", qc.Start, qc.End, mDbase.DBKind);
                }
                return mDbase.QueryDataTable(sql);
            }
            else
            {
                return null;
            }
        }

        public VirtualVariable [] GetVirtualVariables()
        {
            string sql = "select * from " + Configuration.VariablesTableName + " order by VariableName";
            DataTable dt = mDbase.QueryDataTable(sql);
            if (dt != null)
            {
                VirtualVariable[] results = new VirtualVariable[dt.Rows.Count];
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    int id = Convert.ToInt32(dr["VariableID"].ToString());
                    results[i] = GetVirtualVariableInfo(id);
                    i++;
                }
                return results;
            }
            else
            {
                return null;
            }
        }

        public bool DeleteTimeSeries(  DateTime start , DateTime end, int variableID, int siteID)
        {
            string delcmd = "delete from " + Configuration.DataValuesTableName + " where VariableID =" + variableID + " and SiteID=" + siteID
           + " and " + DBFieldFormater.FormatDateTime("DateTimeUTC", start, end, mDbase.DBKind);
           return  mDbase.CreateNonQueryCommand(delcmd);
        }

        public void SaveTimeSeries(IVectorTimeSeries<double> ts, int variableID, int siteID)
        {
            DateTime start = ts.DateTimes.Min();
            DateTime end = ts.DateTimes.Max();

            string delcmd = "delete from "+ Configuration.DataValuesTableName + " where VariableID =" + variableID + " and SiteID=" + siteID
                + " and " + DBFieldFormater.FormatDateTime("DateTimeUTC", start,end, mDbase.DBKind);
            if (mDbase.CreateNonQueryCommand(delcmd))
            {
                int i = 0;
                foreach (DateTime t in ts.DateTimes)
                {
                    string insertcmd = "";
                    if (mDbase.DBKind == DBkind.Access2003 || mDbase.DBKind == DBkind.Access2007)
                    {
                        insertcmd = "insert into " + Configuration.DataValuesTableName + " (DataValue,DateTimeUTC,SiteID,VariableID) values(" + ts.Value[i]
                            + ",'" + t + "'," + siteID + "," + variableID + ")";
                    }
                    else
                    {
                        insertcmd = "insert into " + Configuration.DataValuesTableName + " (DataValue,DateTimeUTC,SiteID,VariableID) values(" + ts.Value[i]
                          + "," + t + "," + siteID + "," + variableID + ")";
                    }
                    mDbase.CreateNonQueryCommand(insertcmd);
                    if (SavingTimeSeries != null)
                        SavingTimeSeries(this,new ProgressEvent(i));
                    i++;
                }
            }
        }

        public void SaveTimeSeries(INumericalSeriesPair[] pairs, int variableID, int siteID)
        {
            int i = 1;
            foreach (INumericalSeriesPair p in pairs)
            {
                string insertcmd = "";
                if (mDbase.DBKind == DBkind.Access2003 || mDbase.DBKind == DBkind.Access2007)
                {
                    insertcmd = "insert into " + Configuration.DataValuesTableName + " (DataValue,DateTimeUTC,SiteID,VariableID) values(" + p.Value
                        + ",'" + p.DateTime + "'," + siteID + "," + variableID + ")";
                }
                else
                {
                    insertcmd = "insert into " + Configuration.DataValuesTableName + " (DataValue,DateTimeUTC,SiteID,VariableID) values(" + p.Value
                      + "," + p.DateTime + "," + siteID + "," + variableID + ")";
                }
                mDbase.CreateNonQueryCommand(insertcmd);
                if (SavingTimeSeries != null)
                  SavingTimeSeries(this,new ProgressEvent(i));
                i++;
            }
        }

        public bool CheckExistanceOfTimeSeries(int variableID, int siteID,DateTime start,DateTime end)
        {
            string slctcmd = "select ValueID from " + Configuration.DataValuesTableName + " where VariableID =" + variableID + " and SiteID=" + siteID
                + " and " + DBFieldFormater.FormatDateTime("DateTimeUTC", start, end, mDbase.DBKind);
            return mDbase.Exists(slctcmd);
        }
    }
}
