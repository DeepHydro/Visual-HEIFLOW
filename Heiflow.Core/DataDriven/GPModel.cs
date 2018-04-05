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
using System.Linq;
using System.Text;

using System.Xml.Linq;

using Heiflow.AI.GeneticProgramming;
using Heiflow.Core;
using System.ComponentModel;
using Heiflow.Models.AI;
using Heiflow.Core.Data;


namespace Heiflow.Core.DataDriven
{
    public class GPModelParameter : IComponentParameter
    {
        public GPModelParameter()
        {
            ConstantsIntervalFrom = 0;
            ConstantsIntervalTo = 10;
            ConstantsNumber = 6;

            //PopulatonInitialization = EInitializationMethod.HalfHalfInitialization;
            //SelectionMethod = ESelectionMethod.Rankselection;
            //Fitness = EFitness.MSE;

            mInitialization = "MyMath.Full";
            mFitnessMethod = "RMSE";
            mSelectionMethodName = "Rank";

            SelectionParam1 = 0;
            MaxInitLevel = 7;
            MaxCossoverLevel = 15;
            MaxMutationLevel = 15;
            PopulationSize = 100;
            Elitism = 10;

            ProbCrossover = 0.9f;
            ProbMutation = 0.05f;
            ProbReproduction = 0.2f;
            ProbPermutation = 0.5f;

            EnvolveIndicator = 0;
            EnvolveConditionValue = 100;

            MultipleCore = false;
        }

        //private EInitializationMethod mPopulatonInitialization;
        //private ESelectionMethod mESelectionMethod;
        //private EFitness mEFitness;

        public Heiflow.Models.AI.IComponent Component
        {
            get;
            set;
        }
        [CategoryAttribute("Constants"), DescriptionAttribute("")]
        public int ConstantsIntervalFrom { get; set; }
        [CategoryAttribute("Constants"), DescriptionAttribute("")]
        public int ConstantsIntervalTo { get; set; }
        [CategoryAttribute("Constants"), DescriptionAttribute("")]
        public int ConstantsNumber { get; set; }

        private string mInitialization;
        private string mSelectionMethodName;
        private string mFitnessMethod;

        [CategoryAttribute("Ignore"), Browsable(false)]
        public EInitializationMethod PopulatonInitialization
        {
            get;
            set;
        }

        [CategoryAttribute("Population"), TypeConverter(typeof(GPInitializationConverter)), DescriptionAttribute("")]
        public string Initialization
        {
            get
            {
                return mInitialization;
            }
            set
            {
                mInitialization = value;
                int index = 0;
                foreach (string str in GPInitializationConverter.ValuesCollection)
                {
                    if (str == mInitialization)
                        break;
                    index++;
                }
                PopulatonInitialization = (EInitializationMethod)index;
            }
        }

       [CategoryAttribute("Ignore"), Browsable(false)]
        public ESelectionMethod SelectionMethod
        {
            get;
            set;
        }

        [CategoryAttribute("Population"), TypeConverter(typeof(GPSelectionConverter)), DescriptionAttribute("")]
        public string SelectionMethodName
        {
            get
            {
                return mSelectionMethodName;
            }
            set
            {
                mSelectionMethodName = value;
                int index = 0;
                foreach (string str in GPSelectionConverter.ValuesCollection)
                {
                    if (str == mSelectionMethodName)
                        break;
                    index++;
                }
                SelectionMethod = (ESelectionMethod)index;
            }
        }


       [CategoryAttribute("Ignore"), Browsable(false)]
        public EFitness Fitness
        {
            get;
            set;
        }      

        [CategoryAttribute("Population"), TypeConverter(typeof(GPFitnessConverter)), DescriptionAttribute("")]
        public string FitnessMethod
        {
            get
            {
                return mFitnessMethod;
            }
            set
            {
                mFitnessMethod = value;
                int index = 0;
                foreach (string str in GPFitnessConverter.ValuesCollection)
                {
                    if (str == mFitnessMethod)
                        break;
                    index++;
                }
                Fitness = (EFitness)index;
            }
        }

        [CategoryAttribute("Population"), DescriptionAttribute("50-5000")]
        public int PopulationSize { get; set; }

        [CategoryAttribute("Selection"), DescriptionAttribute("")]
        public float SelectionParam1 { get; set; }
        [CategoryAttribute("Selection"), DescriptionAttribute("")]
        public float SelectionParam2 { get; set; }
        [CategoryAttribute("Selection"), DescriptionAttribute("Elitism:0-PopSize")]
        public int Elitism { get; set; }


        [CategoryAttribute("Maximalna Dubina Drveta"), DescriptionAttribute("3-17")]
        public int MaxInitLevel { get; set; }
        [CategoryAttribute("Maximalna Dubina Drveta"), DescriptionAttribute("3-17")]
        public int MaxCossoverLevel { get; set; }
        [CategoryAttribute("Maximalna Dubina Drveta"), DescriptionAttribute("3-17")]
        public int MaxMutationLevel { get; set; }


        [CategoryAttribute("Probability of gp operations"), DescriptionAttribute("0.0-1.0")]
        public float ProbCrossover { get; set; }
        [CategoryAttribute("Probability of gp operations"), DescriptionAttribute("0.0-1.0")]
        public float ProbMutation { get; set; }
        [CategoryAttribute("Probability of gp operations"), DescriptionAttribute("0.0-0.5")]
        public float ProbReproduction { get; set; }
        [CategoryAttribute("Probability of gp operations"), DescriptionAttribute("0.0-1.0")]
        public float ProbPermutation { get; set; }

        [CategoryAttribute("Evolution"), DescriptionAttribute("")]
        public int EnvolveConditionValue { get; set; }
        [CategoryAttribute("Evolution"), DescriptionAttribute("0:Generation number; 1: Fitness >=")]
        public int EnvolveIndicator { get; set; }


        [CategoryAttribute("Type of procesors"), DescriptionAttribute("")]
        public bool MultipleCore { get; set; }

    }

    public class GPModel:ForecastingModel
    {
        public GPModel(GPModelParameter para):base(para)
        {
            mGPModelParameter = para;

            Orgnization = "HUST WREIS";
            ID = "3CDA24CD-99D6-4ECB-B332-575BC76998D5";
            Name = "Genetic Programming";
            Descriptions = "";
        }
        private GPModelParameter mGPModelParameter;
        private GPPopulation population;
        private GPFunctionSet functionSet;
        private GPTerminalSet terminalSet;
        private GPChromosome GPBestHromosome;
      
        private GPParameters parameters;
        private List<GPFunction> functionSetsList;
        private int EnvolutionStep;

        public int NumberOfVariables { get; set; }
        public int NumberOfSamples{ get; set; }
        public List<GPFunction> FunctionSetsList
        {
            get
            {
                return functionSetsList;
            }
            set
            {
                functionSetsList = value;
               
            }
        }

        public GPChromosome BestHromosome
        {
            get
            {
                return GPBestHromosome;
            }
        }

        public GPFunctionSet FunctionSet
        {
            get
            {
                return functionSet;
            }
        }
  
        public void Initialize()
        {
            //parameters.eselectionMethod= ESelectionMethod.
            if (parameters == null)
                parameters = new GPParameters();

            parameters.einitializationMethod = mGPModelParameter.PopulatonInitialization;

            parameters.eselectionMethod = mGPModelParameter.SelectionMethod;
            parameters.efitnessFunction = mGPModelParameter.Fitness;
            parameters.InitiFitness();

            parameters.SelParam1 = mGPModelParameter.SelectionParam1;
            parameters.SelParam2 = mGPModelParameter.SelectionParam2;
            parameters.maxInitLevel = mGPModelParameter.MaxInitLevel;
            parameters.maxCossoverLevel = mGPModelParameter.MaxCossoverLevel;
            parameters.maxMutationLevel = mGPModelParameter.MaxMutationLevel;
            parameters.elitism = mGPModelParameter.Elitism;

            parameters.probCrossover = mGPModelParameter.ProbCrossover;
            parameters.probMutation = mGPModelParameter.ProbMutation;
            parameters.probPermutation = mGPModelParameter.ProbPermutation;
            parameters.probReproduction = mGPModelParameter.ProbReproduction;
        }

        public override void Train(IForecastingDataSets datasets)
        {
            OnStartRunning(new ComponentRunEventArgs(datasets));
            NumberOfVariables = datasets.InputVectorLength;
            NumberOfSamples = datasets.Length;
            EnvolutionStep = 0;
            if (functionSet == null)
                functionSet = new GPFunctionSet();
            Initialize();
            GenerateFunction();
            double [] gpConstraints= GenerateConstants(mGPModelParameter.ConstantsIntervalFrom,mGPModelParameter.ConstantsIntervalTo,mGPModelParameter.ConstantsNumber);
            GenerateTerminals(datasets, gpConstraints);

            if (population == null)
            {
                EnvolutionStep = 1;
                population = new GPPopulation(mGPModelParameter.PopulationSize, terminalSet, functionSet, parameters, mGPModelParameter.MultipleCore);
            }
            GPBestHromosome = population.bestChromosome;

            while (ProveEnvolution(EnvolutionStep,mGPModelParameter.EnvolveConditionValue,mGPModelParameter.EnvolveIndicator))
            {
                population.StartEvolution();              
                OnRunningEpoch(new ComponentRunEpochEventArgs(EnvolutionStep));
                EnvolutionStep++;
            }

            int indexOutput = terminalSet.NumConstants + terminalSet.NumVariables - 1;
            List<int> lst = new List<int>();
            FunctionTree.ToListExpression(lst, GPBestHromosome.Root);
            double y = 0;
            datasets.ForecastedData = new double[datasets.Length][];
            for (int i = 0; i < terminalSet.RowCount; i++)
            {
                // evalue the function
                y = functionSet.Evaluate(lst, terminalSet, i);

                // check for correct numeric value
                if (double.IsNaN(y) || double.IsInfinity(y))
                    y = 0;
                datasets.ForecastedData[i] = new double[1];
                datasets.ForecastedData[i][0] = y;
            }
            OnFinishRunning(new ComponentRunEventArgs(datasets) { State = functionSet.DecodeExpression(lst, terminalSet) });
        }

        public override double Forecast(double[] inputVector)
        {
            if (GPBestHromosome != null)
            {
                List<int> lst = new List<int>();
                FunctionTree.ToListExpression(lst, GPBestHromosome.Root);
                return functionSet.Evaluate(lst, inputVector);
            }
            else
            {
                return 0;
            }
        }

        private void GenerateFunction()
        {          
            var q = from c in functionSetsList
                    where c.Selected == true
                    select c;
            //Clear old functions
            functionSet.functions.Clear();
            foreach (var op in q)
            {
                for (int i = 0; i < op.Weight; i++)
                    functionSet.functions.Add(op);
            }
        }

        private void GenerateTerminals(IForecastingDataSets datasets, double[] gpConstants)
        {
            if (terminalSet == null)
                terminalSet = new GPTerminalSet();

            if (gpConstants == null)
                gpConstants = GenerateConstants(mGPModelParameter.ConstantsIntervalFrom, mGPModelParameter.ConstantsIntervalTo, mGPModelParameter.ConstantsNumber);

            //Kada znamo broj konstanti i podatke o experimenti sada mozemo popuniti podatke
            terminalSet.NumConstants = mGPModelParameter.ConstantsNumber;
            terminalSet.NumVariables = NumberOfVariables;
            terminalSet.RowCount = NumberOfSamples;

            terminalSet.TrainingData = new double[terminalSet.RowCount][];
            int numOfVariables = terminalSet.NumVariables + terminalSet.NumConstants + 1/*Output Value of experiment*/;
            for (int i = 0; i < terminalSet.RowCount; i++)
            {
                terminalSet.TrainingData[i] = new double[numOfVariables];
                for (int j = 0; j < numOfVariables; j++)
                {
                    if (j < terminalSet.NumVariables)
                        terminalSet.TrainingData[i][j] = datasets.InputData[i][j];
                    else if (j >= terminalSet.NumVariables && j < numOfVariables - 1)
                        terminalSet.TrainingData[i][j] = gpConstants[j - terminalSet.NumVariables];
                    else
                        terminalSet.TrainingData[i][j] = datasets.OutputData[i][0];
                }
            }
            terminalSet.CalculateStat();

            TerminateExperiments();
        }

        private double[] GenerateConstants(int from, int to, int number)
        {
            double[] result = new double[number];

            for (int i = 0; i < number; i++)
            {
                decimal val = (decimal)(GPPopulation.rand.Next(from, to) + GPPopulation.rand.NextDouble());
                result[i] = (double)decimal.Round(val, 5);
            }

            return result;
        }

        private void TerminateExperiments()
        {
            int numVariable = terminalSet.NumVariables;
            int numConst = terminalSet.NumConstants;
            if (functionSet.terminals == null)
                functionSet.terminals = new List<GPTerminal>();
            else
                functionSet.terminals.Clear();

            //Definisanje terminala
            for (int i = 0; i < numVariable; i++)
            {
                //Terminali
                GPTerminal ter = new GPTerminal();
                ter.IsConstant = false;
                ter.Name = "X" + (i + 1).ToString();
                ter.Value = i;
                functionSet.terminals.Add(ter);

            }
            for (int j = 0; j < numConst; j++)
            {
                //Terminali
                GPTerminal ter = new GPTerminal();
                ter.IsConstant = true;
                ter.Name = "R" + (j + 1).ToString();
                ter.Value = j + numVariable;
                functionSet.terminals.Add(ter);
            }
        }

        private bool ProveEnvolution(int envolutionStep, int conditionValue, int envolutionIndicator)
        {
            if (envolutionIndicator == 0)
            {
                return envolutionStep <= conditionValue;
            }
            else if (envolutionIndicator == 1)
            {
                return GPBestHromosome.Fitness <= conditionValue;
            }
            else
                return envolutionStep <= conditionValue;

        }
    }
}
