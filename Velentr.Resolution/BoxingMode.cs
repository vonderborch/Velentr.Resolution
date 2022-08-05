namespace Velentr.Resolution
{
    /// <summary>
    /// Values that represent boxing modes.
    /// </summary>
    public enum BoxingMode
    {
        /// <summary>
        /// Switch between Letterbox and Pillarbox depending on what gives us the largest screen size
        /// </summary>
        BiggestArea,

        /// <summary>
        /// Always fill the full available screen with no letterboxing or pillarboxing. Will cause stretching unless other code takes this into account
        /// </summary>
        None,

        /// <summary>
        /// Always try to apply a pillarboxing effect to the screen
        /// </summary>
        Pillarbox,

        /// <summary>
        /// Always try to apply a letterboxing effect to the screen
        /// </summary>
        Letterbox,
    }
}