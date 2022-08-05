using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Velentr.AbstractShapes;
using Velentr.Scaling;

using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Velentr.Resolution
{
    public class ResolutionManager : Scalar, IDisposable
    {
        private const double BoxingDimensionModifier = 0.5d;

        private static GraphicsDeviceManager _deviceManager;
        private static GameWindow _window;
        private static GraphicsDevice _graphicsDevice;
        private bool _isMatrixDirty = true;

        private Dimensions _previousResolution;

        private Rectangle _renderTargetDrawBounds;
        private RenderTarget2D _renderTarget;
        private DepthFormat _renderTargetDepthFormat = DepthFormat.None;
        private bool _renderTargetMipMap;
        private int _renderTargetPreferredMultiSampleCount;
        private SurfaceFormat _renderTargetPreferredSurfaceFormat = SurfaceFormat.Color;
        private RenderTargetUsage _renderTargetUsage = RenderTargetUsage.DiscardContents;
        private Matrix _scaleMatrix;
        private bool _isChangingResolutions = false;

        private ResolutionSettings _settings;

        public ResolutionManager(GameWindow window, GraphicsDeviceManager deviceManager, GraphicsDevice graphicsDevice, ResolutionSettings settings)
            : base(
                   0
                 , 0
                 , settings.ActualResolution.Width
                 , settings.ActualResolution.Height
                 , null
                 , settings.VirtualResolution.Width
                 , settings.VirtualResolution.Height
                  )
        {
            _window = window;
            _deviceManager = deviceManager;
            _graphicsDevice = graphicsDevice;
            this._settings = settings;
            _window.ClientSizeChanged += UpdateWindowSize;
            ApplyResolutionChanges();
        }

        public BoxingMode PreferredBoxingMode
        {
            get => this._settings.PreferredBoxingMode;

            set
            {
                this._settings.PreferredBoxingMode = value;
                ApplyResolutionChanges();
            }
        }

        public bool IsUserResizeable
        {
            get => this._settings.IsUserResizeable;

            set
            {
                this._settings.IsUserResizeable = value;
                ApplyResolutionChanges();
            }
        }

        public ScreenMode ScreenMode
        {
            get => this._settings.ScreenMode;

            set
            {
                this._settings.ScreenMode = value;
                ApplyResolutionChanges();
            }
        }

        public int ActualWidth
        {
            get => this.Dimensions.Width;

            set
            {
                this._previousResolution = this.Dimensions;
                this.Dimensions = new Dimensions(value, this.ActualHeight);
                ApplyResolutionChanges();
            }
        }

        public int ActualHeight
        {
            get => this.Dimensions.Height;

            set
            {
                this._previousResolution = this.Dimensions;
                this.Dimensions = new Dimensions(this.ActualWidth, value);
                ApplyResolutionChanges();
            }
        }

        public bool EnforceAspectRatio
        {
            get => this._settings.EnforceAspectRatio;

            set
            {
                this._settings.EnforceAspectRatio = value;
                ApplyResolutionChanges();
            }
        }

        public BoxingMode CurrentBoxingMode { get; private set; }

        public Point RenderPosition { get; private set; }

        public Dimensions RenderActualResolution { get; private set; }

        public GraphicsAdapter CurrentScreen => _graphicsDevice.Adapter;

        public int CurrentScreenWidth => this.CurrentScreen.CurrentDisplayMode.Width;

        public int CurrentScreenHeight => this.CurrentScreen.CurrentDisplayMode.Height;

        public List<DisplayMode> SupportedDisplayModes => this.CurrentScreen.SupportedDisplayModes.ToList();

        public List<Dimensions> SupportedResolutions => this.CurrentScreen.SupportedDisplayModes.Select(x => new Dimensions(x.Width, x.Height)).ToList();

        public bool RenderTargetMipMap
        {
            get => this._renderTargetMipMap;

            set
            {
                this._renderTargetMipMap = value;
                this._isMatrixDirty = true;
            }
        }

        public SurfaceFormat RenderTargetSurfaceFormat
        {
            get => this._renderTargetPreferredSurfaceFormat;

            set
            {
                this._renderTargetPreferredSurfaceFormat = value;
                this._isMatrixDirty = true;
            }
        }

        public DepthFormat RenderTargetDepthFormat
        {
            get => this._renderTargetDepthFormat;

            set
            {
                this._renderTargetDepthFormat = value;
                this._isMatrixDirty = true;
            }
        }

        public int RenderTargetMultiSampleCount
        {
            get => this._renderTargetPreferredMultiSampleCount;

            set
            {
                this._renderTargetPreferredMultiSampleCount = value;
                this._isMatrixDirty = true;
            }
        }

        public RenderTargetUsage RenderTargetUsage
        {
            get => this._renderTargetUsage;

            set
            {
                this._renderTargetUsage = value;
                this._isMatrixDirty = true;
            }
        }

        public int VirtualWidth
        {
            get => this.VirtualDimensions.Width;

            set
            {
                this.VirtualDimensions = new Dimensions(value, this.VirtualHeight);
                this._isMatrixDirty = true;
            }
        }

        public int VirtualHeight
        {
            get => this.VirtualDimensions.Height;

            set
            {
                this.VirtualDimensions = new Dimensions(this.VirtualWidth, value);
                this._isMatrixDirty = true;
            }
        }

        public Matrix ScaleMatrix
        {
            get
            {
                if (this._isMatrixDirty)
                {
                    this._scaleMatrix = Matrix.CreateScale((float) this.InternalScale.X, (float) this.InternalScale.Y, 1f);
                    this._isMatrixDirty = false;
                }

                return this._scaleMatrix;
            }
        }

        public double WidthScale => this.ActualWidth / (double) this.VirtualWidth;

        public double HeightScale => this.ActualHeight / (double) this.VirtualHeight;

        public Scale Scale => new Scale(WidthScale, HeightScale);

        public bool IsFullscreen => ScreenMode == ScreenMode.BorderlessFullScreen || ScreenMode == ScreenMode.FullScreen;

        public bool IsBorderless => ScreenMode == ScreenMode.Borderless || ScreenMode == ScreenMode.BorderlessFullScreen;
         
        public double VirtualAspectRatio => this.VirtualWidth / (double) this.VirtualHeight;

        public double ActualAspectRatio => this.ActualWidth / (double) this.ActualHeight;

        public void Dispose()
        {
            this._renderTarget?.Dispose();
        }

        private void UpdateWindowSize(object sender, EventArgs e)
        {
            this._previousResolution = this.Dimensions;
            this.Dimensions = new Dimensions(_window.ClientBounds.Width, _window.ClientBounds.Height);
            ApplyResolutionChanges();
        }

        public RenderTarget2D ClearScreen(Color? drawAreaColor = null)
        {
            if (this._isMatrixDirty)
            {
                GenerateRenderTarget();
            }

            _graphicsDevice.SetRenderTarget(this._renderTarget);
            _graphicsDevice.Clear(drawAreaColor ?? ResolutionSettings.DefaultDrawAreaColor);

            return this._renderTarget;
        }

        public void EndDraw(SpriteBatch spriteBatch, Color? pillarboxColor = null)
        {
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(pillarboxColor ?? ResolutionSettings.DefaultPillarboxColor);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(this._renderTarget, this._renderTargetDrawBounds, Color.White);
            spriteBatch.End();
        }

        public void ToggleUserResizeable()
        {
            this._settings.IsUserResizeable = !this._settings.IsUserResizeable;
            ApplyResolutionChanges();
        }

        public void ToggleFullScreen()
        {
            switch (this.ScreenMode)
            {
                case ScreenMode.Borderless:
                    this.ScreenMode = ScreenMode.BorderlessFullScreen;

                    break;

                case ScreenMode.Windowed:
                    this.ScreenMode = ScreenMode.FullScreen;

                    break;

                case ScreenMode.FullScreen:
                    this.ScreenMode = ScreenMode.Windowed;

                    break;

                case ScreenMode.BorderlessFullScreen:
                    this.ScreenMode = ScreenMode.Borderless;

                    break;
            }

            ApplyResolutionChanges();
        }

        public void ToggleBorderless()
        {
            switch (this.ScreenMode)
            {
                case ScreenMode.Borderless:
                    this.ScreenMode = ScreenMode.Windowed;

                    break;

                case ScreenMode.Windowed:
                    this.ScreenMode = ScreenMode.Borderless;

                    break;

                case ScreenMode.FullScreen:
                    this.ScreenMode = ScreenMode.BorderlessFullScreen;

                    break;

                case ScreenMode.BorderlessFullScreen:
                    this.ScreenMode = ScreenMode.FullScreen;

                    break;
            }

            ApplyResolutionChanges();
        }

        private void GenerateRenderTarget()
        {
            var width = 0;
            var height = 0;
            var scale = Scale;
            double modifier;
            switch (PreferredBoxingMode)
            {
                case BoxingMode.None:
                    width = this.Dimensions.Width;
                    height = this.Dimensions.Height;
                    CurrentBoxingMode = BoxingMode.None;
                    break;
                case BoxingMode.Letterbox:
                    modifier = scale.X;
                    width = (int)(modifier * this.VirtualWidth + BoxingDimensionModifier);
                    height = (int)(modifier * this.VirtualHeight + BoxingDimensionModifier);
                    CurrentBoxingMode = BoxingMode.Letterbox;

                    break;
                case BoxingMode.Pillarbox:
                    modifier = scale.Y;
                    width = (int)(modifier * this.VirtualWidth + BoxingDimensionModifier);
                    height = (int)(modifier * this.VirtualHeight + BoxingDimensionModifier);
                    CurrentBoxingMode = BoxingMode.Pillarbox;
                    break;
                case BoxingMode.BiggestArea:
                    modifier = MathHelper.Min((float)scale.X, (float)scale.Y);
                    width = (int)(modifier * this.VirtualWidth + BoxingDimensionModifier);
                    height = (int)(modifier * this.VirtualHeight + BoxingDimensionModifier);

                    CurrentBoxingMode = height >= this.Dimensions.Height && width < this.Dimensions.Width
                                            ? BoxingMode.Pillarbox
                                            : width >= this.Dimensions.Height && height <= this.Dimensions.Height
                                                ? BoxingMode.Letterbox
                                                : BoxingMode.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(PreferredBoxingMode));
            }

            var x = this.Dimensions.Width / 2 - width / 2;
            var y = this.Dimensions.Height / 2 - height / 2;

            RenderPosition = new Point(x, y);
            RenderActualResolution = new Dimensions(width, height);
            this._isMatrixDirty = true;
            _ = ScaleMatrix;
            _renderTarget = new RenderTarget2D(_graphicsDevice, VirtualWidth, VirtualHeight, _renderTargetMipMap, _renderTargetPreferredSurfaceFormat, _renderTargetDepthFormat, _renderTargetPreferredMultiSampleCount, _renderTargetUsage);
            this._renderTargetDrawBounds = new Rectangle(x, y, width, height);
        }

        private void ApplyResolutionChanges()
        {
            if (!this._isChangingResolutions)
            {
                _isChangingResolutions = true;
                // If we're in borderless full screen mode, make the resolution match the current screen size
                if (ScreenMode == ScreenMode.BorderlessFullScreen)
                {
                    this.Dimensions = new Dimensions(CurrentScreenWidth, CurrentScreenHeight);
                }

                // If we're enforcing a particular aspect ratio, make sure that the selected resolution the aspect ratio we're looking for
                if (this._settings.EnforceAspectRatio)
                {
                    var dimensions = this.Dimensions;
                    if (dimensions.Width != _previousResolution.Width && dimensions.Height != _previousResolution.Height)
                    {
                        dimensions.Height = (int)Math.Ceiling(dimensions.Width / VirtualAspectRatio);
                    }
                    else if (dimensions.Width != _previousResolution.Width)
                    {
                        // TODO -  this aspect ratio setting or remove
                    }
                    else if (dimensions.Height != _previousResolution.Height)
                    {
                        dimensions.Height = (int)Math.Ceiling(dimensions.Width / VirtualAspectRatio);
                    }
                    this.Dimensions = dimensions;
                }

                // First, check if we're dealing with a valid resolution...
                if ((ScreenMode == ScreenMode.Windowed || ScreenMode == ScreenMode.Borderless) && (this.Dimensions.Height > CurrentScreenWidth || this.Dimensions.Height > CurrentScreenHeight))
                {
                    throw new InvalidResolutionException(this.Dimensions);
                }
                else if ((ScreenMode == ScreenMode.FullScreen || ScreenMode == ScreenMode.BorderlessFullScreen) && !SupportedResolutions.Exists(x => x.Width == this.Dimensions.Width && x.Height == this.Dimensions.Height))
                {
                    throw new InvalidResolutionException(this.Dimensions);
                }
                else if (
                    (PreferredBoxingMode == BoxingMode.Pillarbox && VirtualAspectRatio > this.Dimensions.Width / (double)this.Dimensions.Height) ||
                    (PreferredBoxingMode == BoxingMode.Letterbox && VirtualAspectRatio < this.Dimensions.Width / (double)this.Dimensions.Height)
                )
                {
                    this.Dimensions = new Dimensions(this.Dimensions.Width, (int)Math.Ceiling(this.Dimensions.Width / VirtualAspectRatio));
                }

                _deviceManager.PreferredBackBufferWidth = Dimensions.Width;
                _deviceManager.PreferredBackBufferHeight = Dimensions.Height;
                switch (ScreenMode)
                {
                    case ScreenMode.BorderlessFullScreen:
                        _deviceManager.IsFullScreen = true;
                        _deviceManager.ApplyChanges();
                        InternalSetBorderless(true);
                        break;
                    case ScreenMode.Borderless:
                        _deviceManager.IsFullScreen = false;
                        _deviceManager.ApplyChanges();
                        InternalSetBorderless(true);
                        break;
                    case ScreenMode.FullScreen:
                        _deviceManager.IsFullScreen = true;
                        _deviceManager.ApplyChanges();
                        InternalSetBorderless(false);
                        break;
                    case ScreenMode.Windowed:
                        _deviceManager.IsFullScreen = false;
                        _deviceManager.ApplyChanges();
                        InternalSetBorderless(false);
                        break;
                }

                _window.AllowUserResizing = this._settings.IsUserResizeable;
                _deviceManager.ApplyChanges();
                GenerateRenderTarget();
                _isChangingResolutions = false;
            }
        }

        private void InternalSetBorderless(bool isBorderless)
        {
#if FNA
            _window.IsBorderlessEXT = isBorderless;
#elif MONOGAME
            _window.IsBorderless = isBorderless;
#endif
        }
    }
}
