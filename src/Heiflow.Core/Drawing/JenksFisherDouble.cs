using System;
using System.Collections.Generic;
using System.Linq;


namespace Heiflow.Core.Drawing
{

    /// <summary>
    /// Port of Jenks/Fisher breaks originally created in C by Maarten Hilferink.
    /// <remarks>
    ///     http://wiki.objectvision.nl/index.php/CalcNaturalBreaksCode
    ///     http://wiki.objectvision.nl/index.php/Fisher%27s_Natural_Breaks_Classification
    ///     https://github.com/pschoepf/naturalbreaks
    /// </remarks>
    /// </summary>
    public class JenksFisherDouble
    {
        /// <summary>
        /// Replacing ValueTuple by class,
        /// intellisence didn't work in sqlproj with C# 7.0
        /// </summary>
        private class ValueCountTuple : IComparable, IComparable<ValueCountTuple>, IEquatable<ValueCountTuple>
        {
            public double Value;
            public int Count;

            public ValueCountTuple(double value, int count)
            {
                Value = value;
                Count = count;
            }

            public int CompareTo(object obj)
            {
                if (ReferenceEquals(this, obj)) return 0;
                if (ReferenceEquals(null, obj)) return 1;

                return CompareTo(obj as ValueCountTuple);
            }

            public int CompareTo(ValueCountTuple other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;

                var result = Value.CompareTo(other.Value);
                if (result != 0)
                    return result;
                return Count.CompareTo(other.Count);
            }

            public override int GetHashCode()
            {

                unchecked
                {
                    var hashCode = 1519435568;
                    hashCode = (hashCode * 397) ^ Value.GetHashCode();
                    hashCode = (hashCode * 397) ^ Count.GetHashCode();
                    return hashCode;
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj)) return true;
                if (ReferenceEquals(null, obj)) return false;

                return Equals(obj as ValueCountTuple);
            }

            public bool Equals(ValueCountTuple other)
            {
                if (ReferenceEquals(this, other)) return true;
                if (ReferenceEquals(null, other)) return false;

                return Value == other.Value && Count == other.Count;
            }

            public override string ToString()
            {
                return Value.ToString() + " [" + Count.ToString() + "]";
            }


        }

        private List<ValueCountTuple> _values;
        private int _numValues;
        private int _numBreaks;
        private int _bufferSize;
        private List<double> _previousSSM;
        private List<double> _currentSSM;
        private int[] _classBreaks;
        private int _classBreaksIndex;
        private int _completedRows;

        /// <summary>
        /// Main entry point for creation of Jenks-Fisher natural breaks.
        /// </summary>
        /// <param name="values">values array of the values, do not need to be sorted.</param>
        /// <param name="numBreaks">number of breaks to create</param>
        /// <returns>Array with breaks</returns>
        public static List<double> CreateJenksFisherBreaksArray(List<double> values, int numBreaks)
        {
            var tuples = BuildValueCountTuples(values);
            var breaks = (tuples.Count > numBreaks) ? ClassifyByJenksFisher(numBreaks, tuples) : tuples.Select(x => x.Value).ToList();
            return breaks;
        }

        /// <summary>
        /// Constructor that initializes main variables used in fisher calculation of natural breaks
        /// </summary>
        /// <param name="tuples">
        ///     Ordered list of pairs of values to occurrence counts.
        ///     The value sequence must be strictly increasing, all weights must be positive
        /// </param>
        /// <param name="numBreaks"> Number of breaks to find.</param>
        private JenksFisherDouble(List<ValueCountTuple> tuples, int numBreaks)
        {
            _values = new List<ValueCountTuple>();
            _numValues = tuples.Count;
            _numBreaks = numBreaks;
            _bufferSize = tuples.Count - (_numBreaks - 1);
            _previousSSM = new List<double>(_bufferSize);
            _currentSSM = new List<double>(_bufferSize);
            _classBreaks = new int[_bufferSize * (_numBreaks - 1)];
            double cwv = 0.0;
            int cw = 0, w = 0;

            // avoid array <-> list conversations in future
            _previousSSM.AddRange(Enumerable.Repeat(0.0d, _bufferSize));
            _currentSSM.AddRange(Enumerable.Repeat(0.0d, _bufferSize));
            ValueCountTuple currPair;

            for (int i = 0; i != this._numValues; ++i)
            {
                currPair = tuples[i];
                w = currPair.Count;
                cw += w;
                cwv += w * currPair.Value;
                _values.Add(new ValueCountTuple(cwv, cw));
                if (i < _bufferSize)
                {
                    // prepare sum of squared means for first class. Last (k-1) values are omitted
                    _previousSSM[i] = cwv * cwv / cw;
                }
            }

        }

        /// <summary>
        /// Gets sum of weighs for elements with index b..e.
        /// </summary>
        /// <param name="beginIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private int SumOfWeights(int beginIndex, int endIndex)
        {
            int res = _values[endIndex].Count;
            res -= _values[beginIndex - 1].Count;
            return res;
        }

        /// <summary>
        /// Gets sum of weighed values for elements with index b..e
        /// </summary>
        /// <param name="beginIndex">index of begin element</param>
        /// <param name="endIndex">index of end element</param>
        /// <returns>cumul. sum of the values*weight</returns>
        private double SumOfWeightedValues(int beginIndex, int endIndex)
        {

            double res = _values[endIndex].Value;
            res -= _values[beginIndex - 1].Value;
            return res;
        }

        /// <summary>
        /// Gets the Squared Mean for elements within index b..e, multiplied by weight. Note that n*mean^2 = sum^2/n when mean := sum/n
        /// </summary>
        /// <param name="beginIndex">index of begin element</param>
        /// <param name="endIndex">index of end element</param>
        /// <returns>the sum of squared mean</returns>
        private double SSM(int beginIndex, int endIndex)
        {
            double res = SumOfWeightedValues(beginIndex, endIndex);
            return res * res / SumOfWeights(beginIndex, endIndex);
        }

        /// <summary>
        /// Finds CB[i+completedRows] given that the result is at least bp+(completedRows-1) and less than ep+(completedRows-1)
        /// </summary>
        /// <param name="i"></param>
        /// <param name="bp"></param>
        /// <param name="ep"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Complexity: O(ep-bp) ~ O(m)
        /// </remarks>
        private int FindMaxBreakIndex(int i, int bp, int ep)
        {
            double minSSM = _previousSSM[bp] + SSM(bp + _completedRows, i + _completedRows);
            int foundP = bp;
            while (++bp < ep)
            {
                double currSSM = _previousSSM[bp] + SSM(bp + _completedRows, i + _completedRows);
                if (currSSM > minSSM)
                {
                    minSSM = currSSM;
                    foundP = bp;
                }
            }
            _currentSSM[i] = minSSM;
            return foundP;
        }

        /// <summary>
        /// Find CB[i+completedRows] for all <c>i >= bi and i < ei</c> given that the
        /// results are at least bp+(completedRows-1) and less than ep+(completedRows-1)
        /// </summary>
        /// <param name="bi"></param>
        /// <param name="ei"></param>
        /// <param name="bp"></param>
        /// <param name="ep"></param>
        /// <remarks>
        /// Complexity: O(log(ei-bi)*Max((ei-bi),(ep-bp))) ~ O(m*log(m))
        /// </remarks>
        private void CalculateRange(int bi, int ei, int bp, int ep)
        {
            if (bi == ei)
                return;

            int mi = (int)Math.Floor((bi + ei) * 0.5);
            int mp = FindMaxBreakIndex(mi, bp, Math.Min(ep, mi + 1));

            // solve first half of the sub-problems with lower 'half' of possible outcomes
            CalculateRange(bi, mi, bp, Math.Min(mi, mp + 1));

            // store result for the middle element.
            _classBreaks[_classBreaksIndex + mi] = mp;

            // solve second half of the sub-problems with upper 'half' of possible outcomes
            CalculateRange(mi + 1, ei, mp, ep);
        }

        /// <summary>
        /// Starting point of calculation of breaks.
        /// Complexity: O(n*log(n)*numBreaks)
        /// </summary>
        private void CalculateAll()
        {
            if (_numBreaks >= 2)
            {
                _classBreaksIndex = 0;
                for (_completedRows = 1; _completedRows < _numBreaks - 1; ++_completedRows)
                {
                    // complexity: O(n*log(n))
                    CalculateRange(0, _bufferSize, 0, _bufferSize);
                    // swap ssm lists
                    var temp = _previousSSM;
                    _previousSSM = _currentSSM;
                    _currentSSM = temp;
                    _classBreaksIndex += _bufferSize;
                }
            }
        }

        /// <summary>
        /// Does the internal processing to actually create the breaks.
        /// </summary>
        /// <param name="numBreaks">number of breaks</param>
        /// <param name="tuples">asc ordered input of values and their occurence counts</param>
        /// <returns>created breaks</returns>
        private static List<double> ClassifyByJenksFisher(int numBreaks, List<ValueCountTuple> tuples)
        {
            var breaksArray = new List<double>(numBreaks);
            if (numBreaks == 0)
                return breaksArray;
            // avoid array <-> list conversations
            breaksArray.AddRange(Enumerable.Repeat(0.0d, numBreaks));

            var classificator = new JenksFisherDouble(tuples, numBreaks);
            if (numBreaks > 1)
            {
                // runs the actual calculation
                classificator.CalculateAll();
                int lastClassBreakIndex = classificator.FindMaxBreakIndex(classificator._bufferSize - 1, 0, classificator._bufferSize);
                while (--numBreaks != 0)
                {
                    // assign the break values to the result
                    breaksArray[numBreaks] = tuples[lastClassBreakIndex + numBreaks].Value;

                    if (numBreaks > 1)
                    {
                        classificator._classBreaksIndex -= classificator._bufferSize;
                        lastClassBreakIndex = classificator._classBreaks[classificator._classBreaksIndex + lastClassBreakIndex];
                    }
                }

            }

            breaksArray[0] = tuples[0].Value; // break for the first class is the minimum of the dataset.
            return breaksArray;
        }

        /// <summary>
        /// Calculates the occurence count of given values and returns them in sorted list.
        /// </summary>
        private static List<ValueCountTuple> BuildValueCountTuples(List<double> values)
        {
            var valuesDict = new Dictionary<double, ValueCountTuple>();
            ValueCountTuple tuple = null;

            foreach (var value in values)
            {
                if (valuesDict.TryGetValue(value, out tuple))
                    tuple.Count++;
                else
                    valuesDict.Add(value, new ValueCountTuple(value, 1));
            }
            var result = valuesDict.Values.ToList();
            result.Sort();
            return result;
        }

    }
}
