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
using Heiflow.AI.SVM;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Spatial.SpatialRelation;

namespace Heiflow.Tools.MachineLearning
{
    public class SVM4RS : MLModelTool
    {
        public SVM4RS()
        {
            Name = "Support vector machine";
            Category = "Machine Learning";
            Description = "Train a support vector machine and use it to forecast variable";
            Version = "1.0.0.0";
            Author = "Yong Tian";
            _MLMode = MLMode.Train;
            Parameter = new Parameter();
            Maximum = 50;
            Minimum = 0;
        }
        private Model _trainedModel;
        private RangeTransform mRange;
    
        [Category("Training")]
        [Description("The model parameter")]
        [EditorAttribute(typeof(PropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Parameter Parameter
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
            if (Mode == MLMode.Train)
            {
                var inputdc = Get3DMat(InputTraningDC);
                var outputdc = Get3DMat(OutputTraningDC);

                if (inputdc.Size[2] == outputdc.Size[2])
                {
                    double[] yvalue = null;
                    int nsample = inputdc.Size[2];
                    int ninputvar = inputdc.Size[0];
                    Node[][] nodes = CreateNodes(inputdc, outputdc, out yvalue);
                    Problem problem = new Problem(nodes.Length, yvalue, nodes, ninputvar);
                    mRange = Scaling.DetermineRange(problem);
                    problem = Scaling.Scale(mRange, problem);
                    cancelProgressHandler.Progress("Package_Tool", 10, "Begin to train.");
                    _trainedModel = Training.Train(problem, Parameter);
                    cancelProgressHandler.Progress("Package_Tool", 90, "Train finished.");
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
                    Parameter.Count = _trainedModel.SupportVectorCount;
                    Parameter.Percentage = _trainedModel.SupportVectorCount / (double)problem.Count;
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
                if (_trainedModel != null)
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

        private Node[][] CreateNodes(DataCube<float> inputdc, DataCube<float> outputdc, out double[] yvalue)
        {
            Node[][] nodes;
            int rows = inputdc.Size[2]; // number of samples
            int columns = inputdc.Size[0]; // number of variables
            nodes = new Node[rows][];
            yvalue = new double[rows];
            for (int r = 0; r < rows; r++)
            {
                nodes[r] = new Node[columns];
                yvalue[r] = outputdc[0, 0, r];
                for (int c = 0; c < columns; c++)
                {
                    nodes[r][c] = new Node();
                    nodes[r][c].Index = c + 1;
                    nodes[r][c].Value = inputdc[c, 0, r];
                }
            }
            return nodes;
        }

        protected override double Forecast(double[] vector)
        {
            double result = 0;
            if (_trainedModel != null && vector != null)
            {
                Node[] node = new Node[vector.Length];
                for (int i = 0; i < vector.Length; i++)
                {
                    node[i] = new Node();
                    node[i].Index = i + 1;
                    node[i].Value = vector[i];
                }
                node = mRange.Transform(node);
                result = Prediction.Predict(_trainedModel, node);
            }
            return result;
        }

    }
}
