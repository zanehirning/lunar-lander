using System.Collections.Generic;
using LunarLander.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LunarLander.Views;

namespace LunarLander
{
    public class LunarLander : Game
    {
        private GraphicsDeviceManager m_graphics;
        private IGameState m_currentState;
        private Dictionary<GameStateEnum, IGameState> m_states;
        private Texture2D m_texBackground;
        private Rectangle m_rectBackground;
        private SpriteBatch m_spriteBatch;

        public LunarLander()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            m_graphics.PreferredBackBufferWidth = 1920;
            m_graphics.PreferredBackBufferHeight = 1080;
            m_graphics.ApplyChanges();

            m_states = new Dictionary<GameStateEnum, IGameState>
            {
                { GameStateEnum.MainMenu, new Views.MainMenu.MainMenuView() },
                { GameStateEnum.Credits, new Views.Credits.CreditsView() },
                { GameStateEnum.HighScores, new Views.HighScores.HighScoresView()  },
                { GameStateEnum.Settings,  new Views.Settings.SettingsView() },
                { GameStateEnum.Game, new Views.Game.GameView() },
            };

            foreach (IGameState gameState in m_states.Values)
            {
                gameState.initialize(this.GraphicsDevice, m_graphics);
            }

            m_currentState = m_states[GameStateEnum.MainMenu];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (var item in m_states)
            {
                item.Value.loadContent(this.Content);
            }
            m_texBackground = this.Content.Load<Texture2D>("Images/dark-space-background");
            m_rectBackground = new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            GameStateEnum nextStateEnum = m_currentState.processInput(gameTime);

            // Special case for exiting the game
            if (nextStateEnum == GameStateEnum.Exit)
            {
                Exit();
            }
            else
            {
                m_currentState.update(gameTime);
                m_currentState = m_states[nextStateEnum];
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            // Draw the background
            m_spriteBatch = new SpriteBatch(m_graphics.GraphicsDevice);
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_texBackground, m_rectBackground, Color.White);
            m_spriteBatch.End();


            m_currentState.render(gameTime);

            base.Draw(gameTime);
        }
    }
}
