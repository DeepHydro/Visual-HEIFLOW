using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.AI.NeuronDotNet.Core;
using Heiflow.AI.NeuronDotNet.Core.Backpropagation;
using Heiflow.AI.NeuronDotNet.Core.Initializers;
using Heiflow.AI.SVM;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Core.DataDriven;
using Heiflow.Models.AI;
using Heiflow.Spatial.SpatialRelation;

namespace Heiflow.Tools.MachineLearning
{
    public class ANN : MLModelTool
    {
        public ANN()
        {
            Name = "Artificial Neural Network";
            Category = "Machine Learning";
            Description = "Train a Artificial Neural Network and use it to forecast variable";
            Version = "1.0.0.0";
            Author = "Yong Tian";
            _MLMode = MLMode.Train;
            Parameter = new AnnModelParameter();
            Maximum = 50;
            Minimum = 0;
        }
        private BackpropagationNetwork network;
        private int _count = 0;

        [Category("Training")]
        [Description("The model parameter")]
        [EditorAttribute(typeof(PropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public AnnModelParameter Parameter
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var m1 = Validate(InputTraningDC);
            var m2 = Validate(OutputTraningDC);
            this.Initialized = m1 && m2;
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            _cancelProgressHandler = cancelProgressHandler;
            _count = 0;
            if (Mode == MLMode.Train)
            {
                var inputdc = Get3DMat(InputTraningDC);
                var outputdc = Get3DMat(OutputTraningDC);

                if (inputdc.Size[2] == outputdc.Size[2])
                {
                    double[] yvalue = null;
                    int nsample = inputdc.Size[2];
                    int ninputvar = inputdc.Size[0];
                    LinearLayer inputLayer = new LinearLayer(ninputvar);
                    SigmoidLayer hiddenLayer = new SigmoidLayer(Parameter.HiddenNeuronsCount[0]);
                    SigmoidLayer outputLayer = new SigmoidLayer(1);
                    new BackpropagationConnector(inputLayer, hiddenLayer).Initializer = new RandomFunction(0d, 0.3d);
                    new BackpropagationConnector(hiddenLayer, outputLayer).Initializer = new RandomFunction(0d, 0.3d);
                    network = new BackpropagationNetwork(inputLayer, outputLayer);
                    network.SetLearningRate(Parameter.LearningRate);
                    network.JitterEpoch = Parameter.JitterEpoch;
                    network.JitterNoiseLimit = Parameter.JitterNoiseLimit;
                    network.EndEpochEvent += new TrainingEpochEventHandler(
                       delegate (object senderNetwork, TrainingEpochEventArgs args)
                       {
                   OnRunningEpoch(new AnnModelRunEpochEventArgs(args.TrainingIteration + 1, 0));
                       });

                    network.Learn(ConvertToTrainingSet(inputdc,outputdc,out yvalue), Parameter.Iterations);
                    var trained_output = new double[nsample];
                    var buf = new double[ninputvar];
                    for (int i = 0; i < nsample; i++)
                    {
                        for (int j = 0; j < ninputvar; j++)
                        {
                            buf[j] = inputdc[j, 0, i];
                        }
                        trained_output[i] = Forecast(buf);
                    }
                    cancelProgressHandler.Progress("Package_Tool", 100, "Finished.");
                    WorkspaceView.Plot<double>(yvalue, trained_output, "Observed vs. simulated values", Models.UI.MySeriesChartType.Point);
                    return true;
                }
                else
                {
                    cancelProgressHandler.Progress("Package_Tool", 100, "Failed to process. The numbers of samples in the input and output data cube are not equal.");
                    return false;
                }
            }
            else
            {
                if (network != null)
                {
                    ForecastRaster(cancelProgressHandler);
                    cancelProgressHandler.Progress("Package_Tool", 100, "Finished.");
                    return true;
                }
                else
                {
                    cancelProgressHandler.Progress("Package_Tool", 100, "Warning: you need to train the model at first.");
                    return false;
                }
            }
        }

        public override void AfterExecution(object args)
        {
            WorkspaceView.RefreshChart();
        }

        protected void OnRunningEpoch(ComponentRunEpochEventArgs e)
        {
            int prg = e.TrainingIteration * 100 / Parameter.Iterations;
            if (prg > _count)
            {
                _cancelProgressHandler.Progress("Package_Tool", prg, "Processing " + prg + " %");
                _count++;
            }
        }
        public TrainingSet ConvertToTrainingSet(DataCube<float> inputdc, DataCube<float> outputdc, out double[] yvalue)
        {
            var ninputvar = inputdc.Size[0];
            int nsample = inputdc.Size[2];
            TrainingSet trainingset = new TrainingSet(ninputvar, 1);
            yvalue = new double[nsample];
            for (int i = 0; i < nsample; i++)
            {
                var vec = new double[ninputvar];
                for (int j = 0; j < ninputvar; j++)
                {
                    vec[j] = inputdc[j, 0, i];
                }
                yvalue[i] = outputdc[0, 0, i];
                TrainingSample ts = new TrainingSample(vec, new double[] { outputdc[0, 0, i] });
                trainingset.Add(ts);
            }
            return trainingset;
        }

        protected override double Forecast(double[] inputVector)
        {
            if (network != null)
            {
                return network.Run(inputVector)[0];
            }
            else
            {
                return 0;
            }
        }

      
    }
}
