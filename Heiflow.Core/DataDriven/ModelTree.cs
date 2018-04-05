// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.AI;
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.DataDriven
{
    public enum RuleType { LeftLeaf, RightLeaf, Interior,  Root }

    public class Rule
    {
        public Rule(int splitLocation,double splitValue, RuleType type)
        {
            Type = type;
            SplitLocation = splitLocation;
            SplitValue = splitValue;
            InheritedSplitLocations = new List<int>();
            InheritedSplitLocations.Add(splitLocation);
            InheritedSplitValues = new List<double>();
            InheritedSplitValues.Add(splitValue);
        }

        public Rule(RuleType type)
        {
            Type = type;
            InheritedSplitLocations = new List<int>();
            InheritedSplitValues = new List<double>();
        }

        private IForecastingModel model;
        private Rule mLeftChild;
        private Rule mRightChild;

        public Rule LeftChild
        {
            get
            {
                return mLeftChild;
            }
            set
            {
                if (mLeftChild == null)
                {
                    mLeftChild = value;
                    mLeftChild.InheritedSplitLocations.Add(this.SplitLocation);
                    mLeftChild.InheritedSplitValues.Add(this.SplitValue);
                }
                else
                {
                    mLeftChild = value;
                }              
                value.Parent = this;
            }
        }

        public Rule RightChild
        {
            get
            {
                return mRightChild;
            }
            set
            {
                if (mRightChild == null)
                {
                    mRightChild = value;
                    mRightChild.InheritedSplitLocations.Add(this.SplitLocation);
                    mRightChild.InheritedSplitValues.Add(this.SplitValue);
                }
                else
                {
                    mRightChild = value;
                }
                value.Parent = this;
            }
        }

        public Rule Parent { get; set; }

        public RuleType Type { get; private set; }

        public List<int> InheritedSplitLocations { get; private set; }

        public List<double> InheritedSplitValues { get; private set; }

        public int SplitLocation { get; private set; }

        public double SplitValue { get; private set; }

        public IForecastingModel Model 
        {
            get
            {
                return model;
            }
            set
            {
                if (Type == RuleType.Interior)
                    throw new Exception("This node is a interior one. It can not be assigned with a model!");
                model = value;
            }
        }

        public int CaseCount { get; set; }


        public override string ToString()
        {
            if (Type == RuleType.Interior)
            {
               string str = "if x" + SplitLocation + "<=" + SplitValue + "\n";
                
            }
            return base.ToString();
        }
    }

    public class HybridModelParameter : ModelParameter
    {
        public HybridModelParameter(Rule rule)
        {
            Rule = rule;
        }

        public Rule Rule { get; private set; }

        public Rule CurrentLeafRule { get;  set; }
    }

    public class HybridModel:ForecastingModel
    {
        public HybridModel(HybridModelParameter parameter)
            : base(parameter)
        {
            Orgnization = "HUST WREIS";
            ID = "34934C27-DD41-44D8-B359-BF56B663A6B8";
            Name = "Hybrid Model";
            Descriptions = "";
        }

        public override void Train(IForecastingDataSets datasets)
        {
            HybridModelParameter para = mParameter as HybridModelParameter;
            OnStartRunning(new ComponentRunEventArgs(datasets));
            if (para.Rule.Type == RuleType.Interior)
            {
                BuildRuleTree(para.Rule, datasets);
                datasets.ForecastedData = new double[datasets.InputData.Length][];
                for (int i = 0; i < datasets.InputData.Length; i++)
                {
                    datasets.ForecastedData[i] = new double[1];
                    HybridModelParameter hmp = mParameter as HybridModelParameter;
                    hmp.CurrentLeafRule = LocateRule(datasets.InputData[i], hmp.Rule);
                    datasets.ForecastedData[i][0] = Forecast(datasets.InputData[i]);
                    OnRunningEpoch(new ComponentRunEpochEventArgs(i));
                }
            }
            OnFinishRunning(new ComponentRunEventArgs(datasets));
        }

        public override double Forecast(double[] inputVector)
        {
            HybridModelParameter hmp=mParameter as HybridModelParameter;
            MLRModel mlr= hmp.CurrentLeafRule.Model as MLRModel; 
            if (mlr != null)
            {
                double y = 0;
                for (int i = 0; i < inputVector.Length; i++)
                {
                    y += inputVector[i] * mlr.RegressionCoefficients[i];
                }
                return y;
            }
            else
            {
                return 0;
            }
        }

        private Rule LocateRule(double[] inputVector,Rule parentRule)
        {
            if (inputVector[parentRule.SplitLocation] <= parentRule.SplitValue)
            {
                if (parentRule.LeftChild.Type == RuleType.LeftLeaf)
                    return parentRule.LeftChild;
                else
                    return LocateRule(inputVector, parentRule.LeftChild);
            }
            else
            {
                if (parentRule.RightChild.Type == RuleType.RightLeaf)
                    return parentRule.RightChild;
                else
                    return LocateRule(inputVector, parentRule.RightChild);
            }
        }

        private void BuildRuleTree(Rule rule,IForecastingDataSets datasets)
        {
            if (rule.Type == RuleType.Interior)
            {
                if (rule.LeftChild != null)
                {
                    BuildRuleTree(rule.LeftChild, datasets);
                }
                if (rule.RightChild != null)
                {
                    BuildRuleTree(rule.RightChild, datasets);
                }
            }
            else
            {
                IForecastingDataSets extracted = ExtractSubSets(rule.InheritedSplitLocations.ToArray(),
                   rule.InheritedSplitValues.ToArray(), datasets, rule.Type);
                ModelParameter mp = new ModelParameter();
                if (extracted.Length > 0)
                {
                    MLRModel mlr = new MLRModel(mp);
                    mlr.Train(extracted);
                    rule.Model = mlr;
                }
            }
        }

        private IForecastingDataSets ExtractSubSets(int[] splitLocations, double[] splitValues, IForecastingDataSets datasets, RuleType type)
        {
            List<double[]> listInput = new List<double[]>();
            List<double[]> listOutput = new List<double[]>();
            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < datasets.Length; i++)
            {
                double[] vector = datasets.InputData[i];
                bool isCase = true;
                int j = 0;
                foreach (int sl in splitLocations)
                {
                    if (type == RuleType.LeftLeaf)
                    {
                        if (vector[sl] > splitValues[j])
                        {
                            isCase = false;
                            break;
                        }
                    }
                    else if (type == RuleType.RightLeaf)
                    {
                        if (vector[sl] <= splitValues[j])
                        {
                            isCase = false;
                            break;
                        }
                    }
                    j++;
                }
                if (isCase)
                {
                    listInput.Add(vector);
                    listOutput.Add(datasets.OutputData[i]);
                    dates.Add(datasets.Date[i]);
                }
            }
            IForecastingDataSets extractedSets = new ForecastingDataSets(listInput.ToArray(), listOutput.ToArray());
            extractedSets.Date = dates.ToArray();
            return extractedSets;
        }

        public void Output(Rule rule)
        {
         //   string str = "";
          
        }

        private void GenerateRuleString(Rule rule)
        {
            if (rule.Type == RuleType.Interior)
            {
                GenerateRuleString(rule);
            }
            else if (rule.Type == RuleType.LeftLeaf)
            {

            }
        }
    }
}
