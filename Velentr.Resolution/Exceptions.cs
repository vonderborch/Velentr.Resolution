using System;

using Velentr.AbstractShapes;

namespace Velentr.Resolution
{
    /// <summary>
    ///     Exception for signalling invalid resolution errors.
    /// </summary>
    [Serializable]
    public class InvalidResolutionException : Exception
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="width">    The width. </param>
        /// <param name="height">   The height. </param>
        public InvalidResolutionException(int width, int height)
            : base($"Invalid resolution selected: {width}x{height}!") { }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="resolution">   The resolution. </param>
        public InvalidResolutionException(Dimensions resolution)
            : base($"Invalid resolution selected: {resolution.Width}x{resolution.Height}!") { }
    }
}