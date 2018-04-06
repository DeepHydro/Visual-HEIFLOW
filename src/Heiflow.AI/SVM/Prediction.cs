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
using System.IO;
using System.Diagnostics;

namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Class containing the routines to perform class membership prediction using a trained  Heiflow.AI.SVM.
    /// </summary>
    public static class Prediction
    {
        /// <summary>
        /// Predicts the class memberships of all the vectors in the problem.
        /// </summary>
        /// <param name="problem">The  Heiflow.AI.SVM Problem to solve</param>
        /// <param name="outputFile">File for result output</param>
        /// <param name="model">The Model to use</param>
        /// <param name="predict_probability">Whether to output a distribution over the classes</param>
        /// <returns>Percentage correctly labelled</returns>
        public static double Predict(
            Problem problem,
            string outputFile,
            Model model,
            bool predict_probability)
        {
            int correct = 0;
            int total = 0;
            double error = 0;
            double sumv = 0, sumy = 0, sumvv = 0, sumyy = 0, sumvy = 0;
            StreamWriter output = outputFile != null ? new StreamWriter(outputFile) : null;

            SvmType svm_type = Procedures.svm_get_svm_type(model);
            int nr_class = Procedures.svm_get_nr_class(model);
            int[] labels = new int[nr_class];
            double[] prob_estimates = null;

            if (predict_probability)
            {
                if (svm_type == SvmType.EPSILON_SVR || svm_type == SvmType.NU_SVR)
                {
                    Console.WriteLine("Prob. model for test data: target value = predicted value + z,\nz: Laplace distribution e^(-|z|/sigma)/(2sigma),sigma=" + Procedures.svm_get_svr_probability(model));
                }
                else
                {
                    Procedures.svm_get_labels(model, labels);
                    prob_estimates = new double[nr_class];
                    if (output != null)
                    {
                        output.Write("labels");
                        for (int j = 0; j < nr_class; j++)
                        {
                            output.Write(" " + labels[j]);
                        }
                        output.Write("\n");
                    }
                }
            }
            for (int i = 0; i < problem.Count; i++)
            {
                double target = problem.Y[i];
                Node[] x = problem.X[i];

                double v;
                if (predict_probability && (svm_type == SvmType.C_SVC || svm_type == SvmType.NU_SVC))
                {
                    v = Procedures.svm_predict_probability(model, x, prob_estimates);
                    if (output != null)
                    {
                        output.Write(v + " ");
                        for (int j = 0; j < nr_class; j++)
                        {
                            output.Write(prob_estimates[j] + " ");
                        }
                        output.Write("\n");
                    }
                }
                else
                {
                    v = Procedures.svm_predict(model, x);
                    if(output != null)
                        output.Write(v + "\n");
                }

                if (v == target)
                    ++correct;
                error += (v - target) * (v - target);
                sumv += v;
                sumy += target;
                sumvv += v * v;
                sumyy += target * target;
                sumvy += v * target;
                ++total;
            }
            if(output != null)
                output.Close();
            return (double)correct / total;
        }

        /// <summary>
        /// Predict the class for a single input vector.
        /// </summary>
        /// <param name="model">The Model to use for prediction</param>
        /// <param name="x">The vector for which to predict class</param>
        /// <returns>The result</returns>
        public static double Predict(Model model, Node[] x)
        {
            return Procedures.svm_predict(model, x);
        }

        /// <summary>
        /// Predicts a class distribution for the single input vector.
        /// </summary>
        /// <param name="model">Model to use for prediction</param>
        /// <param name="x">The vector for which to predict the class distribution</param>
        /// <returns>A probability distribtion over classes</returns>
        public static double[] PredictProbability(Model model, Node[] x)
        {
            SvmType svm_type = Procedures.svm_get_svm_type(model);
            if (svm_type != SvmType.C_SVC && svm_type != SvmType.NU_SVC)
                throw new Exception("Model type " + svm_type + " unable to predict probabilities.");
            int nr_class = Procedures.svm_get_nr_class(model);
            double[] probEstimates = new double[nr_class];
            Procedures.svm_predict_probability(model, x, probEstimates);
            return probEstimates;
        }

        private static void exit_with_help()
        {
            Debug.Write("usage: svm_predict [options] test_file model_file output_file\n" + "options:\n" + "-b probability_estimates: whether to predict probability estimates, 0 or 1 (default 0); one-class  Heiflow.AI.SVM not supported yet\n");
            Environment.Exit(1);
        }

        /// <summary>
        /// Legacy method, provided to allow usage as though this were the command line version of libsvm.
        /// </summary>
        /// <param name="args">Standard arguments passed to the svm_predict exectutable.  See libsvm documentation for details.</param>
        [Obsolete("Use the other version of Predict() instead")]
        public static void Predict(params string[] args)
        {
            int i = 0;
            bool predictProbability = false;

            // parse options
            for (i = 0; i < args.Length; i++)
            {
                if (args[i][0] != '-')
                    break;
                ++i;
                switch (args[i - 1][1])
                {

                    case 'b':
                        predictProbability = int.Parse(args[i]) == 1;
                        break;

                    default:
                        throw new ArgumentException("Unknown option");

                }
            }
            if (i >= args.Length)
                throw new ArgumentException("No input, model and output files provided");

            Problem problem = Problem.Read(args[i]);
            Model model = Model.Read(args[i + 1]);
            Predict(problem, args[i + 2], model, predictProbability);
        }
    }
}