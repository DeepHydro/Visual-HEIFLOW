// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Heiflow.Models.AI;
  

namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Contains all of the types of  Heiflow.AI.SVM this library can model.
    /// </summary>
    public enum SvmType { 
        /// <summary>
        /// C-SVC.
        /// </summary>
        C_SVC, 
        /// <summary>
        /// nu-SVC.
        /// </summary>
        NU_SVC, 
        /// <summary>
        /// one-class  Heiflow.AI.SVM
        /// </summary>
        ONE_CLASS, 
        /// <summary>
        /// epsilon-SVR
        /// </summary>
        EPSILON_SVR, 
        /// <summary>
        /// nu-SVR
        /// </summary>
        NU_SVR 
    };
    /// <summary>
    /// Contains the various kernel types this library can use.
    /// </summary>
    public enum KernelType { 
        /// <summary>
        /// Linear: u'*v
        /// </summary>
        LINEAR, 
        /// <summary>
        /// Polynomial: (gamma*u'*v + coef0)^degree
        /// </summary>
        POLY, 
        /// <summary>
        /// Radial basis function: exp(-gamma*|u-v|^2)
        /// </summary>
        RBF, 
        /// <summary>
        /// Sigmoid: tanh(gamma*u'*v + coef0)
        /// </summary>
        SIGMOID,
        /// <summary>
        /// Precomputed kernel
        /// </summary>
        PRECOMPUTED,
    };

    /// <summary>
    /// This class contains the various parameters which can affect the way in which an  Heiflow.AI.SVM
    /// is learned.  Unless you know what you are doing, chances are you are best off using
    /// the default values.
    /// </summary>
	[Serializable]
    public class Parameter : ICloneable, IComponentParameter
	{
        private SvmType _svmType;
        private KernelType _kernelType;
        private int _degree;
        private double _gamma;
        private double _coef0;

        private double _cacheSize;
        private double _C;
        private double _eps;

        private Dictionary<int, double> _weights;
        private double _nu;
        private double _p;
        private bool _shrinking;
        private bool _probability;

        /// <summary>
        /// Default Constructor.  Gives good default values to all parameters.
        /// </summary>
        public Parameter()
        {
            _svmType = SvmType.NU_SVR;
            _kernelType = KernelType.LINEAR;
            _degree = 3;
            _gamma = 0; // 1/k
            _coef0 = 0;
            _nu = 0.5;
            _cacheSize = 100;
            _C = 2;
            _eps = 1e-3;
            _p = 0.1;
            _shrinking = true;
            _probability = false;
            _weights = new Dictionary<int, double>();
        }

        [CategoryAttribute("Model Parameter"), DescriptionAttribute("Type of SVM (default NU_SVR)")]°°
        /// <summary>
        /// Type of  Heiflow.AI.SVM (default C-SVC)
        /// </summary>
        public SvmType SvmType
        {
            get
            {
                return _svmType;
            }
            set
            {
                _svmType = value;
            }
        }

         [CategoryAttribute("Model Parameter"), DescriptionAttribute("Type of kernel function (default Polynomial)")]°°
        /// <summary>
        /// Type of kernel function (default Polynomial)
        /// </summary>
        public KernelType KernelType
        {
            get
            {
                return _kernelType;
            }
            set
            {
                if (value == SVM.KernelType.PRECOMPUTED)
                    _kernelType = SVM.KernelType.LINEAR;
                _kernelType = value;
            }
        }

         [CategoryAttribute("Model Parameter"), DescriptionAttribute("Degree in kernel function (default 3)")]°°
        /// <summary>
        /// Degree in kernel function (default 3).
        /// </summary>
        public int Degree
        {
            get
            {
                return _degree;
            }
            set
            {
                _degree = value;
            }
        }

         [CategoryAttribute("Model Parameter"), DescriptionAttribute("Gamma in kernel function (default 1/k)")]
        /// <summary>
        /// Gamma in kernel function (default 1/k)
        /// </summary>
        public double Gamma
        {
            get
            {
                return _gamma;
            }
            set
            {
                _gamma = value;
            }
        }

         [CategoryAttribute("Model Parameter"), DescriptionAttribute("Zeroeth coefficient in kernel function (default 0)")]
        /// <summary>
        /// Zeroeth coefficient in kernel function (default 0)
        /// </summary>
        public double Coefficient0
        {
            get
            {
                return _coef0;
            }
            set
            {
                _coef0 = value;
            }
        }

         [CategoryAttribute("Model Parameter"), Browsable(false), DescriptionAttribute(" Cache memory size in MB (default 100)")]
        /// <summary>
        /// Cache memory size in MB (default 100)
        /// </summary>
        public double CacheSize
        {
            get
            {
                return _cacheSize;
            }
            set
            {
                _cacheSize = value;
            }
        }

        [CategoryAttribute("Model Parameter"), DescriptionAttribute(" Tolerance of termination criterion (default 0.001)")]
        /// <summary>
        /// Tolerance of termination criterion (default 0.001)
        /// </summary>
        public double EPS
        {
            get
            {
                return _eps;
            }
            set
            {
                _eps = value;
            }
        }

         [CategoryAttribute("Model Parameter"), DescriptionAttribute("The parameter C of C-SVC, epsilon-SVR, and nu-SVR (default 1)")]
        /// <summary>
        /// The parameter C of C-SVC, epsilon-SVR, and nu-SVR (default 1)
        /// </summary>
        public double C
        {
            get
            {
                return _C;
            }
            set
            {
                _C = value;
            }
        }

         [CategoryAttribute("Weights"), Browsable(false)]
        /// <summary>
        /// Contains custom weights for class labels.  Default weight value is 1.
        /// </summary>
        public Dictionary<int,double> Weights
        {
            get{
                return _weights;
            }
        }

        [CategoryAttribute("Model Parameter"), DescriptionAttribute("The parameter nu of nu-SVC, one-class  Heiflow.AI.SVM, and nu-SVR (default 0.5)")]
        /// <summary>
        /// The parameter nu of nu-SVC, one-class  Heiflow.AI.SVM, and nu-SVR (default 0.5)
        /// </summary>
        public double Nu
        {
            get
            {
                return _nu;
            }
            set
            {
                _nu = value;
            }
        }

        [CategoryAttribute("Model Parameter"), DescriptionAttribute("The epsilon in loss function of epsilon-SVR (default 0.1)")]
        /// <summary>
        /// The epsilon in loss function of epsilon-SVR (default 0.1)
        /// </summary>
        public double P
        {
            get
            {
                return _p;
            }
            set
            {
                _p = value;
            }
        }

        [CategoryAttribute("Model Parameter"), DescriptionAttribute(" Whether to use the shrinking heuristics")]
        /// <summary>
        /// Whether to use the shrinking heuristics, (default True)
        /// </summary>
        public bool Shrinking
        {
            get
            {
                return _shrinking;
            }
            set
            {
                _shrinking = value;
            }
        }

         [CategoryAttribute("Model Parameter"), DescriptionAttribute(" Whether to train an SVC or SVR model for probability estimates")]
        /// <summary>
        /// Whether to train an SVC or SVR model for probability estimates, (default False)
        /// </summary>
        public bool Probability
        {
            get
            {
                return _probability;
            }
            set
            {
                _probability = value;
            }
        }

        [CategoryAttribute("Support Vectors"), DescriptionAttribute("Total number of support vectors")]
        /// <summary>
         /// Total number of support vectors
        /// </summary>
         public double Count
         {
             get;
             set;
         }

         [CategoryAttribute("Support Vectors"), DescriptionAttribute("The percentage of support vectors")]
         public double Percentage
         {
             get;
             set;
         }

        #region ICloneable Members
        /// <summary>
        /// Creates a memberwise clone of this parameters object.
        /// </summary>
        /// <returns>The clone (as type Parameter)</returns>
        public object Clone()
        {
            return base.MemberwiseClone();
        }

        #endregion

        #region IComponentParameter ≥…‘±
         [CategoryAttribute("Ignor"),  Browsable(false)]
        public Heiflow.Models.AI.IComponent Component
        {
            get;
            set;
        }

        #endregion
    }
}