// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core.SOM
{
    /// <summary>
    /// This interface represents a neighborhood function. A neighborhood function determines
    /// the neighborhood of every neuron with respect to winner neuron. This function depends
    /// on the the shape of the layer and also on the training progress.
    /// </summary>
    public interface INeighborhoodFunction : ISerializable
    {
        /// <summary>
        /// Determines the neighborhood of every neuron in the given Kohonen layer with respect
        /// to winner neuron.
        /// </summary>
        /// <param name="layer">
        /// The Kohonen Layer containing neurons
        /// </param>
        /// <param name="currentIteration">
        /// Current training iteration
        /// </param>
        /// <param name="trainingEpochs">
        /// Training Epochs
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// If <c>trainingEpochs</c> is zero or negative
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <c>currentIteration</c> is negative or, if it is not less than <c>trainingEpochs</c>
        /// </exception>
        void EvaluateNeighborhood(KohonenLayer layer, int currentIteration, int trainingEpochs);
    }
}
