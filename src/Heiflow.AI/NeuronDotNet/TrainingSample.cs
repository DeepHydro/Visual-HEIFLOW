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
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// This class represents a training sample used to train a neural network
    /// </summary>
    [Serializable]
    public class TrainingSample : ISerializable
    {
        private readonly double[] inputVector;
        private readonly double[] outputVector;
        private readonly double[] normalizedInputVector;
        private readonly double[] normalizedOutputVector;
        private readonly int hashCode;

        /// <summary>
        /// Gets the value of input vector.
        /// </summary>
        /// <value>
        /// Input vector. It is never <c>null</c>.
        /// </value>
        public double[] InputVector
        {
            get { return inputVector; }
        }

        /// <summary>
        /// Gets the value of expected output vector 
        /// </summary>
        /// <value>
        /// Output Vector. It is never <c>null</c>.
        /// </value>
        public double[] OutputVector
        {
            get { return outputVector; }
        }

        /// <summary>
        /// Gets the value of input vector in normalized form
        /// </summary>
        /// <value>
        /// Normalized Input Vector. It is never <c>null</c>.
        /// </value>
        public double[] NormalizedInputVector
        {
            get { return normalizedInputVector; }
        }

        /// <summary>
        /// Gets the value of output vector in normalized form.
        /// </summary>
        /// <value>
        /// Normalized Output Vector. It is never <c>null</c>.
        /// </value>
        public double[] NormalizedOutputVector
        {
            get { return normalizedOutputVector; }
        }

        /// <summary>
        /// Creates a new unsupervised training sample
        /// </summary>
        /// <param name="vector">
        /// The vector representing the unsupervised training sample
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If vector is <c>null</c>
        /// </exception>
        public TrainingSample(double[] vector)
            : this(vector, new double[0])
        {
        }

        /// <summary>
        /// Creates a new training sample. The arguments are cloned into the training sample. So
        /// any modifications to the arguments will NOT be reflected in the training sample.
        /// </summary>
        /// <param name="inputVector">
        /// Input vector
        /// </param>
        /// <param name="outputVector">
        /// Expected output vector
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If any of the arguments is <c>null</c>
        /// </exception>
        public TrainingSample(double[] inputVector, double[] outputVector)
        {
            // Validate
            Helper.ValidateNotNull(inputVector, "inputVector");
            Helper.ValidateNotNull(outputVector, "outputVector");

            // Clone and initialize
            this.inputVector = (double[])inputVector.Clone();
            this.outputVector = (double[])outputVector.Clone();

            // Some neural networks require inputs in normalized form.
            // As an optimization measure, we normalize and store training samples
            this.normalizedInputVector = Helper.Normalize(inputVector);
            this.normalizedOutputVector = Helper.Normalize(outputVector);

            // Calculate the hash code
            hashCode = 0;
            for (int i = 0; i < inputVector.Length; i++)
            {
                hashCode ^= inputVector[i].GetHashCode();
            }
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
        public TrainingSample(SerializationInfo info, StreamingContext context)
        {
            Helper.ValidateNotNull(info, "info");

            this.inputVector = (double[])info.GetValue("inputVector", typeof(double[]));
            this.outputVector = (double[])info.GetValue("outputVector", typeof(double[]));
            this.normalizedInputVector = Helper.Normalize(inputVector);
            this.normalizedOutputVector = Helper.Normalize(outputVector);

            hashCode = 0;
            for (int i = 0; i < inputVector.Length; i++)
            {
                hashCode ^= inputVector[i].GetHashCode();
            }
        }

        /// <summary>
        /// Populates the serialization info with the data needed to serialize the training sample
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

            info.AddValue("inputVector", inputVector, typeof(double[]));
            info.AddValue("outputVector", outputVector, typeof(double[]));
        }

        /// <summary>
        /// Determine whether the given object is equal to this instance
        /// </summary>
        /// <param name="obj">
        /// The object to compare with this instance
        /// </param>
        /// <returns>
        /// <c>true</c> if the given object is equal to this instance, <c>false</c> otherwise
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is TrainingSample)
            {
                TrainingSample sample = (TrainingSample)obj;
                int size;
                if ((size = sample.inputVector.Length) == inputVector.Length)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (inputVector[i] != sample.inputVector[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type
        /// </summary>
        /// <returns>
        /// The hash code for the current object
        /// </returns>
        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
