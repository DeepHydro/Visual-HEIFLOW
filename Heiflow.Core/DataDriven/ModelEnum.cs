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
    public enum AnnLayerType { Input, Hidden, Output };
    /// <summary>
    /// Enums: fann_activationfunc_enum 
    /// </summary>
    public enum fann_activationfunc_enum
    {
        FANN_LINEAR = 0,
        FANN_THRESHOLD,
        FANN_THRESHOLD_SYMMETRIC,
        FANN_SIGMOID,
        FANN_SIGMOID_STEPWISE,
        FANN_SIGMOID_SYMMETRIC,
        FANN_SIGMOID_SYMMETRIC_STEPWISE,
        FANN_GAUSSIAN,
        FANN_GAUSSIAN_SYMMETRIC,
        /* Stepwise linear approximation to gaussian.
         * Faster than gaussian but a bit less precise.
         * NOT implemented yet.
         */
        FANN_GAUSSIAN_STEPWISE,
        FANN_ELLIOT,
        FANN_ELLIOT_SYMMETRIC,
        FANN_LINEAR_PIECE,
        FANN_LINEAR_PIECE_SYMMETRIC
    }
    /// <summary>
    ///  Enum: fann_train_enum
    ///The Training algorithms used when training on <struct fann_train_data> with functions like
    ///fann_train_on_data or fann_train_on_file The incremental training looks alters the weights
    ///after each time it is presented an input pattern, while batch only alters the weights once after
    ///it has been presented to all the patterns.
    /// </summary>
    public enum fann_train_enum
    {
        /// <summary>
        /// Standard backpropagation algorithm, where the weights are 
        ///updated after each training pattern. This means that the weights are updated many 
        ///times during a single epoch. For this reason some problems, will train very fast with 
        ///this algorithm, while other more advanced problems will not train very well.
        /// </summary>
        FANN_TRAIN_INCREMENTAL = 0,
        /// <summary>
        /// Standard backpropagation algorithm, where the weights are updated after 
        ///calculating the mean square error for the whole training set. This means that the weights 
        ///are only updated once during a epoch. For this reason some problems, will train slower with 
        ///this algorithm. But since the mean square error is calculated more correctly than in 
        ///incremental training, some problems will reach a better solutions with this algorithm.
        /// </summary>
        FANN_TRAIN_BATCH,
        /// <summary>
        /// A more advanced batch training algorithm which achieves good results 
        ///for many problems. The RPROP training algorithm is adaptive, and does therefore not 
        ///use the LearningRate. Some other parameters can however be set to change the way the 
        ///RPROP algorithm works, but it is only recommended for users with insight in how the RPROP 
        ///training algorithm works. The RPROP training algorithm is described by 
        ///[Riedmiller and Braun, 1993], but the actual learning algorithm used here is the 
        ///iRPROP- training algorithm which is described by [Igel and Husken, 2000] which 
        ///is an variety of the standard RPROP training algorithm.
        /// </summary>
        FANN_TRAIN_RPROP,
        /// <summary>
        /// A more advanced batch training algorithm which achieves good results 
        ///for many problems. The quickprop training algorithm uses the LearningRate parameter 
        ///along with other more advanced parameters, but it is only recommended to change these 
        ///advanced parameters, for users with insight in how the quickprop training algorithm works.
        ///The quickprop training algorithm is described by [Fahlman, 1988]
        /// </summary>
        FANN_TRAIN_QUICKPROP
    }
}
