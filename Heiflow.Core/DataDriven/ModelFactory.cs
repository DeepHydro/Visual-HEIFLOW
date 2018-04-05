// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.DataDriven
{
    public class ModelFactory
    {
        public static IForecastingModel CreateForecastingModel(string name)
        {
            IForecastingModel model=null;
            switch (name)
            {
                case "Artificial Neural Network":
                    AnnModelParameter annPara = new AnnModelParameter();
                    model = new NeuralNetworkModel(annPara);
                    break;
                case "HIGANN":
                    AnnModelParameter annPara1 = new AnnModelParameter();
                    model = new NeuralNetworkModel(annPara1);
                    break;
                case "Support Vector Machine":
                    Heiflow.AI.SVM.Parameter p = new Heiflow.AI.SVM.Parameter();
                    model = new SVMModel(p);
                    break;
                case "Multiple Linear Regression":
                    ModelParameter mp = new ModelParameter();
                    model = new MLRModel(mp);
                    break;
                case "Genetic Programming":
                    GPModelParameter para = new GPModelParameter();
                    model = new GPModel(para);
                    break;
                case "Model Tree":
                    Rule root = new Rule(5, 0.47035, RuleType.Interior);
                    Rule right = new Rule(RuleType.RightLeaf);
                    root.RightChild = right;
                    Rule left = new Rule(5, 0.30445, RuleType.Interior);
                    root.LeftChild = left;
                    Rule left1 = new Rule(RuleType.LeftLeaf);
                    left.LeftChild = left1;
                    Rule right1 = new Rule(9, 0.156, RuleType.Interior);
                    left.RightChild = right1;

                    Rule right1_left = new Rule(RuleType.LeftLeaf);
                    right1.LeftChild = right1_left;

                    Rule right1_right = new Rule(RuleType.RightLeaf);
                    right1.RightChild = right1_right;

                    HybridModelParameter hmp = new HybridModelParameter(root);
                     model = new HybridModel(hmp);
                    break;
            }
            return model;
        }

        public static IMultivariateAnalysis CreateMultivariateAnalysisModel(string name)
        {
            IMultivariateAnalysis model = null;
            switch (name)
            {
                case "Self-organizing Map":
                    SOMParameter somPara = new SOMParameter();
                    model = new SOMModel(somPara);
                    break;
            }
            return model;
        }

        public static ICalibrationModel CreateCalibrationModel(string name)
        {
            ICalibrationModel model = null;
            switch (name)
            {
                case "MOSCE":
                    MoSCEModelParameter scepara = new MoSCEModelParameter();
                    model = new MoSCEModel(scepara);
                    break;
            }
            return model;
        }
    }
}
