using System;

using Microsoft.Xna.Framework;

using Velentr.AbstractShapes;

namespace Velentr.Resolution
{
    /// <summary>
    ///     Settings for resolution.
    /// </summary>
    public struct ResolutionSettings
    {
        /// <summary>
        ///     (Immutable) the default draw area color.
        /// </summary>
        internal static readonly Color DefaultDrawAreaColor = Color.CornflowerBlue;

        /// <summary>
        ///     (Immutable) the default pillarbox color.
        /// </summary>
        internal static readonly Color DefaultPillarboxColor = Color.Black;

        /// <summary>
        ///     (Immutable) the default width.
        /// </summary>
        internal const int DefaultWidth = 800;

        /// <summary>
        ///     (Immutable) the default height.
        /// </summary>
        internal const int DefaultHeight = 600;

        /// <summary>
        ///     (Immutable) the default resolution.
        /// </summary>
        internal static readonly Dimensions DefaultResolution = new Dimensions(DefaultWidth, DefaultHeight);

        /// <summary>
        ///     (Immutable) the default screen mode.
        /// </summary>
        internal const ScreenMode DefaultScreenMode = ScreenMode.Windowed;

        /// <summary>
        ///     (Immutable) the default boxing mode.
        /// </summary>
        internal const BoxingMode DefaultBoxingMode = BoxingMode.None;

        /// <summary>
        ///     The preferred boxing mode.
        /// </summary>
        public BoxingMode PreferredBoxingMode;

        /// <summary>
        ///     True if is user resizeable, false if not.
        /// </summary>
        public bool IsUserResizeable;

        /// <summary>
        ///     The screen mode.
        /// </summary>
        public ScreenMode ScreenMode;

        /// <summary>
        ///     True to enforce aspect ratio.
        /// </summary>
        public bool EnforceAspectRatio;

        /// <summary>
        ///     The actual resolution.
        /// </summary>
        public Dimensions ActualResolution;

        /// <summary>
        ///     The virtual resolution.
        /// </summary>
        public Dimensions VirtualResolution;

        public ResolutionSettings(
            Dimensions actualResolution
          , Dimensions? virtualResolution
          , ScreenMode? screenMode = null
          , BoxingMode? boxingMode = null
          , bool isUserResizeable = true
          , bool enforceAspectRatio = false
        )
        {
            this.PreferredBoxingMode = boxingMode ?? DefaultBoxingMode;


            this.ActualResolution = actualResolution;
            this.VirtualResolution = virtualResolution ?? DefaultResolution;

            this.ScreenMode = screenMode ?? DefaultScreenMode;

            this.IsUserResizeable = isUserResizeable;
            this.EnforceAspectRatio = enforceAspectRatio;
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="actualResolution">                 The actual resolution. </param>
        /// <param name="virtualResolution">                The virtual resolution. </param>
        /// <param name="screenMode">                       (Optional) The screen mode. </param>
        /// <param name="boxingMode">                       (Optional) The boxing mode. </param>
        /// <param name="isUserResizeable">                 (Optional) The is user resizeable. </param>
        /// <param name="enforceAspectRatio">               (Optional) The enforce aspect ratio. </param>
        public ResolutionSettings(
            Dimensions? actualResolution
          , Dimensions? virtualResolution
          , ScreenMode? screenMode = null
          , BoxingMode? boxingMode = null
          , bool isUserResizeable = true
          , bool enforceAspectRatio = false
        )
            : this(
                   actualResolution ?? DefaultResolution
                 , virtualResolution ?? DefaultResolution
                 , screenMode ?? DefaultScreenMode
                 , boxingMode ?? DefaultBoxingMode
                 , isUserResizeable
                 , enforceAspectRatio
                  ) { }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="actualResolution">                 The actual resolution. </param>
        /// <param name="virtualResolution">                The virtual resolution. </param>
        /// <param name="screenMode">                       (Optional) The screen mode. </param>
        /// <param name="boxingMode">                       (Optional) The boxing mode. </param>
        /// <param name="isUserResizeable">
        ///     (Optional) True if is user resizeable, false
        ///     if not.
        /// </param>
        /// <param name="enforceAspectRatio">               (Optional) True to enforce aspect ratio. </param>
        public ResolutionSettings(
            (int, int)? actualResolution = null
          , (int, int)? virtualResolution = null
          , ScreenMode? screenMode = null
          , BoxingMode? boxingMode = null
          , bool isUserResizeable = true
          , bool enforceAspectRatio = false
        )
            : this(
                   actualResolution == null ? DefaultResolution : new Dimensions(actualResolution.Value.Item1, actualResolution.Value.Item2)
                 , virtualResolution == null ? DefaultResolution : new Dimensions(virtualResolution.Value.Item1, virtualResolution.Value.Item2)
                 , screenMode
                 , boxingMode
                 , isUserResizeable
                 , enforceAspectRatio
                  ) { }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="actualWidth">                      (Optional) Width of the actual. </param>
        /// <param name="actualHeight">                     (Optional) Height of the actual. </param>
        /// <param name="virtualWidth">                     Width of the virtual. </param>
        /// <param name="virtualHeight">                    Height of the virtual. </param>
        /// <param name="screenMode">                       (Optional) The screen mode. </param>
        /// <param name="boxingMode">                       (Optional) The boxing mode. </param>
        /// <param name="isUserResizeable">
        ///     (Optional) True if is user resizeable, false
        ///     if not.
        /// </param>
        /// <param name="enforceAspectRatio">               (Optional) True to enforce aspect ratio. </param>
        public ResolutionSettings(
            int? actualWidth = null
          , int? actualHeight = null
          , int? virtualWidth = null
          , int? virtualHeight = null
          , ScreenMode? screenMode = null
          , BoxingMode? boxingMode = null
          , bool isUserResizeable = true
          , bool enforceAspectRatio = false
        )
            : this(
                   new Dimensions(actualWidth ?? DefaultWidth, actualHeight ?? DefaultHeight)
                 , new Dimensions(virtualWidth ?? DefaultWidth, virtualHeight ?? DefaultHeight)
                 , screenMode
                 , boxingMode
                 , isUserResizeable
                 , enforceAspectRatio
                  ) { }
    }
}
