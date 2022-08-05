using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Velentr.AbstractShapes;
using Velentr.Debugging;
using Velentr.Font;
using Velentr.Resolution;

namespace CoreDev
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private FpsTracker _frameCounter = new FpsTracker(10);
        private PerformanceTracker _performance = new PerformanceTracker(10, enableFpsTracker: true);
        private string _baseTitle = "Velentr.Resolution.DevEnv";
        private string _decimals = "0.000";

        private string _fontName = "MontserratRegular-RpK6l.otf";
        private FontManager _fontManager;
        private Font _font;
        private KeyboardState _previousKeyboardState;

        // Stuff we're testing
        private ResolutionManager _viewport;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _baseTitle = $"{_baseTitle} | FPS: {{0:{_decimals}}} | TPS: {{1:{_decimals}}} | CPU: {{2:{_decimals}}}% | Memory: {{3:{_decimals}}} MB";
            _fontManager = new FontManager(GraphicsDevice);
            _font = _fontManager.GetFont(_fontName, 16);
            _previousKeyboardState = Keyboard.GetState();

            _viewport = new ResolutionManager(
                                              Window
                                            , this._graphics
                                            , GraphicsDevice
                                            , new ResolutionSettings(
                                                                     new Dimensions(1280, 800)
                                                                   , new Dimensions(1024, 768)
                                                                   , ScreenMode.Windowed
                                                                   , BoxingMode.Pillarbox
                                                                   , true
                                                                   , false
                                                                    )
                                             );

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _performance.Update(gameTime.ElapsedGameTime);
            var state = Keyboard.GetState();

            // process keyboard commands to perform demo action
            if (IsTriggered(Keys.Escape, state))
            {
                this.Exit();
            }

            if (IsTriggered(Keys.NumPad1, state))
            {
                var sm = this._viewport.ScreenMode;
                switch (this._viewport.ScreenMode)
                {
                    case ScreenMode.Borderless:
                        sm = ScreenMode.BorderlessFullScreen;

                        break;
                    case ScreenMode.BorderlessFullScreen:
                        sm = ScreenMode.Windowed;

                        break;
                    case ScreenMode.Windowed:
                        sm = ScreenMode.FullScreen;

                        break;
                    case ScreenMode.FullScreen:
                        sm = ScreenMode.Borderless;

                        break;
                }

                this._viewport.ScreenMode = sm;
            }
            if (IsTriggered(Keys.NumPad2, state))
            {
                var sm = this._viewport.PreferredBoxingMode;
                switch (this._viewport.PreferredBoxingMode)
                {
                    case BoxingMode.None:
                        sm = BoxingMode.BiggestArea;

                        break;
                    case BoxingMode.BiggestArea:
                        sm = BoxingMode.Pillarbox;

                        break;
                    case BoxingMode.Pillarbox:
                        sm = BoxingMode.Letterbox;

                        break;
                    case BoxingMode.Letterbox:
                        sm = BoxingMode.None;

                        break;
                }

                this._viewport.PreferredBoxingMode = sm;
            }
            if (IsTriggered(Keys.NumPad3, state))
            {
                this._viewport.IsUserResizeable = !this._viewport.IsUserResizeable;
            }
            if (IsTriggered(Keys.NumPad4, state))
            {
                this._viewport.EnforceAspectRatio = !this._viewport.EnforceAspectRatio;
            }
            if (IsTriggered(Keys.NumPad5, state))
            {
                this._viewport.ToggleFullScreen();
            }
            if (IsTriggered(Keys.NumPad6, state))
            {
                this._viewport.ToggleBorderless();
            }




            _previousKeyboardState = state;

            base.Update(gameTime);
        }

        private bool IsTriggered(Keys key, KeyboardState state)
        {
            return state.IsKeyUp(key) && this._previousKeyboardState.IsKeyDown(key);
        }

        private string FormatDimensions(int width, int height)
        {
            return $"{width}x{height} ({Math.Round(_viewport.CurrentScreenWidth / (double)_viewport.CurrentScreenHeight, 2)})";
        }

        protected override void Draw(GameTime gameTime)
        {
            _frameCounter.Update(gameTime.ElapsedGameTime);
            Window.Title = string.Format(_baseTitle, _frameCounter.AverageFramesPerSecond, _performance.FpsTracker.AverageFramesPerSecond, _performance.CpuTracker.CpuPercent, _performance.MemoryTracker.MemoryUsageMb);

            _viewport.ClearScreen();

            _spriteBatch.Begin();

            var text = new List<string>()
                       {
                           $"Current Screen: {FormatDimensions(_viewport.CurrentScreenWidth, this._viewport.CurrentScreenHeight)}"
                         , $"Window: {FormatDimensions(_viewport.ActualWidth, this._viewport.ActualHeight)}, Render: {FormatDimensions(_viewport.RenderActualResolution.Width, this._viewport.RenderActualResolution.Height)}, Virtual: {FormatDimensions(_viewport.VirtualWidth, this._viewport.VirtualHeight)}"
                         , "", $"Settings: [SM - {this._viewport.ScreenMode}], [BM - {this._viewport.PreferredBoxingMode}], [Resizeable - {this._viewport.IsUserResizeable}]", $"[Aspect Ratio Enforced - {this._viewport.EnforceAspectRatio}], [FullScreen - {this._viewport.IsFullscreen}], [Borderless - {this._viewport.IsBorderless}]"
                         , "", "Controls:", "[ESC] - Exit demo", "[1] - Change ScreenMode", "[2] - Change Boxing Mode", "[3] - Change Is User Resizeable", "[4] - Change Enforce Aspect Ratio", "[5] - Toggle Fullscreen", "[6] - Toggle Borderless"
                       };

            var y = -10;
            for (var i = 0; i < text.Count; i++)
            {
                _spriteBatch.DrawString(_font, text[i], new Vector2(10, y += 20), Color.Black);
            }

            _spriteBatch.End();

            _viewport.EndDraw(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}
