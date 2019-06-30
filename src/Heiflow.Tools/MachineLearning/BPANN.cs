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
using Heiflow.AI.Neuro;
using Heiflow.AI.Neuro.Learning;
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
    public class BPANN : MLModelTool
    {
        public BPANN()
        {
            Name = "BP Artificial Neural Network";
            Category = "Machine Learning";
            Description = "Train a BP Artificial Neural Network and use it to forecast variable";
            Version = "1.0.0.0";
            Author = "Yong Tian";
            _MLMode = MLMode.Train;
            Parameter = new AnnModelParameter();
            Maximum = 50;
            Minimum = 0;
        }
        private volatile bool mNeedToStop;
        private ActivationNetwork mNetwork;
        LayerWeight[] LayerWeightCollection { get; set; }


        [Category("Training")]
        [Description("The model parameter")]
        [EditorAttribute(typeof(PropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public AnnModelParameter Parameter
        {
            get;
            set;
        }
        [Browsable(false)]
        public bool NeedToStop
        {
            get
            {
                return mNeedToStop;
            }
            set
            {
                mNeedToStop = value;
            }
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
            if (Mode == MLMode.Train)
            {
                var inputdc = Get3DMat(InputTraningDC);
                var outputdc = Get3DMat(OutputTraningDC);
                double prg = 0;
                int count = 1;

                if (inputdc.Size[2] == outputdc.Size[2])
                {
                    double[] yvalue = null;
                    int nsample = inputdc.Size[2];
                    int ninputvar = inputdc.Size[0];
                    var mAnnModelParameter = Parameter;

                    NeedToStop = false;
                    IActivationFunction actFunc = null;
                    double sigmoidAlphaValue = mAnnModelParameter.SigmoidAlphaValue;
                    if (mAnnModelParameter.HiddenActivationFunction == fann_activationfunc_enum.FANN_SIGMOID_SYMMETRIC)
                        actFunc = new BipolarSigmoidFunction(sigmoidAlphaValue);
                    else if (mAnnModelParameter.HiddenActivationFunction == fann_activationfunc_enum.FANN_SIGMOID)
                        actFunc = new SigmoidFunction(sigmoidAlphaValue);
                    else if (mAnnModelParameter.HiddenActivationFunction == fann_activationfunc_enum.FANN_THRESHOLD)
                        actFunc = new ThresholdFunction();
                    else
                        actFunc = new BipolarSigmoidFunction(sigmoidAlphaValue);

                    mAnnModelParameter.InputNeuronCount = ninputvar;
                    mAnnModelParameter.OutputNeuronCount =1;
                    int inputsCount = mAnnModelParameter.InputNeuronCount;
                    int outputsCount = mAnnModelParameter.OutputNeuronCount;
                    // mAnnModelParameter.HiddenNeuronsCount = new int[1];
                    //      mAnnModelParameter.HiddenNeuronsCount[0] = datasets.InputData[0].Length * 2 + 1;
                    mAnnModelParameter.HiddenCount = 1;

                    int[] neuronsCount = new int[mAnnModelParameter.HiddenNeuronsCount.Length + 1];
                    for (int i = 0; i < mAnnModelParameter.HiddenNeuronsCount.Length; i++)
                    {
                        neuronsCount[i] = mAnnModelParameter.HiddenNeuronsCount[i];
                    }
                    neuronsCount[mAnnModelParameter.HiddenNeuronsCount.Length] = outputsCount;

                    mNetwork = new ActivationNetwork(actFunc, inputsCount, neuronsCount);
                    BackPropagationLearning teacher = new BackPropagationLearning(mNetwork);
                    AI.Neuro.ActivationLayer layer = mNetwork[0];
                    teacher.LearningRate = mAnnModelParameter.LearningRate;
                    teacher.Momentum = mAnnModelParameter.LearningMomentum;

                    List<double> arError = new List<double>();
                    double[][] input, output;
                    ConvertToTrainingSet(inputdc, outputdc, out input, out output,out yvalue);
                    var trained_output = new double[nsample];
                    int iteration = 1;
                    while (!mNeedToStop)
                    {
                        double error = teacher.RunEpoch(input, output);
                        arError.Add(error);

                        double learningError = 0.0;
                        double predictionError = 0.0;

                        for (int i = 0, n = nsample; i < n; i++)
                        {
                            trained_output[i] = mNetwork.Compute(input[i])[0];

                            if (i >= n - mAnnModelParameter.MaximumWindowSize)
                            {
                                predictionError += System.Math.Abs(output[i][0] - trained_output[i]);
                            }
                            else
                            {
                                learningError += System.Math.Abs(output[i][0] - trained_output[i]);
                            }
                        }
                        if (iteration >= mAnnModelParameter.Iterations)
                        {
                            NeedToStop = true;
                        }
                        if (learningError <= mAnnModelParameter.DesiredError)
                        {
                            NeedToStop = true;
                        }
                        prg = iteration * 100.0 / mAnnModelParameter.Iterations;
                        if (prg > count)
                        {
                            cancelProgressHandler.Progress("Package_Tool", (int)prg, "Processing iteration: " + iteration);
                            count++;
                        }
                        iteration++;
                    }

                    LayerWeightCollection = new LayerWeight[mNetwork.LayersCount];
                    LayerWeightCollection[0].Weight = new double[layer.NeuronsCount][];
                    LayerWeightCollection[0].ThreashHold = new double[layer.NeuronsCount][];
                    for (int i = 0; i < layer.NeuronsCount; i++)
                    {
                        LayerWeightCollection[0].Weight[i] = new double[layer.InputsCount];
                        LayerWeightCollection[0].ThreashHold[i] = new double[layer.InputsCount];
                        for (int j = 0; j < layer.InputsCount; j++)
                        {
                            LayerWeightCollection[0].Weight[i][j] = layer[i][j];
                            LayerWeightCollection[0].ThreashHold[i][j] = layer[i][j];
                        }
                    }

                    layer = mNetwork[1];
                    LayerWeightCollection[1].Weight = new double[layer.NeuronsCount][];
                    LayerWeightCollection[1].ThreashHold = new double[layer.NeuronsCount][];
                    for (int i = 0; i < layer.NeuronsCount; i++)
                    {
                        LayerWeightCollection[1].Weight[i] = new double[layer.InputsCount];
                        LayerWeightCollection[1].ThreashHold[i] = new double[layer.InputsCount];
                        for (int j = 0; j < layer.InputsCount; j++)
                        {
                            LayerWeightCollection[1].Weight[i][j] = layer[i][j];
                            LayerWeightCollection[1].ThreashHold[i][j] = layer[i][j];
                        }
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
                if (mNetwork != null)
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

        public void ConvertToTrainingSet(DataCube<float> inputdc, DataCube<float> outputdc, out double[][] input, out double[][] output, out double[] yvalue)
        {
            var ninputvar = inputdc.Size[0];
            int nsample = inputdc.Size[2];
            input = new double[nsample][];
            output = new double[nsample][];
            yvalue = new double[nsample];
            for (int i = 0; i < nsample; i++)
            {
                input[i] = new double[ninputvar];
                for (int j = 0; j < ninputvar; j++)
                {
                    input[i][j] = inputdc[j, 0, i];
                }
                yvalue[i] = outputdc[0, 0, i];
                output[i] = new double[] { outputdc[0, 0, i] };
            }
        }

        protected override double Forecast(double[] inputVector)
        {
            if (mNetwork != null)
            {
                return mNetwork.Compute(inputVector)[0];
            }
            else
            {
                return 0;
            }
        }

      
    }
}
