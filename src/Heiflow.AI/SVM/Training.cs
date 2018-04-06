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
using System.Collections.Generic;

namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Class containing the routines to train  Heiflow.AI.SVM models.
    /// </summary>
    public static class Training
    {
        /// <summary>
        /// Whether the system will output information to the console during the training process.
        /// </summary>
        public static bool IsVerbose
        {
            get
            {
                return Procedures.IsVerbose;
            }
            set
            {
                Procedures.IsVerbose = value;
            }
        }

        private static double doCrossValidation(Problem problem, Parameter parameters, int nr_fold)
        {
            int i;
            double[] target = new double[problem.Count];
            Procedures.svm_cross_validation(problem, parameters, nr_fold, target);
            int total_correct = 0;
            double total_error = 0;
            double sumv = 0, sumy = 0, sumvv = 0, sumyy = 0, sumvy = 0;
            if (parameters.SvmType == SvmType.EPSILON_SVR || parameters.SvmType == SvmType.NU_SVR)
            {
                for (i = 0; i < problem.Count; i++)
                {
                    double y = problem.Y[i];
                    double v = target[i];
                    total_error += (v - y) * (v - y);
                    sumv += v;
                    sumy += y;
                    sumvv += v * v;
                    sumyy += y * y;
                    sumvy += v * y;
                }
                return (problem.Count * sumvy - sumv * sumy) / (System.Math.Sqrt(problem.Count * sumvv - sumv * sumv) * System.Math.Sqrt(problem.Count * sumyy - sumy * sumy));
            }
            else
                for (i = 0; i < problem.Count; i++)
                    if (target[i] == problem.Y[i])
                        ++total_correct;
            return (double)total_correct / problem.Count;
        }
        /// <summary>
        /// Legacy.  Allows use as if this was svm_train.  See libsvm documentation for details on which arguments to pass.
        /// </summary>
        /// <param name="args"></param>
        [Obsolete("Provided only for legacy compatibility, use the other Train() methods")]
        public static void Train(params string[] args)
        {
            Parameter parameters;
            Problem problem;
            bool crossValidation;
            int nrfold;
            string modelFilename;
            parseCommandLine(args, out parameters, out problem, out crossValidation, out nrfold, out modelFilename);
            if (crossValidation)
                PerformCrossValidation(problem, parameters, nrfold);
            else Model.Write(modelFilename, Train(problem, parameters));
        }

        /// <summary>
        /// Performs cross validation.
        /// </summary>
        /// <param name="problem">The training data</param>
        /// <param name="parameters">The parameters to test</param>
        /// <param name="nrfold">The number of cross validations to use</param>
        /// <returns>The cross validation score</returns>
        public static double PerformCrossValidation(Problem problem, Parameter parameters, int nrfold)
        {
            string error = Procedures.svm_check_parameter(problem, parameters);
            if (error == null)
                return doCrossValidation(problem, parameters, nrfold);
            else throw new Exception(error);
        }

        /// <summary>
        /// Trains a model using the provided training data and parameters.
        /// </summary>
        /// <param name="problem">The training data</param>
        /// <param name="parameters">The parameters to use</param>
        /// <returns>A trained  Heiflow.AI.SVM Model</returns>
        public static Model Train(Problem problem, Parameter parameters)
        {
            string error = Procedures.svm_check_parameter(problem, parameters);

            if (error == null)
                return Procedures.svm_train(problem, parameters);
            else throw new Exception(error);
        }

        private static void parseCommandLine(string[] args, out Parameter parameters, out Problem problem, out bool crossValidation, out int nrfold, out string modelFilename)
        {
            int i;

            parameters = new Parameter();
            // default values

            crossValidation = false;
            nrfold = 0;

            // parse options
            for (i = 0; i < args.Length; i++)
            {
                if (args[i][0] != '-')
                    break;
                ++i;
                switch (args[i - 1][1])
                {

                    case 's':
                        parameters.SvmType = (SvmType)int.Parse(args[i]);
                        break;

                    case 't':
                        parameters.KernelType = (KernelType)int.Parse(args[i]);
                        break;

                    case 'd':
                        parameters.Degree = int.Parse(args[i]);
                        break;

                    case 'g':
                        parameters.Gamma = double.Parse(args[i]);
                        break;

                    case 'r':
                        parameters.Coefficient0 = double.Parse(args[i]);
                        break;

                    case 'n':
                        parameters.Nu = double.Parse(args[i]);
                        break;

                    case 'm':
                        parameters.CacheSize = double.Parse(args[i]);
                        break;

                    case 'c':
                        parameters.C = double.Parse(args[i]);
                        break;

                    case 'e':
                        parameters.EPS = double.Parse(args[i]);
                        break;

                    case 'p':
                        parameters.P = double.Parse(args[i]);
                        break;

                    case 'h':
                        parameters.Shrinking = int.Parse(args[i]) == 1;
                        break;

                    case 'b':
                        parameters.Probability = int.Parse(args[i]) == 1;
                        break;

                    case 'v':
                        crossValidation = true;
                        nrfold = int.Parse(args[i]);
                        if (nrfold < 2)
                        {
                            throw new ArgumentException("n-fold cross validation: n must >= 2");
                        }
                        break;

                    case 'w':
                        parameters.Weights[int.Parse(args[i - 1].Substring(2))] = double.Parse(args[1]);
                        break;

                    default:
                        throw new ArgumentException("Unknown Parameter");
                }
            }

            // determine filenames

            if (i >= args.Length)
                throw new ArgumentException("No input file specified");

            problem = Problem.Read(args[i]);

            if (parameters.Gamma == 0)
                parameters.Gamma = 1.0 / problem.MaxIndex;

            if (i < args.Length - 1)
                modelFilename = args[i + 1];
            else
            {
                int p = args[i].LastIndexOf('/') + 1;
                modelFilename = args[i].Substring(p) + ".model";
            }
        }
    }
}