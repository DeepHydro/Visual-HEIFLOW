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
namespace  Heiflow.AI.GeneticProgramming
{
    
    [Serializable]
    public enum EInitializationMethod
    {
        FullInitialization = 0,
        GrowInitialization=1,
        HalfHalfInitialization=2
    }
    [Serializable]
    public enum ESelectionMethod
    {
       // EliteSelection=0,
        EFitnessProportionateSelection = 0,
        Rankselection=1,
        TournamentSelection=2,
        StochasticUniversalSelection=3,
        FUSSelection=4,
        SkrgicSelection=5
    }
    [Serializable]
    public enum EFitness
    {
        MSE = 0,//Mean square error
        RMSE,//Root mean square error
        MAE,//Mean apsolute error
        RSE,//Root square error
        RRSE,//Relative root square error
        RAE,//Root apsolute error
        rMSE,//relative MSE
        rRMSE,//relative RMSE
        rMAE,//relative MAE
        rRSE,//relative RSE
        rRRSE,//relative RRSE
        rRAE,//relative RAE
        AE,//Apsolute error
        RE, //Relative  error
        CC//Corelation coefficient

    }
    [Serializable]
    public class GPParameters
    {
        //Initialization metods
        public EInitializationMethod einitializationMethod;
        public int maxInitLevel;

       
        //Selection methods
        public ESelectionMethod eselectionMethod;
        public float SelParam1;
        public float SelParam2;
        public int elitism;

        //Fitness function
        public EFitness efitnessFunction;
        //Primary oparation
        public float probCrossover;
        public int maxCossoverLevel;

        //Secondary Operation
        public float probMutation;
        public int maxMutationLevel;
        public float probPermutation;
        //    public float                    probEncaptilation;
        //    public bool                     bEditing;
        //    public bool                     bDecimation;
        public float probReproduction;

        public IFitnessFunction GPFitness;

        public GPParameters()
        {
            einitializationMethod = EInitializationMethod.FullInitialization;
            eselectionMethod = ESelectionMethod.Rankselection;
            efitnessFunction = EFitness.MSE;
            InitiFitness();
            SelParam1 = 3;
            SelParam2 = 0;
            elitism = 1;
            maxInitLevel =7;
            maxCossoverLevel = 10;
            maxMutationLevel = 10;
            probCrossover = 1.0F;
            probMutation = 1.0F;
            probPermutation = 1.0F;
            probReproduction = 0.20F;
        }

        public void InitiFitness()
        {
            switch (efitnessFunction)
            {
                case EFitness.MSE:
                    GPFitness = new MSEFitness();
                    break;
                case EFitness.RMSE:
                    GPFitness = new RMSEFitness();
                    break;
                case EFitness.MAE:
                    GPFitness = new MAEFitness();
                    break;
                case EFitness.RSE:
                    GPFitness = new RSEFitness();
                    break;
                case EFitness.RRSE:
                    GPFitness = new RRSEFitness();
                    break;
                case EFitness.RAE:
                    GPFitness = new RAEFitness();
                    break;
                case EFitness.rMSE:
                    GPFitness = new r_MSEFitness();
                    break;
                case EFitness.rRMSE:
                    GPFitness = new r_RMSEFitness();
                    break;
                case EFitness.rMAE:
                    GPFitness = new r_MAEFitness();
                    break;
                case EFitness.rRSE:
                    GPFitness = new r_MAEFitness();
                    break;
                case EFitness.rRRSE:
                    GPFitness = new r_RRSEFitness();
                    break;
                case EFitness.rRAE:
                    GPFitness = new r_RAEFitness();
                    break;
                case EFitness.AE:
                    GPFitness = new AEFitness();
                    break;
                case EFitness.RE:
                    GPFitness = new REFitness();
                    break;
                case EFitness.CC:
                    GPFitness = new CCFitness();
                    break;
                default:
                    break;
            }
        }
    }
}
