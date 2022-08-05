namespace Velentr.Resolution
{
    public enum VirtualResolutionMode
    {
        /// <summary>
        ///     Virtual resolution will be set to a fixed value.
        /// </summary>
        Fixed

       ,

        /// <summary>
        ///     Virtual resolution will be set to a value equivalent to the actual resolution multiplied by some value. Only works
        ///     with BoxingMode = None.
        /// </summary>
        ResolutionMultiplied
    }
}
