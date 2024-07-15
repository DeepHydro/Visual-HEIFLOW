using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Data.Classification
{
    public enum IntervalSnapMethod
    {
        /// <summary>
        /// Snap the chosen values to the nearest data value.
        /// </summary>
        DataValue,
        /// <summary>
        /// No snapping at all is used
        /// </summary>
        None,
        /// <summary>
        /// Snaps to the nearest integer value.
        /// </summary>
        Rounding,
        /// <summary>
        /// Disregards scale, and preserves a fixed number of figures.
        /// </summary>
        SignificantFigures,
    }
}
