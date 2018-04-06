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
using Heiflow.Models.AI;
using Heiflow.Core.Data;

namespace Heiflow.Core.DataDriven
{
    public class AnnModelParameter:IComponentParameter
    {
        public AnnModelParameter()
        {
            mlearning_rate = 0.1;
            mlearning_momentum = 0.9;
            mIterations = 1000;
            connection_rate = 1;
            HiddenActivationFunction = fann_activationfunc_enum.FANN_SIGMOID;
            msigmoidAlphaValue = 2;
            mDesiredError = 0.0001;
            HiddenActivationFunctionName = "Sigmoid";
            LayerCount = 3;
            HiddenNeuronsCount = new int[1];
            HiddenNeuronsCount[0] = 10;
            JitterEpoch = 10;
            HiddenCount = 1;
            JitterNoiseLimit = 0.3;
            TrainAlgorithmName = "Back Propagation";

            InitializeGA();
        }

        public static string[] ActivationFunctionStr = new string[] { "Linear","Threshold","Sigmoid ",
            "Symmetric Sigmoid","Logsig","Tansig","Gauss"};
        public static string[] TrainAlgorithmStr = new string[] { "Back Propagation", "BFGS" };

        /// <summary>
        /// the learning rate of the network
        /// </summary>
        private double mlearning_rate;

        /// <summary>
        /// The learning momentum used for backpropagation algorithm
        /// </summary>
        private double mlearning_momentum;

        /// <summary>
        /// alpha value
        /// </summary>
        public double msigmoidAlphaValue;

        /// <summary>
        /// Itreation Count
        /// </summary>
        public int mIterations;

        /// <summary>
        /// error that desired
        /// </summary>
        public double mDesiredError;

        /// <summary>
        ///  the connection rate of the network
        /// between 0 and 1, 1 meaning MyMath.Fully connected
        /// </summary>
        public double connection_rate;

        /// <summary>
        ///  is 1 if shortcut connections are used in the ann otherwise 0
        ///Shortcut connections are connections that skip layers.
        ///A MyMath.Fully connected ann with shortcut connections are a ann where
        ///neurons have connections to all neurons in all later layers.
        /// </summary>
        public UInt32 shortcut_connections;

        /// <summary>
        /// the number of data used to calculate the mean square error.
        /// </summary>
        public UInt32 num_MSE;

        /// <summary>
        /// the total error value.the real mean square error is MSE_value/num_MSE
        /// </summary>
        public double MSE_value;
        private string mHiddenActivationFunctionName;
        private string mTrainAlgorithmName;


        [CategoryAttribute("Structure of Network"), DescriptionAttribute("Number of total layers")]
        public int LayerCount
        {
            get;
            set;
        }
        [CategoryAttribute("Structure of Network"), DescriptionAttribute("Number of neurons in input layer")]
        public int InputNeuronCount
        {
            get;
            set;
        }
        [CategoryAttribute("Structure of Network"), DescriptionAttribute("Number of neurons in output layer")]
        public int OutputNeuronCount
        {
            get;
            set;
        }
        [CategoryAttribute("Structure of Network"), DescriptionAttribute("Number of hidden layers")]
        public int HiddenCount
        {
             get;
            set;
        }
        [CategoryAttribute("Structure of Network"), DescriptionAttribute("Number of neurons in hidden layers")]
        public int[] HiddenNeuronsCount
        {
            get;
            set;
        }

         [CategoryAttribute("Ignore"), Browsable(false)]
        public fann_activationfunc_enum HiddenActivationFunction
        {
            get;
            set;
        }

       [CategoryAttribute("Ignore"), Browsable(false)]
        public fann_train_enum TrainAlgorithm
        {
            get;
            set;
        }

        [CategoryAttribute("Network Training"),Browsable(false), DescriptionAttribute(" The maximum window size of input variables")]
       public int MaximumWindowSize
       {
           get;
           set;
       }

        [CategoryAttribute("Network Training"), Browsable(false),DescriptionAttribute(" The Predicationsize of output variable")]
        public int PredicationSize
        {
            get;
            set;
        }

        [CategoryAttribute("Network Training"), DescriptionAttribute(" The epoch (interval) at which jitter is performed. If this value is not positive, no jitter is performed.")]
        public int JitterEpoch
        {
            get;
            set;
        }

        [CategoryAttribute("Network Training"), DescriptionAttribute(" Maximum absolute limit to the random noise added while")]
        public double JitterNoiseLimit
        {
            get;
            set;
        }

        [CategoryAttribute("Network Training"), TypeConverter(typeof(ActivationFuncNameConverter)), DescriptionAttribute("Input-Hidden activation funciton")]
        public string HiddenActivationFunctionName
        {
            get
            {
                return mHiddenActivationFunctionName;
            }
            set
            {
                mHiddenActivationFunctionName = value;
                int index = 0;
                foreach (string str in ActivationFunctionStr)
                {
                    if (str == mHiddenActivationFunctionName)
                        break;
                    index++;
                }
                HiddenActivationFunction = AnnModelParameter.MapActivationFunc(index);
            }
        }

        [CategoryAttribute("Network Training"), TypeConverter(typeof(TrainAlgorithmNameConverter)), DescriptionAttribute("Train Algorithm")]
        public string TrainAlgorithmName
        {
            get
            {
                return mTrainAlgorithmName;
            }
            set
            {
                mTrainAlgorithmName = value;
                int index = 0;
                foreach (string str in TrainAlgorithmStr)
                {
                    if (str == mTrainAlgorithmName)
                        break;
                    index++;
                }
                TrainAlgorithm = AnnModelParameter.MapTrainAlgorithm(index);
            }
        }

        [CategoryAttribute("Network Training"), DescriptionAttribute(" The learning rate of the network")]
        /// <summary>
        /// the learning rate of the network
        /// </summary>
        public double LearningRate
        {
            get
            {
                return mlearning_rate;
            }
            set
            {
                mlearning_rate = value;
                mlearning_rate = Math.Max(0.00001, Math.Min(1, mlearning_rate));

            }
        }
        [CategoryAttribute("Network Training"), DescriptionAttribute("The learning momentum used for backpropagation algorithm")]
        /// <summary>
        /// The learning momentum used for backpropagation algorithm
        /// </summary>
        public double LearningMomentum
        {
            get
            {
                return mlearning_momentum;
            }
            set
            {
                mlearning_momentum = value;
                mlearning_momentum = Math.Max(0, Math.Min(0.5, mlearning_momentum));
            }
        }
        [CategoryAttribute("Network Training"), DescriptionAttribute("Sigmoid value")]
        /// <summary>
        /// alpha value
        /// </summary>
        public double SigmoidAlphaValue
        {
            get
            {
                return msigmoidAlphaValue;
            }
            set
            {
                msigmoidAlphaValue = value;
                msigmoidAlphaValue = Math.Max(0.001, Math.Min(50, msigmoidAlphaValue));
            }
        }
        [CategoryAttribute("Network Training"), DescriptionAttribute("Itreation Count")]
        /// <summary>
        /// Itreation Count
        /// </summary>
        public int Iterations
        {
            get
            {
                return mIterations;
            }
            set
            {
                mIterations = value;
                mIterations = Math.Max(0, mIterations);
            }
        }
        [CategoryAttribute("Network Training"), DescriptionAttribute("Desired error")]
        /// <summary>
        /// error that desired
        /// </summary>
        public double DesiredError
        {
            get
            {
                return mDesiredError;
            }
            set
            {
                mDesiredError = value;
                mDesiredError = Math.Max(0.001, mDesiredError);
            }
        }

        private void InitializeGA()
        {
            CrossoverOperator = "Adaptive";
            CrossoverProbability = 0.8;
            MaximumGenerations = 500;
            MutationOperator = "Adaptive";
            MutationProbability = 0.08;
            PopulationSize = 100;
            SelectionOperator = "Rank-based Selection";

        }

        [CategoryAttribute("Genetic Algorithm"), DescriptionAttribute("")]
        /// <summary>
        /// error that desired
        /// </summary>
        public string CrossoverOperator
        {
            get;
            set;
        }

        [CategoryAttribute("Genetic Algorithm"), DescriptionAttribute("")]
        /// <summary>
        /// error that desired
        /// </summary>
        public double CrossoverProbability 
        {
            get;
            set;
        }

        [CategoryAttribute("Genetic Algorithm"), DescriptionAttribute("")]
        /// <summary>
        /// error that desired
        /// </summary>
        public int MaximumGenerations
        {
            get;
            set;
        }

        [CategoryAttribute("Genetic Algorithm"), DescriptionAttribute("")]
        /// <summary>
        /// error that desired
        /// </summary>
        public string MutationOperator
        {
            get;
            set;
        }

        [CategoryAttribute("Genetic Algorithm"), DescriptionAttribute("")]
        /// <summary>
        /// error that desired
        /// </summary>
        public double MutationProbability
        {
            get;
            set;
        }

        [CategoryAttribute("Genetic Algorithm"), DescriptionAttribute("")]
        /// <summary>
        /// error that desired
        /// </summary>
        public int PopulationSize
        {
            get;
            set;
        }

        [CategoryAttribute("Genetic Algorithm"), DescriptionAttribute("")]
        /// <summary>
        /// error that desired
        /// </summary>
        public string SelectionOperator
        {
            get;
            set;
        }

        public static fann_activationfunc_enum MapActivationFunc(int index)
        {
            fann_activationfunc_enum result = fann_activationfunc_enum.FANN_SIGMOID_STEPWISE;

            switch (index)
            {
                case 0:
                    result = fann_activationfunc_enum.FANN_LINEAR;
                    break;
                case 1:
                    result = fann_activationfunc_enum.FANN_THRESHOLD;
                    break;
                case 2:
                    result = fann_activationfunc_enum.FANN_SIGMOID;
                    break;
                case 3:
                    result = fann_activationfunc_enum.FANN_SIGMOID_SYMMETRIC;
                    break;
                case 4:
                    result = fann_activationfunc_enum.FANN_GAUSSIAN;
                    break;
                case 5:
                    result = fann_activationfunc_enum.FANN_GAUSSIAN_SYMMETRIC;
                    break;
                default:
                    result = fann_activationfunc_enum.FANN_SIGMOID;
                    break;
            }

            return result;
        }

        public static fann_train_enum MapTrainAlgorithm(int index)
        {
            fann_train_enum result = fann_train_enum.FANN_TRAIN_INCREMENTAL;
            switch (index)
            {
                case 0:
                    result = fann_train_enum.FANN_TRAIN_INCREMENTAL;
                    break;
                case 1:
                    result = fann_train_enum.FANN_TRAIN_BATCH;
                    break;
                case 2:
                    result = fann_train_enum.FANN_TRAIN_RPROP; ;
                    break;
                case 3:
                    result = fann_train_enum.FANN_TRAIN_QUICKPROP;
                    break;
                default:
                    result = fann_train_enum.FANN_TRAIN_INCREMENTAL;
                    break;
            }
            return result;
        }
        //int mPopulationSize = 100;
        //int mMaximumGenerations = 500;
        //double mCrossoverProbability = 0.5;
        //double mMutationProbability = 0.05;
        //string mCrossoverOperator = "Adaptive";
        //string mMutationOperator = "Adaptive";
        //string mSelectionOperator = "Rank-based Selection";

        //[CategoryAttribute("遗传算法参数"), DescriptionAttribute("种群规模")]
        //public int PopulationSize
        //{
        //    get
        //    {
        //        return mPopulationSize;
        //    }
        //    set
        //    {
        //        mPopulationSize = value;
        //    }
        //}

        //[CategoryAttribute("遗传算法参数"), DescriptionAttribute("最大遗传代数")]
        //public int MaximumGenerations
        //{
        //    get
        //    {
        //        return mMaximumGenerations;
        //    }
        //    set
        //    {
        //        mMaximumGenerations = value;
        //    }
        //}

        //[CategoryAttribute("遗传算法参数"), DescriptionAttribute("选择算子")]
        //public string SelectionOperator
        //{
        //    get
        //    {
        //        return mSelectionOperator;
        //    }
        //    set
        //    {
        //        mSelectionOperator = value;
        //    }
        //}

        //[CategoryAttribute("遗传算法参数"), DescriptionAttribute("交叉算子")]
        //public string CrossoverOperator
        //{
        //    get
        //    {
        //        return mCrossoverOperator;
        //    }
        //    set
        //    {
        //        mCrossoverOperator = value;
        //    }
        //}

        //[CategoryAttribute("遗传算法参数"), DescriptionAttribute("变异算子")]
        //public string MutationOperator
        //{
        //    get
        //    {
        //        return mMutationOperator;
        //    }
        //    set
        //    {
        //        mMutationOperator = value;
        //    }
        //}

        //[CategoryAttribute("遗传算法参数"), DescriptionAttribute("交叉概率")]
        //public double CrossoverProbability
        //{
        //    get
        //    {
        //        return mCrossoverProbability;
        //    }
        //    set
        //    {
        //        mCrossoverProbability = value;
        //    }
        //}

        //[CategoryAttribute("遗传算法参数"), DescriptionAttribute("变异概率")]
        //public double MutationProbability
        //{
        //    get
        //    {
        //        return mMutationProbability;
        //    }
        //    set
        //    {
        //        mMutationProbability = value;
        //    }
        //}

        #region IComponentParameter 成员
          [CategoryAttribute("Network Training"), Browsable(false)]
        public Heiflow.Models.AI.IComponent Component
        {
            get;
            set;
        }

        #endregion
    }
}
