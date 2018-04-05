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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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
