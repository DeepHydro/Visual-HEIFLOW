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
using System.Drawing;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;
using  Heiflow.AI.NeuronDotNet.Core.SOM.NeighborhoodFunctions;

namespace  Heiflow.AI.NeuronDotNet.Core.SOM
{
    /// <summary>
    /// Kohonen Layer is a layer containing position neurons.
    /// </summary>
    [Serializable]
    public class KohonenLayer : Layer<PositionNeuron>
    {
        private readonly Size size;
        private readonly LatticeTopology topology;
        private bool isRowCircular;
        private bool isColumnCircular;
        private PositionNeuron winner;
        private INeighborhoodFunction neighborhoodFunction;

        /// <summary>
        /// Gets the layer size
        /// </summary>
        /// <value>
        /// Size of the layer (Width is number of columns, and Height is number of rows) (In other
        /// words, width is number of neurons in a row and height is number of neurons in a column)
        /// </value>
        public Size Size
        {
            get { return size; }
        }

        /// <summary>
        /// Gets the lattice topology
        /// </summary>
        /// <value>
        /// Lattice topology of neurons in the layer
        /// </value>
        public LatticeTopology Topology
        {
            get { return topology; }
        }

        /// <summary>
        /// Gets or sets a boolean representing whether the neuron rows are circular
        /// </summary>
        /// <value>
        /// A boolean representing whether the neuron rows are circular
        /// </value>
        public bool IsRowCircular
        {
            get { return isRowCircular; }
            set { isRowCircular = value; }
        }

        /// <summary>
        /// Gets or sets a boolean representing whether the neuron columns are circular
        /// </summary>
        /// <value>
        /// A boolean representing whether the neuron columns are circular
        /// </value>
        public bool IsColumnCircular
        {
            get { return isColumnCircular; }
            set { isColumnCircular = value; }
        }

        /// <summary>
        /// Gets the winner neuron of the layer
        /// </summary>
        /// <value>
        /// Winner Neuron
        /// </value>
        public PositionNeuron Winner
        {
            get { return winner; }
        }

        /// <summary>
        /// Gets or sets the neighborhood function
        /// </summary>
        /// <value>
        /// Neighborhood Function
        /// </value>
        public INeighborhoodFunction NeighborhoodFunction
        {
            get { return neighborhoodFunction; }
            set { neighborhoodFunction = value; }
        }

        /// <summary>
        /// Position Neuron indexer
        /// </summary>
        /// <param name="x">
        /// X-Coordinate of the neuron
        /// </param>
        /// <param name="y">
        /// Y-Coordinate of the neuron
        /// </param>
        /// <returns>
        /// The neuron at given co-ordinates
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If any of the indices are out of range
        /// </exception>
        public PositionNeuron this[int x, int y]
        {
            get { return neurons[x + y * size.Width]; }
        }

        /// <summary>
        /// All neurons in this layer
        /// </summary>
        public PositionNeuron[] PositionNeurons
        {
            get
            {
                return neurons;
            }
        }

        /// <summary>
        /// Creates a linear Kohonen layer
        /// </summary>
        /// <param name="neuronCount">
        /// Number of neurons in the layer
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <c>neuronCount</c> is zero or negative
        /// </exception>
        public KohonenLayer(int neuronCount)
            : this(new Size(neuronCount, 1))
        {
        }

        /// <summary>
        /// Creates a linear Kohonen layer with the given neighborhood function.
        /// </summary>
        /// <param name="neuronCount">
        /// Number of neurons in the layer
        /// </param>
        /// <param name="neighborhoodFunction">
        /// The neighborhood function
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>neighborhoodFunction</c> is <c>null</c>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <c>neuronCount</c> is zero or negative
        /// </exception>
        public KohonenLayer(int neuronCount, INeighborhoodFunction neighborhoodFunction)
            : this(new Size(neuronCount, 1), neighborhoodFunction)
        {
        }

        /// <summary>
        /// Creates a Kohonen Layer with the given size
        /// </summary>
        /// <param name="size">
        /// Size of the layer
        /// </param>
        /// <exception cref="ArgumentException">
        /// If layer width or layer height is not positive
        /// </exception>
        public KohonenLayer(Size size)
            : this(size, new GaussianFunction(System.Math.Max(size.Width, size.Height) / 2))
        {
        }

        /// <summary>
        /// Creates a Kohonen layer with the specified size and neighborhood function
        /// </summary>
        /// <param name="size">
        /// Size of the layer
        /// </param>
        /// <param name="neighborhoodFunction">
        /// Neighborhood function to use
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>neighborhoodFunction</c> is <c>null</c>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If layer width or layer height is not positive
        /// </exception>
        public KohonenLayer(Size size, INeighborhoodFunction neighborhoodFunction)
            : this(size, neighborhoodFunction, LatticeTopology.Rectangular)
        {
        }

        /// <summary>
        /// Creates a Kohonen layer with the specified size and topology
        /// </summary>
        /// <param name="size">
        /// Size of the layer
        /// </param>
        /// <param name="topology">
        /// Lattice topology of neurons
        /// </param>
        /// <exception cref="ArgumentException">
        /// If layer width or layer height is not positive, or if <c>topology</c> is invalid
        /// </exception>
        public KohonenLayer(Size size, LatticeTopology topology)
            : this(size, new GaussianFunction(System.Math.Max(size.Width, size.Height) / 2), topology)
        {
        }

        /// <summary>
        /// Creates a Kohonen layer with the specified size, topology and neighborhood function
        /// </summary>
        /// <param name="size">
        /// Size of the layer
        /// </param>
        /// <param name="neighborhoodFunction">
        /// Neighborhood function to use
        /// </param>
        /// <param name="topology">
        /// Lattice topology of neurons
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>neighborhoodFunction</c> is <c>null</c>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If layer width or layer height is not positive, or if <c>topology</c> is invalid
        /// </exception>
        public KohonenLayer(Size size, INeighborhoodFunction neighborhoodFunction, LatticeTopology topology)
            : base(size.Width * size.Height)
        {
            // The product can be positive when both width and height are negative. So, we need to check one.
            Helper.ValidatePositive(size.Width, "size.Width");

            Helper.ValidateNotNull(neighborhoodFunction, "neighborhoodFunction");
            Helper.ValidateEnum(typeof(LatticeTopology), topology, "topology");
            
            this.size = size;
            this.neighborhoodFunction = neighborhoodFunction;
            this.topology = topology;

            int k = 0;
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    neurons[k++] = new PositionNeuron(new Point(x, y), this);
                }
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
        public KohonenLayer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.size.Height = info.GetInt32("size.Height");
            this.size.Width = info.GetInt32("size.Width");

            this.topology = (LatticeTopology)info.GetValue("topology", typeof(LatticeTopology));
            
            this.neighborhoodFunction
                = info.GetValue("neighborhoodFunction", typeof(INeighborhoodFunction))
                as INeighborhoodFunction;

            this.isRowCircular = info.GetBoolean("isRowCircular");
            this.isColumnCircular = info.GetBoolean("isColumnCircular");
        }

        /// <summary>
        /// Populates the serialization info with the data needed to serialize the layer
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
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("topology", topology);
            info.AddValue("size.Height", size.Height);
            info.AddValue("size.Width", size.Width);
            info.AddValue("neighborhoodFunction", neighborhoodFunction, typeof(INeighborhoodFunction));
            info.AddValue("isRowCircular", isRowCircular);
            info.AddValue("isColumnCircular", isColumnCircular);
        }

        /// <summary>
        /// Initializes all neurons and makes them ready to undergo fresh training.
        /// </summary>
        public override void Initialize()
        {
            //Since there are no initializable parameters in this layer, this is a do-nothing function
        }

        /// <summary>
        /// Runs all neurons in the layer and finds the winner
        /// </summary>
        public override void Run()
        {
            this.winner = neurons[0];
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].Run();
                if (neurons[i].value < winner.value)
                {
                    winner = neurons[i];
                }
            }
        }

        /// <summary>
        /// All neurons and their source connectors are allowed to learn. This method assumes a
        /// learning environment where inputs, outputs and other parameters (if any) are appropriate.
        /// </summary>
        /// <param name="currentIteration">
        /// Current learning iteration
        /// </param>
        /// <param name="trainingEpochs">
        /// Total number of training epochs
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <c>trainingEpochs</c> is zero or negative
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <c>currentIteration</c> is negative or, if it is not less than <c>trainingEpochs</c>
        /// </exception>
        public override void Learn(int currentIteration, int trainingEpochs)
        {
            // Validation Delegated
            neighborhoodFunction.EvaluateNeighborhood(this, currentIteration, trainingEpochs);
            base.Learn(currentIteration, trainingEpochs);
        }

        /// <summary>
        /// Find the winner neuron. The distance between the weight vector of the winner neuron and the input vector is minimal. 
        /// </summary>
        /// <param name="pattern">input vector</param>
        /// <returns>the winner neuron</returns>
        public PositionNeuron FindWinner(double [] pattern)
        {       
            PositionNeuron winner = neurons[0];
            double min = 0;

            int i = 0;
            foreach (PositionNeuron p in neurons)
            {
                IList<ISynapse> synapses = p.SourceSynapses;
                var weights = (from s in synapses select s.Weight).ToArray();
                double newmin = CalculateNormOfVectors(pattern, weights);
                if (i == 0)
                {
                    min = newmin;
                    winner = p;
                }
                else
                {
                    if (newmin < min)
                        winner = p;
                }
                i++;
            }      
            return winner;
        }

        public int[,] CalculateWinnerFreqency(TrainingSet tas,out List<KohonenMapClassification> list)
        {
            int[,] result = new int[size.Width, size.Height];
            List<string> listNames = new List<string>();
          //  if (list == null)
            list = new List<KohonenMapClassification>();
            int i = 0;
            foreach (TrainingSample ts in tas.TrainingSamples)
            {
                PositionNeuron p = FindWinner(ts.InputVector);
                result[p.Coordinate.X , p.Coordinate.Y ]++;
                string clsname = "X" + p.Coordinate.X.ToString() + "Y" + p.Coordinate.Y.ToString();
                if (listNames.Contains(clsname))
                {
                    var k = from c in list where c.ClassName == clsname select c;
                    k.First().ClassifiedInputPatternIndex.Add(i);
                }
                else
                {
                    listNames.Add(clsname);
                    KohonenMapClassification mapcls = new KohonenMapClassification(clsname)
                    {
                        X = p.Coordinate.X,
                        Y = p.Coordinate.Y
                    };
                    mapcls.ClassifiedInputPatternIndex.Add(i);
                    list.Add(mapcls);
                }
                i++;
            }
            ColorMap cmap = new ColorMap(Color.Blue, Color.Red);
            Color[] colors = cmap.GenerateUniqueColors(list.Count);
            i = 0;
            foreach (KohonenMapClassification cls in list)
            {
                cls.ColorIndicator = colors[i];
                i++;
            }
            return result;
        }

      


       private double CalculateNormOfVectors(double [] vector1, double [] vector2)
        {
            double value = 0;
            for (int i = 0; i < vector1.Length; i++)
                value += System.Math.Pow((vector1[i] - vector2[i]), 2);
            value = System.Math.Sqrt(value);
            return value;
        }
    }
}