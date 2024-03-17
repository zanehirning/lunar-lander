using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunarLander.Input;
using LunarLander.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LunarLander.Views.MainMenu
{
    public class MainMenuView : GameStateView
    {
        private SpriteFont m_menuFont;
        private SpriteFont m_menuSelectFont;
        
        private Texture2D m_texPlanet;
        private Rectangle m_rectPlanet;
        private MenuStateEnum m_currentSelection = MenuStateEnum.StartGame;
        private KeyboardInput m_inputKeyboard;

        public override void loadContent(ContentManager contentManager)
        {
            m_texPlanet = contentManager.Load<Texture2D>("Images/Brahe_Sprite");
            m_menuFont = contentManager.Load<SpriteFont>("Fonts/anta-regular");
            m_menuSelectFont = contentManager.Load<SpriteFont>("Fonts/anta-regular-select");

            m_inputKeyboard = new KeyboardInput();
            m_inputKeyboard.registerCommand(Keys.Down, true, new IInputDevice.CommandDelegate(menuDown));
            m_inputKeyboard.registerCommand(Keys.Up, true, new IInputDevice.CommandDelegate(menuUp));
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputKeyboard.Update();
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                switch (m_currentSelection)
                {
                    case MenuStateEnum.StartGame:
                        {
                            m_keybindingsDAO.loadKeybinds();
                            m_highScoresDAO.loadHighScores();
                            return GameStateEnum.Game;
                        }
                    case MenuStateEnum.Credits: return GameStateEnum.Credits;
                    case MenuStateEnum.HighScores:
                        {
                            return GameStateEnum.HighScores;
                        }
                    case MenuStateEnum.Settings: 
                        {
                            m_keybindingsDAO.loadKeybinds();
                            return GameStateEnum.Settings;
                        }
                    case MenuStateEnum.Exit: return GameStateEnum.Exit;
                    default: return GameStateEnum.MainMenu;
                }
            }
            return GameStateEnum.MainMenu;
        }

        public override void update(GameTime gameTime)
        {
            m_highScoresDAO.loadHighScores();
            m_keybindingsDAO.loadKeybinds();
        }

        #region Input
        private void menuDown()
        {
            if (m_currentSelection != MenuStateEnum.Exit)
            {
                m_currentSelection = m_currentSelection + 1;
            }
        }
        private void menuUp()
        {
            if (m_currentSelection != MenuStateEnum.StartGame)
            {
                m_currentSelection = m_currentSelection - 1;
            }
        }
        #endregion

        #region Drawing
        public override void render(GameTime gameTime)
        {
            m_graphics.GraphicsDevice.Clear(Color.Black);
            m_spriteBatch.Begin();
            drawPlanet();
            float bottom = drawMenuItem(m_currentSelection == MenuStateEnum.StartGame ? m_menuSelectFont : m_menuFont, "Start Game", m_graphics.PreferredBackBufferHeight / 2, m_currentSelection == MenuStateEnum.StartGame ? new Color(1, 59, 89) : Color.White);
            bottom = drawMenuItem(m_currentSelection == MenuStateEnum.HighScores ? m_menuSelectFont : m_menuFont, "High Scores", bottom, m_currentSelection == MenuStateEnum.HighScores ? new Color(1, 59, 89) : Color.White);
            bottom = drawMenuItem(m_currentSelection == MenuStateEnum.Credits ? m_menuSelectFont : m_menuFont, "Credits", bottom, m_currentSelection == MenuStateEnum.Credits ? new Color(1, 59, 89) : Color.White);
            bottom = drawMenuItem(m_currentSelection == MenuStateEnum.Settings ? m_menuSelectFont : m_menuFont, "Settings", bottom, m_currentSelection == MenuStateEnum.Settings ? new Color(1, 59, 89) : Color.White);
            drawMenuItem(m_currentSelection == MenuStateEnum.Exit ? m_menuSelectFont : m_menuFont, "Exit", bottom, m_currentSelection == MenuStateEnum.Exit ? new Color(1, 59, 89) : Color.White);            
            m_spriteBatch.End();
        }

        private float drawMenuItem(SpriteFont font, string text, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, y),
                color
            );
            return y + stringSize.Y;
        }

        private void drawPlanet()
        {
            int planetSize = m_graphics.PreferredBackBufferHeight / 3; 
            m_rectPlanet = new Rectangle((m_graphics.PreferredBackBufferWidth / 2) - (planetSize / 2), (m_graphics.PreferredBackBufferHeight / 2) - planetSize - 20, planetSize, planetSize);

            m_spriteBatch.Draw(
                    m_texPlanet,
                    m_rectPlanet,
                    Color.White
                );
        }
        #endregion
    }
}
