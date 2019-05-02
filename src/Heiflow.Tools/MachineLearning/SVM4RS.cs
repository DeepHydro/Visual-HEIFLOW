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
    public class SVM4RS : ModelTool
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
        }

        private MLMode _MLMode;
        private Model _trainedModel;
        private RangeTransform mRange;

        [Category("Parameter")]
        [Description("The quota filename")]
        public MLMode Mode
        {
            get
            {
                return _MLMode;
            }
            set
            {
                _MLMode = value;
            }
        }

        [Category("Training")]
        [Description("The input data cube. The data cube style should be [n][0][:]")]
        public string InputTraningDC
        {
            get;
            set;
        }
        [Category("Training")]
        [Description("The output data cube. The data cube style should be [n][0][:]")]
        public string OutputTraningDC
        {
            get;
            set;
        }
        [Category("Training")]
        [Description("The model parameter")]
        [EditorAttribute(typeof(PropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public Parameter Parameter
        {
            get;
            set;
        }
 
        [Category("Prediction")]
        [Description("The output data cube. The data cube style should be [n][0][:]")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PredictionFileList
        {
            get;
            set;
        }
        [Category("Prediction")]
        [Description("The mask is represented by a polygon shapefile.")]
        public string Mask
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

        private  double Forecast(double[] vector)
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

        private void ForecastRaster(ICancelProgressHandler cancelProgressHandler)
        {
            var inputdc = Get3DMat(InputTraningDC);
            int ninputvar = inputdc.Size[0];
            IFeatureSet mask = null;
            IRaster output = null;
            int ntask = 0;
            int progress = 0;
            string line = "";
            RcIndex index1;
            IRaster input1 = null;
            List<IRaster> rasters = new List<IRaster>();
            double[] vec = new double[ninputvar];
            Extent envelope = null;

            StreamReader sr = new StreamReader(PredictionFileList);
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (TypeConverterEx.IsNotNull(line))
                    ntask++;
            }
            sr.Close();
            sr = new StreamReader(PredictionFileList);
            if (TypeConverterEx.IsNotNull(Mask) && File.Exists(Mask))
            {
                mask = FeatureSet.Open(Mask);
            }
            var mask_polygons = SpatialRelationship.GetPolygons(mask);
            for (int n = 0; n < ntask; n++)
            {
                line = sr.ReadLine();
                var fns = TypeConverterEx.Split<string>(line, ninputvar + 1);
                cancelProgressHandler.Progress("Package_Tool", progress, "begin to process task " + (n + 1));

                rasters.Clear();
                for (int i = 0; i < ninputvar; i++)
                {
                    var ras= Raster.Open(fns[i]);
                    rasters.Add(ras);
                }
                input1 = rasters[0];
                int nrow = input1.NumRows;
                int ncol = input1.NumColumns;
                if (mask != null)
                {
                    nrow = Convert.ToInt32(System.Math.Abs(mask.Extent.Height / input1.CellHeight));
                    ncol = Convert.ToInt32(System.Math.Abs(mask.Extent.Width / input1.CellWidth));
                    envelope = mask.Extent;
                }
                else
                {
                    envelope = input1.Extent;
                }
                output = Raster.CreateRaster(fns[ninputvar], string.Empty, ncol, nrow, 1, typeof(float), new[] { string.Empty });
                RasterBounds bound = new RasterBounds(nrow, ncol, envelope);
                output.Bounds = bound;
                output.Projection = input1.Projection;
                output.NoDataValue = input1.NoDataValue;
                if (mask != null)
                {
                    for(int i=0;i<nrow;i++)
                    {
                        for (int j = 0; j < ncol; j++)
                        {
                            Coordinate cellCenter = output.CellToProj(i, j);
                            if (SpatialRelationship.PointInPolygon(mask_polygons, cellCenter))
                            {
                                for (int k = 0; k < ninputvar; k++)
                                {
                                    var ras = rasters[k];
                                    index1 = ras.ProjToCell(cellCenter);
                                    if (index1.Row <= ras.EndRow && index1.Column <= ras.EndColumn && index1.Row > -1
                                        && index1.Column > -1)
                                    {
                                        vec[k] = ras.Value[index1.Row, index1.Column] == ras.NoDataValue
                                                  ? 0
                                                  : ras.Value[index1.Row, index1.Column];
                                    }
                                    else
                                    {
                                        vec[k] = 0;
                                    }
                                }
                                output.Value[i, j] = Forecast(vec);
                            }
                            else
                            {
                                output.Value[i, j] = input1.NoDataValue;
                            }
                        }
                    }
                }
                else
                {

                }
                output.Save();
                for (int i = 0; i < ninputvar; i++)
                {
                    rasters[i].Close();
                }
                progress = n * 100 / ntask;
                cancelProgressHandler.Progress("Package_Tool", progress, "Output is saved to: " + fns[ninputvar]);
            }
            sr.Close();
        }

    }
}
