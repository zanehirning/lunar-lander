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

namespace LunarLander.Views.Settings
{
    public class SettingsView : GameStateView
    {
        private SpriteFont m_menuFont;
        private SpriteFont m_menuSelectFont;
        
        private Texture2D m_texPlanet;
        private Rectangle m_rectPlanet;
        private SettingsStateEnum m_currentSelection = SettingsStateEnum.Thrust;
        private KeyboardInput m_inputKeyboard;

        public override void loadContent(ContentManager contentManager)
        {
            m_menuFont = contentManager.Load<SpriteFont>("Fonts/anta-regular");
            m_menuSelectFont = contentManager.Load<SpriteFont>("Fonts/anta-regular-select");
            m_texPlanet = contentManager.Load<Texture2D>("Images/Brahe_Sprite");

            m_inputKeyboard = new KeyboardInput();
            m_inputKeyboard.registerCommand(Keys.Down, true, new IInputDevice.CommandDelegate(menuDown));
            m_inputKeyboard.registerCommand(Keys.Up, true, new IInputDevice.CommandDelegate(menuUp));
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            m_inputKeyboard.Update();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
            {
                return GameStateEnum.MainMenu;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Enter))
            {
                switch (m_currentSelection)
                {
                    case SettingsStateEnum.Thrust: 
                        {
                            return GameStateEnum.Settings;
                        };
                    case SettingsStateEnum.RotateLeft: 
                        {
                            return GameStateEnum.Settings;
                        };
                    case SettingsStateEnum.RotateRight: 
                        {
                            return GameStateEnum.Settings;
                        };
                }
            }
            return GameStateEnum.Settings;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            drawPlanet();
            float bottom = drawMenuItem(m_currentSelection == SettingsStateEnum.Thrust ? m_menuSelectFont : m_menuFont, "Thrust: ", m_graphics.PreferredBackBufferHeight / 2, m_currentSelection == SettingsStateEnum.Thrust ? new Color(1, 59, 89) : Color.White);
            bottom = drawMenuItem(m_currentSelection == SettingsStateEnum.RotateLeft ? m_menuSelectFont : m_menuFont, "Rotate Left: ", bottom, m_currentSelection == SettingsStateEnum.RotateLeft ? new Color(1, 59, 89) : Color.White);
            bottom = drawMenuItem(m_currentSelection == SettingsStateEnum.RotateRight ? m_menuSelectFont : m_menuFont, "Rotate Right: ", bottom, m_currentSelection == SettingsStateEnum.RotateRight ? new Color(1, 59, 89) : Color.White);
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
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

        #region Input
        private void menuDown()
        {
            if (m_currentSelection != SettingsStateEnum.RotateRight)
            {
                m_currentSelection = m_currentSelection + 1;
            }
        }
        private void menuUp()
        {
            if (m_currentSelection != SettingsStateEnum.Thrust)
            {
                m_currentSelection = m_currentSelection - 1;
            }
        }
        #endregion
    }
}
