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

namespace  Heiflow.AI.GeneticProgramming
{
    [Serializable]
    public class GPChromosome : IComparable, IEquatable<GPChromosome>
    {
        // tree root
        public FunctionTree Root = new FunctionTree();

        // chromosome's fitness. Fitness is always float value. No meter which of type chromosem is.
        private float fitness = 0;
        //Used with Equatable to increase diversity in population
        private float _diversity = 0.001f;
        /// <summary>
        /// Fitness property
        /// </summary>
        public float Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public GPChromosome()
        { }
        /// <summary>
        /// Clone the chromosome
        /// </summary>
        public GPChromosome Clone()
        {
            return new GPChromosome(this);
        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        public GPChromosome(GPChromosome source)
        {
            //Helper for cloning
            Root = (FunctionTree)source.Root.Clone();
            fitness = source.Fitness;
        }
        /// <summary>
        /// Get string representation of the chromosome. Return the chromosome
        /// in reverse polish notation (postfix notation).
        /// </summary>
        public override string ToString()
        {
            return Root.ToString();
        }
        /// <summary>
        /// Compare two chromosomes
        /// </summary>
        #region IComparable Members

        public int CompareTo(object obj)
        {
            GPChromosome o = (GPChromosome)obj;
            return (Fitness == o.Fitness) ? 0 : (Fitness < o.Fitness) ? 1 : -1;
        }

        #endregion
        #region IEquatable<GPChromosome> Members

        public bool Equals(GPChromosome other)
        {
            if (other == null) return false;
            float f1 = fitness + _diversity;
            float f2 = fitness - _diversity;
            return (other.Fitness <= f1 && other.Fitness > f2);

        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
