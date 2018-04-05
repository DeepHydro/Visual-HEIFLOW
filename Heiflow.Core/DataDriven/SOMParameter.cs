// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heiflow.AI.NeuronDotNet.Core.SOM;
using System.ComponentModel;
using Heiflow.AI.NeuronDotNet.Core.SOM.NeighborhoodFunctions;
using Heiflow.Models.AI;

namespace Heiflow.Core.DataDriven
{
    public enum SOMNeighborhoodFunctionType { Gaussian, MexicanHat}


    public class SOMParameter:IComponentParameter
    {
        public SOMParameter()
        {
            FinalLearningRate = 0.05;
            LayerHeight = 12;
            LayerWidth = 12;
            LearningRate = 0.2;
            Iterations = 500;
            Topology = LatticeTopology.Rectangular;
            NeighborhoodFunctionType = SOMNeighborhoodFunctionType.Gaussian;
        }
        private SOMNeighborhoodFunctionType mNeighborhoodFunctionType;

        [CategoryAttribute("Model Running"), DescriptionAttribute("An positive integer that indicates the maximum number of running step")]
        public int Iterations
        {
            set;
            get;
        }
        [CategoryAttribute("Model Running"), DescriptionAttribute(" Initial value of learning rate")]
        public double LearningRate
        {
            set;
            get;
        }

        [CategoryAttribute("Model Running"), DescriptionAttribute(" Final value of learning rate")]
        public double FinalLearningRate
        {
            set;
            get;
        }
        
       [CategoryAttribute("Ignor"),Browsable(false)]
        public INeighborhoodFunction NeighborhoodFunction
        {
            set;
            get;
        }
   
        [CategoryAttribute("Structure of Network")]
       public SOMNeighborhoodFunctionType NeighborhoodFunctionType
       {
           set
           {
              mNeighborhoodFunctionType=value;
              int learningRadius = Math.Max(LayerWidth, LayerHeight) / 2;
              if (mNeighborhoodFunctionType == SOMNeighborhoodFunctionType.Gaussian)
                  NeighborhoodFunction = new GaussianFunction(learningRadius);
              else if (mNeighborhoodFunctionType == SOMNeighborhoodFunctionType.MexicanHat)
                  NeighborhoodFunction = new MexicanHatFunction(learningRadius);
           }
           get
           {
               return mNeighborhoodFunctionType;
           }
       }
         [CategoryAttribute("Structure of Network"), DescriptionAttribute("Lattice Topology of the position neurons in a Kohonen Layer")]
        public LatticeTopology Topology
        {
            set;
            get;
        }

        [CategoryAttribute("Structure of Network"), DescriptionAttribute("The width of the output layer map (default 12)")]
        public int LayerWidth
        {
            set;
            get;
        }
          [CategoryAttribute("Structure of Network"), DescriptionAttribute("The height of the output layer map (default 12)")]
        public int LayerHeight
        {
            set;
            get;
        }

        [CategoryAttribute("Structure of Network"), DescriptionAttribute("")]
        public bool IsRowCircular
        {
            set;
            get;
        }
          [CategoryAttribute("Structure of Network"), DescriptionAttribute("")]
        public bool IsColumnCircular
        {
            set;
            get;
        }

        #region IComponentParameter 成员
        [CategoryAttribute("Ignor"), Browsable(false)]
        public Heiflow.Models.AI.IComponent Component
        {
            get;
            set;
        }

        #endregion
    }
}
