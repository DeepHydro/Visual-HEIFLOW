﻿//
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
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// A Training Set represents a set of training samples used during 'batch training' process.
    /// </summary>
    [Serializable]
    public sealed class TrainingSet : ISerializable
    {
        private readonly int inputVectorLength;
        private readonly int outputVectorLength;
        private readonly IList<TrainingSample> trainingSamples;

        /// <summary>
        /// Gets the number of training samples in the set
        /// </summary>
        /// <value>
        /// Number of training samples in the set. This value is never negative.
        /// </value>
        public int TrainingSampleCount
        {
            get { return trainingSamples.Count; }
        }

        /// <summary>
        /// Gets the length of input vector. 
        /// </summary>
        /// <value>
        /// Input Vector Length. This is always positive and is equal to the number of input neurons
        /// in the network to be trained.
        /// </value>
        public int InputVectorLength
        {
            get { return inputVectorLength; }
        }

        /// <summary>
        /// Gets the length of expected output vector.
        /// </summary>
        /// <value>
        /// Output Vector Length. This value is zero for unsupervised training sets. For a supervised
        /// training set, this value is positive and is equal to the number of output neurons in the
        /// network.
        /// </value>
        public int OutputVectorLength
        {
            get { return outputVectorLength; }
        }

        /// <summary>
        /// Exposes an Enumerator to iterator over training samples in the set. 
        /// </summary>
        /// <value>
        /// Training Samples Enumerator. No training sample returned is <c>null</c>.
        /// </value>
        public IEnumerable<TrainingSample> TrainingSamples
        {
            get
            {
                for (int i = 0; i < trainingSamples.Count; i++)
                {
                    yield return trainingSamples[i];
                }
            }
        }

        /// <summary>
        /// Training Sample Indexer
        /// </summary>
        /// <param name="index">
        /// Training sample index
        /// </param>
        /// <returns>
        /// Training sample at the given index
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If the index is out of range
        /// </exception>
        public TrainingSample this[int index]
        {
            get { return trainingSamples[index]; }
        }

        /// <summary>
        /// Creates a new unsupervised training set
        /// </summary>
        /// <param name="vectorSize">
        /// Expected size of the vectors in the training set.
        /// (Note : This should be equal to number of input neurons.)
        /// </param>
        /// <exception cref="ArgumentException">
        /// If vectorSize is zero or negative
        /// </exception>
        public TrainingSet(int vectorSize)
            : this(vectorSize, 0)
        {
        }

        /// <summary>
        /// Creates a new supervised training set
        /// </summary>
        /// <param name="inputVectorLength">
        /// Length of input vector
        /// </param>
        /// <param name="outputVectorLength">
        /// Length of expected output vector (zero for unsupervised training)
        /// </param>
        /// <exception cref="ArgumentException">
        /// If input vector length is zero or negative, or if output vector length is negative
        /// </exception>
        public TrainingSet(int inputVectorLength, int outputVectorLength)
        {
            // Validation
            Helper.ValidatePositive(inputVectorLength, "inputVectorLength");
            Helper.ValidateNotNegative(outputVectorLength, "outputVectorLength");

            // Initialize instance variables
            this.inputVectorLength = inputVectorLength;
            this.outputVectorLength = outputVectorLength;
            this.trainingSamples = new List<TrainingSample>();
        }

        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        /// <param name="info">
        /// Serialization information to deserialize and obtain the data
        /// </param>
        /// <param name="context">
        /// Serialization context to use
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>info</c> is <c>null</c>
        /// </exception>
        public TrainingSet(SerializationInfo info, StreamingContext context)
        {
            Helper.ValidateNotNull(info, "info");

            this.inputVectorLength = info.GetInt32("inputVectorLength");
            this.outputVectorLength = info.GetInt32("outputVectorLength");
            this.trainingSamples = info.GetValue("trainingSamples", typeof(IList<TrainingSample>)) as IList<TrainingSample>;
        }

        /// <summary>
        /// Populates the serialization info with the data needed to serialize the training set
        /// </summary>
        /// <param name="info">
        /// The serialization info to populate the data with
        /// </param>
        /// <param name="context">
        /// The serialization context to use
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>info</c> is <c>null</c>
        /// </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Helper.ValidateNotNull(info, "info");

            info.AddValue("inputVectorLength", inputVectorLength);
            info.AddValue("outputVectorLength", outputVectorLength);
            info.AddValue("trainingSamples", trainingSamples, typeof(IList<TrainingSample>));
        }

        /// <summary>
        /// Adds a new supervised training sample to the training set. If already exists, it will
        /// be replaced.
        /// </summary>
        /// <param name="sample">
        /// The sample to add
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>sample</c> is <c>null</c>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If sizes of input vector or output vector are different from their expected sizes
        /// </exception>
        public void Add(TrainingSample sample)
        {
            // Validation
            Helper.ValidateNotNull(sample, "sample");
            if (sample.InputVector.Length != inputVectorLength)
            {
                throw new ArgumentException
                    ("Input vector must be of size " + inputVectorLength, "sample");
            }
            if (sample.OutputVector.Length != outputVectorLength)
            {
                throw new ArgumentException
                    ("Output vector must be of size " + outputVectorLength, "sample");
            }

            // Note that the reference is being added. (Sample is immutable)
            trainingSamples.Add(sample);
        }

        /// <summary>
        /// Removes the training sample corresponding to the given vector
        /// </summary>
        /// <param name="inputVector">
        /// The input vector of the sample to remove
        /// </param>
        /// <returns>
        /// <c>true</c> if successful, <c>false</c> otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If input vector is <c>null</c>
        /// </exception>
        public bool Remove(double[] inputVector)
        {
            return Remove(new TrainingSample(inputVector));
        }

        /// <summary>
        /// Removes the given training sample
        /// </summary>
        /// <param name="sample">
        /// The sample to remove
        /// </param>
        /// <returns>
        /// <c>true</c> if successful, <c>false</c> otherwise
        /// </returns>
        public bool Remove(TrainingSample sample)
        {
            return trainingSamples.Remove(sample);
        }

        /// <summary>
        /// Determines whether the training set contains a training sample corresponding to given
        /// vector
        /// </summary>
        /// <param name="inputVector">
        /// The input vector of the sample to locate
        /// </param>
        /// <returns>
        /// <c>true</c> if present, <c>false</c> otherwise
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If input vector is <c>null</c>
        /// </exception>
        public bool Contains(double[] inputVector)
        {
            return Contains(new TrainingSample(inputVector));
        }

        /// <summary>
        /// Determines whether the training sample is present in the set
        /// </summary>
        /// <param name="sample">
        /// The sample to locate
        /// </param>
        /// <returns>
        /// <c>true</c> if present, <c>false</c> otherwise
        /// </returns>
        public bool Contains(TrainingSample sample)
        {
            return trainingSamples.Contains(sample);
        }

        /// <summary>
        /// Removes all training samples in the training set.
        /// </summary>
        public void Clear()
        {
            trainingSamples.Clear();
        }
    }
}