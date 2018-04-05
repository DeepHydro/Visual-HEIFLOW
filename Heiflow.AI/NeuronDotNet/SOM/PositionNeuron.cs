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
using System.Drawing;

namespace  Heiflow.AI.NeuronDotNet.Core.SOM
{
    /// <summary>
    /// Position Neuron is a neuron in a two-dimensional space used in Kohonen networks.
    /// </summary>
    public class PositionNeuron : INeuron
    {
        private readonly KohonenLayer parent;
        private readonly Point coordinate;
        private readonly IList<ISynapse> sourceSynapses = new List<ISynapse>();
        private readonly IList<ISynapse> targetSynapses = new List<ISynapse>();

        internal double value;
        internal double neighborhoodValue;

        /// <summary>
        /// Gets the parent layer containing this neuron
        /// </summary>
        /// <value>
        /// The parent layer containing this neuron. It is never <c>null</c>
        /// </value>
        public KohonenLayer Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Gets the neuron value
        /// </summary>
        /// <value>
        /// Neuron Value
        /// </value>
        public double Value
        {
            get { return value; }
        }

        double INeuron.Input
        {
            get { return value; }
            set { this.value = value; }
        }

        double INeuron.Output
        {
            get { return value; }
        }

        ILayer INeuron.Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Gets the position of the neuron
        /// </summary>
        /// <value>
        /// Neuron Co-ordinate
        /// </value>
        public Point Coordinate
        {
            get { return coordinate; }
        }

        /// <summary>
        /// Gets the list of source synapses associated with this neuron
        /// </summary>
        /// <value>
        /// A list of source synapses. It can neither be <c>null</c>, nor contain <c>null</c> elements.
        /// </value>
        public IList<ISynapse> SourceSynapses
        {
            get { return sourceSynapses; }
        }

        /// <summary>
        /// Gets the list of target synapses associated with this neuron
        /// </summary>
        /// <value>
        /// A list of target synapses. It can neither be <c>null</c>, nor contains <c>null</c> elements.
        /// </value>
        public IList<ISynapse> TargetSynapses
        {
            get { return targetSynapses; }
        }

        /// <summary>
        /// Gets the neighborhood of this neuron with respect to the winner neuron in this layer
        /// </summary>
        /// <value>
        /// Neighborhood Value
        /// </value>
        public double NeighborhoodValue
        {
            get { return neighborhoodValue; }
        }

        /// <summary>
        /// Creates new position neuron
        /// </summary>
        /// <param name="x">
        /// X-Coordinate of the neuron positon
        /// </param>
        /// <param name="y">
        /// Y-Coordinate of the neuron position
        /// </param>
        /// <param name="parent">
        /// Parent layer containing this neuron
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>parent</c> is <c>null</c>
        /// </exception>
        public PositionNeuron(int x, int y, KohonenLayer parent)
            : this(new Point(x, y), parent)
        {
        }

        /// <summary>
        /// Creates new positon neuron
        /// </summary>
        /// <param name="coordinate">
        /// Neuron Position
        /// </param>
        /// <param name="parent">
        /// Parent neuron containing this neuron
        /// </param>
        /// <value>
        /// If <c>parent</c> is <c>null</c>
        /// </value>
        public PositionNeuron(Point coordinate, KohonenLayer parent)
        {
            Helper.ValidateNotNull(parent, "parent");

            this.coordinate = coordinate;
            this.parent = parent;
            this.value = 0d;
        }

        /// <summary>
        /// Runs the neuron. (Propagates the source synapses and update input and output values)
        /// </summary>
        public void Run()
        {
            if (sourceSynapses.Count > 0)
            {
                value = 0d;
                for (int i = 0; i < sourceSynapses.Count; i++)
                {
                    sourceSynapses[i].Propagate();
                }
                value = System.Math.Sqrt(value);
            }
        }

        /// <summary>
        /// Trains weights of associated source synapses.
        /// </summary>
        /// <param name="learningRate">
        /// The current learning rate (this depends on training progress as well)
        /// </param>
        public void Learn(double learningRate)
        {
            double learningFactor = learningRate * neighborhoodValue;
            for (int i = 0; i < sourceSynapses.Count; i++)
            {
                sourceSynapses[i].OptimizeWeight(learningFactor);
            }
        }
    }
}