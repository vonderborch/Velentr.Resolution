namespace Velentr.Resolution
{
    /// <summary>
    ///     Values that represent screen modes.
    /// </summary>
    public enum ScreenMode
    {
        /// <summary>
        /// The window will be windowed mode
        /// </summary>
        Windowed,

        /// <summary>
        /// The window will be in full-screen mode
        /// </summary>
        FullScreen,

        /// <summary>
        /// The window will be borderless, but not necessarily full screen (not recommended in general)
        /// </summary>
        Borderless,

        /// <summary>
        /// The window will be borderless and full screen
        /// </summary>
        BorderlessFullScreen
    }
}