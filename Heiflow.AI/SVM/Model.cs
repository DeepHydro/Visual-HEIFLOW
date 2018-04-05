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
using System.IO;
using System.Threading;
using System.Globalization;

namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Encapsulates an  Heiflow.AI.SVM Model.
    /// </summary>
	[Serializable]
	public class Model
	{
        private Parameter _parameter;
        private int _numberOfClasses;
        private int _supportVectorCount;
        private Node[][] _supportVectors;
        private double[][] _supportVectorCoefficients;
        private double[] _rho;
        private double[] _pairwiseProbabilityA;
        private double[] _pairwiseProbabilityB;

        private int[] _classLabels;
        private int[] _numberOfSVPerClass;

        internal Model()
        {
        }

        /// <summary>
        /// Parameter object.
        /// </summary>
        public Parameter Parameter
        {
            get
            {
                return _parameter;
            }
            set
            {
                _parameter = value;
            }
        }

        /// <summary>
        /// Number of classes in the model.
        /// </summary>
        public int NumberOfClasses
        {
            get
            {
                return _numberOfClasses;
            }
            set
            {
                _numberOfClasses = value;
            }
        }

        /// <summary>
        /// Total number of support vectors.
        /// </summary>
        public int SupportVectorCount
        {
            get
            {
                return _supportVectorCount;
            }
            set
            {
                _supportVectorCount = value;
            }
        }

        /// <summary>
        /// The support vectors.
        /// </summary>
        public Node[][] SupportVectors
        {
            get
            {
                return _supportVectors;
            }
            set
            {
                _supportVectors = value;
            }
        }

        /// <summary>
        /// The coefficients for the support vectors.
        /// </summary>
        public double[][] SupportVectorCoefficients
        {
            get
            {
                return _supportVectorCoefficients;
            }
            set
            {
                _supportVectorCoefficients = value;
            }
        }

        /// <summary>
        /// Rho values.
        /// </summary>
        public double[] Rho
        {
            get
            {
                return _rho;
            }
            set
            {
                _rho = value;
            }
        }

        /// <summary>
        /// First pairwise probability.
        /// </summary>
        public double[] PairwiseProbabilityA
        {
            get
            {
                return _pairwiseProbabilityA;
            }
            set
            {
                _pairwiseProbabilityA = value;
            }
        }

        /// <summary>
        /// Second pairwise probability.
        /// </summary>
        public double[] PairwiseProbabilityB
        {
            get
            {
                return _pairwiseProbabilityB;
            }
            set
            {
                _pairwiseProbabilityB = value;
            }
        }
		
		// for classification only

        /// <summary>
        /// Class labels.
        /// </summary>
        public int[] ClassLabels
        {
            get
            {
                return _classLabels;
            }
            set
            {
                _classLabels = value;
            }
        }

        /// <summary>
        /// Number of support vectors per class.
        /// </summary>
        public int[] NumberOfSVPerClass
        {
            get
            {
                return _numberOfSVPerClass;
            }
            set
            {
                _numberOfSVPerClass = value;
            }
        }

        /// <summary>
        /// Reads a Model from the provided file.
        /// </summary>
        /// <param name="filename">The name of the file containing the Model</param>
        /// <returns>the Model</returns>
        public static Model Read(string filename)
        {
            FileStream input = File.OpenRead(filename);
            try
            {
                return Read(input);
            }
            finally
            {
                input.Close();
            }
        }

        /// <summary>
        /// Reads a Model from the provided stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the Model.</param>
        /// <returns>the Model</returns>
        public static Model Read(Stream stream)
        {
            TemporaryCulture.Start();

            StreamReader input = new StreamReader(stream);

            // read parameters

            Model model = new Model();
            Parameter param = new Parameter();
            model.Parameter = param;
            model.Rho = null;
            model.PairwiseProbabilityA = null;
            model.PairwiseProbabilityB = null;
            model.ClassLabels = null;
            model.NumberOfSVPerClass = null;

            bool headerFinished = false;
            while (!headerFinished)
            {
                string line = input.ReadLine();
                string cmd, arg;
                int splitIndex = line.IndexOf(' ');
                if (splitIndex >= 0)
                {
                    cmd = line.Substring(0, splitIndex);
                    arg = line.Substring(splitIndex + 1);
                }
                else
                {
                    cmd = line;
                    arg = "";
                }
                arg = arg.ToLower();

                int i,n;
                switch(cmd){
                    case "svm_type":
                        param.SvmType = (SvmType)Enum.Parse(typeof(SvmType), arg.ToUpper());
                        break;
                        
                    case "kernel_type":
                        param.KernelType = (KernelType)Enum.Parse(typeof(KernelType), arg.ToUpper());
                        break;

                    case "degree":
                        param.Degree = int.Parse(arg);
                        break;

                    case "gamma":
                        param.Gamma = double.Parse(arg);
                        break;

                    case "coef0":
                        param.Coefficient0 = double.Parse(arg);
                        break;

                    case "nr_class":
                        model.NumberOfClasses = int.Parse(arg);
                        break;

                    case "total_sv":
                        model.SupportVectorCount = int.Parse(arg);
                        break;

                    case "rho":
                        n = model.NumberOfClasses * (model.NumberOfClasses - 1) / 2;
                        model.Rho = new double[n];
                        string[] rhoParts = arg.Split();
                        for(i=0; i<n; i++)
                            model.Rho[i] = double.Parse(rhoParts[i]);
                        break;

                    case "label":
                        n = model.NumberOfClasses;
                        model.ClassLabels = new int[n];
                        string[] labelParts = arg.Split();
                        for (i = 0; i < n; i++)
                            model.ClassLabels[i] = int.Parse(labelParts[i]);
                        break;

                    case "probA":
                        n = model.NumberOfClasses * (model.NumberOfClasses - 1) / 2;
                        model.PairwiseProbabilityA = new double[n];
                            string[] probAParts = arg.Split();
                        for (i = 0; i < n; i++)
                            model.PairwiseProbabilityA[i] = double.Parse(probAParts[i]);
                        break;

                    case "probB":
                        n = model.NumberOfClasses * (model.NumberOfClasses - 1) / 2;
                        model.PairwiseProbabilityB = new double[n];
                        string[] probBParts = arg.Split();
                        for (i = 0; i < n; i++)
                            model.PairwiseProbabilityB[i] = double.Parse(probBParts[i]);
                        break;

                    case "nr_sv":
                        n = model.NumberOfClasses;
                        model.NumberOfSVPerClass = new int[n];
                        string[] nrsvParts = arg.Split();
                        for (i = 0; i < n; i++)
                            model.NumberOfSVPerClass[i] = int.Parse(nrsvParts[i]);
                        break;

                    case "SV":
                        headerFinished = true;
                        break;

                    default:
                        throw new Exception("Unknown text in model file");  
                }
            }

            // read sv_coef and SV

            int m = model.NumberOfClasses - 1;
            int l = model.SupportVectorCount;
            model.SupportVectorCoefficients = new double[m][];
            for (int i = 0; i < m; i++)
            {
                model.SupportVectorCoefficients[i] = new double[l];
            }
            model.SupportVectors = new Node[l][];

            for (int i = 0; i < l; i++)
            {
                string[] parts = input.ReadLine().Trim().Split();

                for (int k = 0; k < m; k++)
                    model.SupportVectorCoefficients[k][i] = double.Parse(parts[k]);
                int n = parts.Length-m;
                model.SupportVectors[i] = new Node[n];
                for (int j = 0; j < n; j++)
                {
                    string[] nodeParts = parts[m + j].Split(':');
                    model.SupportVectors[i][j] = new Node();
                    model.SupportVectors[i][j].Index = int.Parse(nodeParts[0]);
                    model.SupportVectors[i][j].Value = double.Parse(nodeParts[1]);
                }
            }

            TemporaryCulture.Stop();

            return model;
        }

        /// <summary>
        /// Writes a model to the provided filename.  This will overwrite any previous data in the file.
        /// </summary>
        /// <param name="filename">The desired file</param>
        /// <param name="model">The Model to write</param>
        public static void Write(string filename, Model model)
        {
            FileStream stream = File.Open(filename, FileMode.Create);
            try
            {
                Write(stream, model);
            }
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// Writes a model to the provided stream.
        /// </summary>
        /// <param name="stream">The output stream</param>
        /// <param name="model">The model to write</param>
        public static void Write(Stream stream, Model model)
        {
            TemporaryCulture.Start();

            StreamWriter output = new StreamWriter(stream);

            Parameter param = model.Parameter;

            output.Write("svm_type " + param.SvmType + "\n");
            output.Write("kernel_type " + param.KernelType + "\n");

            if (param.KernelType == KernelType.POLY)
                output.Write("degree " + param.Degree + "\n");

            if (param.KernelType == KernelType.POLY || param.KernelType == KernelType.RBF || param.KernelType == KernelType.SIGMOID)
                output.Write("gamma " + param.Gamma + "\n");

            if (param.KernelType == KernelType.POLY || param.KernelType == KernelType.SIGMOID)
                output.Write("coef0 " + param.Coefficient0 + "\n");

            int nr_class = model.NumberOfClasses;
            int l = model.SupportVectorCount;
            output.Write("nr_class " + nr_class + "\n");
            output.Write("total_sv " + l + "\n");

            {
                output.Write("rho");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    output.Write(" " + model.Rho[i]);
                output.Write("\n");
            }

            if (model.ClassLabels != null)
            {
                output.Write("label");
                for (int i = 0; i < nr_class; i++)
                    output.Write(" " + model.ClassLabels[i]);
                output.Write("\n");
            }

            if (model.PairwiseProbabilityA != null)
            // regression has probA only
            {
                output.Write("probA");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    output.Write(" " + model.PairwiseProbabilityA[i]);
                output.Write("\n");
            }
            if (model.PairwiseProbabilityB != null)
            {
                output.Write("probB");
                for (int i = 0; i < nr_class * (nr_class - 1) / 2; i++)
                    output.Write(" " + model.PairwiseProbabilityB[i]);
                output.Write("\n");
            }

            if (model.NumberOfSVPerClass != null)
            {
                output.Write("nr_sv");
                for (int i = 0; i < nr_class; i++)
                    output.Write(" " + model.NumberOfSVPerClass[i]);
                output.Write("\n");
            }

            output.Write("SV\n");
            double[][] sv_coef = model.SupportVectorCoefficients;
            Node[][] SV = model.SupportVectors;

            for (int i = 0; i < l; i++)
            {
                for (int j = 0; j < nr_class - 1; j++)
                    output.Write(sv_coef[j][i] + " ");

                Node[] p = SV[i];
                if (p.Length == 0)
                {
                    output.WriteLine();
                    continue;
                }
                if (param.KernelType == KernelType.PRECOMPUTED)
                    output.Write("0:{0}", (int)p[0].Value);
                else
                {
                    output.Write("{0}:{1}", p[0].Index, p[0].Value);
                    for (int j = 1; j < p.Length; j++)
                        output.Write(" {0}:{1}", p[j].Index, p[j].Value);
                }
                output.WriteLine();
            }

            output.Flush();

            TemporaryCulture.Stop();
        }
	}
}