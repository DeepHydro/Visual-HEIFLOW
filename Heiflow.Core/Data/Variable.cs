// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Heiflow.Core.Data;

namespace Heiflow.Core
{
    [Serializable]
    [XmlInclude(typeof(RangeNormalization))]
    [XmlInclude(typeof(Units))]
   public  class Variable: IVariable,ICloneable
    {
       public Variable()
       {
           WindowSize = 2;
           PredicationSize = 0;
           IncludeIntradayValue = false;
           TimeUnits = TimeUnits.Second;
       }

       public Variable(int id)
       {
           mVariableID = id;
           WindowSize = 2;
           PredicationSize = 0;
           IncludeIntradayValue = false;
           Name = "Unknown";
           AliasName = "Unknown";
           SampleMedium = "水体";
           TimeUnits = TimeUnits.Day;
           Units = new Units(-9999);
       }

       private int mVariableID;
       private int mWindowSize;

       private string mNormalizationMethod = "Range";

        #region IVariable members
        [CategoryAttribute("Metadata"), DescriptionAttribute("")]
       public int VariableID
       {
           get { return mVariableID; }
       }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public string Name
        {
            get;
            set;
        }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public string Code
        {
            get;
            set;
        }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public string AliasName
        {
            get;
            set;
        }

        [CategoryAttribute("Metadata"),  ReadOnly(true),DescriptionAttribute("")]
        public int SiteID
        {
            get;
            set;
        }

        [CategoryAttribute("Metadata"),  ReadOnly(true),DescriptionAttribute("")]
        public string Specification
        {
            get;
            set;
        }
        [CategoryAttribute("Metadata"),  ReadOnly(true),DescriptionAttribute("")]
        public string GeneralCategory
        {
            get;
            set;
        }

       [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public DateTime BeginDate { get; set; }

       [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public DateTime EndDate { get; set; }

       [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public int ValueCount { get; set; }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public double NoDataValue
        {
            get;
            set;
        }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
       public TimeUnits TimeUnits
        {
            get;
            set;
        }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
       public string SampleMedium { get; set; }

        private IUnits iUnit;

        [XmlIgnore]
        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public IUnits Units
        {
            get
            {
                return iUnit;
            }
            set
            {
                iUnit = value;
                if (iUnit is Units)
                    VariableUnit = iUnit as Units;
            }
        }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public Units VariableUnit
        {
            get;
            set;
        }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("")]
        public double TimeSupport
        {
            get;
            set;
        }

        [CategoryAttribute("Metadata"), ReadOnly(true), DescriptionAttribute("The unit of the variable")]
        public string UnitName
        {
            get;
            set;
        }

         [CategoryAttribute("Forecasting"), ReadOnly(true), DescriptionAttribute("The role this variable plays in forecasting")]
        public ModelRole ModelRole
        {
            get;
            set;
        }

        [CategoryAttribute("Forecasting"), DescriptionAttribute(" The Intraday value is inclued in the forecasting data vector if this property is set true")]
         public bool IncludeIntradayValue { get; set; }

       [CategoryAttribute("Forecasting"), DescriptionAttribute("The lag value prior to the forecasted datetime")]
        public int WindowSize
        {
            get
            {
                return mWindowSize;
            }
            set
            {
                mWindowSize = value;
            }
        }

        [CategoryAttribute("Forecasting"), DescriptionAttribute("The period ahead to be predicated")]
        public int PredicationSize
        {
            get;
            set;
        }

       [CategoryAttribute("Normalization"), TypeConverter(typeof(NormalizationTypeConverter)), Browsable(false), DescriptionAttribute("Method used to normalize the time series of the variable")]
        public string NormalizationMethod
        {
            get
            {
                return mNormalizationMethod;
            }
            set
            {
                mNormalizationMethod = value;
                if (mNormalizationMethod == "Range")
                    NormalizationModel = new RangeNormalization();
                else if (mNormalizationMethod == "Sqrt")
                    NormalizationModel = new SqrtNormalization();
            }
        }

      [XmlIgnore]
      [CategoryAttribute("Normalization"), Browsable(false)]
        public INormalizationModel NormalizationModel
        {
            get;
            set;
        }

      public virtual IVectorTimeSeries<double> GetValues(ITimeSeriesQueryCriteria qc, ITimeSeriesTransform provider)
        {
            if (provider != null && qc != null)
            {
                 return provider.GetTimeSeries(qc);
            }
            else
            {
                return null;
            }
        }

      public virtual IVectorTimeSeries<double> GetTransformedValues(ITimeSeriesQueryCriteria qc, ITimeSeriesTransform provider, double multiplier)
        {
            if (provider != null && qc != null)
            {
                return provider.GetTransformedTimeSeries(qc,multiplier);
            }
            else
            {
                return null;
            }
        }

        #endregion

        public string ValueType { get; set; }
        public string DataType { get; set; }

        public int VariableUnitsID { get; set; }

       public static string GetVariableFieldName(string variableName)
        {
            string result = "";
            switch (variableName)
            {
                case "流量":
                    result = "AVQ";
                    break;
                case "降雨":
                    result = "P";
                    break;
                case "水面蒸发":
                    result = "WSFE";
                    break;
                case "水位":
                    result = "AVZ";
                    break;
                case "水温":
                    result = "WTMP";
                    break;
                case "含沙量":
                    result = "AVCS";
                    break;
                case "输沙率":
                    result = "AVQS";
                    break;
                case "冰流量":
                    result = "AVIQ";
                    break;
                case "水面蒸发辅助项目":
                    //气温、水汽压、水汽压力差、风速
                    result = "ATMP,VP,VPD,WNDV";
                    break;
                case "降水量":
                    result = "P";
                    break;
                case "平均风速":
                    result = "AVWV";
                    break;
                case "平均气温":
                    result = "AVT";
                    break;
                case "平均气压":
                    result = "AVPS";
                    break;
                case "平均相对湿度":
                    result = "AVH";
                    break;
                case "日照时数":
                    result = "SUNH";
                    break;
                case "最大风速":
                    result = "MAXWV";
                    break;
                case "最大风向":
                    result = "MAXWO";
                    break;
                case "最低气温":
                    result = "MINT";
                    break;
                case "最低气压":
                    result = "MINPS";
                    break;
                case "最高气温":
                    result = "MAXT";
                    break;
                case "最高气压":
                    result = "MAXPS";
                    break;
                case "最小相对湿度":
                    result = "MINH";
                    break;
                case "蒸发量":
                    result = "EVP";
                    break;
            }
            return result;
        }


       #region ICloneable 成员

       public virtual object Clone()
       {
           return new Variable(VariableID)
           {
               BeginDate = BeginDate,
               Code = Code,
               EndDate = EndDate,
               GeneralCategory = GeneralCategory,
               ModelRole = ModelRole,
               Name = Name,
               NoDataValue = NoDataValue,
               NormalizationModel = NormalizationModel,
               PredicationSize = PredicationSize,
               SampleMedium = SampleMedium,
               SiteID = SiteID,
               Specification = Specification,
               TimeSupport = TimeSupport,
               TimeUnits = TimeUnits,
               Units = Units,
               ValueCount = ValueCount,
               WindowSize = WindowSize
           };
       }

       #endregion


       public int TimeUnitsID
       {
           get;
           set;
       }
    }


}
