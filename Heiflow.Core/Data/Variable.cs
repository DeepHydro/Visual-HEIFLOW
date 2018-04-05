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
