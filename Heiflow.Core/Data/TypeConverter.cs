// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using Heiflow.AI.GeneticProgramming;
using Heiflow.Core;
using Heiflow.Core.DataDriven;

namespace Heiflow.Core.Data
{
    public class NormalizationTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "Range", "Sqrt" });
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

      public class ActivationFuncNameConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(AnnModelParameter.ActivationFunctionStr);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class TrainAlgorithmNameConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(AnnModelParameter.TrainAlgorithmStr);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class GPInitializationConverter : StringConverter
    {
       
        public static string[] ValuesCollection = new string[] { "MyMath.Full", "Grow", "Half and Half" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "MyMath.Full", "Grow", "Half and Half" });
            //return new StandardValuesCollection(new EInitializationMethod[] { EInitializationMethod.MyMath.FullInitialization, EInitializationMethod.GrowInitialization, EInitializationMethod.HalfHalfInitialization});
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class GPFitnessConverter : StringConverter
    {
        public static string[] ValuesCollection = new string[] { "MSE", "RMSE", "MAE", "RSE", "RAE", "rMSE", "rRMSE", "rMAE", "rRSE", "rRRSE", "rRAE", "AE", "RE" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "MSE", "RMSE", "MAE","RSE","RAE","rMSE","rRMSE","rMAE","rRSE","rRRSE","rRAE","AE","RE" });
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }


    public class GPSelectionConverter : StringConverter
    {
        public  static string[] ValuesCollection = new string []{ "Fitness proportionate", "Rank", "SUS", "FUS", "RAE", "Skrgic"};
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "Fitness proportionate", "Rank", "SUS", "FUS", "RAE", "Skrgic"});
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }
   






}
