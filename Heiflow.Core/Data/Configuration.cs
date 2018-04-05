// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

  namespace Heiflow.Core.Data
{
    public  class Configuration
    {
        public static IDBase DataBase { get; set; }
        public static string DataTablePrefix { get; set; }
        public static string StationsTableName { get; set; }
        public static string  HydroFeaturesTableName { get; set; }
        public static string VariablesTableName { get; set; }
        public static string SeriesCatalogTableName { get; set; }  
        public static string DataValuesTableName { get; set; }

         static Configuration()
        {
            Initialize();
        }

        public static void Initialize()
        {
            Configuration.DataTablePrefix = "";
            Configuration.StationsTableName = Configuration.DataTablePrefix + "Sites";
            Configuration.HydroFeaturesTableName = Configuration.DataTablePrefix + "HydroFeatures";
            Configuration.VariablesTableName = Configuration.DataTablePrefix + "Variables";
            Configuration.SeriesCatalogTableName = Configuration.DataTablePrefix + "SeriesCatalog";
            Configuration.DataValuesTableName = Configuration.DataTablePrefix + "DataValues";
        }

        public static void Save()
        {
        }
    }

}
