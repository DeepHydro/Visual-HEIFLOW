using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Data.Classification
{
    /// <summary>
    /// IntervalMethods
    /// </summary>
    public enum IntervalMethod
    {
        /// <summary>
        /// The breaks are set to being evenly spaced.
        /// </summary>
        EqualInterval,

        /// <summary>
        /// The breaks are positioned to ensure close to equal quantities
        /// in each break. (each group contains approximately same number of values)
        /// </summary>
        EqualFrequency,

        /// <summary>
        /// Jenks natural breaks looks for "clumping" in the data and
        /// attempts to group according to the clumps.
        /// </summary>
        NaturalBreaks,

        /// <summary>
        /// Breaks start equally placed, but can be positioned manually instead.
        /// </summary>
        Manual,
    }
}
