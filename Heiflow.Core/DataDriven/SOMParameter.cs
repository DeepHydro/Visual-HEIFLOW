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
