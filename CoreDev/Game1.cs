using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Velentr.Debugging;

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
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _performance.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _frameCounter.Update(gameTime.ElapsedGameTime);
            Window.Title = string.Format(_baseTitle, _frameCounter.AverageFramesPerSecond, _performance.FpsTracker.AverageFramesPerSecond, _performance.CpuTracker.CpuPercent, _performance.MemoryTracker.MemoryUsageMb);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
